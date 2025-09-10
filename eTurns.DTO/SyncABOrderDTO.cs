using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DTO
{
    public class SyncABOrderDTO
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string StartDateStr
        {
            get;
            set;
        }

        public string EndDateStr
        {
            get;
            set;
        }
    }
}
