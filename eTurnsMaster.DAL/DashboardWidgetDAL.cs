using System.Data;
using System.Linq;
using eTurns.DTO;
using System.Data;
using System.Data.SqlClient;
namespace eTurnsMaster.DAL
{
    public partial class DashboardWidgetDAL : eTurnsMasterBaseDAL
    {

        public DashboardWidgeDTO GetUserWidget(long UserId, long RoomId, long CompanyId, long EnterpriseId, byte Dtype)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@EnterpriseId", EnterpriseId),
                                                    new SqlParameter("@CompanyID", CompanyId),
                                                    new SqlParameter("@RoomID", RoomId),
                                                    new SqlParameter("@UserId", UserId),
                                                    new SqlParameter("@DashboardType", Dtype)
                                                };
                return context.Database.SqlQuery<DashboardWidgeDTO>("exec [GetDashboardWidgetOrderByUser] @EnterpriseId,@CompanyID,@RoomID,@UserId,@DashboardType", params1).FirstOrDefault();
            }
        }

        /// <summary>
        /// Save User Widget
        /// </summary>
        /// <param name="userid">UserId</param>
        /// <param name="widgetorder">WidgetOrder</param>
        public void SaveUserWidget(long userid, string widgetorder, long RoomId, long CompanyId, long EnterpriseId, byte Dtype)
        {
            eTurns_MasterEntities objContext = new eTurns_MasterEntities(base.DataBaseEntityConnectionString);
            DashboardWidgetOrder objuserwidget;
            objuserwidget = (from w in objContext.DashboardWidgetOrders
                             where w.UserId == userid && w.RoomId == RoomId && w.CompanyId == CompanyId && w.EnterpriseId == EnterpriseId && w.DashboardType == Dtype
                             select w).FirstOrDefault();
            if (objuserwidget != null)
            {
                objuserwidget.WidgetOrder = widgetorder;
                objContext.SaveChanges();
            }
            else
            {
                objuserwidget = new DashboardWidgetOrder();
                objuserwidget.UserId = userid;
                objuserwidget.WidgetOrder = widgetorder;
                objuserwidget.RoomId = RoomId;
                objuserwidget.CompanyId = CompanyId;
                objuserwidget.EnterpriseId = EnterpriseId;
                objuserwidget.DashboardType = Dtype;
                objContext.DashboardWidgetOrders.Add(objuserwidget);
                objContext.SaveChanges();               
            }
        }
    }
}
