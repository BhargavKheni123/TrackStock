using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DTO
{
    public class ABProductOffers
    {
        public int offerCount { get; set; }
        public int numberOfPages { get; set; }
        public FeaturedOffer featuredOffer { get; set; }
        public List<Offer> offers { get; set; }
        public List<FilterGroup> filterGroups { get; set; }
    }
    public class FeaturedOffer
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

    public class Filter
    {
        public string id { get; set; }
        public string displayName { get; set; }
    }

    public class FilterGroup
    {
        public string displayName { get; set; }
        public List<Filter> filters { get; set; }
    }
    public class Offer
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


}
