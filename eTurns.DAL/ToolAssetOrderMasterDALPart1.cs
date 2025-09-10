using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eTurns.DTO;
using System.Data;
using Dynamite;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Collections;
using System.Data.SqlClient;
using eTurns.DTO.Resources;
using System.Web;
using System.Globalization;

namespace eTurns.DAL
{
    public partial class ToolAssetOrderMasterDALPart1 : eTurnsBaseDAL
    {
        /// <summary>
        /// UPdateCacheObjectForName
        /// </summary>
        /// <param name="objOrder"></param>
        /// <returns></returns>
        private ToolAssetOrderMasterDTO UPdateCacheObjectForName(ToolAssetOrderMasterDTO objOrder)
        {
            List<Int64> OnlyReceiveOrderIds = GetOnlyReceivedOrderIDs();
            objOrder.OrderIsInReceive = OnlyReceiveOrderIds.Contains(objOrder.ID);

            if (objOrder.CreatedBy.GetValueOrDefault(0) > 0)
            {
                UserMasterDTO lstUserC = new UserMasterDAL(base.DataBaseName).GetRecord(objOrder.CreatedBy.GetValueOrDefault(0));
                if (lstUserC != null)
                    objOrder.CreatedByName = lstUserC.UserName;
            }
            if (objOrder.LastUpdatedBy.GetValueOrDefault(0) > 0)
            {
                UserMasterDTO lstUserU = new UserMasterDAL(base.DataBaseName).GetRecord(objOrder.LastUpdatedBy.GetValueOrDefault(0));

                if (lstUserU != null)
                    objOrder.UpdatedByName = lstUserU.UserName;
            }

            return objOrder;

        }

        /// <summary>
        /// GetOnlyReceivedOrderIDs
        /// </summary>
        /// <returns></returns>
        public List<Int64> GetOnlyReceivedOrderIDs()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //List<Int64> lstReceivedIDs = (from u in context.ExecuteStoreQuery<Int64>(@"SELECT DISTINCT OrderID 
                //                                                                            FROM ToolAssetOrderDetails 
                //                                                                            WHERE ID IN (SELECT DISTINCT OrderDetailID 
                //                                                                                         FROM ReceiveOrderDetails 
                //                                                                                         WHERE ISNULL(IsDeleted,0) = 0 and ISNULL(IsArchived,0) = 0 
                //                                                                                         Union 
                //                                                                                         SELECT DISTINCT OrderDetailID 
                //                                                                                         FROM ItemLocationDetails 
                //                                                                                         WHERE OrderDetailID IS NOT NULL 
                //                                                                                                AND ISNULL(IsDeleted,0) = 0 
                //                                                                                                AND ISNULL(IsArchived,0) = 0)
                //                                                                            AND ISNULL(IsDeleted,0) = 0 
                //                                                                            AND ISNULL(IsArchived,0) = 0 ")
                //                              select u).ToList();

                //return lstReceivedIDs;
                return null;
            }
        }

        public void TempFillOrderNumberSorting()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.CommandTimeout = 120;
                List<ToolAssetOrderMasterDTO> objItem = (from u in context.ExecuteStoreQuery<ToolAssetOrderMasterDTO>("SELECT ID,CompanyID,RoomID,ToolAssetOrderNumber FROM ToolAssetOrderMaster")
                                                         select new ToolAssetOrderMasterDTO
                                                         {
                                                             ID = u.ID,
                                                             ToolAssetOrderNumber = u.ToolAssetOrderNumber,
                                                             CompanyID = u.CompanyID,
                                                             RoomID = u.RoomID,
                                                         }).ToList();

                foreach (var item in objItem)
                {
                    ToolAssetOrderMaster obj = context.ToolAssetOrderMasters.FirstOrDefault(x => x.ID == item.ID && x.RoomID == item.RoomID && x.CompanyID == item.CompanyID);
                    obj.OrderNumber_ForSorting = CommonDAL.GetSortingString(obj.ToolAssetOrderNumber);
                    context.ObjectStateManager.ChangeObjectState(obj, EntityState.Unchanged);
                    context.ToolAssetOrderMasters.Attach(obj);
                    context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                    context.SaveChanges();
                }
            }
        }

        public int GetTotalOrderCountbyOrderName(string OrderNUmber, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ExecuteStoreQuery<int>("Select cast(isnull(Count(ID),0) as int) from ToolAssetOrderMaster where isnull(isdeleted,0)=0 and isnull(isarchived,0)=0 AND RoomID=" + RoomID + " AND CompanyID=" + CompanyID + " AND ToolAssetOrderNumber='" + (OrderNUmber ?? string.Empty).Trim() + "'").First();
            }
        }
    }
}
