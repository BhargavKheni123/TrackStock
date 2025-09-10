using eTurns.DTO.Resources;

namespace eTurns.DTO
{
    public class BillingRoomTypeModuleMasterDTO
    {
        public long? ID { get; set; }
        public int? BillingRoomTypeId { get; set; }
        public long ModuleId { get; set; }
        public bool IsEnable { get; set; }
        public double? Price { get; set; }
        public string ModuleName { get; set; }
        public long EnterpriseID { get; set; }

        public string ResourceKey { get; set; }
        public string ModuleNameFromResource
        {
            get
            {
                if (!string.IsNullOrEmpty(ResourceKey))
                    return ResourceRead.GetResourceValue(ResourceKey, "ResModuleName");
                else
                    return ModuleName;
            }
        }
    }
}
