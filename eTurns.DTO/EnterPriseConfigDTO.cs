using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{
    public class EnterPriseConfigDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        //CompanyID
        [Display(Name = "EnterPriseID", ResourceType = typeof(ResEnterPriseConfig))]
        public Nullable<System.Int64> EnterPriseID { get; set; }

        //ScheduleDaysBefore
        [Display(Name = "ScheduleDaysBefore", ResourceType = typeof(ResEnterPriseConfig))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> ScheduleDaysBefore { get; set; }

        //OperationalHoursBefore
        [Display(Name = "OperationalHoursBefore", ResourceType = typeof(ResEnterPriseConfig))]
        public Nullable<System.Double> OperationalHoursBefore { get; set; }

        //MileageBefore
        [Display(Name = "MileageBefore", ResourceType = typeof(ResEnterPriseConfig))]
        public Nullable<System.Double> MileageBefore { get; set; }

        //ProjectAmountExceed
        [Display(Name = "ProjectAmountExceed", ResourceType = typeof(ResEnterPriseConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> ProjectAmountExceed { get; set; }

        //ProjectItemQuantitExceed
        [Display(Name = "ProjectItemQuantitExceed", ResourceType = typeof(ResEnterPriseConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> ProjectItemQuantitExceed { get; set; }

        //ProjectItemAmountExceed
        [Display(Name = "ProjectItemAmountExceed", ResourceType = typeof(ResEnterPriseConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> ProjectItemAmountExceed { get; set; }

        //CostDecimalPoints
        [Display(Name = "CostDecimalPoints", ResourceType = typeof(ResEnterPriseConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> CostDecimalPoints { get; set; }

        //QuantityDecimalPoints
        [Display(Name = "QuantityDecimalPoints", ResourceType = typeof(ResEnterPriseConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> QuantityDecimalPoints { get; set; }

        //DateFormat
        [Display(Name = "DateFormat", ResourceType = typeof(ResEnterPriseConfig))]
        [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String DateFormat { get; set; }

        //NOBackDays
        [Display(Name = "NOBackDays", ResourceType = typeof(ResEnterPriseConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> NOBackDays { get; set; }

        //NODaysAve
        [Display(Name = "NODaysAve", ResourceType = typeof(ResEnterPriseConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> NODaysAve { get; set; }

        //NOTimes
        [Display(Name = "NOTimes", ResourceType = typeof(ResEnterPriseConfig))]
        public Nullable<System.Decimal> NOTimes { get; set; }

        //MinPer
        [Display(Name = "MinPer", ResourceType = typeof(ResEnterPriseConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> MinPer { get; set; }

        //MaxPer
        [Display(Name = "MaxPer", ResourceType = typeof(ResEnterPriseConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> MaxPer { get; set; }

        //CurrencySymbol
        [Display(Name = "CurrencySymbol", ResourceType = typeof(ResEnterPriseConfig))]
        [StringLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String CurrencySymbol { get; set; }

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        [Display(Name = "Auto Email Time For Pending Orders")]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTPndOrders { get; set; }

        [Display(Name = "Auto Email Time For Pending Requisition")]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTPndRequisition { get; set; }

        [Display(Name = "Auto Email Time For Pending Transfer")]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTPndTransfer { get; set; }

        [Display(Name = "Auto Email Time For Suggested Orders Minimum")]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTSggstOrdMin { get; set; }

        [Display(Name = "Auto Email Time For Suggested Orders Critical")]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTSggstOrdCrt { get; set; }

        [Display(Name = "Auto Email Time For Asset maintenance Due")]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTAssetMntDue { get; set; }

        [Display(Name = "Auto Email Time For Tools maintenance Due")]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTToolsMntDue { get; set; }

        [Display(Name = "Auto Email Time For Items Stock Out")]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTItemStockOut { get; set; }

        [Display(Name = "Auto Email Time For Cycle Count")]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTCycleCount { get; set; }

        [Display(Name = "Auto Email Time For Cycle Count Item Missing")]
        public string AEMTCycleCntItmMiss { get; set; }

        [Display(Name = "Auto Email Time For Item Usege Report")]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTItemUsageRpt { get; set; }

        [Display(Name = "Auto Email Time For Item Receipt Report")]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTItemReceiveRpt { get; set; }


        [Display(Name = "Refresh Time(Second)")]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> GridRefreshTimeInSecond { get; set; }

        public string DateFormatCSharp { get; set; }

        //CompanyID
        [Display(Name = "PasswordExpiryDays", ResourceType = typeof(ResEnterPriseConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> PasswordExpiryDays { get; set; }

        [Display(Name = "PasswordExpiryWarningDays", ResourceType = typeof(ResEnterPriseConfig))]
        [DataType(DataType.Text)]
        [Range(30, int.MaxValue)]
        public Nullable<System.Int32> PasswordExpiryWarningDays { get; set; }


        [Display(Name = "DisplayAgreePopupDays", ResourceType = typeof(ResEnterPriseConfig))]
        [DataType(DataType.Text)]

        public Nullable<System.Int32> DisplayAgreePopupDays { get; set; }


        [Display(Name = "PreviousLastAllowedPWD", ResourceType = typeof(ResEnterPriseConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> PreviousLastAllowedPWD { get; set; }



        [Display(Name = "NumberOfBackDaysToSyncOverPDA", ResourceType = typeof(ResCompanyConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> NumberOfBackDaysToSyncOverPDA { get; set; }

        [Display(Name = "NotAllowedCharacter", ResourceType = typeof(ResEnterPriseConfig))]
        [DataType(DataType.Text)]
        public string NotAllowedCharacter { get; set; }


    }

    public class ResEnterPriseConfig
    {
        private static string ResourceFileName = "ResEnterPriseConfig";

        /// <summary>
        ///   Looks up a localized string similar to Action.
        /// </summary>
        public static string Action
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("Action", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CompanyConfig {0} already exist! Try with Another!.
        /// </summary>
        public static string Duplicate
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("Duplicate", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to HistoryID.
        /// </summary>
        public static string HistoryID
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("HistoryID", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Include Archived:.
        /// </summary>
        public static string IncludeArchived
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("IncludeArchived", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Include Deleted:.
        /// </summary>
        public static string IncludeDeleted
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("IncludeDeleted", ResourceFileName);
            }
        }



        /// <summary>
        ///   Looks up a localized string similar to Search.
        /// </summary>
        public static string Search
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("Search", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CompanyConfig.
        /// </summary>
        public static string CompanyConfigHeader
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("CompanyConfigHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CompanyConfig.
        /// </summary>
        public static string CompanyConfig
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("CompanyConfig", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to View History.
        /// </summary>
        public static string ViewHistory
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("ViewHistory", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ID.
        /// </summary>
        public static string ID
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("ID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CompanyID.
        /// </summary>
        public static string CompanyID
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("CompanyID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ScheduleDaysBefore.
        /// </summary>
        public static string ScheduleDaysBefore
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("ScheduleDaysBefore", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to OperationalHoursBefore.
        /// </summary>
        public static string OperationalHoursBefore
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("OperationalHoursBefore", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MileageBefore.
        /// </summary>
        public static string MileageBefore
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("MileageBefore", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ProjectAmountExceed.
        /// </summary>
        public static string ProjectAmountExceed
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("ProjectAmountExceed", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ProjectItemQuantitExceed.
        /// </summary>
        public static string ProjectItemQuantitExceed
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("ProjectItemQuantitExceed", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ProjectItemAmountExceed.
        /// </summary>
        public static string ProjectItemAmountExceed
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("ProjectItemAmountExceed", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CostDecimalPoints.
        /// </summary>
        public static string CostDecimalPoints
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("CostDecimalPoints", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to QuantityDecimalPoints.
        /// </summary>
        public static string QuantityDecimalPoints
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("QuantityDecimalPoints", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to DateFormat.
        /// </summary>
        public static string DateFormat
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("DateFormat", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to NOBackDays.
        /// </summary>
        public static string NOBackDays
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("NOBackDays", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to NODaysAve.
        /// </summary>
        public static string NODaysAve
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("NODaysAve", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to NODaysAve.
        /// </summary>
        public static string PasswordExpiryDays
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("PasswordExpiryDays", ResourceFileName);
            }
        }
        public static string DisplayAgreePopupDays
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("DisplayAgreePopupDays", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to NOTimes.
        /// </summary>
        public static string NOTimes
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("NOTimes", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MinPer.
        /// </summary>
        public static string MinPer
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("MinPer", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MaxPer.
        /// </summary>
        public static string MaxPer
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("MaxPer", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CurrencySymbol.
        /// </summary>
        public static string CurrencySymbol
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("CurrencySymbol", ResourceFileName);
            }
        }


        ///   Looks up a localized string similar to eTurns: Job Types.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("PageTitle", ResourceFileName);
            }
        }
        public static string NumberOfBackDaysToSyncOverPDA
        {
            get
            { return ResourceRead.GetEnterPriseResourceValue("NumberOfBackDaysToSyncOverPDA", ResourceFileName); }
        }
        public static string PasswordExpiryWarningDays
        {
            get
            { return ResourceRead.GetEnterPriseResourceValue("PasswordExpiryWarningDays", ResourceFileName); }
        }
        public static string PreviousLastAllowedPWD
        {
            get
            { return ResourceRead.GetEnterPriseResourceValue("PreviousLastAllowedPWD", ResourceFileName); }
        }
        public static string NotAllowedCharacter
        {
            get
            { return ResourceRead.GetEnterPriseResourceValue("NotAllowedCharacter", ResourceFileName); }
        }

        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("PageHeader", ResourceFileName);
            }
        }
    }
}



