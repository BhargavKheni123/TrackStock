using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using eTurns.DTO.Resources;
using System.Web;
using System.Web.Mvc;

namespace eTurns.DTO
{
    [Serializable]
    public class QuickBookItemMappingDTO
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }
        public Guid ItemGUID { get; set; }
        public System.String QuickBookItemID { get; set; }
        public Int64 CompanyID { get; set; }
        public Int64 RoomID { get; set; }
        public Nullable<Boolean> IsDeleted { get; set; }
        public Nullable<Boolean> IsArchived { get; set; }
        public Nullable<DateTime> Created { get; set; }
        public Nullable<DateTime> LastUpdated { get; set; }
        public Int64 CreatedBy { get; set; }
        public Int64 LastUpdatedBy { get; set; }
        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
        public System.String ItemNumber { get; set; }
    }
}
