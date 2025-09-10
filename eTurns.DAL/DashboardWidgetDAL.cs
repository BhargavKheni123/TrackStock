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

namespace eTurns.DAL
{
    public class DashboardWidgetDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        public DashboardWidgetDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        public DashboardWidgetDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        {
            DataBaseName = DbName;
            DBServerName = DBServerNm;
            DBUserName = DBUserNm;
            DBPassword = DBPswd;
        }

        #endregion
        
        //public IEnumerable<DashboardWidgeDTO> GetCachedData()
        //{
        //    //Get Cached-Media
        //    IEnumerable<DashboardWidgeDTO> ObjCache = CacheHelper<IEnumerable<DashboardWidgeDTO>>.GetCacheItem("Cached_DashboardWidgetOrders");
        //    if (ObjCache == null)
        //    {
        //        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //        {
        //            IEnumerable<DashboardWidgeDTO> obj = (from w in context.DashboardWidgetOrders
        //                                                  select new DashboardWidgeDTO
        //                                                  {
        //                                                      WidgetID = w.pk_WidgetOrder,
        //                                                      UserId = w.UserId,
        //                                                      RoomId = w.RoomId,
        //                                                      WidgetOrder = w.WidgetOrder
        //                                                  }).AsParallel().ToList();
        //            ObjCache = CacheHelper<IEnumerable<DashboardWidgeDTO>>.AddCacheItem("Cached_DashboardWidgetOrders", obj);
        //        }
        //    }

        //    return ObjCache;
        //}

        
        //public DashboardWidgeDTO GetUserWidget(Int64 RoomId, int userid)
        //{
        //    DashboardWidgeDTO objwidget = new DashboardWidgeDTO();
        //    objwidget = GetCachedData().Where(w => w.UserId == userid && w.RoomId == RoomId).FirstOrDefault();
        //    return objwidget;
        //}
        
        //public void SaveUserWidget(Int64 RoomId, int userid, string widgetorder)
        //{
        //    eTurnsEntities objContext = new eTurnsEntities(base.DataBaseEntityConnectionString);
        //    DashboardWidgetOrder objuserwidget;
        //    objuserwidget = (from w in objContext.DashboardWidgetOrders
        //                     where w.UserId == userid && w.RoomId == RoomId
        //                     select w).FirstOrDefault();
        //    if (objuserwidget != null)
        //    {
        //        objuserwidget.WidgetOrder = widgetorder;
        //        objContext.SaveChanges();

        //        DashboardWidgeDTO obj = new DashboardWidgeDTO();
        //        obj.WidgetID = objuserwidget.pk_WidgetOrder;
        //        obj.UserId = objuserwidget.UserId;
        //        obj.RoomId = objuserwidget.RoomId;
        //        obj.WidgetOrder = objuserwidget.WidgetOrder;

        //        IEnumerable<DashboardWidgeDTO> ObjCache = CacheHelper<IEnumerable<DashboardWidgeDTO>>.GetCacheItem("Cached_DashboardWidgetOrders");
        //        if (ObjCache != null)
        //        {
        //            List<DashboardWidgeDTO> objTemp = ObjCache.ToList();
        //            objTemp.RemoveAll(i => i.WidgetID == obj.WidgetID);
        //            ObjCache = objTemp.AsEnumerable();


        //            List<DashboardWidgeDTO> tempC = new List<DashboardWidgeDTO>();
        //            tempC.Add(obj);
        //            IEnumerable<DashboardWidgeDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
        //            CacheHelper<IEnumerable<DashboardWidgeDTO>>.AppendToCacheItem("Cached_DashboardWidgetOrders", NewCache);
        //        }
        //    }
        //    else
        //    {
        //        objuserwidget = new DashboardWidgetOrder();
        //        objuserwidget.UserId = userid;
        //        objuserwidget.RoomId = RoomId;
        //        objuserwidget.WidgetOrder = widgetorder;
        //        objContext.DashboardWidgetOrders.AddObject(objuserwidget);
        //        objContext.SaveChanges();


        //        DashboardWidgeDTO obj = new DashboardWidgeDTO();
        //        obj.WidgetID = objuserwidget.pk_WidgetOrder;
        //        obj.UserId = objuserwidget.UserId;
        //        obj.RoomId = objuserwidget.RoomId;
        //        obj.WidgetOrder = objuserwidget.WidgetOrder;

