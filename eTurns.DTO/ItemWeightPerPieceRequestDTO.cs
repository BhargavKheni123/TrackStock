using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using eTurns.DTO.Resources;
using System.Web;

namespace eTurns.DTO
{

    public enum eVMIWeightPerPieceRequestType
    {
        WeightPerPiece = 1,
        WeightPerPieceAll = 2,
        WeightPerPieceImmediate = 3
    }

    [Serializable]
    public class ItemWeightPerPieceRequestDTO
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

        public System.Boolean IsWeightStarted { get; set; }

        public Nullable<System.DateTime> WeightStartTime { get; set; }

        public Nullable<System.Double> TotalQty { get; set; }

        public System.Boolean IsWeightCompleted { get; set; }

        public Nullable<System.DateTime> WeightCompletionTime { get; set; }

        public Nullable<System.Double> WeightReading { get; set; }

        public Nullable<System.Double> ItemWeightPerPiece { get; set; }

        public string ErrorDescription { get; set; }

        public System.DateTime Created { get; set; }
        public System.DateTime Updated { get; set; }

        public Nullable<System.Int64> CreatedBy { get; set; }
        public Nullable<System.Int64> UpdatedBy { get; set; }

        public System.Int32 RequestType { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }

        public int? TotalRecords { get; set; }

        public string CreatedDate { get; set; }

        public string UpdatedDate { get; set; }
        public Guid BinGUID { get; set; }

        public string RoomName { get; set; }
        public string ItemNumber { get; set; }

        public string CompanyName { get; set; }

        public string BinNumber { get; set; }

    }

    public class IWPPieceRequestDTO : ItemWeightPerPieceRequestDTO {
        public int TotalRequest { get; set; }
        public int RecordsPerPage { get; set; }

        public int NoOfPages { get; set; }
    }

    public class ResItemWeightPerPieceRequest
    {
        private static string resourceFile = "ResItemWeightPerPieceRequest";
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

        public static string IsWeightStarted
        {
            get
            {
                return ResourceRead.GetResourceValue("IsWeightStarted", resourceFile);
            }
        }

        public static string IsWeightCompleted
        {
            get
            {
                return ResourceRead.GetResourceValue("IsWeightCompleted", resourceFile);
            }
        }

        public static string WeightStartTime
        {
            get
            {
                return ResourceRead.GetResourceValue("WeightStartTime", resourceFile);
            }
        }

        public static string WeightCompletionTime
        {
            get
            {
                return ResourceRead.GetResourceValue("WeightCompletionTime", resourceFile);
            }
        }


        public static string WeightReading
        {
            get
            {
                return ResourceRead.GetResourceValue("WeightReading", resourceFile);
            }
        }

        public static string ErrorDescription
        {
            get
            {
                return ResourceRead.GetResourceValue("ErrorDescription", resourceFile);
            }
        }

        public static string TotalQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("TotalQuantity", resourceFile);
            }
        }

        public static string ItemWeightPerPiece
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemWeightPerPiece", resourceFile);
            }
        }

    }

    public class ItemWeightPerPieceRequestRooms
    {
        public long RoomID { get; set; }
        public long CompanyID { get; set; }
    }

    public class InsertWeightPerPieceResult { 
        public long ReturnFlag { get; set; }
        public long ID { get; set; }

    }
}
