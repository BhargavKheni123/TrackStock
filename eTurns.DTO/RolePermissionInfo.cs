using System;

namespace eTurns.DTO
{
    [Serializable]
    public class RolePermissionInfo
    {
        public long EnterPriseId { get; set; }
        public long CompanyId { get; set; }
        public long RoomId { get; set; }
        public long UserId { get; set; }
        public long RoleId { get; set; }
        public string EnterPriseId_CompanyId { get; set; }
        public string EnterPriseId_CompanyId_RoomId { get; set; }
        public string EnterPriseName { get; set; }
        public string CompanyName { get; set; }
        public string RoomName { get; set; }
        public bool IsEnterpriseSelected { get; set; }
        public bool IsCompanySelected { get; set; }
        public bool IsRoomSelectedSelected { get; set; }
        public int RoleUserType { get; set; }
        public bool IsSelected { get; set; }
        public int Count { get; set; }
    }

    [Serializable]
    public class UserNS
    {
        public long ID { get; set; }
        public long RoleId { get; set; }
        public long EnterpriseID { get; set; }
        public long CompanyID { get; set; }
        public long RoomID { get; set; }
        public string EnterpriseName { get; set; }
        public string CompanyName { get; set; }
        public string RoomName { get; set; }
        public string RoleName { get; set; }
        public int UserCount { get; set; }
        public string NSName { get; set; }

        public string UDF { get; set; }

    }
    [Serializable]
    public class RoleNS
    {
        public long ID { get; set; }
        public long RoleId { get; set; }
        public long EnterpriseID { get; set; }
        public long CompanyID { get; set; }
        public long RoomID { get; set; }
        public string EnterpriseName { get; set; }
        public string CompanyName { get; set; }
        public string RoomName { get; set; }
        public string RoleName { get; set; }
        public int RoleCount { get; set; }
        public string NSName { get; set; }
        public string UDF { get; set; }

    }
}
