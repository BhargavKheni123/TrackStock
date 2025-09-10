using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using System.Data.SqlClient;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Data.Objects;
using System.Collections;
using System.IO;
using System.Net.Mail;
using System.Configuration;
using System.Text;
using eTurns.DTO.Resources;
using System.Collections.Concurrent;

namespace eTurns.DAL
{
    public partial class CartItemDAL : eTurnsBaseDAL
    {
        public IEnumerable<CartItemDTO> GetCartDataUsingRoomId(Int64 RoomID, Int64 CompanyID, bool IsDeleted, bool IsArchived)
        {
            //Get Cached-Media


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //IEnumerable<CartItemDTO> obj = (from u in context.Database.SqlQuery<CartItemDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS 'UpdatedByName', D.RoomName,E.ItemNumber FROM CartItem A inner join ItemMaster as E on  A.ItemGUID = E.GUID LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString())
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };
                IEnumerable<CartItemDTO> obj = (from u in context.Database.SqlQuery<CartItemDTO>("exec [GetCartDataUsingRoomId] @RoomID,@CompanyID,@IsDeleted,@IsArchived", params1) select u).ToList(); //A.IsDeleted!=1 AND A.IsArchived != 1 AND 
                return obj;
            }


        }
        public IEnumerable<CartItemDTO> GetCartData(Int64 RoomID, Int64 CompanyID, bool IsDeleted, bool IsArchived, Guid? ItemGuid, long? CartId, string OrderColumn)
        {
            //Get Cached-Media


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //IEnumerable<CartItemDTO> obj = (from u in context.Database.SqlQuery<CartItemDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS 'UpdatedByName', D.RoomName,E.ItemNumber FROM CartItem A inner join ItemMaster as E on  A.ItemGUID = E.GUID LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString())
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@ItemGuid", ItemGuid), new SqlParameter("@CartId", CartId) };
                IEnumerable<CartItemDTO> obj = (from u in context.Database.SqlQuery<CartItemDTO>("exec [GetCartData] @RoomID,@CompanyID,@IsDeleted,@IsArchived,@ItemGuid,@CartId", params1) select u).ToList(); //A.IsDeleted!=1 AND A.IsArchived != 1 AND 
                if (!string.IsNullOrWhiteSpace(OrderColumn))
                {
                    obj = obj.ToList().OrderBy(OrderColumn);
                }
                return obj;
            }


        }
        public IEnumerable<CartItemDTO> GetDataUsingGuid(Int64 RoomID, Int64 CompanyID, bool IsDeleted, bool IsArchived, Guid? CartGuid)
        {
            //Get Cached-Media


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //IEnumerable<CartItemDTO> obj = (from u in context.Database.SqlQuery<CartItemDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS 'UpdatedByName', D.RoomName,E.ItemNumber FROM CartItem A inner join ItemMaster as E on  A.ItemGUID = E.GUID LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString())
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@CartGuid", CartGuid) };
                IEnumerable<CartItemDTO> obj = (from u in context.Database.SqlQuery<CartItemDTO>("exec [GetCachedDataUsingGuid] @RoomID,@CompanyID,@IsDeleted,@IsArchived,@CartGuid ", params1) select u).ToList(); //A.IsDeleted!=1 AND A.IsArchived != 1 AND 
                return obj;
            }


        }
        public IEnumerable<CartItemDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            //Get Cached-Media
            IEnumerable<CartItemDTO> ObjCache = GetCachedData(RoomID, CompanyID, false, false);
            IEnumerable<CartItemDTO> ObjGlobalCache = ObjCache;
            ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false));

            if (IsArchived && IsDeleted)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived || t.IsDeleted == IsDeleted));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }
            else if (IsArchived)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }
            else if (IsDeleted)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsDeleted == IsDeleted));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }

            if (String.IsNullOrEmpty(SearchTerm))
            {

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<CartItemDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedBy.ToString())))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.LastUpdatedBy.ToString())))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
                    && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
                    && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
                    && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
                    && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
                    && ((Fields[1].Split('@')[24] == "") || (Fields[1].Split('@')[24].Split(',').ToList().Contains(t.ReplenishType)))
                    && ((Fields[1].Split('@')[23] == "") || (Fields[1].Split('@')[23].Split(',').ToList().Contains(t.SupplierId.ToString())))
                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<CartItemDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }
        public IEnumerable<CartItemDTO> GetCartItemsByItemId(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, Guid ItemGUID, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<CartItemDTO> ObjCache = GetCachedData(RoomID, CompanyID, false, false);

            IEnumerable<CartItemDTO> ObjGlobalCache = ObjCache;
            ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false));


            if (IsArchived && IsDeleted)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived || t.IsDeleted == IsDeleted));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }
            else if (IsArchived)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }
            else if (IsDeleted)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsDeleted == IsDeleted));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }

            ObjCache = ObjCache.Where(t => t.ItemGUID == ItemGUID);

            if (String.IsNullOrEmpty(SearchTerm))
            {

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<CartItemDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
                    && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
                    && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
                    && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
                    && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))

                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<CartItemDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }

        }
        public IEnumerable<CartItemDTO> GetUniqueCartItemsPaged(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<CartItemDTO> lstCartitems = GetPagedUniqueCartItems(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, RoomID, CompanyID, IsArchived, IsDeleted);
            lstCartitems = (from ci in lstCartitems
                            group ci by new { ci.ItemGUID, ci.ItemNumber } into groupedItems
                            select new CartItemDTO
                            {
                                ItemGUID = groupedItems.Key.ItemGUID,
                                ItemNumber = groupedItems.Key.ItemNumber,
                                Quantity = groupedItems.Sum(t => t.Quantity)
                            });
            TotalCount = lstCartitems.Count();
            return lstCartitems.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        }
        public IEnumerable<CartItemDTO> GetUniqueCartItemsPagedByItemId(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid ItemGUID)
        {
            IEnumerable<CartItemDTO> lstCartitems = GetPagedUniqueCartItems(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.ItemGUID == ItemGUID);
            //lstCartitems = (from ci in lstCartitems
            //                group ci by new { ci.ItemID, ci.ItemGUID, ci.ItemNumber } into groupedItems
            //                select new CartItemDTO
            //                {
            //                    ItemGUID = groupedItems.Key.ItemGUID,
            //                    ItemID = groupedItems.Key.ItemID,
            //                    ItemNumber = groupedItems.Key.ItemNumber,
            //                    Quantity = groupedItems.Sum(t => t.Quantity)
            //                });
            TotalCount = lstCartitems.Count();
            return lstCartitems.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        }
        public CartItemDTO Get(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            return GetCachedData(RoomID, CompanyID, false, false).Single(t => t.ID == id);
        }
        public CartItemDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.Database.SqlQuery<CartItemDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM CartItem_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + id.ToString())
                        select new CartItemDTO
                        {
                            ID = u.ID,

                            ItemGUID = u.ItemGUID,
                            Quantity = u.Quantity,
                            Status = u.Status,
                            ReplenishType = u.ReplenishType,
                            IsKitComponent = u.IsKitComponent,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            GUID = u.GUID,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            Room = u.Room,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb
                        }).SingleOrDefault();
            }
        }
        public Int64 Insert(CartItemDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CartItem obj = new CartItem();
                obj.ID = 0;

                obj.ItemGUID = objDTO.ItemGUID;
                obj.Quantity = objDTO.Quantity;
                obj.Status = objDTO.Status;
                obj.ReplenishType = objDTO.ReplenishType;
                obj.IsKitComponent = objDTO.IsKitComponent;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.GUID = Guid.NewGuid();
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                if (!string.IsNullOrEmpty(objDTO.AddedFrom))
                    obj.AddedFrom = objDTO.AddedFrom;
                else
                    obj.AddedFrom = "Web";

                if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                    obj.EditedFrom = objDTO.EditedFrom;
                else
                    obj.EditedFrom = "Web";


                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                obj.ReceivedOn = objDTO.ReceivedOn;

                context.CartItems.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;


                if (objDTO.ID > 0)
                {
                    //Get Cached-Media
                    IEnumerable<CartItemDTO> ObjCache = CacheHelper<IEnumerable<CartItemDTO>>.GetCacheItem("Cached_CartItem_" + objDTO.CompanyID.ToString());
                    if (ObjCache != null)
                    {
                        List<CartItemDTO> tempC = new List<CartItemDTO>();
                        tempC.Add(objDTO);

                        IEnumerable<CartItemDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                        CacheHelper<IEnumerable<CartItemDTO>>.AppendToCacheItem("Cached_CartItem_" + objDTO.CompanyID.ToString(), NewCache);
                    }
                }

                return obj.ID;
            }

        }
        public bool Edit(CartItemDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CartItem obj = context.CartItems.Where(x => x.ID == objDTO.ID).FirstOrDefault();
                if (obj != null && obj.ID > 0)
                {
                    //obj.ID = objDTO.ID;

                    obj.ItemGUID = objDTO.ItemGUID;
                    obj.Quantity = objDTO.Quantity;
                    obj.Status = objDTO.Status;
                    obj.ReplenishType = objDTO.ReplenishType;
                    obj.IsKitComponent = objDTO.IsKitComponent;
                    obj.UDF1 = objDTO.UDF1;
                    obj.UDF2 = objDTO.UDF2;
                    obj.UDF3 = objDTO.UDF3;
                    obj.UDF4 = objDTO.UDF4;
                    obj.UDF5 = objDTO.UDF5;
                    obj.GUID = objDTO.GUID;
                    obj.Created = objDTO.Created;
                    obj.Updated = objDTO.Updated;
                    obj.CreatedBy = objDTO.CreatedBy;
                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    obj.IsDeleted = false;
                    obj.IsArchived = false;
                    obj.CompanyID = objDTO.CompanyID;
                    obj.Room = objDTO.Room;
                    if (objDTO.IsOnlyFromItemUI)
                    {
                        obj.EditedFrom = objDTO.EditedFrom;
                        obj.ReceivedOn = objDTO.ReceivedOn;
                    }
                    //context.CartItems.Attach(obj);
                    //context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }

                //Get Cached-Media
                IEnumerable<CartItemDTO> ObjCache = CacheHelper<IEnumerable<CartItemDTO>>.GetCacheItem("Cached_CartItem_" + objDTO.CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<CartItemDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == objDTO.ID);
                    ObjCache = objTemp.AsEnumerable();

                    List<CartItemDTO> tempC = new List<CartItemDTO>();
                    tempC.Add(objDTO);
                    IEnumerable<CartItemDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    CacheHelper<IEnumerable<CartItemDTO>>.AppendToCacheItem("Cached_CartItem_" + objDTO.CompanyID.ToString(), NewCache);
                }


                return true;
            }
        }
        public List<CartChartDTO> GetCartItemTransferList(long companyId, long roomid)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = @"Select ci.ItemGUID as ItemGuid,SUM(Quantity) as Quantity ,im.ItemNumber as ItemNumber from cartItem ci left outer join itemmaster im on im.GUID=ci.ItemGUID where ci.isDeleted = 0 and ci.isarchived=0 and ci.Room=" + roomid + "  and ci.CompanyID=" + companyId + " and ci.ReplenishType='Transfer' group by ItemGUID,im.ItemNumber ";
                return (from u in context.Database.SqlQuery<CartChartDTO>(strQuery)
                        select new CartChartDTO
                        {
                            Quantity = u.Quantity,
                            ItemNumber = u.ItemNumber,
                            ItemGuid = u.ItemGuid
                        }).ToList();
            }

        }
        public List<CartChartDTO> GetCartItemOrderList(long companyId, long roomid)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = @"Select ci.ItemGUID as ItemGuid,SUM(Quantity) as Quantity ,im.ItemNumber as ItemNumber from cartItem ci left outer join itemmaster im on im.GUID=ci.ItemGUID where ci.isDeleted = 0 and ci.isarchived=0 and ci.Room=" + roomid + "  and ci.CompanyID=" + companyId + " and ci.ReplenishType='Purchase' group by ItemGUID,im.ItemNumber ";
                return (from u in context.Database.SqlQuery<CartChartDTO>(strQuery)
                        select new CartChartDTO
                        {
                            Quantity = u.Quantity,
                            ItemNumber = u.ItemNumber,
                            ItemGuid = u.ItemGuid
                        }).ToList();
            }

        }        
        public IEnumerable<CartItemDTO> GetPagedRecordsCartList(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            //Get Cached-Media
            IEnumerable<CartItemDTO> ObjCache = GetCachedData(RoomID, CompanyID, false, false);
            IEnumerable<CartItemDTO> ObjGlobalCache = ObjCache;
            ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false));

            if (IsArchived && IsDeleted)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived || t.IsDeleted == IsDeleted));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }
            else if (IsArchived)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }
            else if (IsDeleted)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsDeleted == IsDeleted));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }

            if (String.IsNullOrEmpty(SearchTerm))
            {

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<CartItemDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedBy.ToString())))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.LastUpdatedBy.ToString())))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
                    && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
                    && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
                    && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
                    && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
                    && ((Fields[1].Split('@')[24] == "") || (Fields[1].Split('@')[24].Split(',').ToList().Contains(t.ReplenishType)))
                    && ((Fields[1].Split('@')[23] == "") || (Fields[1].Split('@')[23].Split(',').ToList().Contains(t.SupplierId.ToString())))
                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<CartItemDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.ItemNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Quantity ?? 0).ToString().IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ReplenishType ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.CreatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.ItemNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Quantity ?? 0).ToString().IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ReplenishType ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.CreatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }
        public bool CreateOrderTransferFromCart(int ActiononCartItems, string SelectedCartIds, long RoomId, long CompanyId, long CreatedBy)
        {
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            DataSet dsCart = new DataSet();
            if (Connectionstring == "")
            {
                return false;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "OrderCreationTransferFromCart", ActiononCartItems, SelectedCartIds, RoomId, CompanyId, CreatedBy);
            return true;
        }
        public void AutoCartUpdate(Guid ItemGUID, long UserId)
        {
            //string Connectionstring = ConfigurationManager.ConnectionStrings["eTurnsConnection"].ConnectionString;
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            if (Connectionstring != "")
            {
                SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
                SqlHelper.ExecuteNonQuery(EturnsConnection, "AutoCartUpdate", ItemGUID, UserId);
            }
        }
        public void AutoCartUpdateByCodeOLD(Guid ItemGUID, long UserId, long EnterpriseId, bool IsFromUI)
        {
            int[] OrdStatus = new int[] { (int)OrderStatus.Approved, (int)OrderStatus.Submitted, (int)OrderStatus.Transmitted, (int)OrderStatus.TransmittedIncomplete, (int)OrderStatus.TransmittedInCompletePastDue, (int)OrderStatus.TransmittedPastDue, (int)OrderStatus.UnSubmitted };
            string[] ReqStatus = new string[] { "Unsubmitted", "Submittted" };
            int[] transStatus = new int[] { (int)TransferStatus.Approved, (int)TransferStatus.FullFillQuantity, (int)TransferStatus.Submitted, (int)TransferStatus.Transmitted, (int)TransferStatus.TransmittedIncomplete, (int)TransferStatus.TransmittedInCompletePastDue, (int)TransferStatus.TransmittedPastDue, (int)TransferStatus.UnSubmitted };
            bool sendMail = true;
            EnterpriseDTO objEnterpriseDTO = new EnterpriseDTO();
            if (HttpContext.Current != null && HttpContext.Current.Session != null && !String.IsNullOrWhiteSpace(Convert.ToString(HttpContext.Current.Session["SendcartMail"])))
            {
                sendMail = Convert.ToBoolean(HttpContext.Current.Session["SendcartMail"]);
            }
            if (EnterpriseId < 1)
            {
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    long.TryParse(Convert.ToString(HttpContext.Current.Session["EnterPriceID"]), out EnterpriseId);
                    if (EnterpriseId < 1)
                    {
                        EnterpriseDAL objEnterpriseDAL = new EnterpriseDAL(base.DataBaseName);
                        objEnterpriseDTO = objEnterpriseDAL.GetEnterpriseByDbName(base.DataBaseName);
                        if (objEnterpriseDTO != null)
                        {
                            EnterpriseId = objEnterpriseDTO.ID;
                        }
                    }
                }
                else
                {
                    EnterpriseDAL objEnterpriseDAL = new EnterpriseDAL(base.DataBaseName);
                    objEnterpriseDTO = objEnterpriseDAL.GetEnterpriseByDbName(base.DataBaseName);
                    if (objEnterpriseDTO != null)
                    {
                        EnterpriseId = objEnterpriseDTO.ID;

                    }

                }
            }
            else
            {
                objEnterpriseDTO = new EnterpriseDAL(base.DataBaseName).GetEnterprise(EnterpriseId);
            }

            Guid EmptyGUID = Guid.Empty;
            SuggestedOrderInfo objSuggestedOrderInfo = new SuggestedOrderInfo();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                double SuggestedOrderqty = 0;
                //double OnOrderQuantity = 0;
                double onOrderQuantity = getOnOrderQty(ItemGUID);

                if (context.CartItems.Any(t => t.ItemGUID == ItemGUID && (t.IsDeleted ?? false) == false))
                {
                    SuggestedOrderqty = context.CartItems.Where(t => t.ItemGUID == ItemGUID && (t.IsDeleted ?? false) == false).Sum(t => (t.Quantity ?? 0));
                }



                objSuggestedOrderInfo = (from im in context.ItemMasters
                                         join rm in context.Rooms on im.Room equals rm.ID
                                         join cm in context.CompanyMasters on rm.CompanyID equals cm.ID
                                         join bm in context.BinMasters on im.DefaultLocation equals bm.ID into im_bm_join
                                         join sm in context.SupplierMasters on im.SupplierID equals sm.ID
                                         from im_bm in im_bm_join.DefaultIfEmpty()
                                         where im.GUID == ItemGUID
                                         select new SuggestedOrderInfo
                                         {
                                             AvailableQuantity = 0,
                                             CartQuantity = 0,
                                             CompanyId = im.CompanyID ?? 0,
                                             DefaultReorderQuantity = im.DefaultReorderQuantity,
                                             devideval = 0,
                                             IsEnforceDefaultReorderQuantity = im.IsEnforceDefaultReorderQuantity ?? false,
                                             IsItemLevelMinMaxQtyRequired = im.IsItemLevelMinMaxQtyRequired,
                                             ItemCriticalQuantity = im.CriticalQuantity,
                                             ItemIsPurchase = im.IsPurchase ?? false,
                                             ItemIsTranser = im.IsTransfer ?? false,
                                             ItemMaximumQuantity = im.MaximumQuantity,
                                             ItemMinimumQuantity = im.MinimumQuantity,
                                             ItemOnHandQuantity = im.OnHandQuantity ?? 0,
                                             ItemOnTransferInQuantity = im.OnTransferQuantity ?? 0,
                                             //ItemOnTransferOutQuantity = im.InTransitquantity ?? 0,
                                             ItemOnTransferOutQuantity = im.OutTransferQuantity,
                                             ItemOrderedQuantity = onOrderQuantity,
                                             ItemRequisitionQuantity = im.RequisitionedQuantity ?? 0,
                                             ItemSuggestedOrderQuantity = SuggestedOrderqty,
                                             moduloval = 0,
                                             ReplenishType = "Purchase",
                                             RoomId = im.Room ?? 0,
                                             SuggestedOrderRoomFlag = rm.SuggestedOrder,
                                             SuggestedTransferRoomFlag = rm.SuggestedTransfer,
                                             TotalCartQtyForItem = 0,
                                             BinId = im.DefaultLocation ?? 0,
                                             RoomName = rm.RoomName,
                                             CompanyName = cm.Name,
                                             BinNumber = im_bm.BinNumber,
                                             ItemGUID = im.GUID,
                                             SupplierID = im.SupplierID ?? 0,
                                             RoomID = im.Room ?? 0,
                                             CompanyID = im.CompanyID ?? 0,
                                             IsSupplierReceivesKitComponents = sm.IsSupplierReceivesKitComponents,
                                             ItemType = im.ItemType,
                                             QtyOnDemand = im.QtyToMeetDemand ?? 0,
                                             ItemTableSO = im.SuggestedOrderQuantity ?? 0
                                         }).FirstOrDefault();

                if (objSuggestedOrderInfo != null && objSuggestedOrderInfo.ItemSuggestedOrderQuantity != objSuggestedOrderInfo.ItemTableSO)
                {
                    ItemMaster objItemMasterSOreclac = context.ItemMasters.Where(r => r.GUID == ItemGUID).FirstOrDefault();
                    if (objItemMasterSOreclac != null)
                    {
                        objItemMasterSOreclac.SuggestedOrderQuantity = objSuggestedOrderInfo.ItemSuggestedOrderQuantity;
                        context.SaveChanges();
                    }
                }

                if ((objSuggestedOrderInfo != null && objSuggestedOrderInfo.SuggestedOrderRoomFlag) || (objSuggestedOrderInfo != null && objSuggestedOrderInfo.SuggestedTransferRoomFlag))
                {
                    /* Explicitlty set 0 as donald instrcuted Transfer out is not a calculaton factor in cart generation*/
                    // TODO: Chirag Patel. Cooment Below line for the issue WI-2639 to consider Transfer out quantity.
                    // objSuggestedOrderInfo.ItemOnTransferOutQuantity = 0;
                    if (objSuggestedOrderInfo.ItemIsPurchase && objSuggestedOrderInfo.ItemIsTranser)
                    {
                        objSuggestedOrderInfo.ReplenishType = "Purchase";
                    }
                    else if (objSuggestedOrderInfo.ItemIsTranser)
                    {
                        objSuggestedOrderInfo.ReplenishType = "Transfer";
                    }
                    else if (objSuggestedOrderInfo.ItemIsPurchase)
                    {
                        objSuggestedOrderInfo.ReplenishType = "Purchase";
                    }
                    else
                    {
                        objSuggestedOrderInfo.ReplenishType = null;
                    }
                    double QtyToKit = 0;
                    if (objSuggestedOrderInfo.IsItemLevelMinMaxQtyRequired != null && objSuggestedOrderInfo.IsItemLevelMinMaxQtyRequired == true)
                    {
                        // Kit As itemGUID : 
                        // Kit Componant as itemGUID

                        // if (Kit As itemGUID  has supplier flag false){ 

                        //Double normal CartChartDTO generation
                        //}

                        //else {


                        //if 
                        //}

                        //if (objSuggestedOrderInfo.IsSupplierReceivesKitComponents && objSuggestedOrderInfo.ItemType == 3)
                        //{
                        //    if (objSuggestedOrderInfo.ItemOnHandQuantity < 0)
                        //    {
                        //        objSuggestedOrderInfo.AvailableQuantity = (0 + objSuggestedOrderInfo.ItemOrderedQuantity + objSuggestedOrderInfo.ItemOnTransferInQuantity + objSuggestedOrderInfo.TotalCartQtyForItem) - (objSuggestedOrderInfo.ItemRequisitionQuantity + objSuggestedOrderInfo.ItemOnTransferOutQuantity);
                        //    }
                        //    else
                        //    {
                        //        objSuggestedOrderInfo.AvailableQuantity = (objSuggestedOrderInfo.ItemOnHandQuantity + objSuggestedOrderInfo.ItemOrderedQuantity + objSuggestedOrderInfo.ItemOnTransferInQuantity + objSuggestedOrderInfo.TotalCartQtyForItem) - (objSuggestedOrderInfo.ItemRequisitionQuantity + objSuggestedOrderInfo.ItemOnTransferOutQuantity);
                        //    }
                        //    // Do nothing
                        //}
                        //else if (!objSuggestedOrderInfo.IsSupplierReceivesKitComponents && objSuggestedOrderInfo.ItemType == 3)
                        //{
                        //    // Normal Procedure
                        //    if (objSuggestedOrderInfo.ItemOnHandQuantity < 0)
                        //    {
                        //        objSuggestedOrderInfo.AvailableQuantity = (0 + objSuggestedOrderInfo.ItemOrderedQuantity + objSuggestedOrderInfo.ItemOnTransferInQuantity + objSuggestedOrderInfo.TotalCartQtyForItem) - (objSuggestedOrderInfo.ItemRequisitionQuantity + objSuggestedOrderInfo.ItemOnTransferOutQuantity);
                        //    }
                        //    else
                        //    {
                        //        objSuggestedOrderInfo.AvailableQuantity = (objSuggestedOrderInfo.ItemOnHandQuantity + objSuggestedOrderInfo.ItemOrderedQuantity + objSuggestedOrderInfo.ItemOnTransferInQuantity + objSuggestedOrderInfo.TotalCartQtyForItem) - (objSuggestedOrderInfo.ItemRequisitionQuantity + objSuggestedOrderInfo.ItemOnTransferOutQuantity);
                        //    }
                        //}
                        //else
                        //{
                        if (objSuggestedOrderInfo.ItemType != 3)
                        {



                            List<SuggestedOrderInfo> objListSuggOrd = new List<SuggestedOrderInfo>();
                            objListSuggOrd = (from km in context.KitDetails
                                              join im in context.ItemMasters on km.KitGUID equals im.GUID
                                              join sm in context.SupplierMasters on im.SupplierID equals sm.ID
                                              join rm in context.Rooms on im.Room equals rm.ID
                                              join cm in context.CompanyMasters on rm.CompanyID equals cm.ID
                                              join bm in context.BinMasters on im.DefaultLocation equals bm.ID into im_bm_join
                                              from im_bm in im_bm_join.DefaultIfEmpty()

                                              where km.ItemGUID == ItemGUID && km.IsDeleted == false && km.IsArchived == false && im.IsDeleted == false
                                              select new SuggestedOrderInfo
                                              {
                                                  AvailableQuantity = 0,
                                                  CartQuantity = 0,
                                                  CompanyId = im.CompanyID ?? 0,
                                                  DefaultReorderQuantity = im.DefaultReorderQuantity,
                                                  devideval = 0,
                                                  IsEnforceDefaultReorderQuantity = im.IsEnforceDefaultReorderQuantity ?? false,
                                                  IsItemLevelMinMaxQtyRequired = im.IsItemLevelMinMaxQtyRequired,
                                                  ItemCriticalQuantity = im.CriticalQuantity,
                                                  ItemIsPurchase = im.IsPurchase ?? false,
                                                  ItemIsTranser = im.IsTransfer ?? false,
                                                  ItemMaximumQuantity = im.MaximumQuantity,
                                                  ItemMinimumQuantity = im.MinimumQuantity,
                                                  ItemOnHandQuantity = im.OnHandQuantity ?? 0,
                                                  ItemOnTransferInQuantity = im.OnTransferQuantity ?? 0,
                                                  //ItemOnTransferOutQuantity = im.InTransitquantity ?? 0,
                                                  ItemOnTransferOutQuantity = im.OutTransferQuantity,

                                                  ItemOrderedQuantity = onOrderQuantity,
                                                  ItemRequisitionQuantity = im.RequisitionedQuantity ?? 0,
                                                  ItemSuggestedOrderQuantity = SuggestedOrderqty,
                                                  moduloval = 0,
                                                  ReplenishType = "Purchase",
                                                  RoomId = im.Room ?? 0,
                                                  SuggestedOrderRoomFlag = rm.SuggestedOrder,
                                                  SuggestedTransferRoomFlag = rm.SuggestedTransfer,
                                                  TotalCartQtyForItem = 0,
                                                  BinId = im.DefaultLocation ?? 0,
                                                  RoomName = rm.RoomName,
                                                  CompanyName = cm.Name,
                                                  BinNumber = im_bm.BinNumber,
                                                  ItemGUID = im.GUID,
                                                  SupplierID = im.SupplierID ?? 0,
                                                  RoomID = im.Room ?? 0,
                                                  CompanyID = im.CompanyID ?? 0,
                                                  IsSupplierReceivesKitComponents = sm.IsSupplierReceivesKitComponents,
                                                  ItemType = im.ItemType,
                                                  QtyOnDemand = km.QtyToMeetDemand ?? 0,
                                              }
                                       ).ToList();
                            if (objListSuggOrd != null && objListSuggOrd.Count() > 0)
                            {
                                foreach (var item in objListSuggOrd.ToList())
                                {
                                    if (item.IsSupplierReceivesKitComponents)
                                    {
                                        QtyToKit = QtyToKit + item.QtyOnDemand;
                                    }
                                }
                            }
                        }
                        // Get all the kits headetr for this item and get supplier details and chreck for the flag and if one of the supplier found true then deduct Quantity to meet demad while calc of available
                        // Get all the kits headetr for this item and get supplier details and chreck for the flag and if one of the supplier found true then deduct Quantity to meet demad while calc of available
                        //}


                        objSuggestedOrderInfo.TotalCartQtyForItem = GetSuggestedOrderQty(ItemGUID, objSuggestedOrderInfo.ReplenishType);
                        //if (objSuggestedOrderInfo.IsSupplierReceivesKitComponents && objSuggestedOrderInfo.ItemType == 3)
                        //{
                        //    if (objSuggestedOrderInfo.ItemOnHandQuantity < 0)
                        //    {
                        //        objSuggestedOrderInfo.AvailableQuantity = (0 + objSuggestedOrderInfo.ItemOrderedQuantity + objSuggestedOrderInfo.ItemOnTransferInQuantity + objSuggestedOrderInfo.TotalCartQtyForItem) - (objSuggestedOrderInfo.ItemRequisitionQuantity + objSuggestedOrderInfo.ItemOnTransferOutQuantity + objSuggestedOrderInfo.QtyOnDemand);
                        //    }
                        //    else
                        //    {
                        //        objSuggestedOrderInfo.AvailableQuantity = (objSuggestedOrderInfo.ItemOnHandQuantity + objSuggestedOrderInfo.ItemOrderedQuantity + objSuggestedOrderInfo.ItemOnTransferInQuantity + objSuggestedOrderInfo.TotalCartQtyForItem) - (objSuggestedOrderInfo.ItemRequisitionQuantity + objSuggestedOrderInfo.ItemOnTransferOutQuantity + objSuggestedOrderInfo.QtyOnDemand);
                        //    }
                        //}
                        //else
                        //{

                        if (objSuggestedOrderInfo.ItemOnHandQuantity < 0)
                        {
                            objSuggestedOrderInfo.AvailableQuantity = (0 + objSuggestedOrderInfo.ItemOrderedQuantity + objSuggestedOrderInfo.ItemOnTransferInQuantity + objSuggestedOrderInfo.TotalCartQtyForItem) - (objSuggestedOrderInfo.ItemRequisitionQuantity + objSuggestedOrderInfo.ItemOnTransferOutQuantity + QtyToKit);
                        }
                        else
                        {
                            objSuggestedOrderInfo.AvailableQuantity = (objSuggestedOrderInfo.ItemOnHandQuantity + objSuggestedOrderInfo.ItemOrderedQuantity + objSuggestedOrderInfo.ItemOnTransferInQuantity + objSuggestedOrderInfo.TotalCartQtyForItem) - (objSuggestedOrderInfo.ItemRequisitionQuantity + objSuggestedOrderInfo.ItemOnTransferOutQuantity + QtyToKit);
                        }
                        // }

                        if (objSuggestedOrderInfo.AvailableQuantity < objSuggestedOrderInfo.ItemMinimumQuantity && objSuggestedOrderInfo.AvailableQuantity < 0)
                        {
                            objSuggestedOrderInfo.CartQuantity = objSuggestedOrderInfo.ItemMaximumQuantity - objSuggestedOrderInfo.AvailableQuantity;
                        }
                        else if (objSuggestedOrderInfo.AvailableQuantity < objSuggestedOrderInfo.ItemMinimumQuantity && objSuggestedOrderInfo.AvailableQuantity >= 0)
                        {
                            objSuggestedOrderInfo.AvailableQuantity = objSuggestedOrderInfo.AvailableQuantity + objSuggestedOrderInfo.ItemRequisitionQuantity;
                            objSuggestedOrderInfo.CartQuantity = objSuggestedOrderInfo.ItemMaximumQuantity - objSuggestedOrderInfo.AvailableQuantity;
                        }
                        else
                        {
                            CartItem objCartItem = context.CartItems.FirstOrDefault(t => t.ItemGUID == ItemGUID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false && t.IsAutoMatedEntry == true && t.ReplenishType == objSuggestedOrderInfo.ReplenishType && t.Room == objSuggestedOrderInfo.RoomId && t.CompanyID == objSuggestedOrderInfo.CompanyId);
                            if (objCartItem != null)
                            {
                                objCartItem.IsDeleted = true;
                                objCartItem.LastUpdatedBy = UserId;
                                objCartItem.Updated = DateTimeUtility.DateTimeNow;
                                if (IsFromUI)
                                {
                                    objCartItem.EditedFrom = "Web";
                                    objCartItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                                }
                                context.SaveChanges();
                                ItemMaster objItemMaster = context.ItemMasters.FirstOrDefault(t => t.GUID == ItemGUID);
                                if (objItemMaster != null)
                                {
                                    Dictionary<string, double> SOT = GetItemSuggestedOrderQty(ItemGUID);
                                    objItemMaster.SuggestedOrderQuantity = SOT["SO"];
                                    objItemMaster.SuggestedTransferQuantity = SOT["ST"];
                                    objItemMaster.WhatWhereAction = "Cart";
                                    context.SaveChanges();
                                }
                            }
                        }
                        if (objSuggestedOrderInfo.CartQuantity > 0)
                        {
                            if (objSuggestedOrderInfo.IsEnforceDefaultReorderQuantity && objSuggestedOrderInfo.DefaultReorderQuantity > 0)
                            {
                                objSuggestedOrderInfo.moduloval = objSuggestedOrderInfo.CartQuantity % objSuggestedOrderInfo.DefaultReorderQuantity;
                                if (objSuggestedOrderInfo.moduloval != 0)
                                {
                                    objSuggestedOrderInfo.devideval = Convert.ToInt64(Math.Ceiling(objSuggestedOrderInfo.CartQuantity / objSuggestedOrderInfo.DefaultReorderQuantity));
                                    objSuggestedOrderInfo.CartQuantity = objSuggestedOrderInfo.DefaultReorderQuantity * objSuggestedOrderInfo.devideval;
                                }
                            }
                            //else if (objSuggestedOrderInfo.CartQuantity >= objSuggestedOrderInfo.ItemMaximumQuantity)
                            //{
                            //    objSuggestedOrderInfo.CartQuantity = objSuggestedOrderInfo.ItemMaximumQuantity;
                            //}

                            if ((objSuggestedOrderInfo.ReplenishType == "Purchase" && objSuggestedOrderInfo.SuggestedOrderRoomFlag == true) || (objSuggestedOrderInfo.ReplenishType == "Transfer" && objSuggestedOrderInfo.SuggestedTransferRoomFlag == true))
                            {
                                //CartItem objCartItem = context.CartItems.FirstOrDefault(t => t.ItemGUID == ItemGUID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false && t.IsAutoMatedEntry == true && t.ReplenishType == objSuggestedOrderInfo.ReplenishType && t.Room == objSuggestedOrderInfo.RoomId && t.CompanyID == objSuggestedOrderInfo.CompanyId);
                                CartItem objCartItem = null;
                                IQueryable<CartItem> lstAutocarts = context.CartItems.Where(t => t.ItemGUID == ItemGUID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false && t.IsAutoMatedEntry == true && t.ReplenishType == objSuggestedOrderInfo.ReplenishType && t.Room == objSuggestedOrderInfo.RoomId && t.CompanyID == objSuggestedOrderInfo.CompanyId);
                                int i = 0;
                                foreach (CartItem mulitem in lstAutocarts)
                                {
                                    if (i == 0)
                                    {
                                        objCartItem = mulitem;
                                    }
                                    else
                                    {
                                        mulitem.IsDeleted = true;
                                    }
                                    i++;
                                }
                                if (objCartItem != null)
                                {
                                    objCartItem.ReplenishType = objSuggestedOrderInfo.ReplenishType;
                                    objCartItem.BinId = objSuggestedOrderInfo.BinId;
                                    objCartItem.LastUpdatedBy = UserId;
                                    objCartItem.Updated = DateTimeUtility.DateTimeNow;
                                    objCartItem.Quantity = objSuggestedOrderInfo.CartQuantity;
                                    if (IsFromUI)
                                    {
                                        objCartItem.EditedFrom = "Web";
                                        objCartItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    }
                                    context.SaveChanges();
                                    ItemMaster objItemMaster = context.ItemMasters.FirstOrDefault(t => t.GUID == ItemGUID);
                                    if (objItemMaster != null)
                                    {
                                        Dictionary<string, double> SOT = GetItemSuggestedOrderQty(ItemGUID);
                                        objItemMaster.SuggestedOrderQuantity = SOT["SO"];
                                        objItemMaster.SuggestedTransferQuantity = SOT["ST"];

                                        objItemMaster.WhatWhereAction = "Cart";
                                        context.SaveChanges();
                                        if (objSuggestedOrderInfo.ItemSuggestedOrderQuantity != (objItemMaster.SuggestedOrderQuantity ?? 0))
                                        {
                                            if ((objItemMaster.OnHandQuantity ?? 0) <= objItemMaster.CriticalQuantity)
                                            {
                                                if (sendMail)
                                                {

                                                    SendMailForSuggestedOrder(objItemMaster.ItemNumber, objItemMaster.OnHandQuantity ?? 0, objItemMaster.CriticalQuantity, objItemMaster.MinimumQuantity, objItemMaster.Room ?? 0, objItemMaster.CompanyID ?? 0, "Critical", objSuggestedOrderInfo.RoomName, UserId, "SuggestedOrdersCritical", EnterpriseId, objItemMaster.SuggestedOrderQuantity ?? 0, objSuggestedOrderInfo.BinNumber);
                                                }
                                                if (HttpContext.Current != null)
                                                {
                                                    // GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.All.UpdateRedCircleCountInClients();
                                                }
                                            }
                                            else if ((objItemMaster.OnHandQuantity ?? 0) < objItemMaster.MinimumQuantity)
                                            {
                                                if (sendMail)
                                                {

                                                    SendMailForSuggestedOrder(objItemMaster.ItemNumber, objItemMaster.OnHandQuantity ?? 0, objItemMaster.CriticalQuantity, objItemMaster.MinimumQuantity, objItemMaster.Room ?? 0, objItemMaster.CompanyID ?? 0, "Minimum", objSuggestedOrderInfo.RoomName, UserId, "SuggestedOrdersMinimum", EnterpriseId, objItemMaster.SuggestedOrderQuantity ?? 0, objSuggestedOrderInfo.BinNumber);
                                                }
                                                if (HttpContext.Current != null)
                                                {
                                                    // GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.All.UpdateRedCircleCountInClients();
                                                }
                                            }
                                            //if ((objItemMaster.OnHandQuantity ?? 0) >= objItemMaster.CriticalQuantity && (objItemMaster.OnHandQuantity ?? 0) < objItemMaster.MinimumQuantity)
                                            //{
                                            //    //SuggestedOrdersMinimum,SuggestedOrdersCritical
                                            //    SendMailForSuggestedOrder(objItemMaster.ItemNumber, objItemMaster.OnHandQuantity ?? 0, objItemMaster.CriticalQuantity, objItemMaster.MinimumQuantity, objItemMaster.Room ?? 0, objItemMaster.CompanyID ?? 0, "Minimum", objSuggestedOrderInfo.RoomName, UserId, "SuggestedOrdersMinimum", EnterpriseId, objItemMaster.SuggestedOrderQuantity ?? 0, objSuggestedOrderInfo.BinNumber);
                                            //}
                                            //else if ((objItemMaster.OnHandQuantity ?? 0) < objItemMaster.CriticalQuantity)
                                            //{
                                            //    SendMailForSuggestedOrder(objItemMaster.ItemNumber, objItemMaster.OnHandQuantity ?? 0, objItemMaster.CriticalQuantity, objItemMaster.MinimumQuantity, objItemMaster.Room ?? 0, objItemMaster.CompanyID ?? 0, "Critical", objSuggestedOrderInfo.RoomName, UserId, "SuggestedOrdersCritical", EnterpriseId, objItemMaster.SuggestedOrderQuantity ?? 0, objSuggestedOrderInfo.BinNumber);
                                            //}
                                        }
                                    }
                                }
                                else
                                {
                                    CartItemDTO objCartItemDTO = new CartItemDTO();
                                    objCartItemDTO.BinGUID = null;
                                    objCartItemDTO.BinId = objSuggestedOrderInfo.BinId;
                                    objCartItemDTO.BinName = null;
                                    objCartItemDTO.CompanyID = objSuggestedOrderInfo.CompanyId;
                                    objCartItemDTO.Created = DateTimeUtility.DateTimeNow;
                                    objCartItemDTO.CreatedBy = UserId;
                                    objCartItemDTO.CreatedByName = null;
                                    objCartItemDTO.GUID = Guid.NewGuid();
                                    objCartItemDTO.ID = 0;
                                    objCartItemDTO.IsArchived = false;
                                    objCartItemDTO.IsAutoMatedEntry = true;
                                    objCartItemDTO.IsDeleted = false;
                                    objCartItemDTO.IsKitComponent = false;
                                    objCartItemDTO.IsPurchase = true;
                                    objCartItemDTO.IsTransfer = false;
                                    objCartItemDTO.ItemGUID = ItemGUID;
                                    objCartItemDTO.ItemNumber = null;
                                    objCartItemDTO.LastUpdatedBy = UserId;
                                    objCartItemDTO.Quantity = objSuggestedOrderInfo.CartQuantity;
                                    objCartItemDTO.ReplenishType = objSuggestedOrderInfo.ReplenishType;
                                    objCartItemDTO.Room = objSuggestedOrderInfo.RoomId;
                                    objCartItemDTO.RoomName = null;
                                    objCartItemDTO.Status = "A";
                                    objCartItemDTO.SupplierId = 0;
                                    objCartItemDTO.SupplierName = null;
                                    objCartItemDTO.UDF1 = "";
                                    objCartItemDTO.UDF2 = "";
                                    objCartItemDTO.UDF3 = "";
                                    objCartItemDTO.UDF4 = "";
                                    objCartItemDTO.UDF5 = "";
                                    objCartItemDTO.Updated = DateTimeUtility.DateTimeNow;
                                    objCartItemDTO.UpdatedByName = null;
                                    objCartItemDTO.IsOnlyFromItemUI = IsFromUI;
                                    objCartItemDTO.AddedFrom = "Web";
                                    objCartItemDTO.EditedFrom = "Web";
                                    objCartItemDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    objCartItemDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                    SaveCartItem(objCartItemDTO);
                                    ItemMaster objItemMaster = context.ItemMasters.FirstOrDefault(t => t.GUID == ItemGUID);
                                    if (objItemMaster != null)
                                    {
                                        Dictionary<string, double> SOT = GetItemSuggestedOrderQty(ItemGUID);
                                        objItemMaster.SuggestedOrderQuantity = SOT["SO"];
                                        objItemMaster.SuggestedTransferQuantity = SOT["ST"];

                                        objItemMaster.WhatWhereAction = "Cart";
                                        context.SaveChanges();
                                    }
                                    //if ((objItemMaster.OnHandQuantity ?? 0) >= objItemMaster.CriticalQuantity && (objItemMaster.OnHandQuantity ?? 0) < objItemMaster.MinimumQuantity)
                                    //{
                                    //    SendMailForSuggestedOrder(objItemMaster.ItemNumber, objItemMaster.OnHandQuantity ?? 0, objItemMaster.CriticalQuantity, objItemMaster.MinimumQuantity, objItemMaster.Room ?? 0, objItemMaster.CompanyID ?? 0, "Minimum", objSuggestedOrderInfo.RoomName, UserId, "SuggestedOrdersMinimum", EnterpriseId, (objItemMaster.SuggestedOrderQuantity ?? 0), objSuggestedOrderInfo.BinNumber);
                                    //}
                                    //else if ((objItemMaster.OnHandQuantity ?? 0) < objItemMaster.CriticalQuantity)
                                    //{
                                    //    SendMailForSuggestedOrder(objItemMaster.ItemNumber, objItemMaster.OnHandQuantity ?? 0, objItemMaster.CriticalQuantity, objItemMaster.MinimumQuantity, objItemMaster.Room ?? 0, objItemMaster.CompanyID ?? 0, "Critical", objSuggestedOrderInfo.RoomName, UserId, "SuggestedOrdersCritical", EnterpriseId, (objItemMaster.SuggestedOrderQuantity ?? 0), objSuggestedOrderInfo.BinNumber);
                                    //}

                                    if ((objItemMaster.OnHandQuantity ?? 0) <= objItemMaster.CriticalQuantity)
                                    {
                                        if (sendMail)
                                        {

                                            SendMailForSuggestedOrder(objItemMaster.ItemNumber, objItemMaster.OnHandQuantity ?? 0, objItemMaster.CriticalQuantity, objItemMaster.MinimumQuantity, objItemMaster.Room ?? 0, objItemMaster.CompanyID ?? 0, "Critical", objSuggestedOrderInfo.RoomName, UserId, "SuggestedOrdersCritical", EnterpriseId, objItemMaster.SuggestedOrderQuantity ?? 0, objSuggestedOrderInfo.BinNumber);
                                        }
                                        if (HttpContext.Current != null)
                                        {
                                            //  GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.All.UpdateRedCircleCountInClients();
                                        }
                                    }
                                    else if ((objItemMaster.OnHandQuantity ?? 0) < objItemMaster.MinimumQuantity)
                                    {
                                        if (sendMail)
                                        {

                                            SendMailForSuggestedOrder(objItemMaster.ItemNumber, objItemMaster.OnHandQuantity ?? 0, objItemMaster.CriticalQuantity, objItemMaster.MinimumQuantity, objItemMaster.Room ?? 0, objItemMaster.CompanyID ?? 0, "Minimum", objSuggestedOrderInfo.RoomName, UserId, "SuggestedOrdersMinimum", EnterpriseId, objItemMaster.SuggestedOrderQuantity ?? 0, objSuggestedOrderInfo.BinNumber);
                                        }
                                        if (HttpContext.Current != null)
                                        {
                                            // GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.All.UpdateRedCircleCountInClients();
                                        }
                                    }

                                    //SendMailForSuggestedOrder(objItemMaster.ItemNumber,objItemMaster.OnHandQuantity,objItemMaster.CriticalQuantity,objItemMaster.MinimumQuantity,objItemMaster.Room??0,objItemMaster.CompanyID??0,)
                                }
                            }
                        }
                        else
                        {
                            CartItem objCartItem = context.CartItems.FirstOrDefault(ci => ci.ItemGUID == ItemGUID && (ci.IsDeleted ?? false) == false && (ci.IsArchived ?? false) == false && ci.IsAutoMatedEntry == true && ci.ReplenishType == objSuggestedOrderInfo.ReplenishType && ci.Room == objSuggestedOrderInfo.RoomId && ci.CompanyID == objSuggestedOrderInfo.CompanyId);
                            if (objCartItem != null)
                            {
                                objCartItem.IsDeleted = true;
                                objCartItem.LastUpdatedBy = UserId;
                                objCartItem.Updated = DateTimeUtility.DateTimeNow;
                                if (IsFromUI)
                                {
                                    objCartItem.EditedFrom = "Web";
                                    objCartItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                                }
                                context.SaveChanges();
                                ItemMaster objItemMaster = context.ItemMasters.FirstOrDefault(im => im.GUID == ItemGUID);
                                if (objItemMaster != null)
                                {
                                    Dictionary<string, double> SOT = GetItemSuggestedOrderQty(ItemGUID);
                                    objItemMaster.SuggestedOrderQuantity = SOT["SO"];
                                    objItemMaster.SuggestedTransferQuantity = SOT["ST"];
                                    objItemMaster.WhatWhereAction = "Cart";
                                    context.SaveChanges();
                                }
                            }
                        }
                    }
                    else if (objSuggestedOrderInfo.IsItemLevelMinMaxQtyRequired != null && objSuggestedOrderInfo.IsItemLevelMinMaxQtyRequired == false)
                    {
                        //List<BinMaster> lstItemlocations = new List<BinMaster>();
                        //lstItemlocations = context.BinMasters.OrderBy(t => t.BinNumber).Where(t => t.IsDeleted == false && (t.IsArchived ?? false) == false && t.ItemGUID == ItemGUID).ToList();
                        List<BinMasterDTO> lstItemlocations1 = GetItemLocations(ItemGUID).OrderBy(t => t.BinNumber).ToList();
                        int cnt = 0;
                        lstItemlocations1.ForEach(t =>
                        {
                            double ItemLocationMinQuantity = t.MinimumQuantity ?? 0;
                            double ItemLocationMaxQuantity = t.MaximumQuantity ?? 0;
                            double ItemLocationCriticalQuantity = t.CriticalQuantity ?? 0;
                            double ItemLocationCustomerOwnedQuantity = 0;
                            double ItemLocationConsignedQuantity = 0;
                            double ItemLocationOnHandQuantity = 0;
                            double ItemLocationOnOrderedQuantity = 0;
                            double ItemLocationOnRequisitionedQuantity = 0;
                            double ItemLocationOnTransferQuantity = 0;
                            if ((t.IsDefault ?? false))
                            {
                                var ords = (from od in context.OrderDetails
                                            join om in context.OrderMasters on od.OrderGUID equals om.GUID
                                            where od.ItemGUID == ItemGUID && OrdStatus.Contains(om.OrderStatus) && om.IsDeleted == false && (od.IsDeleted ?? false) == false && od.Room == objSuggestedOrderInfo.RoomId && od.CompanyID == objSuggestedOrderInfo.CompanyId && om.OrderType == 1 && (od.Bin == null || od.Bin == t.ID)
                                            select od);
                                if (ords.Any())
                                {
                                    ItemLocationOnOrderedQuantity = ords.Sum(sumt => ((sumt.RequestedQuantity ?? 0) - (sumt.ReceivedQuantity ?? 0)) > 0 ? ((sumt.RequestedQuantity ?? 0) - (sumt.ReceivedQuantity ?? 0)) : 0);
                                }
                                var reqs = (from od in context.RequisitionDetails
                                            join om in context.RequisitionMasters on od.RequisitionGUID equals om.GUID
                                            where od.ItemGUID == ItemGUID && ReqStatus.Contains(om.RequisitionStatus) && (om.IsDeleted ?? false) == false && (od.IsDeleted ?? false) == false && od.Room == objSuggestedOrderInfo.RoomId && od.CompanyID == objSuggestedOrderInfo.CompanyId && (od.BinID == null || od.BinID == t.ID)
                                            select od);
                                if (reqs.Any())
                                {
                                    ItemLocationOnRequisitionedQuantity = reqs.Sum(sumt => ((sumt.QuantityRequisitioned ?? 0) - (sumt.QuantityPulled ?? 0)) > 0 ? ((sumt.QuantityRequisitioned ?? 0) - (sumt.QuantityPulled ?? 0)) : 0);
                                }
                                var trasns = (from od in context.TransferDetails
                                              join om in context.TransferMasters on od.TransferGUID equals om.GUID
                                              where od.ItemGUID == ItemGUID && od.ItemGUID == ItemGUID && transStatus.Contains(om.TransferStatus ?? 0) && (om.IsDeleted ?? false) == false && (od.IsDeleted ?? false) == false && od.Room == objSuggestedOrderInfo.RoomId && od.CompanyID == objSuggestedOrderInfo.CompanyId && (od.Bin == null || od.Bin == t.ID)
                                              select od);
                                if (trasns.Any())
                                {
                                    ItemLocationOnTransferQuantity = trasns.Sum(sumt => (sumt.RequestedQuantity - (sumt.ReceivedQuantity ?? 0)) > 0 ? (sumt.RequestedQuantity - (sumt.ReceivedQuantity ?? 0)) : 0);
                                }
                            }
                            else
                            {
                                var ords = (from od in context.OrderDetails
                                            join om in context.OrderMasters on od.OrderGUID equals om.GUID
                                            where od.ItemGUID == ItemGUID && OrdStatus.Contains(om.OrderStatus) && om.IsDeleted == false && (od.IsDeleted ?? false) == false && od.Room == objSuggestedOrderInfo.RoomId && od.CompanyID == objSuggestedOrderInfo.CompanyId && om.OrderType == 1 && (od.Bin == t.ID)
                                            select od);
                                if (ords.Any())
                                {
                                    ItemLocationOnOrderedQuantity = ords.Sum(sumt => ((sumt.RequestedQuantity ?? 0) - (sumt.ReceivedQuantity ?? 0)) > 0 ? ((sumt.RequestedQuantity ?? 0) - (sumt.ReceivedQuantity ?? 0)) : 0);
                                }
                                var reqs = (from od in context.RequisitionDetails
                                            join om in context.RequisitionMasters on od.RequisitionGUID equals om.GUID
                                            where od.ItemGUID == ItemGUID && ReqStatus.Contains(om.RequisitionStatus) && (om.IsDeleted ?? false) == false && (od.IsDeleted ?? false) == false && od.Room == objSuggestedOrderInfo.RoomId && od.CompanyID == objSuggestedOrderInfo.CompanyId && (od.BinID == t.ID)
                                            select od);
                                if (reqs.Any())
                                {
                                    ItemLocationOnRequisitionedQuantity = reqs.Sum(sumt => ((sumt.QuantityRequisitioned ?? 0) - (sumt.QuantityPulled ?? 0)) > 0 ? ((sumt.QuantityRequisitioned ?? 0) - (sumt.QuantityPulled ?? 0)) : 0);
                                }
                                var trasns = (from od in context.TransferDetails
                                              join om in context.TransferMasters on od.TransferGUID equals om.GUID
                                              where od.ItemGUID == ItemGUID && transStatus.Contains(om.TransferStatus ?? 0) && (om.IsDeleted ?? false) == false && (od.IsDeleted ?? false) == false && od.Room == objSuggestedOrderInfo.RoomId && od.CompanyID == objSuggestedOrderInfo.CompanyId && (od.Bin == t.ID)
                                              select od);
                                if (trasns.Any())
                                {
                                    ItemLocationOnTransferQuantity = trasns.Sum(sumt => (sumt.RequestedQuantity - (sumt.ReceivedQuantity ?? 0)) > 0 ? (sumt.RequestedQuantity - (sumt.ReceivedQuantity ?? 0)) : 0);
                                }
                            }
                            cnt++;
                            if (context.ItemLocationDetails.Any(il => (il.IsDeleted ?? false) == false && (il.IsArchived ?? false) == false && (il.ItemGUID ?? Guid.Empty) == ItemGUID && (il.BinID ?? 0) == t.ID))
                            {
                                ItemLocationCustomerOwnedQuantity = context.ItemLocationDetails.Where(il => (il.IsDeleted ?? false) == false && (il.IsArchived ?? false) == false && (il.ItemGUID ?? Guid.Empty) == ItemGUID && (il.BinID ?? 0) == t.ID).Sum(il => (il.CustomerOwnedQuantity ?? 0));
                                ItemLocationConsignedQuantity = context.ItemLocationDetails.Where(il => (il.IsDeleted ?? false) == false && (il.IsArchived ?? false) == false && (il.ItemGUID ?? Guid.Empty) == ItemGUID && (il.BinID ?? 0) == t.ID).Sum(il => (il.ConsignedQuantity ?? 0));
                                ItemLocationOnHandQuantity = ItemLocationConsignedQuantity + ItemLocationCustomerOwnedQuantity;
                            }

                            //if (context.OrderDetails.Any(il => (il.IsDeleted ?? false) == false && (il.IsArchived ?? false) == false && (il.ItemGUID ?? Guid.Empty) == ItemGUID && (il.Bin ?? 0) == t.BinID))
                            //{
                            //    ItemLocationOnOrderedreqQuantity = context.OrderDetails.Where(il => (il.IsDeleted ?? false) == false && (il.IsArchived ?? false) == false && (il.ItemGUID ?? Guid.Empty) == ItemGUID && (il.Bin ?? 0) == t.BinID).Sum(il => (il.RequestedQuantity ?? 0));
                            //    ItemLocationOnOrderedrecQuantity = context.OrderDetails.Where(il => (il.IsDeleted ?? false) == false && (il.IsArchived ?? false) == false && (il.ItemGUID ?? Guid.Empty) == ItemGUID && (il.Bin ?? 0) == t.BinID).Sum(il => (il.ReceivedQuantity ?? 0));
                            //    ItemLocationOnOrderedQuantity = ItemLocationOnOrderedreqQuantity - ItemLocationOnOrderedrecQuantity;
                            //    if (ItemLocationOnOrderedQuantity < 0)
                            //    {
                            //        ItemLocationOnOrderedQuantity = 0;
                            //    }
                            //}

                            double ItemLocationCartQuantity = 0;
                            if (context.CartItems.Any(ci => (ci.IsDeleted ?? false) == false && (ci.IsArchived ?? false) == false && (ci.ItemGUID ?? Guid.Empty) == ItemGUID && (ci.BinId ?? 0) == t.ID && ci.ReplenishType == objSuggestedOrderInfo.ReplenishType && ci.IsAutoMatedEntry == false))
                            {
                                ItemLocationCartQuantity = context.CartItems.Where(ci => (ci.IsDeleted ?? false) == false && (ci.IsArchived ?? false) == false && (ci.ItemGUID ?? Guid.Empty) == ItemGUID && (ci.BinId ?? 0) == t.ID && ci.ReplenishType == objSuggestedOrderInfo.ReplenishType && ci.IsAutoMatedEntry == false).Sum(ci => (ci.Quantity ?? 0));
                            }
                            //double ItemLocationAvailableQuantity = ItemLocationOnHandQuantity + ItemLocationCartQuantity + ItemLocationOnOrderedQuantity;
                            double ItemLocationAvailableQuantity = 0;
                            double ItemLocationSuggestedCartQuantity = 0;
                            if (ItemLocationOnHandQuantity < 0)
                            {
                                ItemLocationAvailableQuantity = (0 + ItemLocationOnOrderedQuantity + ItemLocationOnTransferQuantity + ItemLocationCartQuantity) - (ItemLocationOnRequisitionedQuantity);
                            }
                            else
                            {
                                ItemLocationAvailableQuantity = (ItemLocationOnHandQuantity + ItemLocationOnOrderedQuantity + ItemLocationOnTransferQuantity + ItemLocationCartQuantity) - (ItemLocationOnRequisitionedQuantity);
                            }




                            if (ItemLocationAvailableQuantity < ItemLocationMinQuantity && ItemLocationAvailableQuantity < 0)
                            {
                                ItemLocationSuggestedCartQuantity = ItemLocationMaxQuantity - ItemLocationAvailableQuantity;
                            }
                            else if (ItemLocationAvailableQuantity < ItemLocationMinQuantity && ItemLocationAvailableQuantity >= 0)
                            {
                                ItemLocationAvailableQuantity = ItemLocationAvailableQuantity + ItemLocationOnRequisitionedQuantity;
                                ItemLocationSuggestedCartQuantity = ItemLocationMaxQuantity - ItemLocationAvailableQuantity;
                            }
                            else
                            {
                                CartItem objCartItem = context.CartItems.FirstOrDefault(ci => ci.ItemGUID == ItemGUID && (ci.IsDeleted ?? false) == false && (ci.IsArchived ?? false) == false && ci.IsAutoMatedEntry == true && ci.ReplenishType == objSuggestedOrderInfo.ReplenishType && ci.Room == objSuggestedOrderInfo.RoomId && ci.CompanyID == objSuggestedOrderInfo.CompanyId && (ci.BinId ?? 0) == t.ID);
                                if (objCartItem != null)
                                {
                                    objCartItem.IsDeleted = true;
                                    objCartItem.LastUpdatedBy = UserId;
                                    objCartItem.Updated = DateTimeUtility.DateTimeNow;
                                    if (IsFromUI)
                                    {
                                        objCartItem.EditedFrom = "Web";
                                        objCartItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    }
                                    context.SaveChanges();
                                    //ItemMaster objItemMaster = context.ItemMasters.FirstOrDefault(im => im.GUID == ItemGUID);
                                    //if (objItemMaster != null)
                                    //{
                                    //    objItemMaster.SuggestedOrderQuantity = GetItemSuggestedOrderQty(ItemGUID);
                                    //    context.SaveChanges();
                                    //}
                                }
                            }
                            if (ItemLocationSuggestedCartQuantity > 0)
                            {
                                if (objSuggestedOrderInfo.IsEnforceDefaultReorderQuantity && objSuggestedOrderInfo.DefaultReorderQuantity > 0)
                                {
                                    objSuggestedOrderInfo.moduloval = Convert.ToInt64(ItemLocationSuggestedCartQuantity) % Convert.ToInt64(objSuggestedOrderInfo.DefaultReorderQuantity);
                                    if (objSuggestedOrderInfo.moduloval != 0)
                                    {
                                        objSuggestedOrderInfo.devideval = (Convert.ToInt64(ItemLocationSuggestedCartQuantity) / Convert.ToInt64(objSuggestedOrderInfo.DefaultReorderQuantity)) + 1;
                                        ItemLocationSuggestedCartQuantity = objSuggestedOrderInfo.DefaultReorderQuantity * objSuggestedOrderInfo.devideval;
                                    }
                                }
                                //else if (ItemLocationSuggestedCartQuantity >= ItemLocationMaxQuantity)
                                //{
                                //    ItemLocationSuggestedCartQuantity = ItemLocationMaxQuantity;
                                //}

                                if ((objSuggestedOrderInfo.ReplenishType == "Purchase" && objSuggestedOrderInfo.SuggestedOrderRoomFlag == true) || (objSuggestedOrderInfo.ReplenishType == "Transfer" && objSuggestedOrderInfo.SuggestedTransferRoomFlag == true))
                                {
                                    //CartItem objCartItem = context.CartItems.FirstOrDefault(ci => ci.ItemGUID == ItemGUID && (ci.IsDeleted ?? false) == false && (ci.IsArchived ?? false) == false && ci.IsAutoMatedEntry == true && ci.ReplenishType == objSuggestedOrderInfo.ReplenishType && ci.Room == objSuggestedOrderInfo.RoomId && ci.CompanyID == objSuggestedOrderInfo.CompanyId && (ci.BinId ?? 0) == t.ID);
                                    CartItem objCartItem = null;
                                    IQueryable<CartItem> lstAutocarts = context.CartItems.Where(ci => ci.ItemGUID == ItemGUID && (ci.IsDeleted ?? false) == false && (ci.IsArchived ?? false) == false && ci.IsAutoMatedEntry == true && ci.ReplenishType == objSuggestedOrderInfo.ReplenishType && ci.Room == objSuggestedOrderInfo.RoomId && ci.CompanyID == objSuggestedOrderInfo.CompanyId && (ci.BinId ?? 0) == t.ID);
                                    int i = 0;
                                    foreach (CartItem mulitem in lstAutocarts)
                                    {
                                        if (i == 0)
                                        {
                                            objCartItem = mulitem;
                                        }
                                        else
                                        {
                                            mulitem.IsDeleted = true;
                                        }
                                        i++;
                                    }
                                    if (objCartItem != null)
                                    {
                                        objCartItem.ReplenishType = objSuggestedOrderInfo.ReplenishType;
                                        objCartItem.LastUpdatedBy = UserId;
                                        objCartItem.Updated = DateTimeUtility.DateTimeNow;
                                        objCartItem.Quantity = ItemLocationSuggestedCartQuantity;
                                        if (IsFromUI)
                                        {
                                            objCartItem.EditedFrom = "Web";
                                            objCartItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        }
                                        context.SaveChanges();
                                        ItemMaster objItemMaster = context.ItemMasters.FirstOrDefault(im => im.GUID == ItemGUID);
                                        if (objItemMaster != null)
                                        {
                                            Dictionary<string, double> SOT = GetItemSuggestedOrderQty(ItemGUID);
                                            objItemMaster.SuggestedOrderQuantity = SOT["SO"];
                                            objItemMaster.SuggestedTransferQuantity = SOT["ST"];
                                            objItemMaster.WhatWhereAction = "Cart";
                                            context.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        CartItemDTO objCartItemDTO = new CartItemDTO();
                                        objCartItemDTO.BinGUID = t.GUID;
                                        objCartItemDTO.BinId = t.ID;
                                        objCartItemDTO.BinName = null;
                                        objCartItemDTO.CompanyID = objSuggestedOrderInfo.CompanyId;
                                        objCartItemDTO.Created = DateTimeUtility.DateTimeNow;
                                        objCartItemDTO.CreatedBy = UserId;
                                        objCartItemDTO.CreatedByName = null;
                                        objCartItemDTO.GUID = Guid.NewGuid();
                                        objCartItemDTO.ID = 0;
                                        objCartItemDTO.IsArchived = false;
                                        objCartItemDTO.IsAutoMatedEntry = true;
                                        objCartItemDTO.IsDeleted = false;
                                        objCartItemDTO.IsKitComponent = false;
                                        objCartItemDTO.IsPurchase = true;
                                        objCartItemDTO.IsTransfer = false;
                                        objCartItemDTO.ItemGUID = ItemGUID;
                                        objCartItemDTO.ItemNumber = null;
                                        objCartItemDTO.LastUpdatedBy = UserId;
                                        objCartItemDTO.Quantity = ItemLocationSuggestedCartQuantity;
                                        objCartItemDTO.ReplenishType = objSuggestedOrderInfo.ReplenishType;
                                        objCartItemDTO.Room = objSuggestedOrderInfo.RoomId;
                                        objCartItemDTO.RoomName = null;
                                        objCartItemDTO.Status = "A";
                                        objCartItemDTO.SupplierId = 0;
                                        objCartItemDTO.SupplierName = null;
                                        objCartItemDTO.UDF1 = "";
                                        objCartItemDTO.UDF2 = "";
                                        objCartItemDTO.UDF3 = "";
                                        objCartItemDTO.UDF4 = "";
                                        objCartItemDTO.UDF5 = "";
                                        objCartItemDTO.Updated = DateTimeUtility.DateTimeNow;
                                        objCartItemDTO.UpdatedByName = null;
                                        objCartItemDTO.IsOnlyFromItemUI = IsFromUI;
                                        objCartItemDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objCartItemDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objCartItemDTO.AddedFrom = "Web";
                                        objCartItemDTO.EditedFrom = "Web";
                                        SaveCartItem(objCartItemDTO);

                                        //if ((objItemMaster.OnHandQuantity ?? 0) >= objItemMaster.CriticalQuantity && (objItemMaster.OnHandQuantity ?? 0) < objItemMaster.MinimumQuantity)
                                        //{
                                        //    SendMailForSuggestedOrder(objItemMaster.ItemNumber, objItemMaster.OnHandQuantity ?? 0, objItemMaster.CriticalQuantity, objItemMaster.MinimumQuantity, objItemMaster.Room ?? 0, objItemMaster.CompanyID ?? 0, "Minimum", "RoomName");
                                        //}
                                        //else if ((objItemMaster.OnHandQuantity ?? 0) < objItemMaster.CriticalQuantity)
                                        //{
                                        //    SendMailForSuggestedOrder(objItemMaster.ItemNumber, objItemMaster.OnHandQuantity ?? 0, objItemMaster.CriticalQuantity, objItemMaster.MinimumQuantity, objItemMaster.Room ?? 0, objItemMaster.CompanyID ?? 0, "Critical", "RoomName");
                                        //}
                                        //ItemMaster objItemMaster = context.ItemMasters.FirstOrDefault(im => im.GUID == ItemGUID);
                                        //if (objItemMaster != null)
                                        //{
                                        //    objItemMaster.SuggestedOrderQuantity = GetItemSuggestedOrderQty(ItemGUID);
                                        //}
                                        //context.SaveChanges();
                                    }
                                }
                            }
                        });
                        DeleteAutoCartItems(ItemGUID, IsFromUI);
                        List<BinMasterDTO> lstItemlocations2 = GetItemLocations(ItemGUID);
                        lstItemlocations2.Where(t => (t.SuggestedOrderQuantity ?? 0) > 0).ToList().ForEach(t =>
                        {
                            BinMasterDTO objThisBin = lstItemlocations1.Where(a => a.ItemGUID == t.ItemGUID && a.ID == t.ID).FirstOrDefault();
                            if (objThisBin == null)
                            {
                                if ((t.OnHandQuantity ?? 0) >= t.CriticalQuantity && (t.OnHandQuantity ?? 0) < t.MinimumQuantity)
                                {
                                    if (sendMail)
                                    {
                                        if ((t.OnHandQuantity ?? 0) <= t.CriticalQuantity)
                                        {
                                            SendMailForSuggestedOrder(t.ItemNumber, t.OnHandQuantity ?? 0, t.CriticalQuantity ?? 0, t.MinimumQuantity ?? 0, t.Room ?? 0, t.CompanyID ?? 0, "Critical", t.RoomName, UserId, "SuggestedOrdersCritical", EnterpriseId, (t.SuggestedOrderQuantity ?? 0), t.BinNumber);
                                        }
                                        else if ((t.OnHandQuantity ?? 0) < t.MinimumQuantity)
                                        {
                                            SendMailForSuggestedOrder(t.ItemNumber, t.OnHandQuantity ?? 0, t.CriticalQuantity ?? 0, t.MinimumQuantity ?? 0, t.Room ?? 0, t.CompanyID ?? 0, "Minimum", t.RoomName, UserId, "SuggestedOrdersMinimum", EnterpriseId, (t.SuggestedOrderQuantity ?? 0), t.BinNumber);
                                        }
                                    }

                                    if (HttpContext.Current != null)
                                    {
                                        //GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.All.UpdateRedCircleCountInClients();
                                    }
                                }
                            }
                            else
                            {
                                if ((objThisBin.SuggestedOrderQuantity ?? 0) != (t.SuggestedOrderQuantity ?? 0))
                                {
                                    if (sendMail)
                                    {
                                        if ((t.OnHandQuantity ?? 0) <= t.CriticalQuantity)
                                        {
                                            SendMailForSuggestedOrder(t.ItemNumber, t.OnHandQuantity ?? 0, t.CriticalQuantity ?? 0, t.MinimumQuantity ?? 0, t.Room ?? 0, t.CompanyID ?? 0, "Critical", t.RoomName, UserId, "SuggestedOrdersCritical", EnterpriseId, (t.SuggestedOrderQuantity ?? 0), t.BinNumber);
                                        }
                                        else if ((t.OnHandQuantity ?? 0) < t.MinimumQuantity)
                                        {
                                            SendMailForSuggestedOrder(t.ItemNumber, t.OnHandQuantity ?? 0, t.CriticalQuantity ?? 0, t.MinimumQuantity ?? 0, t.Room ?? 0, t.CompanyID ?? 0, "Minimum", t.RoomName, UserId, "SuggestedOrdersMinimum", EnterpriseId, (t.SuggestedOrderQuantity ?? 0), t.BinNumber);
                                        }
                                    }
                                    if (HttpContext.Current != null)
                                    {
                                        //  GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.All.UpdateRedCircleCountInClients();
                                    }
                                }
                            }
                        });

                    }
                    else
                    {
                        // Do nothing
                    }

                    //GenerateIMMOrder(objSuggestedOrderInfo, UserId, objEnterpriseDTO);
                    //ItemMaster objItemMasterNew = context.ItemMasters.FirstOrDefault(t => t.GUID == ItemGUID);
                    //if ((objItemMasterNew.SuggestedOrderQuantity ?? 0) != (objItemMasterNew.SuggestedOrderQuantity ?? 0))
                    //{
                    //    SendMailForSuggestedOrder(objItemMasterNew.ItemNumber,objItemMasterNew.OnHandQuantity??0,objItemMasterNew.CriticalQuantity??0
                    //}

                    //if (HttpContext.Current != null)
                    //{
                    //    GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.All.UpdateRedCircleCountInClients();
                    //}
                }
            }
            if (HttpContext.Current != null && objSuggestedOrderInfo != null)
            {

                HttpRuntime.Cache["LastCalledEach"] = DateTime.UtcNow;
                if (RedCircleStatic.SignalGroups == null)
                {
                    RedCircleStatic.SignalGroups = new List<ECR>();
                }
                if (!RedCircleStatic.SignalGroups.Any(t => t.EID == EnterpriseId && t.CID == objSuggestedOrderInfo.CompanyID && t.RID == objSuggestedOrderInfo.RoomID && t.IsProcessed == false))
                {
                    RedCircleStatic.SignalGroups.Add(new ECR() { EID = EnterpriseId, CID = objSuggestedOrderInfo.CompanyID, RID = objSuggestedOrderInfo.RoomID, IsProcessed = false });
                }


                //System.Web.HttpContext.Current.Application["LastCalledEach"] = DateTime.UtcNow;

                //if (System.Web.HttpContext.Current.Application["LastCalledRedCircle"] == null)
                //{
                //    GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.All.UpdateRedCircleCountInClients();
                //    HttpContext.Current.Application["LastCalledRedCircle"] = DateTime.UtcNow;
                //}
                //else
                //{
                //    DateTime Lastcalled = Convert.ToDateTime(HttpContext.Current.Application["LastCalledRedCircle"]);
                //    if ((DateTime.UtcNow - Lastcalled).TotalSeconds > 60)
                //    {
                //        GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.All.UpdateRedCircleCountInClients();
                //        HttpContext.Current.Application["LastCalledRedCircle"] = DateTime.UtcNow;
                //    }
                //}
            }
        }
        private string GetCallStackData(string Provided, string generated)
        {
            string returndata = (Provided ?? string.Empty) + "[[[]]]" + (generated ?? string.Empty);
            if (returndata.Length > 490)
            {
                returndata = returndata.Substring(0, 490);
            }
            return returndata;
        }
        public void GenerateIMMOrder(SuggestedOrderInfo objSuggestedOrderInfo, long UserID, EnterpriseDTO objEnterpriseDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemMaster objitem = context.ItemMasters.FirstOrDefault(t => t.GUID == objSuggestedOrderInfo.ItemGUID);
                if (objSuggestedOrderInfo.CartQuantity > 0)
                {
                    SchedulerDTO objSchedulerDTO = new SchedulerDTO();
                    SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(base.DataBaseName);
                    objSchedulerDTO = objSupplierMasterDAL.GetRoomSchedule(objSuggestedOrderInfo.SupplierID, objSuggestedOrderInfo.RoomID, 2);
                    if (objSchedulerDTO != null)
                    {
                        if (objSchedulerDTO.ScheduleMode == 5)
                        {
                            IList<OrderMasterDTO> lstOrders = GetOrdersByCartIds(null, objSuggestedOrderInfo.RoomId, objSuggestedOrderInfo.CompanyId, UserID, base.DataBaseName, objSuggestedOrderInfo.SupplierID, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
                            Dictionary<string, string> rejectedOrderItems = new Dictionary<string, string>();
                            List<OrderMasterDTO> OrdersCreated = CreateOrdersByCart(lstOrders.ToList(), objSuggestedOrderInfo.RoomID, objSuggestedOrderInfo.CompanyID, UserID, base.DataBaseName, objSchedulerDTO.SubmissionMethod, objEnterpriseDTO.ID, out rejectedOrderItems);
                            if (objSchedulerDTO.SubmissionMethod == 2)
                            {
                                SupplierMasterDTO objSupplierDTO = new SupplierMasterDAL(base.DataBaseName).GetRecord(objSuggestedOrderInfo.SupplierID, objSuggestedOrderInfo.RoomID, objSuggestedOrderInfo.CompanyID, false);
                                // CompanyConfigDTO objCompanyConfigDTO = new CompanyConfigDAL(objEnterpriseDTO.EnterpriseDBName).GetRecord(objRoomScheduleDetailDTO.CompanyId);
                                eTurnsRegionInfo objCompanyConfigDTO = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(objSuggestedOrderInfo.RoomID, objSuggestedOrderInfo.CompanyId, 0);


                                foreach (OrderMasterDTO OrderItemCreated in OrdersCreated)
                                {
                                    SendMailToSupplier(objSupplierDTO, OrderItemCreated, objSuggestedOrderInfo.RoomName, objSuggestedOrderInfo.CompanyName, string.Empty, objEnterpriseDTO.ID, objCompanyConfigDTO);
                                }
                            }
                        }
                    }
                }
            }
        }
        public double GetSuggestedOrderQty(Guid ItemGUIDId, string ReplenishType, long BinID)
        {
            double TotalSuggestedQty = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (context.CartItems.Any(t => t.ItemGUID == ItemGUIDId && t.BinId == BinID && t.IsDeleted == false && t.IsArchived == false && t.ReplenishType == ReplenishType && t.IsAutoMatedEntry == false))
                {
                    TotalSuggestedQty = context.CartItems.Where(t => t.ItemGUID == ItemGUIDId && t.ReplenishType == ReplenishType && t.IsDeleted == false && t.IsArchived == false && t.IsAutoMatedEntry == false).Sum(t => (t.Quantity ?? 0));
                }
                else
                {
                    TotalSuggestedQty = 0;
                }
            }
            return TotalSuggestedQty;

        }
        public void DeleteCartByItemGUID(Guid[] ItemGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IQueryable<CartItem> cartitems = context.CartItems.Where(t => ItemGUID.Contains(t.ItemGUID ?? Guid.Empty));
                if (cartitems.Any())
                {
                    foreach (var item in cartitems)
                    {
                        item.IsDeleted = true;
                    }
                    context.SaveChanges();
                }
            }
        }
        public List<CartItemDTO> GetCartListForDailyEmail(long RoomId, long CompanyId, string DBConnectionstring)
        {
            List<CartItemDTO> lstCartItems = new List<CartItemDTO>();

            using (var context = new eTurnsEntities(DBConnectionstring))
            {
                lstCartItems = (from ci in context.CartItems
                                join itm in context.ItemMasters on ci.ItemGUID equals itm.GUID
                                join cc in context.UserMasters on ci.CreatedBy equals cc.ID into ci_cc_join
                                from ci_cc in ci_cc_join.DefaultIfEmpty()
                                join cu in context.UserMasters on ci.LastUpdatedBy equals cu.ID into ci_cu_join
                                from ci_cu in ci_cu_join.DefaultIfEmpty()
                                join rm in context.Rooms on ci.Room equals rm.ID into ci_rm_join
                                from ci_rm in ci_rm_join.DefaultIfEmpty()
                                where ci.CompanyID == CompanyId && ci.Room == RoomId && (ci.IsDeleted ?? false) == false && (ci.IsArchived ?? false) == false && ci.ReplenishType == "Purchase"
                                select new CartItemDTO
                                {
                                    ID = ci.ID,
                                    ItemNumber = itm.ItemNumber,
                                    ItemGUID = itm.GUID,
                                    Quantity = ci.Quantity,
                                    Status = ci.Status,
                                    ReplenishType = ci.ReplenishType,
                                    IsKitComponent = ci.IsKitComponent,
                                    UDF1 = ci.UDF1,
                                    UDF2 = ci.UDF2,
                                    UDF3 = ci.UDF3,
                                    UDF4 = ci.UDF4,
                                    UDF5 = ci.UDF5,
                                    GUID = ci.GUID,
                                    Created = ci.Created,
                                    Updated = ci.Updated,
                                    CreatedBy = ci.CreatedBy,
                                    LastUpdatedBy = ci.LastUpdatedBy,
                                    IsDeleted = ci.IsDeleted,
                                    IsArchived = ci.IsArchived,
                                    CompanyID = ci.CompanyID,
                                    Room = ci.Room,
                                    CreatedByName = ci_cc.UserName,
                                    UpdatedByName = ci_cu.UserName,
                                    RoomName = ci_rm.RoomName,
                                    IsAutoMatedEntry = ci.IsAutoMatedEntry,
                                    SupplierId = itm.SupplierID ?? 0,
                                    CriticalQuantity = itm.CriticalQuantity,
                                    MinimumQuantity = itm.MinimumQuantity,
                                    OnHandQuantity = itm.OnHandQuantity ?? 0,
                                    AddedFrom = ci.AddedFrom,
                                    EditedFrom = ci.EditedFrom,
                                    ReceivedOn = ci.ReceivedOn,
                                    ReceivedOnWeb = ci.ReceivedOnWeb,
                                }).OrderByDescending(t => t.ID).ToList();


            }
            return lstCartItems;
        }
        public IEnumerable<CartItemDTO> GetCartItemsByItemGUIDANDBINID(Int64 RoomID, Int64 CompanyId, Guid ItemGUID, Int64 BinId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.Database.SqlQuery<CartItemDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS 'UpdatedByName', D.RoomName,E.ItemNumber FROM CartItem A inner join ItemMaster as E on  A.ItemGUID = E.GUID LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.ItemGUID = '" + ItemGUID.ToString() + "' AND A.BinId = " + BinId.ToString() + "  AND A.Room =  " + RoomID.ToString() + " AND A.CompanyID = " + CompanyId.ToString())
                        select new CartItemDTO
                        {
                            ID = u.ID,
                            ItemNumber = u.ItemNumber,
                            ItemGUID = u.ItemGUID,
                            Quantity = u.Quantity,
                            Status = u.Status,
                            ReplenishType = u.ReplenishType,
                            IsKitComponent = u.IsKitComponent,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            GUID = u.GUID,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            Room = u.Room,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                        }).AsParallel().ToList();
            }
        }
        public double GetItemSuggestedOrderQtyForService(Guid ItemGUIDId, string ConnectionString)
        {
            double TotalSuggestedQty = 0;
            using (var context = new eTurnsEntities(ConnectionString))
            {
                if (context.CartItems.Any(t => t.ItemGUID == ItemGUIDId && t.IsDeleted == false && t.IsArchived == false && t.ReplenishType == "Purchase"))
                {
                    TotalSuggestedQty = context.CartItems.Where(t => t.ItemGUID == ItemGUIDId && t.ReplenishType == "Purchase" && t.IsDeleted == false && t.IsArchived == false).Sum(t => (t.Quantity ?? 0));
                }
                else
                {
                    TotalSuggestedQty = 0;
                }
            }
            return TotalSuggestedQty;

        }

        public double GetSuggestedOrderQtyForBinForService(Guid ItemGUIDId, long BinID, string ConnectionString)
        {

            if (ItemGUIDId == Guid.Parse("A8E415A8-B4A1-4955-8482-D49C0CF57999"))
            {

            }


            double TotalSuggestedQty = 0;
            using (var context = new eTurnsEntities(ConnectionString))
            {
                if (context.CartItems.Any(t => t.ItemGUID == ItemGUIDId && t.BinId == BinID && t.IsDeleted == false && t.IsArchived == false && t.ReplenishType == "Purchase"))
                {
                    TotalSuggestedQty = context.CartItems.Where(t => t.ItemGUID == ItemGUIDId && t.BinId == BinID && t.ReplenishType == "Purchase" && t.IsDeleted == false && t.IsArchived == false).Sum(t => (t.Quantity ?? 0));
                }
                else
                {
                    TotalSuggestedQty = 0;
                }
            }
            return TotalSuggestedQty;

        }

        public double GetSuggestedOrderQtyForService(Guid ItemGUIDId, string ReplenishType, string ConnectionString)
        {
            double TotalSuggestedQty = 0;
            using (var context = new eTurnsEntities(ConnectionString))
            {
                if (context.CartItems.Any(t => t.ItemGUID == ItemGUIDId && t.IsDeleted == false && t.IsArchived == false && t.ReplenishType == ReplenishType && t.IsAutoMatedEntry == false))
                {
                    TotalSuggestedQty = context.CartItems.Where(t => t.ItemGUID == ItemGUIDId && t.ReplenishType == ReplenishType && t.IsDeleted == false && t.IsArchived == false && t.IsAutoMatedEntry == false).Sum(t => (t.Quantity ?? 0));
                }
                else
                {
                    TotalSuggestedQty = 0;
                }
            }
            return TotalSuggestedQty;

        }
        //public long SaveCartItemForService(CartItemDTO objDTO, string ConnectionString)
        //{
        //    CartItem objCartItem = new CartItem();
        //    using (eTurnsEntities context = new eTurnsEntities(ConnectionString))
        //    {
        //        if (objDTO.ID > 0)
        //        {
        //            objCartItem = context.CartItems.FirstOrDefault(t => t.ID == objDTO.ID);
        //            objDTO.CompanyID = objCartItem.CompanyID;
        //            objDTO.Created = objCartItem.Created;
        //            objDTO.CreatedBy = objCartItem.CreatedBy;
        //            objDTO.CreatedByName = string.Empty;
        //            objDTO.GUID = objCartItem.GUID;
        //            objDTO.ID = objCartItem.ID;
        //            objDTO.IsArchived = objCartItem.IsArchived;
        //            objDTO.IsDeleted = objCartItem.IsDeleted;
        //            objDTO.IsKitComponent = objCartItem.IsKitComponent;
        //            objDTO.ItemGUID = objCartItem.ItemGUID;
        //            objDTO.Room = objCartItem.Room;

        //            objCartItem.Updated = objDTO.Updated;
        //            objCartItem.LastUpdatedBy = objDTO.LastUpdatedBy;
        //            objCartItem.Quantity = objDTO.Quantity;
        //            objCartItem.ReplenishType = objDTO.ReplenishType;
        //            objCartItem.Status = "U";
        //            objCartItem.UDF1 = objDTO.UDF1;
        //            objCartItem.UDF2 = objDTO.UDF2;
        //            objCartItem.UDF3 = objDTO.UDF3;
        //            objCartItem.UDF4 = objDTO.UDF4;
        //            objCartItem.UDF5 = objDTO.UDF5;
        //            objCartItem.BinId = objDTO.BinId;
        //            objCartItem.BinGUID = objDTO.BinGUID;
        //            objCartItem.AddedFrom = "Web";
        //            objCartItem.EditedFrom = "Web";
        //            objCartItem.ReceivedOn = DateTimeUtility.DateTimeNow;
        //            objCartItem.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
        //            objCartItem.IsAutoMatedEntry = objDTO.IsAutoMatedEntry;
        //            context.SaveChanges();
        //            IEnumerable<CartItemDTO> ObjCache = CacheHelper<IEnumerable<CartItemDTO>>.GetCacheItem("Cached_CartItem_" + objDTO.CompanyID.ToString());
        //            if (ObjCache != null)
        //            {
        //                List<CartItemDTO> objTemp = ObjCache.ToList();
        //                objTemp.RemoveAll(i => i.ID == objDTO.ID);
        //                ObjCache = objTemp.AsEnumerable();

        //                List<CartItemDTO> tempC = new List<CartItemDTO>();
        //                tempC.Add(objDTO);
        //                IEnumerable<CartItemDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
        //                CacheHelper<IEnumerable<CartItemDTO>>.AppendToCacheItem("Cached_CartItem_" + objDTO.CompanyID.ToString(), NewCache);
        //            }

        //        }
        //        else
        //        {
        //            objDTO.IsArchived = false;
        //            objDTO.IsDeleted = false;

        //            objCartItem.Created = objDTO.Created;
        //            objCartItem.CreatedBy = objDTO.CreatedBy;
        //            objCartItem.Updated = objDTO.Updated;
        //            objCartItem.LastUpdatedBy = objDTO.LastUpdatedBy;

        //            objCartItem.CompanyID = objDTO.CompanyID;
        //            objCartItem.GUID = Guid.NewGuid(); //objDTO.GUID;
        //            objCartItem.ItemGUID = objDTO.ItemGUID;
        //            objCartItem.Quantity = objDTO.Quantity;
        //            objCartItem.ReplenishType = objDTO.ReplenishType;
        //            objCartItem.Room = objDTO.Room;
        //            objCartItem.Status = objDTO.Status;
        //            objCartItem.UDF1 = objDTO.UDF1;
        //            objCartItem.UDF2 = objDTO.UDF2;
        //            objCartItem.UDF3 = objDTO.UDF3;
        //            objCartItem.UDF4 = objDTO.UDF4;
        //            objCartItem.UDF5 = objDTO.UDF5;
        //            objCartItem.IsDeleted = objDTO.IsDeleted;
        //            objCartItem.IsArchived = objDTO.IsArchived;
        //            objCartItem.IsKitComponent = objCartItem.IsKitComponent;
        //            objCartItem.BinId = objDTO.BinId;
        //            objCartItem.BinGUID = objDTO.BinGUID;
        //            objCartItem.IsAutoMatedEntry = objDTO.IsAutoMatedEntry;
        //            objCartItem.AddedFrom = "Web";
        //            objCartItem.EditedFrom = "Web";
        //            objCartItem.ReceivedOn = DateTimeUtility.DateTimeNow;
        //            objCartItem.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
        //            context.CartItems.Add(objCartItem);
        //            context.SaveChanges();
        //            objDTO.ID = objCartItem.ID;
        //            if (objDTO.ID > 0)
        //            {
        //                //Get Cached-Media
        //                IEnumerable<CartItemDTO> ObjCache = CacheHelper<IEnumerable<CartItemDTO>>.GetCacheItem("Cached_CartItem_" + objDTO.CompanyID.ToString());
        //                if (ObjCache != null)
        //                {
        //                    List<CartItemDTO> tempC = new List<CartItemDTO>();
        //                    tempC.Add(objDTO);

        //                    IEnumerable<CartItemDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
        //                    CacheHelper<IEnumerable<CartItemDTO>>.AppendToCacheItem("Cached_CartItem_" + objDTO.CompanyID.ToString(), NewCache);
        //                }
        //            }

        //        }

        //    }
        //    if (objDTO.IsAutoMatedEntry == false)
        //    {
        //        UpdateSuggestedQtyOfItemForService(objDTO.ItemGUID ?? Guid.NewGuid(), objDTO.Room ?? 0, objDTO.CompanyID ?? 0, objDTO.LastUpdatedBy ?? 0, ConnectionString);
        //    }
        //    return objCartItem.ID;
        //}

        //public bool UpdateSuggestedQtyOfItemForService(Guid ItemGUIDId, long RoomId, long CompanyId, long UserId, string ConnectionString)
        //{
        //    double TotalSuggestedQty = 0;
        //    using (var context = new eTurnsEntities(ConnectionString))
        //    {
        //        if (context.CartItems.Any(t => t.ItemGUID == ItemGUIDId && t.IsDeleted == false && t.IsArchived == false && t.ReplenishType == "Purchase"))
        //        {
        //            TotalSuggestedQty = context.CartItems.Where(t => t.ItemGUID == ItemGUIDId && t.ReplenishType == "Purchase" && t.IsDeleted == false && t.IsArchived == false).Sum(t => (t.Quantity ?? 0));
        //        }
        //        else
        //        {
        //            TotalSuggestedQty = 0;
        //        }
        //    }

        //    double TotalSuggestedTransferQty = 0;
        //    using (var context = new eTurnsEntities(ConnectionString))
        //    {
        //        if (context.CartItems.Any(t => t.ItemGUID == ItemGUIDId && t.IsDeleted == false && t.IsArchived == false && t.ReplenishType == "Transfer"))
        //        {
        //            TotalSuggestedTransferQty = context.CartItems.Where(t => t.ItemGUID == ItemGUIDId && t.ReplenishType == "Transfer" && t.IsDeleted == false && t.IsArchived == false).Sum(t => (t.Quantity ?? 0));
        //        }
        //        else
        //        {
        //            TotalSuggestedTransferQty = 0;
        //        }
        //    }

        //    ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(ConnectionString);
        //    ItemMasterDTO objItem = objItemMasterDAL.GetItemWithoutJoins(null, ItemGUIDId);
        //    if (objItem != null)
        //    {
        //        objItem.LastUpdatedBy = UserId;
        //        objItem.Updated = DateTimeUtility.DateTimeNow;
        //        objItem.SuggestedOrderQuantity = TotalSuggestedQty;
        //        objItem.SuggestedTransferQuantity = TotalSuggestedTransferQty;
        //        objItemMasterDAL.EditForService(objItem, ConnectionString);
        //    }
        //    return true;

        //}
        //public void AutoCartUpdateByCodeForService(Guid ItemGUID, long UserId, string ConnectionString)
        //{
        //    Guid EmptyGUID = Guid.Empty;
        //    SuggestedOrderInfo objSuggestedOrderInfo = new SuggestedOrderInfo();
        //    using (var context = new eTurnsEntities(ConnectionString))
        //    {
        //        objSuggestedOrderInfo = (from im in context.ItemMasters
        //                                 join rm in context.Rooms on im.Room equals rm.ID
        //                                 where im.GUID == ItemGUID
        //                                 select new SuggestedOrderInfo
        //                                 {
        //                                     AvailableQuantity = 0,
        //                                     CartQuantity = 0,
        //                                     CompanyId = im.CompanyID ?? 0,
        //                                     DefaultReorderQuantity = im.DefaultReorderQuantity,
        //                                     devideval = 0,
        //                                     IsEnforceDefaultReorderQuantity = im.IsEnforceDefaultReorderQuantity ?? false,
        //                                     IsItemLevelMinMaxQtyRequired = im.IsItemLevelMinMaxQtyRequired,
        //                                     ItemCriticalQuantity = im.CriticalQuantity,
        //                                     ItemIsPurchase = im.IsPurchase ?? false,
        //                                     ItemIsTranser = im.IsTransfer ?? false,
        //                                     ItemMaximumQuantity = im.MaximumQuantity,
        //                                     ItemMinimumQuantity = im.MinimumQuantity,
        //                                     ItemOnHandQuantity = im.OnHandQuantity ?? 0,
        //                                     ItemOnTransferInQuantity = im.OnTransferQuantity ?? 0,
        //                                     ItemOnTransferOutQuantity = im.InTransitquantity ?? 0,
        //                                     ItemOrderedQuantity = im.OnOrderQuantity ?? 0,
        //                                     ItemRequisitionQuantity = im.RequisitionedQuantity ?? 0,
        //                                     ItemSuggestedOrderQuantity = im.SuggestedOrderQuantity ?? 0,
        //                                     moduloval = 0,
        //                                     ReplenishType = "Purchase",
        //                                     RoomId = im.Room ?? 0,
        //                                     SuggestedOrderRoomFlag = rm.SuggestedOrder,
        //                                     SuggestedTransferRoomFlag = rm.SuggestedTransfer,
        //                                     TotalCartQtyForItem = 0,
        //                                     BinId = im.DefaultLocation ?? 0
        //                                 }).FirstOrDefault();
        //        if (objSuggestedOrderInfo != null)
        //        {
        //            if (objSuggestedOrderInfo.ItemIsPurchase && objSuggestedOrderInfo.ItemIsTranser)
        //            {
        //                objSuggestedOrderInfo.ReplenishType = "Purchase";
        //            }
        //            else if (objSuggestedOrderInfo.ItemIsTranser)
        //            {
        //                objSuggestedOrderInfo.ReplenishType = "Transfer";
        //            }
        //            else if (objSuggestedOrderInfo.ItemIsPurchase)
        //            {
        //                objSuggestedOrderInfo.ReplenishType = "Purchase";
        //            }
        //            else
        //            {
        //                objSuggestedOrderInfo.ReplenishType = null;
        //            }
        //            if (objSuggestedOrderInfo.IsItemLevelMinMaxQtyRequired != null && objSuggestedOrderInfo.IsItemLevelMinMaxQtyRequired == true)
        //            {
        //                objSuggestedOrderInfo.TotalCartQtyForItem = GetSuggestedOrderQtyForService(ItemGUID, objSuggestedOrderInfo.ReplenishType, ConnectionString);
        //                objSuggestedOrderInfo.AvailableQuantity = (objSuggestedOrderInfo.ItemOnHandQuantity + objSuggestedOrderInfo.ItemOrderedQuantity + objSuggestedOrderInfo.ItemOnTransferInQuantity + objSuggestedOrderInfo.TotalCartQtyForItem) - (objSuggestedOrderInfo.ItemRequisitionQuantity + objSuggestedOrderInfo.ItemOnTransferOutQuantity);

        //                if (objSuggestedOrderInfo.AvailableQuantity < objSuggestedOrderInfo.ItemMinimumQuantity && objSuggestedOrderInfo.AvailableQuantity < 0)
        //                {
        //                    objSuggestedOrderInfo.CartQuantity = objSuggestedOrderInfo.ItemMaximumQuantity - objSuggestedOrderInfo.AvailableQuantity;
        //                }
        //                else if (objSuggestedOrderInfo.AvailableQuantity < objSuggestedOrderInfo.ItemMinimumQuantity && objSuggestedOrderInfo.AvailableQuantity >= 0)
        //                {
        //                    objSuggestedOrderInfo.AvailableQuantity = objSuggestedOrderInfo.AvailableQuantity + objSuggestedOrderInfo.ItemRequisitionQuantity;
        //                    objSuggestedOrderInfo.CartQuantity = objSuggestedOrderInfo.ItemMaximumQuantity - objSuggestedOrderInfo.AvailableQuantity;
        //                }
        //                else
        //                {
        //                    CartItem objCartItem = context.CartItems.FirstOrDefault(t => t.ItemGUID == ItemGUID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false && t.IsAutoMatedEntry == true && t.ReplenishType == objSuggestedOrderInfo.ReplenishType && t.Room == objSuggestedOrderInfo.RoomId && t.CompanyID == objSuggestedOrderInfo.CompanyId);
        //                    if (objCartItem != null)
        //                    {
        //                        objCartItem.IsDeleted = true;
        //                        objCartItem.LastUpdatedBy = UserId;
        //                        objCartItem.Updated = DateTimeUtility.DateTimeNow;
        //                        context.SaveChanges();
        //                        ItemMaster objItemMaster = context.ItemMasters.FirstOrDefault(t => t.GUID == ItemGUID);
        //                        if (objItemMaster != null)
        //                        {
        //                            objItemMaster.SuggestedOrderQuantity = GetItemSuggestedOrderQtyForService(ItemGUID, ConnectionString);
        //                            objItemMaster.WhatWhereAction = "Cart";
        //                            context.SaveChanges();
        //                        }
        //                    }
        //                }
        //                if (objSuggestedOrderInfo.CartQuantity > 0)
        //                {
        //                    if (objSuggestedOrderInfo.IsEnforceDefaultReorderQuantity && objSuggestedOrderInfo.DefaultReorderQuantity > 0)
        //                    {
        //                        objSuggestedOrderInfo.moduloval = Convert.ToInt64(objSuggestedOrderInfo.CartQuantity) % Convert.ToInt64(objSuggestedOrderInfo.DefaultReorderQuantity);
        //                        if (objSuggestedOrderInfo.moduloval != 0)
        //                        {
        //                            objSuggestedOrderInfo.devideval = (Convert.ToInt64(objSuggestedOrderInfo.CartQuantity) / Convert.ToInt64(objSuggestedOrderInfo.DefaultReorderQuantity)) + 1;
        //                            objSuggestedOrderInfo.CartQuantity = objSuggestedOrderInfo.DefaultReorderQuantity * objSuggestedOrderInfo.devideval;
        //                        }
        //                    }
        //                    //else if (objSuggestedOrderInfo.CartQuantity >= objSuggestedOrderInfo.ItemMaximumQuantity)
        //                    //{
        //                    //    objSuggestedOrderInfo.CartQuantity = objSuggestedOrderInfo.ItemMaximumQuantity;
        //                    //}

        //                    if ((objSuggestedOrderInfo.ReplenishType == "Purchase" && objSuggestedOrderInfo.SuggestedOrderRoomFlag == true) || (objSuggestedOrderInfo.ReplenishType == "Transfer" && objSuggestedOrderInfo.SuggestedTransferRoomFlag == true))
        //                    {
        //                        CartItem objCartItem = context.CartItems.FirstOrDefault(t => t.ItemGUID == ItemGUID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false && t.IsAutoMatedEntry == true && t.ReplenishType == objSuggestedOrderInfo.ReplenishType && t.Room == objSuggestedOrderInfo.RoomId && t.CompanyID == objSuggestedOrderInfo.CompanyId);
        //                        if (objCartItem != null)
        //                        {
        //                            objCartItem.ReplenishType = objSuggestedOrderInfo.ReplenishType;
        //                            objCartItem.LastUpdatedBy = UserId;
        //                            objCartItem.Updated = DateTimeUtility.DateTimeNow;
        //                            objCartItem.Quantity = objSuggestedOrderInfo.CartQuantity;
        //                            context.SaveChanges();
        //                            ItemMaster objItemMaster = context.ItemMasters.FirstOrDefault(t => t.GUID == ItemGUID);
        //                            if (objItemMaster != null)
        //                            {
        //                                objItemMaster.SuggestedOrderQuantity = GetItemSuggestedOrderQtyForService(ItemGUID, ConnectionString);
        //                                objItemMaster.WhatWhereAction = "Cart";
        //                                context.SaveChanges();
        //                            }
        //                        }
        //                        else
        //                        {
        //                            CartItemDTO objCartItemDTO = new CartItemDTO();
        //                            objCartItemDTO.BinGUID = null;
        //                            objCartItemDTO.BinId = objSuggestedOrderInfo.BinId;
        //                            objCartItemDTO.BinName = null;
        //                            objCartItemDTO.CompanyID = objSuggestedOrderInfo.CompanyId;
        //                            objCartItemDTO.Created = DateTimeUtility.DateTimeNow;
        //                            objCartItemDTO.CreatedBy = UserId;
        //                            objCartItemDTO.CreatedByName = null;
        //                            objCartItemDTO.GUID = Guid.NewGuid();
        //                            objCartItemDTO.ID = 0;
        //                            objCartItemDTO.IsArchived = false;
        //                            objCartItemDTO.IsAutoMatedEntry = true;
        //                            objCartItemDTO.IsDeleted = false;
        //                            objCartItemDTO.IsKitComponent = false;
        //                            objCartItemDTO.IsPurchase = true;
        //                            objCartItemDTO.IsTransfer = false;
        //                            objCartItemDTO.ItemGUID = ItemGUID;
        //                            objCartItemDTO.ItemNumber = null;
        //                            objCartItemDTO.LastUpdatedBy = UserId;
        //                            objCartItemDTO.Quantity = objSuggestedOrderInfo.CartQuantity;
        //                            objCartItemDTO.ReplenishType = objSuggestedOrderInfo.ReplenishType;
        //                            objCartItemDTO.Room = objSuggestedOrderInfo.RoomId;
        //                            objCartItemDTO.RoomName = null;
        //                            objCartItemDTO.Status = "A";
        //                            objCartItemDTO.SupplierId = 0;
        //                            objCartItemDTO.SupplierName = null;
        //                            objCartItemDTO.UDF1 = "";
        //                            objCartItemDTO.UDF2 = "";
        //                            objCartItemDTO.UDF3 = "";
        //                            objCartItemDTO.UDF4 = "";
        //                            objCartItemDTO.UDF5 = "";
        //                            objCartItemDTO.Updated = DateTimeUtility.DateTimeNow;
        //                            objCartItemDTO.UpdatedByName = null;
        //                            SaveCartItemForService(objCartItemDTO, ConnectionString);
        //                            ItemMaster objItemMaster = context.ItemMasters.FirstOrDefault(t => t.GUID == ItemGUID);
        //                            if (objItemMaster != null)
        //                            {
        //                                objItemMaster.SuggestedOrderQuantity = GetItemSuggestedOrderQtyForService(ItemGUID, ConnectionString);
        //                                objItemMaster.WhatWhereAction = "Cart";
        //                                context.SaveChanges();
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    CartItem objCartItem = context.CartItems.FirstOrDefault(ci => ci.ItemGUID == ItemGUID && (ci.IsDeleted ?? false) == false && (ci.IsArchived ?? false) == false && ci.IsAutoMatedEntry == true && ci.ReplenishType == objSuggestedOrderInfo.ReplenishType && ci.Room == objSuggestedOrderInfo.RoomId && ci.CompanyID == objSuggestedOrderInfo.CompanyId);
        //                    if (objCartItem != null)
        //                    {
        //                        objCartItem.IsDeleted = true;
        //                        objCartItem.LastUpdatedBy = UserId;
        //                        objCartItem.Updated = DateTimeUtility.DateTimeNow;
        //                        context.SaveChanges();
        //                        ItemMaster objItemMaster = context.ItemMasters.FirstOrDefault(im => im.GUID == ItemGUID);
        //                        if (objItemMaster != null)
        //                        {
        //                            objItemMaster.SuggestedOrderQuantity = GetItemSuggestedOrderQtyForService(ItemGUID, ConnectionString);
        //                            objItemMaster.WhatWhereAction = "Cart";
        //                            context.SaveChanges();
        //                        }
        //                    }
        //                }
        //            }
        //            else if (objSuggestedOrderInfo.IsItemLevelMinMaxQtyRequired != null && objSuggestedOrderInfo.IsItemLevelMinMaxQtyRequired == false)
        //            {
        //                //List<BinMaster> lstItemlocations = new List<BinMaster>();
        //                //lstItemlocations = context.BinMasters.Where(t => t.IsDeleted == false && (t.IsArchived ?? false) == false && t.ItemGUID == ItemGUID).ToList();
        //                List<BinMasterDTO> lstItemlocations1 = GetItemLocations(ItemGUID);



        //                lstItemlocations1.ForEach(t =>
        //                {
        //                    double ItemLocationMinQuantity = t.MinimumQuantity ?? 0;
        //                    double ItemLocationMaxQuantity = t.MaximumQuantity ?? 0;
        //                    double ItemLocationCriticalQuantity = t.CriticalQuantity ?? 0;
        //                    double ItemLocationCustomerOwnedQuantity = 0;
        //                    double ItemLocationConsignedQuantity = 0;
        //                    double ItemLocationOnHandQuantity = 0;
        //                    if (context.ItemLocationDetails.Any(il => (il.IsDeleted ?? false) == false && (il.IsArchived ?? false) == false && (il.ItemGUID ?? Guid.Empty) == ItemGUID && (il.BinID ?? 0) == t.ID))
        //                    {
        //                        ItemLocationCustomerOwnedQuantity = context.ItemLocationDetails.Where(il => (il.IsDeleted ?? false) == false && (il.IsArchived ?? false) == false && (il.ItemGUID ?? Guid.Empty) == ItemGUID && (il.BinID ?? 0) == t.ID).Sum(il => (il.CustomerOwnedQuantity ?? 0));
        //                        ItemLocationConsignedQuantity = context.ItemLocationDetails.Where(il => (il.IsDeleted ?? false) == false && (il.IsArchived ?? false) == false && (il.ItemGUID ?? Guid.Empty) == ItemGUID && (il.BinID ?? 0) == t.ID).Sum(il => (il.ConsignedQuantity ?? 0));
        //                        ItemLocationOnHandQuantity = ItemLocationConsignedQuantity + ItemLocationCustomerOwnedQuantity;
        //                    }
        //                    double ItemLocationOnOrderedreqQuantity = 0;
        //                    double ItemLocationOnOrderedrecQuantity = 0;
        //                    double ItemLocationOnOrderedQuantity = 0;
        //                    if (context.OrderDetails.Any(il => (il.IsDeleted ?? false) == false && (il.IsArchived ?? false) == false && (il.ItemGUID ?? Guid.Empty) == ItemGUID && (il.Bin ?? 0) == t.ID))
        //                    {
        //                        ItemLocationOnOrderedreqQuantity = context.OrderDetails.Where(il => (il.IsDeleted ?? false) == false && (il.IsArchived ?? false) == false && (il.ItemGUID ?? Guid.Empty) == ItemGUID && (il.Bin ?? 0) == t.ID).Sum(il => (il.RequestedQuantity ?? 0));
        //                        ItemLocationOnOrderedrecQuantity = context.OrderDetails.Where(il => (il.IsDeleted ?? false) == false && (il.IsArchived ?? false) == false && (il.ItemGUID ?? Guid.Empty) == ItemGUID && (il.Bin ?? 0) == t.ID).Sum(il => (il.ReceivedQuantity ?? 0));
        //                        ItemLocationOnOrderedQuantity = ItemLocationOnOrderedreqQuantity - ItemLocationOnOrderedrecQuantity;
        //                        if (ItemLocationOnOrderedQuantity < 0)
        //                        {
        //                            ItemLocationOnOrderedQuantity = 0;
        //                        }
        //                    }
        //                    double ItemLocationCartQuantity = 0;

        //                    if (context.CartItems.Any(ci => (ci.IsDeleted ?? false) == false && (ci.IsArchived ?? false) == false && (ci.ItemGUID ?? Guid.Empty) == ItemGUID && (ci.BinId ?? 0) == t.ID && ci.ReplenishType == objSuggestedOrderInfo.ReplenishType && ci.IsAutoMatedEntry == false))
        //                    {
        //                        ItemLocationCartQuantity = context.CartItems.Where(ci => (ci.IsDeleted ?? false) == false && (ci.IsArchived ?? false) == false && (ci.ItemGUID ?? Guid.Empty) == ItemGUID && (ci.BinId ?? 0) == t.ID && ci.ReplenishType == objSuggestedOrderInfo.ReplenishType && ci.IsAutoMatedEntry == false).Sum(ci => (ci.Quantity ?? 0));
        //                    }
        //                    double ItemLocationAvailableQuantity = ItemLocationOnHandQuantity + ItemLocationCartQuantity + ItemLocationOnOrderedQuantity;
        //                    double ItemLocationSuggestedCartQuantity = 0;
        //                    if (ItemLocationAvailableQuantity < ItemLocationMinQuantity)
        //                    {
        //                        ItemLocationSuggestedCartQuantity = ItemLocationMaxQuantity - ItemLocationAvailableQuantity;
        //                    }
        //                    else
        //                    {
        //                        CartItem objCartItem = context.CartItems.FirstOrDefault(ci => ci.ItemGUID == ItemGUID && (ci.IsDeleted ?? false) == false && (ci.IsArchived ?? false) == false && ci.IsAutoMatedEntry == true && ci.ReplenishType == objSuggestedOrderInfo.ReplenishType && ci.Room == objSuggestedOrderInfo.RoomId && ci.CompanyID == objSuggestedOrderInfo.CompanyId && (ci.BinId ?? 0) == t.ID);
        //                        if (objCartItem != null)
        //                        {
        //                            objCartItem.IsDeleted = true;
        //                            objCartItem.LastUpdatedBy = UserId;
        //                            objCartItem.Updated = DateTimeUtility.DateTimeNow;
        //                            context.SaveChanges();
        //                            //ItemMaster objItemMaster = context.ItemMasters.FirstOrDefault(im => im.GUID == ItemGUID);
        //                            //if (objItemMaster != null)
        //                            //{
        //                            //    objItemMaster.SuggestedOrderQuantity = GetItemSuggestedOrderQty(ItemGUID);
        //                            //    context.SaveChanges();
        //                            //}
        //                        }
        //                    }
        //                    if (ItemLocationSuggestedCartQuantity > 0)
        //                    {
        //                        if (objSuggestedOrderInfo.IsEnforceDefaultReorderQuantity && objSuggestedOrderInfo.DefaultReorderQuantity > 0)
        //                        {
        //                            objSuggestedOrderInfo.moduloval = Convert.ToInt64(ItemLocationSuggestedCartQuantity) % Convert.ToInt64(objSuggestedOrderInfo.DefaultReorderQuantity);
        //                            if (objSuggestedOrderInfo.moduloval != 0)
        //                            {
        //                                objSuggestedOrderInfo.devideval = (Convert.ToInt64(ItemLocationSuggestedCartQuantity) / Convert.ToInt64(objSuggestedOrderInfo.DefaultReorderQuantity)) + 1;
        //                                ItemLocationSuggestedCartQuantity = objSuggestedOrderInfo.DefaultReorderQuantity * objSuggestedOrderInfo.devideval;
        //                            }
        //                        }
        //                        else if (ItemLocationSuggestedCartQuantity >= ItemLocationMaxQuantity)
        //                        {
        //                            ItemLocationSuggestedCartQuantity = ItemLocationMaxQuantity;
        //                        }

        //                        if ((objSuggestedOrderInfo.ReplenishType == "Purchase" && objSuggestedOrderInfo.SuggestedOrderRoomFlag == true) || (objSuggestedOrderInfo.ReplenishType == "Transfer" && objSuggestedOrderInfo.SuggestedTransferRoomFlag == true))
        //                        {
        //                            CartItem objCartItem = context.CartItems.FirstOrDefault(ci => ci.ItemGUID == ItemGUID && (ci.IsDeleted ?? false) == false && (ci.IsArchived ?? false) == false && ci.IsAutoMatedEntry == true && ci.ReplenishType == objSuggestedOrderInfo.ReplenishType && ci.Room == objSuggestedOrderInfo.RoomId && ci.CompanyID == objSuggestedOrderInfo.CompanyId && (ci.BinId ?? 0) == t.ID);
        //                            if (objCartItem != null)
        //                            {
        //                                objCartItem.ReplenishType = objSuggestedOrderInfo.ReplenishType;
        //                                objCartItem.LastUpdatedBy = UserId;
        //                                objCartItem.Updated = DateTimeUtility.DateTimeNow;
        //                                objCartItem.Quantity = ItemLocationSuggestedCartQuantity;
        //                                context.SaveChanges();
        //                                ItemMaster objItemMaster = context.ItemMasters.FirstOrDefault(im => im.GUID == ItemGUID);
        //                                if (objItemMaster != null)
        //                                {
        //                                    objItemMaster.SuggestedOrderQuantity = GetItemSuggestedOrderQtyForService(ItemGUID, ConnectionString);
        //                                    objItemMaster.WhatWhereAction = "Cart";
        //                                    context.SaveChanges();
        //                                }
        //                            }
        //                            else
        //                            {
        //                                CartItemDTO objCartItemDTO = new CartItemDTO();
        //                                objCartItemDTO.BinGUID = t.GUID;
        //                                objCartItemDTO.BinId = t.ID;
        //                                objCartItemDTO.BinName = null;
        //                                objCartItemDTO.CompanyID = objSuggestedOrderInfo.CompanyId;
        //                                objCartItemDTO.Created = DateTimeUtility.DateTimeNow;
        //                                objCartItemDTO.CreatedBy = UserId;
        //                                objCartItemDTO.CreatedByName = null;
        //                                objCartItemDTO.GUID = Guid.NewGuid();
        //                                objCartItemDTO.ID = 0;
        //                                objCartItemDTO.IsArchived = false;
        //                                objCartItemDTO.IsAutoMatedEntry = true;
        //                                objCartItemDTO.IsDeleted = false;
        //                                objCartItemDTO.IsKitComponent = false;
        //                                objCartItemDTO.IsPurchase = true;
        //                                objCartItemDTO.IsTransfer = false;
        //                                objCartItemDTO.ItemGUID = ItemGUID;
        //                                objCartItemDTO.ItemNumber = null;
        //                                objCartItemDTO.LastUpdatedBy = UserId;
        //                                objCartItemDTO.Quantity = ItemLocationSuggestedCartQuantity;
        //                                objCartItemDTO.ReplenishType = objSuggestedOrderInfo.ReplenishType;
        //                                objCartItemDTO.Room = objSuggestedOrderInfo.RoomId;
        //                                objCartItemDTO.RoomName = null;
        //                                objCartItemDTO.Status = "A";
        //                                objCartItemDTO.SupplierId = 0;
        //                                objCartItemDTO.SupplierName = null;
        //                                objCartItemDTO.UDF1 = "";
        //                                objCartItemDTO.UDF2 = "";
        //                                objCartItemDTO.UDF3 = "";
        //                                objCartItemDTO.UDF4 = "";
        //                                objCartItemDTO.UDF5 = "";
        //                                objCartItemDTO.Updated = DateTimeUtility.DateTimeNow;
        //                                objCartItemDTO.UpdatedByName = null;
        //                                SaveCartItem(objCartItemDTO);
        //                                //ItemMaster objItemMaster = context.ItemMasters.FirstOrDefault(im => im.GUID == ItemGUID);
        //                                //if (objItemMaster != null)
        //                                //{
        //                                //    objItemMaster.SuggestedOrderQuantity = GetItemSuggestedOrderQty(ItemGUID);
        //                                //}
        //                                //context.SaveChanges();
        //                            }
        //                        }
        //                    }
        //                });
        //                DeleteAutoCartItemsForService(ItemGUID, ConnectionString);


        //            }
        //            else
        //            {
        //                // Do nothing
        //            }

        //        }
        //    }


        //}

        private void DeleteAutoCartItemsForService(Guid ItemGUID, string ConnectionString)
        {
            using (var context = new eTurnsEntities(ConnectionString))
            {
                List<BinMaster> lstItemlocations = context.BinMasters.Where(t => t.IsDeleted == false && (t.IsArchived ?? false) == false && t.ItemGUID == ItemGUID).ToList();
                long[] binIds = lstItemlocations.Select(t => t.ID).ToArray();
                var carts = context.CartItems.Where(t => (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false && t.IsAutoMatedEntry == true && t.ItemGUID == ItemGUID && t.BinId != null);
                foreach (var item in carts)
                {
                    if (!binIds.Contains(item.BinId ?? 0))
                    {
                        item.IsDeleted = true;

                    }
                }
                context.SaveChanges();
                ItemMaster objItemMaster = context.ItemMasters.FirstOrDefault(im => im.GUID == ItemGUID);
                if (objItemMaster != null)
                {
                    objItemMaster.SuggestedOrderQuantity = GetItemSuggestedOrderQtyForService(ItemGUID, ConnectionString);
                    objItemMaster.WhatWhereAction = "Cart";
                }
                context.SaveChanges();
            }
        }
        //public bool CreateOrderTransferFromCartCode(int ActiononCartItems, List<Guid> SelectedCartIds, long RoomId, long CompanyId, long CreatedBy)
        //{

        //    List<CartItemDTO> lstcartItems = new List<CartItemDTO>();
        //    OrderMasterDAL objOrderMasterDAL = new OrderMasterDAL(base.DataBaseName);
        //    OrderDetailsDAL objOrderDetailsDAL = new OrderDetailsDAL(base.DataBaseName);
        //    TransferMasterDAL objTransferMasterDAL = new TransferMasterDAL(base.DataBaseName);
        //    TransferDetailDAL objTransferDetailDAL = new TransferDetailDAL(base.DataBaseName);
        //    eTurnsEntities context = null;
        //    string DbConnectionString = string.Empty;
        //    if (!string.IsNullOrWhiteSpace(DbConnectionString))
        //    {
        //        context = new eTurnsEntities(DbConnectionString);
        //    }
        //    else
        //    {
        //        context = new eTurnsEntities(base.DataBaseEntityConnectionString);
        //    }

        //    using (context)
        //    {
        //        if (SelectedCartIds.Count > 0)
        //        {
        //            lstcartItems = GetCartItemsByGuids(SelectedCartIds, RoomId, CompanyId, true, null);
        //            //lstcartItems = context.CartItems.Where(t => SelectedCartIds.Contains(t.GUID)).ToList();
        //        }
        //        else
        //        {
        //            lstcartItems = GetCartItemsByGuids(SelectedCartIds, RoomId, CompanyId, false, null);
        //            //lstcartItems = context.CartItems.Where(t => (t.Room ?? 0) == RoomId && (t.CompanyID ?? 0) == CompanyId && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).ToList();
        //        }
        //        switch (ActiononCartItems)
        //        {
        //            case 1:
        //                if (lstcartItems.Any(t => t.ReplenishType == "Purchase"))
        //                {
        //                    List<CartItemDTO> lstSupplierBPOs = (from ci in lstcartItems
        //                                                         where ci.ReplenishType == "Purchase"
        //                                                         group ci by new { ci.SupplierId, ci.BlanketPOID, ci.SupplierName, ci.BlanketPONumber } into ciGrouped
        //                                                         select new CartItemDTO
        //                                                         {
        //                                                             SupplierId = ciGrouped.Key.SupplierId,
        //                                                             BlanketPOID = ciGrouped.Key.BlanketPOID,
        //                                                             SupplierName = ciGrouped.Key.SupplierName,
        //                                                             BlanketPONumber = ciGrouped.Key.BlanketPONumber
        //                                                         }).ToList();

        //                    foreach (var item in lstSupplierBPOs)
        //                    {
        //                        List<CartItemDTO> OrderLineItems = lstcartItems.Where(t => t.SupplierId == item.SupplierId && t.BlanketPOID == item.BlanketPOID).ToList();
        //                        string Ordernumber = string.Empty;
        //                        if ((item.BlanketPOID ?? 0) > 0)
        //                        {
        //                            Ordernumber = item.BlanketPONumber;
        //                        }
        //                        else
        //                        {
        //                            //Ordernumber = "#O" + new AutoSequenceDAL(base.DataBaseName).GetSuggestedOrderNumberFormat(item.SupplierId, RoomId, CompanyId);
        //                            Ordernumber = new AutoSequenceDAL(base.DataBaseName).GetSuggestedOrderNumberFormat(item.SupplierId, RoomId, CompanyId);
        //                        }

        //                        OrderMasterDTO objOrderMasterDTO = new OrderMasterDTO();
        //                        int ReleaseNo = 1;
        //                        if (!string.IsNullOrEmpty(Ordernumber))
        //                        {

        //                            OrderMasterDAL objOrderDAL = new OrderMasterDAL(base.DataBaseName);
        //                            IEnumerable<OrderMasterDTO> objOrderList = objOrderDAL.GetAllRecords(RoomId, CompanyId, false, false, OrderType.Order);
        //                            ReleaseNo = objOrderList.Where(x => x.OrderNumber == Ordernumber).Count() + 1;
        //                        }
        //                        OrderMasterDTO objDTO = new OrderMasterDTO()
        //                        {
        //                            RequiredDate = DateTime.Now.AddDays(7),
        //                            OrderNumber = Ordernumber,
        //                            ReleaseNumber = ReleaseNo.ToString(),
        //                            OrderStatus = (int)OrderStatus.UnSubmitted,
        //                            LastUpdated = DateTime.Now,
        //                            Created = DateTime.Now,
        //                            CreatedBy = CreatedBy,
        //                            CreatedByName = string.Empty,
        //                            LastUpdatedBy = CreatedBy,
        //                            CompanyID = CompanyId,
        //                            Room = RoomId,
        //                            RoomName = string.Empty,
        //                            UpdatedByName = string.Empty,
        //                            GUID = Guid.NewGuid(),
        //                            IsDeleted = false,
        //                            IsArchived = false,
        //                            Supplier = item.SupplierId,
        //                            OrderDate = DateTime.Now,
        //                            WhatWhereAction = "Cart",
        //                        };
        //                        objDTO = objOrderMasterDAL.InsertOrder(objDTO);
        //                        foreach (var politem in OrderLineItems)
        //                        {
        //                            OrderDetailsDTO objOrderDetailsDTO = new OrderDetailsDTO();
        //                            objOrderDetailsDTO.Action = "insert";
        //                            objOrderDetailsDTO.ASNNumber = string.Empty;
        //                            objOrderDetailsDTO.Bin = politem.BinId;
        //                            objOrderDetailsDTO.BinName = null;
        //                            objOrderDetailsDTO.CompanyID = CompanyId;
        //                            objOrderDetailsDTO.Created = DateTimeUtility.DateTimeNow;
        //                            objOrderDetailsDTO.CreatedBy = CreatedBy;
        //                            objOrderDetailsDTO.CreatedByName = string.Empty;
        //                            objOrderDetailsDTO.GUID = Guid.NewGuid();
        //                            objOrderDetailsDTO.HistoryID = 0;
        //                            objOrderDetailsDTO.IsArchived = false;
        //                            objOrderDetailsDTO.IsDeleted = false;
        //                            objOrderDetailsDTO.IsHistory = false;
        //                            objOrderDetailsDTO.IsEDISent = false;
        //                            objOrderDetailsDTO.ItemGUID = politem.ItemGUID;
        //                            objOrderDetailsDTO.LastUpdated = DateTimeUtility.DateTimeNow;
        //                            objOrderDetailsDTO.LastUpdatedBy = CreatedBy;
        //                            objOrderDetailsDTO.OrderGUID = objDTO.GUID;
        //                            objOrderDetailsDTO.ReceivedQuantity = 0;
        //                            objOrderDetailsDTO.RequestedQuantity = politem.Quantity;
        //                            objOrderDetailsDTO.RequiredDate = objDTO.RequiredDate;
        //                            objOrderDetailsDTO.Room = RoomId;
        //                            objOrderDetailsDTO.RoomName = string.Empty;
        //                            objOrderDetailsDTO.UpdatedByName = string.Empty;
        //                            objOrderDetailsDTO.ID = 0;
        //                            objOrderDetailsDAL.Insert(objOrderDetailsDTO);
        //                            lstcartItems.Where(t => t.ReplenishType == "Purchase" && t.SupplierId == item.SupplierId && t.BlanketPOID == item.BlanketPOID).ToList().ForEach(t =>
        //                            {
        //                                CartItem objcartItem = context.CartItems.FirstOrDefault(local => local.GUID == t.GUID);
        //                                if (objcartItem != null)
        //                                {
        //                                    objcartItem.IsOrderTransferMade = true;
        //                                    objcartItem.IsDeleted = true;
        //                                    objcartItem.Updated = DateTimeUtility.DateTimeNow;
        //                                    objcartItem.LastUpdatedBy = CreatedBy;
        //                                    objcartItem.RefOrderTransferGUID = objDTO.GUID;
        //                                    objcartItem.RefOrderTransferId = objDTO.ID;
        //                                }
        //                            });
        //                            context.SaveChanges();
        //                        }

        //                    }
        //                }
        //                if (lstcartItems.Any(t => t.ReplenishType == "Transfer"))
        //                {
        //                    //string autoNumber = new AutoSequenceDAL(base.DataBaseName).GetLastGeneratedROOMID("NextTransferNo", RoomId, CompanyId).ToString();
        //                    string autoNumber = new AutoSequenceDAL(base.DataBaseName).GetNextAutoNumberByModule("NextTransferNo", RoomId, CompanyId);
        //                    TransferMasterDTO objDTO = new TransferMasterDTO()
        //                    {
        //                        RequireDate = DateTime.Now.AddDays(7),
        //                        //TransferNumber = "#Z" + autoNumber,
        //                        TransferNumber = autoNumber,
        //                        TransferStatus = (int)TransferStatus.UnSubmitted,
        //                        RequestType = (int)RequestType.In,
        //                        RequestingRoomID = RoomId,
        //                        Created = DateTime.Now,
        //                        Updated = DateTime.Now,
        //                        CreatedBy = CreatedBy,
        //                        CreatedByName = string.Empty,
        //                        LastUpdatedBy = CreatedBy,
        //                        RoomID = RoomId,
        //                        CompanyID = CompanyId,
        //                        RoomName = string.Empty,
        //                        UpdatedByName = string.Empty
        //                    };
        //                    objDTO = objTransferMasterDAL.InsertTransfer(objDTO);
        //                    List<CartItem> lstTransfers = (from ci in lstcartItems
        //                                                   where ci.ReplenishType == "Transfer"
        //                                                   group ci by new { ci.ItemGUID } into ciGrouped
        //                                                   select new CartItem
        //                                                   {
        //                                                       ItemGUID = ciGrouped.Key.ItemGUID ?? Guid.Empty,
        //                                                       Quantity = ciGrouped.Sum(t => (t.Quantity ?? 0))
        //                                                   }).ToList();
        //                    foreach (var politem in lstTransfers)
        //                    {
        //                        TransferDetailDTO objTransferDetailDTO = new TransferDetailDTO();
        //                        objTransferDetailDTO.ItemGUID = politem.ItemGUID ?? Guid.Empty;
        //                        objTransferDetailDTO.Action = "insert";
        //                        objTransferDetailDTO.Bin = null;
        //                        objTransferDetailDTO.BinName = null;
        //                        objTransferDetailDTO.CompanyID = CompanyId;
        //                        objTransferDetailDTO.Created = DateTimeUtility.DateTimeNow;
        //                        objTransferDetailDTO.CreatedBy = CreatedBy;
        //                        objTransferDetailDTO.CreatedByName = string.Empty;
        //                        objTransferDetailDTO.GUID = Guid.NewGuid();
        //                        objTransferDetailDTO.HistoryID = 0;
        //                        objTransferDetailDTO.IsArchived = false;
        //                        objTransferDetailDTO.IsDeleted = false;
        //                        objTransferDetailDTO.IsHistory = false;
        //                        objTransferDetailDTO.ItemGUID = politem.ItemGUID ?? Guid.Empty;
        //                        objTransferDetailDTO.LastUpdated = DateTimeUtility.DateTimeNow;
        //                        objTransferDetailDTO.LastUpdatedBy = CreatedBy;
        //                        objTransferDetailDTO.TransferGUID = objDTO.GUID;
        //                        objTransferDetailDTO.ReceivedQuantity = 0;
        //                        objTransferDetailDTO.RequestedQuantity = politem.Quantity ?? 0;
        //                        objTransferDetailDTO.RequiredDate = objDTO.RequireDate;
        //                        objTransferDetailDTO.Room = RoomId;
        //                        objTransferDetailDTO.RoomName = string.Empty;
        //                        objTransferDetailDTO.UpdatedByName = string.Empty;
        //                        objTransferDetailDTO.ID = 0;
        //                        objTransferDetailDAL.Insert(objTransferDetailDTO);

        //                    }
        //                    lstcartItems.Where(t => t.ReplenishType == "Transfer").ToList().ForEach(t =>
        //                    {
        //                        CartItem objcartItem = context.CartItems.FirstOrDefault(local => local.GUID == t.GUID);
        //                        if (objcartItem != null)
        //                        {
        //                            objcartItem.IsOrderTransferMade = true;
        //                            objcartItem.IsDeleted = true;
        //                            objcartItem.Updated = DateTimeUtility.DateTimeNow;
        //                            objcartItem.LastUpdatedBy = CreatedBy;
        //                            objcartItem.RefOrderTransferGUID = objDTO.GUID;
        //                            objcartItem.RefOrderTransferId = objDTO.ID;
        //                        }
        //                    });
        //                    context.SaveChanges();
        //                }
        //                break;
        //            case 2:
        //                //if (lstcartItems.Any(t => t.ReplenishType == "Purchase" && t.IsAutoMatedEntry == false))
        //                //{
        //                //    string nextNo = new AutoSequenceDAL(base.DataBaseName).GetLastGeneratedROOMID("NextOrderNo", RoomId, CompanyId).ToString();
        //                //    OrderMasterDTO objOrderMasterDTO = new OrderMasterDTO();
        //                //    OrderMasterDTO objDTO = new OrderMasterDTO()
        //                //    {
        //                //        RequiredDate = DateTime.Now.AddDays(7),
        //                //        OrderNumber = "#O" + nextNo,
        //                //        OrderStatus = (int)OrderStatus.UnSubmitted,
        //                //        LastUpdated = DateTime.Now,
        //                //        Created = DateTime.Now,
        //                //        CreatedBy = CreatedBy,
        //                //        CreatedByName = string.Empty,
        //                //        LastUpdatedBy = CreatedBy,
        //                //        CompanyID = CompanyId,
        //                //        Room = RoomId,
        //                //        RoomName = string.Empty,
        //                //        UpdatedByName = string.Empty,
        //                //        GUID = Guid.NewGuid(),
        //                //        IsDeleted = false,
        //                //        IsArchived = false,
        //                //        Supplier = new SupplierMasterDAL(base.DataBaseName).GetAllRecords(RoomId, CompanyId, false, false).FirstOrDefault().ID
        //                //    };
        //                //    objDTO = objOrderMasterDAL.InsertOrder(objDTO);
        //                //    List<CartItem> lstPurchases = (from ci in lstcartItems
        //                //                                   where ci.ReplenishType == "Purchase" && ci.IsAutoMatedEntry == false
        //                //                                   group ci by new { ci.ItemGUID } into ciGrouped
        //                //                                   select new CartItem
        //                //                                   {
        //                //                                       ItemGUID = ciGrouped.Key.ItemGUID ?? Guid.Empty,
        //                //                                       Quantity = ciGrouped.Sum(t => (t.Quantity ?? 0))
        //                //                                   }).ToList();
        //                //    foreach (var politem in lstPurchases)
        //                //    {
        //                //        OrderDetailsDTO objOrderDetailsDTO = new OrderDetailsDTO();
        //                //        objOrderDetailsDTO.Action = "insert";
        //                //        objOrderDetailsDTO.ASNNumber = string.Empty;
        //                //        objOrderDetailsDTO.Bin = null;
        //                //        objOrderDetailsDTO.BinName = null;
        //                //        objOrderDetailsDTO.CompanyID = CompanyId;
        //                //        objOrderDetailsDTO.Created = DateTimeUtility.DateTimeNow;
        //                //        objOrderDetailsDTO.CreatedBy = CreatedBy;
        //                //        objOrderDetailsDTO.CreatedByName = string.Empty;
        //                //        objOrderDetailsDTO.GUID = Guid.NewGuid();
        //                //        objOrderDetailsDTO.HistoryID = 0;
        //                //        objOrderDetailsDTO.IsArchived = false;
        //                //        objOrderDetailsDTO.IsDeleted = false;
        //                //        objOrderDetailsDTO.IsHistory = false;
        //                //        objOrderDetailsDTO.IsSendToEDI = false;
        //                //        objOrderDetailsDTO.ItemGUID = politem.ItemGUID;
        //                //        objOrderDetailsDTO.LastUpdated = DateTimeUtility.DateTimeNow;
        //                //        objOrderDetailsDTO.LastUpdatedBy = CreatedBy;
        //                //        objOrderDetailsDTO.OrderGUID = objDTO.GUID;
        //                //        objOrderDetailsDTO.ReceivedQuantity = 0;
        //                //        objOrderDetailsDTO.RequestedQuantity = politem.Quantity;
        //                //        objOrderDetailsDTO.RequiredDate = objDTO.RequiredDate;
        //                //        objOrderDetailsDTO.Room = RoomId;
        //                //        objOrderDetailsDTO.RoomName = string.Empty;
        //                //        objOrderDetailsDTO.UpdatedByName = string.Empty;
        //                //        objOrderDetailsDTO.ID = 0;
        //                //        objOrderDetailsDAL.Insert(objOrderDetailsDTO);

        //                //    }
        //                //    lstcartItems.Where(t => t.ReplenishType == "Purchase" && t.IsAutoMatedEntry == false).ToList().ForEach(t =>
        //                //    {
        //                //        CartItem objcartItem = context.CartItems.FirstOrDefault(local => local.GUID == t.GUID);
        //                //        if (objcartItem != null)
        //                //        {
        //                //            objcartItem.IsOrderTransferMade = true;
        //                //            objcartItem.IsDeleted = true;
        //                //            objcartItem.Updated = DateTimeUtility.DateTimeNow;
        //                //            objcartItem.LastUpdatedBy = CreatedBy;
        //                //            objcartItem.RefOrderTransferGUID = objDTO.GUID;
        //                //            objcartItem.RefOrderTransferId = objDTO.ID;
        //                //        }
        //                //    });
        //                //    context.SaveChanges();
        //                //}
        //                if (lstcartItems.Any(t => t.ReplenishType == "Purchase" && t.IsAutoMatedEntry == false))
        //                {
        //                    List<CartItemDTO> lstPurchases = (from ci in lstcartItems
        //                                                      where ci.ReplenishType == "Purchase" && ci.IsAutoMatedEntry == false
        //                                                      group ci by new { ci.SupplierId } into ciGrouped
        //                                                      select new CartItemDTO
        //                                                      {
        //                                                          SupplierId = ciGrouped.Key.SupplierId,
        //                                                          Quantity = ciGrouped.Sum(t => (t.Quantity ?? 0))
        //                                                      }).ToList();
        //                    if (lstPurchases != null && lstPurchases.Any())
        //                    {
        //                        foreach (var item in lstPurchases)
        //                        {
        //                            string nextNo = new AutoSequenceDAL(base.DataBaseName).GetLastGeneratedROOMID("NextOrderNo", RoomId, CompanyId).ToString();
        //                            OrderMasterDTO objOrderMasterDTO = new OrderMasterDTO();
        //                            OrderMasterDTO objDTO = new OrderMasterDTO()
        //                            {
        //                                RequiredDate = DateTime.Now.AddDays(7),
        //                                //OrderNumber = "#O" + nextNo,
        //                                OrderNumber = nextNo,
        //                                OrderStatus = (int)OrderStatus.UnSubmitted,
        //                                LastUpdated = DateTime.Now,
        //                                Created = DateTime.Now,
        //                                CreatedBy = CreatedBy,
        //                                CreatedByName = string.Empty,
        //                                LastUpdatedBy = CreatedBy,
        //                                CompanyID = CompanyId,
        //                                Room = RoomId,
        //                                RoomName = string.Empty,
        //                                UpdatedByName = string.Empty,
        //                                GUID = Guid.NewGuid(),
        //                                IsDeleted = false,
        //                                IsArchived = false,
        //                                Supplier = item.SupplierId,
        //                                OrderDate = DateTime.Now,
        //                                WhatWhereAction = "Cart",
        //                            };
        //                            objDTO = objOrderMasterDAL.InsertOrder(objDTO);
        //                            List<CartItemDTO> lstLocalPurchases = (from cil in lstcartItems
        //                                                                   where cil.SupplierId == item.SupplierId && cil.ReplenishType == "Purchase"
        //                                                                   group cil by new { cil.BinId, cil.ItemGUID } into grouped_cil
        //                                                                   select new CartItemDTO
        //                                                                   {
        //                                                                       BinId = grouped_cil.Key.BinId,
        //                                                                       ItemGUID = grouped_cil.Key.ItemGUID,
        //                                                                       Quantity = grouped_cil.Sum(t => t.Quantity ?? 0)
        //                                                                   }).ToList();
        //                            foreach (var politem in lstLocalPurchases)
        //                            {
        //                                OrderDetailsDTO objOrderDetailsDTO = new OrderDetailsDTO();
        //                                objOrderDetailsDTO.Action = "insert";
        //                                objOrderDetailsDTO.ASNNumber = string.Empty;
        //                                objOrderDetailsDTO.Bin = politem.BinId;
        //                                objOrderDetailsDTO.BinName = null;
        //                                objOrderDetailsDTO.CompanyID = CompanyId;
        //                                objOrderDetailsDTO.Created = DateTimeUtility.DateTimeNow;
        //                                objOrderDetailsDTO.CreatedBy = CreatedBy;
        //                                objOrderDetailsDTO.CreatedByName = string.Empty;
        //                                objOrderDetailsDTO.GUID = Guid.NewGuid();
        //                                objOrderDetailsDTO.HistoryID = 0;
        //                                objOrderDetailsDTO.IsArchived = false;
        //                                objOrderDetailsDTO.IsDeleted = false;
        //                                objOrderDetailsDTO.IsHistory = false;
        //                                objOrderDetailsDTO.IsEDISent = false;
        //                                objOrderDetailsDTO.ItemGUID = politem.ItemGUID;
        //                                objOrderDetailsDTO.LastUpdated = DateTimeUtility.DateTimeNow;
        //                                objOrderDetailsDTO.LastUpdatedBy = CreatedBy;
        //                                objOrderDetailsDTO.OrderGUID = objDTO.GUID;
        //                                objOrderDetailsDTO.ReceivedQuantity = 0;
        //                                objOrderDetailsDTO.RequestedQuantity = politem.Quantity;
        //                                objOrderDetailsDTO.RequiredDate = objDTO.RequiredDate;
        //                                objOrderDetailsDTO.Room = RoomId;
        //                                objOrderDetailsDTO.RoomName = string.Empty;
        //                                objOrderDetailsDTO.UpdatedByName = string.Empty;
        //                                objOrderDetailsDTO.ID = 0;
        //                                objOrderDetailsDAL.Insert(objOrderDetailsDTO);

        //                            }
        //                            lstcartItems.Where(t => t.ReplenishType == "Purchase" && t.SupplierId == item.SupplierId).ToList().ForEach(t =>
        //                            {
        //                                CartItem objcartItem = context.CartItems.FirstOrDefault(local => local.GUID == t.GUID);
        //                                if (objcartItem != null)
        //                                {
        //                                    objcartItem.IsOrderTransferMade = true;
        //                                    objcartItem.IsDeleted = true;
        //                                    objcartItem.Updated = DateTimeUtility.DateTimeNow;
        //                                    objcartItem.LastUpdatedBy = CreatedBy;
        //                                    objcartItem.RefOrderTransferGUID = objDTO.GUID;
        //                                    objcartItem.RefOrderTransferId = objDTO.ID;
        //                                }
        //                            });
        //                            context.SaveChanges();
        //                        }
        //                    }
        //                }
        //                if (lstcartItems.Any(t => t.ReplenishType == "Transfer" && t.IsAutoMatedEntry == false))
        //                {
        //                    //string autoNumber = new AutoSequenceDAL(base.DataBaseName).GetLastGeneratedROOMID("NextTransferNo", RoomId, CompanyId).ToString();
        //                    string autoNumber = new AutoSequenceDAL(base.DataBaseName).GetNextAutoNumberByModule("NextTransferNo", RoomId, CompanyId);
        //                    TransferMasterDTO objDTO = new TransferMasterDTO()
        //                    {
        //                        RequireDate = DateTime.Now.AddDays(7),
        //                        //TransferNumber = "#Z" + autoNumber,
        //                        TransferNumber = autoNumber,
        //                        TransferStatus = (int)TransferStatus.UnSubmitted,
        //                        RequestType = (int)RequestType.In,
        //                        RequestingRoomID = RoomId,
        //                        Created = DateTime.Now,
        //                        Updated = DateTime.Now,
        //                        CreatedBy = CreatedBy,
        //                        CreatedByName = string.Empty,
        //                        LastUpdatedBy = CreatedBy,
        //                        RoomID = RoomId,
        //                        CompanyID = CompanyId,
        //                        RoomName = string.Empty,
        //                        UpdatedByName = string.Empty
        //                    };
        //                    objDTO = objTransferMasterDAL.InsertTransfer(objDTO);
        //                    List<CartItem> lstTransfers = (from ci in lstcartItems
        //                                                   where ci.ReplenishType == "Transfer" && ci.IsAutoMatedEntry == false
        //                                                   group ci by new { ci.ItemGUID } into ciGrouped
        //                                                   select new CartItem
        //                                                   {
        //                                                       ItemGUID = ciGrouped.Key.ItemGUID ?? Guid.Empty,
        //                                                       Quantity = ciGrouped.Sum(t => (t.Quantity ?? 0))
        //                                                   }).ToList();
        //                    foreach (var politem in lstTransfers)
        //                    {
        //                        TransferDetailDTO objTransferDetailDTO = new TransferDetailDTO();
        //                        objTransferDetailDTO.Action = "insert";
        //                        objTransferDetailDTO.Bin = null;
        //                        objTransferDetailDTO.BinName = null;
        //                        objTransferDetailDTO.CompanyID = CompanyId;
        //                        objTransferDetailDTO.Created = DateTimeUtility.DateTimeNow;
        //                        objTransferDetailDTO.CreatedBy = CreatedBy;
        //                        objTransferDetailDTO.CreatedByName = string.Empty;
        //                        objTransferDetailDTO.GUID = Guid.NewGuid();
        //                        objTransferDetailDTO.HistoryID = 0;
        //                        objTransferDetailDTO.IsArchived = false;
        //                        objTransferDetailDTO.IsDeleted = false;
        //                        objTransferDetailDTO.IsHistory = false;
        //                        objTransferDetailDTO.ItemGUID = politem.ItemGUID ?? Guid.Empty;
        //                        objTransferDetailDTO.LastUpdated = DateTimeUtility.DateTimeNow;
        //                        objTransferDetailDTO.LastUpdatedBy = CreatedBy;
        //                        objTransferDetailDTO.TransferGUID = objDTO.GUID;
        //                        objTransferDetailDTO.ReceivedQuantity = 0;
        //                        objTransferDetailDTO.RequestedQuantity = politem.Quantity ?? 0;
        //                        objTransferDetailDTO.RequiredDate = objDTO.RequireDate;
        //                        objTransferDetailDTO.Room = RoomId;
        //                        objTransferDetailDTO.RoomName = string.Empty;
        //                        objTransferDetailDTO.UpdatedByName = string.Empty;
        //                        objTransferDetailDTO.ID = 0;
        //                        objTransferDetailDAL.Insert(objTransferDetailDTO);
        //                    }
        //                    lstcartItems.Where(t => t.ReplenishType == "Transfer" && t.IsAutoMatedEntry == false).ToList().ForEach(t =>
        //                    {
        //                        CartItem objcartItem = context.CartItems.FirstOrDefault(local => local.GUID == t.GUID);
        //                        if (objcartItem != null)
        //                        {
        //                            objcartItem.IsOrderTransferMade = true;
        //                            objcartItem.IsDeleted = true;
        //                            objcartItem.Updated = DateTimeUtility.DateTimeNow;
        //                            objcartItem.LastUpdatedBy = CreatedBy;
        //                            objcartItem.RefOrderTransferGUID = objDTO.GUID;
        //                            objcartItem.RefOrderTransferId = objDTO.ID;
        //                        }
        //                    });
        //                    context.SaveChanges();
        //                }
        //                break;
        //            case 3:
        //                if (lstcartItems.Any(t => t.ReplenishType == "Transfer"))
        //                {
        //                    //string autoNumber = new AutoSequenceDAL(base.DataBaseName).GetLastGeneratedROOMID("NextTransferNo", RoomId, CompanyId).ToString();
        //                    string autoNumber = new AutoSequenceDAL(base.DataBaseName).GetNextAutoNumberByModule("NextTransferNo", RoomId, CompanyId);
        //                    TransferMasterDTO objDTO = new TransferMasterDTO()
        //                    {
        //                        RequireDate = DateTime.Now.AddDays(7),
        //                        //TransferNumber = "#Z" + autoNumber,
        //                        TransferNumber = autoNumber,
        //                        TransferStatus = (int)TransferStatus.UnSubmitted,
        //                        RequestType = (int)RequestType.In,
        //                        RequestingRoomID = RoomId,
        //                        Created = DateTime.Now,
        //                        Updated = DateTime.Now,
        //                        CreatedBy = CreatedBy,
        //                        CreatedByName = string.Empty,
        //                        LastUpdatedBy = CreatedBy,
        //                        RoomID = RoomId,
        //                        CompanyID = CompanyId,
        //                        RoomName = string.Empty,
        //                        UpdatedByName = string.Empty
        //                    };
        //                    objDTO = objTransferMasterDAL.InsertTransfer(objDTO);
        //                    List<CartItem> lstTransfers = (from ci in lstcartItems
        //                                                   where ci.ReplenishType == "Transfer"
        //                                                   group ci by new { ci.ItemGUID } into ciGrouped
        //                                                   select new CartItem
        //                                                   {
        //                                                       ItemGUID = ciGrouped.Key.ItemGUID ?? Guid.Empty,
        //                                                       Quantity = ciGrouped.Sum(t => (t.Quantity ?? 0))
        //                                                   }).ToList();
        //                    foreach (var politem in lstTransfers)
        //                    {
        //                        TransferDetailDTO objTransferDetailDTO = new TransferDetailDTO();
        //                        objTransferDetailDTO.Action = "insert";
        //                        objTransferDetailDTO.Bin = null;
        //                        objTransferDetailDTO.BinName = null;
        //                        objTransferDetailDTO.CompanyID = CompanyId;
        //                        objTransferDetailDTO.Created = DateTimeUtility.DateTimeNow;
        //                        objTransferDetailDTO.CreatedBy = CreatedBy;
        //                        objTransferDetailDTO.CreatedByName = string.Empty;
        //                        objTransferDetailDTO.GUID = Guid.NewGuid();
        //                        objTransferDetailDTO.HistoryID = 0;
        //                        objTransferDetailDTO.IsArchived = false;
        //                        objTransferDetailDTO.IsDeleted = false;
        //                        objTransferDetailDTO.IsHistory = false;
        //                        objTransferDetailDTO.ItemGUID = politem.ItemGUID ?? Guid.Empty;
        //                        objTransferDetailDTO.LastUpdated = DateTimeUtility.DateTimeNow;
        //                        objTransferDetailDTO.LastUpdatedBy = CreatedBy;
        //                        objTransferDetailDTO.TransferGUID = objDTO.GUID;
        //                        objTransferDetailDTO.ReceivedQuantity = 0;
        //                        objTransferDetailDTO.RequestedQuantity = politem.Quantity ?? 0;
        //                        objTransferDetailDTO.RequiredDate = objDTO.RequireDate;
        //                        objTransferDetailDTO.Room = RoomId;
        //                        objTransferDetailDTO.RoomName = string.Empty;
        //                        objTransferDetailDTO.UpdatedByName = string.Empty;
        //                        objTransferDetailDTO.ID = 0;
        //                        objTransferDetailDAL.Insert(objTransferDetailDTO);

        //                    }
        //                    lstcartItems.Where(t => t.ReplenishType == "Transfer").ToList().ForEach(t =>
        //                    {
        //                        CartItem objcartItem = context.CartItems.FirstOrDefault(local => local.GUID == t.GUID);
        //                        if (objcartItem != null)
        //                        {
        //                            objcartItem.IsOrderTransferMade = true;
        //                            objcartItem.IsDeleted = true;
        //                            objcartItem.Updated = DateTimeUtility.DateTimeNow;
        //                            objcartItem.LastUpdatedBy = CreatedBy;
        //                            objcartItem.RefOrderTransferGUID = objDTO.GUID;
        //                            objcartItem.RefOrderTransferId = objDTO.ID;
        //                        }
        //                    });
        //                    context.SaveChanges();
        //                }
        //                break;
        //            case 4:
        //                if (lstcartItems.Any(t => t.ReplenishType == "Purchase"))
        //                {
        //                    List<CartItemDTO> lstSupplierBPOs = (from ci in lstcartItems
        //                                                         where ci.ReplenishType == "Purchase"
        //                                                         group ci by new { ci.SupplierId, ci.BlanketPOID, ci.SupplierName, ci.BlanketPONumber } into ciGrouped
        //                                                         select new CartItemDTO
        //                                                         {
        //                                                             SupplierId = ciGrouped.Key.SupplierId,
        //                                                             BlanketPOID = ciGrouped.Key.BlanketPOID,
        //                                                             SupplierName = ciGrouped.Key.SupplierName,
        //                                                             BlanketPONumber = ciGrouped.Key.BlanketPONumber
        //                                                         }).ToList();

        //                    foreach (var item in lstSupplierBPOs)
        //                    {
        //                        List<CartItemDTO> OrderLineItems = lstcartItems.Where(t => t.SupplierId == item.SupplierId && t.BlanketPOID == item.BlanketPOID).ToList();
        //                        string Ordernumber = string.Empty;
        //                        if ((item.BlanketPOID ?? 0) > 0)
        //                        {
        //                            Ordernumber = item.BlanketPONumber;
        //                        }
        //                        else
        //                        {
        //                            //Ordernumber = "#O" + new AutoSequenceDAL(base.DataBaseName).GetSuggestedOrderNumberFormat(item.SupplierId, RoomId, CompanyId);
        //                            Ordernumber = new AutoSequenceDAL(base.DataBaseName).GetSuggestedOrderNumberFormat(item.SupplierId, RoomId, CompanyId);
        //                        }

        //                        OrderMasterDTO objOrderMasterDTO = new OrderMasterDTO();
        //                        int ReleaseNo = 1;
        //                        if (!string.IsNullOrEmpty(Ordernumber))
        //                        {

        //                            OrderMasterDAL objOrderDAL = new OrderMasterDAL(base.DataBaseName);
        //                            IEnumerable<OrderMasterDTO> objOrderList = objOrderDAL.GetAllRecords(RoomId, CompanyId, false, false, OrderType.Order);
        //                            ReleaseNo = objOrderList.Where(x => x.OrderNumber == Ordernumber).Count() + 1;
        //                        }
        //                        OrderMasterDTO objDTO = new OrderMasterDTO()
        //                        {
        //                            RequiredDate = DateTime.Now.AddDays(7),
        //                            OrderNumber = Ordernumber,
        //                            OrderStatus = (int)OrderStatus.UnSubmitted,
        //                            ReleaseNumber = ReleaseNo.ToString(),
        //                            LastUpdated = DateTime.Now,
        //                            Created = DateTime.Now,
        //                            CreatedBy = CreatedBy,
        //                            CreatedByName = string.Empty,
        //                            LastUpdatedBy = CreatedBy,
        //                            CompanyID = CompanyId,
        //                            Room = RoomId,
        //                            RoomName = string.Empty,
        //                            UpdatedByName = string.Empty,
        //                            GUID = Guid.NewGuid(),
        //                            IsDeleted = false,
        //                            IsArchived = false,
        //                            Supplier = item.SupplierId,
        //                            OrderDate = DateTime.Now,
        //                            WhatWhereAction = "Cart",
        //                        };
        //                        objDTO = objOrderMasterDAL.InsertOrder(objDTO);
        //                        foreach (var politem in OrderLineItems)
        //                        {
        //                            OrderDetailsDTO objOrderDetailsDTO = new OrderDetailsDTO();
        //                            objOrderDetailsDTO.Action = "insert";
        //                            objOrderDetailsDTO.ASNNumber = string.Empty;
        //                            objOrderDetailsDTO.Bin = politem.BinId;
        //                            objOrderDetailsDTO.BinName = null;
        //                            objOrderDetailsDTO.CompanyID = CompanyId;
        //                            objOrderDetailsDTO.Created = DateTimeUtility.DateTimeNow;
        //                            objOrderDetailsDTO.CreatedBy = CreatedBy;
        //                            objOrderDetailsDTO.CreatedByName = string.Empty;
        //                            objOrderDetailsDTO.GUID = Guid.NewGuid();
        //                            objOrderDetailsDTO.HistoryID = 0;
        //                            objOrderDetailsDTO.IsArchived = false;
        //                            objOrderDetailsDTO.IsDeleted = false;
        //                            objOrderDetailsDTO.IsHistory = false;
        //                            objOrderDetailsDTO.IsEDISent = false;
        //                            objOrderDetailsDTO.ItemGUID = politem.ItemGUID;
        //                            objOrderDetailsDTO.LastUpdated = DateTimeUtility.DateTimeNow;
        //                            objOrderDetailsDTO.LastUpdatedBy = CreatedBy;
        //                            objOrderDetailsDTO.OrderGUID = objDTO.GUID;
        //                            objOrderDetailsDTO.ReceivedQuantity = 0;
        //                            objOrderDetailsDTO.RequestedQuantity = politem.Quantity;
        //                            objOrderDetailsDTO.RequiredDate = objDTO.RequiredDate;
        //                            objOrderDetailsDTO.Room = RoomId;
        //                            objOrderDetailsDTO.RoomName = string.Empty;
        //                            objOrderDetailsDTO.UpdatedByName = string.Empty;
        //                            objOrderDetailsDTO.ID = 0;
        //                            objOrderDetailsDAL.Insert(objOrderDetailsDTO);
        //                            lstcartItems.Where(t => t.ReplenishType == "Purchase" && t.SupplierId == item.SupplierId && t.BlanketPOID == item.BlanketPOID).ToList().ForEach(t =>
        //                            {
        //                                CartItem objcartItem = context.CartItems.FirstOrDefault(local => local.GUID == t.GUID);
        //                                if (objcartItem != null)
        //                                {
        //                                    objcartItem.IsOrderTransferMade = true;
        //                                    objcartItem.IsDeleted = true;
        //                                    objcartItem.Updated = DateTimeUtility.DateTimeNow;
        //                                    objcartItem.LastUpdatedBy = CreatedBy;
        //                                    objcartItem.RefOrderTransferGUID = objDTO.GUID;
        //                                    objcartItem.RefOrderTransferId = objDTO.ID;
        //                                }
        //                            });
        //                            context.SaveChanges();
        //                        }

        //                    }
        //                }
        //                break;
        //        }
        //    }

        //    return true;
        //}
        public IEnumerable<BinMasterDTO> DeleteLocationCount(Int64 RoomID, Int64 CompanyId, Guid ItemGUID, Int64 BinId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.Database.SqlQuery<BinMasterDTO>(@"SELECT ItemLocationQTY.CustomerOwnedQuantity, ItemLocationQTY.ConsignedQuantity, ItemLocationQTY.Quantity
                          FROM ItemMaster INNER JOIN
                          BinMaster ON ItemMaster.GUID = BinMaster.ItemGUID INNER JOIN
                          ItemLocationQTY ON BinMaster.ItemGUID = ItemLocationQTY.ItemGUID
						  WHERE ItemMaster.IsDeleted!=1 AND ItemMaster.IsArchived != 1 AND ItemMaster.GUID = '" + ItemGUID.ToString() + "' AND ItemLocationQTY.BinId = " + BinId.ToString() + "  AND ItemMaster.Room =  " + RoomID.ToString() + " AND ItemMaster.CompanyID = " + CompanyId.ToString())
                        select new BinMasterDTO
                        {
                            CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                            ConsignedQuantity = u.ConsignedQuantity,

                        }).AsParallel().ToList();
            }
        }
        public CartItemDTO GetRecordByGUID(Guid CartGUID, Int64 RoomID, Int64 CompanyID)
        {
            return GetCachedData(RoomID, CompanyID, false, false).Single(t => t.GUID == CartGUID);
        }
        public CartItemDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID, bool TakeFromDB)
        {
            CartItemDTO objCartItemDTO = new CartItemDTO();

            if (TakeFromDB)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    //  objCartItemDTO = (from ci in context.GetCartByID(id, null)
                    var params1 = new SqlParameter[] { new SqlParameter("@id", id), new SqlParameter("@GUID", Guid.Empty) };
                    objCartItemDTO = (from u in context.Database.SqlQuery<CartItemDTO>("exec [GetCartByID] @id,@GUID", params1) select u).FirstOrDefault();

                    //objCartItemDTO = (from ci in context.CartItems
                    //                  join itm in context.ItemMasters on ci.ItemGUID equals itm.GUID
                    //                  join itmSup in context.ItemSupplierDetails on new { itmguid = itm.GUID, supid = itm.SupplierID ?? 0, isdef = true, isa = false, isd = false } equals new { itmguid = itmSup.ItemGUID ?? Guid.Empty, supid = itmSup.SupplierID, isdef = itmSup.IsDefault ?? false, isa = itmSup.IsArchived ?? false, isd = itmSup.IsDeleted ?? false } into itm_itmSup_join
                    //                  from itm_itmSup in itm_itmSup_join.DefaultIfEmpty()
                    //                  join sm in context.SupplierMasters on itm.SupplierID equals sm.ID into itm_itmSup_sm_join
                    //                  from itm_itmSup_sm in itm_itmSup_sm_join.DefaultIfEmpty()
                    //                  join supblnk in context.SupplierBlanketPODetails on new { subbid = itm_itmSup.BlanketPOID ?? 0, isactv = true } equals new { subbid = supblnk.ID, isactv = (EntityFunctions.TruncateTime(supblnk.StartDate ?? DateTime.MinValue) <= EntityFunctions.TruncateTime(DateTime.Now)) && (EntityFunctions.TruncateTime(supblnk.Enddate ?? DateTime.MaxValue) >= EntityFunctions.TruncateTime(DateTime.Now)) } into itm_itmSup_supblnk_join
                    //                  from itm_itmSup_supblnk in itm_itmSup_supblnk_join.DefaultIfEmpty()
                    //                  join bm in context.BinMasters on ci.BinId equals bm.ID into ci_bm_join
                    //                  from ci_bm in ci_bm_join.DefaultIfEmpty()
                    //                  join catm in context.CategoryMasters on itm.CategoryID equals catm.ID into Itm_catm_join
                    //                  from Itm_catm in Itm_catm_join.DefaultIfEmpty()
                    //                  join manm in context.ManufacturerMasters on itm.ManufacturerID equals manm.ID into itm_manm_join
                    //                  from itm_manm in itm_manm_join.DefaultIfEmpty()
                    //                  join uom in context.UnitMasters on itm.UOMID equals uom.ID into itm_uom_join
                    //                  from itm_uom in itm_uom_join.DefaultIfEmpty()
                    //                  join cc in context.UserMasters on ci.CreatedBy equals cc.ID into ci_cc_join
                    //                  from ci_cc in ci_cc_join.DefaultIfEmpty()
                    //                  join cu in context.UserMasters on ci.LastUpdatedBy equals cu.ID into ci_cu_join
                    //                  from ci_cu in ci_cu_join.DefaultIfEmpty()
                    //                  join rm in context.Rooms on ci.Room equals rm.ID into ci_rm_join
                    //                  from ci_rm in ci_rm_join.DefaultIfEmpty()
                    //                  where ci.CompanyID == CompanyID && ci.Room == RoomID && (ci.IsDeleted ?? false) == false && (ci.IsArchived ?? false) == false
                    //                  select new CartItemDTO
                    //                  {
                    //                      BinGUID = ci.BinGUID,
                    //                      BinId = ci.BinId,
                    //                      BinName = ci_bm.BinNumber,
                    //                      BlanketPOID = itm_itmSup.BlanketPOID,
                    //                      BlanketPONumber = itm_itmSup_supblnk.BlanketPO,
                    //                      CategoryID = itm.CategoryID,
                    //                      CategoryName = Itm_catm.Category,
                    //                      CompanyID = ci.CompanyID,
                    //                      Cost = itm.Cost,
                    //                      Created = ci.Created,
                    //                      CreatedBy = ci.CreatedBy,
                    //                      CreatedByName = ci_cc.UserName,
                    //                      CriticalQuantity = itm.CriticalQuantity,
                    //                      DefaultReorderQuantity = itm.DefaultReorderQuantity,
                    //                      GUID = ci.GUID,
                    //                      ID = ci.ID,
                    //                      IsArchived = ci.IsArchived,
                    //                      IsAutoMatedEntry = ci.IsAutoMatedEntry,
                    //                      IsDeleted = ci.IsDeleted,
                    //                      IsItemLevelMinMaxQtyRequired = itm.IsItemLevelMinMaxQtyRequired,
                    //                      IsKitComponent = false,
                    //                      IsPurchase = itm.IsPurchase,
                    //                      IsTransfer = itm.IsTransfer,
                    //                      ItemGUID = ci.ItemGUID,
                    //                      ItemId = itm.ID,
                    //                      ItemLocationCriticalQuantity = 12,
                    //                      ItemLocationMaximumQuantity = 0,
                    //                      ItemLocationMinimumQuantity = 0,
                    //                      ItemNumber = itm.ItemNumber,
                    //                      ItemUDF1 = itm.UDF1,
                    //                      ItemUDF2 = itm.UDF2,
                    //                      ItemUDF3 = itm.UDF3,
                    //                      ItemUDF4 = itm.UDF4,
                    //                      ItemUDF5 = itm.UDF5,
                    //                      LastUpdatedBy = ci.LastUpdatedBy,
                    //                      ManufacturerID = itm.ManufacturerID,
                    //                      ManufacturerName = itm_manm.Manufacturer,
                    //                      ManufacturerNumber = itm.ManufacturerNumber,
                    //                      Markup = itm.Markup,
                    //                      MaximumQuantity = itm.MaximumQuantity,
                    //                      MaxOrderSize = itm_itmSup_sm.MaximumOrderSize,
                    //                      MinimumQuantity = itm.MinimumQuantity,
                    //                      OnHandQuantity = itm.OnHandQuantity ?? 0,
                    //                      OnOrderQuantity = itm.OnOrderQuantity,
                    //                      OnTransferQuantity = itm.OnTransferQuantity,
                    //                      PackingQuantity = itm.PackingQuantity,
                    //                      Quantity = ci.Quantity,
                    //                      ReplenishType = ci.ReplenishType,
                    //                      RequisitionedQuantity = itm.RequisitionedQuantity,
                    //                      Room = ci.Room,
                    //                      RoomName = ci_rm.RoomName,
                    //                      SellPrice = itm.SellPrice,
                    //                      SerialNumberTracking = itm.SerialNumberTracking,
                    //                      Status = ci.Status,
                    //                      SupplierId = itm.SupplierID ?? 0,
                    //                      SupplierName = itm_itmSup_sm.SupplierName,
                    //                      SupplierPartNo = itm.SupplierPartNo,
                    //                      UDF1 = ci.UDF1,
                    //                      UDF2 = ci.UDF2,
                    //                      UDF3 = ci.UDF3,
                    //                      UDF4 = ci.UDF4,
                    //                      UDF5 = ci.UDF5,
                    //                      UnitName = itm_uom.Unit,
                    //                      UOMID = itm.UOMID ?? 0,
                    //                      Updated = ci.Updated,
                    //                      UpdatedByName = ci_cu.UserName,
                    //                      WhatWhereAction = ci.WhatWhereAction
                    //                  }).FirstOrDefault();
                }
            }
            else
            {
                return GetCachedData(RoomID, CompanyID, false, false).Single(t => t.ID == id);
            }
            return objCartItemDTO;
        }
        public IEnumerable<CartItemDTO> GetPagedUniqueCartItems(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            //Get Cached-Media
            IEnumerable<CartItemDTO> ObjCache = GetCachedData(RoomID, CompanyID, IsDeleted, IsArchived);

            //IEnumerable<CartItemDTO> ObjGlobalCache = ObjCache;
            //ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false));

            //if (IsArchived && IsDeleted)
            //{
            //    ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived || t.IsDeleted == IsDeleted));
            //    ObjCache = ObjCache.Concat(ObjGlobalCache);
            //}
            //else if (IsArchived)
            //{
            //    ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived));
            //    ObjCache = ObjCache.Concat(ObjGlobalCache);
            //}
            //else if (IsDeleted)
            //{
            //    ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsDeleted == IsDeleted));
            //    ObjCache = ObjCache.Concat(ObjGlobalCache);
            //}
            IEnumerable<ItemMasterDTO> lstItems = new ItemMasterDAL(base.DataBaseName).GetAllRecords(RoomID, CompanyID);
            IEnumerable<SupplierMasterDTO> lstSuppliers = new SupplierMasterDAL(base.DataBaseName).GetAllRecords(RoomID, CompanyID, false, false, false);
            IEnumerable<UserMasterDTO> lstUsers = new UserMasterDAL(base.DataBaseName).GetAllRecords(RoomID, CompanyID);
            ObjCache = (from ci in ObjCache
                        join itm in lstItems on ci.ItemGUID equals itm.GUID
                        join sup in lstSuppliers on itm.SupplierID equals sup.ID into supitmjoin
                        from supitm in supitmjoin.DefaultIfEmpty()
                        join crtr in lstUsers on ci.CreatedBy equals crtr.ID into cicrtrjoin
                        from cicrtr in cicrtrjoin.DefaultIfEmpty()
                        join updtr in lstUsers on ci.LastUpdatedBy equals updtr.ID into ciupdtrjoin
                        from ciupdtr in ciupdtrjoin.DefaultIfEmpty()
                        select new CartItemDTO
                        {
                            CompanyID = ci.CompanyID,
                            Created = ci.Created,
                            CreatedBy = ci.CreatedBy,
                            CreatedByName = cicrtr != null ? cicrtr.UserName : string.Empty,
                            GUID = ci.GUID,
                            ID = ci.ID,
                            IsArchived = ci.IsArchived,
                            IsDeleted = ci.IsDeleted,
                            IsKitComponent = ci.IsKitComponent,
                            IsPurchase = ci.IsPurchase,
                            IsTransfer = ci.IsTransfer,
                            ItemGUID = ci.ItemGUID,
                            ItemNumber = itm.ItemNumber,
                            LastUpdatedBy = ci.LastUpdatedBy,
                            Quantity = ci.Quantity,
                            ReplenishType = ci.ReplenishType,
                            Room = ci.Room,
                            RoomName = ci.RoomName,
                            Status = ci.Status,
                            SupplierId = itm.SupplierID ?? 0,
                            SupplierName = supitm != null ? supitm.SupplierName : "",
                            UDF1 = ci.UDF1,
                            UDF2 = ci.UDF2,
                            UDF3 = ci.UDF3,
                            UDF4 = ci.UDF4,
                            UDF5 = ci.UDF5,
                            UpdatedByName = ciupdtr != null ? ciupdtr.UserName : string.Empty,
                            Updated = ci.Updated,
                            AddedFrom = ci.AddedFrom,
                            EditedFrom = ci.EditedFrom,
                            ReceivedOn = ci.ReceivedOn,
                            ReceivedOnWeb = ci.ReceivedOnWeb
                        });
            if (String.IsNullOrEmpty(SearchTerm))
            {

                TotalCount = ObjCache.Count();
                return ObjCache;
                //return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<CartItemDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedBy.ToString())))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.LastUpdatedBy.ToString())))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
                    && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
                    && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
                    && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
                    && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
                    && ((Fields[1].Split('@')[24] == "") || (Fields[1].Split('@')[24].Split(',').ToList().Contains(t.ReplenishType)))
                    && ((Fields[1].Split('@')[23] == "") || (Fields[1].Split('@')[23].Split(',').ToList().Contains(t.SupplierId.ToString())))
                    );

                TotalCount = ObjCache.Count();
                return ObjCache;
                //return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<CartItemDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjCache.Where
                    (
                        t => t.ItemGUID.ToString().Contains(SearchTerm) ||
                        //(t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //(t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //(t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //(t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //(t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ItemNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Quantity.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 //||
                                                                                                                            //(t.ItemID.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjCache.Where
                    (t => t.ItemGUID.ToString().Contains(SearchTerm) ||
                        //(t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //(t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //(t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //(t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //(t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ItemNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Quantity.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 //||
                    //(t.ItemID.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    );
                //return ObjCache.Where
                //    (t => t.ID.ToString().Contains(SearchTerm) ||
                //        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                //        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                //        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                //        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                //        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                //        (t.ItemNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                //        (t.Quantity.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                //        (t.ItemID.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                //    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }

        public IEnumerable<CartItemDTO> GetCartItemsByItemId(Int64 RoomID, Int64 CompanyId, Guid ItemGUID, string ColumnName)
        {
            return GetCachedData(RoomID, CompanyId, false, false).Where(t => (t.ItemGUID == ItemGUID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(ColumnName);
        }
        public IEnumerable<CartItemDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsDeleted, bool IsArchived)
        {
            //Get Cached-Media
            eTurnsRegionInfo objeTurnsRegionInfo = null;
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, 0);
            if (objeTurnsRegionInfo == null)
            {
                objeTurnsRegionInfo = new eTurnsRegionInfo();
            }
            IEnumerable<CartItemDTO> ObjCache = null;
            if (!IsDeleted && !IsArchived)
            {
                ObjCache = GetCachedData(RoomID, CompanyID);
                ObjCache = ObjCache.Where(t => t.IsDeleted == false && t.IsArchived == false);
            }
            else
            {
                string sSQL = "";
                if (IsArchived && IsDeleted)
                {
                    sSQL += "A.IsDeleted = 1 AND A.IsArchived = 1";
                }
                else if (IsArchived)
                {
                    sSQL += "A.IsArchived = 1";
                }
                else if (IsDeleted)
                {
                    sSQL += "A.IsDeleted =1";
                }
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    ObjCache = (from u in context.Database.SqlQuery<CartItemDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS 'UpdatedByName', D.RoomName,E.ItemNumber FROM CartItem A inner join ItemMaster as E on  A.ItemGUID = E.GUID LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.CompanyID = " + CompanyID.ToString() + " AND A.Room = " + RoomID.ToString() + @" AND " + sSQL)
                                select new CartItemDTO
                                {
                                    ID = u.ID,

                                    ItemNumber = u.ItemNumber,
                                    ItemGUID = u.ItemGUID,
                                    Quantity = Math.Round(u.Quantity.GetValueOrDefault(0), objeTurnsRegionInfo.NumberDecimalDigits),
                                    Status = u.Status,
                                    ReplenishType = u.ReplenishType,
                                    IsKitComponent = u.IsKitComponent,
                                    UDF1 = u.UDF1,
                                    UDF2 = u.UDF2,
                                    UDF3 = u.UDF3,
                                    UDF4 = u.UDF4,
                                    UDF5 = u.UDF5,
                                    GUID = u.GUID,
                                    Created = u.Created,
                                    Updated = u.Updated,
                                    CreatedBy = u.CreatedBy,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    IsDeleted = u.IsDeleted,
                                    IsArchived = u.IsArchived,
                                    CompanyID = u.CompanyID,
                                    Room = u.Room,
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    RoomName = u.RoomName,
                                    AddedFrom = u.AddedFrom,
                                    EditedFrom = u.EditedFrom,
                                    ReceivedOnWeb = u.ReceivedOnWeb,
                                    ReceivedOn = u.ReceivedOn
                                }).AsParallel().ToList();
                }

            }

            return ObjCache;
        }
        public IEnumerable<CartItemDTO> GetCachedData(Int64 RoomID, Int64 CompanyID)
        {
            eTurnsRegionInfo objeTurnsRegionInfo = null;
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, 0);
            if (objeTurnsRegionInfo == null)
            {
                objeTurnsRegionInfo = new eTurnsRegionInfo();
            }

            IEnumerable<CartItemDTO> ObjCache = CacheHelper<IEnumerable<CartItemDTO>>.GetCacheItem("Cached_CartItem_" + CompanyID.ToString());
            if (ObjCache == null)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<CartItemDTO> obj = (from u in context.Database.SqlQuery<CartItemDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS 'UpdatedByName', D.RoomName,E.ItemNumber FROM CartItem A inner join ItemMaster as E on  A.ItemGUID = E.GUID LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString())
                                                    select new CartItemDTO
                                                    {
                                                        ID = u.ID,
                                                        ItemNumber = u.ItemNumber,
                                                        ItemGUID = u.ItemGUID,
                                                        Quantity = Math.Round(u.Quantity.GetValueOrDefault(0), objeTurnsRegionInfo.NumberDecimalDigits),
                                                        Status = u.Status,
                                                        ReplenishType = u.ReplenishType,
                                                        IsKitComponent = u.IsKitComponent,
                                                        UDF1 = u.UDF1,
                                                        UDF2 = u.UDF2,
                                                        UDF3 = u.UDF3,
                                                        UDF4 = u.UDF4,
                                                        UDF5 = u.UDF5,
                                                        GUID = u.GUID,
                                                        Created = u.Created,
                                                        Updated = u.Updated,
                                                        CreatedBy = u.CreatedBy,
                                                        LastUpdatedBy = u.LastUpdatedBy,
                                                        IsDeleted = u.IsDeleted,
                                                        IsArchived = u.IsArchived,
                                                        CompanyID = u.CompanyID,
                                                        Room = u.Room,
                                                        CreatedByName = u.CreatedByName,
                                                        UpdatedByName = u.UpdatedByName,
                                                        RoomName = u.RoomName,
                                                        AddedFrom = u.AddedFrom,
                                                        EditedFrom = u.EditedFrom,
                                                        ReceivedOn = u.ReceivedOn,
                                                        ReceivedOnWeb = u.ReceivedOnWeb
                                                    }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<CartItemDTO>>.AddCacheItem("Cached_CartItem_" + CompanyID.ToString(), obj);
                }
            }

            return ObjCache.Where(t => t.Room == RoomID);
        }
        private void DeleteAutoCartItems(Guid ItemGUID, bool IsFromUI)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<BinMaster> lstItemlocations = context.BinMasters.Where(t => t.IsDeleted == false && (t.IsArchived ?? false) == false && t.ItemGUID == ItemGUID).ToList();
                long[] binIds = lstItemlocations.Select(t => t.ID).ToArray();
                var carts = context.CartItems.Where(t => (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false && t.IsAutoMatedEntry == true && t.ItemGUID == ItemGUID && t.BinId != null);
                foreach (var item in carts)
                {
                    if (!binIds.Contains(item.BinId ?? 0))
                    {
                        item.IsDeleted = true;
                        if (IsFromUI)
                        {
                            item.EditedFrom = "Web";
                            item.ReceivedOn = DateTimeUtility.DateTimeNow;
                        }
                    }
                }
                context.SaveChanges();
                ItemMaster objItemMaster = context.ItemMasters.FirstOrDefault(im => im.GUID == ItemGUID);
                if (objItemMaster != null)
                {
                    Dictionary<string, double> SOT = GetItemSuggestedOrderQty(ItemGUID);
                    objItemMaster.SuggestedOrderQuantity = SOT["SO"];
                    objItemMaster.SuggestedTransferQuantity = SOT["ST"];
                    objItemMaster.WhatWhereAction = "Cart";
                }
                context.SaveChanges();
            }
        }
        public double GetSuggestedOrderQty(Guid ItemGUIDId, string ReplenishType)
        {
            double TotalSuggestedQty = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (context.CartItems.Any(t => t.ItemGUID == ItemGUIDId && t.IsDeleted == false && t.IsArchived == false && t.ReplenishType == ReplenishType && t.IsAutoMatedEntry == false))
                {
                    TotalSuggestedQty = context.CartItems.Where(t => t.ItemGUID == ItemGUIDId && t.ReplenishType == ReplenishType && t.IsDeleted == false && t.IsArchived == false && t.IsAutoMatedEntry == false).Sum(t => (t.Quantity ?? 0));
                }
                else
                {
                    TotalSuggestedQty = 0;
                }
            }
            return TotalSuggestedQty;

        }

        public Dictionary<string, double> GetItemSuggestedOrderQty(Guid ItemGUIDId)
        {
            Dictionary<string, double> retDictionary = new Dictionary<string, double>();
            double TotalSuggestedQty = 0;
            double TotalSuggestedTQty = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (context.CartItems.Any(t => t.ItemGUID == ItemGUIDId && t.IsDeleted == false && t.IsArchived == false && t.ReplenishType == "Purchase"))
                {
                    TotalSuggestedQty = context.CartItems.Where(t => t.ItemGUID == ItemGUIDId && t.ReplenishType == "Purchase" && t.IsDeleted == false && t.IsArchived == false).Sum(t => (t.Quantity ?? 0));
                }
                else
                {
                    TotalSuggestedQty = 0;
                }

                if (context.CartItems.Any(t => t.ItemGUID == ItemGUIDId && t.IsDeleted == false && t.IsArchived == false && t.ReplenishType == "Transfer"))
                {
                    TotalSuggestedTQty = context.CartItems.Where(t => t.ItemGUID == ItemGUIDId && t.ReplenishType == "Transfer" && t.IsDeleted == false && t.IsArchived == false).Sum(t => (t.Quantity ?? 0));
                }
                else
                {
                    TotalSuggestedTQty = 0;
                }
            }
            retDictionary["SO"] = TotalSuggestedQty;
            retDictionary["ST"] = TotalSuggestedTQty;
            return retDictionary;

        }

        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyID, Int64 EnterpriseID, long SessionUserId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                var guidInString = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        if (string.IsNullOrEmpty(guidInString))
                        {
                            guidInString += "'" + item.Trim() + "'";
                        }
                        else
                        {
                            guidInString += ",'" + item.Trim() + "'";
                        }
                    }
                }
                if (!string.IsNullOrEmpty(guidInString))
                {
                    strQuery += "UPDATE CartItem SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1,EditedFrom='Web',ReceivedOn='" + DateTimeUtility.DateTimeNow + "' WHERE GUID IN (" + guidInString.Trim().ToString() + ");";
                    context.ExecuteStoreCommand(strQuery);
                }

                //Get Cached-Media
                //IEnumerable<CartItemDTO> ObjCache = CacheHelper<IEnumerable<CartItemDTO>>.GetCacheItem("Cached_CartItem_" + CompanyID.ToString());
                //if (ObjCache != null)
                //{
                //    List<CartItemDTO> objTemp = ObjCache.ToList();
                //    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.GUID.ToString()));
                //    ObjCache = objTemp.AsEnumerable();
                //    CacheHelper<IEnumerable<CartItemDTO>>.AppendToCacheItem("Cached_CartItem_" + CompanyID.ToString(), ObjCache);
                //}

                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        CartItem objCartItemDTO = new CartItem();
                        Guid ciguid = Guid.Empty;
                        if (Guid.TryParse(item.ToString(), out ciguid))
                        {
                            objCartItemDTO = context.CartItems.FirstOrDefault(t => t.GUID == ciguid);
                            if (objCartItemDTO != null)
                            {
                                UpdateSuggestedQtyOfItem(objCartItemDTO.ItemGUID ?? Guid.Empty, objCartItemDTO.Room ?? 0, objCartItemDTO.CompanyID ?? 0, objCartItemDTO.LastUpdatedBy ?? 0, SessionUserId);
                            }
                        }
                    }
                }
                return true;
            }
        }

        public void SuggestedOrderRoom(long RoomId, long CompanyId, long UserId, long EnterpriseID, long SessionUserId, bool isAfterSync = false)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Room objRoom = context.Rooms.FirstOrDefault(t => t.ID == RoomId && t.IsDeleted == false);
                if (objRoom != null)
                {
                    if (objRoom.SuggestedOrder)
                    {
                        List<Guid> AllRoomItems;
                        if (isAfterSync)
                        {
                            List<Guid> oItemGuid = (from u in context.ExecuteStoreQuery<Guid>(string.Format(@"
                                SELECT IM.GUID FROM ItemMaster as IM INNER JOIN ItemMaster_tracking IT ON IM.GUID=IT.GUID
                                WHERE ISNULL(IM.IsDeleted,0)=0 AND ISNULL(IM.ISArchived,0)=0 AND Im.Room={0} AND IM.CompanyID={1}
                                AND IT.last_change_datetime > DATEADD(Minute,-30,GETDATE())", RoomId, CompanyId))
                                                    select u 
                                               ).AsParallel().ToList();

                            AllRoomItems = oItemGuid;
                        }
                        else
                        {
                            AllRoomItems = context.ItemMasters.Where(t => t.Room == RoomId && t.CompanyID == CompanyId && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Select(t => t.GUID).ToList();
                        }

                        if (AllRoomItems != null && AllRoomItems.Any())
                        {
                            foreach (var item in AllRoomItems)
                            {
                                //AutoCartUpdateByCode(item, UserId, "web", "CartItemDAL>SuggestedOrderRoom");
                                AutoCartUpdateByCode(item, UserId, "web", "Replenish >> Order", SessionUserId);

                            }
                        }
                    }
                    else
                    {
                        //IQueryable<CartItem> lstcarts = context.CartItems.Where(t => t.Room == RoomId && (t.IsDeleted ?? false) == false && t.IsAutoMatedEntry == true && t.CompanyID == CompanyId && t.ReplenishType == "Purchase");
                        //if (lstcarts.Any())
                        //{
                        //    foreach (CartItem cartitem in lstcarts)
                        //    {
                        //        cartitem.IsDeleted = true;
                        //    }
                        //    context.SaveChanges();
                        //}
                        string DeleteSOferqry = "UPDATE CartItem SET IsDeleted = 1, editedfrom = 'web', lastupdatedby = 2, updated = GETUTCDATE(), whatwhereaction = 'Web all cart from Room', receivedon = GETUTCDATE() WHERE Room = " + RoomId + " AND CompanyID = " + CompanyId + " AND isnull(IsDeleted, 0) = 0 AND ReplenishType = 'Purchase' AND IsAutoMatedEntry = 1; UPDATE ItemMaster  SET SuggestedOrderQuantity = 0 WHERE isnull(Isdeleted, 0) = 0 AND Room = " + RoomId + " AND CompanyID = " + CompanyId + " and isnull(SuggestedOrderQuantity,0) > 0";
                        context.ExecuteStoreCommand(DeleteSOferqry);
                    }

                    if (objRoom.SuggestedTransfer)
                    {
                        List<Guid> AllRoomItems;
                        if (isAfterSync)
                        {
                            List<Guid> oItemGuid = (from u in context.ExecuteStoreQuery<Guid>(string.Format(@"
                                SELECT IM.GUID FROM ItemMaster as IM INNER JOIN ItemMaster_tracking IT ON IM.GUID=IT.GUID
                                WHERE ISNULL(IM.IsDeleted,0)=0 AND ISNULL(IM.ISArchived,0)=0 AND Im.Room={0} AND IM.CompanyID={1}
                                AND IT.last_change_datetime > DATEADD(Minute,-30,GETDATE())", RoomId, CompanyId))
                                                    select u
                                               ).AsParallel().ToList();

                            AllRoomItems = oItemGuid;
                        }
                        else
                        {
                            AllRoomItems = context.ItemMasters.Where(t => t.Room == RoomId && t.CompanyID == CompanyId && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Select(t => t.GUID).ToList();
                        }

                        if (AllRoomItems != null && AllRoomItems.Any())
                        {
                            foreach (var item in AllRoomItems)
                            {
                                //AutoCartUpdateByCode(item, UserId, "web", "CartItemDAL>SaveCartItems");
                                AutoCartUpdateByCode(item, UserId, "web", "Replenish >> SaveCart", SessionUserId);
                            }
                        }
                    }
                    else
                    {
                        string DeleteSTqry = "UPDATE CartItem SET IsDeleted = 1, editedfrom = 'web', lastupdatedby = 2, updated = GETUTCDATE(), whatwhereaction = 'Web all cart from Room', receivedon = GETUTCDATE() WHERE Room = " + RoomId + " AND CompanyID = " + CompanyId + " AND isnull(IsDeleted, 0) = 0 AND ReplenishType = 'Transfer' AND IsAutoMatedEntry = 1; UPDATE ItemMaster  SET SuggestedTransferQuantity = 0 WHERE isnull(Isdeleted, 0) = 0 AND Room = " + RoomId + " AND CompanyID = " + CompanyId + " and isnull(SuggestedTransferQuantity,0) > 0";
                        context.ExecuteStoreCommand(DeleteSTqry);


                        //IQueryable<CartItem> lstcarts = context.CartItems.Where(t => t.Room == RoomId && (t.IsDeleted ?? false) == false && t.IsAutoMatedEntry == true && t.CompanyID == CompanyId && t.ReplenishType == "Transfer");                        

                        //IQueryable<CartItem> lstcarts = context.CartItems.Where(t => t.Room == RoomId && (t.IsDeleted ?? false) == false && t.IsAutoMatedEntry == true && t.CompanyID == CompanyId && t.ReplenishType == "Transfer");
                        //if (lstcarts.Any())
                        //{
                        //    foreach (CartItem cartitem in lstcarts)
                        //    {
                        //        cartitem.IsDeleted = true;
                        //    }
                        //    context.SaveChanges();
                        //}
                    }

                    if (isAfterSync && objRoom.SuggestedReturn)
                    {
                        List<Guid> AllRoomItems;
                        if (isAfterSync)
                        {
                            List<Guid> oItemGuid = (from u in context.ExecuteStoreQuery<Guid>(string.Format(@"
                                SELECT IM.GUID FROM ItemMaster as IM INNER JOIN ItemMaster_tracking IT ON IM.GUID=IT.GUID
                                WHERE ISNULL(IM.IsDeleted,0)=0 AND ISNULL(IM.ISArchived,0)=0 AND Im.Room={0} AND IM.CompanyID={1}
                                AND IT.last_change_datetime > DATEADD(Minute,-30,GETDATE())", RoomId, CompanyId))
                                                    select u
                                               ).AsParallel().ToList();

                            AllRoomItems = oItemGuid;
                        }
                        else
                        {
                            AllRoomItems = context.ItemMasters.Where(t => t.Room == RoomId && t.CompanyID == CompanyId && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Select(t => t.GUID).ToList();
                        }

                        if (AllRoomItems != null && AllRoomItems.Any())
                        {
                            foreach (var item in AllRoomItems)
                            {
                                AutoCartForSuggestedReturnUpdateByCode(item, UserId, "web", "Replenish >> SaveCart", SessionUserId);
                            }
                        }
                    }
                    else
                    {
                        var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId),
                                                           new SqlParameter("@CompanyID", CompanyId),
                                                           new SqlParameter("@userid", UserId),
                                                           new SqlParameter("@editedfrom", "web"),
                                                           new SqlParameter("@calledfromfunctionname", "Web all cart from Room"),
                                                           new SqlParameter("@ReplenishType", "SuggestedReturn")
                                                         };
                        context.CommandTimeout = 3600;
                        context.ExecuteStoreCommand("EXEC [DeleteSuggestedReturnCartFromRoomSave] @RoomID,@CompanyID,@userid,@editedfrom,@calledfromfunctionname,@ReplenishType", params1);

                        //string DeleteSTqry = "UPDATE CartItem SET IsDeleted = 1, editedfrom = 'web', lastupdatedby = 2, updated = GETUTCDATE(), whatwhereaction = 'Web all cart from Room', receivedon = GETUTCDATE() WHERE Room = " + RoomId + " AND CompanyID = " + CompanyId + " AND isnull(IsDeleted, 0) = 0 AND ReplenishType = 'SuggestedReturn' AND IsAutoMatedEntry = 1; UPDATE ItemMaster  SET SuggestedReturnQuantity = 0 WHERE isnull(Isdeleted, 0) = 0 AND Room = " + RoomId + " AND CompanyID = " + CompanyId + " and isnull(SuggestedReturnQuantity,0) > 0";
                        //context.ExecuteStoreCommand(DeleteSTqry);
                    }
                }
            }
        }

        public void SuggestedReturnOrderRoom(long RoomId, long CompanyId, long UserId, long EnterpriseID, long SessionUserId, bool isAfterSync = false)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Room objRoom = context.Rooms.FirstOrDefault(t => t.ID == RoomId && t.IsDeleted == false);
                if (objRoom != null)
                {
                    #region Suggested Return

                    if (objRoom.SuggestedReturn)
                    {
                        List<Guid> AllRoomItems;
                        if (isAfterSync)
                        {
                            List<Guid> oItemGuid = (from u in context.ExecuteStoreQuery<Guid>(string.Format(@"
                                SELECT IM.GUID FROM ItemMaster as IM INNER JOIN ItemMaster_tracking IT ON IM.GUID=IT.GUID
                                WHERE ISNULL(IM.IsDeleted,0)=0 AND ISNULL(IM.ISArchived,0)=0 AND Im.Room={0} AND IM.CompanyID={1}
                                AND IT.last_change_datetime > DATEADD(Minute,-30,GETDATE())", RoomId, CompanyId))
                                                    select u
                                               ).AsParallel().ToList();

                            AllRoomItems = oItemGuid;
                        }
                        else
                        {
                            AllRoomItems = context.ItemMasters.Where(t => t.Room == RoomId && t.CompanyID == CompanyId && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Select(t => t.GUID).ToList();
                        }

                        if (AllRoomItems != null && AllRoomItems.Any())
                        {
                            foreach (var item in AllRoomItems)
                            {
                                AutoCartForSuggestedReturnUpdateByCode(item, UserId, "web", "Replenish >> Suggested Return", SessionUserId);
                            }
                        }
                    }
                    else
                    {
                        var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId),
                                                           new SqlParameter("@CompanyID", CompanyId),
                                                           new SqlParameter("@userid", UserId),
                                                           new SqlParameter("@editedfrom", "web"),
                                                           new SqlParameter("@calledfromfunctionname", "Web all cart from Room"),
                                                           new SqlParameter("@ReplenishType", "SuggestedReturn")
                                                         };
                        context.ExecuteStoreCommand("EXEC [DeleteSuggestedReturnCartFromRoomSave] @RoomID,@CompanyID,@userid,@editedfrom,@calledfromfunctionname,@ReplenishType", params1);

                        //string DeleteSOferqry = "UPDATE CartItem SET IsDeleted = 1, editedfrom = 'web', lastupdatedby = 2, updated = GETUTCDATE(), whatwhereaction = 'Web all cart from Room', receivedon = GETUTCDATE() WHERE Room = " + RoomId + " AND CompanyID = " + CompanyId + " AND isnull(IsDeleted, 0) = 0 AND ReplenishType = 'SuggestedReturn' AND IsAutoMatedEntry = 1; UPDATE ItemMaster  SET SuggestedReturnQuantity = 0 WHERE isnull(Isdeleted, 0) = 0 AND Room = " + RoomId + " AND CompanyID = " + CompanyId + " and isnull(SuggestedReturnQuantity,0) > 0";
                        //context.ExecuteStoreCommand(DeleteSOferqry);
                    }

                    #endregion
                }
            }
        }
    }
}
