using System;

namespace eTurns.DTO
{
    //public class MaterialStagingPullDtl
    //{
    //    [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
    //    public System.Int64 ID { get; set; }

    //    [Display(Name = "PULLID", ResourceType = typeof(ResPullDetails))]
    //    public Nullable<System.Int64> PULLID { get; set; }

    //    [Display(Name = "ItemID", ResourceType = typeof(ResPullDetails))]
    //    public Nullable<System.Int64> ItemID { get; set; }

    //    [Display(Name = "ItemGUID", ResourceType = typeof(ResPullDetails))]
    //    public Nullable<Guid> ItemGUID { get; set; }

    //    [Display(Name = "ItemCost", ResourceType = typeof(ResPullDetails))]
    //    [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
    //    public Nullable<System.Decimal> ItemCost { get; set; }

    //    [Display(Name = "CustomerOwnedQuantity", ResourceType = typeof(ResPullDetails))]
    //    [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
    //    public Nullable<System.Double> CustomerOwnedQuantity { get; set; }

    //    [Display(Name = "ConsignedQuantity", ResourceType = typeof(ResPullDetails))]
    //    [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
    //    public Nullable<System.Double> ConsignedQuantity { get; set; }

    //    [Display(Name = "PoolQuantity", ResourceType = typeof(ResPullDetails))]
    //    [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
    //    public Nullable<System.Double> PoolQuantity { get; set; }

    //    [Display(Name = "SerialNumber", ResourceType = typeof(ResPullDetails))]
    //    [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
    //    [AllowHtml]
    //    public System.String SerialNumber { get; set; }

    //    [Display(Name = "LotNumber", ResourceType = typeof(ResPullDetails))]
    //    [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
    //    [AllowHtml]
    //    public System.String LotNumber { get; set; }

    //    [Display(Name = "Expiration", ResourceType = typeof(ResPullDetails))]
    //    [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
    //    [AllowHtml]
    //    public System.String Expiration { get; set; }

    //    [Display(Name = "Received", ResourceType = typeof(ResPullDetails))]
    //    [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
    //    [AllowHtml]
    //    public System.String Received { get; set; }

    //    [Display(Name = "BinID", ResourceType = typeof(ResPullDetails))]
    //    public Nullable<System.Int64> BinID { get; set; }

    //    [Display(Name = "Created", ResourceType = typeof(Resources.ResCommon))]
    //    public Nullable<System.DateTime> Created { get; set; }

    //    [Display(Name = "Updated", ResourceType = typeof(Resources.ResCommon))]
    //    public Nullable<System.DateTime> Updated { get; set; }

    //    [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
    //    public Nullable<System.Int64> CreatedBy { get; set; }

    //    [Display(Name = "LastUpdatedBy", ResourceType = typeof(Resources.ResCommon))]
    //    public Nullable<System.Int64> LastUpdatedBy { get; set; }

    //    public Nullable<Boolean> IsDeleted { get; set; }

    //    public Nullable<Boolean> IsArchived { get; set; }

    //    [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
    //    public Nullable<System.Int64> CompanyID { get; set; }

    //    [Display(Name = "Room", ResourceType = typeof(Resources.ResCommon))]
    //    public Nullable<System.Int64> Room { get; set; }


    //    [Display(Name = "PullCredit", ResourceType = typeof(ResPullDetails))]
    //    [StringLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
    //    [AllowHtml]
    //    public System.String PullCredit { get; set; }


    //    [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
    //    public string RoomName { get; set; }

    //    [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
    //    public string CreatedByName { get; set; }

    //    [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
    //    public string UpdatedByName { get; set; }

    //    public Nullable<Int64> ItemLocationDetailID { get; set; }

    //    public string BinName { get; set; }

    //    public string ItemNumber { get; set; }

    //    public Int32 ItemType { get; set; }

    //    public long StagingBinID { get; set; }

    //    public string StagingBinName { get; set; }
    //    public double ActualAvailableQuantity { get; set; }
    //}

    public class StagingActionResult
    {
        //public long MaterialStagingId { get; set; }
        //public long MaterialStagingDetailsId { get; set; }

        public Guid MaterialStagingGUId { get; set; }
        public Guid MaterialStagingDetailsGUId { get; set; }


        public long BinId { get; set; }
        public string BinName { get; set; }
        public long StagingBinId { get; set; }
        public string StagingBinName { get; set; }
        public float StagedQuantity { get; set; }
        public string ReturnMessage { get; set; }
        public int ReturnCode { get; set; }
        public float CustomerOwnQty { get; set; }
        public float ConsignedQty { get; set; }
    }
}
