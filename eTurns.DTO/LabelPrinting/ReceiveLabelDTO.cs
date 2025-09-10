namespace eTurns.DTO.LabelPrinting
{
    public class ReceiveLabelDTO
    {
        public string ItemNumber { get; set; }
        public string BinNumber { get; set; }
        public string QuantityReceived { get; set; }
        public string ReceivedDate { get; set; }
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
        public string SerialNumber { get; set; }
        public string LotNumber { get; set; }
        public string DateCode { get; set; }
        public string PurchaseOrTransfer { get; set; }
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

    }
}
