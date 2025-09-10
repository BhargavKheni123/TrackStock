using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{
    public enum MaintenanceStatus
    {
        Open = 1,
        Start = 2,
        Close = 3
    }

    public enum MaintenanceType
    {
        Scheduled = 1,
        UnScheduled = 2,
        Past = 3,
    }
    public enum MaintenanceScheduleFor
    {
        Asset = 1,
        Tool = 2
    }

    public enum MaintenanceScheduleType
    {
        TimeBase = 1,
        OperationalHours = 2,
        Mileage = 3,
        CheckOuts = 4,
        None = 0
    }
    public enum ScheduleMappingType
    {
        Scheduled = 1,
        UnScheduled = 2,
    }



    public class ToolsMaintenanceDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        //MaintenanceName

        [Display(Name = "MaintenanceName", ResourceType = typeof(ResToolsMaintenance))]
        [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String MaintenanceName { get; set; }

        //MaintenanceDate
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "MaintenanceDate", ResourceType = typeof(ResToolsMaintenance))]
        [DisplayFormat(ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> MaintenanceDate { get; set; }

        private string _MaintenanceDate;
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "MaintenanceDateStr", ResourceType = typeof(ResToolsMaintenance))]
        public string MaintenanceDateStr
        {
            get
            {
                if (string.IsNullOrEmpty(_MaintenanceDate))
                {
                    _MaintenanceDate = FnCommon.ConvertDateByTimeZone(MaintenanceDate, true, true);
                }
                return _MaintenanceDate;
            }
            set { this._MaintenanceDate = value; }
        }


        [Display(Name = "EntryDate", ResourceType = typeof(ResToolsMaintenance))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string EntryDate { get; set; }

        //SchedulerGUID
        [Display(Name = "SchedulerGUID", ResourceType = typeof(ResToolsMaintenance))]
        public Nullable<Guid> SchedulerGUID { get; set; }

        [Display(Name = "ScheduleDate", ResourceType = typeof(ResToolsMaintenance))]
        public Nullable<DateTime> ScheduleDate { get; set; }
        private string _ScheduleDate;
        public string ScheduleDateStr
        {
            get
            {
                if (string.IsNullOrEmpty(_ScheduleDate))
                {
                    _ScheduleDate = FnCommon.ConvertDateByTimeZone(ScheduleDate, true);
                }
                return _ScheduleDate;
            }
            set { this._ScheduleDate = value; }
        }

        private string _ScheduleDt;
        public string ScheduleDateStrOnlyDate
        {
            get
            {
                if (string.IsNullOrEmpty(_ScheduleDt))
                {
                    _ScheduleDt = FnCommon.ConvertDateByTimeZone(ScheduleDate, true, true);
                }
                return _ScheduleDt;
            }
            set { this._ScheduleDt = value; }
        }

        //TrackingMeasurement
        //[Display(Name = "TrackingMeasurement", ResourceType = typeof(ResToolsMaintenance))]                
        //public int TrackingMeasurement { get; set; }

        [Display(Name = "TrackingMeasurement", ResourceType = typeof(ResToolsMaintenance))]
        public int TrackngMeasurement { get; set; }

        //TrackingMeasurementValue
        [Display(Name = "TrackingMeasurementValue", ResourceType = typeof(ResToolsMaintenance))]
        [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String TrackingMeasurementValue { get; set; }

        //LastMaintenanceDate
        [Display(Name = "LastMaintenanceDate", ResourceType = typeof(ResToolsMaintenance))]
        public Nullable<System.DateTime> LastMaintenanceDate { get; set; }
        public string LastMaintenanceDateStr { get; set; }
        //LastMeasurementValue
        [Display(Name = "LastMeasurementValue", ResourceType = typeof(ResToolsMaintenance))]
        [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String LastMeasurementValue { get; set; }

        //WorkorderID
        [Display(Name = "WorkorderID", ResourceType = typeof(ResToolsMaintenance))]
        public Nullable<System.Guid> WorkorderGUID { get; set; }

        public System.String WOName { get; set; }

        //RequisitionID
        [Display(Name = "RequisitionID", ResourceType = typeof(ResToolsMaintenance))]
        public Nullable<System.Guid> RequisitionGUID { get; set; }

        public System.String RequisitionName { get; set; }

        //Status
        [Display(Name = "Status", ResourceType = typeof(ResToolsMaintenance))]
        [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String Status { get; set; }

        //Created
        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public System.DateTime Created { get; set; }

        //CreatedBy
        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CreatedBy { get; set; }

        //Updated
        [Display(Name = "UpdatedOn", ResourceType = typeof(Resources.ResCommon))]
        public System.DateTime Updated { get; set; }

        //LastUpdatedBy
        [Display(Name = "LastUpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> LastUpdatedBy { get; set; }

        //Room
        [Display(Name = "Room", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> Room { get; set; }

        //IsArchived
        public Nullable<Boolean> IsArchived { get; set; }

        //IsDeleted
        public Nullable<Boolean> IsDeleted { get; set; }

        //GUID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Guid GUID { get; set; }

        //CompanyID
        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CompanyID { get; set; }

        //UDF1
        [Display(Name = "UDF1", ResourceType = typeof(ResToolsMaintenance))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String UDF1 { get; set; }

        //UDF2
        [Display(Name = "UDF2", ResourceType = typeof(ResToolsMaintenance))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String UDF2 { get; set; }

        //UDF3
        [Display(Name = "UDF3", ResourceType = typeof(ResToolsMaintenance))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String UDF3 { get; set; }

        //UDF4
        [Display(Name = "UDF4", ResourceType = typeof(ResToolsMaintenance))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String UDF4 { get; set; }

        //UDF5
        [Display(Name = "UDF5", ResourceType = typeof(ResToolsMaintenance))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String UDF5 { get; set; }

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        //SchedulerGUID
        [Display(Name = "ToolGUID", ResourceType = typeof(ResToolsMaintenance))]
        public Nullable<Guid> ToolGUID { get; set; }

        //SchedulerGUID
        [Display(Name = "AssetGUID", ResourceType = typeof(ResToolsMaintenance))]
        public Nullable<Guid> AssetGUID { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "SchedulerFor", ResourceType = typeof(ResToolsMaintenance))]
        public Nullable<byte> ScheduleFor { get; set; }
        public string SchedulerForName { get; set; }
        public string SchedulerTypeName { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "AssetToolGUID", ResourceType = typeof(ResToolsMaintenance))]
        public Nullable<Guid> AssetToolGUID { get; set; }

        [Display(Name = "Itemname", ResourceType = typeof(ResToolsMaintenance))]
        public string Itemname { get; set; }


        [Display(Name = "SchedulerName", ResourceType = typeof(ResToolsScheduler))]
        public string SchedulerName { get; set; }
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]

        [Display(Name = "ScheduleName", ResourceType = typeof(ResToolsMaintenance))]
        public Nullable<Guid> ToolSchedulerGuid { get; set; }

        public string MaintenanceType { get; set; }
        public Nullable<byte> SchedulerType { get; set; }

        public int? TrackingMeasurementMapping { get; set; }

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
        public string ToolName { get; set; }
        public string AssetName { get; set; }
        public Guid? MappingGUID { get; set; }
        public System.Int64? AssetID { get; set; }
        public System.Int64? ToolID { get; set; }
        public int DaysDiff { get; set; }

        public double? ReadingValue { get; set; }
        public double? TrackingMeasurementTimeBase { get; set; }

        public System.String Description { get; set; }
        public System.String Make { get; set; }
        public System.String Model { get; set; }
        public System.String Serial { get; set; }
        public Nullable<System.Double> TotalCost { get; set; }

        public Nullable<System.Int64> ToolCategoryID { get; set; }
        public Nullable<System.DateTime> PurchaseDate { get; set; }
        private string _PurchaseDate;
        public string PurchaseDateString
        {
            get
            {
                if (string.IsNullOrEmpty(_PurchaseDate))
                {
                    _PurchaseDate = FnCommon.ConvertDateByTimeZone(PurchaseDate, true, true);
                }
                return _PurchaseDate;
            }
            set { this._PurchaseDate = value; }
        }
        public System.String PurchaseDateStr { get; set; }

        public Nullable<System.Double> PurchasePrice { get; set; }
        public Nullable<System.Double> DepreciatedValue { get; set; }
        public Nullable<System.DateTime> SuggestedMaintenanceDate { get; set; }
        public System.String _SuggestedMaintenanceDateStr { get; set; }
        public string SuggestedMaintenanceDateStr
        {
            get
            {
                if (string.IsNullOrEmpty(_SuggestedMaintenanceDateStr))
                {
                    _SuggestedMaintenanceDateStr = FnCommon.ConvertDateByTimeZone(SuggestedMaintenanceDate, true);
                }
                return _SuggestedMaintenanceDateStr;
            }
            set { this._SuggestedMaintenanceDateStr = value; }
        }

        public System.String UDF6 { get; set; }
        public System.String UDF7 { get; set; }
        public System.String UDF8 { get; set; }
        public System.String UDF9 { get; set; }
        public System.String UDF10 { get; set; }
        public Nullable<System.Int64> AssetCategoryID { get; set; }
        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }

        public Nullable<System.DateTime> ReceivedOn { get; set; }
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

        public Nullable<System.DateTime> ReceivedOnWeb { get; set; }
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

        public int? NoOfPastMntsToConsider { get; set; }
        public System.String ImagePath { get; set; }
        public string ImageType { get; set; }
        public System.String AssetImageExternalURL { get; set; }
        public string AssetCategory { get; set; }
        public string ToolCategory { get; set; }

        public List<ToolMaintenanceDetailsDTO> ToolMaintenanceListItem { get; set; }
        public Nullable<int> NoOfLineItems { get; set; }

        public int? TotalRecords { get; set; }
    }

    public class ResToolsMaintenance
    {
        private static string ResourceFileName = "ResToolsMaintenance";

        public static string ReadingValue
        {
            get
            {
                return ResourceRead.GetResourceValue("ReadingValue", ResourceFileName);
            }
        }
        public static string ItemModelHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemModelHeader", ResourceFileName);
            }
        }

        public static string TrackingMeasurementTimeBase
        {
            get
            {
                return ResourceRead.GetResourceValue("TrackingMeasurementTimeBase", ResourceFileName);
            }
        }

        public static string SchedulerFor
        {
            get
            {
                return ResourceRead.GetResourceValue("SchedulerFor", ResourceFileName);
            }
        }
        public static string Reading
        {
            get
            {
                return ResourceRead.GetResourceValue("Reading", ResourceFileName);
            }
        }
        public static string EntryDate
        {
            get
            {
                return ResourceRead.GetResourceValue("EntryDate", ResourceFileName);
            }
        }

        public static string Itemname
        {
            get
            {
                return ResourceRead.GetResourceValue("Itemname", ResourceFileName);
            }
        }

        
        public static string AssetToolGUID
        {
            get
            {
                return ResourceRead.GetResourceValue("Itemname", ResourceFileName);
            }
        }

        public static string ScheduleName
        {
            get
            {
                return ResourceRead.GetResourceValue("ScheduleName", ResourceFileName);
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
        ///   Looks up a localized string similar to ToolsMaintenance {0} already exist! Try with Another!.
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
        public static string AlreadyStartMntsExist
        {
            get
            {
                return ResourceRead.GetResourceValue("AlreadyStartMntsExist", ResourceFileName);
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
        ///   Looks up a localized string similar to ToolsMaintenance.
        /// </summary>
        public static string ToolsMaintenanceHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolsMaintenanceHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ToolsMaintenance.
        /// </summary>
        public static string ToolsMaintenance
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolsMaintenance", ResourceFileName);
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
        ///   Looks up a localized string similar to MaintenanceName.
        /// </summary>
        public static string MaintenanceName
        {
            get
            {
                return ResourceRead.GetResourceValue("MaintenanceName", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MaintenanceDate.
        /// </summary>
        public static string MaintenanceDate
        {
            get
            {
                return ResourceRead.GetResourceValue("MaintenanceDate", ResourceFileName);
            }
        }

        
        public static string MaintenanceDateStr
        {
            get
            {
                return ResourceRead.GetResourceValue("MaintenanceDate", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to SchedulerID.
        /// </summary>
        public static string SchedulerID
        {
            get
            {
                return ResourceRead.GetResourceValue("SchedulerID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to SchedulerGUID.
        /// </summary>
        public static string SchedulerGUID
        {
            get
            {
                return ResourceRead.GetResourceValue("SchedulerGUID", ResourceFileName);
            }
        }


        public static string ScheduleDate
        {
            get
            {
                return ResourceRead.GetResourceValue("ScheduleDate", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to TrackingMeasurement.
        /// </summary>
        public static string TrackingMeasurement
        {
            get
            {
                return ResourceRead.GetResourceValue("TrackingMeasurement", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to TrackingMeasurementValue.
        /// </summary>
        public static string TrackingMeasurementValue
        {
            get
            {
                return ResourceRead.GetResourceValue("TrackingMeasurementValue", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to LastMaintenanceDate.
        /// </summary>
        public static string LastMaintenanceDate
        {
            get
            {
                return ResourceRead.GetResourceValue("LastMaintenanceDate", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to LastMeasurementValue.
        /// </summary>
        public static string LastMeasurementValue
        {
            get
            {
                return ResourceRead.GetResourceValue("LastMeasurementValue", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to WorkorderID.
        /// </summary>
        public static string WorkorderID
        {
            get
            {
                return ResourceRead.GetResourceValue("WorkorderID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to RequisitionID.
        /// </summary>
        public static string RequisitionID
        {
            get
            {
                return ResourceRead.GetResourceValue("RequisitionID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Status.
        /// </summary>
        public static string Status
        {
            get
            {
                return ResourceRead.GetResourceValue("Status", ResourceFileName);
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
        ///   Looks up a localized string similar to CreatedBy.
        /// </summary>
        public static string CreatedBy
        {
            get
            {
                return ResourceRead.GetResourceValue("CreatedBy", ResourceFileName);
            }
        }
        public static string TotalCost
        {
            get
            {
                return ResourceRead.GetResourceValue("TotalCost", ResourceFileName);
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


        ///   Looks up a localized string similar to eTurns: Job Types.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", ResourceFileName);
            }
        }
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", ResourceFileName);
            }
        }
        public static string msgNoMileageOrOdometer
        {
            get
            {
                return ResourceRead.GetResourceValue("msgNoMileageOrOdometer", ResourceFileName);
            }
        }
        public static string EnterProperMaintenanceDate 
        {
            get
            {
                return ResourceRead.GetResourceValue("EnterProperMaintenanceDate", ResourceFileName);
            }
        }
        public static string MaintenanceTypeUnScheduled
        {
            get
            {
                return ResourceRead.GetResourceValue("MaintenanceTypeUnScheduled", ResourceFileName);
            }
        }
        public static string MaintenanceTypePast
        {
            get
            {
                return ResourceRead.GetResourceValue("MaintenanceTypePast", ResourceFileName);
            }
        }
    }

    public class PullOnMaintenanceDTO
    {
        public Guid? ItemGUID { get; set; }
        public Guid? MaintenanceGUID { get; set; }
        public double? QuanityPulled { get; set; }
    }
}


