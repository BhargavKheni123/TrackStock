using System;

namespace eTurns.DTO
{
    public class UserSupplierFilterDTO
    {

        public long ID { get; set; }

        public Guid GUID { get; set; }

        public long UserId { get; set; }

        public long SuperAdminId { get; set; }

        public DateTime Created { get; set; }

        public DateTime? LastUpdated { get; set; }

        public long? CreatedBy { get; set; }

        public long? LastUpdatedBy { get; set; }
        public long Room { get; set; }
        public long CompanyID { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsArchived { get; set; }
        public string AddedFrom { get; set; }
        public string EditedFrom { get; set; }
    }

    public class UserSupplierFilterParam
    {
        public long RoomId { get; set; }
        public long CompanyId { get; set; }
    }
}
