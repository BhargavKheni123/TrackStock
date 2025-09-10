using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.DTO
{
    public class PullRequestDTO
    {
        public long ID { get; set; }
        public Guid PullGUID { get; set; }
        public Guid ItemGUID { get; set; }
        public long BinID { get; set; }
        public long UserID { get; set; }
        public double PullQuantity { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string WhatWhereAction { get; set; }
        public string ProjectName { get; set; }
        public string PullOrderNumber { get; set; }
        public string AddedFrom { get; set; }
        public string EditedFrom { get; set; }
        public Guid WorkOrderGUID { get; set; }
        public Guid RequisitionDetailGUID { get; set; }
        public string ControlNumber { get; set; }

        public List<PullDetailRequestDTO> lstPullDetail { get; set; }
    }
    public class PullDetailRequestDTO
    {
        public double ConsignedQuantity { get; set; }
        public double CustomerOwnedQuantity { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string LotNumber { get; set; }
        public string SerialNumber { get; set; }
        public double PullQuantity { get; set; }
    }


    public class PullResponseDTO
    {
        public long ID { get; set; }
        public Guid PullGUID { get; set; }
        public Guid ItemGUID { get; set; }
        public long BinID { get; set; }
        public long UserID { get; set; }
        public double PullQuantity { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string WhatWhereAction { get; set; }
        public string ProjectName { get; set; }
        public string PullOrderNumber { get; set; }
        public string AddedFrom { get; set; }
        public string EditedFrom { get; set; }
        public Guid WorkOrderGUID { get; set; }
        public Guid RequisitionDetailGUID { get; set; }
        public string ControlNumber { get; set; }
        public List<PullDetailResponseDTO> lstPullDetail { get; set; }
    }
    public class PullDetailResponseDTO
    {
        public double ConsignedQuantity { get; set; }
        public double CustomerOwnedQuantity { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string LotNumber { get; set; }
        public string SerialNumber { get; set; }
    }
}
