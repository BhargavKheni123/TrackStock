using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.DTO
{
	public class UserRoleDTO
	{
		public long ID { get; set; }
		public bool EnforceRolePermission { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public long RoleId { get; set; }
		
		public string UserName { get; set; }
		public string Email { get; set; }
		public int UserType { get; set; }
		public long EnterpriseId { get; set; }
		public long CompanyID { get; set; }
	}
}
