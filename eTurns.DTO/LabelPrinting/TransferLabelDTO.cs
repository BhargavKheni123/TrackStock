namespace eTurns.DTO.LabelPrinting
{
    public class TransferLabelDTO
    {
        public string TransferNumber { get; set; }
        public string StagingName { get; set; }
        public string ItemNumber { get; set; }
        public string BinNumber { get; set; }
        public string ItemMinimumQuantity { get; set; }
        public string ItemMaximumQuantity { get; set; }
        public string GLAccount { get; set; }
        public string Cost { get; set; }
        public string SellPrice { get; set; }
        public string Manufacturer { get; set; }
        public string ManufacturerNumber { get; set; }
        public string Supplier { get; set; }
        public string SupplierNumber { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        public string PackageQuantity { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }

        public string ID { get; set; }
        public string DefaultPullQuantity { get; set; }
        public string DefaultReorderQuantity { get; set; }
        public string HardCodeQuantity { get; set; }

        public string EnterpriseLogo { get; set; }
        public string CompanyLogo { get; set; }
        public string SupplierLogo { get; set; }
        public string ItemImage { get; set; }

        public string ImagePath { get; set; }
        public string ItemUniqueNumber { get; set; }
        public string UPCNumber { get; set; }

        public string Replenishinglocation { get; set; }
        public string Requestinglocation { get; set; }
        public string RepleneshingStockRoomName { get; set; }
        public string RequestingStockRoomName { get; set; }
        public string ItemUDF1 { get; set; }
        public string ItemUDF2 { get; set; }
        public string ItemUDF3 { get; set; }
        public string ItemUDF4 { get; set; }
        public string ItemUDF5 { get; set; }
        public string ItemUDF6 { get; set; }
        public string ItemUDF7 { get; set; }
        public string ItemUDF8 { get; set; }
        public string ItemUDF9 { get; set; }
        public string ItemUDF10 { get; set; }
        public string SerialNumberOfItem { get; set; }
        public string LotNumberOfItem { get; set; }
        public string ExpirationDateItem { get; set; }
        public string Quantitytransferred { get; set; }
    }
}
