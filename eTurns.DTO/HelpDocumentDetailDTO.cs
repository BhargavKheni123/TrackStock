using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using eTurns.DTO.Resources;

namespace eTurns.DTO
{
    public class HelpDocumentDetailDTO
    {
        public System.Int64 ID { get; set; }
        public System.Int64? HelpDocMasterID { get; set; }
        public string ModuleDocName { get; set; }
        public string ModuleDocPath { get; set; }
        public string ModuleVideoName { get; set; }
        public string ModuleVideoPath { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<System.Int64> CreatedBy { get; set; }
        public Nullable<System.Int64> LastUpdatedBy { get; set; }
        public Nullable<Boolean> IsDeleted { get; set; }
        public Nullable<Boolean> IsArchived { get; set; }
        public bool? IsDoc { get; set; }
        public bool? IsVideo { get; set; }
        public bool IsDocShow { get; set; }
        public bool IsVideoShow { get; set; }

        public string ModuleDocNoExt { get; set; }
        public string ModuleVideoNoExt { get; set; }
        public string ModuleName { get; set; }
        public int DocType { get; set; }
    }
}
