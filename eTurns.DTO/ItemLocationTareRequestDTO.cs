using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using eTurns.DTO.Resources;
using System.Web;

namespace eTurns.DTO
{
    public enum TareRequestType
    {
        Tare = 1,
        TareAll = 2,
        TareImmediate = 3
    }

    [Serializable]
    public class ItemLocationTareRequestDTO
    {
        public System.Int64 ID { get; set; }
        public Guid ItemGUID { get; set; }
        public System.Int64 BinID { get; set; }
        public System.Int32 ScaleID { get; set; }

        public System.Int32 ChannelID { get; set; }

        public System.Int64 RoomID { get; set; }
        public System.Int64 CompanyID { get; set; }

        public System.Int64 ComPortMasterID { get; set; }

        public string ComPortName { get; set; }

        public System.Int32 RequestType { get; set; }

        public System.Boolean IsTareStarted { get; set; }

        public Nullable<System.DateTime> TareStartTime { get; set; }

        public string TareStartTimeStr { get; set; }

        public System.Boolean IsTareCompleted { get; set; }

        public Nullable<System.DateTime> TareCompletionTime { get; set; }

        public string TareCompletionTimeStr { get; set; }

        public string ErrorDescription { get; set; }

        public System.DateTime Created { get; set; }
        public string CreatedStr { get; set; }
        public System.DateTime Updated { get; set; }
        public string UpdatedStr { get; set; }
        public string RoomName { get; set; }
        public string CompanyName { get; set; }
        public string BinNumber { get; set; }
        public string ItemNumber { get; set; }

        public Int64 CreatedBy { get; set; }

        public Int64 UpdatedBy { get; set; }

        public int? TotalRecords { get; set; }
        public long? RowNum { get; set; }
        
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }

        public string RequestTypes { get; set; }

    }

    public class ILTareRequestDTO : ItemLocationTareRequestDTO
    {
        public int TotalRequest { get; set; }
        public int RecordsPerPage { get; set; }

        public int NoOfPages { get; set; }
    }

    public class ResItemLocationTareRequest
    {
        private static string resourceFile = "ResItemLocationTareRequest";
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

        public static string IsTareStarted
        {
            get
            {
                return ResourceRead.GetResourceValue("IsTareStarted", resourceFile);
            }
        }

        public static string IsTareCompleted
        {
            get
            {
                return ResourceRead.GetResourceValue("IsTareCompleted", resourceFile);
            }
        }
                

        public static string TareStartTime
        {
            get
            {
                return ResourceRead.GetResourceValue("TareStartTime", resourceFile);
            }
        }

        public static string TareCompletionTime
        {
            get
            {
                return ResourceRead.GetResourceValue("TareCompletionTime", resourceFile);
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

    public class ItemLocationTareRequestRooms
    {
        public long RoomID { get; set; }
        public long CompanyID { get; set; }
    }


}
