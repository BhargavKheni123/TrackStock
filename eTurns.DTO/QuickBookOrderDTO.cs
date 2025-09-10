using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DTO
{
    [Serializable]
    public class QuickBookOrderDTO
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }
        public Guid OrderGUID { get; set; }
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
        public System.String QBPOID { get; set; }
        public System.String WhatWhereAction { get; set; }

        public int OrderStatus { get;set;}

        //public System.String OrderNumber { get; set; }
        //public Int64 SupplierId { get; set; }
        //public System.String SupplierName { get; set; }
        //public Int64 CustomerID { get; set; }
        public System.String Customer { get; set; }
        public System.String Vender { get; set; }
        public long VenderID { get;set;}

        public double? TotalCost { get;set;}
        public double? TotalPrice { get; set; }
        //public string Description { get; set; }

        //public Nullable<System.Decimal> TotalCost { get; set; }
        //public Nullable<System.Decimal> TotalPrice { get; set; }

        public List<QuickBookOrderLineItemsDTO> QBOrderLineItems { get; set; }
        public Nullable<System.Decimal> OrderPrice { get;set;}
        
        public System.String OrderName { get; set; }
        public System.String Description { get; set; }
        // public Int64 nameId { get; set; }
        public System.String Vendor { get; set; }
        public System.String BillEmail { get; set; }
        public System.String billingAddress { get; set; }
        public System.String ShipTo { get; set; }
        public System.String ShipAddr { get; set; }
        public System.String ShipViaName { get; set; }

        //public Int64 TechnicianID { get; set; }
        //public System.String customVal1 { get; set; } //Technician
        //public System.String customVal2 { get; set; } //SalesRep
        public Nullable<System.DateTime> TxnDate { get; set; } //Purchase Order Date
                                                               //   public CustomField[] customField { get; set; }

        public System.String QBJSON { get; set; }
        public Guid CustomerGUID { get; set; }

        public Nullable<Int64> ShipViaID { get; set; }
    }
}
