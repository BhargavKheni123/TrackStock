using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DTO
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

    public class ABOrderDTO
    {
        public List<Order> orders { get; set; }
        public string nextPageToken { get; set; }
        public int size { get; set; }
    }

    public class BuyingCustomer
    {
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
    }

    public class ItemNetTotal
    {
        public string currencyCode { get; set; }
        public string amount { get; set; }
    }

    public class ItemPromotion
    {
        public string currencyCode { get; set; }
        public string amount { get; set; }
    }

    public class ItemShippingAndHandling
    {
        public string currencyCode { get; set; }
        public string amount { get; set; }
    }

    public class ItemSubTotal
    {
        public string currencyCode { get; set; }
        public string amount { get; set; }
    }

    public class ItemTax
    {
        public string currencyCode { get; set; }
        public string amount { get; set; }
    }

    public class LineItem
    {
        public string productCategory { get; set; }
        public string asin { get; set; }
        public string title { get; set; }
        public string unspsc { get; set; }
        public string productCondition { get; set; }
        public ListedPricePerUnit listedPricePerUnit { get; set; }
        public PurchasedPricePerUnit purchasedPricePerUnit { get; set; }
        public int itemQuantity { get; set; }
        public ItemSubTotal itemSubTotal { get; set; }
        public ItemShippingAndHandling itemShippingAndHandling { get; set; }
        public ItemPromotion itemPromotion { get; set; }
        public ItemTax itemTax { get; set; }
        public ItemNetTotal itemNetTotal { get; set; }
        public object purchaseOrderLineItem { get; set; }
        public object taxExemptionApplied { get; set; }
        public object taxExemptionType { get; set; }
        public object taxExemptOptOut { get; set; }
        public string discountProgram { get; set; }
        public object discountType { get; set; }
        public object discountAmount { get; set; }
        public object discountRatio { get; set; }
        public Seller seller { get; set; }
        public List<object> sellerCredentials { get; set; }
        public string brandCode { get; set; }
        public string brandName { get; set; }
        public string manufacturerName { get; set; }
    }

    public class ListedPricePerUnit
    {
        public string currencyCode { get; set; }
        public string amount { get; set; }
    }

    public class Order
    {
        public DateTime orderDate { get; set; }
        public string orderId { get; set; }
        public string purchaseOrderNumber { get; set; }
        public int orderQuantity { get; set; }
        public string orderStatus { get; set; }
        public object lastOrderApproverName { get; set; }
        public BuyingCustomer buyingCustomer { get; set; }
        public string buyerGroupName { get; set; }
        public OrderSubTotal orderSubTotal { get; set; }
        public OrderShippingAndHandling orderShippingAndHandling { get; set; }
        public OrderPromotion orderPromotion { get; set; }
        public OrderTax orderTax { get; set; }
        public OrderNetTotal orderNetTotal { get; set; }
        public List<LineItem> lineItems { get; set; }
        public List<object> shipments { get; set; }
        public List<object> charges { get; set; }
    }

    public class OrderNetTotal
    {
        public string currencyCode { get; set; }
        public string amount { get; set; }
    }

    public class OrderPromotion
    {
        public string currencyCode { get; set; }
        public string amount { get; set; }
    }

    public class OrderShippingAndHandling
    {
        public string currencyCode { get; set; }
        public string amount { get; set; }
    }

    public class OrderSubTotal
    {
        public string currencyCode { get; set; }
        public string amount { get; set; }
    }

    public class OrderTax
    {
        public string currencyCode { get; set; }
        public string amount { get; set; }
    }

    public class PurchasedPricePerUnit
    {
        public string currencyCode { get; set; }
        public string amount { get; set; }
    }

    

    public class Seller
    {
        public string sellerName { get; set; }
        public string sellerCity { get; set; }
        public string sellerState { get; set; }
        public string sellerPostalCode { get; set; }
    }

    public enum ABOrderSyncMode: byte
    {
        Online = 1,
        Offline = 2
    }
}
