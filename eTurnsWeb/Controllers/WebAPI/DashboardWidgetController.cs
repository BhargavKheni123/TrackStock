using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using eTurns.DTO;
using eTurns.DAL;


namespace eTurnsWeb.Controllers.WebAPI
{
    public class DashboardWidgetController : ApiController
    {
        public DashboardWidgetDAL _repository = new DashboardWidgetDAL();

        public DashboardWidgeDTO GetUserWidget(Int64 RoomId, int userid)
        {
            return _repository.GetUserWidget(RoomId, userid);
        }
        public void SaveUserWidget(Int64 RoomId, int userid, string widgetorder)
        {
            _repository.SaveUserWidget(RoomId, userid, widgetorder);
        }
        public List<ItemMasterDTO> GetCategoryWiseCost()
        {
            return _repository.GetCategoryWiseCost();
        }
    }
}
