using System;

namespace eTurns.DTO
{
    public class ItemOrderInfoDTO
    {
        public double OnOrderQuantity { get; set; }
        public double OnOrderInTransitQuantity { get; set; }
        public double OnReturnQuantity { get; set; }

        public Guid ItemGUID { get; set; }
    }
}
