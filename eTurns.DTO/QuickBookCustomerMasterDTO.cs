using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DTO
{
    public class QuickBookCustomerMasterDTO
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }
        public Guid CustomerGUID { get; set; }
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
        public System.String QBCustomerID { get; set; }
        public System.String WhatWhereAction { get; set; }
        public System.String CustomerName { get; set; }
        public System.String Account { get; set; }
        public System.String Contact { get; set; }
        public System.String Address { get; set; }
        public System.String City { get; set; }
        public System.String State { get; set; }
        public System.String ZipCode { get; set; }
        public System.String Country { get; set; }
        public System.String Phone { get; set; }
        public System.String Email { get; set; }

    }
}
