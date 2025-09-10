using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;

namespace eTurns.DTO
{
    [Serializable]
    public class eTurnsRegionInfo
    {

        public long? ID { get; set; }

        [Required]
        [Display(Name = "CultureCode", ResourceType = typeof(ResRegionalSettings))]
        public string CultureCode { get; set; }

        [Display(Name = "CultureName", ResourceType = typeof(ResRegionalSettings))]
        public string CultureName { get; set; }

        [Display(Name = "CultureDisplayName", ResourceType = typeof(ResRegionalSettings))]
        public string CultureDisplayName { get; set; }

        [Required]
        [Display(Name = "ShortDatePattern", ResourceType = typeof(ResRegionalSettings))]
        public string ShortDatePattern { get; set; }

        [Display(Name = "LongDatePattern", ResourceType = typeof(ResRegionalSettings))]
        public string LongDatePattern { get; set; }

        [Required]
        [Display(Name = "ShortTimePattern", ResourceType = typeof(ResRegionalSettings))]
        public string ShortTimePattern { get; set; }

        [Display(Name = "LongTimePattern", ResourceType = typeof(ResRegionalSettings))]
        public string LongTimePattern { get; set; }

        [Display(Name = "NumberDecimalDigits", ResourceType = typeof(ResRegionalSettings))]
        public int NumberDecimalDigits { get; set; }

        [Display(Name = "NumberGroupSeparator", ResourceType = typeof(ResRegionalSettings))]
        public string NumberGroupSeparator { get; set; }

        [Display(Name = "CurrencyDecimalDigits", ResourceType = typeof(ResRegionalSettings))]
        public int CurrencyDecimalDigits { get; set; }

        [Display(Name = "NumberDecimalSeparator", ResourceType = typeof(ResRegionalSettings))]
        public string NumberDecimalSeparator { get; set; }

        [Display(Name = "CurrencyGroupSeparator", ResourceType = typeof(ResRegionalSettings))]
        public string CurrencyGroupSeparator { get; set; }

        [Required]
        [Display(Name = "TimeZoneName", ResourceType = typeof(ResRegionalSettings))]
        public string TimeZoneName { get; set; }
        public int TimeZoneOffSet { get; set; }

        public long CompanyId { get; set; }
        public long EnterpriseId { get; set; }
        public long RoomId { get; set; }
        public long UserId { get; set; }
        public List<string> lstLongDatePatterns { get; set; }
        public List<string> lstLongTimePatterns { get; set; }
        public List<string> lstShortTimePatterns { get; set; }
        public List<string> lstShortDatePatterns { get; set; }

        //CurrencySymbol
        [Display(Name = "CurrencySymbol", ResourceType = typeof(ResCompanyConfig))]
        [StringLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String CurrencySymbol { get; set; }

        [Display(Name = "GridRefreshTimeInSecond", ResourceType = typeof(ResCompanyConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> GridRefreshTimeInSecond { get; set; }

        [Display(Name = "WeightDecimalPoints", ResourceType = typeof(ResCompanyConfig))]
        public Nullable<System.Double> WeightDecimalPoints { get; set; }

        [Display(Name = "TurnsAvgDecimalPoints", ResourceType = typeof(ResCompanyConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> TurnsAvgDecimalPoints { get; set; }

        [Display(Name = "NumberOfBackDaysToSyncOverPDA", ResourceType = typeof(ResCompanyConfig))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> NumberOfBackDaysToSyncOverPDA { get; set; }
        public bool TZSupportDayLight { get; set; }
        public DateTime? DayLightStartTime { get; set; }
        public DateTime? DayLightEndTime { get; set; }
        public DateTime? TZDateTimeNow { get; set; }
        public List<RegionInfo> lstCurrencySymbol { get; set; }
    }
    public class RegionLanguage
    {
        public string CultureCode { get; set; }
        public string CultureName { get; set; }
        public string CultureDisplayName { get; set; }

    }

    public class ResRegionalSettings
    {
        private static string ResourceFileName = "ResRegionalSettings";

        /// <summary>
        ///   Looks up a localized string similar to Action.
        /// </summary>
        public static string Action
        {
            get
            {
                return ResourceHelper.GetResourceValue("Action", ResourceFileName);
            }
        }


        public static string CultureCode
        {
            get
            {
                return ResourceHelper.GetResourceValue("CultureCode", ResourceFileName);
            }
        }


        public static string CultureName
        {
            get
            {
                return ResourceHelper.GetResourceValue("CultureName", ResourceFileName);
            }
        }


        public static string CultureDisplayName
        {
            get
            {
                return ResourceHelper.GetResourceValue("CultureDisplayName", ResourceFileName);
            }
        }

        public static string ShortDatePattern
        {
            get
            {
                return ResourceHelper.GetResourceValue("ShortDatePattern", ResourceFileName);
            }
        }

        public static string LongDatePattern
        {
            get
            {
                return ResourceHelper.GetResourceValue("LongDatePattern", ResourceFileName);
            }
        }

        public static string ShortTimePattern
        {
            get
            {
                return ResourceHelper.GetResourceValue("ShortTimePattern", ResourceFileName);
            }
        }

        public static string LongTimePattern
        {
            get
            {
                return ResourceHelper.GetResourceValue("LongTimePattern", ResourceFileName);
            }
        }

        public static string NumberDecimalDigits
        {
            get
            {
                return ResourceHelper.GetResourceValue("NumberDecimalDigits", ResourceFileName);
            }
        }

        public static string NumberGroupSeparator
        {
            get
            {
                return ResourceHelper.GetResourceValue("NumberGroupSeparator", ResourceFileName);
            }
        }

        public static string CurrencyDecimalDigits
        {
            get
            {
                return ResourceHelper.GetResourceValue("CurrencyDecimalDigits", ResourceFileName);
            }
        }

        public static string NumberDecimalSeparator
        {
            get
            {
                return ResourceHelper.GetResourceValue("NumberDecimalSeparator", ResourceFileName);
            }
        }

        public static string CurrencyGroupSeparator
        {
            get
            {
                return ResourceHelper.GetResourceValue("CurrencyGroupSeparator", ResourceFileName);
            }
        }

        public static string TimeZoneName
        {
            get
            {
                return ResourceHelper.GetResourceValue("TimeZoneName", ResourceFileName);
            }
        }
    }

}
