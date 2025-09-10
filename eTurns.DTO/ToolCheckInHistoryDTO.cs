using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class ToolCheckInHistoryDTO
    {
        public System.Int64 ID { get; set; }

        [Display(Name = "CheckInCheckOutID", ResourceType = typeof(ResToolCheckInHistory))]
        public Nullable<System.Guid> CheckInCheckOutGUID { get; set; }

        [Display(Name = "CheckOutStatus", ResourceType = typeof(ResToolCheckInHistory))]
        public System.String CheckOutStatus { get; set; }

        [Display(Name = "CheckedOutQTY", ResourceType = typeof(ResToolCheckInHistory))]
        public Nullable<System.Double> CheckedOutQTY { get; set; }

        [Display(Name = "CheckedOutMQTY", ResourceType = typeof(ResToolCheckInHistory))]
        public Nullable<System.Double> CheckedOutMQTY { get; set; }

        [Display(Name = "CheckOutDate", ResourceType = typeof(ResToolCheckInHistory))]
        public Nullable<System.DateTime> CheckOutDate { get; set; }

        [Display(Name = "CheckInDate", ResourceType = typeof(ResToolCheckInHistory))]
        public Nullable<System.DateTime> CheckInDate { get; set; }

        [Display(Name = "Created", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Created { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CreatedBy { get; set; }

        [Display(Name = "Updated", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Updated { get; set; }

        [Display(Name = "LastUpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> LastUpdatedBy { get; set; }

        [Display(Name = "Room", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> Room { get; set; }

        public Nullable<Boolean> IsArchived { get; set; }

        public Nullable<Boolean> IsDeleted { get; set; }

        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CompanyID { get; set; }

        [Display(Name = "UDF1", ResourceType = typeof(ResToolCheckInHistory))]
        public System.String UDF1 { get; set; }

        [Display(Name = "UDF2", ResourceType = typeof(ResToolCheckInHistory))]
        public System.String UDF2 { get; set; }

        [Display(Name = "UDF3", ResourceType = typeof(ResToolCheckInHistory))]
        public System.String UDF3 { get; set; }

        [Display(Name = "UDF4", ResourceType = typeof(ResToolCheckInHistory))]
        public System.String UDF4 { get; set; }

        [Display(Name = "UDF5", ResourceType = typeof(ResToolCheckInHistory))]
        public System.String UDF5 { get; set; }

        [Display(Name = "CheckedOutQTYCurrent", ResourceType = typeof(ResToolCheckInHistory))]
        public Nullable<System.Double> CheckedOutQTYCurrent { get; set; }

        [Display(Name = "CheckedOutMQTYCurrent", ResourceType = typeof(ResToolCheckInHistory))]
        public Nullable<System.Double> CheckedOutMQTYCurrent { get; set; }

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        public Guid GUID { get; set; }

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
        private string _checkInDated;
        public string CheckInDated
        {
            get
            {
                if (string.IsNullOrEmpty(_checkInDated))
                {
                    _checkInDated = FnCommon.ConvertDateByTimeZone(CheckInDate, true);
                }
                return _checkInDated;
            }
            set { this._checkInDated = value; }
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
        public Nullable<Guid> TechnicianGuid { get; set; }
        public string Technician { get; set; }
        public string TechnicianCode { get; set; }

        public Nullable<System.Guid> ToolDetailGUID { get; set; }

        public string SerialNumber { get; set; }
    }

    public class ResToolCheckInHistory
    {
        private static string ResourceFileName = "ResToolCheckInHistory";

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
        ///   Looks up a localized string similar to ToolCheckInHistory {0} already exist! Try with Another!.
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
        ///   Looks up a localized string similar to ToolCheckInHistory.
        /// </summary>
        public static string ToolCheckInHistoryHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolCheckInHistoryHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ToolCheckInHistory.
        /// </summary>
        public static string ToolCheckInHistory
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolCheckInHistory", ResourceFileName);
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
        ///   Looks up a localized string similar to CheckInCheckOutID.
        /// </summary>
        public static string CheckInCheckOutID
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckInCheckOutID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CheckOutStatus.
        /// </summary>
        public static string CheckOutStatus
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckOutStatus", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CheckedOutQTY.
        /// </summary>
        public static string CheckedOutQTY
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckedOutQTY", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CheckedOutMQTY.
        /// </summary>
        public static string CheckedOutMQTY
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckedOutMQTY", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CheckOutDate.
        /// </summary>
        public static string CheckOutDate
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckOutDate", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CheckInDate.
        /// </summary>
        public static string CheckInDate
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckInDate", ResourceFileName);
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

        /// <summary>
        ///   Looks up a localized string similar to CheckedOutQTYCurrent.
        /// </summary>
        public static string CheckedOutQTYCurrent
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckedOutQTYCurrent", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CheckedOutMQTYCurrent.
        /// </summary>
        public static string CheckedOutMQTYCurrent
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckedOutMQTYCurrent", ResourceFileName);
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
    }
}


