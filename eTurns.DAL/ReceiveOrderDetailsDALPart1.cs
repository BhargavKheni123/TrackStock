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


namespace eTurns.DAL
{
    public partial class ReceiveOrderDetailsDALPart1 : eTurnsBaseDAL
    {

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(ReceiveOrderDetailsDTO objDTO)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ReceiveOrderDetail obj = context.ReceiveOrderDetails.Single(t => t.ID == objDTO.ID && t.OrderDetailGUID == objDTO.OrderDetailGUID);

                double PreviousQuentity = obj.ReceiveQuantity.GetValueOrDefault(0);


                obj.ReceiveBin = objDTO.ReceiveBin;
                obj.ReceiveDate = objDTO.ReceiveDate;
                obj.ReceiveQuantity = objDTO.ReceiveQuantity;
                obj.LastUpdated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.CompanyID = objDTO.CompanyID;
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Unchanged);
                context.ReceiveOrderDetails.Attach(obj);
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                context.SaveChanges();




                OrderDetailsDAL OrdDetail = new OrderDetailsDAL(base.DataBaseName);
                OrderDetailsDTO ordDetailDTO = OrdDetail.GetRecord(obj.OrderDetailGUID.GetValueOrDefault(Guid.Empty), obj.Room.GetValueOrDefault(0), obj.CompanyID.GetValueOrDefault(0));
                if (ordDetailDTO != null && ordDetailDTO.ItemGUID != Guid.Empty)
                {
                    double UpdateQutenty = (objDTO.ReceiveQuantity.GetValueOrDefault(0) - PreviousQuentity);
                    int UpdateType = UpdateQutenty < 0 ? -1 : 1;

                    CommonDAL commonDAL = new CommonDAL(base.DataBaseName);
                    commonDAL.UpdateItemQuentity(ordDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.ReceiveBin.GetValueOrDefault(0), UpdateQutenty, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.LastUpdatedBy.GetValueOrDefault(0), UpdateType);
                }

                //Get Cached-Media
                IEnumerable<ReceiveOrderDetailsDTO> ObjCache = CacheHelper<IEnumerable<ReceiveOrderDetailsDTO>>.GetCacheItem("Cached_ReceiveOrderDetails_" + objDTO.CompanyID.ToString());
                if (ObjCache != null)
                {

                    objDTO.ID = obj.ID;
                    objDTO.GUID = obj.GUID;
                    objDTO.Created = obj.Created;
                    objDTO.CreatedBy = obj.CreatedBy;
                    objDTO.CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Int64.Parse(Convert.ToString(obj.CreatedBy))).UserName;
                    objDTO.IsArchived = false;
                    objDTO.IsDeleted = false;
                    objDTO.OrderDetailGUID = obj.OrderDetailGUID;

