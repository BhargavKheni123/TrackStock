using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using eTurns.DTO.Resources;
using System.Web;

namespace eTurns.DTO
{
    public enum eVMIShelfRequestType
    {
        GetShelfID = 1,
        SetShelfID = 2,
        GetShelfIDImmediate = 3,
        SetShelfIDImmediate = 4,
    }

    [Serializable]
    public class eVMIShelfRequestDTO
    {
        public System.Int64 ID { get; set; }
       
        public int ShelfID { get; set; }

        public System.Int64 RoomID { get; set; }
        public System.Int64 CompanyID { get; set; }

        public System.Int32 RequestType { get; set; }

        public System.Boolean IsShelfStarted { get; set; }

        public Nullable<System.DateTime> ShelfStartTime { get; set; }

        public System.Boolean IsShelfCompleted { get; set; }

        public Nullable<System.DateTime> ShelfCompletionTime { get; set; }

        public string ErrorDescription { get; set; }

        public System.DateTime Created { get; set; }
        public System.DateTime Updated { get; set; }

        public string RoomName { get; set; }

        public string CompanyName { get; set; }

        public Int64 CreatedBy { get; set; }

        public Int64 UpdatedBy { get; set; }

        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }

        public int? TotalRecords { get; set; }

        public string CreatedDate { get; set; }

        public string UpdatedDate { get; set; }

        public string RequestTypes { get; set; }

        public System.Int64 ComPortMasterID { get; set; }

        public string ComPortName { get; set; }
        public long ComPortRoomMappingID { get; set; }
    }

    public class eVMIShelfReqDTO : eVMIShelfRequestDTO
    {
        public int TotalRequest { get; set; }
        public int RecordsPerPage { get; set; }

        public int NoOfPages { get; set; }
    }
    public class eVMIShelfRequestRooms
    {
        public long RoomID { get; set; }
        public long CompanyID { get; set; }
    }

    public class ResShelfRequest
    {
        private static string resourceFile = "ResShelfRequest";
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

        public static string ShelfID
        {
            get
            {
                return ResourceRead.GetResourceValue("ShelfID", resourceFile);
            }
        }

        

        public static string ComPort
        {
            get
            {
                return ResourceRead.GetResourceValue("ComPort", resourceFile);
            }
        }

        public static string IsShelfStarted
        {
            get
            {
                return ResourceRead.GetResourceValue("IsShelfStarted", resourceFile);
            }
        }

        public static string IsShelfCompleted
        {
            get
            {
                return ResourceRead.GetResourceValue("IsShelfCompleted", resourceFile);
            }
        }

        public static string ShelfStartTime
        {
            get
            {
                return ResourceRead.GetResourceValue("ShelfStartTime", resourceFile);
            }
        }

        public static string ShelfCompletionTime
        {
            get
            {
                return ResourceRead.GetResourceValue("ShelfCompletionTime", resourceFile);
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
