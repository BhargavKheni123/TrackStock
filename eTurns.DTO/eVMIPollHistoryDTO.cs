using System;

namespace eTurns.DTO
{
    public class eVMIPollHistoryDTO
    {
        public System.Int64 ID { get; set; }
        public Guid ItemGUID { get; set; }
        public Guid GUID { get; set; }
        public Int64 BinID { get; set; }
        public int PollType { get; set; }

        public Nullable<double> BegingCustOwnQty { get; set; }
        public Nullable<double> EndingCustOwnQty { get; set; }
        public Nullable<double> BegingConsQty { get; set; }
        public Nullable<double> EndingConsQty { get; set; }

        /// <summary>
        /// This coming from evmi application
        /// </summary>
        public Nullable<double> BeginingQty { get; set; }
        public Nullable<double> EndingQty { get; set; }

        public Nullable<double> NewQuantity { get; set; }
        public Nullable<int> PollStatus { get; set; }
        public string PollStatusDesc { get; set; }

        public DateTime PollDate { get; set; }
        public DateTime Created { get; set; }
        public Int64 CreatedBy { get; set; }
        public Int64 CompanyID { get; set; }
        public Int64 RoomID { get; set; }
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


    }

    public class RPT_eMVIHistory
    {
        public string ItemNumber { get; set; }
        public string BinName { get; set; }
        public double eVMISensorID { get; set; }
        public double EndingQty { get; set; }
        public double NewQuantity { get; set; }
        public double BeginingQty { get; set; }
        public double ReceivedQty { get; set; }
        public double ReturnQty { get; set; }
        public double UsageQty { get; set; }
        public string PollDate { get; set; }
        public double OnHandQuantity { get; set; }
        public string ManufacturerNumber { get; set; }
        public string SupplierPartNo { get; set; }
        public string UPC { get; set; }
        public string UNSPSC { get; set; }
        public string ItemDescription { get; set; }
        public string ItemUDF1 { get; set; }
        public string ItemUDF2 { get; set; }
        public string ItemUDF3 { get; set; }
        public string ItemUDF4 { get; set; }
        public string ItemUDF5 { get; set; }
        public string ItemBlanketPO { get; set; }
        public string ItemSupplier { get; set; }
        public string ManufacturerName { get; set; }
        public string CategoryName { get; set; }
        public string Unit { get; set; }
        public string CostUOM { get; set; }
        public string ItemTypeName { get; set; }
        public string Markup { get; set; }
        public string WeightPerPiece { get; set; }
        public string Consignment { get; set; }
        public string RoomInfo { get; set; }
        public int QuantityDecimalPoint { get; set; }
        public int CostDecimalPoint { get; set; }
        public string CreatedBy { get; set; }
        public string RoomName { get; set; }
        public string CompanyName { get; set; }
        public Int64 RoomID { get; set; }
        public Int64 CompanyID { get; set; }
        public string CurrentDateTime { get; set; }
        public Guid ItemGUID { get; set; }
        public Int64? BinID { get; set; }
        public Int64 ID { get; set; }
        public int PollStatus { get; set; }
        public int PollType { get; set; }
        public string PollStatusDesc { get; set; }
        public string PollTypeDesc { get; set; }
    }
}
