using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DTO
{
    public class ABRoomStoreSettingDTO
    {
        public long ID { get; set; }
        public long EnterpriseID { get; set; }
        public long CompanyID { get; set; }
        public long RoomID { get; set; }
        [Display(Name = "ABLocale", ResourceType = typeof(ResABRoomStoreSetting))]
        public string ABLocale { get; set; }
        [Display(Name = "ABCountryCode", ResourceType = typeof(ResABRoomStoreSetting))]
        public string ABCountryCode { get; set; }

        [Display(Name = "AuthCode", ResourceType = typeof(ResABRoomStoreSetting))]
        public string AuthCode { get; set; }

        [Display(Name = "EmailAddress", ResourceType = typeof(ResABRoomStoreSetting))]
        public string EmailAddress { get; set; }

        [Display(Name = "ProductSearchFacets", ResourceType = typeof(ResABRoomStoreSetting))]
        public string ProductSearchFacets { get; set; }

        [Display(Name = "MarketplaceID", ResourceType = typeof(ResABRoomStoreSetting))]
        public string MarketplaceID { get; set; }

        public string ABCountryCodeMarketplaceID
        {
            get
            {
                return ABCountryCode + "||" + MarketplaceID;
            }
        }

        public bool? IsSuccessDisplay { get; set; }

        [Display(Name = "CreatedOn", ResourceType = typeof(ResCommon))]
        public DateTime Created { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(ResCommon))]
        public DateTime Updated { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }



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
        public string ActionButton { get; set; }

        [Display(Name = "ProductSearchPageSize", ResourceType = typeof(ResABRoomStoreSetting))]
        public int ProductSearchPageSize { get; set; }

        public string RegionCurrencyCode { get; set; }

        public string RegionCurrencySymbol { get; set; }

        [Display(Name = "AbSupplierName", ResourceType = typeof(ResABRoomStoreSetting))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string AbSupplierName { get; set; }

        [Display(Name = "ProductVariantLimit", ResourceType = typeof(ResABRoomStoreSetting))]
        [Range(0, 10, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public int ProductVariantLimit { get; set; }

        [Display(Name = "ABSystemID", ResourceType = typeof(ResABRoomStoreSetting))]
        public string ABSystemID { get; set; }

        [Display(Name = "ABSystemPassword", ResourceType = typeof(ResABRoomStoreSetting))]
        public string ABSystemPassword { get; set; }

        [Display(Name = "ItemUDFforSeller", ResourceType = typeof(ResABRoomStoreSetting))]
        public string ItemUDFforSeller { get; set; }

        [Display(Name = "AmazonOrderEndpoint", ResourceType = typeof(ResABRoomStoreSetting))]
        public string AmazonOrderEndpoint { get; set; }


    }
    public class ResABRoomStoreSetting
    {
        private static string resourceFile = "ResABRoomStoreSetting";
        public static string PageTitle { get { return ResourceRead.GetResourceValue("PageTitle", resourceFile); } }
        public static string PageHeader { get { return ResourceRead.GetResourceValue("PageHeader", resourceFile); } }
        public static string ABLocale { get { return ResourceRead.GetResourceValue("ABLocale", resourceFile); } }
        public static string ABCountryCode { get { return ResourceRead.GetResourceValue("ABCountryCode", resourceFile); } }
        public static string AuthCode { get { return ResourceRead.GetResourceValue("AuthCode", resourceFile); } }
        public static string ConnectCTAButton { get { return ResourceRead.GetResourceValue("ConnectCTAButton", resourceFile); } }
        public static string ReConnectCTAButton { get { return ResourceRead.GetResourceValue("ReConnectCTAButton", resourceFile); } }
        public static string EmailAddress { get { return ResourceRead.GetResourceValue("EmailAddress", resourceFile); } }
        public static string ProductSearchFacets { get { return ResourceRead.GetResourceValue("ProductSearchFacets", resourceFile); } }
        public static string CTAButtonLabel { get { return ResourceRead.GetResourceValue("CTAButtonLabel", resourceFile); } }
        public static string MarketplaceID { get { return ResourceRead.GetResourceValue("MarketplaceID", resourceFile); } }
        public static string ProductSearchPageSize { get { return ResourceRead.GetResourceValue("ProductSearchPageSize", resourceFile); } }
        public static string AbSupplierName { get { return ResourceRead.GetResourceValue("AbSupplierName", resourceFile); } }
        public static string ProductVariantLimit { get { return ResourceRead.GetResourceValue("ProductVariantLimit", resourceFile); } }
        public static string ABSystemID { get { return ResourceRead.GetResourceValue("ABSystemID", resourceFile); } }
        public static string ABSystemPassword { get { return ResourceRead.GetResourceValue("ABSystemPassword", resourceFile); } }
        public static string ItemUDFforSeller { get { return ResourceRead.GetResourceValue("ItemUDFforSeller", resourceFile); } }
        public static string PurchasingCred { get { return ResourceRead.GetResourceValue("PurchasingCred", resourceFile); } }
        public static string DisconnectABAccount { get { return ResourceRead.GetResourceValue("DisconnectABAccount", resourceFile); } }
        public static string AmazonAbConnectHeader { get { return ResourceRead.GetResourceValue("AmazonAbConnectHeader", resourceFile); } }
        public static string BlockPopupWarnning { get { return ResourceRead.GetResourceValue("BlockPopupWarnning", resourceFile); } }
        public static string AamzonBussinessEmailWarnning { get { return ResourceRead.GetResourceValue("AamzonBussinessEmailWarnning", resourceFile); } }
        public static string OtherSettings { get { return ResourceRead.GetResourceValue("OtherSettings", resourceFile); } }
        public static string AmazonOrderEndpoint { get { return ResourceRead.GetResourceValue("AmazonOrderEndpoint", resourceFile); } }
    }
}
