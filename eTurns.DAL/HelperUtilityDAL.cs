using System;
using System.Collections.Generic;
using System.Linq;

namespace eTurns.DAL
{
    public class HelperUtilityDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        public HelperUtilityDAL()
        {

        }
        public HelperUtilityDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }
        public HelperUtilityDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        {
            DataBaseName = DbName;
            DBServerName = DBServerNm;
            DBUserName = DBUserNm;
            DBPassword = DBPswd;
        }
        #endregion
        public List<OrderIDRoomID> GetClosedOrderGuids()
        {
            List<OrderIDRoomID> lstguids = new List<OrderIDRoomID>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstguids = context.Database.SqlQuery<OrderIDRoomID>("Select OM.[ID],OM.[GUID],OM.[CompanyID],OM.[Room] from OrderMaster as OM inner join ROom as RM on OM.Room = Rm.ID inner join CompanyMaster as CM on OM.CompanyID = CM.ID where OM.Created<'2018-01-01 00:00:00.000' and OM.OrderType=1 and OM.OrderStatus < 8 and ISNULL(OM.IsDeleted,0)=0 and ISNULL(CM.IsDeleted,0)=0 and ISNULL(RM.IsDeleted,0)=0 Order by OM.ID ASC").ToList();
            }
            return lstguids;
        }

    }

    public class OrderIDRoomID
    {
        public long ID { get; set; }
        public Guid GUID { get; set; }
        public long? CompanyID { get; set; }
        public long? Room { get; set; }
    }
}
