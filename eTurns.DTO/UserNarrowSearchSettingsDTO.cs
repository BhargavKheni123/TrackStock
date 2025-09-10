using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace eTurns.DTO
{
    public class UserNarrowSearchSettingsDTO
    {
        public long ID { get; set; }
        public long UserID { get; set; }
        public long RoomID { get; set; }
        public long CompanyID { get; set; }
        public long EnterpriseID { get; set; }
        public string ListName { get; set; }
        public string SettingsJson { get; set; }
        //public DateTime? Created { get; set; }
        //public DateTime? Updated { get; set; }
        public long? CreatedBy { get; set; }
        //public long? LastUpdatedBy { get; set; }
    }

    public class SaveUserNarrowSearchRespDTO
    {
        public string Status { get; set; }
        public long ID { get; set; }
    }

    public class NarrowSearchValues
    {
        public string field { get; set; }
        public string ctlId { get; set; }
        public List<string> arrVal { get; set; }
        public string val { get; set; }
        public string ctlType { get; set; }
    }

    //public class NarrowSearchValues
    //{
    //    public string GlobalSearch { get; set; }
    //    public string PullSupplier { get; set; }
    //    public string PullCategory { get; set; }
    //    public string Manufacturer { get; set; }
    //    public string ItemLocation { get; set; }
    //    public string ItemTrackingType { get; set; }
    //    public string StockStatus { get; set; }
    //    public string IsActive { get; set; }
    //    public string PullCost { get; set; }
    //    public string AverageUsage { get; set; }
    //    public string Turns { get; set; }
    //    public string ItemTypeNarroDDL { get; set; }
    //    public string InventoryClassificationDDL { get; set; }
    //    public string UserCreated { get; set; }
    //    public string UserUpdated { get; set; }
    //    public NarrowSearchDateCreated DateCreated { get; set; }
    //    public NarrowSearchDateUpdated DateUpdated { get; set; }
    //    public string UDF1 { get; set; }
    //    public string UDF2 { get; set; }
    //    public string UDF3 { get; set; }
    //    public string UDF4 { get; set; }
    //    public string UDF5 { get; set; }
    //    public string UDF6 { get; set; }
    //    public string UDF7 { get; set; }
    //    public string UDF8 { get; set; }
    //    public string UDF9 { get; set; }
    //    public string UDF10 { get; set; }
    //}

    //public class NarrowSearchDateCreated
    //{
    //    public string DateCFrom { get; set; }
    //    public string DateCTo { get; set; }
    //}

    //public class NarrowSearchDateUpdated
    //{
    //    public string DateUFrom { get; set; }
    //    public string DateUTo { get; set; }
    //}

}
