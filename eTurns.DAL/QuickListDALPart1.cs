using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using System.Data.SqlClient;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using eTurns.DTO.Resources;
using System.Web;

namespace eTurns.DAL
{
    public class QuickListDAL : eTurnsBaseDAL
    {
        public QuickListMasterDTO GetQuickListMasterByName(string Name, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            QuickListMasterDTO oQuickList;
            List<long> supplierIds = new List<long>();
            string sSQL = "(A.IsDeleted = " + (IsDeleted ? "1" : "0 OR A.IsDeleted IS NULL") + ") AND (A.IsArchived = " + (IsArchived ? "1" : "0 OR A.IsArchived IS NULL") + ") ";

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                oQuickList = (from u in context.ExecuteStoreQuery<QuickListMasterDTO>(@"SELECT A.*  ,ISNULL(B.UserName,'') AS 'CreatedByName'
		                                                                                              ,ISNULL(C.UserName,'') AS UpdatedByName
		                                                                                              ,ISNULL(D.RoomName,'') AS 'RoomName'
		                                                                                              --,(SELECT Count(ID) From QuickListItems E WHERE A.GUID= E.QuickListGUID) as NoOfItems
                                                                                           FROM QuickListMaster A 
                                                                                                LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID 
					                                                                            LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
					                                                                            LEFT OUTER JOIN Room D on A.Room = D.ID 
                                                                                           WHERE A.Name='" + Name + "' AND A.CompanyID = " + CompanyID.ToString() + " AND A.Room=" + RoomID.ToString() + " AND " + sSQL)
                              select new QuickListMasterDTO
                              {
                                  ID = u.ID,
                                  Name = u.Name,
                                  Comment = u.Comment,
                                  CompanyID = u.CompanyID,
                                  IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                  IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                  //NoOfItems = u.NoOfItems,
                                  Type = u.Type,
                                  Created = u.Created,
                                  LastUpdated = u.LastUpdated,
                                  CreatedByName = u.CreatedByName,
                                  UpdatedByName = u.UpdatedByName,
                                  RoomName = u.RoomName,
                                  CreatedBy = u.CreatedBy,
                                  LastUpdatedBy = u.LastUpdatedBy,
                                  Room = u.Room,
                                  GUID = u.GUID,
                                  UDF1 = u.UDF1,
                                  UDF2 = u.UDF2,
                                  UDF3 = u.UDF3,
                                  UDF4 = u.UDF4,
                                  UDF5 = u.UDF5,
                                  ReceivedOn = u.ReceivedOn,
                                  ReceivedOnWeb = u.ReceivedOnWeb,
                                  AddedFrom = u.AddedFrom,
                                  EditedFrom = u.EditedFrom,
                                  AppendedBarcodeString = string.Empty,
                                  NoOfItems = GetQuickListItemsRecords(RoomID, CompanyID, u.GUID.ToString(), supplierIds).Count(),
                              }).FirstOrDefault();   //.AsParallel().ToList().FirstOrDefault();
            }

            return oQuickList;
        }

        public List<QuickListDetailDTO> GetQuickListLineItemHistory(string QuickListMasterHistoryGUID)
        {
            //Get Cached-Media
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string qry = @"SELECT	 A.* ,B.UserName AS 'CreatedByName' ,C.UserName AS UpdatedByName ,D.RoomName 
                               FROM QuickListItems_History A LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID 
                                                             LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
					                                         LEFT OUTER JOIN Room D on A.Room = D.ID 
                                WHERE  A.QuickListGUID = '" + QuickListMasterHistoryGUID + "'";

                return (from u in context.ExecuteStoreQuery<QuickListDetailDTO>(qry)
                        select new QuickListDetailDTO
                        {
                            ID = u.ID,

                            Quantity = u.Quantity,
                            CompanyID = u.CompanyID,
                            IsArchived = u.IsArchived,
                            IsDeleted = u.IsDeleted,
                            Created = u.Created,
                            LastUpdated = u.LastUpdated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            GUID = u.GUID,
                            ItemGUID = u.ItemGUID,
                            QuickListGUID = u.QuickListGUID,
                            CreatedByName = u.CreatedByName,
                            RoomName = u.RoomName,
                            UpdatedByName = u.UpdatedByName,
                            Action = u.Action,
                            HistoryID = u.HistoryID,
                            IsHistory = true,
                            QuickListHistoryID = u.QuickListHistoryID,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            ItemDetail = new ItemMasterDAL(base.DataBaseName).GetItemWithMasterTableJoins(null, u.ItemGUID, u.Room.GetValueOrDefault(0), u.CompanyID.GetValueOrDefault(0)),
                            ConsignedQuantity = u.ConsignedQuantity
                        }).AsParallel().ToList();
            }

        }

        public QuickListMasterDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<QuickListMasterDTO>(@"SELECT	 A.* ,ISNULL(B.UserName,'') AS 'CreatedByName'
		                                                                                     ,ISNULL(C.UserName,'') AS UpdatedByName
		                                                                                     ,ISNULL(D.RoomName,'') 
                                                                             FROM QuickListMaster_History A LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID 
                                                                                                    LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
					                                                                                LEFT OUTER JOIN Room D on A.Room = D.ID 
                                                                             WHERE A.HistoryID=" + id.ToString())
                        select new QuickListMasterDTO
                        {
                            ID = u.ID,
                            Name = u.Name,
                            Comment = u.Comment,
                            CompanyID = u.CompanyID,
                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                            HistoryID = u.HistoryID,
                            Action = u.Action,
                            IsHistory = true,
                            Type = u.Type,
                            Created = u.Created,
                            LastUpdated = u.LastUpdated,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            GUID = u.GUID,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            //QuickListDetailList = GetQuickListLineItemHistory(u.HistoryID)
                        }).SingleOrDefault();
            }
        }

        public List<QuickListDetailDTO> GetQuickListItemsRequirePackslip(Int64 RoomID, Int64 CompanyID, string QuickListGUID, bool IsDelete, bool IsArchived)
        {
            List<QuickListDetailDTO> obj = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                obj = (from u in context.ExecuteStoreQuery<QuickListDetailDTO>(@"SELECT	 A.* 
                                                                                         ,B.UserName AS 'CreatedByName'
		                                                                                 ,C.UserName AS UpdatedByName
		                                                                                 ,D.RoomName 
                                                                                        FROM QuickListItems A INNER JOIN Itemmaster IM on A.ItemGUID = IM.GUID
                                                                                                            LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID 
					                                                                                        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
					                                                                                        LEFT OUTER JOIN Room D on A.Room = D.ID 
                                                                                    WHERE IsNull(IM.IsPackslipMandatoryAtReceive,0)=1 AND A.IsDeleted = 0 AND A.IsArchived = 0  AND QuickListGUID= '" + QuickListGUID + @"'  AND A.CompanyID = " + CompanyID + @" AND A.Room = " + RoomID)
                       select new QuickListDetailDTO
                       {
                           ID = u.ID,
                           QuickListGUID = u.QuickListGUID,
                           CompanyID = u.CompanyID,
                           IsArchived = u.IsArchived,
                           IsDeleted = u.IsDeleted,
                           Room = u.Room,
                           GUID = u.GUID,
                           ItemGUID = u.ItemGUID,
                           RoomName = u.RoomName,
                           ReceivedOn = u.ReceivedOn,
                           ReceivedOnWeb = u.ReceivedOnWeb,
                           AddedFrom = u.AddedFrom,
                           EditedFrom = u.EditedFrom,
                       }).AsParallel().ToList();
            }
            return obj;
        }
    }
}
