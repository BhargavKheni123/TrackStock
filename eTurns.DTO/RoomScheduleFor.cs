using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.DTO
{
    public enum RoomScheduleFor
    {
        SupplierOrder = 1,//eTurnsServices.ScheduledOrdersTransfers        
        RoomTransfer = 2,//eTurnsServices.ScheduledOrdersTransfers
        NOtificationReport = 5, //eTurns.SchedulerService
        NotificationAlert = 6, //eTurns.SchedulerService
        SupplierPullBilling = 7,//eTurnsServices.ScheduledOrdersTransfers
        DailyMidNightCalculations = 8,//eTurnsServices.DailyAnalytics
        UserSchedule = 9//eTurnsServices.DailyAnalytics
    }
}
