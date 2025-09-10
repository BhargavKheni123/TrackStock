using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.DTO
{

    public enum eVMICOMCommonRequestType
    {
        GetFirmWareVersion = 1,
        GetSerialNo = 2,
        GetModelNo = 3,
        SetModelNo = 4,
        GetFirmWareVersionImmediate = 5,
        GetSerialNoImmediate = 6,
        GetModelNoImmediate = 7,
        SetModelNoImmediate = 8
    }

    public class eVMICOMCommonRequestDTO
    {
        public System.Int64 ID { get; set; }

        public long ComPortMasterID { get; set; }
        public string ComPortName {get;set; }
        public string Version { get;set;}
        public string SerialNumber { get; set; }
        public string ModelNumber { get; set; }
        public Guid? ItemGUID { get; set; }
        public Nullable<Int32> ScaleID { get; set; }
        public long BinID{ get; set; }
        public System.Int64 RoomID { get; set; }
        public System.Int64 CompanyID { get; set; }

        public System.Int32 RequestType { get; set; }

        public System.Boolean IsComReqStarted { get; set; }

        public Nullable<System.DateTime> ComStartTime { get; set; }

        public System.Boolean IsComCompleted { get; set; }

        public Nullable<System.DateTime> ComCompletionTime { get; set; }

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

        public long ComPortRoomMappingID { get; set; }
    }

    public class eVMICommonReqDTO : eVMICOMCommonRequestDTO
    {
        public int TotalRequest { get; set; }
        public int RecordsPerPage { get; set; }

        public int NoOfPages { get; set; }
    }

    public class ReseVMICOMRequest
    {
        private static string resourceFile = "ReseVMICOMRequest";
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

        public static string ComPortMasterID
        {
            get
            {
                return ResourceRead.GetResourceValue("ComPortMasterID", resourceFile);
            }
        }



        public static string ComPort
        {
            get
            {
                return ResourceRead.GetResourceValue("ComPort", resourceFile);
            }
        }

        public static string IsComReqStarted
        {
            get
            {
                return ResourceRead.GetResourceValue("IsComReqStarted", resourceFile);
            }
        }

        public static string IsComCompleted
        {
            get
            {
                return ResourceRead.GetResourceValue("IsComCompleted", resourceFile);
            }
        }

        public static string ComStartTime
        {
            get
            {
                return ResourceRead.GetResourceValue("ComStartTime", resourceFile);
            }
        }

        public static string ComCompletionTime
        {
            get
            {
                return ResourceRead.GetResourceValue("ComCompletionTime", resourceFile);
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

        public static string ScaleID
        {
            get
            {
                return ResourceRead.GetResourceValue("ScaleID", resourceFile);
            }
        }


    }
}
