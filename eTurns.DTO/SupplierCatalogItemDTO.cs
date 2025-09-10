using System;

namespace eTurns.DTO
{
    public class SupplierCatalogItemDTO
    {
        public string ActionButton { get; set; }
        public Int64 SupplierCatalogItemID { get; set; }
        public string ItemNumber { get; set; }
        public string UPC { get; set; }
        public string Description { get; set; }
        public double? SellPrice { get; set; }

        public double? PackingQantity { get; set; }
        public string ManufacturerName { get; set; }
        public string ManufacturerPartNumber { get; set; }
        public string SupplierName { get; set; }
        public string SupplierPartNumber { get; set; }
        public string ImagePath { get; set; }
        public string SourcePageName { get; set; }
        public Int64 SupplierID { get; set; }
        public Int64? ManufacturerID { get; set; }
        public double Quantity { get; set; }
        public string inputQuantity { get; set; }
        public string DestinationModule { get; set; }
        public bool OpenPopup { get; set; }
        public string ButtonText { get; set; }
        public Guid OrderGUID { get; set; }
        public string OrderSupplier { get; set; }

        public string UOM { get; set; }
        public string CostUOM { get; set; }

        public double? Cost { get; set; }
        public string UNSPSC { get; set; }
        public string LongDescription { get; set; }
        public string Category { get; set; }
        public long? CategoryID { get; set; }
    }

}


