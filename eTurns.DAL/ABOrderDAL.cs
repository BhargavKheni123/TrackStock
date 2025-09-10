using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTurns.DTO;

namespace eTurns.DAL
{
    public class ABOrderDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        public ABOrderDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        #endregion

        #region Methods
        
        public void InsertABOrder(InsertABOrderDTO ABOrder)
        {
            var params1 = new SqlParameter[] { 
                                                new SqlParameter("@OrderId", ABOrder.OrderId),
                                                new SqlParameter("@RoomId", ABOrder.RoomId),
                                                new SqlParameter("@CompanyId", ABOrder.CompanyId),
                                                new SqlParameter("@OrderJson", ABOrder.OrderJson ?? (object)DBNull.Value),
                                                new SqlParameter("@OrderCreated", ABOrder.OrderCreated ?? (object)DBNull.Value),
                                                new SqlParameter("@OrderLastUpdated", ABOrder.OrderLastUpdated ?? (object)DBNull.Value),
                                                new SqlParameter("@OrderJsonEncrypted", ABOrder.OrderJsonEncrypted ?? (object)DBNull.Value), 
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("exec [InsertABOrder] @OrderId,@RoomId,@CompanyId,@OrderJson,@OrderCreated,@OrderLastUpdated,@OrderJsonEncrypted", params1);
            }
        }

        public InsertABOrderDTO GetABOrderByOrderIdAndRoom(string OrderId, long RoomId, long CompanyId)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@OrderId", OrderId),
                                                new SqlParameter("@RoomId", RoomId),
                                                new SqlParameter("@CompanyId", CompanyId)
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<InsertABOrderDTO>("exec [GetABOrderByOrderIdAndRoom] @OrderId,@RoomId,@CompanyId", params1).FirstOrDefault();
            }
        }

        public List<InsertABOrderDTO> GetABOrdersByRoomId(long RoomId, long CompanyId)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@RoomId", RoomId),
                                                new SqlParameter("@CompanyId", CompanyId)
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<InsertABOrderDTO>("exec [GetABOrdersByRoomId] @RoomId,@CompanyId", params1).ToList();
            }
        }

        public long InsertABOrderSyncRequest(DateTime StartDate, DateTime EndDate,byte Mode, long EnterpriseId,long CompanyId, long RoomId, long UserId)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@StartDate", StartDate),
                                               new SqlParameter("@EndDate", EndDate),
                                               new SqlParameter("@Mode", Mode),
                                               new SqlParameter("@EnterpriseId", EnterpriseId),
                                               new SqlParameter("@CompanyId", CompanyId),
                                               new SqlParameter("@RoomId", RoomId),
                                               new SqlParameter("@UserId", UserId)
                                             };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<long>("exec [InsertABOrderSyncRequest] @StartDate,@EndDate,@Mode,@EnterpriseId,@CompanyId,@RoomId,@UserId", params1).FirstOrDefault();
                //return context.Database.SqlQuery<ZohoReponseDTO>("exec [" + DbConnectionHelper.GeteTurnsLoggingDBName() + "].[dbo].[UpdateABECRCreateWithUserIDRoleID] @ID,@RoleID,@UserID", params1).FirstOrDefault();
            }
        }

        public bool GetStatusOfABOrdersSyncOrNotById(long Id)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@Id", Id) };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<bool>("exec [GetStatusOfABOrdersSyncOrNotById] @Id", params1).FirstOrDefault();
            }
        }

        public List<SyncABOrderRequestDTO> GetABOrdersToSync()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SyncABOrderRequestDTO>("exec [GetABOrdersToSync] ").ToList();
            }
        }

        public void SetSyncABOrderRequestCompletedWithSuccess(long ID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("exec [" + DbConnectionHelper.GeteTurnsLoggingDBName() + "].[dbo].[SetSyncABOrderRequestCompletedWithSuccess] @ID", params1);
            }
        }

        public void SetSyncABOrderRequestCompletedWithError(long ID, string ErrorException)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@ErrorException", ErrorException ?? (object)DBNull.Value)};

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("exec [" + DbConnectionHelper.GeteTurnsLoggingDBName() + "].[dbo].[SetSyncABOrderRequestCompletedWithError] @ID,@ErrorException", params1);
            }
        }

        #endregion
    }
}
