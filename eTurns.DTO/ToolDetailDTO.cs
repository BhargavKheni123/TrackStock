using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    //Model Tool Detail For Kit
    //28-Aug-2018

    public class ToolDetailDTO
    {
        // Tool ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Guid GUID { get; set; }

        public System.Int64? ToolID { get; set; }
        public Nullable<Guid> ToolGUID { get; set; }

        public Nullable<Guid> ToolItemGUID { get; set; }

        //QuantityPerKit
        [Display(Name = "QuantityPerKit", ResourceType = typeof(ResKitMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Int64> QuantityPerKit { get; set; }

        //QuantityReadyForAssembly
        [Display(Name = "QuantityReadyForAssembly", ResourceType = typeof(ResKitMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Int64> QuantityReadyForAssembly { get; set; }

        //AvailableItemsInWIP
        [Display(Name = "AvailableItemsInWIP", ResourceType = typeof(ResKitMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> AvailableItemsInWIP { get; set; }

        public double? Cost { get; set; }
        public int SessionSr { get; set; }
        public string Serial { get; set; }
        public string ToolName { get; set; }

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

        //Tool IsArchived
        [Display(Name = "IsArchived", ResourceType = typeof(ResCommon))]
        public Nullable<bool> IsArchived { get; set; }

        //Tool IsDeleted
        [Display(Name = "IsDeleted", ResourceType = typeof(ResCommon))]
        public Nullable<bool> IsDeleted { get; set; }

        //Tool Category Name
        [Display(Name = "Location", ResourceType = typeof(ResToolMaster))]
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
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Double Quantity { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
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

        public string WhatWhereAction { get; set; }
        public bool SerialNumberTracking { get; set; }


    }


}
