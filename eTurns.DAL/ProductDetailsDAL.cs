using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using eTurns.DTO;

namespace eTurns.DAL
{
    public class ProductDetailsDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        public ProductDetailsDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }
        #endregion

        public ItemMasterDTO InsertABItemToRoom(string ABProductjson,Int64 CreatedBy,Int64 LastUpdatedBy, Int64 CompanyID,Int64 RoomID)
        {
            //Int64 ItemID = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] 
                            {
                                new SqlParameter("@ABProductjson", ABProductjson),
                                new SqlParameter("@CreatedBy", CreatedBy),
                                new SqlParameter("@LastUpdatedBy", LastUpdatedBy),
                                new SqlParameter("@CompanyID", CompanyID),
                                new SqlParameter("@RoomID", RoomID),
                            };

                return context.Database.SqlQuery<ItemMasterDTO>("exec [InsertABItemToRoom] @ABProductjson,@CreatedBy,@LastUpdatedBy,@CompanyID,@RoomID", params1).FirstOrDefault();
                //return ItemID;
            }
        }

        public Int64 CheckABItemExistInRoomByAsin(string ASIN, Int64 CompanyID, Int64 RoomID)
        {
            Int64 ABItemMappingID = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[]
                            {
                                new SqlParameter("@ASIN", ASIN),
                                new SqlParameter("@CompanyID", CompanyID),
                                new SqlParameter("@RoomID", RoomID),
                            };

                ABItemMappingID = context.Database.SqlQuery<Int64>("exec [CheckABItemExistInRoomByAsin] @ASIN,@CompanyID,@RoomID", params1).FirstOrDefault();
                return ABItemMappingID;
            }
        }

        public Int64 CheckItemAddedFromAB(string ASIN,Guid ItemGuid, Int64 CompanyID, Int64 RoomID)
        {
            Int64 ABItemMappingID = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[]
                            {
                                new SqlParameter("@ASIN", ASIN),
                                new SqlParameter("@ItemGuid", ItemGuid),
                                new SqlParameter("@CompanyID", CompanyID),
                                new SqlParameter("@RoomID", RoomID),
                            };

                ABItemMappingID = context.Database.SqlQuery<Int64>("exec [CheckItemAddedFromAB] @ASIN,@ItemGuid,@CompanyID,@RoomID", params1).FirstOrDefault();
                return ABItemMappingID;
            }
        }

        public List<string> GetABItemsAsinByRoomId(long RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[]
                            {
                                new SqlParameter("@RoomID", RoomID),
                            };

                return context.Database.SqlQuery<string>("exec [GetABItemsAsinByRoomId] @RoomID", params1).ToList();
                
            }
        }
        public List<string> GetAllABItemsAsinByRoomId(long RoomID,long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[]
                            {
                                new SqlParameter("@RoomID", RoomID),
                                new SqlParameter("@CompanyID", CompanyID)
                            };

                return context.Database.SqlQuery<string>("exec [GetAllABItemsAsinByRoomId] @RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<ItemMasterDTO> SyncABItemToRoom(string ABProductjson, Int64 LastUpdatedBy, Int64 CompanyID, Int64 RoomID)
        {
            //int ReturnValue = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[]
                            {
                                new SqlParameter("@ABProductjson", ABProductjson),
                                new SqlParameter("@LastUpdatedBy", LastUpdatedBy),
                                new SqlParameter("@CompanyID", CompanyID),
                                new SqlParameter("@RoomID", RoomID),
                            };
                //string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
                //SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
                //System.Data.DataSet lst  = SqlHelper.ExecuteDataset(EturnsConnection, "SyncABItemToRoom", ABProductjson, LastUpdatedBy, CompanyID, RoomID);
                return context.Database.SqlQuery<ItemMasterDTO>("exec [SyncABItemToRoom] @ABProductjson,@LastUpdatedBy,@CompanyID,@RoomID", params1).ToList();
                //return ReturnValue;
            }
        }
        public ItemMasterDTO GetItemByABItemMappingId(long ABItemMappingId, long CompanyID, long RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[]
                            {
                                new SqlParameter("@ABItemMappingId", ABItemMappingId),
                                new SqlParameter("@CompanyID", CompanyID),
                                new SqlParameter("@RoomID", RoomID),
                            };

                return context.Database.SqlQuery<ItemMasterDTO>("exec [GetItemByABItemMappingId] @ABItemMappingId,@CompanyID,@RoomID", params1).FirstOrDefault();
            }
        }

    }
}
