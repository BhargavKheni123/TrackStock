using eTurns.DTO;
using System.Collections.Generic;

namespace eTurnsWeb.Models
{
    public class WorkOrderModel
    {
        public WorkOrderDTO WorkOrderDTO { get; set; }
        public bool ViewOnly { get; set; }
        //public object UDFs { get; set; }
        public List<CustomerMasterDTO> CustomerBAG { get; set; }
        public List<TechnicianMasterDTO> TechnicianBAG { get; set; }
        //public IEnumerable<GXPRConsigmentJobMasterDTO> GXPRConsigmentBAG { get; set; }
        public List<AssetMasterDTO> AssetBAG { get; set; }
        public List<ToolMasterDTO> ToolBAG { get; set; }
        //public List<JobTypeMasterDTO> JobTypeBAG { get; set; }
        public List<CommonDTO> WOStatusBag { get; set; }
        //public List<CommonDTO> WOTypeBag { get; set; }
        public List<SupplierMasterDTO> SupplierList { get; set; }
        public IEnumerable<SupplierAccountDetailsDTO> SupplierAccount { get; set; }

        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
    }
}