using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{
    public class eVMISetupDTO
    {
        //ID

        public System.Int64 ID { get; set; }

        //PollType
        [Display(Name = "PollType", ResourceType = typeof(ReseVMISetup))]
        public Nullable<System.Int32> PollType { get; set; }

        //PollInterval
        [Display(Name = "PollInterval", ResourceType = typeof(ReseVMISetup))]
        public Nullable<System.Int32> PollInterval { get; set; }

        //PollTime1
        [Display(Name = "PollTime1", ResourceType = typeof(ReseVMISetup))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.TimeSpan> PollTime1 { get; set; }

        //PollTime2
        [Display(Name = "PollTime2", ResourceType = typeof(ReseVMISetup))]
        public Nullable<System.TimeSpan> PollTime2 { get; set; }

        //PollTime3
        [Display(Name = "PollTime3", ResourceType = typeof(ReseVMISetup))]
        public Nullable<System.TimeSpan> PollTime3 { get; set; }

        //PollTime4
        [Display(Name = "PollTime4", ResourceType = typeof(ReseVMISetup))]
        public Nullable<System.TimeSpan> PollTime4 { get; set; }

        //PollTime5
        [Display(Name = "PollTime5", ResourceType = typeof(ReseVMISetup))]
        public Nullable<System.TimeSpan> PollTime5 { get; set; }

        //PollTime6
        [Display(Name = "PollTime6", ResourceType = typeof(ReseVMISetup))]
        public Nullable<System.TimeSpan> PollTime6 { get; set; }

        //ErrorEmailAddresses
        [Display(Name = "ErrorEmailAddresses", ResourceType = typeof(ReseVMISetup))]
        [StringLength(4000, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String ErrorEmailAddresses { get; set; }

        //IsSuggstedOrder
        [Display(Name = "IsSuggstedOrder", ResourceType = typeof(ReseVMISetup))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Boolean IsSuggstedOrder { get; set; }

        //IsCriticalOrders
        [Display(Name = "IsCriticalOrders", ResourceType = typeof(ReseVMISetup))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Boolean IsCriticalOrders { get; set; }

        //IsBadPolls
        [Display(Name = "IsBadPolls", ResourceType = typeof(ReseVMISetup))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Boolean IsBadPolls { get; set; }

        //InactivityReportStart
        [Display(Name = "InactivityReportStart", ResourceType = typeof(ReseVMISetup))]
        public Nullable<System.TimeSpan> InactivityReportStart { get; set; }

        //InactivityReportEnd
        [Display(Name = "InactivityReportEnd", ResourceType = typeof(ReseVMISetup))]
        public Nullable<System.TimeSpan> InactivityReportEnd { get; set; }

        //InactivityonSaturday
        [Display(Name = "InactivityonSaturday", ResourceType = typeof(ReseVMISetup))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Boolean InactivityonSaturday { get; set; }

        //InactivityonSunday
        [Display(Name = "InactivityonSunday", ResourceType = typeof(ReseVMISetup))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Boolean InactivityonSunday { get; set; }

        //[Display(Name = "IsActiveSchedule", ResourceType = typeof(ReseVMISetup))]
        [Display(Name = "IsScheduleActive", ResourceType = typeof(ResSchedulerReportList))]
        public Boolean IsActiveSchedule { get; set; }

        //GUID

        public Guid GUID { get; set; }



        //IsDeleted
        public Nullable<Boolean> IsDeleted { get; set; }

        //IsArchived
        public Nullable<Boolean> IsArchived { get; set; }

        //CompanyID
        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CompanyID { get; set; }

        //Room
        [Display(Name = "Room", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> Room { get; set; }

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Created { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Updated { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CreatedBy { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> LastUpdatedBy { get; set; }

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

        [Display(Name = "ComPort", ResourceType = typeof(ReseVMISetup))]
        public string ComPort { get; set; }

        public string SelectedComPort { get; set; }
        [Display(Name = "NextPollDate", ResourceType = typeof(ReseVMISetup))]
        public Nullable<System.DateTime> NextPollDate { get; set; }

        //CountType
        [Display(Name = "CountType", ResourceType = typeof(ResInventoryCount))]
        public System.String CountType { get; set; }

        //PollAllBetweenTime
        [Display(Name = "PollAllBetweenTime", ResourceType = typeof(ReseVMISetup))]
        public Nullable<System.Int64> PollAllBetweenTime { get; set; }

        public string NextRunDateTime { get; set; }

        [Display(Name = "GetShelfID", ResourceType = typeof(ReseVMISetup))]
        [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 999, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Int32> ShelfID { get; set; }

        [Display(Name = "SetShelfID", ResourceType = typeof(ReseVMISetup))]
        [Range(1, 999, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> SetShelfID { get; set; }

        public Nullable<System.Int64> ComPortMasterID { get; set; }

        public TimeSpan? ScheduleTime { get; set; }
        public long? RoomScheduleID { get; set; }
        public bool IsScheduleActive { get; set; }
        public string NextPollDateTime { get; set; }
        public long EnterpriseId { get; set; }
        public string EnterpriseName { get; set; }
        public string CompanyName { get; set; }
        public string EnterpriseDBName { get; set; }
        public long eVMIID { get; set; }
    }

    public class ReseVMISetup
    {
        private static string ResourceFileName = "ReseVMISetup";

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
        ///   Looks up a localized string similar to eVMISetup {0} already exist! Try with Another!.
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
        ///   Looks up a localized string similar to eVMISetup.
        /// </summary>
        public static string eVMISetupHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("eVMISetupHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eVMISetup.
        /// </summary>
        public static string eVMISetup
        {
            get
            {
                return ResourceRead.GetResourceValue("eVMISetup", ResourceFileName);
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
        ///   Looks up a localized string similar to PollType.
        /// </summary>
        public static string PollType
        {
            get
            {
                return ResourceRead.GetResourceValue("PollType", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to PollInterval.
        /// </summary>
        public static string PollInterval
        {
            get
            {
                return ResourceRead.GetResourceValue("PollInterval", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to PollTime1.
        /// </summary>
        public static string PollTime1
        {
            get
            {
                return ResourceRead.GetResourceValue("PollTime1", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to PollTime2.
        /// </summary>
        public static string PollTime2
        {
            get
            {
                return ResourceRead.GetResourceValue("PollTime2", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to PollTime3.
        /// </summary>
        public static string PollTime3
        {
            get
            {
                return ResourceRead.GetResourceValue("PollTime3", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to PollTime4.
        /// </summary>
        public static string PollTime4
        {
            get
            {
                return ResourceRead.GetResourceValue("PollTime4", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to PollTime5.
        /// </summary>
        public static string PollTime5
        {
            get
            {
                return ResourceRead.GetResourceValue("PollTime5", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to PollTime6.
        /// </summary>
        public static string PollTime6
        {
            get
            {
                return ResourceRead.GetResourceValue("PollTime6", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ErrorEmailAddresses.
        /// </summary>
        public static string ErrorEmailAddresses
        {
            get
            {
                return ResourceRead.GetResourceValue("ErrorEmailAddresses", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsSuggstedOrder.
        /// </summary>
        public static string IsSuggstedOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("IsSuggstedOrder", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsCriticalOrders.
        /// </summary>
        public static string IsCriticalOrders
        {
            get
            {
                return ResourceRead.GetResourceValue("IsCriticalOrders", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsBadPolls.
        /// </summary>
        public static string IsBadPolls
        {
            get
            {
                return ResourceRead.GetResourceValue("IsBadPolls", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to InactivityReportStart.
        /// </summary>
        public static string InactivityReportStart
        {
            get
            {
                return ResourceRead.GetResourceValue("InactivityReportStart", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to InactivityReportEnd.
        /// </summary>
        public static string InactivityReportEnd
        {
            get
            {
                return ResourceRead.GetResourceValue("InactivityReportEnd", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to InactivityonSaturday.
        /// </summary>
        public static string InactivityonSaturday
        {
            get
            {
                return ResourceRead.GetResourceValue("InactivityonSaturday", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to InactivityonSunday.
        /// </summary>
        public static string InactivityonSunday
        {
            get
            {
                return ResourceRead.GetResourceValue("InactivityonSunday", ResourceFileName);
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
        public static string Room
        {
            get
            {
                return ResourceRead.GetResourceValue("Room", ResourceFileName);
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

        public static string ComPort
        {
            get
            {
                return ResourceRead.GetResourceValue("ComPort", ResourceFileName);
            }
        }
        public static string TCPPort
        {
            get
            {
                return ResourceRead.GetResourceValue("TCPPort", ResourceFileName);
            }
        }
        public static string ShelfID
        {
            get
            {
                return ResourceRead.GetResourceValue("ShelfID", ResourceFileName);
            }
        }

        public static string Poll
        {
            get
            {
                return ResourceRead.GetResourceValue("Poll", ResourceFileName);
            }
        }

        public static string ComportRequire
        {
            get
            {
                return ResourceRead.GetResourceValue("ComportRequire", ResourceFileName);
            }
        }

        public static string ComportValidate
        {
            get
            {
                return ResourceRead.GetResourceValue("ComportValidate", ResourceFileName);
            }
        }

        public static string ComportSingleValidate
        {
            get
            {
                return ResourceRead.GetResourceValue("ComportSingleValidate", ResourceFileName);
            }
        }

        public static string Tare
        {
            get
            {
                return ResourceRead.GetResourceValue("Tare", ResourceFileName);
            }
        }

        public static string NextPollDate
        {
            get
            {
                return ResourceRead.GetResourceValue("NextPollDate", ResourceFileName);
            }
        }

        public static string PollAllBetweenTime
        {
            get
            {
                return ResourceRead.GetResourceValue("PollAllBetweenTime", ResourceFileName);
            }
        }

        public static string ValidatePollBetween
        {
            get
            {
                return ResourceRead.GetResourceValue("ValidatePollBetween", ResourceFileName);
            }
        }
        public static string msgRemainingItems
        {
            get
            {
                return ResourceRead.GetResourceValue("msgRemainingItems", ResourceFileName);
            }
        }
        public static string ValidateSchedulePoll
        {
            get
            {
                return ResourceRead.GetResourceValue("ValidateSchedulePoll", ResourceFileName);
            }
        }

        public static string IsActiveSchedule
        {
            get
            {
                return ResourceRead.GetResourceValue("IsActiveSchedule", ResourceFileName);
            }
        }

        public static string GetShelfID
        {
            get
            {
                return ResourceRead.GetResourceValue("GetShelfID", ResourceFileName);
            }
        }

        public static string SetShelfID
        {
            get
            {
                return ResourceRead.GetResourceValue("SetShelfID", ResourceFileName);
            }
        }

        public static string Calibrate
        {
            get
            {
                return ResourceRead.GetResourceValue("Calibrate", ResourceFileName);
            }
        }

        public static string CalibrationWeight
        {
            get
            {
                return ResourceRead.GetResourceValue("CalibrationWeight", ResourceFileName);
            }
        }


        public static string Request
        {
            get
            {
                return ResourceRead.GetResourceValue("Request", ResourceFileName);
            }
        }


        public static string TareDirect
        {
            get
            {
                return ResourceRead.GetResourceValue("TareDirect", ResourceFileName);
            }
        }

        public static string Version
        {
            get
            {
                return ResourceRead.GetResourceValue("Version", ResourceFileName);
            }
        }
        public static string SrNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("SrNumber", ResourceFileName);
            }
        }
        public static string Model
        {
            get
            {
                return ResourceRead.GetResourceValue("Model", ResourceFileName);
            }
        }
        public static string ValidateShelfID
        {
            get
            {
                return ResourceRead.GetResourceValue("ValidateShelfID", ResourceFileName);
            }
        }

        public static string Calibration { get { return ResourceRead.GetResourceValue("Calibration", ResourceFileName); } }
        public static string StepOne { get { return ResourceRead.GetResourceValue("StepOne", ResourceFileName); } }
        public static string StepTwo { get { return ResourceRead.GetResourceValue("StepTwo", ResourceFileName); } }
        public static string StepThree { get { return ResourceRead.GetResourceValue("StepThree", ResourceFileName); } }
        public static string StepOneDescription { get { return ResourceRead.GetResourceValue("StepOneDescription", ResourceFileName); } }
        public static string StepTwoDescription { get { return ResourceRead.GetResourceValue("StepTwoDescription", ResourceFileName); } }
        public static string StepThreeDescription { get { return ResourceRead.GetResourceValue("StepThreeDescription", ResourceFileName); } }
        public static string CalibrateRequestNoteOne { get { return ResourceRead.GetResourceValue("CalibrateRequestNoteOne", ResourceFileName); } }
        public static string CalibrateRequestNoteTwo { get { return ResourceRead.GetResourceValue("CalibrateRequestNoteTwo", ResourceFileName); } }
        public static string CalibrationStart { get { return ResourceRead.GetResourceValue("CalibrationStart", ResourceFileName); } }
        public static string CalibrateZero { get { return ResourceRead.GetResourceValue("CalibrateZero", ResourceFileName); } }
        public static string CalibrateXWeight { get { return ResourceRead.GetResourceValue("CalibrateXWeight", ResourceFileName); } }
        public static string CalibrateStepOneSuccess { get { return ResourceRead.GetResourceValue("CalibrateStepOneSuccess", ResourceFileName); } }
        public static string CalibrateStepTwoSuccess { get { return ResourceRead.GetResourceValue("CalibrateStepTwoSuccess", ResourceFileName); } }
        public static string CalibrateStepThreeSuccess { get { return ResourceRead.GetResourceValue("CalibrateStepThreeSuccess", ResourceFileName); } }
        public static string COMPortAndRequestTypeMandatory { get { return ResourceRead.GetResourceValue("COMPortAndRequestTypeMandatory", ResourceFileName); } }
        public static string SelectItemLocationValue { get { return ResourceRead.GetResourceValue("SelectItemLocationValue", ResourceFileName); } }
        public static string SelectScaleValue { get { return ResourceRead.GetResourceValue("SelectScaleValue", ResourceFileName); } }
        public static string SelectModelToSet { get { return ResourceRead.GetResourceValue("SelectModelToSet", ResourceFileName); } }
        public static string EnterCalibrationWeight { get { return ResourceRead.GetResourceValue("EnterCalibrationWeight", ResourceFileName); } }
        public static string EnterShelfIDToSet { get { return ResourceRead.GetResourceValue("EnterShelfIDToSet", ResourceFileName); } }
        public static string EvmiSetting { get { return ResourceRead.GetResourceValue("EvmiSetting", ResourceFileName); } }
        public static string TimedPoll { get { return ResourceRead.GetResourceValue("TimedPoll", ResourceFileName); } }
        public static string PollTime { get { return ResourceRead.GetResourceValue("PollTime", ResourceFileName); } }
        public static string NoScheduledPolling { get { return ResourceRead.GetResourceValue("NoScheduledPolling", ResourceFileName); } }
        public static string FrequentPolling { get { return ResourceRead.GetResourceValue("FrequentPolling", ResourceFileName); } }
        public static string PerDayPolling { get { return ResourceRead.GetResourceValue("PerDayPolling", ResourceFileName); } }

    }

    public enum eVMIScheduleFor
    {
        eVMISchedule = 11
    }

    public class EVMIMissedPollDTO
    {
        public int RowNo { get; set; }
        public long CompanyID { get; set; }
        public long RoomID { get; set; }
        public DateTime? NextPollDate { get; set; }
        public DateTime? LastPollDone { get; set; }
        public DateTime UTCDATE { get; set; }
        public int? CurrentTimeDiff { get; set; }
        public int? LastPollNextPollTimeDiff { get; set; }
        public bool IsMissedPoll { get; set; }

        public string RoomName { get; set; }
        public string CompanyName { get; set; }
    }
}


