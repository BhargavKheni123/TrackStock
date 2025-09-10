namespace eTurns.DTO.LabelPrinting
{
    public class AssetLabelDTO
    {
        public string TemplateName { get; set; }
        public string AssetName { get; set; }
        public string Description { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public string Category { get; set; }
        public string PurchaseDate { get; set; }
        public string PurchasePrice { get; set; }
        public string DepreciatedValue { get; set; }
        public string SuggestedMaintenanceDate { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }

        public string ID { get; set; }
        public string EnterpriseLogo { get; set; }
        public string CompanyLogo { get; set; }
        public string AssetImage { get; set; }
        public string ImageType { get; set; }

    }
}
