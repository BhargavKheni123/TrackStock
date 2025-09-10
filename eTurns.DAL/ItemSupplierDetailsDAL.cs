using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;


namespace eTurns.DAL
{
    public partial class ItemSupplierDetailsDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        public ItemSupplierDetailsDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ItemSupplierDetailsDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion
        #region [Class Methods]
        public List<ItemSupplierDetailsDTO> GetSuppliersByRoomNormal(Int64 RoomID, Int64 CompanyID, string OrderBy = null)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                      new SqlParameter("@RoomID", RoomID)
                    , new SqlParameter("@CompanyID", CompanyID)
                    , new SqlParameter("@OrderBy", OrderBy?? (object)DBNull.Value)
                };
                return context.Database.SqlQuery<ItemSupplierDetailsDTO>("exec GetSuppliersByRoomNormal @RoomID,@CompanyID,@OrderBy", params1).ToList();
            }
        }
        public List<ItemSupplierDetailsDTO> GetSuppliersByItemGuidNormal(Int64 RoomID, Int64 CompanyID, Guid ItemGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                      new SqlParameter("@RoomID", RoomID)
                    , new SqlParameter("@CompanyID", CompanyID)
                    , new SqlParameter("@ItemGuid", ItemGuid)
                };
                return context.Database.SqlQuery<ItemSupplierDetailsDTO>("exec GetSuppliersByItemGuidNormal @RoomID,@CompanyID,@ItemGuid", params1).ToList();
            }
        }
        public ItemSupplierDetailsDTO GetSupplierByItemGuidBySupplierID(Int64 RoomID, Int64 CompanyID, Guid ItemGuid, Int64? SupplierID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                      new SqlParameter("@RoomID", RoomID)
                    , new SqlParameter("@CompanyID", CompanyID)
                    , new SqlParameter("@ItemGuid", ItemGuid)
                    , new SqlParameter("@SupplierID", SupplierID)
                };
                return context.Database.SqlQuery<ItemSupplierDetailsDTO>("exec GetSupplierByItemGuidBySupplierID @RoomID,@CompanyID,@ItemGuid,@SupplierID", params1).FirstOrDefault();
            }
        }
        public ItemSupplierDetailsDTO GetSupplierBySupplierIDByMasterSupplierID(Int64 RoomID, Int64 CompanyID, Guid ItemGuid, Int64? SupplierID, Int64? MasterSupplierID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                      new SqlParameter("@RoomID", RoomID)
                    , new SqlParameter("@CompanyID", CompanyID)
                    , new SqlParameter("@ItemGuid", ItemGuid)
                    , new SqlParameter("@SupplierID", SupplierID)
                    , new SqlParameter("@MasterSupplierID", MasterSupplierID)

                };
                return context.Database.SqlQuery<ItemSupplierDetailsDTO>("exec GetSupplierBySupplierIDByMasterSupplierID @RoomID,@CompanyID,@ItemGuid,@SupplierID,@MasterSupplierID", params1).FirstOrDefault();
            }
        }
        public List<ItemSupplier> GetItemSupplierExportByRoomNormal(long companyId, long RoomId, string SortNameString)
        {
            List<ItemSupplier> lstSupplierDetailsmain = new List<ItemSupplier>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                      new SqlParameter("@RoomID", RoomId)
                    , new SqlParameter("@CompanyID", companyId)
                    , new SqlParameter("@SortName",  SortNameString?? (object)DBNull.Value)
                };
                return context.Database.SqlQuery<ItemSupplier>("exec GetItemSupplierExportByRoomNormal @RoomID,@CompanyID,@SortName", params1).ToList();
            }
        }
        public Int64 Insert(ItemSupplierDetailsDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemSupplierDetail obj = new ItemSupplierDetail();
                obj.ID = 0;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.SupplierID = objDTO.SupplierID;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.IsDefault = objDTO.IsDefault;

                if (objDTO.BlanketPOID == 0)
                    objDTO.BlanketPOID = null;
                else
                    objDTO.BlanketPOID = objDTO.BlanketPOID;

                obj.BlanketPOID = objDTO.BlanketPOID;
                obj.SupplierName = objDTO.SupplierName;
                obj.SupplierNumber = objDTO.SupplierNumber;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                objDTO.GUID = Guid.NewGuid();
                obj.GUID = objDTO.GUID;
                obj.AddedFrom = "Web";
                obj.EditedFrom = "Web";
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                context.ItemSupplierDetails.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;

                return obj.ID;
            }

        }
        public bool Edit(ItemSupplierDetailsDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemSupplierDetail obj = new ItemSupplierDetail();
                obj.ID = objDTO.ID;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.SupplierID = objDTO.SupplierID;
                obj.Created = objDTO.Created;
                obj.Updated = objDTO.Updated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.IsDefault = objDTO.IsDefault;

                if (objDTO.BlanketPOID == 0)
                    objDTO.BlanketPOID = null;
                else
                    objDTO.BlanketPOID = objDTO.BlanketPOID;

                obj.BlanketPOID = objDTO.BlanketPOID;
                obj.SupplierName = objDTO.SupplierName;
                obj.SupplierNumber = objDTO.SupplierNumber;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.GUID = objDTO.GUID;
                obj.AddedFrom = objDTO.AddedFrom;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.EditedFrom = objDTO.EditedFrom;



                context.ItemSupplierDetails.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return true;
            }
        }

        public long InsertItemSupplierDetailFromSVC(ImportDTO.ItemSupplierDetailsImportDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemSupplierDetail obj = new ItemSupplierDetail();
                obj.ID = 0;
                obj.ItemGUID = objDTO.ItemGuid;
                obj.SupplierID = objDTO.SupplierID.GetValueOrDefault(0);
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.UserID;
                obj.LastUpdatedBy = objDTO.UserID;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.IsDefault = objDTO.IsDefault;

                if (objDTO.BlanketPOID == 0)
                    objDTO.BlanketPOID = null;
                else
                    objDTO.BlanketPOID = objDTO.BlanketPOID;

                obj.BlanketPOID = objDTO.BlanketPOID;
                obj.SupplierName = objDTO.SupplierName;
                obj.SupplierNumber = objDTO.SupplierNumber;
                obj.Room = objDTO.RoomID;
                obj.CompanyID = objDTO.CompanyID;
                //objDTO.GUID = Guid.NewGuid();
                obj.GUID = Guid.NewGuid();
                obj.AddedFrom = "SVC";
                obj.EditedFrom = "SVC";
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                context.ItemSupplierDetails.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;

                return obj.ID;
            }

        }

        public bool EditItemSupplierDetailFromSVC(ImportDTO.ItemSupplierDetailsImportDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemSupplierDetail obj = context.ItemSupplierDetails.Where(x => x.ID == objDTO.ID).FirstOrDefault();
                
                if (obj != null)
                {
                    obj.ItemGUID = objDTO.ItemGuid;
                    obj.SupplierID = objDTO.SupplierID.GetValueOrDefault(0);
                    obj.Updated = DateTimeUtility.DateTimeNow;
                    obj.LastUpdatedBy = objDTO.UserID;
                    obj.IsDeleted = false;
                    obj.IsArchived = false;
                    obj.IsDefault = objDTO.IsDefault;

                    if (objDTO.BlanketPOID == 0)
                        objDTO.BlanketPOID = null;
                    else
                        objDTO.BlanketPOID = objDTO.BlanketPOID;

                    obj.BlanketPOID = objDTO.BlanketPOID;
                    obj.SupplierName = objDTO.SupplierName;
                    obj.SupplierNumber = objDTO.SupplierNumber;
                    obj.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                    obj.EditedFrom = "SVC";

                    context.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public bool DeleteItemSupplierRecordsExcept(string IDs, Guid ItemGUID, Int64 userid)
        {
            if (!string.IsNullOrWhiteSpace(IDs))
            {
                using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    string vIDs = (IDs ?? string.Empty).TrimEnd(',');
                    if (!string.IsNullOrWhiteSpace(vIDs))
                    {
                        var params1 = new SqlParameter[] {
                          new SqlParameter("@UserID", userid)
                         ,new SqlParameter("@IDs", vIDs)
                         ,new SqlParameter("@ItemGuid", ItemGUID)
                         ,new SqlParameter("@ReceivedOn",DateTimeUtility.DateTimeNow)
                         };

                        string strQuery = @"EXEC DeleteItemSupplierRecordsExcept @UserID,@IDs,@ItemGuid,@ReceivedOn";
                        context.Database.ExecuteSqlCommand(strQuery, params1);

                        return true;
                    }
                    return false;
                }
            }
            return false;
        }
        public List<ItemSupplierDetailsDTO> GetItemSupplersByItemGuidNormal(Guid itemGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", itemGUID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemSupplierDetailsDTO>("exec [GetItemSupplersByItemGuidNormal] @ItemGUID", params1).ToList();
            }
        }
        public void UnDeleteItemSupplierByItemGUID(Guid ItemGUID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGuid", ItemGUID) };
                string strQuery = @"EXEC UnDeleteItemSupplierByItemGUID @ItemGuid";
                context.Database.ExecuteSqlCommand(strQuery, params1);
            }
        }

        public List<SupplierPartDTO> GetItemSupplierPartDetails(Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                      new SqlParameter("@RoomID", RoomID)
                    , new SqlParameter("@CompanyID", CompanyID)

                };
                return context.Database.SqlQuery<SupplierPartDTO>("exec GetItemSupplierPartDetails @RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<SupplierPartDTO> GetItemSupplierPartNoByRoomIds(string RoomIds, string SupplierPartNo)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                      new SqlParameter("@RoomIds", RoomIds)
                    , new SqlParameter("@SupplierPartNo", SupplierPartNo)
                };
                return context.Database.SqlQuery<SupplierPartDTO>("exec GetItemSupplierPartNoByRoomIds @RoomIds,@SupplierPartNo", params1).ToList();
            }
        }

        public void UpdateBlanketPOID(Guid ItemGUID,long BlanketPOID,long SupplierID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { 
                    new SqlParameter("@ItemGuid", ItemGUID),
                    new SqlParameter("@BlanketPOID", BlanketPOID),
                    new SqlParameter("@SupplierID", SupplierID)
                };
                string strQuery = @"EXEC UpdateBlanketPOID @ItemGuid,@BlanketPOID,@SupplierID";
                context.Database.ExecuteSqlCommand(strQuery, params1);
            }
        }

        #endregion
    }
}



