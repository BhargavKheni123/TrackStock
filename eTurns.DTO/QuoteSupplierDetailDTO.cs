using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{
    public partial class QuoteSupplierDetailDTO
    {
        public long ID { get; set; }
        public System.Guid GUID { get; set; }
        public System.Guid QuoteGUID { get; set; }
        public long QuoteID { get; set; }
        public long SupplierID { get; set; }
        public long RoomID { get; set; }
        public long CompanyID { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime Updated { get; set; }
        public bool IsDeleted { get; set; }
        public string SupplierName { get; set; }
        public string QuoteNumber { get; set; }
    }
}
