using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.DTO
{
    public class OfflineImportFileHistoryDTO
    {
        public long ID { get; set; }
        public long CompanyID { get; set; }
        public long Room { get; set; }
        public int ModuleId { get; set; }
        public string FileName { get; set; }
        public string FileUniqueName { get; set; }
        public string FilePath { get; set; }
        public bool IsProcessed { get; set; }
        public DateTime? ProcessStart { get; set; }
        public DateTime? ProcessEnd { get; set; }

        public DateTime? Created { get; set; }
        //CreatedBy
        public long? CreatedBy { get; set; }
        //Updated
        public DateTime? Updated { get; set; }
        //LastUpdatedBy
        public long? LastUpdatedBy { get; set; }
        public string Email { get; set; }
    }
}
