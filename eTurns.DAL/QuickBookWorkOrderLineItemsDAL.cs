using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTurns.DTO;
using System.Data;
using Dynamite;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Collections;
using System.Data.SqlClient;
using eTurns.DTO.Resources;
using System.Web;
using System.Transactions;

namespace eTurns.DAL
{
    public class QuickBookWorkOrderLineItemsDAL : eTurnsBaseDAL
    {
        public QuickBookWorkOrderLineItemsDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        public List<QuickBookWorkOrderLineItemsDTO> GetQBWorkOrderLineItemByCompanyRoomID(Int64 CompanyID,Int64 RoomID,bool? IsProcess,Guid WorkOrderGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID) 
                                ,new SqlParameter("@RoomID", RoomID)
                                ,new SqlParameter("@IsProcess", IsProcess)
                                ,new SqlParameter("@WorkOrderGUID", WorkOrderGUID)};
                List<QuickBookWorkOrderLineItemsDTO> obj = (from u in context.Database.SqlQuery<QuickBookWorkOrderLineItemsDTO>("exec [GetQBWorkOrderLineItemByCompanyRoomID] @CompanyID,@RoomID,@IsProcess,@WorkOrderGUID", params1)
                                                   select new QuickBookWorkOrderLineItemsDTO
                                                   {
                                                       ID = u.ID,
                                                       GUID = u.GUID,
                                                       WorkOrderGUID = u.WorkOrderGUID,
                                                       PullGUID = u.PullGUID,
                                                       ItemGUID = u.ItemGUID,
                                                       IsProcess = u.IsProcess,
                                                       IsSuccess = u.IsSuccess,
                                                       ErrorDescription = u.ErrorDescription,
                                                       CompanyID = u.CompanyID,
                                                       RoomID = u.RoomID,
                                                       Action = u.Action,
                                                       IsDeleted = u.IsDeleted,
                                                       IsArchived = u.IsArchived,
                                                       Created = u.Created,
                                                       LastUpdated = u.LastUpdated,
                                                       CreatedBy = u.CreatedBy,
                                                       LastUpdatedBy = u.LastUpdatedBy,
                                                       AddedFrom = u.AddedFrom,
                                                       EditedFrom = u.EditedFrom,
                                                       QBInvoiceID = u.QBInvoiceID,
                                                       QBInvoiceLineID = u.QBInvoiceLineID,
                                                       QuickBookItemID = u.QuickBookItemID,
                                                       WhatWhereAction = u.WhatWhereAction,
                                                       ConsignedQuantity = u.ConsignedQuantity,
                                                       CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                       PULLCost = u.PULLCost,
                                                       PullPrice = u.PullPrice,
                                                       PoolQuantity = u.PoolQuantity,
                                                       WorkOrderDetailGUID = u.WorkOrderDetailGUID,
                                                       txnDate = u.txnDate,
                                                       Description = u.Description,
                                                       Itemnumber= u.Itemnumber,
                                                       ItemRate = u.ItemRate
                                                   }).ToList();
                return obj;
            }
        }
        public List<QuickBookWorkOrderLineItemsDTO> GetQuickBookWorkOrderLineItemMappingByWorkOrderGUID(Guid WorkOrderGUID, Int64 CompanyID, Int64 RoomID, Guid PullGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@WorkOrderGUID", WorkOrderGUID)
                                ,new SqlParameter("@CompanyID", CompanyID)
                                ,new SqlParameter("@RoomID", RoomID)
                                ,new SqlParameter("@PullGUID", PullGUID)};
                return context.Database.SqlQuery<QuickBookWorkOrderLineItemsDTO>("exec [GetQuickBookWorkOrderLineItemMappingByWorkOrderGUID] @WorkOrderGUID,@CompanyID,@RoomID,@PullGUID", params1).ToList();
            }
        }
    }
}
