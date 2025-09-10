using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DTO
{
    [Serializable]
    public class QuickBookOrderLineItemsDTO
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }
        public Guid OrderGUID { get; set; }
        public Guid OrderDetailGUID { get; set; }
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
        public Nullable<DateTime> txnDate { get; set; }
        
        public Nullable<DateTime> LastUpdated { get; set; }
        public Int64 CreatedBy { get; set; }
        public Int64 LastUpdatedBy { get; set; }
        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }       
        public System.String QBPOID { get; set; }
        public System.String QuickBookItemID { get; set; }
        public System.String WhatWhereAction { get; set; }
        public System.String ItemNumber { get; set; }
        public System.String Customer { get; set; }
        public Nullable<System.Double> ItemRate { get; set; }
        public Nullable<System.Double> ItemAmount { get; set; }

        public Nullable<System.Double> RequestedQuantity { get;set;}
        public Nullable<System.Double> ItemSellPrice { get; set; }
        
        //public System.String Description { get; set; }
        //public Nullable<System.Double> RequestedQuantity { get; set; }
        //public Nullable<System.Double> ItemCost { get; set; }
        //public Nullable<System.Double> ItemRate { get; set; }
        //public Nullable<Double> ConsignedQuantity { get;set;}
        //public Nullable<Double> CustomerOwnedQuantity { get; set; }
        //public Nullable<Double> OrderCost { get; set; }
        //public Nullable<Double> OrderPrice { get; set; }
        //public Nullable<Double> ReceivedQuantity { get; set; }
        //public Guid? OrderDetailGUID { get; set; }
        //public Nullable<DateTime> txnDate { get;set;}
        //public System.String OrdNumber { get;set;}

        public string QBId { get; set; }
        //
        // Summary:
        //     Product: QBW Description: Specifies the position of the line in the collection
        //     of transaction lines. Supported only for QuickBooks Windows desktop.
        public string LineNum { get; set; }
        //
        // Summary:
        //     Product: QBO Description: Free form text description of the line item that appears
        //     in the printed record.[br /]Max. length: 4000 characters.[br /]Not supported
        //     for BillPayment or Payment. Product: QBW Description: Free form text description
        //     of the line item that appears in the printed record. Max. length: 4000 characters.
        public string Description { get; set; } //item description
        //
        // Summary:
        //     Product: QBW Description: The amount of the line, which depends on the type of
        //     the line. It can represent the discount amount, charge amount, tax amount, or
        //     subtotal amount based on the line type detail. Product: QBO Description: The
        //     amount of the line depending on the type of the line. It can represent the discount
        //     amount, charge amount, tax amount, or subtotal amount based on the line type
        //     detail.[br /]Required for BillPayment, Check, Estimate, Invoice, JournalEntry,
        //     Payment, SalesReceipt. Required: QBO
        public decimal Amount { get; set; }  //orderdetail price
        public System.String ItemFullyQualifiedName { get; set; }

        public Nullable<System.Double> OrderQuantity { get; set; }
        public Nullable<System.Double> ReceivedQuantity { get; set; }

        public Nullable<System.Boolean> IsCloseItem { get; set; }
    }
    
}
