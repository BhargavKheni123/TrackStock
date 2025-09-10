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
    public class QuickBookWorkOrderDAL : eTurnsBaseDAL
    {
        public QuickBookWorkOrderDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }
        public List<QuickBookWorkOrderDTO> GetQBWorkOrderByCompanyRoomID(Int64 CompanyID, Int64 RoomID, bool? IsProcess)
        {
            var params1 = new SqlParameter[] {
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@IsProcess", IsProcess ?? (object)DBNull.Value),
            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<QuickBookWorkOrderDTO> obj = (from u in context.Database.SqlQuery<QuickBookWorkOrderDTO>("Exec [GetQBWorkOrderByCompanyRoomID] @CompanyID,@RoomID,@IsProcess", params1)
                                                   select new QuickBookWorkOrderDTO
                                                   {
                                                       ID = u.ID,
                                                       GUID = u.GUID,
                                                       WorkOrderGUID = u.WorkOrderGUID,
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
                                                       WhatWhereAction = u.WhatWhereAction,
                                                       WOName = u.WOName,
                                                       Description = u.Description,
                                                       TechnicianID = u.TechnicianID,
                                                       Technician = u.Technician,
                                                       CustomerID = u.CustomerID,
                                                       Customer = u.Customer,
                                                       SupplierId = u.SupplierId,
                                                       SupplierName = u.SupplierName,
                                                       WOStatus = u.WOStatus,
                                                       InvoiceDate = u.InvoiceDate,
                                                       TotalCost = u.TotalCost,
                                                       TotalPrice = u.TotalPrice
                                                   }).ToList();

                return obj;
            }

        }

        public List<QuickBookWorkOrderDTO> GetQBWorkOrderByCompanyRoomID_New(Int64 CompanyID, Int64 RoomID, bool? IsProcess)
        {
            var params1 = new SqlParameter[] {
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@IsProcess", IsProcess ?? (object)DBNull.Value),
            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<QuickBookWorkOrderDTO> obj = (from u in context.Database.SqlQuery<QuickBookWorkOrderDTO>("Exec [GetQBWorkOrderByCompanyRoomID_New] @CompanyID,@RoomID,@IsProcess", params1)
                                                   select new QuickBookWorkOrderDTO
                                                   {
                                                       ID = u.ID,
                                                       GUID = u.GUID,
                                                       WorkOrderGUID = u.WorkOrderGUID,
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
                                                       WhatWhereAction = u.WhatWhereAction,
                                                       WOName = u.WOName,
                                                       Description = u.Description,
                                                       TechnicianID = u.TechnicianID,
                                                       Technician = u.Technician,
                                                       CustomerID = u.CustomerID,
                                                       Customer = u.Customer,
                                                       SupplierId = u.SupplierId,
                                                       SupplierName = u.SupplierName,
                                                       WOStatus = u.WOStatus,
                                                       InvoiceDate = u.InvoiceDate,
                                                       TotalCost = u.TotalCost,
                                                       TotalPrice = u.TotalPrice,
                                                       WorkOrderID = u.WorkOrderID
                                                   }).ToList();

                return obj;
            }

        }

        public List<QuickBookWorkOrderDTO> GetQuickBookWorkOrderMappingByWorkOrderGUID(Guid WorkOrderGUID,Int64 CompanyID,Int64 RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@WorkOrderGUID", WorkOrderGUID) 
                                ,new SqlParameter("@CompanyID", CompanyID)
                                ,new SqlParameter("@RoomID", RoomID)};
                return context.Database.SqlQuery<QuickBookWorkOrderDTO>("exec [GetQuickBookWorkOrderMappingByWorkOrderGUID] @WorkOrderGUID,@CompanyID,@RoomID", params1).ToList();
            }
        }
        public bool InsertQuickBookWorkOrder(Guid WorkOrderGUID, Int64 EnterpriseID, Int64 CompanyID, Int64 RoomID,string Actions,bool IsDeleted,Int64 UserID,string AddedFrom,string WhatWhereAction)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@WorkOrderGUID", WorkOrderGUID)
                                ,new SqlParameter("@EnterpriseID", EnterpriseID)
                                ,new SqlParameter("@CompanyID", CompanyID)
                                ,new SqlParameter("@RoomID", RoomID)
                                ,new SqlParameter("@Action", Actions ?? (object)DBNull.Value)
                                ,new SqlParameter("@IsDeleted", IsDeleted)
                                ,new SqlParameter("@UserID", UserID)
                                ,new SqlParameter("@AddedFrom", AddedFrom ?? (object)DBNull.Value)
                                ,new SqlParameter("@WhatWhereAction", WhatWhereAction ?? (object)DBNull.Value)};
                context.Database.ExecuteSqlCommand("exec [InsertQuickBookWorkOrder] @WorkOrderGUID,@EnterpriseID,@CompanyID,@RoomID,@Action,@IsDeleted,@UserID,@AddedFrom,@WhatWhereAction", params1);
                return true;
            }
        }
        public bool InsertWorkOrderQuickBookSetup(Int64 EnterpriseID, Int64 CompanyID, Int64 RoomID, Int64 UserID,string AddedFrom,string WhatWhereAction)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseID", EnterpriseID)
                                ,new SqlParameter("@CompanyID", CompanyID)
                                ,new SqlParameter("@RoomID", RoomID)
                                ,new SqlParameter("@UserID", UserID)
                                ,new SqlParameter("@AddedFrom", AddedFrom ?? (object)DBNull.Value)
                                ,new SqlParameter("@WhatWhereAction", WhatWhereAction ?? (object)DBNull.Value)};
                context.Database.ExecuteSqlCommand("exec [InsertWorkOrderQuickBookSetup] @EnterpriseID,@CompanyID,@RoomID,@UserID,@AddedFrom,@WhatWhereAction", params1);
                return true;
            }
        }
        public bool UpdateQuickBookWorkOrder(QuickBookWorkOrderDTO quickBookWorkOrder,DataTable DTLineItems)
            //Guid GUID, Guid WorkOrderGUID, bool IsProcess, bool IsSuccess, string ErrorDescription, Int64 CompanyID,Int64 RoomID,bool IsDeleted,bool IsArchived,DateTime LastUpdated,Int64 LastUpdatedBy,string EditedFrom,string QuickBookWorkOrderID)
        {
            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{
                //var params1 = new SqlParameter[] { new SqlParameter("@GUID", quickBookWorkOrder.GUID)
                //                ,new SqlParameter("@WorkOrderGUID", quickBookWorkOrder.WorkOrderGUID)
                //                ,new SqlParameter("@IsProcess", quickBookWorkOrder.IsProcess ?? (object)DBNull.Value)
                //                ,new SqlParameter("@IsSuccess", quickBookWorkOrder.IsSuccess ?? (object)DBNull.Value)
                //                ,new SqlParameter("@ErrorDescription", quickBookWorkOrder.ErrorDescription ?? (object)DBNull.Value)
                //                ,new SqlParameter("@CompanyID", quickBookWorkOrder.CompanyID)
                //                ,new SqlParameter("@RoomID", quickBookWorkOrder.RoomID)
                //                ,new SqlParameter("@IsDeleted", quickBookWorkOrder.IsDeleted ?? (object)DBNull.Value)
                //                ,new SqlParameter("@IsArchived", quickBookWorkOrder.IsArchived ?? (object)DBNull.Value)
                //                ,new SqlParameter("@LastUpdated", quickBookWorkOrder.LastUpdated ?? (object)DBNull.Value)
                //                ,new SqlParameter("@LastUpdatedBy", quickBookWorkOrder.LastUpdatedBy)
                //                ,new SqlParameter("@EditedFrom", quickBookWorkOrder.EditedFrom ?? (object)DBNull.Value)
                //                ,new SqlParameter("@QuickBookWorkOrderID", quickBookWorkOrder.QBInvoiceID ?? (object)DBNull.Value)};
                //context.Database.ExecuteSqlCommand("exec [UpdateQuickBookWorkOrder] @GUID,@WorkOrderGUID,@IsProcess,@IsSuccess,@ErrorDescription,@CompanyID,@RoomID,@IsDeleted,@IsArchived,@LastUpdated,@LastUpdatedBy,@EditedFrom,@QuickBookWorkOrderID", params1);

                SqlConnection ChildDbConnection = new SqlConnection(base.DataBaseConnectionString);
                DataSet Ds = SqlHelper.ExecuteDataset(ChildDbConnection, "UpdateQuickBookWorkOrder"
                                                     , quickBookWorkOrder.GUID
                                                     , quickBookWorkOrder.WorkOrderGUID
                                                     , quickBookWorkOrder.IsProcess ?? (object)DBNull.Value
                                                     , quickBookWorkOrder.IsSuccess ?? (object)DBNull.Value
                                                     , quickBookWorkOrder.ErrorDescription ?? (object)DBNull.Value
                                                     , quickBookWorkOrder.CompanyID
                                                     , quickBookWorkOrder.RoomID
                                                     , quickBookWorkOrder.IsDeleted ?? (object)DBNull.Value
                                                     , quickBookWorkOrder.IsArchived ?? (object)DBNull.Value
                                                     , quickBookWorkOrder.LastUpdated ?? (object)DBNull.Value
                                                     , quickBookWorkOrder.LastUpdatedBy
                                                     , quickBookWorkOrder.EditedFrom ?? (object)DBNull.Value
                                                     , quickBookWorkOrder.QBInvoiceID ?? (object)DBNull.Value
                                                     , DTLineItems);

                return true;
            //}
        }

        public List<QuickBookWorkOrderLineItemsDTO> GetQBWorkOrderLineItemByCompanyRoomID_New(Int64 CompanyID, Int64 RoomID, Guid WorkOrderGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID)
                                ,new SqlParameter("@RoomID", RoomID)
                                ,new SqlParameter("@WorkOrderGUID", WorkOrderGUID)};
                List<QuickBookWorkOrderLineItemsDTO> obj = (from u in context.Database.SqlQuery<QuickBookWorkOrderLineItemsDTO>("exec [GetQBWorkOrderLineItemByCompanyRoomID_New] @CompanyID,@RoomID,@WorkOrderGUID", params1)
                                                            select new QuickBookWorkOrderLineItemsDTO
                                                            {
                                                                ID = u.ID,
                                                                WorkOrderGUID = u.WorkOrderGUID,
                                                                PullGUID = u.PullGUID,
                                                                ItemGUID = u.ItemGUID,
                                                                CompanyID = u.CompanyID,
                                                                RoomID = u.RoomID,
                                                                Action = u.Action,
                                                                IsDeleted = u.IsDeleted,
                                                                IsArchived = u.IsArchived,
                                                                //Created = u.Created,
                                                                //LastUpdated = u.LastUpdated,
                                                                //CreatedBy = u.CreatedBy,
                                                                //LastUpdatedBy = u.LastUpdatedBy,
                                                                //AddedFrom = u.AddedFrom,
                                                                //EditedFrom = u.EditedFrom,
                                                                //QBInvoiceID = u.QBInvoiceID,
                                                                //QBInvoiceLineID = u.QBInvoiceLineID,
                                                                //QuickBookItemID = u.QuickBookItemID,
                                                                //WhatWhereAction = u.WhatWhereAction,
                                                                ConsignedQuantity = u.ConsignedQuantity,
                                                                CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                                PULLCost = u.PULLCost,
                                                                PullPrice = u.PullPrice,
                                                                PoolQuantity = u.PoolQuantity,
                                                                WorkOrderDetailGUID = u.WorkOrderDetailGUID,
                                                                txnDate = u.txnDate,
                                                                Description = u.Description,
                                                                Itemnumber = u.Itemnumber,
                                                                ItemRate = u.ItemRate,
                                                                ItemFullyQualifiedName = u.ItemFullyQualifiedName,
                                                                ItemTaxable = u.ItemTaxable
                                                            }).ToList();
                return obj;
            }
        }

        public bool UpdateQuickBookWorkOrder_New(QuickBookWorkOrderDTO quickBookWorkOrder, DataTable DTLineItems)       
        {
            SqlConnection ChildDbConnection = new SqlConnection(base.DataBaseConnectionString);
            DataSet Ds = SqlHelper.ExecuteDataset(ChildDbConnection, "UpdateQuickBookWorkOrder_New"
                                                 , quickBookWorkOrder.GUID
                                                 , quickBookWorkOrder.WorkOrderGUID
                                                 , quickBookWorkOrder.IsProcess ?? (object)DBNull.Value
                                                 , quickBookWorkOrder.IsSuccess ?? (object)DBNull.Value
                                                 , quickBookWorkOrder.ErrorDescription ?? (object)DBNull.Value
                                                 , quickBookWorkOrder.CompanyID
                                                 , quickBookWorkOrder.RoomID
                                                 , quickBookWorkOrder.IsDeleted ?? (object)DBNull.Value
                                                 , quickBookWorkOrder.IsArchived ?? (object)DBNull.Value
                                                 , quickBookWorkOrder.LastUpdated ?? (object)DBNull.Value
                                                 , quickBookWorkOrder.LastUpdatedBy
                                                 , quickBookWorkOrder.EditedFrom ?? (object)DBNull.Value
                                                 , quickBookWorkOrder.QBInvoiceID ?? (object)DBNull.Value
                                                 , quickBookWorkOrder.QBJSON ?? (object)DBNull.Value
                                                 , DTLineItems);

            return true;
            //}
        }

    }
}
