using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;


namespace eTurns.DAL
{
    public partial class ItemManufacturerDetailsDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public ItemManufacturerDetailsDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ItemManufacturerDetailsDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion
        #region [Class Methods]
        public List<ItemManufacturerDetailsDTO> GetItemManufacturerByRoomNormal(Int64 RoomID, Int64 CompanyID)
        {
            var params1 = new SqlParameter[] {  new SqlParameter("@RoomID", RoomID)
                                               ,new SqlParameter("@CompanyID", CompanyID)
                                              };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemManufacturerDetailsDTO>("exec [GetItemManufacturerByRoomNormal] @RoomID,@CompanyID", params1).ToList();
            }
        }
        public ItemManufacturerDetailsDTO GetManufacturerByItemGuidByManufacturerID(Int64 RoomID, Int64 CompanyID, Guid ItemGuid, bool IsDeleted, long? ManufacturerID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID)
                    , new SqlParameter("@CompanyID", CompanyID)
                    , new SqlParameter("@ItemGUID", ItemGuid)
                    , new SqlParameter("@IsDeleted", IsDeleted)
                    ,new SqlParameter("@ManufacturerID", ManufacturerID ?? (object)DBNull.Value)
            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemManufacturerDetailsDTO>("exec [GetManufacturerByItemGuidByManufacturerID] @RoomID,@CompanyID,@ItemGUID,@IsDeleted,@ManufacturerID", params1).FirstOrDefault();
            }
        }
        public List<ItemManufacturerDetailsDTO> GetManufacturerByItemGuidNormal(Int64 RoomID, Int64 CompanyID, Guid ItemGuid, bool IsDeleted)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID.ToString())
                    , new SqlParameter("@CompanyID", CompanyID)
                    , new SqlParameter("@ItemGUID", ItemGuid)
                    , new SqlParameter("@IsDeleted", IsDeleted)
            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemManufacturerDetailsDTO>("exec [GetManufacturerByItemGuidNormal] @RoomID,@CompanyID,@ItemGUID,@IsDeleted", params1).ToList();
            }
        }
        public List<ItemManufacturer> GetItemManufacturerExport(long companyId, long RoomId)
        {
            List<ItemManufacturer> lstManufacturerDetailsmain = new List<ItemManufacturer>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstManufacturerDetailsmain = (from p in context.ItemManufacturerDetails
                                              join im in context.ItemMasters on p.ItemGUID equals im.GUID
                                              where p.CompanyID == companyId && p.Room == RoomId && (p.IsArchived ?? false) == false && (p.IsDeleted ?? false) == false
                                              && im.IsDeleted == false
                                              select new ItemManufacturer
                                              {
                                                  ID = p.ID,
                                                  ItemNumber = im.ItemNumber,
                                                  ManufacturerNumber = p.ManufacturerNumber,
                                                  ManufacturerName = p.ManufacturerName,
                                                  ItemGUID = p.ItemGUID,
                                                  IsDefault = p.IsDefault,

                                              }).ToList();
            }
            return lstManufacturerDetailsmain;
        }

        public Int64 Insert(ItemManufacturerDetailsDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemManufacturerDetail obj = new ItemManufacturerDetail();
                obj.ID = 0;

                obj.ManufacturerID = objDTO.ManufacturerID;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.IsDefault = objDTO.IsDefault;
                obj.ManufacturerName = objDTO.ManufacturerName;
                obj.ManufacturerNumber = objDTO.ManufacturerNumber;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.ItemGUID = objDTO.ItemGUID;
                objDTO.GUID = Guid.NewGuid();
                obj.GUID = objDTO.GUID;
                obj.AddedFrom = objDTO.AddedFrom;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;

                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.EditedFrom = objDTO.EditedFrom;

                context.ItemManufacturerDetails.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;

                return obj.ID;
            }

        }

        public bool Edit(ItemManufacturerDetailsDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemManufacturerDetail obj = new ItemManufacturerDetail();
                obj.ID = objDTO.ID;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.ManufacturerID = objDTO.ManufacturerID;
                obj.Created = objDTO.Created;
                obj.Updated = objDTO.Updated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = objDTO.IsDeleted == null ? false : objDTO.IsDeleted;
                obj.IsArchived = false;
                obj.IsDefault = objDTO.IsDefault;
                obj.ManufacturerName = objDTO.ManufacturerName;
                obj.ManufacturerNumber = objDTO.ManufacturerNumber;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.GUID = objDTO.GUID;
                obj.AddedFrom = objDTO.AddedFrom;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.EditedFrom = objDTO.EditedFrom;
                context.ItemManufacturerDetails.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                return true;
            }
        }

        public long InsertItemManufacturerDetailSVC(ImportDTO.ItemManufacturerDetailsImportDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemManufacturerDetail obj = new ItemManufacturerDetail();
                obj.ID = 0;
                obj.ManufacturerID = objDTO.ManufacturerID.GetValueOrDefault(0);
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.UserID;
                obj.LastUpdatedBy = objDTO.UserID;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.IsDefault = objDTO.IsDefault;
                obj.ManufacturerName = objDTO.ManufacturerName;
                obj.ManufacturerNumber = objDTO.ManufacturerNumber;
                obj.Room = objDTO.RoomID;
                obj.CompanyID = objDTO.CompanyID;
                obj.ItemGUID = objDTO.ItemGuid;
                //objDTO.GUID = Guid.NewGuid();
                obj.GUID = Guid.NewGuid();
                obj.AddedFrom = "SVC";
                obj.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.EditedFrom = "SVC";

                context.ItemManufacturerDetails.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;

                return obj.ID;
            }
        }

        public bool EditItemManufacturerDetailSVC(ImportDTO.ItemManufacturerDetailsImportDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemManufacturerDetail obj = context.ItemManufacturerDetails.Where(e=> e.ID == objDTO.ID).FirstOrDefault();
                //obj.ID = objDTO.ID;
                obj.ItemGUID = objDTO.ItemGuid;
                obj.ManufacturerID = objDTO.ManufacturerID.GetValueOrDefault(0);
                //obj.Created = DateTimeUtility.DateTimeNow;
                obj.Updated = DateTimeUtility.DateTimeNow;
                //obj.CreatedBy = objDTO.UserID;
                obj.LastUpdatedBy = objDTO.UserID;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.IsDefault = objDTO.IsDefault;
                obj.ManufacturerName = objDTO.ManufacturerName;
                obj.ManufacturerNumber = objDTO.ManufacturerNumber;
                obj.Room = objDTO.RoomID;
                obj.CompanyID = objDTO.CompanyID;
                //obj.GUID = objDTO.GUID;
                //obj.AddedFrom = objDTO.AddedFrom;
                obj.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.EditedFrom = "SVC";
                
                context.SaveChanges();

                return true;
            }
        }

        public bool DeleteItemManufacturerRecordsExcept(string IDs, Guid ItemGUID, Int64 userid)
        {
            if (!string.IsNullOrWhiteSpace(IDs) || ItemGUID != null)
            {
                using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    string vIDs = (IDs ?? string.Empty).TrimEnd(',');
                    if (!string.IsNullOrWhiteSpace(vIDs) || ItemGUID != null)
                    {
                        var params1 = new SqlParameter[] {
                          new SqlParameter("@UserID", userid)
                         ,new SqlParameter("@ManIDs", vIDs)
                         ,new SqlParameter("@ItemGuid", ItemGUID)
                         ,new SqlParameter("@ReceivedOn",DateTimeUtility.DateTimeNow)
                         };

                        string strQuery = @"EXEC DeleteItemManufacturerRecordsExcept @UserID,@ManIDs,@ItemGuid,@ReceivedOn";
                        context.Database.ExecuteSqlCommand(strQuery, params1);

                        return true;
                    }
                    return false;
                }
            }
            return false;
        }
        public bool CheckManufacturerNoDuplicate(string ManufacturerNumber, long RoomID, long CompanyID, Guid? Itemguid, bool IsBOM = false)
        {
            bool result = false;
            try
            {
                using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var qry = (from em in context.ItemManufacturerDetails
                               join it in context.ItemMasters on em.ItemGUID equals it.GUID
                               where em.ManufacturerNumber == ManufacturerNumber && em.ItemGUID == Itemguid
                               && em.IsArchived == false && em.IsDeleted == false && em.Room == RoomID && em.CompanyID == CompanyID
                               && it.IsDeleted == false && it.Room == RoomID
                               && it.IsBOMItem == IsBOM
                               select em);
                    if (qry.Any())
                    {
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public List<ItemManufacturerDetailsDTO> GetItemManufacturerByGUIDNormal(Guid itemGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", itemGUID.ToString()) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemManufacturerDetailsDTO>("exec [GetItemManufacturerByGUIDNormal] @ItemGUID", params1).ToList();
            }
        }
        public void UnDeleteItemManufacturerDetailsByItemGUID(Guid ItemGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                eTurns.DAL.ItemMaster itemMaster = context.ItemMasters.Where(t => t.GUID == ItemGUID).FirstOrDefault();
                if (itemMaster != null)
                {
                    ItemManufacturerDetail itemsManufacturerDetails = context.ItemManufacturerDetails.Where(t => t.ManufacturerID == itemMaster.ManufacturerID && t.ItemGUID == ItemGUID && (t.IsDefault ?? false) == true).FirstOrDefault();
                    if (itemsManufacturerDetails != null)
                    {
                        itemsManufacturerDetails.IsDeleted = false;
                        context.SaveChanges();
                    }
                }
            }
        }

        public bool DeleteItemManufacturerDetailsByItemGUID(string ItemGUID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID) };

                context.Database.ExecuteSqlCommand("EXEC DeleteItemManuDtlByItemGUID @ItemGUID,@RoomID,@CompanyID", params1);
            }
            return true;
        }
        #endregion
    }
}


