using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;

namespace eTurns.DTO
{

    public class KitItemToMoveDTO
    {
        public long EnterpriseId { get; set; }
        public long RoomId { get; set; }
        public long CompanyId { get; set; }
        public long ItemID { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorCode { get; set; }
        public List<ItemLocationLotSerialDTO> lstItemPullDetails { get; set; }
        public List<PullErrorInfo> ErrorList { get; set; }
        public double TotalCustomerOwnedTobePulled { get; set; }
        public double TotalConsignedTobePulled { get; set; }
        public Guid ItemGUID { get; set; }
        public string ItemNumber { get; set; }
        public long BinID { get; set; }
        public string ReturnDate { get; set; }
        public Int64 KitDetailID { get; set; }
        public Guid KitDetailGUID { get; set; }
        public string BinNumber { get; set; }
        public double QtyToMoveIn { get; set; }
        public string KitPartNumber { get; set; }
        public Guid KitGuid { get; set; }
        public bool IsLotTrack { get; set; }
        public bool IsSRTrack { get; set; }
        public bool IsDCTrack { get; set; }
        public string InventoryConsuptionMethod { get; set; }
        public long CreatedBy { get; set; }
        public long LastUpdatedBy { get; set; }
        public bool CanOverrideProjectLimits { get; set; }

        public string MoveType { get; set; }
        public string BinName { get; set; }
        public double? maximumQuantityForMoveOut { get; set; }
        public double? maximumQuantityForMoveIn { get; set; }
        public bool ValidateOnHandQty { get; set; }
        public bool hasValidOnHandQty { get; set; }
    }

    public class MoveInOutQtyDetail
    {
        public LocationWiseQty[] BinWiseQty { get; set; }
        public double TotalQty { get; set; }
        public string ItemGUID { get; set; }
        public string ButtonText { get; set; }
        public string KitDetailGUID { get; set; }
    }

    public class LocationWiseQty
    {
        public Int64 LocationID { get; set; }
        public double Quantity { get; set; }
        public string LocationName { get; set; }
    }

    public class KitMoveInOutDetailDTO
    {
        public Int64 ID { get; set; }
        public Nullable<Guid> KitDetailGUID { get; set; }
        public Nullable<Guid> ItemLocationDetailGUID { get; set; }
        public Nullable<Guid> ItemGUID { get; set; }
        public Nullable<Int64> BinID { get; set; }
        public string MoveInOut { get; set; }
        public Nullable<double> CustomerOwnedQuantity { get; set; }
        public Nullable<double> ConsignedQuantity { get; set; }
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
        public string BinNumber { get; set; }

        public Nullable<DateTime> ExpirationDate { get; set; }
        public string SerialNumber { get; set; }
        public string LotNumber { get; set; }
    }

    public class ResKitMoveInOutDetail
    {
        private static string ResourceFileName = "ResKitMoveInOutDetail";

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

        public static string ItemBinDoesNotHaveSufficientQtyToMoveIn
		 { 
			get
            { 
			   return ResourceRead.GetResourceValue("ItemBinDoesNotHaveSufficientQtyToMoveIn", ResourceFileName); 
		    } 
		}

    }
}


