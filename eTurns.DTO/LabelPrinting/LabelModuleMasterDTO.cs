namespace eTurns.DTO.LabelPrinting
{
    public enum LabelModule
    {
        Asset = 1,
        Inventory = 2,
        Kitting = 3,
        Orders = 4,
        QuickLists = 5,
        Receipts = 6,
        Staging = 7,
        Tools = 8,
        Transfer = 9,
        Technician = 10,
        Locations = 11,
        WorkOrder = 12
    }

    public class LabelModuleMasterDTO
    {
        public System.Int64 ID { get; set; }
        public System.String ModuleName { get; set; }
        public System.String ModuleDTOName { get; set; }

    }

}


