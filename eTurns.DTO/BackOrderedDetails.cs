using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DTO
{
   public class BackOrderedDetails
   {
        public Guid GUID { get; set; }
        public Guid OrderDetailsGUID { get; set; }
        public Guid ItemGuid { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public double? BackOrderQuantity { get; set; }

        private string _expectedDate;
        public string ExpectedDateStr
        {
            get
            {
                if (string.IsNullOrEmpty(_expectedDate))
                {
                    _expectedDate = FnCommon.ConvertDateByTimeZone(ExpectedDate, true);
                }
                return _expectedDate;
            }
            set { this._expectedDate = value; }
        }
    }
}
