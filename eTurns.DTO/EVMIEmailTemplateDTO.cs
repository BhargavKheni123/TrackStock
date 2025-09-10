using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.DTO
{
	public class EVMIEmailTemplateDTO
	{
		public long NotificationID { get; set; }
		public long RoomScheduleID { get; set; }
		public string EmailAddress { get; set; }
		public long? EmailTemplateID { get; set; }
		public int NotificationMode { get; set; }
		public bool ShowSignature { get; set; }
		public bool? SendEmptyEmail { get; set; }
		public bool HideHeader { get; set; }
		public string MailBodyText { get; set; }
		public string MailSubject { get; set; }
		public short ScheduleMode { get; set; }
		public string CultureCode { get; set; }
	}
}
