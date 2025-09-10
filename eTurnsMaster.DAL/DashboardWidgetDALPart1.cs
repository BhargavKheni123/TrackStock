using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using System.Data.SqlClient;

namespace eTurnsMaster.DAL
{
    public partial class DashboardWidgetDAL : eTurnsMasterBaseDAL
    {

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DashboardWidgeDTO> GetCachedData()
        {
            //Get Cached-Media
            IEnumerable<DashboardWidgeDTO> ObjCache = CacheHelper<IEnumerable<DashboardWidgeDTO>>.GetCacheItem("Cached_DashboardWidgetOrdersMasterMaster");
            if (ObjCache == null)
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<DashboardWidgeDTO> obj = (from w in context.DashboardWidgetOrders
                                                          select new DashboardWidgeDTO
                                                          {
                                                              WidgetID = w.pk_WidgetOrder,
                                                              UserId = w.UserId,
                                                              WidgetOrder = w.WidgetOrder,
                                                              CompanyId = w.CompanyId,
                                                              RoomId = w.RoomId,
                                                              EnterpriseId = w.EnterpriseId,
                                                              DashboardType = w.DashboardType
                                                          }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<DashboardWidgeDTO>>.AddCacheItem("Cached_DashboardWidgetOrdersMaster", obj);
                }
            }

            return ObjCache;
        }


        public DashboardWidgeDTO GetUserWidget(long userid, long RoomId, long CompanyId, long EnterpriseId, byte Dtype)
        {
            DashboardWidgeDTO objwidget = new DashboardWidgeDTO();
            objwidget = GetCachedData().Where(w => w.UserId == userid && w.RoomId == RoomId && w.CompanyId == CompanyId && w.EnterpriseId == EnterpriseId && w.DashboardType == Dtype).FirstOrDefault();
            return objwidget;
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

                DashboardWidgeDTO obj = new DashboardWidgeDTO();
                obj.WidgetID = objuserwidget.pk_WidgetOrder;
                obj.UserId = objuserwidget.UserId;
                objuserwidget.RoomId = RoomId;
                objuserwidget.CompanyId = CompanyId;
                objuserwidget.EnterpriseId = EnterpriseId;
                objuserwidget.DashboardType = Dtype;
                obj.WidgetOrder = objuserwidget.WidgetOrder;

                IEnumerable<DashboardWidgeDTO> ObjCache = CacheHelper<IEnumerable<DashboardWidgeDTO>>.GetCacheItem("Cached_DashboardWidgetOrdersMaster");
                if (ObjCache != null)
                {
                    List<DashboardWidgeDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.WidgetID == obj.WidgetID);
                    ObjCache = objTemp.AsEnumerable();


                    List<DashboardWidgeDTO> tempC = new List<DashboardWidgeDTO>();
                    tempC.Add(obj);
                    IEnumerable<DashboardWidgeDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    CacheHelper<IEnumerable<DashboardWidgeDTO>>.AppendToCacheItem("Cached_DashboardWidgetOrdersMaster", NewCache);
                }
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
                objContext.DashboardWidgetOrders.AddObject(objuserwidget);
                objContext.SaveChanges();


                DashboardWidgeDTO obj = new DashboardWidgeDTO();
                obj.WidgetID = objuserwidget.pk_WidgetOrder;
                obj.UserId = objuserwidget.UserId;
                objuserwidget.RoomId = RoomId;
                objuserwidget.CompanyId = CompanyId;
                objuserwidget.EnterpriseId = EnterpriseId;
                objuserwidget.DashboardType = Dtype;
                obj.WidgetOrder = objuserwidget.WidgetOrder;

                if (obj.WidgetID > 0)
                {
                    //Get Cached-Media
                    IEnumerable<DashboardWidgeDTO> ObjCache = CacheHelper<IEnumerable<DashboardWidgeDTO>>.GetCacheItem("Cached_DashboardWidgetOrdersMaster");
                    if (ObjCache != null)
                    {
                        List<DashboardWidgeDTO> tempC = new List<DashboardWidgeDTO>();
                        tempC.Add(obj);

                        IEnumerable<DashboardWidgeDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                        CacheHelper<IEnumerable<DashboardWidgeDTO>>.AppendToCacheItem("Cached_DashboardWidgetOrdersMaster", NewCache);
                    }
                }
            }
        }

        public void SetWidgetOrder(List<Int64> lstModuleId, Int64 UserId)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                List<ModuleMasterDTO> lst = (from m in context.ModuleMasters
                                             join a in context.ParentModuleMasters on m.ParentID equals a.ID
                                             where lstModuleId.Contains(m.ID) && m.ParentID != null && (m.IsDeleted ?? false) == false
                                             select new ModuleMasterDTO
                                             {
                                                 ModuleName = a.ParentModuleName
                                             }).Distinct().ToList();

                //List<ModuleMasterDTO> lst = (from m in context.ParentModuleMasters
                //                             where lstparentId.Contains(m.ID)
                //                             select new ModuleMasterDTO
                //                             {
                //                                 ModuleName = m.ParentModuleName
                //                             }).Distinct().ToList();

                DashboardWidgeDTO objorder = (from m in context.DashboardWidgetOrders
                                              where m.UserId == UserId
                                              select new DashboardWidgeDTO
                                              {
                                                  WidgetOrder = m.WidgetOrder,
                                                  UserId = m.UserId,
                                                  WidgetID = m.pk_WidgetOrder
                                              }).SingleOrDefault();
                string ParentOrder = string.Empty;
                if (objorder != null)
                {
                    foreach (ModuleMasterDTO item in lst)
                    {
                        if (!objorder.WidgetOrder.Contains(item.ModuleName))
                        {
                            ParentOrder = (ParentOrder == "" ? item.ModuleName : (ParentOrder + "," + item.ModuleName));
                        }
                    }

                    if (ParentOrder != string.Empty)
                    {
                        string[] strorder = objorder.WidgetOrder.Split(':');
                        string strQuery = "";
                        if (strorder.Length == 2)
                        {
                            strQuery = "UPDATE DashboardWidgetOrder SET WidgetOrder = '" + (strorder[0].ToString() + ":" + ParentOrder + "," + strorder[1].ToString()) + "' WHERE UserID=" + objorder.UserId;
                            objorder.WidgetOrder = strorder[0].ToString() + ":" + ParentOrder + "," + strorder[1].ToString();
                        }
                        else
                        {
                            strQuery = "UPDATE DashboardWidgetOrder SET WidgetOrder = '" + (strorder[0].ToString() + "," + ParentOrder) + "' WHERE UserID=" + objorder.UserId;
                            objorder.WidgetOrder = strorder[0].ToString() + "," + ParentOrder;
                        }

                        context.ExecuteStoreCommand(strQuery);


                        IEnumerable<DashboardWidgeDTO> ObjCache = CacheHelper<IEnumerable<DashboardWidgeDTO>>.GetCacheItem("Cached_DashboardWidgetOrdersMaster");
                        if (ObjCache != null)
                        {
                            List<DashboardWidgeDTO> objTemp = ObjCache.ToList();
                            objTemp.RemoveAll(i => i.WidgetID == objorder.WidgetID);
                            ObjCache = objTemp.AsEnumerable();


                            List<DashboardWidgeDTO> tempC = new List<DashboardWidgeDTO>();
                            tempC.Add(objorder);
                            IEnumerable<DashboardWidgeDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                            CacheHelper<IEnumerable<DashboardWidgeDTO>>.AppendToCacheItem("Cached_DashboardWidgetOrdersMaster", NewCache);
                        }


                    }


                }
            }
        }


    }
}
