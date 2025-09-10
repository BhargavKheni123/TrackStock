using eTurns.DTO;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public partial class InventoryCountDetailDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        //public InventoryCountDetailDAL(base.DataBaseName)
        //{

        //}

        public InventoryCountDetailDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public InventoryCountDetailDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]
        public InventoryCountDetailDTO GetInventoryDetailByGUIDPlain(Guid IGuid, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                      new SqlParameter("@IGuid", IGuid)
                    , new SqlParameter("@RoomID", RoomID)
                    , new SqlParameter("@CompanyID", CompanyID)
                };
                return context.Database.SqlQuery<InventoryCountDetailDTO>("exec GetInventoryDetailByGUIDPlain  @IGuid,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public int GetUnAppliedInventoryDetailItemCount(long RoomId, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                      new SqlParameter("@RoomID", RoomId)
                    , new SqlParameter("@CompanyID", CompanyID)
                };
                //return context.Database.SqlQuery<InventoryCountDetailDTO>("exec GetUnAppliedInventoryDetailItemCount @RoomID,@CompanyID", params1).ToList().Count();
                int cnt = context.Database.SqlQuery<int>("exec GetUnAppliedInventoryDetailItemCnt @RoomID,@CompanyID", params1).FirstOrDefault();
                return cnt;
            }

        }
        public bool IsAppliedInventoryDetailById(long ID, bool IsApplied)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", ID)
                ,  new SqlParameter("@IsApplied", IsApplied)};
                string strQuery = @"EXEC IsAppliedInventoryDetailById @ID,@IsApplied";
                context.Database.ExecuteSqlCommand(strQuery, params1);
                return true;
            }
        }
        public bool IsInventoryLineItemGuidExists(Guid LineItemGuid)
        {
            Int32 LineItemCount = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                      new SqlParameter("@LineItemGuid", LineItemGuid)
                };
                LineItemCount = context.Database.SqlQuery<InventoryCountDetailDTO>("exec IsInventoryLineItemGuidExists @LineItemGuid", params1).ToList().Count();
            }
            if (LineItemCount > 0)
            {
                return true;
            }
            return false;
        }
        public bool IsInventoryLineItemDetailsGuidExists(Guid LineItemDetailGuid)
        {
            Int32 LineItemDetailCount = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                      new SqlParameter("@LineItemDetailGuid", LineItemDetailGuid)
                };
                LineItemDetailCount = context.Database.SqlQuery<InventoryCountDetailDTO>("exec IsInventoryLineItemDetailsGuidExists @LineItemDetailGuid", params1).ToList().Count();
            }
            if (LineItemDetailCount > 0)
            {
                return true;
            }
            return false;
        }

        public int GetUnAppliedInventoryDetailCountByICGUIDPlain(Guid InventoryCountGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@InventoryCountGuid", InventoryCountGuid) };
                return context.Database.SqlQuery<int>("exec [GetUnAppliedInventoryDetailCountByICGUIDPlain] @InventoryCountGuid", params1).FirstOrDefault();
            }
        }

        #endregion

    }
}


