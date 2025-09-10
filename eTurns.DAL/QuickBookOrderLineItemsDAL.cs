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
    public class QuickBookOrderLineItemsDAL : eTurnsBaseDAL
    {
        public QuickBookOrderLineItemsDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }
        public List<QuickBookOrderLineItemsDTO> GetQuickBookOrderLineItemMappingByOrderGUID(Guid OrderGUID, Int64 CompanyID, Int64 RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@OrderGUID", OrderGUID)
                                ,new SqlParameter("@CompanyID", CompanyID)
                                ,new SqlParameter("@RoomID", RoomID)};
                return context.Database.SqlQuery<QuickBookOrderLineItemsDTO>("exec [GetQuickBookOrderLineItemMappingByOrderGUID] @OrderGUID,@CompanyID,@RoomID", params1).ToList();
            }
        }
        public List<QuickBookOrderLineItemsDTO> GetQBOrderLineItemByCompanyRoomID(Int64 CompanyID,Int64 RoomID,bool? IsProcess,Guid OrderGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID) 
                                ,new SqlParameter("@RoomID", RoomID)
                                ,new SqlParameter("@IsProcess", IsProcess)
                                ,new SqlParameter("@OrderGUID", OrderGUID)};
                return context.Database.SqlQuery<QuickBookOrderLineItemsDTO>("exec [GetQBOrderLineItemByCompanyRoomID] @CompanyID,@RoomID,@IsProcess,@OrderGUID", params1).ToList();
            }
        }
        public List<QuickBookOrderLineItemsDTO> GetQuickBookOrderMappingByOrderGUID(Guid OrderGUID, Int64 CompanyID, Int64 RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@OrderGUID", OrderGUID)
                                ,new SqlParameter("@CompanyID", CompanyID)
                                ,new SqlParameter("@RoomID", RoomID) };
                return context.Database.SqlQuery<QuickBookOrderLineItemsDTO>("exec [GetQuickBookOrderMappingByOrderGUID] @OrderGUID,@CompanyID,@RoomID", params1).ToList();
            }
        }
    }
}
