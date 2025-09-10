using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DTO
{
    [Serializable]
    public class QuickBookWorkOrderLineItemsDTO
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }
        public Guid WorkOrderGUID { get; set; }
        public Guid PullGUID { get; set; }
        public Guid ItemGUID { get; set; }
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
        public System.String WhatWhereAction { get; set; }
        public System.String QBInvoiceID { get; set; }
        public System.String QBInvoiceLineID { get; set; }
        public System.String QuickBookItemID { get; set; }
        public System.String Description { get; set; }
        public Nullable<System.Double> Quantity { get; set; }
        public System.String Itemnumber { get; set; }

        public Nullable<System.Double> ItemRate { get; set; }
        public Nullable<System.Double> ItemAmount { get; set; }

        
        public Nullable<Double> ConsignedQuantity { get;set;}
        public Nullable<Double> CustomerOwnedQuantity { get; set; }
        public Nullable<Double> PULLCost { get; set; }
        public Nullable<Double> PullPrice { get; set; }
        public Nullable<Double> PoolQuantity { get; set; }
        public Guid? WorkOrderDetailGUID { get; set; }
        public Nullable<DateTime> txnDate { get;set;}
        public System.String ItemFullyQualifiedName { get; set; }

        public Nullable<Boolean> ItemTaxable { get; set; }

    }
}
