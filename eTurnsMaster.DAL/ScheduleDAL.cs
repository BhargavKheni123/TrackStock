using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace eTurnsMaster.DAL
{
    public class ScheduleDAL : eTurnsMasterBaseDAL
    {

        public List<NotificationMasterDTO> GetAllSchedulesByTimePeriod(DateTime FromDate, DateTime ToDate)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@FromDate", FromDate), new SqlParameter("@ToDate", ToDate) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NotificationMasterDTO>("EXEC GetReportScheduletoRunNow @FromDate,@ToDate", params1).ToList();
            }
        }
        public List<CycleCountSettingMasterDTO> GetAllCountSchedulesByTimePeriod(DateTime FromDate, DateTime ToDate)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@FromDate", FromDate), new SqlParameter("@ToDate", ToDate) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CycleCountSettingMasterDTO>("EXEC GetCountScheduletoRunNow @FromDate,@ToDate", params1).ToList();
            }
        }
        public List<RoomScheduleDetailMasterDTO> GetAllOrderScheduletoRunNow(DateTime FromDate, DateTime ToDate)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@FromDate", FromDate), new SqlParameter("@ToDate", ToDate) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RoomScheduleDetailMasterDTO>("EXEC GetOrderScheduletoRunNow @FromDate,@ToDate", params1).ToList();
            }
        }

        public List<eVMISetupDTO> GetAlleVMISchedulesByTimePeriod(DateTime FromDate, DateTime ToDate)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@FromDate", FromDate), new SqlParameter("@ToDate", ToDate) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<eVMISetupDTO>("EXEC GetAlleVMISchedulesByTimePeriod @FromDate,@ToDate", params1).ToList();
            }
        }

        public List<RoomScheduleDetailMasterDTO> GetAllPastRoomSchedules()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RoomScheduleDetailMasterDTO>("EXEC [GetDeadRoomSchedules]").ToList();
            }
        }
        public List<NotificationMasterDTO> GetAllPastNotification()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NotificationMasterDTO>("EXEC [GetDeadNotifications]").ToList();
            }
        }

        public List<RoomScheduleDetailMasterDTO> GetAllMidNightSchedules(DateTime FromDate, DateTime ToDate)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@FromDate", FromDate), new SqlParameter("@ToDate", ToDate) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RoomScheduleDetailMasterDTO>("EXEC GetAllMidNightSchedules @FromDate,@ToDate", params1).ToList();
            }
        }
    }
}
