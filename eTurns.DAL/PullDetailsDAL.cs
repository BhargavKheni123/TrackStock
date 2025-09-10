using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public partial class PullDetailsDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        public PullDetailsDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public PullDetailsDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        public PullDetailsDTO GetPullDetailByGuidNormal(Guid GUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Guid", GUID) };
                return context.Database.SqlQuery<PullDetailsDTO>("exec [GetPullDetailByGuidNormal] @Guid", params1).FirstOrDefault();
            }
        }

        public List<PullDetailsDTO> GetPullDetailsByPullGuidPlain(Guid PullGUID, long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId), new SqlParameter("@CompanyID", CompanyId), new SqlParameter("@PullGUID", PullGUID) };
                return context.Database.SqlQuery<PullDetailsDTO>("exec [GetPullDetailsByPullGuidPlain] @RoomID,@CompanyID,@PullGUID", params1).ToList();
            }
        }

        public List<PullDetailsDTO> GetPullDetailsHavingPendingCredit(Guid PullGUID, long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId), new SqlParameter("@CompanyID", CompanyId), new SqlParameter("@PullGUID", PullGUID) };
                return context.Database.SqlQuery<PullDetailsDTO>("exec [GetPullDetailsHavingPendingCredit] @RoomID,@CompanyID,@PullGUID", params1).ToList();
            }
        }

        /// <summary>
        /// Get Paged Records from the PullDetails Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public List<PullDetailsDTO> GetPagedPullDetails(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string SortColumnName, long RoomId, long CompanyId, bool IsArchived, bool IsDeleted, Guid PullGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@StartRowIndex", StartRowIndex),
                                                   new SqlParameter("@MaxRows", MaxRows),
                                                   new SqlParameter("@SearchTerm", SearchTerm ?? (object)DBNull.Value),
                                                   new SqlParameter("@sortColumnName", SortColumnName ?? (object)DBNull.Value),
                                                   new SqlParameter("@IsDeleted", IsDeleted),
                                                   new SqlParameter("@IsArchived", IsArchived),
                                                   new SqlParameter("@CompanyID", CompanyId),
                                                   new SqlParameter("@Room", RoomId),
                                                   new SqlParameter("@PullGuid", PullGUID)
                };
                List<PullDetailsDTO> pullDetails = new List<PullDetailsDTO>();
                if (IsArchived)
                 pullDetails = context.Database.SqlQuery<PullDetailsDTO>("exec [GetPagedPullDetails_Archieve] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@IsDeleted,@IsArchived,@CompanyID,@Room,@PullGuid", params1).ToList();
                else
                pullDetails = context.Database.SqlQuery<PullDetailsDTO>("exec [GetPagedPullDetails] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@IsDeleted,@IsArchived,@CompanyID,@Room,@PullGuid", params1).ToList();

                TotalCount = 0;
                if (pullDetails != null && pullDetails.Count > 0)
                {
                    TotalCount = pullDetails.First().TotalCount ?? 0;
                }

                return pullDetails;
            }
        }

        /// <summary>
        /// Insert Record in the DataBase PullDetails
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public long Insert(PullDetailsDTO objDTO)
        {
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.IsDeleted = false;
            objDTO.IsArchived = false;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                PullDetail obj = new PullDetail();
                obj.ID = 0;
                obj.PULLGUID = objDTO.PULLGUID;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.ProjectSpendGUID = objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : objDTO.ProjectSpendGUID;
                obj.ItemCost = objDTO.ItemCost;
                obj.ItemPrice = objDTO.ItemPrice;
                obj.CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity;
                obj.ConsignedQuantity = objDTO.ConsignedQuantity;
                obj.PoolQuantity = objDTO.PoolQuantity;
                obj.SerialNumber = objDTO.SerialNumber;
                obj.LotNumber = objDTO.LotNumber;
                obj.Expiration = objDTO.Expiration;
                obj.Received = (!string.IsNullOrEmpty(objDTO.Received) ? objDTO.Received : Convert.ToString(DateTimeUtility.DateTimeNow.ToShortDateString()));
                obj.BinID = objDTO.BinID;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
                obj.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.PullCredit = objDTO.PullCredit;
                obj.GUID = ((objDTO.GUID != null && objDTO.GUID != Guid.Empty) ? objDTO.GUID : Guid.NewGuid());
                obj.ItemLocationDetailGUID = objDTO.ItemLocationDetailGUID;
                obj.MaterialStagingPullDetailGUID = objDTO.MaterialStagingPullDetailGUID;
                obj.ReceivedOn = objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                if (string.IsNullOrEmpty(objDTO.AddedFrom))
                    objDTO.AddedFrom = "Web";

                if (string.IsNullOrEmpty(objDTO.EditedFrom))
                    objDTO.EditedFrom = "Web";

                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;
                obj.CreditCustomerOwnedQuantity = objDTO.CreditCustomerOwnedQuantity;
                obj.CreditConsignedQuantity = objDTO.CreditConsignedQuantity;
                context.PullDetails.Add(obj);
                context.SaveChanges();

                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;
                return obj.ID;
            }

        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(PullDetailsDTO objDTO)
        {
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                PullDetail obj = new PullDetail();
                obj = context.PullDetails.FirstOrDefault(t => t.ID == objDTO.ID && t.PULLGUID == objDTO.PULLGUID);

                if (obj == null)
                    obj = new PullDetail();

                obj.ID = objDTO.ID;
                obj.PULLGUID = objDTO.PULLGUID;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.ItemCost = objDTO.ItemCost;
                obj.ItemPrice = objDTO.ItemPrice;
                obj.CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity;
                obj.ConsignedQuantity = objDTO.ConsignedQuantity;
                obj.PoolQuantity = objDTO.PoolQuantity;
                obj.SerialNumber = objDTO.SerialNumber;
                obj.LotNumber = objDTO.LotNumber;
                obj.Expiration = objDTO.Expiration;
                obj.Received = objDTO.Received;
                obj.BinID = objDTO.BinID;
                obj.Created = objDTO.Created;
                obj.Updated = objDTO.Updated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
                obj.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.PullCredit = objDTO.PullCredit;
                obj.ItemLocationDetailGUID = objDTO.ItemLocationDetailGUID;
                obj.MaterialStagingPullDetailGUID = objDTO.MaterialStagingPullDetailGUID;
                obj.GUID = objDTO.GUID;
                obj.CreditCustomerOwnedQuantity = objDTO.CreditCustomerOwnedQuantity;
                obj.CreditConsignedQuantity = objDTO.CreditConsignedQuantity;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                obj.AddedFrom = objDTO.AddedFrom;
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.EditedFrom = objDTO.EditedFrom;
                context.SaveChanges();

                return true;
            }
        }

        public bool EditCreditQuantity(PullDetailsDTO objDTO)
        {
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                PullDetail obj = context.PullDetails.Where(x => x.ID == objDTO.ID).FirstOrDefault();
                obj.CreditCustomerOwnedQuantity = objDTO.CreditCustomerOwnedQuantity;
                obj.CreditConsignedQuantity = objDTO.CreditConsignedQuantity;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                context.SaveChanges();

                return true;
            }
        }

        public bool CheckPullExist(Guid WorkOrderGUID)
        {
            bool Ret = false;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Ret = context.PullMasters.Any(x => x.WorkOrderDetailGUID == WorkOrderGUID);
            }
            return Ret;
        }

        #region New Changes related to Credit History Table

        public long InsertCreditHistory(PullCreditHistoryDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? objDTO.IsArchived : false;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CreditHistory obj = new CreditHistory();
                obj.ID = 0;
                obj.GUID = objDTO.GUID;
                obj.PullDetailGuid = objDTO.PullDetailGuid;
                obj.PULLGUID = objDTO.PULLGUID;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.CreditCustomerOwnedQuantity = objDTO.CreditCustomerOwnedQuantity;
                obj.CreditConsignedQuantity = objDTO.CreditConsignedQuantity;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;
                obj.WhatWhereAction = objDTO.WhatWhereAction;

                if (string.IsNullOrEmpty(obj.AddedFrom))
                    obj.AddedFrom = "Web";

                if (string.IsNullOrEmpty(obj.EditedFrom))
                    obj.EditedFrom = "Web";

                obj.CreditGuid = objDTO.CreditGuid;
                obj.CreditDetailGuid = objDTO.CreditDetailGuid;

                context.CreditHistories.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;

                return obj.ID;
            }
        }

        public IEnumerable<PullDetailsDTO> GetPullDetailsByItemGuid_CreditHistoryFull(Guid ItemGuid, string PullCredit, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {  new SqlParameter("@ItemGuid",ItemGuid)
                                                    , new SqlParameter("@PullCredit",PullCredit)
                                                    , new SqlParameter("@RoomID",RoomID)
                                                    , new SqlParameter("@CompanyID",CompanyID)};
                return context.Database.SqlQuery<PullDetailsDTO>("exec [GetPullDetailsByItemGuid_CreditHistoryFull] @ItemGuid,@PullCredit,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public IEnumerable<PullDetailsDTO> GetPullDetailsByItemGuid_CreditHistoryPlain(Guid ItemGuid, string PullCredit, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {  new SqlParameter("@ItemGuid",ItemGuid)
                                                    , new SqlParameter("@PullCredit",PullCredit)
                                                    , new SqlParameter("@RoomID",RoomID)
                                                    , new SqlParameter("@CompanyID",CompanyID)};
                return context.Database.SqlQuery<PullDetailsDTO>("exec [GetPullDetailsByItemGuid_CreditHistoryPlain] @ItemGuid,@PullCredit,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public IEnumerable<PullDetailsDTO> GetPullDetailsByItemGuidAndSerialNo_CreditHistoryPlain(Guid ItemGuid, string PullCredit, long RoomId, long CompanyId, string SerialNumber)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {  new SqlParameter("@ItemGuid",ItemGuid)
                                                    , new SqlParameter("@PullCredit",PullCredit)
                                                    , new SqlParameter("@RoomID",RoomId)
                                                    , new SqlParameter("@CompanyID",CompanyId)
                                                    , new SqlParameter("@SerialNumber", SerialNumber)};
                return context.Database.SqlQuery<PullDetailsDTO>("exec [GetPullDetailsByItemGuidAndSerialNo_CreditHistoryPlain] @ItemGuid,@PullCredit,@RoomID,@CompanyID,@SerialNumber", params1).ToList();
            }
        }

        public IEnumerable<PullDetailsDTO> GetPullDetailsByItemGuidAndSerialNo_CreditHistoryFull(Guid ItemGuid, string PullCredit, long RoomId, long CompanyId, string SerialNumber)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {  new SqlParameter("@ItemGuid",ItemGuid)
                                                    , new SqlParameter("@PullCredit",PullCredit)
                                                    , new SqlParameter("@RoomID",RoomId)
                                                    , new SqlParameter("@CompanyID",CompanyId)
                                                    , new SqlParameter("@SerialNumber", SerialNumber)};
                return context.Database.SqlQuery<PullDetailsDTO>("exec [GetPullDetailsByItemGuidAndSerialNo_CreditHistoryFull] @ItemGuid,@PullCredit,@RoomID,@CompanyID,@SerialNumber", params1).ToList();
            }
        }

        public IEnumerable<PullDetailsDTO> GetPullDetailsByItemGuidAndLotNo_CreditHistoryPlain(Guid ItemGuid, string PullCredit, long RoomId, long CompanyId, string LotNumber)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {  new SqlParameter("@ItemGuid",ItemGuid)
                                                    , new SqlParameter("@PullCredit",PullCredit)
                                                    , new SqlParameter("@RoomID",RoomId)
                                                    , new SqlParameter("@CompanyID",CompanyId)
                                                    , new SqlParameter("@LotNumber", LotNumber)};
                return context.Database.SqlQuery<PullDetailsDTO>("exec [GetPullDetailsByItemGuidAndLotNo_CreditHistoryPlain] @ItemGuid,@PullCredit,@RoomID,@CompanyID,@LotNumber", params1).ToList();
            }
        }

        public IEnumerable<PullDetailsDTO> GetPullDetailsByItemGuidAndLotNo_CreditHistoryFull(Guid ItemGuid, string PullCredit, long RoomId, long CompanyId, string LotNumber)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {  new SqlParameter("@ItemGuid",ItemGuid)
                                                    , new SqlParameter("@PullCredit",PullCredit)
                                                    , new SqlParameter("@RoomID",RoomId)
                                                    , new SqlParameter("@CompanyID",CompanyId)
                                                    , new SqlParameter("@LotNumber", LotNumber)};
                return context.Database.SqlQuery<PullDetailsDTO>("exec [GetPullDetailsByItemGuidAndLotNo_CreditHistoryFull] @ItemGuid,@PullCredit,@RoomID,@CompanyID,@LotNumber", params1).ToList();
            }
        }
        public IEnumerable<PullDetailsDTO> GetPullDetailsByItemGuidAndExpiration_CreditHistoryFull(Guid ItemGuid, string PullCredit, long RoomId, long CompanyId, string Expiration)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {  new SqlParameter("@ItemGuid",ItemGuid)
                                                    , new SqlParameter("@PullCredit",PullCredit)
                                                    , new SqlParameter("@RoomID",RoomId)
                                                    , new SqlParameter("@CompanyID",CompanyId)
                                                    , new SqlParameter("@Expiration", Expiration)};
                return context.Database.SqlQuery<PullDetailsDTO>("exec [GetPullDetailsByItemGuidAndExpiration_CreditHistoryFull] @ItemGuid,@PullCredit,@RoomID,@CompanyID,@Expiration", params1).ToList();
            }
        }
        #endregion New Changes related to Credit History Table

        #region Applied checked pull qunatity based on Item Level Consignement Flag

        public IEnumerable<PullDetailsDTO> GetPullDetailsByItemGuid_CreditHistoryForCountNormal(Guid ItemGuid, string PullCredit, long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] {  new SqlParameter("@ItemGuid",ItemGuid)
                                                    , new SqlParameter("@PullCredit",PullCredit)
                                                    , new SqlParameter("@RoomID",RoomId)
                                                    , new SqlParameter("@CompanyID",CompanyId)};

                return context.Database.SqlQuery<PullDetailsDTO>("exec [GetPullDetailsByItemGuid_CreditHistoryForCountNormal] @ItemGuid,@PullCredit,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public IEnumerable<PullDetailsDTO> GetPullDetailsByItemGuidAndSerialNo_CreditHistoryForCountNormal(Guid ItemGuid, string PullCredit, long RoomId, long CompanyId, string SerialNumber)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] {  new SqlParameter("@ItemGuid",ItemGuid)
                                                    , new SqlParameter("@PullCredit",PullCredit)
                                                    , new SqlParameter("@RoomID",RoomId)
                                                    , new SqlParameter("@CompanyID",CompanyId)
                                                    , new SqlParameter("@SerialNumber", SerialNumber)};

                return context.Database.SqlQuery<PullDetailsDTO>("exec [GetPullDetailsByItemGuidAndSerialNo_CreditHistoryForCountNormal] @ItemGuid,@PullCredit,@RoomID,@CompanyID,@SerialNumber", params1).ToList();
            }
        }

        public IEnumerable<PullDetailsDTO> GetPullDetailsByItemGuidAndLotNo_CreditHistoryForCountNormal(Guid ItemGuid, string PullCredit, long RoomId, long CompanyId, string LotNumber)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] {  new SqlParameter("@ItemGuid",ItemGuid)
                                                    , new SqlParameter("@PullCredit",PullCredit)
                                                    , new SqlParameter("@RoomID",RoomId)
                                                    , new SqlParameter("@CompanyID",CompanyId)
                                                    , new SqlParameter("@LotNumber", LotNumber)};

                return context.Database.SqlQuery<PullDetailsDTO>("exec [GetPullDetailsByItemGuidAndLotNo_CreditHistoryForCountNormal] @ItemGuid,@PullCredit,@RoomID,@CompanyID,@LotNumber", params1).ToList();
            }
        }

        public IEnumerable<PullDetailsDTO> GetPullDetailsByItemGuidAndExpDate_Count(Guid ItemGuid, string PullCredit, long RoomId, long CompanyId, string Expiration)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] {  new SqlParameter("@ItemGuid",ItemGuid)
                                                    , new SqlParameter("@PullCredit",PullCredit)
                                                    , new SqlParameter("@RoomID",RoomId)
                                                    , new SqlParameter("@CompanyID",CompanyId)
                                                    , new SqlParameter("@Expiration", Expiration)};

                return context.Database.SqlQuery<PullDetailsDTO>("exec [GetPullDetailsByItemGuidAndExpDate_Count] @ItemGuid,@PullCredit,@RoomID,@CompanyID,@Expiration", params1).ToList();
            }
        }

        #endregion Applied checked pull qunatity based on Item Level Consignement Flag

        #region WI-4873

        public List<PullDetailsDTO> GetMSPullDetailsByMSPullDetailGuidPlain(Guid ItemGuid, Guid MaterialStagingPullDetailGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] {  new SqlParameter("@ItemGuid",ItemGuid)
                                                    , new SqlParameter("@MaterialStagingPullDetailGuid",MaterialStagingPullDetailGuid)};

                return context.Database.SqlQuery<PullDetailsDTO>("exec [GetMSPullDetailsByMSPullDetailGuidPlain] @ItemGuid,@MaterialStagingPullDetailGuid", params1).ToList();
            }
        }

        public List<PullDetailsDTO> GetMSPullDetailsByMSPullDetailGuidAndSerialNoPlain(Guid ItemGuid, Guid MaterialStagingPullDetailGuid, string SerialNumber)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] {  new SqlParameter("@ItemGuid",ItemGuid)
                                                    , new SqlParameter("@MaterialStagingPullDetailGuid",MaterialStagingPullDetailGuid)
                                                    , new SqlParameter("@SerialNumber",SerialNumber)};

                return context.Database.SqlQuery<PullDetailsDTO>("exec [GetMSPullDetailsByMSPullDetailGuidAndSerialNoPlain] @ItemGuid,@MaterialStagingPullDetailGuid,@SerialNumber", params1).ToList();
            }
        }

        public List<PullDetailsDTO> GetMSPullDetailsByMSPullDetailGuidAndLotNoPlain(Guid ItemGuid, Guid MaterialStagingPullDetailGuid, string LotNumber)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] {  new SqlParameter("@ItemGuid",ItemGuid)
                                                    , new SqlParameter("@MaterialStagingPullDetailGuid",MaterialStagingPullDetailGuid)
                                                    , new SqlParameter("@LotNumber",LotNumber)};

                return context.Database.SqlQuery<PullDetailsDTO>("exec [GetMSPullDetailsByMSPullDetailGuidAndLotNoPlain] @ItemGuid,@MaterialStagingPullDetailGuid,@LotNumber", params1).ToList();
            }
        }

        public List<PullDetailsDTO> GetMSPullDetailsByMSPullDetailGuidAndExpirationPlain(Guid ItemGuid, Guid MaterialStagingPullDetailGuid, string Expiration)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] {  new SqlParameter("@ItemGuid",ItemGuid)
                                                    , new SqlParameter("@MaterialStagingPullDetailGuid",MaterialStagingPullDetailGuid)
                                                    , new SqlParameter("@Expiration",Expiration)};

                return context.Database.SqlQuery<PullDetailsDTO>("exec [GetMSPullDetailsByMSPullDetailGuidAndExpirationPlain] @ItemGuid,@MaterialStagingPullDetailGuid,@Expiration", params1).ToList();
            }
        }

        #region 3055

        public List<PullDetailsDTO> GetPullDetailsByPullGuid(Guid PullGuid, Int64 RoomID, Int64 CompanyId)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@PullGuid", PullGuid), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyId) };
            string Qry = "EXEC dbo.[GetPullDetailsByPullGuid] @PullGuid,@RoomID,@CompanyId";
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<PullDetailsDTO>(Qry, params1).ToList();
            }
        }

        public List<PullCreditHistoryDTO> GetCreditHistoryDetailsByPullGuid(Guid PullGuid, Int64 RoomID, Int64 CompanyId)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@PullGuid", PullGuid), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyId) };
            string Qry = "EXEC dbo.[GetCreditHistoryDetailsByPullGuid] @PullGuid,@RoomID,@CompanyId";
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<PullCreditHistoryDTO>(Qry, params1).ToList();
            }
        }

        public bool UpdateCreditHistory(PullCreditHistoryDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (objDTO != null)
                {
                    CreditHistory obj = new CreditHistory();
                    obj = context.CreditHistories.FirstOrDefault(t => t.ID == objDTO.ID && t.GUID == objDTO.GUID);

                    if (obj != null && obj.ID > 0)
                    {
                        obj.CreditCustomerOwnedQuantity = objDTO.CreditCustomerOwnedQuantity.GetValueOrDefault(0);
                        obj.CreditConsignedQuantity = objDTO.CreditConsignedQuantity.GetValueOrDefault(0);
                        obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                        obj.Updated = DateTimeUtility.DateTimeNow;
                        obj.EditedFrom = objDTO.EditedFrom;
                        context.SaveChanges();
                    }
                }
                return true;
            }
        }

        public bool UpdatePullDetailsForCreditQuantity(PullCreditHistoryDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (objDTO != null)
                {
                    PullDetail obj = new PullDetail();
                    obj = context.PullDetails.FirstOrDefault(t => t.GUID == objDTO.PullDetailGuid);

                    if (obj != null && obj.ID > 0)
                    {
                        obj.CreditCustomerOwnedQuantity = objDTO.CreditCustomerOwnedQuantity.GetValueOrDefault(0);
                        obj.CreditConsignedQuantity = objDTO.CreditConsignedQuantity.GetValueOrDefault(0);
                        obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                        obj.Updated = DateTimeUtility.DateTimeNow;
                        obj.EditedFrom = objDTO.EditedFrom;
                        context.SaveChanges();
                    }
                }
                return true;
            }
        }

        public bool UpdatePullMasterForCreditQuantity(PullCreditHistoryDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (objDTO != null)
                {
                    List<PullDetailsDTO> lstPullDtl = GetPullDetailsByPullGuidPlain(objDTO.PULLGUID, objDTO.Room, objDTO.CompanyID);

                    if (lstPullDtl != null && lstPullDtl.Count > 0)
                    {
                        PullMaster obj = new PullMaster();
                        obj = context.PullMasters.FirstOrDefault(t => t.GUID == objDTO.PULLGUID);

                        if (obj != null && obj.ID > 0)
                        {
                            obj.CreditCustomerOwnedQuantity = lstPullDtl.Sum(x => x.CreditCustomerOwnedQuantity.GetValueOrDefault(0));
                            obj.CreditConsignedQuantity = lstPullDtl.Sum(x => x.CreditConsignedQuantity.GetValueOrDefault(0));
                            obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                            obj.Updated = DateTimeUtility.DateTimeNow;
                            obj.EditedFrom = objDTO.EditedFrom;
                            context.SaveChanges();
                        }
                    }
                }

                return true;
            }
        }

        #endregion

        #endregion  
    }
}


