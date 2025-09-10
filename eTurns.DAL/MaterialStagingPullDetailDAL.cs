using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public class MaterialStagingPullDetailDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        public MaterialStagingPullDetailDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public MaterialStagingPullDetailDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        public List<MaterialStagingPullDetailDTO> GetMsPullDetailsByMsDetailsId(Guid MaterialStagingDetailsGUID)
        {
            List<MaterialStagingPullDetailDTO> lstMaterialStagingPullDetailDTO = new List<MaterialStagingPullDetailDTO>();
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstMaterialStagingPullDetailDTO = (from u in context.MaterialStagingPullDetails
                                                   where u.IsArchived == false && u.IsDeleted == false && u.MaterialStagingdtlGUID == MaterialStagingDetailsGUID
                                                   select new MaterialStagingPullDetailDTO
                                                   {
                                                       ID = u.ID,
                                                       MaterialStagingGUID = u.MaterialStagingGUID,
                                                       MaterialStagingdtlGUID = u.MaterialStagingdtlGUID,
                                                       GUID = u.GUID,
                                                       ItemGUID = u.ItemGUID,
                                                       ItemCost = u.ItemCost,
                                                       CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                       ConsignedQuantity = u.ConsignedQuantity,
                                                       PoolQuantity = u.PoolQuantity,
                                                       SerialNumber = u.SerialNumber,
                                                       LotNumber = u.LotNumber,
                                                       Expiration = u.Expiration,
                                                       ExpirationDate = u.ExpirationDate,
                                                       Received = u.Received,
                                                       ReceivedDate = u.ReceivedDate,
                                                       BinID = u.BinID,
                                                       Created = u.Created,
                                                       Updated = u.Updated,
                                                       CreatedBy = u.CreatedBy,
                                                       LastUpdatedBy = u.LastUpdatedBy,
                                                       IsDeleted = u.IsDeleted,
                                                       IsArchived = u.IsArchived,
                                                       CompanyID = u.CompanyID,
                                                       Room = u.Room,
                                                       PullCredit = u.PullCredit,
                                                       ItemLocationDetailGUID = u.ItemLocationDetailGUID,
                                                       StagingBinId = u.StagingBinId,
                                                       OrderDetailGUID = u.OrderDetailGUID,
                                                       AddedFrom = u.AddedFrom,
                                                       EditedFrom = u.EditedFrom,
                                                       ReceivedOn = u.ReceivedOn,
                                                       ReceivedOnWeb = u.ReceivedOnWeb,
                                                       InitialQuantityWeb = u.InitialQuantityWeb ?? 0,
                                                       InitialQuantityPDA = u.InitialQuantityPDA ?? 0
                                                   }).ToList();
            }
            return lstMaterialStagingPullDetailDTO;
        }

        public List<MaterialStagingPullDetailDTO> GetMsPullDetailsByMsDetailsIdForUndelete(Guid MaterialStagingDetailsGUID)
        {
            List<MaterialStagingPullDetailDTO> lstMaterialStagingPullDetailDTO = new List<MaterialStagingPullDetailDTO>();
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstMaterialStagingPullDetailDTO = (from u in context.MaterialStagingPullDetails
                                                   where u.IsArchived == false && u.IsDeleted == true && u.MaterialStagingdtlGUID == MaterialStagingDetailsGUID
                                                   select new MaterialStagingPullDetailDTO
                                                   {
                                                       ID = u.ID,
                                                       MaterialStagingGUID = u.MaterialStagingGUID,
                                                       MaterialStagingdtlGUID = u.MaterialStagingdtlGUID,
                                                       GUID = u.GUID,
                                                       ItemGUID = u.ItemGUID,
                                                       ItemCost = u.ItemCost,
                                                       CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                       ConsignedQuantity = u.ConsignedQuantity,
                                                       PoolQuantity = u.PoolQuantity,
                                                       SerialNumber = u.SerialNumber,
                                                       LotNumber = u.LotNumber,
                                                       Expiration = u.Expiration,
                                                       Received = u.Received,
                                                       BinID = u.BinID,
                                                       Created = u.Created,
                                                       Updated = u.Updated,
                                                       CreatedBy = u.CreatedBy,
                                                       LastUpdatedBy = u.LastUpdatedBy,
                                                       IsDeleted = u.IsDeleted,
                                                       IsArchived = u.IsArchived,
                                                       CompanyID = u.CompanyID,
                                                       Room = u.Room,
                                                       PullCredit = u.PullCredit,
                                                       ItemLocationDetailGUID = u.ItemLocationDetailGUID,
                                                       StagingBinId = u.StagingBinId,
                                                       OrderDetailGUID = u.OrderDetailGUID,
                                                       AddedFrom = u.AddedFrom,
                                                       EditedFrom = u.EditedFrom,
                                                       ReceivedOn = u.ReceivedOn,
                                                       ReceivedOnWeb = u.ReceivedOnWeb,
                                                       InitialQuantityWeb = u.InitialQuantityWeb ?? 0,
                                                       InitialQuantityPDA = u.InitialQuantityPDA ?? 0
                                                   }).ToList();
            }
            return lstMaterialStagingPullDetailDTO;
        }
        public List<MaterialStagingPullDetailDTO> GetMsPullDetailsByItemGUIDANDBinID(Guid ItemGUID, long BinID, Int64 RoomID, Int64 CompanyID)
        {
            List<MaterialStagingPullDetailDTO> lstMaterialStagingPullDetailDTO = new List<MaterialStagingPullDetailDTO>();
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstMaterialStagingPullDetailDTO = (from u in context.MaterialStagingPullDetails
                                                   where u.IsArchived != true && u.IsDeleted != true && u.ItemGUID == ItemGUID && u.StagingBinId == BinID && u.Room == RoomID && u.CompanyID == CompanyID
                                                   select new MaterialStagingPullDetailDTO
                                                   {
                                                       ID = u.ID,
                                                       MaterialStagingGUID = u.MaterialStagingGUID,
                                                       MaterialStagingdtlGUID = u.MaterialStagingdtlGUID,
                                                       GUID = u.GUID,
                                                       ItemGUID = u.ItemGUID,
                                                       ItemCost = u.ItemCost,
                                                       CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                       ConsignedQuantity = u.ConsignedQuantity,
                                                       PoolQuantity = u.PoolQuantity,
                                                       SerialNumber = u.SerialNumber,
                                                       LotNumber = u.LotNumber,
                                                       Expiration = u.Expiration,
                                                       ExpirationDate = u.ExpirationDate,
                                                       Received = u.Received,
                                                       ReceivedDate = u.ReceivedDate,
                                                       BinID = u.BinID,
                                                       Created = u.Created,
                                                       Updated = u.Updated,
                                                       CreatedBy = u.CreatedBy,
                                                       LastUpdatedBy = u.LastUpdatedBy,
                                                       IsDeleted = u.IsDeleted,
                                                       IsArchived = u.IsArchived,
                                                       CompanyID = u.CompanyID,
                                                       Room = u.Room,
                                                       PullCredit = u.PullCredit,
                                                       ItemLocationDetailGUID = u.ItemLocationDetailGUID,
                                                       StagingBinId = u.StagingBinId,
                                                       OrderDetailGUID = u.OrderDetailGUID,
                                                       AddedFrom = u.AddedFrom,
                                                       EditedFrom = u.EditedFrom,
                                                       ReceivedOn = u.ReceivedOn,
                                                       ReceivedOnWeb = u.ReceivedOnWeb,
                                                       InitialQuantityWeb = u.InitialQuantityWeb ?? 0,
                                                       InitialQuantityPDA = u.InitialQuantityPDA ?? 0
                                                   }).ToList();
            }
            return lstMaterialStagingPullDetailDTO;
        }
        
        public List<MaterialStagingPullDetailDTO> GetMsPullDetailsByItemGUIDANDBinIDANDMSGUID(Guid ItemGUID, long BinID, Int64 RoomID, Int64 CompanyID, Guid MaterialStagingGUID)
        {
            List<MaterialStagingPullDetailDTO> lstMaterialStagingPullDetailDTO = new List<MaterialStagingPullDetailDTO>();
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGuid", ItemGUID), 
                                                   new SqlParameter("@StagingBinId", BinID),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@MaterialStagingGUID", MaterialStagingGUID) };

                lstMaterialStagingPullDetailDTO = (from u in context.Database.SqlQuery<MaterialStagingPullDetailDTO>("exec [GetMsPullDetailsByItemGUIDANDBinIDANDMSGUID] @ItemGuid,@StagingBinId,@RoomID,@CompanyID,@MaterialStagingGUID", params1)
                                                   select new MaterialStagingPullDetailDTO
                                                   {
                                                       ID = u.ID,
                                                       MaterialStagingGUID = u.MaterialStagingGUID,
                                                       MaterialStagingdtlGUID = u.MaterialStagingdtlGUID,
                                                       GUID = u.GUID,
                                                       ItemGUID = u.ItemGUID,
                                                       ItemCost = u.ItemCost,
                                                       CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                       ConsignedQuantity = u.ConsignedQuantity,
                                                       PoolQuantity = u.PoolQuantity,
                                                       SerialNumber = u.SerialNumber,
                                                       LotNumber = u.LotNumber,
                                                       Expiration = u.Expiration,
                                                       ExpirationDate = u.ExpirationDate,
                                                       Received = u.Received,
                                                       ReceivedDate = u.ReceivedDate,
                                                       BinID = u.BinID,
                                                       Created = u.Created,
                                                       Updated = u.Updated,
                                                       CreatedBy = u.CreatedBy,
                                                       LastUpdatedBy = u.LastUpdatedBy,
                                                       IsDeleted = u.IsDeleted,
                                                       IsArchived = u.IsArchived,
                                                       CompanyID = u.CompanyID,
                                                       Room = u.Room,
                                                       PullCredit = u.PullCredit,
                                                       ItemLocationDetailGUID = u.ItemLocationDetailGUID,
                                                       StagingBinId = u.StagingBinId,
                                                       OrderDetailGUID = u.OrderDetailGUID,
                                                       AddedFrom = u.AddedFrom,
                                                       EditedFrom = u.EditedFrom,
                                                       ReceivedOn = u.ReceivedOn,
                                                       ReceivedOnWeb = u.ReceivedOnWeb,
                                                       InitialQuantityWeb = u.InitialQuantityWeb ?? 0,
                                                       InitialQuantityPDA = u.InitialQuantityPDA ?? 0
                                                   }).ToList();
            }
            return lstMaterialStagingPullDetailDTO;
        }
        public List<MaterialStagingPullDetailDTO> GetMsPullDetailsByItemGUIDANDBinIDForLotSr(Guid ItemGUID, long BinID, Int64 RoomID, Int64 CompanyID, string LotNumber, string SerialNumber)
        {
            List<MaterialStagingPullDetailDTO> lstMaterialStagingPullDetailDTO = new List<MaterialStagingPullDetailDTO>();
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstMaterialStagingPullDetailDTO = (from u in context.MaterialStagingPullDetails
                                                   where u.IsArchived != true && u.IsDeleted != true && u.ItemGUID == ItemGUID && u.StagingBinId == BinID && u.Room == RoomID && u.CompanyID == CompanyID
                                                   && (u.SerialNumber == SerialNumber || SerialNumber.Equals(null) || SerialNumber.Equals(string.Empty)) 
                                                   && (u.LotNumber == LotNumber || LotNumber.Equals(null) || LotNumber.Equals(string.Empty))                                                   
                                                   select new MaterialStagingPullDetailDTO
                                                   {
                                                       ID = u.ID,
                                                       MaterialStagingGUID = u.MaterialStagingGUID,
                                                       MaterialStagingdtlGUID = u.MaterialStagingdtlGUID,
                                                       GUID = u.GUID,
                                                       ItemGUID = u.ItemGUID,
                                                       ItemCost = u.ItemCost,
                                                       CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                       ConsignedQuantity = u.ConsignedQuantity,
                                                       PoolQuantity = u.PoolQuantity,
                                                       SerialNumber = u.SerialNumber,
                                                       LotNumber = u.LotNumber,
                                                       Expiration = u.Expiration,
                                                       ExpirationDate = u.ExpirationDate,
                                                       Received = u.Received,
                                                       ReceivedDate = u.ReceivedDate,
                                                       BinID = u.BinID,
                                                       Created = u.Created,
                                                       Updated = u.Updated,
                                                       CreatedBy = u.CreatedBy,
                                                       LastUpdatedBy = u.LastUpdatedBy,
                                                       IsDeleted = u.IsDeleted,
                                                       IsArchived = u.IsArchived,
                                                       CompanyID = u.CompanyID,
                                                       Room = u.Room,
                                                       PullCredit = u.PullCredit,
                                                       ItemLocationDetailGUID = u.ItemLocationDetailGUID,
                                                       StagingBinId = u.StagingBinId,
                                                       OrderDetailGUID = u.OrderDetailGUID,
                                                       AddedFrom = u.AddedFrom,
                                                       EditedFrom = u.EditedFrom,
                                                       ReceivedOn = u.ReceivedOn,
                                                       ReceivedOnWeb = u.ReceivedOnWeb,
                                                       InitialQuantityWeb = u.InitialQuantityWeb ?? 0,
                                                       InitialQuantityPDA = u.InitialQuantityPDA ?? 0
                                                   }).ToList();
            }
            return lstMaterialStagingPullDetailDTO;
        }

        public List<MaterialStagingPullDetailDTO> GetMSLocationsDateCodeQty(Guid ItemGUID, long BinID, long RoomId, long CompanyId, DateTime ExpirationDate)
        {
            List<MaterialStagingPullDetailDTO> oItemLocations = new List<MaterialStagingPullDetailDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID),
                                                        new SqlParameter("@BinID", BinID),
                                                        new SqlParameter("@RoomID", RoomId),
                                                        new SqlParameter("@CompanyID", CompanyId ),
                                                        new SqlParameter("@ExpirationDate", ExpirationDate),
                                                        new SqlParameter("@IsStagingLocation", true) };
                oItemLocations = (from il in context.Database.SqlQuery<MaterialStagingPullDetailDTO>(@"exec GetItemsLocationsDateCodeQty @ItemGUID,@BinID,@RoomID,@CompanyID,@ExpirationDate,@IsStagingLocation", params1)
                                  select new MaterialStagingPullDetailDTO
                                  {
                                      ID = il.ID,
                                      GUID = il.GUID,
                                      CustomerOwnedQuantity = il.CustomerOwnedQuantity,
                                      ConsignedQuantity = il.ConsignedQuantity,
                                      ExpirationDate = il.ExpirationDate
                                  }).ToList();
            }
            return oItemLocations;
        }

        public List<MaterialStagingPullDetailDTO> GetStagingSerLotQtyForImport(Guid ItemGUID, long BinID, Int64 RoomID, Int64 CompanyID, string LotNumber, string SerialNumber, DateTime ExpirationDate)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetStagingSerLotQtyForImport] @ItemGUID,@BinID,@CompanyId,@RoomId,@LotNumber,@SerailNumber,@ExpirationDate";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@ItemGUID", ItemGUID),
                    new SqlParameter("@BinID", BinID),
                    new SqlParameter("@CompanyId", CompanyID),
                     new SqlParameter("@RoomId", RoomID),
                      new SqlParameter("@LotNumber",(string.IsNullOrWhiteSpace(LotNumber) ? (object)DBNull.Value : LotNumber)),
                       new SqlParameter("@SerailNumber",(string.IsNullOrWhiteSpace(SerialNumber) ? (object)DBNull.Value : SerialNumber)),
                        new SqlParameter("@ExpirationDate",ExpirationDate)
                };
                return context.Database.SqlQuery<MaterialStagingPullDetailDTO>(stryQry, params1).ToList();

            }
        }


        public List<MaterialStagingPullDetailDTO> GetMsPullDetailsByItemGUID(Guid ItemGUID, Int64 RoomID, Int64 CompanyID)
        {
            List<MaterialStagingPullDetailDTO> lstMaterialStagingPullDetailDTO = new List<MaterialStagingPullDetailDTO>();
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstMaterialStagingPullDetailDTO = (from u in context.MaterialStagingPullDetails
                                                   where u.IsArchived == false && u.IsDeleted == false && u.ItemGUID == ItemGUID && u.Room == RoomID && u.CompanyID == CompanyID
                                                   select new MaterialStagingPullDetailDTO
                                                   {
                                                       ID = u.ID,
                                                       MaterialStagingGUID = u.MaterialStagingGUID,
                                                       MaterialStagingdtlGUID = u.MaterialStagingdtlGUID,
                                                       GUID = u.GUID,
                                                       ItemGUID = u.ItemGUID,
                                                       ItemCost = u.ItemCost,
                                                       CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                       ConsignedQuantity = u.ConsignedQuantity,
                                                       PoolQuantity = u.PoolQuantity,
                                                       SerialNumber = u.SerialNumber,
                                                       LotNumber = u.LotNumber,
                                                       Expiration = u.Expiration,
                                                       ExpirationDate = u.ExpirationDate,
                                                       Received = u.Received,
                                                       ReceivedDate = u.ReceivedDate,
                                                       BinID = u.BinID,
                                                       Created = u.Created,
                                                       Updated = u.Updated,
                                                       CreatedBy = u.CreatedBy,
                                                       LastUpdatedBy = u.LastUpdatedBy,
                                                       IsDeleted = u.IsDeleted,
                                                       IsArchived = u.IsArchived,
                                                       CompanyID = u.CompanyID,
                                                       Room = u.Room,
                                                       PullCredit = u.PullCredit,
                                                       ItemLocationDetailGUID = u.ItemLocationDetailGUID,
                                                       StagingBinId = u.StagingBinId,
                                                       OrderDetailGUID = u.OrderDetailGUID,
                                                       AddedFrom = u.AddedFrom,
                                                       EditedFrom = u.EditedFrom,
                                                       ReceivedOn = u.ReceivedOn,
                                                       ReceivedOnWeb = u.ReceivedOnWeb,
                                                       InitialQuantityWeb = u.InitialQuantityWeb ?? 0,
                                                       InitialQuantityPDA = u.InitialQuantityPDA ?? 0
                                                   }).ToList();
            }
            return lstMaterialStagingPullDetailDTO;
        }

         
        public List<MaterialStagingPullDetailDTO> GetMsPullDetailsByItemGUID(Guid ItemGUID, Int64 RoomID, Int64 CompanyID, string LotNumber, string SerialNumber,string Expiration, string Received, Int64? StagingBinID, string MSPullDtlGUID, bool? IsDeleted)
        { 
            List<MaterialStagingPullDetailDTO> lstMaterialStagingPullDetailDTO = new List<MaterialStagingPullDetailDTO>();
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID),  
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@LotNumber", LotNumber),
                                                   new SqlParameter("@SerialNumber", SerialNumber),
                                                   new SqlParameter("@Expiration", Expiration),
                                                   new SqlParameter("@Received", Received),
                                                   new SqlParameter("@StagingBinID", StagingBinID ?? (object)DBNull.Value),
                                                   new SqlParameter("@MSPullDtlGUID", MSPullDtlGUID),
                                                   new SqlParameter("@IsDeleted", IsDeleted ?? (object)DBNull.Value) };

                lstMaterialStagingPullDetailDTO = (from u in context.Database.SqlQuery<MaterialStagingPullDetailDTO>("EXEC [GetMsPullDetailsByItemGUID] @ItemGUID,@RoomID,@CompanyID,@LotNumber,@SerialNumber,@Expiration,@Received,@StagingBinID,@MSPullDtlGUID,@IsDeleted", params1)
                                                   select new MaterialStagingPullDetailDTO
                                                   {
                                                       ID = u.ID,
                                                       MaterialStagingGUID = u.MaterialStagingGUID,
                                                       MaterialStagingdtlGUID = u.MaterialStagingdtlGUID,
                                                       GUID = u.GUID,
                                                       ItemGUID = u.ItemGUID,
                                                       ItemCost = u.ItemCost,
                                                       CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                       ConsignedQuantity = u.ConsignedQuantity,
                                                       PoolQuantity = u.PoolQuantity,
                                                       SerialNumber = u.SerialNumber,
                                                       LotNumber = u.LotNumber,
                                                       Expiration = u.Expiration,
                                                       ExpirationDate = u.ExpirationDate,
                                                       Received = u.Received,
                                                       ReceivedDate = u.ReceivedDate,
                                                       BinID = u.BinID,
                                                       Created = u.Created,
                                                       Updated = u.Updated,
                                                       CreatedBy = u.CreatedBy,
                                                       LastUpdatedBy = u.LastUpdatedBy,
                                                       IsDeleted = u.IsDeleted,
                                                       IsArchived = u.IsArchived,
                                                       CompanyID = u.CompanyID,
                                                       Room = u.Room,
                                                       PullCredit = u.PullCredit,
                                                       ItemLocationDetailGUID = u.ItemLocationDetailGUID,
                                                       StagingBinId = u.StagingBinId,
                                                       OrderDetailGUID = u.OrderDetailGUID,
                                                       AddedFrom = u.AddedFrom,
                                                       EditedFrom = u.EditedFrom,
                                                       ReceivedOn = u.ReceivedOn,
                                                       ReceivedOnWeb = u.ReceivedOnWeb,
                                                       InitialQuantityWeb = u.InitialQuantityWeb ?? 0,
                                                       InitialQuantityPDA = u.InitialQuantityPDA ?? 0
                                                   }).ToList();
            }
            return lstMaterialStagingPullDetailDTO;
        }

        /// <summary>
        /// Get Paged Records from the MaterialStagingPullDetail Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<MaterialStagingPullDetailDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId)
        {
            IEnumerable<MaterialStagingPullDetailDTO> lstMaterialStagingPullDetail = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyId) };
                lstMaterialStagingPullDetail = (from u in context.Database.SqlQuery<MaterialStagingPullDetailDTO>("exec [GetMaterialStagingPullDetailByCompanyRoomId] @RoomID,@CompanyID", params1)
                                                select new MaterialStagingPullDetailDTO
                                                {
                                                    ID = u.ID,
                                                    MaterialStagingGUID = u.MaterialStagingGUID,
                                                    MaterialStagingdtlGUID = u.MaterialStagingdtlGUID,
                                                    ItemGUID = u.ItemGUID,
                                                    ItemCost = u.ItemCost,
                                                    CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                    ConsignedQuantity = u.ConsignedQuantity,
                                                    PoolQuantity = u.PoolQuantity,
                                                    SerialNumber = u.SerialNumber,
                                                    LotNumber = u.LotNumber,
                                                    Expiration = u.Expiration == null && u.DateCodeTracking ? DateTime.Now.ToString("MM-dd-yy") : u.Expiration,
                                                    Received = u.Received == null ? DateTime.Now.ToString("MM-dd-yy") : u.Received,
                                                    BinID = u.BinID,
                                                    Created = u.Created,
                                                    Updated = u.Updated,
                                                    CreatedBy = u.CreatedBy,
                                                    LastUpdatedBy = u.LastUpdatedBy,
                                                    CreatedByName = u.CreatedByName,
                                                    UpdatedByName = u.UpdatedByName,
                                                    IsDeleted = u.IsDeleted == null ? false : u.IsDeleted,
                                                    IsArchived = u.IsArchived == null ? false : u.IsArchived,
                                                    CompanyID = u.CompanyID,
                                                    Room = u.Room,
                                                    PullCredit = u.PullCredit,
                                                    StagingBinId = u.StagingBinId,
                                                    GUID = u.GUID,
                                                    OrderDetailGUID = u.OrderDetailGUID,
                                                    IsConsignedSerialLot = u.IsConsignedSerialLot,
                                                    InitialQuantityWeb = u.InitialQuantityWeb ?? 0,
                                                    InitialQuantityPDA = u.InitialQuantityPDA ?? 0,
                                                    ItemLocationDetailGUID = u.ItemLocationDetailGUID,
                                                    AddedFrom = u.AddedFrom,
                                                    ReceivedOnWeb = u.ReceivedOnWeb,
                                                    ReceivedOn = u.ReceivedOn,
                                                    EditedFrom = u.EditedFrom
                                                }).AsParallel().ToList();
            }

            return lstMaterialStagingPullDetail;
            //return GetCachedData(RoomID, CompanyId).OrderBy("ID DESC");
        }
        
        public IEnumerable<MaterialStagingPullDetailDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, string MaterialStagingGUID, Int64? StagingBinId, string ItemGUID, bool? IsDeleted, bool? IsArchived, string OrderDetailGUID)
        {
            IEnumerable<MaterialStagingPullDetailDTO> lstMaterialStagingPullDetail = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {  new SqlParameter("@RoomID", RoomID),  
                                                    new SqlParameter("@CompanyID", CompanyId),
                                                    new SqlParameter("@MaterialStagingGUID", MaterialStagingGUID),
                                                    new SqlParameter("@StagingBinID", StagingBinId ?? (object)DBNull.Value),
                                                    new SqlParameter("@ItemGUID", ItemGUID),
                                                    new SqlParameter("@IsDeleted", IsDeleted ?? (object)DBNull.Value),
                                                    new SqlParameter("@IsArchived", IsArchived ?? (object)DBNull.Value),
                                                    new SqlParameter("@OrderDetailGUID", OrderDetailGUID) };

                lstMaterialStagingPullDetail = (from u in context.Database.SqlQuery<MaterialStagingPullDetailDTO>("exec [GetMaterialStagingPullDetailByCompanyRoomMSGUID] @RoomID,@CompanyID,@MaterialStagingGUID,@StagingBinID,@ItemGUID,@IsDeleted,@IsArchived,@OrderDetailGUID", params1)
                                                select new MaterialStagingPullDetailDTO
                                                {
                                                    ID = u.ID,
                                                    MaterialStagingGUID = u.MaterialStagingGUID,
                                                    MaterialStagingdtlGUID = u.MaterialStagingdtlGUID,
                                                    ItemGUID = u.ItemGUID,
                                                    ItemCost = u.ItemCost,
                                                    CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                    ConsignedQuantity = u.ConsignedQuantity,
                                                    PoolQuantity = u.PoolQuantity,
                                                    SerialNumber = u.SerialNumber,
                                                    LotNumber = u.LotNumber,
                                                    Expiration = u.Expiration == null && u.DateCodeTracking ? DateTime.Now.ToString("MM-dd-yy") : u.Expiration,
                                                    Received = u.Received == null ? DateTime.Now.ToString("MM-dd-yy") : u.Received,
                                                    BinID = u.BinID,
                                                    Created = u.Created,
                                                    Updated = u.Updated,
                                                    CreatedBy = u.CreatedBy,
                                                    LastUpdatedBy = u.LastUpdatedBy,
                                                    CreatedByName = u.CreatedByName,
                                                    UpdatedByName = u.UpdatedByName,
                                                    IsDeleted = u.IsDeleted == null ? false : u.IsDeleted,
                                                    IsArchived = u.IsArchived == null ? false : u.IsArchived,
                                                    CompanyID = u.CompanyID,
                                                    Room = u.Room,
                                                    PullCredit = u.PullCredit,
                                                    StagingBinId = u.StagingBinId,
                                                    GUID = u.GUID,
                                                    OrderDetailGUID = u.OrderDetailGUID,
                                                    IsConsignedSerialLot = u.IsConsignedSerialLot,
                                                    InitialQuantityWeb = u.InitialQuantityWeb ?? 0,
                                                    InitialQuantityPDA = u.InitialQuantityPDA ?? 0,
                                                    ItemLocationDetailGUID = u.ItemLocationDetailGUID,
                                                    AddedFrom = u.AddedFrom,
                                                    ReceivedOnWeb = u.ReceivedOnWeb,
                                                    ReceivedOn = u.ReceivedOn,
                                                    EditedFrom = u.EditedFrom
                                                }).AsParallel().ToList();
            }

            return lstMaterialStagingPullDetail;
            //return GetCachedData(RoomID, CompanyId).OrderBy("ID DESC");
        }

        public IEnumerable<MaterialStagingPullDetailDTO> GetAllRecords(Guid MsDetailsGUID, Int64 RoomID, Int64 CompanyId)
        {
            IEnumerable<MaterialStagingPullDetailDTO> lstMaterialStagingPullDetail = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyId), new SqlParameter("@MaterialStagingdtlGUID", MsDetailsGUID) };
                lstMaterialStagingPullDetail = (from u in context.Database.SqlQuery<MaterialStagingPullDetailDTO>("exec [GetMaterialStagingPullDetailByMSDGUId] @RoomID,@CompanyID,@MaterialStagingdtlGUID", params1)
                                                select new MaterialStagingPullDetailDTO
                                                {
                                                    ID = u.ID,
                                                    MaterialStagingGUID = u.MaterialStagingGUID,
                                                    MaterialStagingdtlGUID = u.MaterialStagingdtlGUID,
                                                    ItemGUID = u.ItemGUID,
                                                    ItemCost = u.ItemCost,
                                                    CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                    ConsignedQuantity = u.ConsignedQuantity,
                                                    PoolQuantity = u.PoolQuantity,
                                                    SerialNumber = u.SerialNumber,
                                                    LotNumber = u.LotNumber,
                                                    Expiration = u.Expiration == null && u.DateCodeTracking ? DateTime.Now.ToString("MM-dd-yy") : u.Expiration,
                                                    Received = u.Received == null ? DateTime.Now.ToString("MM-dd-yy") : u.Received,
                                                    BinID = u.BinID,
                                                    Created = u.Created,
                                                    Updated = u.Updated,
                                                    CreatedBy = u.CreatedBy,
                                                    LastUpdatedBy = u.LastUpdatedBy,
                                                    CreatedByName = u.CreatedByName,
                                                    UpdatedByName = u.UpdatedByName,
                                                    IsDeleted = u.IsDeleted == null ? false : u.IsDeleted,
                                                    IsArchived = u.IsArchived == null ? false : u.IsArchived,
                                                    CompanyID = u.CompanyID,
                                                    Room = u.Room,
                                                    PullCredit = u.PullCredit,
                                                    StagingBinId = u.StagingBinId,
                                                    GUID = u.GUID,
                                                    OrderDetailGUID = u.OrderDetailGUID,
                                                    IsConsignedSerialLot = u.IsConsignedSerialLot,
                                                    InitialQuantityWeb = u.InitialQuantityWeb ?? 0,
                                                    InitialQuantityPDA = u.InitialQuantityPDA ?? 0,
                                                    ItemLocationDetailGUID = u.ItemLocationDetailGUID,
                                                    AddedFrom = u.AddedFrom,
                                                    ReceivedOnWeb = u.ReceivedOnWeb,
                                                    ReceivedOn = u.ReceivedOn,
                                                    EditedFrom = u.EditedFrom
                                                }).AsParallel().ToList();
            }

            return lstMaterialStagingPullDetail;
            //return GetCachedData(RoomID, CompanyId).OrderBy("ID DESC").Where(t => t.MaterialStagingdtlGUID == MsDetailsGUID);
        }

        public IEnumerable<MaterialStagingPullDetailDTO> GetMsPullDetailsByMsDetailsId_Page(Guid MsDetailsGUID, Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, Guid ItemGUID)
        {
            IEnumerable<MaterialStagingPullDetailDTO> ObjCache = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@MsDetailsGUID", MsDetailsGUID), new SqlParameter("@ItemGUID", ItemGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };

                //ObjCache = (from u in context.Database.SqlQuery<MaterialStagingPullDetailDTO>(@"SELECT A.*,  I.CriticalQuantity,I.MinimumQuantity, I.MaximumQuantity, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,
                //    E.ItemNumber,E.Consignment,E.ItemType,E.SerialNumberTracking,E.LotNumberTracking,E.DateCodeTracking, I.BinNumber,E.Markup 
                //    FROM MaterialStagingPullDetail A 
                //    LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID 
                //    LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID 
                //    LEFT OUTER JOIN ItemMaster E on A.ItemGUID = E.GUID LEFT OUTER JOIN BinMaster I on A.BinID = I.ID  AND A.ItemGUID = I.ItemGUID 
                //    WHERE A.MaterialStagingdtlGUID = '" + MsDetailsGUID.ToString() + "' AND A.ITEMGUID = '" + ItemGUID.ToString() + "' AND A.Room = " + RoomID.ToString() + " AND A.CompanyID = " + CompanyID.ToString() + " ")
                ObjCache = (from u in context.Database.SqlQuery<MaterialStagingPullDetailDTO>("exec [GetMsPullDetailsByMsDetailsId] @MsDetailsGUID, @ItemGUID,@RoomID,@CompanyID", params1)
                            select new MaterialStagingPullDetailDTO
                            {
                                ID = u.ID,
                                BinID = u.BinID,
                                CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                ConsignedQuantity = u.ConsignedQuantity,
                                LotNumber = u.LotNumber,
                                SerialNumber = u.SerialNumber,
                                Expiration = u.Expiration == null && u.DateCodeTracking ? DateTime.Now.ToString("MM-dd-yy") : u.Expiration,
                                GUID = u.GUID,
                                MaterialStagingGUID = u.MaterialStagingGUID,
                                MaterialStagingdtlGUID = u.MaterialStagingdtlGUID,
                                ItemGUID = u.ItemGUID,
                                PoolQuantity = u.PoolQuantity,
                                Created = u.Created,
                                Updated = u.Updated,
                                CreatedBy = u.CreatedBy,
                                LastUpdatedBy = u.LastUpdatedBy,
                                CreatedByName = u.CreatedByName,
                                UpdatedByName = u.UpdatedByName,
                                InitialQuantityWeb = u.InitialQuantityWeb ?? 0,
                                InitialQuantityPDA = u.InitialQuantityPDA ?? 0
                            }).AsParallel().ToList();
            }

            if (String.IsNullOrEmpty(SearchTerm))
            {

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
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

                TotalCount = ObjCache.Where(t => t.ID.ToString().Contains(SearchTerm)).Count();
                return ObjCache.Where(t => t.ID.ToString().Contains(SearchTerm)).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }

        public MaterialStagingPullDetailDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            MaterialStagingPullDetailDTO objMaterialStagingPullDetail = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ID", id) };
                objMaterialStagingPullDetail = (from u in context.Database.SqlQuery<MaterialStagingPullDetailDTO>("exec [GetMaterialStagingPullDetailByID] @RoomID,@CompanyID,@ID", params1)
                                                select new MaterialStagingPullDetailDTO
                                                {
                                                    ID = u.ID,
                                                    MaterialStagingGUID = u.MaterialStagingGUID,
                                                    MaterialStagingdtlGUID = u.MaterialStagingdtlGUID,
                                                    ItemGUID = u.ItemGUID,
                                                    ItemCost = u.ItemCost,
                                                    CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                    ConsignedQuantity = u.ConsignedQuantity,
                                                    PoolQuantity = u.PoolQuantity,
                                                    SerialNumber = u.SerialNumber,
                                                    LotNumber = u.LotNumber,
                                                    Expiration = u.Expiration == null && u.DateCodeTracking ? DateTime.Now.ToString("MM-dd-yy") : u.Expiration,
                                                    Received = u.Received == null ? DateTime.Now.ToString("MM-dd-yy") : u.Received,
                                                    BinID = u.BinID,
                                                    Created = u.Created,
                                                    Updated = u.Updated,
                                                    CreatedBy = u.CreatedBy,
                                                    LastUpdatedBy = u.LastUpdatedBy,
                                                    CreatedByName = u.CreatedByName,
                                                    UpdatedByName = u.UpdatedByName,
                                                    IsDeleted = u.IsDeleted == null ? false : u.IsDeleted,
                                                    IsArchived = u.IsArchived == null ? false : u.IsArchived,
                                                    CompanyID = u.CompanyID,
                                                    Room = u.Room,
                                                    PullCredit = u.PullCredit,
                                                    StagingBinId = u.StagingBinId,
                                                    GUID = u.GUID,
                                                    OrderDetailGUID = u.OrderDetailGUID,
                                                    IsConsignedSerialLot = u.IsConsignedSerialLot,
                                                    InitialQuantityWeb = u.InitialQuantityWeb ?? 0,
                                                    InitialQuantityPDA = u.InitialQuantityPDA ?? 0,
                                                    ItemLocationDetailGUID = u.ItemLocationDetailGUID,
                                                    AddedFrom = u.AddedFrom,
                                                    ReceivedOnWeb = u.ReceivedOnWeb,
                                                    ReceivedOn = u.ReceivedOn,
                                                    EditedFrom = u.EditedFrom
                                                }).AsParallel().FirstOrDefault();
            }

            return objMaterialStagingPullDetail;

            // return GetCachedData(RoomID, CompanyID).Single(t => t.ID == id);
        }

        public MaterialStagingPullDetailDTO GetRecord(Guid id, Int64 RoomID, Int64 CompanyID)
        {
            MaterialStagingPullDetailDTO objMaterialStagingPullDetail = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@GUID", id) };
                objMaterialStagingPullDetail = (from u in context.Database.SqlQuery<MaterialStagingPullDetailDTO>("exec [GetMaterialStagingPullDetailByGUID] @RoomID,@CompanyID,@GUID", params1)
                                                select new MaterialStagingPullDetailDTO
                                                {
                                                    ID = u.ID,
                                                    MaterialStagingGUID = u.MaterialStagingGUID,
                                                    MaterialStagingdtlGUID = u.MaterialStagingdtlGUID,
                                                    ItemGUID = u.ItemGUID,
                                                    ItemCost = u.ItemCost,
                                                    CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                    ConsignedQuantity = u.ConsignedQuantity,
                                                    PoolQuantity = u.PoolQuantity,
                                                    SerialNumber = u.SerialNumber,
                                                    LotNumber = u.LotNumber,
                                                    Expiration = u.Expiration == null && u.DateCodeTracking ? DateTime.Now.ToString("MM-dd-yy") : u.Expiration,
                                                    Received = u.Received == null ? DateTime.Now.ToString("MM-dd-yy") : u.Received,
                                                    BinID = u.BinID,
                                                    Created = u.Created,
                                                    Updated = u.Updated,
                                                    CreatedBy = u.CreatedBy,
                                                    LastUpdatedBy = u.LastUpdatedBy,
                                                    CreatedByName = u.CreatedByName,
                                                    UpdatedByName = u.UpdatedByName,
                                                    IsDeleted = u.IsDeleted == null ? false : u.IsDeleted,
                                                    IsArchived = u.IsArchived == null ? false : u.IsArchived,
                                                    CompanyID = u.CompanyID,
                                                    Room = u.Room,
                                                    PullCredit = u.PullCredit,
                                                    StagingBinId = u.StagingBinId,
                                                    GUID = u.GUID,
                                                    OrderDetailGUID = u.OrderDetailGUID,
                                                    IsConsignedSerialLot = u.IsConsignedSerialLot,
                                                    InitialQuantityWeb = u.InitialQuantityWeb ?? 0,
                                                    InitialQuantityPDA = u.InitialQuantityPDA ?? 0,
                                                    ItemLocationDetailGUID = u.ItemLocationDetailGUID,
                                                    AddedFrom = u.AddedFrom,
                                                    ReceivedOnWeb = u.ReceivedOnWeb,
                                                    ReceivedOn = u.ReceivedOn,
                                                    EditedFrom = u.EditedFrom
                                                }).AsParallel().FirstOrDefault();
            }

            return objMaterialStagingPullDetail;
            //return GetCachedData(RoomID, CompanyID).Single(t => t.GUID == id);
        }

        /// <summary>
        /// Get Particullar Record from the MaterialStagingPullDetail by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public MaterialStagingPullDetailDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", id) };

                return (from u in context.Database.SqlQuery<MaterialStagingPullDetailDTO>("EXEC dbo.GetMaterialStagingPullDetailHistoryByID @ID", params1)
                        select new MaterialStagingPullDetailDTO
                        {
                            ID = u.ID,
                            MaterialStagingGUID = u.MaterialStagingGUID,
                            MaterialStagingdtlGUID = u.MaterialStagingdtlGUID,
                            GUID = u.GUID,
                            ItemGUID = u.ItemGUID,
                            ItemCost = u.ItemCost,
                            CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                            ConsignedQuantity = u.ConsignedQuantity,
                            PoolQuantity = u.PoolQuantity,
                            SerialNumber = u.SerialNumber,
                            LotNumber = u.LotNumber,
                            Expiration = u.Expiration,
                            Received = u.Received,
                            BinID = u.BinID,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            Room = u.Room,
                            PullCredit = u.PullCredit,
                            ItemLocationDetailGUID = u.ItemLocationDetailGUID,
                            StagingBinId = u.StagingBinId,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            InitialQuantityWeb = u.InitialQuantityWeb ?? 0,
                            InitialQuantityPDA = u.InitialQuantityPDA ?? 0
                        }).SingleOrDefault();
            }
        }

        /// <summary>
        /// Insert Record in the DataBase MaterialStagingPullDetail
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(MaterialStagingPullDetailDTO objDTO)
        {
            //objDTO.GUID = Guid.NewGuid();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                MaterialStagingPullDetail obj = new MaterialStagingPullDetail();
                obj.ID = 0;
                obj.MaterialStagingGUID = objDTO.MaterialStagingGUID;
                obj.MaterialStagingdtlGUID = objDTO.MaterialStagingdtlGUID;
                obj.GUID = objDTO.GUID;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.ItemCost = objDTO.ItemCost;
                obj.CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity;
                obj.ConsignedQuantity = objDTO.ConsignedQuantity;
                obj.PoolQuantity = objDTO.PoolQuantity;
                obj.SerialNumber = objDTO.SerialNumber;
                obj.LotNumber = objDTO.LotNumber;
                obj.Expiration = objDTO.Expiration;
                obj.ExpirationDate = objDTO.ExpirationDate;
                obj.Received = objDTO.Received;
                obj.ReceivedDate = objDTO.ReceivedDate;
                obj.BinID = objDTO.BinID;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.PullCredit = objDTO.PullCredit;
                obj.ItemLocationDetailGUID = objDTO.ItemLocationDetailGUID;
                obj.StagingBinId = objDTO.StagingBinId;
                obj.OrderDetailGUID = objDTO.OrderDetailGUID;
                obj.AddedFrom = (objDTO.AddedFrom == null ? "Web" : objDTO.AddedFrom);
                obj.EditedFrom = (objDTO.EditedFrom == null ? "Web" : objDTO.EditedFrom);
                obj.ReceivedOn = (objDTO.ReceivedOn);//objDTO.ReceivedOn == null ? DateTimeUtility.DateTimeNow : 
                obj.ReceivedOnWeb = (objDTO.ReceivedOnWeb);//objDTO.ReceivedOnWeb == null ? DateTimeUtility.DateTimeNow :
                obj.InitialQuantityWeb = ((objDTO.ConsignedQuantity ?? 0) + (objDTO.CustomerOwnedQuantity ?? 0));
                obj.InitialQuantityPDA = 0;
                context.MaterialStagingPullDetails.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;


                //if (objDTO.ID > 0)
                //{
                //    //Get Cached-Media
                //    IEnumerable<MaterialStagingPullDetailDTO> ObjCache = CacheHelper<IEnumerable<MaterialStagingPullDetailDTO>>.GetCacheItem("Cached_MaterialStagingPullDetail_" + objDTO.CompanyID.ToString());
                //    if (ObjCache != null)
                //    {
                //        List<MaterialStagingPullDetailDTO> tempC = new List<MaterialStagingPullDetailDTO>();
                //        tempC.Add(objDTO);

                //        IEnumerable<MaterialStagingPullDetailDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                //        CacheHelper<IEnumerable<MaterialStagingPullDetailDTO>>.AppendToCacheItem("Cached_MaterialStagingPullDetail_" + objDTO.CompanyID.ToString(), NewCache);
                //    }
                //}

                return obj.ID;
            }

        }

        /// <summary>
        /// Insert Record in the DataBase MaterialStagingPullDetail
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public MaterialStagingPullDetailDTO Insert(bool IsFromDirectCredit, MaterialStagingPullDetailDTO objDTO, string RoomDateFormat)
        {
            //objDTO.GUID = Guid.NewGuid();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                MaterialStagingPullDetail obj = new MaterialStagingPullDetail();
                obj.ID = 0;
                obj.MaterialStagingGUID = objDTO.MaterialStagingGUID;
                obj.MaterialStagingdtlGUID = objDTO.MaterialStagingdtlGUID;
                obj.GUID = objDTO.GUID;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.ItemCost = objDTO.ItemCost;
                obj.CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity;
                obj.ConsignedQuantity = objDTO.ConsignedQuantity;
                obj.PoolQuantity = objDTO.PoolQuantity;
                obj.SerialNumber = objDTO.SerialNumber;
                obj.LotNumber = objDTO.LotNumber;
                obj.Expiration = objDTO.Expiration;
                obj.Received = objDTO.Received;
                obj.BinID = objDTO.BinID;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.PullCredit = objDTO.PullCredit;
                obj.ItemLocationDetailGUID = objDTO.ItemLocationDetailGUID;
                obj.StagingBinId = objDTO.StagingBinId;
                obj.OrderDetailGUID = objDTO.OrderDetailGUID;
                obj.AddedFrom = (objDTO.AddedFrom == null ? "Web" : objDTO.AddedFrom);
                obj.EditedFrom = (objDTO.EditedFrom == null ? "Web" : objDTO.EditedFrom);
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                obj.InitialQuantityWeb = ((objDTO.ConsignedQuantity ?? 0) + (objDTO.CustomerOwnedQuantity ?? 0));
                obj.InitialQuantityPDA = 0;
                obj.IsConsignedSerialLot = objDTO.IsConsignedSerialLot;
                context.MaterialStagingPullDetails.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;

                if (objDTO.ID > 0)
                {
                    //Get Cached-Media
                    //IEnumerable<MaterialStagingPullDetailDTO> ObjCache = CacheHelper<IEnumerable<MaterialStagingPullDetailDTO>>.GetCacheItem("Cached_MaterialStagingPullDetail_" + objDTO.CompanyID.ToString());
                    //if (ObjCache != null)
                    //{
                    //    List<MaterialStagingPullDetailDTO> tempC = new List<MaterialStagingPullDetailDTO>();
                    //    tempC.Add(objDTO);

                    //    IEnumerable<MaterialStagingPullDetailDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    //    CacheHelper<IEnumerable<MaterialStagingPullDetailDTO>>.AppendToCacheItem("Cached_MaterialStagingPullDetail_" + objDTO.CompanyID.ToString(), NewCache);
                    //}

                    if (objDTO.OrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty || (objDTO.AddedFrom == "Web Transfer Staging" && objDTO.EditedFrom == "Web Transfer Staging"))
                    {
                        if (string.IsNullOrEmpty(objDTO.AddedFrom))
                        {
                            objDTO.AddedFrom = "Web";
                            objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            objDTO.EditedFrom = "Web";
                            objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        }
                        new ReceivedOrderTransferDetailDAL(base.DataBaseName).InsertMS(objDTO, RoomDateFormat);
                    }
                }

                return objDTO;
            }

        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(MaterialStagingPullDetailDTO objDTO)
        {
            MaterialStagingPullDetail objMaterialStagingPullDetail = new MaterialStagingPullDetail();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objMaterialStagingPullDetail = context.MaterialStagingPullDetails.FirstOrDefault(p => p.ID == objDTO.ID);
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                MaterialStagingPullDetail obj = new MaterialStagingPullDetail();
                obj.ID = objDTO.ID;
                obj.MaterialStagingGUID = objDTO.MaterialStagingGUID;
                obj.MaterialStagingdtlGUID = objDTO.MaterialStagingdtlGUID;
                obj.GUID = objDTO.GUID;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.ItemCost = objDTO.ItemCost;
                obj.CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity;
                obj.ConsignedQuantity = objDTO.ConsignedQuantity;
                obj.PoolQuantity = objDTO.PoolQuantity;
                obj.SerialNumber = objDTO.SerialNumber;
                obj.LotNumber = objDTO.LotNumber;
                obj.Expiration = objDTO.Expiration;
                obj.ExpirationDate = objDTO.ExpirationDate;
                obj.Received = objDTO.Received;
                obj.ReceivedDate = objDTO.ReceivedDate;
                obj.BinID = objDTO.BinID;
                obj.Created = objDTO.Created;
                obj.Updated = objDTO.Updated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = objDTO.IsDeleted != null ? objDTO.IsDeleted : false;
                obj.IsArchived = objDTO.IsArchived != null ? objDTO.IsArchived : false;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.PullCredit = objDTO.PullCredit;
                obj.MaterialStagingdtlGUID = objDTO.MaterialStagingdtlGUID;
                obj.StagingBinId = objDTO.StagingBinId;
                obj.OrderDetailGUID = objDTO.OrderDetailGUID;
                obj.AddedFrom = (objDTO.AddedFrom == null ? "Web" : objDTO.AddedFrom);
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                obj.ReceivedOn = objDTO.ReceivedOn;
                if (objMaterialStagingPullDetail != null && objMaterialStagingPullDetail.ID > 0)
                {
                    obj.ItemLocationDetailGUID = objMaterialStagingPullDetail.ItemLocationDetailGUID;
                    obj.InitialQuantityWeb = objMaterialStagingPullDetail.InitialQuantityWeb;
                    obj.InitialQuantityPDA = 0;
                }
                obj.EditedFrom = objDTO.EditedFrom == null ? "Web" : objDTO.EditedFrom;
                if (objDTO.IsOnlyFromItemUI)
                {
                    obj.EditedFrom = (objDTO.EditedFrom == null ? "Web" : objDTO.EditedFrom);
                    obj.ReceivedOn = objDTO.ReceivedOn;
                }

                context.MaterialStagingPullDetails.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                //Get Cached-Media
                //IEnumerable<MaterialStagingPullDetailDTO> ObjCache = CacheHelper<IEnumerable<MaterialStagingPullDetailDTO>>.GetCacheItem("Cached_MaterialStagingPullDetail_" + objDTO.CompanyID.ToString());
                //if (ObjCache != null)
                //{
                //    List<MaterialStagingPullDetailDTO> objTemp = ObjCache.ToList();
                //    objTemp.RemoveAll(i => i.ID == objDTO.ID);
                //    ObjCache = objTemp.AsEnumerable();

                //    List<MaterialStagingPullDetailDTO> tempC = new List<MaterialStagingPullDetailDTO>();
                //    tempC.Add(objDTO);
                //    IEnumerable<MaterialStagingPullDetailDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                //    CacheHelper<IEnumerable<MaterialStagingPullDetailDTO>>.AppendToCacheItem("Cached_MaterialStagingPullDetail_" + objDTO.CompanyID.ToString(), NewCache);
                //}

                if (objDTO.OrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                    new ReceivedOrderTransferDetailDAL(base.DataBaseName).EditMS(objDTO);


                return true;
            }
        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@IDs", IDs),
                                                   new SqlParameter("@UserID", userid),
                                                   new SqlParameter("@CompanyID", CompanyID) };

                context.Database.SqlQuery<int>("EXEC [DeleteMSPullDetail] @IDs,@UserID,@CompanyID", params1).FirstOrDefault();
                return true;
            }
        }

        public bool UnDeleteRecords(string IDs, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@IDs", IDs),
                                                   new SqlParameter("@UserID", userid),
                                                   new SqlParameter("@CompanyID", CompanyID) };

                context.Database.SqlQuery<int>("EXEC [UnDeleteMSPullDetail] @IDs,@UserID,@CompanyID", params1).FirstOrDefault();
                return true;
            }
        }

        public bool UnDeleteRecordsByGUID(string GUIDs, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@GUIDs", GUIDs),
                                                   new SqlParameter("@UserID", userid),
                                                   new SqlParameter("@CompanyID", CompanyID) };

                context.Database.SqlQuery<int>("EXEC [UnDeleteMSPullDetailByGUID] @GUIDs,@UserID,@CompanyID", params1).FirstOrDefault();
                return true;
            }
        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteRecordsFromOrder(string IDs, Int64 userid, Int64 RoomID, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Double Totalqty = 0;
                Guid MaterialStagingDetailGUID = Guid.Empty;
                
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        MaterialStagingPullDetailDTO objDTO = GetRecord(Guid.Parse(item), RoomID, CompanyID);
                        if (MaterialStagingDetailGUID == Guid.Empty)
                            MaterialStagingDetailGUID = objDTO.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty);

                        Totalqty += objDTO.ConsignedQuantity.GetValueOrDefault(0) + objDTO.CustomerOwnedQuantity.GetValueOrDefault(0);
                        
                        var params1 = new SqlParameter[] { new SqlParameter("@GUID", item.ToString()),
                                                           new SqlParameter("@UserID", userid),
                                                           new SqlParameter("@CompanyID", CompanyID) };

                        context.Database.SqlQuery<int>("EXEC [DeleteMSPullDetailFromOrder] @GUID,@UserID,@CompanyID,", params1).FirstOrDefault();
                    }
                }
                
                if (MaterialStagingDetailGUID != Guid.Empty)
                {
                    MaterialStagingDetailDAL detailDAL = new MaterialStagingDetailDAL(base.DataBaseName);
                    //MaterialStagingDetailDTO objDetailDTO = detailDAL.GetRecord(MaterialStagingDetailGUID, RoomID, CompanyID);
                    MaterialStagingDetailDTO objDetailDTO = detailDAL.GetMaterialStagingDetailByGUID(MaterialStagingDetailGUID, RoomID, CompanyID);
                    if (Totalqty <= objDetailDTO.Quantity)
                        objDetailDTO.Quantity = objDetailDTO.Quantity - Totalqty;
                    else
                        objDetailDTO.Quantity = 0;

                    detailDAL.Edit(objDetailDTO);
                }
                return true;
            }
        }

        public void UpdateStagedQuantity(Guid ITEMGUID, Int64 RoomID, Int64 CompanyID, long SessionUserId,long EnterpriseId)
        {
            ItemMasterDAL objItem = new ItemMasterDAL(base.DataBaseName);
            Double StagedQuantity = 0;
            ItemMasterDTO ItemDTO = objItem.GetItemWithoutJoins(null, ITEMGUID);
            if (ItemDTO != null)
            {
                List<MaterialStagingPullDetailDTO> objReturn = GetMsPullDetailsByItemGUID(ITEMGUID, RoomID, CompanyID).ToList();
                objReturn.Select(c => { StagedQuantity += (c.ConsignedQuantity.GetValueOrDefault(0) + c.CustomerOwnedQuantity.GetValueOrDefault(0)); return c; }).ToList();
                ItemDTO.StagedQuantity = StagedQuantity;
                ItemDTO.WhatWhereAction = "Staging";
                objItem.Edit(ItemDTO, SessionUserId,EnterpriseId);
            }
        }

        public void UpdateStagedQuantity(Guid ITEMGUID, Int64 RoomID, Int64 CompanyID, string WhatWhereAction, long SessionUserId,long EnterpriseId)
        {
            ItemMasterDAL objItem = new ItemMasterDAL(base.DataBaseName);
            Double StagedQuantity = 0;
            ItemMasterDTO ItemDTO = objItem.GetItemWithoutJoins(null, ITEMGUID);
            if (ItemDTO != null)
            {
                List<MaterialStagingPullDetailDTO> objReturn = GetMsPullDetailsByItemGUID(ITEMGUID, RoomID, CompanyID).ToList();
                objReturn.Select(c => { StagedQuantity += (c.ConsignedQuantity.GetValueOrDefault(0) + c.CustomerOwnedQuantity.GetValueOrDefault(0)); return c; }).ToList();
                ItemDTO.StagedQuantity = StagedQuantity;
                ItemDTO.WhatWhereAction = "Staging";
                if (!string.IsNullOrEmpty(WhatWhereAction))
                {
                    ItemDTO.WhatWhereAction = WhatWhereAction;
                }
                objItem.Edit(ItemDTO, SessionUserId,EnterpriseId);
            }
        }

        public IEnumerable<MaterialStagingPullDetailDTO> GetCountDifferenceforValidatMSPull(Guid ItemGUID, Int64 RoomID, Int64 CompanyID, Int64 StagingBinId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramA = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@ItemGUID", ItemGUID), new SqlParameter("@StagingBinId", StagingBinId) };
                //, new SqlParameter("@SerialNumber", SerialNumber ?? (object)DBNull.Value), new SqlParameter("@LotNumber", LotNumber ?? (object)DBNull.Value) };,@SerialNumber,@LotNumber

                IEnumerable<MaterialStagingPullDetailDTO> obj = context.Database.SqlQuery<MaterialStagingPullDetailDTO>("exec [GetMSPullDtlWithCountQuantity] @CompanyID,@RoomID,@ItemGUID,@StagingBinId", paramA).ToList();
                //select new MaterialStagingPullDetailDTO
                //{
                //    ItemGUID = u.ItemGUID,
                //    StagingBinId = u.StagingBinId,
                //    LotNumber = u.LotNumber,
                //    SerialNumber = u.SerialNumber,
                //    ExpirationDate = u.ExpirationDate,
                //    CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                //    ConsignedQuantity = u.ConsignedQuantity
                //}).AsParallel().ToList();
                return obj;
            }
        }

        public List<ItemLocationLotSerialDTO> GetMaterialStagingPullWithLotSerialsForMove(Guid ItemGUID, long RoomId, long CompanyId)
        {
            List<ItemLocationLotSerialDTO> lstItemLocations = new List<ItemLocationLotSerialDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstItemLocations = (from mspd in context.MaterialStagingPullDetails
                                    join im in context.ItemMasters on mspd.ItemGUID equals im.GUID
                                    join bm in context.BinMasters on mspd.StagingBinId equals bm.ID
                                    where mspd.ItemGUID == ItemGUID && mspd.Room == RoomId && mspd.CompanyID == CompanyId
                                    && (((mspd.CustomerOwnedQuantity ?? 0) + (mspd.ConsignedQuantity ?? 0)) > 0)
                                    && (mspd.IsDeleted ?? false) == false && (mspd.IsArchived ?? false) == false
                                    group new
                                    {
                                        mspd.ItemGUID,
                                        bm.BinNumber,
                                        mspd.StagingBinId,
                                        mspd.SerialNumber,
                                        mspd.LotNumber,
                                        mspd.ExpirationDate,
                                        im.LotNumberTracking,
                                        im.SerialNumberTracking,
                                        im.DateCodeTracking,
                                        mspd.ConsignedQuantity,
                                        mspd.CustomerOwnedQuantity,
                                        mspd.MaterialStagingGUID
                                    } by new
                                    {
                                        mspd.ItemGUID,
                                        bm.BinNumber,
                                        mspd.StagingBinId,
                                        SerialNumber = mspd.SerialNumber == null ? "" : mspd.SerialNumber,
                                        LotNumber = mspd.LotNumber == null ? "" : mspd.LotNumber,
                                        ExpirationDate = mspd.ExpirationDate == null ? DateTime.MinValue : mspd.ExpirationDate,
                                        im.LotNumberTracking,
                                        im.SerialNumberTracking,
                                        im.DateCodeTracking,
                                        mspd.MaterialStagingGUID
                                    } into GroupLocation
                                    select new ItemLocationLotSerialDTO
                                    {
                                        ItemGUID = GroupLocation.Key.ItemGUID,
                                        BinNumber = GroupLocation.Key.BinNumber,
                                        BinID = GroupLocation.Key.StagingBinId,
                                        ID = GroupLocation.Key.StagingBinId,
                                        ConsignedQuantity = GroupLocation.Sum(x => x.ConsignedQuantity ?? 0),
                                        CustomerOwnedQuantity = GroupLocation.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                        SerialNumber = GroupLocation.Key.SerialNumber,
                                        LotNumber = GroupLocation.Key.LotNumber,
                                        LotNumberTracking = GroupLocation.Key.LotNumberTracking,
                                        SerialNumberTracking = GroupLocation.Key.SerialNumberTracking,
                                        DateCodeTracking = GroupLocation.Key.DateCodeTracking,
                                        LotSerialQuantity = (GroupLocation.Sum(x => x.CustomerOwnedQuantity ?? 0)) + (GroupLocation.Sum(x => x.ConsignedQuantity ?? 0)),
                                        QuantityToMove = (GroupLocation.Sum(x => x.CustomerOwnedQuantity ?? 0)) + (GroupLocation.Sum(x => x.ConsignedQuantity ?? 0)),
                                        LotOrSerailNumber = GroupLocation.Key.LotNumberTracking ? GroupLocation.Key.LotNumber : GroupLocation.Key.SerialNumberTracking ? GroupLocation.Key.SerialNumber : string.Empty,
                                        MaterialStagingGUID = GroupLocation.Key.MaterialStagingGUID
                                    }).ToList();

                //strExpirationDate = GroupLocation.Key.DateCodeTracking ? (SqlFunctions.StringConvert((double)GroupLocation.Key.ExpirationDate.Value.Month).TrimStart() + "/" + SqlFunctions.StringConvert((double)GroupLocation.Key.ExpirationDate.Value.Day).TrimStart() + "/" + SqlFunctions.StringConvert((double)GroupLocation.Key.ExpirationDate.Value.Year).TrimStart()) : string.Empty,


                //                        SerialLotExpirationcombin = GroupLocation.Key.SerialNumberTracking && GroupLocation.Key.DateCodeTracking ?
                //                                                            (GroupLocation.Key.SerialNumber + "_" + (SqlFunctions.StringConvert((double)GroupLocation.Key.ExpirationDate.Value.Month).TrimStart() + "/" + SqlFunctions.StringConvert((double)GroupLocation.Key.ExpirationDate.Value.Day).TrimStart() + "/" + SqlFunctions.StringConvert((double)GroupLocation.Key.ExpirationDate.Value.Year).TrimStart()))
                //                                                            :
                //                                                            (GroupLocation.Key.LotNumberTracking && GroupLocation.Key.DateCodeTracking ?
                //                                                            (GroupLocation.Key.LotNumber + "_" + (SqlFunctions.StringConvert((double)GroupLocation.Key.ExpirationDate.Value.Month).TrimStart() + "/" + SqlFunctions.StringConvert((double)GroupLocation.Key.ExpirationDate.Value.Day).TrimStart() + "/" + SqlFunctions.StringConvert((double)GroupLocation.Key.ExpirationDate.Value.Year).TrimStart()))
                //                                                                    : (SqlFunctions.StringConvert((double)GroupLocation.Key.ExpirationDate.Value.Month).TrimStart() + "/" + SqlFunctions.StringConvert((double)GroupLocation.Key.ExpirationDate.Value.Day).TrimStart() + "/" + SqlFunctions.StringConvert((double)GroupLocation.Key.ExpirationDate.Value.Year).TrimStart()))
            }

            foreach (ItemLocationLotSerialDTO objItemLocation in lstItemLocations)
            {
                if (objItemLocation.DateCodeTracking && objItemLocation.ExpirationDate != null)
                {
                    objItemLocation.strExpirationDate = objItemLocation.ExpirationDate.Value.ToShortDateString();
                }
                if (objItemLocation.SerialNumberTracking && objItemLocation.DateCodeTracking && objItemLocation.ExpirationDate != null)
                {
                    objItemLocation.SerialLotExpirationcombin = objItemLocation.SerialNumber + "_" + objItemLocation.ExpirationDate.Value.ToShortDateString();
                }
                else if (objItemLocation.LotNumberTracking && objItemLocation.DateCodeTracking && objItemLocation.ExpirationDate != null)
                {
                    objItemLocation.SerialLotExpirationcombin = objItemLocation.LotNumber + "_" + objItemLocation.ExpirationDate.Value.ToShortDateString();
                }
                else
                {
                    if (objItemLocation.ExpirationDate != null)
                    {
                        objItemLocation.SerialLotExpirationcombin = objItemLocation.ExpirationDate.Value.ToShortDateString();
                    }
                }
            }

            return lstItemLocations;
        }


        /// <summary>
        /// Set ItemLocationDetail
        /// WI-1298
        /// </summary>
        /// <param name="rotd"></param>
        public MaterialStagingPullDetail SetMSPullDetail(MaterialStagingPullDetail objILD, ReceivedOrderTransferDetailDTO objNewDTO, double PrevReciveCustOwnedQty, double PrevReceivedConsQty, OrderType ot, bool IsBinDeleted)
        {
            OrderDetailsDTO ordDetailDTO = null;
            OrderDetailsDAL ordDetailDAL = new OrderDetailsDAL(base.DataBaseName);
            ordDetailDTO = ordDetailDAL.GetOrderDetailByGuidNormal(objNewDTO.OrderDetailGUID.GetValueOrDefault(Guid.Empty), objNewDTO.Room.GetValueOrDefault(0), objNewDTO.CompanyID.GetValueOrDefault(0));
            //OrderMasterDTO ordDTO = null;
            //OrderMasterDAL ordDAL = new OrderMasterDAL(base.DataBaseName);
            //ordDTO = ordDAL.GetOrderByGuidPlain(ordDetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty));

            if (ordDetailDTO.OrderType.GetValueOrDefault(1) == 1 && !IsBinDeleted)
            {
                objILD.ConsignedQuantity = (objILD.ConsignedQuantity.GetValueOrDefault(0) - PrevReceivedConsQty) + objNewDTO.ConsignedQuantity.GetValueOrDefault(0);
                objILD.CustomerOwnedQuantity = (objILD.CustomerOwnedQuantity.GetValueOrDefault(0) - PrevReciveCustOwnedQty) + objNewDTO.CustomerOwnedQuantity;
            }
            else if (ordDetailDTO.OrderType.GetValueOrDefault(1) == 2 && !IsBinDeleted)
            {
                objILD.ConsignedQuantity = (objILD.ConsignedQuantity.GetValueOrDefault(0) + PrevReceivedConsQty) - objNewDTO.ConsignedQuantity.GetValueOrDefault(0);
                objILD.CustomerOwnedQuantity = (objILD.CustomerOwnedQuantity.GetValueOrDefault(0) + PrevReciveCustOwnedQty) - objNewDTO.CustomerOwnedQuantity;
            }

            if (!string.IsNullOrEmpty(objNewDTO.Expiration))
                objILD.Expiration = objNewDTO.Expiration.Replace("-", "/");

            if (!string.IsNullOrEmpty(objNewDTO.Received))
                objILD.Received = objNewDTO.Received.Replace("-", "/");

            objILD.LotNumber = (!string.IsNullOrWhiteSpace(objNewDTO.LotNumber)) ? objNewDTO.LotNumber.Trim() : string.Empty;
            objILD.SerialNumber = (!string.IsNullOrWhiteSpace(objNewDTO.SerialNumber)) ? objNewDTO.SerialNumber.Trim() : string.Empty;
            objILD.BinID = objNewDTO.BinID;
            objILD.ItemCost = objNewDTO.Cost;
            objILD.LastUpdatedBy = objNewDTO.LastUpdatedBy;
            objILD.Updated = DateTimeUtility.DateTimeNow;
            objILD.EditedFrom = "EditReceipt";

            if (IsBinDeleted)
            {
                objILD.EditedFrom = "EditReceipt-ForDeletedBin";
            }

            objILD.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

            return objILD;
        }


        public List<MaterialStagingPullDetailDTO> GetStagePullWithDetails(List<MaterialStagingPullDetailDTO> lstPullInfo, long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                Room objRoom = context.Rooms.FirstOrDefault(p => p.ID == RoomId);
                if (lstPullInfo != null)
                {
                    lstPullInfo.ForEach(t =>
                    {
                        ItemMaster objItem = context.ItemMasters.FirstOrDefault(p => p.GUID == t.ItemGUID);
                        BinMaster objBin = new BinMaster();
                        if (t.BinID.HasValue && (t.BinID ?? 0) > 0)
                        {
                            objBin = context.BinMasters.FirstOrDefault(p => p.ID == t.BinID);
                            if (objBin == null)
                                objBin = new BinMaster();
                        }

                        if (objItem != null)
                        {
                            t.SerialNumberTracking = objItem.SerialNumberTracking;
                            t.LotNumberTracking = objItem.LotNumberTracking;
                            t.DateCodeTracking = objItem.DateCodeTracking;
                            t.ItemType = objItem.ItemType;
                            t.Consignment = objItem.Consignment;
                            t.ItemNumber = objItem.ItemNumber;
                            t.BinNumber = objBin.BinNumber;
                            t.InventoryConsuptionMethod = objRoom.InventoryConsuptionMethod;
                        }

                    });
                }
                if (lstPullInfo == null)
                {
                    lstPullInfo = new List<MaterialStagingPullDetailDTO>();
                }
                return lstPullInfo;

            }

        }

        public List<MaterialStagingPullDetailDTO> GetRecordsByBinNumberAndLotSerial(Guid ItemGuid, string BinNumber, string LotSerialNumber, Int64 RoomID, Int64 CompanyID)
        {
            //Need to changes...
            //return GetCachedData(0,RoomID, CompanyID).Where(t => t.ID == id).SingleOrDefault();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<MaterialStagingPullDetailDTO> lstItemLocationDetailsDTO = new List<MaterialStagingPullDetailDTO>();

                var params1 = new SqlParameter[] { new SqlParameter("@ItemGuid", Convert.ToString(ItemGuid)),
                                                   new SqlParameter("@BinNumber", BinNumber),
                                                   new SqlParameter("@LotSerialNumber", LotSerialNumber),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID) };

                lstItemLocationDetailsDTO = (from u in context.Database.SqlQuery<MaterialStagingPullDetailDTO>("EXEC [GetMSPullDtlByBinNumberAndLotSerial] @ItemGuid,@BinNumber,@LotSerialNumber,@RoomID,@CompanyID", params1)
                                             select new MaterialStagingPullDetailDTO
                                             {
                                                 ID = u.ID,

                                                 BinID = u.BinID,
                                                 CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                 ConsignedQuantity = u.ConsignedQuantity,
                                                 LotNumber = u.LotNumber,
                                                 SerialNumber = u.SerialNumber,
                                                 Expiration = u.Expiration == null && u.DateCodeTracking ? DateTime.Now.ToString("MM /dd/yy") : u.Expiration,
                                                 Received = u.Received == null ? DateTime.Now.ToString("MM/dd/yy") : u.Received,
                                                 ItemCost = u.ItemCost,
                                                 GUID = u.GUID,
                                                 ItemGUID = u.ItemGUID,
                                                 Created = u.Created,
                                                 Updated = u.Updated,
                                                 CreatedBy = u.CreatedBy,
                                                 LastUpdatedBy = u.LastUpdatedBy,
                                                 IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                 IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
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
                                                 MaterialStagingdtlGUID = u.MaterialStagingdtlGUID,
                                                 MaterialStagingGUID = u.MaterialStagingGUID,
                                                 AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                                                 EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                                                 ReceivedOn = u.ReceivedOn,
                                                 ReceivedOnWeb = u.ReceivedOnWeb,
                                                 InitialQuantityWeb = u.InitialQuantityWeb ?? 0,
                                                 InitialQuantityPDA = u.InitialQuantityPDA ?? 0
                                             }).ToList();

                return lstItemLocationDetailsDTO;
            }
        }

        public List<MaterialStagingPullDetailDTO> GetRecordsByBinNumberAndDateCode(Guid ItemGuid, string BinNumber, DateTime ExpirationDate, Int64 RoomID, Int64 CompanyID)
        {
            //Need to changes...
            //return GetCachedData(0,RoomID, CompanyID).Where(t => t.ID == id).SingleOrDefault();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<MaterialStagingPullDetailDTO> lstItemLocationDetailsDTO = new List<MaterialStagingPullDetailDTO>();
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGuid", ItemGuid),
                                                        new SqlParameter("@BinNumber", BinNumber),
                                                        new SqlParameter("@ExpirationDate", ExpirationDate),
                                                        new SqlParameter("@RoomID", RoomID),
                                                        new SqlParameter("@CompanyID", CompanyID ),
                                                        new SqlParameter("@IsStagingLocation", true) };
                lstItemLocationDetailsDTO = (from u in context.Database.SqlQuery<MaterialStagingPullDetailDTO>(@"exec GetRecordsByBinNumberAndDateCode @ItemGuid,@BinNumber,@ExpirationDate,@RoomID,@CompanyID,@IsStagingLocation", params1)

                                             select new MaterialStagingPullDetailDTO
                                             {
                                                 ID = u.ID,

                                                 BinID = u.BinID,
                                                 CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                 ConsignedQuantity = u.ConsignedQuantity,
                                                 LotNumber = u.LotNumber,
                                                 SerialNumber = u.SerialNumber,
                                                 Expiration = u.Expiration == null && u.DateCodeTracking ? DateTime.Now.ToString("MM /dd/yy") : u.Expiration,
                                                 Received = u.Received == null ? DateTime.Now.ToString("MM/dd/yy") : u.Received,
                                                 ItemCost = u.ItemCost,
                                                 GUID = u.GUID,
                                                 ItemGUID = u.ItemGUID,
                                                 Created = u.Created,
                                                 Updated = u.Updated,
                                                 CreatedBy = u.CreatedBy,
                                                 LastUpdatedBy = u.LastUpdatedBy,
                                                 IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                 IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
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
                                                 MaterialStagingdtlGUID = u.MaterialStagingdtlGUID,
                                                 MaterialStagingGUID = u.MaterialStagingGUID,
                                                 AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                                                 EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                                                 ReceivedOn = u.ReceivedOn,
                                                 ReceivedOnWeb = u.ReceivedOnWeb,
                                                 InitialQuantityWeb = u.InitialQuantityWeb ?? 0,
                                                 InitialQuantityPDA = u.InitialQuantityPDA ?? 0
                                             }).ToList();

                return lstItemLocationDetailsDTO;
            }
        }


        #region For MS CREDIT

        public bool CheckSerialExistsOrNot(string SerialNumber, Guid ItemGuid, Int64 RoomId, Int64 CompanyID)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@SerialNumber", SerialNumber), new SqlParameter("@ItemGuid", ItemGuid), new SqlParameter("@RoomId", RoomId), new SqlParameter("@CompanyID", CompanyID) };
                    if (context.Database.SqlQuery<Double>("exec [CheckSerialExistsOrNotForMS] @SerialNumber,@ItemGuid,@RoomId,@CompanyID", params1).FirstOrDefault() > 0)
                    {
                        return false;
                    }
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public MaterialStagingPullDetailDTO InsertMS(bool IsFromDirectCredit, MaterialStagingPullDetailDTO objDTO, string RoomDateFormat)
        {
            //objDTO.GUID = Guid.NewGuid();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Guid newGUID = Guid.NewGuid();

                MaterialStagingPullDetail obj = new MaterialStagingPullDetail();
                obj.ID = 0;
                obj.MaterialStagingGUID = objDTO.MaterialStagingGUID;
                obj.MaterialStagingdtlGUID = objDTO.MaterialStagingdtlGUID;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.ItemCost = objDTO.ItemCost;
                obj.CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity;
                obj.ConsignedQuantity = objDTO.ConsignedQuantity;
                obj.PoolQuantity = objDTO.PoolQuantity;
                obj.SerialNumber = objDTO.SerialNumber;
                obj.LotNumber = objDTO.LotNumber;
                obj.Expiration = objDTO.Expiration;
                obj.ExpirationDate = objDTO.ExpirationDate;
                obj.ReceivedDate = objDTO.ReceivedDate;
                obj.Received = objDTO.Received;
                obj.BinID = objDTO.BinID;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.PullCredit = objDTO.PullCredit;
                obj.ItemLocationDetailGUID = null;
                obj.StagingBinId = objDTO.StagingBinId;
                obj.GUID = newGUID;
                obj.OrderDetailGUID = objDTO.OrderDetailGUID;
                obj.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.AddedFrom = (objDTO.AddedFrom == null ? "Web" : objDTO.AddedFrom);
                obj.EditedFrom = (objDTO.EditedFrom == null ? "Web" : objDTO.EditedFrom);
                //obj.ExpirationDate = DateTimeUtility.DateTimeNow;
                //obj.ReceivedDate = DateTimeUtility.DateTimeNow;
                obj.IsConsignedSerialLot = objDTO.IsConsignedSerialLot;
                obj.InitialQuantityWeb = ((objDTO.ConsignedQuantity ?? 0) + (objDTO.CustomerOwnedQuantity ?? 0));
                obj.InitialQuantityPDA = 0;
                context.MaterialStagingPullDetails.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;

                if (objDTO.ID > 0)
                {
                    //Get Cached-Media
                    //IEnumerable<MaterialStagingPullDetailDTO> ObjCache = CacheHelper<IEnumerable<MaterialStagingPullDetailDTO>>.GetCacheItem("Cached_MaterialStagingPullDetail_" + objDTO.CompanyID.ToString());
                    //if (ObjCache != null)
                    //{
                    //    List<MaterialStagingPullDetailDTO> tempC = new List<MaterialStagingPullDetailDTO>();
                    //    tempC.Add(objDTO);

                    //    IEnumerable<MaterialStagingPullDetailDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    //    CacheHelper<IEnumerable<MaterialStagingPullDetailDTO>>.AppendToCacheItem("Cached_MaterialStagingPullDetail_" + objDTO.CompanyID.ToString(), NewCache);
                    //}

                    if (objDTO.OrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                    {
                        if (string.IsNullOrEmpty(objDTO.AddedFrom))
                        {
                            objDTO.AddedFrom = "Web";
                            objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            objDTO.EditedFrom = "Web";
                            objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        }
                        new ReceivedOrderTransferDetailDAL(base.DataBaseName).InsertMS(objDTO, RoomDateFormat);
                    }
                }

                return objDTO;
            }

        }

        public MaterialStagingPullDetailDTO GetMsPullDetailsByMsPullDetailsGUID(Guid MsPullDetailsGUID)
        {
            MaterialStagingPullDetailDTO lstMaterialStagingPullDetailDTO = new MaterialStagingPullDetailDTO();
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstMaterialStagingPullDetailDTO = (from u in context.MaterialStagingPullDetails
                                                   where u.IsArchived == false && u.IsDeleted == false && u.GUID == MsPullDetailsGUID
                                                   select new MaterialStagingPullDetailDTO
                                                   {
                                                       ID = u.ID,
                                                       MaterialStagingGUID = u.MaterialStagingGUID,
                                                       MaterialStagingdtlGUID = u.MaterialStagingdtlGUID,
                                                       GUID = u.GUID,
                                                       ItemGUID = u.ItemGUID,
                                                       ItemCost = u.ItemCost,
                                                       CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                       ConsignedQuantity = u.ConsignedQuantity,
                                                       PoolQuantity = u.PoolQuantity,
                                                       SerialNumber = u.SerialNumber,
                                                       LotNumber = u.LotNumber,
                                                       Expiration = u.Expiration,
                                                       ExpirationDate = u.ExpirationDate,
                                                       Received = u.Received,
                                                       ReceivedDate = u.ReceivedDate,
                                                       BinID = u.BinID,
                                                       Created = u.Created,
                                                       Updated = u.Updated,
                                                       CreatedBy = u.CreatedBy,
                                                       LastUpdatedBy = u.LastUpdatedBy,
                                                       IsDeleted = u.IsDeleted,
                                                       IsArchived = u.IsArchived,
                                                       CompanyID = u.CompanyID,
                                                       Room = u.Room,
                                                       PullCredit = u.PullCredit,
                                                       ItemLocationDetailGUID = u.ItemLocationDetailGUID,
                                                       StagingBinId = u.StagingBinId,
                                                       OrderDetailGUID = u.OrderDetailGUID,
                                                       AddedFrom = u.AddedFrom,
                                                       EditedFrom = u.EditedFrom,
                                                       ReceivedOn = u.ReceivedOn,
                                                       ReceivedOnWeb = u.ReceivedOnWeb,
                                                       InitialQuantityWeb = u.InitialQuantityWeb ?? 0,
                                                       InitialQuantityPDA = u.InitialQuantityPDA ?? 0
                                                   }).FirstOrDefault();
            }
            return lstMaterialStagingPullDetailDTO;
        }

        #endregion

        public IEnumerable<MaterialStagingPullDetailDTO> GetAllRecordsByItemGuid(Int64 RoomID, Int64 CompanyId, Guid ItemGUID, string SerialNumber = "")
        {
            IEnumerable<MaterialStagingPullDetailDTO> lstMaterialStagingPullDetail = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyId), new SqlParameter("@ItemGUID", ItemGUID), new SqlParameter("@SerialNumber", SerialNumber) };
                lstMaterialStagingPullDetail = (from u in context.Database.SqlQuery<MaterialStagingPullDetailDTO>("exec [GetMaterialStagingPullDetailByItemGUID] @RoomID,@CompanyID,@ItemGUID,@SerialNumber", params1)
                                                select new MaterialStagingPullDetailDTO
                                                {
                                                    ID = u.ID,
                                                    MaterialStagingGUID = u.MaterialStagingGUID,
                                                    MaterialStagingdtlGUID = u.MaterialStagingdtlGUID,
                                                    ItemGUID = u.ItemGUID,
                                                    ItemCost = u.ItemCost,
                                                    CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                    ConsignedQuantity = u.ConsignedQuantity,
                                                    PoolQuantity = u.PoolQuantity,
                                                    SerialNumber = u.SerialNumber,
                                                    LotNumber = u.LotNumber,
                                                    Expiration = u.Expiration == null && u.DateCodeTracking ? DateTime.Now.ToString("MM-dd-yy") : u.Expiration,
                                                    Received = u.Received == null ? DateTime.Now.ToString("MM-dd-yy") : u.Received,
                                                    ExpirationDate = u.ExpirationDate,
                                                    ReceivedDate = u.ReceivedDate,
                                                    BinID = u.BinID,
                                                    Created = u.Created,
                                                    Updated = u.Updated,
                                                    CreatedBy = u.CreatedBy,
                                                    LastUpdatedBy = u.LastUpdatedBy,
                                                    CreatedByName = u.CreatedByName,
                                                    UpdatedByName = u.UpdatedByName,
                                                    IsDeleted = u.IsDeleted == null ? false : u.IsDeleted,
                                                    IsArchived = u.IsArchived == null ? false : u.IsArchived,
                                                    CompanyID = u.CompanyID,
                                                    Room = u.Room,
                                                    PullCredit = u.PullCredit,
                                                    StagingBinId = u.StagingBinId,
                                                    GUID = u.GUID,
                                                    OrderDetailGUID = u.OrderDetailGUID,
                                                    IsConsignedSerialLot = u.IsConsignedSerialLot,
                                                    InitialQuantityWeb = u.InitialQuantityWeb ?? 0,
                                                    InitialQuantityPDA = u.InitialQuantityPDA ?? 0,
                                                    ItemLocationDetailGUID = u.ItemLocationDetailGUID,
                                                    AddedFrom = u.AddedFrom,
                                                    ReceivedOnWeb = u.ReceivedOnWeb,
                                                    ReceivedOn = u.ReceivedOn,
                                                    EditedFrom = u.EditedFrom

                                                }).AsParallel().ToList();
            }

            return lstMaterialStagingPullDetail;
        }

        #region WI-4873

        public bool UpdateMaterialStagingPullDetailIntialQtyandPoolQty(Int64 RoomID, Int64 CompanyId, Int64 UserID, Guid MaterialStagingPulldetailGuid, Guid ItemGuid, double InitialQuantityWeb)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@MSPulldetailGuid", MaterialStagingPulldetailGuid),
                                                   new SqlParameter("@ItemGuid", ItemGuid),
                                                   new SqlParameter("@UserID", UserID),
                                                   new SqlParameter("@InitialQuantityWeb", InitialQuantityWeb) };

                context.Database.SqlQuery<int>("EXEC [UpdateMSPullDtlIntialQtyandPoolQty] @MSPulldetailGuid,@ItemGuid,@UserID,@InitialQuantityWeb", params1).FirstOrDefault();
                return true;
            }
        }

        #endregion      

        public List<MaterialStagingPullDetailDTO> GetAvialableMsPullDetailsByItemGUIDANDBinID(Guid ItemGUID, long BinID, Int64 RoomID, Int64 CompanyID)
        {
            List<MaterialStagingPullDetailDTO> lstMaterialStagingPullDetailDTO = new List<MaterialStagingPullDetailDTO>();
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstMaterialStagingPullDetailDTO = (from u in context.MaterialStagingPullDetails
                                                   where u.IsArchived != true && u.IsDeleted != true && u.ItemGUID == ItemGUID && u.StagingBinId == BinID && u.Room == RoomID && u.CompanyID == CompanyID
                                                   && ((u.ConsignedQuantity ?? 0) + (u.CustomerOwnedQuantity ?? 0)) > 0
                                                   select new MaterialStagingPullDetailDTO
                                                   {
                                                       ID = u.ID,
                                                       MaterialStagingGUID = u.MaterialStagingGUID,
                                                       MaterialStagingdtlGUID = u.MaterialStagingdtlGUID,
                                                       GUID = u.GUID,
                                                       ItemGUID = u.ItemGUID,
                                                       ItemCost = u.ItemCost,
                                                       CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                       ConsignedQuantity = u.ConsignedQuantity,
                                                       PoolQuantity = u.PoolQuantity,
                                                       SerialNumber = u.SerialNumber,
                                                       LotNumber = u.LotNumber,
                                                       Expiration = u.Expiration,
                                                       ExpirationDate = u.ExpirationDate,
                                                       Received = u.Received,
                                                       ReceivedDate = u.ReceivedDate,
                                                       BinID = u.BinID,
                                                       Created = u.Created,
                                                       Updated = u.Updated,
                                                       CreatedBy = u.CreatedBy,
                                                       LastUpdatedBy = u.LastUpdatedBy,
                                                       IsDeleted = u.IsDeleted,
                                                       IsArchived = u.IsArchived,
                                                       CompanyID = u.CompanyID,
                                                       Room = u.Room,
                                                       PullCredit = u.PullCredit,
                                                       ItemLocationDetailGUID = u.ItemLocationDetailGUID,
                                                       StagingBinId = u.StagingBinId,
                                                       OrderDetailGUID = u.OrderDetailGUID,
                                                       AddedFrom = u.AddedFrom,
                                                       EditedFrom = u.EditedFrom,
                                                       ReceivedOn = u.ReceivedOn,
                                                       ReceivedOnWeb = u.ReceivedOnWeb,
                                                       InitialQuantityWeb = u.InitialQuantityWeb ?? 0,
                                                       InitialQuantityPDA = u.InitialQuantityPDA ?? 0
                                                   }).ToList();
            }
            return lstMaterialStagingPullDetailDTO;
        }

        public MaterialStagingPullDetailDTO GetItemStagingQtyByLocation(long BinId, Guid ItemGUID, long RoomId, long CompanyId, long UserId)
        {
            MaterialStagingPullDetailDTO lstItemBins = new MaterialStagingPullDetailDTO();
            using (eTurnsEntities dbeTurnsEntities = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstItemBins = (from u in dbeTurnsEntities.MaterialStagingPullDetails
                               join bm in dbeTurnsEntities.BinMasters on new { iid = u.ItemGUID, bid = u.StagingBinId} equals new { iid = bm.ItemGUID ?? Guid.Empty, bid = bm.ID }
                               where u.IsArchived == false && u.IsDeleted == false && u.StagingBinId == BinId && u.ItemGUID == ItemGUID && u.Room == RoomId && u.CompanyID == CompanyId
                               group u by new { u.ItemGUID, u.StagingBinId, bm.BinNumber } into groupedItemLocation
                               select new MaterialStagingPullDetailDTO
                               {
                                   StagingBinId = groupedItemLocation.Key.StagingBinId,
                                   BinNumber = groupedItemLocation.Key.BinNumber,
                                   CompanyID = CompanyId,
                                   ConsignedQuantity = groupedItemLocation.Sum(t => t.ConsignedQuantity ?? 0),                                   
                                   CustomerOwnedQuantity = groupedItemLocation.Sum(t => t.CustomerOwnedQuantity ?? 0),
                                   GUID = Guid.NewGuid(),
                                   ID = 0,
                                   ItemGUID = groupedItemLocation.Key.ItemGUID,
                                   ItemNumber = string.Empty,
                                   LastUpdated = DateTime.Now,
                                   LastUpdatedBy = UserId,
                                   LotNumber = string.Empty,                                  
                                   ActualAvailableQuantity = groupedItemLocation.Sum(t => t.ConsignedQuantity ?? 0) + groupedItemLocation.Sum(t => t.CustomerOwnedQuantity ?? 0),
                                   Room = RoomId,
                                   RoomName = string.Empty,
                                   SellPrice = 0
                               }).FirstOrDefault();
            }

            return lstItemBins;
        }

    }
}


