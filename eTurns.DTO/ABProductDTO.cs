using eTurns.DTO.Resources;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace eTurns.DTO
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class ProductSearchResultDTO
    {
        public int matchingProductCount { get; set; }
        public List<SearchRefinement> searchRefinements { get; set; }
        public Refinements refinements { get; set; }
        public int numberOfPages { get; set; }
        public List<Product> products { get; set; }
        public int pageSize { get; set; }
        public string RegionCurrencySymbol { get; set; }

        public List<string> RoomItemsASIN { get; set; }
    }

    public class AvailabilityOption
    {
        public string displayName { get; set; }
        public string id { get; set; }
    }

    public class BookInformation
    {
        public Isbn isbn { get; set; }
        public object publicationDate { get; set; }
        public object publishedLanguage { get; set; }
    }

    public class Category
    {
        public string displayName { get; set; }
        public string id { get; set; }
    }

    public class Condition
    {
        public string conditionValue { get; set; }
        public string conditionNote { get; set; }
        public string subCondition { get; set; }
    }

    public class CustomerReviewsSummary
    {
        public object numberOfReviews { get; set; }
        public object starRating { get; set; }
    }

    public class DeliveryDayOption
    {
        public string displayName { get; set; }
        public string id { get; set; }
    }

    public class Dimension
    {
        public int index { get; set; }
        public string displayString { get; set; }
        public List<DimensionValue> dimensionValues { get; set; }
    }

    public class DimensionValue
    {
        public int index { get; set; }
        public string displayString { get; set; }
    }

    public class EligibleForFreeShippingOption
    {
        public string displayName { get; set; }
        public string id { get; set; }
    }

    public class IMAGE
    {
        public string altText { get; set; }
        public Large large { get; set; }
        public Medium medium { get; set; }
        public Small small { get; set; }
        public Thumbnail thumbnail { get; set; }
    }

    public class IncludedDataTypes
    {
        public List<OFFER> OFFERS { get; set; }
        public List<IMAGE> IMAGES { get; set; }
    }

    public class Isbn
    {
        public object isbn10 { get; set; }
        public object isbn13 { get; set; }
    }

    public class Large
    {
        public int height { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

    public class ListPrice
    {
        public Value value { get; set; }
        public object formattedPrice { get; set; }
        public string priceType { get; set; }
    }

    public class MediaInformation
    {
        public List<object> editions { get; set; }
        public List<object> mediaFormats { get; set; }
    }

    public class Medium
    {
        public int height { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

    public class Merchant
    {
        public object merchantId { get; set; }
        public string name { get; set; }
        public object meanFeedbackRating { get; set; }
        public object totalFeedbackCount { get; set; }
    }

    public class OFFER
    {
        public string availability { get; set; }
        public string buyingGuidance { get; set; }
        public List<object> buyingRestrictions { get; set; }
        public string fulfillmentType { get; set; }
        public Merchant merchant { get; set; }
        public string offerId { get; set; }
        public Price price { get; set; }
        public ListPrice listPrice { get; set; }
        public string productCondition { get; set; }
        public string productConditionNote { get; set; }
        public Condition condition { get; set; }
        public QuantityLimits quantityLimits { get; set; }
        public QuantityPrice quantityPrice { get; set; }
        public TaxExclusivePrice taxExclusivePrice { get; set; }
        public string deliveryInformation { get; set; }
    }

    public class Price
    {
        public Value value { get; set; }
        public object formattedPrice { get; set; }
        public string priceType { get; set; }
    }

    public class PrimeEligible
    {
        public string displayName { get; set; }
        public string id { get; set; }
    }

    public class Product
    {
        public string asin { get; set; }
        public string asinType { get; set; }
        public string signedProductId { get; set; }
        public IncludedDataTypes includedDataTypes { get; set; }
        public List<string> features { get; set; }
        public List<object> editorialReviews { get; set; }
        public List<Taxonomy> taxonomies { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public object format { get; set; }
        public BookInformation bookInformation { get; set; }
        public List<object> byLine { get; set; }
        public MediaInformation mediaInformation { get; set; }
        public Dictionary<string, string> productOverview { get; set; }
        public Dictionary<string, Dictionary<string, string>> productDetails { get; set; }
        public ProductVariations productVariations { get; set; }
        public CustomerReviewsSummary customerReviewsSummary { get; set; }
    }

    public class ProductDetails
    {
    }

    public class ProductOverview
    {
        [JsonProperty("Package Dimensions")]
        public string PackageDimensions { get; set; }

        [JsonProperty("Item model number")]
        public string ItemModelNumber { get; set; }
        public string Manufacturer { get; set; }
        public string UPC { get; set; }

        [JsonProperty("Product Dimensions")]
        public string ProductDimensions { get; set; }
    }

    public class ProductVariations
    {
        public List<Dimension> dimensions { get; set; }
        public List<Variation> variations { get; set; }
    }

    public class QuantityLimits
    {
        public int? maxQuantity { get; set; }
        public int? minQuantity { get; set; }
    }

    public class QuantityPrice
    {
        public List<QuantityPriceTier> quantityPriceTiers { get; set; }
    }

    public class QuantityPriceTier
    {
        public string quantityDisplay { get; set; }
        public UnitPrice unitPrice { get; set; }
        public object minQuantity { get; set; }
        public object price { get; set; }
        public object savingMessage { get; set; }
        public object taxExclusivePrice { get; set; }
    }

    public class Refinements
    {
        public List<Category> categories { get; set; }
        public List<object> subCategories { get; set; }
        public List<AvailabilityOption> availabilityOptions { get; set; }
        public List<DeliveryDayOption> deliveryDayOptions { get; set; }
        public List<EligibleForFreeShippingOption> eligibleForFreeShippingOptions { get; set; }
        public List<PrimeEligible> primeEligible { get; set; }
    }

    public class RefinementValue
    {
        public string displayName { get; set; }
        public string searchRefinementValue { get; set; }
    }



    public class SearchRefinement
    {
        public string selectionType { get; set; }
        public string displayValue { get; set; }
        public List<RefinementValue> refinementValues { get; set; }
    }

    public class Small
    {
        public int height { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

    public class TaxExclusivePrice
    {
        public object taxExclusiveAmount { get; set; }
        public string displayString { get; set; }
        public string formattedPrice { get; set; }
        public string label { get; set; }
    }

    public class Taxonomy
    {
        public string taxonomyCode { get; set; }
        public string title { get; set; }
        public string type { get; set; }
    }

    public class Thumbnail
    {
        public int height { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

    public class UnitPrice
    {
        public double amount { get; set; }
        public string currencyCode { get; set; }
    }

    public class Value
    {
        public double amount { get; set; }
        public string currencyCode { get; set; }
    }

    public class Variation
    {
        public string asin { get; set; }
        public List<VariationValue> variationValues { get; set; }
    }

    public class VariationValue
    {
        public int index { get; set; }
        public int value { get; set; }
    }
    public class ByLine
    {
        public string name { get; set; }
        public List<string> roles { get; set; }
    }
    public class ResABProducts
    {
        private static string ResourceFileName = "ResABProducts";
        public static string PageTitleListPage
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitleListPage", ResourceFileName);
            }
        }
        public static string PageTitleGridPage
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitleGridPage", ResourceFileName);
            }
        }
        public static string PageTitleDetailPage
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitleDetailPage", ResourceFileName);
            }
        }
        public static string Search
        {
            get
            {
                return ResourceRead.GetResourceValue("Search", ResourceFileName);
            }
        }
        public static string ListPrice
        {
            get
            {
                return ResourceRead.GetResourceValue("ListPrice", ResourceFileName);
            }
        }
        public static string Price
        {
            get
            {
                return ResourceRead.GetResourceValue("Price", ResourceFileName);
            }
        }
        public static string YouSave
        {
            get
            {
                return ResourceRead.GetResourceValue("YouSave", ResourceFileName);
            }
        }
        public static string CurrentlyUnavailable
        {
            get
            {
                return ResourceRead.GetResourceValue("CurrentlyUnavailable", ResourceFileName);
            }
        }
        public static string ItemModelNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemModelNumber", ResourceFileName);
            }
        }
        public static string Manufacturer
        {
            get
            {
                return ResourceRead.GetResourceValue("Manufacturer", ResourceFileName);
            }
        }
        public static string ProductDimensions
        {
            get
            {
                return ResourceRead.GetResourceValue("ProductDimensions", ResourceFileName);
            }
        }

        public static string Aboutthisitem
        {
            get
            {
                return ResourceRead.GetResourceValue("Aboutthisitem", ResourceFileName);
            }
        }

        public static string addtoroom
        {
            get
            {
                return ResourceRead.GetResourceValue("addtoroom", ResourceFileName);
            }
        }

        public static string ItemaddtoroomSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemaddtoroomSuccessfully", ResourceFileName);
            }
        }

        public static string ItemaddtoroomFail
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemaddtoroomFail", ResourceFileName);
            }
        }
        public static string AsinAlreadyExist
        {
            get
            {
                return ResourceRead.GetResourceValue("AsinAlreadyExist", ResourceFileName);
            }
        }
        public static string ASINNotAvailableinResponse
        {
            get
            {
                return ResourceRead.GetResourceValue("ASINNotAvailableinResponse", ResourceFileName);
            }
        }

        public static string Results { get { return ResourceRead.GetResourceValue("Results", ResourceFileName); } }
        public static string Availability { get { return ResourceRead.GetResourceValue("Availability", ResourceFileName); } }
        public static string IncludeOutofStock { get { return ResourceRead.GetResourceValue("IncludeOutofStock", ResourceFileName); } }
        public static string InStockOnly { get { return ResourceRead.GetResourceValue("InStockOnly", ResourceFileName); } }
        public static string GetItByTommorrow { get { return ResourceRead.GetResourceValue("GetItByTommorrow", ResourceFileName); } }
        public static string GetItByToday { get { return ResourceRead.GetResourceValue("GetItByToday", ResourceFileName); } }
        public static string DeliveryDay { get { return ResourceRead.GetResourceValue("DeliveryDay", ResourceFileName); } }
        public static string EligibleForFreeShipping { get { return ResourceRead.GetResourceValue("EligibleForFreeShipping", ResourceFileName); } }
        public static string EligibleForPrime { get { return ResourceRead.GetResourceValue("EligibleForPrime", ResourceFileName); } }
        public static string Categories { get { return ResourceRead.GetResourceValue("Categories", ResourceFileName); } }
        public static string AnyCategory { get { return ResourceRead.GetResourceValue("AnyCategory", ResourceFileName); } }
        public static string RefineBy { get { return ResourceRead.GetResourceValue("RefineBy", ResourceFileName); } }
        public static string List { get { return ResourceRead.GetResourceValue("List", ResourceFileName); } }
        public static string Grid { get { return ResourceRead.GetResourceValue("Grid", ResourceFileName); } }
        public static string Back { get { return ResourceRead.GetResourceValue("Back", ResourceFileName); } }
        public static string addtocart
        {
            get
            {
                return ResourceRead.GetResourceValue("addtocart", ResourceFileName);
            }
        }

        public static string CartQuantity { get { return ResourceRead.GetResourceValue("CartQuantity", ResourceFileName); } }
        public static string CartQuantityMustBeGreaterThanZero { get { return ResourceRead.GetResourceValue("CartQuantityMustBeGreaterThanZero", ResourceFileName); } }
        public static string ItemAddedToCartSuccessfully { get { return ResourceRead.GetResourceValue("ItemAddedToCartSuccessfully", ResourceFileName); } }
        public static string FailToAddItemToCart { get { return ResourceRead.GetResourceValue("FailToAddItemToCart", ResourceFileName); } }
        public static string ProductQuantityPopUpTitle { get { return ResourceRead.GetResourceValue("ProductQuantityPopUpTitle", ResourceFileName); } }
        public static string ItemSync
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemSync", ResourceFileName);
            }
        }

        public static string SelectPageSize { get { return ResourceRead.GetResourceValue("SelectPageSize", ResourceFileName); } }
        public static string ProductSearch { get { return ResourceRead.GetResourceValue("ProductSearch", ResourceFileName); } }
        public static string ItemsSyncSuccessfully { get { return ResourceRead.GetResourceValue("ItemsSyncSuccessfully", ResourceFileName); } }
        public static string FailToSyncItems { get { return ResourceRead.GetResourceValue("FailToSyncItems", ResourceFileName); } }
        public static string PageTitleSyncABOrderedItems { get { return ResourceRead.GetResourceValue("PageTitleSyncABOrderedItems", ResourceFileName); } }
        public static string PageHeaderSyncABOrderedItems { get { return ResourceRead.GetResourceValue("PageHeaderSyncABOrderedItems", ResourceFileName); } }
        public static string Brand { get { return ResourceRead.GetResourceValue("Brand", ResourceFileName); } }

        public static string ratings { get { return ResourceRead.GetResourceValue("ratings", ResourceFileName); } }
        public static string ratingsoutFive { get { return ResourceRead.GetResourceValue("ratingsoutFive", ResourceFileName); } }
        public static string globalratings { get { return ResourceRead.GetResourceValue("globalratings", ResourceFileName); } }
        public static string SoldBy { get { return ResourceRead.GetResourceValue("SoldBy", ResourceFileName); } }
        public static string Fulfilledbyamazon { get { return ResourceRead.GetResourceValue("Fulfilledbyamazon", ResourceFileName); } }
        public static string ProductSpecifications { get { return ResourceRead.GetResourceValue("ProductSpecifications", ResourceFileName); } }
        public static string Condition { get { return ResourceRead.GetResourceValue("Condition", ResourceFileName); } }
        public static string ProductConditionNewandOld { get { return ResourceRead.GetResourceValue("ProductConditionNewandOld", ResourceFileName); } }
        public static string ProductConditionNew { get { return ResourceRead.GetResourceValue("ProductConditionNew", ResourceFileName); } }
        public static string ProductConditionOld { get { return ResourceRead.GetResourceValue("ProductConditionOld", ResourceFileName); } }
        public static string from { get { return ResourceRead.GetResourceValue("from", ResourceFileName); } }
        public static string MoreBuyingChoices { get { return ResourceRead.GetResourceValue("MoreBuyingChoices", ResourceFileName); } }
        public static string OtherSellersonAmazon { get { return ResourceRead.GetResourceValue("OtherSellersonAmazon", ResourceFileName); } }

    }
}