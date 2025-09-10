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
    public partial class ItemLocationQTYDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public ItemLocationQTYDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ItemLocationQTYDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        public IEnumerable<ItemLocationQTYDTO> GetItemLocationQTY(Int64 RoomID, Int64 CompanyId, Int64? BinID, string ItemGUID)
        {
            List<ItemLocationQTYDTO> lstItemLocationQTY = new List<ItemLocationQTYDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),  
                                                   new SqlParameter("@CompanyID", CompanyId),
                                                   new SqlParameter("@ItemGUID", ItemGUID ?? (object)DBNull.Value),
                                                   new SqlParameter("@BinID", BinID ?? (object)DBNull.Value) };

                lstItemLocationQTY = (from u in context.Database.SqlQuery<ItemLocationQTYDTO>("exec [GetItemLocationQTYByItemBin] @RoomID,@CompanyID,@ItemGUID,@BinID", params1)
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

            return lstItemLocationQTY;
        }

        public ItemLocationQTYDTO GetItemLocationQTYByItemBinPlain(long RoomID, long CompanyId, long? BinID, string ItemGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyId),
                                                   new SqlParameter("@ItemGUID", ItemGUID ?? (object)DBNull.Value),
                                                   new SqlParameter("@BinID", BinID ?? (object)DBNull.Value) };

                return context.Database.SqlQuery<ItemLocationQTYDTO>("exec [GetItemLocationQTYByItemBinPlain] @RoomID,@CompanyID,@ItemGUID,@BinID", params1).FirstOrDefault();
                                      
            }            
        }

        public IEnumerable<ItemLocationQTYDTO> GetItemLocationPositiveQTYByItemGUID(Int64 RoomID, Int64 CompanyId, Int64? BinID, Guid ItemGUID)
        {
            List<ItemLocationQTYDTO> lstItemLocationQTY = new List<ItemLocationQTYDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyId),
                                                   new SqlParameter("@ItemGUID", ItemGUID),
                                                   new SqlParameter("@BinID", BinID ?? (object)DBNull.Value) };

                lstItemLocationQTY = (from u in context.Database.SqlQuery<ItemLocationQTYDTO>("exec [GetItemLocationQTYByItemGUID] @RoomID,@CompanyID,@ItemGUID,@BinID", params1)
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

            return lstItemLocationQTY;
        }

        public IEnumerable<ItemLocationQTYDTO> GetRecordByItem(Guid ItemGUID, Int64 RoomID, Int64 CompanyID)
        {
            //return GetCachedData(RoomID, CompanyID).Where(t => t.ItemID == ItemID && t.BinID == BinID).SingleOrDefault();
             
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID), 
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID) };

                IEnumerable<ItemLocationQTYDTO> obj = (from u in context.Database.SqlQuery<ItemLocationQTYDTO>("exec [GetItemLocationQTYByItem] @ItemGUID,@RoomID,@CompanyID", params1)
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
                                                       }).AsParallel().ToList();
                return obj;
            }
        }

        public IEnumerable<ItemLocationQTYDTO> GetRecordByItemBIn(Guid ItemGUID, Int64 RoomID, Int64 CompanyID, Int64? BinID, bool? IsQty)
        {
            
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@BinID", BinID ?? (object)DBNull.Value),
                                                   new SqlParameter("@IsQty", IsQty ?? (object)DBNull.Value) };

                IEnumerable<ItemLocationQTYDTO> obj = (from u in context.Database.SqlQuery<ItemLocationQTYDTO>("exec [GetItemLocationQTYByItemBinID] @ItemGUID,@RoomID,@CompanyID,@BinID,@IsQty", params1)
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
                                                       }).AsParallel().ToList();
                return obj;
            }
        }

        public double GetItemaConsignedQty(Guid ItemGUID, Int64 RoomID, Int64 CompanyID)
        {
            double QtySum = 0;
            IEnumerable<ItemLocationQTYDTO> lstLocQty = null;
            lstLocQty = this.GetRecordByItem(ItemGUID, RoomID, CompanyID);
            if (lstLocQty != null && lstLocQty.Count() > 0)
            {
                QtySum = lstLocQty.Sum(t => (t.ConsignedQuantity ?? 0));
            }
            return QtySum;
        }

        public ItemLocationQTYDTO GetRecordByBinItem(Guid ItemGUID, Int64 BinID, Int64 RoomID, Int64 CompanyID)
        {
            //return GetCachedData(RoomID, CompanyID).Where(t => t.ItemID == ItemID && t.BinID == BinID).SingleOrDefault();
            
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),  
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@ItemGUID", ItemGUID),
                                                   new SqlParameter("@BinID", BinID) };

                ItemLocationQTYDTO obj = (from u in context.Database.SqlQuery<ItemLocationQTYDTO>("exec [GetItemLocationQTYBinItemGUID] @RoomID,@CompanyID,@ItemGUID,@BinID", params1)
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
                                          }).FirstOrDefault();
                return obj;
            }
        }

        public List<ItemLocationDetailsDTO> GetItemBinsHaveQty(Guid ItemGUID, Int64 RoomID, Int64 CompanyID)
        {
            bool Consignment = false;
            List<ItemLocationDetailsDTO> lstLocations = new List<ItemLocationDetailsDTO>();
            ItemMasterDTO objItemMasterDTO = new ItemMasterDAL(base.DataBaseName).GetItemWithoutJoins(null, ItemGUID);
            if (objItemMasterDTO != null)
            {
                Consignment = objItemMasterDTO.Consignment;
            }
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                lstLocations = (from ilq in context.ItemLocationDetails
                                join Itm in context.ItemMasters on ilq.ItemGUID equals Itm.GUID
                                join bn in context.BinMasters on ilq.BinID equals bn.ID
                                where ilq.ItemGUID == ItemGUID && ilq.Room == RoomID && ilq.CompanyID == CompanyID && ilq.IsArchived == false && ilq.IsDeleted == false && bn.IsArchived == false && bn.IsDeleted == false && (Consignment ? ((ilq.CustomerOwnedQuantity ?? 0) + (ilq.ConsignedQuantity ?? 0) > 0) : (ilq.CustomerOwnedQuantity ?? 0) > 0)
                                group ilq by new { ilq.ItemGUID, ilq.BinID, Itm.ItemNumber, bn.BinNumber } into groupedItems
                                select new ItemLocationDetailsDTO
                                {
                                    ItemGUID = groupedItems.Key.ItemGUID,
                                    ItemNumber = groupedItems.Key.ItemNumber,
                                    BinID = groupedItems.Key.BinID,
                                    BinNumber = groupedItems.Key.BinNumber,
                                    ConsignedQuantity = groupedItems.Sum(t => t.ConsignedQuantity),
                                    CustomerOwnedQuantity = groupedItems.Sum(t => t.CustomerOwnedQuantity)
                                }).OrderBy(t => t.BinNumber).ToList();
            }
            return lstLocations;
        }

        public List<ItemLocationQTYDTO> GetBinsByItem(Guid ItemGUID, Int64 RoomID, Int64 CompanyID, string sortColumnName, bool IsLocationRequireQty = false)
        {
            List<ItemLocationQTYDTO> lstLocations = new List<ItemLocationQTYDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstLocations = (from iqty in context.BinMasters
                                join Itm in context.ItemMasters on new { ItemGUID = (iqty.ItemGUID ?? Guid.Empty) } equals new { ItemGUID = Itm.GUID }
                                join ilq in context.ItemLocationQTies on new { BinId = iqty.ID, ItemGUID = (iqty.ItemGUID ?? Guid.Empty) } equals new { BinId = ilq.BinID, ItemGUID = (ilq.ItemGUID ?? Guid.Empty) } into iqty_ilq_join
                                from iqty_ilq in iqty_ilq_join.DefaultIfEmpty()
                                join use1 in context.UserMasters on new { userid = (Int64)(iqty.CreatedBy) } equals new { userid = use1.ID }
                                join use2 in context.UserMasters on new { userid1 = (Int64)(iqty.LastUpdatedBy) } equals new { userid1 = use2.ID }
                                join evmi in context.ItemLocationeVMISetups on new { BinID = iqty.ID, iqty.ItemGUID } equals new { BinID = (Int64)(evmi.BinID), evmi.ItemGUID } into iqty_evmi_join
                                from iqty_evmi in iqty_evmi_join.DefaultIfEmpty()
                                join costuom in context.CostUOMMasters on new { CostUOMID = Itm.CostUOMID ?? 0 } equals new { CostUOMID = costuom.ID } into costuom_Itm_join
                                from costuom_Itm in costuom_Itm_join.DefaultIfEmpty()
                                where iqty.ItemGUID == ItemGUID && iqty.Room == RoomID && iqty.CompanyID == CompanyID && iqty.IsDeleted == false && iqty.IsStagingLocation == false
                                //&& (((iqty_ilq.CustomerOwnedQuantity ?? 0) > 0) || ((iqty_ilq.ConsignedQuantity ?? 0) > 0))
                                select new ItemLocationQTYDTO
                                {
                                    ID = iqty.ID,
                                    BinID = iqty.ID,
                                    CustomerOwnedQuantity = iqty_ilq.CustomerOwnedQuantity,
                                    ConsignedQuantity = iqty_ilq.ConsignedQuantity,
                                    Quantity = iqty_ilq == null ? 0 : iqty_ilq.Quantity,
                                    LotNumber = iqty_ilq.LotNumber,
                                    GUID = iqty.GUID,
                                    ItemGUID = iqty.ItemGUID,
                                    Created = iqty_ilq.Created,
                                    LastUpdated = iqty_ilq.LastUpdated,
                                    CreatedByName = use1.UserName,
                                    UpdatedByName = use2.UserName,
                                    Room = iqty.Room,
                                    CompanyID = iqty.CompanyID,
                                    BinNumber = iqty.BinNumber,
                                    ItemNumber = Itm.ItemNumber,
                                    CriticalQuantity = iqty.CriticalQuantity,
                                    MaximumQuantity = iqty.MaximumQuantity,
                                    MinimumQuantity = iqty.MinimumQuantity,
                                    SuggestedOrderQuantity = iqty.SuggestedOrderQuantity,
                                    Cost = Itm.Cost,
                                    Markup = Itm.Markup,
                                    SellPrice = Itm.SellPrice,
                                    Averagecost = Itm.AverageCost,
                                    ExtendedCost = Itm.ExtendedCost,
                                    CostUOMID = Itm.CostUOMID,
                                    eVMISensorID = iqty_evmi.eVMISensorID,
                                    eVMISensorPort = iqty_evmi.eVMISensorPort,
                                    CostUOMName = costuom_Itm.CostUOM,
                                    IsItemLevelMinMaxQtyRequired = Itm.IsItemLevelMinMaxQtyRequired
                                }).OrderBy(t => t.BinNumber).ToList();
            }
            if (IsLocationRequireQty)
                lstLocations = lstLocations.Where(t => (t.ConsignedQuantity ?? 0) != 0 || (t.CustomerOwnedQuantity ?? 0) != 0).ToList();
            return lstLocations.OrderBy(sortColumnName).ToList();
        }

        /// <summary>
        /// Insert Record in the DataBase ItemLocationQTY
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(ItemLocationQTYDTO objDTO, long SessionUserId,long EnterpriseId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemLocationQTY obj = new ItemLocationQTY();
                obj.ID = 0;

                obj.BinID = objDTO.BinID;
                obj.Quantity = objDTO.Quantity;
                obj.CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity;
                obj.ConsignedQuantity = objDTO.ConsignedQuantity;
                obj.LotNumber = objDTO.LotNumber;
                obj.GUID = Guid.NewGuid();
                obj.ItemGUID = objDTO.ItemGUID;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.LastUpdated = objDTO.LastUpdated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.AddedFrom = "Web";
                obj.EditedFrom = "Web";
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                context.ItemLocationQTies.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;

                if (obj.ID > 0)
                {
                    UpdateOnHandQuantity(objDTO.ItemGUID.Value, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), SessionUserId,EnterpriseId);
                }

                return obj.ID;
            }



        }

        /// <summary>
        /// Save list of Records....
        /// </summary>
        /// <param name="lstitem"></param>
        /// <returns></returns>
        public bool Save(List<ItemLocationQTYDTO> lstitem, long SessionUserId,long EnterpriseId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                foreach (var objDTO in lstitem)
                {
                    DeleteItemLocationsByItemAndBinId(objDTO.ItemGUID.Value, Convert.ToInt64(objDTO.BinID), Convert.ToInt64(objDTO.Room), Convert.ToInt64(objDTO.CompanyID));
                    ItemLocationQTY obj = new ItemLocationQTY();
                    obj.BinID = objDTO.BinID;
                    //obj.CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity;
                    //obj.ConsignedQuantity = objDTO.ConsignedQuantity;
                    //obj.Quantity = objDTO.CustomerOwnedQuantity.GetValueOrDefault(0) + objDTO.ConsignedQuantity.GetValueOrDefault(0);

                    IEnumerable<ItemLocationDetailsDTO> lstItmLocatinDetail = new ItemLocationDetailsDAL(base.DataBaseName).GetItemLocationDetailsByBinIdAndItemGuidPlain(objDTO.BinID, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), "ID Desc");
                    double custQty = lstItmLocatinDetail.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0));
                    double consQty = lstItmLocatinDetail.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0));

                    obj.CustomerOwnedQuantity = custQty;
                    obj.ConsignedQuantity = consQty;
                    obj.Quantity = consQty + custQty;

                    obj.LotNumber = objDTO.LotNumber;
                    obj.GUID = Guid.NewGuid();
                    obj.ItemGUID = objDTO.ItemGUID;
                    obj.Created = objDTO.Created;
                    obj.LastUpdated = objDTO.LastUpdated;
                    obj.CreatedBy = objDTO.CreatedBy;
                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    obj.Room = objDTO.Room;
                    obj.CompanyID = objDTO.CompanyID;

                    obj.AddedFrom = "Web";
                    obj.EditedFrom = "Web";

                    if (!string.IsNullOrEmpty(objDTO.AddedFrom))
                        obj.AddedFrom = objDTO.AddedFrom;

                    if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                        obj.EditedFrom = objDTO.EditedFrom;

                    obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                    obj.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    context.ItemLocationQTies.Add(obj);
                    context.SaveChanges();
                    objDTO.ID = obj.ID;

                    //UpdateOnHandQuantity(objDTO.ItemGUID.Value, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));
                }
                if (lstitem.Count() > 0)
                    UpdateOnHandQuantity(lstitem[0].ItemGUID.Value, lstitem[0].Room.GetValueOrDefault(0), lstitem[0].CompanyID.GetValueOrDefault(0), SessionUserId,EnterpriseId);

                // done below changes of UpdateOnHandQTY to avoid un-nessasory updation in Item table.

                return true;
            }
        }

        /// <summary>
        /// Save list of Records....
        /// </summary>
        /// <param name="lstitem"></param>
        /// <returns></returns>
        public bool Save(List<ItemLocationQTYDTO> lstitem, string WhatWhereAction, long SessionUserId,long EnterpriseId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                foreach (var objDTO in lstitem)
                {
                    DeleteItemLocationsByItemAndBinId(objDTO.ItemGUID.Value, Convert.ToInt64(objDTO.BinID), Convert.ToInt64(objDTO.Room), Convert.ToInt64(objDTO.CompanyID));
                    ItemLocationQTY obj = new ItemLocationQTY();

                    obj.BinID = objDTO.BinID;

                    //obj.CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity;
                    //obj.ConsignedQuantity = objDTO.ConsignedQuantity;
                    //obj.Quantity = objDTO.CustomerOwnedQuantity.GetValueOrDefault(0) + objDTO.ConsignedQuantity.GetValueOrDefault(0);

                    IEnumerable<ItemLocationDetailsDTO> lstItmLocatinDetail = new ItemLocationDetailsDAL(base.DataBaseName).GetItemLocationDetailsByBinIdAndItemGuidPlain(objDTO.BinID, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), "ID Desc");
                    double custQty = lstItmLocatinDetail.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0));
                    double consQty = lstItmLocatinDetail.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0));

                    obj.CustomerOwnedQuantity = custQty;
                    obj.ConsignedQuantity = consQty;
                    obj.Quantity = consQty + custQty;

                    obj.LotNumber = objDTO.LotNumber;
                    obj.GUID = Guid.NewGuid();
                    obj.ItemGUID = objDTO.ItemGUID;
                    obj.Created = objDTO.Created;

                    if (objDTO.Created == null || objDTO.Created.GetValueOrDefault(DateTime.MinValue) <= DateTime.MinValue)
                        obj.Created = DateTimeUtility.DateTimeNow;
                    obj.LastUpdated = objDTO.LastUpdated;
                    if (objDTO.LastUpdated == null || objDTO.LastUpdated.GetValueOrDefault(DateTime.MinValue) <= DateTime.MinValue)
                        obj.LastUpdated = DateTimeUtility.DateTimeNow;

                    obj.CreatedBy = objDTO.CreatedBy;
                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    obj.Room = objDTO.Room;
                    obj.CompanyID = objDTO.CompanyID;
                    obj.AddedFrom = "Web";
                    obj.EditedFrom = "Web";
                    obj.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                    context.ItemLocationQTies.Add(obj);
                    context.SaveChanges();
                    objDTO.ID = obj.ID;

                    //UpdateOnHandQuantity(objDTO.ItemGUID.Value, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));
                }
                if (lstitem.Count() > 0)
                    UpdateOnHandQuantity(lstitem[0].ItemGUID.Value, lstitem[0].Room.GetValueOrDefault(0), lstitem[0].CompanyID.GetValueOrDefault(0), WhatWhereAction, SessionUserId,EnterpriseId);

                // done below changes of UpdateOnHandQTY to avoid un-nessasory updation in Item table.

                return true;
            }
        }

        public bool DeleteItemLocationsByItemAndBinId(Guid ItemGUID, Int64 BinID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@ItemGuid", ItemGUID),
                                                    new SqlParameter("@RoomID", RoomID),
                                                    new SqlParameter("@CompanyID", CompanyID),
                                                    new SqlParameter("@BinId", BinID)
                                                };
                context.Database.ExecuteSqlCommand("exec [DeleteItemLocationsByItemAndBinId] @ItemGuid,@RoomID,@CompanyID,@BinId", params1);
                return true;
            }            
        }

        /// <summary>
        /// function to update the On Hand Quantity
        /// </summary>
        /// <param name="ItemID"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        public void UpdateOnHandQuantity(Guid ItemGUID, Int64 RoomID, Int64 CompanyID, long SessionUserId,long EnterpriseId)
        {
            ItemMasterDAL objItem = new ItemMasterDAL(base.DataBaseName);
            Double OnHandQuantity = 0;
            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{
            ItemMasterDTO ItemDTO = objItem.GetItemWithoutJoins(null, ItemGUID); //context.ItemMasters.FirstOrDefault(x => x.GUID == ItemGUID); //objItem.GetRecord(ItemGUID.ToString(), RoomID, CompanyID);
            if (ItemDTO != null)
            {
                List<ItemLocationQTYDTO> objReturn = GetItemLocationQTYByItemGuiPlain(RoomID, CompanyID, ItemGUID).ToList();
                objReturn.Select(c => { OnHandQuantity += c.Quantity; return c; }).ToList();
                ItemDTO.OnHandQuantity = OnHandQuantity;
                ItemDTO.WhatWhereAction = "Item Location";
                objItem.Edit(ItemDTO, SessionUserId,EnterpriseId);
                //context.SaveChanges();
            }
            //}
        }

        /// <summary>
        /// function to update the On Hand Quantity
        /// </summary>
        /// <param name="ItemID"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        public void UpdateOnHandQuantity(Guid ItemGUID, Int64 RoomID, Int64 CompanyID, string WhatWhereAction, long SessionUserId,long EnterpriseId)
        {
            ItemMasterDAL objItem = new ItemMasterDAL(base.DataBaseName);
            Double OnHandQuantity = 0;
            ItemMasterDTO ItemDTO = objItem.GetItemWithoutJoins(null, ItemGUID);
            if (ItemDTO != null)
            {
                List<ItemLocationQTYDTO> objReturn = GetItemLocationQTYByItemGuiPlain(RoomID, CompanyID, ItemGUID).ToList();
                objReturn.Select(c => { OnHandQuantity += c.Quantity; return c; }).ToList();
                ItemDTO.OnHandQuantity = OnHandQuantity;
                ItemDTO.WhatWhereAction = "Item Location";
                if (!string.IsNullOrEmpty(WhatWhereAction))
                {
                    ItemDTO.WhatWhereAction = WhatWhereAction;
                }
                objItem.Edit(ItemDTO, SessionUserId,EnterpriseId);
            }

        }

        public List<ItemLocationQTYDTO> GetBinsByItemDB(Guid ItemGUID, Int64 RoomID, Int64 CompanyID, string sortColumnName, bool IsLocationRequireQty = false)
        {
            List<ItemLocationQTYDTO> lstItemLocations = new List<ItemLocationQTYDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //List<GetItemLocationSummary_Result> lstfromDB = context.GetItemLocationSummary(RoomID, CompanyID, ItemGUID).ToList();
                //lstfromDB
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ItemGUID", ItemGUID) };

                lstItemLocations = (from iqty in context.Database.SqlQuery<ItemLocationQTYDTO>("exec [GetItemLocationSummary] @RoomID,@CompanyID,@ItemGUID", params1)
                                    select iqty).ToList();

            }
            lstItemLocations.Sort(sortColumnName);
            return lstItemLocations;
        }


        public List<ItemLocationQTYDTO> GetBinsByItemDBWithTransfer(Guid ItemGUID, Int64 RoomID, Int64 CompanyID, string sortColumnName, bool IsLocationRequireQty = false)
        {
            List<ItemLocationQTYDTO> lstItemLocations = new List<ItemLocationQTYDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //  List<GetItemLocationSummaryTransferWise_Result> lstfromDB = context.GetItemLocationSummaryTransferWise(RoomID, CompanyID, ItemGUID).ToList();
                //lstfromDB
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ItemGUID", ItemGUID) };

                lstItemLocations = (from iqty in context.Database.SqlQuery<ItemLocationQTYDTO>("exec [GetItemLocationSummaryTransferWise] @RoomID,@CompanyID,@ItemGUID", params1)
                                    select iqty).ToList();
                //lstItemLocations = (from iqty in lstfromDB
                //                    select new ItemLocationQTYDTO
                //                    {
                //                        ID = iqty.ID,
                //                        BinID = iqty.ID,
                //                        CustomerOwnedQuantity = iqty.CustomerOwnedQuantity,
                //                        ConsignedQuantity = iqty.ConsignedQuantity,
                //                        Quantity = iqty.OnHandQuantity ?? 0,
                //                        ItemGUID = iqty.ItemGUID,
                //                        Created = iqty.Created,
                //                        LastUpdated = iqty.LastUpdated,
                //                        CreatedByName = iqty.CreatedByName,
                //                        UpdatedByName = iqty.UpdatedByName,
                //                        Room = iqty.Room,
                //                        CompanyID = iqty.CompanyID,
                //                        BinNumber = iqty.BinNumber,
                //                        ItemNumber = iqty.itemNumber,
                //                        CriticalQuantity = iqty.CriticalQuantity,
                //                        MaximumQuantity = iqty.MaximumQuantity,
                //                        MinimumQuantity = iqty.MinimumQuantity,
                //                        Cost = Convert.ToDouble(iqty.Cost),
                //                        Markup = Convert.ToDouble(iqty.Markup),
                //                        SellPrice = Convert.ToDouble(iqty.SellPrice),
                //                        Averagecost = (double)iqty.AverageCost,
                //                        ExtendedCost = (double)iqty.ExtendedCost,
                //                        CostUOMID = iqty.CostUOMID,
                //                        eVMISensorID = iqty.eVMISensorID,
                //                        eVMISensorPort = iqty.eVMISensorPort,
                //                        CostUOMName = iqty.CostUOM,
                //                        IsItemLevelMinMaxQtyRequired = iqty.IsItemLevelMinMaxQtyRequired,
                //                        SuggestedOrderQuantity = iqty.CartQuantity,
                //                        AddedFrom = iqty.AddedFrom,
                //                        EditedFrom = iqty.EditedFrom,
                //                        ReceivedOn = iqty.ReceivedOn,
                //                        ReceivedOnWeb = iqty.ReceivedOnWeb,
                //                        SuggestedTransferQuantity = iqty.CartTransferQuantity,
                //                    }).ToList();

            }
            lstItemLocations.Sort(sortColumnName);
            return lstItemLocations;
        }

        public List<ItemLocationQTYDTO> GetListOfItemLocQty(Guid ItemGUID, Int64 RoomID, Int64 CompanyID, Int64 UserID)
        {
            List<ItemLocationDetailsDTO> ListLocDtl = new List<ItemLocationDetailsDTO>();
            List<ItemLocationQTYDTO> resultList = new List<ItemLocationQTYDTO>();
            ItemLocationQTYDTO objItemLocQty = new ItemLocationQTYDTO();
            ListLocDtl = new ItemLocationDetailsDAL(base.DataBaseName).GetItemsLocations(ItemGUID, RoomID, CompanyID);
            if (ListLocDtl != null && ListLocDtl.Count > 0)
            {
                foreach (var objItemLoc in ListLocDtl)
                {
                    if (objItemLoc != null)
                    {
                        objItemLocQty = new ItemLocationQTYDTO();
                        objItemLocQty.Room = RoomID;
                        objItemLocQty.CompanyID = CompanyID;
                        objItemLocQty.BinID = objItemLoc.BinID.GetValueOrDefault(0);
                        objItemLocQty.ItemGUID = ItemGUID;
                        objItemLocQty.CustomerOwnedQuantity = objItemLoc.CustomerOwnedQuantity.GetValueOrDefault(0);
                        objItemLocQty.ConsignedQuantity = objItemLoc.ConsignedQuantity.GetValueOrDefault(0);
                        objItemLocQty.Quantity = objItemLocQty.CustomerOwnedQuantity.GetValueOrDefault(0) + objItemLocQty.ConsignedQuantity.GetValueOrDefault(0);
                        objItemLocQty.Created = DateTimeUtility.DateTimeNow;
                        objItemLocQty.CreatedBy = UserID;
                        objItemLocQty.LastUpdated = DateTimeUtility.DateTimeNow;
                        objItemLocQty.LastUpdatedBy = UserID;
                        resultList.Add(objItemLocQty);
                    }
                }
            }
            return resultList;
        }

        public List<ItemLocationQTYDTO> GetItemLocationQTYByItemGuidNormal(long RoomId, long CompanyId, Guid ItemGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                   new SqlParameter("@ItemGuid", ItemGuid),
                                                   new SqlParameter("@RoomID", RoomId),
                                                   new SqlParameter("@CompanyId", CompanyId)
                                                   
                                                };
                return context.Database.SqlQuery<ItemLocationQTYDTO>("exec [GetItemLocationQTYByItemGuidNormal] @ItemGuid,@RoomID,@CompanyId ", params1).ToList();
            }
        }

        public List<ItemLocationQTYDTO> GetItemLocationQTYByItemGuiPlain(long RoomId, long CompanyId, Guid ItemGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                   new SqlParameter("@ItemGuid", ItemGuid),
                                                   new SqlParameter("@RoomID", RoomId),
                                                   new SqlParameter("@CompanyId", CompanyId)

                                                };
                return context.Database.SqlQuery<ItemLocationQTYDTO>("exec [GetItemLocationQTYByItemGuidPlain] @ItemGuid,@RoomID,@CompanyId ", params1).ToList();
            }
        }
    }
}


