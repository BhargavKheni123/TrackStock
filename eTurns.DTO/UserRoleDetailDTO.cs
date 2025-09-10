namespace eTurns.DTO
{
    public class UserRoleDetailDTO
    {
        public long UserID { get; set; }
        public long RoleID { get; set; }
        public bool IsChecked { get; set; }
        public bool IsDelete { get; set; }
        public bool IsInsert { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsView { get; set; }
        public bool ShowDeleted { get; set; }
        public bool ShowArchived { get; set; }
        public bool ShowUDF { get; set; }
        public bool ShowChangeLog { get; set; }
        public long ModuleID { get; set; }
        public string ModuleValue { get; set; }
        public long RoomId { get; set; }
        public long CompanyID { get; set; }
        public long EnterpriseID { get; set; }

    }
}
