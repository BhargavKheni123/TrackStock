//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace eTurns.DTO
//{
//    public class CompanyConfigDTO
//    {
//        public System.Int64 ID { get; set; }
//        public System.Int64 CompanyID { get; set; }

//        public System.Int32? ScheduleDaysBefore { get; set; }
//        public System.Double? OperationalHoursBefore { get; set; }
//        public System.Double? MileageBefore { get; set; }
//        public System.Int32 ProjectAmountExceed { get; set; }
//        public System.Int32 ProjectItemQuantitExceed { get; set; }
//        public System.Int32 ProjectItemAmountExceed { get; set; }
//        public System.Int32 NOBackDays { get; set; }
//        public System.Int32 NODaysAve { get; set; }
//        public System.Decimal NOTimes { get; set; }
//        public System.Int32 MinPer { get; set; }
//        public System.Int32 MaxPer { get; set; }
//        public string CurrencySymbol { get; set; }

//    }
//}

using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{
    [Serializable]
    public class CompanyConfigDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        //CompanyID
        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CompanyID { get; set; }

        //ScheduleDaysBefore
        [Display(Name = "ScheduleDaysBefore", ResourceType = typeof(ResCompanyConfig))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> ScheduleDaysBefore { get; set; }

        //OperationalHoursBefore
        [Display(Name = "OperationalHoursBefore", ResourceType = typeof(ResCompanyConfig))]
        public Nullable<System.Double> OperationalHoursBefore { get; set; }

        //MileageBefore
        [Display(Name = "MileageBefore", ResourceType = typeof(ResCompanyConfig))]
        public Nullable<System.Double> MileageBefore { get; set; }

        //ProjectAmountExceed
        [Display(Name = "ProjectAmountExceed", ResourceType = typeof(ResCompanyConfig))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> ProjectAmountExceed { get; set; }

        //ProjectItemQuantitExceed
        [Display(Name = "ProjectItemQuantitExceed", ResourceType = typeof(ResCompanyConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> ProjectItemQuantitExceed { get; set; }

        //ProjectItemAmountExceed
        [Display(Name = "ProjectItemAmountExceed", ResourceType = typeof(ResCompanyConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> ProjectItemAmountExceed { get; set; }

        //CostDecimalPoints
        [Display(Name = "CostDecimalPoints", ResourceType = typeof(ResCompanyConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> CostDecimalPoints { get; set; }

        //QuantityDecimalPoints
        [Display(Name = "QuantityDecimalPoints", ResourceType = typeof(ResCompanyConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> QuantityDecimalPoints { get; set; }

        //DateFormat
        [Display(Name = "DateFormat", ResourceType = typeof(ResCompanyConfig))]
        [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String DateFormat { get; set; }

        //NOBackDays
        [Display(Name = "NOBackDays", ResourceType = typeof(ResCompanyConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> NOBackDays { get; set; }

        //NODaysAve
        [Display(Name = "NODaysAve", ResourceType = typeof(ResCompanyConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> NODaysAve { get; set; }

        //NOTimes
        [Display(Name = "NOTimes", ResourceType = typeof(ResCompanyConfig))]
        public Nullable<System.Decimal> NOTimes { get; set; }

        //MinPer
        [Display(Name = "MinPer", ResourceType = typeof(ResCompanyConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> MinPer { get; set; }

        //MaxPer
        [Display(Name = "MaxPer", ResourceType = typeof(ResCompanyConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> MaxPer { get; set; }

        //CurrencySymbol
        [Display(Name = "CurrencySymbol", ResourceType = typeof(ResCompanyConfig))]
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

        [Display(Name = "AEMTPndOrders", ResourceType = typeof(ResCompanyConfig))]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTPndOrders { get; set; }

        [Display(Name = "AEMTPndRequisition", ResourceType = typeof(ResCompanyConfig))]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTPndRequisition { get; set; }

        [Display(Name = "AEMTPndTransfer", ResourceType = typeof(ResCompanyConfig))]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTPndTransfer { get; set; }

        [Display(Name = "AEMTSggstOrdMin", ResourceType = typeof(ResCompanyConfig))]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTSggstOrdMin { get; set; }

        [Display(Name = "AEMTSggstOrdCrt", ResourceType = typeof(ResCompanyConfig))]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTSggstOrdCrt { get; set; }

        [Display(Name = "AEMTAssetMntDue", ResourceType = typeof(ResCompanyConfig))]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTAssetMntDue { get; set; }

        [Display(Name = "AEMTToolsMntDue", ResourceType = typeof(ResCompanyConfig))]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTToolsMntDue { get; set; }

        [Display(Name = "AEMTItemStockOut", ResourceType = typeof(ResCompanyConfig))]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTItemStockOut { get; set; }

        [Display(Name = "AEMTCycleCount", ResourceType = typeof(ResCompanyConfig))]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTCycleCount { get; set; }

        [Display(Name = "AEMTCycleCntItmMiss", ResourceType = typeof(ResCompanyConfig))]
        public string AEMTCycleCntItmMiss { get; set; }

        [Display(Name = "AEMTItemUsageRpt", ResourceType = typeof(ResCompanyConfig))]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTItemUsageRpt { get; set; }

        [Display(Name = "AEMTItemReceiveRpt", ResourceType = typeof(ResCompanyConfig))]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTItemReceiveRpt { get; set; }


        [Display(Name = "GridRefreshTimeInSecond", ResourceType = typeof(ResCompanyConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> GridRefreshTimeInSecond { get; set; }

        [Display(Name = "DateFormatCSharp", ResourceType = typeof(ResCompanyConfig))]
        public string DateFormatCSharp { get; set; }

        [Display(Name = "WeightDecimalPoints", ResourceType = typeof(ResCompanyConfig))]
        public Nullable<System.Double> WeightDecimalPoints { get; set; }

        [Display(Name = "TurnsAvgDecimalPoints", ResourceType = typeof(ResCompanyConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> TurnsAvgDecimalPoints { get; set; }

        [Display(Name = "IsPackSlipRequired", ResourceType = typeof(ResCompanyConfig))]
        public Nullable<Boolean> IsPackSlipRequired { get; set; }

        [Display(Name = "NumberOfBackDaysToSyncOverPDA", ResourceType = typeof(ResCompanyConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> NumberOfBackDaysToSyncOverPDA { get; set; }
    }

    public class ResCompanyConfig
    {
        private static string ResourceFileName = "ResCompanyConfig";


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
        ///   Looks up a localized string similar to CompanyConfig {0} already exist! Try with Another!.
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
        ///   Looks up a localized string similar to CompanyConfig.
        /// </summary>
        public static string CompanyConfigHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("CompanyConfigHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CompanyConfig.
        /// </summary>
        public static string CompanyConfig
        {
            get
            {
                return ResourceRead.GetResourceValue("CompanyConfig", ResourceFileName);
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
        ///   Looks up a localized string similar to ScheduleDaysBefore.
        /// </summary>
        public static string ScheduleDaysBefore
        {
            get
            {
                return ResourceRead.GetResourceValue("ScheduleDaysBefore", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to OperationalHoursBefore.
        /// </summary>
        public static string OperationalHoursBefore
        {
            get
            {
                return ResourceRead.GetResourceValue("OperationalHoursBefore", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MileageBefore.
        /// </summary>
        public static string MileageBefore
        {
            get
            {
                return ResourceRead.GetResourceValue("MileageBefore", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ProjectAmountExceed.
        /// </summary>
        public static string ProjectAmountExceed
        {
            get
            {
                return ResourceRead.GetResourceValue("ProjectAmountExceed", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ProjectItemQuantitExceed.
        /// </summary>
        public static string ProjectItemQuantitExceed
        {
            get
            {
                return ResourceRead.GetResourceValue("ProjectItemQuantitExceed", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ProjectItemAmountExceed.
        /// </summary>
        public static string ProjectItemAmountExceed
        {
            get
            {
                return ResourceRead.GetResourceValue("ProjectItemAmountExceed", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CostDecimalPoints.
        /// </summary>
        public static string CostDecimalPoints
        {
            get
            {
                return ResourceRead.GetResourceValue("CostDecimalPoints", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to QuantityDecimalPoints.
        /// </summary>
        public static string QuantityDecimalPoints
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityDecimalPoints", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to DateFormat.
        /// </summary>
        public static string DateFormat
        {
            get
            {
                return ResourceRead.GetResourceValue("DateFormat", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to NOBackDays.
        /// </summary>
        public static string NOBackDays
        {
            get
            {
                return ResourceRead.GetResourceValue("NOBackDays", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to NODaysAve.
        /// </summary>
        public static string NODaysAve
        {
            get
            {
                return ResourceRead.GetResourceValue("NODaysAve", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to NOTimes.
        /// </summary>
        public static string NOTimes
        {
            get
            {
                return ResourceRead.GetResourceValue("NOTimes", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MinPer.
        /// </summary>
        public static string MinPer
        {
            get
            {
                return ResourceRead.GetResourceValue("MinPer", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MaxPer.
        /// </summary>
        public static string MaxPer
        {
            get
            {
                return ResourceRead.GetResourceValue("MaxPer", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CurrencySymbol.
        /// </summary>
        public static string CurrencySymbol
        {
            get
            {
                return ResourceRead.GetResourceValue("CurrencySymbol", ResourceFileName);
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
        public static string AEMTPndOrders
        {
            get
            {
                return ResourceRead.GetResourceValue("AEMTPndOrders", ResourceFileName);
            }
        }
        public static string AEMTPndRequisition
        {
            get
            {
                return ResourceRead.GetResourceValue("AEMTPndRequisition", ResourceFileName);
            }
        }
        public static string AEMTPndTransfer
        {
            get
            {
                return ResourceRead.GetResourceValue("AEMTPndTransfer", ResourceFileName);
            }
        }
        public static string AEMTSggstOrdMin
        {
            get
            {
                return ResourceRead.GetResourceValue("AEMTSggstOrdMin", ResourceFileName);
            }
        }
        public static string AEMTSggstOrdCrt
        {
            get
            {
                return ResourceRead.GetResourceValue("AEMTSggstOrdCrt", ResourceFileName);
            }
        }
        public static string AEMTAssetMntDue
        {
            get
            {
                return ResourceRead.GetResourceValue("AEMTAssetMntDue", ResourceFileName);
            }
        }
        public static string AEMTToolsMntDue
        {
            get
            {
                return ResourceRead.GetResourceValue("AEMTToolsMntDue", ResourceFileName);
            }
        }
        public static string AEMTItemStockOut
        {
            get
            {
                return ResourceRead.GetResourceValue("AEMTItemStockOut", ResourceFileName);
            }
        }
        public static string AEMTCycleCount
        {
            get
            {
                return ResourceRead.GetResourceValue("AEMTCycleCount", ResourceFileName);
            }
        }
        public static string AEMTCycleCntItmMiss
        {
            get
            {
                return ResourceRead.GetResourceValue("AEMTCycleCntItmMiss", ResourceFileName);
            }
        }
        public static string AEMTItemUsageRpt
        {
            get
            {
                return ResourceRead.GetResourceValue("AEMTItemUsageRpt", ResourceFileName);
            }
        }
        public static string AEMTItemReceiveRpt
        {
            get
            {
                return ResourceRead.GetResourceValue("AEMTItemReceiveRpt", ResourceFileName);
            }
        }
        public static string GridRefreshTimeInSecond
        {
            get
            {
                return ResourceRead.GetResourceValue("GridRefreshTimeInSecond", ResourceFileName);
            }
        }
        public static string DateFormatCSharp
        {
            get
            {
                return ResourceRead.GetResourceValue("DateFormatCSharp", ResourceFileName);
            }
        }
        public static string WeightDecimalPoints
        {
            get
            {
                return ResourceRead.GetResourceValue("WeightDecimalPoints", ResourceFileName);
            }
        }
        public static string TurnsAvgDecimalPoints
        {
            get
            {
                return ResourceRead.GetResourceValue("TurnsAvgDecimalPoints", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsPackSlipRequired.
        /// </summary>
        public static string IsPackSlipRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("IsPackSlipRequired", ResourceFileName);
            }
        }
        public static string NumberOfBackDaysToSyncOverPDA
        {
            get
            { return ResourceRead.GetResourceValue("NumberOfBackDaysToSyncOverPDA", ResourceFileName); }
        }

    }

    public class GlobalSettingDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        //CompanyID
        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CompanyID { get; set; }

        //ScheduleDaysBefore
        [Display(Name = "ScheduleDaysBefore", ResourceType = typeof(ResCompanyConfig))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> ScheduleDaysBefore { get; set; }

        //OperationalHoursBefore
        [Display(Name = "OperationalHoursBefore", ResourceType = typeof(ResCompanyConfig))]
        public Nullable<System.Double> OperationalHoursBefore { get; set; }

        //MileageBefore
        [Display(Name = "MileageBefore", ResourceType = typeof(ResCompanyConfig))]
        public Nullable<System.Double> MileageBefore { get; set; }

        //ProjectAmountExceed
        [Display(Name = "ProjectAmountExceed", ResourceType = typeof(ResCompanyConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> ProjectAmountExceed { get; set; }

        //ProjectItemQuantitExceed
        [Display(Name = "ProjectItemQuantitExceed", ResourceType = typeof(ResCompanyConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> ProjectItemQuantitExceed { get; set; }

        //ProjectItemAmountExceed
        [Display(Name = "ProjectItemAmountExceed", ResourceType = typeof(ResCompanyConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> ProjectItemAmountExceed { get; set; }

        //CostDecimalPoints
        [Display(Name = "CostDecimalPoints", ResourceType = typeof(ResCompanyConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> CostDecimalPoints { get; set; }

        //QuantityDecimalPoints
        [Display(Name = "QuantityDecimalPoints", ResourceType = typeof(ResCompanyConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> QuantityDecimalPoints { get; set; }

        //DateFormat
        [Display(Name = "DateFormat", ResourceType = typeof(ResCompanyConfig))]
        [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String DateFormat { get; set; }

        //NOBackDays
        [Display(Name = "NOBackDays", ResourceType = typeof(ResCompanyConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> NOBackDays { get; set; }

        //NODaysAve
        [Display(Name = "NODaysAve", ResourceType = typeof(ResCompanyConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> NODaysAve { get; set; }

        //NOTimes
        [Display(Name = "NOTimes", ResourceType = typeof(ResCompanyConfig))]
        public Nullable<System.Decimal> NOTimes { get; set; }

        //MinPer
        [Display(Name = "MinPer", ResourceType = typeof(ResCompanyConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> MinPer { get; set; }

        //MaxPer
        [Display(Name = "MaxPer", ResourceType = typeof(ResCompanyConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> MaxPer { get; set; }

        //CurrencySymbol
        [Display(Name = "CurrencySymbol", ResourceType = typeof(ResCompanyConfig))]
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

        [Display(Name = "AEMTPndOrders", ResourceType = typeof(ResCompanyConfig))]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTPndOrders { get; set; }

        [Display(Name = "AEMTPndRequisition", ResourceType = typeof(ResCompanyConfig))]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTPndRequisition { get; set; }

        [Display(Name = "AEMTPndTransfer", ResourceType = typeof(ResCompanyConfig))]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTPndTransfer { get; set; }

        [Display(Name = "AEMTSggstOrdMin", ResourceType = typeof(ResCompanyConfig))]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTSggstOrdMin { get; set; }

        [Display(Name = "AEMTSggstOrdCrt", ResourceType = typeof(ResCompanyConfig))]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTSggstOrdCrt { get; set; }

        [Display(Name = "AEMTAssetMntDue", ResourceType = typeof(ResCompanyConfig))]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTAssetMntDue { get; set; }

        [Display(Name = "AEMTToolsMntDue", ResourceType = typeof(ResCompanyConfig))]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTToolsMntDue { get; set; }

        [Display(Name = "AEMTItemStockOut", ResourceType = typeof(ResCompanyConfig))]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTItemStockOut { get; set; }

        [Display(Name = "AEMTCycleCount", ResourceType = typeof(ResCompanyConfig))]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTCycleCount { get; set; }

        [Display(Name = "AEMTCycleCntItmMiss", ResourceType = typeof(ResCompanyConfig))]
        public string AEMTCycleCntItmMiss { get; set; }

        [Display(Name = "AEMTItemUsageRpt", ResourceType = typeof(ResCompanyConfig))]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTItemUsageRpt { get; set; }

        [Display(Name = "AEMTItemReceiveRpt", ResourceType = typeof(ResCompanyConfig))]
        [RegularExpression(@"^([0-2][0-9]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string AEMTItemReceiveRpt { get; set; }


        [Display(Name = "GridRefreshTimeInSecond", ResourceType = typeof(ResCompanyConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> GridRefreshTimeInSecond { get; set; }

        [Display(Name = "DateFormatCSharp", ResourceType = typeof(ResCompanyConfig))]
        public string DateFormatCSharp { get; set; }

        [Display(Name = "WeightDecimalPoints", ResourceType = typeof(ResCompanyConfig))]
        public Nullable<System.Double> WeightDecimalPoints { get; set; }

        [Display(Name = "TurnsAvgDecimalPoints", ResourceType = typeof(ResCompanyConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> TurnsAvgDecimalPoints { get; set; }

        [Display(Name = "IsPackSlipRequired", ResourceType = typeof(ResCompanyConfig))]
        public Nullable<Boolean> IsPackSlipRequired { get; set; }

        [Display(Name = "NumberOfBackDaysToSyncOverPDA", ResourceType = typeof(ResCompanyConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> NumberOfBackDaysToSyncOverPDA { get; set; }
    }
}



