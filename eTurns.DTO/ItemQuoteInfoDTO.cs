using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.DTO
{
    public class ItemQuoteInfoDTO
    {
        public double OnQuotedQuantity { get; set; }
        public double OnQuoteInTransitQuantity { get; set; }

        public Guid ItemGUID { get; set; }
    }
}
