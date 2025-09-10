using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using eTurns.DTO.Resources;
using System.Web;
using System.Web.Mvc;

namespace eTurns.DTO
{
    [Serializable]
    public class QuickBookItemDTO
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }
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

        public System.String ItemNumber { get; set; }
        public System.String ItemUniqueNumber { get; set; }
        public System.String Description { get; set; }
        public System.String LongDescription { get; set; }
        public Nullable<System.Int64> CategoryID { get; set; }
        public System.String CategoryName { get; set; }
        public Nullable<System.Double> Cost { get; set; }
        public Nullable<System.Double> SellPrice { get; set; }

        public Nullable<System.Double> MinimumQuantity { get; set; }
        public Nullable<System.Double> MaximumQuantity { get; set; }
        public Nullable<System.Double> CriticalQuantity { get; set; }
        public Nullable<System.Double> OnOrderQuantity { get; set; }
        public Nullable<System.Double> OnHandQuantity { get; set; }
        public System.String QuickBookItemID { get; set; }

        public System.String WhatWhereAction { get; set; }
        public System.String ImagePath { get; set; }
        public System.String ItemImageExternalURL { get; set; }
        public System.String ImageType { get; set; }
        public Int64 ItemMasterID { get; set; }

        public System.String ItemFullyQualifiedName { get; set; }

        public System.String QBJSON { get; set; }

        public System.Boolean Taxable { get; set; }
    }
}
