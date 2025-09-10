using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace eTurns.DAL
{
    public class WorkOrderLineItemsDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public WorkOrderLineItemsDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        #endregion

        public void UpdateWOItemAndTotalCost(string WorkOrderGUID, Int64 Room, Int64 CompanyID)
        {
            WorkOrderDAL objDAL = new WorkOrderDAL(base.DataBaseName);
            Guid WoGUID = Guid.Empty;
            Guid.TryParse(WorkOrderGUID, out WoGUID);
            WorkOrderDTO objDTO = objDAL.GetWorkOrderByGUIDPlain(WoGUID);

            if (objDTO != null)
            {
                IEnumerable<PullMasterViewDTO> lstPullMaster = new PullMasterDAL(base.DataBaseName).GetPullByWorkOrderDetailGUIDPlain(Room, CompanyID, WoGUID);
                if (lstPullMaster != null && lstPullMaster.Count() > 0)
                {
                    objDTO.UsedItems = lstPullMaster.Count();
                    objDTO.UsedItemsCost = (double)(lstPullMaster.Where(x => (x.ActionType ?? "Pull").ToLower().Contains("pull")).Sum(x => x.PullCost.GetValueOrDefault(0)) - lstPullMaster.Where(x => (x.ActionType ?? "Pull").ToLower().Contains("credit") || (x.ActionType ?? "Pull").ToLower().Contains("ms credit")).Sum(x => (x.PullCost.GetValueOrDefault(0))));
                    objDTO.UsedItemsSellPrice = (double)(lstPullMaster.Where(x => (x.ActionType ?? "Pull").ToLower().Contains("pull")).Sum(x => x.PullPrice.GetValueOrDefault(0)) - lstPullMaster.Where(x => (x.ActionType ?? "Pull").ToLower().Contains("credit") || (x.ActionType ?? "Pull").ToLower().Contains("ms credit")).Sum(x => (x.PullPrice.GetValueOrDefault(0))));
                }
                else
                {
                    objDTO.UsedItems = 0;
                    objDTO.UsedItemsCost = 0;
                    objDTO.UsedItemsSellPrice = 0;
                }

                ToolCheckInOutHistoryDAL objTCDAL = new ToolCheckInOutHistoryDAL(base.DataBaseName);
                //var objModel = objTCDAL.GetCachedData(Room, CompanyID).Where(x => x.WorkOrderGuid == objDTO.GUID);
                int checkoutcount = objTCDAL.GetCheckoutCount(null, objDTO.GUID, null, Room, CompanyID);
                objDTO.UsedItems = objDTO.UsedItems.GetValueOrDefault(0) + checkoutcount;

                objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objDTO.EditedFrom = "Web";
                objDAL.Edit(objDTO);
            }
        }

    }
}