                    objDTO.RoomName = new RoomDAL(base.DataBaseName).GetRecord(Int64.Parse(Convert.ToString(obj.Room)), Convert.ToInt64(objDTO.CompanyID), false, false).RoomName;
                    objDTO.UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Int64.Parse(Convert.ToString(obj.LastUpdatedBy))).UserName;
                    objDTO.LastUpdated = obj.LastUpdated;
                    objDTO.LastUpdatedBy = obj.LastUpdatedBy;
                    objDTO.ReceiveBin = obj.ReceiveBin;
                    objDTO.ReceiveDate = obj.ReceiveDate;
                    objDTO.ReceiveQuantity = obj.ReceiveQuantity;
                    objDTO.Room = obj.Room;
                    objDTO.CompanyID = obj.CompanyID;


                    List<ReceiveOrderDetailsDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(x => x.ID == objDTO.ID);
                    ObjCache = objTemp.AsEnumerable();

                    List<ReceiveOrderDetailsDTO> tempC = new List<ReceiveOrderDetailsDTO>();
                    tempC.Add(objDTO);
                    IEnumerable<ReceiveOrderDetailsDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    CacheHelper<IEnumerable<ReceiveOrderDetailsDTO>>.AppendToCacheItem("Cached_ReceiveOrderDetails_" + objDTO.CompanyID.ToString(), NewCache);
                }


                return true;
            }
        }

        public IEnumerable<ReceiveOrderDetailsDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid OrderDetailGUID)
        {
            //Get Cached-Media
            IEnumerable<ReceiveOrderDetailsDTO> ObjCache = GetCachedData(RoomID, CompanyID).Where(x => x.OrderDetailGUID.GetValueOrDefault(Guid.Empty) == OrderDetailGUID && x.IsDeleted == IsDeleted && x.IsArchived == IsArchived);
            IEnumerable<ReceiveOrderDetailsDTO> ObjGlobalCache = ObjCache;
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
                //IEnumerable<ReceiveOrderDetailsDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.LastUpdated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.LastUpdated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<ReceiveOrderDetailsDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm)
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm)
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }

        public IEnumerable<ReceiveOrderDetailsDTO> GetCachedData(Int64 RoomID, Int64 CompanyID)
        {
            //Get Cached-Media
            IEnumerable<ReceiveOrderDetailsDTO> ObjCache = CacheHelper<IEnumerable<ReceiveOrderDetailsDTO>>.GetCacheItem("Cached_ReceiveOrderDetails_" + CompanyID.ToString());
            if (ObjCache == null || ObjCache.Count() <= 0)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<ReceiveOrderDetailsDTO> obj = (from u in context.ExecuteStoreQuery<ReceiveOrderDetailsDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM ReceiveOrderDetails A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.CompanyID = " + CompanyID.ToString())
                                                               select new ReceiveOrderDetailsDTO
                                                               {
                                                                   ID = u.ID,
                                                                   OrderDetailGUID = u.OrderDetailGUID,
                                                                   ReceiveBin = u.ReceiveBin,
                                                                   ReceiveDate = u.ReceiveDate,
                                                                   ReceiveQuantity = u.ReceiveQuantity,
                                                                   Created = u.Created,
                                                                   LastUpdated = u.LastUpdated,
                                                                   CreatedBy = u.CreatedBy,
                                                                   LastUpdatedBy = u.LastUpdatedBy,
                                                                   Room = u.Room,
                                                                   IsDeleted = u.IsDeleted,
                                                                   IsArchived = u.IsArchived,
                                                                   CompanyID = u.CompanyID,
                                                                   GUID = u.GUID,
                                                                   CreatedByName = u.CreatedByName,
                                                                   UpdatedByName = u.UpdatedByName,
                                                                   RoomName = u.RoomName,
                                                                   ReceiveBinName = u.ReceiveBin.GetValueOrDefault(0) > 0 ? new BinMasterDAL(base.DataBaseName).GetBinByID(u.ReceiveBin.GetValueOrDefault(0), u.Room.GetValueOrDefault(0), u.CompanyID.GetValueOrDefault(0)).BinNumber : string.Empty,
                                                                   //ReceiveBinName = u.ReceiveBin.GetValueOrDefault(0) > 0 ? new BinMasterDAL(base.DataBaseName).GetItemLocation( u.Room.GetValueOrDefault(0), u.CompanyID.GetValueOrDefault(0), false, false,Guid.Empty, u.ReceiveBin.GetValueOrDefault(0),null,null).FirstOrDefault().BinNumber : string.Empty,

                                                               }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<ReceiveOrderDetailsDTO>>.AddCacheItem("Cached_ReceiveOrderDetails_" + CompanyID.ToString(), obj);
                }
            }

            //ObjCache.ToList().ForEach(x => x.OrderDetail = new OrderDetailsDAL(base.DataBaseName).GetRecord(x.OrderDetailID.GetValueOrDefault(0), x.Room.GetValueOrDefault(0), x.CompanyID.GetValueOrDefault(0)));
            return ObjCache.Where(t => t.Room == RoomID);
        }

        public ReceiveOrderDetailsDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            return GetCachedData(RoomID, CompanyID).Single(t => t.ID == id);
        }

        public IEnumerable<ReceiveOrderDetailsDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId)
        {
            return GetCachedData(RoomID, CompanyId).OrderBy("ID DESC");
        }

        public ReceiveOrderDetailsDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<ReceiveOrderDetailsDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM ReceiveOrderDetails_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + id.ToString())
                        select new ReceiveOrderDetailsDTO
                        {
                            ID = u.ID,
                            OrderDetailGUID = u.OrderDetailGUID,
                            ReceiveBin = u.ReceiveBin,
                            ReceiveDate = u.ReceiveDate,
                            ReceiveQuantity = u.ReceiveQuantity,
                            Created = u.Created,
                            LastUpdated = u.LastUpdated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            GUID = u.GUID,
                        }).SingleOrDefault();
            }
        }

        public IEnumerable<ReceiveOrderLineItemDetailsDTO> GetPagedPendigLineItemsRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            //Get Cached-Media
            IEnumerable<ReceiveOrderLineItemDetailsDTO> ObjLineItems = GetPendigLineItemsRecords(RoomID, CompanyID);

            if (String.IsNullOrEmpty(SearchTerm))
            {

                TotalCount = ObjLineItems.Count();
                return ObjLineItems.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<OrderDetailsDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                ObjLineItems = GetPendigLineItemsRecordsForNarrawSearch(RoomID, CompanyID);
                ObjLineItems = ObjLineItems.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.LastUpdated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.LastUpdated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[26] == "") || (Fields[1].Split('@')[26].Split(',').ToList().Contains(t.OrderNumber))));

                var objItems = (from u in ObjLineItems
                                group u by new { u.ItemGUID, u.ItemNumber, u.Description, u.ItemType } into oo
                                select new ReceiveOrderLineItemDetailsDTO
                                {
                                    ItemGUID = oo.Key.ItemGUID,
                                    ItemNumber = oo.Key.ItemNumber,
                                    Description = oo.Key.Description,
                                    ItemType = oo.Key.ItemType,
                                }).Distinct().ToList();

                TotalCount = objItems.Count();
                return objItems.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<OrderDetailsDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjLineItems.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        ((t.ItemID ?? 0).ToString()).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ItemNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Description ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjLineItems.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        ((t.ItemID ?? 0).ToString()).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ItemNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Description ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }

        public IEnumerable<ReceiveOrderLineItemDetailsDTO> GetPendigLineItemsRecords(Int64 RoomID, Int64 CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                // string qry = " SELECT distinct(A.ItemID),A.ReceivedQuantity,IM.ItemNumber as ItemNumber,IM.Description,IM.ItemType ";
                string qry = " SELECT distinct (A.ItemGUID),IM.ItemNumber as ItemNumber,IM.Description,IM.ItemType ";
                qry += " FROM OrderDetails A LEFT OUTER  JOIN OrderMaster OM on OM.GUID= A.OrderGUID ";
                qry += " LEFT OUTER  JOIN ItemMaster IM on IM.GUID= A.ItemGUID LEFT OUTER JOIN Room D on A.Room = D.ID ";
                //qry += " WHERE A.CompanyID =  " + CompanyId.ToString() + " and OM.OrderStatus in (2,3,4,5,6) and OM.Room=" + RoomID + " and A.ReceivedQuantity < A.RequestedQuantity ";
                qry += " WHERE A.CompanyID =  " + CompanyId.ToString() + " and OM.OrderStatus in (4,5,6,7) and OM.Room=" + RoomID + " and ISNULL(A.ReceivedQuantity,0) < ISNULL(A.RequestedQuantity,0) ";
                qry += "  and ISNULL(OM.IsArchived,0) = 0 and ISNULL(OM.IsDeleted,0) =0 and ISNULL(A.IsArchived,0) = 0 and ISNULL(A.IsDeleted,0) =0 and ISNULL(A.IsCloseItem,0) =0";
                IEnumerable<ReceiveOrderLineItemDetailsDTO> obj = (from u in context.ExecuteStoreQuery<ReceiveOrderLineItemDetailsDTO>(qry)
                                                                   select new ReceiveOrderLineItemDetailsDTO
                                                                   {
                                                                       ReceivedQuantity = u.ReceivedQuantity,
                                                                       RequestedQuantity = u.RequestedQuantity,
                                                                       ItemGUID = u.ItemGUID,
                                                                       ItemNumber = u.ItemNumber,
                                                                       Description = u.Description,
                                                                       OrderID = u.OrderID,
                                                                       RequiredDate = u.RequiredDate,
                                                                       Room = u.Room,
                                                                       CurrentReceivedDate = DateTime.Now,
                                                                       ItemType = u.ItemType,
                                                                   }).AsParallel().ToList();
                return obj;
            }
        }

        public IEnumerable<ReceiveOrderLineItemDetailsDTO> GetPendigLineItemsRecordsForNarrawSearch(Int64 RoomID, Int64 CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                string qry = @"  SELECT distinct (A.ItemGUID), IM.ItemNumber as ItemNumber, IM.Description,IM.ItemType  
	                                            ,OM.Created,OM.LastUpdated,OM.OrderNumber,OM.CreatedBy,OM.LastUpdatedBy, UMC.UserName as CreatedByName,UMU.UserName as UpdatedByName
                                FROM OrderDetails A LEFT OUTER  JOIN OrderMaster OM on OM.GUID= A.OrderGUID  
					                                LEFT OUTER  JOIN ItemMaster IM on IM.GUID= A.ItemGUID 
					                                LEFT OUTER JOIN Room D on A.Room = D.ID 
					                                Left OUter Join UserMaster UMC ON OM.CreatedBy = UMC.ID
					                                Left OUter Join UserMaster UMU ON OM.LastUpdatedBy= UMU.ID
                                WHERE A.CompanyID =  " + CompanyId + @" and OM.OrderStatus in (4,5,6,7) 
                                and OM.Room=" + RoomID + @" and ISNULL(A.ReceivedQuantity,0) < ISNULL(A.RequestedQuantity,0)  
                                and ISNULL(Om.IsArchived,0) = 0 and ISNULL(Om.IsCloseItem,0) = 0 and ISNULL(OM.IsDeleted,0) =0  and ISNULL(A.IsArchived,0) = 0 and ISNULL(A.IsDeleted,0) =0";
                IEnumerable<ReceiveOrderLineItemDetailsDTO> obj = (from u in context.ExecuteStoreQuery<ReceiveOrderLineItemDetailsDTO>(qry)
                                                                   select new ReceiveOrderLineItemDetailsDTO
                                                                   {
                                                                       GUID = u.GUID,
                                                                       ReceivedQuantity = u.ReceivedQuantity,
                                                                       RequestedQuantity = u.RequestedQuantity,
                                                                       ItemNumber = u.ItemNumber,
                                                                       ItemGUID = u.ItemGUID,
                                                                       Description = u.Description,
                                                                       OrderGUID = u.OrderGUID,
                                                                       RequiredDate = u.RequiredDate,
                                                                       Room = u.Room,
                                                                       CurrentReceivedDate = DateTime.Now,
                                                                       ItemType = u.ItemType,
                                                                       CreatedByName = u.CreatedByName,
                                                                       UpdatedByName = u.UpdatedByName,
                                                                       OrderNumber = u.OrderNumber,
                                                                       Created = u.Created,
                                                                       LastUpdated = u.LastUpdated,
                                                                   }).AsParallel().ToList();
                return obj;
            }
        }

        public IEnumerable<ReceiveOrderLineItemDetailsDTO> GetLineItemsOrderRecords(Int64 RoomID, Int64 CompanyId, string ItemGUID, Int64 OrderID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                string qry = " SELECT A.GUID,A.ItemGUID,A.OrderGUID,A.RequestedQuantity,  A.ReceivedQuantity,IM.ItemNumber as ItemNumber, ";
                qry += " IM.Description,IM.SupplierID as Supplier,SM.SupplierName,A.RequiredDate,OM.Room,OM.OrderNumber,IM.ItemType FROM OrderDetails A  ";
                qry += " LEFT OUTER  JOIN OrderMaster OM on OM.GUID= A.OrderGUID LEFT OUTER  JOIN ItemMaster IM on IM.GUID= A.ItemGUID ";
                qry += " LEFT OUTER JOIN Room D on A.Room = D.ID LEFT OUTER  JOIN SupplierMaster SM on SM.ID= IM.SupplierID ";
                //qry += " WHERE A.CompanyID =  " + CompanyId.ToString() + " and OM.OrderStatus in (2,3,4,5,6) and OM.Room=" + RoomID + " and A.ReceivedQuantity < A.RequestedQuantity ";
                qry += " WHERE A.CompanyID =  " + CompanyId.ToString() + " and OM.OrderStatus in (4,5,6,7) and OM.Room=" + RoomID + " and ISNULL(A.ReceivedQuantity,0) < ISNULL(A.RequestedQuantity,0) ";
                qry += " and Om.IsArchived!=1 and OM.IsDeleted!=1 and A.IsArchived!=1 and A.IsDeleted!=1 and IsNull(A.IsCloseItem,0) =0 and A.ItemGUID= '" + ItemGUID.ToString() + "' ";
                if (OrderID > 0)
                {
                    qry += " and A.OrderID= " + OrderID.ToString() + " ";
                }
                IEnumerable<ReceiveOrderLineItemDetailsDTO> obj = (from u in context.ExecuteStoreQuery<ReceiveOrderLineItemDetailsDTO>(qry)
                                                                   select new ReceiveOrderLineItemDetailsDTO
                                                                   {
                                                                       GUID = u.GUID,
                                                                       ItemGUID = u.ItemGUID,
                                                                       OrderGUID = u.OrderGUID,
                                                                       //OrderID = u.OrderID,
                                                                       ItemNumber = u.ItemNumber,
                                                                       ReceivedQuantity = u.ReceivedQuantity,
                                                                       RequestedQuantity = u.RequestedQuantity,
                                                                       SupplierName = u.SupplierName,
                                                                       Supplier = u.Supplier,
                                                                       Description = u.Description,
                                                                       RequiredDate = u.RequiredDate,
                                                                       OrderNumber = u.OrderNumber,
                                                                       Room = u.Room,
                                                                       ItemType = u.ItemType,

                                                                   }).AsParallel().ToList();
                return obj;
            }
        }

        public ReceiveOrderLineItemDetailsDTO GetLineItemsOrderDetails(Guid OrderDetailsID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                string qry = " SELECT A.ID,A.GUID,A.ItemGUID,A.OrderGUID,A.RequestedQuantity,  A.ReceivedQuantity,IM.ItemNumber as ItemNumber, ";
                qry += " IM.Description,IM.SupplierID as Supplier,SM.SupplierName,A.RequiredDate,OM.Room,OM.OrderNumber,IM.ItemType, A.Bin as DefaultBin ";
                qry += " ,OM.StagingID, OM.CustomerAddress FROM OrderDetails A  ";
                qry += " LEFT OUTER  JOIN OrderMaster OM on OM.GUID= A.OrderGUID LEFT OUTER  JOIN ItemMaster IM on IM.GUID= A.ItemGUID ";
                qry += " LEFT OUTER JOIN Room D on A.Room = D.ID LEFT OUTER  JOIN SupplierMaster SM on SM.ID= IM.SupplierID ";
                //qry += " WHERE OM.OrderStatus in (2,3,4,5,6) and A.ReceivedQuantity < A.RequestedQuantity ";
                qry += " WHERE OM.OrderStatus in (4,5,6,7) and ISNULL(A.ReceivedQuantity,0) < ISNULL(A.RequestedQuantity,0) ";
                qry += " and Om.IsArchived!=1 and OM.IsDeleted!=1 and IsNull(A.IsCloseItem,0) =0 and A.GUID= '" + OrderDetailsID.ToString() + "' ";
                ReceiveOrderLineItemDetailsDTO obj = (from u in context.ExecuteStoreQuery<ReceiveOrderLineItemDetailsDTO>(qry)
                                                      select new ReceiveOrderLineItemDetailsDTO
                                                      {
                                                          ID = u.ID,
                                                          GUID = u.GUID,
                                                          OrderGUID = u.OrderGUID,
                                                          ItemGUID = u.ItemGUID,
                                                          ItemNumber = u.ItemNumber,
                                                          ReceivedQuantity = u.ReceivedQuantity,
                                                          RequestedQuantity = u.RequestedQuantity,
                                                          SupplierName = u.SupplierName,
                                                          Supplier = u.Supplier,
                                                          Description = u.Description,
                                                          RequiredDate = u.RequiredDate,
                                                          OrderNumber = u.OrderNumber,
                                                          Room = u.Room,
                                                          ItemType = u.ItemType,
                                                          DefaultBin = u.DefaultBin,
                                                          StagingID = u.StagingID.GetValueOrDefault(0),
                                                          CustomerAddress = u.CustomerAddress
                                                      }).SingleOrDefault();
                return obj;
            }
        }

        public IEnumerable<ReceiveOrderLineItemDetailsDTO> GetStatuswiseOrderRecords(Int64 RoomID, Int64 CompanyId, Guid ItemGUID, Int64 OrderStatus)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                string qry = " SELECT A.ID,A.GUID,A.ItemGUID,A.OrderGUID,A.RequestedQuantity,  A.ReceivedQuantity,IM.ItemNumber as ItemNumber, ";
                qry += " IM.Description,IM.SupplierID as Supplier,SM.SupplierName,A.RequiredDate,OM.Room,OM.OrderNumber,IM.ItemType FROM OrderDetails A  ";
                qry += " LEFT OUTER  JOIN OrderMaster OM on OM.GUID= A.OrderGUID LEFT OUTER  JOIN ItemMaster IM on IM.GUID= A.ItemGUID ";
                qry += " LEFT OUTER JOIN Room D on A.Room = D.ID LEFT OUTER  JOIN SupplierMaster SM on SM.ID= IM.SupplierID ";
                qry += " WHERE A.CompanyID =  " + CompanyId.ToString() + "  and OM.Room=" + RoomID + " and A.ReceivedQuantity < A.RequestedQuantity ";
                qry += " and Om.IsArchived!=1 and OM.IsDeleted!=1 and A.ItemGUID= '" + ItemGUID.ToString() + "' ";
                if (OrderStatus == 1) //For all open record
                {
                    qry += " and OM.OrderStatus in (0,1,2,3,4,5,6) ";
                }
                else if (OrderStatus == 1) //For all close record
                {
                    qry += " and OM.OrderStatus in (7) ";
                }
                else//For all close record
                {
                    qry += " and OM.OrderStatus in (0,1,2,3,4,5,6) ";
                }
                IEnumerable<ReceiveOrderLineItemDetailsDTO> obj = (from u in context.ExecuteStoreQuery<ReceiveOrderLineItemDetailsDTO>(qry)
                                                                   select new ReceiveOrderLineItemDetailsDTO
                                                                   {
                                                                       ID = u.ID,
                                                                       GUID = u.GUID,
                                                                       ItemGUID = u.ItemGUID,
                                                                       OrderGUID = u.OrderGUID,
                                                                       ItemNumber = u.ItemNumber,
                                                                       ReceivedQuantity = u.ReceivedQuantity,
                                                                       RequestedQuantity = u.RequestedQuantity,
                                                                       SupplierName = u.SupplierName,
                                                                       Supplier = u.Supplier,
                                                                       Description = u.Description,
                                                                       RequiredDate = u.RequiredDate,
                                                                       OrderNumber = u.OrderNumber,
                                                                       Room = u.Room,
                                                                       ItemType = u.ItemType,
                                                                   }).AsParallel().ToList();
                return obj;
            }
        }



    }
}
