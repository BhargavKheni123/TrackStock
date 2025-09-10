using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using eTurns.DTO.Resources;
using System.Web;


namespace eTurns.DTO
{
	public enum eVMIResetRequestType
	{
		Reset = 1,
		ResetAll = 2,
		ResetImmediate = 3
	}

	public class eVMIResetRequestDTO
    {
		public long ID { get; set; }
		public int ScaleID { get; set; }
		public int ChannelID { get; set; }
		public Guid ItemGUID { get; set; }
		public System.Int64 BinID { get; set; }

		public string BinNumber { get; set; }

		public System.Int32 RequestType { get; set; }
		public long ComPortMasterID { get; set; }
		public string ComPortName { get; set; }
		public long RoomID { get; set; }

		public string RoomName { get; set; }
		public long CompanyID { get; set; }

		public string CompanyName { get; set; }

		public string ItemNumber { get; set; }

		public bool IsResetStarted { get; set; }
		public DateTime? ResetStartTime { get; set; }
		public bool IsResetCompleted { get; set; }
		public DateTime? ResetCompletionTime { get; set; }
		public string ErrorDescription { get; set; }
		public DateTime Created { get; set; }
		public DateTime? Updated { get; set; }
		public long CreatedBy { get; set; }
		public long? UpdatedBy { get; set; }

        public int? TotalRecords { get; set; }

        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }

        public string CreatedDate { get; set; }

        public string UpdatedDate { get; set; }

        public string RequestTypes { get; set; }
        public long ComPortRoomMappingID { get; set; }
    }

	public class eVMIResetRequestRooms
    {
		public long RoomID { get; set; }
		public long CompanyID { get; set; }
	}

	public class eVMIResetReqDTO : eVMIResetRequestDTO
	{
		public int TotalRequest { get; set; }
		public int RecordsPerPage { get; set; }

		public int NoOfPages { get; set; }
	}

    public class ResResetRequest
    {
        private static string resourceFile = "ResResetRequest";
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", resourceFile);
            }
        }

        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", resourceFile);
            }
        }

        public static string ScaleID
        {
            get
            {
                return ResourceRead.GetResourceValue("ScaleID", resourceFile);
            }
        }

        public static string ChannelID
        {
            get
            {
                return ResourceRead.GetResourceValue("ChannelID", resourceFile);
            }
        }

        public static string ComPort
        {
            get
            {
                return ResourceRead.GetResourceValue("ComPort", resourceFile);
            }
        }

        public static string IsResetStarted
        {
            get
            {
                return ResourceRead.GetResourceValue("IsResetStarted", resourceFile);
            }
        }

        public static string IsResetCompleted
        {
            get
            {
                return ResourceRead.GetResourceValue("IsResetCompleted", resourceFile);
            }
        }

        public static string ResetStartTime
        {
            get
            {
                return ResourceRead.GetResourceValue("ResetStartTime", resourceFile);
            }
        }

        public static string ResetCompletionTime
        {
            get
            {
                return ResourceRead.GetResourceValue("ResetCompletionTime", resourceFile);
            }
        }

        public static string ErrorDescription
        {
            get
            {
                return ResourceRead.GetResourceValue("ErrorDescription", resourceFile);
            }
        }

        public static string RequestType
        {
            get
            {
                return ResourceRead.GetResourceValue("RequestType", resourceFile);
            }
        }

    }


}
