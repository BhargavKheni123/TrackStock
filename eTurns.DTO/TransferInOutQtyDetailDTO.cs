using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{

    public class TransferInOutQtyDetailDTO
    {
        public Int64 ID { get; set; }
        public Nullable<Guid> TransferGUID { get; set; }
        public Nullable<Guid> TransferDetailGUID { get; set; }
        public Nullable<Guid> ItemGUID { get; set; }
        public Nullable<Int64> BinID { get; set; }
        public string TransferInOut { get; set; }
        public Nullable<double> CustomerOwnedQuantity { get; set; }
        public Nullable<double> ConsignedQuantity { get; set; }
        public double TotalQuantity { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Int64 CreatedBy { get; set; }
        public Int64 LastUpdatedBy { get; set; }
        public Int64 Room { get; set; }
        public Int64 CompanyID { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public Guid GUID { get; set; }
        public bool IsTransfered { get; set; }
        public List<TransferInOutItemDetailDTO> TrfInOutItemDetail { get; set; }
        public string BinNumber { get; set; }
        public string ItemNumber { get; set; }

        public bool? SerialNumberTracking { get; set; }

        public bool? LotNumberTracking { get; set; }

        public bool? DateCodeTracking { get; set; }

        private string _createdDate;
        public string CreatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_createdDate))
                {
                    _createdDate = FnCommon.ConvertDateByTimeZone(Created, true);
                }
                return _createdDate;
            }
            set { this._createdDate = value; }
        }

        private string _updatedDate;
        public string UpdatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_updatedDate))
                {
                    _updatedDate = FnCommon.ConvertDateByTimeZone(Updated, true);
                }
                return _updatedDate;
            }
            set { this._updatedDate = value; }
        }

        [Display(Name = "ReceivedOn", ResourceType = typeof(ResCommon))]
        public System.DateTime ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResCommon))]
        public System.DateTime ReceivedOnWeb { get; set; }

        //AddedFrom
        [Display(Name = "AddedFrom", ResourceType = typeof(ResCommon))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String AddedFrom { get; set; }

        //EditedFrom
        [Display(Name = "EditedFrom", ResourceType = typeof(ResCommon))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String EditedFrom { get; set; }

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
    }

    public class TakenQtyDetail
    {
        public QuantityByLocation[] BinWiseQty { get; set; }
        public double TotalQty { get; set; }
        public double ReceivedQty { get; set; }
        public string ItemGUID { get; set; }
        public string ButtonText { get; set; }
        public string DetailGUID { get; set; }

        public bool IsSignleTransfer { get; set; }

        public List<ItemLocationLotSerialDTO> ItemTransferDetails { get; set; }
    }

    public class QuantityByLocation
    {
        public Int64 LocationID { get; set; }
        public double Quantity { get; set; }
        public string LocationName { get; set; }
    }

    public class TransferInOutQtyDetailLimited
    {
        public Int64 TotalQty { get; set; }
        public string LotNumber { get; set; }
        public string SerialNumber { get; set; }
        public DateTime? ExpirationDate { get; set; }

        private string _Expiration;
        public string ExpirationStr
        {
            get
            {
                if (string.IsNullOrEmpty(_Expiration))
                {
                    _Expiration = FnCommon.ConvertDateByTimeZone(ExpirationDate,false,true);
                }
                return _Expiration;
            }
            set { this._Expiration = value; }
        }

        public string BinNumber { get; set; }
    }


}
