using System;

namespace eTurns.DTO.LabelPrinting
{
    public class LabelModuleTemplateDetailDTO
    {

        public System.Int64 ID { get; set; }

        public System.Int64 ModuleID { get; set; }

        public System.Int64 TemplateDetailID { get; set; }

        public System.Int64 CompanyID { get; set; }

        public System.Int64 CreatedBy { get; set; }

        public System.Int64 UpdatedBy { get; set; }

        public System.DateTime CreatedOn { get; set; }

        public System.DateTime UpdatedOn { get; set; }

        public string CreatedByName { get; set; }

        public string UpdatedByName { get; set; }
        public Nullable<System.Int64> RoomID { get; set; }

    }


}


