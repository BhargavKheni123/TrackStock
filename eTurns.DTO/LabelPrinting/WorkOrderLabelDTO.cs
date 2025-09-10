namespace eTurns.DTO.LabelPrinting
{
    public class WorkOrderLabelDTO
    {
        public string ID { get; set; }
        public string GUID { get; set; }
        public string Name { get; set; }
        public string Customer { get; set; }
        public string Asset { get; set; }
        public string Tool { get; set; }
        public string Technician { get; set; }
        public string ProjectSpend { get; set; }
        public string Description { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string EnterpriseLogo { get; set; }
        public string CompanyLogo { get; set; }

        public string ItemNumber { get; set; }
        public string BinNumber { get; set; }
        public string PoolQuantity { get; set; }
        public string PULLCost { get; set; }
        public string PullPrice { get; set; }
        public string ItemMinimumQuantity { get; set; }
        public string ItemMaximumQuantity { get; set; }
        public string GLAccount { get; set; }
        public string Manufacturer { get; set; }
        public string ManufacturerNumber { get; set; }
        public string Supplier { get; set; }
        public string SupplierNumber { get; set; }
        public string UOM { get; set; }
        public string ImagePath { get; set; }
        public string ItemImage { get; set; }
        public string SupplierLogo { get; set; }
    }
}
