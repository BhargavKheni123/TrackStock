using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Data.SqlClient;
using System.Web;

namespace eTurns.DAL
{
    public class KitMoveInOutDetailDALPart1
    {
        /// <summary>
        /// Get Paged Records from the KitMoveInOutDetail Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<KitMoveInOutDetailDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId)
        {
            return GetCachedData(RoomID, CompanyId).OrderBy("ID DESC");
        }

        /// <summary>
        /// Get Paged Records from the KitMoveInOutDetail Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<KitMoveInOutDetailDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            //Get Cached-Media
            IEnumerable<KitMoveInOutDetailDTO> ObjCache = GetCachedData(RoomID, CompanyID);
            IEnumerable<KitMoveInOutDetailDTO> ObjGlobalCache = ObjCache;
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
                //IEnumerable<KitMoveInOutDetailDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.Updated >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))

                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<KitMoveInOutDetailDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm)

                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm)

                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }


        /// <summary>
        /// Get Particullar Record from the KitMoveInOutDetail by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public KitMoveInOutDetailDTO GetRecord(string GUID, Int64 RoomID, Int64 CompanyID)
        {
            return GetCachedData(RoomID, CompanyID).Single(t => t.GUID == Guid.Parse(GUID));
        }

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KitMoveInOutDetailDTO> GetCachedData(Int64 RoomID, Int64 CompanyID)
        {
            IEnumerable<KitMoveInOutDetailDTO> ObjCache = CacheHelper<IEnumerable<KitMoveInOutDetailDTO>>.GetCacheItem("Cached_KitMoveInOutDetail_" + CompanyID.ToString());

            if (ObjCache == null)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<KitMoveInOutDetailDTO> obj = (from u in context.ExecuteStoreQuery<KitMoveInOutDetailDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM KitMoveInOutDetail A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.CompanyID = " + CompanyID.ToString())
                                                              select new KitMoveInOutDetailDTO
                                                              {
                                                                  ID = u.ID,
                                                                  KitDetailGUID = u.KitDetailGUID,
                                                                  ItemGUID = u.ItemGUID,
                                                                  BinID = u.BinID,
                                                                  MoveInOut = u.MoveInOut,
                                                                  CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                                  ConsignedQuantity = u.ConsignedQuantity,
                                                                  TotalQuantity = u.TotalQuantity,
                                                                  Created = u.Created,
                                                                  Updated = u.Updated,
                                                                  CreatedBy = u.CreatedBy,
                                                                  LastUpdatedBy = u.LastUpdatedBy,
                                                                  Room = u.Room,
                                                                  CompanyID = u.CompanyID,
                                                                  IsDeleted = u.IsDeleted,
                                                                  IsArchived = u.IsArchived,
                                                                  GUID = u.GUID,
                                                                  CreatedByName = u.CreatedByName,
                                                                  UpdatedByName = u.UpdatedByName,
                                                                  RoomName = u.RoomName,
                                                                  ReceivedOn = u.ReceivedOn,
                                                                  ReceivedOnWeb = u.ReceivedOnWeb,
                                                                  AddedFrom = u.AddedFrom,
                                                                  EditedFrom = u.EditedFrom,
                                                                  ExpirationDate = u.ExpirationDate,
                                                                  SerialNumber = u.SerialNumber,
                                                                  LotNumber = u.LotNumber,
                                                              }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<KitMoveInOutDetailDTO>>.AddCacheItem("Cached_KitMoveInOutDetail_" + CompanyID.ToString(), obj);
                }
            }

            return ObjCache.Where(x => x.Room == RoomID);
        }

        public bool DeleteRecords(string GUIDs, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                string[] arrGUIDs = GUIDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in arrGUIDs)
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE KitMoveInOutDetail SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1, ReceivedOn=GETUTCDATE(),EditedFrom='Web' WHERE GUID = '" + item + "';";
                    }
                }
                context.ExecuteStoreCommand(strQuery);

                return true;
            }
        }

    }
}
