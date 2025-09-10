namespace eTurns.DTO.LabelPrinting
{
    public class InventoryLabelDTO
    {
        public string ItemNumber { get; set; }
        public string BinNumber { get; set; }
        public string ManufacturerNumber { get; set; }
        public string SupplierNumber { get; set; }
        public string UPCNumber { get; set; }
        public string Supplier { get; set; }
        public string Description { get; set; }
        public string LongDescription { get; set; }
        public string Manufacturer { get; set; }
        public string Category { get; set; }
        public string GLAccount { get; set; }
        public string UOM { get; set; }
        //public string PackageQuantity { get; set; }

        public string Cost { get; set; }
        public string SellPrice { get; set; }
        //public string Taxable { get; set; }

        public string ItemCriticalQuantity { get; set; }
        public string ItemMinimumQuantity { get; set; }
        public string ItemMaximumQuantity { get; set; }
        public string Staging { get; set; }
        public string Picture { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }



        public string ID { get; set; }
        public string DefaultPullQuantity { get; set; }
        public string DefaultReorderQuantity { get; set; }

        public string BinDefaultPullQuantity { get; set; }
        public string BinDefaultReorderQuantity { get; set; }

        public string HardCodeQuantity { get; set; }
        public string ItemUniqueNumber { get; set; }

        public string EnterpriseLogo { get; set; }
        public string CompanyLogo { get; set; }
        public string SupplierLogo { get; set; }
        public string ItemImage { get; set; }

        public string ImagePath { get; set; }

        public string StagingLocation { get; set; }
        public string UNSPSC { get; set; }
        public string ItemType { get; set; }

        public string SerialOrLotNumber { get; set; }
        public string ItemTrackingType { get; set; }

        public string CostUOM { get; set; }

        public string UDF6 { get; set; }
        public string UDF7 { get; set; }
        public string UDF8 { get; set; }
        public string UDF9 { get; set; }
        public string UDF10 { get; set; }
        public System.String ExpirationDate { get; set; }

        public string SerialNumberTracking { get; set; }
        public string LotNumberTracking { get; set; }
        public string DateCodeTracking { get; set; }

    }
}
