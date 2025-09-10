using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public partial class ItemLocationQTYDAL : eTurnsBaseDAL
    {
        /// <summary>
        /// Get Paged Records from the ItemLocationQTY Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<ItemLocationQTYDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<ItemLocationQTYDTO>(@"
                        SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName , E.BinNumber
                        FROM ItemLocationQTY A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                        LEFT OUTER JOIN Room D on A.Room = D.ID 
                        left outer join BinMaster E on A.BinID = E.ID
                        LEFT OUTER JOIN ItemMaster I on A.ItemGUID = I.GUID
                        WHERE A.Room = " + RoomID.ToString() + " AND A.CompanyID = " + CompanyId.ToString()
                                         + @" AND E.IsDeleted = 0 AND E.IsArchived = 0"
                        )
                        select new ItemLocationQTYDTO
                        {
                            ID = u.ID,
                            BinID = u.BinID,
                            CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                            ConsignedQuantity = u.ConsignedQuantity,
                            Quantity = u.Quantity,
                            LotNumber = u.LotNumber,
                            GUID = u.GUID,
                            ItemGUID = u.ItemGUID,
                            Created = u.Created,
                            LastUpdated = u.LastUpdated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            CompanyID = u.CompanyID,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            BinNumber = u.BinNumber, // u.BinID > 0 ? lstBins.Where(x => x.ID == u.BinID).FirstOrDefault().BinNumber : "",
                            ItemNumber = u.ItemNumber,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            ReceivedOn = u.ReceivedOn,
                        }).AsParallel().ToList();
            }
        }

        /// <summary>
        /// Get Paged Records from the ItemLocationQTY Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<ItemLocationQTYDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            //Get Cached-Media
            IEnumerable<ItemLocationQTYDTO> ObjCache = GetAllRecords(RoomID, CompanyID);


            if (String.IsNullOrEmpty(SearchTerm))
            {

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<ItemLocationQTYDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.LastUpdated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.LastUpdated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<ItemLocationQTYDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm)).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm)).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }

        /// <summary>
        /// Get Particullar Record from the ItemLocationQTY by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public ItemLocationQTYDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            //return GetCachedData(RoomID, CompanyID).Single(t => t.ID == id);

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemLocationQTYDTO obj = (from u in context.ExecuteStoreQuery<ItemLocationQTYDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM ItemLocationQTY A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.ID = " + id.ToString() + " AND A.Room = " + RoomID.ToString() + " AND A.CompanyID = " + CompanyID.ToString())
                                          select new ItemLocationQTYDTO
                                          {
                                              ID = u.ID,

                                              BinID = u.BinID,
                                              CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                              ConsignedQuantity = u.ConsignedQuantity,
                                              Quantity = u.Quantity,
                                              LotNumber = u.LotNumber,
                                              GUID = u.GUID,
                                              ItemGUID = u.ItemGUID,
                                              Created = u.Created,
                                              LastUpdated = u.LastUpdated,
                                              CreatedBy = u.CreatedBy,
                                              LastUpdatedBy = u.LastUpdatedBy,
                                              Room = u.Room,
                                              CompanyID = u.CompanyID,
                                              CreatedByName = u.CreatedByName,
                                              UpdatedByName = u.UpdatedByName,
                                              RoomName = u.RoomName,
                                              AddedFrom = u.AddedFrom,
                                              EditedFrom = u.EditedFrom,
                                              ReceivedOnWeb = u.ReceivedOnWeb,
                                              ReceivedOn = u.ReceivedOn,
                                          }).SingleOrDefault();
                return obj;
            }
        }

        /// <summary>
        /// Get Particullar Record from the ItemLocationQTY by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public ItemLocationQTYDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<ItemLocationQTYDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM ItemLocationQTY_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + id.ToString())
                        select new ItemLocationQTYDTO
                        {
                            ID = u.ID,

                            BinID = u.BinID,
                            ConsignedQuantity = u.ConsignedQuantity,
                            CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                            Quantity = u.Quantity,
                            LotNumber = u.LotNumber,
                            GUID = u.GUID,
                            ItemGUID = u.ItemGUID,
                            Created = u.Created,
                            LastUpdated = u.LastUpdated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            CompanyID = u.CompanyID,
                        }).SingleOrDefault();
            }
        }

        /// <summary>
        /// Delete records and maintan the qunatity.....
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        public bool DeleteRecords(Guid ItemGUID, Int64 BinID, Int64 RoomID, Int64 CompanyID, long SessionUserId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                strQuery += "Delete from ItemLocationQTY WHERE Room =" + RoomID.ToString() + " AND CompanyID =" + CompanyID.ToString() + " AND ItemGUID ='" + ItemGUID.ToString() + "' AND BinID = " + BinID.ToString() + "; Delete from ItemLocationQTY where Room =" + RoomID.ToString() + " AND CompanyID =" + CompanyID.ToString() + " AND ItemGUID ='" + ItemGUID.ToString() + "' AND BINID Not in (SELECT BINID from ItemLocationDetails WHERE ItemGUID = '" + ItemGUID.ToString() + "' AND Room =" + RoomID.ToString() + " AND CompanyID =" + CompanyID.ToString() + ") ";

                if (context.ExecuteStoreCommand(strQuery) > 0)
                {
                    UpdateOnHandQuantity(ItemGUID, RoomID, CompanyID, SessionUserId);
                }
                return true;
            }
        }

        public List<ItemLocationQTYDTO> GetRecordByItem(Guid? ItemGUID, long? RoomID, long? CompanyID)
        {
            //return GetCachedData(RoomID, CompanyID).Where(t => t.ItemID == ItemID && t.BinID == BinID).SingleOrDefault();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<ItemLocationQTYDTO> obj = (from u in context.ExecuteStoreQuery<ItemLocationQTYDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM ItemLocationQTY A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.ItemGUID = '" + ItemGUID.ToString() + "' AND A.Room = " + RoomID.ToString() + " AND A.CompanyID = " + CompanyID.ToString())
                                                       select new ItemLocationQTYDTO
                                                       {
                                                           ID = u.ID,

                                                           BinID = u.BinID,
                                                           CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                           ConsignedQuantity = u.ConsignedQuantity,
                                                           Quantity = u.Quantity,
                                                           LotNumber = u.LotNumber,
                                                           GUID = u.GUID,
                                                           ItemGUID = u.ItemGUID,
                                                           Created = u.Created,
                                                           LastUpdated = u.LastUpdated,
                                                           CreatedBy = u.CreatedBy,
                                                           LastUpdatedBy = u.LastUpdatedBy,
                                                           Room = u.Room,
                                                           CompanyID = u.CompanyID,
                                                           CreatedByName = u.CreatedByName,
                                                           UpdatedByName = u.UpdatedByName,
                                                           RoomName = u.RoomName,
                                                           AddedFrom = u.AddedFrom,
                                                           EditedFrom = u.EditedFrom,
                                                           ReceivedOn = u.ReceivedOn,
                                                           ReceivedOnWeb = u.ReceivedOnWeb,
                                                       }).AsParallel().ToList();
                return obj.ToList();
            }
        }
    }
}
