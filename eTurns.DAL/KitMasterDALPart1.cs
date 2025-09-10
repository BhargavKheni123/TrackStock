using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using eTurns.DTO.Resources;
using System.Web;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public class KitMasterDAL : eTurnsBaseDAL
    {
        public IEnumerable<KitMasterDTO> GetCachedData_OldKit(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            // Get Cached-Media
            IEnumerable<KitMasterDTO> ObjCache = null;
            KitDetailDAL objKitDetailDTO = new KitDetailDAL(base.DataBaseName);
            if (IsArchived == false && IsDeleted == false)
            {
                ObjCache = CacheHelper<IEnumerable<KitMasterDTO>>.GetCacheItem("Cached_KitMaster_" + CompanyID.ToString());

                if (ObjCache == null || ObjCache.Count() <= 0)
                {
                    BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(base.DataBaseName);
                    IEnumerable<BarcodeMasterDTO> lstBarcodeDTO = objBarcodeMasterDAL.GetAllRecords(RoomID, CompanyID);

                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        IEnumerable<KitMasterDTO> obj = (from u in context.ExecuteStoreQuery<KitMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM KitMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString())
                                                         select new KitMasterDTO
                                                         {
                                                             ID = u.ID,
                                                             KitPartNumber = u.KitPartNumber,
                                                             Description = u.Description,
                                                             ReOrderType = u.ReOrderType,
                                                             KitCategory = u.KitCategory,
                                                             AvailableKitQuantity = u.AvailableKitQuantity.GetValueOrDefault(0),
                                                             AvailableWIPKit = u.AvailableWIPKit.GetValueOrDefault(0),
                                                             KitDemand = u.KitDemand.GetValueOrDefault(0),
                                                             KitCost = u.KitCost,
                                                             KitSellPrice = u.KitSellPrice,
                                                             MinimumKitQuantity = u.MinimumKitQuantity.GetValueOrDefault(0),
                                                             MaximumKitQuantity = u.MaximumKitQuantity.GetValueOrDefault(0),
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
                                                             IsNotAbleToDelete = false,
                                                             CriticalQuantity = u.CriticalQuantity,
                                                             ReceivedOn = u.ReceivedOn,
                                                             ReceivedOnWeb = u.ReceivedOnWeb,
                                                             AddedFrom = u.AddedFrom,
                                                             EditedFrom = u.EditedFrom,
                                                             AppendedBarcodeString = objBarcodeMasterDAL.GetComaSaperatedBarcodes(lstBarcodeDTO, u.GUID, "Kits")
                                                         }).AsParallel().ToList();
                        ObjCache = CacheHelper<IEnumerable<KitMasterDTO>>.AddCacheItem("Cached_KitMaster_" + CompanyID.ToString(), obj);
                    }
                }

                ObjCache = ObjCache.Where(x => x.Room == RoomID);
                IEnumerable<KitDetailDTO> lstKitDetailDTO = objKitDetailDTO.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted, false);
                foreach (var item in ObjCache)
                {
                    item.KitItemList = lstKitDetailDTO.Where(x => x.KitGUID == item.GUID).ToList();
                    item.NoOfItemsInKit = item.KitItemList.Count;

                    if (item.AvailableKitQuantity.GetValueOrDefault(0) > 0 || item.AvailableWIPKit.GetValueOrDefault(0) > 0)
                    {
                        item.IsNotAbleToDelete = true;
                    }
                    else if (item.KitItemList != null && item.KitItemList.Count > 0)
                    {
                        item.IsNotAbleToDelete = item.KitItemList.FindIndex(x => x.AvailableItemsInWIP.GetValueOrDefault(0) > 0) >= 0;
                    }
                }
            }
            else
            {
                BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(base.DataBaseName);
                IEnumerable<BarcodeMasterDTO> lstBarcodeDTO = objBarcodeMasterDAL.GetAllRecords(RoomID, CompanyID);

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
                    ObjCache = (from u in context.ExecuteStoreQuery<KitMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM KitMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.CompanyID = " + CompanyID.ToString() + @" And A.Room = " + RoomID.ToString() + " AND " + sSQL)
                                select new KitMasterDTO
                                {
                                    ID = u.ID,
                                    KitPartNumber = u.KitPartNumber,
                                    Description = u.Description,
                                    ReOrderType = u.ReOrderType,
                                    KitCategory = u.KitCategory,
                                    AvailableKitQuantity = u.AvailableKitQuantity.GetValueOrDefault(0),
                                    AvailableWIPKit = u.AvailableWIPKit.GetValueOrDefault(0),
                                    KitDemand = (5 - u.KitDemand.GetValueOrDefault(0)),
                                    KitCost = u.KitCost,
                                    KitSellPrice = u.KitSellPrice,
                                    MinimumKitQuantity = u.MinimumKitQuantity.GetValueOrDefault(0),
                                    MaximumKitQuantity = u.MaximumKitQuantity.GetValueOrDefault(0),
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
                                    IsNotAbleToDelete = true,
                                    CriticalQuantity = u.CriticalQuantity,
                                    ReceivedOn = u.ReceivedOn,
                                    ReceivedOnWeb = u.ReceivedOnWeb,
                                    AddedFrom = u.AddedFrom,
                                    EditedFrom = u.EditedFrom,
                                    AppendedBarcodeString = objBarcodeMasterDAL.GetComaSaperatedBarcodes(lstBarcodeDTO, u.GUID, "Kits")
                                }).AsParallel().ToList();
                }
            }


            return ObjCache;
        }

        public bool DeleteRecords(string GUIDs, Int64 userid, Int64 RoomID, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                string[] arrGuids = GUIDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in arrGuids)
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        KitDetailDAL kitDetailDAL = new KitDetailDAL(base.DataBaseName);
                        IEnumerable<KitDetailDTO> lstKitDetailDTO = kitDetailDAL.GetAllRecords(RoomID, CompanyID, false, false, false).Where(x => x.KitGUID == Guid.Parse(item));
                        if (lstKitDetailDTO != null && lstKitDetailDTO.Count() > 0)
                        {
                            foreach (var KDDTO in lstKitDetailDTO)
                            {
                                kitDetailDAL.DeleteRecords(KDDTO.GUID.ToString(), userid, CompanyID);
                            }
                        }
                        strQuery += "UPDATE KitMaster SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1, ReceivedOn=GETUTCDATE(),EditedFrom='Web' WHERE GUID = '" + item.ToString() + "';";
                    }
                }
                context.ExecuteStoreCommand(strQuery);


                //Get Cached-Media
                IEnumerable<KitMasterDTO> ObjCache = CacheHelper<IEnumerable<KitMasterDTO>>.GetCacheItem("Cached_KitMaster_" + CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<KitMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => arrGuids.Contains(i.GUID.ToString()));
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<KitMasterDTO>>.AppendToCacheItem("Cached_KitMaster_" + CompanyID.ToString(), ObjCache);
                }

                return true;
            }
        }

        public KitMasterDTO GetHistoryRecord(Int64 HistoryID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<KitMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM KitMaster_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + HistoryID)
                        select new KitMasterDTO
                        {
                            ID = u.ID,
                            KitPartNumber = u.KitPartNumber,
                            Description = u.Description,
                            ReOrderType = u.ReOrderType,
                            KitCategory = u.KitCategory,
                            AvailableKitQuantity = u.AvailableKitQuantity,
                            AvailableWIPKit = u.AvailableWIPKit,
                            KitDemand = u.KitDemand,
                            KitCost = u.KitCost,
                            KitSellPrice = u.KitSellPrice,
                            MinimumKitQuantity = u.MinimumKitQuantity,
                            MaximumKitQuantity = u.MaximumKitQuantity,
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
                            HistoryID = u.HistoryID,
                            Action = u.Action,
                            CriticalQuantity = u.CriticalQuantity,
                            UpdatedByName = u.UpdatedByName,
                            CreatedByName = u.CreatedByName,
                            RoomName = u.RoomName,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                        }).SingleOrDefault();
            }
        }
    }
}
