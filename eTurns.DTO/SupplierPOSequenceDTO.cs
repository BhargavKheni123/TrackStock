using System.ComponentModel;

namespace eTurns.DTO
{
    class SupplierPOSequenceDTO
    {
        public enum SupplierPOSequence
        {
            Blank,

            [Description("Fixed")]
            Fixed,

            [Description("Blanket Order#")]
            BlanketOrder,


            [Description("Increamenting by Order#")]
            IncreamentingByOrder,

            [Description("Increamenting by Day")]
            IncreamentingByDay,

            [Description("Date + Incrementing#")]
            DatePlusIncrementingNo,

            Date
        }

    }
}
