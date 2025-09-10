using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DTO
{
    [Serializable]
    public class QuickBookWorkOrderDTO
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }
        public Guid WorkOrderGUID { get; set; }
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
        public System.String QBInvoiceID { get; set; }
        public System.String WhatWhereAction { get; set; }
        public System.String WOName { get; set; }
        public Int64 TechnicianID { get; set; }
        public System.String Technician { get; set; }
        public Int64 SupplierId { get; set; }
        public System.String SupplierName { get; set; }
        public Int64 CustomerID { get; set; }
        public System.String Customer { get; set; }
        public System.String WOStatus { get; set; }
        public System.String BillingAddress { get; set; }
        public Nullable<DateTime> InvoiceDate { get; set; }
        public Nullable<DateTime> DueDate { get; set; }

        public string Description { get; set; }

        public Nullable<System.Double> TotalCost { get; set; }
        public Nullable<System.Double> TotalPrice { get; set; }
        public System.String QBJSON { get; set; }

        public System.String WOImageName { get; set; }

        public Int64 WorkOrderID { get; set; }
        public List<QuickBookWorkOrderLineItemsDTO> QBLineItems { get; set; }               
    }
}
