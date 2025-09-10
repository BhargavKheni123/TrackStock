using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public class KitDetailDAL : eTurnsBaseDAL
    {
        string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];

        #region [Class Constructor]

        public KitDetailDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public KitDetailDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion
        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KitDetailDTO> GetKitDetail(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool NeedItemDetail, string KitGUID, string ItemGUID)
        {
            IEnumerable<KitDetailDTO> lstKitDetail = null;
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            if (IsArchived == false && IsDeleted == false)
            {
                lstKitDetail = null;
                if (lstKitDetail == null || lstKitDetail.Count() <= 0)
                {
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                                           new SqlParameter("@CompanyID", CompanyID),
                                                           new SqlParameter("@KitGUID", KitGUID),
                                                           new SqlParameter("@ItemGUID", ItemGUID) };

                        lstKitDetail = (from u in context.Database.SqlQuery<KitDetailDTO>("exec [GetKitDetail] @RoomID,@CompanyID,@KitGUID,@ItemGUID", params1)
                                        select new KitDetailDTO
                                        {
                                            ID = u.ID,
                                            KitGUID = u.KitGUID,
                                            ItemGUID = u.ItemGUID,
                                            QuantityPerKit = u.QuantityPerKit.GetValueOrDefault(0),
                                            QuantityReadyForAssembly = u.QuantityReadyForAssembly.GetValueOrDefault(0),
                                            AvailableItemsInWIP = u.AvailableItemsInWIP.GetValueOrDefault(0),
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
                                            ReceivedOn = u.ReceivedOn,
                                            ReceivedOnWeb = u.ReceivedOnWeb,
                                            AddedFrom = u.AddedFrom,
                                            EditedFrom = u.EditedFrom,
                                            QtyToMeetDemand = GetQtyOnDemand(Convert.ToString(u.KitGUID), u.QuantityPerKit.GetValueOrDefault(0), u.AvailableItemsInWIP.GetValueOrDefault(0), u.Room, u.CompanyID)
                                        }).AsParallel().ToList();
                    }
                }

                if (NeedItemDetail)
                {
                    IList<ItemMasterDTO> ObjItemCache = new ItemMasterDAL(base.DataBaseName).GetAllItemsWithJoins(RoomID, CompanyID, false, false, null).ToList();
                    lstKitDetail = (from u in lstKitDetail
                                    join ica in ObjItemCache on u.ItemGUID equals ica.GUID into ica_u_join
                                    from ica_u in ica_u_join.DefaultIfEmpty()
                                    select new KitDetailDTO
                                    {
                                        ID = u.ID,
                                        KitGUID = u.KitGUID,
                                        ItemGUID = u.ItemGUID,
                                        QuantityPerKit = u.QuantityPerKit.GetValueOrDefault(0),
                                        QuantityReadyForAssembly = u.QuantityReadyForAssembly.GetValueOrDefault(0),
                                        AvailableItemsInWIP = u.AvailableItemsInWIP.GetValueOrDefault(0),
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
                                        ItemNumber = ica_u.ItemNumber,
                                        ItemType = ica_u.ItemType,
                                        ItemDetail = ica_u,
                                        ReceivedOn = u.ReceivedOn,
                                        ReceivedOnWeb = u.ReceivedOnWeb,
                                        AddedFrom = u.AddedFrom,
                                        EditedFrom = u.EditedFrom,
                                        QtyToMeetDemand = GetQtyOnDemand(Convert.ToString(u.KitGUID), u.QuantityPerKit.GetValueOrDefault(0), u.AvailableItemsInWIP.GetValueOrDefault(0), u.Room, u.CompanyID),
                                    }).AsParallel().ToList();
                }
            }
            else
            {
                IEnumerable<ItemMasterDTO> objItemDTO = objItemDAL.GetAllItemsPlain(RoomID, CompanyID);

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                                       new SqlParameter("@CompanyID", CompanyID),
                                                       new SqlParameter("@KitGUID", KitGUID),
                                                       new SqlParameter("@ItemGUID", ItemGUID),
                                                       new SqlParameter("@IsDeleted", IsDeleted),
                                                       new SqlParameter("@IsArchived", IsArchived) };

                    IEnumerable<KitDetailDTO> obj = (from u in context.Database.SqlQuery<KitDetailDTO>("exec [GetKitDetailByKitItemGUID] @RoomID,@CompanyID,@KitGUID,@ItemGUID,@IsDeleted,@IsArchived", params1)
                                                     select new KitDetailDTO
                                                     {
                                                         ID = u.ID,
                                                         KitGUID = u.KitGUID,
                                                         ItemGUID = u.ItemGUID,
                                                         QuantityPerKit = u.QuantityPerKit.GetValueOrDefault(0),
                                                         QuantityReadyForAssembly = u.QuantityReadyForAssembly.GetValueOrDefault(0),
                                                         AvailableItemsInWIP = u.AvailableItemsInWIP.GetValueOrDefault(0),
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
                                                         ReceivedOn = u.ReceivedOn,
                                                         ReceivedOnWeb = u.ReceivedOnWeb,
                                                         AddedFrom = u.AddedFrom,
                                                         EditedFrom = u.EditedFrom,
                                                         ItemDetail = objItemDTO.Where(x => x.GUID == u.ItemGUID).SingleOrDefault(),
                                                         QtyToMeetDemand = GetQtyOnDemand(Convert.ToString(u.KitGUID), u.QuantityPerKit.GetValueOrDefault(0), u.AvailableItemsInWIP.GetValueOrDefault(0), u.Room, u.CompanyID)
                                                     }).AsParallel().ToList();
                    return obj;
                }

            }
            return lstKitDetail;
        }

        public IEnumerable<KitDetailDTO> GetKitDetailNew(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool NeedItemDetail)
        {
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            IEnumerable<KitDetailDTO> lstKitDetail = null;
            ItemMasterDAL objobjItemDAL = new ItemMasterDAL(base.DataBaseName);
            if (IsArchived == false && IsDeleted == false)
            {
                lstKitDetail = null;
                if (lstKitDetail == null || lstKitDetail.Count() <= 0)
                {
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                                           new SqlParameter("@CompanyID", CompanyID) };

                        lstKitDetail = (from u in context.Database.SqlQuery<KitDetailDTO>("exec [GetKitDetailByRoomIDCompanyID] @RoomID,@CompanyID", params1)
                                        select new KitDetailDTO
                                        {
                                            ID = u.ID,
                                            KitGUID = u.KitGUID,
                                            ItemGUID = u.ItemGUID,
                                            QuantityPerKit = u.QuantityPerKit.GetValueOrDefault(0),
                                            QuantityReadyForAssembly = u.QuantityReadyForAssembly.GetValueOrDefault(0),
                                            AvailableItemsInWIP = u.AvailableItemsInWIP.GetValueOrDefault(0),
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
                                            ReceivedOn = u.ReceivedOn,
                                            ReceivedOnWeb = u.ReceivedOnWeb,
                                            AddedFrom = u.AddedFrom,
                                            EditedFrom = u.EditedFrom,
                                            QtyToMeetDemand = GetQtyOnDemand(Convert.ToString(u.KitGUID), u.QuantityPerKit.GetValueOrDefault(0), u.AvailableItemsInWIP.GetValueOrDefault(0), u.Room, u.CompanyID)
                                        }).AsParallel().ToList();
                    }
                }


                if (NeedItemDetail)
                {
                    IList<ItemMasterDTO> ObjItemCache = new ItemMasterDAL(base.DataBaseName).GetAllItemsWithJoins(RoomID, CompanyID, false, false, null).ToList();
                    if (ObjItemCache != null && ObjItemCache.Count > 0)
                    {
                        lstKitDetail = (from u in lstKitDetail
                                        join ica in ObjItemCache on u.ItemGUID equals ica.GUID into ica_u_join
                                        from ica_u in ica_u_join.DefaultIfEmpty()
                                        select new KitDetailDTO
                                        {
                                            ID = u.ID,
                                            KitGUID = u.KitGUID,
                                            ItemGUID = u.ItemGUID,
                                            QuantityPerKit = u.QuantityPerKit.GetValueOrDefault(0),
                                            QuantityReadyForAssembly = u.QuantityReadyForAssembly.GetValueOrDefault(0),
                                            AvailableItemsInWIP = u.AvailableItemsInWIP.GetValueOrDefault(0),
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
                                            ItemNumber = ica_u.ItemNumber,
                                            ItemType = ica_u.ItemType,
                                            ItemDetail = ica_u,
                                            ReceivedOn = u.ReceivedOn,
                                            ReceivedOnWeb = u.ReceivedOnWeb,
                                            AddedFrom = u.AddedFrom,
                                            EditedFrom = u.EditedFrom,
                                            QtyToMeetDemand = GetQtyOnDemand(Convert.ToString(u.KitGUID), u.QuantityPerKit.GetValueOrDefault(0), u.AvailableItemsInWIP.GetValueOrDefault(0), u.Room, u.CompanyID),
                                        }).AsParallel().ToList();
                    }
                }
            }
            else
            {
                IEnumerable<ItemMasterDTO> objItemDTO = objItemDAL.GetAllItemsPlain(RoomID, CompanyID);

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                                       new SqlParameter("@CompanyID", CompanyID),
                                                       new SqlParameter("@KitGUID", (object)DBNull.Value),
                                                       new SqlParameter("@ItemGUID", (object)DBNull.Value),
                                                       new SqlParameter("@IsDeleted", IsDeleted),
                                                       new SqlParameter("@IsArchived", IsArchived) };

                    IEnumerable<KitDetailDTO> obj = (from u in context.Database.SqlQuery<KitDetailDTO>("exec [GetKitDetailByKitItemGUID] @RoomID,@CompanyID,@KitGUID,@ItemGUID,@IsDeleted,@IsArchived", params1)
                                                     select new KitDetailDTO
                                                     {
                                                         ID = u.ID,
                                                         KitGUID = u.KitGUID,
                                                         ItemGUID = u.ItemGUID,
                                                         QuantityPerKit = u.QuantityPerKit.GetValueOrDefault(0),
                                                         QuantityReadyForAssembly = u.QuantityReadyForAssembly.GetValueOrDefault(0),
                                                         AvailableItemsInWIP = u.AvailableItemsInWIP.GetValueOrDefault(0),
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
                                                         ReceivedOn = u.ReceivedOn,
                                                         ReceivedOnWeb = u.ReceivedOnWeb,
                                                         AddedFrom = u.AddedFrom,
                                                         EditedFrom = u.EditedFrom,
                                                         ItemDetail = objItemDTO.Where(x => x.GUID == u.ItemGUID).SingleOrDefault(),
                                                         QtyToMeetDemand = GetQtyOnDemand(Convert.ToString(u.KitGUID), u.QuantityPerKit.GetValueOrDefault(0), u.AvailableItemsInWIP.GetValueOrDefault(0), u.Room, u.CompanyID)
                                                     }).AsParallel().ToList();
                    return obj;
                }

            }
            return lstKitDetail;
        }

        public IEnumerable<KitDetailDTO> GetCachedDataNewWithItemGuid(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool NeedItemDetail, Guid ItemGuid)
        {
            IEnumerable<KitDetailDTO> lstKitDetail = null;
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);

            if (IsArchived == false && IsDeleted == false)
            {
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                                           new SqlParameter("@CompanyID", CompanyID),
                                                           new SqlParameter("@ItemGUID", Convert.ToString(ItemGuid)) };

                        lstKitDetail = (from u in context.Database.SqlQuery<KitDetailDTO>("exec [GetKitDetailByItemGUID] @RoomID,@CompanyID,@ItemGUID", params1)
                                        select new KitDetailDTO
                                        {
                                            ID = u.ID,
                                            KitGUID = u.KitGUID,
                                            ItemGUID = u.ItemGUID,
                                            QuantityPerKit = u.QuantityPerKit.GetValueOrDefault(0),
                                            QuantityReadyForAssembly = u.QuantityReadyForAssembly.GetValueOrDefault(0),
                                            AvailableItemsInWIP = u.AvailableItemsInWIP.GetValueOrDefault(0),
                                            Created = u.Created,
                                            LastUpdated = u.LastUpdated,
                                            CreatedBy = u.CreatedBy,
                                            LastUpdatedBy = u.LastUpdatedBy,
                                            Room = u.Room,
                                            IsDeleted = u.IsDeleted,
                                            IsArchived = u.IsArchived,
                                            CompanyID = u.CompanyID,
                                            GUID = u.GUID,
                                            //CreatedByName = u.CreatedByName,
                                            //UpdatedByName = u.UpdatedByName,
                                            //RoomName = u.RoomName,
                                            ReceivedOn = u.ReceivedOn,
                                            ReceivedOnWeb = u.ReceivedOnWeb,
                                            AddedFrom = u.AddedFrom,
                                            EditedFrom = u.EditedFrom,
                                            QtyToMeetDemand = GetQtyOnDemand(Convert.ToString(u.KitGUID), u.QuantityPerKit.GetValueOrDefault(0), u.AvailableItemsInWIP.GetValueOrDefault(0), u.Room, u.CompanyID)
                                        }).AsParallel().ToList();
                    }
                

                //if (NeedItemDetail)
                //{
                //    IList<ItemMasterDTO> ObjItemCache = new ItemMasterDAL(base.DataBaseName).GetAllItemsWithJoins(RoomID, CompanyID, false, false, null).ToList();
                //    lstKitDetail = (from u in lstKitDetail
                //                    join ica in ObjItemCache on u.ItemGUID equals ica.GUID into ica_u_join
                //                    from ica_u in ica_u_join.DefaultIfEmpty()
                //                    select new KitDetailDTO
                //                    {
                //                        ID = u.ID,
                //                        KitGUID = u.KitGUID,
                //                        ItemGUID = u.ItemGUID,
                //                        QuantityPerKit = u.QuantityPerKit.GetValueOrDefault(0),
                //                        QuantityReadyForAssembly = u.QuantityReadyForAssembly.GetValueOrDefault(0),
                //                        AvailableItemsInWIP = u.AvailableItemsInWIP.GetValueOrDefault(0),
                //                        Created = u.Created,
                //                        LastUpdated = u.LastUpdated,
                //                        CreatedBy = u.CreatedBy,
                //                        LastUpdatedBy = u.LastUpdatedBy,
                //                        Room = u.Room,
                //                        IsDeleted = u.IsDeleted,
                //                        IsArchived = u.IsArchived,
                //                        CompanyID = u.CompanyID,
                //                        GUID = u.GUID,
                //                        CreatedByName = u.CreatedByName,
                //                        UpdatedByName = u.UpdatedByName,
                //                        RoomName = u.RoomName,
                //                        ItemNumber = ica_u.ItemNumber,
                //                        ItemType = ica_u.ItemType,
                //                        ItemDetail = ica_u,
                //                        ReceivedOn = u.ReceivedOn,
                //                        ReceivedOnWeb = u.ReceivedOnWeb,
                //                        AddedFrom = u.AddedFrom,
                //                        EditedFrom = u.EditedFrom,
                //                        QtyToMeetDemand = GetQtyOnDemand(Convert.ToString(u.KitGUID), u.QuantityPerKit.GetValueOrDefault(0), u.AvailableItemsInWIP.GetValueOrDefault(0), u.Room, u.CompanyID),
                //                    }).AsParallel().ToList();
                //}
            }
            else
            {
                IEnumerable<ItemMasterDTO> objItemDTO = objItemDAL.GetAllItemsPlain(RoomID, CompanyID);

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                                       new SqlParameter("@CompanyID", CompanyID),
                                                       new SqlParameter("@KitGUID", (object)DBNull.Value),
                                                       new SqlParameter("@ItemGUID", Convert.ToString(ItemGuid)),
                                                       new SqlParameter("@IsDeleted", IsDeleted),
                                                       new SqlParameter("@IsArchived", IsArchived) };

                    IEnumerable<KitDetailDTO> obj = (from u in context.Database.SqlQuery<KitDetailDTO>("exec [GetKitDetailByKitItemGUID] @RoomID,@CompanyID,@KitGUID,@ItemGUID,@IsDeleted,@IsArchived", params1)
                                                     select new KitDetailDTO
                                                     {
                                                         ID = u.ID,
                                                         KitGUID = u.KitGUID,
                                                         ItemGUID = u.ItemGUID,
                                                         QuantityPerKit = u.QuantityPerKit.GetValueOrDefault(0),
                                                         QuantityReadyForAssembly = u.QuantityReadyForAssembly.GetValueOrDefault(0),
                                                         AvailableItemsInWIP = u.AvailableItemsInWIP.GetValueOrDefault(0),
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
                                                         ReceivedOn = u.ReceivedOn,
                                                         ReceivedOnWeb = u.ReceivedOnWeb,
                                                         AddedFrom = u.AddedFrom,
                                                         EditedFrom = u.EditedFrom,
                                                         ItemDetail = objItemDTO.Where(x => x.GUID == u.ItemGUID).SingleOrDefault(),
                                                         QtyToMeetDemand = GetQtyOnDemand(Convert.ToString(u.KitGUID), u.QuantityPerKit.GetValueOrDefault(0), u.AvailableItemsInWIP.GetValueOrDefault(0), u.Room, u.CompanyID)
                                                     }).AsParallel().ToList();
                    return obj;
                }

            }
            return lstKitDetail;
        }

        /// <summary>
        /// Get Paged Records from the KitDetail Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<KitDetailDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, bool IsNeededItemDetail)
        {
            return DB_GetKitRecords(null, null, null, RoomID, CompanyId);
            //return GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted, IsNeededItemDetail).OrderBy("ID DESC");
        }

        /// <summary>
        /// Get Paged Records from the KitDetail Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<KitDetailDTO> GetAllRecordsByKitGUID(Guid KitGUID, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, bool IsNeededItemDetail)
        {
            return DB_GetKitRecords(KitGUID, null, null, RoomID, CompanyId);
            //return GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted, IsNeededItemDetail).OrderBy("ID DESC").Where(x => x.KitGUID == KitGUID);
        }

        /// <summary>
        /// Get Particullar Record from the KitDetail by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public KitDetailDTO GetRecord(string GUID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool IsNeededItemDetail)
        {
            return DB_GetKitRecords(null, null, Guid.Parse(GUID), RoomID, CompanyID).FirstOrDefault();
            //return GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted, IsNeededItemDetail).SingleOrDefault(t => t.GUID == Guid.Parse(GUID));
        }

        public KitDetailDTO GetKitDetailByGuidNormal(Guid GUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var param = new SqlParameter[] { new SqlParameter("@GUID", GUID) };
                return context.Database.SqlQuery<KitDetailDTO>("exec [GetKitDetailByGuidNormal] @GUID", param).FirstOrDefault();
            }
        }

        public List<KitDetailDTO> GetKitDetailByKitGUIDNormal(Guid KitGUID, long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var param = new SqlParameter[] { new SqlParameter("@KitGUID", KitGUID), new SqlParameter("@RoomID", RoomId), new SqlParameter("@CompanyID", CompanyId) };
                return context.Database.SqlQuery<KitDetailDTO>("exec [GetKitDetailByKitGUIDNormal] @KitGUID,@RoomID,@CompanyID", param).ToList();
            }
        }

        /// <summary>
        /// Get Particullar Record from the KitDetail by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public KitDetailDTO GetHistoryRecord(Int64 HistoryID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //return (from u in context.Database.SqlQuery<KitDetailDTO>(@"
                //                                                        SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName 
                //                                                        FROM KitDetail_History A 
                //                                                        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                //                                                        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                //                                                        LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + HistoryID)
                //
                var paramskts = new SqlParameter[] { new SqlParameter("@HistoryID", HistoryID), new SqlParameter("@dbName", DataBaseName) };
                return (from u in context.Database.SqlQuery<KitDetailDTO>("exec [GetKitDetailChangeLogByHistoryID] @HistoryID,@dbName", paramskts)
                        select new KitDetailDTO
                        {
                            ID = u.ID,
                            KitGUID = u.KitGUID,
                            ItemGUID = u.ItemGUID,
                            QuantityPerKit = u.QuantityPerKit,
                            QuantityReadyForAssembly = u.QuantityReadyForAssembly,
                            AvailableItemsInWIP = u.AvailableItemsInWIP,
                            Created = u.Created,
                            LastUpdated = u.LastUpdated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            GUID = u.GUID,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            QtyToMeetDemand = GetQtyOnDemand(Convert.ToString(u.KitGUID), u.QuantityPerKit.GetValueOrDefault(0), u.AvailableItemsInWIP.GetValueOrDefault(0), u.Room, u.CompanyID)
                        }).SingleOrDefault();
            }
        }

        /// <summary>
        /// Insert Record in the DataBase KitDetail
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(KitDetailDTO objDTO, long SessionUserId,long EnterpriseId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objDTO.Created = DateTimeUtility.DateTimeNow;
                objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                objDTO.IsDeleted = false;
                objDTO.IsArchived = false;

                KitDetail obj = new KitDetail();
                obj.ID = 0;
                obj.KitGUID = objDTO.KitGUID;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.QuantityPerKit = objDTO.QuantityPerKit;
                obj.QuantityReadyForAssembly = objDTO.QuantityReadyForAssembly;
                obj.AvailableItemsInWIP = objDTO.AvailableItemsInWIP;
                obj.Created = objDTO.Created;
                obj.LastUpdated = objDTO.LastUpdated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.CompanyID = objDTO.CompanyID;
                obj.GUID = Guid.NewGuid();
                obj.ReceivedOn = objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                obj.AddedFrom = objDTO.AddedFrom = "Web";
                obj.EditedFrom = objDTO.EditedFrom = "Web";
                if (objDTO.Room > 0)
                {
                    obj.QtyToMeetDemand = GetQtyOnDemand(Convert.ToString(objDTO.KitGUID), objDTO.QuantityPerKit.GetValueOrDefault(0), objDTO.AvailableItemsInWIP.GetValueOrDefault(0), objDTO.Room, objDTO.CompanyID);
                }
                else
                {
                    obj.QtyToMeetDemand = 0;
                }
                context.KitDetails.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;



                if (objDTO.ID > 0)
                {
                    //Get Cached-Media
                    //IEnumerable<KitDetailDTO> ObjCache = CacheHelper<IEnumerable<KitDetailDTO>>.GetCacheItem("Cached_KitDetail_" + objDTO.CompanyID.ToString());
                    //if (ObjCache != null)
                    //{
                    //    List<KitDetailDTO> tempC = new List<KitDetailDTO>();
                    //    tempC.Add(objDTO);

                    //    IEnumerable<KitDetailDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    //    CacheHelper<IEnumerable<KitDetailDTO>>.AppendToCacheItem("Cached_KitDetail_" + objDTO.CompanyID.ToString(), NewCache);
                    //}

                    if (objDTO.Room > 0)
                    {
                        // Added for WI-5267
                        var kit = context.ItemMasters.SingleOrDefault(x => x.GUID == objDTO.KitGUID);

                        if (kit != null && kit.IsBuildBreak.GetValueOrDefault(false) == true)
                        {
                            GetSumOfQtyOnDemandForItem(Convert.ToString(objDTO.ItemGUID), objDTO.Room, objDTO.CompanyID, SessionUserId,EnterpriseId);
                        }
                    }

                }

                return obj.ID;
            }

        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(KitDetailDTO objDTO, long SessionUserId,long EnterpriseId)
        {
            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                KitDetail obj = context.KitDetails.SingleOrDefault(x => x.GUID == objDTO.GUID);
                obj.ID = objDTO.ID;
                obj.QuantityPerKit = objDTO.QuantityPerKit;
                obj.LastUpdated = objDTO.LastUpdated;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                //if (objDTO.AvailableItemsInWIP.GetValueOrDefault(0) > 0)
                obj.AvailableItemsInWIP = objDTO.AvailableItemsInWIP;

                if (obj.QuantityPerKit.GetValueOrDefault(0) > 0)
                {
                    obj.QuantityReadyForAssembly = Math.Floor(obj.AvailableItemsInWIP.GetValueOrDefault(0) / obj.QuantityPerKit.GetValueOrDefault(0));
                }


                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.EditedFrom = objDTO.EditedFrom;


                double QtyValue = 0;
                if (objDTO.Room > 0)
                {
                    QtyValue = this.GetQtyOnDemand(objDTO.KitGUID.ToString(), obj.QuantityPerKit.GetValueOrDefault(0), obj.AvailableItemsInWIP.GetValueOrDefault(0), objDTO.Room, objDTO.CompanyID);
                }

                obj.QtyToMeetDemand = QtyValue;


                //context.Entry(obj).State = System.Data.Entity.EntityState.Unchanged;
                //context.KitDetails.Attach(obj);
                //context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                if (objDTO.Room > 0)
                {
                    // Added for WI-5267
                    var kit = context.ItemMasters.SingleOrDefault(x => x.GUID == objDTO.KitGUID);

                    if (kit != null && kit.IsBuildBreak.GetValueOrDefault(false) == true)
                    {
                        GetSumOfQtyOnDemandForItem(Convert.ToString(objDTO.ItemGUID), objDTO.Room, objDTO.CompanyID, SessionUserId,EnterpriseId);
                    }
                }

                //Get Cached-Media
                //IEnumerable<KitDetailDTO> ObjCache = CacheHelper<IEnumerable<KitDetailDTO>>.GetCacheItem("Cached_KitDetail_" + objDTO.CompanyID.ToString());
                //if (ObjCache != null)
                //{
                objDTO.Created = obj.Created;
                objDTO.CreatedBy = obj.CreatedBy;
                objDTO.GUID = obj.GUID;
                objDTO.IsArchived = obj.IsArchived;
                objDTO.IsDeleted = obj.IsDeleted;
                objDTO.QuantityReadyForAssembly = obj.QuantityReadyForAssembly;
                objDTO.AvailableItemsInWIP = obj.AvailableItemsInWIP;
                QtyValue = 0;
                if (objDTO.Room > 0)
                {
                    QtyValue = this.GetQtyOnDemand(objDTO.KitGUID.ToString(), objDTO.QuantityPerKit.GetValueOrDefault(0), objDTO.AvailableItemsInWIP.GetValueOrDefault(0), objDTO.Room, objDTO.CompanyID);
                }

                objDTO.QtyToMeetDemand = QtyValue;


                //List<KitDetailDTO> objTemp = ObjCache.ToList();
                //objTemp.RemoveAll(i => i.GUID == objDTO.GUID);
                //ObjCache = objTemp.AsEnumerable();

                //List<KitDetailDTO> tempC = new List<KitDetailDTO>();
                //tempC.Add(objDTO);
                //IEnumerable<KitDetailDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                //CacheHelper<IEnumerable<KitDetailDTO>>.AppendToCacheItem("Cached_KitDetail_" + objDTO.CompanyID.ToString(), NewCache);
                //}


                return true;
            }
        }


        public void UpdateQtyToMeedDemand(Guid? KitGUID, long UserID, long SessionUserId)
        {


            if (SessionUserId > 0 && UserID != SessionUserId)
            {
                UserID = SessionUserId;
            }

            var params1 = new SqlParameter[] { new SqlParameter("@KitGUID", KitGUID.GetValueOrDefault(Guid.Empty)), new SqlParameter("@UserID", UserID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("exec [UpdateQtyToMeetDemand] @KitGUID, @UserID", params1);
            }
        }


        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteRecords(string GUIDs, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@GUIDs", GUIDs),
                                                   new SqlParameter("@UserID", userid) };

                context.Database.ExecuteSqlCommand("EXEC DeleteKitDetail @GUIDs,@UserID", params1);
                return true;
            }
        }


        public bool DeleteRecordsExcept(string IDs, Guid KitGUID, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (IDs != "")
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@KitGUID", KitGUID),
                                                       new SqlParameter("@IDs", IDs),
                                                       new SqlParameter("@UserID", userid)  };

                    context.Database.ExecuteSqlCommand("EXEC DeleteKitDetailExcept @KitGUID,@IDs,@UserID", params1);
                    KitDetailDAL objKitDetailDAL = new KitDetailDAL(base.DataBaseName);
                    objKitDetailDAL.GetKitDetailNew(0, CompanyID, false, false, true);
                }
                else
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@KitGUID", KitGUID),
                                                       new SqlParameter("@IDs", (object)DBNull.Value),
                                                       new SqlParameter("@UserID", userid)  };

                    context.Database.ExecuteSqlCommand("EXEC DeleteKitDetailExcept @KitGUID,@IDs,@UserID", params1);
                    KitDetailDAL objKitDetailDAL = new KitDetailDAL(base.DataBaseName);
                    objKitDetailDAL.GetKitDetailNew(0, CompanyID, false, false, true);
                }
                return true;
            }
        }


        public double GetQtyOnDemand(string ItemGUID, double QtyPerKit, double AvailableItemsInWIP, Int64 RoomId, Int64 CompanyID)
        {
            double QtyToMeetDemand = 0;
            ItemMasterDAL obj = new ItemMasterDAL(base.DataBaseName);
            Guid itemGUID1 = Guid.Empty;
            ItemMasterDTO objDTO = null;
            if (Guid.TryParse(ItemGUID, out itemGUID1))
            {
                objDTO = obj.GetItemWithoutJoins(null, itemGUID1);
            }
            if (objDTO != null)
            {
                QtyPerKit = QtyPerKit * objDTO.SuggestedOrderQuantity ?? 0;

                if ((QtyPerKit - AvailableItemsInWIP) > 0)
                    QtyToMeetDemand = QtyPerKit - AvailableItemsInWIP;
                else
                    QtyToMeetDemand = 0;
            }
            return QtyToMeetDemand;
        }

        public void GetSumOfQtyOnDemandForItem(string ItemGUID, Int64 RoomId, Int64 CompanyID, long SessionUserId,long EnterpriseId)
        {
            double SumOfQty = 0;
            ItemMasterDAL obj = new ItemMasterDAL(base.DataBaseName);
            ItemMasterDTO objDTO = obj.GetItemWithoutJoins(null, Guid.Parse(ItemGUID));
            if (objDTO != null)
            {
                IEnumerable<KitDetailDTO> listKitDtl = null;
                listKitDtl = GetCachedDataNewWithItemGuid(RoomId, CompanyID, false, false, true, Guid.Parse(ItemGUID)).ToList();
                if (listKitDtl != null && listKitDtl.Count() > 0)
                {
                    //listKitDtl = listKitDtl.Where(x => x.ItemGUID.ToString() == Convert.ToString(ItemGUID)).ToList();
                    //if (listKitDtl != null && listKitDtl.Count() > 0)
                    //{
                    SumOfQty = 0;
                    foreach (var item in listKitDtl.ToList())
                    {
                        if (item.QtyToMeetDemand.GetValueOrDefault(0) == 0)
                        {
                            item.QtyToMeetDemand = GetQtyOnDemand(Convert.ToString(item.KitGUID), item.QuantityPerKit.GetValueOrDefault(0), item.AvailableItemsInWIP.GetValueOrDefault(0), item.Room, item.CompanyID);
                            SumOfQty = SumOfQty + item.QtyToMeetDemand.GetValueOrDefault(0);
                            //this.Edit(item);
                        }
                        else
                        {
                            SumOfQty = SumOfQty + item.QtyToMeetDemand.GetValueOrDefault(0);
                        }
                    }
                    //Update item
                    objDTO.QtyToMeetDemand = SumOfQty;
                    obj.Edit(objDTO, SessionUserId,EnterpriseId);
                    //}
                }
            }
        }
        /// <summary>
        /// Get Particullar Record from the KitDetail by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public List<KitDetailDTO> GetAllHistoryRecord(string KitGUID, Int64 RoomID, Int64 CompanyID)
        {
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<ItemMasterDTO> lstItems = objItemDAL.GetAllItemsPlain(RoomID, CompanyID);
                //return (from u in context.Database.SqlQuery<KitDetailDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName 
                //                                                            FROM KitDetail_History A 
                //                                                            LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                //                                                            LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                //                                                            LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.KitGUID= '" + KitGUID + "'")
                var paramskt = new SqlParameter[] { new SqlParameter("@KitGUID", KitGUID), new SqlParameter("@dbName", DataBaseName) };
                return (from u in context.Database.SqlQuery<KitDetailDTO>("exec [GetKitDetailChangeLog] @KitGUID,@dbName", paramskt)
                        select new KitDetailDTO
                        {
                            ID = u.ID,
                            KitGUID = u.KitGUID,
                            ItemGUID = u.ItemGUID,
                            QuantityPerKit = u.QuantityPerKit,
                            QuantityReadyForAssembly = u.QuantityReadyForAssembly,
                            AvailableItemsInWIP = u.AvailableItemsInWIP,
                            Created = u.Created,
                            LastUpdated = u.LastUpdated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            GUID = u.GUID,
                            HistoryID = u.HistoryID,
                            Action = u.Action,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ItemDetail = lstItems.Where(x => x.GUID == u.ItemGUID).SingleOrDefault(),
                            QtyToMeetDemand = GetQtyOnDemand(Convert.ToString(u.KitGUID), u.QuantityPerKit.GetValueOrDefault(0), u.AvailableItemsInWIP.GetValueOrDefault(0), u.Room, u.CompanyID)
                        }).ToList();
            }
        }

        public bool IsKitItemDeletable(string KitItemGUID, Int64 RoomId, Int64 CompanyID)
        {
            KitDetailDTO KitDetailDTO = GetRecord(KitItemGUID, RoomId, CompanyID, false, false, false);
            if (KitDetailDTO == null)
                return false;

            KitMasterDTO KitMasterDTO = new KitMasterDAL(base.DataBaseName).GetRecord(KitDetailDTO.KitGUID.GetValueOrDefault(Guid.Empty).ToString());

            if (KitMasterDTO != null)
            {
                if (KitMasterDTO.AvailableKitQuantity.GetValueOrDefault(0) > 0)
                    return false;
                else if (KitMasterDTO.AvailableWIPKit.GetValueOrDefault(0) > 0)
                    return false;
            }

            if (KitDetailDTO != null && KitDetailDTO.AvailableItemsInWIP.GetValueOrDefault(0) > 0)
            {
                return false;
            }

            return true;
        }
        public List<KitDetailmain> GetKitDetailExport(long companyId, long RoomId)
        {
            List<KitDetailmain> lstKitDetailmain = new List<KitDetailmain>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId), new SqlParameter("@CompanyID", companyId) };
                List<KitDetailmain> lstresult = context.Database.SqlQuery<KitDetailmain>("exec Kit_GetKitData_Export @RoomID,@CompanyID", params1).ToList();
                lstKitDetailmain = (from K in lstresult
                                    select new KitDetailmain
                                    {
                                        ID = K.ID,
                                        KitPartNumber = K.KitPartNumber,
                                        ItemNumber = K.ItemNumber,
                                        QuantityPerKit = K.QuantityPerKit,
                                        KitGUID = K.KitGUID,
                                        ItemGUID = K.ItemGUID,
                                        OnHandQuantity = K.OnHandQuantity,
                                        IsDeleted = K.IsDeleted,
                                        IsBuildBreak = K.IsBuildBreak,
                                        // AvailableItemsInWIP = K.QuantityReadyForAssembly,
                                        // KitDemand = K.KitDemand,
                                        AvailableKitQuantity = K.OnHandQuantity,
                                        Description = K.Description,
                                        CriticalQuantity = K.CriticalQuantity,
                                        MinimumQuantity = K.MinimumQuantity,
                                        MaximumQuantity = K.MaximumQuantity,
                                        ReOrderType = K.ReOrderType,
                                        KitCategory = K.IsBuildBreak == true ? "WIP" : "Direct",
                                        SupplierName = K.SupplierName,
                                        SupplierPartNo = K.SupplierPartNo,
                                        DefaultLocationName = K.DefaultLocationName,
                                        CostUOMName = K.CostUOMName,
                                        UOM = K.UOM,
                                        DefaultReorderQuantity = K.DefaultReorderQuantity,
                                        DefaultPullQuantity = K.DefaultPullQuantity,
                                        // ItemTypeName = K.ItemTypeName,
                                        IsItemLevelMinMaxQtyRequired = K.IsItemLevelMinMaxQtyRequired,
                                        SerialNumberTracking = K.SerialNumberTracking,
                                        LotNumberTracking = K.LotNumberTracking,
                                        DateCodeTracking = K.DateCodeTracking,
                                        IsActive = K.IsActive,
                                    }).ToList();

            }
            return lstKitDetailmain;
        }

        public List<KitDetailmain> GetKitBinDetailExport(long companyId, long RoomId, string BinIDs)
        {
            List<KitDetailmain> lstKitDetailmain = new List<KitDetailmain>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId), new SqlParameter("@CompanyID", companyId), new SqlParameter("@BinIDs", BinIDs) };
                List<KitDetailmain> lstresult = context.Database.SqlQuery<KitDetailmain>("exec Kit_GetKitBinData_Export @RoomID,@CompanyID,@BinIDs", params1).ToList();
                lstKitDetailmain = (from K in lstresult
                                    select new KitDetailmain
                                    {
                                        ID = K.ID,
                                        KitPartNumber = K.KitPartNumber,
                                        ItemNumber = K.ItemNumber,
                                        QuantityPerKit = K.QuantityPerKit,
                                        KitGUID = K.KitGUID,
                                        ItemGUID = K.ItemGUID,
                                        OnHandQuantity = K.OnHandQuantity,
                                        IsDeleted = K.IsDeleted,
                                        IsBuildBreak = K.IsBuildBreak,
                                        // AvailableItemsInWIP = K.QuantityReadyForAssembly,
                                        // KitDemand = K.KitDemand,
                                        AvailableKitQuantity = K.OnHandQuantity,
                                        Description = K.Description,
                                        CriticalQuantity = K.CriticalQuantity,
                                        MinimumQuantity = K.MinimumQuantity,
                                        MaximumQuantity = K.MaximumQuantity,
                                        ReOrderType = K.ReOrderType,
                                        KitCategory = K.IsBuildBreak == true ? "WIP" : "Direct",
                                        SupplierName = K.SupplierName,
                                        SupplierPartNo = K.SupplierPartNo,
                                        DefaultLocationName = K.DefaultLocationName,
                                        CostUOMName = K.CostUOMName,
                                        UOM = K.UOM,
                                        DefaultReorderQuantity = K.DefaultReorderQuantity,
                                        DefaultPullQuantity = K.DefaultPullQuantity,
                                        // ItemTypeName = K.ItemTypeName,
                                        IsItemLevelMinMaxQtyRequired = K.IsItemLevelMinMaxQtyRequired,
                                        SerialNumberTracking = K.SerialNumberTracking,
                                        LotNumberTracking = K.LotNumberTracking,
                                        DateCodeTracking = K.DateCodeTracking,
                                        IsActive = K.IsActive,
                                        BinNumber = K.BinNumber
                                    }).ToList();

            }
            return lstKitDetailmain;
        }

        /// <summary>
        /// Get Paged Records from the OrderMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        private IEnumerable<KitDetailDTO> DB_GetKitRecords(Guid? KitGuid, Int64? ID, Guid? GUID, Int64? RoomID, Int64? CompanyID)
        {
            //string strQuer = @"";


            //if (KitGuid.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            //{
            //    if (strQuer.Length > 0)
            //        strQuer += ", ";
            //    strQuer += "@KitGUID= '" + KitGuid.GetValueOrDefault(Guid.Empty) + "'";
            //}

            //if (ID.GetValueOrDefault(0) > 0)
            //{
            //    if (strQuer.Length > 0)
            //        strQuer += ", ";
            //    strQuer += " @ID=" + ID.GetValueOrDefault(0);
            //}
            //if (GUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            //{
            //    if (strQuer.Length > 0)
            //        strQuer += ", ";
            //    strQuer += "@GUID= '" + GUID.GetValueOrDefault(Guid.Empty) + "'";
            //}


            //if (RoomID.GetValueOrDefault(0) > 0)
            //{
            //    if (strQuer.Length > 0)
            //        strQuer += ", ";
            //    strQuer += " @RoomID=" + RoomID.GetValueOrDefault(0);
            //}

            //if (CompanyID.GetValueOrDefault(0) > 0)
            //{
            //    if (strQuer.Length > 0)
            //        strQuer += ", ";
            //    strQuer += " @CompanyID=" + CompanyID.GetValueOrDefault(0);
            //}

            //strQuer = @"EXEC [Kit_GetKitDetailData] " + strQuer;

            //IEnumerable<KitDetailDTO> obj = ExecuteQuery(strQuer);

            IEnumerable<KitDetailDTO> obj = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@KitGUID",(KitGuid.GetValueOrDefault(Guid.Empty) == Guid.Empty ? (object)DBNull.Value : KitGuid)),
                                                   new SqlParameter("@ID", ID ?? (object)DBNull.Value),
                                                   new SqlParameter("@GUID", (GUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? (object)DBNull.Value : GUID)),
                                                   new SqlParameter("@RoomID", RoomID?? (object)DBNull.Value),
                                                   new SqlParameter("@CompanyID", CompanyID ?? (object)DBNull.Value)};
                
                obj = (from u in context.Database.SqlQuery<KitDetailDTO>("exec Kit_GetKitDetailData @KitGUID,@ID,@GUID,@RoomID,@CompanyID", params1)
                       select new KitDetailDTO
                       {
                           ID = u.ID,
                           KitGUID = u.KitGUID,
                           ItemGUID = u.ItemGUID,
                           QuantityPerKit = u.QuantityPerKit.GetValueOrDefault(0),
                           QuantityReadyForAssembly = u.QuantityReadyForAssembly.GetValueOrDefault(0),
                           AvailableItemsInWIP = u.AvailableItemsInWIP.GetValueOrDefault(0),
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
                           ReceivedOn = u.ReceivedOn,
                           ReceivedOnWeb = u.ReceivedOnWeb,
                           AddedFrom = u.AddedFrom,
                           EditedFrom = u.EditedFrom,
                           QtyToMeetDemand = u.QtyToMeetDemand,
                           ItemNumber = u.ItemNumber,
                           ItemType = u.ItemType,
                           CategoryName = u.CategoryName,
                           Cost = u.Cost,
                           DateCodeTracking = u.DateCodeTracking,
                           Description = u.Description,
                           LotNumberTracking = u.LotNumberTracking,
                           ManufacturerName = u.ManufacturerName,
                           ManufacturerNumber = u.ManufacturerNumber,
                           OnHandQuantity = u.OnHandQuantity,
                           Markup = u.Markup,
                           SellPrice = u.SellPrice,
                           SerialNumberTracking = u.SerialNumberTracking,
                           SupplierName = u.SupplierName,
                           SupplierPartNo = u.SupplierPartNo,
                           Unit = u.Unit,
                           Action = string.Empty,
                           DefaultBinID = u.DefaultBinID,
                           DefualtBinName = u.DefualtBinName,
                       }).AsParallel().ToList();

                return obj;
            }
        }

        /// <summary>
        /// Executer Query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private IEnumerable<KitDetailDTO> ExecuteQuery(string query)
        {
            IEnumerable<KitDetailDTO> obj = null;
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    obj = (from u in context.Database.SqlQuery<KitDetailDTO>(query)
                           select new KitDetailDTO
                           {
                               ID = u.ID,
                               KitGUID = u.KitGUID,
                               ItemGUID = u.ItemGUID,
                               QuantityPerKit = u.QuantityPerKit.GetValueOrDefault(0),
                               QuantityReadyForAssembly = u.QuantityReadyForAssembly.GetValueOrDefault(0),
                               AvailableItemsInWIP = u.AvailableItemsInWIP.GetValueOrDefault(0),
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
                               ReceivedOn = u.ReceivedOn,
                               ReceivedOnWeb = u.ReceivedOnWeb,
                               AddedFrom = u.AddedFrom,
                               EditedFrom = u.EditedFrom,
                               QtyToMeetDemand = u.QtyToMeetDemand,
                               ItemNumber = u.ItemNumber,
                               ItemType = u.ItemType,
                               CategoryName = u.CategoryName,
                               Cost = u.Cost,
                               DateCodeTracking = u.DateCodeTracking,
                               Description = u.Description,
                               LotNumberTracking = u.LotNumberTracking,
                               ManufacturerName = u.ManufacturerName,
                               ManufacturerNumber = u.ManufacturerNumber,
                               OnHandQuantity = u.OnHandQuantity,
                               Markup = u.Markup,
                               SellPrice = u.SellPrice,
                               SerialNumberTracking = u.SerialNumberTracking,
                               SupplierName = u.SupplierName,
                               SupplierPartNo = u.SupplierPartNo,
                               Unit = u.Unit,
                               Action = string.Empty,
                               DefaultBinID = u.DefaultBinID,
                               DefualtBinName = u.DefualtBinName,
                           }).AsParallel().ToList();

                    return obj;
                }
            }
            finally
            {
                obj = null;
            }
        }

        /// <summary>
        /// QtyToMoveIn
        /// </summary>
        /// <param name="KitDetailGuid"></param>
        /// <param name="Qty"></param>
        /// <param name="Bin"></param>
        public string QtyToMoveIn(KitMoveInOutDetailDTO InOutDTO, long SessionUserId, long EnterpriseId, long CompanyId, long RoomId, string CultureCode)
        {
            Int64 BinID = 0;
            Guid ItemGuid = Guid.Empty;
            KitDetailDTO kitDetail = null;
            BinMasterDAL binDAL = null;

            try
            {
                binDAL = new BinMasterDAL(base.DataBaseName);
                kitDetail = GetKitDetailByGuidNormal(InOutDTO.KitDetailGUID.GetValueOrDefault(Guid.Empty));

                if (kitDetail.ItemType != 4)
                {
                    ItemGuid = kitDetail.ItemGUID.GetValueOrDefault(Guid.Empty);
                    Int64? binid = binDAL.GetOrInsertBinIDByName(ItemGuid, InOutDTO.BinNumber, InOutDTO.CreatedBy.GetValueOrDefault(0), InOutDTO.Room.GetValueOrDefault(0), InOutDTO.CompanyID.GetValueOrDefault(0), false);
                    BinID = binid.GetValueOrDefault(0);
                    InOutDTO.ItemGUID = ItemGuid;
                    InOutDTO.BinID = BinID;

                    int moveInQty = QtyPullFromLocationDetails(InOutDTO, SessionUserId,EnterpriseId);
                    if (moveInQty == -1)
                    {
                        var kitResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResKitMaster", CultureCode, EnterpriseId, CompanyId);
                        string msgItemsHasNotEnoughQuantityToMoveIn = ResourceRead.GetResourceValueByKeyAndFullFilePath("ItemsHasNotEnoughQuantityToMoveIn", kitResourceFilePath, EnterpriseId, CompanyId, RoomId, "ResKitMaster", CultureCode);
                        return msgItemsHasNotEnoughQuantityToMoveIn;
                    }
                        
                }
                kitDetail.AvailableItemsInWIP = kitDetail.AvailableItemsInWIP.GetValueOrDefault(0) + InOutDTO.TotalQuantity;
                kitDetail.LastUpdatedBy = InOutDTO.LastUpdatedBy.GetValueOrDefault(0);
                kitDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                kitDetail.EditedFrom = "Web";
                Edit(kitDetail, SessionUserId,EnterpriseId);

                return "";
            }
            finally
            {


            }


        }

        /// <summary>
        /// QtyPullFromLocationDetails
        /// </summary>
        /// <param name="KitDetailGUID"></param>
        /// <param name="MoveQty"></param>
        /// <param name="ItemGuid"></param>
        /// <param name="BinID"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        private int QtyPullFromLocationDetails(KitMoveInOutDetailDTO InOutDTO, long SessionUserId,long EnterpriseId)
        {
            KitMoveInOutDetailDAL kitMoveInOutDAL = null;
            RoomDAL roomDAL = null;
            CommonDAL objDAL = null;
            RoomDTO room = null;
            ItemLocationDetailsDAL locDetailDAL = null;
            List<ItemLocationDetailsDTO> custQtyLocations = null;
            List<ItemLocationDetailsDTO> consQtyLocations = null;
            IEnumerable<ItemLocationDetailsDTO> itemLocations = null;


            try
            {
                locDetailDAL = new ItemLocationDetailsDAL(base.DataBaseName);
                kitMoveInOutDAL = new KitMoveInOutDetailDAL(base.DataBaseName);
                roomDAL = new RoomDAL(base.DataBaseName);
                objDAL = new CommonDAL(base.DataBaseName);
                //room = roomDAL.GetRoomByIDPlain(InOutDTO.Room.GetValueOrDefault(0));
                string columnList = "ID,RoomName,InventoryConsuptionMethod";
                room = objDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + InOutDTO.Room.GetValueOrDefault(0).ToString() + "", "");


                bool IsFIFO = true;
                bool IsUpdateQOH = false;
                if (room != null)
                {
                    if (!string.IsNullOrEmpty(room.InventoryConsuptionMethod) && room.InventoryConsuptionMethod.ToLower() == "lifo")
                        IsFIFO = false;
                }

                itemLocations = locDetailDAL.GetItemLocationeDetailsByItemGuidBinIdPlain(InOutDTO.ItemGUID.GetValueOrDefault(Guid.Empty), InOutDTO.BinID.GetValueOrDefault(0), InOutDTO.Room.GetValueOrDefault(0), InOutDTO.CompanyID.GetValueOrDefault(0));

                custQtyLocations = itemLocations.Where(x => x.CustomerOwnedQuantity.GetValueOrDefault(0) > 0).OrderBy(x => x.ReceivedDate.GetValueOrDefault(DateTime.MinValue)).ToList();
                consQtyLocations = itemLocations.Where(x => x.ConsignedQuantity.GetValueOrDefault(0) > 0).OrderBy(x => x.ReceivedDate.GetValueOrDefault(DateTime.MinValue)).ToList();

                double totalCustQty = custQtyLocations.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0));
                double totalConsQty = consQtyLocations.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0));

                double totalAvailQty = totalCustQty + totalConsQty;
                double tempMoveQty = InOutDTO.TotalQuantity;

                if (tempMoveQty > totalAvailQty)
                    return -1;

                if (!IsFIFO)
                {
                    custQtyLocations = custQtyLocations.OrderByDescending(x => x.ReceivedDate.GetValueOrDefault(DateTime.MinValue)).ToList();
                    consQtyLocations = consQtyLocations.OrderByDescending(x => x.ReceivedDate.GetValueOrDefault(DateTime.MinValue)).ToList();
                }


                custQtyLocations.ForEach(x =>
                {
                    if (x.CustomerOwnedQuantity.GetValueOrDefault(0) > 0 && tempMoveQty > 0)
                    {
                        if (x.CustomerOwnedQuantity.GetValueOrDefault(0) >= tempMoveQty)
                        {
                            InOutDTO.CustomerOwnedQuantity = tempMoveQty;
                            x.CustomerOwnedQuantity = x.CustomerOwnedQuantity.GetValueOrDefault(0) - tempMoveQty;
                            tempMoveQty = 0;
                        }
                        else if (x.CustomerOwnedQuantity.GetValueOrDefault(0) < tempMoveQty)
                        {
                            InOutDTO.CustomerOwnedQuantity = x.CustomerOwnedQuantity.GetValueOrDefault(0);
                            tempMoveQty = tempMoveQty - x.CustomerOwnedQuantity.GetValueOrDefault(0);
                            x.CustomerOwnedQuantity = 0;

                        }
                        x.EditedFrom = "Web";
                        x.LastUpdatedBy = InOutDTO.LastUpdatedBy.GetValueOrDefault(0);
                        x.Updated = DateTimeUtility.DateTimeNow;
                        x.ReceivedOn = DateTimeUtility.DateTimeNow;
                        x.KitDetailGUID = InOutDTO.KitDetailGUID;
                        locDetailDAL.Edit(x);

                        InOutDTO.ConsignedQuantity = 0;
                        InOutDTO.ItemLocationDetailGUID = x.GUID;
                        kitMoveInOutDAL.Insert(InOutDTO);

                        if (!IsUpdateQOH)
                            IsUpdateQOH = true;
                    }
                    else
                        return;
                });


                consQtyLocations.ForEach(x =>
                {
                    if (x.ConsignedQuantity.GetValueOrDefault(0) > 0 && tempMoveQty > 0)
                    {
                        if (x.ConsignedQuantity.GetValueOrDefault(0) >= tempMoveQty)
                        {
                            InOutDTO.ConsignedQuantity = tempMoveQty;
                            x.ConsignedQuantity = x.ConsignedQuantity.GetValueOrDefault(0) - tempMoveQty;
                            tempMoveQty = 0;
                        }
                        else if (x.ConsignedQuantity.GetValueOrDefault(0) < tempMoveQty)
                        {
                            InOutDTO.ConsignedQuantity = x.ConsignedQuantity.GetValueOrDefault(0);
                            tempMoveQty = tempMoveQty - x.ConsignedQuantity.GetValueOrDefault(0);
                            x.ConsignedQuantity = 0;

                        }
                        x.EditedFrom = "Web";
                        x.LastUpdatedBy = InOutDTO.LastUpdatedBy;
                        x.Updated = DateTimeUtility.DateTimeNow;
                        x.ReceivedOn = DateTimeUtility.DateTimeNow;
                        x.KitDetailGUID = InOutDTO.KitDetailGUID;
                        locDetailDAL.Edit(x);

                        InOutDTO.CustomerOwnedQuantity = 0;
                        InOutDTO.ItemLocationDetailGUID = x.GUID;
                        kitMoveInOutDAL.Insert(InOutDTO);
                        if (!IsUpdateQOH)
                            IsUpdateQOH = true;
                    }
                    else
                        return;
                });


                //double AvailCustQty = custQtyLocations.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0));
                //double AvailConsQty = consQtyLocations.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0));

                if (IsUpdateQOH)
                {
                    UpdateLocationQtyAndQOH(InOutDTO.ItemGUID.GetValueOrDefault(Guid.Empty), InOutDTO.BinID.GetValueOrDefault(0), InOutDTO.Room.GetValueOrDefault(0), InOutDTO.CompanyID.GetValueOrDefault(0), InOutDTO.CreatedBy.GetValueOrDefault(0), "Kit Move IN", SessionUserId,EnterpriseId);
                }

                return 1;
            }
            finally
            {
                kitMoveInOutDAL = null;
                roomDAL = null;
                room = null;
                locDetailDAL = null;
                custQtyLocations = null;
                consQtyLocations = null;
            }

        }

        /// <summary>
        /// QtyCreditToLocationDetails
        /// </summary>
        /// <param name="InOutDTO"></param>
        /// <returns></returns>
        private int QtyCreditToLocationDetails(KitMoveInOutDetailDTO InOutDTO, long SessionUserId,long EnterpriseId)
        {
            KitMoveInOutDetailDAL kitMoveInOutDAL = new KitMoveInOutDetailDAL(base.DataBaseName); ;
            ItemMasterDAL itemDAL = null;
            ItemLocationDetailsDAL locDetailDAL = null;
            List<ItemLocationDetailsDTO> custQtyLocations = null;
            List<ItemLocationDetailsDTO> consQtyLocations = null;
            IEnumerable<ItemLocationDetailsDTO> itemLocations = null;

            try
            {
                var result = kitMoveInOutDAL.GetCorrespondingMoveInEntry(InOutDTO);
                if (result)
                {
                    locDetailDAL = new ItemLocationDetailsDAL(base.DataBaseName);
                    itemDAL = new ItemMasterDAL(base.DataBaseName);
                    ItemMasterDTO itmDOT = itemDAL.GetItemWithoutJoins(null, InOutDTO.ItemGUID);

                    ItemLocationDetailsDTO newLocDto = new ItemLocationDetailsDTO()
                    {
                        ID = 0,
                        BinID = InOutDTO.BinID,
                        CustomerOwnedQuantity = itmDOT.Consignment ? 0 : InOutDTO.TotalQuantity,
                        ConsignedQuantity = itmDOT.Consignment ? InOutDTO.TotalQuantity : 0,
                        ReceivedDate = DateTimeUtility.DateTimeNow,
                        Cost = itmDOT.Cost,
                        GUID = Guid.Empty,
                        ItemGUID = InOutDTO.ItemGUID,
                        Created = DateTimeUtility.DateTimeNow,
                        Updated = DateTimeUtility.DateTimeNow,
                        CreatedBy = InOutDTO.CreatedBy,
                        LastUpdatedBy = InOutDTO.LastUpdatedBy,
                        IsDeleted = false,
                        IsArchived = false,
                        CompanyID = InOutDTO.CompanyID,
                        Room = InOutDTO.Room,
                        KitDetailGUID = InOutDTO.KitDetailGUID,
                        IsConsignedSerialLot = false,
                        InitialQuantity = InOutDTO.TotalQuantity,
                        InitialQuantityWeb = InOutDTO.TotalQuantity,
                        InsertedFrom = " Web Kit Move Out",
                        ReceivedOn = DateTimeUtility.DateTimeNow,
                        ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                        AddedFrom = "Web",
                        EditedFrom = "Web",
                    };
                    locDetailDAL.Insert(newLocDto);
                    itemLocations = locDetailDAL.GetItemLocationeDetailsByItemGuidBinIdPlain(InOutDTO.ItemGUID.GetValueOrDefault(Guid.Empty),InOutDTO.BinID.GetValueOrDefault(0), InOutDTO.Room.GetValueOrDefault(0), InOutDTO.CompanyID.GetValueOrDefault(0));
                    custQtyLocations = itemLocations.Where(x => x.CustomerOwnedQuantity.GetValueOrDefault(0) > 0).OrderBy(x => x.ReceivedDate.GetValueOrDefault(DateTime.MinValue)).ToList();
                    consQtyLocations = itemLocations.Where(x => x.ConsignedQuantity.GetValueOrDefault(0) > 0).OrderBy(x => x.ReceivedDate.GetValueOrDefault(DateTime.MinValue)).ToList();
                    double AvailCustQty = custQtyLocations.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0));
                    double AvailConsQty = consQtyLocations.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0));
                    double totalAvailQty = AvailCustQty + AvailConsQty;

                    UpdateLocationQtyAndQOH(InOutDTO.ItemGUID.GetValueOrDefault(Guid.Empty), InOutDTO.BinID.GetValueOrDefault(0), InOutDTO.Room.GetValueOrDefault(0), InOutDTO.CompanyID.GetValueOrDefault(0), InOutDTO.CreatedBy.GetValueOrDefault(0), "Kit Move Out", SessionUserId,EnterpriseId);

                    InOutDTO.ConsignedQuantity = itmDOT.Consignment ? 0 : InOutDTO.TotalQuantity;
                    InOutDTO.CustomerOwnedQuantity = itmDOT.Consignment ? InOutDTO.TotalQuantity : 0;

                    InOutDTO.ItemLocationDetailGUID = newLocDto.GUID;
                    kitMoveInOutDAL.Insert(InOutDTO);
                    var isUpdateIsMovedOutFlagInMoveInEntry = kitMoveInOutDAL.UpdateIsMovedInFlag(InOutDTO);
                    return 1;
                }
                else
                {
                    return -2; // in case of in entry not founf for this item and bin
                }
            }
            finally
            {
                kitMoveInOutDAL = null;

                locDetailDAL = null;
                custQtyLocations = null;
                consQtyLocations = null;
            }
        }

        /// <summary>
        /// UpdateLocationQtyAndQOH
        /// </summary>
        /// <param name="ItemGuid"></param>
        /// <param name="BinID"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="UserID"></param>
        private void UpdateLocationQtyAndQOH(Guid ItemGuid, Int64 BinID, Int64 RoomID, Int64 CompanyID, Int64 UserID, string WhatWhereAction, long SessionUserId,long EenterpriseId)
        {
            ItemLocationQTYDAL locQtyDAL = null;
            List<ItemLocationQTYDTO> lstLocQtyDTO = null;
            ItemLocationQTYDTO locQty = null;
            try
            {
                locQtyDAL = new ItemLocationQTYDAL(base.DataBaseName);
                lstLocQtyDTO = new List<ItemLocationQTYDTO>();
                locQty = new ItemLocationQTYDTO()
                {
                    ItemGUID = ItemGuid,
                    BinID = BinID,
                    Room = RoomID,
                    CompanyID = CompanyID,
                    CreatedBy = UserID,
                    LastUpdatedBy = UserID,
                };

                lstLocQtyDTO.Add(locQty);
                locQtyDAL.Save(lstLocQtyDTO, WhatWhereAction, SessionUserId,EenterpriseId);
            }
            finally
            {
                locQtyDAL = null;
                lstLocQtyDTO = null;
                locQty = null;
            }
        }

        /// <summary>
        /// QtyToMoveIn
        /// </summary>
        /// <param name="KitDetailGuid"></param>
        /// <param name="Qty"></param>
        /// <param name="Bin"></param>
        public string QtyToMoveOut(KitMoveInOutDetailDTO InOutDTO, long SessionUserId, long EnterpriseId, long CompanyId, long RoomId, string CultureCode)
        {
            Int64 BinID = 0;
            Guid ItemGuid = Guid.Empty;
            KitDetailDTO kitDetail = null;
            BinMasterDAL binDAL = null;

            try
            {
                binDAL = new BinMasterDAL(base.DataBaseName);
                kitDetail = GetKitDetailByGuidNormal(InOutDTO.KitDetailGUID.GetValueOrDefault(Guid.Empty));

                if (kitDetail.ItemType != 4)
                {
                    ItemGuid = kitDetail.ItemGUID.GetValueOrDefault(Guid.Empty);
                    Int64? binid = binDAL.GetOrInsertBinIDByName(ItemGuid, InOutDTO.BinNumber, InOutDTO.CreatedBy.GetValueOrDefault(0), InOutDTO.Room.GetValueOrDefault(0), InOutDTO.CompanyID.GetValueOrDefault(0), false);
                    BinID = binid.GetValueOrDefault(0);
                    InOutDTO.ItemGUID = ItemGuid;
                    InOutDTO.BinID = BinID;

                    if (InOutDTO.TotalQuantity > kitDetail.AvailableItemsInWIP.GetValueOrDefault(0))
                    {
                        var toolResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResToolMaster", CultureCode, EnterpriseId, CompanyId);
                        string msgNotEnoughQtyInWIP = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotEnoughQtyInWIP", toolResourceFilePath, EnterpriseId, CompanyId, RoomId, "ResToolMaster", CultureCode);
                        return msgNotEnoughQtyInWIP;
                    }
                        

                    int moveInQty = QtyCreditToLocationDetails(InOutDTO, SessionUserId,EnterpriseId);

                    if (moveInQty == -2)
                    {
                        var transferResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResTransfer", CultureCode, EnterpriseId, CompanyId);
                        string msgNotEnoughQuantity = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotEnoughQuantity", transferResourceFilePath, EnterpriseId, CompanyId, RoomId, "ResTransfer", CultureCode);
                        return msgNotEnoughQuantity;
                    }                    
                    else if (moveInQty == -1)
                    {
                        var toolResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResToolMaster", CultureCode, EnterpriseId, CompanyId);
                        string msgNotEnoughQtyInWIP = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotEnoughQtyInWIP", toolResourceFilePath, EnterpriseId, CompanyId, RoomId, "ResToolMaster", CultureCode);
                        return msgNotEnoughQtyInWIP;
                    }
                        
                }
                if (InOutDTO.MoveInOut == "IN")
                    kitDetail.AvailableItemsInWIP = kitDetail.AvailableItemsInWIP.GetValueOrDefault(0) + InOutDTO.TotalQuantity;
                else if (InOutDTO.MoveInOut == "OUT")
                    kitDetail.AvailableItemsInWIP = kitDetail.AvailableItemsInWIP.GetValueOrDefault(0) - InOutDTO.TotalQuantity;

                kitDetail.LastUpdatedBy = InOutDTO.LastUpdatedBy.GetValueOrDefault(0);
                kitDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                kitDetail.EditedFrom = "Web";
                Edit(kitDetail, SessionUserId,EnterpriseId);

                return "";
            }
            finally
            {
            }
        }
    }
}


