using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DTO
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        
    public class ABProductDetails
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
        public List<ByLine> byLine { get; set; }
        public MediaInformation mediaInformation { get; set; }
        public Dictionary<string,string> productOverview { get; set; }
        public Dictionary<string, Dictionary<string, string>> productDetails { get; set; }
        public ProductVariations productVariations { get; set; }
        public CustomerReviewsSummary customerReviewsSummary { get; set; }
        public List<VariationWithPrice> variationWithPrice { get; set; }

        public int offerCount { get; set; }
        public int numberOfPages { get; set; }
        public FeaturedOffer featuredOffer { get; set; }
        public List<Offer> offers { get; set; }
        public List<FilterGroup> filterGroups { get; set; }
    }
    public class VariationWithPrice
    {
        public string asin { get; set; }
        public Value variationPrice { get; set; }
    }
}