        //        if (obj.WidgetID > 0)
        //        {
        //            //Get Cached-Media
        //            IEnumerable<DashboardWidgeDTO> ObjCache = CacheHelper<IEnumerable<DashboardWidgeDTO>>.GetCacheItem("Cached_DashboardWidgetOrders");
        //            if (ObjCache != null)
        //            {
        //                List<DashboardWidgeDTO> tempC = new List<DashboardWidgeDTO>();
        //                tempC.Add(obj);

        //                IEnumerable<DashboardWidgeDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
        //                CacheHelper<IEnumerable<DashboardWidgeDTO>>.AppendToCacheItem("Cached_DashboardWidgetOrders", NewCache);
        //            }
        //        }
        //    }
        //}

        //public void SetWidgetOrder(List<Int64> lstModuleId, Int64 RoomId, Int64 UserId)
        //{
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        List<ModuleMasterDTO> lst = (from m in context.ModuleMasters
        //                                     join a in context.ParentModuleMasters on m.ParentID equals a.ID
        //                                     where lstModuleId.Contains(m.ID) && m.ParentID != null && (m.IsDeleted ?? false) == false
        //                                     select new ModuleMasterDTO
        //                                       {
        //                                           ModuleName = a.ParentModuleName
        //                                       }).Distinct().ToList();

        //        //List<ModuleMasterDTO> lst = (from m in context.ParentModuleMasters
        //        //                             where lstparentId.Contains(m.ID)
        //        //                             select new ModuleMasterDTO
        //        //                             {
        //        //                                 ModuleName = m.ParentModuleName
        //        //                             }).Distinct().ToList();

        //        DashboardWidgeDTO objorder = (from m in context.DashboardWidgetOrders
        //                                      where m.RoomId == RoomId && m.UserId == UserId
        //                                      select new DashboardWidgeDTO
        //                                      {
        //                                          WidgetOrder = m.WidgetOrder,
        //                                          RoomId = m.RoomId,
        //                                          UserId = m.UserId,
        //                                          WidgetID = m.pk_WidgetOrder
        //                                      }).SingleOrDefault();
        //        string ParentOrder = string.Empty;
        //        if (objorder != null)
        //        {
        //            foreach (ModuleMasterDTO item in lst)
        //            {
        //                if (!objorder.WidgetOrder.Contains(item.ModuleName))
        //                {
        //                    ParentOrder = (ParentOrder == "" ? item.ModuleName : (ParentOrder + "," + item.ModuleName));
        //                }
        //            }

        //            if (ParentOrder != string.Empty)
        //            {
        //                string[] strorder = objorder.WidgetOrder.Split(':');
        //                string strQuery = "";
        //                if (strorder.Length == 2)
        //                {
        //                    strQuery = "UPDATE DashboardWidgetOrder SET WidgetOrder = '" + (strorder[0].ToString() + ":" + ParentOrder + "," + strorder[1].ToString()) + "' WHERE UserID=" + objorder.UserId + " AND RoomID=" + objorder.RoomId;
        //                    objorder.WidgetOrder = strorder[0].ToString() + ":" + ParentOrder + "," + strorder[1].ToString();
        //                }
        //                else
        //                {
        //                    strQuery = "UPDATE DashboardWidgetOrder SET WidgetOrder = '" + (strorder[0].ToString() + "," + ParentOrder) + "' WHERE UserID=" + objorder.UserId + " AND RoomID=" + objorder.RoomId;
        //                    objorder.WidgetOrder = strorder[0].ToString() + "," + ParentOrder;
        //                }

        //                context.ExecuteStoreCommand(strQuery);


        //                IEnumerable<DashboardWidgeDTO> ObjCache = CacheHelper<IEnumerable<DashboardWidgeDTO>>.GetCacheItem("Cached_DashboardWidgetOrders");
        //                if (ObjCache != null)
        //                {
        //                    List<DashboardWidgeDTO> objTemp = ObjCache.ToList();
        //                    objTemp.RemoveAll(i => i.WidgetID == objorder.WidgetID);
        //                    ObjCache = objTemp.AsEnumerable();


        //                    List<DashboardWidgeDTO> tempC = new List<DashboardWidgeDTO>();
        //                    tempC.Add(objorder);
        //                    IEnumerable<DashboardWidgeDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
        //                    CacheHelper<IEnumerable<DashboardWidgeDTO>>.AppendToCacheItem("Cached_DashboardWidgetOrders", NewCache);
        //                }


        //            }


        //        }
        //    }
        //}

        


    }
}
