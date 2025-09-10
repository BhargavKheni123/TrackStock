using System;

namespace eTurns.DTO
{
    public class ToolImageDetailDTO
    {
        public Int64 ID { get; set; }

        public Guid GUID { get; set; }

        public Guid ToolGuid { get; set; }

        public string SerialNumber { get; set; }

        public string ImagePath { get; set; }

        public string ImageType { get; set; }

        public Int64 CompanyId { get; set; }

        public Int64 RoomId { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Updated { get; set; }

        public Int64 CreatedBy { get; set; }

        public Int64 LastUpdatedBy { get; set; }

        public DateTime? ReceivedOn { get; set; }

        public DateTime? ReceivedOnWeb { get; set; }

        public bool? IsDeleted { get; set; }
        public bool? IsArchived { get; set; }

        public string AddedFrom { get; set; }

        public string EditedFrom { get; set; }

    }
}
