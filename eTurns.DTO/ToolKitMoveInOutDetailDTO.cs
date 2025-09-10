using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;

namespace eTurns.DTO
{

    public class ToolKitToolToMoveDTO
    {
        public long EnterpriseId { get; set; }
        public long RoomId { get; set; }
        public long CompanyId { get; set; }
        public long ToolID { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorCode { get; set; }
        public List<ToolLocationLotSerialDTO> lstToolPullDetails { get; set; }
        public List<PullToolAssetErrorInfo> ErrorList { get; set; }
        public double TotalbePulled { get; set; }
        public Guid ToolGUID { get; set; }
        public string ToolName { get; set; }
        public long ToolBinID { get; set; }
        public string ReturnDate { get; set; }
        public Int64 ToolKitDetailID { get; set; }
        public Guid ToolKitDetailGUID { get; set; }
        public string Location { get; set; }
        public double QtyToMoveIn { get; set; }
        public string KitPartNumber { get; set; }
        public Guid ToolKitGuid { get; set; }
        public bool IsSRTrack { get; set; }
        public string InventoryConsuptionMethod { get; set; }
        public long CreatedBy { get; set; }
        public long LastUpdatedBy { get; set; }

    }

    public class ToolAssetMoveInOutQtyDetail
    {
        public LocationWiseQty[] BinWiseQty { get; set; }
        public double TotalQty { get; set; }
        public string ToolGUID { get; set; }
        public string ButtonText { get; set; }
        public string ToolKitDetailGUID { get; set; }
    }

    public class ToolAssetLocationWiseQty
    {
        public Int64 LocationID { get; set; }
        public double Quantity { get; set; }
        public string LocationName { get; set; }
    }

    public class ToolAssetKitMoveInOutDetailDTO
    {
        public Int64 ID { get; set; }
        public Nullable<Guid> ToolKitDetailGUID { get; set; }
        public Nullable<Guid> ToolQuantityDetailGUID { get; set; }
        public Nullable<Guid> ToolGUID { get; set; }
        public Nullable<Int64> ToolBinID { get; set; }
        public string MoveInOut { get; set; }
        public Nullable<double> Quantity { get; set; }
        public double TotalQuantity { get; set; }
        public Nullable<DateTime> Created { get; set; }
        public Nullable<DateTime> Updated { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> LastUpdatedBy { get; set; }
        public Nullable<Int64> Room { get; set; }
        public Nullable<Int64> CompanyID { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public Guid GUID { get; set; }
        public string RoomName { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        public DateTime ReceivedOn { get; set; }
        public DateTime ReceivedOnWeb { get; set; }
        public string AddedFrom { get; set; }
        public string EditedFrom { get; set; }
        private string _ReceivedOn;
        public string ReceivedOnDate
        {
            get
            {
                if (string.IsNullOrEmpty(_ReceivedOn))
                {
                    _ReceivedOn = FnCommon.ConvertDateByTimeZone(ReceivedOn, true);
                }
                return _ReceivedOn;
            }
            set { this._ReceivedOn = value; }
        }
        private string _ReceivedOnWeb;
        public string ReceivedOnWebDate
        {
            get
            {
                if (string.IsNullOrEmpty(_ReceivedOnWeb))
                {
                    _ReceivedOnWeb = FnCommon.ConvertDateByTimeZone(ReceivedOnWeb, true);
                }
                return _ReceivedOnWeb;
            }
            set { this._ReceivedOnWeb = value; }
        }
        public string Location { get; set; }

        public string SerialNumber { get; set; }


    }

    public class ResToolKitMoveInOutDetail
    {
        private static string ResourceFileName = "ResToolKitMoveInOutDetail";

        /// <summary>
        ///   Looks up a localized string similar to KitDetailID.
        /// </summary>
        public static string KitDetailID
        {
            get
            {
                return ResourceRead.GetResourceValue("KitDetailID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemID.
        /// </summary>
        public static string ItemID
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to BinID.
        /// </summary>
        public static string BinID
        {
            get
            {
                return ResourceRead.GetResourceValue("BinID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MoveInOut.
        /// </summary>
        public static string MoveInOut
        {
            get
            {
                return ResourceRead.GetResourceValue("MoveInOut", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CustomerOwnedQuantity.
        /// </summary>
        public static string CustomerOwnedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("CustomerOwnedQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ConsignedQuantity.
        /// </summary>
        public static string ConsignedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ConsignedQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to TotalQuantity.
        /// </summary>
        public static string TotalQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("TotalQuantity", ResourceFileName);
            }
        }


    }
}


