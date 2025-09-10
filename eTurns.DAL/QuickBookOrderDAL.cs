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
    public class QuickBookOrderDAL : eTurnsBaseDAL
    {
        public QuickBookOrderDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }
        
        public List<QuickBookOrderDTO> GetQBOrderByCompanyRoomID( Int64 CompanyID, Int64 RoomID,bool? IsProcess)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID)
                                ,new SqlParameter("@RoomID", RoomID)
                                ,new SqlParameter("@IsProcess", IsProcess)};
                return context.Database.SqlQuery<QuickBookOrderDTO>("exec [GetQBOrderByCompanyRoomID] @CompanyID,@RoomID,@IsProcess", params1).ToList();
            }
        }
        public List<QuickBookOrderLineItemsDTO> GetQBOrderLineItemByCompanyRoomID_New(Int64 CompanyID, Int64 RoomID,   Guid OrderGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID)
                                ,new SqlParameter("@RoomID", RoomID)
                              
                                ,new SqlParameter("@OrderGUID", OrderGUID)};
                
                List<QuickBookOrderLineItemsDTO> obj = (from u in context.Database.SqlQuery<QuickBookOrderLineItemsDTO>("exec [GetQBOrderLineItemByCompanyRoomID_New] @CompanyID,@RoomID,@OrderGUID", params1)
                                                            select new QuickBookOrderLineItemsDTO
                                                            {
                                                                ID = u.ID,
                                                                OrderGUID = u.OrderGUID,
                                                                OrderDetailGUID = u.OrderDetailGUID,
                                                                ItemGUID = u.ItemGUID,
                                                                CompanyID = u.CompanyID,
                                                                RoomID = u.RoomID,
                                                                Action = u.Action,
                                                                IsDeleted = u.IsDeleted,
                                                                IsArchived = u.IsArchived,
                                                                RequestedQuantity = u.RequestedQuantity,
                                                                txnDate = u.txnDate,
                                                                Description = u.Description,
                                                                ItemNumber = u.ItemNumber,
                                                                ItemRate = u.ItemRate,
                                                                ItemFullyQualifiedName = u.ItemFullyQualifiedName,
                                                                OrderQuantity = u.OrderQuantity,
                                                                ReceivedQuantity = u.ReceivedQuantity,
                                                                IsCloseItem = u.IsCloseItem
                                                            }).ToList();
                return obj;
            }
        }
        public List<QuickBookOrderDTO> GetQBOrderByCompanyRoomID_New(Int64 CompanyID, Int64 RoomID, bool? IsProcess)
        {
            var params1 = new SqlParameter[] {
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@IsProcess", IsProcess ?? (object)DBNull.Value),
            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<QuickBookOrderDTO> obj = (from u in context.Database.SqlQuery<QuickBookOrderDTO>("Exec [GetQBOrderByCompanyRoomID_New] @CompanyID,@RoomID,@IsProcess", params1)
                                                   select new QuickBookOrderDTO
                                                   {
                                                       ID = u.ID,
                                                       GUID = u.GUID,
                                                       OrderGUID = u.OrderGUID,
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
                                                       QBPOID = u.QBPOID,
                                                       WhatWhereAction = u.WhatWhereAction,
                                                       OrderName = u.OrderName,
                                                       Description = u.Description,

                                                       VenderID = u.VenderID,
                                                       Customer = u.Customer,


                                                       TxnDate = u.TxnDate,
                                                       TotalCost = u.TotalCost,
                                                       TotalPrice = u.TotalPrice,
                                                       OrderStatus = u.OrderStatus,
                                                       CustomerGUID = u.CustomerGUID,
                                                       ShipViaID = u.ShipViaID,
                                                       ShipViaName = u.ShipViaName
                                                   }).ToList();

                return obj;
            }

        }
        
        public bool InsertQuickBookOrder(Guid OrderGUID, Int64 EnterpriseID, Int64 CompanyID, Int64 RoomID,string Actions,bool IsDeleted,Int64 UserID,string AddedFrom,string WhatWhereAction)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@OrderGUID", OrderGUID)
                                ,new SqlParameter("@EnterpriseID", EnterpriseID)
                                ,new SqlParameter("@CompanyID", CompanyID)
                                ,new SqlParameter("@RoomID", RoomID)
                                ,new SqlParameter("@Action", Actions ?? (object)DBNull.Value)
                                ,new SqlParameter("@IsDeleted", IsDeleted)
                                ,new SqlParameter("@UserID", UserID)
                                ,new SqlParameter("@AddedFrom", AddedFrom ?? (object)DBNull.Value)
                                ,new SqlParameter("@WhatWhereAction", WhatWhereAction ?? (object)DBNull.Value)};
                context.Database.ExecuteSqlCommand("exec [InsertQuickBookOrder] @OrderGUID,@EnterpriseID,@CompanyID,@RoomID,@Action,@IsDeleted,@UserID,@AddedFrom,@WhatWhereAction", params1);
                return true;
            }
        }
        public bool InsertOrderQuickBookSetup(Int64 EnterpriseID, Int64 CompanyID, Int64 RoomID, Int64 UserID,string AddedFrom,string WhatWhereAction)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseID", EnterpriseID)
                                ,new SqlParameter("@CompanyID", CompanyID)
                                ,new SqlParameter("@RoomID", RoomID)
                                ,new SqlParameter("@UserID", UserID)
                                ,new SqlParameter("@AddedFrom", AddedFrom ?? (object)DBNull.Value)
                                ,new SqlParameter("@WhatWhereAction", WhatWhereAction ?? (object)DBNull.Value)};
                context.Database.ExecuteSqlCommand("exec [InsertOrderQuickBookSetup] @EnterpriseID,@CompanyID,@RoomID,@UserID,@AddedFrom,@WhatWhereAction", params1);
                return true;
            }
        }
        public bool UpdateQuickBookOrder(QuickBookOrderDTO objQuickBookOrderDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@GUID", objQuickBookOrderDTO.GUID)
                                ,new SqlParameter("@OrderGUID", objQuickBookOrderDTO.OrderGUID)
                                ,new SqlParameter("@IsProcess", objQuickBookOrderDTO.IsProcess)
                                ,new SqlParameter("@IsSuccess", objQuickBookOrderDTO.IsSuccess)
                                ,new SqlParameter("@ErrorDescription", objQuickBookOrderDTO.ErrorDescription ?? (object)DBNull.Value)
                                ,new SqlParameter("@CompanyID", objQuickBookOrderDTO.CompanyID)
                                ,new SqlParameter("@RoomID", objQuickBookOrderDTO.RoomID)
                                ,new SqlParameter("@IsDeleted", objQuickBookOrderDTO.IsDeleted)
                                ,new SqlParameter("@IsArchived", objQuickBookOrderDTO.IsArchived)
                                ,new SqlParameter("@LastUpdated", objQuickBookOrderDTO.LastUpdated)
                                ,new SqlParameter("@LastUpdatedBy", objQuickBookOrderDTO.LastUpdatedBy)
                                ,new SqlParameter("@EditedFrom", objQuickBookOrderDTO.EditedFrom ?? (object)DBNull.Value)
                                ,new SqlParameter("@QuickBookOrderID", objQuickBookOrderDTO.QBPOID ?? (object)DBNull.Value)};
                context.Database.ExecuteSqlCommand("exec [UpdateQuickBookOrder] @GUID,@OrderGUID,@IsProcess,@IsSuccess,@ErrorDescription,@CompanyID,@RoomID,@IsDeleted,@IsArchived,@LastUpdated,@LastUpdatedBy,@EditedFrom,@QuickBookOrderID", params1);
                return true;
            }
        }
        public bool UpdateQuickBookOrder_New(QuickBookOrderDTO objQuickBookOrderDTO, DataTable DTLineItems)
        {
            SqlConnection ChildDbConnection = new SqlConnection(base.DataBaseConnectionString);
            DataSet Ds = SqlHelper.ExecuteDataset(ChildDbConnection, "UpdateQuickBookOrder_New"
                                                 , objQuickBookOrderDTO.GUID
                                                 , objQuickBookOrderDTO.OrderGUID
                                                 , objQuickBookOrderDTO.IsProcess ?? (object)DBNull.Value
                                                 , objQuickBookOrderDTO.IsSuccess ?? (object)DBNull.Value
                                                 , objQuickBookOrderDTO.ErrorDescription ?? (object)DBNull.Value
                                                 , objQuickBookOrderDTO.CompanyID
                                                 , objQuickBookOrderDTO.RoomID
                                                 , objQuickBookOrderDTO.IsDeleted ?? (object)DBNull.Value
                                                 , objQuickBookOrderDTO.IsArchived ?? (object)DBNull.Value
                                                 , objQuickBookOrderDTO.LastUpdated ?? (object)DBNull.Value
                                                 , objQuickBookOrderDTO.LastUpdatedBy
                                                 , objQuickBookOrderDTO.EditedFrom ?? (object)DBNull.Value
                                                 , objQuickBookOrderDTO.QBPOID ?? (object)DBNull.Value
                                                 , objQuickBookOrderDTO.QBJSON ?? (object)DBNull.Value
                                                 , DTLineItems);

            return true;
        }

    }
}
