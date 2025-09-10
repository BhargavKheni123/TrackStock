using System;
using System.Collections.Generic;

namespace eTurns.DTO
{
    [Serializable]
    public class UserAccessDTO
    {
        public long ID { get; set; }
        public long UserId { get; set; }
        public long RoleId { get; set; }
        public long CompanyId { get; set; }
        public long RoomId { get; set; }
        public long EnterpriseId { get; set; }
        public string EnterpriseName { get; set; }
        public string CompanyName { get; set; }
        public string RoomName { get; set; }
        public string CompanyLogo { get; set; }
        public bool IsRoomActive { get; set; }
        public bool IsCompanyActive { get; set; }
        public bool IsEnterpriseActive { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public int UserType { get; set; }
        public bool isEVMI { get; set; }
        public string SupplierIDs { get; set; }
    }


    [Serializable]
    public class UserRoomModuleAccessDTO
    {
        public long ID { get; set; }
        public long UserId { get; set; }
        public long RoleId { get; set; }
        public long CompanyId { get; set; }
        public long RoomId { get; set; }
        public long EnterpriseId { get; set; }
        public string EnterpriseName { get; set; }
        public string CompanyName { get; set; }
        public string RoomName { get; set; }
        public string CompanyLogo { get; set; }
        public bool IsRoomActive { get; set; }
        public bool IsCompanyActive { get; set; }
        public bool IsEnterpriseActive { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public int UserType { get; set; }

        public List<RoomModuleDetailsDTO> RoomModuleAccessDetails { get; set; }
    }

    [Serializable]

    public class RoomModuleDetailsDTO
    {

        public Int64? ID { get; set; }


        public Int64 ModuleID { get; set; }


        public string ModuleName { get; set; }


        public bool IsInsert { get; set; }

        public bool IsUpdate { get; set; }

        public bool IsDelete { get; set; }

        public bool IsView { get; set; }

        public bool IsChecked { get; set; }
        public string ModuleValue { get; set; }
        public bool ShowDeleted { get; set; }
        public bool ShowArchived { get; set; }
        public bool ShowUDF { get; set; }
        public bool ShowChangeLog { get; set; }
    }

    public class UserCredentialDTO
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }

}
