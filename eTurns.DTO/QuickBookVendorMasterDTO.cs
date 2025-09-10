using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DTO
{
    public class QuickBookVendorMasterDTO
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }
        public Guid VendorGUID { get; set; }
        public Nullable<Boolean> IsProcess { get; set; }
        public Nullable<Boolean> IsSuccess { get; set; }
        public System.String ErrorDescription { get; set; }
        public Int64 CompanyID { get; set; }
        public Int64 RoomID { get; set; }
        public string Action { get; set; }
        public Nullable<Boolean> IsDeleted { get; set; }
        public Nullable<Boolean> IsArchived { get; set; }
        public Nullable<DateTime> Created { get; set; }
        public Nullable<DateTime> LastUpdated { get; set; }
        public Int64 CreatedBy { get; set; }
        public Int64 LastUpdatedBy { get; set; }
        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
        public System.String QBVendorID { get; set; }
        public System.String WhatWhereAction { get; set; }
    }
}
