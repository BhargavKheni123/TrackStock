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
    public partial class BinMasterDAL : eTurnsBaseDAL
    {


        public IEnumerable<BinMasterDTO> GetBinMasterByLocationName(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string LocationName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@BinNumber", LocationName) };
                IEnumerable<BinMasterDTO> obj = (from u in context.Database.SqlQuery<BinMasterDTO>("exec [GetBinMasterByLocationName] @RoomID,@CompanyID,@IsDeleted,@IsArchived,@BinNumber", params1) //A.IsDeleted!=1 AND A.IsArchived != 1 AND 
                                                 select new BinMasterDTO
                                                 {
                                                     ID = u.ID,
                                                     BinNumber = u.BinNumber,
                                                     Created = u.Created,
                                                     LastUpdated = u.LastUpdated,
                                                     IsStagingLocation = u.IsStagingLocation,
                                                     IsStagingHeader = u.IsStagingHeader,
                                                     MaterialStagingGUID = u.MaterialStagingGUID,
                                                     CreatedByName = u.CreatedByName,
                                                     UpdatedByName = u.UpdatedByName,
                                                     RoomName = u.RoomName,
                                                     CreatedBy = u.CreatedBy,
                                                     LastUpdatedBy = u.LastUpdatedBy,
                                                     Room = u.Room,
                                                     CompanyID = u.CompanyID,
                                                     IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                     IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                     GUID = u.GUID,
                                                     UDF1 = u.UDF1,
                                                     UDF2 = u.UDF2,
                                                     UDF3 = u.UDF3,
                                                     UDF4 = u.UDF4,
                                                     UDF5 = u.UDF5,
                                                     ItemGUID = u.ItemGUID,
                                                     MinimumQuantity = u.MinimumQuantity,
                                                     MaximumQuantity = u.MaximumQuantity,
                                                     CriticalQuantity = u.CriticalQuantity,
                                                     ConsignedQuantity = u.ConsignedQuantity,
                                                     SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                                                     IsDefault = u.IsDefault,
                                                     CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                     ItemNumber = u.ItemNumber,
                                                     eVMISensorID = u.eVMISensorID,
                                                     eVMISensorPort = u.eVMISensorPort,
                                                     AddedFrom = u.AddedFrom,
                                                     EditedFrom = u.EditedFrom,
                                                     ReceivedOn = u.ReceivedOn,
                                                     ReceivedOnWeb = u.ReceivedOnWeb,
                                                     IsEnforceDefaultPullQuantity = u.IsEnforceDefaultPullQuantity,
                                                     IsEnforceDefaultReorderQuantity = u.IsEnforceDefaultReorderQuantity,
                                                     DefaultPullQuantity = u.DefaultPullQuantity,
                                                     DefaultReorderQuantity = u.DefaultReorderQuantity
                                                 }).AsParallel().ToList();
                return obj;
            }



        }

        public IEnumerable<BinMasterDTO> GetCachedDataWithOutLabor(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };
                IEnumerable<BinMasterDTO> obj = (from u in context.Database.SqlQuery<BinMasterDTO>("exec [GetCachedDataWithOutLabor] @RoomID,@CompanyID,@IsDeleted,@IsArchived", params1) //A.IsDeleted!=1 AND A.IsArchived != 1 AND 
                                                 select new BinMasterDTO
                                                 {
                                                     ID = u.ID,
                                                     BinNumber = u.BinNumber,
                                                     Created = u.Created,
                                                     LastUpdated = u.LastUpdated,
                                                     IsStagingLocation = u.IsStagingLocation,
                                                     IsStagingHeader = u.IsStagingHeader,
                                                     MaterialStagingGUID = u.MaterialStagingGUID,
                                                     CreatedByName = u.CreatedByName,
                                                     UpdatedByName = u.UpdatedByName,
                                                     RoomName = u.RoomName,
                                                     CreatedBy = u.CreatedBy,
                                                     LastUpdatedBy = u.LastUpdatedBy,
                                                     Room = u.Room,
                                                     CompanyID = u.CompanyID,
                                                     IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                     IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                     GUID = u.GUID,
                                                     UDF1 = u.UDF1,
                                                     UDF2 = u.UDF2,
                                                     UDF3 = u.UDF3,
                                                     UDF4 = u.UDF4,
                                                     UDF5 = u.UDF5,
                                                     ItemGUID = u.ItemGUID,
                                                     MinimumQuantity = u.MinimumQuantity,
                                                     MaximumQuantity = u.MaximumQuantity,
                                                     CriticalQuantity = u.CriticalQuantity,
                                                     ConsignedQuantity = u.ConsignedQuantity,
                                                     SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                                                     IsDefault = u.IsDefault,
                                                     CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                     ItemNumber = u.ItemNumber,
                                                     eVMISensorID = u.eVMISensorID,
                                                     eVMISensorPort = u.eVMISensorPort,
                                                     AddedFrom = u.AddedFrom,
                                                     EditedFrom = u.EditedFrom,
                                                     ReceivedOn = u.ReceivedOn,
                                                     ReceivedOnWeb = u.ReceivedOnWeb,
                                                     IsEnforceDefaultPullQuantity = u.IsEnforceDefaultPullQuantity,
                                                     IsEnforceDefaultReorderQuantity = u.IsEnforceDefaultReorderQuantity,
                                                     DefaultPullQuantity = u.DefaultPullQuantity,
                                                     DefaultReorderQuantity = u.DefaultReorderQuantity
                                                 }).AsParallel().ToList();
                return obj;
            }
        }

        public BinMasterDTO GetRecordForService(Int64 id, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string ConnectionString)
        {
            using (var context = new eTurnsEntities(ConnectionString))
            {
                return (from u in context.Database.SqlQuery<BinMasterDTO>(@"select a.id,a.binnumber, a.created, a.createdby,a.room, a.lastupdatedby, a.lastupdated,a.companyid, b.username as 'createdbyname', c.username as updatedbyname, d.roomname, a.guid, a.udf1, a.udf2, a.udf3, a.udf4, a.udf5,a.IsEnforceDefaultPullQuantity,a.IsEnforceDefaultReorderQuantity,a.DefaultPullQuantity,a.DefaultReorderQuantity from binmaster a left outer  join usermaster b on a.createdby = b.id left outer join usermaster c on a.lastupdatedby = c.id left outer join room d on a.room = d.id where a.id=" + id.ToString())
                        select new BinMasterDTO
                        {
                            ID = u.ID,
                            BinNumber = u.BinNumber,
                            Created = u.Created,
                            IsStagingLocation = u.IsStagingLocation,
                            IsStagingHeader = u.IsStagingHeader,
                            MaterialStagingGUID = u.MaterialStagingGUID,
                            LastUpdated = u.LastUpdated,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            CompanyID = u.CompanyID,
                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                            GUID = u.GUID,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            ItemGUID = u.ItemGUID,
                            MinimumQuantity = u.MinimumQuantity,
                            MaximumQuantity = u.MaximumQuantity,
                            CriticalQuantity = u.CriticalQuantity,
                            ConsignedQuantity = u.ConsignedQuantity,
                            SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                            IsDefault = (u.IsDefault ?? u.IsDefault),
                            CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                            IsEnforceDefaultPullQuantity = u.IsEnforceDefaultPullQuantity,
                            IsEnforceDefaultReorderQuantity = u.IsEnforceDefaultReorderQuantity,
                            DefaultPullQuantity = u.DefaultPullQuantity,
                            DefaultReorderQuantity = u.DefaultReorderQuantity

                        }).SingleOrDefault();
            }
        }
        //public BinMasterDTO GetRecord(string LocationName, Guid ItemGUID, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        //{
        //    return GetCachedData(RoomID, CompanyId, false, false).Where(t => (t.BinNumber == null ? string.Empty : t.BinNumber.ToLower()) == LocationName.ToLower() && (t.ItemGUID ?? Guid.Empty) == ItemGUID).FirstOrDefault();
        //}
        public bool CSP_DeletePull(Guid PullGUID, long UserId, string EditedFrom, string WhatWhereAction)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    context.Database.ExecuteSqlCommand("EXEC CSP_DeletePull '" + PullGUID.ToString() + "', " + UserId.ToString() + ", '" + EditedFrom + "', '" + WhatWhereAction + "'");
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool DeleteBinAfterItemDelete(Guid ItemGuid, Int64 UserId, string DeletefFrom, out List<long> DeletedBinIds)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                DeletedBinIds = new List<long>();
                BinMaster ObjBinMasterParent = null;
                int ParentBinCount = 0;
                List<BinMaster> lstBinMaster = (from B in context.BinMasters
                                                where B.ItemGUID == ItemGuid
                                                      && B.IsDeleted == false
                                                      && (B.IsArchived == null || B.IsArchived == false)
                                                select B
                                                ).ToList();

                if (lstBinMaster != null && lstBinMaster.Count() > 0)
                {
                    foreach (BinMaster objBinMaster in lstBinMaster)
                    {
                        objBinMaster.IsDeleted = true;
                        objBinMaster.LastUpdated = DateTime.UtcNow;
                        objBinMaster.LastUpdatedBy = UserId;
                        objBinMaster.EditedFrom = DeletefFrom;
                        DeletedBinIds.Add(objBinMaster.ID);

                        ParentBinCount = (from B in context.BinMasters
                                          where B.ParentBinId == objBinMaster.ParentBinId
                                                && B.ID != objBinMaster.ID
                                                && B.IsDeleted == false
                                                && (B.IsArchived == null || B.IsArchived == false)
                                          select B.ID).Count();

                        if (ParentBinCount <= 0)
                        {
                            ObjBinMasterParent = (from B in context.BinMasters
                                                  where B.ID == objBinMaster.ParentBinId
                                                        && B.IsDeleted == false
                                                        && (B.IsArchived == null || B.IsArchived == false)
                                                  select B).FirstOrDefault();

                            ObjBinMasterParent.IsDeleted = true;
                            ObjBinMasterParent.LastUpdated = DateTime.UtcNow;
                            ObjBinMasterParent.LastUpdatedBy = UserId;
                            ObjBinMasterParent.EditedFrom = DeletefFrom;
                        }
                    }
                }

                context.SaveChanges();

                return true;
            }
        }
        public List<BinMasterDTO> CheckBinInUse(long RoomID, long CompanyID, long BinID, Guid ItemGUID)
        {
            try
            {
                string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
                List<BinMasterDTO> lstBins = new List<BinMasterDTO>();
                if (Connectionstring == "")
                {
                    return null;
                }
                SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
                DataSet dsBins = SqlHelper.ExecuteDataset(EturnsConnection, "CheckItemBinReference", RoomID, CompanyID, BinID, ItemGUID);
                if (dsBins != null && dsBins.Tables.Count > 0)
                {
                    DataTable dt = dsBins.Tables[0];
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            BinMasterDTO objBinMaster = new BinMasterDTO();
                            if (dt.Columns.Contains("CustomerOwnedQuantity"))
                            {
                                double tmpdbl = 0;
                                double.TryParse(Convert.ToString(dr["CustomerOwnedQuantity"]), out tmpdbl);
                                objBinMaster.CustomerOwnedQuantity = tmpdbl;
                            }
                            if (dt.Columns.Contains("ConsignedQuantity"))
                            {
                                double tmpdbl = 0;
                                double.TryParse(Convert.ToString(dr["ConsignedQuantity"]), out tmpdbl);
                                objBinMaster.ConsignedQuantity = tmpdbl;
                            }
                            if (dt.Columns.Contains("RequestedQuantity"))
                            {
                                double tmpdbl = 0;
                                double.TryParse(Convert.ToString(dr["RequestedQuantity"]), out tmpdbl);
                                objBinMaster.CriticalQuantity = tmpdbl;
                            }
                            if (dt.Columns.Contains("QuantityRequisitioned"))
                            {
                                double tmpdbl = 0;
                                double.TryParse(Convert.ToString(dr["QuantityRequisitioned"]), out tmpdbl);
                                objBinMaster.MinimumQuantity = tmpdbl;
                            }
                            if (dt.Columns.Contains("RequestedQuantityTransfer"))
                            {
                                double tmpdbl = 0;
                                double.TryParse(Convert.ToString(dr["RequestedQuantityTransfer"]), out tmpdbl);
                                objBinMaster.MaximumQuantity = tmpdbl;
                            }
                            if (dt.Columns.Contains("SuggestedOrderQuantity"))
                            {
                                double tmpdbl = 0;
                                double.TryParse(Convert.ToString(dr["SuggestedOrderQuantity"]), out tmpdbl);
                                objBinMaster.SuggestedOrderQuantity = tmpdbl;
                            }
                            lstBins.Add(objBinMaster);
                        }
                    }
                }
                //foreach (var item in lstBins)
                //{
                //    if (((item.CustomerOwnedQuantity ?? 0) + (item.ConsignedQuantity ?? 0) + (item.MinimumQuantity ?? 0) + (item.MaximumQuantity ?? 0) + (item.CriticalQuantity ?? 0) + (item.SuggestedOrderQuantity ?? 0)) != 0)
                //    {
                //        retval = true;
                //        break;
                //    }
                //}
                return lstBins;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool IsBinInUse(long RoomID, long CompanyID, long BinID, Guid ItemGUID)
        {
            bool retval = false;
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            List<BinMasterDTO> lstBins = new List<BinMasterDTO>();
            if (Connectionstring == "")
            {
                return false;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            DataSet dsBins = SqlHelper.ExecuteDataset(EturnsConnection, "CheckItemBinReference", RoomID, CompanyID, BinID, ItemGUID);
            if (dsBins != null && dsBins.Tables.Count > 0)
            {
                DataTable dt = dsBins.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        BinMasterDTO objBinMaster = new BinMasterDTO();
                        if (dt.Columns.Contains("CustomerOwnedQuantity"))
                        {
                            double tmpdbl = 0;
                            double.TryParse(Convert.ToString(dr["CustomerOwnedQuantity"]), out tmpdbl);
                            objBinMaster.CustomerOwnedQuantity = tmpdbl;
                        }
                        if (dt.Columns.Contains("ConsignedQuantity"))
                        {
                            double tmpdbl = 0;
                            double.TryParse(Convert.ToString(dr["ConsignedQuantity"]), out tmpdbl);
                            objBinMaster.ConsignedQuantity = tmpdbl;
                        }
                        if (dt.Columns.Contains("RequestedQuantity"))
                        {
                            double tmpdbl = 0;
                            double.TryParse(Convert.ToString(dr["RequestedQuantity"]), out tmpdbl);
                            objBinMaster.CriticalQuantity = tmpdbl;
                        }
                        if (dt.Columns.Contains("QuantityRequisitioned"))
                        {
                            double tmpdbl = 0;
                            double.TryParse(Convert.ToString(dr["QuantityRequisitioned"]), out tmpdbl);
                            objBinMaster.MinimumQuantity = tmpdbl;
                        }
                        if (dt.Columns.Contains("RequestedQuantityTransfer"))
                        {
                            double tmpdbl = 0;
                            double.TryParse(Convert.ToString(dr["RequestedQuantityTransfer"]), out tmpdbl);
                            objBinMaster.MaximumQuantity = tmpdbl;
                        }
                        if (dt.Columns.Contains("SuggestedOrderQuantity"))
                        {
                            double tmpdbl = 0;
                            double.TryParse(Convert.ToString(dr["SuggestedOrderQuantity"]), out tmpdbl);
                            objBinMaster.SuggestedOrderQuantity = tmpdbl;
                        }
                        lstBins.Add(objBinMaster);
                    }
                }
            }
            foreach (var item in lstBins)
            {
                if (((item.CustomerOwnedQuantity ?? 0) + (item.ConsignedQuantity ?? 0) + (item.MinimumQuantity ?? 0) + (item.MaximumQuantity ?? 0) + (item.CriticalQuantity ?? 0) + (item.SuggestedOrderQuantity ?? 0)) != 0)
                {
                    retval = true;
                    break;
                }
            }
            return retval;
        }
        public bool DeleteRecordsExcept(string IDs, Guid ItemGUID, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (IDs != "")
                {
                    string strQuery = "";
                    strQuery += "UPDATE BinMaster SET LastUpdated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ItemGUID = '" + ItemGUID.ToString() + "' AND ID Not in( ";
                    foreach (var item in IDs.Split(','))
                    {
                        if (!string.IsNullOrEmpty(item.Trim()))
                        {
                            strQuery += item.ToString() + ",";
                        }
                    }
                    strQuery = strQuery.Substring(0, strQuery.Length - 1);
                    strQuery += ");";
                    context.Database.ExecuteSqlCommand(strQuery);


                    //string strUpdateOnHand = "EXEC [dbo].[AutoCartEntryonInventoryUpDown] '" + ItemGUID.ToString() + "', " + userid;
                    //context.Database.ExecuteSqlCommand(strUpdateOnHand);
                    //new CartItemDAL(base.DataBaseName).AutoCartUpdateByCode(ItemGUID, userid);

                    //Get Cached-Media
                    CacheHelper<IEnumerable<BinMasterDTO>>.InvalidateCache();

                    //BinMasterDAL objItemLocationLevelQuanityDAL = new BinMasterDAL(base.DataBaseName);
                    //objItemLocationLevelQuanityDAL.GetCachedData(0, CompanyID, false, false);
                }

                return true;
            }
        }
        //public IEnumerable<BinMasterDTO> GetAllRecordsItemLocationQty(Int64 RoomID, Int64 CompanyId)
        //{
        //    return GetItemLocationQty(RoomID, CompanyId).OrderBy("ID DESC");
        //}
        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE BinMaster SET LastUpdated = '" + DateTime.Now.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1,EditedFrom='Web',ReceivedOn='" + DateTimeUtility.DateTimeNow + "' WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.Database.ExecuteSqlCommand(strQuery);

                //Get Cached-Media
                IEnumerable<BinMasterDTO> ObjCache = CacheHelper<IEnumerable<BinMasterDTO>>.GetCacheItem("Cached_BinMaster_" + CompanyId.ToString());
                if (ObjCache != null)
                {
                    //Remove the items from the cache    
                    List<BinMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                    ObjCache = objTemp.AsEnumerable();

                    ////Add the deleted items to the cache querying the database
                    //IDs = IDs.TrimEnd(',');
                    //IEnumerable<BinMasterDTO> obj = (from u in context.Database.SqlQuery<BinMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM BinMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.ID IN (" + IDs + ")") //+ @" AND A.Room = " + RoomID.ToString()) //A.IsDeleted!=1 AND A.IsArchived != 1 AND 
                    //                                 select new BinMasterDTO
                    //                                 {
                    //                                     ID = u.ID,
                    //                                     BinNumber = u.BinNumber,
                    //                                     Created = u.Created,
                    //                                     LastUpdated = u.LastUpdated,
                    //                                     CreatedByName = u.CreatedByName,
                    //                                     UpdatedByName = u.UpdatedByName,
                    //                                     RoomName = u.RoomName,
                    //                                     CreatedBy = u.CreatedBy,
                    //                                     LastUpdatedBy = u.LastUpdatedBy,
                    //                                     CompanyID = u.CompanyID,
                    //                                     IsArchived = u.IsArchived,
                    //                                     IsDeleted = u.IsDeleted,
                    //                                     GUID = u.GUID,
                    //                                     UDF1 = u.UDF1,
                    //                                     UDF2 = u.UDF2,
                    //                                     UDF3 = u.UDF3,
                    //                                     UDF4 = u.UDF4,
                    //                                     UDF5 = u.UDF5
                    //                                 }).AsParallel().ToList();

                    CacheHelper<IEnumerable<BinMasterDTO>>.AppendToCacheItem("Cached_BinMaster_" + CompanyId.ToString(), ObjCache);
                }

                return true;
            }
        }
        public bool Delete(Int64 id, Int64 userid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                BinMaster obj = context.BinMasters.Single(t => t.ID == id);
                obj.IsDeleted = true;
                obj.IsArchived = false;
                obj.LastUpdated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = userid;
                obj.EditedFrom = "Web-BinMaster.Delete";
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                context.Entry(obj).State = System.Data.Entity.EntityState.Unchanged;
                context.BinMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                //Get Cached-Media
                IEnumerable<BinMasterDTO> ObjCache = CacheHelper<IEnumerable<BinMasterDTO>>.GetCacheItem("Cached_BinMaster_" + obj.CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<BinMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == id);
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<BinMasterDTO>>.AppendToCacheItem("Cached_BinMaster_" + obj.CompanyID.ToString(), ObjCache);
                }
                return true;
            }
        }
        public bool Delete(Int64 id, Int64 userid, Guid ItemGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                BinMaster obj = context.BinMasters.Single(t => t.ID == id && t.ItemGUID == ItemGUID);
                obj.IsDeleted = true;
                obj.IsArchived = false;
                obj.LastUpdated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = userid;
                obj.EditedFrom = "Web-BinMaster.Delete3Para";
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                context.Entry(obj).State = System.Data.Entity.EntityState.Unchanged;
                context.BinMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                //Get Cached-Media
                IEnumerable<BinMasterDTO> ObjCache = CacheHelper<IEnumerable<BinMasterDTO>>.GetCacheItem("Cached_BinMaster_" + obj.CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<BinMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == id);
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<BinMasterDTO>>.AppendToCacheItem("Cached_BinMaster_" + obj.CompanyID.ToString(), ObjCache);
                }
                return true;
            }
        }
        public List<BinMasterDTO> GetAllRecordsPULLBin(Guid ItemGUID, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            ItemLocationDetailsDAL objdal = new ItemLocationDetailsDAL(base.DataBaseName);
            IEnumerable<ItemLocationDetailsDTO> objDetailDTO = objdal.GetAllRecords(RoomID, CompanyId, ItemGUID, null, "BinNumber ASC").Where(t => t.ConsignedQuantity > 0 || t.CustomerOwnedQuantity > 0);
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            eTurnsRegionInfo objeTurnsRegionInfo = null;
            objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyId, 0);
            string qtyFormat = "N";
            //CompanyConfigDTO objcompConfig = new CompanyConfigDAL(base.DataBaseName).GetRecord(CompanyId);

            //string qtyFormat = "N" + objcompConfig.QuantityDecimalPoints;
            if (objeTurnsRegionInfo != null)
                qtyFormat = "N" + objeTurnsRegionInfo.NumberDecimalDigits;

            var LocationData = (from x in objDetailDTO
                                group x by new { x.BinID, x.BinNumber, x.ItemGUID }
                                    into grp
                                select new BinMasterDTO
                                {
                                    ID = Convert.ToInt64(grp.Key.BinID),
                                    BinNumber = grp.Key.BinNumber + " (" + grp.Sum(y => y.ConsignedQuantity.GetValueOrDefault(0) + y.CustomerOwnedQuantity.GetValueOrDefault(0)).ToString(qtyFormat) + ")",
                                    //grp.Key.ItemID
                                }).ToList();

            List<BinMasterDTO> FinalLData = new List<BinMasterDTO>();
            FinalLData.AddRange(LocationData);

            return FinalLData;
        }
        public IEnumerable<BinMasterDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<BinMasterDTO> ObjCache;
            if (IsArchived == false && IsDeleted == false)
            {
                //Get Cached-Media
                ObjCache = CacheHelper<IEnumerable<BinMasterDTO>>.GetCacheItem("Cached_BinMaster_" + CompanyID.ToString());
                if (ObjCache == null)
                {
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        IEnumerable<BinMasterDTO> obj = (from u in context.Database.SqlQuery<BinMasterDTO>(@"SELECT A.*, Iv.eVMISensorID, Iv.eVMISensorPort, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName , I.ItemNumber FROM BinMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID LEFT OUTER JOIN ItemMaster I on A.ItemGUID = I.GUID  Left Outer join ItemLocationeVMISetup Iv on Iv.BinID = A.Id and Iv.ItemGUID = A.ItemGUID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString()) //A.IsDeleted!=1 AND A.IsArchived != 1 AND 
                                                         select new BinMasterDTO
                                                         {
                                                             ID = u.ID,
                                                             BinNumber = u.BinNumber,
                                                             Created = u.Created,
                                                             LastUpdated = u.LastUpdated,
                                                             IsStagingLocation = u.IsStagingLocation,
                                                             IsStagingHeader = u.IsStagingHeader,
                                                             MaterialStagingGUID = u.MaterialStagingGUID,
                                                             CreatedByName = u.CreatedByName,
                                                             UpdatedByName = u.UpdatedByName,
                                                             RoomName = u.RoomName,
                                                             CreatedBy = u.CreatedBy,
                                                             LastUpdatedBy = u.LastUpdatedBy,
                                                             Room = u.Room,
                                                             CompanyID = u.CompanyID,
                                                             IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                             IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                             GUID = u.GUID,
                                                             UDF1 = u.UDF1,
                                                             UDF2 = u.UDF2,
                                                             UDF3 = u.UDF3,
                                                             UDF4 = u.UDF4,
                                                             UDF5 = u.UDF5,
                                                             ItemGUID = u.ItemGUID,
                                                             MinimumQuantity = u.MinimumQuantity,
                                                             MaximumQuantity = u.MaximumQuantity,
                                                             CriticalQuantity = u.CriticalQuantity,
                                                             ConsignedQuantity = u.ConsignedQuantity,
                                                             SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                                                             IsDefault = u.IsDefault,
                                                             CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                             ItemNumber = u.ItemNumber,
                                                             eVMISensorID = u.eVMISensorID,
                                                             eVMISensorPort = u.eVMISensorPort,
                                                             AddedFrom = u.AddedFrom,
                                                             EditedFrom = u.EditedFrom,
                                                             ReceivedOn = u.ReceivedOn,
                                                             ReceivedOnWeb = u.ReceivedOnWeb,
                                                             IsEnforceDefaultPullQuantity = u.IsEnforceDefaultPullQuantity,
                                                             IsEnforceDefaultReorderQuantity = u.IsEnforceDefaultReorderQuantity,
                                                             DefaultPullQuantity = u.DefaultPullQuantity,
                                                             DefaultReorderQuantity = u.DefaultReorderQuantity
                                                         }).AsParallel().ToList();
                        ObjCache = CacheHelper<IEnumerable<BinMasterDTO>>.AddCacheItem("Cached_BinMaster_" + CompanyID.ToString(), obj);
                    }
                }
            }
            else
            {
                string sSQL = "";
                if (IsArchived && IsDeleted)
                {
                    sSQL += "A.IsDeleted = 1 AND A.IsArchived = 1";
                }
                else if (IsArchived)
                {
                    sSQL += "A.IsArchived = 1";
                }
                else if (IsDeleted)
                {
                    sSQL += "A.IsDeleted =1";
                }
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    ObjCache = (from u in context.Database.SqlQuery<BinMasterDTO>(@"SELECT A.*, Iv.eVMISensorID, Iv.eVMISensorPort, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM BinMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID Left Outer join ItemLocationeVMISetup Iv on Iv.BinID = A.Id and Iv.ItemGUID = A.ItemGUID WHERE A.CompanyID = " + CompanyID.ToString() + " AND " + sSQL)
                                select new BinMasterDTO
                                {
                                    ID = u.ID,
                                    BinNumber = u.BinNumber,
                                    Created = u.Created,
                                    LastUpdated = u.LastUpdated,
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    RoomName = u.RoomName,
                                    IsStagingLocation = u.IsStagingLocation,
                                    IsStagingHeader = u.IsStagingHeader,
                                    MaterialStagingGUID = u.MaterialStagingGUID,
                                    CreatedBy = u.CreatedBy,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    Room = u.Room,
                                    CompanyID = u.CompanyID,
                                    IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                    IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                    GUID = u.GUID,
                                    UDF1 = u.UDF1,
                                    UDF2 = u.UDF2,
                                    UDF3 = u.UDF3,
                                    UDF4 = u.UDF4,
                                    UDF5 = u.UDF5,
                                    ItemGUID = u.ItemGUID,
                                    MinimumQuantity = u.MinimumQuantity,
                                    MaximumQuantity = u.MaximumQuantity,
                                    CriticalQuantity = u.CriticalQuantity,
                                    ConsignedQuantity = u.ConsignedQuantity,
                                    SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                                    IsDefault = (u.IsDefault ?? u.IsDefault),
                                    CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                    eVMISensorID = u.eVMISensorID,
                                    eVMISensorPort = u.eVMISensorPort,
                                    AddedFrom = u.AddedFrom,
                                    EditedFrom = u.EditedFrom,
                                    ReceivedOn = u.ReceivedOn,
                                    ReceivedOnWeb = u.ReceivedOnWeb,
                                    IsEnforceDefaultPullQuantity = u.IsEnforceDefaultPullQuantity,
                                    IsEnforceDefaultReorderQuantity = u.IsEnforceDefaultReorderQuantity,
                                    DefaultPullQuantity = u.DefaultPullQuantity,
                                    DefaultReorderQuantity = u.DefaultReorderQuantity
                                }).AsParallel().ToList();
                }
            }

            return ObjCache.Where(t => t.Room == RoomID);
        }
        public IEnumerable<BinMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            return GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted).OrderBy("ID DESC");
        }
        public BinMasterDTO GetCachedDataLocationQty(Guid ItemGUID, Int64 BinID, Int64 RoomID, Int64 CompanyID)
        {
            //Get Cached-Media
            IEnumerable<BinMasterDTO> ObjCache;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<BinMasterDTO> obj = (from u in context.Database.SqlQuery<BinMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.BinNumber 
                    FROM BinMaster A 
                    LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    LEFT OUTER JOIN Room D on A.Room = D.ID 
                    WHERE A.CompanyID = " + CompanyID.ToString())
                                                 select new BinMasterDTO
                                                 {
                                                     ID = u.ID,
                                                     BinNumber = u.BinNumber,
                                                     CriticalQuantity = u.CriticalQuantity,
                                                     MinimumQuantity = u.MinimumQuantity,
                                                     MaximumQuantity = u.MaximumQuantity,
                                                     ItemGUID = u.ItemGUID,
                                                     CompanyID = u.CompanyID,
                                                     Room = u.Room,
                                                     IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                     IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                     Created = u.Created,
                                                     LastUpdated = u.LastUpdated,
                                                     CreatedBy = u.CreatedBy,
                                                     LastUpdatedBy = u.LastUpdatedBy,
                                                     CreatedByName = u.CreatedByName,
                                                     UpdatedByName = u.UpdatedByName,
                                                     RoomName = u.RoomName,
                                                     GUID = u.GUID,
                                                     SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                                                     IsEnforceDefaultPullQuantity = u.IsEnforceDefaultPullQuantity,
                                                     IsEnforceDefaultReorderQuantity = u.IsEnforceDefaultReorderQuantity,
                                                     DefaultPullQuantity = u.DefaultPullQuantity,
                                                     DefaultReorderQuantity = u.DefaultReorderQuantity
                                                 }).AsParallel().ToList();
                ObjCache = CacheHelper<IEnumerable<BinMasterDTO>>.AddCacheItem("Cached_ItemLocationLevelQuanity_" + CompanyID.ToString(), obj);
            }

            return ObjCache.Where(t => t.Room == RoomID && t.ItemGUID == ItemGUID && t.ID == BinID).FirstOrDefault();
        }
        //public BinMasterDTO GetRecord(string LocationName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        //{
        //    return GetCachedData(RoomID, CompanyId, false, false).Where(t => t.BinNumber.ToLower() == LocationName.ToLower()).FirstOrDefault();
        //}
        //public IEnumerable<BinMasterDTO> GetRecords(Int64 RoomID, Int64 CompanyId)
        //{
        //    return GetCachedData(RoomID, CompanyId, false, false).OrderBy("ID DESC");
        //}
        //public BinMasterDTO GetRecordByItemGuid(string LocationName, Guid ItemGuid, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        //{
        //    return GetCachedData(RoomID, CompanyId, false, false).Where(t => (t.BinNumber ?? string.Empty).ToLower() == (LocationName ?? string.Empty).ToLower() && t.ItemGUID == ItemGuid).FirstOrDefault();
        //}
        //public IEnumerable<BinMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        //{
        //    //Get Cached-Media
        //    IEnumerable<BinMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted);
        //    //IEnumerable<BinMasterDTO> ObjCache = GetCachedDataWithOutLabor(RoomID, CompanyId, IsArchived, IsDeleted);

        //    if (String.IsNullOrEmpty(SearchTerm))
        //    {
        //        //Get Cached-Media
        //        TotalCount = ObjCache.Count();
        //        return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        //    }
        //    else if (SearchTerm.Contains("[###]"))
        //    {
        //        //Get Cached-Media
        //        //IEnumerable<BinMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
        //        string[] stringSeparators = new string[] { "[###]" };
        //        string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
        //        //CreatedBy,UpdatedBy,DateCreatedFrom,DateUpdatedFrom,UDF1,UDF2,UDF3,UDF4,UDF5,[###]admin,niraj$$$$$$$test2$$
        //        // 1 = createdBy, 2 = updatedby , 3 = CreatedFrom, 4 = CreatedTo, 5=UpdatedFrom,6=UpdatedTo

        //        ObjCache = ObjCache.Where(t =>
        //               ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
        //            && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
        //            && ((Fields[1].Split('@')[2] == "") || (t.Created.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
        //            && ((Fields[1].Split('@')[3] == "") || (t.LastUpdated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.LastUpdated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
        //            && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
        //            && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
        //            && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
        //            && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
        //            && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
        //            );

        //        #region Previous code
        //        //foreach (var item in Fields)
        //        //{
        //        //    if (item.Length > 0)
        //        //    {
        //        //        if (item.Contains("CreatedBy"))
        //        //        {
        //        //            ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.CreatedByName.ToString()));
        //        //        }
        //        //        else if (item.Contains("UpdatedBy"))
        //        //        {
        //        //            ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UpdatedByName.ToString()));
        //        //        }
        //        //        else if (item.Contains("DateCreatedFrom"))
        //        //        {
        //        //            ObjCache = ObjCache.Where(t => t.Created.Value.Date >= Convert.ToDateTime(item.Split('#')[1]).Date && t.Created.Value.Date <= Convert.ToDateTime(item.Split('#')[3]).Date);
        //        //        }
        //        //        else if (item.Contains("DateUpdatedFrom"))
        //        //        {
        //        //            ObjCache = ObjCache.Where(t => t.LastUpdated.Value.Date >= Convert.ToDateTime(item.Split('#')[1]).Date && t.LastUpdated.Value.Date <= Convert.ToDateTime(item.Split('#')[3]).Date);
        //        //        }
        //        //        else if (item.Contains("UDF1"))
        //        //        {
        //        //            ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UDF1));
        //        //        }
        //        //        else if (item.Contains("UDF2"))
        //        //        {
        //        //            ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UDF2));
        //        //        }
        //        //        else if (item.Contains("UDF3"))
        //        //        {
        //        //            ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UDF3));
        //        //        }
        //        //        else if (item.Contains("UDF4"))
        //        //        {
        //        //            ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UDF4));
        //        //        }
        //        //        else if (item.Contains("UDF5"))
        //        //        {
        //        //            ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UDF5));
        //        //        }
        //        //    }
        //        //} 
        //        #endregion
        //        TotalCount = ObjCache.Count();
        //        return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        //    }
        //    else
        //    {
        //        //Get Cached-Media
        //        //IEnumerable<BinMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
        //        TotalCount = ObjCache.Where
        //            (
        //                t => t.ID.ToString().Contains(SearchTerm) ||
        //                (t.BinNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.ItemNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.eVMISensorID ?? 0).ToString().IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.eVMISensorPort ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.MinimumQuantity ?? 0).ToString().IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.MaximumQuantity ?? 0).ToString().IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.CriticalQuantity ?? 0).ToString().IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.RoomName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.CreatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
        //            ).Count();
        //        return ObjCache.Where
        //            (t => t.ID.ToString().Contains(SearchTerm) ||
        //                (t.BinNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.ItemNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.eVMISensorID ?? 0).ToString().IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.eVMISensorPort ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.MinimumQuantity ?? 0).ToString().IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.MaximumQuantity ?? 0).ToString().IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.CriticalQuantity ?? 0).ToString().IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.RoomName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.CreatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
        //            ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        //    }
        //}

        public BinMasterDTO GetBinByID(long ID, Int64 RoomID, Int64 CompanyId, Guid BinGUID)
        {
            BinMasterDTO objBinMasterDTO = new BinMasterDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objBinMasterDTO = (from ci in context.BinMasters
                                   join cc in context.UserMasters on ci.CreatedBy equals cc.ID into ci_cc_join
                                   from ci_cc in ci_cc_join.DefaultIfEmpty()
                                   join cu in context.UserMasters on ci.LastUpdatedBy equals cu.ID into ci_cu_join
                                   from ci_cu in ci_cu_join.DefaultIfEmpty()
                                   where (ci.ID == ID || ci.GUID == BinGUID) && ci.Room == RoomID && ci.CompanyID == CompanyId && ci.IsDeleted == false
                                   select new BinMasterDTO
                                   {
                                       BinNumber = ci.BinNumber,
                                       CompanyID = ci.CompanyID,
                                       Created = ci.Created,
                                       CreatedBy = ci.CreatedBy,
                                       CriticalQuantity = ci.CriticalQuantity,
                                       GUID = ci.GUID,
                                       ID = ci.ID,
                                       IsArchived = ci.IsArchived,
                                       IsDefault = ci.IsDefault,
                                       IsDeleted = ci.IsDeleted,
                                       IsStagingHeader = ci.IsStagingHeader,
                                       IsStagingLocation = ci.IsStagingLocation,
                                       ItemGUID = ci.ItemGUID,
                                       LastUpdated = ci.LastUpdated,
                                       LastUpdatedBy = ci.LastUpdatedBy,
                                       MaterialStagingGUID = ci.MaterialStagingGUID,
                                       MaximumQuantity = ci.MaximumQuantity,
                                       MinimumQuantity = ci.MinimumQuantity,
                                       ParentBinId = ci.ParentBinId,
                                       Room = ci.Room,
                                       SuggestedOrderQuantity = ci.SuggestedOrderQuantity,
                                       UDF1 = ci.UDF1,
                                       UDF10 = ci.UDF10,
                                       UDF2 = ci.UDF2,
                                       UDF3 = ci.UDF3,
                                       UDF4 = ci.UDF4,
                                       UDF5 = ci.UDF5,
                                       CreatedByName = ci_cc.UserName,
                                       UpdatedByName = ci_cu.UserName,
                                       AddedFrom = (ci.AddedFrom == null ? "Web" : ci.AddedFrom),
                                       EditedFrom = (ci.EditedFrom == null ? "Web" : ci.EditedFrom),
                                       ReceivedOn = (ci.ReceivedOn == null ? DateTimeUtility.DateTimeNow : ci.ReceivedOn),
                                       ReceivedOnWeb = (ci.ReceivedOnWeb == null ? DateTimeUtility.DateTimeNow : ci.ReceivedOnWeb),
                                       IsEnforceDefaultPullQuantity = ci.IsEnforceDefaultPullQuantity,
                                       IsEnforceDefaultReorderQuantity = ci.IsEnforceDefaultReorderQuantity,
                                       DefaultPullQuantity = ci.DefaultPullQuantity,
                                       DefaultReorderQuantity = ci.DefaultReorderQuantity
                                   }).FirstOrDefault();
            }
            return objBinMasterDTO;
        }
    }
}
