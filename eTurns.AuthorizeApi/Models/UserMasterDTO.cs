using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.AuthorizeApi.Models
{
    public class UserMasterDTO
    {
        public Guid GUID { get; set; }
        public Int64 ID { get; set; }
        public Int64 CompanyID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public Int64 RoleID { get; set; }
        public string RoleName { get; set; }
        public string Email { get; set; }
        public string EnterpriseName { get; set; }
        public string EnterpriseDbName { get; set; }
        public long EnterpriseId { get; set; }
        public int UserType { get; set; }
        public string UserTypeName { get; set; }
        public bool IsLicenceAccepted { get; set; }
        public string CompanyName { get; set; }
        public bool IsLocked { get; set; }
        public bool IseTurnsAdmin { get; set; }

    }
}

