using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace eTurns.DAL
{
    public class ItemLocationDetailsDAL : eTurnsBaseDAL
    {
        string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];

        #region [Class Constructor]

        //public ItemLocationDetailsDAL(base.DataBaseName)
        //{

        //}

        public ItemLocationDetailsDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ItemLocationDetailsDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]

        public ItemLocationDetail setOldValueInNewItemLocationDetail(ItemLocationDetail oldILD)
        {

            ItemLocationDetail obj = new ItemLocationDetail();
            obj.ID = 0;

            obj.BinID = oldILD.BinID;
            obj.CustomerOwnedQuantity = oldILD.CustomerOwnedQuantity;
            obj.ConsignedQuantity = oldILD.ConsignedQuantity;
            obj.MeasurementID = oldILD.MeasurementID;
            obj.LotNumber = oldILD.LotNumber;
            obj.SerialNumber = oldILD.SerialNumber;
            obj.ExpirationDate = oldILD.ExpirationDate;
            obj.Cost = oldILD.Cost;
            obj.eVMISensorPort = oldILD.eVMISensorPort;
            obj.eVMISensorID = oldILD.eVMISensorID;
            obj.UDF1 = oldILD.UDF1;
            obj.UDF2 = oldILD.UDF2;
            obj.UDF3 = oldILD.UDF3;
            obj.UDF4 = oldILD.UDF4;
            obj.UDF5 = oldILD.UDF5;
            obj.GUID = Guid.NewGuid();
            obj.ItemGUID = oldILD.ItemGUID;
            obj.Created = oldILD.Created;
            obj.Updated = oldILD.Updated;
            obj.CreatedBy = oldILD.CreatedBy;
            obj.LastUpdatedBy = oldILD.LastUpdatedBy;
            obj.IsDeleted = oldILD.IsDeleted;
            obj.IsArchived = oldILD.IsArchived;
            obj.CompanyID = oldILD.CompanyID;
            obj.Room = oldILD.Room;
            obj.PULLGUID = oldILD.PULLGUID;
            obj.OrderDetailGUID = oldILD.OrderDetailGUID;
            obj.KitDetailGUID = oldILD.KitDetailGUID;

            obj.TransferDetailGUID = oldILD.TransferDetailGUID;
            obj.IsWebEdit = oldILD.IsWebEdit;
            obj.IsPDAEdit = oldILD.IsPDAEdit;

            obj.IsConsignedSerialLot = oldILD.IsConsignedSerialLot;

            obj.RefPDASelfGUID = oldILD.RefPDASelfGUID;
            obj.RefWebSelfGUID = oldILD.RefWebSelfGUID;

            obj.Expiration = oldILD.Expiration;
            obj.ExpirationDate = oldILD.ExpirationDate;
            obj.Received = oldILD.Received;
            obj.ReceivedDate = oldILD.ReceivedDate;
            obj.InsertedFrom = oldILD.InsertedFrom;
            obj.InitialQuantity = oldILD.InitialQuantity;
            obj.InitialQuantityWeb = oldILD.InitialQuantityWeb;
            obj.InitialQuantityPDA = oldILD.InitialQuantityPDA;
            obj.AddedFrom = oldILD.AddedFrom;
            obj.EditedFrom = oldILD.EditedFrom;
            obj.ReceivedOn = oldILD.ReceivedOn;
            obj.ReceivedOnWeb = oldILD.ReceivedOnWeb;

            return obj;
        }

        public ItemLocationDetail SetItemLocationDetailAsPerNewROTD(ItemLocationDetail objILD, ReceivedOrderTransferDetailDTO objNewDTO, double PrevReciveCustOwnedQty, double PrevReceivedConsQty, OrderType ot, bool IsBinDeleted)
        {

            if (ot == OrderType.Order && !IsBinDeleted)
            {
                objILD.ConsignedQuantity = objNewDTO.ConsignedQuantity.GetValueOrDefault(0);
                objILD.CustomerOwnedQuantity = objNewDTO.CustomerOwnedQuantity;
            }
            else if (ot == OrderType.RuturnOrder && !IsBinDeleted)
            {
                objILD.ConsignedQuantity = objNewDTO.ConsignedQuantity.GetValueOrDefault(0);
                objILD.CustomerOwnedQuantity = objNewDTO.CustomerOwnedQuantity;
            }

            if (ot == OrderType.Order && !IsBinDeleted)
            {
                objILD.InitialQuantity = (objNewDTO.CustomerOwnedQuantity.GetValueOrDefault(0)) + (objNewDTO.ConsignedQuantity.GetValueOrDefault(0));

                if (objILD.InitialQuantityWeb.GetValueOrDefault(0) > 0)
                    objILD.InitialQuantityWeb = (objNewDTO.CustomerOwnedQuantity.GetValueOrDefault(0)) + (objNewDTO.ConsignedQuantity.GetValueOrDefault(0));
                else if (objILD.InitialQuantityPDA.GetValueOrDefault(0) > 0)
                    objILD.InitialQuantityPDA = (objNewDTO.CustomerOwnedQuantity.GetValueOrDefault(0)) + (objNewDTO.ConsignedQuantity.GetValueOrDefault(0));
            }


            if (!string.IsNullOrEmpty(objNewDTO.Expiration))
                objILD.Expiration = objNewDTO.Expiration.Replace("-", "/");
            if (!string.IsNullOrEmpty(objNewDTO.Received))
                objILD.Received = objNewDTO.Received.Replace("-", "/");

            objILD.ExpirationDate = objNewDTO.ExpirationDate;
            objILD.LotNumber = (!string.IsNullOrWhiteSpace(objNewDTO.LotNumber)) ? objNewDTO.LotNumber.Trim() : string.Empty;
            objILD.SerialNumber = (!string.IsNullOrWhiteSpace(objNewDTO.SerialNumber)) ? objNewDTO.SerialNumber.Trim() : string.Empty;
            objILD.BinID = objNewDTO.BinID;
            objILD.ReceivedDate = objNewDTO.ReceivedDate;
            objILD.Cost = objNewDTO.Cost;
            objILD.UDF1 = objNewDTO.UDF1;
            objILD.UDF2 = objNewDTO.UDF2;
            objILD.UDF3 = objNewDTO.UDF3;
            objILD.UDF4 = objNewDTO.UDF4;
            objILD.UDF5 = objNewDTO.UDF5;

            objILD.LastUpdatedBy = objNewDTO.LastUpdatedBy;
            objILD.Updated = DateTimeUtility.DateTimeNow;
            objILD.IsWebEdit = true;
            objILD.EditedFrom = "Web";
            objILD.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
            objILD.InsertedFrom = "EditReceipt";
            if (IsBinDeleted)
            {
                objILD.InsertedFrom = "EditReceipt of deleted Bin, So qties not updated";
            }
            if (ot == OrderType.RuturnOrder)
            {
                objILD.InsertedFrom = "Edit Return";
                if (IsBinDeleted)
                {
                    objILD.InsertedFrom = "Edit Return of deleted Bin, So qties not updated";
                }
            }

            return objILD;
        }

        public ItemLocationDetail SetItemLocationDetail(ItemLocationDetail objILD, ReceivedOrderTransferDetailDTO objNewDTO, double PrevReciveCustOwnedQty, double PrevReceivedConsQty, OrderType ot, bool IsBinDeleted)
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

            if (ot == OrderType.Order && !IsBinDeleted)
            {
                objILD.InitialQuantity = (objNewDTO.CustomerOwnedQuantity.GetValueOrDefault(0)) + (objNewDTO.ConsignedQuantity.GetValueOrDefault(0));

                if (objILD.InitialQuantityWeb.GetValueOrDefault(0) > 0)
                    objILD.InitialQuantityWeb = (objNewDTO.CustomerOwnedQuantity.GetValueOrDefault(0)) + (objNewDTO.ConsignedQuantity.GetValueOrDefault(0));
                else if (objILD.InitialQuantityPDA.GetValueOrDefault(0) > 0)
                    objILD.InitialQuantityPDA = (objNewDTO.CustomerOwnedQuantity.GetValueOrDefault(0)) + (objNewDTO.ConsignedQuantity.GetValueOrDefault(0));
            }

            if (!string.IsNullOrEmpty(objNewDTO.Expiration))
                objILD.Expiration = objNewDTO.Expiration.Replace("-", "/");
            if (!string.IsNullOrEmpty(objNewDTO.Received))
                objILD.Received = objNewDTO.Received.Replace("-", "/");

            objILD.ExpirationDate = objNewDTO.ExpirationDate;
            objILD.LotNumber = (!string.IsNullOrWhiteSpace(objNewDTO.LotNumber)) ? objNewDTO.LotNumber.Trim() : string.Empty;
            objILD.SerialNumber = (!string.IsNullOrWhiteSpace(objNewDTO.SerialNumber)) ? objNewDTO.SerialNumber.Trim() : string.Empty;
            objILD.BinID = objNewDTO.BinID;
            objILD.ReceivedDate = objNewDTO.ReceivedDate;
            objILD.Cost = objNewDTO.Cost;
            objILD.UDF1 = objNewDTO.UDF1;
            objILD.UDF2 = objNewDTO.UDF2;
            objILD.UDF3 = objNewDTO.UDF3;
            objILD.UDF4 = objNewDTO.UDF4;
            objILD.UDF5 = objNewDTO.UDF5;

            objILD.LastUpdatedBy = objNewDTO.LastUpdatedBy;
            objILD.Updated = DateTimeUtility.DateTimeNow;
            objILD.IsWebEdit = true;
            objILD.EditedFrom = "Web";
            objILD.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
            objILD.InsertedFrom = "EditReceipt";
            if (IsBinDeleted)
            {
                objILD.InsertedFrom = "EditReceipt of deleted Bin, So qties not updated";
            }
            if (ot == OrderType.RuturnOrder)
            {
                objILD.InsertedFrom = "Edit Return";
                if (IsBinDeleted)
                {
                    objILD.InsertedFrom = "Edit Return of deleted Bin, So qties not updated";
                }
            }

            return objILD;
        }



        #endregion
        public IEnumerable<ItemLocationDetailsDTO> GetItemLocationDetail(Guid ItemGUID, Int64 RoomID, Int64 CompanyID, Int64? BinID, string SerialNumber, string LotNumber, string OrderDetailGUID, bool? IsDeleted, bool? IsArcvhived, string OrderDetailGUID_Exclude)
        {
            List<ItemLocationDetailsDTO> lstItemLocationDetails = new List<ItemLocationDetailsDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramA = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID),
                                                  new SqlParameter("@RoomID", RoomID),
                                                  new SqlParameter("@CompanyID", CompanyID),
                                                  new SqlParameter("@BinID", BinID ?? (object)DBNull.Value),
                                                  new SqlParameter("@SerialNumber", SerialNumber),
                                                  new SqlParameter("@LotNumber", LotNumber),
                                                  new SqlParameter("@OrderDetailGUID", OrderDetailGUID),
                                                  new SqlParameter("@IsDeleted", IsDeleted ?? (object)DBNull.Value),
                                                  new SqlParameter("@IsArcvhived", IsArcvhived ?? (object)DBNull.Value),
                                                  new SqlParameter("@OrderDetailGUID_Exclude", OrderDetailGUID_Exclude ?? (object)DBNull.Value)  };

                lstItemLocationDetails = (from u in context.Database.SqlQuery<ItemLocationDetailsDTO>("exec [GetItemLocDtlByRoomCompanyItemGUID] @ItemGUID,@RoomID,@CompanyID,@BinID,@SerialNumber,@LotNumber,@OrderDetailGUID,@IsDeleted,@IsArcvhived,@OrderDetailGUID_Exclude", paramA)
                                          select new ItemLocationDetailsDTO
                                          {
                                              ID = u.ID,
                                              BinID = u.BinID,
                                              CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                              ConsignedQuantity = u.ConsignedQuantity,
                                              MeasurementID = u.MeasurementID,
                                              LotNumber = u.LotNumber,
                                              SerialNumber = u.SerialNumber,
                                              ExpirationDate = u.ExpirationDate,
                                              ReceivedDate = u.ReceivedDate,
                                              Expiration = u.Expiration == null && u.DateCodeTracking ? DateTime.Now.ToString("MM-dd-yy") : u.Expiration,
                                              Received = u.Received == null ? DateTime.Now.ToString("MM-dd-yy") : u.Received,
                                              Cost = u.Cost,
                                              eVMISensorPort = u.eVMISensorPort,
                                              eVMISensorID = u.eVMISensorID,
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
                                              TransferDetailGUID = u.TransferDetailGUID,
                                              KitDetailGUID = u.KitDetailGUID,
                                              CriticalQuantity = u.CriticalQuantity,
                                              MinimumQuantity = u.MinimumQuantity,
                                              MaximumQuantity = u.MaximumQuantity,
                                              SuggestedOrderQuantity = u.SuggestedOrderQuantity,//(new CartItemDAL(base.DataBaseName)).GetSuggestedOrderQtyForBin(u.ItemGUID.GetValueOrDefault(Guid.Empty), u.BinID.GetValueOrDefault(0)),
                                              Markup = u.Markup,
                                              AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                                              EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                                              ReceivedOn = u.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                                              ReceivedOnWeb = u.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                                          }).AsParallel().ToList();
            }

            return lstItemLocationDetails;
        }
        public IEnumerable<ItemLocationDetailsDTO> GetLocationContainsQuantityOfItem(Guid ItemGUID, Int64 RoomID, Int64 CompanyID, Guid? OrderDetailGUID)
        {
            //Here Orderdetails guid is not taken in use. please check sp.. only parameters passed
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ItemGUID", ItemGUID), new SqlParameter("@OrderDetailGUID", OrderDetailGUID) };
                IEnumerable<ItemLocationDetailsDTO> obj = (from u in context.Database.SqlQuery<ItemLocationDetailsDTO>("exec [GetLocationContainsQuantityOfItem] @RoomID,@CompanyID,@ItemGUID,@OrderDetailGUID", params1) //A.IsDeleted!=1 AND A.IsArchived != 1 AND 

                                                           select new ItemLocationDetailsDTO
                                                           {
                                                               ID = u.ID,
                                                               BinID = u.BinID,
                                                               CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                               ConsignedQuantity = u.ConsignedQuantity,
                                                               MeasurementID = u.MeasurementID,
                                                               LotNumber = u.LotNumber,
                                                               SerialNumber = u.SerialNumber,
                                                               ExpirationDate = u.ExpirationDate,
                                                               ReceivedDate = u.ReceivedDate,
                                                               Expiration = u.Expiration == null && u.DateCodeTracking ? DateTime.Now.ToString("MM-dd-yy") : u.Expiration,
                                                               Received = u.Received == null ? DateTime.Now.ToString("MM-dd-yy") : u.Received,
                                                               Cost = u.Cost,
                                                               eVMISensorPort = u.eVMISensorPort,
                                                               eVMISensorID = u.eVMISensorID,
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
                                                               TransferDetailGUID = u.TransferDetailGUID,
                                                               KitDetailGUID = u.KitDetailGUID,
                                                               CriticalQuantity = u.CriticalQuantity,
                                                               MinimumQuantity = u.MinimumQuantity,
                                                               MaximumQuantity = u.MaximumQuantity,
                                                               SuggestedOrderQuantity = u.SuggestedOrderQuantity,//(new CartItemDAL(base.DataBaseName)).GetSuggestedOrderQtyForBin(u.ItemGUID.GetValueOrDefault(Guid.Empty), u.BinID.GetValueOrDefault(0)),
                                                               Markup = u.Markup,
                                                               AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                                                               EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                                                               ReceivedOn = u.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                                                               ReceivedOnWeb = u.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                                                           }).AsParallel().ToList();
                return obj;
            }

        }

        public IEnumerable<ItemLocationDetailsDTO> GetCachedDataeVMI(Guid ItemGUID, Int64 RoomID, Int64 CompanyID)
        {
            //Get Cached-Media
            IEnumerable<ItemLocationDetailsDTO> ObjCache = null;
            if (ObjCache == null || ObjCache.Count() <= 0)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID),
                                                       new SqlParameter("@RoomID", RoomID),
                                                       new SqlParameter("@CompanyID", CompanyID) };

                    ObjCache = (from u in context.Database.SqlQuery<ItemLocationDetailsDTO>("exec [GetItemLocationeVMI] @ItemGUID,@RoomID,@CompanyID", params1)
                                select new ItemLocationDetailsDTO
                                {
                                    ID = u.ID,
                                    BinID = u.BinID,
                                    CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                    ConsignedQuantity = u.ConsignedQuantity,
                                    MeasurementID = u.MeasurementID,
                                    LotNumber = u.LotNumber,
                                    SerialNumber = u.SerialNumber,
                                    ExpirationDate = u.ExpirationDate,
                                    ReceivedDate = u.ReceivedDate,
                                    Expiration = u.Expiration,
                                    Received = u.Received == null ? DateTime.Now.ToString("MM-dd-yy") : u.Received,
                                    Cost = u.Cost,
                                    eVMISensorPort = u.eVMISensorPort,
                                    eVMISensorID = u.eVMISensorID,
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
                                    TransferDetailGUID = u.TransferDetailGUID,
                                    KitDetailGUID = u.KitDetailGUID,
                                    CriticalQuantity = u.CriticalQuantity,
                                    MinimumQuantity = u.MinimumQuantity,
                                    MaximumQuantity = u.MaximumQuantity,
                                    SuggestedOrderQuantity = (new CartItemDAL(base.DataBaseName)).GetSuggestedOrderQtyForBin(u.ItemGUID.GetValueOrDefault(Guid.Empty), u.BinID.GetValueOrDefault(0)),
                                    AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                                    EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                                    ReceivedOn = u.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                                    ReceivedOnWeb = u.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow),

                                }).AsParallel().ToList();

                }
            }

            //IEnumerable<ItemMasterDTO> ObjItemCache = CacheHelper<IEnumerable<ItemMasterDTO>>.GetCacheItem("Cached_ItemMaster_" + CompanyID.ToString());
            //BinMasterDAL objBinDAL = new BinMasterDAL(base.DataBaseName);
            //IEnumerable<BinMasterDTO> ObjBinCache = objBinDAL.GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted);
            //ProjectMasterDAL objProjectDAL = new ProjectMasterDAL(base.DataBaseName);
            //IEnumerable<ProjectMasterDTO> ObjProjectCache = objProjectDAL.GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted);
            //UserMasterDAL objUserDAL = new UserMasterDAL(base.DataBaseName);
            //IEnumerable<UserMasterDTO> ObjUserCache = objUserDAL.GetAllRecords(0, CompanyID);
            //RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
            //IEnumerable<RoomDTO> ObjRoomCache = objRoomDAL.GetCachedData(CompanyID, IsArchived, IsDeleted);
            return ObjCache;
        }

        public IEnumerable<ItemLocationDetailsDTO> GetItemLocationDetailsForTransferByItemGuid(Guid ItemGUID, Int64 RoomID, Int64 CompanyID, List<string> serialNumbers, List<string> lotNumbers)
        {
            IEnumerable<ItemLocationDetailsDTO> locations = null;

            var params1 = new SqlParameter[] {
                                                   new SqlParameter("@ItemGuid", ItemGUID),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@SerialNumbers", string.Join(",", serialNumbers)),
                                                   new SqlParameter("@LotNumbers", string.Join(",", lotNumbers)),
                                                };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                locations = (from u in context.Database.SqlQuery<ItemLocationDetailsDTO>("exec [GetItemLocationDetailsForTransferByItemGuid] @ItemGuid,@RoomID,@CompanyID,@SerialNumbers,@LotNumbers ", params1)

                             select new ItemLocationDetailsDTO
                             {
                                 ID = u.ID,
                                 BinID = u.BinID,
                                 CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                 ConsignedQuantity = u.ConsignedQuantity,
                                 MeasurementID = u.MeasurementID,
                                 LotNumber = u.LotNumber,
                                 SerialNumber = u.SerialNumber,
                                 ExpirationDate = u.ExpirationDate,
                                 ReceivedDate = u.ReceivedDate,
                                 Expiration = u.Expiration,
                                 Received = u.Received == null ? DateTime.Now.ToString("MM-dd-yy") : u.Received,
                                 Cost = u.Cost,
                                 eVMISensorPort = u.eVMISensorPort,
                                 eVMISensorID = u.eVMISensorID,
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
                                 TransferDetailGUID = u.TransferDetailGUID,
                                 KitDetailGUID = u.KitDetailGUID,
                                 CriticalQuantity = u.CriticalQuantity,
                                 MinimumQuantity = u.MinimumQuantity,
                                 MaximumQuantity = u.MaximumQuantity,
                                 SuggestedOrderQuantity = (new CartItemDAL(base.DataBaseName)).GetSuggestedOrderQtyForBin(u.ItemGUID.GetValueOrDefault(Guid.Empty), u.BinID.GetValueOrDefault(0)),
                                 AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                                 EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                                 ReceivedOn = u.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                                 ReceivedOnWeb = u.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow),

                             }).AsParallel().ToList();

            }

            if (serialNumbers != null && serialNumbers.Any())
            {
                return locations.GroupBy(x => x.SerialNumber).Select(x => x.FirstOrDefault());
            }
            else if (lotNumbers != null && lotNumbers.Any())
            {
                return locations.GroupBy(x => x.LotNumber).Select(x => x.FirstOrDefault());
            }
            else
            {
                return locations;
            }

        }

        public IEnumerable<ItemLocationDetailsDTO> GetItemLocationDetailsForTransferByBinId(long BinId, Guid ItemGUID, Int64 RoomID, Int64 CompanyID, List<string> serialNumbers, List<string> lotNumbers)
        {
            var params1 = new SqlParameter[] {
                                                   new SqlParameter("@BinId", BinId),
                                                   new SqlParameter("@ItemGuid", ItemGUID),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@SerialNumbers", string.Join(",", serialNumbers)),
                                                   new SqlParameter("@LotNumbers", string.Join(",", lotNumbers)),
                                                };

            IEnumerable<ItemLocationDetailsDTO> locations = null;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                locations = (from u in context.Database.SqlQuery<ItemLocationDetailsDTO>("exec [GetItemLocationDetailsForTransferByBinId] @BinId,@ItemGuid,@RoomID,@CompanyID,@SerialNumbers,@LotNumbers ", params1)

                             select new ItemLocationDetailsDTO
                             {
                                 ID = u.ID,
                                 BinID = u.BinID,
                                 CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                 ConsignedQuantity = u.ConsignedQuantity,
                                 MeasurementID = u.MeasurementID,
                                 LotNumber = u.LotNumber,
                                 SerialNumber = u.SerialNumber,
                                 ExpirationDate = u.ExpirationDate,
                                 ReceivedDate = u.ReceivedDate,
                                 Expiration = u.Expiration,
                                 Received = u.Received == null ? DateTime.Now.ToString("MM-dd-yy") : u.Received,
                                 Cost = u.Cost,
                                 eVMISensorPort = u.eVMISensorPort,
                                 eVMISensorID = u.eVMISensorID,
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
                                 //CreatedByName = u.CreatedByName,
                                 //UpdatedByName = u.UpdatedByName,
                                 //RoomName = u.RoomName,
                                 //BinNumber = u.BinNumber,
                                 //ItemNumber = u.ItemNumber,
                                 //SerialNumberTracking = u.SerialNumberTracking,
                                 //LotNumberTracking = u.LotNumberTracking,
                                 //DateCodeTracking = u.DateCodeTracking,
                                 OrderDetailGUID = u.OrderDetailGUID,
                                 TransferDetailGUID = u.TransferDetailGUID,
                                 KitDetailGUID = u.KitDetailGUID,
                                 //CriticalQuantity = u.CriticalQuantity,
                                 //MinimumQuantity = u.MinimumQuantity,
                                 //MaximumQuantity = u.MaximumQuantity,
                                 SuggestedOrderQuantity = (new CartItemDAL(base.DataBaseName)).GetSuggestedOrderQtyForBin(u.ItemGUID.GetValueOrDefault(Guid.Empty), u.BinID.GetValueOrDefault(0)),
                                 AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                                 EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                                 ReceivedOn = u.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                                 ReceivedOnWeb = u.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow),

                             }).AsParallel().ToList();

            }

            if (serialNumbers != null && serialNumbers.Any())
            {
                return locations.GroupBy(x => x.SerialNumber).Select(x => x.FirstOrDefault());
            }
            else if (lotNumbers != null && lotNumbers.Any())
            {
                return locations.GroupBy(x => x.LotNumber).Select(x => x.FirstOrDefault());
            }
            else
            {
                return locations;
            }

        }

        public IEnumerable<ItemLocationDetailsDTO> GetCachedDataBinWise(Int64 BinID, Guid ItemGUID, Int64 RoomID, Int64 CompanyID)
        {
            //return GetCachedData(ItemGUID, RoomID, CompanyID).Where(t => t.BinID == BinID);
            return GetItemLocationDetail(ItemGUID, RoomID, CompanyID, BinID, string.Empty, string.Empty, string.Empty, null, null, string.Empty);
        }
        public IEnumerable<ItemLocationDetailsDTO> GetCachedDataBinWiseeVMI(Int64 BinID, Guid ItemGUID, Int64 RoomID, Int64 CompanyID)
        {
            return GetCachedDataeVMI(ItemGUID, RoomID, CompanyID).Where(t => t.BinID == BinID);
        }

        /// <summary>
        /// Get Paged Records from the ItemLocationDetails Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<ItemLocationDetailsDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, Guid ItemGUID, Guid? OrderDetailGUID, string ColumnName)
        {
            // var result = GetCachedData(RoomID, CompanyId).Where(t => (t.ItemID == ItemID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(ColumnName);
            IEnumerable<ItemLocationDetailsDTO> result = null;
            if (OrderDetailGUID == null || OrderDetailGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty)
            {
                //result = GetCachedData(ItemGUID, RoomID, CompanyId).Where(t => ((t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(ColumnName);
                result = GetItemLocationDetail(ItemGUID, RoomID, CompanyId, null, string.Empty, string.Empty, string.Empty, false, false, string.Empty).OrderBy(ColumnName);
            }
            else
            {
                //result = GetCachedData(ItemGUID, RoomID, CompanyId).Where(t => (t.OrderDetailGUID.GetValueOrDefault(Guid.Empty) == OrderDetailGUID.GetValueOrDefault(Guid.Empty) && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(ColumnName);
                result = GetItemLocationDetail(ItemGUID, RoomID, CompanyId, null, string.Empty, string.Empty, Convert.ToString(OrderDetailGUID.GetValueOrDefault(Guid.Empty)), false, false, string.Empty).OrderBy(ColumnName);
            }
            return result;
        }

        public IEnumerable<ItemLocationDetailsDTO> GetAllRecordseVmi(Int64 RoomID, Int64 CompanyId, Guid ItemGUID, Guid? OrderDetailGUID, string ColumnName, List<string> serialNumbers, List<string> lotNumbers)
        {
            // var result = GetCachedData(RoomID, CompanyId).Where(t => (t.ItemID == ItemID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(ColumnName);
            if (serialNumbers == null)
                serialNumbers = new List<string>();

            if (lotNumbers == null)
                lotNumbers = new List<string>();

            IEnumerable<ItemLocationDetailsDTO> result = null;
            if (OrderDetailGUID == null || OrderDetailGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty)
                result = GetItemLocationDetailsForTransferByItemGuid(ItemGUID, RoomID, CompanyId, serialNumbers, lotNumbers).OrderBy(ColumnName);
            else
                result = GetItemLocationDetailsForTransferByItemGuid(ItemGUID, RoomID, CompanyId, serialNumbers, lotNumbers).Where(t => (t.OrderDetailGUID.GetValueOrDefault(Guid.Empty) == OrderDetailGUID.GetValueOrDefault(Guid.Empty) && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(ColumnName);

            return result;
        }

        public IEnumerable<ItemLocationDetailsDTO> GetItemLocationsByBinId(long binId, Int64 RoomID, Int64 CompanyId, Guid ItemGUID, string ColumnName, List<string> serialNumbers, List<string> lotNumbers)
        {
            return GetItemLocationDetailsForTransferByBinId(binId, ItemGUID, RoomID, CompanyId, serialNumbers, lotNumbers).OrderBy(ColumnName);
        }

        public IEnumerable<ItemLocationDetailsDTO> GetAllRecordsBinWise(Int64 BinID, Int64 RoomID, Int64 CompanyId, Guid ItemGUID, Guid? OrderDetailGUID, string ColumnName)
        {
            // var result = GetCachedData(RoomID, CompanyId).Where(t => (t.ItemID == ItemID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(ColumnName);
            IEnumerable<ItemLocationDetailsDTO> result = null;
            if (OrderDetailGUID == null || OrderDetailGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty)
                result = GetCachedDataBinWise(BinID, ItemGUID, RoomID, CompanyId).Where(t => ((t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false && (t.ConsignedQuantity.GetValueOrDefault(0) != 0 || t.CustomerOwnedQuantity.GetValueOrDefault(0) != 0))).OrderBy(ColumnName);
            else
                result = GetCachedDataBinWise(BinID, ItemGUID, RoomID, CompanyId).Where(t => (t.OrderDetailGUID.GetValueOrDefault(Guid.Empty) == OrderDetailGUID.GetValueOrDefault(Guid.Empty) && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(ColumnName);

            return result;
        }

        public IEnumerable<ItemLocationDetailsDTO> GetItemLocationDetailsByBinIdAndItemGuidPlain(Int64 BinID, Int64 RoomID, Int64 CompanyId, Guid ItemGUID, string ColumnName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                   new SqlParameter("@BinId", BinID),
                                                   new SqlParameter("@ItemGuid", ItemGUID),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyId)
                                                };
                return context.Database.SqlQuery<ItemLocationDetailsDTO>("exec [GetItemLocationDetailsByBinIdAndItemGuidPlain] @BinId,@ItemGuid,@RoomID,@CompanyID", params1).ToList().OrderBy(ColumnName);
            }
        }

        /// <summary>
        /// Get Paged Records from the ItemLocationDetails Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<ItemLocationDetailsDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, Guid ItemID, Guid? OrderDetailGUID, string ColumnName, Guid? OrderDetailID_Exclude)
        {
            // var result = GetCachedData(RoomID, CompanyId).Where(t => (t.ItemID == ItemID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(ColumnName);
            IEnumerable<ItemLocationDetailsDTO> result = null;
            if (OrderDetailGUID == null)
            {
                //result = GetCachedData(ItemID, RoomID, CompanyId).Where(t => (t.OrderDetailGUID != OrderDetailID_Exclude && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(ColumnName);
                result = GetItemLocationDetail(ItemID, RoomID, CompanyId, null, string.Empty, string.Empty, string.Empty, false, false, Convert.ToString(OrderDetailID_Exclude)).OrderBy(ColumnName);
            }
            else
            {
                ////result = GetCachedData(ItemID, RoomID, CompanyId).Where(t => (t.OrderDetailGUID == OrderDetailGUID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(ColumnName);
                result = GetItemLocationDetail(ItemID, RoomID, CompanyId, null, string.Empty, string.Empty, Convert.ToString(OrderDetailGUID), false, false, string.Empty).OrderBy(ColumnName);
            }
            return result;
        }

        /// <summary>
        /// Get Paged Records from the ItemLocationDetails Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<ItemLocationDetailsDTO> GetAllRecordsAvailableAt(Int64 BinID, Int64 RoomID, Int64 CompanyId, Guid ItemGUID, Guid? OrderDetailGUID, string ColumnName)
        {
            IEnumerable<ItemLocationDetailsDTO> result = null;
            if (OrderDetailGUID == null)
                result = GetCachedDataeVMI(ItemGUID, RoomID, CompanyId).Where(t => (t.ItemGUID == ItemGUID && t.BinID == BinID && (t.CustomerOwnedQuantity.GetValueOrDefault(0) > 0 || t.ConsignedQuantity.GetValueOrDefault(0) > 0) && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(ColumnName);
            else
                result = GetCachedDataeVMI(ItemGUID, RoomID, CompanyId).Where(t => (t.ItemGUID == ItemGUID && t.BinID == BinID && (t.CustomerOwnedQuantity.GetValueOrDefault(0) > 0 || t.ConsignedQuantity.GetValueOrDefault(0) > 0) && t.OrderDetailGUID == OrderDetailGUID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(ColumnName);

            return result;
        }
        public List<ItemLocationDetailsDTO> GetItemLocationDetailsForConsume(bool IsFifo, Int64 BinID, Int64 RoomID, Int64 CompanyId, Guid ItemGUID, Guid? OrderDetailGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@IsFifo", IsFifo), new SqlParameter("@BinID", BinID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyId), new SqlParameter("@ItemGUID", ItemGUID), new SqlParameter("@OrderDetailGUID", OrderDetailGUID ?? (object)DBNull.Value) };
            string Qry = "EXEC dbo.[GetCustomerFirstThenConsigedByLIFOFIFO] @IsFifo,@BinID,@RoomID,@CompanyId,@ItemGUID,@OrderDetailGUID";
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemLocationDetailsDTO>(Qry, params1).ToList();
            }
        }

        public IEnumerable<ItemLocationDetailsDTO> GetCustomerFirstThenConsigedByLIFOFIFO(bool IsFifo, Int64 BinID, Int64 RoomID, Int64 CompanyId, Guid ItemGUID, Guid? OrderDetailGUID)
        {

            List<ItemLocationDetailsDTO> lstFromDB = GetItemLocationDetailsForConsume(IsFifo, BinID, RoomID, CompanyId, ItemGUID, OrderDetailGUID);

            string sOrderBy = "";
            if (IsFifo)
            {
                sOrderBy = "ID ASC";
            }
            else
            {
                sOrderBy = "ID DESC";
            }

            IEnumerable<ItemLocationDetailsDTO> result = null;
            if (OrderDetailGUID == null)
            {
                //first customer 
                //result = GetCachedData(ItemGUID, RoomID, CompanyId).Where(t => (t.ItemGUID == ItemGUID && t.BinID == BinID && (t.CustomerOwnedQuantity.GetValueOrDefault(0) > 0 && t.ConsignedQuantity.GetValueOrDefault(0) == 0) && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(sOrderBy);
                result = lstFromDB.Where(t => (t.ItemGUID == ItemGUID && t.BinID == BinID && (t.CustQConsumable.GetValueOrDefault(0) > 0) && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(sOrderBy);

                //then consigned
                IEnumerable<ItemLocationDetailsDTO> resultcons = null;
                //resultcons = GetCachedData(ItemGUID, RoomID, CompanyId).Where(t => (t.ItemGUID == ItemGUID && t.BinID == BinID && (t.CustomerOwnedQuantity.GetValueOrDefault(0) == 0 && t.ConsignedQuantity.GetValueOrDefault(0) > 0) && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(sOrderBy);
                resultcons = lstFromDB.Where(t => (t.ItemGUID == ItemGUID && t.BinID == BinID && (t.ConQConsumable.GetValueOrDefault(0) > 0) && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(sOrderBy);

                List<ItemLocationDetailsDTO> FinalResult = new List<ItemLocationDetailsDTO>();
                foreach (var item in result)
                {
                    //item.CustomerOwnedQuantity = item.CustQConsumable;
                    //item.ConsignedQuantity = item.ConQConsumable;
                    FinalResult.Add(item);
                }
                foreach (var item in resultcons)
                {
                    if (FinalResult.Where(x => x.GUID == item.GUID).Count() <= 0)
                    {
                        //item.CustomerOwnedQuantity = item.CustQConsumable;
                        //item.ConsignedQuantity = item.ConQConsumable;
                        FinalResult.Add(item);
                    }
                }
                //FinalResult.ForEach(t => { t.CustomerOwnedQuantity = t.CustomerOwnedQuantityConsumable; t.ConsignedQuantity = t.ConsignedQuantityConsumable });
                return FinalResult;
                //return result.Concat(resultcons);
            }
            else
            {
                //first customer
                //result = GetCachedData(ItemGUID, RoomID, CompanyId).Where(t => (t.ItemGUID == ItemGUID && t.BinID == BinID && (t.CustomerOwnedQuantity.GetValueOrDefault(0) > 0 && t.ConsignedQuantity.GetValueOrDefault(0) == 0) && t.OrderDetailGUID == OrderDetailGUID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(sOrderBy);
                result = lstFromDB.Where(t => (t.ItemGUID == ItemGUID && t.BinID == BinID && (t.CustQConsumable.GetValueOrDefault(0) > 0) && t.OrderDetailGUID == OrderDetailGUID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(sOrderBy);

                //then consigned
                IEnumerable<ItemLocationDetailsDTO> resultcons = null;
                //resultcons = GetCachedData(ItemGUID, RoomID, CompanyId).Where(t => (t.ItemGUID == ItemGUID && t.BinID == BinID && (t.CustomerOwnedQuantity.GetValueOrDefault(0) == 0 && t.ConsignedQuantity.GetValueOrDefault(0) > 0) && t.OrderDetailGUID == OrderDetailGUID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(sOrderBy);
                resultcons = lstFromDB.Where(t => (t.ItemGUID == ItemGUID && t.BinID == BinID && (t.ConQConsumable.GetValueOrDefault(0) > 0) && t.OrderDetailGUID == OrderDetailGUID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(sOrderBy);

                List<ItemLocationDetailsDTO> FinalResult = new List<ItemLocationDetailsDTO>();
                foreach (var item in result)
                {
                    FinalResult.Add(item);
                }
                foreach (var item in resultcons)
                {
                    if (FinalResult.Where(x => x.GUID == item.GUID).Count() <= 0)
                    {
                        FinalResult.Add(item);
                    }
                }
                return FinalResult;

                //return result.Concat(resultcons);
            }

            //return null;
        }

        public IEnumerable<ItemLocationDetailsDTO> GetCustForLIFOFIFO(bool IsFifo, Int64 BinID, Int64 RoomID, Int64 CompanyId, Guid ItemGUID, Guid? OrderDetailGUID)
        {
            string sOrderBy = "";
            if (IsFifo)
            {
                sOrderBy = "ID ASC";
            }
            else
            {
                sOrderBy = "ID DESC";
            }
            IEnumerable<ItemLocationDetailsDTO> result = null;
            if (OrderDetailGUID == null)
            {

                result = GetCachedDataeVMI(ItemGUID, RoomID, CompanyId).Where(t => (t.ItemGUID == ItemGUID && t.BinID == BinID && (t.CustomerOwnedQuantity.GetValueOrDefault(0) > 0) && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(sOrderBy);
            }
            else
            {
                result = GetCachedDataeVMI(ItemGUID, RoomID, CompanyId).Where(t => (t.ItemGUID == ItemGUID && t.BinID == BinID && (t.CustomerOwnedQuantity.GetValueOrDefault(0) > 0) && t.OrderDetailGUID == OrderDetailGUID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(sOrderBy);
            }
            return result;
        }

        public IEnumerable<ItemLocationDetailsDTO> GetConsForLIFOFIFO(bool IsFifo, Int64 BinID, Int64 RoomID, Int64 CompanyId, Guid ItemGUID, Guid? OrderDetailGUID)
        {
            string sOrderBy = "";
            if (IsFifo)
            {
                sOrderBy = "ID ASC";
            }
            else
            {
                sOrderBy = "ID DESC";
            }
            IEnumerable<ItemLocationDetailsDTO> result = null;
            if (OrderDetailGUID == null)
            {

                result = GetCachedDataeVMI(ItemGUID, RoomID, CompanyId).Where(t => (t.ItemGUID == ItemGUID && t.BinID == BinID && (t.ConsignedQuantity.GetValueOrDefault(0) > 0) && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(sOrderBy);
            }
            else
            {
                result = GetCachedDataeVMI(ItemGUID, RoomID, CompanyId).Where(t => (t.ItemGUID == ItemGUID && t.BinID == BinID && (t.ConsignedQuantity.GetValueOrDefault(0) > 0) && t.OrderDetailGUID == OrderDetailGUID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(sOrderBy);
            }

            return result;
        }

        /// <summary>
        /// Get Paged Records from the ItemLocationDetails Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>

        public IEnumerable<ItemLocationDetailsDTO> GetPagedRecords_NoCache(Int64 BinID, Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid ItemGUID, Guid? OrderDetailGUID)
        {
            //Get Cached-Media
            IEnumerable<ItemLocationDetailsDTO> ObjCache = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@BinID", BinID) };

                ObjCache = (from u in context.Database.SqlQuery<ItemLocationDetailsDTO>("EXEC [GetItemLocationDtlByPositiveQTY] @ItemGUID,@RoomID,@CompanyID,@BinID", params1)
                            select new ItemLocationDetailsDTO
                            {
                                ID = u.ID,
                                BinID = u.BinID,
                                CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                ConsignedQuantity = u.ConsignedQuantity,
                                MeasurementID = u.MeasurementID,
                                LotNumber = u.LotNumber,
                                SerialNumber = u.SerialNumber,
                                ExpirationDate = u.ExpirationDate,
                                ReceivedDate = u.ReceivedDate,
                                Expiration = u.Expiration == null && u.DateCodeTracking ? DateTime.Now.ToString("MM-dd-yyyy") : u.Expiration,
                                Received = u.Received == null ? DateTime.Now.ToString("MM-dd-yyyy") : u.Received,
                                Cost = u.Cost,
                                Markup = u.Markup,
                                SellPrice = u.SellPrice,
                                eVMISensorPort = u.eVMISensorPort,
                                eVMISensorID = u.eVMISensorID,
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
                                TransferDetailGUID = u.TransferDetailGUID,
                                KitDetailGUID = u.KitDetailGUID,
                                CriticalQuantity = u.CriticalQuantity,
                                MinimumQuantity = u.MinimumQuantity,
                                MaximumQuantity = u.MaximumQuantity,
                                SuggestedOrderQuantity = (new CartItemDAL(base.DataBaseName)).GetSuggestedOrderQtyForBin(u.ItemGUID.GetValueOrDefault(Guid.Empty), u.BinID.GetValueOrDefault(0)),
                                AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                                EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                                ReceivedOn = u.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                                ReceivedOnWeb = u.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                            }).AsParallel().ToList();
            }




            IEnumerable<ItemLocationDetailsDTO> ObjGlobalCache = null;

            if (OrderDetailGUID == null)
            {
                ObjGlobalCache = ObjCache;
                ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false && t.ItemGUID == ItemGUID));
            }
            else
            {
                ObjGlobalCache = ObjCache.Where(t => t.OrderDetailGUID == OrderDetailGUID);
                ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false && t.OrderDetailGUID == OrderDetailGUID));
            }

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
                //IEnumerable<ItemLocationDetailsDTO> ObjCache = GetCachedData(RoomID, CompanyID);
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
                //IEnumerable<ItemLocationDetailsDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjCache.Where(t => t.ID.ToString().Contains(SearchTerm)).Count();
                return ObjCache.Where(t => t.ID.ToString().Contains(SearchTerm)).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }

        /// <summary>
        /// Get Particullar Record from the ItemLocationDetails by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public ItemLocationDetailsDTO GetRecord(Guid GUID, Int64 RoomID, Int64 CompanyID)
        {
            //Need to changes...
            //return GetCachedData(0,RoomID, CompanyID).Where(t => t.ID == id).SingleOrDefault();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@GUID", GUID),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID) };


                return (from u in context.Database.SqlQuery<ItemLocationDetailsDTO>("exec [GetItemLocationDtlByGUID] @GUID,@RoomID,@CompanyID", params1)
                        select new ItemLocationDetailsDTO
                        {
                            ID = u.ID,

                            BinID = u.BinID,
                            CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                            ConsignedQuantity = u.ConsignedQuantity,
                            MeasurementID = u.MeasurementID,
                            LotNumber = u.LotNumber,
                            SerialNumber = u.SerialNumber,
                            ExpirationDate = u.ExpirationDate,
                            ReceivedDate = u.ReceivedDate,
                            Expiration = u.Expiration == null && u.DateCodeTracking ? DateTime.Now.ToString("MM/dd/yy") : u.Expiration,
                            Received = u.Received == null ? DateTime.Now.ToString("MM/dd/yy") : u.Received,
                            Cost = u.Cost,
                            eVMISensorPort = u.eVMISensorPort,
                            eVMISensorID = u.eVMISensorID,
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
                            TransferDetailGUID = u.TransferDetailGUID,
                            KitDetailGUID = u.KitDetailGUID,
                            CriticalQuantity = u.CriticalQuantity,
                            MinimumQuantity = u.MinimumQuantity,
                            MaximumQuantity = u.MaximumQuantity,
                            SuggestedOrderQuantity = (new CartItemDAL(base.DataBaseName)).GetSuggestedOrderQtyForBin(u.ItemGUID.GetValueOrDefault(Guid.Empty), u.BinID.GetValueOrDefault(0)),
                            AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                            EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                            ReceivedOn = u.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                            ReceivedOnWeb = u.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                        }).SingleOrDefault();

            }
        }

        /// <summary>
        /// Get Particullar Record from the ItemLocationDetails by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public ItemLocationDetailsDTO GetRecord(Int64 ID, Int64 RoomID, Int64 CompanyID)
        {
            //Need to changes...
            //return GetCachedData(0,RoomID, CompanyID).Where(t => t.ID == id).SingleOrDefault();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", ID),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID) };

                return (from u in context.Database.SqlQuery<ItemLocationDetailsDTO>("exec [GetItemLocationDtlByID] @ID,@RoomID,@CompanyID", params1)
                        select new ItemLocationDetailsDTO
                        {
                            ID = u.ID,

                            BinID = u.BinID,
                            CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                            ConsignedQuantity = u.ConsignedQuantity,
                            MeasurementID = u.MeasurementID,
                            LotNumber = u.LotNumber,
                            SerialNumber = u.SerialNumber,
                            ExpirationDate = u.ExpirationDate,
                            ReceivedDate = u.ReceivedDate,
                            Expiration = u.Expiration == null && u.DateCodeTracking ? DateTime.Now.ToString("MM/dd/yy") : u.Expiration,
                            Received = u.Received == null ? DateTime.Now.ToString("MM/dd/yy") : u.Received,
                            Cost = u.Cost,
                            eVMISensorPort = u.eVMISensorPort,
                            eVMISensorID = u.eVMISensorID,
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
                            TransferDetailGUID = u.TransferDetailGUID,
                            KitDetailGUID = u.KitDetailGUID,
                            CriticalQuantity = u.CriticalQuantity,
                            MinimumQuantity = u.MinimumQuantity,
                            MaximumQuantity = u.MaximumQuantity,
                            SuggestedOrderQuantity = (new CartItemDAL(base.DataBaseName)).GetSuggestedOrderQtyForBin(u.ItemGUID.GetValueOrDefault(Guid.Empty), u.BinID.GetValueOrDefault(0)),
                            AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                            EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                            ReceivedOn = u.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                            ReceivedOnWeb = u.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                        }).SingleOrDefault();

            }
        }

        public ItemLocationDetailsDTO GetItemLocationDetailByIdNormal(long ID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", ID)
                                                 };

                return (from u in context.Database.SqlQuery<ItemLocationDetailsDTO>("exec [GetItemLocationDetailByIdNormal] @ID", params1)
                        select new ItemLocationDetailsDTO
                        {
                            ID = u.ID,
                            BinID = u.BinID,
                            CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                            ConsignedQuantity = u.ConsignedQuantity,
                            MeasurementID = u.MeasurementID,
                            LotNumber = u.LotNumber,
                            SerialNumber = u.SerialNumber,
                            ExpirationDate = u.ExpirationDate,
                            ReceivedDate = u.ReceivedDate,
                            Expiration = u.Expiration == null && u.DateCodeTracking ? DateTime.Now.ToString("MM/dd/yy") : u.Expiration,
                            Received = u.Received == null ? DateTime.Now.ToString("MM/dd/yy") : u.Received,
                            Cost = u.Cost,
                            eVMISensorPort = u.eVMISensorPort,
                            eVMISensorID = u.eVMISensorID,
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
                            //CreatedByName = u.CreatedByName,
                            //UpdatedByName = u.UpdatedByName,
                            //RoomName = u.RoomName,
                            //BinNumber = u.BinNumber,
                            ItemNumber = u.ItemNumber,
                            SerialNumberTracking = u.SerialNumberTracking,
                            LotNumberTracking = u.LotNumberTracking,
                            DateCodeTracking = u.DateCodeTracking,
                            ItemType = u.ItemType,
                            OrderDetailGUID = u.OrderDetailGUID,
                            TransferDetailGUID = u.TransferDetailGUID,
                            KitDetailGUID = u.KitDetailGUID,
                            //CriticalQuantity = u.CriticalQuantity,
                            //MinimumQuantity = u.MinimumQuantity,
                            //MaximumQuantity = u.MaximumQuantity,
                            SuggestedOrderQuantity = (new CartItemDAL(base.DataBaseName)).GetSuggestedOrderQtyForBin(u.ItemGUID.GetValueOrDefault(Guid.Empty), u.BinID.GetValueOrDefault(0)),
                            AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                            EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                            ReceivedOn = u.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                            ReceivedOnWeb = u.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                        }).SingleOrDefault();

            }
        }

        /// <summary>
        /// Insert Record in the DataBase ItemLocationDetails
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public ItemLocationDetailsDTO Insert(ItemLocationDetailsDTO objDTO)
        {
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemLocationDetail obj = new ItemLocationDetail();
                obj.ID = 0;

                obj.BinID = objDTO.BinID;
                obj.CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity;
                obj.ConsignedQuantity = objDTO.ConsignedQuantity;
                obj.MeasurementID = objDTO.MeasurementID;
                obj.LotNumber = objDTO.LotNumber;
                obj.SerialNumber = objDTO.SerialNumber;
                obj.ExpirationDate = objDTO.ExpirationDate;
                obj.Cost = objDTO.Cost;
                obj.eVMISensorPort = objDTO.eVMISensorPort;
                obj.eVMISensorID = objDTO.eVMISensorID;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.GUID = Guid.NewGuid();
                obj.ItemGUID = objDTO.ItemGUID;
                obj.Created = objDTO.Created;
                obj.Updated = objDTO.Updated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.OrderDetailGUID = objDTO.OrderDetailGUID;
                obj.KitDetailGUID = objDTO.KitDetailGUID;

                obj.TransferDetailGUID = objDTO.TransferDetailGUID;

                if (objDTO.ConsignedQuantity.GetValueOrDefault(0) > 0)
                    obj.IsConsignedSerialLot = true;
                else if (objDTO.ConsignedQuantity.GetValueOrDefault(0) < 0 && objDTO.CountCustOrConsQty.GetValueOrDefault(0) == 0)
                    obj.IsConsignedSerialLot = true;

                if (!string.IsNullOrEmpty(objDTO.Expiration))
                {
                    obj.Expiration = objDTO.Expiration.Replace("-", "/");
                }
                else if (objDTO.ExpirationDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                {
                    obj.Expiration = objDTO.ExpirationDate.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy");
                }

                obj.ReceivedDate = objDTO.ReceivedDate;

                if (!string.IsNullOrEmpty(objDTO.Received))
                {
                    obj.Received = objDTO.Received.Replace("-", "/");
                }
                else if (objDTO.ReceivedDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                {
                    obj.Received = objDTO.ReceivedDate.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy");
                }


                obj.InsertedFrom = objDTO.InsertedFrom;
                obj.InitialQuantity = (objDTO.CustomerOwnedQuantity ?? 0) + (objDTO.ConsignedQuantity ?? 0);
                obj.InitialQuantityWeb = (objDTO.CustomerOwnedQuantity ?? 0) + (objDTO.ConsignedQuantity ?? 0);

                if (!string.IsNullOrEmpty(objDTO.AddedFrom))
                    obj.AddedFrom = objDTO.AddedFrom;
                else
                    obj.AddedFrom = "Unknown";

                if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                    obj.EditedFrom = objDTO.EditedFrom;
                else
                    obj.EditedFrom = "Unknown";

                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                context.ItemLocationDetails.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;


                if (objDTO.Action == "Receive")
                {
                    ItemMaster oItemMaster = context.ItemMasters.Where(x => x.GUID == objDTO.ItemGUID && x.CompanyID == objDTO.CompanyID && x.Room == objDTO.Room).FirstOrDefault();
                    Room oRoom = context.Rooms.Where(x => x.ID == oItemMaster.Room && x.CompanyID == oItemMaster.CompanyID).FirstOrDefault();
                    if (oRoom.MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString() && !oItemMaster.Consignment)
                    {
                        new ItemMasterDAL(base.DataBaseName).UpdateItemAndPastReceiptCostByReceiveCost(oItemMaster.GUID, oItemMaster.Room.GetValueOrDefault(0), oItemMaster.CompanyID.GetValueOrDefault(0), false, objDTO.EditedFrom, objDTO.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow));
                    }
                    else if (oRoom.MethodOfValuingInventory == ((int)InventoryValuationMethod.LastCost).ToString() && !oItemMaster.Consignment)
                    {
                        new ItemMasterDAL(base.DataBaseName).UpdateItemAndPastReceiptCostByReceiveCost(oItemMaster.GUID, oItemMaster.Room.GetValueOrDefault(0), oItemMaster.CompanyID.GetValueOrDefault(0), true, objDTO.EditedFrom, objDTO.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow));
                    }
                }

                if (objDTO.ID > 0)
                {

                    //Get Cached-Media
                    //IEnumerable<ItemLocationDetailsDTO> ObjCache = CacheHelper<IEnumerable<ItemLocationDetailsDTO>>.GetCacheItem("Cached_ItemLocationDetails_" + objDTO.CompanyID.ToString());
                    //if (ObjCache != null)
                    //{
                    objDTO.SuggestedOrderQuantity = (new CartItemDAL(base.DataBaseName)).GetSuggestedOrderQtyForBin(objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.BinID.GetValueOrDefault(0));

                    //List<ItemLocationDetailsDTO> tempC = new List<ItemLocationDetailsDTO>();
                    //tempC.Add(objDTO);

                    //IEnumerable<ItemLocationDetailsDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    //CacheHelper<IEnumerable<ItemLocationDetailsDTO>>.AppendToCacheItem("Cached_ItemLocationDetails_" + objDTO.CompanyID.ToString(), NewCache);
                    //}

                    if (objDTO.OrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty || objDTO.TransferDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                    {
                        if (string.IsNullOrEmpty(objDTO.AddedFrom))
                        {
                            if (string.IsNullOrEmpty(objDTO.AddedFrom))
                                objDTO.AddedFrom = "Web";
                            objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            if (string.IsNullOrEmpty(objDTO.EditedFrom))
                                objDTO.EditedFrom = "Web";
                            objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        }
                        new ReceivedOrderTransferDetailDAL(base.DataBaseName).Insert(objDTO);
                    }

                }

                return objDTO;
            }
        }

        /// <summary>
        /// Insert Record in the DataBase ItemLocationDetails
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public ItemLocationDetailsDTO InsertByGuid(ItemLocationDetailsDTO objDTO)
        {
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemLocationDetail obj = new ItemLocationDetail();
                obj.ID = 0;

                obj.BinID = objDTO.BinID;
                obj.CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity;
                obj.ConsignedQuantity = objDTO.ConsignedQuantity;
                obj.MeasurementID = objDTO.MeasurementID;
                obj.LotNumber = objDTO.LotNumber;
                obj.SerialNumber = objDTO.SerialNumber;
                obj.ExpirationDate = objDTO.ExpirationDate;
                obj.Cost = objDTO.Cost;
                obj.eVMISensorPort = objDTO.eVMISensorPort;
                obj.eVMISensorID = objDTO.eVMISensorID;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.GUID = objDTO.GUID;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.Created = objDTO.Created;
                obj.Updated = objDTO.Updated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.OrderDetailGUID = objDTO.OrderDetailGUID;
                obj.KitDetailGUID = objDTO.KitDetailGUID;

                obj.TransferDetailGUID = objDTO.TransferDetailGUID;

                if (objDTO.ConsignedQuantity.GetValueOrDefault(0) > 0)
                    obj.IsConsignedSerialLot = true;
                else if (objDTO.ConsignedQuantity.GetValueOrDefault(0) < 0 && objDTO.CountCustOrConsQty.GetValueOrDefault(0) == 0)
                    obj.IsConsignedSerialLot = true;

                if (!string.IsNullOrEmpty(objDTO.Expiration))
                {
                    obj.Expiration = objDTO.Expiration.Replace("-", "/");
                }
                else if (objDTO.ExpirationDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                {
                    obj.Expiration = objDTO.ExpirationDate.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy");
                }

                obj.ReceivedDate = objDTO.ReceivedDate;

                if (!string.IsNullOrEmpty(objDTO.Received))
                {
                    obj.Received = objDTO.Received.Replace("-", "/");
                }
                else if (objDTO.ReceivedDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                {
                    obj.Received = objDTO.ReceivedDate.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy");
                }

                obj.InsertedFrom = objDTO.InsertedFrom;

                obj.InitialQuantity = (objDTO.CustomerOwnedQuantity ?? 0) + (objDTO.ConsignedQuantity ?? 0);
                if (obj.InitialQuantity < 0)
                    obj.InitialQuantity = 0;

                obj.InitialQuantityWeb = (objDTO.CustomerOwnedQuantity ?? 0) + (objDTO.ConsignedQuantity ?? 0);
                if (obj.InitialQuantityWeb < 0)
                    obj.InitialQuantityWeb = 0;

                if (!string.IsNullOrEmpty(objDTO.AddedFrom))
                    obj.AddedFrom = objDTO.AddedFrom;
                else
                    obj.AddedFrom = "Web";

                if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                    obj.EditedFrom = objDTO.EditedFrom;
                else
                    obj.EditedFrom = "Web";

                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                context.ItemLocationDetails.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;


                if (objDTO.Action == "Receive")
                {
                    ItemMaster oItemMaster = context.ItemMasters.Where(x => x.GUID == objDTO.ItemGUID && x.CompanyID == objDTO.CompanyID && x.Room == objDTO.Room).FirstOrDefault();
                    Room oRoom = context.Rooms.Where(x => x.ID == oItemMaster.Room && x.CompanyID == oItemMaster.CompanyID).FirstOrDefault();
                    if (oRoom.MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString() && !oItemMaster.Consignment)
                    {
                        new ItemMasterDAL(base.DataBaseName).UpdateItemAndPastReceiptCostByReceiveCost(oItemMaster.GUID, oItemMaster.Room.GetValueOrDefault(0), oItemMaster.CompanyID.GetValueOrDefault(0), false, objDTO.EditedFrom, objDTO.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow));
                    }
                    else if (oRoom.MethodOfValuingInventory == ((int)InventoryValuationMethod.LastCost).ToString() && !oItemMaster.Consignment)
                    {
                        new ItemMasterDAL(base.DataBaseName).UpdateItemAndPastReceiptCostByReceiveCost(oItemMaster.GUID, oItemMaster.Room.GetValueOrDefault(0), oItemMaster.CompanyID.GetValueOrDefault(0), true, objDTO.EditedFrom, objDTO.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow));
                    }
                }

                if (objDTO.ID > 0)
                {

                    //Get Cached-Media
                    //IEnumerable<ItemLocationDetailsDTO> ObjCache = CacheHelper<IEnumerable<ItemLocationDetailsDTO>>.GetCacheItem("Cached_ItemLocationDetails_" + objDTO.CompanyID.ToString());
                    //if (ObjCache != null)
                    //{
                    objDTO.SuggestedOrderQuantity = (new CartItemDAL(base.DataBaseName)).GetSuggestedOrderQtyForBin(objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.BinID.GetValueOrDefault(0));

                    //List<ItemLocationDetailsDTO> tempC = new List<ItemLocationDetailsDTO>();
                    //tempC.Add(objDTO);

                    //IEnumerable<ItemLocationDetailsDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    //CacheHelper<IEnumerable<ItemLocationDetailsDTO>>.AppendToCacheItem("Cached_ItemLocationDetails_" + objDTO.CompanyID.ToString(), NewCache);
                    //}

                    if (objDTO.OrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty || objDTO.TransferDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                    {
                        if (string.IsNullOrEmpty(objDTO.AddedFrom))
                        {
                            objDTO.AddedFrom = "Web";
                            objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            objDTO.EditedFrom = "Web";
                            objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        }
                        new ReceivedOrderTransferDetailDAL(base.DataBaseName).Insert(objDTO);
                    }

                }

                return objDTO;
            }
        }


        public ItemLocationDetailsDTO InsertIntemLocationDetail(ItemLocationDetailsDTO objDTO)
        {
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemLocationDetail obj = new ItemLocationDetail();
                obj.ID = 0;

                obj.BinID = objDTO.BinID;
                obj.CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity;
                obj.ConsignedQuantity = objDTO.ConsignedQuantity;
                obj.MeasurementID = objDTO.MeasurementID;
                obj.LotNumber = objDTO.LotNumber;
                obj.SerialNumber = objDTO.SerialNumber;
                obj.ExpirationDate = objDTO.ExpirationDate;
                obj.Cost = objDTO.Cost;
                obj.eVMISensorPort = objDTO.eVMISensorPort;
                obj.eVMISensorID = objDTO.eVMISensorID;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.GUID = Guid.NewGuid();
                obj.ItemGUID = objDTO.ItemGUID;
                obj.Created = objDTO.Created;
                obj.Updated = objDTO.Updated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.OrderDetailGUID = objDTO.OrderDetailGUID;
                obj.KitDetailGUID = objDTO.KitDetailGUID;
                obj.TransferDetailGUID = objDTO.TransferDetailGUID;

                if (objDTO.ConsignedQuantity.GetValueOrDefault(0) > 0)
                    obj.IsConsignedSerialLot = true;
                else if (objDTO.ConsignedQuantity.GetValueOrDefault(0) < 0 && objDTO.CountCustOrConsQty.GetValueOrDefault(0) == 0)
                    obj.IsConsignedSerialLot = true;

                obj.ReceivedDate = objDTO.ReceivedDate;

                if (!string.IsNullOrEmpty(objDTO.Expiration))
                {
                    obj.Expiration = objDTO.Expiration.Replace("-", "/");
                }
                else if (objDTO.ExpirationDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                {
                    obj.Expiration = objDTO.ExpirationDate.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy");
                }


                if (!string.IsNullOrEmpty(objDTO.Received))
                {
                    obj.Received = objDTO.Received.Replace("-", "/");
                }
                else if (objDTO.ReceivedDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                {
                    obj.Received = objDTO.ReceivedDate.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy");
                }


                obj.InitialQuantity = objDTO.InitialQuantity;
                obj.InitialQuantityWeb = objDTO.InitialQuantity;

                obj.RefWebSelfGUID = objDTO.RefWebSelfGUID;
                if (!string.IsNullOrEmpty(objDTO.AddedFrom))
                    obj.AddedFrom = objDTO.AddedFrom;
                else
                    obj.AddedFrom = "Web";

                if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                    obj.EditedFrom = objDTO.AddedFrom;
                else
                    obj.EditedFrom = "Web";

                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                context.ItemLocationDetails.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;

                if (objDTO.Action == "Receive")
                {
                    ItemMaster oItemMaster = context.ItemMasters.Where(x => x.GUID == objDTO.ItemGUID && x.CompanyID == objDTO.CompanyID && x.Room == objDTO.Room).FirstOrDefault();
                    Room oRoom = context.Rooms.Where(x => x.ID == oItemMaster.Room && x.CompanyID == oItemMaster.CompanyID).FirstOrDefault();
                    if (oRoom.MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString() && !oItemMaster.Consignment)
                    {
                        new ItemMasterDAL(base.DataBaseName).UpdateItemAndPastReceiptCostByReceiveCost(oItemMaster.GUID, oItemMaster.Room.GetValueOrDefault(0), oItemMaster.CompanyID.GetValueOrDefault(0), false, objDTO.EditedFrom, objDTO.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow));
                    }
                    else if (oRoom.MethodOfValuingInventory == ((int)InventoryValuationMethod.LastCost).ToString() && !oItemMaster.Consignment)
                    {
                        new ItemMasterDAL(base.DataBaseName).UpdateItemAndPastReceiptCostByReceiveCost(oItemMaster.GUID, oItemMaster.Room.GetValueOrDefault(0), oItemMaster.CompanyID.GetValueOrDefault(0), true, objDTO.EditedFrom, objDTO.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow));
                    }
                }

                if (objDTO.ID > 0)
                {

                    //Get Cached-Media
                    //IEnumerable<ItemLocationDetailsDTO> ObjCache = CacheHelper<IEnumerable<ItemLocationDetailsDTO>>.GetCacheItem("Cached_ItemLocationDetails_" + objDTO.CompanyID.ToString());
                    //if (ObjCache != null)
                    //{
                    objDTO.SuggestedOrderQuantity = (new CartItemDAL(base.DataBaseName)).GetSuggestedOrderQtyForBin(objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.BinID.GetValueOrDefault(0));

                    //List<ItemLocationDetailsDTO> tempC = new List<ItemLocationDetailsDTO>();
                    //tempC.Add(objDTO);

                    //IEnumerable<ItemLocationDetailsDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    //CacheHelper<IEnumerable<ItemLocationDetailsDTO>>.AppendToCacheItem("Cached_ItemLocationDetails_" + objDTO.CompanyID.ToString(), NewCache);
                    //}

                    if (objDTO.OrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty || objDTO.TransferDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                    {
                        if (string.IsNullOrEmpty(objDTO.AddedFrom))
                        {
                            objDTO.AddedFrom = "Web";
                            objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            objDTO.EditedFrom = "Web";
                            objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        }
                        new ReceivedOrderTransferDetailDAL(base.DataBaseName).Insert(objDTO);
                    }

                }

                return objDTO;
            }
        }
        /// <summary>
        /// This method will be used for the Item Master, Item locations details, Quantity Updates....
        /// </summary>
        /// <param name="objData"></param>
        /// <returns></returns>
        public bool ItemLocationDetailsSave(List<ItemLocationDetailsDTO> objData, string RoomDateFormat, long SessionUserId, long EnterpriseId)
        {
            bool TempSerialNumberTracking = false;
            bool TempLotNumberTracking = false;
            //bool Consignment = false;
            ItemMasterDTO objItemDTO = null;
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            bool IsQTYSave = false;

            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                int LooptotalCount = objData.Count();
                int Counter = 0;
                Guid? ItemGUID = Guid.NewGuid();
                long RoomID = 0;
                long CompanyID = 0;
                //Added By Esha on 21/03/2013 To Add New Location To LocationMaster Table

                foreach (ItemLocationDetailsDTO item in objData)
                {
                    BinMasterDTO Itembin = GetItemBin(item.ItemGUID ?? Guid.Empty, item.BinNumber, item.Room ?? 0, item.CompanyID ?? 0, item.CreatedBy ?? 0, false);

                    if (Itembin != null)
                    {
                        item.BinID = Itembin.ID;
                    }

                    if (item.ItemGUID != null)
                    {
                        ItemGUID = item.ItemGUID;
                        RoomID = Convert.ToInt64(item.Room);
                        CompanyID = Convert.ToInt64(item.CompanyID);
                    }
                }

                //End By Esha
                foreach (ItemLocationDetailsDTO item in objData)
                {
                    #region "Location Detail Save"
                    Counter++;

                    if (item.BinID == null)
                    {
                        continue;
                    }
                    if (objItemDTO == null)
                    {
                        //objItemDTO = objItemDAL.GetRecord(item.ItemGUID.ToString(), (Int64)item.Room, (Int64)item.CompanyID, false, false);
                        objItemDTO = objItemDAL.GetItemWithoutJoins(null, item.ItemGUID);
                    }

                    item.IsArchived = (item.IsArchived == null ? false : item.IsArchived);
                    item.IsDeleted = (item.IsDeleted == null ? false : item.IsDeleted);
                    BinMasterDAL objBin = new BinMasterDAL(base.DataBaseName);
                    BinMasterDTO objBinDTO = objBin.GetBinByID(Convert.ToInt64(item.BinID), Convert.ToInt64(item.Room), Convert.ToInt64(item.CompanyID));

                    bool IsValid = false;

                    if (!string.IsNullOrEmpty(item.Received))
                        IsValid = true;
                    else if (!string.IsNullOrEmpty(item.SerialNumber))
                    {
                        IsValid = true;
                    }
                    else if (!string.IsNullOrEmpty(item.LotNumber))
                        IsValid = true;
                    else if (!string.IsNullOrEmpty(item.Expiration))
                        IsValid = true;
                    else if (item.LotNumberTracking && item.SerialNumberTracking)
                        IsValid = true;
                    else if (item.CustomerOwnedQuantity.HasValue && item.BinID.HasValue && item.Cost.HasValue && !string.IsNullOrEmpty(item.Received))
                        IsValid = true;

                    if (IsValid || (LooptotalCount == Counter))
                    {
                        //if (!String.IsNullOrEmpty(item.SerialNumber) || !String.IsNullOrEmpty(item.LotNumber) || item.CustomerOwnedQuantity.Value > 0 || item.ConsignedQuantity.Value > 0)
                        //{
                        ItemLocationDetailsDAL objItemDetailDAL = new ItemLocationDetailsDAL(base.DataBaseName);
                        ItemLocationDetailsDTO objItem = new ItemLocationDetailsDTO();
                        objItem.BinID = item.BinID;

                        objItem.CompanyID = item.CompanyID;
                        objItem.Cost = item.Cost;
                        objItem.Created = item.Created;

                        if (objItemDTO.Consignment)
                        {
                            //Consignment = true;
                        }

                        objItem.CustomerOwnedQuantity = item.CustomerOwnedQuantity;
                        objItem.ConsignedQuantity = item.ConsignedQuantity;


                        objItem.ExpirationDate = item.ExpirationDate;

                        objItem.Expiration = item.Expiration;
                        objItem.Received = item.Received;
                        objItem.ReceivedDate = item.ReceivedDate;
                        if (objItem.ReceivedDate.GetValueOrDefault(DateTime.MinValue) <= DateTime.MinValue && !string.IsNullOrEmpty(objItem.Received))
                        {
                            objItem.ReceivedDate = objItem.Received != null ? DateTime.ParseExact(objItem.Received, RoomDateFormat, eTurns.DTO.Resources.ResourceHelper.CurrentCult) : Convert.ToDateTime(objItem.Received);
                        }

                        objItem.ID = item.ID; objItem.ItemGUID = item.ItemGUID;

                        objItem.LotNumber = item.LotNumber;
                        if (item.LotNumber != null && item.LotNumberTracking)
                            TempLotNumberTracking = true;
                        objItem.SerialNumber = item.SerialNumber;
                        if (item.SerialNumber != null && item.SerialNumberTracking)
                            TempSerialNumberTracking = true;
                        objItem.CreatedBy = item.CreatedBy; objItem.CompanyID = item.CompanyID;
                        objItem.Room = item.Room; objItem.Updated = item.Updated;
                        objItem.LastUpdatedBy = item.LastUpdatedBy;
                        objItem.IsArchived = item.IsArchived; objItem.IsDeleted = item.IsDeleted;
                        objItem.Created = item.Created; objItem.CreatedBy = item.CreatedBy;
                        objItem.OrderDetailGUID = item.OrderDetailGUID;
                        if (objBinDTO != null)
                        {
                            objItem.BinNumber = objBinDTO.BinNumber;
                        }
                        objItem.CreatedByName = item.CreatedByName; objItem.UpdatedByName = item.UpdatedByName;
                        objItem.PackSlipNumber = item.PackSlipNumber;
                        objItem.UDF1 = item.UDF1;
                        objItem.UDF2 = item.UDF2;
                        objItem.UDF3 = item.UDF3;
                        objItem.UDF4 = item.UDF4;
                        objItem.UDF5 = item.UDF5;
                        objItem.Action = item.Action;

                        if (item.KitDetailGUID != null)
                            objItem.KitDetailGUID = item.KitDetailGUID;

                        if (item.TransferDetailGUID != null)
                            objItem.TransferDetailGUID = item.TransferDetailGUID;

                        //Fill with extra detail - Cache
                        FillWithExtraDetail(objItem);

                        if (objItemDTO.Consignment)
                        {
                            objItem.InitialQuantity = objItem.ConsignedQuantity;
                            objItem.IsConsignedSerialLot = true;
                        }
                        else
                        {
                            objItem.InitialQuantity = objItem.CustomerOwnedQuantity;
                            objItem.IsConsignedSerialLot = false;
                        }

                        if (item.ID > 0 && IsValid)
                        {
                            objItem.GUID = item.GUID;
                            if (item.IsOnlyFromUI)
                            {
                                objItem.EditedFrom = "Web";
                                objItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                            }
                            objItemDetailDAL.EditItemLocationQuantity(objItem);
                            // here needs to implement the edit quantity logic for location details table
                            IsQTYSave = true;
                        }
                        else
                        {
                            if (IsValid)
                            {
                                objItem.AddedFrom = "Web";
                                objItem.EditedFrom = "Web";
                                if (!string.IsNullOrEmpty(item.AddedFrom))
                                    objItem.AddedFrom = item.AddedFrom;

                                if (!string.IsNullOrEmpty(item.EditedFrom))
                                    objItem.EditedFrom = item.EditedFrom;

                                if (!string.IsNullOrEmpty(item.InsertedFrom))
                                    objItem.InsertedFrom = item.InsertedFrom;

                                //if ((objItem.ConsignedQuantity ?? 0) > 0)
                                //{
                                //    objItem.IsConsignedSerialLot = true;
                                //}
                                //else
                                //{
                                //    objItem.IsConsignedSerialLot = false;
                                //}
                                objItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objItem.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                ItemLocationDetailsDTO itemloctem = objItemDetailDAL.Insert(objItem);
                                item.ID = itemloctem.ID;
                                item.GUID = itemloctem.GUID;
                                IsQTYSave = true;
                            }
                        }
                    }
                    #endregion
                }

                #region "Insert QTY logic"
                if (IsQTYSave)
                {
                    if (TempSerialNumberTracking)
                    {
                        #region "Insert mode for Serial Number"
                        ItemLocationQTYDAL objQtyMgt = new ItemLocationQTYDAL(base.DataBaseName);

                        //FOR INSERT MODE
                        var objilqInsert = new List<ItemLocationQTYDTO>();

                        objilqInsert = (from x in objData
                                        where (x.SerialNumberTracking == true && x.SerialNumber != null) && x.BinID != null
                                        group x by new { x.BinID }
                                            into grp
                                        select new ItemLocationQTYDTO
                                        {
                                            BinID = Convert.ToInt64(grp.Key.BinID),
                                            ItemGUID = objData[0].ItemGUID,
                                            CompanyID = objData[0].CompanyID,
                                            Room = objData[0].Room,
                                            LastUpdated = DateTime.Now,
                                            Created = DateTime.Now,
                                            CreatedBy = objData[0].CreatedBy,
                                            LastUpdatedBy = objData[0].LastUpdatedBy,
                                            CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                            ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                            Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                        }).ToList();


                        //objilqInsert.Select(c => 
                        //{
                        //    if (objItemDTO.Consignment)
                        //    {
                        //        c.ConsignedQuantity= 1;
                        //    }
                        //    else
                        //    {
                        //        c.CustomerOwnedQuantity = 1;
                        //    }
                        //    return c; 
                        //}).ToList();



                        if (objilqInsert.Count() > 0)
                        {
                            objQtyMgt.Save(objilqInsert, SessionUserId, EnterpriseId);
                        }
                        #endregion
                    }
                    else if (TempLotNumberTracking)
                    {
                        #region "Insert Mode for Lot#, Others"
                        ItemLocationQTYDAL objQtyMgt = new ItemLocationQTYDAL(base.DataBaseName);
                        if (objItemDTO.Consignment)
                        {
                            var objilq = (from x in objData
                                          where (x.LotNumberTracking == true && x.LotNumber != null) && x.BinID != null
                                          group x by new { x.BinID } into grp
                                          select new ItemLocationQTYDTO
                                          {
                                              BinID = Convert.ToInt64(grp.Key.BinID),
                                              ItemGUID = objData[0].ItemGUID,
                                              CompanyID = objData[0].CompanyID,
                                              Room = objData[0].Room,
                                              LastUpdated = DateTime.Now,
                                              Created = DateTime.Now,
                                              CreatedBy = objData[0].CreatedBy,
                                              LastUpdatedBy = objData[0].LastUpdatedBy,
                                              CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                              ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                              Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                              //LotNumber = x.LotNumber,
                                          }).ToList();
                            //Save
                            objQtyMgt.Save(objilq, SessionUserId, EnterpriseId);
                            //Update on_hand qunatity
                            //objilq.Select(c => { OnHandQuantity += c.Quantity; return c; }).ToList();
                        }
                        else
                        {
                            var objilq = (from x in objData
                                          where (x.LotNumberTracking == true && x.LotNumber != null) && x.BinID != null
                                          group x by new { x.BinID } into grp
                                          select new ItemLocationQTYDTO
                                          {
                                              BinID = Convert.ToInt64(grp.Key.BinID),
                                              ItemGUID = objData[0].ItemGUID,
                                              CompanyID = objData[0].CompanyID,
                                              Room = objData[0].Room,
                                              LastUpdated = DateTime.Now,
                                              Created = DateTime.Now,
                                              CreatedBy = objData[0].CreatedBy,
                                              LastUpdatedBy = objData[0].LastUpdatedBy,
                                              CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                              ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                              Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                              //LotNumber = x.LotNumber,
                                          }).ToList();

                            //Save
                            objQtyMgt.Save(objilq, SessionUserId, EnterpriseId);
                        }
                        //Save on hand quantity
                        #endregion
                    }
                    else
                    {
                        #region "Insert Mode for Lot#, Others"
                        ItemLocationQTYDAL objQtyMgt = new ItemLocationQTYDAL(base.DataBaseName);
                        if (objItemDTO.Consignment)
                        {
                            var objilq = (from x in objData
                                          where x.BinID != null
                                          group x by new { x.BinID } into grp
                                          select new ItemLocationQTYDTO
                                          {
                                              BinID = Convert.ToInt64(grp.Key.BinID),
                                              ItemGUID = objData[0].ItemGUID,
                                              CompanyID = objData[0].CompanyID,
                                              Room = objData[0].Room,
                                              LastUpdated = DateTime.Now,
                                              Created = DateTime.Now,
                                              CreatedBy = objData[0].CreatedBy,
                                              LastUpdatedBy = objData[0].LastUpdatedBy,
                                              CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                              ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                              Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                              //LotNumber = x.LotNumber,
                                          }).ToList();
                            //Save
                            objQtyMgt.Save(objilq, SessionUserId, EnterpriseId);
                            //Update on_hand qunatity
                            //objilq.Select(c => { OnHandQuantity += c.Quantity; return c; }).ToList();
                        }
                        else
                        {
                            var objilq = (from x in objData
                                          where x.BinID != null
                                          group x by new { x.BinID } into grp
                                          select new ItemLocationQTYDTO
                                          {
                                              BinID = Convert.ToInt64(grp.Key.BinID),
                                              ItemGUID = objData[0].ItemGUID,
                                              CompanyID = objData[0].CompanyID,
                                              Room = objData[0].Room,
                                              LastUpdated = DateTime.Now,
                                              Created = DateTime.Now,
                                              CreatedBy = objData[0].CreatedBy,
                                              LastUpdatedBy = objData[0].LastUpdatedBy,
                                              CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                              ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                              Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                              //LotNumber = x.LotNumber,
                                          }).ToList();

                            //Save
                            objQtyMgt.Save(objilq, SessionUserId, EnterpriseId);
                        }
                        //Save on hand quantity
                        #endregion
                    }
                }
                #endregion

                #region "update Cost"

                //if (RoomID > 0 && CompanyID > 0)
                //{
                //    Guid defaultId = Guid.NewGuid();
                //    objItemDAL.UpdateCostMarkupSellPrice(ItemGUID ?? defaultId, Convert.ToInt64(RoomID), Convert.ToInt64(CompanyID), string.Empty, false);
                //}

                #endregion
            }
            return true;
        }


        public BinMasterDTO GetItemBin(Guid ItemGUID, string BinName, long RoomID, long CompanyID, long UserID, bool StagingLocation, string EditedFrom = "", bool IsFromService = false, Guid? materialStagingGUID = null)
        {
            BinMasterDAL objBinMasterDAL = new BinMasterDAL(base.DataBaseName);
            return objBinMasterDAL.GetItemBin(ItemGUID, BinName, RoomID, CompanyID, UserID, StagingLocation, EditedFrom, IsFromService, materialStagingGUID);
        }

        /// <summary>
        /// This method will be used for the Item Master, Item locations details, Quantity Updates....
        /// </summary>
        /// <param name="objData"></param>
        /// <returns></returns>
        public bool ItemLocationDetailsImportSave(ItemLocationDetailsDTO objData, long SessionUserId, long EnterpriseId)
        {
            bool TempSerialNumberTracking = false;
            bool TempLotNumberTracking = false;
            //bool Consignment = false;
            ItemMasterDTO objItemDTO = null;
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            bool IsQTYSave = false;

            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //int LooptotalCount = 1;
                //int Counter = 0;
                Guid? ItemGUID = Guid.NewGuid();
                long RoomID = 0;
                long CompanyID = 0;
                //Added By Esha on 21/03/2013 To Add New Location To LocationMaster Table

                // foreach (ItemLocationDetailsDTO item in objData)
                //{
                if (objData.ItemGUID != null)
                {
                    ItemGUID = objData.ItemGUID;
                    RoomID = Convert.ToInt64(objData.Room);
                    CompanyID = Convert.ToInt64(objData.CompanyID);
                }

                // }

                //End By Esha
                //  foreach (ItemLocationDetailsDTO item in objData)
                //  {
                #region "Location Detail Save"
                //Counter++;

                if (objData.BinID == null)
                {
                    return false;
                }
                if (objItemDTO == null)
                {
                    objItemDTO = objItemDAL.GetRecordOnlyItemsFields(objData.ItemGUID.ToString(), (Int64)objData.Room, (Int64)objData.CompanyID, false, false);
                }

                objData.IsArchived = (objData.IsArchived == null ? false : objData.IsArchived);
                objData.IsDeleted = (objData.IsDeleted == null ? false : objData.IsDeleted);
                BinMasterDAL objBin = new BinMasterDAL(base.DataBaseName);
                BinMasterDTO objBinDTO = objBin.GetBinByID(Convert.ToInt64(objData.BinID), Convert.ToInt64(objData.Room), Convert.ToInt64(objData.CompanyID));
                //BinMasterDTO objBinDTO = objBin.GetItemLocation( Convert.ToInt64(objData.Room), Convert.ToInt64(objData.CompanyID), false, false,Guid.Empty, Convert.ToInt64(objData.BinID),null,null).FirstOrDefault();

                bool IsValid = false;

                if (!string.IsNullOrEmpty(objData.Received))
                    IsValid = true;
                else if (!string.IsNullOrEmpty(objData.SerialNumber))
                {
                    IsValid = true;
                }
                else if (!string.IsNullOrEmpty(objData.LotNumber))
                    IsValid = true;
                else if (!string.IsNullOrEmpty(objData.Expiration))
                    IsValid = true;
                //else if (objData.LotNumberTracking && objData.SerialNumberTracking)
                //    IsValid = true;
                else if (objData.CustomerOwnedQuantity.HasValue && objData.BinID.HasValue && objData.Cost.HasValue && !string.IsNullOrEmpty(objData.Received))
                    IsValid = true;

                if (IsValid)
                {
                    //if (!String.IsNullOrEmpty(item.SerialNumber) || !String.IsNullOrEmpty(item.LotNumber) || item.CustomerOwnedQuantity.Value > 0 || item.ConsignedQuantity.Value > 0)
                    //{
                    ItemLocationDetailsDAL objItemDetailDAL = new ItemLocationDetailsDAL(base.DataBaseName);
                    ItemLocationDetailsDTO objItem = new ItemLocationDetailsDTO();
                    objItem.BinID = objData.BinID;

                    objItem.CompanyID = objData.CompanyID;
                    objItem.Cost = objData.Cost;
                    objItem.Created = objData.Created;

                    if (objItemDTO.Consignment)
                    {
                        //Consignment = true;
                    }

                    objItem.CustomerOwnedQuantity = objData.CustomerOwnedQuantity;
                    objItem.ConsignedQuantity = objData.ConsignedQuantity;
                    objItem.ExpirationDate = objData.ExpirationDate;

                    objItem.Expiration = objData.Expiration;
                    objItem.Received = objData.Received;
                    objItem.ReceivedDate = objData.ReceivedDate;

                    if (objItem.ReceivedDate.GetValueOrDefault(DateTime.MinValue) <= DateTime.MinValue && !string.IsNullOrEmpty(objItem.Received))
                    {
                        objItem.ReceivedDate = DateTime.Parse(objItem.Received);
                    }
                    if (objItem.ExpirationDate.GetValueOrDefault(DateTime.MinValue) <= DateTime.MinValue && !string.IsNullOrEmpty(objItem.Expiration))
                    {
                        objItem.ExpirationDate = DateTime.Parse(objItem.Expiration);
                    }

                    objItem.ID = objData.ID; objItem.ItemGUID = objData.ItemGUID;

                    objItem.LotNumber = objData.LotNumber;
                    if (objData.LotNumber != null && objItemDTO.LotNumberTracking == true)
                    {
                        TempLotNumberTracking = true;
                        objData.LotNumberTracking = true;
                    }

                    objItem.SerialNumber = objData.SerialNumber;
                    if (objData.SerialNumber != null && objItemDTO.SerialNumberTracking == true)
                    {
                        TempSerialNumberTracking = true;
                        objData.SerialNumberTracking = true;
                    }

                    objItem.CreatedBy = objData.CreatedBy; objItem.CompanyID = objData.CompanyID;
                    objItem.Room = objData.Room; objItem.Updated = objData.Updated;
                    objItem.LastUpdatedBy = objData.LastUpdatedBy;
                    objItem.IsArchived = objData.IsArchived; objItem.IsDeleted = objData.IsDeleted;
                    objItem.Created = objData.Created; objItem.CreatedBy = objData.CreatedBy;
                    objItem.OrderDetailGUID = objData.OrderDetailGUID;
                    objItem.BinNumber = objBinDTO.BinNumber;
                    objItem.CreatedByName = objData.CreatedByName;
                    objItem.UpdatedByName = objData.UpdatedByName;
                    objItem.InsertedFrom = objData.InsertedFrom;

                    if (objItem.ConsignedQuantity.GetValueOrDefault(0) > 0)
                        objItem.IsConsignedSerialLot = true;
                    else
                        objItem.IsConsignedSerialLot = false;

                    if (objData.KitDetailGUID != null)
                        objItem.KitDetailGUID = objData.KitDetailGUID;

                    if (objData.TransferDetailGUID != null)
                        objItem.TransferDetailGUID = objData.TransferDetailGUID;

                    //Fill with extra detail - Cache
                    FillWithExtraDetail(objItem);

                    objItem.Action = "Receive";
                    if (objData.ID > 0 && IsValid)
                    {
                        objItem.GUID = objData.GUID;
                        objItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objItem.EditedFrom = "Web";
                        objItemDetailDAL.Edit(objItem);
                        // here needs to implement the edit quantity logic for location details table
                        IsQTYSave = true;
                    }
                    else
                    {
                        if (IsValid)
                        {
                            objItem.AddedFrom = "Web";
                            objItem.EditedFrom = "Web";
                            objItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objItem.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                            IQueryable<ItemLocationDetail> oPrevReceived = context.ItemLocationDetails.Where(x => (x.IsArchived ?? false) == false && (x.IsDeleted ?? false) == false && x.BinID == objItem.BinID && x.ItemGUID == objItem.ItemGUID && x.Room == objItem.Room && x.CompanyID == objItem.CompanyID && (x.SerialNumber ?? string.Empty) == (objItem.SerialNumber ?? string.Empty) && (x.LotNumber ?? string.Empty) == (objItem.LotNumber ?? string.Empty) && (x.ConsignedQuantity > 0 || x.CustomerOwnedQuantity > 0));

                            if (oPrevReceived != null && oPrevReceived.Count() > 0)
                            {
                                foreach (ItemLocationDetail item in oPrevReceived)
                                {
                                    ReceivedOrderTransferDetail oReceiveOrd = context.ReceivedOrderTransferDetails.FirstOrDefault(x => x.ItemLocationDetailGUID == item.GUID);

                                    double TransactionQuantity = 0;
                                    if (item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                                    {
                                        if (item.InitialQuantityPDA.GetValueOrDefault(0) > 0)
                                        {
                                            item.InitialQuantityPDA = item.InitialQuantityPDA.Value - item.ConsignedQuantity.Value;
                                        }
                                        else if (item.InitialQuantityWeb.GetValueOrDefault(0) > 0)
                                        {
                                            item.InitialQuantityWeb = item.InitialQuantityWeb.Value - item.ConsignedQuantity.Value;
                                        }

                                        item.InitialQuantity = item.InitialQuantity.Value - item.ConsignedQuantity.Value;
                                        TransactionQuantity = item.ConsignedQuantity.Value;
                                        item.ConsignedQuantity = 0;
                                        if (oReceiveOrd != null)
                                            oReceiveOrd.ConsignedQuantity = item.InitialQuantity.GetValueOrDefault(0);
                                    }

                                    if (item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                                    {
                                        if (item.InitialQuantityPDA.GetValueOrDefault(0) > 0)
                                        {
                                            item.InitialQuantityPDA = item.InitialQuantityPDA.Value - item.CustomerOwnedQuantity.Value;
                                        }
                                        else if (item.InitialQuantityWeb.GetValueOrDefault(0) > 0)
                                        {
                                            item.InitialQuantityWeb = item.InitialQuantityWeb.Value - item.CustomerOwnedQuantity.Value;
                                        }

                                        item.InitialQuantity = item.InitialQuantity.Value - item.CustomerOwnedQuantity.Value;
                                        TransactionQuantity = item.CustomerOwnedQuantity.Value;
                                        item.CustomerOwnedQuantity = 0;
                                        if (oReceiveOrd != null)
                                            oReceiveOrd.CustomerOwnedQuantity = item.InitialQuantity.GetValueOrDefault(0);
                                    }

                                    item.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    item.EditedFrom = "Web";

                                    if (oReceiveOrd != null)
                                    {
                                        oReceiveOrd.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        oReceiveOrd.EditedFrom = "Web";
                                    }

                                    if (!string.IsNullOrWhiteSpace(item.InsertedFrom) && item.InsertedFrom.ToLower() == "import")
                                    {
                                        string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
                                        SqlHelper.ExecuteNonQuery(Connectionstring, "AT_InsertImportUpdate", item.ID, TransactionQuantity);
                                    }
                                }
                                context.SaveChanges();
                            }

                            ItemLocationDetailsDTO itemloctem = objItemDetailDAL.Insert(objItem);
                            objData.ID = itemloctem.ID;
                            objData.GUID = itemloctem.GUID;
                            IsQTYSave = true;
                        }
                    }
                }
                #endregion

                #region "Insert QTY logic"
                if (IsQTYSave)
                {
                    if (TempSerialNumberTracking)
                    {
                        #region "Insert mode for Serial Number"
                        ItemLocationQTYDAL objQtyMgt = new ItemLocationQTYDAL(base.DataBaseName);

                        //FOR INSERT MODE
                        var objilqInsert = new List<ItemLocationQTYDTO>();
                        var objList = new ItemLocationQTYDTO();

                        if ((objData.SerialNumberTracking == true && objData.SerialNumber != null) && objData.BinID != null)
                        {
                            objList.BinID = Convert.ToInt64(objData.BinID);
                            objList.ItemGUID = objData.ItemGUID;
                            objList.CompanyID = objData.CompanyID;
                            objList.Room = objData.Room;
                            objList.LastUpdated = DateTimeUtility.DateTimeNow;
                            objList.Created = DateTimeUtility.DateTimeNow;
                            objList.CreatedBy = objData.CreatedBy;
                            objList.LastUpdatedBy = objData.LastUpdatedBy;
                            objList.CustomerOwnedQuantity = objData.CustomerOwnedQuantity ?? 0;
                            objList.ConsignedQuantity = objData.ConsignedQuantity ?? 0;
                            objList.Quantity = objData.ConsignedQuantity ?? 0 + objData.CustomerOwnedQuantity ?? 0;

                            objilqInsert.Add(objList);

                            if (objilqInsert.Count() > 0)
                            {
                                objQtyMgt.Save(objilqInsert, objData.InsertedFrom, SessionUserId, EnterpriseId);
                            }
                        }


                        #endregion
                    }
                    else if (TempLotNumberTracking)
                    {
                        #region "Insert Mode for Lot#, Others"
                        ItemLocationQTYDAL objQtyMgt = new ItemLocationQTYDAL(base.DataBaseName);

                        if (objItemDTO.Consignment)
                        {
                            var objilqInsert = new List<ItemLocationQTYDTO>();
                            var objList = new ItemLocationQTYDTO();

                            if ((objData.LotNumberTracking == true && objData.LotNumber != null) && objData.BinID != null)
                            {
                                objList.BinID = Convert.ToInt64(objData.BinID);
                                objList.ItemGUID = objData.ItemGUID;
                                objList.CompanyID = objData.CompanyID;
                                objList.Room = objData.Room;
                                objList.LastUpdated = DateTimeUtility.DateTimeNow;
                                objList.Created = DateTimeUtility.DateTimeNow;
                                objList.CreatedBy = objData.CreatedBy;
                                objList.LastUpdatedBy = objData.LastUpdatedBy;
                                objList.CustomerOwnedQuantity = objData.CustomerOwnedQuantity ?? 0;
                                objList.ConsignedQuantity = objData.ConsignedQuantity ?? 0;
                                objList.Quantity = objData.ConsignedQuantity ?? 0 + objData.CustomerOwnedQuantity ?? 0;
                                objList.LotNumber = objData.LotNumber;
                                objilqInsert.Add(objList);
                                objQtyMgt.Save(objilqInsert, objData.InsertedFrom, SessionUserId, EnterpriseId);
                            }




                        }
                        else
                        {

                            var objilqInsert = new List<ItemLocationQTYDTO>();
                            var objList = new ItemLocationQTYDTO();

                            if ((objData.LotNumberTracking == true && objData.LotNumber != null) && objData.BinID != null)
                            {
                                objList.BinID = Convert.ToInt64(objData.BinID);
                                objList.ItemGUID = objData.ItemGUID;
                                objList.CompanyID = objData.CompanyID;
                                objList.Room = objData.Room;
                                objList.LastUpdated = DateTimeUtility.DateTimeNow;
                                objList.Created = DateTimeUtility.DateTimeNow;
                                objList.CreatedBy = objData.CreatedBy;
                                objList.LastUpdatedBy = objData.LastUpdatedBy;
                                objList.CustomerOwnedQuantity = objData.CustomerOwnedQuantity ?? 0;
                                objList.ConsignedQuantity = objData.ConsignedQuantity ?? 0;
                                objList.Quantity = objData.ConsignedQuantity ?? 0 + objData.CustomerOwnedQuantity ?? 0;
                                // objList.LotNumber = objData.LotNumber;
                                objilqInsert.Add(objList);
                                objQtyMgt.Save(objilqInsert, objData.InsertedFrom, SessionUserId, EnterpriseId);
                            }






                        }
                        //Save on hand quantity
                        #endregion
                    }
                    else
                    {
                        #region "Insert Mode for Lot#, Others"
                        ItemLocationQTYDAL objQtyMgt = new ItemLocationQTYDAL(base.DataBaseName);



                        if (objItemDTO.Consignment)
                        {
                            var objilqInsert = new List<ItemLocationQTYDTO>();
                            var objList = new ItemLocationQTYDTO();

                            if (objData.BinID != null)
                            {
                                objList.BinID = Convert.ToInt64(objData.BinID);
                                objList.ItemGUID = objData.ItemGUID;
                                objList.CompanyID = objData.CompanyID;
                                objList.Room = objData.Room;
                                objList.LastUpdated = DateTimeUtility.DateTimeNow;
                                objList.Created = DateTimeUtility.DateTimeNow;
                                objList.CreatedBy = objData.CreatedBy;
                                objList.LastUpdatedBy = objData.LastUpdatedBy;
                                objList.CustomerOwnedQuantity = objData.CustomerOwnedQuantity ?? 0;
                                objList.ConsignedQuantity = objData.ConsignedQuantity ?? 0;
                                objList.Quantity = objData.ConsignedQuantity ?? 0 + objData.CustomerOwnedQuantity ?? 0;
                                // objList.LotNumber = objData.LotNumber;
                                objilqInsert.Add(objList);
                                objQtyMgt.Save(objilqInsert, objData.InsertedFrom, SessionUserId, EnterpriseId);
                            }

                        }
                        else
                        {

                            var objilqInsert = new List<ItemLocationQTYDTO>();
                            var objList = new ItemLocationQTYDTO();

                            if (objData.BinID != null)
                            {
                                objList.BinID = Convert.ToInt64(objData.BinID);
                                objList.ItemGUID = objData.ItemGUID;
                                objList.CompanyID = objData.CompanyID;
                                objList.Room = objData.Room;
                                objList.LastUpdated = DateTimeUtility.DateTimeNow;
                                objList.Created = DateTimeUtility.DateTimeNow;
                                objList.CreatedBy = objData.CreatedBy;
                                objList.LastUpdatedBy = objData.LastUpdatedBy;
                                objList.CustomerOwnedQuantity = objData.CustomerOwnedQuantity ?? 0;
                                objList.ConsignedQuantity = objData.ConsignedQuantity ?? 0;
                                objList.Quantity = objData.ConsignedQuantity ?? 0 + objData.CustomerOwnedQuantity ?? 0;
                                // objList.LotNumber = objData.LotNumber;
                                objilqInsert.Add(objList);
                                objQtyMgt.Save(objilqInsert, objData.InsertedFrom, SessionUserId, EnterpriseId);
                            }





                        }
                        //Save on hand quantity
                        #endregion
                    }
                }
                #endregion

                #region "update Cost"

                //Guid defaultId = Guid.NewGuid();
                //objItemDAL.UpdateCostMarkupSellPrice(ItemGUID ?? defaultId, Convert.ToInt64(RoomID), Convert.ToInt64(CompanyID), objData.InsertedFrom, false);

                #endregion
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public ItemLocationDetailsDTO FillWithExtraDetail(ItemLocationDetailsDTO objDTO)
        {
            BinMasterDAL objITLQDAL = new BinMasterDAL(base.DataBaseName);
            BinMasterDTO objResult = objITLQDAL.GetBinByID((Int64)objDTO.BinID.GetValueOrDefault(0), (Int64)objDTO.Room.GetValueOrDefault(0), (Int64)objDTO.CompanyID.GetValueOrDefault(0));

            if (objResult != null)
            {
                objDTO.CriticalQuantity = objResult.CriticalQuantity;
                objDTO.MinimumQuantity = objResult.MinimumQuantity;
                objDTO.MaximumQuantity = objResult.MaximumQuantity;
            }
            return objDTO;
        }
        public bool CheckSerialExistsOrNot(string SerialNumber, Guid ItemGuid, Int64 RoomId, Int64 CompanyID)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@SerialNumber", SerialNumber), new SqlParameter("@ItemGuid", ItemGuid), new SqlParameter("@RoomId", RoomId), new SqlParameter("@CompanyID", CompanyID) };
                    if (context.Database.SqlQuery<Double>("exec [CheckSerialExistsOrNot] @SerialNumber,@ItemGuid,@RoomId,@CompanyID", params1).FirstOrDefault() > 0)
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
        public bool ItemLocationDetailsSaveForCreditPullnew(List<ItemLocationDetailsDTO> objData, string calledfrom, string RoomDateFormat, out List<CreditHistory> lstCreditGuids, long SessionUserId, long EnterpriseId, string PullAction = "credit", string WhatWhereAction = "")
        {
            lstCreditGuids = new List<CreditHistory>();

            if (!string.IsNullOrWhiteSpace(calledfrom))
            {
                if ("evmisvc" == calledfrom)
                {
                    PullAction = "cr_evmi";
                }
            }

            bool TempSerialNumberTracking = false;
            bool TempLotNumberTracking = false;
            //bool Consignment = false;
            ItemMasterDTO objItemDTO = null;
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            bool IsQTYSave = false;
            bool IsBothQty = false;
            string CountdtlGUID = string.Empty;
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                int LooptotalCount = objData.Count();
                int Counter = 0;
                foreach (ItemLocationDetailsDTO item in objData)
                {
                    BinMasterDTO Itembin = GetItemBin(item.ItemGUID ?? Guid.Empty, item.BinNumber, item.Room ?? 0, item.CompanyID ?? 0, item.CreatedBy ?? 0, false);
                    item.BinID = Itembin.ID;
                }
                List<PullDetailsDTO> objPullDetailsDTOs = new List<PullDetailsDTO>();
                foreach (ItemLocationDetailsDTO item in objData)
                {
                    #region "Location Detail Save"
                    Counter++;

                    if (item.BinID == null)
                    {
                        continue;
                    }
                    if (objItemDTO == null)
                    {
                        objItemDTO = objItemDAL.GetItemWithoutJoins(null, item.ItemGUID);
                    }

                    item.IsArchived = (item.IsArchived == null ? false : item.IsArchived);
                    item.IsDeleted = (item.IsDeleted == null ? false : item.IsDeleted);
                    BinMasterDAL objBin = new BinMasterDAL(base.DataBaseName);
                    BinMasterDTO objBinDTO = objBin.GetBinByID(Convert.ToInt64(item.BinID), Convert.ToInt64(item.Room), Convert.ToInt64(item.CompanyID));
                    //BinMasterDTO objBinDTO = objBin.GetItemLocation(Convert.ToInt64(item.Room), Convert.ToInt64(item.CompanyID), false, false,Guid.Empty, Convert.ToInt64(item.BinID), null,null).FirstOrDefault();

                    if (string.IsNullOrEmpty(item.Received) && item.ReceivedDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                        item.Received = item.ReceivedDate.Value.ToString("M/d/yyyy");

                    bool IsValid = false;

                    if (!string.IsNullOrEmpty(item.Received) || item.ReceivedDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                        IsValid = true;
                    else if (!string.IsNullOrEmpty(item.SerialNumber))
                    {
                        IsValid = true;
                    }
                    else if (!string.IsNullOrEmpty(item.LotNumber))
                        IsValid = true;
                    else if (!string.IsNullOrEmpty(item.Expiration) || (item.ExpirationDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue))
                        IsValid = true;
                    else if (item.LotNumberTracking && item.SerialNumberTracking)
                        IsValid = true;
                    else if (item.BinID.HasValue && item.Cost.HasValue && (!string.IsNullOrEmpty(item.Received) || item.ReceivedDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue))
                        IsValid = true;

                    if (IsValid || (LooptotalCount == Counter))
                    {
                        //if (!String.IsNullOrEmpty(item.SerialNumber) || !String.IsNullOrEmpty(item.LotNumber) || item.CustomerOwnedQuantity.Value > 0 || item.ConsignedQuantity.Value > 0)
                        //{
                        double CustVal = 0;
                        double ConsignVal = 0;
                        bool IsConsinAndCustBothQty = false;
                        if (item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0 && item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                        {
                            CustVal = item.CustomerOwnedQuantity.GetValueOrDefault(0);
                            ConsignVal = item.ConsignedQuantity.GetValueOrDefault(0);
                            IsConsinAndCustBothQty = true;
                            IsBothQty = true;
                        }
                        else if (item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0 && item.ConsignedQuantity.GetValueOrDefault(0) <= 0)
                        {
                            CustVal = item.CustomerOwnedQuantity.GetValueOrDefault(0);
                            ConsignVal = 0;
                            IsConsinAndCustBothQty = false;
                            IsBothQty = false;
                        }
                        else if (item.ConsignedQuantity.GetValueOrDefault(0) > 0 && item.CustomerOwnedQuantity.GetValueOrDefault(0) <= 0)
                        {
                            ConsignVal = item.ConsignedQuantity.GetValueOrDefault(0);
                            CustVal = 0;
                            IsBothQty = false;
                            IsConsinAndCustBothQty = false;
                        }

                        if (IsConsinAndCustBothQty)
                        {
                            item.ConsignedQuantity = ConsignVal;
                            item.CustomerOwnedQuantity = 0;
                            SaveBothCustAndConsigenQtyForCount(item, objItemDTO, objBinDTO, IsValid, PullAction, objData, SessionUserId, EnterpriseId);

                            item.ConsignedQuantity = 0;
                            item.CustomerOwnedQuantity = CustVal;
                            item.ID = 0;
                            item.GUID = Guid.Empty;
                            SaveBothCustAndConsigenQtyForCount(item, objItemDTO, objBinDTO, IsValid, PullAction, objData, SessionUserId, EnterpriseId);
                            continue;
                        }
                        else
                        {
                            ItemLocationDetailsDAL objItemDetailDAL = new ItemLocationDetailsDAL(base.DataBaseName);
                            ItemLocationDetailsDTO objItem = new ItemLocationDetailsDTO();
                            objItem.BinID = item.BinID;

                            objItem.CompanyID = item.CompanyID;
                            objItem.Cost = item.Cost;
                            objItem.Created = item.Created;


                            objItem.CustomerOwnedQuantity = item.CustomerOwnedQuantity;
                            objItem.ConsignedQuantity = item.ConsignedQuantity;
                            objItem.InitialQuantity = (item.ConsignedQuantity ?? 0) + (item.CustomerOwnedQuantity ?? 0);
                            objItem.InitialQuantityWeb = (item.ConsignedQuantity ?? 0) + (item.CustomerOwnedQuantity ?? 0);
                            objItem.IsConsignedSerialLot = objItemDTO.Consignment;
                            objItem.ExpirationDate = item.ExpirationDate;

                            objItem.Expiration = item.Expiration;
                            objItem.Received = item.Received;
                            objItem.ReceivedDate = item.ReceivedDate;
                            if (objItem.ReceivedDate.GetValueOrDefault(DateTime.MinValue) <= DateTime.MinValue && !string.IsNullOrEmpty(objItem.Received))
                            {
                                objItem.ReceivedDate = DateTime.Parse(objItem.Received);
                            }
                            if (item.DateCodeTracking && objItem.ExpirationDate.GetValueOrDefault(DateTime.MinValue) <= DateTime.MinValue && !string.IsNullOrEmpty(objItem.Expiration))
                            {
                                DateTime dt = new DateTime();
                                if (DateTime.TryParseExact(objItem.Expiration, RoomDateFormat, ResourceHelper.CurrentCult, DateTimeStyles.None, out dt))
                                    objItem.ExpirationDate = dt;
                            }

                            objItem.ID = item.ID; objItem.ItemGUID = item.ItemGUID;

                            objItem.LotNumber = item.LotNumber;
                            if (item.LotNumber != null && item.LotNumberTracking)
                                TempLotNumberTracking = true;
                            objItem.SerialNumber = item.SerialNumber;
                            if (item.SerialNumber != null && item.SerialNumberTracking)
                                TempSerialNumberTracking = true;

                            objItem.CreatedBy = item.CreatedBy; objItem.CompanyID = item.CompanyID;
                            objItem.Room = item.Room; objItem.Updated = item.Updated;
                            objItem.LastUpdatedBy = item.LastUpdatedBy;
                            objItem.IsArchived = item.IsArchived; objItem.IsDeleted = item.IsDeleted;
                            objItem.Created = item.Created; objItem.CreatedBy = item.CreatedBy;
                            objItem.OrderDetailGUID = item.OrderDetailGUID;

                            if (objBinDTO != null)
                                objItem.BinNumber = objBinDTO.BinNumber;

                            if (item.ID > 0 && IsValid)
                            {
                                objItem.GUID = item.GUID;
                                objItem.IsOnlyFromUI = item.IsOnlyFromUI;
                                if (item.IsOnlyFromUI)
                                {
                                    objItem.EditedFrom = "Web";
                                    objItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                                }
                                objItemDetailDAL.Edit(objItem);
                                // here needs to implement the edit quantity logic for location details table
                                IsQTYSave = true;
                            }
                            else
                            {
                                if (IsValid)
                                {
                                    bool IsRecordSave = false;
                                    //// here needs to write code for 4 points as following...
                                    // 1 // if item is SR. then check for duplication if found then make them IsDeleted = 1
                                    //   // And then Insert the New item with same SR. number
                                    // 2 // If item is Lot, then check if duplicate number then only update the quantity
                                    //   // for that particular lot number also check BinLocation if exist then update else insert ....
                                    // 3 // Needs to enter only one entry to PullMaster with Total Quantity and data
                                    // 4 // Then needs to enter all the entry to PullDetails with itemlocation data ..
                                    #region "SR Case"
                                    if (TempSerialNumberTracking)
                                    {
                                        //var TempDupSrs = objItemDetailDAL.GetCachedData(item.ItemGUID.Value, item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0)).Where(x => x.SerialNumber == item.SerialNumber).ToList();
                                        var TempDupSrs = objItemDetailDAL.GetItemLocationDetail(item.ItemGUID.Value, item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0), null, item.SerialNumber, string.Empty, string.Empty, null, null, string.Empty).ToList();
                                        if (TempDupSrs != null && TempDupSrs.Count() > 0)
                                        {
                                            foreach (ItemLocationDetailsDTO IDTemp in TempDupSrs)
                                            {
                                                IDTemp.IsDeleted = true;
                                                IDTemp.IsArchived = true;
                                                IDTemp.LastUpdatedBy = item.LastUpdatedBy;
                                                IDTemp.Updated = DateTimeUtility.DateTimeNow;
                                                IDTemp.IsOnlyFromUI = true;
                                                IDTemp.EditedFrom = "Web";
                                                IDTemp.ReceivedOn = DateTimeUtility.DateTimeNow;
                                                objItemDetailDAL.Edit(IDTemp);
                                            }
                                        }
                                    }
                                    #endregion
                                    //#region "LOT Case"
                                    //if (TempLotNumberTracking)
                                    //{// bin wise check the lot nummber if found then updated else insert case ....
                                    //    var TempDupSrs = objItemDetailDAL.GetCachedData(item.ItemGUID.Value, item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0)).Where(x => x.LotNumber == item.LotNumber && x.BinID == item.BinID).SingleOrDefault();
                                    //    if (TempDupSrs != null)
                                    //    {
                                    //        TempDupSrs.ConsignedQuantity = TempDupSrs.ConsignedQuantity.GetValueOrDefault(0) + objItem.ConsignedQuantity.GetValueOrDefault(0);
                                    //        TempDupSrs.CustomerOwnedQuantity = TempDupSrs.CustomerOwnedQuantity.GetValueOrDefault(0) + objItem.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    //        objItemDetailDAL.Edit(TempDupSrs);
                                    //        IsRecordSave = true;
                                    //    }
                                    //}
                                    //#endregion

                                    if (!IsRecordSave)
                                    {
                                        objItem.AddedFrom = "Web";
                                        objItem.EditedFrom = "Web";
                                        objItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objItem.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                        objItem.IsOnlyFromUI = item.IsOnlyFromUI;
                                        ItemLocationDetailsDTO itemloctemp = objItemDetailDAL.Insert(objItem);
                                        item.ID = itemloctemp.ID;
                                        item.GUID = itemloctemp.GUID;
                                        IsQTYSave = true;
                                    }
                                    #region "PULL Details Entry"
                                    PullDetailsDTO objPullDetailDTO = new PullDetailsDTO();
                                    objPullDetailDTO.BinID = item.BinID;
                                    objPullDetailDTO.CompanyID = item.CompanyID;
                                    objPullDetailDTO.ConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0);
                                    objPullDetailDTO.Created = DateTimeUtility.DateTimeNow;
                                    objPullDetailDTO.CreatedBy = item.CreatedBy.GetValueOrDefault(0);
                                    objPullDetailDTO.CreatedByName = item.CreatedByName;
                                    objPullDetailDTO.CustomerOwnedQuantity = item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    objPullDetailDTO.Expiration = item.Expiration;
                                    objPullDetailDTO.IsArchived = item.IsArchived;
                                    objPullDetailDTO.IsDeleted = item.IsDeleted;
                                    objPullDetailDTO.ItemCost = CalculateAndGetPullCreditCost(item.Cost, objItemDTO.CostUOMID, item.Room ?? 0, item.CompanyID ?? 0);
                                    objPullDetailDTO.ItemPrice = CalculateAndGetPullCreditCost(item.SellPrice, objItemDTO.CostUOMID, item.Room ?? 0, item.CompanyID ?? 0);
                                    objPullDetailDTO.ItemGUID = item.ItemGUID;
                                    objPullDetailDTO.LastUpdatedBy = item.LastUpdatedBy.GetValueOrDefault(0);
                                    objPullDetailDTO.LotNumber = item.LotNumber;
                                    objPullDetailDTO.PoolQuantity = (item.ConsignedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0));
                                    objPullDetailDTO.PullCredit = PullAction;
                                    objPullDetailDTO.Received = item.Received;
                                    objPullDetailDTO.Room = item.Room;
                                    objPullDetailDTO.RoomName = item.RoomName;
                                    objPullDetailDTO.SerialNumber = item.SerialNumber;
                                    objPullDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                                    objPullDetailDTO.UpdatedByName = item.UpdatedByName;
                                    objPullDetailDTO.ItemLocationDetailGUID = item.GUID;
                                    objPullDetailDTO.ProjectSpendGUID = item.ProjectSpentGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : item.ProjectSpentGUID;
                                    objPullDetailDTO.WorkOrderGUID = item.WorkOrderGUID;
                                    objPullDetailDTO.COuntLineItemGUID = item.CountLineItemDtlGUID;
                                    objPullDetailDTO.SupplierAccountGuid = item.SupplierAccountGuid;

                                    if (string.IsNullOrEmpty(item.AddedFrom) || string.IsNullOrWhiteSpace(item.AddedFrom))
                                        objPullDetailDTO.AddedFrom = "Web";
                                    else
                                        objPullDetailDTO.AddedFrom = item.AddedFrom;

                                    if (string.IsNullOrEmpty(item.EditedFrom) || string.IsNullOrWhiteSpace(item.EditedFrom))
                                        objPullDetailDTO.EditedFrom = "Web";
                                    else
                                        objPullDetailDTO.EditedFrom = item.EditedFrom;

                                    objPullDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    objPullDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                    objPullDetailDTO.PullOrderNumber = item.PullOrderNumber;

                                    //This PULLGUID set for apply PullGUID in Service WI-4305 
                                    objPullDetailDTO.NEW_PullGUID = item.PullGUID;
                                    objPullDetailDTO.GUID = Guid.NewGuid();

                                    objPullDetailsDTOs.Add(objPullDetailDTO);

                                    CreditHistory objCreditHistory = new CreditHistory();
                                    objCreditHistory.PULLGUID = item.PullGUIDForCreditHistory ?? Guid.Empty;
                                    objCreditHistory.PullDetailGuid = item.PullDetailGUIDForCreditHistory ?? Guid.Empty;

                                    objCreditHistory.CreditDetailGuid = objPullDetailDTO.GUID;

                                    lstCreditGuids.Add(objCreditHistory);

                                    #endregion
                                }
                            }
                        }
                    }
                    #endregion
                }

                #region "PULL Master and Detail Save"

                PullMasterDAL objPullDAL = new PullMasterDAL(base.DataBaseName);
                PullMasterViewDTO objPullDTO = new PullMasterViewDTO();
                if (objPullDetailsDTOs.Count > 0)
                {
                    objPullDTO.ActionType = PullAction;
                    objPullDTO.BinID = objPullDetailsDTOs[0].BinID;
                    objPullDTO.CompanyID = objPullDetailsDTOs[0].CompanyID;
                    objPullDTO.ConsignedQuantity = objPullDetailsDTOs.Select(x => x.ConsignedQuantity.GetValueOrDefault(0)).Sum();
                    objPullDTO.CustomerOwnedQuantity = objPullDetailsDTOs.Select(x => x.CustomerOwnedQuantity.GetValueOrDefault(0)).Sum();
                    objPullDTO.Created = DateTimeUtility.DateTimeNow;
                    objPullDTO.CreatedBy = objPullDetailsDTOs[0].CreatedBy.GetValueOrDefault(0);
                    objPullDTO.CreatedByName = objPullDetailsDTOs[0].CreatedByName;

                    /*This PULLGUID related change for apply PullGUID in Service WI-4305 */

                    if (objPullDetailsDTOs[0].NEW_PullGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                    {
                        objPullDTO.GUID = objPullDetailsDTOs[0].NEW_PullGUID.GetValueOrDefault(Guid.Empty);
                    }
                    else
                    {
                        objPullDTO.GUID = Guid.NewGuid();
                    }

                    lstCreditGuids.ForEach(b => b.CreditGuid = objPullDTO.GUID);

                    objPullDTO.IsArchived = false;
                    objPullDTO.IsDeleted = false;
                    objPullDTO.ItemGUID = objItemDTO.GUID;
                    objPullDTO.ItemNumber = objItemDTO.ItemNumber;
                    objPullDTO.ItemType = objItemDTO.ItemType;
                    objPullDTO.LastUpdatedBy = objPullDetailsDTOs[0].LastUpdatedBy.GetValueOrDefault(0);
                    objPullDTO.PoolQuantity = objPullDetailsDTOs.Select(x => x.PoolQuantity.GetValueOrDefault(0)).Sum();
                    objPullDTO.PullCredit = PullAction;
                    objPullDTO.PullCost = Convert.ToDouble(objPullDetailsDTOs.Select(x => x.ItemCost.GetValueOrDefault(0) * x.PoolQuantity.GetValueOrDefault(0)).Sum());
                    objPullDTO.PullPrice = Convert.ToDouble(objPullDetailsDTOs.Select(x => x.ItemPrice.GetValueOrDefault(0) * x.PoolQuantity.GetValueOrDefault(0)).Sum());
                    objPullDTO.Room = objPullDetailsDTOs[0].Room;
                    objPullDTO.RoomName = objPullDetailsDTOs[0].RoomName;
                    objPullDTO.Updated = DateTimeUtility.DateTimeNow;
                    objPullDTO.UpdatedByName = objPullDetailsDTOs[0].UpdatedByName;
                    objPullDTO.ProjectSpendGUID = objPullDetailsDTOs[0].ProjectSpendGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : objPullDetailsDTOs[0].ProjectSpendGUID;
                    objPullDTO.WorkOrderDetailGUID = objPullDetailsDTOs[0].WorkOrderGUID;
                    objPullDTO.WorkOrderGUID = objPullDetailsDTOs[0].WorkOrderGUID;
                    objPullDTO.CountLineItemGuid = objPullDetailsDTOs[0].COuntLineItemGUID;
                    objPullDTO.SupplierAccountGuid = objPullDetailsDTOs[0].SupplierAccountGuid;
                    objPullDTO.WhatWhereAction = WhatWhereAction;


                    if (objData != null && objData.Count > 0)
                    {
                        objPullDTO.UDF1 = objData[0].UDF1 != null ? objData[0].UDF1.Trim() : objData[0].UDF1;
                        objPullDTO.UDF2 = objData[0].UDF2 != null ? objData[0].UDF2.Trim() : objData[0].UDF2;
                        objPullDTO.UDF3 = objData[0].UDF3 != null ? objData[0].UDF3.Trim() : objData[0].UDF3;
                        objPullDTO.UDF4 = objData[0].UDF4 != null ? objData[0].UDF4.Trim() : objData[0].UDF4;
                        objPullDTO.UDF5 = objData[0].UDF5 != null ? objData[0].UDF5.Trim() : objData[0].UDF5;
                    }

                    if (string.IsNullOrEmpty(objPullDetailsDTOs[0].AddedFrom) || string.IsNullOrWhiteSpace(objPullDetailsDTOs[0].AddedFrom))
                        objPullDTO.AddedFrom = "Web";
                    else
                        objPullDTO.AddedFrom = objPullDetailsDTOs[0].AddedFrom;

                    if (string.IsNullOrEmpty(objPullDetailsDTOs[0].EditedFrom) || string.IsNullOrWhiteSpace(objPullDetailsDTOs[0].EditedFrom))
                        objPullDTO.EditedFrom = "Web";
                    else
                        objPullDTO.EditedFrom = objPullDetailsDTOs[0].EditedFrom;

                    objPullDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objPullDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objPullDTO.PullOrderNumber = objPullDetailsDTOs[0].PullOrderNumber;
                    if (objPullDTO.ActionType != "adjcredit" && objItemDTO.Consignment)
                    {
                        if (string.IsNullOrWhiteSpace(objPullDTO.PullOrderNumber))
                        {
                            AutoOrderNumberGenerate objAutoOrderNumberGenerate = new AutoSequenceDAL(base.DataBaseName).GetNextPullOrderNumber(objPullDTO.Room ?? 0, objPullDTO.CompanyID ?? 0, objItemDTO.SupplierID ?? 0, objItemDTO.GUID, objItemDTO.EnterpriseId, null, false);
                            if (objAutoOrderNumberGenerate != null && !string.IsNullOrWhiteSpace(objAutoOrderNumberGenerate.OrderNumber))
                            {
                                objPullDTO.PullOrderNumber = objAutoOrderNumberGenerate.OrderNumber;
                            }
                            else
                            {
                                DateTime datetimetoConsider = new RegionSettingDAL(base.DataBaseName).GetCurrentDatetimebyTimeZone(objPullDTO.Room ?? 0, objPullDTO.CompanyID ?? 0, 0);
                                objPullDTO.PullOrderNumber = datetimetoConsider.ToString("yyyyMMdd");
                            }

                        }
                    }

                    objPullDTO.ID = objPullDAL.Insert(objPullDTO);
                    PullDetailsDAL objPullDetailsDAL = new PullDetailsDAL(base.DataBaseName);
                    foreach (PullDetailsDTO temppdDTO in objPullDetailsDTOs)
                    {
                        temppdDTO.PULLGUID = objPullDTO.GUID;

                        if (string.IsNullOrEmpty(temppdDTO.AddedFrom) || string.IsNullOrWhiteSpace(temppdDTO.AddedFrom))
                            temppdDTO.AddedFrom = "Web";

                        if (string.IsNullOrEmpty(temppdDTO.EditedFrom) || string.IsNullOrWhiteSpace(temppdDTO.EditedFrom))
                            temppdDTO.EditedFrom = "Web";


                        temppdDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        temppdDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objPullDetailsDAL.Insert(temppdDTO);


                        //Project Spend update
                        if (temppdDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                        {
                            ProjectMasterDAL objPrjMsgDAL = new ProjectMasterDAL(base.DataBaseName);
                            ProjectSpendItemsDAL objPrjSpenItmDAL = new ProjectSpendItemsDAL(base.DataBaseName);
                            ProjectMasterDTO objPrjMstDTO = new ProjectMasterDTO();
                            ProjectSpendItemsDTO objPrjSpenItmDTO = new ProjectSpendItemsDTO();
                            var tmpsupplierIds = new List<long>();
                            //objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecords(objPullDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)objPullDetailsDTOs[0].Room, (Int64)objPullDetailsDTOs[0].CompanyID, tmpsupplierIds).Where(x => x.ItemGUID == objPullDTO.ItemGUID && x.Created.Value <= temppdDTO.Created.Value).SingleOrDefault();
                            objPrjSpenItmDTO = objPrjSpenItmDAL.GetProjectSpendItemCompareDate(objPullDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)objPullDetailsDTOs[0].Room, (Int64)objPullDetailsDTOs[0].CompanyID, tmpsupplierIds, Convert.ToString(objPullDTO.ItemGUID), temppdDTO.Created.Value.ToString("yyyy-MM-dd HH:mm:ss")).FirstOrDefault();
                            objPrjMstDTO = objPrjMsgDAL.GetRecord(objPullDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)objPullDetailsDTOs[0].Room, (Int64)objPullDetailsDTOs[0].CompanyID, false, false);

                            if (objPrjMstDTO != null)
                            {

                                objPrjMstDTO.DollarUsedAmount = objPrjMstDTO.DollarUsedAmount.GetValueOrDefault(0) - ((decimal)temppdDTO.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(temppdDTO.ItemCost.GetValueOrDefault(0)));
                                objPrjMstDTO.WhatWhereAction = WhatWhereAction;
                                //objPrjMstDTO.EditedFrom = "Web";
                                //objPrjMstDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objPrjMsgDAL.Edit(objPrjMstDTO);

                                if (objPrjSpenItmDTO != null)
                                {

                                    objPrjSpenItmDTO.DollarUsedAmount = objPrjSpenItmDTO.DollarUsedAmount.GetValueOrDefault(0) - ((decimal)temppdDTO.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(temppdDTO.ItemCost.GetValueOrDefault(0)));
                                    objPrjSpenItmDTO.QuantityUsed = objPrjSpenItmDTO.QuantityUsed.GetValueOrDefault(0) - temppdDTO.PoolQuantity.GetValueOrDefault(0);
                                    //objPrjSpenItmDTO.EditedFrom = "Web";
                                    //objPrjSpenItmDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    objPrjSpenItmDAL.Edit(objPrjSpenItmDTO);
                                }
                            }
                        }

                    }
                }
                #endregion
                if (!IsBothQty)
                {
                    // if (objItemDTO != null && objItemDTO.ItemType != 4)
                    if (objItemDTO != null)
                    {
                        #region "Insert QTY logic"
                        if (IsQTYSave)
                        {
                            if (TempSerialNumberTracking)
                            {
                                #region "Insert mode for Serial Number"
                                ItemLocationQTYDAL objQtyMgt = new ItemLocationQTYDAL(base.DataBaseName);

                                //FOR INSERT MODE
                                var objilqInsert = new List<ItemLocationQTYDTO>();

                                objilqInsert = (from x in objData
                                                where (x.SerialNumberTracking == true && x.SerialNumber != null) && x.BinID != null
                                                group x by new { x.BinID }
                                                    into grp
                                                select new ItemLocationQTYDTO
                                                {
                                                    BinID = Convert.ToInt64(grp.Key.BinID),
                                                    ItemGUID = objData[0].ItemGUID,
                                                    CompanyID = objData[0].CompanyID,
                                                    Room = objData[0].Room,
                                                    LastUpdated = DateTime.Now,
                                                    Created = DateTime.Now,
                                                    CreatedBy = objData[0].CreatedBy,
                                                    LastUpdatedBy = objData[0].LastUpdatedBy,
                                                    CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                                    ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                                    Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                                }).ToList();


                                //objilqInsert.Select(c => 
                                //{
                                //    if (objItemDTO.Consignment)
                                //    {
                                //        c.ConsignedQuantity= 1;
                                //    }
                                //    else
                                //    {
                                //        c.CustomerOwnedQuantity = 1;
                                //    }
                                //    return c; 
                                //}).ToList();



                                if (objilqInsert.Count() > 0)
                                {
                                    objQtyMgt.Save(objilqInsert, "Credit", SessionUserId, EnterpriseId);
                                }
                                #endregion
                            }
                            else if (TempLotNumberTracking)
                            {
                                #region "Insert Mode for Lot#, Others"
                                ItemLocationQTYDAL objQtyMgt = new ItemLocationQTYDAL(base.DataBaseName);
                                if (objItemDTO.Consignment)
                                {
                                    var objilq = (from x in objData
                                                  where (x.LotNumberTracking == true && x.LotNumber != null) && x.BinID != null
                                                  group x by new { x.BinID } into grp
                                                  select new ItemLocationQTYDTO
                                                  {
                                                      BinID = Convert.ToInt64(grp.Key.BinID),
                                                      ItemGUID = objData[0].ItemGUID,
                                                      CompanyID = objData[0].CompanyID,
                                                      Room = objData[0].Room,
                                                      LastUpdated = DateTime.Now,
                                                      Created = DateTime.Now,
                                                      CreatedBy = objData[0].CreatedBy,
                                                      LastUpdatedBy = objData[0].LastUpdatedBy,
                                                      CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                                      ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                                      Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                                      //LotNumber = x.LotNumber,
                                                  }).ToList();
                                    //Save
                                    objQtyMgt.Save(objilq, "Credit", SessionUserId, EnterpriseId);
                                    //Update on_hand qunatity
                                    //objilq.Select(c => { OnHandQuantity += c.Quantity; return c; }).ToList();
                                }
                                else
                                {
                                    var objilq = (from x in objData
                                                  where (x.LotNumberTracking == true && x.LotNumber != null) && x.BinID != null
                                                  group x by new { x.BinID } into grp
                                                  select new ItemLocationQTYDTO
                                                  {
                                                      BinID = Convert.ToInt64(grp.Key.BinID),
                                                      ItemGUID = objData[0].ItemGUID,
                                                      CompanyID = objData[0].CompanyID,
                                                      Room = objData[0].Room,
                                                      LastUpdated = DateTime.Now,
                                                      Created = DateTime.Now,
                                                      CreatedBy = objData[0].CreatedBy,
                                                      LastUpdatedBy = objData[0].LastUpdatedBy,
                                                      CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                                      ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                                      Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                                      //LotNumber = x.LotNumber,
                                                  }).ToList();

                                    //Save
                                    objQtyMgt.Save(objilq, "Credit", SessionUserId, EnterpriseId);
                                }
                                //Save on hand quantity
                                #endregion
                            }
                            else
                            {
                                #region "Insert Mode for Lot#, Others"
                                ItemLocationQTYDAL objQtyMgt = new ItemLocationQTYDAL(base.DataBaseName);
                                if (objItemDTO.Consignment)
                                {
                                    var objilq = (from x in objData
                                                  where x.BinID != null
                                                  group x by new { x.BinID } into grp
                                                  select new ItemLocationQTYDTO
                                                  {
                                                      BinID = Convert.ToInt64(grp.Key.BinID),
                                                      ItemGUID = objData[0].ItemGUID,
                                                      CompanyID = objData[0].CompanyID,
                                                      Room = objData[0].Room,
                                                      LastUpdated = DateTime.Now,
                                                      Created = DateTime.Now,
                                                      CreatedBy = objData[0].CreatedBy,
                                                      LastUpdatedBy = objData[0].LastUpdatedBy,
                                                      CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                                      ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                                      Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                                      //LotNumber = x.LotNumber,
                                                  }).ToList();
                                    //Save
                                    objQtyMgt.Save(objilq, "Credit", SessionUserId, EnterpriseId);
                                    //Update on_hand qunatity
                                    //objilq.Select(c => { OnHandQuantity += c.Quantity; return c; }).ToList();
                                }
                                else
                                {
                                    var objilq = (from x in objData
                                                  where x.BinID != null
                                                  group x by new { x.BinID } into grp
                                                  select new ItemLocationQTYDTO
                                                  {
                                                      BinID = Convert.ToInt64(grp.Key.BinID),
                                                      ItemGUID = objData[0].ItemGUID,
                                                      CompanyID = objData[0].CompanyID,
                                                      Room = objData[0].Room,
                                                      LastUpdated = DateTime.Now,
                                                      Created = DateTime.Now,
                                                      CreatedBy = objData[0].CreatedBy,
                                                      LastUpdatedBy = objData[0].LastUpdatedBy,
                                                      CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                                      ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                                      Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                                      //LotNumber = x.LotNumber,
                                                  }).ToList();

                                    //Save
                                    objQtyMgt.Save(objilq, "Credit", SessionUserId, EnterpriseId);
                                }
                                //Save on hand quantity
                                #endregion
                            }
                        }
                        #endregion
                    }

                    objPullDTO = new PullMasterDAL(base.DataBaseName).UpdateCumulativeOnHand(objPullDTO);
                }
            }
            return true;
        }

        public void SaveBothCustAndConsigenQtyForCount(ItemLocationDetailsDTO item, ItemMasterDTO objItemDTO, BinMasterDTO objBinDTO, bool IsValid, string PullAction, List<ItemLocationDetailsDTO> objData,
            long SessionUserId, long EnterpriseId)
        {
            bool TempLotNumberTracking = false;
            bool TempSerialNumberTracking = false;
            bool IsQTYSave = false;
            ItemLocationDetailsDAL objItemDetailDAL = new ItemLocationDetailsDAL(base.DataBaseName);
            ItemLocationDetailsDTO objItem = new ItemLocationDetailsDTO();
            List<PullDetailsDTO> objPullDetailsDTOs = new List<PullDetailsDTO>();

            objItem.BinID = item.BinID;
            objItem.CompanyID = item.CompanyID;
            objItem.Cost = item.Cost;
            objItem.Created = item.Created;
            objItem.CustomerOwnedQuantity = item.CustomerOwnedQuantity;
            objItem.ConsignedQuantity = item.ConsignedQuantity;
            objItem.InitialQuantity = (item.ConsignedQuantity ?? 0) + (item.CustomerOwnedQuantity ?? 0);
            objItem.InitialQuantityWeb = (item.ConsignedQuantity ?? 0) + (item.CustomerOwnedQuantity ?? 0);
            if (item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                objItem.IsConsignedSerialLot = true;
            else
                objItem.IsConsignedSerialLot = false;

            //objItem.IsConsignedSerialLot = objItemDTO.Consignment;
            objItem.ExpirationDate = item.ExpirationDate;
            objItem.Expiration = item.Expiration;
            objItem.Received = item.Received;
            objItem.ReceivedDate = item.ReceivedDate;
            if (objItem.ReceivedDate.GetValueOrDefault(DateTime.MinValue) <= DateTime.MinValue && !string.IsNullOrEmpty(objItem.Received))
            {
                objItem.ReceivedDate = DateTime.Parse(objItem.Received);
            }
            if (objItem.ExpirationDate.GetValueOrDefault(DateTime.MinValue) <= DateTime.MinValue && !string.IsNullOrEmpty(objItem.Expiration))
            {
                objItem.ExpirationDate = DateTime.Parse(objItem.Expiration);
            }
            objItem.ID = item.ID; objItem.ItemGUID = item.ItemGUID;

            objItem.LotNumber = item.LotNumber;
            if (item.LotNumber != null && item.LotNumberTracking)
                TempLotNumberTracking = true;
            objItem.SerialNumber = item.SerialNumber;
            if (item.SerialNumber != null && item.SerialNumberTracking)
                TempSerialNumberTracking = true;

            objItem.CreatedBy = item.CreatedBy; objItem.CompanyID = item.CompanyID;
            objItem.Room = item.Room; objItem.Updated = item.Updated;
            objItem.LastUpdatedBy = item.LastUpdatedBy;
            objItem.IsArchived = item.IsArchived; objItem.IsDeleted = item.IsDeleted;
            objItem.Created = item.Created; objItem.CreatedBy = item.CreatedBy;
            objItem.OrderDetailGUID = item.OrderDetailGUID;

            if (objBinDTO != null)
                objItem.BinNumber = objBinDTO.BinNumber;

            if (item.ID > 0 && IsValid)
            {
                objItem.GUID = item.GUID;
                objItem.IsOnlyFromUI = item.IsOnlyFromUI;
                if (item.IsOnlyFromUI)
                {
                    objItem.EditedFrom = "Web";
                    objItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                }
                objItemDetailDAL.Edit(objItem);
                IsQTYSave = true;
            }
            else
            {
                if (IsValid)
                {
                    bool IsRecordSave = false;
                    #region "SR Case"
                    if (TempSerialNumberTracking)
                    {
                        //var TempDupSrs = objItemDetailDAL.GetCachedData(item.ItemGUID.Value, item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0)).Where(x => x.SerialNumber == item.SerialNumber).ToList();
                        var TempDupSrs = objItemDetailDAL.GetItemLocationDetail(item.ItemGUID.Value, item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0), null, item.SerialNumber, string.Empty, string.Empty, null, null, string.Empty).ToList();
                        if (TempDupSrs != null && TempDupSrs.Count() > 0)
                        {
                            foreach (ItemLocationDetailsDTO IDTemp in TempDupSrs)
                            {
                                IDTemp.IsDeleted = true;
                                IDTemp.IsArchived = true;
                                IDTemp.LastUpdatedBy = item.LastUpdatedBy;
                                IDTemp.Updated = DateTimeUtility.DateTimeNow;
                                IDTemp.IsOnlyFromUI = true;
                                IDTemp.EditedFrom = "Web";
                                IDTemp.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objItemDetailDAL.Edit(IDTemp);
                            }
                        }
                    }
                    #endregion
                    if (!IsRecordSave)
                    {
                        objItem.AddedFrom = "Web";
                        objItem.EditedFrom = "Web";
                        objItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objItem.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objItem.IsOnlyFromUI = item.IsOnlyFromUI;
                        ItemLocationDetailsDTO itemloctemp = objItemDetailDAL.Insert(objItem);
                        item.ID = itemloctemp.ID;
                        item.GUID = itemloctemp.GUID;
                        IsQTYSave = true;
                    }
                    #region "PULL Details Entry"
                    PullDetailsDTO objPullDetailDTO = new PullDetailsDTO();
                    objPullDetailDTO.BinID = item.BinID;
                    objPullDetailDTO.CompanyID = item.CompanyID;
                    objPullDetailDTO.ConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0);
                    objPullDetailDTO.Created = DateTimeUtility.DateTimeNow;
                    objPullDetailDTO.CreatedBy = item.CreatedBy.GetValueOrDefault(0);
                    objPullDetailDTO.CreatedByName = item.CreatedByName;
                    objPullDetailDTO.CustomerOwnedQuantity = item.CustomerOwnedQuantity.GetValueOrDefault(0);
                    objPullDetailDTO.Expiration = item.Expiration;
                    objPullDetailDTO.IsArchived = item.IsArchived;
                    objPullDetailDTO.IsDeleted = item.IsDeleted;
                    objPullDetailDTO.ItemCost = CalculateAndGetPullCreditCost(objItem.Cost, objItemDTO.CostUOMID, item.Room ?? 0, item.CompanyID ?? 0);
                    objPullDetailDTO.ItemGUID = item.ItemGUID;
                    objPullDetailDTO.LastUpdatedBy = item.LastUpdatedBy.GetValueOrDefault(0);
                    objPullDetailDTO.LotNumber = item.LotNumber;
                    objPullDetailDTO.PoolQuantity = (item.ConsignedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0));
                    objPullDetailDTO.PullCredit = PullAction;
                    objPullDetailDTO.Received = item.Received;
                    objPullDetailDTO.Room = item.Room;
                    objPullDetailDTO.RoomName = item.RoomName;
                    objPullDetailDTO.SerialNumber = item.SerialNumber;
                    objPullDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                    objPullDetailDTO.UpdatedByName = item.UpdatedByName;
                    objPullDetailDTO.ItemLocationDetailGUID = item.GUID;
                    objPullDetailDTO.ProjectSpendGUID = item.ProjectSpentGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : item.ProjectSpentGUID;
                    objPullDetailDTO.WorkOrderGUID = item.WorkOrderGUID;
                    objPullDetailDTO.COuntLineItemGUID = item.CountLineItemDtlGUID;
                    objPullDetailDTO.AddedFrom = "Web";
                    objPullDetailDTO.EditedFrom = "Web";
                    objPullDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objPullDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                    objPullDetailsDTOs.Add(objPullDetailDTO);
                    #endregion
                }
            }

            #region "PULL Master and Detail Save"

            PullMasterDAL objPullDAL = new PullMasterDAL(base.DataBaseName);
            PullMasterViewDTO objPullDTO = new PullMasterViewDTO();
            if (objPullDetailsDTOs.Count > 0)
            {
                objPullDTO.ActionType = PullAction;
                objPullDTO.BinID = objPullDetailsDTOs[0].BinID;
                objPullDTO.CompanyID = objPullDetailsDTOs[0].CompanyID;
                objPullDTO.ConsignedQuantity = objPullDetailsDTOs.Select(x => x.ConsignedQuantity.GetValueOrDefault(0)).Sum();
                objPullDTO.CustomerOwnedQuantity = objPullDetailsDTOs.Select(x => x.CustomerOwnedQuantity.GetValueOrDefault(0)).Sum();
                objPullDTO.Created = DateTimeUtility.DateTimeNow;
                objPullDTO.CreatedBy = objPullDetailsDTOs[0].CreatedBy.GetValueOrDefault(0);
                objPullDTO.CreatedByName = objPullDetailsDTOs[0].CreatedByName;
                objPullDTO.GUID = Guid.NewGuid();
                objPullDTO.IsArchived = false;
                objPullDTO.IsDeleted = false;
                objPullDTO.ItemGUID = objItemDTO.GUID;
                objPullDTO.ItemNumber = objItemDTO.ItemNumber;
                objPullDTO.ItemType = objItemDTO.ItemType;
                objPullDTO.LastUpdatedBy = objPullDetailsDTOs[0].LastUpdatedBy.GetValueOrDefault(0);
                objPullDTO.PoolQuantity = objPullDetailsDTOs.Select(x => x.PoolQuantity.GetValueOrDefault(0)).Sum();
                objPullDTO.PullCredit = PullAction;
                objPullDTO.PullCost = Convert.ToDouble(objPullDetailsDTOs.Select(x => x.ItemCost.GetValueOrDefault(0) * x.PoolQuantity.GetValueOrDefault(0)).Sum());
                objPullDTO.Room = objPullDetailsDTOs[0].Room;
                objPullDTO.RoomName = objPullDetailsDTOs[0].RoomName;
                objPullDTO.Updated = DateTimeUtility.DateTimeNow;
                objPullDTO.UpdatedByName = objPullDetailsDTOs[0].UpdatedByName;
                objPullDTO.ProjectSpendGUID = objPullDetailsDTOs[0].ProjectSpendGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : objPullDetailsDTOs[0].ProjectSpendGUID;
                objPullDTO.WorkOrderDetailGUID = objPullDetailsDTOs[0].WorkOrderGUID;
                objPullDTO.WorkOrderGUID = objPullDetailsDTOs[0].WorkOrderGUID;
                objPullDTO.CountLineItemGuid = objPullDetailsDTOs[0].COuntLineItemGUID;
                objPullDTO.WhatWhereAction = PullAction;

                objPullDTO.AddedFrom = "Web";
                objPullDTO.EditedFrom = "Web";
                objPullDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objPullDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                objPullDTO.ID = objPullDAL.Insert(objPullDTO);
                PullDetailsDAL objPullDetailsDAL = new PullDetailsDAL(base.DataBaseName);
                foreach (PullDetailsDTO temppdDTO in objPullDetailsDTOs)
                {
                    temppdDTO.PULLGUID = objPullDTO.GUID;
                    temppdDTO.AddedFrom = "Web";
                    temppdDTO.EditedFrom = "Web";
                    temppdDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    temppdDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objPullDetailsDAL.Insert(temppdDTO);


                    //Project Spend update
                    if (temppdDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                    {
                        ProjectMasterDAL objPrjMsgDAL = new ProjectMasterDAL(base.DataBaseName);
                        ProjectSpendItemsDAL objPrjSpenItmDAL = new ProjectSpendItemsDAL(base.DataBaseName);
                        ProjectMasterDTO objPrjMstDTO = new ProjectMasterDTO();
                        ProjectSpendItemsDTO objPrjSpenItmDTO = new ProjectSpendItemsDTO();
                        var tmpsupplierIds = new List<long>();
                        //objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecords(objPullDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)objPullDetailsDTOs[0].Room, (Int64)objPullDetailsDTOs[0].CompanyID, tmpsupplierIds).Where(x => x.ItemGUID == objPullDTO.ItemGUID && x.Created.Value <= temppdDTO.Created.Value).SingleOrDefault();
                        objPrjSpenItmDTO = objPrjSpenItmDAL.GetProjectSpendItemCompareDate(objPullDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)objPullDetailsDTOs[0].Room, (Int64)objPullDetailsDTOs[0].CompanyID, tmpsupplierIds, Convert.ToString(objPullDTO.ItemGUID), temppdDTO.Created.Value.ToString("yyyy-MM-dd HH:mm:ss")).SingleOrDefault();
                        objPrjMstDTO = objPrjMsgDAL.GetRecord(objPullDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)objPullDetailsDTOs[0].Room, (Int64)objPullDetailsDTOs[0].CompanyID, false, false);

                        if (objPrjMstDTO != null)
                        {

                            objPrjMstDTO.DollarUsedAmount = objPrjMstDTO.DollarUsedAmount.GetValueOrDefault(0) - ((decimal)temppdDTO.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(temppdDTO.ItemCost.GetValueOrDefault(0)));
                            objPrjMstDTO.WhatWhereAction = PullAction;
                            //objPrjMstDTO.EditedFrom = "Web";
                            //objPrjMstDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objPrjMsgDAL.Edit(objPrjMstDTO);

                            if (objPrjSpenItmDTO != null)
                            {

                                objPrjSpenItmDTO.DollarUsedAmount = objPrjSpenItmDTO.DollarUsedAmount.GetValueOrDefault(0) - ((decimal)temppdDTO.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(temppdDTO.ItemCost.GetValueOrDefault(0)));
                                objPrjSpenItmDTO.QuantityUsed = objPrjSpenItmDTO.QuantityUsed.GetValueOrDefault(0) - temppdDTO.PoolQuantity.GetValueOrDefault(0);
                                //objPrjSpenItmDTO.EditedFrom = "Web";
                                //objPrjSpenItmDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objPrjSpenItmDAL.Edit(objPrjSpenItmDTO);
                            }
                        }
                    }

                }




            }
            #endregion

            if (objItemDTO != null && objItemDTO.ItemType != 4)
            {
                #region "Insert QTY logic"
                if (IsQTYSave)
                {
                    if (TempSerialNumberTracking)
                    {
                        #region "Insert mode for Serial Number"
                        ItemLocationQTYDAL objQtyMgt = new ItemLocationQTYDAL(base.DataBaseName);

                        //FOR INSERT MODE
                        var objilqInsert = new List<ItemLocationQTYDTO>();

                        objilqInsert = (from x in objData
                                        where (x.SerialNumberTracking == true && x.SerialNumber != null) && x.BinID != null
                                        group x by new { x.BinID }
                                            into grp
                                        select new ItemLocationQTYDTO
                                        {
                                            BinID = Convert.ToInt64(grp.Key.BinID),
                                            ItemGUID = objData[0].ItemGUID,
                                            CompanyID = objData[0].CompanyID,
                                            Room = objData[0].Room,
                                            LastUpdated = DateTime.Now,
                                            Created = DateTime.Now,
                                            CreatedBy = objData[0].CreatedBy,
                                            LastUpdatedBy = objData[0].LastUpdatedBy,
                                            CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                            ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                            Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                        }).ToList();


                        //objilqInsert.Select(c => 
                        //{
                        //    if (objItemDTO.Consignment)
                        //    {
                        //        c.ConsignedQuantity= 1;
                        //    }
                        //    else
                        //    {
                        //        c.CustomerOwnedQuantity = 1;
                        //    }
                        //    return c; 
                        //}).ToList();



                        if (objilqInsert.Count() > 0)
                        {
                            objQtyMgt.Save(objilqInsert, "Credit", SessionUserId, EnterpriseId);
                        }
                        #endregion
                    }
                    else if (TempLotNumberTracking)
                    {
                        #region "Insert Mode for Lot#, Others"
                        ItemLocationQTYDAL objQtyMgt = new ItemLocationQTYDAL(base.DataBaseName);
                        //if (objItemDTO.Consignment)
                        if (item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                        {
                            var objilq = (from x in objData
                                          where (x.LotNumberTracking == true && x.LotNumber != null) && x.BinID != null
                                          group x by new { x.BinID } into grp
                                          select new ItemLocationQTYDTO
                                          {
                                              BinID = Convert.ToInt64(grp.Key.BinID),
                                              ItemGUID = objData[0].ItemGUID,
                                              CompanyID = objData[0].CompanyID,
                                              Room = objData[0].Room,
                                              LastUpdated = DateTime.Now,
                                              Created = DateTime.Now,
                                              CreatedBy = objData[0].CreatedBy,
                                              LastUpdatedBy = objData[0].LastUpdatedBy,
                                              CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                              ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                              Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                              //LotNumber = x.LotNumber,
                                          }).ToList();
                            //Save
                            objQtyMgt.Save(objilq, "Credit", SessionUserId, EnterpriseId);
                            //Update on_hand qunatity
                            //objilq.Select(c => { OnHandQuantity += c.Quantity; return c; }).ToList();
                        }
                        else
                        {
                            var objilq = (from x in objData
                                          where (x.LotNumberTracking == true && x.LotNumber != null) && x.BinID != null
                                          group x by new { x.BinID } into grp
                                          select new ItemLocationQTYDTO
                                          {
                                              BinID = Convert.ToInt64(grp.Key.BinID),
                                              ItemGUID = objData[0].ItemGUID,
                                              CompanyID = objData[0].CompanyID,
                                              Room = objData[0].Room,
                                              LastUpdated = DateTime.Now,
                                              Created = DateTime.Now,
                                              CreatedBy = objData[0].CreatedBy,
                                              LastUpdatedBy = objData[0].LastUpdatedBy,
                                              CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                              ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                              Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                              //LotNumber = x.LotNumber,
                                          }).ToList();

                            //Save
                            objQtyMgt.Save(objilq, "Credit", SessionUserId, EnterpriseId);
                        }
                        //Save on hand quantity
                        #endregion
                    }
                    else
                    {
                        #region "Insert Mode for Lot#, Others"
                        ItemLocationQTYDAL objQtyMgt = new ItemLocationQTYDAL(base.DataBaseName);
                        //if (objItemDTO.Consignment)
                        if (item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                        {
                            var objilq = (from x in objData
                                          where x.BinID != null
                                          group x by new { x.BinID } into grp
                                          select new ItemLocationQTYDTO
                                          {
                                              BinID = Convert.ToInt64(grp.Key.BinID),
                                              ItemGUID = objData[0].ItemGUID,
                                              CompanyID = objData[0].CompanyID,
                                              Room = objData[0].Room,
                                              LastUpdated = DateTime.Now,
                                              Created = DateTime.Now,
                                              CreatedBy = objData[0].CreatedBy,
                                              LastUpdatedBy = objData[0].LastUpdatedBy,
                                              CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                              ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                              Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                              //LotNumber = x.LotNumber,
                                          }).ToList();
                            //Save
                            objQtyMgt.Save(objilq, "Credit", SessionUserId, EnterpriseId);
                            //Update on_hand qunatity
                            //objilq.Select(c => { OnHandQuantity += c.Quantity; return c; }).ToList();
                        }
                        else
                        {
                            var objilq = (from x in objData
                                          where x.BinID != null
                                          group x by new { x.BinID } into grp
                                          select new ItemLocationQTYDTO
                                          {
                                              BinID = Convert.ToInt64(grp.Key.BinID),
                                              ItemGUID = objData[0].ItemGUID,
                                              CompanyID = objData[0].CompanyID,
                                              Room = objData[0].Room,
                                              LastUpdated = DateTime.Now,
                                              Created = DateTime.Now,
                                              CreatedBy = objData[0].CreatedBy,
                                              LastUpdatedBy = objData[0].LastUpdatedBy,
                                              CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                              ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                              Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                              //LotNumber = x.LotNumber,
                                          }).ToList();

                            //Save
                            objQtyMgt.Save(objilq, "Credit", SessionUserId, EnterpriseId);
                        }
                        //Save on hand quantity
                        #endregion
                    }
                }
                #endregion
            }

            objPullDTO = new PullMasterDAL(base.DataBaseName).UpdateCumulativeOnHandNew(objPullDTO);

        }

        public bool ItemLocationDetailsSaveForCreditPull(List<ItemLocationDetailsDTO> objData, string calledfrom, long SessionUserId, string PullAction = "credit")
        {
            if (!string.IsNullOrWhiteSpace(calledfrom))
            {
                if ("evmisvc" == calledfrom)
                {
                    PullAction = "cr_evmi";
                }
            }

            bool TempSerialNumberTracking = false;
            bool TempLotNumberTracking = false;
            //bool Consignment = false;
            ItemMasterDTO objItemDTO = null;
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            bool IsQTYSave = false;

            string CountdtlGUID = string.Empty;
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                int LooptotalCount = objData.Count();
                int Counter = 0;
                //Added By Esha on 21/03/2013 To Add New Location To LocationMaster Table

                foreach (ItemLocationDetailsDTO item in objData)
                {
                    BinMasterDTO Itembin = GetItemBin(item.ItemGUID ?? Guid.Empty, item.BinNumber, item.Room ?? 0, item.CompanyID ?? 0, item.CreatedBy ?? 0, false);
                    item.BinID = Itembin.ID;

                    //if (item.BinID == null)
                    //{
                    //    if (item.BinNumber != null)
                    //    {
                    //        BinMasterDAL objBin = new BinMasterDAL(base.DataBaseName);
                    //        BinMasterDTO objBinDTO = objBin.GetRecord(item.BinNumber, Convert.ToInt64(item.Room), Convert.ToInt64(item.CompanyID), false, false);
                    //        if (objBinDTO.IsStagingLocation == true)
                    //            objBinDTO = null;

                    //        if (objBinDTO == null)
                    //        {
                    //            Guid g = Guid.NewGuid();
                    //            BinMaster objbin = new BinMaster();
                    //            BinMasterDTO objbinDTO = new BinMasterDTO();
                    //            objbin.ID = 0;
                    //            objbin.BinNumber = item.BinNumber;
                    //            objbin.CreatedBy = item.CreatedBy;
                    //            objbin.LastUpdatedBy = item.LastUpdatedBy;
                    //            objbin.Room = item.Room;
                    //            objbin.LastUpdated = item.Updated;
                    //            objbin.Created = item.Created;
                    //            objbin.IsDeleted = false;
                    //            objbin.IsArchived = false;
                    //            objbin.CompanyID = item.CompanyID;
                    //            objbin.IsStagingLocation = false;
                    //            objbin.IsStagingHeader = false;
                    //            objbin.MaterialStagingGUID = null;
                    //            objbin.GUID = g;
                    //            objbin.UDF1 = null;
                    //            objbin.UDF2 = null;
                    //            objbin.UDF3 = null;
                    //            objbin.UDF4 = null;
                    //            objbin.UDF5 = null;
                    //            context.BinMasters(objbin);
                    //            context.SaveChanges();

                    //            objbinDTO.ID = objbin.ID;
                    //            objbinDTO.BinNumber = item.BinNumber;
                    //            objbinDTO.CreatedBy = item.CreatedBy;
                    //            objbinDTO.LastUpdatedBy = item.LastUpdatedBy;
                    //            objbinDTO.Room = item.Room;
                    //            objbinDTO.LastUpdated = item.Updated;
                    //            objbinDTO.Created = item.Created;
                    //            objbinDTO.IsDeleted = false;
                    //            objbinDTO.IsArchived = false;
                    //            objbinDTO.CompanyID = item.CompanyID;
                    //            objbinDTO.IsStagingLocation = false;
                    //            objbinDTO.IsStagingHeader = false;
                    //            objbinDTO.MaterialStagingGUID = null;
                    //            objbinDTO.GUID = g;
                    //            objbinDTO.UDF1 = null;
                    //            objbinDTO.UDF2 = null;
                    //            objbinDTO.UDF3 = null;
                    //            objbinDTO.UDF4 = null;
                    //            objbinDTO.UDF5 = null;

                    //            if (objbin.ID > 0)
                    //            {
                    //                //Get Cached-Media
                    //                IEnumerable<BinMasterDTO> ObjCache = CacheHelper<IEnumerable<BinMasterDTO>>.GetCacheItem("Cached_BinMaster_" + objbinDTO.CompanyID.ToString());
                    //                if (ObjCache != null)
                    //                {
                    //                    List<BinMasterDTO> tempC = new List<BinMasterDTO>();
                    //                    tempC.Add(objbinDTO);

                    //                    IEnumerable<BinMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    //                    CacheHelper<IEnumerable<BinMasterDTO>>.AppendToCacheItem("Cached_BinMaster_" + objbinDTO.CompanyID.ToString(), NewCache);
                    //                }
                    //            }

                    //            item.BinID = objbin.ID;
                    //        }
                    //        else
                    //        {
                    //            item.BinID = objBinDTO.ID;
                    //        }
                    //    }
                    //}
                }


                //End By Esha

                List<PullDetailsDTO> objPullDetailsDTOs = new List<PullDetailsDTO>();
                foreach (ItemLocationDetailsDTO item in objData)
                {

                    #region "Location Detail Save"
                    Counter++;

                    if (item.BinID == null)
                    {
                        continue;
                    }
                    if (objItemDTO == null)
                    {
                        objItemDTO = objItemDAL.GetItemWithoutJoins(null, item.ItemGUID);
                    }

                    item.IsArchived = (item.IsArchived == null ? false : item.IsArchived);
                    item.IsDeleted = (item.IsDeleted == null ? false : item.IsDeleted);
                    BinMasterDAL objBin = new BinMasterDAL(base.DataBaseName);
                    BinMasterDTO objBinDTO = objBin.GetBinByID(Convert.ToInt64(item.BinID), Convert.ToInt64(item.Room), Convert.ToInt64(item.CompanyID));
                    // BinMasterDTO objBinDTO = objBin.GetItemLocation( Convert.ToInt64(item.Room), Convert.ToInt64(item.CompanyID), false, false,Guid.Empty, Convert.ToInt64(item.BinID),null,null).FirstOrDefault();

                    if (string.IsNullOrEmpty(item.Received) && item.ReceivedDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                        item.Received = item.ReceivedDate.Value.ToString("M/d/yyyy");

                    bool IsValid = false;

                    if (!string.IsNullOrEmpty(item.Received) || item.ReceivedDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                        IsValid = true;
                    else if (!string.IsNullOrEmpty(item.SerialNumber))
                    {
                        IsValid = true;
                    }
                    else if (!string.IsNullOrEmpty(item.LotNumber))
                        IsValid = true;
                    else if (!string.IsNullOrEmpty(item.Expiration) || (item.ExpirationDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue))
                        IsValid = true;
                    else if (item.LotNumberTracking && item.SerialNumberTracking)
                        IsValid = true;
                    else if (item.CustomerOwnedQuantity.HasValue && item.BinID.HasValue && item.Cost.HasValue && (!string.IsNullOrEmpty(item.Received) || item.ReceivedDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue))
                        IsValid = true;

                    if (IsValid || (LooptotalCount == Counter))
                    {
                        //if (!String.IsNullOrEmpty(item.SerialNumber) || !String.IsNullOrEmpty(item.LotNumber) || item.CustomerOwnedQuantity.Value > 0 || item.ConsignedQuantity.Value > 0)
                        //{
                        ItemLocationDetailsDAL objItemDetailDAL = new ItemLocationDetailsDAL(base.DataBaseName);
                        ItemLocationDetailsDTO objItem = new ItemLocationDetailsDTO();
                        objItem.BinID = item.BinID;

                        objItem.CompanyID = item.CompanyID;
                        objItem.Cost = item.Cost;
                        objItem.Created = item.Created;

                        if (objItemDTO.Consignment)
                        {
                            //Consignment = true;
                        }

                        objItem.CustomerOwnedQuantity = item.CustomerOwnedQuantity;
                        objItem.ConsignedQuantity = item.ConsignedQuantity;
                        objItem.InitialQuantity = (item.ConsignedQuantity ?? 0) + (item.CustomerOwnedQuantity ?? 0);
                        objItem.InitialQuantityWeb = (item.ConsignedQuantity ?? 0) + (item.CustomerOwnedQuantity ?? 0);
                        objItem.IsConsignedSerialLot = objItemDTO.Consignment;
                        objItem.ExpirationDate = item.ExpirationDate;

                        objItem.Expiration = item.Expiration;
                        objItem.Received = item.Received;
                        objItem.ReceivedDate = item.ReceivedDate;
                        if (objItem.ReceivedDate.GetValueOrDefault(DateTime.MinValue) <= DateTime.MinValue && !string.IsNullOrEmpty(objItem.Received))
                        {
                            objItem.ReceivedDate = DateTime.Parse(objItem.Received);
                        }
                        if (objItem.ExpirationDate.GetValueOrDefault(DateTime.MinValue) <= DateTime.MinValue && !string.IsNullOrEmpty(objItem.Expiration))
                        {
                            objItem.ExpirationDate = DateTime.Parse(objItem.Expiration);
                        }
                        objItem.ID = item.ID; objItem.ItemGUID = item.ItemGUID;

                        //objItem.LotNumber = item.LotNumber;
                        //if (item.LotNumber != null)
                        //    TempLotNumberTracking = true;
                        //objItem.SerialNumber = item.SerialNumber;
                        //if (item.SerialNumber != null)
                        //    TempSerialNumberTracking = true;

                        objItem.LotNumber = item.LotNumber;
                        if (item.LotNumber != null && item.LotNumberTracking)
                            TempLotNumberTracking = true;
                        objItem.SerialNumber = item.SerialNumber;
                        if (item.SerialNumber != null && item.SerialNumberTracking)
                            TempSerialNumberTracking = true;

                        objItem.CreatedBy = item.CreatedBy; objItem.CompanyID = item.CompanyID;
                        objItem.Room = item.Room; objItem.Updated = item.Updated;
                        objItem.LastUpdatedBy = item.LastUpdatedBy;
                        objItem.IsArchived = item.IsArchived; objItem.IsDeleted = item.IsDeleted;
                        objItem.Created = item.Created; objItem.CreatedBy = item.CreatedBy;
                        objItem.OrderDetailGUID = item.OrderDetailGUID;

                        if (objBinDTO != null)
                            objItem.BinNumber = objBinDTO.BinNumber;

                        if (item.ID > 0 && IsValid)
                        {
                            objItem.GUID = item.GUID;
                            objItem.IsOnlyFromUI = item.IsOnlyFromUI;
                            if (item.IsOnlyFromUI)
                            {
                                objItem.EditedFrom = "Web";
                                objItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                            }
                            objItemDetailDAL.Edit(objItem);
                            // here needs to implement the edit quantity logic for location details table
                            IsQTYSave = true;
                        }
                        else
                        {
                            if (IsValid)
                            {
                                bool IsRecordSave = false;
                                //// here needs to write code for 4 points as following...
                                // 1 // if item is SR. then check for duplication if found then make them IsDeleted = 1
                                //   // And then Insert the New item with same SR. number
                                // 2 // If item is Lot, then check if duplicate number then only update the quantity
                                //   // for that particular lot number also check BinLocation if exist then update else insert ....
                                // 3 // Needs to enter only one entry to PullMaster with Total Quantity and data
                                // 4 // Then needs to enter all the entry to PullDetails with itemlocation data ..
                                #region "SR Case"
                                if (TempSerialNumberTracking)
                                {
                                    //var TempDupSrs = objItemDetailDAL.GetCachedData(item.ItemGUID.Value, item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0)).Where(x => x.SerialNumber == item.SerialNumber).ToList();
                                    var TempDupSrs = objItemDetailDAL.GetItemLocationDetail(item.ItemGUID.Value, item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0), null, item.SerialNumber, string.Empty, string.Empty, null, null, string.Empty).ToList();
                                    if (TempDupSrs != null && TempDupSrs.Count() > 0)
                                    {
                                        foreach (ItemLocationDetailsDTO IDTemp in TempDupSrs)
                                        {
                                            IDTemp.IsDeleted = true;
                                            IDTemp.IsArchived = true;
                                            IDTemp.LastUpdatedBy = item.LastUpdatedBy;
                                            IDTemp.Updated = DateTimeUtility.DateTimeNow;
                                            IDTemp.IsOnlyFromUI = true;
                                            IDTemp.EditedFrom = "Web";
                                            IDTemp.ReceivedOn = DateTimeUtility.DateTimeNow;
                                            objItemDetailDAL.Edit(IDTemp);
                                        }
                                    }
                                }
                                #endregion
                                //#region "LOT Case"
                                //if (TempLotNumberTracking)
                                //{// bin wise check the lot nummber if found then updated else insert case ....
                                //    var TempDupSrs = objItemDetailDAL.GetCachedData(item.ItemGUID.Value, item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0)).Where(x => x.LotNumber == item.LotNumber && x.BinID == item.BinID).SingleOrDefault();
                                //    if (TempDupSrs != null)
                                //    {
                                //        TempDupSrs.ConsignedQuantity = TempDupSrs.ConsignedQuantity.GetValueOrDefault(0) + objItem.ConsignedQuantity.GetValueOrDefault(0);
                                //        TempDupSrs.CustomerOwnedQuantity = TempDupSrs.CustomerOwnedQuantity.GetValueOrDefault(0) + objItem.CustomerOwnedQuantity.GetValueOrDefault(0);
                                //        objItemDetailDAL.Edit(TempDupSrs);
                                //        IsRecordSave = true;
                                //    }
                                //}
                                //#endregion

                                if (!IsRecordSave)
                                {
                                    objItem.AddedFrom = "Web";
                                    objItem.EditedFrom = "Web";
                                    objItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    objItem.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                    objItem.IsOnlyFromUI = item.IsOnlyFromUI;
                                    ItemLocationDetailsDTO itemloctemp = objItemDetailDAL.Insert(objItem);
                                    item.ID = itemloctemp.ID;
                                    item.GUID = itemloctemp.GUID;
                                    IsQTYSave = true;
                                }
                                #region "PULL Details Entry"
                                PullDetailsDTO objPullDetailDTO = new PullDetailsDTO();
                                objPullDetailDTO.BinID = item.BinID;
                                objPullDetailDTO.CompanyID = item.CompanyID;
                                objPullDetailDTO.ConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0);
                                objPullDetailDTO.Created = DateTimeUtility.DateTimeNow;
                                objPullDetailDTO.CreatedBy = item.CreatedBy.GetValueOrDefault(0);
                                objPullDetailDTO.CreatedByName = item.CreatedByName;
                                objPullDetailDTO.CustomerOwnedQuantity = item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                objPullDetailDTO.Expiration = item.Expiration;
                                objPullDetailDTO.IsArchived = item.IsArchived;
                                objPullDetailDTO.IsDeleted = item.IsDeleted;
                                objPullDetailDTO.ItemCost = CalculateAndGetPullCreditCost(objItem.Cost, objItemDTO.CostUOMID, item.Room ?? 0, item.CompanyID ?? 0);
                                objPullDetailDTO.ItemGUID = item.ItemGUID;
                                objPullDetailDTO.LastUpdatedBy = item.LastUpdatedBy.GetValueOrDefault(0);
                                objPullDetailDTO.LotNumber = item.LotNumber;
                                objPullDetailDTO.PoolQuantity = (item.ConsignedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0));
                                objPullDetailDTO.PullCredit = PullAction;
                                objPullDetailDTO.Received = item.Received;
                                objPullDetailDTO.Room = item.Room;
                                objPullDetailDTO.RoomName = item.RoomName;
                                objPullDetailDTO.SerialNumber = item.SerialNumber;
                                objPullDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                                objPullDetailDTO.UpdatedByName = item.UpdatedByName;
                                objPullDetailDTO.ItemLocationDetailGUID = item.GUID;
                                objPullDetailDTO.ProjectSpendGUID = item.ProjectSpentGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : item.ProjectSpentGUID;
                                objPullDetailDTO.WorkOrderGUID = item.WorkOrderGUID;
                                objPullDetailDTO.COuntLineItemGUID = item.CountLineItemDtlGUID;
                                objPullDetailDTO.AddedFrom = "Web";
                                objPullDetailDTO.EditedFrom = "Web";
                                objPullDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objPullDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                                objPullDetailsDTOs.Add(objPullDetailDTO);
                                #endregion
                            }
                        }
                    }
                    #endregion
                }

                #region "PULL Master and Detail Save"
                PullMasterDAL objPullDAL = new PullMasterDAL(base.DataBaseName);
                PullMasterViewDTO objPullDTO = new PullMasterViewDTO();
                if (objPullDetailsDTOs.Count > 0)
                {
                    objPullDTO.ActionType = PullAction;
                    objPullDTO.BinID = objPullDetailsDTOs[0].BinID;
                    objPullDTO.CompanyID = objPullDetailsDTOs[0].CompanyID;
                    objPullDTO.ConsignedQuantity = objPullDetailsDTOs.Select(x => x.ConsignedQuantity.GetValueOrDefault(0)).Sum();
                    objPullDTO.CustomerOwnedQuantity = objPullDetailsDTOs.Select(x => x.CustomerOwnedQuantity.GetValueOrDefault(0)).Sum();
                    objPullDTO.Created = DateTimeUtility.DateTimeNow;
                    objPullDTO.CreatedBy = objPullDetailsDTOs[0].CreatedBy.GetValueOrDefault(0);
                    objPullDTO.CreatedByName = objPullDetailsDTOs[0].CreatedByName;
                    objPullDTO.GUID = Guid.NewGuid();
                    objPullDTO.IsArchived = false;
                    objPullDTO.IsDeleted = false;
                    objPullDTO.ItemGUID = objItemDTO.GUID;
                    objPullDTO.ItemNumber = objItemDTO.ItemNumber;
                    objPullDTO.ItemType = objItemDTO.ItemType;
                    objPullDTO.LastUpdatedBy = objPullDetailsDTOs[0].LastUpdatedBy.GetValueOrDefault(0);
                    objPullDTO.PoolQuantity = objPullDetailsDTOs.Select(x => x.PoolQuantity.GetValueOrDefault(0)).Sum();
                    objPullDTO.PullCredit = PullAction;
                    objPullDTO.PullCost = Convert.ToDouble(objPullDetailsDTOs.Select(x => x.ItemCost.GetValueOrDefault(0) * x.PoolQuantity.GetValueOrDefault(0)).Sum());
                    objPullDTO.Room = objPullDetailsDTOs[0].Room;
                    objPullDTO.RoomName = objPullDetailsDTOs[0].RoomName;
                    objPullDTO.Updated = DateTimeUtility.DateTimeNow;
                    objPullDTO.UpdatedByName = objPullDetailsDTOs[0].UpdatedByName;
                    objPullDTO.ProjectSpendGUID = objPullDetailsDTOs[0].ProjectSpendGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : objPullDetailsDTOs[0].ProjectSpendGUID;
                    objPullDTO.WorkOrderDetailGUID = objPullDetailsDTOs[0].WorkOrderGUID;
                    objPullDTO.WorkOrderGUID = objPullDetailsDTOs[0].WorkOrderGUID;
                    objPullDTO.CountLineItemGuid = objPullDetailsDTOs[0].COuntLineItemGUID;
                    objPullDTO.WhatWhereAction = PullAction;

                    objPullDTO.AddedFrom = "Web";
                    objPullDTO.EditedFrom = "Web";
                    objPullDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objPullDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                    objPullDTO.ID = objPullDAL.Insert(objPullDTO);
                    PullDetailsDAL objPullDetailsDAL = new PullDetailsDAL(base.DataBaseName);
                    foreach (PullDetailsDTO temppdDTO in objPullDetailsDTOs)
                    {
                        temppdDTO.PULLGUID = objPullDTO.GUID;
                        temppdDTO.AddedFrom = "Web";
                        temppdDTO.EditedFrom = "Web";
                        temppdDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        temppdDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objPullDetailsDAL.Insert(temppdDTO);


                        //Project Spend update
                        if (temppdDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                        {
                            ProjectMasterDAL objPrjMsgDAL = new ProjectMasterDAL(base.DataBaseName);
                            ProjectSpendItemsDAL objPrjSpenItmDAL = new ProjectSpendItemsDAL(base.DataBaseName);
                            ProjectMasterDTO objPrjMstDTO = new ProjectMasterDTO();
                            ProjectSpendItemsDTO objPrjSpenItmDTO = new ProjectSpendItemsDTO();
                            var tmpsupplierIds = new List<long>();
                            //objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecords(objPullDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)objPullDetailsDTOs[0].Room, (Int64)objPullDetailsDTOs[0].CompanyID, tmpsupplierIds).Where(x => x.ItemGUID == objPullDTO.ItemGUID && x.Created.Value <= temppdDTO.Created.Value).SingleOrDefault();
                            objPrjSpenItmDTO = objPrjSpenItmDAL.GetProjectSpendItemCompareDate(objPullDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)objPullDetailsDTOs[0].Room, (Int64)objPullDetailsDTOs[0].CompanyID, tmpsupplierIds, Convert.ToString(objPullDTO.ItemGUID), temppdDTO.Created.Value.ToString("yyyy-MM-dd HH:mm:ss")).SingleOrDefault();
                            objPrjMstDTO = objPrjMsgDAL.GetRecord(objPullDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)objPullDetailsDTOs[0].Room, (Int64)objPullDetailsDTOs[0].CompanyID, false, false);

                            if (objPrjMstDTO != null)
                            {

                                objPrjMstDTO.DollarUsedAmount = objPrjMstDTO.DollarUsedAmount.GetValueOrDefault(0) - ((decimal)temppdDTO.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(temppdDTO.ItemCost.GetValueOrDefault(0)));
                                objPrjMstDTO.WhatWhereAction = PullAction;
                                //objPrjMstDTO.EditedFrom = "Web";
                                //objPrjMstDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objPrjMsgDAL.Edit(objPrjMstDTO);

                                if (objPrjSpenItmDTO != null)
                                {

                                    objPrjSpenItmDTO.DollarUsedAmount = objPrjSpenItmDTO.DollarUsedAmount.GetValueOrDefault(0) - ((decimal)temppdDTO.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(temppdDTO.ItemCost.GetValueOrDefault(0)));
                                    objPrjSpenItmDTO.QuantityUsed = objPrjSpenItmDTO.QuantityUsed.GetValueOrDefault(0) - temppdDTO.PoolQuantity.GetValueOrDefault(0);
                                    //objPrjSpenItmDTO.EditedFrom = "Web";
                                    //objPrjSpenItmDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    objPrjSpenItmDAL.Edit(objPrjSpenItmDTO);
                                }
                            }
                        }

                    }




                }
                #endregion

                if (objItemDTO != null && objItemDTO.ItemType != 4)
                {
                    #region "Insert QTY logic"
                    if (IsQTYSave)
                    {
                        if (TempSerialNumberTracking)
                        {
                            #region "Insert mode for Serial Number"
                            ItemLocationQTYDAL objQtyMgt = new ItemLocationQTYDAL(base.DataBaseName);

                            //FOR INSERT MODE
                            var objilqInsert = new List<ItemLocationQTYDTO>();

                            objilqInsert = (from x in objData
                                            where (x.SerialNumberTracking == true && x.SerialNumber != null) && x.BinID != null
                                            group x by new { x.BinID }
                                                into grp
                                            select new ItemLocationQTYDTO
                                            {
                                                BinID = Convert.ToInt64(grp.Key.BinID),
                                                ItemGUID = objData[0].ItemGUID,
                                                CompanyID = objData[0].CompanyID,
                                                Room = objData[0].Room,
                                                LastUpdated = DateTime.Now,
                                                Created = DateTime.Now,
                                                CreatedBy = objData[0].CreatedBy,
                                                LastUpdatedBy = objData[0].LastUpdatedBy,
                                                CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                                ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                                Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                            }).ToList();


                            //objilqInsert.Select(c => 
                            //{
                            //    if (objItemDTO.Consignment)
                            //    {
                            //        c.ConsignedQuantity= 1;
                            //    }
                            //    else
                            //    {
                            //        c.CustomerOwnedQuantity = 1;
                            //    }
                            //    return c; 
                            //}).ToList();



                            if (objilqInsert.Count() > 0)
                            {
                                objQtyMgt.Save(objilqInsert, "Credit", SessionUserId, 0);
                            }
                            #endregion
                        }
                        else if (TempLotNumberTracking)
                        {
                            #region "Insert Mode for Lot#, Others"
                            ItemLocationQTYDAL objQtyMgt = new ItemLocationQTYDAL(base.DataBaseName);
                            if (objItemDTO.Consignment)
                            {
                                var objilq = (from x in objData
                                              where (x.LotNumberTracking == true && x.LotNumber != null) && x.BinID != null
                                              group x by new { x.BinID } into grp
                                              select new ItemLocationQTYDTO
                                              {
                                                  BinID = Convert.ToInt64(grp.Key.BinID),
                                                  ItemGUID = objData[0].ItemGUID,
                                                  CompanyID = objData[0].CompanyID,
                                                  Room = objData[0].Room,
                                                  LastUpdated = DateTime.Now,
                                                  Created = DateTime.Now,
                                                  CreatedBy = objData[0].CreatedBy,
                                                  LastUpdatedBy = objData[0].LastUpdatedBy,
                                                  CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                                  ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                                  Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                                  //LotNumber = x.LotNumber,
                                              }).ToList();
                                //Save
                                objQtyMgt.Save(objilq, "Credit", SessionUserId, 0);
                                //Update on_hand qunatity
                                //objilq.Select(c => { OnHandQuantity += c.Quantity; return c; }).ToList();
                            }
                            else
                            {
                                var objilq = (from x in objData
                                              where (x.LotNumberTracking == true && x.LotNumber != null) && x.BinID != null
                                              group x by new { x.BinID } into grp
                                              select new ItemLocationQTYDTO
                                              {
                                                  BinID = Convert.ToInt64(grp.Key.BinID),
                                                  ItemGUID = objData[0].ItemGUID,
                                                  CompanyID = objData[0].CompanyID,
                                                  Room = objData[0].Room,
                                                  LastUpdated = DateTime.Now,
                                                  Created = DateTime.Now,
                                                  CreatedBy = objData[0].CreatedBy,
                                                  LastUpdatedBy = objData[0].LastUpdatedBy,
                                                  CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                                  ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                                  Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                                  //LotNumber = x.LotNumber,
                                              }).ToList();

                                //Save
                                objQtyMgt.Save(objilq, "Credit", SessionUserId, 0);
                            }
                            //Save on hand quantity
                            #endregion
                        }
                        else
                        {
                            #region "Insert Mode for Lot#, Others"
                            ItemLocationQTYDAL objQtyMgt = new ItemLocationQTYDAL(base.DataBaseName);
                            if (objItemDTO.Consignment)
                            {
                                var objilq = (from x in objData
                                              where x.BinID != null
                                              group x by new { x.BinID } into grp
                                              select new ItemLocationQTYDTO
                                              {
                                                  BinID = Convert.ToInt64(grp.Key.BinID),
                                                  ItemGUID = objData[0].ItemGUID,
                                                  CompanyID = objData[0].CompanyID,
                                                  Room = objData[0].Room,
                                                  LastUpdated = DateTime.Now,
                                                  Created = DateTime.Now,
                                                  CreatedBy = objData[0].CreatedBy,
                                                  LastUpdatedBy = objData[0].LastUpdatedBy,
                                                  CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                                  ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                                  Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                                  //LotNumber = x.LotNumber,
                                              }).ToList();
                                //Save
                                objQtyMgt.Save(objilq, "Credit", SessionUserId, 0);
                                //Update on_hand qunatity
                                //objilq.Select(c => { OnHandQuantity += c.Quantity; return c; }).ToList();
                            }
                            else
                            {
                                var objilq = (from x in objData
                                              where x.BinID != null
                                              group x by new { x.BinID } into grp
                                              select new ItemLocationQTYDTO
                                              {
                                                  BinID = Convert.ToInt64(grp.Key.BinID),
                                                  ItemGUID = objData[0].ItemGUID,
                                                  CompanyID = objData[0].CompanyID,
                                                  Room = objData[0].Room,
                                                  LastUpdated = DateTime.Now,
                                                  Created = DateTime.Now,
                                                  CreatedBy = objData[0].CreatedBy,
                                                  LastUpdatedBy = objData[0].LastUpdatedBy,
                                                  CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                                  ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                                  Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                                  //LotNumber = x.LotNumber,
                                              }).ToList();

                                //Save
                                objQtyMgt.Save(objilq, "Credit", SessionUserId, 0);
                            }
                            //Save on hand quantity
                            #endregion
                        }
                    }
                    #endregion
                }

                objPullDTO = new PullMasterDAL(base.DataBaseName).UpdateCumulativeOnHand(objPullDTO);
            }

            return true;
        }

        /// <summary>
        /// Method to Credit MS
        /// </summary>
        /// <param name="objData"></param>
        /// <returns></returns>
        public bool ItemLocationDetailsSaveForMSCredit(List<MaterialStagingPullDetailDTO> objData, Int64 RoomID, Int64 CompanyID, Int64 UserID, string RoomDateFormat,
            long SessionUserId, long EnterpriseId, string PullCreditType = "MS Credit")
        {
            bool TempSerialNumberTracking = false;
            bool TempLotNumberTracking = false;
            //bool Consignment = false;
            ItemMasterDTO objItemDTO = null;
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            //bool IsQTYSave = false;


            MaterialStagingDAL objMSDAL = new MaterialStagingDAL(base.DataBaseName);
            MaterialStagingDetailDAL objMSDDAL = new MaterialStagingDetailDAL(base.DataBaseName);
            MaterialStagingPullDetailDAL objItemDetailDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
            MaterialStagingDTO MSDTO = objMSDAL.GetRecordByName("Staging Credit", RoomID, CompanyID);

            if (MSDTO == null)
            {
                MSDTO = new MaterialStagingDTO();
                MSDTO.GUID = Guid.NewGuid();
                MSDTO.StagingName = "Staging Credit";
                MSDTO.Description = "This is record for credit in MaterialStaging";
                MSDTO.CompanyID = CompanyID;
                MSDTO.Room = RoomID;
                MSDTO.IsArchived = false;
                MSDTO.IsDeleted = false;
                MSDTO.Created = DateTimeUtility.DateTimeNow;
                MSDTO.Updated = DateTimeUtility.DateTimeNow;
                MSDTO.LastUpdatedBy = UserID;
                MSDTO.CreatedBy = UserID;
                MSDTO.WhatWhereAction = "MS Credit";
                MSDTO.AddedFrom = "Web";
                MSDTO.EditedFrom = "Web";
                MSDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                MSDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                MSDTO.StagingStatus = 1;

                objMSDAL.Insert(MSDTO);

                MSDTO = objMSDAL.GetRecordByName("Staging Credit", RoomID, CompanyID);

                ////                string stlQuery = @"INSERT INTO [dbo].[MaterialStaging] ([StagingName],[Description],[BinID],[GUID],[CompanyID],[Room],[IsDeleted],[IsArchived],[Created],[Updated],[CreatedBy],[LastUpdatedBy],[BinGUID])
                ////                                        VALUES ('Staging Credit','This is record for credit in MaterialStaging',null,NEWID()," + CompanyID.ToString() + ", " + RoomID.ToString() + ",0,0,getutcdate(),getutcdate()," + UserID.ToString() + "," + UserID.ToString() + ",null) ";
                ////                using (eTurnsEntities context1 = new eTurnsEntities(base.DataBaseEntityConnectionString))
                //                {
                //                    context1.ExecuteStoreCommand(stlQuery);

                //                    MSDTO = objMSDAL.GetCachedData(RoomID, CompanyID).Where(t => t.StagingName == "Staging Credit").OrderByDescending(t => t.ID).FirstOrDefault();
                //                }
            }

            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                int LooptotalCount = objData.Count();
                int Counter = 0;
                bool IsFromPull = true;
                List<PullDetailsDTO> objPullDetailsDTOs = new List<PullDetailsDTO>();
                foreach (MaterialStagingPullDetailDTO item in objData)
                {

                    #region "Location Detail Save"
                    Counter++;

                    if (item.BinID == null)
                    {
                        continue;
                    }
                    if (objItemDTO == null)
                    {
                        objItemDTO = objItemDAL.GetItemWithoutJoins(null, item.ItemGUID);
                    }

                    item.IsArchived = (item.IsArchived == null ? false : item.IsArchived);
                    item.IsDeleted = (item.IsDeleted == null ? false : item.IsDeleted);
                    BinMasterDAL objBin = new BinMasterDAL(base.DataBaseName);
                    BinMasterDTO objBinDTO = objBin.GetBinByID(Convert.ToInt64(item.BinID), Convert.ToInt64(item.Room), Convert.ToInt64(item.CompanyID));
                    //BinMasterDTO objBinDTO = objBin.GetItemLocation( Convert.ToInt64(item.Room), Convert.ToInt64(item.CompanyID), false, false,Guid.Empty, Convert.ToInt64(item.BinID),null,null).FirstOrDefault();



                    bool IsValid = false;

                    if (!string.IsNullOrEmpty(item.Received))
                        IsValid = true;
                    else if (!string.IsNullOrEmpty(item.SerialNumber))
                    {
                        IsValid = true;
                    }
                    else if (!string.IsNullOrEmpty(item.LotNumber))
                        IsValid = true;
                    else if (!string.IsNullOrEmpty(item.Expiration))
                        IsValid = true;
                    else if (item.LotNumberTracking && item.SerialNumberTracking)
                        IsValid = true;
                    else if (item.CustomerOwnedQuantity.HasValue && item.BinID.HasValue && item.ItemCost.HasValue && !string.IsNullOrEmpty(item.Received))
                        IsValid = true;


                    if (!(item.ConsignedQuantity.GetValueOrDefault(0) > 0 || item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0))
                    {
                        IsValid = false;
                    }


                    if (IsValid)
                    {
                        //find out - MaterialStagingDetail and update it
                        MaterialStagingDetailDTO objMSDDTO = new MaterialStagingDetailDTO(); //objMSDDAL.GetMaterialStagingDetailbyItemGUIDANDStagingBINID(item.ItemGUID, item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0)).Take(1).SingleOrDefault();
                        objMSDDTO = objMSDDAL.GetMaterialStagingDetailbyItemGUIDANDStagingBINID(item.MaterialStagingGUID.GetValueOrDefault(Guid.Empty), item.BinID.GetValueOrDefault(0), item.ItemGUID, item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0));

                        //TODO: IF block Added By CP for Select Staging Header 
                        if (item.OrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                        {
                            OrderDetailsDTO objOrdDtlDTO = new OrderDetailsDAL(base.DataBaseName).GetOrderDetailByGuidNormal(item.OrderDetailGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                            //OrderMasterDTO objOrdMstDTO = new OrderMasterDAL(base.DataBaseName).GetOrderByGuidPlain(objOrdDtlDTO.OrderGUID.GetValueOrDefault(Guid.Empty));

                            if (objOrdDtlDTO.StagingID.GetValueOrDefault(0) > 0)
                            {
                                MSDTO = new MaterialStagingDAL(base.DataBaseName).GetRecord(objOrdDtlDTO.StagingID.GetValueOrDefault(0), RoomID, CompanyID);
                            }
                        }
                        else if (item.MaterialStagingGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                        {
                            MSDTO = new MaterialStagingDAL(base.DataBaseName).GetRecord(item.MaterialStagingGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                        }

                        if (objMSDDTO == null)
                        {
                            objMSDDTO = new MaterialStagingDetailDTO();
                            objMSDDTO.MaterialStagingGUID = MSDTO.GUID;
                            objMSDDTO.GUID = Guid.NewGuid(); //Guid.Parse(msdtlGUID);
                            objMSDDTO.ItemGUID = item.ItemGUID;
                            objMSDDTO.Quantity = (item.CustomerOwnedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0));
                            objMSDDTO.StagingBinID = item.BinID.GetValueOrDefault(0);
                            objMSDDTO.StagingBinName = objBinDTO.BinNumber;
                            objMSDDTO.RoomId = RoomID;
                            objMSDDTO.CompanyID = CompanyID;
                            objMSDDTO.Created = DateTimeUtility.DateTimeNow;
                            objMSDDTO.LastUpdatedBy = UserID;
                            objMSDDTO.Updated = DateTimeUtility.DateTimeNow;
                            objMSDDTO.CreatedBy = UserID;
                            objMSDDTO.AddedFrom = "Web";
                            objMSDDTO.EditedFrom = "Web";
                            objMSDDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objMSDDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            //objMSDDTO.WhatWhereAction = "MS Credit";
                            objMSDDAL.Insert(objMSDDTO);
                        }

                        item.MaterialStagingdtlGUID = objMSDDTO.GUID;
                        //if (!String.IsNullOrEmpty(item.SerialNumber) || !String.IsNullOrEmpty(item.LotNumber) || item.CustomerOwnedQuantity.Value > 0 || item.ConsignedQuantity.Value > 0)
                        //{

                        MaterialStagingPullDetailDTO objItem = new MaterialStagingPullDetailDTO();
                        //objItem.BinID = item.BinID;
                        objItem.StagingBinId = item.BinID.GetValueOrDefault(0);

                        objItem.MaterialStagingdtlGUID = objMSDDTO.GUID;
                        objItem.MaterialStagingGUID = MSDTO.GUID;

                        objItem.CompanyID = item.CompanyID;
                        objItem.ItemCost = item.ItemCost;
                        objItem.Created = item.Created;

                        objItem.GUID = Guid.NewGuid();

                        if (objItemDTO.Consignment)
                        {
                            //Consignment = true;
                        }

                        objItem.CustomerOwnedQuantity = item.CustomerOwnedQuantity;
                        objItem.ConsignedQuantity = item.ConsignedQuantity;

                        objItem.IsConsignedSerialLot = (objItem.ConsignedQuantity.HasValue && objItem.ConsignedQuantity.Value > 0 ? true : false);
                        //objItem.ExpirationDate = item.ExpirationDate;

                        objItem.Expiration = item.Expiration;
                        objItem.Received = item.Received;

                        //if (!string.IsNullOrEmpty(objItem.Received))
                        //{
                        //    //objItem.ReceivedDate = DateTime.Parse(objItem.Received);
                        //    objItem.Received = objItem.Received;
                        //}

                        objItem.ID = item.ID; objItem.ItemGUID = item.ItemGUID;

                        //objItem.LotNumber = item.LotNumber;
                        //if (item.LotNumber != null)
                        //    TempLotNumberTracking = true;
                        //objItem.SerialNumber = item.SerialNumber;
                        //if (item.SerialNumber != null)
                        //    TempSerialNumberTracking = true;

                        objItem.LotNumber = item.LotNumber;
                        if (item.LotNumber != null && item.LotNumberTracking)
                            TempLotNumberTracking = true;
                        objItem.SerialNumber = item.SerialNumber;
                        if (item.SerialNumber != null && item.SerialNumberTracking)
                            TempSerialNumberTracking = true;

                        objItem.CreatedBy = item.CreatedBy; objItem.CompanyID = item.CompanyID; objItem.Room = item.Room; objItem.Updated = item.Updated; objItem.LastUpdatedBy = item.LastUpdatedBy; objItem.Received = item.Received; objItem.IsArchived = item.IsArchived; objItem.IsDeleted = item.IsDeleted; objItem.Created = item.Created; objItem.CreatedBy = item.CreatedBy; objItem.OrderDetailGUID = item.OrderDetailGUID;
                        if (objBinDTO != null)
                        {
                            objItem.BinNumber = objBinDTO.BinNumber;
                        }

                        objItem.PackSlipNumber = item.PackSlipNumber;
                        objItem.UDF1 = item.UDF1;
                        objItem.UDF2 = item.UDF2;
                        objItem.UDF3 = item.UDF3;
                        objItem.UDF4 = item.UDF4;
                        objItem.UDF5 = item.UDF5;

                        if (IsFromPull && (item.OrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty || PullCreditType == "Staging Transfer Receive"))
                        {
                            IsFromPull = false;
                        }

                        if (item.ID > 0 && IsValid)
                        {
                            objItem.GUID = item.GUID;
                            objItemDetailDAL.Edit(objItem);
                            // here needs to implement the edit quantity logic for location details table
                            //IsQTYSave = true;
                        }
                        else
                        {
                            if (IsValid)
                            {
                                bool IsRecordSave = false;
                                //// here needs to write code for 4 points as following...
                                // 1 // if item is SR. then check for duplication if found then make them IsDeleted = 1
                                //   // And then Insert the New item with same SR. number
                                // 2 // If item is Lot, then check if duplicate number then only update the quantity
                                //   // for that particular lot number also check BinLocation if exist then update else insert ....
                                // 3 // Needs to enter only one entry to PullMaster with Total Quantity and data
                                // 4 // Then needs to enter all the entry to PullDetails with itemlocation data ..
                                #region "SR Case"
                                if (TempSerialNumberTracking)
                                {
                                    //var TempDupSrs = objItemDetailDAL.GetMsPullDetailsByItemGUID(item.ItemGUID, item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0)).Where(x => x.SerialNumber == item.SerialNumber).ToList();
                                    var TempDupSrs = objItemDetailDAL.GetMsPullDetailsByItemGUID(item.ItemGUID, item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0), string.Empty, item.SerialNumber, string.Empty, string.Empty, 0, string.Empty, null).ToList();
                                    if (TempDupSrs != null && TempDupSrs.Count() > 0)
                                    {
                                        foreach (MaterialStagingPullDetailDTO IDTemp in TempDupSrs)
                                        {
                                            IDTemp.IsDeleted = true;
                                            IDTemp.IsArchived = true;
                                            IDTemp.LastUpdatedBy = item.LastUpdatedBy;
                                            IDTemp.Updated = DateTimeUtility.DateTimeNow;
                                            objItemDetailDAL.Edit(IDTemp);
                                        }
                                    }
                                }
                                #endregion
                                #region "LOT Case"
                                if (TempLotNumberTracking)
                                {// bin wise check the lot nummber if found then updated else insert case ....
                                    //var TempDupSrs = objItemDetailDAL.GetMsPullDetailsByItemGUID(item.ItemGUID, item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0)).Where(x => x.LotNumber == item.LotNumber && x.StagingBinId == item.StagingBinId).SingleOrDefault();
                                    var TempDupSrs = objItemDetailDAL.GetMsPullDetailsByItemGUID(item.ItemGUID, item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0), item.LotNumber, string.Empty, string.Empty, string.Empty, item.StagingBinId, string.Empty, null).FirstOrDefault();
                                    if (TempDupSrs != null)
                                    {
                                        TempDupSrs.ConsignedQuantity = TempDupSrs.ConsignedQuantity.GetValueOrDefault(0) + objItem.ConsignedQuantity.GetValueOrDefault(0);
                                        TempDupSrs.CustomerOwnedQuantity = TempDupSrs.CustomerOwnedQuantity.GetValueOrDefault(0) + objItem.CustomerOwnedQuantity.GetValueOrDefault(0);
                                        objItemDetailDAL.Edit(TempDupSrs);
                                        IsRecordSave = true;
                                    }
                                }
                                #endregion

                                if (!IsRecordSave)
                                {
                                    if (PullCreditType == "Staging Transfer Receive")
                                    {
                                        objItem.TransferDetailGUID = item.TransferDetailGUID;
                                        objItem.EditedFrom = "Web Transfer Staging";
                                        objItem.AddedFrom = "Web Transfer Staging";
                                    }

                                    MaterialStagingPullDetailDTO itemloctemp = objItemDetailDAL.Insert(true, objItem, RoomDateFormat);
                                    item.ID = itemloctemp.ID;
                                    item.GUID = itemloctemp.GUID;
                                    //IsQTYSave = true;
                                }
                                #region "PULL Details Entry"
                                if (IsFromPull)
                                {
                                    PullDetailsDTO objPullDetailDTO = new PullDetailsDTO();
                                    objPullDetailDTO.BinID = item.BinID;
                                    objPullDetailDTO.CompanyID = item.CompanyID;
                                    objPullDetailDTO.ConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0);
                                    objPullDetailDTO.Created = DateTimeUtility.DateTimeNow;
                                    objPullDetailDTO.CreatedBy = item.CreatedBy.GetValueOrDefault(0);
                                    objPullDetailDTO.CreatedByName = item.CreatedByName;
                                    objPullDetailDTO.CustomerOwnedQuantity = item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    objPullDetailDTO.Expiration = item.Expiration;
                                    objPullDetailDTO.IsArchived = item.IsArchived;
                                    objPullDetailDTO.IsDeleted = item.IsDeleted;
                                    objPullDetailDTO.ItemCost = objItem.ItemCost;
                                    objPullDetailDTO.ItemGUID = item.ItemGUID;
                                    objPullDetailDTO.LastUpdatedBy = item.LastUpdatedBy.GetValueOrDefault(0);
                                    objPullDetailDTO.LotNumber = item.LotNumber;
                                    objPullDetailDTO.PoolQuantity = (item.ConsignedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0));
                                    objPullDetailDTO.PullCredit = "MS credit";
                                    objPullDetailDTO.Received = item.Received;
                                    objPullDetailDTO.Room = item.Room;
                                    objPullDetailDTO.RoomName = item.RoomName;
                                    objPullDetailDTO.SerialNumber = item.SerialNumber;
                                    objPullDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                                    objPullDetailDTO.UpdatedByName = item.UpdatedByName;
                                    objPullDetailDTO.MaterialStagingPullDetailGUID = item.MaterialStagingdtlGUID;
                                    objPullDetailDTO.WorkOrderGUID = item.WorkOrderGuid;
                                    objPullDetailDTO.AddedFrom = "Web";
                                    objPullDetailDTO.EditedFrom = "Web";
                                    objPullDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    objPullDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                                    objPullDetailsDTOs.Add(objPullDetailDTO);
                                }
                                #endregion
                            }
                        }

                        //MaterialStagingDetailDTO objmsddto = objMSDDAL.GetRecord(item.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty), item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0));
                        MaterialStagingDetailDTO objmsddto = objMSDDAL.GetMaterialStagingDetailByGUID(item.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty), item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0));
                        if (objmsddto != null)
                        {
                            MaterialStagingPullDetailDAL MSPDDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
                            List<MaterialStagingPullDetailDTO> lstMSPDDTO = MSPDDAL.GetAllRecords(objmsddto.GUID, item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0)).ToList();

                            //objmsddto.Quantity = objmsddto.Quantity + (objItem.CustomerOwnedQuantity.GetValueOrDefault(0) + objItem.ConsignedQuantity.GetValueOrDefault(0));
                            objmsddto.Quantity = lstMSPDDTO.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0));
                            objMSDDAL.Edit(objmsddto);
                        }

                        string strWhatWhereAction = "MS Credit";
                        if (item.OrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                        {
                            strWhatWhereAction = "ILDsDAL.ILDsSaveMSCredit.StageOrderOrder";
                        }
                        else if (PullCreditType == "Staging Transfer Receive")
                        {
                            strWhatWhereAction = "ILDsDAL.ILDsSaveMSCredit.StageTransferRecieved";
                        }

                        //Update started quantity...
                        objItemDetailDAL.UpdateStagedQuantity(item.ItemGUID, item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0), strWhatWhereAction, SessionUserId, EnterpriseId);


                    }
                    #endregion
                }


                #region "PULL Master and Detail Save"
                if (IsFromPull)
                {
                    PullMasterDAL objPullDAL = new PullMasterDAL(base.DataBaseName);
                    PullMasterViewDTO objPullDTO = new PullMasterViewDTO();
                    if (objPullDetailsDTOs.Count > 0)
                    {
                        objPullDTO.ActionType = PullCreditType;
                        objPullDTO.BinID = objPullDetailsDTOs[0].BinID;
                        objPullDTO.CompanyID = objPullDetailsDTOs[0].CompanyID;
                        objPullDTO.ConsignedQuantity = objPullDetailsDTOs.Select(x => x.ConsignedQuantity.GetValueOrDefault(0)).Sum();
                        objPullDTO.CustomerOwnedQuantity = objPullDetailsDTOs.Select(x => x.CustomerOwnedQuantity.GetValueOrDefault(0)).Sum();
                        objPullDTO.Created = DateTimeUtility.DateTimeNow;
                        objPullDTO.CreatedBy = objPullDetailsDTOs[0].CreatedBy.GetValueOrDefault(0);
                        objPullDTO.CreatedByName = objPullDetailsDTOs[0].CreatedByName;
                        objPullDTO.GUID = Guid.NewGuid();
                        objPullDTO.IsArchived = false;
                        objPullDTO.IsDeleted = false;
                        objPullDTO.ItemGUID = objItemDTO.GUID;
                        objPullDTO.ItemNumber = objItemDTO.ItemNumber;
                        objPullDTO.ItemType = objItemDTO.ItemType;
                        objPullDTO.LastUpdatedBy = objPullDetailsDTOs[0].LastUpdatedBy.GetValueOrDefault(0);
                        objPullDTO.PoolQuantity = objPullDetailsDTOs.Select(x => x.PoolQuantity.GetValueOrDefault(0)).Sum();
                        objPullDTO.PullCredit = PullCreditType;
                        objPullDTO.PullCost = objPullDetailsDTOs.Select(x => x.ItemCost.GetValueOrDefault(0)).Sum();
                        objPullDTO.Room = objPullDetailsDTOs[0].Room;
                        objPullDTO.RoomName = objPullDetailsDTOs[0].RoomName;
                        objPullDTO.Updated = DateTimeUtility.DateTimeNow;
                        objPullDTO.UpdatedByName = objPullDetailsDTOs[0].UpdatedByName;
                        objPullDTO.WorkOrderGUID = objPullDetailsDTOs[0].WorkOrderGUID;
                        objPullDTO.WorkOrderDetailGUID = objPullDetailsDTOs[0].WorkOrderGUID;
                        objPullDTO.WhatWhereAction = PullCreditType;
                        objPullDTO.AddedFrom = "Web";
                        objPullDTO.EditedFrom = "Web";
                        objPullDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objPullDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objPullDTO.ID = objPullDAL.Insert(objPullDTO);
                        PullDetailsDAL objPullDetailsDAL = new PullDetailsDAL(base.DataBaseName);
                        foreach (PullDetailsDTO temppdDTO in objPullDetailsDTOs)
                        {
                            temppdDTO.PULLGUID = objPullDTO.GUID;
                            temppdDTO.AddedFrom = "Web";
                            temppdDTO.EditedFrom = "Web";
                            temppdDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            temppdDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objPullDetailsDAL.Insert(temppdDTO);
                        }
                    }
                    new PullMasterDAL(base.DataBaseName).UpdateCumulativeOnHand(objPullDTO);
                }
                #endregion

            }
            return true;
        }

        public bool ItemLocationDetailsSaveForCreditKit(List<ItemLocationDetailsDTO> objData, long SessionUserId, long EnterpriseId)
        {
            bool TempSerialNumberTracking = false;
            bool TempLotNumberTracking = false;
            //bool Consignment = false;
            ItemMasterDTO objItemDTO = null;
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            bool IsQTYSave = false;

            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                int LooptotalCount = objData.Count();
                int Counter = 0;
                List<PullDetailsDTO> objPullDetailsDTOs = new List<PullDetailsDTO>();
                foreach (ItemLocationDetailsDTO item in objData)
                {

                    #region "Location Detail Save"
                    Counter++;

                    if (item.BinID == null)
                    {
                        continue;
                    }
                    if (objItemDTO == null)
                    {
                        objItemDTO = objItemDAL.GetItemWithoutJoins(null, item.ItemGUID);
                    }

                    item.IsArchived = (item.IsArchived == null ? false : item.IsArchived);
                    item.IsDeleted = (item.IsDeleted == null ? false : item.IsDeleted);
                    BinMasterDAL objBin = new BinMasterDAL(base.DataBaseName);
                    BinMasterDTO objBinDTO = objBin.GetBinByID(Convert.ToInt64(item.BinID), Convert.ToInt64(item.Room), Convert.ToInt64(item.CompanyID));
                    //BinMasterDTO objBinDTO = objBin.GetItemLocation( Convert.ToInt64(item.Room), Convert.ToInt64(item.CompanyID), false, false,Guid.Empty, Convert.ToInt64(item.BinID),null,null).FirstOrDefault();

                    bool IsValid = false;

                    if (string.IsNullOrEmpty(item.Received) && item.ReceivedDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                        item.Received = item.ReceivedDate.Value.ToString("M/d/yyyy");

                    if (!string.IsNullOrEmpty(item.Received) || item.ReceivedDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                        IsValid = true;
                    else if (!string.IsNullOrEmpty(item.SerialNumber))
                        IsValid = true;
                    else if (!string.IsNullOrEmpty(item.LotNumber))
                        IsValid = true;
                    else if (!string.IsNullOrEmpty(item.Expiration) || (item.ExpirationDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue))
                        IsValid = true;
                    else if (item.LotNumberTracking && item.SerialNumberTracking)
                        IsValid = true;
                    else if (item.CustomerOwnedQuantity.HasValue && item.BinID.HasValue && item.Cost.HasValue && (!string.IsNullOrEmpty(item.Received) || item.ReceivedDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue))
                        IsValid = true;


                    if (IsValid || (LooptotalCount == Counter))
                    {
                        //if (!String.IsNullOrEmpty(item.SerialNumber) || !String.IsNullOrEmpty(item.LotNumber) || item.CustomerOwnedQuantity.Value > 0 || item.ConsignedQuantity.Value > 0)
                        //{
                        ItemLocationDetailsDAL objItemDetailDAL = new ItemLocationDetailsDAL(base.DataBaseName);
                        ItemLocationDetailsDTO objItem = new ItemLocationDetailsDTO();
                        objItem.BinID = item.BinID;

                        objItem.CompanyID = item.CompanyID;
                        objItem.Cost = CalculateAndGetPullCreditCost(item.Cost, objItemDTO.CostUOMID, item.Room ?? 0, item.CompanyID ?? 0);
                        objItem.Created = item.Created;

                        if (objItemDTO.Consignment)
                        {
                            //Consignment = true;
                        }

                        objItem.CustomerOwnedQuantity = item.CustomerOwnedQuantity;
                        objItem.ConsignedQuantity = item.ConsignedQuantity;


                        objItem.ExpirationDate = item.ExpirationDate;

                        objItem.Expiration = item.Expiration;
                        objItem.Received = item.Received;
                        objItem.ReceivedDate = item.ReceivedDate;

                        if (objItem.ReceivedDate.GetValueOrDefault(DateTime.MinValue) <= DateTime.MinValue && !string.IsNullOrEmpty(objItem.Received))
                        {
                            objItem.ReceivedDate = DateTime.Parse(objItem.Received);
                        }
                        if (objItem.ExpirationDate.GetValueOrDefault(DateTime.MinValue) <= DateTime.MinValue && !string.IsNullOrEmpty(objItem.Expiration))
                        {
                            objItem.ExpirationDate = DateTime.Parse(objItem.Expiration);
                        }

                        objItem.ID = item.ID; objItem.ItemGUID = item.ItemGUID;

                        //objItem.LotNumber = item.LotNumber;
                        //if (item.LotNumber != null)
                        //    TempLotNumberTracking = true;
                        //objItem.SerialNumber = item.SerialNumber;
                        //if (item.SerialNumber != null)
                        //    TempSerialNumberTracking = true;

                        objItem.LotNumber = item.LotNumber;
                        if (item.LotNumber != null && item.LotNumberTracking)
                            TempLotNumberTracking = true;
                        objItem.SerialNumber = item.SerialNumber;
                        if (item.SerialNumber != null && item.SerialNumberTracking)
                            TempSerialNumberTracking = true;


                        objItem.CreatedBy = item.CreatedBy; objItem.CompanyID = item.CompanyID;
                        objItem.Room = item.Room; objItem.Updated = item.Updated;
                        objItem.LastUpdatedBy = item.LastUpdatedBy;
                        objItem.IsArchived = item.IsArchived; objItem.IsDeleted = item.IsDeleted;
                        objItem.Created = item.Created; objItem.CreatedBy = item.CreatedBy;
                        objItem.OrderDetailGUID = item.OrderDetailGUID;


                        if (objBinDTO != null)
                            objItem.BinNumber = objBinDTO.BinNumber;

                        if (item.KitDetailGUID != null)
                            objItem.KitDetailGUID = item.KitDetailGUID;

                        if (item.ID > 0 && IsValid)
                        {
                            objItem.GUID = item.GUID;
                            objItemDetailDAL.Edit(objItem);
                            // here needs to implement the edit quantity logic for location details table
                            IsQTYSave = true;
                        }
                        else
                        {
                            if (IsValid)
                            {
                                bool IsRecordSave = false;
                                #region "SR Case"
                                if (TempSerialNumberTracking)
                                {
                                    //var TempDupSrs = objItemDetailDAL.GetCachedData(item.ItemGUID.Value, item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0)).Where(x => x.SerialNumber == item.SerialNumber).ToList();
                                    var TempDupSrs = objItemDetailDAL.GetItemLocationDetail(item.ItemGUID.Value, item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0), null, item.SerialNumber, string.Empty, string.Empty, null, null, string.Empty).ToList();
                                    if (TempDupSrs != null && TempDupSrs.Count() > 0)
                                    {
                                        foreach (ItemLocationDetailsDTO IDTemp in TempDupSrs)
                                        {
                                            IDTemp.IsDeleted = true;
                                            IDTemp.IsArchived = true;
                                            IDTemp.LastUpdatedBy = item.LastUpdatedBy;
                                            IDTemp.Updated = DateTimeUtility.DateTimeNow;
                                            objItemDetailDAL.Edit(IDTemp);
                                        }
                                    }
                                }
                                #endregion
                                #region "LOT Case"
                                if (TempLotNumberTracking)
                                {// bin wise check the lot nummber if found then updated else insert case ....
                                    //var TempDupSrs = objItemDetailDAL.GetCachedData(item.ItemGUID.Value, item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0)).Where(x => x.LotNumber == item.LotNumber && x.BinID == item.BinID).SingleOrDefault();
                                    var TempDupSrs = objItemDetailDAL.GetItemLocationDetail(item.ItemGUID.Value, item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0), item.BinID, string.Empty, item.LotNumber, string.Empty, null, null, string.Empty).FirstOrDefault();
                                    if (TempDupSrs != null)
                                    {
                                        TempDupSrs.ConsignedQuantity = TempDupSrs.ConsignedQuantity.GetValueOrDefault(0) + objItem.ConsignedQuantity.GetValueOrDefault(0);
                                        TempDupSrs.CustomerOwnedQuantity = TempDupSrs.CustomerOwnedQuantity.GetValueOrDefault(0) + objItem.CustomerOwnedQuantity.GetValueOrDefault(0);
                                        objItemDetailDAL.Edit(TempDupSrs);
                                        IsRecordSave = true;
                                    }
                                }
                                #endregion

                                if (!IsRecordSave)
                                {
                                    objItem.GUID = Guid.NewGuid();
                                    objItem.AddedFrom = "Web";
                                    objItem.EditedFrom = "Web";
                                    objItem.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                    objItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    ItemLocationDetailsDTO itemlocTemp = objItemDetailDAL.Insert(objItem);
                                    item.ID = itemlocTemp.ID;
                                    item.GUID = itemlocTemp.GUID;
                                    IsQTYSave = true;
                                }

                            }
                        }
                    }
                    #endregion
                }



                #region "Insert QTY logic"
                if (IsQTYSave)
                {
                    if (TempSerialNumberTracking)
                    {
                        #region "Insert mode for Serial Number"
                        ItemLocationQTYDAL objQtyMgt = new ItemLocationQTYDAL(base.DataBaseName);

                        //FOR INSERT MODE
                        var objilqInsert = new List<ItemLocationQTYDTO>();

                        objilqInsert = (from x in objData
                                        where (x.SerialNumberTracking == true && x.SerialNumber != null) && x.BinID != null
                                        group x by new { x.BinID }
                                            into grp
                                        select new ItemLocationQTYDTO
                                        {
                                            BinID = Convert.ToInt64(grp.Key.BinID),
                                            ItemGUID = objData[0].ItemGUID,
                                            CompanyID = objData[0].CompanyID,
                                            Room = objData[0].Room,
                                            LastUpdated = DateTime.Now,
                                            Created = DateTime.Now,
                                            CreatedBy = objData[0].CreatedBy,
                                            LastUpdatedBy = objData[0].LastUpdatedBy,
                                            CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                            ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                            Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                        }).ToList();

                        if (objilqInsert.Count() > 0)
                        {
                            objQtyMgt.Save(objilqInsert, SessionUserId, EnterpriseId);
                        }
                        #endregion
                    }
                    else if (TempLotNumberTracking)
                    {
                        #region "Insert Mode for Lot#, Others"
                        ItemLocationQTYDAL objQtyMgt = new ItemLocationQTYDAL(base.DataBaseName);
                        if (objItemDTO.Consignment)
                        {
                            var objilq = (from x in objData
                                          where (x.LotNumberTracking == true && x.LotNumber != null) && x.BinID != null
                                          group x by new { x.BinID } into grp
                                          select new ItemLocationQTYDTO
                                          {
                                              BinID = Convert.ToInt64(grp.Key.BinID),
                                              ItemGUID = objData[0].ItemGUID,
                                              CompanyID = objData[0].CompanyID,
                                              Room = objData[0].Room,
                                              LastUpdated = DateTime.Now,
                                              Created = DateTime.Now,
                                              CreatedBy = objData[0].CreatedBy,
                                              LastUpdatedBy = objData[0].LastUpdatedBy,
                                              CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                              ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                              Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                          }).ToList();
                            //Save
                            objQtyMgt.Save(objilq, SessionUserId, EnterpriseId);
                        }
                        else
                        {
                            var objilq = (from x in objData
                                          where (x.LotNumberTracking == true && x.LotNumber != null) && x.BinID != null
                                          group x by new { x.BinID } into grp
                                          select new ItemLocationQTYDTO
                                          {
                                              BinID = Convert.ToInt64(grp.Key.BinID),
                                              ItemGUID = objData[0].ItemGUID,
                                              CompanyID = objData[0].CompanyID,
                                              Room = objData[0].Room,
                                              LastUpdated = DateTime.Now,
                                              Created = DateTime.Now,
                                              CreatedBy = objData[0].CreatedBy,
                                              LastUpdatedBy = objData[0].LastUpdatedBy,
                                              CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                              ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                              Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                          }).ToList();

                            //Save
                            objQtyMgt.Save(objilq, SessionUserId, EnterpriseId);
                        }
                        //Save on hand quantity
                        #endregion
                    }
                    else
                    {
                        #region "Insert Mode for Lot#, Others"
                        ItemLocationQTYDAL objQtyMgt = new ItemLocationQTYDAL(base.DataBaseName);
                        if (objItemDTO.Consignment)
                        {
                            var objilq = (from x in objData
                                          where x.BinID != null
                                          group x by new { x.BinID } into grp
                                          select new ItemLocationQTYDTO
                                          {
                                              BinID = Convert.ToInt64(grp.Key.BinID),
                                              ItemGUID = objData[0].ItemGUID,
                                              CompanyID = objData[0].CompanyID,
                                              Room = objData[0].Room,
                                              LastUpdated = DateTime.Now,
                                              Created = DateTime.Now,
                                              CreatedBy = objData[0].CreatedBy,
                                              LastUpdatedBy = objData[0].LastUpdatedBy,
                                              CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                              ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                              Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                              //LotNumber = x.LotNumber,
                                          }).ToList();
                            //Save
                            objQtyMgt.Save(objilq, SessionUserId, EnterpriseId);
                        }
                        else
                        {
                            var objilq = (from x in objData
                                          where x.BinID != null
                                          group x by new { x.BinID } into grp
                                          select new ItemLocationQTYDTO
                                          {
                                              BinID = Convert.ToInt64(grp.Key.BinID),
                                              ItemGUID = objData[0].ItemGUID,
                                              CompanyID = objData[0].CompanyID,
                                              Room = objData[0].Room,
                                              LastUpdated = DateTime.Now,
                                              Created = DateTime.Now,
                                              CreatedBy = objData[0].CreatedBy,
                                              LastUpdatedBy = objData[0].LastUpdatedBy,
                                              CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                              ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                              Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                              //LotNumber = x.LotNumber,
                                          }).ToList();

                            //Save
                            objQtyMgt.Save(objilq, SessionUserId, EnterpriseId);
                        }
                        //Save on hand quantity
                        #endregion
                    }
                }
                #endregion
            }
            return true;
        }

        public bool ItemManufacturerDetailsSave(List<ItemManufacturerDetailsDTO> objData)
        {
            ItemManufacturerDetailsDAL objManuDAL = new ItemManufacturerDetailsDAL(base.DataBaseName);
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Guid ItemGuid = Guid.Empty;
                Guid.TryParse(objData[0].ItemGUID.ToString(), out ItemGuid);
                var TempData = objManuDAL.GetManufacturerByItemGuidNormal((Int64)objData[0].Room, (Int64)objData[0].CompanyID, ItemGuid, false);
                // if exists then first delete 
                if (TempData != null && TempData.Count() > 0)
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", Convert.ToString(objData[0].ItemGUID)),
                                                       new SqlParameter("@RoomID", objData[0].Room),
                                                       new SqlParameter("@CompanyID", objData[0].CompanyID) };

                    context.Database.ExecuteSqlCommand("EXEC DeleteItemManuDtlByItemGUID @ItemGUID,@RoomID,@CompanyID", params1);
                }
                //and then re-insert the data.
                foreach (ItemManufacturerDetailsDTO item in objData)
                {


                    objManuDAL.Insert(item);
                    if ((bool)item.IsDefault)
                    {
                        ItemMasterDAL TempItemDAL = new ItemMasterDAL(base.DataBaseName);
                        // update this record to Item Master table ....
                        ItemMasterDTO TempItemDTO = TempItemDAL.GetItemWithoutJoins(null, item.ItemGUID);
                        if (TempItemDTO != null)
                        {
                            TempItemDTO.ManufacturerID = item.ManufacturerID;
                            TempItemDTO.ManufacturerNumber = item.ManufacturerNumber;
                            TempItemDTO.CreatedBy = item.CreatedBy;
                            TempItemDTO.LastUpdatedBy = item.LastUpdatedBy;
                            TempItemDAL.EditMultiple(TempItemDTO);
                        }
                    }
                }
            }
            return true;
        }

        public bool ItemSupplierDetailsSave(List<ItemSupplierDetailsDTO> objData)
        {
            ItemSupplierDetailsDAL objSuppDAL = new ItemSupplierDetailsDAL(base.DataBaseName);
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                // first check ItemID to detail table whether exist or not             
                Guid ItemGuid = Guid.Empty;
                Guid.TryParse(objData[0].ItemGUID.ToString(), out ItemGuid);
                var TempData = objSuppDAL.GetSuppliersByItemGuidNormal((Int64)objData[0].Room, (Int64)objData[0].CompanyID, ItemGuid);
                // if exists then first delete 
                if (TempData != null && TempData.Count() > 0)
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", Convert.ToString(objData[0].ItemGUID)),
                                                       new SqlParameter("@RoomID", objData[0].Room),
                                                       new SqlParameter("@CompanyID", objData[0].CompanyID) };

                    context.Database.ExecuteSqlCommand("EXEC DeleteItemSuppDtlByItemGUID @ItemGUID,@RoomID,@CompanyID", params1);

                }
                //and then re-insert the data.
                foreach (ItemSupplierDetailsDTO item in objData)
                {
                    item.EditedFrom = "Web";
                    item.ReceivedOn = DateTimeUtility.DateTimeNow;
                    item.AddedFrom = "Web";
                    item.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objSuppDAL.Insert(item);
                    if ((bool)item.IsDefault)
                    {
                        ItemMasterDAL TempItemDAL = new ItemMasterDAL(base.DataBaseName);
                        // update this record to Item Master table ....
                        ItemMasterDTO TempItemDTO = TempItemDAL.GetItemWithoutJoins(null, item.ItemGUID);
                        if (TempItemDTO != null)
                        {
                            TempItemDTO.SupplierID = item.SupplierID;
                            TempItemDTO.SupplierPartNo = item.SupplierNumber;
                            TempItemDTO.CreatedBy = item.CreatedBy;
                            TempItemDTO.LastUpdatedBy = item.LastUpdatedBy;
                            TempItemDAL.EditMultiple(TempItemDTO);
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(ItemLocationDetailsDTO objDTO)
        {
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemLocationDetail obj = context.ItemLocationDetails.FirstOrDefault(t => t.ID == objDTO.ID);
                if (obj != null)
                {

                    double? oldCost = obj.Cost;
                    //obj.ID = objDTO.ID;

                    obj.BinID = objDTO.BinID;
                    obj.CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity;
                    obj.ConsignedQuantity = objDTO.ConsignedQuantity;
                    obj.MeasurementID = objDTO.MeasurementID;
                    obj.LotNumber = objDTO.LotNumber;
                    obj.SerialNumber = objDTO.SerialNumber;
                    obj.ExpirationDate = objDTO.ExpirationDate;
                    obj.Cost = objDTO.Cost;
                    obj.eVMISensorPort = objDTO.eVMISensorPort;
                    obj.eVMISensorID = objDTO.eVMISensorID;
                    obj.UDF1 = objDTO.UDF1;
                    obj.UDF2 = objDTO.UDF2;
                    obj.UDF3 = objDTO.UDF3;
                    obj.UDF4 = objDTO.UDF4;
                    obj.UDF5 = objDTO.UDF5;
                    obj.GUID = objDTO.GUID;
                    obj.ItemGUID = objDTO.ItemGUID;
                    obj.Created = objDTO.Created;
                    obj.Updated = objDTO.Updated;
                    obj.CreatedBy = objDTO.CreatedBy;
                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    //obj.IsDeleted = false;
                    //obj.IsArchived = false;
                    obj.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
                    obj.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
                    obj.CompanyID = objDTO.CompanyID;
                    obj.Room = objDTO.Room;
                    obj.OrderDetailGUID = objDTO.OrderDetailGUID;
                    obj.KitDetailGUID = objDTO.KitDetailGUID;
                    obj.TransferDetailGUID = objDTO.TransferDetailGUID;

                    if (!string.IsNullOrEmpty(objDTO.Expiration))
                    {
                        obj.Expiration = objDTO.Expiration.Replace("-", "/");
                    }
                    else if (objDTO.ExpirationDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                    {
                        obj.Expiration = objDTO.ExpirationDate.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy");
                    }


                    if (!string.IsNullOrEmpty(objDTO.Received))
                    {
                        obj.Received = objDTO.Received.Replace("-", "/");
                    }
                    else if (objDTO.ReceivedDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                    {
                        obj.Received = objDTO.ReceivedDate.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy");
                    }

                    obj.KitDetailGUID = objDTO.KitDetailGUID;
                    obj.TransferDetailGUID = objDTO.TransferDetailGUID;
                    obj.ReceivedDate = objDTO.ReceivedDate;


                    //if ((obj.InitialQuantityPDA ?? 0) < 1)
                    //{
                    //    obj.InitialQuantity = obj.InitialQuantity - objDTO.MoveQuantity;
                    //    obj.InitialQuantityWeb = obj.InitialQuantity;
                    //}


                    objDTO.InitialQuantity = obj.InitialQuantity;
                    objDTO.InitialQuantityWeb = obj.InitialQuantityWeb;
                    objDTO.AddedFrom = obj.AddedFrom == null ? "Web" : obj.AddedFrom;

                    objDTO.EditedFrom = obj.EditedFrom == null ? "Web" : obj.EditedFrom;
                    objDTO.ReceivedOn = obj.ReceivedOn;
                    objDTO.ReceivedOnWeb = obj.ReceivedOnWeb;

                    if (obj.ConsignedQuantity.GetValueOrDefault(0) > 0)
                        obj.IsConsignedSerialLot = true;

                    context.SaveChanges();



                    //if (oldCost != obj.Cost)
                    //{
                    //    ItemMaster oItemMaster = context.ItemMasters.Where(x => x.GUID == objDTO.ItemGUID && x.CompanyID == objDTO.CompanyID && x.Room == objDTO.Room).FirstOrDefault();
                    //    Room oRoom = context.Rooms.Where(x => x.ID == oItemMaster.Room && x.CompanyID == oItemMaster.CompanyID).FirstOrDefault();
                    //    if (oRoom.MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString() && !oItemMaster.Consignment)
                    //    {
                    //        new ItemMasterDAL(base.DataBaseName).UpdateItemAndPastReceiptCostByReceiveCost(oItemMaster.GUID, oItemMaster.Room.GetValueOrDefault(0), oItemMaster.CompanyID.GetValueOrDefault(0), false);
                    //    }
                    //    else if (oRoom.MethodOfValuingInventory == ((int)InventoryValuationMethod.LastCost).ToString() && !oItemMaster.Consignment)
                    //    {
                    //        new ItemMasterDAL(base.DataBaseName).UpdateItemAndPastReceiptCostByReceiveCost(oItemMaster.GUID, oItemMaster.Room.GetValueOrDefault(0), oItemMaster.CompanyID.GetValueOrDefault(0), true);
                    //    }
                    //}
                }
                //context.ItemLocationDetails.Attach(obj);
                //context.Entry(obj).State = System.Data.Entity.EntityState.Modified;



                //Get Cached-Media
                //IEnumerable<ItemLocationDetailsDTO> ObjCache = CacheHelper<IEnumerable<ItemLocationDetailsDTO>>.GetCacheItem("Cached_ItemLocationDetails_" + objDTO.CompanyID.ToString());
                //if (ObjCache != null)
                //{

                objDTO.SuggestedOrderQuantity = (new CartItemDAL(base.DataBaseName)).GetSuggestedOrderQtyForBin(objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.BinID.GetValueOrDefault(0));

                //List<ItemLocationDetailsDTO> objTemp = ObjCache.ToList();
                //objTemp.RemoveAll(i => i.ID == objDTO.ID);
                //ObjCache = objTemp.AsEnumerable();

                //List<ItemLocationDetailsDTO> tempC = new List<ItemLocationDetailsDTO>();
                //tempC.Add(objDTO);
                //IEnumerable<ItemLocationDetailsDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                //CacheHelper<IEnumerable<ItemLocationDetailsDTO>>.AppendToCacheItem("Cached_ItemLocationDetails_" + objDTO.CompanyID.ToString(), NewCache);
                //}


                if (objDTO.OrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty || objDTO.TransferDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                    new ReceivedOrderTransferDetailDAL(base.DataBaseName).Edit(objDTO);


                return true;
            }
        }

        public ItemLocationDetailsDTO EditItemLocationQuantity(ItemLocationDetailsDTO objDTO)
        {
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemLocationDetail objItemLocationDetail = context.ItemLocationDetails.FirstOrDefault(t => t.ID == objDTO.ID);
                if (objItemLocationDetail != null)
                {
                    double initialweb = 0;
                    if (context.PullDetails.Any(t => t.ItemLocationDetailGUID == objItemLocationDetail.GUID && t.IsDeleted == false))
                    {
                        initialweb = context.PullDetails.Where(t => t.ItemLocationDetailGUID == objItemLocationDetail.GUID && t.IsDeleted == false).Sum(t => ((t.CustomerOwnedQuantity ?? 0) + (t.ConsignedQuantity ?? 0)));
                    }
                    objItemLocationDetail.CustomerOwnedQuantity = 0;
                    objItemLocationDetail.ConsignedQuantity = 0;
                    objItemLocationDetail.InitialQuantity = initialweb;
                    objItemLocationDetail.InitialQuantityWeb = initialweb;
                    objItemLocationDetail.IsWebEdit = true;
                    objItemLocationDetail.LastUpdatedBy = objDTO.LastUpdatedBy;
                    objItemLocationDetail.Updated = DateTimeUtility.DateTimeNow;
                    objItemLocationDetail.EditedFrom = "Web";
                    objItemLocationDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.RefWebSelfGUID = objItemLocationDetail.GUID;
                    objDTO.AddedFrom = "Web";
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    context.SaveChanges();
                }
                objDTO.ID = 0;
                objDTO = InsertIntemLocationDetail(objDTO);


                //CacheHelper<IEnumerable<ItemLocationDetailsDTO>>.InvalidateCache();
                return objDTO;
            }
        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteRecords(string IDs, Int64 userid, Int64 RoomID, Int64 CompanyID, string WhatWhereAction, long SessionUserId)
        {
            CartItemDAL objCartItemDAL = new CartItemDAL(base.DataBaseName);
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<Guid> lstItemGUIDs = (from u in context.Database.SqlQuery<Guid>(@"CSP_DeleteLocationDataById '" + IDs + "', " + CompanyID.ToString() + ", " + RoomID.ToString() + ", " + userid + ", '" + WhatWhereAction + "'")
                                           select u).ToList();

                if (lstItemGUIDs != null && lstItemGUIDs.Count > 0)
                {
                    foreach (Guid ItemGuid in lstItemGUIDs)
                    {
                        objCartItemDAL.AutoCartUpdateByCode(ItemGuid, userid, "web", WhatWhereAction, SessionUserId);
                    }
                    return true;
                }
                else
                {
                    return false;
                }

                //string strQuery = "";
                //foreach (var item in IDs.Split(','))
                //{
                //    if (!string.IsNullOrEmpty(item.Trim()))
                //    {

                //        strQuery += "UPDATE ItemLocationDetails SET  Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1,ReceivedOn='" + DateTimeUtility.DateTimeNow + "',EditedFrom='Web' WHERE GUID ='" + item.ToString() + "';";
                //    }
                //}
                //if (context.Database.ExecuteSqlCommand(strQuery) > 0)
                //{
                //    #region "Cache Mgt"
                //    //Get Cached-Media
                //    IEnumerable<ItemLocationDetailsDTO> ObjCache = CacheHelper<IEnumerable<ItemLocationDetailsDTO>>.GetCacheItem("Cached_ItemLocationDetails_" + CompanyID.ToString());
                //    if (ObjCache != null)
                //    {
                //        List<ItemLocationDetailsDTO> objTemp = ObjCache.ToList();
                //        objTemp.RemoveAll(i => IDs.Split(',').Contains(i.GUID.ToString()));
                //        ObjCache = objTemp.AsEnumerable();
                //        CacheHelper<IEnumerable<ItemLocationDetailsDTO>>.AppendToCacheItem("Cached_ItemLocationDetails_" + CompanyID.ToString(), ObjCache);
                //    }

                //    #endregion

                //    Guid Itemguid = Guid.Empty;
                //    long BinID = 0;

                //    string[] ilids = IDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                //    if (ilids != null && ilids.Count() > 0)
                //    {
                //        foreach (var item in ilids)
                //        {
                //            Guid ItemlocdID = Guid.Empty;
                //            if (Guid.TryParse(item, out ItemlocdID))
                //            {
                //                ItemLocationDetail objItemLocationDetail = context.ItemLocationDetails.FirstOrDefault(t => t.GUID == ItemlocdID);
                //                if (objItemLocationDetail != null)
                //                {
                //                    Itemguid = objItemLocationDetail.ItemGUID ?? Guid.Empty;
                //                    BinID = objItemLocationDetail.BinID ?? 0;
                //                    break;
                //                }
                //            }
                //        }

                //        if (Itemguid != Guid.Empty && BinID > 0)
                //        {
                //            IQueryable<ItemLocationQTY> lstItemLocQty = context.ItemLocationQTies.Where(t => t.ItemGUID == Itemguid && t.BinID == BinID);
                //            if (lstItemLocQty.Any())
                //            {
                //                foreach (var item in lstItemLocQty)
                //                {
                //                    context.ItemLocationQTies.Remove(item);
                //                }
                //                ItemLocationQTY objItemLocationQTY = new ItemLocationQTY();
                //                objItemLocationQTY.AddedFrom = "web";
                //                objItemLocationQTY.BinID = BinID;
                //                objItemLocationQTY.CompanyID = CompanyID;
                //                objItemLocationQTY.ConsignedQuantity = context.ItemLocationDetails.Where(e => e.ItemGUID == Itemguid && e.BinID == BinID && (e.IsDeleted ?? false) == false).Any() ? context.ItemLocationDetails.Where(e => e.ItemGUID == Itemguid && e.BinID == BinID && (e.IsDeleted ?? false) == false).Sum(t => (t.ConsignedQuantity ?? 0)) : 0;
                //                objItemLocationQTY.Created = DateTime.UtcNow;
                //                objItemLocationQTY.CreatedBy = userid;
                //                objItemLocationQTY.CustomerOwnedQuantity = context.ItemLocationDetails.Where(e => e.ItemGUID == Itemguid && e.BinID == BinID && (e.IsDeleted ?? false) == false).Any() ? context.ItemLocationDetails.Where(e => e.ItemGUID == Itemguid && e.BinID == BinID && (e.IsDeleted ?? false) == false).Sum(t => (t.CustomerOwnedQuantity ?? 0)) : 0;
                //                objItemLocationQTY.EditedFrom = "web";
                //                objItemLocationQTY.GUID = Guid.NewGuid();
                //                objItemLocationQTY.ID = 0;
                //                objItemLocationQTY.ItemGUID = Itemguid;
                //                objItemLocationQTY.LastUpdated = DateTime.UtcNow;
                //                objItemLocationQTY.LastUpdatedBy = userid;
                //                objItemLocationQTY.LotNumber = string.Empty;
                //                objItemLocationQTY.Quantity = (objItemLocationQTY.CustomerOwnedQuantity ?? 0) + (objItemLocationQTY.ConsignedQuantity ?? 0);
                //                objItemLocationQTY.ReceivedOn = DateTime.UtcNow;
                //                objItemLocationQTY.ReceivedOnWeb = DateTime.UtcNow;
                //                objItemLocationQTY.Room = RoomID;
                //                context.ItemLocationQTies.Add(objItemLocationQTY);
                //            }
                //        }
                //        ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == Itemguid);
                //        if (objItem != null)
                //        {
                //            objItem.OnHandQuantity = context.ItemLocationDetails.Where(e => e.ItemGUID == Itemguid && (e.IsDeleted ?? false) == false).Any() ? context.ItemLocationDetails.Where(e => e.ItemGUID == Itemguid && (e.IsDeleted ?? false) == false).Sum(t => (t.ConsignedQuantity ?? 0)) + context.ItemLocationDetails.Where(e => e.ItemGUID == Itemguid && (e.IsDeleted ?? false) == false).Sum(t => (t.CustomerOwnedQuantity ?? 0)) : 0;
                //        }
                //        context.SaveChanges();
                //    }




                //    //foreach (var item in IDs.Split(','))
                //    //{
                //    //    if (!string.IsNullOrEmpty(item.Trim()))
                //    //    {
                //    //        ItemLocationDetailsDAL objDAL = new ItemLocationDetailsDAL(base.DataBaseName);
                //    //        ItemLocationDetailsDTO objDTO = objDAL.GetRecord(Guid.Parse(item), RoomID, CompanyID);

                //    //        if (objDTO != null)
                //    //        {
                //    //            ItemLocationQTYDAL objILQ = new ItemLocationQTYDAL(base.DataBaseName);
                //    //            ItemLocationQTYDTO objILQDTO = objILQ.GetRecordByBinItem(objDTO.ItemGUID.Value, Convert.ToInt64(objDTO.BinID), RoomID, CompanyID);

                //    //            if (objILQDTO != null)
                //    //            {

                //    //                objILQDTO.ConsignedQuantity = objILQDTO.ConsignedQuantity.GetValueOrDefault(0) - (objDTO.ConsignedQuantity.GetValueOrDefault(0));
                //    //                objILQDTO.CustomerOwnedQuantity = objILQDTO.CustomerOwnedQuantity.GetValueOrDefault(0) - (objDTO.CustomerOwnedQuantity.GetValueOrDefault(0));


                //    //                List<ItemLocationQTYDTO> lst = new List<ItemLocationQTYDTO>();
                //    //                lst.Add(objILQDTO);
                //    //                objILQ.Save(lst);
                //    //            }
                //    //        }
                //    //    }
                //    //}


                //}

                //return true;
            }
        }

        public List<ItemLocationDetailsDTO> GetItemQuantityByLocation(long BinId, Guid ItemGUID, long RoomId, long CompanyId)
        {
            List<ItemLocationDetailsDTO> lstItemBins = new List<ItemLocationDetailsDTO>();
            using (eTurnsEntities dbeTurnsEntities = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstItemBins = (from u in dbeTurnsEntities.ItemLocationDetails
                               where u.IsArchived == false && u.IsDeleted == false && u.BinID == BinId && u.ItemGUID == ItemGUID && u.Room == RoomId && u.CompanyID == CompanyId
                               select new ItemLocationDetailsDTO
                               {
                                   ID = u.ID,

                                   BinID = u.BinID,
                                   CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                   ConsignedQuantity = u.ConsignedQuantity,
                                   MeasurementID = u.MeasurementID,
                                   LotNumber = u.LotNumber,
                                   SerialNumber = u.SerialNumber,
                                   ExpirationDate = u.ExpirationDate,
                                   ReceivedDate = u.ReceivedDate,
                                   //Expiration = u.Expiration == null ? DateTime.Now.ToString("MM-dd-yy") : u.Expiration,
                                   //Received = u.Received == null ? DateTime.Now.ToString("MM-dd-yy") : u.Received,
                                   Cost = u.Cost,
                                   eVMISensorPort = u.eVMISensorPort,
                                   eVMISensorID = u.eVMISensorID,
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
                                   AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                                   EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                                   ReceivedOn = u.ReceivedOn == null ? DateTimeUtility.DateTimeNow : u.ReceivedOn,
                                   ReceivedOnWeb = u.ReceivedOnWeb == null ? DateTimeUtility.DateTimeNow : u.ReceivedOnWeb,
                               }).ToList();
            }

            return lstItemBins;
        }

        public ItemLocationQTYDTO GetItemQtyByLocation(long BinId, Guid ItemGUID, long RoomId, long CompanyId, long UserId)
        {
            ItemLocationQTYDTO lstItemBins = new ItemLocationQTYDTO();
            using (eTurnsEntities dbeTurnsEntities = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstItemBins = (from u in dbeTurnsEntities.ItemLocationDetails
                               join bm in dbeTurnsEntities.BinMasters on new { iid = u.ItemGUID, bid = u.BinID ?? 0 } equals new { iid = bm.ItemGUID, bid = bm.ID }
                               where u.IsArchived == false && u.IsDeleted == false && u.BinID == BinId && u.ItemGUID == ItemGUID && u.Room == RoomId && u.CompanyID == CompanyId
                               group u by new { u.ItemGUID, u.BinID, bm.BinNumber } into groupedItemLocation
                               select new ItemLocationQTYDTO
                               {
                                   Averagecost = 0,
                                   BinID = groupedItemLocation.Key.BinID ?? 0,
                                   BinNumber = groupedItemLocation.Key.BinNumber,
                                   CompanyID = CompanyId,
                                   ConsignedQuantity = groupedItemLocation.Sum(t => t.ConsignedQuantity ?? 0),
                                   Cost = null,
                                   CostUOMID = null,
                                   CostUOMName = null,
                                   Created = DateTime.Now,
                                   CreatedBy = UserId,
                                   CreatedByName = string.Empty,
                                   CriticalQuantity = 0,
                                   CustomerOwnedQuantity = groupedItemLocation.Sum(t => t.CustomerOwnedQuantity ?? 0),
                                   GUID = Guid.NewGuid(),
                                   ID = 0,
                                   ItemGUID = groupedItemLocation.Key.ItemGUID,
                                   ItemNumber = string.Empty,
                                   LastUpdated = DateTime.Now,
                                   LastUpdatedBy = UserId,
                                   LotNumber = string.Empty,
                                   Markup = 0,
                                   MaximumQuantity = 0,
                                   MinimumQuantity = 0,
                                   Quantity = groupedItemLocation.Sum(t => t.ConsignedQuantity ?? 0) + groupedItemLocation.Sum(t => t.ConsignedQuantity ?? 0),
                                   Room = RoomId,
                                   RoomName = string.Empty,
                                   SellPrice = 0,




                                   //ItemGUID = groupedItemLocation.Key.ItemGUID,
                                   //BinID = groupedItemLocation.Key.BinID ?? 0,
                                   //BinNumber = groupedItemLocation.Key.BinNumber,
                                   //CompanyID = CompanyId,
                                   //ConsignedQuantity = groupedItemLocation.Sum(t => t.ConsignedQuantity ?? 0),
                                   //CustomerOwnedQuantity = groupedItemLocation.Sum(t => t.CustomerOwnedQuantity ?? 0),
                                   //Quantity = groupedItemLocation.Sum(t => t.ConsignedQuantity ?? 0) + groupedItemLocation.Sum(t => t.ConsignedQuantity ?? 0)


                               }).FirstOrDefault();
            }

            return lstItemBins;
        }


        public List<ItemLocationDetailsDTO> GetItemsLocations(Guid ItemGUID, long RoomId, long CompanyId)
        {
            List<ItemLocationDetailsDTO> lstItemLocations = new List<ItemLocationDetailsDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstItemLocations = (from ild in context.ItemLocationDetails
                                    join bm in context.BinMasters on ild.BinID equals bm.ID
                                    where (ild.IsArchived ?? false) == false && (ild.IsDeleted ?? false) == false && ild.ItemGUID == ItemGUID && ild.Room == RoomId && ild.CompanyID == CompanyId
                                    group ild by new { ild.BinID, bm.BinNumber } into grouped_ild
                                    select new ItemLocationDetailsDTO
                                    {
                                        BinID = grouped_ild.Key.BinID,
                                        BinNumber = grouped_ild.Key.BinNumber,
                                        ConsignedQuantity = grouped_ild.Sum(t => (t.ConsignedQuantity ?? 0)),
                                        CustomerOwnedQuantity = grouped_ild.Sum(t => (t.CustomerOwnedQuantity ?? 0))
                                    }).ToList();
            }
            return lstItemLocations;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objDTO"></param>
        /// <param name="IsCreditPullNothing"></param> 1 = credit, 2 = pull, 3 = nothing
        /// <returns></returns>
        /// 
        public bool MoveQuanityToLocation(Int64 BinID, Guid ItemGuid, Int64 MoveBinID, double Quantity, Int64 RoomID, Int64 CompanyID, Int64 UserID, long SessionUserId)
        {
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            ItemMasterDTO ItemDTO = objItemDAL.GetItemWithoutJoins(null, ItemGuid);
            List<ItemLocationDetailsDTO> ObjItemLocation = null;
            List<ItemLocationDetailsDTO> ObjItemLocationCreditQty = new List<ItemLocationDetailsDTO>();
            ItemLocationDetailsDTO objLocDetCrediQtyDTO = null;
            ItemLocationQTYDTO lstLocDTO = new ItemLocationQTYDTO();
            ItemLocationQTYDAL objLocQTY = new ItemLocationQTYDAL(base.DataBaseName);
            BinMasterDAL objBINDAL = new BinMasterDAL(base.DataBaseName);
            BinMasterDTO objBINDTO = new BinMasterDTO();
            string ItemLocationMSG = string.Empty;
            PullMasterDAL objPull = new PullMasterDAL(base.DataBaseName);

            string MoveBinNumber = "";
            if (MoveBinID > 0)
            {
                BinMasterDTO tempObjBinDTO = objBINDAL.GetBinByID(MoveBinID, RoomID, CompanyID);
                //BinMasterDTO tempObjBinDTO = objBINDAL.GetItemLocation( RoomID, CompanyID, false, false,Guid.Empty, MoveBinID,null,null).FirstOrDefault();
                if (tempObjBinDTO != null)
                {
                    MoveBinNumber = tempObjBinDTO.BinNumber;
                }
                else
                {
                    MoveBinNumber = "";
                }
            }

            #region "LIFO FIFO"
            RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
            CommonDAL objCommonDAL = new CommonDAL(base.DataBaseName);
            RoomDTO objRoomDTO = new RoomDTO();
            string columnList = "ID,RoomName,InventoryConsuptionMethod";
            objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
            //objRoomDTO = objRoomDAL.GetRoomByIDPlain(RoomID);

            Boolean IsFIFO = false;
            if (objRoomDTO != null && objRoomDTO.ID > 0)
            {
                if (!string.IsNullOrEmpty(objRoomDTO.InventoryConsuptionMethod) && objRoomDTO.InventoryConsuptionMethod.ToLower() == "fifo")
                    IsFIFO = true;
                if (!string.IsNullOrEmpty(objRoomDTO.InventoryConsuptionMethod) && objRoomDTO.InventoryConsuptionMethod.ToLower() == "lifo")
                    IsFIFO = false;
            }
            else
            {
                IsFIFO = true;
            }

            if (ItemDTO.SerialNumberTracking)
            {
                ObjItemLocation = GetCustomerFirstThenConsigedByLIFOFIFO(IsFIFO, BinID, RoomID, CompanyID, ItemGuid, null).Take(int.Parse(Quantity.ToString())).ToList();
            }
            else
            {
                ObjItemLocation = GetCustomerFirstThenConsigedByLIFOFIFO(IsFIFO, BinID, RoomID, CompanyID, ItemGuid, null).ToList();
            }
            #endregion

            #region "Item Location & Quantity  Wise Deduction"

            #region "ItemLocation Deduction"

            if (ItemDTO.SerialNumberTracking)
            {
                #region "Serial logic"

                foreach (var itemoil in ObjItemLocation)
                {
                    objLocDetCrediQtyDTO = GetObjectFromItemLocationDTO(itemoil);
                    objLocDetCrediQtyDTO.ConsignedQuantity = 0;
                    objLocDetCrediQtyDTO.CustomerOwnedQuantity = 0;

                    if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0)
                    {
                        itemoil.ConsignedQuantity = 0;
                        objLocDetCrediQtyDTO.ConsignedQuantity = 1;
                    }
                    else
                    {
                        itemoil.CustomerOwnedQuantity = 0;
                        objLocDetCrediQtyDTO.CustomerOwnedQuantity = 1;
                    }

                    Edit(itemoil);
                    objLocDetCrediQtyDTO.BinID = MoveBinID;
                    objLocDetCrediQtyDTO.BinNumber = MoveBinNumber;
                    objLocDetCrediQtyDTO.ID = 0;
                    objLocDetCrediQtyDTO.GUID = Guid.Empty;
                    objLocDetCrediQtyDTO.Room = RoomID;
                    objLocDetCrediQtyDTO.CompanyID = CompanyID;
                    objLocDetCrediQtyDTO.CreatedBy = UserID;
                    objLocDetCrediQtyDTO.LastUpdatedBy = UserID;
                    objLocDetCrediQtyDTO.AddedFrom = "Web";
                    objLocDetCrediQtyDTO.EditedFrom = "Web";
                    objLocDetCrediQtyDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objLocDetCrediQtyDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    Insert(objLocDetCrediQtyDTO);
                    ObjItemLocationCreditQty.Add(objLocDetCrediQtyDTO);
                }

                #endregion
            }
            else
            {
                #region "LOt and other type logic"

                Double takenQunatity = 0;
                foreach (var itemoil in ObjItemLocation)
                {
                    objLocDetCrediQtyDTO = null;
                    if (takenQunatity == Quantity)
                    {
                        break;
                    }
                    objLocDetCrediQtyDTO = GetObjectFromItemLocationDTO(itemoil);
                    objLocDetCrediQtyDTO.CustomerOwnedQuantity = 0;
                    objLocDetCrediQtyDTO.ConsignedQuantity = 0;

                    #region "Consignment Credit and Pull"
                    if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) >= (Quantity - takenQunatity))
                    {
                        itemoil.CustomerOwnedQuantity = (Double)itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - (Quantity - takenQunatity);
                        objLocDetCrediQtyDTO.CustomerOwnedQuantity = Quantity - takenQunatity;
                        takenQunatity += Quantity - takenQunatity;
                    }
                    else if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0 && itemoil.ConsignedQuantity.GetValueOrDefault(0) >= (Quantity - takenQunatity))
                    {
                        itemoil.ConsignedQuantity = (Double)itemoil.ConsignedQuantity - (Quantity - takenQunatity);
                        objLocDetCrediQtyDTO.ConsignedQuantity = (Quantity - takenQunatity); ;
                        takenQunatity += (Quantity - takenQunatity);
                    }
                    else
                    {
                        takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                        objLocDetCrediQtyDTO.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                        itemoil.CustomerOwnedQuantity = 0;
                        if (itemoil.ConsignedQuantity >= (Quantity - takenQunatity))
                        {
                            itemoil.ConsignedQuantity = itemoil.ConsignedQuantity.GetValueOrDefault(0) - (Quantity - takenQunatity);
                            objLocDetCrediQtyDTO.ConsignedQuantity = Quantity - takenQunatity;
                            takenQunatity += Quantity - takenQunatity;
                        }
                        else
                        {
                            takenQunatity += itemoil.ConsignedQuantity.GetValueOrDefault(0);
                            objLocDetCrediQtyDTO.ConsignedQuantity = itemoil.ConsignedQuantity.GetValueOrDefault(0);
                            itemoil.ConsignedQuantity = 0;

                        }
                    }

                    #endregion

                    Edit(itemoil);
                    objLocDetCrediQtyDTO.BinID = MoveBinID;
                    objLocDetCrediQtyDTO.BinNumber = MoveBinNumber;
                    objLocDetCrediQtyDTO.ID = 0;
                    objLocDetCrediQtyDTO.GUID = Guid.Empty;
                    objLocDetCrediQtyDTO.Room = RoomID;
                    objLocDetCrediQtyDTO.CompanyID = CompanyID;
                    objLocDetCrediQtyDTO.CreatedBy = UserID;
                    objLocDetCrediQtyDTO.LastUpdatedBy = UserID;
                    objLocDetCrediQtyDTO.AddedFrom = "Web";
                    objLocDetCrediQtyDTO.EditedFrom = "Web";
                    objLocDetCrediQtyDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objLocDetCrediQtyDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    Insert(objLocDetCrediQtyDTO);
                    ObjItemLocationCreditQty.Add(objLocDetCrediQtyDTO);
                }
                #endregion
            }

            #endregion

            #region "ItemLocation Quantity Deduction"
            List<ItemLocationQTYDTO> lstUpdate = new List<ItemLocationQTYDTO>();
            lstLocDTO = objLocQTY.GetRecordByBinItem(ItemGuid, BinID, RoomID, CompanyID);
            ObjItemLocation = GetCustomerFirstThenConsigedByLIFOFIFO(IsFIFO, BinID, RoomID, CompanyID, ItemGuid, null).ToList();
            lstLocDTO.CustomerOwnedQuantity = ObjItemLocation.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0));
            lstLocDTO.ConsignedQuantity = ObjItemLocation.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0));
            lstLocDTO.Quantity = lstLocDTO.ConsignedQuantity.GetValueOrDefault(0) + lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0);
            lstLocDTO.LastUpdatedBy = UserID;
            lstUpdate.Add(lstLocDTO);

            lstLocDTO = objLocQTY.GetRecordByBinItem(ItemGuid, MoveBinID, RoomID, CompanyID);
            if (lstLocDTO == null)
            {
                lstLocDTO = new ItemLocationQTYDTO()
                {
                    BinID = MoveBinID,
                    CompanyID = CompanyID,
                    Room = RoomID,
                    ItemGUID = ItemGuid,
                    LastUpdatedBy = UserID,
                    CreatedBy = UserID,
                    CustomerOwnedQuantity = 0,
                    ConsignedQuantity = 0,
                    Quantity = 0,
                };


            }
            ObjItemLocation = GetCustomerFirstThenConsigedByLIFOFIFO(IsFIFO, MoveBinID, RoomID, CompanyID, ItemGuid, null).ToList();
            lstLocDTO.CustomerOwnedQuantity = ObjItemLocation.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0));
            lstLocDTO.ConsignedQuantity = ObjItemLocation.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0));
            lstLocDTO.Quantity = lstLocDTO.ConsignedQuantity.GetValueOrDefault(0) + lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0);
            lstLocDTO.LastUpdatedBy = UserID;
            lstUpdate.Add(lstLocDTO);

            objLocQTY.Save(lstUpdate, SessionUserId, 0);

            #endregion

            #endregion


            #region Insert Data ItemLocationLevelQuanity

            List<ItemLocationQTYDTO> objItemQty = new List<ItemLocationQTYDTO>();
            List<BinMasterDTO> objItemQuanity = new List<BinMasterDTO>();
            ItemLocationQTYDAL objDalItemQty = new ItemLocationQTYDAL(base.DataBaseName);
            BinMasterDAL objDalItemQuanity = new BinMasterDAL(base.DataBaseName);
            BinMasterDTO obj;

            objItemQty = objDalItemQty.GetRecordByItem(ItemGuid, RoomID, CompanyID).ToList();
            //objItemQuanity = objDalItemQuanity.GetAllRecordsItemWise(ItemGuid, RoomID, CompanyID).ToList();
            objItemQuanity = objDalItemQuanity.GetAllRecordsItemWise(ItemGuid, RoomID, CompanyID, null, null, string.Empty).ToList();
            //objItemQuanity = objDalItemQuanity.GetAllRecordsItemWise(ItemGuid, RoomID, CompanyID,null,null).ToList();

            foreach (var iTemp in objItemQty)
            {
                if (objItemQuanity.FindIndex(x => x.ID == iTemp.BinID) < 0)
                {
                    obj = new BinMasterDTO();
                    obj.ID = 0;
                    obj.CriticalQuantity = 0;
                    obj.MinimumQuantity = 0;
                    obj.MaximumQuantity = 0;
                    obj.ItemGUID = iTemp.ItemGUID;
                    obj.CreatedBy = iTemp.CreatedBy;
                    obj.Created = DateTimeUtility.DateTimeNow;
                    obj.LastUpdatedBy = iTemp.LastUpdatedBy;
                    obj.LastUpdated = DateTimeUtility.DateTimeNow;
                    obj.GUID = Guid.NewGuid();
                    obj.Room = iTemp.Room;
                    obj.CompanyID = iTemp.CompanyID;
                    obj.RoomName = iTemp.RoomName;
                    obj.UpdatedByName = iTemp.UpdatedByName;
                    obj.CreatedByName = iTemp.CreatedByName;
                    obj.AddedFrom = "Web";
                    obj.EditedFrom = "Web";
                    obj.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDalItemQuanity.Insert(obj);
                }
            }
            #endregion



            return false;
        }

        public ItemLocationDetailsDTO GetObjectFromItemLocationDTO(ItemLocationDetailsDTO itemoil)
        {
            Type type = itemoil.GetType();
            object toret = Activator.CreateInstance(itemoil.GetType());
            System.Reflection.FieldInfo[] fields = type.GetFields(System.Reflection.BindingFlags.Public |
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            foreach (System.Reflection.FieldInfo field in fields)
            {
                object fieldValue = field.GetValue(itemoil);
                if (fieldValue == null)
                    continue;
                field.SetValue(toret, fieldValue);
            }
            return (ItemLocationDetailsDTO)toret;

            //ItemLocationDetailsDTO objLocDetCrediQtyDTO = new ItemLocationDetailsDTO()
            //{
            //    Action = itemoil.Action,
            //    SerialNumberTracking = itemoil.SerialNumberTracking,
            //    BinID = itemoil.BinID,
            //    BinNumber = itemoil.BinNumber,
            //    CompanyID = itemoil.CompanyID,
            //    ConsignedQuantity = itemoil.ConsignedQuantity,
            //    Cost = itemoil.Cost,
            //    Created = DateTime.Now,
            //    CreatedBy = itemoil.CreatedBy,
            //    CreatedByName = itemoil.CreatedByName,
            //    CriticalQuantity = itemoil.CriticalQuantity,
            //    CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity,
            //    DateCodeTracking = itemoil.DateCodeTracking,
            //    eVMISensorID = itemoil.eVMISensorID,
            //    eVMISensorPort = itemoil.eVMISensorPort,
            //    Expiration = itemoil.Expiration,
            //    ExpirationDate = itemoil.ExpirationDate,
            //    GUID = Guid.Empty,
            //    HistoryID = 0,
            //    ID = 0,
            //    IsArchived = false,
            //    IsCreditPull = false,
            //    IsDeleted = false,
            //    ItemGUID = itemoil.ItemGUID,
            //    ItemNumber = itemoil.ItemNumber,
            //    ItemType = itemoil.ItemType,
            //    KitDetailGUID = null,
            //    LastUpdatedBy = itemoil.LastUpdatedBy,
            //    LotNumber = itemoil.LotNumber,
            //    LotNumberTracking = itemoil.LotNumberTracking,
            //    mode = itemoil.mode,
            //    MaximumQuantity = null,
            //    MeasurementID = null,
            //    MinimumQuantity = null,
            //    OrderDetailGUID = null,
            //    Received = itemoil.Received,
            //    ReceivedDate = itemoil.ReceivedDate,
            //    Room = itemoil.Room,
            //    RoomName = "",
            //    SerialNumber = itemoil.SerialNumber,
            //    SuggestedOrderQuantity = null,
            //    TransferDetailGUID = null,
            //    Updated = DateTime.Now,
            //    UpdatedByName = "",
            //    UDF1 = null,
            //    UDF2 = null,
            //    UDF3 = null,
            //    UDF4 = null,
            //    UDF5 = null,
            //    UDF6 = null,
            //    UDF7 = null,
            //    UDF8 = null,
            //    UDF9 = null,
            //    UDF10 = null,
            //};
            //return objLocDetCrediQtyDTO;
        }


        private double? CalculateAndGetPullCreditCost(double? itemDetailCost, long? costUOM, Int64 RoomID, Int64 CompanyID)
        {
            double? pullCost = null;
            int costUOMValue = 1;
            CostUOMMasterDTO oCostUOMMasterDTO = new CostUOMMasterDAL(base.DataBaseName).GetCostUOMByID(costUOM.GetValueOrDefault(0));
            if (oCostUOMMasterDTO != null)
            {
                costUOMValue = oCostUOMMasterDTO.CostUOMValue.GetValueOrDefault(0);
                if (costUOMValue == 0)
                    costUOMValue = 1;
            }

            if (itemDetailCost.HasValue)
                pullCost = itemDetailCost.Value / costUOMValue;

            return pullCost;
        }

        public bool CheckIsQuantityDifferences(Guid ItemGUID, long BinID, double CountConsignedQty, double CountCusOwnedQty)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                double ItemLocationConsignedQty = 0;
                double ItemLocationCoownedQty = 0;
                var q = context.ItemLocationDetails.Where(t => (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false && t.ItemGUID == ItemGUID && t.BinID == BinID);
                if (q.Any())
                {
                    ItemLocationConsignedQty = q.Sum(t => (t.ConsignedQuantity ?? 0));
                    ItemLocationCoownedQty = q.Sum(t => (t.CustomerOwnedQuantity ?? 0));
                }

                if (CountCusOwnedQty == ItemLocationCoownedQty && CountConsignedQty == ItemLocationConsignedQty)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }


        public void CopyItemLocationDetailsObjects(ItemLocationDetailsDTO fromDto, ItemLocationDetailsDTO toDto)
        {
            toDto.GUID = fromDto.GUID;
            toDto.Created = fromDto.Created;
            toDto.CreatedBy = fromDto.CreatedBy;
            toDto.IsConsignedSerialLot = fromDto.IsConsignedSerialLot;
            toDto.ItemGUID = fromDto.ItemGUID;
            toDto.ItemNumber = fromDto.ItemNumber;
            toDto.BinNumber = fromDto.BinNumber;
            toDto.BinID = fromDto.BinID;
            toDto.Received = fromDto.Received;
            if (fromDto.ReceivedDate.HasValue)
                toDto.ReceivedDate = fromDto.ReceivedDate;
            else
                toDto.ReceivedDate = DateTimeUtility.DateTimeNow.Date;
            toDto.IsCreditPull = fromDto.IsCreditPull;
            toDto.LotNumber = fromDto.LotNumber;
            toDto.SerialNumber = fromDto.SerialNumber;
            toDto.SerialNumberTracking = fromDto.SerialNumberTracking;
            toDto.LotNumberTracking = fromDto.LotNumberTracking;
            toDto.DateCodeTracking = fromDto.DateCodeTracking;
            toDto.Expiration = fromDto.Expiration;
            toDto.ExpirationDate = fromDto.ExpirationDate;
            toDto.CompanyID = fromDto.CompanyID;
            toDto.Room = fromDto.Room;
            toDto.Updated = fromDto.Updated;
            toDto.LastUpdatedBy = fromDto.LastUpdatedBy;
            toDto.IsArchived = fromDto.IsArchived;
            toDto.IsDeleted = fromDto.IsDeleted;
            toDto.UDF1 = fromDto.UDF1;
            toDto.UDF2 = fromDto.UDF2;
            toDto.UDF3 = fromDto.UDF3;
            toDto.UDF4 = fromDto.UDF4;
            toDto.UDF5 = fromDto.UDF5;
            toDto.IsOnlyFromUI = fromDto.IsOnlyFromUI;
            toDto.OrderDetailGUID = fromDto.OrderDetailGUID;
            toDto.KitDetailGUID = fromDto.KitDetailGUID;
            toDto.TransferDetailGUID = fromDto.TransferDetailGUID;
            toDto.ProjectSpentGUID = fromDto.ProjectSpentGUID;
            toDto.WorkOrderGUID = fromDto.WorkOrderGUID;
        }

        public void CopyItemLocationDetailsObjectsMS(MaterialStagingPullDetailDTO fromDto, MaterialStagingPullDetailDTO toDto)
        {
            toDto.GUID = fromDto.GUID;
            toDto.Created = fromDto.Created;
            toDto.CreatedBy = fromDto.CreatedBy;
            toDto.ItemGUID = fromDto.ItemGUID;
            toDto.ItemNumber = fromDto.ItemNumber;
            toDto.BinNumber = fromDto.BinNumber;
            toDto.BinID = fromDto.BinID;
            toDto.Received = fromDto.Received;
            toDto.LotNumber = fromDto.LotNumber;
            toDto.SerialNumber = fromDto.SerialNumber;
            toDto.SerialNumberTracking = fromDto.SerialNumberTracking;
            toDto.LotNumberTracking = fromDto.LotNumberTracking;
            toDto.DateCodeTracking = fromDto.DateCodeTracking;
            toDto.Expiration = fromDto.Expiration;
            toDto.CompanyID = fromDto.CompanyID;
            toDto.Room = fromDto.Room;
            toDto.Updated = fromDto.Updated;
            toDto.LastUpdatedBy = fromDto.LastUpdatedBy;
            toDto.IsArchived = fromDto.IsArchived;
            toDto.IsDeleted = fromDto.IsDeleted;
            toDto.UDF1 = fromDto.UDF1;
            toDto.UDF2 = fromDto.UDF2;
            toDto.UDF3 = fromDto.UDF3;
            toDto.UDF4 = fromDto.UDF4;
            toDto.UDF5 = fromDto.UDF5;
            toDto.OrderDetailGUID = fromDto.OrderDetailGUID;
            toDto.WorkOrderGuid = fromDto.WorkOrderGuid;
            toDto.MaterialStagingGUID = fromDto.MaterialStagingGUID;
            toDto.MaterialStagingdtlGUID = fromDto.MaterialStagingdtlGUID;
            toDto.PullCredit = fromDto.PullCredit;
            toDto.ItemLocationDetailGUID = fromDto.ItemLocationDetailGUID;
            toDto.StagingBinId = fromDto.StagingBinId;
            toDto.IsOnlyFromItemUI = fromDto.IsOnlyFromItemUI;
        }


        /// <summary>
        /// This method will be used for the Item Master, Item locations details, Quantity Updates....
        /// </summary>
        /// <param name="objData"></param>
        /// <returns></returns>
        public bool InsertItemLocationDetailsFromRecieve(List<ItemLocationDetailsDTO> objData, long SessionUserId, long EnterpriseId)
        {
            ReceivedOrderTransferDetailDAL objROTDDAL = new ReceivedOrderTransferDetailDAL(base.DataBaseName);
            IEnumerable<ReceivedOrderTransferDetailDTO> lst;
            OrderDetailsDTO OrdDetailDTO = null;
            OrderDetailsDAL ordDetailDAL = null;

            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemLocationQTYDAL objItemQtyDAL = null;
                #region "Location Detail Save"

                foreach (ItemLocationDetailsDTO item in objData)
                {
                    if (item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                        item.IsConsignedSerialLot = true;

                    item.InitialQuantity = item.ConsignedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0);
                    item.InitialQuantityWeb = item.InitialQuantity;

                    if (string.IsNullOrEmpty(item.AddedFrom))
                        item.AddedFrom = "UnKnown";

                    if (string.IsNullOrEmpty(item.EditedFrom))
                        item.EditedFrom = "UnKnown";

                    item.ReceivedOn = DateTimeUtility.DateTimeNow;
                    item.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    if (string.IsNullOrEmpty(item.Action))
                        item.Action = "Receive";

                    Insert(item);

                    #region "Insert QTY logic"
                    var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", item.ItemGUID.GetValueOrDefault(Guid.Empty)) };
                    var objilq = (from x in context.Database.SqlQuery<ItemLocationQTYDTO>("EXEC GetItemLocationDetailsByItemGuidPlain @ItemGUID", params1)
                                  select new ItemLocationQTYDTO
                                  {
                                      BinID = x.BinID,
                                      ItemGUID = x.ItemGUID,
                                      ConsignedQuantity = x.ConsignedQuantity,
                                      CustomerOwnedQuantity = x.CustomerOwnedQuantity,
                                      Quantity = x.Quantity,
                                      Room = x.Room,
                                      CompanyID = x.CompanyID,
                                      LastUpdated = DateTime.Now,
                                      Created = DateTime.Now,
                                      CreatedBy = objData[0].CreatedBy,
                                      LastUpdatedBy = objData[0].LastUpdatedBy,
                                  }).ToList();



                    objItemQtyDAL = new ItemLocationQTYDAL(base.DataBaseName);
                    objItemQtyDAL.Save(objilq, SessionUserId, EnterpriseId);

                    #endregion


                    ordDetailDAL = new OrderDetailsDAL(base.DataBaseName);
                    OrdDetailDTO = ordDetailDAL.GetOrderDetailByGuidFull(item.OrderDetailGUID.GetValueOrDefault(Guid.Empty), item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0));

                    lst = objROTDDAL.GetROTDByOrderDetailGUIDPlain(item.OrderDetailGUID.GetValueOrDefault(Guid.Empty), item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0)).OrderByDescending(x => x.ID).ToList();
                    double rcvQty = lst.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0));
                    rcvQty += lst.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0));

                    OrdDetailDTO.ReceivedQuantity = rcvQty;
                    OrdDetailDTO.IsOnlyFromUI = true;
                    OrdDetailDTO.EditedFrom = "Web";
                    OrdDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    ordDetailDAL.Edit(OrdDetailDTO, SessionUserId, EnterpriseId);


                }

                #endregion


            }
            return true;
        }

        public List<ItemLocationDetailsDTO> GetItemsLocationsSerLotQty(Guid ItemGUID, long BinID, string LotNumber, string SerialNumber, long RoomId, long CompanyId)
        {
            List<ItemLocationDetailsDTO> oItemLocations = new List<ItemLocationDetailsDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                oItemLocations = (from il in context.ItemLocationDetails
                                  where il.ItemGUID == ItemGUID && il.BinID == BinID && il.Room == RoomId && il.CompanyID == CompanyId
                                  && (il.LotNumber == LotNumber || LotNumber == string.Empty)
                                  && (il.SerialNumber == SerialNumber || SerialNumber == string.Empty)
                                  && (il.IsDeleted ?? false) == false && (il.IsArchived ?? false) == false
                                  && ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) > 0
                                  select new ItemLocationDetailsDTO
                                  {
                                      ID = il.ID,
                                      GUID = il.GUID,
                                      CustomerOwnedQuantity = il.CustomerOwnedQuantity,
                                      ConsignedQuantity = il.ConsignedQuantity,
                                      LotNumber = il.LotNumber,
                                      SerialNumber = il.SerialNumber,
                                      ExpirationDate = il.ExpirationDate,
                                      Expiration = il.Expiration
                                  }).ToList();
            }
            return oItemLocations;
        }

        public List<ItemLocationDetailsDTO> GetItemsLocationsDateCodeQty(Guid ItemGUID, long BinID, long RoomId, long CompanyId, DateTime ExpirationDate)
        {
            List<ItemLocationDetailsDTO> oItemLocations = new List<ItemLocationDetailsDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID),
                                                        new SqlParameter("@BinID", BinID),
                                                        new SqlParameter("@RoomID", RoomId),
                                                        new SqlParameter("@CompanyID", CompanyId ),
                                                        new SqlParameter("@ExpirationDate", ExpirationDate),
                                                        new SqlParameter("@IsStagingLocation", false) };
                oItemLocations = (from il in context.Database.SqlQuery<ItemLocationDetailsDTO>(@"exec GetItemsLocationsDateCodeQty @ItemGUID,@BinID,@RoomID,@CompanyID,@ExpirationDate,@IsStagingLocation", params1)
                                  select new ItemLocationDetailsDTO
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

        public List<ItemLocationDetailsDTO> GetItemsLocationsSerLotQtyForImport(Guid ItemGUID, long BinID, string LotNumber, string SerialNumber, long RoomId, long CompanyId, string ExpirationDate)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetItemsLocationsSerLotQtyForImport] @ItemGUID,@BinID,@CompanyId,@RoomId,@LotNumber,@SerailNumber,@ExpirationDate";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@ItemGUID", ItemGUID),
                    new SqlParameter("@BinID", BinID),
                    new SqlParameter("@CompanyId", CompanyId),
                     new SqlParameter("@RoomId", RoomId),
                      new SqlParameter("@LotNumber",(string.IsNullOrWhiteSpace(LotNumber) ? (object)DBNull.Value : LotNumber)),
                       new SqlParameter("@SerailNumber",(string.IsNullOrWhiteSpace(SerialNumber) ? (object)DBNull.Value : SerialNumber)),
                        new SqlParameter("@ExpirationDate",(string.IsNullOrWhiteSpace(ExpirationDate) ? (object)DBNull.Value : ExpirationDate))
                };
                return context.Database.SqlQuery<ItemLocationDetailsDTO>(stryQry, params1).ToList();

            }

            //List<ItemLocationDetailsDTO> oItemLocations = new List<ItemLocationDetailsDTO>();
            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{
            //    oItemLocations = (from il in context.ItemLocationDetails
            //                      where il.ItemGUID == ItemGUID && il.BinID == BinID && il.Room == RoomId && il.CompanyID == CompanyId
            //                      && (il.LotNumber == LotNumber || LotNumber == string.Empty)
            //                      && (il.SerialNumber == SerialNumber || SerialNumber == string.Empty)
            //                      && (il.Expiration == ExpirationDate || ExpirationDate == string.Empty)
            //                      && (il.IsDeleted ?? false) == false && (il.IsArchived ?? false) == false
            //                      && ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) > 0
            //                      select new ItemLocationDetailsDTO
            //                      {
            //                          ID = il.ID,
            //                          GUID = il.GUID,
            //                          CustomerOwnedQuantity = il.CustomerOwnedQuantity,
            //                          ConsignedQuantity = il.ConsignedQuantity,
            //                          LotNumber = il.LotNumber,
            //                          SerialNumber = il.SerialNumber,
            //                          Expiration = il.Expiration
            //                      }).ToList();
            //}
            //return oItemLocations;
        }

        public List<ItemLocationDetailsDTO> GetItemsLocationsSerLotQtyForMultiImport(Guid ItemGUID, long BinID, string LotNumber, string SerialNumber, long RoomId, long CompanyId, string ExpirationDate)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetItemsLocationsSerLotQtyForMultiImport] @ItemGUID,@BinID,@CompanyId,@RoomId,@LotNumber,@SerailNumber,@ExpirationDate";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@ItemGUID", ItemGUID),
                    new SqlParameter("@BinID", BinID),
                    new SqlParameter("@CompanyId", CompanyId),
                     new SqlParameter("@RoomId", RoomId),
                      new SqlParameter("@LotNumber",(string.IsNullOrWhiteSpace(LotNumber) ? (object)DBNull.Value : LotNumber)),
                       new SqlParameter("@SerailNumber",(string.IsNullOrWhiteSpace(SerialNumber) ? (object)DBNull.Value : SerialNumber)),
                        new SqlParameter("@ExpirationDate",(string.IsNullOrWhiteSpace(ExpirationDate) ? (object)DBNull.Value : ExpirationDate))
                };
                return context.Database.SqlQuery<ItemLocationDetailsDTO>(stryQry, params1).ToList();

            }
        }

        public List<ItemLocationLotSerialDTO> GetItemLocationsWithLotSerialsForMove(Guid ItemGUID, long RoomId, long CompanyId)
        {
            List<ItemLocationLotSerialDTO> lstItemLocations = new List<ItemLocationLotSerialDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstItemLocations = (from il in context.ItemLocationDetails
                                    join im in context.ItemMasters on il.ItemGUID equals im.GUID
                                    join bm in context.BinMasters on il.BinID equals bm.ID
                                    where il.ItemGUID == ItemGUID && il.Room == RoomId && il.CompanyID == CompanyId
                                    && (((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) > 0)
                                    && (il.IsDeleted ?? false) == false && (il.IsArchived ?? false) == false
                                    && bm.IsDeleted == false
                                    group new
                                    {
                                        il.ItemGUID,
                                        bm.BinNumber,
                                        il.BinID,
                                        il.SerialNumber,
                                        il.LotNumber,
                                        il.ExpirationDate,
                                        im.LotNumberTracking,
                                        im.SerialNumberTracking,
                                        im.DateCodeTracking,
                                        il.ConsignedQuantity,
                                        il.CustomerOwnedQuantity
                                    } by new
                                    {
                                        il.ItemGUID,
                                        bm.BinNumber,
                                        il.BinID,
                                        SerialNumber = il.SerialNumber == null ? "" : il.SerialNumber,
                                        LotNumber = il.LotNumber == null ? "" : il.LotNumber,
                                        ExpirationDate = il.ExpirationDate == null ? DateTime.MinValue : il.ExpirationDate,
                                        im.LotNumberTracking,
                                        im.SerialNumberTracking,
                                        im.DateCodeTracking
                                    } into GroupLocation
                                    select new ItemLocationLotSerialDTO
                                    {
                                        ItemGUID = GroupLocation.Key.ItemGUID,
                                        BinNumber = GroupLocation.Key.BinNumber,
                                        BinID = GroupLocation.Key.BinID,
                                        ID = GroupLocation.Key.BinID.Value,
                                        ConsignedQuantity = GroupLocation.Sum(x => x.ConsignedQuantity ?? 0),
                                        CustomerOwnedQuantity = GroupLocation.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                        SerialNumber = GroupLocation.Key.SerialNumber,
                                        LotNumber = GroupLocation.Key.LotNumber,
                                        ExpirationDate = GroupLocation.Key.ExpirationDate,
                                        LotNumberTracking = GroupLocation.Key.LotNumberTracking,
                                        SerialNumberTracking = GroupLocation.Key.SerialNumberTracking,
                                        DateCodeTracking = GroupLocation.Key.DateCodeTracking,
                                        LotSerialQuantity = (GroupLocation.Sum(x => x.CustomerOwnedQuantity ?? 0)) + (GroupLocation.Sum(x => x.ConsignedQuantity ?? 0)),
                                        QuantityToMove = (GroupLocation.Sum(x => x.CustomerOwnedQuantity ?? 0)) + (GroupLocation.Sum(x => x.ConsignedQuantity ?? 0)),
                                        LotOrSerailNumber = GroupLocation.Key.LotNumberTracking ? GroupLocation.Key.LotNumber : GroupLocation.Key.SerialNumberTracking ? GroupLocation.Key.SerialNumber : string.Empty
                                    }).ToList();
            }

            //strExpirationDate = GroupLocation.Key.DateCodeTracking ? (SqlFunctions.StringConvert((double)GroupLocation.Key.ExpirationDate.Value.Month).TrimStart() + "/" + SqlFunctions.StringConvert((double)GroupLocation.Key.ExpirationDate.Value.Day).TrimStart() + "/" + SqlFunctions.StringConvert((double)GroupLocation.Key.ExpirationDate.Value.Year).TrimStart()) : string.Empty,


            //                            SerialLotExpirationcombin = GroupLocation.Key.SerialNumberTracking &&  GroupLocation.Key.DateCodeTracking ?
            //                                                                (GroupLocation.Key.SerialNumber + "_" + (SqlFunctions.StringConvert((double)GroupLocation.Key.ExpirationDate.Value.Month).TrimStart() + "/" + SqlFunctions.StringConvert((double)GroupLocation.Key.ExpirationDate.Value.Day).TrimStart() + "/" + SqlFunctions.StringConvert((double)GroupLocation.Key.ExpirationDate.Value.Year).TrimStart()))
            //                                                                :
            //                                                                (GroupLocation.Key.LotNumberTracking &&  GroupLocation.Key.DateCodeTracking ?
            //                                                                (GroupLocation.Key.LotNumber + "_" + (SqlFunctions.StringConvert((double)GroupLocation.Key.ExpirationDate.Value.Month).TrimStart() + "/" + SqlFunctions.StringConvert((double)GroupLocation.Key.ExpirationDate.Value.Day).TrimStart() + "/" + SqlFunctions.StringConvert((double)GroupLocation.Key.ExpirationDate.Value.Year).TrimStart()))
            //                                                                        : (SqlFunctions.StringConvert((double)GroupLocation.Key.ExpirationDate.Value.Month).TrimStart() + "/" + SqlFunctions.StringConvert((double)GroupLocation.Key.ExpirationDate.Value.Day).TrimStart() + "/" + SqlFunctions.StringConvert((double)GroupLocation.Key.ExpirationDate.Value.Year).TrimStart()))

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

        public IEnumerable<ItemLocationDetailsDTO> GetCustomerFirstThenConsigedByLIFOFIFOForLotSr(bool IsFifo, Int64 BinID, Int64 RoomID, Int64 CompanyId, Guid ItemGUID, Guid? OrderDetailGUID, string LotNumber, string SerialNumber)
        {
            string sOrderBy = "";
            if (IsFifo)
            {
                sOrderBy = "ReceivedDate ASC";
            }
            else
            {
                sOrderBy = "ReceivedDate DESC";
            }

            IEnumerable<ItemLocationDetailsDTO> result = null;
            if (OrderDetailGUID == null)
            {
                //first customer 
                //result = GetCachedData(ItemGUID, RoomID, CompanyId).Where(t => (t.ItemGUID == ItemGUID && t.BinID == BinID && (t.CustomerOwnedQuantity.GetValueOrDefault(0) > 0 && t.ConsignedQuantity.GetValueOrDefault(0) == 0) && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(sOrderBy);
                result = GetCachedDataeVMI(ItemGUID, RoomID, CompanyId).Where(t => (t.ItemGUID == ItemGUID && t.BinID == BinID
                    && (t.SerialNumber == SerialNumber || string.IsNullOrWhiteSpace(SerialNumber)) && (t.LotNumber == LotNumber || string.IsNullOrWhiteSpace(LotNumber))
                    && (t.CustomerOwnedQuantity.GetValueOrDefault(0) > 0) && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(sOrderBy);

                //then consigned
                IEnumerable<ItemLocationDetailsDTO> resultcons = null;
                //resultcons = GetCachedData(ItemGUID, RoomID, CompanyId).Where(t => (t.ItemGUID == ItemGUID && t.BinID == BinID && (t.CustomerOwnedQuantity.GetValueOrDefault(0) == 0 && t.ConsignedQuantity.GetValueOrDefault(0) > 0) && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(sOrderBy);
                resultcons = GetCachedDataeVMI(ItemGUID, RoomID, CompanyId).Where(t => (t.ItemGUID == ItemGUID && t.BinID == BinID
                    && (t.SerialNumber == SerialNumber || string.IsNullOrWhiteSpace(SerialNumber)) && (t.LotNumber == LotNumber || string.IsNullOrWhiteSpace(LotNumber))
                    && (t.ConsignedQuantity.GetValueOrDefault(0) > 0) && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(sOrderBy);

                List<ItemLocationDetailsDTO> FinalResult = new List<ItemLocationDetailsDTO>();
                foreach (var item in result)
                {
                    FinalResult.Add(item);
                }
                foreach (var item in resultcons)
                {
                    if (FinalResult.Where(x => x.GUID == item.GUID).Count() <= 0)
                    {
                        FinalResult.Add(item);
                    }
                }
                return FinalResult;
                //return result.Concat(resultcons);
            }
            else
            {
                //first customer
                //result = GetCachedData(ItemGUID, RoomID, CompanyId).Where(t => (t.ItemGUID == ItemGUID && t.BinID == BinID && (t.CustomerOwnedQuantity.GetValueOrDefault(0) > 0 && t.ConsignedQuantity.GetValueOrDefault(0) == 0) && t.OrderDetailGUID == OrderDetailGUID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(sOrderBy);
                result = GetCachedDataeVMI(ItemGUID, RoomID, CompanyId).Where(t => (t.ItemGUID == ItemGUID && t.BinID == BinID
                    && (t.SerialNumber == SerialNumber || string.IsNullOrWhiteSpace(SerialNumber)) && (t.LotNumber == LotNumber || string.IsNullOrWhiteSpace(LotNumber))
                    && (t.CustomerOwnedQuantity.GetValueOrDefault(0) > 0) && t.OrderDetailGUID == OrderDetailGUID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(sOrderBy);

                //then consigned
                IEnumerable<ItemLocationDetailsDTO> resultcons = null;
                //resultcons = GetCachedData(ItemGUID, RoomID, CompanyId).Where(t => (t.ItemGUID == ItemGUID && t.BinID == BinID && (t.CustomerOwnedQuantity.GetValueOrDefault(0) == 0 && t.ConsignedQuantity.GetValueOrDefault(0) > 0) && t.OrderDetailGUID == OrderDetailGUID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(sOrderBy);
                resultcons = GetCachedDataeVMI(ItemGUID, RoomID, CompanyId).Where(t => (t.ItemGUID == ItemGUID && t.BinID == BinID
                    && (t.SerialNumber == SerialNumber || string.IsNullOrWhiteSpace(SerialNumber)) && (t.LotNumber == LotNumber || string.IsNullOrWhiteSpace(LotNumber))
                    && (t.ConsignedQuantity.GetValueOrDefault(0) > 0) && t.OrderDetailGUID == OrderDetailGUID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(sOrderBy);

                List<ItemLocationDetailsDTO> FinalResult = new List<ItemLocationDetailsDTO>();
                foreach (var item in result)
                {
                    FinalResult.Add(item);
                }
                foreach (var item in resultcons)
                {
                    if (FinalResult.Where(x => x.GUID == item.GUID).Count() <= 0)
                    {
                        FinalResult.Add(item);
                    }
                }
                return FinalResult;

                //return result.Concat(resultcons);
            }

            //return null;
        }
        public bool CheckBinForDelete(string binNumber, Guid ItemGuid, long roomId, long companyid)
        {
            bool result = false;
            try
            {
                ItemLocationDetailsDTO objItemLocationDetailsDTO = new ItemLocationDetailsDTO();
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    double? total = (from u in context.ItemLocationDetails
                                     join b in context.BinMasters on u.BinID equals b.ID
                                     join im in context.ItemMasters on b.ItemGUID equals im.GUID
                                     where b.IsDeleted == false && b.BinNumber == binNumber && b.Room == roomId && b.CompanyID == companyid
                                     && u.CompanyID == companyid && u.Room == roomId
                                     && im.Room == roomId && im.CompanyID == companyid && im.IsDeleted == false && im.IsArchived == false
                                     && im.GUID == ItemGuid && u.ItemGUID == ItemGuid && b.ItemGUID == ItemGuid
                                     select new ItemLocationDetailsDTO()
                                     {
                                         CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                         ConsignedQuantity = u.ConsignedQuantity,

                                     }).ToList().Sum(c => c.ConsignedQuantity.GetValueOrDefault(0) + c.CustomerOwnedQuantity.GetValueOrDefault(0));
                    if ((total ?? 0) <= 0)
                    {
                        result = true;
                    }

                }
            }
            catch
            {
                return result;
            }
            return result;
        }
        public List<ItemLocationDetailsDTO> ValidateILQRecords(List<InventoryLocationMain> lstItemLocationDetails, long RoomID, long CompanyID, long UserID, string RoomDateFormat, long EnterpriseID
            , CultureInfo CurrentCult, out string MsgCreditTransactionForSerialNumber, out string MsgPullQtyIsntEnoughForCredit, bool isUDFRequired = true, bool isCountPullValidate = false)
        {
            List<ItemLocationDetailsDTO> lstReturnList = new List<ItemLocationDetailsDTO>();
            ItemMasterDTO objItem = new ItemMasterDTO();
            BinMasterDTO objBinMasterDTO = new BinMasterDTO();
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
            eTurns.DAL.UDFDAL objUDFDAL = new eTurns.DAL.UDFDAL(base.DataBaseName);
            CommonDAL cmnDAL = new CommonDAL(base.DataBaseName);
            IEnumerable<UDFDTO> UDFDataFromDB = null;
            if (isUDFRequired == true)
                UDFDataFromDB = objUDFDAL.GetRequiredUDFsByUDFTableNamePlain("InventoryCount", RoomID, CompanyID);

            //RoomDTO objRoomDTO = new RoomDAL(base.DataBaseName).GetRoomByIDPlain(RoomID);
            string columnList = "ID,RoomName,IsIgnoreCreditRule";
            RoomDTO objRoomDTO = new CommonDAL(base.DataBaseName).GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
            string CultureCode = "en-US";

            if (CurrentCult != null)
            {
                CultureCode = CurrentCult.Name;
            }

            var inventoryCountResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResInventoryCount", CultureCode, EnterpriseID, CompanyID);
            var pullMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResPullMaster", CultureCode, EnterpriseID, CompanyID);
            string msgCreditTransactionForSerialNumber = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgCreditTransactionForSerialNumber", pullMasterResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResPullMaster", CultureCode);
            string msgPullQtyIsntEnoughForCredit = ResourceRead.GetResourceValueByKeyAndFullFilePath("PullQtyIsntEnoughForCredit", inventoryCountResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResInventoryCount", CultureCode);
            MsgCreditTransactionForSerialNumber = msgCreditTransactionForSerialNumber;
            MsgPullQtyIsntEnoughForCredit = msgPullQtyIsntEnoughForCredit;

            if (lstItemLocationDetails != null && lstItemLocationDetails.Count > 0)
            {
                var receiveOrderDetailsResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResReceiveOrderDetails", CultureCode, EnterpriseID, CompanyID);
                var toolMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResToolMaster", CultureCode, EnterpriseID, CompanyID);

                string msgEnterItemNumber = ResourceRead.GetResourceValueByKeyAndFullFilePath("EnterItemNumber", inventoryCountResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResInventoryCount", CultureCode);
                string msgItemDoesNotExistsInRoom = ResourceRead.GetResourceValueByKeyAndFullFilePath("ItemDoesNotExistsInRoom", inventoryCountResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResInventoryCount", CultureCode);
                string msgLotNumberCantBeBlank = ResourceRead.GetResourceValueByKeyAndFullFilePath("LotNumberCantBeBlank", inventoryCountResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResInventoryCount", CultureCode);
                string msgEnterReceiveDateFormat = ResourceRead.GetResourceValueByKeyAndFullFilePath("EnterReceiveDateFormat", inventoryCountResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResInventoryCount", CultureCode);
                string msgExpirationDateCantBeEmpty = ResourceRead.GetResourceValueByKeyAndFullFilePath("ExpirationDateCantBeEmpty", inventoryCountResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResInventoryCount", CultureCode);
                string msgEnterExpiryDateFormat = ResourceRead.GetResourceValueByKeyAndFullFilePath("EnterExpiryDateFormat", inventoryCountResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResInventoryCount", CultureCode);
                string msgCountCustOrConsQtyShouldBeGreaterEqualsZero = ResourceRead.GetResourceValueByKeyAndFullFilePath("CountCustOrConsQtyShouldBeGreaterEqualsZero", inventoryCountResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResInventoryCount", CultureCode);
                string msgSameLotDifferentExpirationNotAllowed = ResourceRead.GetResourceValueByKeyAndFullFilePath("SameLotDifferentExpirationNotAllowed", inventoryCountResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResInventoryCount", CultureCode);
                string msgBinNumberValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgBinNumberValidation", receiveOrderDetailsResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResReceiveOrderDetails", CultureCode);
                string msgSerialNoCantBeBlank = ResourceRead.GetResourceValueByKeyAndFullFilePath("SerialNoCantBeBlank", toolMasterResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResToolMaster", CultureCode);

                foreach (var Locitem in lstItemLocationDetails)
                {
                    ItemLocationDetailsDTO Locitemmain = new ItemLocationDetailsDTO();
                    Locitemmain.BinNumber = Locitem.BinNumber;
                    Locitemmain.CompanyID = CompanyID;
                    Locitemmain.ConsignedQuantity = Locitem.consignedquantity;
                    Locitemmain.Cost = Locitem.Cost;
                    Locitemmain.Created = DateTime.UtcNow;
                    Locitemmain.CreatedBy = UserID;
                    Locitemmain.CustomerOwnedQuantity = Locitem.customerownedquantity;
                    //Locitemmain.Expiration = Locitem.Expiration;
                    Locitemmain.GUID = Locitem.GUID;
                    Locitemmain.ID = Locitem.ID;
                    Locitemmain.InsertedFrom = Locitem.InsertedFrom;
                    Locitemmain.IsArchived = Locitem.IsArchived;
                    Locitemmain.IsDeleted = Locitem.IsDeleted;
                    Locitemmain.ItemGUID = Locitem.ItemGUID;
                    Locitemmain.ItemNumber = Locitem.ItemNumber;
                    Locitemmain.LastUpdatedBy = Locitem.LastUpdatedBy;
                    Locitemmain.LotNumber = (!string.IsNullOrWhiteSpace(Locitem.LotNumber)) ? Locitem.LotNumber.Trim() : string.Empty;
                    Locitemmain.Received = Locitem.Received;
                    Locitemmain.Room = RoomID;
                    Locitemmain.SerialNumber = (!string.IsNullOrWhiteSpace(Locitem.SerialNumber)) ? Locitem.SerialNumber.Trim() : string.Empty;
                    Locitemmain.Updated = DateTime.UtcNow;

                    Locitemmain.ProjectSpend = Locitem.ProjectSpend;

                    string Errormessage = string.Empty;

                    if (string.IsNullOrWhiteSpace(Locitem.ItemNumber))
                    {
                        Errormessage += ";" + msgEnterItemNumber;
                    }
                    if (string.IsNullOrWhiteSpace(Locitem.BinNumber))
                    {
                        Errormessage += ";" + msgBinNumberValidation;
                    }
                    if ((Locitem.ItemNumber != objItem.ItemNumber) && string.IsNullOrWhiteSpace(Errormessage))
                    {
                        objItem = objItemMasterDAL.GetItemByItemNumberPlain(Locitem.ItemNumber, RoomID, CompanyID);
                        if (objItem == null)
                        {
                            objItem = new ItemMasterDTO();
                        }
                        //Locitemmain.ItemNumber = objItem.ItemNumber;
                        Locitemmain.ItemGUID = objItem.GUID;
                    }
                    else
                    {
                        Locitemmain.ItemNumber = objItem.ItemNumber;
                        Locitemmain.ItemGUID = objItem.GUID;
                        if (objItem == null)
                        {
                            objItem = new ItemMasterDTO();
                        }
                    }
                    if (objItem.ID < 1)
                    {
                        Errormessage += ";" + msgItemDoesNotExistsInRoom;
                    }
                    if (objItem.ID > 0)
                    {
                        if (objItem.SerialNumberTracking || objItem.LotNumberTracking)
                        {
                            //Errormessage += ";Serial and lot import will be available soon,The work is in progress";
                        }
                        if (objItem.Consignment)
                        {
                            Locitemmain.Cost = objItem.Cost.GetValueOrDefault(0);
                        }
                        else if (Locitem.Cost == null)
                        {
                            Locitemmain.Cost = objItem.Cost.GetValueOrDefault(0);
                        }
                    }
                    if (string.IsNullOrWhiteSpace(Errormessage) && objItem.ID > 0)
                    {
                        objBinMasterDTO = GetItemBin(objItem.GUID, Locitem.BinNumber, RoomID, CompanyID, UserID, false);
                        if (objBinMasterDTO != null)
                        {
                            Locitemmain.BinNumber = objBinMasterDTO.BinNumber;
                            Locitemmain.BinID = objBinMasterDTO.ID;
                        }
                    }
                    if (string.IsNullOrWhiteSpace(Errormessage) && objItem.LotNumberTracking)
                    {
                        if (string.IsNullOrWhiteSpace(Locitem.LotNumber))
                        {
                            Errormessage += ";" + msgLotNumberCantBeBlank;
                        }
                        Locitemmain.SerialNumber = null;
                    }


                    if (!string.IsNullOrEmpty(Locitem.Received))
                    {
                        DateTime expiryDate = DateTime.MinValue;
                        DateTime.TryParseExact(Locitem.Received, RoomDateFormat, CurrentCult, DateTimeStyles.None, out expiryDate);
                        if (expiryDate != DateTime.MinValue)
                        {
                            Locitemmain.ReceivedDate = expiryDate;
                        }
                        else
                        {
                            Locitemmain.ReceivedDate = null;
                            Errormessage += ";" + string.Format(msgEnterReceiveDateFormat, RoomDateFormat);
                        }
                    }
                    if (string.IsNullOrWhiteSpace(Errormessage) && objItem.SerialNumberTracking)
                    {
                        if (string.IsNullOrWhiteSpace(Locitem.SerialNumber))
                        {
                            Errormessage += ";" + msgSerialNoCantBeBlank;
                        }
                        Locitemmain.LotNumber = null;
                    }
                    if (string.IsNullOrWhiteSpace(Errormessage) && objItem.DateCodeTracking)
                    {
                        if (string.IsNullOrWhiteSpace(Locitem.Expiration))
                        {
                            Errormessage += ";" + msgExpirationDateCantBeEmpty;
                        }
                        else
                        {
                            DateTime expiryDate = DateTime.MinValue;
                            DateTime.TryParseExact(Locitem.Expiration, RoomDateFormat, ResourceHelper.CurrentCult, DateTimeStyles.None, out expiryDate);
                            if (expiryDate != DateTime.MinValue)
                            {
                                Locitemmain.ExpirationDate = expiryDate;
                                Locitemmain.Expiration = expiryDate.ToString("MM/dd/yyyy");
                            }
                            else
                            {
                                Locitemmain.ExpirationDate = null;
                                Errormessage += ";" + string.Format(msgEnterExpiryDateFormat, RoomDateFormat);
                            }
                        }
                    }
                    if (objItem.ID > 0 && !objItem.Consignment)
                    {
                        Locitemmain.ConsignedQuantity = 0;
                    }

                    if (string.IsNullOrWhiteSpace(Errormessage) && ((Locitemmain.CustomerOwnedQuantity ?? 0) < 0 || (Locitemmain.ConsignedQuantity ?? 0) < 0))
                    {
                        Errormessage += ";" + msgCountCustOrConsQtyShouldBeGreaterEqualsZero;
                    }

                    if (objItem.DateCodeTracking && objItem.LotNumberTracking)
                    {
                        if (Locitemmain.ExpirationDate.GetValueOrDefault(DateTime.MinValue) != DateTime.MinValue && !string.IsNullOrWhiteSpace(Locitemmain.LotNumber))
                        {
                            string msg = cmnDAL.CheckDuplicateLotAndExpiration(Locitemmain.LotNumber, Locitemmain.Expiration, Locitemmain.ExpirationDate.GetValueOrDefault(DateTime.MinValue), 0, RoomID, CompanyID, objItem.GUID, UserID, EnterpriseID);
                            if (string.IsNullOrWhiteSpace(msg) || (msg ?? string.Empty).ToLower() == "ok")
                            {
                                if (lstReturnList != null && lstReturnList.Count > 0)
                                {
                                    var isExists = lstReturnList.Where(x => (x.Expiration ?? string.Empty) != (Locitemmain.Expiration ?? string.Empty) && (x.LotNumber ?? string.Empty).ToLower() == (Locitemmain.LotNumber ?? string.Empty).ToLower()).FirstOrDefault();
                                    if (isExists != null)
                                    {
                                        Errormessage += ";" + msgSameLotDifferentExpirationNotAllowed;
                                    }
                                }
                            }
                            else
                            {
                                Errormessage += ";" + msg;
                            }
                        }
                    }


                    if (isCountPullValidate == true)
                    {
                        //// check for 'credit' 

                        IEnumerable<ItemLocationDetailsDTO> lstItemLocationDetail = null;
                        lstItemLocationDetail = new ItemLocationDetailsDAL(base.DataBaseName).GetCountDifferenceforValidatPull(objItem.GUID, objItem.Room.GetValueOrDefault(0), objItem.CompanyID.GetValueOrDefault(0), Locitemmain.BinID.GetValueOrDefault(0));

                        if (objItem.SerialNumberTracking && !string.IsNullOrEmpty(Locitem.SerialNumber) && objItem.DateCodeTracking && Locitemmain.ExpirationDate != null && Locitemmain.ExpirationDate != DateTime.MinValue)
                        {
                            lstItemLocationDetail = lstItemLocationDetail.Where(x => x.SerialNumber.Equals(Locitem.SerialNumber) && x.ExpirationDate.Value.ToShortDateString().Equals(Locitemmain.ExpirationDate.Value.ToShortDateString())).ToList();
                        }
                        else if (objItem.LotNumberTracking && !string.IsNullOrEmpty(Locitem.LotNumber) && objItem.DateCodeTracking && Locitemmain.ExpirationDate != null && Locitemmain.ExpirationDate != DateTime.MinValue)
                        {
                            lstItemLocationDetail = lstItemLocationDetail.Where(x => x.LotNumber.Equals(Locitem.LotNumber) && x.ExpirationDate.Value.ToShortDateString().Equals(Locitemmain.ExpirationDate.Value.ToShortDateString())).ToList();
                        }
                        else if (objItem.SerialNumberTracking && !string.IsNullOrEmpty(Locitem.SerialNumber))
                        {
                            lstItemLocationDetail = lstItemLocationDetail.Where(x => x.SerialNumber.Equals(Locitem.SerialNumber)).ToList();
                        }
                        else if (objItem.LotNumberTracking && !string.IsNullOrEmpty(Locitem.LotNumber))
                        {
                            lstItemLocationDetail = lstItemLocationDetail.Where(x => x.LotNumber.Equals(Locitem.LotNumber)).ToList();
                        }
                        else if (objItem.DateCodeTracking && Locitemmain.ExpirationDate != null && Locitemmain.ExpirationDate != DateTime.MinValue)
                        {
                            lstItemLocationDetail = lstItemLocationDetail.Where(x => x.ExpirationDate.Value.ToShortDateString().Equals(Locitemmain.ExpirationDate.Value.ToShortDateString())).ToList();
                        }

                        //if (objItem.SerialNumberTracking && !string.IsNullOrEmpty(Locitem.SerialNumber))
                        //{
                        //    lstItemLocationDetail = lstItemLocationDetail.Where(x => x.SerialNumber.Equals(Locitem.SerialNumber));
                        //}
                        //else if (objItem.LotNumberTracking && !string.IsNullOrEmpty(Locitem.LotNumber))
                        //{
                        //    lstItemLocationDetail = lstItemLocationDetail.Where(x => x.LotNumber.Equals(Locitem.LotNumber));
                        //}
                        //if (lstItemLocationDetail != null && lstItemLocationDetail.Count() > 0)
                        //{
                        /* select * from itemlocationdetail where itegmguid and bindid
                        * counted - applied */
                        //
                        double? CusOwnedDifference = 0;
                        double? ConsignedDifference = 0;

                        double? countedCusOwned = 0;
                        double? countedConsigned = 0;

                        if (objItem.ID > 0)
                        {
                            if (lstItemLocationDetail != null && lstItemLocationDetail.Count() > 0)
                            {
                                countedCusOwned = lstItemLocationDetail.Sum(x => x.CustomerOwnedQuantity) ?? 0;
                                countedConsigned = lstItemLocationDetail.Sum(x => x.ConsignedQuantity) ?? 0;
                            }

                            CusOwnedDifference = (Locitem.customerownedquantity - countedCusOwned);
                            ConsignedDifference = (Locitem.consignedquantity - countedConsigned);

                            if (ConsignedDifference > 0 || CusOwnedDifference > 0)
                            {
                                double TotalConsignedPoolQuantity = 0;
                                double TotalCusOwnedPoolQuantity = 0;

                                if (ConsignedDifference > 0)
                                {
                                    if (objItem.SerialNumberTracking)
                                    {
                                        bool IsSerailAvailableForCredit = new PullTransactionDAL(base.DataBaseName).ValidateSerialNumberForCredit(objItem.GUID, Locitem.SerialNumber, CompanyID, RoomID);
                                        if (IsSerailAvailableForCredit == false)
                                        {
                                            Errormessage = msgCreditTransactionForSerialNumber + " " + Locitem.SerialNumber;//ResPullMaster.MsgCreditTransactionForSerialNumber
                                        }
                                    }
                                    if (CusOwnedDifference > 0)
                                    {
                                        if (objItem.SerialNumberTracking)
                                        {
                                            bool IsSerailAvailableForCredit = new PullTransactionDAL(base.DataBaseName).ValidateSerialNumberForCredit(objItem.GUID, Locitem.SerialNumber, CompanyID, RoomID);
                                            if (IsSerailAvailableForCredit == false)
                                            {
                                                if (string.IsNullOrWhiteSpace(Errormessage))
                                                    Errormessage = msgCreditTransactionForSerialNumber + " " + Locitem.SerialNumber;//ResPullMaster.MsgCreditTransactionForSerialNumber
                                                else
                                                    Errormessage += ";" + msgCreditTransactionForSerialNumber + " " + Locitem.SerialNumber;//ResPullMaster.MsgCreditTransactionForSerialNumber
                                            }
                                        }
                                    }
                                }

                                IEnumerable<PullDetailsDTO> lstPullDetailDTO = null;
                                PullDetailsDAL pullDetailDAL = new PullDetailsDAL(base.DataBaseName);

                                if (objItem.SerialNumberTracking && !string.IsNullOrEmpty(Locitem.SerialNumber))
                                {
                                    lstPullDetailDTO = pullDetailDAL.GetPullDetailsByItemGuidAndSerialNo_CreditHistoryPlain(objItem.GUID, "pull", RoomID, CompanyID, Locitem.SerialNumber).ToList();
                                }
                                else if (objItem.LotNumberTracking && !string.IsNullOrEmpty(Locitem.LotNumber))
                                {
                                    lstPullDetailDTO = pullDetailDAL.GetPullDetailsByItemGuidAndLotNo_CreditHistoryPlain(objItem.GUID, "pull", RoomID, CompanyID, Locitem.LotNumber).ToList();
                                }
                                else
                                {
                                    lstPullDetailDTO = pullDetailDAL.GetPullDetailsByItemGuid_CreditHistoryPlain(objItem.GUID, "pull", RoomID, CompanyID);
                                }
                                if (lstPullDetailDTO != null && lstPullDetailDTO.Count() > 0)
                                {
                                    if (ConsignedDifference > 0)
                                    {
                                        TotalConsignedPoolQuantity = lstPullDetailDTO.Where(x => (x.ConsignedQuantity ?? 0) > 0).Sum(x => x.PoolQuantity ?? 0);
                                        if (TotalConsignedPoolQuantity < (ConsignedDifference ?? 0))
                                        {
                                            if (!objRoomDTO.IsIgnoreCreditRule)
                                            {
                                                if (string.IsNullOrWhiteSpace(Errormessage))
                                                    Errormessage = msgPullQtyIsntEnoughForCredit;
                                                else
                                                    Errormessage += ";" + msgPullQtyIsntEnoughForCredit;
                                            }
                                        }
                                        else
                                        {
                                            if (objItem.SerialNumberTracking)
                                            {
                                                bool IsSerailAvailableForCredit = new PullTransactionDAL(base.DataBaseName).ValidateSerialNumberForCredit(objItem.GUID, Locitem.SerialNumber, CompanyID, RoomID);
                                                if (IsSerailAvailableForCredit == false)
                                                {
                                                    Errormessage = msgCreditTransactionForSerialNumber + " " + Locitem.SerialNumber;//ResPullMaster.MsgCreditTransactionForSerialNumber
                                                }
                                            }
                                        }
                                    }
                                    if (CusOwnedDifference > 0)
                                    {
                                        TotalCusOwnedPoolQuantity = lstPullDetailDTO.Where(x => (x.CustomerOwnedQuantity ?? 0) > 0).Sum(x => x.PoolQuantity ?? 0);
                                        if (TotalCusOwnedPoolQuantity < (CusOwnedDifference ?? 0))
                                        {
                                            if (!objRoomDTO.IsIgnoreCreditRule)
                                            {
                                                if (string.IsNullOrWhiteSpace(Errormessage))
                                                    Errormessage = msgPullQtyIsntEnoughForCredit;
                                                else
                                                    Errormessage += ";" + msgPullQtyIsntEnoughForCredit;
                                            }
                                        }
                                        else
                                        {
                                            if (objItem.SerialNumberTracking)
                                            {
                                                bool IsSerailAvailableForCredit = new PullTransactionDAL(base.DataBaseName).ValidateSerialNumberForCredit(objItem.GUID, Locitem.SerialNumber, CompanyID, RoomID);
                                                if (IsSerailAvailableForCredit == false)
                                                {
                                                    if (string.IsNullOrWhiteSpace(Errormessage))
                                                        Errormessage = msgCreditTransactionForSerialNumber + " " + Locitem.SerialNumber;//ResPullMaster.MsgCreditTransactionForSerialNumber
                                                    else
                                                        Errormessage += ";" + msgCreditTransactionForSerialNumber + " " + Locitem.SerialNumber;//ResPullMaster.MsgCreditTransactionForSerialNumber
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (!objRoomDTO.IsIgnoreCreditRule)
                                    {
                                        if (string.IsNullOrWhiteSpace(Errormessage))
                                            Errormessage = msgPullQtyIsntEnoughForCredit;
                                        else
                                            Errormessage += ";" + msgPullQtyIsntEnoughForCredit;
                                    }
                                }
                            }
                            //}
                        }

                    }
                    else
                    {
                        IEnumerable<ItemLocationDetailsDTO> lstItemLocationDetail = null;
                        lstItemLocationDetail = new ItemLocationDetailsDAL(base.DataBaseName).GetCountDifferenceforValidatPull(objItem.GUID, objItem.Room.GetValueOrDefault(0), objItem.CompanyID.GetValueOrDefault(0), Locitemmain.BinID.GetValueOrDefault(0));

                        if (objItem.SerialNumberTracking && !string.IsNullOrEmpty(Locitem.SerialNumber) && objItem.DateCodeTracking && Locitemmain.ExpirationDate != null && Locitemmain.ExpirationDate != DateTime.MinValue)
                        {
                            lstItemLocationDetail = lstItemLocationDetail.Where(x => x.SerialNumber.Equals(Locitem.SerialNumber) && x.ExpirationDate.Value.ToShortDateString().Equals(Locitemmain.ExpirationDate.Value.ToShortDateString())).ToList();
                        }
                        else if (objItem.SerialNumberTracking && !string.IsNullOrEmpty(Locitem.SerialNumber))
                        {
                            lstItemLocationDetail = lstItemLocationDetail.Where(x => x.SerialNumber.Equals(Locitem.SerialNumber)).ToList();
                        }

                        double? CusOwnedDifference = 0;
                        double? ConsignedDifference = 0;

                        double? countedCusOwned = 0;
                        double? countedConsigned = 0;

                        if (objItem.ID > 0)
                        {
                            if (lstItemLocationDetail != null && lstItemLocationDetail.Count() > 0)
                            {
                                countedCusOwned = lstItemLocationDetail.Sum(x => x.CustomerOwnedQuantity) ?? 0;
                                countedConsigned = lstItemLocationDetail.Sum(x => x.ConsignedQuantity) ?? 0;
                            }

                            CusOwnedDifference = (Locitem.customerownedquantity - countedCusOwned);
                            ConsignedDifference = (Locitem.consignedquantity - countedConsigned);

                            if (ConsignedDifference > 0 || CusOwnedDifference > 0)
                            {
                                if (objItem.SerialNumberTracking)
                                {
                                    bool IsSerailAvailableForCredit = new PullTransactionDAL(base.DataBaseName).ValidateSerialNumberForCredit(objItem.GUID, Locitem.SerialNumber, CompanyID, RoomID);
                                    if (IsSerailAvailableForCredit == false)
                                    {
                                        if (string.IsNullOrWhiteSpace(Errormessage))
                                            Errormessage = msgCreditTransactionForSerialNumber + " " + Locitem.SerialNumber;//ResPullMaster.MsgCreditTransactionForSerialNumber
                                        else
                                            Errormessage += ";" + msgCreditTransactionForSerialNumber + " " + Locitem.SerialNumber;//ResPullMaster.MsgCreditTransactionForSerialNumber
                                    }
                                }
                            }
                        }
                    }
                    ///////////check for 'credit' 

                    Locitemmain.UDF1 = Locitem.UDF1;
                    Locitemmain.UDF2 = Locitem.UDF2;
                    Locitemmain.UDF3 = Locitem.UDF3;
                    Locitemmain.UDF4 = Locitem.UDF4;
                    Locitemmain.UDF5 = Locitem.UDF5;

                    string errorMsg = string.Empty;
                    if (UDFDataFromDB != null && isUDFRequired == true)
                        CommonUtilityHelper.CheckUDFIsRequired(UDFDataFromDB, Locitem.UDF1, Locitem.UDF2, Locitem.UDF3, Locitem.UDF4, Locitem.UDF5, out errorMsg, EnterpriseID, CompanyID, RoomID, CurrentCult.Name);
                    if (!string.IsNullOrWhiteSpace(errorMsg))
                    {
                        //Locitem.Status = "Fail";
                        if (!string.IsNullOrEmpty(Errormessage))
                            Errormessage += "; " + errorMsg;
                        else
                            Errormessage = "; " + errorMsg;


                    }

                    if (!string.IsNullOrWhiteSpace(Errormessage))
                    {
                        Locitemmain.ErrorMessege = Errormessage;
                    }

                    Locitemmain.ItemType = objItem.ItemType;
                    Locitemmain.IsConsignedSerialLot = objItem.Consignment;
                    Locitemmain.SerialNumberTracking = objItem.SerialNumberTracking;
                    Locitemmain.LotNumberTracking = objItem.LotNumberTracking;
                    Locitemmain.DateCodeTracking = objItem.DateCodeTracking;


                    lstReturnList.Add(Locitemmain);
                }
            }
            return lstReturnList;
        }
        public List<ItemLocationDetailsDTO> ValidateILQRecordsForCount(List<InventoryLocationMain> lstItemLocationDetails, long RoomID, long CompanyID, long UserID, string RoomDateFormat, long EnterpriseID,
               CultureInfo CurrentCult, out string MsgCreditTransactionForSerialNumber, out string MsgPullQtyIsntEnoughForCredit, bool isUDFRequired = true, bool isCountPullValidate = false)
        {
            List<ItemLocationDetailsDTO> lstReturnList = new List<ItemLocationDetailsDTO>();
            ItemMasterDTO objItem = new ItemMasterDTO();
            BinMasterDTO objBinMasterDTO = new BinMasterDTO();
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
            eTurns.DAL.UDFDAL objUDFDAL = new eTurns.DAL.UDFDAL(base.DataBaseName);
            CommonDAL cmnDAL = new CommonDAL(base.DataBaseName);
            IEnumerable<UDFDTO> UDFDataFromDB = null;
            if (isUDFRequired == true)
                UDFDataFromDB = objUDFDAL.GetRequiredUDFsByUDFTableNamePlain("InventoryCount", RoomID, CompanyID);

            //RoomDTO objRoomDTO = new RoomDAL(base.DataBaseName).GetRoomByIDPlain(RoomID);
            string columnList = "ID,RoomName,IsIgnoreCreditRule";
            RoomDTO objRoomDTO = new CommonDAL(base.DataBaseName).GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
            string CultureCode = "en-US";

            if (CurrentCult != null)
            {
                CultureCode = CurrentCult.Name;
            }

            var inventoryCountResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResInventoryCount", CultureCode, EnterpriseID, CompanyID);
            var pullMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResPullMaster", CultureCode, EnterpriseID, CompanyID);
            string msgCreditTransactionForSerialNumber = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgCreditTransactionForSerialNumber", pullMasterResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResPullMaster", CultureCode);
            string msgPullQtyIsntEnoughForCredit = ResourceRead.GetResourceValueByKeyAndFullFilePath("PullQtyIsntEnoughForCredit", inventoryCountResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResInventoryCount", CultureCode);
            MsgCreditTransactionForSerialNumber = msgCreditTransactionForSerialNumber;
            MsgPullQtyIsntEnoughForCredit = msgPullQtyIsntEnoughForCredit;

            if (lstItemLocationDetails != null && lstItemLocationDetails.Count > 0)
            {
                var receiveOrderDetailsResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResReceiveOrderDetails", CultureCode, EnterpriseID, CompanyID);
                var toolMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResToolMaster", CultureCode, EnterpriseID, CompanyID);

                string msgEnterItemNumber = ResourceRead.GetResourceValueByKeyAndFullFilePath("EnterItemNumber", inventoryCountResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResInventoryCount", CultureCode);
                string msgItemDoesNotExistsInRoom = ResourceRead.GetResourceValueByKeyAndFullFilePath("ItemDoesNotExistsInRoom", inventoryCountResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResInventoryCount", CultureCode);
                string msgLotNumberCantBeBlank = ResourceRead.GetResourceValueByKeyAndFullFilePath("LotNumberCantBeBlank", inventoryCountResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResInventoryCount", CultureCode);
                string msgEnterReceiveDateFormat = ResourceRead.GetResourceValueByKeyAndFullFilePath("EnterReceiveDateFormat", inventoryCountResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResInventoryCount", CultureCode);
                string msgExpirationDateCantBeEmpty = ResourceRead.GetResourceValueByKeyAndFullFilePath("ExpirationDateCantBeEmpty", inventoryCountResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResInventoryCount", CultureCode);
                string msgEnterExpiryDateFormat = ResourceRead.GetResourceValueByKeyAndFullFilePath("EnterExpiryDateFormat", inventoryCountResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResInventoryCount", CultureCode);
                string msgCountCustOrConsQtyShouldBeGreaterEqualsZero = ResourceRead.GetResourceValueByKeyAndFullFilePath("CountCustOrConsQtyShouldBeGreaterEqualsZero", inventoryCountResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResInventoryCount", CultureCode);
                string msgSameLotDifferentExpirationNotAllowed = ResourceRead.GetResourceValueByKeyAndFullFilePath("SameLotDifferentExpirationNotAllowed", inventoryCountResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResInventoryCount", CultureCode);
                string msgBinNumberValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgBinNumberValidation", receiveOrderDetailsResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResReceiveOrderDetails", CultureCode);
                string msgSerialNoCantBeBlank = ResourceRead.GetResourceValueByKeyAndFullFilePath("SerialNoCantBeBlank", toolMasterResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResToolMaster", CultureCode);

                foreach (var Locitem in lstItemLocationDetails)
                {
                    ItemLocationDetailsDTO Locitemmain = new ItemLocationDetailsDTO();
                    Locitemmain.BinNumber = Locitem.BinNumber;
                    Locitemmain.CompanyID = CompanyID;
                    Locitemmain.ConsignedQuantity = Locitem.consignedquantity;
                    Locitemmain.Cost = Locitem.Cost;
                    Locitemmain.Created = DateTime.UtcNow;
                    Locitemmain.CreatedBy = UserID;
                    Locitemmain.CustomerOwnedQuantity = Locitem.customerownedquantity;
                    //Locitemmain.Expiration = Locitem.Expiration;
                    Locitemmain.GUID = Locitem.GUID;
                    Locitemmain.ID = Locitem.ID;
                    Locitemmain.InsertedFrom = Locitem.InsertedFrom;
                    Locitemmain.IsArchived = Locitem.IsArchived;
                    Locitemmain.IsDeleted = Locitem.IsDeleted;
                    Locitemmain.ItemGUID = Locitem.ItemGUID;
                    Locitemmain.ItemNumber = Locitem.ItemNumber;
                    Locitemmain.LastUpdatedBy = Locitem.LastUpdatedBy;
                    Locitemmain.LotNumber = (!string.IsNullOrWhiteSpace(Locitem.LotNumber)) ? Locitem.LotNumber.Trim() : string.Empty;
                    Locitemmain.Received = Locitem.Received;
                    Locitemmain.Room = RoomID;
                    Locitemmain.SerialNumber = (!string.IsNullOrWhiteSpace(Locitem.SerialNumber)) ? Locitem.SerialNumber.Trim() : string.Empty;
                    Locitemmain.Updated = DateTime.UtcNow;
                    Locitemmain.ItemDescription = Locitem.ItemDescription;
                    Locitemmain.ProjectSpend = Locitem.ProjectSpend;

                    string Errormessage = string.Empty;

                    if (string.IsNullOrWhiteSpace(Locitem.ItemNumber))
                    {
                        Errormessage += ";" + msgEnterItemNumber;
                    }
                    if (string.IsNullOrWhiteSpace(Locitem.BinNumber))
                    {
                        Errormessage += ";" + msgBinNumberValidation;
                    }
                    if ((Locitem.ItemNumber != objItem.ItemNumber) && string.IsNullOrWhiteSpace(Errormessage))
                    {
                        objItem = objItemMasterDAL.GetItemByItemNumberPlain(Locitem.ItemNumber, RoomID, CompanyID);
                        if (objItem == null)
                        {
                            objItem = new ItemMasterDTO();
                        }
                        //Locitemmain.ItemNumber = objItem.ItemNumber;
                        Locitemmain.ItemGUID = objItem.GUID;
                    }
                    else
                    {
                        Locitemmain.ItemNumber = objItem.ItemNumber;
                        Locitemmain.ItemGUID = objItem.GUID;
                        if (objItem == null)
                        {
                            objItem = new ItemMasterDTO();
                        }
                    }
                    if (objItem.ID < 1)
                    {
                        Errormessage += ";" + msgItemDoesNotExistsInRoom;
                    }
                    if (objItem.ID > 0)
                    {
                        if (objItem.SerialNumberTracking || objItem.LotNumberTracking)
                        {
                            //Errormessage += ";Serial and lot import will be available soon,The work is in progress";
                        }
                    }
                    if (string.IsNullOrWhiteSpace(Errormessage) && objItem.ID > 0)
                    {
                        objBinMasterDTO = GetItemBin(objItem.GUID, Locitem.BinNumber, RoomID, CompanyID, UserID, false);
                        if (objBinMasterDTO != null)
                        {
                            Locitemmain.BinNumber = objBinMasterDTO.BinNumber;
                            Locitemmain.BinID = objBinMasterDTO.ID;
                        }
                    }
                    if (string.IsNullOrWhiteSpace(Errormessage) && objItem.LotNumberTracking)
                    {
                        if (string.IsNullOrWhiteSpace(Locitem.LotNumber))
                        {
                            Errormessage += ";" + msgLotNumberCantBeBlank;
                        }
                        Locitemmain.SerialNumber = null;
                    }


                    if (!string.IsNullOrEmpty(Locitem.Received))
                    {
                        DateTime expiryDate = DateTime.MinValue;
                        DateTime.TryParseExact(Locitem.Received, RoomDateFormat, CurrentCult, DateTimeStyles.None, out expiryDate);
                        if (expiryDate != DateTime.MinValue)
                        {
                            Locitemmain.ReceivedDate = expiryDate;
                        }
                        else
                        {
                            Locitemmain.ReceivedDate = null;
                            Errormessage += ";" + string.Format(msgEnterReceiveDateFormat, RoomDateFormat);
                        }
                    }
                    if (string.IsNullOrWhiteSpace(Errormessage) && objItem.SerialNumberTracking)
                    {
                        if (string.IsNullOrWhiteSpace(Locitem.SerialNumber)
                            && (Locitem.customerownedquantity.GetValueOrDefault(0) + Locitem.consignedquantity.GetValueOrDefault(0) > 0))
                        {
                            Errormessage += ";" + msgSerialNoCantBeBlank;
                        }
                        Locitemmain.LotNumber = null;
                    }
                    if (string.IsNullOrWhiteSpace(Errormessage) && objItem.DateCodeTracking)
                    {
                        if (string.IsNullOrWhiteSpace(Locitem.Expiration))
                        {
                            Errormessage += ";" + msgExpirationDateCantBeEmpty;
                        }
                        else
                        {
                            DateTime expiryDate = DateTime.MinValue;
                            DateTime.TryParseExact(Locitem.Expiration, RoomDateFormat, ResourceHelper.CurrentCult, DateTimeStyles.None, out expiryDate);
                            if (expiryDate != DateTime.MinValue)
                            {
                                Locitemmain.ExpirationDate = expiryDate;
                                Locitemmain.Expiration = expiryDate.ToString("MM/dd/yyyy");
                            }
                            else
                            {
                                Locitemmain.ExpirationDate = null;
                                Errormessage += ";" + string.Format(msgEnterExpiryDateFormat, RoomDateFormat);
                            }
                        }
                    }
                    if (objItem.ID > 0 && !objItem.Consignment)
                    {
                        Locitemmain.ConsignedQuantity = 0;
                    }

                    if (string.IsNullOrWhiteSpace(Errormessage) && ((Locitemmain.CustomerOwnedQuantity ?? 0) < 0 || (Locitemmain.ConsignedQuantity ?? 0) < 0))
                    {

                        Errormessage += ";" + msgCountCustOrConsQtyShouldBeGreaterEqualsZero;
                    }

                    if (objItem.DateCodeTracking && objItem.LotNumberTracking)
                    {
                        if (Locitemmain.ExpirationDate.GetValueOrDefault(DateTime.MinValue) != DateTime.MinValue && !string.IsNullOrWhiteSpace(Locitemmain.LotNumber))
                        {
                            string msg = cmnDAL.CheckDuplicateLotAndExpiration(Locitemmain.LotNumber, Locitemmain.Expiration, Locitemmain.ExpirationDate.GetValueOrDefault(DateTime.MinValue), 0, RoomID, CompanyID, objItem.GUID, UserID, EnterpriseID);
                            if (string.IsNullOrWhiteSpace(msg) || (msg ?? string.Empty).ToLower() == "ok")
                            {
                                if (lstReturnList != null && lstReturnList.Count > 0)
                                {
                                    var isExists = lstReturnList.Where(x => (x.Expiration ?? string.Empty) != (Locitemmain.Expiration ?? string.Empty) && (x.LotNumber ?? string.Empty).ToLower() == (Locitemmain.LotNumber ?? string.Empty).ToLower()).FirstOrDefault();
                                    if (isExists != null)
                                    {
                                        Errormessage += ";" + msgSameLotDifferentExpirationNotAllowed;
                                    }
                                }
                            }
                            else
                            {
                                Errormessage += ";" + msg;
                            }
                        }
                    }


                    if (isCountPullValidate == true)
                    {
                        //// check for 'credit' 

                        IEnumerable<ItemLocationDetailsDTO> lstItemLocationDetail = null;
                        lstItemLocationDetail = new ItemLocationDetailsDAL(base.DataBaseName).GetCountDifferenceforValidatPull(objItem.GUID, objItem.Room.GetValueOrDefault(0), objItem.CompanyID.GetValueOrDefault(0), Locitemmain.BinID.GetValueOrDefault(0));

                        if (objItem.SerialNumberTracking && !string.IsNullOrEmpty(Locitem.SerialNumber) && objItem.DateCodeTracking && Locitemmain.ExpirationDate != null && Locitemmain.ExpirationDate != DateTime.MinValue)
                        {
                            lstItemLocationDetail = lstItemLocationDetail.Where(x => x.SerialNumber.Equals(Locitem.SerialNumber) && x.ExpirationDate.Value.ToShortDateString().Equals(Locitemmain.ExpirationDate.Value.ToShortDateString())).ToList();
                        }
                        else if (objItem.LotNumberTracking && !string.IsNullOrEmpty(Locitem.LotNumber) && objItem.DateCodeTracking && Locitemmain.ExpirationDate != null && Locitemmain.ExpirationDate != DateTime.MinValue)
                        {
                            lstItemLocationDetail = lstItemLocationDetail.Where(x => x.LotNumber.Equals(Locitem.LotNumber) && x.ExpirationDate.Value.ToShortDateString().Equals(Locitemmain.ExpirationDate.Value.ToShortDateString())).ToList();
                        }
                        else if (objItem.SerialNumberTracking && !string.IsNullOrEmpty(Locitem.SerialNumber))
                        {
                            lstItemLocationDetail = lstItemLocationDetail.Where(x => x.SerialNumber.Equals(Locitem.SerialNumber)).ToList();
                        }
                        else if (objItem.LotNumberTracking && !string.IsNullOrEmpty(Locitem.LotNumber))
                        {
                            lstItemLocationDetail = lstItemLocationDetail.Where(x => x.LotNumber.Equals(Locitem.LotNumber)).ToList();
                        }
                        else if (objItem.DateCodeTracking && Locitemmain.ExpirationDate != null && Locitemmain.ExpirationDate != DateTime.MinValue)
                        {
                            lstItemLocationDetail = lstItemLocationDetail.Where(x => x.ExpirationDate.Value.ToShortDateString().Equals(Locitemmain.ExpirationDate.Value.ToShortDateString())).ToList();
                        }

                        //if (objItem.SerialNumberTracking && !string.IsNullOrEmpty(Locitem.SerialNumber))
                        //{
                        //    lstItemLocationDetail = lstItemLocationDetail.Where(x => x.SerialNumber.Equals(Locitem.SerialNumber));
                        //}
                        //else if (objItem.LotNumberTracking && !string.IsNullOrEmpty(Locitem.LotNumber))
                        //{
                        //    lstItemLocationDetail = lstItemLocationDetail.Where(x => x.LotNumber.Equals(Locitem.LotNumber));
                        //}
                        //if (lstItemLocationDetail != null && lstItemLocationDetail.Count() > 0)
                        //{
                        /* select * from itemlocationdetail where itegmguid and bindid
                        * counted - applied */
                        //
                        double? CusOwnedDifference = 0;
                        double? ConsignedDifference = 0;

                        double? countedCusOwned = 0;
                        double? countedConsigned = 0;

                        if (objItem.ID > 0)
                        {
                            if (lstItemLocationDetail != null && lstItemLocationDetail.Count() > 0)
                            {
                                countedCusOwned = lstItemLocationDetail.Sum(x => x.CustomerOwnedQuantity) ?? 0;
                                countedConsigned = lstItemLocationDetail.Sum(x => x.ConsignedQuantity) ?? 0;
                            }

                            CusOwnedDifference = (Locitem.customerownedquantity - countedCusOwned);
                            ConsignedDifference = (Locitem.consignedquantity - countedConsigned);

                            if (ConsignedDifference > 0 || CusOwnedDifference > 0)
                            {
                                double TotalConsignedPoolQuantity = 0;
                                double TotalCusOwnedPoolQuantity = 0;

                                if (ConsignedDifference > 0)
                                {
                                    if (objItem.SerialNumberTracking)
                                    {
                                        //bool IsSerailAvailableForCredit = new PullTransactionDAL(base.DataBaseName).ValidateSerialNumberForCredit(objItem.GUID, Locitem.SerialNumber, CompanyID, RoomID);
                                        bool IsSerailAvailableForCredit = new PullTransactionDAL(base.DataBaseName).ValidateSerialNumberForCreditCount(objItem.GUID, Locitem.SerialNumber, CompanyID, RoomID, ConsignedDifference.GetValueOrDefault(0), CusOwnedDifference.GetValueOrDefault(0));
                                        if (IsSerailAvailableForCredit == false)
                                        {
                                            Errormessage = msgCreditTransactionForSerialNumber + " " + Locitem.SerialNumber;
                                        }
                                    }
                                    if (CusOwnedDifference > 0)
                                    {
                                        if (objItem.SerialNumberTracking)
                                        {
                                            //bool IsSerailAvailableForCredit = new PullTransactionDAL(base.DataBaseName).ValidateSerialNumberForCredit(objItem.GUID, Locitem.SerialNumber, CompanyID, RoomID);
                                            bool IsSerailAvailableForCredit = new PullTransactionDAL(base.DataBaseName).ValidateSerialNumberForCreditCount(objItem.GUID, Locitem.SerialNumber, CompanyID, RoomID, ConsignedDifference.GetValueOrDefault(0), CusOwnedDifference.GetValueOrDefault(0));
                                            if (IsSerailAvailableForCredit == false)
                                            {
                                                if (string.IsNullOrWhiteSpace(Errormessage))
                                                    Errormessage = msgCreditTransactionForSerialNumber + " " + Locitem.SerialNumber;
                                                else
                                                    Errormessage += ";" + msgCreditTransactionForSerialNumber + " " + Locitem.SerialNumber;
                                            }
                                        }
                                    }
                                }

                                IEnumerable<PullDetailsDTO> lstPullDetailDTO = null;
                                PullDetailsDAL pullDetailDAL = new PullDetailsDAL(base.DataBaseName);

                                if (objItem.SerialNumberTracking && !string.IsNullOrEmpty(Locitem.SerialNumber))
                                {
                                    lstPullDetailDTO = pullDetailDAL.GetPullDetailsByItemGuidAndSerialNo_CreditHistoryPlain(objItem.GUID, "pull", RoomID, CompanyID, Locitem.SerialNumber).ToList();
                                }
                                else if (objItem.LotNumberTracking && !string.IsNullOrEmpty(Locitem.LotNumber))
                                {
                                    lstPullDetailDTO = pullDetailDAL.GetPullDetailsByItemGuidAndLotNo_CreditHistoryPlain(objItem.GUID, "pull", RoomID, CompanyID, Locitem.LotNumber).ToList();
                                }
                                else
                                {
                                    lstPullDetailDTO = pullDetailDAL.GetPullDetailsByItemGuid_CreditHistoryPlain(objItem.GUID, "pull", RoomID, CompanyID);
                                }
                                if (lstPullDetailDTO != null && lstPullDetailDTO.Count() > 0)
                                {
                                    if (ConsignedDifference > 0)
                                    {
                                        TotalConsignedPoolQuantity = lstPullDetailDTO.Where(x => (x.ConsignedQuantity ?? 0) > 0).Sum(x => x.PoolQuantity ?? 0);
                                        if (TotalConsignedPoolQuantity < (ConsignedDifference ?? 0))
                                        {
                                            if (!objRoomDTO.IsIgnoreCreditRule)
                                            {
                                                if (string.IsNullOrWhiteSpace(Errormessage))
                                                    Errormessage = msgPullQtyIsntEnoughForCredit;
                                                else
                                                    Errormessage += ";" + msgPullQtyIsntEnoughForCredit;
                                            }
                                        }
                                        else
                                        {
                                            if (objItem.SerialNumberTracking)
                                            {
                                                //bool IsSerailAvailableForCredit = new PullTransactionDAL(base.DataBaseName).ValidateSerialNumberForCredit(objItem.GUID, Locitem.SerialNumber, CompanyID, RoomID);
                                                bool IsSerailAvailableForCredit = new PullTransactionDAL(base.DataBaseName).ValidateSerialNumberForCreditCount(objItem.GUID, Locitem.SerialNumber, CompanyID, RoomID, ConsignedDifference.GetValueOrDefault(0), CusOwnedDifference.GetValueOrDefault(0));
                                                if (IsSerailAvailableForCredit == false)
                                                {
                                                    Errormessage = msgCreditTransactionForSerialNumber + " " + Locitem.SerialNumber;
                                                }
                                            }
                                        }
                                    }
                                    if (CusOwnedDifference > 0)
                                    {
                                        TotalCusOwnedPoolQuantity = lstPullDetailDTO.Where(x => (x.CustomerOwnedQuantity ?? 0) > 0).Sum(x => x.PoolQuantity ?? 0);
                                        if (TotalCusOwnedPoolQuantity < (CusOwnedDifference ?? 0))
                                        {
                                            if (!objRoomDTO.IsIgnoreCreditRule)
                                            {
                                                if (string.IsNullOrWhiteSpace(Errormessage))
                                                    Errormessage = msgPullQtyIsntEnoughForCredit;
                                                else
                                                    Errormessage += ";" + msgPullQtyIsntEnoughForCredit;
                                            }
                                        }
                                        else
                                        {
                                            if (objItem.SerialNumberTracking)
                                            {
                                                //bool IsSerailAvailableForCredit = new PullTransactionDAL(base.DataBaseName).ValidateSerialNumberForCredit(objItem.GUID, Locitem.SerialNumber, CompanyID, RoomID);
                                                bool IsSerailAvailableForCredit = new PullTransactionDAL(base.DataBaseName).ValidateSerialNumberForCreditCount(objItem.GUID, Locitem.SerialNumber, CompanyID, RoomID, ConsignedDifference.GetValueOrDefault(0), CusOwnedDifference.GetValueOrDefault(0));
                                                if (IsSerailAvailableForCredit == false)
                                                {
                                                    if (string.IsNullOrWhiteSpace(Errormessage))
                                                        Errormessage = msgCreditTransactionForSerialNumber + " " + Locitem.SerialNumber;
                                                    else
                                                        Errormessage += ";" + msgCreditTransactionForSerialNumber + " " + Locitem.SerialNumber;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (!objRoomDTO.IsIgnoreCreditRule)
                                    {
                                        if (string.IsNullOrWhiteSpace(Errormessage))
                                            Errormessage = msgPullQtyIsntEnoughForCredit;
                                        else
                                            Errormessage += ";" + msgPullQtyIsntEnoughForCredit;
                                    }
                                }
                            }
                            //}
                        }

                    }
                    else
                    {
                        IEnumerable<ItemLocationDetailsDTO> lstItemLocationDetail = null;
                        lstItemLocationDetail = new ItemLocationDetailsDAL(base.DataBaseName).GetCountDifferenceforValidatPull(objItem.GUID, objItem.Room.GetValueOrDefault(0), objItem.CompanyID.GetValueOrDefault(0), Locitemmain.BinID.GetValueOrDefault(0));

                        if (objItem.SerialNumberTracking && !string.IsNullOrEmpty(Locitem.SerialNumber) && objItem.DateCodeTracking && Locitemmain.ExpirationDate != null && Locitemmain.ExpirationDate != DateTime.MinValue)
                        {
                            lstItemLocationDetail = lstItemLocationDetail.Where(x => x.SerialNumber.Equals(Locitem.SerialNumber) && x.ExpirationDate.Value.ToShortDateString().Equals(Locitemmain.ExpirationDate.Value.ToShortDateString())).ToList();
                        }
                        else if (objItem.SerialNumberTracking && !string.IsNullOrEmpty(Locitem.SerialNumber))
                        {
                            lstItemLocationDetail = lstItemLocationDetail.Where(x => x.SerialNumber.Equals(Locitem.SerialNumber)).ToList();
                        }

                        double? CusOwnedDifference = 0;
                        double? ConsignedDifference = 0;

                        double? countedCusOwned = 0;
                        double? countedConsigned = 0;

                        if (objItem.ID > 0)
                        {
                            if (lstItemLocationDetail != null && lstItemLocationDetail.Count() > 0)
                            {
                                countedCusOwned = lstItemLocationDetail.Sum(x => x.CustomerOwnedQuantity) ?? 0;
                                countedConsigned = lstItemLocationDetail.Sum(x => x.ConsignedQuantity) ?? 0;
                            }

                            CusOwnedDifference = (Locitem.customerownedquantity - countedCusOwned);
                            ConsignedDifference = (Locitem.consignedquantity - countedConsigned);

                            if (ConsignedDifference > 0 || CusOwnedDifference > 0)
                            {
                                if (objItem.SerialNumberTracking)
                                {
                                    //bool IsSerailAvailableForCredit = new PullTransactionDAL(base.DataBaseName).ValidateSerialNumberForCredit(objItem.GUID, Locitem.SerialNumber, CompanyID, RoomID);
                                    bool IsSerailAvailableForCredit = new PullTransactionDAL(base.DataBaseName).ValidateSerialNumberForCreditCount(objItem.GUID, Locitem.SerialNumber, CompanyID, RoomID, ConsignedDifference.GetValueOrDefault(0), CusOwnedDifference.GetValueOrDefault(0));
                                    if (IsSerailAvailableForCredit == false)
                                    {
                                        if (string.IsNullOrWhiteSpace(Errormessage))
                                            Errormessage = msgCreditTransactionForSerialNumber + " " + Locitem.SerialNumber;
                                        else
                                            Errormessage += ";" + msgCreditTransactionForSerialNumber + " " + Locitem.SerialNumber;
                                    }
                                }
                            }
                        }
                    }
                    ///////////check for 'credit' 

                    Locitemmain.UDF1 = Locitem.UDF1;
                    Locitemmain.UDF2 = Locitem.UDF2;
                    Locitemmain.UDF3 = Locitem.UDF3;
                    Locitemmain.UDF4 = Locitem.UDF4;
                    Locitemmain.UDF5 = Locitem.UDF5;

                    string errorMsg = string.Empty;
                    if (UDFDataFromDB != null && isUDFRequired == true)
                        CommonUtilityHelper.CheckUDFIsRequired(UDFDataFromDB, Locitem.UDF1, Locitem.UDF2, Locitem.UDF3, Locitem.UDF4, Locitem.UDF5, out errorMsg, EnterpriseID, CompanyID, RoomID, CurrentCult.Name);
                    if (!string.IsNullOrWhiteSpace(errorMsg))
                    {
                        //Locitem.Status = "Fail";
                        if (!string.IsNullOrEmpty(Errormessage))
                            Errormessage += "; " + errorMsg;
                        else
                            Errormessage = "; " + errorMsg;


                    }

                    if (!string.IsNullOrWhiteSpace(Errormessage))
                    {
                        Locitemmain.ErrorMessege = Errormessage;
                    }

                    Locitemmain.ItemType = objItem.ItemType;
                    Locitemmain.IsConsignedSerialLot = objItem.Consignment;
                    Locitemmain.SerialNumberTracking = objItem.SerialNumberTracking;
                    Locitemmain.LotNumberTracking = objItem.LotNumberTracking;
                    Locitemmain.DateCodeTracking = objItem.DateCodeTracking;


                    lstReturnList.Add(Locitemmain);
                }
            }
            return lstReturnList;
        }
        public List<ItemLocationDetailsDTO> ValidateILQRecords_NewImport(List<ImportMastersNewDTO.InventoryLocationImport> lstItemLocationDetails, long RoomID, long CompanyID, long UserID, string RoomDateFormat)
        {
            List<ItemLocationDetailsDTO> lstReturnList = new List<ItemLocationDetailsDTO>();
            ItemMasterDTO objItem = new ItemMasterDTO();
            BinMasterDTO objBinMasterDTO = new BinMasterDTO();
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
            if (lstItemLocationDetails != null && lstItemLocationDetails.Count > 0)
            {
                foreach (var Locitem in lstItemLocationDetails)
                {
                    ItemLocationDetailsDTO Locitemmain = new ItemLocationDetailsDTO();
                    Locitemmain.BinNumber = Locitem.BinNumber;
                    Locitemmain.CompanyID = CompanyID;
                    Locitemmain.ConsignedQuantity = Locitem.consignedquantity;
                    Locitemmain.Cost = Locitem.Cost;
                    Locitemmain.Created = DateTime.UtcNow;
                    Locitemmain.CreatedBy = UserID;
                    Locitemmain.CustomerOwnedQuantity = Locitem.customerownedquantity;
                    Locitemmain.Expiration = Locitem.Expiration;
                    Locitemmain.GUID = Locitem.GUID;
                    Locitemmain.ID = Locitem.ID;
                    Locitemmain.InsertedFrom = Locitem.InsertedFrom;
                    Locitemmain.IsArchived = Locitem.IsArchived;
                    Locitemmain.IsDeleted = Locitem.IsDeleted;
                    Locitemmain.ItemGUID = Locitem.ItemGUID;
                    Locitemmain.ItemNumber = Locitem.ItemNumber;
                    Locitemmain.LastUpdatedBy = Locitem.LastUpdatedBy;
                    Locitemmain.LotNumber = Locitem.LotNumber;
                    Locitemmain.Received = Locitem.Received;
                    Locitemmain.Room = RoomID;
                    Locitemmain.SerialNumber = Locitem.SerialNumber;
                    Locitemmain.Updated = DateTime.UtcNow;

                    Locitemmain.ProjectSpend = Locitem.ProjectSpend;

                    string Errormessage = string.Empty;

                    if (string.IsNullOrWhiteSpace(Locitem.ItemNumber))
                    {
                        Errormessage += ";please enter item number";
                    }
                    if (string.IsNullOrWhiteSpace(Locitem.BinNumber))
                    {
                        Errormessage += ";please enter bin number";
                    }
                    if ((Locitem.ItemNumber != objItem.ItemNumber) && string.IsNullOrWhiteSpace(Errormessage))
                    {
                        objItem = objItemMasterDAL.GetItemByItemNumberPlain(Locitem.ItemNumber, RoomID, CompanyID);
                        if (objItem == null)
                        {
                            objItem = new ItemMasterDTO();
                        }
                        //Locitemmain.ItemNumber = objItem.ItemNumber;
                        Locitemmain.ItemGUID = objItem.GUID;
                    }
                    else
                    {
                        Locitemmain.ItemNumber = objItem.ItemNumber;
                        Locitemmain.ItemGUID = objItem.GUID;
                        if (objItem == null)
                        {
                            objItem = new ItemMasterDTO();
                        }
                    }
                    if (objItem.ID < 1)
                    {
                        Errormessage += ";item Does not exists in room";
                    }
                    if (objItem.ID > 0)
                    {
                        if (objItem.SerialNumberTracking || objItem.LotNumberTracking)
                        {
                            //Errormessage += ";Serial and lot import will be available soon,The work is in progress";
                        }
                    }
                    if (string.IsNullOrWhiteSpace(Errormessage) && objItem.ID > 0)
                    {
                        objBinMasterDTO = GetItemBin(objItem.GUID, Locitem.BinNumber, RoomID, CompanyID, UserID, false);
                        if (objBinMasterDTO != null)
                        {
                            Locitemmain.BinNumber = objBinMasterDTO.BinNumber;
                            Locitemmain.BinID = objBinMasterDTO.ID;
                        }
                    }
                    if (string.IsNullOrWhiteSpace(Errormessage) && objItem.LotNumberTracking)
                    {
                        if (string.IsNullOrWhiteSpace(Locitem.LotNumber))
                        {
                            Errormessage += ";Lot number can not be blank";
                        }
                        Locitemmain.SerialNumber = null;
                    }
                    if (!string.IsNullOrEmpty(Locitem.Expiration))
                    {
                        DateTime expiryDate = DateTime.MinValue;
                        DateTime.TryParseExact(Locitem.Expiration, RoomDateFormat, ResourceHelper.CurrentCult, DateTimeStyles.None, out expiryDate);
                        if (expiryDate != DateTime.MinValue)
                        {
                            Locitemmain.ExpirationDate = expiryDate;
                        }
                        else
                        {
                            Locitemmain.ExpirationDate = null;
                            Errormessage += ";Please enter " + RoomDateFormat + " Expiry date format.";
                        }
                    }
                    if (!string.IsNullOrEmpty(Locitem.Received))
                    {
                        DateTime expiryDate = DateTime.MinValue;
                        DateTime.TryParseExact(Locitem.Received, RoomDateFormat, ResourceHelper.CurrentCult, DateTimeStyles.None, out expiryDate);
                        if (expiryDate != DateTime.MinValue)
                        {
                            Locitemmain.ReceivedDate = expiryDate;
                        }
                        else
                        {
                            Locitemmain.ReceivedDate = null;
                            Errormessage += ";Please enter " + RoomDateFormat + " receive date format.";
                        }
                    }
                    if (string.IsNullOrWhiteSpace(Errormessage) && objItem.SerialNumberTracking)
                    {
                        if (string.IsNullOrWhiteSpace(Locitem.SerialNumber))
                        {
                            //string msgSerialNoCantBeBlank = ResourceRead.GetResourceValueByKeyAndFullFilePath("SerialNoCantBeBlank", toolMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResToolMaster", CultureCode);
                            Errormessage += ";serial number can not be blank";
                        }
                        Locitemmain.LotNumber = null;
                    }
                    if (string.IsNullOrWhiteSpace(Errormessage) && objItem.DateCodeTracking)
                    {
                        if (string.IsNullOrWhiteSpace(Locitem.Expiration))
                        {
                            Errormessage += ";expiration date can not be empty";
                        }
                    }
                    if (objItem.ID > 0 && !objItem.Consignment)
                    {
                        Locitemmain.ConsignedQuantity = 0;
                    }
                    if (!string.IsNullOrWhiteSpace(Errormessage))
                    {
                        Locitemmain.ErrorMessege = Errormessage;
                    }
                    lstReturnList.Add(Locitemmain);
                }
            }
            return lstReturnList;
        }

        public List<InventoryLocationMain> ProceedILQImport(List<InventoryLocationMain> lstItemLocationDetails, long RoomID, long CompanyID, long UserID)
        {
            List<InventoryLocationMain> lstReturnIlq = new List<InventoryLocationMain>();

            return lstReturnIlq;
        }

        public void ApplyCountLineitem(DataTable DT, long RoomID, long CompanyID, long UserID)
        {
            try
            {
                string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
                SqlConnection ChildDbConnection = new SqlConnection(Connectionstring);
                DataSet Ds = SqlHelper.ExecuteDataset(ChildDbConnection, "CountAnItem", RoomID, CompanyID, UserID, DT);
            }
            catch
            {

            }
        }
        public void ApplyItemLocationQtyWithCost(DataTable DT, long RoomID, long CompanyID, long UserID)
        {
            try
            {
                string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
                SqlConnection ChildDbConnection = new SqlConnection(Connectionstring);
                DataSet Ds = SqlHelper.ExecuteDataset(ChildDbConnection, "ItemLocationQtyCostImport", RoomID, CompanyID, UserID, DT);
            }
            catch
            {

            }
        }
        public void ApplyCountLineitem(DataTable DT, long RoomID, long CompanyID, long UserID, Guid CountGUID, Guid CountLineItemGUID)
        {
            try
            {
                SqlConnection ChildDbConnection = new SqlConnection(base.DataBaseConnectionString);
                DataSet Ds = SqlHelper.ExecuteDataset(ChildDbConnection, "CountAnItemUI", RoomID, CompanyID, UserID, CountGUID, CountLineItemGUID, DT);
            }
            catch
            {

            }
        }

        //
        public void ApplyStageCountLineitem(DataTable DT, long RoomID, long CompanyID, long UserID, Guid CountGUID, Guid CountLineItemGUID)
        {
            try
            {
                SqlConnection ChildDbConnection = new SqlConnection(base.DataBaseConnectionString);
                DataSet Ds = SqlHelper.ExecuteDataset(ChildDbConnection, "CountAnItemUIStaging", RoomID, CompanyID, UserID, CountGUID, CountLineItemGUID, DT);
            }
            catch
            {

            }
        }


        public List<ItemLocationDetailsDTO> GetRecordsByBinNumberAndLotSerial(Guid ItemGuid, string BinNumber, string LotSerialNumber, Int64 RoomID, Int64 CompanyID)
        {
            //Need to changes...
            //return GetCachedData(0,RoomID, CompanyID).Where(t => t.ID == id).SingleOrDefault();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<ItemLocationDetailsDTO> lstItemLocationDetailsDTO = new List<ItemLocationDetailsDTO>();

                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGuid),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@BinNumber", BinNumber),
                                                   new SqlParameter("@LotSerialNumber", LotSerialNumber)  };

                lstItemLocationDetailsDTO = (from u in context.Database.SqlQuery<ItemLocationDetailsDTO>("EXEC GetItemLocByBinLotSerial @ItemGUID,@RoomID,@CompanyID,@BinNumber,@LotSerialNumber", params1)
                                             select new ItemLocationDetailsDTO
                                             {
                                                 ID = u.ID,

                                                 BinID = u.BinID,
                                                 CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                 ConsignedQuantity = u.ConsignedQuantity,
                                                 MeasurementID = u.MeasurementID,
                                                 LotNumber = u.LotNumber,
                                                 SerialNumber = u.SerialNumber,
                                                 ExpirationDate = u.ExpirationDate,
                                                 ReceivedDate = u.ReceivedDate,
                                                 Expiration = u.Expiration == null && u.DateCodeTracking ? DateTime.Now.ToString("MM /dd/yy") : u.Expiration,
                                                 Received = u.Received == null ? DateTime.Now.ToString("MM/dd/yy") : u.Received,
                                                 Cost = u.Cost,
                                                 eVMISensorPort = u.eVMISensorPort,
                                                 eVMISensorID = u.eVMISensorID,
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
                                                 TransferDetailGUID = u.TransferDetailGUID,
                                                 KitDetailGUID = u.KitDetailGUID,
                                                 CriticalQuantity = u.CriticalQuantity,
                                                 MinimumQuantity = u.MinimumQuantity,
                                                 MaximumQuantity = u.MaximumQuantity,
                                                 SuggestedOrderQuantity = (new CartItemDAL(base.DataBaseName)).GetSuggestedOrderQtyForBin(u.ItemGUID.GetValueOrDefault(Guid.Empty), u.BinID.GetValueOrDefault(0)),
                                                 AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                                                 EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                                                 ReceivedOn = u.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                                                 ReceivedOnWeb = u.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                                             }).ToList();

                return lstItemLocationDetailsDTO;
            }
        }

        public List<ItemLocationDetailsDTO> GetRecordsByBinNumberAndDateCode(Guid ItemGuid, string BinNumber, DateTime ExpirationDate, Int64 RoomID, Int64 CompanyID)
        {
            //Need to changes...
            //return GetCachedData(0,RoomID, CompanyID).Where(t => t.ID == id).SingleOrDefault();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<ItemLocationDetailsDTO> lstItemLocationDetailsDTO = new List<ItemLocationDetailsDTO>();
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGuid", ItemGuid),
                                                        new SqlParameter("@BinNumber", BinNumber),
                                                        new SqlParameter("@ExpirationDate", ExpirationDate),
                                                        new SqlParameter("@RoomID", RoomID),
                                                        new SqlParameter("@CompanyID", CompanyID ),
                                                        new SqlParameter("@IsStagingLocation", false) };
                lstItemLocationDetailsDTO = (from u in context.Database.SqlQuery<ItemLocationDetailsDTO>(@"exec GetRecordsByBinNumberAndDateCode @ItemGuid,@BinNumber,@ExpirationDate,@RoomID,@CompanyID,@IsStagingLocation", params1)
                                             select new ItemLocationDetailsDTO
                                             {
                                                 ID = u.ID,

                                                 BinID = u.BinID,
                                                 CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                 ConsignedQuantity = u.ConsignedQuantity,
                                                 MeasurementID = u.MeasurementID,
                                                 LotNumber = u.LotNumber,
                                                 SerialNumber = u.SerialNumber,
                                                 ExpirationDate = u.ExpirationDate,
                                                 ReceivedDate = u.ReceivedDate,
                                                 Expiration = u.Expiration == null && u.DateCodeTracking ? DateTime.Now.ToString("MM /dd/yy") : u.Expiration,
                                                 Received = u.Received == null ? DateTime.Now.ToString("MM/dd/yy") : u.Received,
                                                 Cost = u.Cost,
                                                 eVMISensorPort = u.eVMISensorPort,
                                                 eVMISensorID = u.eVMISensorID,
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
                                                 TransferDetailGUID = u.TransferDetailGUID,
                                                 KitDetailGUID = u.KitDetailGUID,
                                                 CriticalQuantity = u.CriticalQuantity,
                                                 MinimumQuantity = u.MinimumQuantity,
                                                 MaximumQuantity = u.MaximumQuantity,
                                                 SuggestedOrderQuantity = (new CartItemDAL(base.DataBaseName)).GetSuggestedOrderQtyForBin(u.ItemGUID.GetValueOrDefault(Guid.Empty), u.BinID.GetValueOrDefault(0)),
                                                 AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                                                 EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                                                 ReceivedOn = u.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                                                 ReceivedOnWeb = u.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                                             }).ToList();

                return lstItemLocationDetailsDTO;
            }
        }

        public bool CloseCountIfAllApplied(Guid InventoryCountGUID, long UserID, string EditedFrom)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    int OpenCount = (from A in context.InventoryCountDetails
                                     where A.InventoryCountGUID == InventoryCountGUID
                                           && A.IsDeleted == false && A.IsArchived == false
                                           && A.IsApplied == false
                                     select A.ID).Count();

                    if (OpenCount <= 0)
                    {
                        InventoryCount objInventoryCount = (from A in context.InventoryCounts
                                                            where A.GUID == InventoryCountGUID
                                                                  && A.IsDeleted == false && A.IsArchived == false
                                                            select A).FirstOrDefault();
                        if (objInventoryCount != null)
                        {
                            objInventoryCount.CountStatus = "C";
                            objInventoryCount.IsClosed = true;
                            objInventoryCount.EditedFrom = EditedFrom;
                            objInventoryCount.Updated = DateTime.UtcNow;
                            objInventoryCount.LastUpdatedBy = UserID;
                            context.SaveChanges();
                            return true;
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<ItemLocationDetailsDTO> GetRecordsByItemGuidsForLabels(string ItemGUID, Int64 RoomID, Int64 CompanyID)
        {
            //Get Cached-Media
            IEnumerable<ItemLocationDetailsDTO> ObjCache = null;
            if (ObjCache == null || ObjCache.Count() <= 0)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {

                    var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID),
                                                        new SqlParameter("@RoomID", RoomID),
                                                        new SqlParameter("@ItemGUID", ItemGUID ?? (object)DBNull.Value) };
                    ObjCache = (from u in context.Database.SqlQuery<ItemLocationDetailsDTO>(@"exec GetItemSerialAndLotList @CompanyID,@RoomID,@ItemGUID", params1)

                                select new ItemLocationDetailsDTO
                                {
                                    ID = u.ID,
                                    BinID = u.BinID,
                                    CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                    ConsignedQuantity = u.ConsignedQuantity,
                                    MeasurementID = u.MeasurementID,
                                    LotNumber = u.LotNumber,
                                    SerialNumber = u.SerialNumber,
                                    ExpirationDate = u.ExpirationDate,
                                    ReceivedDate = u.ReceivedDate,
                                    Expiration = u.Expiration,
                                    Received = u.Received == null ? DateTime.Now.ToString("MM-dd-yy") : u.Received,
                                    Cost = u.Cost,
                                    eVMISensorPort = u.eVMISensorPort,
                                    eVMISensorID = u.eVMISensorID,
                                    GUID = u.GUID,
                                    ItemGUID = u.ItemGUID,
                                    ItemID = u.ItemID,
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
                                    TransferDetailGUID = u.TransferDetailGUID,
                                    KitDetailGUID = u.KitDetailGUID,
                                    CriticalQuantity = u.CriticalQuantity,
                                    MinimumQuantity = u.MinimumQuantity,
                                    MaximumQuantity = u.MaximumQuantity,
                                    SuggestedOrderQuantity = (new CartItemDAL(base.DataBaseName)).GetSuggestedOrderQtyForBin(u.ItemGUID.GetValueOrDefault(Guid.Empty), u.BinID.GetValueOrDefault(0)),
                                    AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                                    EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                                    ReceivedOn = u.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                                    ReceivedOnWeb = u.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow),

                                }).AsParallel().ToList();

                }
            }

            return ObjCache;
        }

        public IEnumerable<ItemLocationDetailsDTO> GetRecordsByItemGuidsForLabels(string ItemGUID, Int64 RoomID, Int64 CompanyID, string BinIDs)
        {
            //Get Cached-Media
            IEnumerable<ItemLocationDetailsDTO> ObjCache = null;
            if (ObjCache == null || ObjCache.Count() <= 0)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {

                    var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID),
                                                        new SqlParameter("@RoomID", RoomID),
                                                        new SqlParameter("@ItemGUID", ItemGUID ?? (object)DBNull.Value),
                                                        new SqlParameter("@BinIDs", BinIDs ?? (object)DBNull.Value),
                                                        };
                    ObjCache = (from u in context.Database.SqlQuery<ItemLocationDetailsDTO>(@"exec GetItemBinSerialAndLotList @CompanyID,@RoomID,@ItemGUID,@BinIDs", params1)

                                select new ItemLocationDetailsDTO
                                {
                                    ID = u.ID,
                                    BinID = u.BinID,
                                    CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                    ConsignedQuantity = u.ConsignedQuantity,
                                    MeasurementID = u.MeasurementID,
                                    LotNumber = u.LotNumber,
                                    SerialNumber = u.SerialNumber,
                                    ExpirationDate = u.ExpirationDate,
                                    ReceivedDate = u.ReceivedDate,
                                    Expiration = u.Expiration,
                                    Received = u.Received == null ? DateTime.Now.ToString("MM-dd-yy") : u.Received,
                                    Cost = u.Cost,
                                    eVMISensorPort = u.eVMISensorPort,
                                    eVMISensorID = u.eVMISensorID,
                                    GUID = u.GUID,
                                    ItemGUID = u.ItemGUID,
                                    ItemID = u.ItemID,
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
                                    TransferDetailGUID = u.TransferDetailGUID,
                                    KitDetailGUID = u.KitDetailGUID,
                                    CriticalQuantity = u.CriticalQuantity,
                                    MinimumQuantity = u.MinimumQuantity,
                                    MaximumQuantity = u.MaximumQuantity,
                                    SuggestedOrderQuantity = (new CartItemDAL(base.DataBaseName)).GetSuggestedOrderQtyForBin(u.ItemGUID.GetValueOrDefault(Guid.Empty), u.BinID.GetValueOrDefault(0)),
                                    AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                                    EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                                    ReceivedOn = u.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                                    ReceivedOnWeb = u.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow),

                                }).AsParallel().ToList();

                }
            }

            return ObjCache;
        }

        public IEnumerable<ItemLocationDetailsDTO> GetRecordsByIDs(string IDs, Int64 RoomID, Int64 CompanyID)
        {
            //Get Cached-Media
            IEnumerable<ItemLocationDetailsDTO> ObjCache = null;
            if (ObjCache == null || ObjCache.Count() <= 0)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@IDs", IDs),
                                                       new SqlParameter("@RoomID", RoomID),
                                                       new SqlParameter("@CompanyID", CompanyID)  };

                    ObjCache = (from u in context.Database.SqlQuery<ItemLocationDetailsDTO>("EXEC [GetItemLocationDtlByIDs] @IDs,@RoomID,@CompanyID", params1)

                                select new ItemLocationDetailsDTO
                                {
                                    ID = u.ID,
                                    BinID = u.BinID,
                                    CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                    ConsignedQuantity = u.ConsignedQuantity,
                                    MeasurementID = u.MeasurementID,
                                    LotNumber = u.LotNumber,
                                    SerialNumber = u.SerialNumber,
                                    ExpirationDate = u.ExpirationDate,
                                    ReceivedDate = u.ReceivedDate,
                                    Expiration = u.Expiration,
                                    Received = u.Received == null ? DateTime.Now.ToString("MM-dd-yy") : u.Received,
                                    Cost = u.Cost,
                                    eVMISensorPort = u.eVMISensorPort,
                                    eVMISensorID = u.eVMISensorID,
                                    GUID = u.GUID,
                                    ItemGUID = u.ItemGUID,
                                    ItemID = u.ItemID,
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
                                    TransferDetailGUID = u.TransferDetailGUID,
                                    KitDetailGUID = u.KitDetailGUID,
                                    CriticalQuantity = u.CriticalQuantity,
                                    MinimumQuantity = u.MinimumQuantity,
                                    MaximumQuantity = u.MaximumQuantity,
                                    SuggestedOrderQuantity = (new CartItemDAL(base.DataBaseName)).GetSuggestedOrderQtyForBin(u.ItemGUID.GetValueOrDefault(Guid.Empty), u.BinID.GetValueOrDefault(0)),
                                    AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                                    EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                                    ReceivedOn = u.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                                    ReceivedOnWeb = u.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow),

                                }).AsParallel().ToList();

                }
            }

            return ObjCache;
        }

        public IEnumerable<ItemLocationDetailsDTO> GetRecordsByItemGuid(Guid ItemGUID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID) };

                IEnumerable<ItemLocationDetailsDTO> obj = (from u in context.Database.SqlQuery<ItemLocationDetailsDTO>("EXEC [GetItemLocationDtlByItemGUID]  @ItemGUID,@RoomID,@CompanyID", params1)
                                                           select new ItemLocationDetailsDTO
                                                           {
                                                               ID = u.ID,
                                                               BinID = u.BinID,
                                                               CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                               ConsignedQuantity = u.ConsignedQuantity,
                                                               MeasurementID = u.MeasurementID,
                                                               LotNumber = u.LotNumber,
                                                               SerialNumber = u.SerialNumber,
                                                               ExpirationDate = u.ExpirationDate,
                                                               ReceivedDate = u.ReceivedDate,
                                                               Expiration = u.Expiration == null && u.DateCodeTracking ? DateTime.Now.ToString("MM-dd-yy") : u.Expiration,
                                                               Received = u.Received == null ? DateTime.Now.ToString("MM-dd-yy") : u.Received,
                                                               Cost = u.Cost,
                                                               eVMISensorPort = u.eVMISensorPort,
                                                               eVMISensorID = u.eVMISensorID,
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
                                                               TransferDetailGUID = u.TransferDetailGUID,
                                                               KitDetailGUID = u.KitDetailGUID,
                                                               CriticalQuantity = u.CriticalQuantity,
                                                               MinimumQuantity = u.MinimumQuantity,
                                                               MaximumQuantity = u.MaximumQuantity,
                                                               SuggestedOrderQuantity = (new CartItemDAL(base.DataBaseName)).GetSuggestedOrderQtyForBin(u.ItemGUID.GetValueOrDefault(Guid.Empty), u.BinID.GetValueOrDefault(0)),
                                                               Markup = u.Markup,
                                                               AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                                                               EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                                                               ReceivedOn = u.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                                                               ReceivedOnWeb = u.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                                                           }).AsParallel().ToList();

                return obj;
            }
        }

        /// <summary>
        /// This method is used to get the count for specific serial number in ItemLocationDetails table
        /// </summary>
        /// <param name="ItemGUID"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        public long? GetCountForSerialItem(Guid itemGUID, long roomID, long companyID, string serialNumber)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@ItemGUID", itemGUID),
                                                    new SqlParameter("@CompanyID", companyID),
                                                    new SqlParameter("@RoomId", roomID),
                                                    new SqlParameter("@SerialNumber", serialNumber)
                                                };
                return context.Database.SqlQuery<long?>("exec [GetSerialQuantity] @ItemGUID,@CompanyID,@RoomId,@SerialNumber ", params1).FirstOrDefault();
            }
        }


        #region MS Credit

        public bool MaterialStagingPoolDetailsSaveForMSCreditPullnew(List<MaterialStagingPullDetailDTO> objData, string calledfrom, string RoomDateFormat, out List<CreditHistory> lstCreditGuids, long SessionUserId, long EnterpriseId, string PullAction = "ms credit", string WhatWhereAction = "")
        {
            lstCreditGuids = new List<CreditHistory>();
            if (!string.IsNullOrWhiteSpace(calledfrom))
            {
                if ("evmisvc" == calledfrom)
                {
                    PullAction = "mscr_evmi";
                }
            }

            bool TempSerialNumberTracking = false;
            bool TempLotNumberTracking = false;
            //bool Consignment = false;
            ItemMasterDTO objItemDTO = null;
            IEnumerable<MaterialStagingDetailDTO> lstItemDTO = null;
            MaterialStagingDetailDAL objItemDAL = new MaterialStagingDetailDAL(base.DataBaseName);
            bool IsQTYSave = false;
            bool IsBothQty = false;
            string CountdtlGUID = string.Empty;
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                int LooptotalCount = objData.Count();
                int Counter = 0;
                foreach (MaterialStagingPullDetailDTO item in objData)
                {
                    BinMasterDTO Itembin = GetItemBin(item.ItemGUID, item.BinNumber, item.Room ?? 0, item.CompanyID ?? 0, item.CreatedBy ?? 0, true);
                    item.BinID = Itembin.ID;
                }
                List<PullDetailsDTO> objPullDetailsDTOs = new List<PullDetailsDTO>();
                foreach (MaterialStagingPullDetailDTO item in objData)
                {
                    #region "Location Detail Save"
                    Counter++;

                    if (item.BinID == null)
                    {
                        continue;
                    }
                    if (objItemDTO == null)
                    {
                        objItemDTO = new ItemMasterDAL(base.DataBaseName).GetItemWithoutJoins(0, item.ItemGUID);
                    }

                    lstItemDTO = objItemDAL.GetAllRecordsRoomItemWise(item.Room ?? 0, item.CompanyID ?? 0, false, false, item.ItemGUID);

                    item.IsArchived = (item.IsArchived == null ? false : item.IsArchived);
                    item.IsDeleted = (item.IsDeleted == null ? false : item.IsDeleted);
                    BinMasterDAL objBin = new BinMasterDAL(base.DataBaseName);
                    BinMasterDTO objBinDTO = objBin.GetBinByID(Convert.ToInt64(item.BinID), Convert.ToInt64(item.Room), Convert.ToInt64(item.CompanyID));
                    //BinMasterDTO objBinDTO = objBin.GetItemLocation(Convert.ToInt64(item.Room), Convert.ToInt64(item.CompanyID), false, false,Guid.Empty, Convert.ToInt64(item.BinID), null,null).FirstOrDefault();

                    if (string.IsNullOrEmpty(item.Received) && item.ReceivedDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                        item.Received = item.ReceivedDate == null ? string.Empty : item.ReceivedDate.Value.ToString("M/d/yyyy");

                    bool IsValid = false;

                    if (!string.IsNullOrEmpty(item.Received) || item.ReceivedDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                        IsValid = true;
                    else if (!string.IsNullOrEmpty(item.SerialNumber))
                    {
                        IsValid = true;
                    }
                    else if (!string.IsNullOrEmpty(item.LotNumber))
                        IsValid = true;
                    else if (!string.IsNullOrEmpty(item.Expiration) || (item.ExpirationDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue))
                        IsValid = true;
                    else if (item.LotNumberTracking && item.SerialNumberTracking)
                        IsValid = true;
                    else if (item.BinID.HasValue && item.Cost.HasValue && (!string.IsNullOrEmpty(item.Received) || item.ReceivedDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue))
                        IsValid = true;

                    if (IsValid || (LooptotalCount == Counter))
                    {
                        //if (!String.IsNullOrEmpty(item.SerialNumber) || !String.IsNullOrEmpty(item.LotNumber) || item.CustomerOwnedQuantity.Value > 0 || item.ConsignedQuantity.Value > 0)
                        //{
                        double CustVal = 0;
                        double ConsignVal = 0;
                        bool IsConsinAndCustBothQty = false;
                        if (item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0 && item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                        {
                            CustVal = item.CustomerOwnedQuantity.GetValueOrDefault(0);
                            ConsignVal = item.ConsignedQuantity.GetValueOrDefault(0);
                            IsConsinAndCustBothQty = true;
                            IsBothQty = true;
                        }
                        else if (item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0 && item.ConsignedQuantity.GetValueOrDefault(0) <= 0)
                        {
                            CustVal = item.CustomerOwnedQuantity.GetValueOrDefault(0);
                            ConsignVal = 0;
                            IsConsinAndCustBothQty = false;
                            IsBothQty = false;
                        }
                        else if (item.ConsignedQuantity.GetValueOrDefault(0) > 0 && item.CustomerOwnedQuantity.GetValueOrDefault(0) <= 0)
                        {
                            ConsignVal = item.ConsignedQuantity.GetValueOrDefault(0);
                            CustVal = 0;
                            IsBothQty = false;
                            IsConsinAndCustBothQty = false;
                        }

                        if (IsConsinAndCustBothQty)
                        {
                            item.ConsignedQuantity = ConsignVal;
                            item.CustomerOwnedQuantity = 0;
                            SaveBothCustAndConsigenQtyForCountForMS(item, objItemDTO, objBinDTO, IsValid, PullAction, lstItemDTO, RoomDateFormat);

                            item.ConsignedQuantity = 0;
                            item.CustomerOwnedQuantity = CustVal;
                            item.ID = 0;
                            item.GUID = Guid.Empty;
                            SaveBothCustAndConsigenQtyForCountForMS(item, objItemDTO, objBinDTO, IsValid, PullAction, lstItemDTO, RoomDateFormat);
                            continue;
                        }
                        else
                        {
                            MaterialStagingPullDetailDAL objItemDetailDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
                            MaterialStagingPullDetailDTO objItem = new MaterialStagingPullDetailDTO();

                            if (item.MaterialStagingGUID == null || item.MaterialStagingGUID == Guid.Empty)
                            {
                                // RoomDAL objRoomDal = new RoomDAL(base.DataBaseName);
                                // RoomDTO objRoom = new RoomDTO();
                                CommonDAL objCommonDal = new CommonDAL(base.DataBaseName);
                                string columnList = "ID,RoomName,IsIgnoreCreditRule";
                                RoomDTO objRoom = objCommonDal.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + item.Room.GetValueOrDefault(0).ToString() + "", "");
                                // objRoom = objRoomDal.GetRoomByIDPlain(item.Room.GetValueOrDefault(0));

                                if (objRoom != null && objRoom.IsIgnoreCreditRule)
                                {
                                    if (item.MaterialStagingdtlGUID != null && item.MaterialStagingdtlGUID != Guid.Empty)
                                    {
                                        MaterialStagingDetailDTO objMSDTO = new MaterialStagingDetailDTO();
                                        //objMSDTO = new MaterialStagingDetailDAL(base.DataBaseName).GetRecordwithoutCaching(item.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty), item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0));
                                        objMSDTO = new MaterialStagingDetailDAL(base.DataBaseName).GetMaterialStagingDetailByGUID(item.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty), item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0));
                                        if (objMSDTO != null)
                                        {
                                            item.MaterialStagingGUID = objMSDTO.MaterialStagingGUID;
                                        }
                                    }
                                }
                            }

                            objItem.MaterialStagingGUID = item.MaterialStagingGUID;
                            objItem.MaterialStagingdtlGUID = item.MaterialStagingdtlGUID;
                            objItem.BinID = null;
                            if (objBinDTO != null)
                            {
                                objItem.StagingBinId = objBinDTO.ID;
                            }
                            else
                                objItem.StagingBinId = item.BinID ?? 0;

                            objItem.CompanyID = item.CompanyID;
                            objItem.Cost = item.Cost;
                            objItem.Created = item.Created;
                            objItem.CustomerOwnedQuantity = item.CustomerOwnedQuantity;
                            objItem.ConsignedQuantity = item.ConsignedQuantity;

                            objItem.InitialQuantity = (item.ConsignedQuantity ?? 0) + (item.CustomerOwnedQuantity ?? 0);
                            objItem.InitialQuantityWeb = (item.ConsignedQuantity ?? 0) + (item.CustomerOwnedQuantity ?? 0);
                            //objItem.IsConsignedSerialLot = objItemDTO.Consignment;

                            // TODO: - Expiration date should be null if item is not date code trakcing.

                            // objItem.ExpirationDate = item.ExpirationDate;
                            if (item.DateCodeTracking)
                            {
                                objItem.Expiration = item.Expiration;
                                objItem.ExpirationDate = item.ExpirationDate;
                            }

                            objItem.Received = item.Received;
                            objItem.ReceivedDate = item.ReceivedDate;
                            if (objItem.ReceivedDate.GetValueOrDefault(DateTime.MinValue) <= DateTime.MinValue && !string.IsNullOrEmpty(objItem.Received))
                            {
                                objItem.ReceivedDate = DateTime.Parse(objItem.Received);
                            }
                            if (item.DateCodeTracking && objItem.ExpirationDate.GetValueOrDefault(DateTime.MinValue) <= DateTime.MinValue && !string.IsNullOrEmpty(objItem.Expiration))
                            {
                                DateTime dt = new DateTime();
                                if (DateTime.TryParseExact(objItem.Expiration, RoomDateFormat, ResourceHelper.CurrentCult, DateTimeStyles.None, out dt))
                                    objItem.ExpirationDate = dt;
                            }

                            objItem.ID = item.ID; objItem.ItemGUID = item.ItemGUID;

                            objItem.LotNumber = item.LotNumber;
                            if (item.LotNumberTracking)
                                TempLotNumberTracking = true;
                            objItem.SerialNumber = item.SerialNumber;
                            //if (item.SerialNumber != null && item.SerialNumberTracking)
                            if (item.SerialNumberTracking)
                                TempSerialNumberTracking = true;

                            objItem.CreatedBy = item.CreatedBy; objItem.CompanyID = item.CompanyID;
                            objItem.Room = item.Room; objItem.Updated = item.Updated;
                            objItem.LastUpdatedBy = item.LastUpdatedBy;
                            objItem.IsArchived = item.IsArchived; objItem.IsDeleted = item.IsDeleted;
                            objItem.Created = item.Created; objItem.CreatedBy = item.CreatedBy;
                            objItem.OrderDetailGUID = item.OrderDetailGUID;

                            if (objBinDTO != null)
                                objItem.BinNumber = objBinDTO.BinNumber;

                            objItem.ItemLocationDetailGUID = item.ItemLocationDetailGUID;
                            objItem.ItemCost = item.Cost;
                            //objItem.PoolQuantity = item.PoolQuantity;
                            objItem.PoolQuantity = (item.ConsignedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0));
                            objItem.PullCredit = PullAction;
                            if (item.ID > 0 && IsValid)
                            {
                                objItem.GUID = item.GUID;
                                objItem.IsOnlyFromUI = item.IsOnlyFromUI;
                                if (item.IsOnlyFromUI)
                                {
                                    objItem.EditedFrom = "Web";
                                    objItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                                }
                                objItemDetailDAL.Edit(objItem);
                                // here needs to implement the edit quantity logic for location details table
                                IsQTYSave = true;
                            }
                            else
                            {
                                if (IsValid)
                                {
                                    bool IsRecordSave = false;
                                    //// here needs to write code for 4 points as following...
                                    // 1 // if item is SR. then check for duplication if found then make them IsDeleted = 1
                                    //   // And then Insert the New item with same SR. number
                                    // 2 // If item is Lot, then check if duplicate number then only update the quantity
                                    //   // for that particular lot number also check BinLocation if exist then update else insert ....
                                    // 3 // Needs to enter only one entry to PullMaster with Total Quantity and data
                                    // 4 // Then needs to enter all the entry to PullDetails with itemlocation data ..
                                    #region "SR Case"
                                    if (TempSerialNumberTracking)
                                    {
                                        IEnumerable<MaterialStagingPullDetailDTO> TempDupSrs = objItemDetailDAL.GetAllRecordsByItemGuid(item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0), item.ItemGUID, item.SerialNumber).ToList();
                                        if (TempDupSrs != null && TempDupSrs.Count() > 0)
                                        {
                                            foreach (MaterialStagingPullDetailDTO IDTemp in TempDupSrs)
                                            {
                                                IDTemp.IsDeleted = true;
                                                IDTemp.IsArchived = true;
                                                IDTemp.LastUpdatedBy = item.LastUpdatedBy;
                                                IDTemp.Updated = DateTimeUtility.DateTimeNow;
                                                IDTemp.IsOnlyFromUI = true;
                                                IDTemp.EditedFrom = "Web";
                                                IDTemp.ReceivedOn = DateTimeUtility.DateTimeNow;
                                                objItemDetailDAL.Edit(IDTemp);
                                            }
                                        }
                                    }
                                    #endregion
                                    if (!IsRecordSave)
                                    {
                                        objItem.AddedFrom = "Web";
                                        objItem.EditedFrom = "Web";
                                        objItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objItem.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                        objItem.IsOnlyFromUI = item.IsOnlyFromUI;
                                        MaterialStagingPullDetailDTO itemloctemp = objItemDetailDAL.InsertMS(true, objItem, RoomDateFormat);
                                        item.ID = itemloctemp.ID;
                                        item.GUID = itemloctemp.GUID;
                                        IsQTYSave = true;
                                    }
                                    #region "PULL Details Entry"
                                    PullDetailsDTO objPullDetailDTO = new PullDetailsDTO();
                                    objPullDetailDTO.BinID = item.BinID;
                                    objPullDetailDTO.CompanyID = item.CompanyID;
                                    objPullDetailDTO.ConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0);
                                    //objPullDetailDTO.CreditConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0);
                                    objPullDetailDTO.Created = DateTimeUtility.DateTimeNow;
                                    objPullDetailDTO.CreatedBy = item.CreatedBy.GetValueOrDefault(0);
                                    objPullDetailDTO.CreatedByName = item.CreatedByName;
                                    objPullDetailDTO.CustomerOwnedQuantity = item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    //objPullDetailDTO.CreditCustomerOwnedQuantity = item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    objPullDetailDTO.Expiration = item.Expiration;
                                    objPullDetailDTO.IsArchived = item.IsArchived;
                                    objPullDetailDTO.IsDeleted = item.IsDeleted;
                                    objPullDetailDTO.ItemCost = CalculateAndGetPullCreditCost(item.Cost, objItemDTO.CostUOMID, item.Room ?? 0, item.CompanyID ?? 0);
                                    objPullDetailDTO.ItemPrice = CalculateAndGetPullCreditCost(item.SellPrice, objItemDTO.CostUOMID, item.Room ?? 0, item.CompanyID ?? 0);
                                    objPullDetailDTO.ItemGUID = item.ItemGUID;
                                    objPullDetailDTO.LastUpdatedBy = item.LastUpdatedBy.GetValueOrDefault(0);
                                    objPullDetailDTO.LotNumber = item.LotNumber;
                                    //objPullDetailDTO.PoolQuantity = item.PoolQuantity;
                                    objPullDetailDTO.PoolQuantity = (item.ConsignedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0));
                                    objPullDetailDTO.PullCredit = PullAction;
                                    objPullDetailDTO.Received = item.Received;
                                    objPullDetailDTO.Room = item.Room;
                                    objPullDetailDTO.RoomName = item.RoomName;
                                    objPullDetailDTO.SerialNumber = item.SerialNumber;
                                    objPullDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                                    objPullDetailDTO.UpdatedByName = item.UpdatedByName;
                                    //objPullDetailDTO.ItemLocationDetailGUID = item.GUID;
                                    objPullDetailDTO.MaterialStagingPullDetailGUID = item.GUID;
                                    objPullDetailDTO.ProjectSpendGUID = item.ProjectSpentGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : item.ProjectSpentGUID;
                                    objPullDetailDTO.WorkOrderGUID = item.WorkOrderGuid;
                                    objPullDetailDTO.COuntLineItemGUID = item.CountLineItemDtlGUID;

                                    if (string.IsNullOrEmpty(item.AddedFrom) || string.IsNullOrWhiteSpace(item.AddedFrom))
                                        objPullDetailDTO.AddedFrom = "Web";
                                    else
                                        objPullDetailDTO.AddedFrom = item.AddedFrom;

                                    if (string.IsNullOrEmpty(item.EditedFrom) || string.IsNullOrWhiteSpace(item.EditedFrom))
                                        objPullDetailDTO.EditedFrom = "Web";
                                    else
                                        objPullDetailDTO.EditedFrom = item.EditedFrom;

                                    objPullDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    objPullDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                    objPullDetailDTO.PullOrderNumber = item.PullOrderNumber;
                                    objPullDetailDTO.SupplierAccountGuid = item.SupplierAccountGuid;
                                    objPullDetailDTO.GUID = Guid.NewGuid();
                                    CreditHistory objCreditHistory = new CreditHistory();
                                    objCreditHistory.PULLGUID = item.PullGUIDForCreditHistory ?? Guid.Empty;
                                    objCreditHistory.PullDetailGuid = item.PullDetailGUIDForCreditHistory ?? Guid.Empty;

                                    objCreditHistory.CreditDetailGuid = objPullDetailDTO.GUID;

                                    lstCreditGuids.Add(objCreditHistory);

                                    objPullDetailsDTOs.Add(objPullDetailDTO);
                                    #endregion
                                }
                            }
                            //Update staged quantity...
                            objItemDetailDAL.UpdateStagedQuantity(item.ItemGUID, item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0), SessionUserId, EnterpriseId);

                        }
                    }
                    #endregion
                }

                #region "PULL Master and Detail Save"

                PullMasterDAL objPullDAL = new PullMasterDAL(base.DataBaseName);
                PullMasterViewDTO objPullDTO = new PullMasterViewDTO();
                if (objPullDetailsDTOs.Count > 0)
                {
                    objPullDTO.ActionType = PullAction;
                    objPullDTO.BinID = objPullDetailsDTOs[0].BinID;
                    objPullDTO.CompanyID = objPullDetailsDTOs[0].CompanyID;
                    objPullDTO.ConsignedQuantity = objPullDetailsDTOs.Select(x => x.ConsignedQuantity.GetValueOrDefault(0)).Sum();
                    objPullDTO.CustomerOwnedQuantity = objPullDetailsDTOs.Select(x => x.CustomerOwnedQuantity.GetValueOrDefault(0)).Sum();

                    //objPullDTO.CreditConsignedQuantity = objPullDetailsDTOs.Select(x => x.ConsignedQuantity.GetValueOrDefault(0)).Sum();
                    //objPullDTO.CreditCustomerOwnedQuantity = objPullDetailsDTOs.Select(x => x.CustomerOwnedQuantity.GetValueOrDefault(0)).Sum();

                    objPullDTO.Created = DateTimeUtility.DateTimeNow;
                    objPullDTO.CreatedBy = objPullDetailsDTOs[0].CreatedBy.GetValueOrDefault(0);
                    objPullDTO.CreatedByName = objPullDetailsDTOs[0].CreatedByName;
                    objPullDTO.GUID = Guid.NewGuid();

                    lstCreditGuids.ForEach(b => b.CreditGuid = objPullDTO.GUID);

                    objPullDTO.IsArchived = false;
                    objPullDTO.IsDeleted = false;
                    objPullDTO.ItemGUID = objData[0].ItemGUID;
                    objPullDTO.ItemNumber = objData[0].ItemNumber;
                    objPullDTO.ItemType = objData[0].ItemType;
                    objPullDTO.LastUpdatedBy = objPullDetailsDTOs[0].LastUpdatedBy.GetValueOrDefault(0);
                    objPullDTO.PoolQuantity = objPullDetailsDTOs.Select(x => x.PoolQuantity.GetValueOrDefault(0)).Sum();
                    objPullDTO.PullCredit = PullAction;
                    objPullDTO.PullCost = Convert.ToDouble(objPullDetailsDTOs.Select(x => x.ItemCost.GetValueOrDefault(0) * x.PoolQuantity.GetValueOrDefault(0)).Sum());
                    objPullDTO.PullPrice = Convert.ToDouble(objPullDetailsDTOs.Select(x => x.ItemPrice.GetValueOrDefault(0) * x.PoolQuantity.GetValueOrDefault(0)).Sum());
                    objPullDTO.Room = objPullDetailsDTOs[0].Room;
                    objPullDTO.RoomName = objPullDetailsDTOs[0].RoomName;
                    objPullDTO.Updated = DateTimeUtility.DateTimeNow;
                    objPullDTO.UpdatedByName = objPullDetailsDTOs[0].UpdatedByName;
                    objPullDTO.ProjectSpendGUID = objPullDetailsDTOs[0].ProjectSpendGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : objPullDetailsDTOs[0].ProjectSpendGUID;
                    objPullDTO.WorkOrderDetailGUID = objPullDetailsDTOs[0].WorkOrderGUID;
                    objPullDTO.WorkOrderGUID = objPullDetailsDTOs[0].WorkOrderGUID;
                    objPullDTO.CountLineItemGuid = objPullDetailsDTOs[0].COuntLineItemGUID;
                    objPullDTO.WhatWhereAction = "MS Credit Pull";
                    objPullDTO.SupplierAccountGuid = objPullDetailsDTOs[0].SupplierAccountGuid;
                    if (objData != null && objData.Count > 0)
                    {
                        objPullDTO.UDF1 = objData[0].UDF1 != null ? objData[0].UDF1.Trim() : objData[0].UDF1;
                        objPullDTO.UDF2 = objData[0].UDF2 != null ? objData[0].UDF2.Trim() : objData[0].UDF2;
                        objPullDTO.UDF3 = objData[0].UDF3 != null ? objData[0].UDF3.Trim() : objData[0].UDF3;
                        objPullDTO.UDF4 = objData[0].UDF4 != null ? objData[0].UDF4.Trim() : objData[0].UDF4;
                        objPullDTO.UDF5 = objData[0].UDF5 != null ? objData[0].UDF5.Trim() : objData[0].UDF5;
                    }

                    if (string.IsNullOrEmpty(objPullDetailsDTOs[0].AddedFrom) || string.IsNullOrWhiteSpace(objPullDetailsDTOs[0].AddedFrom))
                        objPullDTO.AddedFrom = "Web";
                    else
                        objPullDTO.AddedFrom = objPullDetailsDTOs[0].AddedFrom;

                    if (string.IsNullOrEmpty(objPullDetailsDTOs[0].EditedFrom) || string.IsNullOrWhiteSpace(objPullDetailsDTOs[0].EditedFrom))
                        objPullDTO.EditedFrom = "Web";
                    else
                        objPullDTO.EditedFrom = objPullDetailsDTOs[0].EditedFrom;

                    objPullDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objPullDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objPullDTO.PullOrderNumber = objPullDetailsDTOs[0].PullOrderNumber;
                    if (objPullDTO.ActionType != "adjmscredit" //&& objItemDTO.Consignment
                        )
                    {
                        if (string.IsNullOrWhiteSpace(objPullDTO.PullOrderNumber))
                        {
                            AutoOrderNumberGenerate objAutoOrderNumberGenerate = new AutoSequenceDAL(base.DataBaseName).GetNextPullOrderNumber(objPullDTO.Room ?? 0, objPullDTO.CompanyID ?? 0, objItemDTO.SupplierID ?? 0, objItemDTO.GUID, EnterpriseId, null, false);
                            if (objAutoOrderNumberGenerate != null && !string.IsNullOrWhiteSpace(objAutoOrderNumberGenerate.OrderNumber))
                            {
                                objPullDTO.PullOrderNumber = objAutoOrderNumberGenerate.OrderNumber;
                            }
                            else
                            {
                                DateTime datetimetoConsider = new RegionSettingDAL(base.DataBaseName).GetCurrentDatetimebyTimeZone(objPullDTO.Room ?? 0, objPullDTO.CompanyID ?? 0, 0);
                                objPullDTO.PullOrderNumber = datetimetoConsider.ToString("yyyyMMdd");
                            }

                        }
                    }

                    objPullDTO.ID = objPullDAL.Insert(objPullDTO);
                    PullDetailsDAL objPullDetailsDAL = new PullDetailsDAL(base.DataBaseName);
                    foreach (PullDetailsDTO temppdDTO in objPullDetailsDTOs)
                    {
                        temppdDTO.PULLGUID = objPullDTO.GUID;

                        if (string.IsNullOrEmpty(temppdDTO.AddedFrom) || string.IsNullOrWhiteSpace(temppdDTO.AddedFrom))
                            temppdDTO.AddedFrom = "Web";

                        if (string.IsNullOrEmpty(temppdDTO.EditedFrom) || string.IsNullOrWhiteSpace(temppdDTO.EditedFrom))
                            temppdDTO.EditedFrom = "Web";


                        temppdDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        temppdDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objPullDetailsDAL.Insert(temppdDTO);


                        //Project Spend update
                        if (temppdDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                        {
                            ProjectMasterDAL objPrjMsgDAL = new ProjectMasterDAL(base.DataBaseName);
                            ProjectSpendItemsDAL objPrjSpenItmDAL = new ProjectSpendItemsDAL(base.DataBaseName);
                            ProjectMasterDTO objPrjMstDTO = new ProjectMasterDTO();
                            ProjectSpendItemsDTO objPrjSpenItmDTO = new ProjectSpendItemsDTO();
                            var tmpsupplierIds = new List<long>();
                            //objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecords(objPullDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)objPullDetailsDTOs[0].Room, (Int64)objPullDetailsDTOs[0].CompanyID, tmpsupplierIds).Where(x => x.ItemGUID == objPullDTO.ItemGUID && x.Created.Value <= temppdDTO.Created.Value).SingleOrDefault();
                            objPrjSpenItmDTO = objPrjSpenItmDAL.GetProjectSpendItemCompareDate(objPullDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)objPullDetailsDTOs[0].Room, (Int64)objPullDetailsDTOs[0].CompanyID, tmpsupplierIds, Convert.ToString(objPullDTO.ItemGUID), temppdDTO.Created.Value.ToString("yyyy-MM-dd HH:mm:ss")).SingleOrDefault();
                            objPrjMstDTO = objPrjMsgDAL.GetRecord(objPullDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)objPullDetailsDTOs[0].Room, (Int64)objPullDetailsDTOs[0].CompanyID, false, false);

                            if (objPrjMstDTO != null)
                            {

                                objPrjMstDTO.DollarUsedAmount = objPrjMstDTO.DollarUsedAmount.GetValueOrDefault(0) - ((decimal)temppdDTO.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(temppdDTO.ItemCost.GetValueOrDefault(0)));
                                objPrjMstDTO.WhatWhereAction = WhatWhereAction;
                                //objPrjMstDTO.EditedFrom = "Web";
                                //objPrjMstDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objPrjMsgDAL.Edit(objPrjMstDTO);

                                if (objPrjSpenItmDTO != null)
                                {

                                    objPrjSpenItmDTO.DollarUsedAmount = objPrjSpenItmDTO.DollarUsedAmount.GetValueOrDefault(0) - ((decimal)temppdDTO.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(temppdDTO.ItemCost.GetValueOrDefault(0)));
                                    objPrjSpenItmDTO.QuantityUsed = objPrjSpenItmDTO.QuantityUsed.GetValueOrDefault(0) - temppdDTO.PoolQuantity.GetValueOrDefault(0);
                                    //objPrjSpenItmDTO.EditedFrom = "Web";
                                    //objPrjSpenItmDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    objPrjSpenItmDAL.Edit(objPrjSpenItmDTO);
                                }
                            }
                        }

                    }
                }
                #endregion
                if (!IsBothQty)
                {
                    // if (objItemDTO != null && objItemDTO.ItemType != 4)
                    if (objItemDTO != null)
                    {
                        #region "Insert QTY logic"
                        if (IsQTYSave)
                        {
                            MaterialStagingDetailDTO objMaterialStagingDetailDTO = new MaterialStagingDetailDTO();

                            if (TempSerialNumberTracking)
                            {
                                #region "Insert mode for Serial Number"
                                MaterialStagingDetailDAL objQtyMgt = new MaterialStagingDetailDAL(base.DataBaseName);

                                //FOR INSERT MODE
                                var objilqInsert = new List<MaterialStagingDetailDTO>();

                                objilqInsert = (from x in objData
                                                where (x.SerialNumberTracking == true && x.SerialNumber != null) && x.BinID != null
                                                group x by new { x.BinID }
                                                    into grp
                                                select new MaterialStagingDetailDTO
                                                {
                                                    MaterialStagingGUID = objData[0].MaterialStagingGUID,
                                                    BinID = null,
                                                    StagingBinID = objData[0].StagingBinId,
                                                    StagingBinName = objData[0].BinNumber,
                                                    StagingBinGUID = objData[0].GUID,
                                                    ItemGUID = objData[0].ItemGUID,
                                                    CompanyID = objData[0].CompanyID ?? 0,
                                                    RoomId = objData[0].Room ?? 0,
                                                    Updated = DateTime.Now,
                                                    Created = DateTime.Now,
                                                    CreatedBy = objData[0].CreatedBy ?? 0,
                                                    LastUpdatedBy = objData[0].LastUpdatedBy ?? 0,
                                                    CustomerOwnedQuantity = Convert.ToInt16(grp.Sum(x => x.CustomerOwnedQuantity ?? 0)),
                                                    ConsignedQuantity = Convert.ToInt16(grp.Sum(x => x.ConsignedQuantity ?? 0)),
                                                    Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                                }).ToList();

                                if (objilqInsert.Count() > 0)
                                {
                                    foreach (MaterialStagingDetailDTO objDataInsert in objilqInsert)
                                    {
                                        objMaterialStagingDetailDTO = objQtyMgt.SaveMaterialStagingDetailsForMSCredit(objDataInsert);
                                    }
                                }
                                #endregion
                            }
                            else if (TempLotNumberTracking)
                            {
                                #region "Insert Mode for Lot#, Others"
                                MaterialStagingDetailDAL objQtyMgt = new MaterialStagingDetailDAL(base.DataBaseName);
                                if (true //objItemDTO.Consignment
                                    )
                                {
                                    var objilq = (from x in objData
                                                  where (x.LotNumberTracking == true && x.LotNumber != null) && x.BinID != null
                                                  group x by new { x.BinID } into grp
                                                  select new MaterialStagingDetailDTO
                                                  {
                                                      MaterialStagingGUID = objData[0].MaterialStagingGUID,
                                                      BinID = null,
                                                      StagingBinID = objData[0].StagingBinId,
                                                      StagingBinName = objData[0].BinNumber,
                                                      StagingBinGUID = objData[0].GUID,
                                                      ItemGUID = objData[0].ItemGUID,
                                                      CompanyID = objData[0].CompanyID ?? 0,
                                                      RoomId = objData[0].Room ?? 0,
                                                      Updated = DateTime.Now,
                                                      Created = DateTime.Now,
                                                      CreatedBy = objData[0].CreatedBy ?? 0,
                                                      LastUpdatedBy = objData[0].LastUpdatedBy ?? 0,
                                                      CustomerOwnedQuantity = Convert.ToInt32(grp.Sum(x => x.CustomerOwnedQuantity ?? 0)),
                                                      ConsignedQuantity = Convert.ToInt32(grp.Sum(x => x.ConsignedQuantity ?? 0)),
                                                      Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0)
                                                      //LotNumber = x.LotNumber,
                                                  }).ToList();
                                    //Save
                                    if (objilq.Count() > 0)
                                    {
                                        foreach (MaterialStagingDetailDTO objDataInsertLot in objilq)
                                        {
                                            objMaterialStagingDetailDTO = objQtyMgt.SaveMaterialStagingDetailsForMSCredit(objDataInsertLot);
                                        }
                                    }
                                    //Update on_hand qunatity
                                    //objilq.Select(c => { OnHandQuantity += c.Quantity; return c; }).ToList();
                                }
                                else
                                {
                                    //var objilq = (from x in objData
                                    //              where (x.LotNumberTracking == true && x.LotNumber != null) && x.BinID != null
                                    //              group x by new { x.BinID } into grp
                                    //              select new MaterialStagingDetailDTO
                                    //              {
                                    //                  MaterialStagingGUID = objData[0].MaterialStagingGUID,
                                    //                  BinID = null,
                                    //                  StagingBinID = objData[0].StagingBinId,
                                    //                  StagingBinName = objData[0].BinNumber,
                                    //                  StagingBinGUID = objData[0].GUID,
                                    //                  ItemGUID = objData[0].ItemGUID,
                                    //                  CompanyID = objData[0].CompanyID ?? 0,
                                    //                  RoomId = objData[0].Room ?? 0,
                                    //                  Updated = DateTime.Now,
                                    //                  Created = DateTime.Now,
                                    //                  CreatedBy = objData[0].CreatedBy ?? 0,
                                    //                  LastUpdatedBy = objData[0].LastUpdatedBy ?? 0,
                                    //                  CustomerOwnedQuantity = Convert.ToInt32(grp.Sum(x => x.CustomerOwnedQuantity ?? 0)),
                                    //                  ConsignedQuantity = Convert.ToInt32(grp.Sum(x => x.ConsignedQuantity ?? 0)),
                                    //                  Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                    //                  //LotNumber = x.LotNumber,
                                    //              }).ToList();

                                    ////Save
                                    //if (objilq.Count() > 0)
                                    //{
                                    //    foreach (MaterialStagingDetailDTO objDataInsertLot in objilq)
                                    //    {
                                    //        objMaterialStagingDetailDTO = objQtyMgt.SaveMaterialStagingDetailsForMSCredit(objDataInsertLot);
                                    //    }
                                    //}
                                }
                                //Save on hand quantity
                                #endregion
                            }
                            else
                            {
                                #region "Insert Mode for Lot#, Others"
                                MaterialStagingDetailDAL objQtyMgt = new MaterialStagingDetailDAL(base.DataBaseName);
                                if (true//objItemDTO.Consignment
                                    )
                                {
                                    var objilq = (from x in objData
                                                  where x.BinID != null
                                                  group x by new { x.BinID } into grp
                                                  select new MaterialStagingDetailDTO
                                                  {
                                                      MaterialStagingGUID = objData[0].MaterialStagingGUID,
                                                      BinID = null,
                                                      StagingBinID = objData[0].StagingBinId,
                                                      StagingBinName = objData[0].BinNumber,
                                                      StagingBinGUID = objData[0].GUID,
                                                      ItemGUID = objData[0].ItemGUID,
                                                      CompanyID = objData[0].CompanyID ?? 0,
                                                      RoomId = Convert.ToInt32(objData[0].Room),
                                                      Updated = DateTime.Now,
                                                      Created = DateTime.Now,
                                                      CreatedBy = objData[0].CreatedBy ?? 0,
                                                      LastUpdatedBy = objData[0].LastUpdatedBy ?? 0,
                                                      CustomerOwnedQuantity = Convert.ToInt32(grp.Sum(x => x.CustomerOwnedQuantity ?? 0)),
                                                      ConsignedQuantity = Convert.ToInt32(grp.Sum(x => x.ConsignedQuantity ?? 0)),
                                                      Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0)
                                                      //LotNumber = x.LotNumber,
                                                  }).ToList();
                                    //Save
                                    if (objilq.Count() > 0)
                                    {
                                        foreach (MaterialStagingDetailDTO objDataInsertLot in objilq)
                                        {
                                            objMaterialStagingDetailDTO = objQtyMgt.SaveMaterialStagingDetailsForMSCredit(objDataInsertLot);
                                        }
                                    }
                                    //Update on_hand qunatity
                                    //objilq.Select(c => { OnHandQuantity += c.Quantity; return c; }).ToList();
                                }
                                else
                                {
                                    //var objilq = (from x in objData
                                    //              where x.BinID != null
                                    //              group x by new { x.BinID } into grp
                                    //              select new MaterialStagingDetailDTO
                                    //              {
                                    //                  MaterialStagingGUID = objData[0].MaterialStagingGUID,
                                    //                  BinID = null,
                                    //                  StagingBinID = objData[0].StagingBinId,
                                    //                  StagingBinName = objData[0].BinNumber,
                                    //                  StagingBinGUID = objData[0].GUID,
                                    //                  ItemGUID = objData[0].ItemGUID,
                                    //                  CompanyID = objData[0].CompanyID ?? 0,
                                    //                  RoomId = Convert.ToInt32(objData[0].Room),
                                    //                  Updated = DateTime.Now,
                                    //                  Created = DateTime.Now,
                                    //                  CreatedBy = objData[0].CreatedBy ?? 0,
                                    //                  LastUpdatedBy = objData[0].LastUpdatedBy ?? 0,
                                    //                  CustomerOwnedQuantity = Convert.ToInt32(grp.Sum(x => x.CustomerOwnedQuantity ?? 0)),
                                    //                  ConsignedQuantity = Convert.ToInt32(grp.Sum(x => x.ConsignedQuantity ?? 0)),
                                    //                  Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                    //                  //LotNumber = x.LotNumber,
                                    //              }).ToList();

                                    ////Save
                                    //if (objilq.Count() > 0)
                                    //{
                                    //    foreach (MaterialStagingDetailDTO objDataInsertLot in objilq)
                                    //    {
                                    //        objMaterialStagingDetailDTO = objQtyMgt.SaveMaterialStagingDetailsForMSCredit(objDataInsertLot);
                                    //    }
                                    //}
                                }
                                //Save on hand quantity
                                #endregion
                            }

                            #region Update MaterialStagingdtlGUID in MaterialStagingPullDetail Table

                            if (objMaterialStagingDetailDTO != null && objMaterialStagingDetailDTO.GUID != Guid.Empty)
                            {
                                foreach (MaterialStagingPullDetailDTO item in objData)
                                {
                                    MaterialStagingPullDetailDAL objMaterialPullDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
                                    MaterialStagingPullDetailDTO objMaterialPullDTO = new MaterialStagingPullDetailDTO();

                                    objMaterialPullDTO = objMaterialPullDAL.GetMsPullDetailsByMsPullDetailsGUID(item.GUID);
                                    if (objMaterialPullDTO != null && objMaterialPullDTO.GUID != Guid.Empty)
                                    {
                                        objMaterialPullDTO.MaterialStagingdtlGUID = objMaterialStagingDetailDTO.GUID;
                                        objMaterialPullDAL.Edit(objMaterialPullDTO);
                                    }
                                }
                            }

                            #endregion

                        }
                        #endregion
                    }

                    objPullDTO = new PullMasterDAL(base.DataBaseName).UpdateCumulativeOnHandNewForMS(objPullDTO);

                }
            }
            return true;
        }

        public void SaveBothCustAndConsigenQtyForCountForMS(MaterialStagingPullDetailDTO item, ItemMasterDTO objItemDTO, BinMasterDTO objBinDTO, bool IsValid, string PullAction, IEnumerable<MaterialStagingDetailDTO> objData, string RoomDateFormat)
        {
            bool TempLotNumberTracking = false;
            bool TempSerialNumberTracking = false;
            bool IsQTYSave = false;
            MaterialStagingPullDetailDAL objItemDetailDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
            MaterialStagingPullDetailDTO objItem = new MaterialStagingPullDetailDTO();
            List<PullDetailsDTO> objPullDetailsDTOs = new List<PullDetailsDTO>();

            objItem.BinID = item.BinID;
            objItem.CompanyID = item.CompanyID ?? 0;
            objItem.Cost = item.Cost;
            objItem.Created = item.Created;
            objItem.CustomerOwnedQuantity = item.CustomerOwnedQuantity;
            objItem.ConsignedQuantity = item.ConsignedQuantity;
            objItem.InitialQuantity = (item.ConsignedQuantity ?? 0) + (item.CustomerOwnedQuantity ?? 0);
            objItem.InitialQuantityWeb = (item.ConsignedQuantity ?? 0) + (item.CustomerOwnedQuantity ?? 0);
            if (item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                objItem.IsConsignedSerialLot = true;
            else
                objItem.IsConsignedSerialLot = false;

            //objItem.IsConsignedSerialLot = objItemDTO.Consignment;
            objItem.ExpirationDate = item.ExpirationDate;
            objItem.Expiration = item.Expiration;
            objItem.Received = item.Received;
            objItem.ReceivedDate = item.ReceivedDate;
            if (objItem.ReceivedDate.GetValueOrDefault(DateTime.MinValue) <= DateTime.MinValue && !string.IsNullOrEmpty(objItem.Received))
            {
                objItem.ReceivedDate = DateTime.Parse(objItem.Received);
            }
            if (objItem.ExpirationDate.GetValueOrDefault(DateTime.MinValue) <= DateTime.MinValue && !string.IsNullOrEmpty(objItem.Expiration))
            {
                objItem.ExpirationDate = DateTime.Parse(objItem.Expiration);
            }
            objItem.ID = item.ID; objItem.ItemGUID = item.ItemGUID;

            objItem.LotNumber = item.LotNumber;
            if (item.LotNumber != null && item.LotNumberTracking)
                TempLotNumberTracking = true;
            objItem.SerialNumber = item.SerialNumber;
            if (item.SerialNumber != null && item.SerialNumberTracking)
                TempSerialNumberTracking = true;

            objItem.CreatedBy = item.CreatedBy ?? 0; objItem.CompanyID = item.CompanyID ?? 0;
            objItem.Room = item.Room; objItem.Updated = item.Updated;
            objItem.LastUpdatedBy = item.LastUpdatedBy ?? 0;
            objItem.IsArchived = item.IsArchived ?? false; objItem.IsDeleted = item.IsDeleted ?? false;
            objItem.Created = item.Created; objItem.CreatedBy = item.CreatedBy ?? 0;
            objItem.OrderDetailGUID = item.OrderDetailGUID;

            if (objBinDTO != null)
                objItem.BinNumber = objBinDTO.BinNumber;

            if (item.ID > 0 && IsValid)
            {
                objItem.GUID = item.GUID;
                objItem.IsOnlyFromUI = item.IsOnlyFromUI;
                if (item.IsOnlyFromUI)
                {
                    objItem.EditedFrom = "Web";
                    objItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                }
                objItemDetailDAL.Edit(objItem);
                IsQTYSave = true;
            }
            else
            {
                if (IsValid)
                {
                    bool IsRecordSave = false;
                    #region "SR Case"
                    if (TempSerialNumberTracking)
                    {
                        IEnumerable<MaterialStagingPullDetailDTO> TempDupSrs = objItemDetailDAL.GetAllRecordsByItemGuid(item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0), item.ItemGUID, item.SerialNumber).ToList();
                        if (TempDupSrs != null && TempDupSrs.Count() > 0)
                        {
                            foreach (MaterialStagingPullDetailDTO IDTemp in TempDupSrs)
                            {
                                IDTemp.IsDeleted = true;
                                IDTemp.IsArchived = true;
                                IDTemp.LastUpdatedBy = item.LastUpdatedBy;
                                IDTemp.Updated = DateTimeUtility.DateTimeNow;
                                IDTemp.IsOnlyFromUI = true;
                                IDTemp.EditedFrom = "Web";
                                IDTemp.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objItemDetailDAL.Edit(IDTemp);
                            }
                        }
                    }
                    #endregion
                    if (!IsRecordSave)
                    {
                        objItem.AddedFrom = "Web";
                        objItem.EditedFrom = "Web";
                        objItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objItem.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objItem.IsOnlyFromUI = item.IsOnlyFromUI;
                        MaterialStagingPullDetailDTO itemloctemp = objItemDetailDAL.Insert(true, objItem, RoomDateFormat);
                        item.ID = itemloctemp.ID;
                        item.GUID = itemloctemp.GUID;
                        IsQTYSave = true;
                    }
                    #region "PULL Details Entry"
                    PullDetailsDTO objPullDetailDTO = new PullDetailsDTO();
                    objPullDetailDTO.BinID = item.BinID;
                    objPullDetailDTO.CompanyID = item.CompanyID;

                    objPullDetailDTO.ConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0);
                    objPullDetailDTO.CustomerOwnedQuantity = item.CustomerOwnedQuantity.GetValueOrDefault(0);

                    //objPullDetailDTO.CreditConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0);
                    //objPullDetailDTO.CreditCustomerOwnedQuantity = item.CustomerOwnedQuantity.GetValueOrDefault(0);

                    objPullDetailDTO.Created = DateTimeUtility.DateTimeNow;
                    objPullDetailDTO.CreatedBy = item.CreatedBy.GetValueOrDefault(0);
                    objPullDetailDTO.CreatedByName = item.CreatedByName;

                    objPullDetailDTO.Expiration = item.Expiration;
                    objPullDetailDTO.IsArchived = item.IsArchived;
                    objPullDetailDTO.IsDeleted = item.IsDeleted;
                    objPullDetailDTO.ItemCost = CalculateAndGetPullCreditCost(objItem.Cost, objItemDTO.CostUOMID, item.Room ?? 0, item.CompanyID ?? 0);

                    objPullDetailDTO.ItemPrice = objPullDetailDTO.ItemCost;

                    objPullDetailDTO.ItemGUID = item.ItemGUID;
                    objPullDetailDTO.LastUpdatedBy = item.LastUpdatedBy.GetValueOrDefault(0);
                    objPullDetailDTO.LotNumber = item.LotNumber;
                    objPullDetailDTO.PoolQuantity = (item.ConsignedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0));
                    objPullDetailDTO.PullCredit = PullAction;
                    objPullDetailDTO.Received = item.Received;
                    objPullDetailDTO.Room = item.Room;
                    objPullDetailDTO.RoomName = item.RoomName;
                    objPullDetailDTO.SerialNumber = item.SerialNumber;
                    objPullDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                    objPullDetailDTO.UpdatedByName = item.UpdatedByName;
                    objPullDetailDTO.ItemLocationDetailGUID = null;
                    objPullDetailDTO.ProjectSpendGUID = item.ProjectSpentGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : item.ProjectSpentGUID;
                    objPullDetailDTO.WorkOrderGUID = item.WorkOrderGuid;
                    objPullDetailDTO.COuntLineItemGUID = item.CountLineItemDtlGUID;
                    objPullDetailDTO.AddedFrom = "Web";
                    objPullDetailDTO.EditedFrom = "Web";
                    objPullDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objPullDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                    objPullDetailDTO.MaterialStagingPullDetailGUID = item.GUID;
                    objPullDetailDTO.SupplierAccountGuid = item.SupplierAccountGuid;

                    objPullDetailsDTOs.Add(objPullDetailDTO);
                    #endregion
                }
            }

            #region "PULL Master and Detail Save"

            PullMasterDAL objPullDAL = new PullMasterDAL(base.DataBaseName);
            PullMasterViewDTO objPullDTO = new PullMasterViewDTO();
            if (objPullDetailsDTOs.Count > 0)
            {
                objPullDTO.ActionType = PullAction;
                objPullDTO.BinID = objPullDetailsDTOs[0].BinID;
                objPullDTO.CompanyID = objPullDetailsDTOs[0].CompanyID;
                objPullDTO.ConsignedQuantity = objPullDetailsDTOs.Select(x => x.ConsignedQuantity.GetValueOrDefault(0)).Sum();
                objPullDTO.CustomerOwnedQuantity = objPullDetailsDTOs.Select(x => x.CustomerOwnedQuantity.GetValueOrDefault(0)).Sum();
                //objPullDTO.CreditConsignedQuantity = objPullDetailsDTOs.Select(x => x.CreditConsignedQuantity.GetValueOrDefault(0)).Sum();
                //objPullDTO.CreditCustomerOwnedQuantity = objPullDetailsDTOs.Select(x => x.CreditCustomerOwnedQuantity.GetValueOrDefault(0)).Sum();
                objPullDTO.Created = DateTimeUtility.DateTimeNow;
                objPullDTO.CreatedBy = objPullDetailsDTOs[0].CreatedBy.GetValueOrDefault(0);
                objPullDTO.CreatedByName = objPullDetailsDTOs[0].CreatedByName;
                objPullDTO.GUID = Guid.NewGuid();
                objPullDTO.IsArchived = false;
                objPullDTO.IsDeleted = false;
                objPullDTO.ItemGUID = objItemDTO.GUID;
                objPullDTO.ItemNumber = objItemDTO.ItemNumber;
                objPullDTO.ItemType = objItemDTO.ItemType;
                objPullDTO.LastUpdatedBy = objPullDetailsDTOs[0].LastUpdatedBy.GetValueOrDefault(0);
                objPullDTO.PoolQuantity = objPullDetailsDTOs.Select(x => x.PoolQuantity.GetValueOrDefault(0)).Sum();
                objPullDTO.PullCredit = PullAction;
                objPullDTO.PullCost = Convert.ToDouble(objPullDetailsDTOs.Select(x => x.ItemCost.GetValueOrDefault(0) * x.PoolQuantity.GetValueOrDefault(0)).Sum());
                objPullDTO.Room = objPullDetailsDTOs[0].Room;
                objPullDTO.RoomName = objPullDetailsDTOs[0].RoomName;
                objPullDTO.Updated = DateTimeUtility.DateTimeNow;
                objPullDTO.UpdatedByName = objPullDetailsDTOs[0].UpdatedByName;
                objPullDTO.ProjectSpendGUID = objPullDetailsDTOs[0].ProjectSpendGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : objPullDetailsDTOs[0].ProjectSpendGUID;
                objPullDTO.WorkOrderDetailGUID = objPullDetailsDTOs[0].WorkOrderGUID;
                objPullDTO.WorkOrderGUID = objPullDetailsDTOs[0].WorkOrderGUID;
                objPullDTO.CountLineItemGuid = objPullDetailsDTOs[0].COuntLineItemGUID;
                objPullDTO.WhatWhereAction = PullAction;

                objPullDTO.AddedFrom = "Web";
                objPullDTO.EditedFrom = "Web";
                objPullDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objPullDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                objPullDTO.SupplierAccountGuid = objPullDetailsDTOs[0].SupplierAccountGuid;
                //objPullDTO.PullPrice = 

                objPullDTO.ID = objPullDAL.Insert(objPullDTO);
                PullDetailsDAL objPullDetailsDAL = new PullDetailsDAL(base.DataBaseName);
                foreach (PullDetailsDTO temppdDTO in objPullDetailsDTOs)
                {
                    temppdDTO.PULLGUID = objPullDTO.GUID;
                    temppdDTO.AddedFrom = "Web";
                    temppdDTO.EditedFrom = "Web";
                    temppdDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    temppdDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objPullDetailsDAL.Insert(temppdDTO);


                    //Project Spend update
                    if (temppdDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                    {
                        ProjectMasterDAL objPrjMsgDAL = new ProjectMasterDAL(base.DataBaseName);
                        ProjectSpendItemsDAL objPrjSpenItmDAL = new ProjectSpendItemsDAL(base.DataBaseName);
                        ProjectMasterDTO objPrjMstDTO = new ProjectMasterDTO();
                        ProjectSpendItemsDTO objPrjSpenItmDTO = new ProjectSpendItemsDTO();
                        var tmpsupplierIds = new List<long>();
                        //objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecords(objPullDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)objPullDetailsDTOs[0].Room, (Int64)objPullDetailsDTOs[0].CompanyID, tmpsupplierIds).Where(x => x.ItemGUID == objPullDTO.ItemGUID && x.Created.Value <= temppdDTO.Created.Value).SingleOrDefault();
                        objPrjSpenItmDTO = objPrjSpenItmDAL.GetProjectSpendItemCompareDate(objPullDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)objPullDetailsDTOs[0].Room, (Int64)objPullDetailsDTOs[0].CompanyID, tmpsupplierIds, Convert.ToString(objPullDTO.ItemGUID), temppdDTO.Created.Value.ToString("yyyy-MM-dd HH:mm:ss")).SingleOrDefault();
                        objPrjMstDTO = objPrjMsgDAL.GetRecord(objPullDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)objPullDetailsDTOs[0].Room, (Int64)objPullDetailsDTOs[0].CompanyID, false, false);

                        if (objPrjMstDTO != null)
                        {

                            objPrjMstDTO.DollarUsedAmount = objPrjMstDTO.DollarUsedAmount.GetValueOrDefault(0) - ((decimal)temppdDTO.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(temppdDTO.ItemCost.GetValueOrDefault(0)));
                            objPrjMstDTO.WhatWhereAction = PullAction;
                            //objPrjMstDTO.EditedFrom = "Web";
                            //objPrjMstDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objPrjMsgDAL.Edit(objPrjMstDTO);

                            if (objPrjSpenItmDTO != null)
                            {

                                objPrjSpenItmDTO.DollarUsedAmount = objPrjSpenItmDTO.DollarUsedAmount.GetValueOrDefault(0) - ((decimal)temppdDTO.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(temppdDTO.ItemCost.GetValueOrDefault(0)));
                                objPrjSpenItmDTO.QuantityUsed = objPrjSpenItmDTO.QuantityUsed.GetValueOrDefault(0) - temppdDTO.PoolQuantity.GetValueOrDefault(0);
                                //objPrjSpenItmDTO.EditedFrom = "Web";
                                //objPrjSpenItmDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objPrjSpenItmDAL.Edit(objPrjSpenItmDTO);
                            }
                        }
                    }

                }




            }
            #endregion

            if (objItemDTO != null && objItemDTO.ItemType != 4)
            {
                #region "Insert QTY logic"
                if (IsQTYSave)
                {
                    if (TempSerialNumberTracking)
                    {
                        #region "Insert mode for Serial Number"
                        MaterialStagingDetailDAL objQtyMgt = new MaterialStagingDetailDAL(base.DataBaseName);

                        //FOR INSERT MODE
                        var objilqInsert = new List<MaterialStagingDetailDTO>();

                        objilqInsert = (from x in objData
                                        where (x.SerialNumberTracking == true) && x.BinID != null
                                        group x by new { x.BinID }
                                            into grp
                                        select new MaterialStagingDetailDTO
                                        {
                                            MaterialStagingGUID = objData.FirstOrDefault().MaterialStagingGUID,
                                            BinID = Convert.ToInt64(grp.Key.BinID),
                                            StagingBinID = objData.FirstOrDefault().StagingBinID,
                                            StagingBinName = objData.FirstOrDefault().StagingBinName,
                                            ItemGUID = objData.FirstOrDefault().ItemGUID ?? Guid.Empty,
                                            CompanyID = objData.FirstOrDefault().CompanyID,
                                            RoomId = objData.FirstOrDefault().RoomId,
                                            Updated = DateTime.Now,
                                            Created = DateTime.Now,
                                            CreatedBy = objData.FirstOrDefault().CreatedBy,
                                            LastUpdatedBy = objData.FirstOrDefault().LastUpdatedBy,
                                            CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity),
                                            ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity),
                                            Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                        }).ToList();

                        if (objilqInsert.Count() > 0)
                        {
                            foreach (MaterialStagingDetailDTO objDataInsert in objilqInsert)
                            {
                                objQtyMgt.SaveMaterialStagingDetailsForMSCredit(objDataInsert);
                            }
                        }
                        #endregion
                    }
                    else if (TempLotNumberTracking)
                    {
                        #region "Insert Mode for Lot#, Others"
                        MaterialStagingDetailDAL objQtyMgt = new MaterialStagingDetailDAL(base.DataBaseName);
                        //if (objItemDTO.Consignment)
                        if (item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                        {
                            var objilq = new List<MaterialStagingDetailDTO>();

                            objilq = (from x in objData
                                      where (x.LotNumberTracking == true) && x.BinID != null
                                      group x by new { x.BinID } into grp
                                      select new MaterialStagingDetailDTO
                                      {
                                          MaterialStagingGUID = objData.FirstOrDefault().MaterialStagingGUID,
                                          BinID = Convert.ToInt64(grp.Key.BinID),
                                          StagingBinID = objData.FirstOrDefault().StagingBinID,
                                          StagingBinName = objData.FirstOrDefault().StagingBinName,
                                          ItemGUID = objData.FirstOrDefault().ItemGUID ?? Guid.Empty,
                                          CompanyID = objData.FirstOrDefault().CompanyID,
                                          RoomId = objData.FirstOrDefault().RoomId,
                                          Updated = DateTime.Now,
                                          Created = DateTime.Now,
                                          CreatedBy = objData.FirstOrDefault().CreatedBy,
                                          LastUpdatedBy = objData.FirstOrDefault().LastUpdatedBy,
                                          CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity),
                                          ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity),
                                          Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                          //LotNumber = x.LotNumber,
                                      }).ToList();
                            //Save

                            if (objilq.Count() > 0)
                            {
                                foreach (MaterialStagingDetailDTO objDataLot in objilq)
                                {
                                    objQtyMgt.SaveMaterialStagingDetailsForMSCredit(objDataLot);
                                }
                            }
                            //Update on_hand qunatity
                            //objilq.Select(c => { OnHandQuantity += c.Quantity; return c; }).ToList();
                        }
                        else
                        {
                            var objilq = new List<MaterialStagingDetailDTO>();
                            objilq = (from x in objData
                                      where (x.LotNumberTracking == true) && x.BinID != null
                                      group x by new { x.BinID } into grp
                                      select new MaterialStagingDetailDTO
                                      {
                                          MaterialStagingGUID = objData.FirstOrDefault().MaterialStagingGUID,
                                          BinID = Convert.ToInt64(grp.Key.BinID),
                                          StagingBinID = objData.FirstOrDefault().StagingBinID,
                                          StagingBinName = objData.FirstOrDefault().StagingBinName,
                                          ItemGUID = objData.FirstOrDefault().ItemGUID,
                                          CompanyID = objData.FirstOrDefault().CompanyID,
                                          RoomId = objData.FirstOrDefault().RoomId,
                                          Updated = DateTime.Now,
                                          Created = DateTime.Now,
                                          CreatedBy = objData.FirstOrDefault().CreatedBy,
                                          LastUpdatedBy = objData.FirstOrDefault().LastUpdatedBy,
                                          CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity),
                                          ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity),
                                          Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                          //LotNumber = x.LotNumber,
                                      }).ToList();

                            //Save
                            if (objilq.Count() > 0)
                            {
                                foreach (MaterialStagingDetailDTO objDataLot in objilq)
                                {
                                    objQtyMgt.SaveMaterialStagingDetailsForMSCredit(objDataLot);
                                }
                            }
                        }
                        //Save on hand quantity
                        #endregion
                    }
                    else
                    {
                        #region "Insert Mode for Lot#, Others"
                        MaterialStagingDetailDAL objQtyMgt = new MaterialStagingDetailDAL(base.DataBaseName);
                        //if (objItemDTO.Consignment)
                        if (item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                        {
                            var objilq = new List<MaterialStagingDetailDTO>();
                            objilq = (from x in objData
                                      where x.BinID != null
                                      group x by new { x.BinID } into grp
                                      select new MaterialStagingDetailDTO
                                      {
                                          MaterialStagingGUID = objData.FirstOrDefault().MaterialStagingGUID,
                                          BinID = Convert.ToInt64(grp.Key.BinID),
                                          StagingBinID = objData.FirstOrDefault().StagingBinID,
                                          StagingBinName = objData.FirstOrDefault().StagingBinName,
                                          ItemGUID = objData.FirstOrDefault().ItemGUID,
                                          CompanyID = objData.FirstOrDefault().CompanyID,
                                          RoomId = objData.FirstOrDefault().RoomId,
                                          Updated = DateTime.Now,
                                          Created = DateTime.Now,
                                          CreatedBy = objData.FirstOrDefault().CreatedBy,
                                          LastUpdatedBy = objData.FirstOrDefault().LastUpdatedBy,
                                          CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity),
                                          ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity),
                                          Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                          //LotNumber = x.LotNumber,
                                      }).ToList();
                            //Save
                            if (objilq.Count() > 0)
                            {
                                foreach (MaterialStagingDetailDTO objDataLot in objilq)
                                {
                                    objQtyMgt.SaveMaterialStagingDetailsForMSCredit(objDataLot);
                                }
                            }
                            //Update on_hand qunatity
                            //objilq.Select(c => { OnHandQuantity += c.Quantity; return c; }).ToList();
                        }
                        else
                        {
                            var objilq = new List<MaterialStagingDetailDTO>();
                            objilq = (from x in objData
                                      where x.BinID != null
                                      group x by new { x.BinID } into grp
                                      select new MaterialStagingDetailDTO
                                      {
                                          MaterialStagingGUID = objData.FirstOrDefault().MaterialStagingGUID,
                                          BinID = Convert.ToInt64(grp.Key.BinID),
                                          StagingBinID = objData.FirstOrDefault().StagingBinID,
                                          StagingBinName = objData.FirstOrDefault().StagingBinName,
                                          ItemGUID = objData.FirstOrDefault().ItemGUID,
                                          CompanyID = objData.FirstOrDefault().CompanyID,
                                          RoomId = objData.FirstOrDefault().RoomId,
                                          Updated = DateTime.Now,
                                          Created = DateTime.Now,
                                          CreatedBy = objData.FirstOrDefault().CreatedBy,
                                          LastUpdatedBy = objData.FirstOrDefault().LastUpdatedBy,
                                          CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity),
                                          ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity),
                                          Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                          //LotNumber = x.LotNumber,
                                      }).ToList();

                            //Save
                            if (objilq.Count() > 0)
                            {
                                foreach (MaterialStagingDetailDTO objDataLot in objilq)
                                {
                                    objQtyMgt.SaveMaterialStagingDetailsForMSCredit(objDataLot);
                                }
                            }
                        }
                        //Save on hand quantity
                        #endregion
                    }
                }
                #endregion
            }

            objPullDTO = new PullMasterDAL(base.DataBaseName).UpdateCumulativeOnHandNewForMS(objPullDTO);

        }

        #endregion

        #region for Validate Pull Count Credit

        public IEnumerable<ItemLocationDetailsDTO> GetCountDifferenceforValidatPull(Guid ItemGUID, Int64 RoomID, Int64 CompanyID, Int64 BinID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramA = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@ItemGUID", ItemGUID), new SqlParameter("@BinID", BinID) };
                //, new SqlParameter("@SerialNumber", SerialNumber ?? (object)DBNull.Value), new SqlParameter("@LotNumber", LotNumber ?? (object)DBNull.Value) };,@SerialNumber,@LotNumber

                IEnumerable<ItemLocationDetailsDTO> obj = context.Database.SqlQuery<ItemLocationDetailsDTO>("exec [GetItemLocDtlWithCountQuantity] @CompanyID,@RoomID,@ItemGUID,@BinID", paramA).ToList();
                //select new ItemLocationDetailsDTO
                //{
                //    ItemGUID = u.ItemGUID,
                //    BinID = u.BinID,
                //    LotNumber = u.LotNumber,
                //    SerialNumber = u.SerialNumber,
                //    ExpirationDate = u.ExpirationDate,
                //    CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                //    ConsignedQuantity = u.ConsignedQuantity
                //}).AsParallel().ToList();
                return obj;
            }
        }


        public void InsertCreditHistoryForCount(Int64 RoomID, Int64 CompanyID, Guid appliedcountguid, Guid appliedcountdetailguid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramA = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@appliedcountguid", appliedcountguid),
                                                   new SqlParameter("@appliedcountdetailguid", appliedcountdetailguid) };


                context.Database.ExecuteSqlCommand("EXEC [InsertCreditHistoryForCount] @RoomID,@CompanyID,@appliedcountguid,@appliedcountdetailguid", paramA);
            }
        }

        #endregion

        public List<ItemLocationDetailsDTO> GetItemsLocationsSerLotQtyForMoveOut(Guid ItemGUID, string LotNumber, string SerialNumber, long RoomId, long CompanyId)
        {
            List<ItemLocationDetailsDTO> oItemLocations = new List<ItemLocationDetailsDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                oItemLocations = (from il in context.ItemLocationDetails
                                  where il.ItemGUID == ItemGUID && il.Room == RoomId && il.CompanyID == CompanyId
                                  && (il.LotNumber == LotNumber || LotNumber == string.Empty)
                                  && (il.SerialNumber == SerialNumber || SerialNumber == string.Empty)
                                  && (il.IsDeleted ?? false) == false && (il.IsArchived ?? false) == false
                                  && ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) > 0
                                  select new ItemLocationDetailsDTO
                                  {
                                      ID = il.ID,
                                      GUID = il.GUID,
                                      CustomerOwnedQuantity = il.CustomerOwnedQuantity,
                                      ConsignedQuantity = il.ConsignedQuantity,
                                      LotNumber = il.LotNumber,
                                      SerialNumber = il.SerialNumber,
                                  }).ToList();
            }
            return oItemLocations;
        }

        public ItemLocationDetailsDTO GetRecordsByItemLocationDetailGuid(Guid ItemLocationDetailGuid, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramA = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ItemLocationDetailGuid", ItemLocationDetailGuid) };

                ItemLocationDetailsDTO obj = (from u in context.Database.SqlQuery<ItemLocationDetailsDTO>("exec [GetRecordsByItemLocationDetailGuid] @CompanyID,@RoomID,@ItemLocationDetailGuid", paramA)

                                              select new ItemLocationDetailsDTO
                                              {
                                                  ID = u.ID,
                                                  BinID = u.BinID,
                                                  CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                  ConsignedQuantity = u.ConsignedQuantity,
                                                  MeasurementID = u.MeasurementID,
                                                  LotNumber = u.LotNumber,
                                                  SerialNumber = u.SerialNumber,
                                                  ExpirationDate = u.ExpirationDate,
                                                  ReceivedDate = u.ReceivedDate,
                                                  Expiration = u.Expiration == null && u.DateCodeTracking ? DateTime.Now.ToString("MM-dd-yy") : u.Expiration,
                                                  Received = u.Received == null ? DateTime.Now.ToString("MM-dd-yy") : u.Received,
                                                  Cost = u.Cost,
                                                  eVMISensorPort = u.eVMISensorPort,
                                                  eVMISensorID = u.eVMISensorID,
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
                                                  OrderDetailGUID = u.OrderDetailGUID,
                                                  TransferDetailGUID = u.TransferDetailGUID,
                                                  InitialQuantity = u.InitialQuantity,
                                                  InitialQuantityWeb = u.InitialQuantityWeb,
                                                  InitialQuantityPDA = u.InitialQuantityPDA,
                                                  AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                                                  EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                                                  ReceivedOn = u.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                                                  ReceivedOnWeb = u.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                                              }).AsParallel().FirstOrDefault();

                return obj;
            }
        }

        //#region WI-4991

        //public Guid ApplyAdjustmentCountForImport(DataTable DT, long RoomID, long CompanyID, long UserID)
        //{
        //    Guid InsertedCountGuid = Guid.Empty;
        //    try
        //    {
        //        SqlParameter[] spParameter = new SqlParameter[5];

        //        spParameter[0] = new SqlParameter("@RoomID", RoomID);
        //        spParameter[0].Value = RoomID;

        //        spParameter[1] = new SqlParameter("@CompanyID", CompanyID);
        //        spParameter[1].Value = CompanyID;

        //        spParameter[2] = new SqlParameter("@UserID", UserID);
        //        spParameter[2].Value = UserID;

        //        spParameter[3] = new SqlParameter("@InsertedCountGuid", InsertedCountGuid);
        //        spParameter[3].Value = InsertedCountGuid;
        //        spParameter[3].Direction = ParameterDirection.Output;

        //        spParameter[4] = new SqlParameter("@tblitemlocdetails", DT);
        //        spParameter[4].Value = DT;


        //        string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
        //        SqlConnection ChildDbConnection = new SqlConnection(Connectionstring);
        //        SqlHelper.ExecuteDataset(ChildDbConnection, CommandType.StoredProcedure, "AdjustmentCountForImport", spParameter);

        //        InsertedCountGuid = Guid.Parse(Convert.ToString(spParameter[3].Value));

        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    return InsertedCountGuid;

        //}

        //#endregion

        #region Material staging count - WI-4801

        public void InsertMSCreditHistoryForCount(Int64 RoomID, Int64 CompanyID, Guid appliedcountguid, Guid appliedcountdetailguid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramA = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@appliedcountguid", appliedcountguid),
                                                   new SqlParameter("@appliedcountdetailguid", appliedcountdetailguid) };


                context.Database.ExecuteSqlCommand("EXEC [InsertMSCreditHistoryForCount] @RoomID,@CompanyID,@appliedcountguid,@appliedcountdetailguid", paramA);
            }
        }

        #endregion                

        #region 3055

        public ItemLocationDetailsDTO GetItemLocationDetailsByLocationGuid(Guid ItemLocationDetailGuid, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramA = new SqlParameter[] { new SqlParameter("@CompnayID", CompanyID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@ItemLocationDetailGuid", ItemLocationDetailGuid) };
                return context.Database.SqlQuery<ItemLocationDetailsDTO>("GetItemLocationDetailsByLocationGuid @CompnayID,@RoomID,@ItemLocationDetailGuid", paramA).FirstOrDefault();
            }
        }

        public bool ItemLocationDetailsEditForCreditPullnew(List<ItemLocationDetailsDTO> objData, double NewPullQuantity, double OldPullQuantity, string calledfrom, string RoomDateFormat, out List<CreditHistory> lstCreditGuids, long SessionUserId, long EnterpriseId, string PullAction = "credit", string WhatWhereAction = "")
        {
            lstCreditGuids = new List<CreditHistory>();

            if (!string.IsNullOrWhiteSpace(calledfrom))
            {
                if ("evmisvc" == calledfrom)
                {
                    PullAction = "cr_evmi";
                }
            }

            bool TempSerialNumberTracking = false;
            bool TempLotNumberTracking = false;
            //bool Consignment = false;
            ItemMasterDTO objItemDTO = null;
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            bool IsQTYSave = false;
            bool IsBothQty = false;
            string CountdtlGUID = string.Empty;
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                int LooptotalCount = objData.Count();
                int Counter = 0;
                foreach (ItemLocationDetailsDTO item in objData)
                {
                    BinMasterDTO Itembin = GetItemBin(item.ItemGUID ?? Guid.Empty, item.BinNumber, item.Room ?? 0, item.CompanyID ?? 0, item.CreatedBy ?? 0, false);
                    item.BinID = Itembin.ID;
                }
                List<PullDetailsDTO> objPullDetailsDTOs = new List<PullDetailsDTO>();
                foreach (ItemLocationDetailsDTO item in objData)
                {
                    #region "Location Detail Save"
                    Counter++;

                    if (item.BinID == null)
                    {
                        continue;
                    }
                    if (objItemDTO == null)
                    {
                        objItemDTO = objItemDAL.GetItemWithoutJoins(null, item.ItemGUID);
                    }

                    item.IsArchived = (item.IsArchived == null ? false : item.IsArchived);
                    item.IsDeleted = (item.IsDeleted == null ? false : item.IsDeleted);
                    BinMasterDAL objBin = new BinMasterDAL(base.DataBaseName);
                    BinMasterDTO objBinDTO = objBin.GetBinByID(Convert.ToInt64(item.BinID), Convert.ToInt64(item.Room), Convert.ToInt64(item.CompanyID));
                    //BinMasterDTO objBinDTO = objBin.GetItemLocation(Convert.ToInt64(item.Room), Convert.ToInt64(item.CompanyID), false, false,Guid.Empty, Convert.ToInt64(item.BinID), null,null).FirstOrDefault();

                    if (string.IsNullOrEmpty(item.Received) && item.ReceivedDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                        item.Received = item.ReceivedDate.Value.ToString("M/d/yyyy");

                    bool IsValid = false;

                    if (!string.IsNullOrEmpty(item.Received) || item.ReceivedDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                        IsValid = true;
                    else if (!string.IsNullOrEmpty(item.SerialNumber))
                    {
                        IsValid = true;
                    }
                    else if (!string.IsNullOrEmpty(item.LotNumber))
                        IsValid = true;
                    else if (!string.IsNullOrEmpty(item.Expiration) || (item.ExpirationDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue))
                        IsValid = true;
                    else if (item.LotNumberTracking && item.SerialNumberTracking)
                        IsValid = true;
                    else if (item.BinID.HasValue && item.Cost.HasValue && (!string.IsNullOrEmpty(item.Received) || item.ReceivedDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue))
                        IsValid = true;

                    if (IsValid || (LooptotalCount == Counter))
                    {
                        //if (!String.IsNullOrEmpty(item.SerialNumber) || !String.IsNullOrEmpty(item.LotNumber) || item.CustomerOwnedQuantity.Value > 0 || item.ConsignedQuantity.Value > 0)
                        //{
                        double CustVal = 0;
                        double ConsignVal = 0;
                        bool IsConsinAndCustBothQty = false;
                        if (item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0 && item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                        {
                            CustVal = item.CustomerOwnedQuantity.GetValueOrDefault(0);
                            ConsignVal = item.ConsignedQuantity.GetValueOrDefault(0);
                            IsConsinAndCustBothQty = true;
                            IsBothQty = true;
                        }
                        else if (item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0 && item.ConsignedQuantity.GetValueOrDefault(0) <= 0)
                        {
                            CustVal = item.CustomerOwnedQuantity.GetValueOrDefault(0);
                            ConsignVal = 0;
                            IsConsinAndCustBothQty = false;
                            IsBothQty = false;
                        }
                        else if (item.ConsignedQuantity.GetValueOrDefault(0) > 0 && item.CustomerOwnedQuantity.GetValueOrDefault(0) <= 0)
                        {
                            ConsignVal = item.ConsignedQuantity.GetValueOrDefault(0);
                            CustVal = 0;
                            IsBothQty = false;
                            IsConsinAndCustBothQty = false;
                        }

                        if (IsConsinAndCustBothQty)
                        {
                            item.ConsignedQuantity = ConsignVal;
                            item.CustomerOwnedQuantity = 0;
                            SaveBothCustAndConsigenQtyForCount(item, objItemDTO, objBinDTO, IsValid, PullAction, objData, SessionUserId, EnterpriseId);

                            item.ConsignedQuantity = 0;
                            item.CustomerOwnedQuantity = CustVal;
                            item.ID = 0;
                            item.GUID = Guid.Empty;
                            SaveBothCustAndConsigenQtyForCount(item, objItemDTO, objBinDTO, IsValid, PullAction, objData, SessionUserId, EnterpriseId);
                            continue;
                        }
                        else
                        {
                            ItemLocationDetailsDAL objItemDetailDAL = new ItemLocationDetailsDAL(base.DataBaseName);
                            ItemLocationDetailsDTO objItem = new ItemLocationDetailsDTO();
                            objItem.BinID = item.BinID;

                            objItem.CompanyID = item.CompanyID;
                            objItem.Cost = item.Cost;
                            objItem.Created = item.Created;


                            objItem.CustomerOwnedQuantity = item.CustomerOwnedQuantity;
                            objItem.ConsignedQuantity = item.ConsignedQuantity;
                            objItem.InitialQuantity = (item.ConsignedQuantity ?? 0) + (item.CustomerOwnedQuantity ?? 0);
                            objItem.InitialQuantityWeb = (item.ConsignedQuantity ?? 0) + (item.CustomerOwnedQuantity ?? 0);
                            objItem.IsConsignedSerialLot = objItemDTO.Consignment;
                            objItem.ExpirationDate = item.ExpirationDate;

                            objItem.Expiration = item.Expiration;
                            objItem.Received = item.Received;
                            objItem.ReceivedDate = item.ReceivedDate;
                            if (objItem.ReceivedDate.GetValueOrDefault(DateTime.MinValue) <= DateTime.MinValue && !string.IsNullOrEmpty(objItem.Received))
                            {
                                objItem.ReceivedDate = DateTime.Parse(objItem.Received);
                            }
                            if (item.DateCodeTracking && objItem.ExpirationDate.GetValueOrDefault(DateTime.MinValue) <= DateTime.MinValue && !string.IsNullOrEmpty(objItem.Expiration))
                            {
                                DateTime dt = new DateTime();
                                if (DateTime.TryParseExact(objItem.Expiration, RoomDateFormat, ResourceHelper.CurrentCult, DateTimeStyles.None, out dt))
                                    objItem.ExpirationDate = dt;
                            }

                            objItem.ID = item.ID; objItem.ItemGUID = item.ItemGUID;

                            objItem.LotNumber = item.LotNumber;
                            if (item.LotNumber != null && item.LotNumberTracking)
                                TempLotNumberTracking = true;
                            objItem.SerialNumber = item.SerialNumber;
                            if (item.SerialNumber != null && item.SerialNumberTracking)
                                TempSerialNumberTracking = true;

                            objItem.CreatedBy = item.CreatedBy; objItem.CompanyID = item.CompanyID;
                            objItem.Room = item.Room; objItem.Updated = item.Updated;
                            objItem.LastUpdatedBy = item.LastUpdatedBy;
                            objItem.IsArchived = item.IsArchived; objItem.IsDeleted = item.IsDeleted;
                            objItem.Created = item.Created; objItem.CreatedBy = item.CreatedBy;
                            objItem.OrderDetailGUID = item.OrderDetailGUID;

                            if (objBinDTO != null)
                                objItem.BinNumber = objBinDTO.BinNumber;

                            if (item.ID > 0 && IsValid)
                            {
                                objItem.GUID = item.GUID;
                                objItem.IsOnlyFromUI = item.IsOnlyFromUI;
                                if (item.IsOnlyFromUI)
                                {
                                    objItem.EditedFrom = "Web";
                                    objItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                                }
                                objItemDetailDAL.Edit(objItem);
                                // here needs to implement the edit quantity logic for location details table
                                IsQTYSave = true;
                            }
                            else
                            {
                                if (IsValid)
                                {
                                    bool IsRecordSave = false;
                                    //// here needs to write code for 4 points as following...
                                    // 1 // if item is SR. then check for duplication if found then make them IsDeleted = 1
                                    //   // And then Insert the New item with same SR. number
                                    // 2 // If item is Lot, then check if duplicate number then only update the quantity
                                    //   // for that particular lot number also check BinLocation if exist then update else insert ....
                                    // 3 // Needs to enter only one entry to PullMaster with Total Quantity and data
                                    // 4 // Then needs to enter all the entry to PullDetails with itemlocation data ..
                                    #region "SR Case"
                                    if (TempSerialNumberTracking)
                                    {
                                        //var TempDupSrs = objItemDetailDAL.GetCachedData(item.ItemGUID.Value, item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0)).Where(x => x.SerialNumber == item.SerialNumber).ToList();
                                        var TempDupSrs = objItemDetailDAL.GetItemLocationDetail(item.ItemGUID.Value, item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0), null, item.SerialNumber, string.Empty, string.Empty, null, null, string.Empty).ToList();
                                        if (TempDupSrs != null && TempDupSrs.Count() > 0)
                                        {
                                            foreach (ItemLocationDetailsDTO IDTemp in TempDupSrs)
                                            {
                                                IDTemp.IsDeleted = true;
                                                IDTemp.IsArchived = true;
                                                IDTemp.LastUpdatedBy = item.LastUpdatedBy;
                                                IDTemp.Updated = DateTimeUtility.DateTimeNow;
                                                IDTemp.IsOnlyFromUI = true;
                                                IDTemp.EditedFrom = "Web";
                                                IDTemp.ReceivedOn = DateTimeUtility.DateTimeNow;
                                                objItemDetailDAL.Edit(IDTemp);
                                            }
                                        }
                                    }
                                    #endregion
                                    //#region "LOT Case"
                                    //if (TempLotNumberTracking)
                                    //{// bin wise check the lot nummber if found then updated else insert case ....
                                    //    var TempDupSrs = objItemDetailDAL.GetCachedData(item.ItemGUID.Value, item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0)).Where(x => x.LotNumber == item.LotNumber && x.BinID == item.BinID).SingleOrDefault();
                                    //    if (TempDupSrs != null)
                                    //    {
                                    //        TempDupSrs.ConsignedQuantity = TempDupSrs.ConsignedQuantity.GetValueOrDefault(0) + objItem.ConsignedQuantity.GetValueOrDefault(0);
                                    //        TempDupSrs.CustomerOwnedQuantity = TempDupSrs.CustomerOwnedQuantity.GetValueOrDefault(0) + objItem.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    //        objItemDetailDAL.Edit(TempDupSrs);
                                    //        IsRecordSave = true;
                                    //    }
                                    //}
                                    //#endregion

                                    if (!IsRecordSave)
                                    {
                                        objItem.AddedFrom = "Web";
                                        objItem.EditedFrom = "Web";
                                        objItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objItem.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                        objItem.IsOnlyFromUI = item.IsOnlyFromUI;
                                        ItemLocationDetailsDTO itemloctemp = objItemDetailDAL.Insert(objItem);
                                        item.ID = itemloctemp.ID;
                                        item.GUID = itemloctemp.GUID;
                                        IsQTYSave = true;
                                    }
                                    #region "PULL Details Entry"
                                    PullDetailsDTO objPullDetailDTO = new PullDetailsDTO();
                                    objPullDetailDTO.BinID = item.BinID;
                                    objPullDetailDTO.CompanyID = item.CompanyID;
                                    objPullDetailDTO.ConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0);
                                    objPullDetailDTO.Created = DateTimeUtility.DateTimeNow;
                                    objPullDetailDTO.CreatedBy = item.CreatedBy.GetValueOrDefault(0);
                                    objPullDetailDTO.CreatedByName = item.CreatedByName;
                                    objPullDetailDTO.CustomerOwnedQuantity = item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    objPullDetailDTO.Expiration = item.Expiration;
                                    objPullDetailDTO.IsArchived = item.IsArchived;
                                    objPullDetailDTO.IsDeleted = item.IsDeleted;
                                    objPullDetailDTO.ItemCost = CalculateAndGetPullCreditCost(item.Cost, objItemDTO.CostUOMID, item.Room ?? 0, item.CompanyID ?? 0);
                                    objPullDetailDTO.ItemPrice = CalculateAndGetPullCreditCost(item.SellPrice, objItemDTO.CostUOMID, item.Room ?? 0, item.CompanyID ?? 0);
                                    objPullDetailDTO.ItemGUID = item.ItemGUID;
                                    objPullDetailDTO.LastUpdatedBy = item.LastUpdatedBy.GetValueOrDefault(0);
                                    objPullDetailDTO.LotNumber = item.LotNumber;
                                    objPullDetailDTO.PoolQuantity = (item.ConsignedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0));
                                    objPullDetailDTO.PullCredit = PullAction;
                                    objPullDetailDTO.Received = item.Received;
                                    objPullDetailDTO.Room = item.Room;
                                    objPullDetailDTO.RoomName = item.RoomName;
                                    objPullDetailDTO.SerialNumber = item.SerialNumber;
                                    objPullDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                                    objPullDetailDTO.UpdatedByName = item.UpdatedByName;
                                    objPullDetailDTO.ItemLocationDetailGUID = item.GUID;
                                    objPullDetailDTO.ProjectSpendGUID = item.ProjectSpentGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : item.ProjectSpentGUID;
                                    objPullDetailDTO.WorkOrderGUID = item.WorkOrderGUID;
                                    objPullDetailDTO.COuntLineItemGUID = item.CountLineItemDtlGUID;
                                    objPullDetailDTO.SupplierAccountGuid = item.SupplierAccountGuid;

                                    if (string.IsNullOrEmpty(item.AddedFrom) || string.IsNullOrWhiteSpace(item.AddedFrom))
                                        objPullDetailDTO.AddedFrom = "Web";
                                    else
                                        objPullDetailDTO.AddedFrom = item.AddedFrom;

                                    if (string.IsNullOrEmpty(item.EditedFrom) || string.IsNullOrWhiteSpace(item.EditedFrom))
                                        objPullDetailDTO.EditedFrom = "Web";
                                    else
                                        objPullDetailDTO.EditedFrom = item.EditedFrom;

                                    objPullDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    objPullDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                    objPullDetailDTO.PullOrderNumber = item.PullOrderNumber;

                                    //This PULLGUID set for apply PullGUID in Service WI-4305 
                                    objPullDetailDTO.NEW_PullGUID = item.PullGUID;
                                    objPullDetailDTO.PULLGUID = item.PullGUID;
                                    objPullDetailDTO.GUID = Guid.NewGuid();

                                    objPullDetailsDTOs.Add(objPullDetailDTO);

                                    CreditHistory objCreditHistory = new CreditHistory();
                                    objCreditHistory.PULLGUID = item.PullGUIDForCreditHistory ?? Guid.Empty;
                                    objCreditHistory.PullDetailGuid = item.PullDetailGUIDForCreditHistory ?? Guid.Empty;

                                    objCreditHistory.CreditDetailGuid = objPullDetailDTO.GUID;

                                    lstCreditGuids.Add(objCreditHistory);

                                    #endregion
                                }
                            }
                        }
                    }
                    #endregion
                }

                #region "PULL Master and Detail Save"

                PullMasterDAL objPullDAL = new PullMasterDAL(base.DataBaseName);
                PullMasterViewDTO objPullDTO = new PullMasterViewDTO();
                objPullDTO = objPullDAL.GetPullByGuidPlain(objPullDetailsDTOs[0].NEW_PullGUID.GetValueOrDefault(Guid.Empty));

                if (objPullDetailsDTOs.Count > 0)
                {
                    objPullDTO.ConsignedQuantity = (objPullDTO.ConsignedQuantity ?? 0) + objPullDetailsDTOs.Select(x => x.ConsignedQuantity.GetValueOrDefault(0)).Sum();
                    objPullDTO.CustomerOwnedQuantity = (objPullDTO.CustomerOwnedQuantity ?? 0) + objPullDetailsDTOs.Select(x => x.CustomerOwnedQuantity.GetValueOrDefault(0)).Sum();

                    lstCreditGuids.ForEach(b => b.CreditGuid = objPullDTO.GUID);
                    objPullDTO.LastUpdatedBy = objPullDetailsDTOs[0].LastUpdatedBy.GetValueOrDefault(0);
                    objPullDTO.PoolQuantity = (objPullDTO.PoolQuantity ?? 0) + objPullDetailsDTOs.Select(x => x.PoolQuantity.GetValueOrDefault(0)).Sum();

                    objPullDTO.PullCost = objPullDTO.PullCost + Convert.ToDouble(objPullDetailsDTOs.Select(x => x.ItemCost.GetValueOrDefault(0) * x.PoolQuantity.GetValueOrDefault(0)).Sum());
                    objPullDTO.PullPrice = objPullDTO.PullPrice + Convert.ToDouble(objPullDetailsDTOs.Select(x => x.ItemPrice.GetValueOrDefault(0) * x.PoolQuantity.GetValueOrDefault(0)).Sum());

                    objPullDTO.WhatWhereAction = WhatWhereAction;

                    if (string.IsNullOrEmpty(objPullDetailsDTOs[0].AddedFrom) || string.IsNullOrWhiteSpace(objPullDetailsDTOs[0].AddedFrom))
                        objPullDTO.AddedFrom = "Web";
                    else
                        objPullDTO.AddedFrom = objPullDetailsDTOs[0].AddedFrom;

                    if (string.IsNullOrEmpty(objPullDetailsDTOs[0].EditedFrom) || string.IsNullOrWhiteSpace(objPullDetailsDTOs[0].EditedFrom))
                        objPullDTO.EditedFrom = "Web";
                    else
                        objPullDTO.EditedFrom = objPullDetailsDTOs[0].EditedFrom;

                    objPullDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objPullDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                    ItemMasterDTO oItemRecord = new ItemMasterDAL(base.DataBaseName).GetItemWithoutJoins(null, objPullDTO.ItemGUID.GetValueOrDefault(Guid.Empty));
                    if (oItemRecord != null && oItemRecord.ID > 0)
                    {
                        oItemRecord.OnHandQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == oItemRecord.GUID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.CustomerOwnedQuantity ?? 0) + (t.ConsignedQuantity ?? 0));

                        objPullDTO.ItemOnhandQty = oItemRecord.OnHandQuantity;
                        objPullDTO.ItemStageQty = oItemRecord.StagedQuantity;
                        objPullDTO.ItemLocationOnHandQty = 0;

                        ItemLocationQTYDTO objItemLocationQuantity = GetItemQtyByLocation(objPullDTO.BinID ?? 0, objPullDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objPullDTO.Room.GetValueOrDefault(0), objPullDTO.CompanyID.GetValueOrDefault(0), SessionUserId);
                        if (objItemLocationQuantity != null && objItemLocationQuantity.BinID > 0)
                        {
                            objPullDTO.ItemLocationOnHandQty = objItemLocationQuantity.CustomerOwnedQuantity.GetValueOrDefault(0) + objItemLocationQuantity.ConsignedQuantity.GetValueOrDefault(0);
                        }
                    }

                    objPullDAL.EditForPullQty(objPullDTO);

                    PullDetailsDAL objPullDetailsDAL = new PullDetailsDAL(base.DataBaseName);
                    foreach (PullDetailsDTO temppdDTO in objPullDetailsDTOs)
                    {
                        temppdDTO.PULLGUID = objPullDTO.GUID;

                        if (string.IsNullOrEmpty(temppdDTO.AddedFrom) || string.IsNullOrWhiteSpace(temppdDTO.AddedFrom))
                            temppdDTO.AddedFrom = "Web";

                        if (string.IsNullOrEmpty(temppdDTO.EditedFrom) || string.IsNullOrWhiteSpace(temppdDTO.EditedFrom))
                            temppdDTO.EditedFrom = "Web";


                        temppdDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        temppdDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objPullDetailsDAL.Insert(temppdDTO);


                        //Project Spend update
                        if (temppdDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                        {
                            ProjectMasterDAL objPrjMsgDAL = new ProjectMasterDAL(base.DataBaseName);
                            ProjectSpendItemsDAL objPrjSpenItmDAL = new ProjectSpendItemsDAL(base.DataBaseName);
                            ProjectMasterDTO objPrjMstDTO = new ProjectMasterDTO();
                            ProjectSpendItemsDTO objPrjSpenItmDTO = new ProjectSpendItemsDTO();
                            var tmpsupplierIds = new List<long>();
                            //objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecords(objPullDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)objPullDetailsDTOs[0].Room, (Int64)objPullDetailsDTOs[0].CompanyID, tmpsupplierIds).Where(x => x.ItemGUID == objPullDTO.ItemGUID && x.Created.Value <= temppdDTO.Created.Value).SingleOrDefault();
                            objPrjSpenItmDTO = objPrjSpenItmDAL.GetProjectSpendItemCompareDate(objPullDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)objPullDetailsDTOs[0].Room, (Int64)objPullDetailsDTOs[0].CompanyID, tmpsupplierIds, Convert.ToString(objPullDTO.ItemGUID), temppdDTO.Created.Value.ToString("yyyy-MM-dd HH:mm:ss")).SingleOrDefault();
                            objPrjMstDTO = objPrjMsgDAL.GetRecord(objPullDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)objPullDetailsDTOs[0].Room, (Int64)objPullDetailsDTOs[0].CompanyID, false, false);

                            if (objPrjMstDTO != null)
                            {

                                objPrjMstDTO.DollarUsedAmount = objPrjMstDTO.DollarUsedAmount.GetValueOrDefault(0) - ((decimal)temppdDTO.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(temppdDTO.ItemCost.GetValueOrDefault(0)));
                                objPrjMstDTO.WhatWhereAction = WhatWhereAction;
                                //objPrjMstDTO.EditedFrom = "Web";
                                //objPrjMstDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objPrjMsgDAL.Edit(objPrjMstDTO);

                                if (objPrjSpenItmDTO != null)
                                {

                                    objPrjSpenItmDTO.DollarUsedAmount = objPrjSpenItmDTO.DollarUsedAmount.GetValueOrDefault(0) - ((decimal)temppdDTO.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(temppdDTO.ItemCost.GetValueOrDefault(0)));
                                    objPrjSpenItmDTO.QuantityUsed = objPrjSpenItmDTO.QuantityUsed.GetValueOrDefault(0) - temppdDTO.PoolQuantity.GetValueOrDefault(0);
                                    //objPrjSpenItmDTO.EditedFrom = "Web";
                                    //objPrjSpenItmDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    objPrjSpenItmDAL.Edit(objPrjSpenItmDTO);
                                }
                            }
                        }

                    }
                }
                #endregion
                if (!IsBothQty)
                {
                    // if (objItemDTO != null && objItemDTO.ItemType != 4)
                    if (objItemDTO != null)
                    {
                        #region "Insert QTY logic"
                        if (IsQTYSave)
                        {
                            if (TempSerialNumberTracking)
                            {
                                #region "Insert mode for Serial Number"
                                ItemLocationQTYDAL objQtyMgt = new ItemLocationQTYDAL(base.DataBaseName);

                                //FOR INSERT MODE
                                var objilqInsert = new List<ItemLocationQTYDTO>();

                                objilqInsert = (from x in objData
                                                where (x.SerialNumberTracking == true && x.SerialNumber != null) && x.BinID != null
                                                group x by new { x.BinID }
                                                    into grp
                                                select new ItemLocationQTYDTO
                                                {
                                                    BinID = Convert.ToInt64(grp.Key.BinID),
                                                    ItemGUID = objData[0].ItemGUID,
                                                    CompanyID = objData[0].CompanyID,
                                                    Room = objData[0].Room,
                                                    LastUpdated = DateTime.Now,
                                                    Created = DateTime.Now,
                                                    CreatedBy = objData[0].CreatedBy,
                                                    LastUpdatedBy = objData[0].LastUpdatedBy,
                                                    CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                                    ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                                    Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                                }).ToList();

                                if (objilqInsert.Count() > 0)
                                {
                                    objQtyMgt.Save(objilqInsert, "Credit", SessionUserId, EnterpriseId);
                                }
                                #endregion
                            }
                            else if (TempLotNumberTracking)
                            {
                                #region "Insert Mode for Lot#, Others"
                                ItemLocationQTYDAL objQtyMgt = new ItemLocationQTYDAL(base.DataBaseName);
                                if (objItemDTO.Consignment)
                                {
                                    var objilq = (from x in objData
                                                  where (x.LotNumberTracking == true && x.LotNumber != null) && x.BinID != null
                                                  group x by new { x.BinID } into grp
                                                  select new ItemLocationQTYDTO
                                                  {
                                                      BinID = Convert.ToInt64(grp.Key.BinID),
                                                      ItemGUID = objData[0].ItemGUID,
                                                      CompanyID = objData[0].CompanyID,
                                                      Room = objData[0].Room,
                                                      LastUpdated = DateTime.Now,
                                                      Created = DateTime.Now,
                                                      CreatedBy = objData[0].CreatedBy,
                                                      LastUpdatedBy = objData[0].LastUpdatedBy,
                                                      CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                                      ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                                      Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                                      //LotNumber = x.LotNumber,
                                                  }).ToList();
                                    //Save
                                    objQtyMgt.Save(objilq, "Credit", SessionUserId, EnterpriseId);
                                    //Update on_hand qunatity
                                    //objilq.Select(c => { OnHandQuantity += c.Quantity; return c; }).ToList();
                                }
                                else
                                {
                                    var objilq = (from x in objData
                                                  where (x.LotNumberTracking == true && x.LotNumber != null) && x.BinID != null
                                                  group x by new { x.BinID } into grp
                                                  select new ItemLocationQTYDTO
                                                  {
                                                      BinID = Convert.ToInt64(grp.Key.BinID),
                                                      ItemGUID = objData[0].ItemGUID,
                                                      CompanyID = objData[0].CompanyID,
                                                      Room = objData[0].Room,
                                                      LastUpdated = DateTime.Now,
                                                      Created = DateTime.Now,
                                                      CreatedBy = objData[0].CreatedBy,
                                                      LastUpdatedBy = objData[0].LastUpdatedBy,
                                                      CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                                      ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                                      Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                                      //LotNumber = x.LotNumber,
                                                  }).ToList();

                                    //Save
                                    objQtyMgt.Save(objilq, "Credit", SessionUserId, EnterpriseId);
                                }
                                //Save on hand quantity
                                #endregion
                            }
                            else
                            {
                                #region "Insert Mode for Lot#, Others"
                                ItemLocationQTYDAL objQtyMgt = new ItemLocationQTYDAL(base.DataBaseName);
                                if (objItemDTO.Consignment)
                                {
                                    var objilq = (from x in objData
                                                  where x.BinID != null
                                                  group x by new { x.BinID } into grp
                                                  select new ItemLocationQTYDTO
                                                  {
                                                      BinID = Convert.ToInt64(grp.Key.BinID),
                                                      ItemGUID = objData[0].ItemGUID,
                                                      CompanyID = objData[0].CompanyID,
                                                      Room = objData[0].Room,
                                                      LastUpdated = DateTime.Now,
                                                      Created = DateTime.Now,
                                                      CreatedBy = objData[0].CreatedBy,
                                                      LastUpdatedBy = objData[0].LastUpdatedBy,
                                                      CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                                      ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                                      Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                                      //LotNumber = x.LotNumber,
                                                  }).ToList();
                                    //Save
                                    objQtyMgt.Save(objilq, "Credit", SessionUserId, EnterpriseId);
                                    //Update on_hand qunatity
                                    //objilq.Select(c => { OnHandQuantity += c.Quantity; return c; }).ToList();
                                }
                                else
                                {
                                    var objilq = (from x in objData
                                                  where x.BinID != null
                                                  group x by new { x.BinID } into grp
                                                  select new ItemLocationQTYDTO
                                                  {
                                                      BinID = Convert.ToInt64(grp.Key.BinID),
                                                      ItemGUID = objData[0].ItemGUID,
                                                      CompanyID = objData[0].CompanyID,
                                                      Room = objData[0].Room,
                                                      LastUpdated = DateTime.Now,
                                                      Created = DateTime.Now,
                                                      CreatedBy = objData[0].CreatedBy,
                                                      LastUpdatedBy = objData[0].LastUpdatedBy,
                                                      CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                                      ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
                                                      Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                                      //LotNumber = x.LotNumber,
                                                  }).ToList();

                                    //Save
                                    objQtyMgt.Save(objilq, "Credit", SessionUserId, EnterpriseId);
                                }
                                //Save on hand quantity
                                #endregion
                            }
                        }
                        #endregion
                    }

                    objPullDTO = new PullMasterDAL(base.DataBaseName).UpdateCumulativeOnHand(objPullDTO);
                }
            }
            return true;
        }

        public bool MaterialStagingPoolDetailsEditForMSCreditPullnew(List<MaterialStagingPullDetailDTO> objData, double NewPullQuantity, double OldPullQuantity, Guid PullGUID, string calledfrom,
            string RoomDateFormat, out List<CreditHistory> lstCreditGuids, long SessionUserId, long EnterpriseId, string PullAction = "ms credit", string WhatWhereAction = "")
        {
            lstCreditGuids = new List<CreditHistory>();
            if (!string.IsNullOrWhiteSpace(calledfrom))
            {
                if ("evmisvc" == calledfrom)
                {
                    PullAction = "mscr_evmi";
                }
            }

            bool TempSerialNumberTracking = false;
            bool TempLotNumberTracking = false;
            //bool Consignment = false;
            ItemMasterDTO objItemDTO = null;
            IEnumerable<MaterialStagingDetailDTO> lstItemDTO = null;
            MaterialStagingDetailDAL objItemDAL = new MaterialStagingDetailDAL(base.DataBaseName);
            bool IsQTYSave = false;
            bool IsBothQty = false;
            string CountdtlGUID = string.Empty;
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                int LooptotalCount = objData.Count();
                int Counter = 0;
                foreach (MaterialStagingPullDetailDTO item in objData)
                {
                    BinMasterDTO Itembin = GetItemBin(item.ItemGUID, item.BinNumber, item.Room ?? 0, item.CompanyID ?? 0, item.CreatedBy ?? 0, true);
                    item.BinID = Itembin.ID;
                }
                List<PullDetailsDTO> objPullDetailsDTOs = new List<PullDetailsDTO>();
                foreach (MaterialStagingPullDetailDTO item in objData)
                {
                    #region "Location Detail Save"
                    Counter++;

                    if (item.BinID == null)
                    {
                        continue;
                    }
                    if (objItemDTO == null)
                    {
                        objItemDTO = new ItemMasterDAL(base.DataBaseName).GetItemWithoutJoins(0, item.ItemGUID);
                    }

                    lstItemDTO = objItemDAL.GetAllRecordsRoomItemWise(item.Room ?? 0, item.CompanyID ?? 0, false, false, item.ItemGUID);

                    item.IsArchived = (item.IsArchived == null ? false : item.IsArchived);
                    item.IsDeleted = (item.IsDeleted == null ? false : item.IsDeleted);
                    BinMasterDAL objBin = new BinMasterDAL(base.DataBaseName);
                    BinMasterDTO objBinDTO = objBin.GetBinByID(Convert.ToInt64(item.BinID), Convert.ToInt64(item.Room), Convert.ToInt64(item.CompanyID));
                    //BinMasterDTO objBinDTO = objBin.GetItemLocation(Convert.ToInt64(item.Room), Convert.ToInt64(item.CompanyID), false, false,Guid.Empty, Convert.ToInt64(item.BinID), null,null).FirstOrDefault();

                    if (string.IsNullOrEmpty(item.Received) && item.ReceivedDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                        item.Received = item.ReceivedDate == null ? string.Empty : item.ReceivedDate.Value.ToString("M/d/yyyy");

                    bool IsValid = false;

                    if (!string.IsNullOrEmpty(item.Received) || item.ReceivedDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                        IsValid = true;
                    else if (!string.IsNullOrEmpty(item.SerialNumber))
                    {
                        IsValid = true;
                    }
                    else if (!string.IsNullOrEmpty(item.LotNumber))
                        IsValid = true;
                    else if (!string.IsNullOrEmpty(item.Expiration) || (item.ExpirationDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue))
                        IsValid = true;
                    else if (item.LotNumberTracking && item.SerialNumberTracking)
                        IsValid = true;
                    else if (item.BinID.HasValue && item.Cost.HasValue && (!string.IsNullOrEmpty(item.Received) || item.ReceivedDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue))
                        IsValid = true;

                    if (IsValid || (LooptotalCount == Counter))
                    {
                        //if (!String.IsNullOrEmpty(item.SerialNumber) || !String.IsNullOrEmpty(item.LotNumber) || item.CustomerOwnedQuantity.Value > 0 || item.ConsignedQuantity.Value > 0)
                        //{
                        double CustVal = 0;
                        double ConsignVal = 0;
                        bool IsConsinAndCustBothQty = false;
                        if (item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0 && item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                        {
                            CustVal = item.CustomerOwnedQuantity.GetValueOrDefault(0);
                            ConsignVal = item.ConsignedQuantity.GetValueOrDefault(0);
                            IsConsinAndCustBothQty = true;
                            IsBothQty = true;
                        }
                        else if (item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0 && item.ConsignedQuantity.GetValueOrDefault(0) <= 0)
                        {
                            CustVal = item.CustomerOwnedQuantity.GetValueOrDefault(0);
                            ConsignVal = 0;
                            IsConsinAndCustBothQty = false;
                            IsBothQty = false;
                        }
                        else if (item.ConsignedQuantity.GetValueOrDefault(0) > 0 && item.CustomerOwnedQuantity.GetValueOrDefault(0) <= 0)
                        {
                            ConsignVal = item.ConsignedQuantity.GetValueOrDefault(0);
                            CustVal = 0;
                            IsBothQty = false;
                            IsConsinAndCustBothQty = false;
                        }

                        if (IsConsinAndCustBothQty)
                        {
                            item.ConsignedQuantity = ConsignVal;
                            item.CustomerOwnedQuantity = 0;
                            SaveBothCustAndConsigenQtyForCountForMS(item, objItemDTO, objBinDTO, IsValid, PullAction, lstItemDTO, RoomDateFormat);

                            item.ConsignedQuantity = 0;
                            item.CustomerOwnedQuantity = CustVal;
                            item.ID = 0;
                            item.GUID = Guid.Empty;
                            SaveBothCustAndConsigenQtyForCountForMS(item, objItemDTO, objBinDTO, IsValid, PullAction, lstItemDTO, RoomDateFormat);
                            continue;
                        }
                        else
                        {
                            MaterialStagingPullDetailDAL objItemDetailDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
                            MaterialStagingPullDetailDTO objItem = new MaterialStagingPullDetailDTO();

                            if (item.MaterialStagingGUID == null || item.MaterialStagingGUID == Guid.Empty)
                            {
                                // RoomDAL objRoomDal = new RoomDAL(base.DataBaseName);
                                // RoomDTO objRoom = new RoomDTO();
                                CommonDAL objCommonDal = new CommonDAL(base.DataBaseName);
                                string columnList = "ID,RoomName,IsIgnoreCreditRule";
                                RoomDTO objRoom = objCommonDal.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + item.Room.GetValueOrDefault(0).ToString() + "", "");
                                // objRoom = objRoomDal.GetRoomByIDPlain(item.Room.GetValueOrDefault(0));

                                if (objRoom != null && objRoom.IsIgnoreCreditRule)
                                {
                                    if (item.MaterialStagingdtlGUID != null && item.MaterialStagingdtlGUID != Guid.Empty)
                                    {
                                        MaterialStagingDetailDTO objMSDTO = new MaterialStagingDetailDTO();
                                        //objMSDTO = new MaterialStagingDetailDAL(base.DataBaseName).GetRecordwithoutCaching(item.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty), item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0));
                                        objMSDTO = new MaterialStagingDetailDAL(base.DataBaseName).GetMaterialStagingDetailByGUID(item.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty), item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0));
                                        if (objMSDTO != null)
                                        {
                                            item.MaterialStagingGUID = objMSDTO.MaterialStagingGUID;
                                        }
                                    }
                                }
                            }

                            objItem.MaterialStagingGUID = item.MaterialStagingGUID;
                            objItem.MaterialStagingdtlGUID = item.MaterialStagingdtlGUID;
                            objItem.BinID = null;
                            if (objBinDTO != null)
                            {
                                objItem.StagingBinId = objBinDTO.ID;
                            }
                            else
                                objItem.StagingBinId = item.BinID ?? 0;

                            objItem.CompanyID = item.CompanyID;
                            objItem.Cost = item.Cost;
                            objItem.Created = item.Created;
                            objItem.CustomerOwnedQuantity = item.CustomerOwnedQuantity;
                            objItem.ConsignedQuantity = item.ConsignedQuantity;

                            objItem.InitialQuantity = (item.ConsignedQuantity ?? 0) + (item.CustomerOwnedQuantity ?? 0);
                            objItem.InitialQuantityWeb = (item.ConsignedQuantity ?? 0) + (item.CustomerOwnedQuantity ?? 0);
                            //objItem.IsConsignedSerialLot = objItemDTO.Consignment;

                            // TODO: - Expiration date should be null if item is not date code trakcing.

                            // objItem.ExpirationDate = item.ExpirationDate;
                            if (item.DateCodeTracking)
                            {
                                objItem.Expiration = item.Expiration;
                                objItem.ExpirationDate = item.ExpirationDate;
                            }

                            objItem.Received = item.Received;
                            objItem.ReceivedDate = item.ReceivedDate;
                            if (objItem.ReceivedDate.GetValueOrDefault(DateTime.MinValue) <= DateTime.MinValue && !string.IsNullOrEmpty(objItem.Received))
                            {
                                objItem.ReceivedDate = DateTime.Parse(objItem.Received);
                            }
                            if (item.DateCodeTracking && objItem.ExpirationDate.GetValueOrDefault(DateTime.MinValue) <= DateTime.MinValue && !string.IsNullOrEmpty(objItem.Expiration))
                            {
                                DateTime dt = new DateTime();
                                if (DateTime.TryParseExact(objItem.Expiration, RoomDateFormat, ResourceHelper.CurrentCult, DateTimeStyles.None, out dt))
                                    objItem.ExpirationDate = dt;
                            }

                            objItem.ID = item.ID; objItem.ItemGUID = item.ItemGUID;

                            objItem.LotNumber = item.LotNumber;
                            if (item.LotNumberTracking)
                                TempLotNumberTracking = true;
                            objItem.SerialNumber = item.SerialNumber;
                            //if (item.SerialNumber != null && item.SerialNumberTracking)
                            if (item.SerialNumberTracking)
                                TempSerialNumberTracking = true;

                            objItem.CreatedBy = item.CreatedBy; objItem.CompanyID = item.CompanyID;
                            objItem.Room = item.Room; objItem.Updated = item.Updated;
                            objItem.LastUpdatedBy = item.LastUpdatedBy;
                            objItem.IsArchived = item.IsArchived; objItem.IsDeleted = item.IsDeleted;
                            objItem.Created = item.Created; objItem.CreatedBy = item.CreatedBy;
                            objItem.OrderDetailGUID = item.OrderDetailGUID;

                            if (objBinDTO != null)
                                objItem.BinNumber = objBinDTO.BinNumber;

                            objItem.ItemLocationDetailGUID = item.ItemLocationDetailGUID;
                            objItem.ItemCost = item.Cost;
                            //objItem.PoolQuantity = item.PoolQuantity;
                            objItem.PoolQuantity = (item.ConsignedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0));
                            objItem.PullCredit = PullAction;
                            if (item.ID > 0 && IsValid)
                            {
                                objItem.GUID = item.GUID;
                                objItem.IsOnlyFromUI = item.IsOnlyFromUI;
                                if (item.IsOnlyFromUI)
                                {
                                    objItem.EditedFrom = "Web";
                                    objItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                                }
                                objItemDetailDAL.Edit(objItem);
                                // here needs to implement the edit quantity logic for location details table
                                IsQTYSave = true;
                            }
                            else
                            {
                                if (IsValid)
                                {
                                    bool IsRecordSave = false;
                                    //// here needs to write code for 4 points as following...
                                    // 1 // if item is SR. then check for duplication if found then make them IsDeleted = 1
                                    //   // And then Insert the New item with same SR. number
                                    // 2 // If item is Lot, then check if duplicate number then only update the quantity
                                    //   // for that particular lot number also check BinLocation if exist then update else insert ....
                                    // 3 // Needs to enter only one entry to PullMaster with Total Quantity and data
                                    // 4 // Then needs to enter all the entry to PullDetails with itemlocation data ..
                                    #region "SR Case"
                                    if (TempSerialNumberTracking)
                                    {
                                        IEnumerable<MaterialStagingPullDetailDTO> TempDupSrs = objItemDetailDAL.GetAllRecordsByItemGuid(item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0), item.ItemGUID, item.SerialNumber).ToList();
                                        if (TempDupSrs != null && TempDupSrs.Count() > 0)
                                        {
                                            foreach (MaterialStagingPullDetailDTO IDTemp in TempDupSrs)
                                            {
                                                IDTemp.IsDeleted = true;
                                                IDTemp.IsArchived = true;
                                                IDTemp.LastUpdatedBy = item.LastUpdatedBy;
                                                IDTemp.Updated = DateTimeUtility.DateTimeNow;
                                                IDTemp.IsOnlyFromUI = true;
                                                IDTemp.EditedFrom = "Web";
                                                IDTemp.ReceivedOn = DateTimeUtility.DateTimeNow;
                                                objItemDetailDAL.Edit(IDTemp);
                                            }
                                        }
                                    }
                                    #endregion
                                    if (!IsRecordSave)
                                    {
                                        objItem.AddedFrom = "Web";
                                        objItem.EditedFrom = "Web";
                                        objItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objItem.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                        objItem.IsOnlyFromUI = item.IsOnlyFromUI;
                                        MaterialStagingPullDetailDTO itemloctemp = objItemDetailDAL.InsertMS(true, objItem, RoomDateFormat);
                                        item.ID = itemloctemp.ID;
                                        item.GUID = itemloctemp.GUID;
                                        IsQTYSave = true;
                                    }
                                    #region "PULL Details Entry"
                                    PullDetailsDTO objPullDetailDTO = new PullDetailsDTO();
                                    objPullDetailDTO.BinID = item.BinID;
                                    objPullDetailDTO.PULLGUID = PullGUID;
                                    objPullDetailDTO.CompanyID = item.CompanyID;
                                    objPullDetailDTO.ConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0);
                                    //objPullDetailDTO.CreditConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0);
                                    objPullDetailDTO.Created = DateTimeUtility.DateTimeNow;
                                    objPullDetailDTO.CreatedBy = item.CreatedBy.GetValueOrDefault(0);
                                    objPullDetailDTO.CreatedByName = item.CreatedByName;
                                    objPullDetailDTO.CustomerOwnedQuantity = item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    //objPullDetailDTO.CreditCustomerOwnedQuantity = item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    objPullDetailDTO.Expiration = item.Expiration;
                                    objPullDetailDTO.IsArchived = item.IsArchived;
                                    objPullDetailDTO.IsDeleted = item.IsDeleted;
                                    objPullDetailDTO.ItemCost = CalculateAndGetPullCreditCost(item.Cost, objItemDTO.CostUOMID, item.Room ?? 0, item.CompanyID ?? 0);
                                    objPullDetailDTO.ItemPrice = CalculateAndGetPullCreditCost(item.SellPrice, objItemDTO.CostUOMID, item.Room ?? 0, item.CompanyID ?? 0);
                                    objPullDetailDTO.ItemGUID = item.ItemGUID;
                                    objPullDetailDTO.LastUpdatedBy = item.LastUpdatedBy.GetValueOrDefault(0);
                                    objPullDetailDTO.LotNumber = item.LotNumber;
                                    //objPullDetailDTO.PoolQuantity = item.PoolQuantity;
                                    objPullDetailDTO.PoolQuantity = (item.ConsignedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0));
                                    objPullDetailDTO.PullCredit = PullAction;
                                    objPullDetailDTO.Received = item.Received;
                                    objPullDetailDTO.Room = item.Room;
                                    objPullDetailDTO.RoomName = item.RoomName;
                                    objPullDetailDTO.SerialNumber = item.SerialNumber;
                                    objPullDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                                    objPullDetailDTO.UpdatedByName = item.UpdatedByName;
                                    //objPullDetailDTO.ItemLocationDetailGUID = item.GUID;
                                    objPullDetailDTO.MaterialStagingPullDetailGUID = item.GUID;
                                    objPullDetailDTO.ProjectSpendGUID = item.ProjectSpentGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : item.ProjectSpentGUID;
                                    objPullDetailDTO.WorkOrderGUID = item.WorkOrderGuid;
                                    objPullDetailDTO.COuntLineItemGUID = item.CountLineItemDtlGUID;

                                    if (string.IsNullOrEmpty(item.AddedFrom) || string.IsNullOrWhiteSpace(item.AddedFrom))
                                        objPullDetailDTO.AddedFrom = "Web";
                                    else
                                        objPullDetailDTO.AddedFrom = item.AddedFrom;

                                    if (string.IsNullOrEmpty(item.EditedFrom) || string.IsNullOrWhiteSpace(item.EditedFrom))
                                        objPullDetailDTO.EditedFrom = "Web";
                                    else
                                        objPullDetailDTO.EditedFrom = item.EditedFrom;

                                    objPullDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    objPullDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                    objPullDetailDTO.PullOrderNumber = item.PullOrderNumber;
                                    objPullDetailDTO.SupplierAccountGuid = item.SupplierAccountGuid;
                                    objPullDetailDTO.GUID = Guid.NewGuid();
                                    CreditHistory objCreditHistory = new CreditHistory();
                                    objCreditHistory.PULLGUID = item.PullGUIDForCreditHistory ?? Guid.Empty;
                                    objCreditHistory.PullDetailGuid = item.PullDetailGUIDForCreditHistory ?? Guid.Empty;

                                    objCreditHistory.CreditDetailGuid = objPullDetailDTO.GUID;

                                    lstCreditGuids.Add(objCreditHistory);

                                    objPullDetailsDTOs.Add(objPullDetailDTO);
                                    #endregion
                                }
                            }
                            //Update staged quantity...
                            objItemDetailDAL.UpdateStagedQuantity(item.ItemGUID, item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0), SessionUserId, EnterpriseId);

                        }
                    }
                    #endregion
                }

                #region "PULL Master and Detail Save"

                PullMasterDAL objPullDAL = new PullMasterDAL(base.DataBaseName);
                PullMasterViewDTO objPullDTO = new PullMasterViewDTO();
                objPullDTO = objPullDAL.GetPullByGuidPlain(objPullDetailsDTOs[0].PULLGUID.GetValueOrDefault(Guid.Empty));

                if (objPullDetailsDTOs.Count > 0)
                {
                    objPullDTO.ConsignedQuantity = (objPullDTO.ConsignedQuantity ?? 0) + objPullDetailsDTOs.Select(x => x.ConsignedQuantity.GetValueOrDefault(0)).Sum();
                    objPullDTO.CustomerOwnedQuantity = (objPullDTO.CustomerOwnedQuantity ?? 0) + objPullDetailsDTOs.Select(x => x.CustomerOwnedQuantity.GetValueOrDefault(0)).Sum();

                    lstCreditGuids.ForEach(b => b.CreditGuid = objPullDTO.GUID);
                    objPullDTO.LastUpdatedBy = objPullDetailsDTOs[0].LastUpdatedBy.GetValueOrDefault(0);
                    objPullDTO.PoolQuantity = (objPullDTO.PoolQuantity ?? 0) + objPullDetailsDTOs.Select(x => x.PoolQuantity.GetValueOrDefault(0)).Sum();

                    objPullDTO.PullCost = objPullDTO.PullCost + Convert.ToDouble(objPullDetailsDTOs.Select(x => x.ItemCost.GetValueOrDefault(0) * x.PoolQuantity.GetValueOrDefault(0)).Sum());
                    objPullDTO.PullPrice = objPullDTO.PullPrice + Convert.ToDouble(objPullDetailsDTOs.Select(x => x.ItemPrice.GetValueOrDefault(0) * x.PoolQuantity.GetValueOrDefault(0)).Sum());

                    objPullDTO.WhatWhereAction = WhatWhereAction;

                    if (string.IsNullOrEmpty(objPullDetailsDTOs[0].AddedFrom) || string.IsNullOrWhiteSpace(objPullDetailsDTOs[0].AddedFrom))
                        objPullDTO.AddedFrom = "Web";
                    else
                        objPullDTO.AddedFrom = objPullDetailsDTOs[0].AddedFrom;

                    if (string.IsNullOrEmpty(objPullDetailsDTOs[0].EditedFrom) || string.IsNullOrWhiteSpace(objPullDetailsDTOs[0].EditedFrom))
                        objPullDTO.EditedFrom = "Web";
                    else
                        objPullDTO.EditedFrom = objPullDetailsDTOs[0].EditedFrom;

                    objPullDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objPullDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                    ItemMasterDTO oItemRecord = new ItemMasterDAL(base.DataBaseName).GetItemWithoutJoins(null, objPullDTO.ItemGUID.GetValueOrDefault(Guid.Empty));
                    if (oItemRecord != null && oItemRecord.ID > 0)
                    {
                        oItemRecord.StagedQuantity = context.MaterialStagingPullDetails.Where(t => t.ItemGUID == oItemRecord.GUID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.CustomerOwnedQuantity ?? 0) + (t.ConsignedQuantity ?? 0));

                        objPullDTO.ItemOnhandQty = oItemRecord.OnHandQuantity;
                        objPullDTO.ItemStageQty = oItemRecord.StagedQuantity;
                        objPullDTO.ItemStageLocationQty = 0;

                        MaterialStagingPullDetailDAL objMaterialStagingPullDetailDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
                        MaterialStagingPullDetailDTO objItemLocationStageQuantity = objMaterialStagingPullDetailDAL.GetItemStagingQtyByLocation(objPullDTO.BinID ?? 0, oItemRecord.GUID, objPullDTO.Room.GetValueOrDefault(0), objPullDTO.CompanyID.GetValueOrDefault(0), SessionUserId);
                        if (objItemLocationStageQuantity != null && objItemLocationStageQuantity.StagingBinId > 0)
                        {
                            objPullDTO.ItemStageLocationQty = objItemLocationStageQuantity.CustomerOwnedQuantity.GetValueOrDefault(0) + objItemLocationStageQuantity.ConsignedQuantity.GetValueOrDefault(0);
                        }
                    }

                    objPullDAL.EditForPullQty(objPullDTO);
                    objPullDAL.InsertPullEditHistory(objPullDTO.GUID, NewPullQuantity, OldPullQuantity, WhatWhereAction);

                    //objPullDAL.Edit(objPullDTO);
                    PullDetailsDAL objPullDetailsDAL = new PullDetailsDAL(base.DataBaseName);
                    foreach (PullDetailsDTO temppdDTO in objPullDetailsDTOs)
                    {
                        temppdDTO.PULLGUID = objPullDTO.GUID;

                        if (string.IsNullOrEmpty(temppdDTO.AddedFrom) || string.IsNullOrWhiteSpace(temppdDTO.AddedFrom))
                            temppdDTO.AddedFrom = "Web";

                        if (string.IsNullOrEmpty(temppdDTO.EditedFrom) || string.IsNullOrWhiteSpace(temppdDTO.EditedFrom))
                            temppdDTO.EditedFrom = "Web";


                        temppdDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        temppdDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objPullDetailsDAL.Insert(temppdDTO);


                        //Project Spend update
                        if (temppdDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                        {
                            ProjectMasterDAL objPrjMsgDAL = new ProjectMasterDAL(base.DataBaseName);
                            ProjectSpendItemsDAL objPrjSpenItmDAL = new ProjectSpendItemsDAL(base.DataBaseName);
                            ProjectMasterDTO objPrjMstDTO = new ProjectMasterDTO();
                            ProjectSpendItemsDTO objPrjSpenItmDTO = new ProjectSpendItemsDTO();
                            var tmpsupplierIds = new List<long>();
                            //objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecords(objPullDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)objPullDetailsDTOs[0].Room, (Int64)objPullDetailsDTOs[0].CompanyID, tmpsupplierIds).Where(x => x.ItemGUID == objPullDTO.ItemGUID && x.Created.Value <= temppdDTO.Created.Value).SingleOrDefault();
                            objPrjSpenItmDTO = objPrjSpenItmDAL.GetProjectSpendItemCompareDate(objPullDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)objPullDetailsDTOs[0].Room, (Int64)objPullDetailsDTOs[0].CompanyID, tmpsupplierIds, Convert.ToString(objPullDTO.ItemGUID), temppdDTO.Created.Value.ToString("yyyy-MM-dd HH:mm:ss")).SingleOrDefault();
                            objPrjMstDTO = objPrjMsgDAL.GetRecord(objPullDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)objPullDetailsDTOs[0].Room, (Int64)objPullDetailsDTOs[0].CompanyID, false, false);

                            if (objPrjMstDTO != null)
                            {

                                objPrjMstDTO.DollarUsedAmount = objPrjMstDTO.DollarUsedAmount.GetValueOrDefault(0) - ((decimal)temppdDTO.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(temppdDTO.ItemCost.GetValueOrDefault(0)));
                                objPrjMstDTO.WhatWhereAction = WhatWhereAction;
                                //objPrjMstDTO.EditedFrom = "Web";
                                //objPrjMstDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objPrjMsgDAL.Edit(objPrjMstDTO);

                                if (objPrjSpenItmDTO != null)
                                {

                                    objPrjSpenItmDTO.DollarUsedAmount = objPrjSpenItmDTO.DollarUsedAmount.GetValueOrDefault(0) - ((decimal)temppdDTO.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(temppdDTO.ItemCost.GetValueOrDefault(0)));
                                    objPrjSpenItmDTO.QuantityUsed = objPrjSpenItmDTO.QuantityUsed.GetValueOrDefault(0) - temppdDTO.PoolQuantity.GetValueOrDefault(0);
                                    //objPrjSpenItmDTO.EditedFrom = "Web";
                                    //objPrjSpenItmDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    objPrjSpenItmDAL.Edit(objPrjSpenItmDTO);
                                }
                            }
                        }

                    }
                }
                #endregion
                if (!IsBothQty)
                {
                    // if (objItemDTO != null && objItemDTO.ItemType != 4)
                    if (objItemDTO != null)
                    {
                        #region "Insert QTY logic"
                        if (IsQTYSave)
                        {
                            MaterialStagingDetailDTO objMaterialStagingDetailDTO = new MaterialStagingDetailDTO();

                            if (TempSerialNumberTracking)
                            {
                                #region "Insert mode for Serial Number"
                                MaterialStagingDetailDAL objQtyMgt = new MaterialStagingDetailDAL(base.DataBaseName);

                                //FOR INSERT MODE
                                var objilqInsert = new List<MaterialStagingDetailDTO>();

                                objilqInsert = (from x in objData
                                                where (x.SerialNumberTracking == true && x.SerialNumber != null) && x.BinID != null
                                                group x by new { x.BinID }
                                                    into grp
                                                select new MaterialStagingDetailDTO
                                                {
                                                    MaterialStagingGUID = objData[0].MaterialStagingGUID,
                                                    BinID = null,
                                                    StagingBinID = objData[0].StagingBinId,
                                                    StagingBinName = objData[0].BinNumber,
                                                    StagingBinGUID = objData[0].GUID,
                                                    ItemGUID = objData[0].ItemGUID,
                                                    CompanyID = objData[0].CompanyID ?? 0,
                                                    RoomId = objData[0].Room ?? 0,
                                                    Updated = DateTime.Now,
                                                    Created = DateTime.Now,
                                                    CreatedBy = objData[0].CreatedBy ?? 0,
                                                    LastUpdatedBy = objData[0].LastUpdatedBy ?? 0,
                                                    CustomerOwnedQuantity = Convert.ToInt16(grp.Sum(x => x.CustomerOwnedQuantity ?? 0)),
                                                    ConsignedQuantity = Convert.ToInt16(grp.Sum(x => x.ConsignedQuantity ?? 0)),
                                                    Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
                                                }).ToList();

                                if (objilqInsert.Count() > 0)
                                {
                                    foreach (MaterialStagingDetailDTO objDataInsert in objilqInsert)
                                    {
                                        objMaterialStagingDetailDTO = objQtyMgt.SaveMaterialStagingDetailsForMSCredit(objDataInsert);
                                    }
                                }
                                #endregion
                            }
                            else if (TempLotNumberTracking)
                            {
                                #region "Insert Mode for Lot#, Others"
                                MaterialStagingDetailDAL objQtyMgt = new MaterialStagingDetailDAL(base.DataBaseName);
                                if (true //objItemDTO.Consignment
                                    )
                                {
                                    var objilq = (from x in objData
                                                  where (x.LotNumberTracking == true && x.LotNumber != null) && x.BinID != null
                                                  group x by new { x.BinID } into grp
                                                  select new MaterialStagingDetailDTO
                                                  {
                                                      MaterialStagingGUID = objData[0].MaterialStagingGUID,
                                                      BinID = null,
                                                      StagingBinID = objData[0].StagingBinId,
                                                      StagingBinName = objData[0].BinNumber,
                                                      StagingBinGUID = objData[0].GUID,
                                                      ItemGUID = objData[0].ItemGUID,
                                                      CompanyID = objData[0].CompanyID ?? 0,
                                                      RoomId = objData[0].Room ?? 0,
                                                      Updated = DateTime.Now,
                                                      Created = DateTime.Now,
                                                      CreatedBy = objData[0].CreatedBy ?? 0,
                                                      LastUpdatedBy = objData[0].LastUpdatedBy ?? 0,
                                                      CustomerOwnedQuantity = Convert.ToInt32(grp.Sum(x => x.CustomerOwnedQuantity ?? 0)),
                                                      ConsignedQuantity = Convert.ToInt32(grp.Sum(x => x.ConsignedQuantity ?? 0)),
                                                      Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0)
                                                      //LotNumber = x.LotNumber,
                                                  }).ToList();
                                    //Save
                                    if (objilq.Count() > 0)
                                    {
                                        foreach (MaterialStagingDetailDTO objDataInsertLot in objilq)
                                        {
                                            objMaterialStagingDetailDTO = objQtyMgt.SaveMaterialStagingDetailsForMSCredit(objDataInsertLot);
                                        }
                                    }
                                    //Update on_hand qunatity
                                    //objilq.Select(c => { OnHandQuantity += c.Quantity; return c; }).ToList();
                                }
                                //Save on hand quantity
                                #endregion
                            }
                            else
                            {
                                #region "Insert Mode for Lot#, Others"
                                MaterialStagingDetailDAL objQtyMgt = new MaterialStagingDetailDAL(base.DataBaseName);
                                if (true//objItemDTO.Consignment
                                    )
                                {
                                    var objilq = (from x in objData
                                                  where x.BinID != null
                                                  group x by new { x.BinID } into grp
                                                  select new MaterialStagingDetailDTO
                                                  {
                                                      MaterialStagingGUID = objData[0].MaterialStagingGUID,
                                                      BinID = null,
                                                      StagingBinID = objData[0].StagingBinId,
                                                      StagingBinName = objData[0].BinNumber,
                                                      StagingBinGUID = objData[0].GUID,
                                                      ItemGUID = objData[0].ItemGUID,
                                                      CompanyID = objData[0].CompanyID ?? 0,
                                                      RoomId = Convert.ToInt32(objData[0].Room),
                                                      Updated = DateTime.Now,
                                                      Created = DateTime.Now,
                                                      CreatedBy = objData[0].CreatedBy ?? 0,
                                                      LastUpdatedBy = objData[0].LastUpdatedBy ?? 0,
                                                      CustomerOwnedQuantity = Convert.ToInt32(grp.Sum(x => x.CustomerOwnedQuantity ?? 0)),
                                                      ConsignedQuantity = Convert.ToInt32(grp.Sum(x => x.ConsignedQuantity ?? 0)),
                                                      Quantity = grp.Sum(x => x.ConsignedQuantity ?? 0) + grp.Sum(x => x.CustomerOwnedQuantity ?? 0)
                                                      //LotNumber = x.LotNumber,
                                                  }).ToList();
                                    //Save
                                    if (objilq.Count() > 0)
                                    {
                                        foreach (MaterialStagingDetailDTO objDataInsertLot in objilq)
                                        {
                                            objMaterialStagingDetailDTO = objQtyMgt.SaveMaterialStagingDetailsForMSCredit(objDataInsertLot);
                                        }
                                    }
                                    //Update on_hand qunatity
                                    //objilq.Select(c => { OnHandQuantity += c.Quantity; return c; }).ToList();
                                }
                                //Save on hand quantity
                                #endregion
                            }

                            #region Update MaterialStagingdtlGUID in MaterialStagingPullDetail Table

                            if (objMaterialStagingDetailDTO != null && objMaterialStagingDetailDTO.GUID != Guid.Empty)
                            {
                                foreach (MaterialStagingPullDetailDTO item in objData)
                                {
                                    MaterialStagingPullDetailDAL objMaterialPullDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
                                    MaterialStagingPullDetailDTO objMaterialPullDTO = new MaterialStagingPullDetailDTO();

                                    objMaterialPullDTO = objMaterialPullDAL.GetMsPullDetailsByMsPullDetailsGUID(item.GUID);
                                    if (objMaterialPullDTO != null && objMaterialPullDTO.GUID != Guid.Empty)
                                    {
                                        objMaterialPullDTO.MaterialStagingdtlGUID = objMaterialStagingDetailDTO.GUID;
                                        objMaterialPullDAL.Edit(objMaterialPullDTO);
                                    }
                                }
                            }

                            #endregion

                        }
                        #endregion
                    }

                    objPullDTO = new PullMasterDAL(base.DataBaseName).UpdateCumulativeOnHandNewForMS(objPullDTO);

                }
            }
            return true;
        }

        public ItemLocationDetailsDTO GetItemLocationDetailsByItemGUID(Guid ItemGUID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramA = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomID", RoomID) };
                return context.Database.SqlQuery<ItemLocationDetailsDTO>("GetItemLocationQuantityByItemGUID @ItemGUID,@CompanyID,@RoomID", paramA).FirstOrDefault();
            }
        }
        #endregion

        public List<ItemLocationDetailsDTO> GetItemLocationDetailsHistoryByGuidFull(Guid Guid)
        {
            List<ItemLocationDetailsDTO> lstItemLocationDetails = new List<ItemLocationDetailsDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramA = new SqlParameter[] { new SqlParameter("@GUID", Guid) };
                return context.Database.SqlQuery<ItemLocationDetailsDTO>("exec [GetItemLocationDetailsHistoryByGuidFull] @GUID", paramA).ToList();
            }

        }

        public List<ItemLocationDetailsDTO> GetItemLocationeForKitMove(Guid ItemGUID, long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramA = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID),
                                                       new SqlParameter("@RoomID", RoomId),
                                                       new SqlParameter("@CompanyID", CompanyId) };
                return context.Database.SqlQuery<ItemLocationDetailsDTO>("exec [GetItemLocationeForKitMove] @ItemGUID,@RoomID,@CompanyID", paramA).ToList();
            }
        }

        public List<ItemLocationDetailsDTO> GetItemLocationeDetailsByItemGuidBinIdPlain(Guid ItemGUID, long BinId, long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramA = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID),
                                                       new SqlParameter("@BinId", BinId),
                                                       new SqlParameter("@RoomID", RoomId),
                                                       new SqlParameter("@CompanyID", CompanyId) };
                return context.Database.SqlQuery<ItemLocationDetailsDTO>("exec [GetItemLocationeDetailsByItemGuidBinIdPlain] @ItemGUID,@BinId,@RoomID,@CompanyID", paramA).ToList();
            }
        }
    }
}


