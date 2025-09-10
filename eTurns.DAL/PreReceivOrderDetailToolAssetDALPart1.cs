using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data.SqlClient;


namespace eTurns.DAL
{
    public partial class PreReceivOrderDetailToolAssetDAL : eTurnsBaseDAL
    {
        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PreReceivOrderDetailDTO> GetAllRecordsByRoom(Int64 RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<PreReceivOrderDetailDTO> obj = (from u in context.ExecuteStoreQuery<PreReceivOrderDetailDTO>(@"
                                                            SELECT A.*,
                                                                    B.UserName AS CreatedByName,
                                                                    C.UserName AS UpdatedByName, 
                                                                    D.RoomName, 
                                                                    E.ItemNumber,E.Consignment,E.ItemType,E.SerialNumberTracking,E.LotNumberTracking,E.DateCodeTracking, I.BinNumber 
                                                            FROM PreReceivOrderDetail A LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID 
							                                                            LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                                                                        LEFT OUTER JOIN Room D on A.Room = D.ID 
							                                                            LEFT OUTER JOIN ItemMaster E on A.ItemGUID = E.GUID 
                                                                                        LEFT OUTER JOIN BinMaster I on A.BinID = I.ID  AND A.ItemGUID = I.ItemGUID 
                                                            WHERE A.Room = " + RoomID)
                                                            select new PreReceivOrderDetailDTO
                                                            {
                                                                ID = u.ID,
                                                                BinID = u.BinID,
                                                                Quantity = u.Quantity,
                                                                LotNumber = u.LotNumber,
                                                                SerialNumber = u.SerialNumber,
                                                                ExpirationDate = u.ExpirationDate,
                                                                ReceivedDate = u.ReceivedDate,
                                                                Cost = u.Cost,
                                                                GUID = u.GUID,
                                                                ItemGUID = u.ItemGUID,
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
                                                                BinNumber = u.BinNumber,
                                                                ItemNumber = u.ItemNumber,
                                                                SerialNumberTracking = u.SerialNumberTracking,
                                                                LotNumberTracking = u.LotNumberTracking,
                                                                DateCodeTracking = u.DateCodeTracking,
                                                                OrderDetailGUID = u.OrderDetailGUID,
                                                                PackSlipNumber = u.PackSlipNumber,
                                                                UDF1 = u.UDF1,
                                                                UDF2 = u.UDF2,
                                                                UDF3 = u.UDF3,
                                                                UDF4 = u.UDF4,
                                                                UDF5 = u.UDF5,
                                                                AddedFrom = u.AddedFrom,
                                                                EditedFrom = u.EditedFrom,
                                                                ReceivedOn = u.ReceivedOn,
                                                                ReceivedOnWeb = u.ReceivedOnWeb,
                                                                Consignment = u.Consignment,
                                                                IsReceived = u.IsReceived,
                                                                ItemType = u.ItemType,
                                                            }).AsParallel().ToList();


                return obj;
            }
        }
    }
}
