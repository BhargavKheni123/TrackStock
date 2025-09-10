using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace eTurns.DAL
{
    public class BarcodeMasterDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        //public BarcodeMasterDAL(base.DataBaseName)
        //{

        //}

        public BarcodeMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public BarcodeMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion
        string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
        /// <summary>
        /// Get Paged Records from the Technician Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<BarcodeMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return (from x in context.Database.SqlQuery<BarcodeMasterDTO>("exec [GetBarcodeRecord] @RoomID,@CompanyID", params1)
                        select new BarcodeMasterDTO
                        {
                            ID = x.ID,
                            CreatedOn = x.CreatedOn,
                            UpdatedOn = x.UpdatedOn,
                            CreatedBy = x.CreatedBy,
                            UpdatedBy = x.UpdatedBy,
                            IsDeleted = x.IsDeleted,
                            GUID = x.GUID,
                            UpdatedByName = x.UpdatedByName,
                            Action = string.Empty,
                            BarcodeString = x.BarcodeString,
                            CompanyID = x.CompanyID,
                            CreatedByName = x.CreatedByName,
                            HistoryID = 0,
                            IsArchived = x.IsArchived,
                            ModuleGUID = x.ModuleGUID,
                            ModuleName = x.ModuleName,
                            RefGUID = x.RefGUID,
                            RefNumber = string.Empty,
                            RoomID = x.RoomID,
                            RoomName = x.RoomName,
                            BarcodeAdded = x.BarcodeAdded,

                        }).ToList();

            }
        }

        public IEnumerable<BarcodeMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyID, bool Isdeleted, bool IsArchieved)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@Isdeleted", Isdeleted), new SqlParameter("@IsArchieved", IsArchieved) };

                return (from x in context.Database.SqlQuery<BarcodeMasterDTO>("exec [BarcodeNarrowSearch] @RoomID,@CompanyID,@Isdeleted,@IsArchieved", params1)
                        select new BarcodeMasterDTO
                        {
                            ID = x.ID,
                            CreatedOn = x.CreatedOn,
                            UpdatedOn = x.UpdatedOn,
                            CreatedBy = x.CreatedBy,
                            UpdatedBy = x.UpdatedBy,
                            IsDeleted = x.IsDeleted,
                            GUID = x.GUID,
                            UpdatedByName = x.UpdatedByName,
                            Action = string.Empty,
                            BarcodeString = x.BarcodeString,
                            CompanyID = x.CompanyID,
                            CreatedByName = x.CreatedByName,
                            HistoryID = 0,
                            IsArchived = x.IsArchived,
                            ModuleGUID = x.ModuleGUID,
                            ModuleName = x.ModuleName,
                            RefGUID = x.RefGUID,
                            RefNumber = string.Empty,
                            RoomID = x.RoomID,
                            RoomName = x.RoomName,
                            BarcodeAdded = x.BarcodeAdded,
                            BinGuid = x.BinGuid,
                            BinNumber = x.BinNumber,
                            items = x.items,
                            manufacturername = x.manufacturername,
                            ManufacturerNumber = x.ManufacturerNumber,
                            SupplierName = x.SupplierName,
                            SupplierNumber = x.SupplierNumber,
                            OldBarcodeString = x.BarcodeString,
                        }).ToList();

            }
        }
        public IEnumerable<SupplierMasterDTO> GetAllRecordsSupplier(Int64 RoomID, Int64 CompanyID, bool Isdeleted, bool IsArchieved, string ModuleGuid)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@Isdeleted", Isdeleted), new SqlParameter("@IsArchieved", IsArchieved), new SqlParameter("@ModuleGuid", ModuleGuid) };

                return (from x in context.Database.SqlQuery<SupplierMasterDTO>("exec [BarcodeNarrowSearchSupplier] @RoomID,@CompanyID,@Isdeleted,@IsArchieved,@ModuleGuid", params1)
                        select new SupplierMasterDTO
                        {
                            ID = x.ID,
                            SupplierName = x.SupplierName,
                            Count = x.Count
                        }).ToList();

            }
        }
        public IEnumerable<CategoryMasterDTO> GetAllRecordsCategory(Int64 RoomID, Int64 CompanyID, bool Isdeleted, bool IsArchieved, string ModuleGuid)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@Isdeleted", Isdeleted), new SqlParameter("@IsArchieved", IsArchieved), new SqlParameter("@ModuleGuid", ModuleGuid) };

                return (from x in context.Database.SqlQuery<CategoryMasterDTO>("exec [BarcodeNarrowSearchCategory] @RoomID,@CompanyID,@Isdeleted,@IsArchieved,@ModuleGuid", params1)
                        select new CategoryMasterDTO
                        {
                            ID = x.ID,
                            Category = x.Category,
                            Count = x.Count
                        }).ToList();

            }
        }

        public IEnumerable<BarcodeMasterDTO> GetAllRecordsUsingModuleId(Int64 RoomID, Int64 CompanyID, bool Isdeleted, bool IsArchieved, string ModuleGuid)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@Isdeleted", Isdeleted), new SqlParameter("@IsArchieved", IsArchieved), new SqlParameter("@ModuleGuid", ModuleGuid) };

                return (from x in context.Database.SqlQuery<BarcodeMasterDTO>("exec [BarcodeNarrowSearchModuleBy] @RoomID,@CompanyID,@Isdeleted,@IsArchieved,@ModuleGuid", params1)
                        select new BarcodeMasterDTO
                        {
                            ID = x.ID,
                            CreatedOn = x.CreatedOn,
                            UpdatedOn = x.UpdatedOn,
                            CreatedBy = x.CreatedBy,
                            UpdatedBy = x.UpdatedBy,
                            IsDeleted = x.IsDeleted,
                            GUID = x.GUID,
                            UpdatedByName = x.UpdatedByName,
                            Action = string.Empty,
                            BarcodeString = x.BarcodeString,
                            CompanyID = x.CompanyID,
                            CreatedByName = x.CreatedByName,
                            HistoryID = 0,
                            IsArchived = x.IsArchived,
                            ModuleGUID = x.ModuleGUID,
                            ModuleName = x.ModuleName,
                            RefGUID = x.RefGUID,
                            RefNumber = string.Empty,
                            RoomID = x.RoomID,
                            RoomName = x.RoomName,
                            BarcodeAdded = x.BarcodeAdded,
                            BinGuid = x.BinGuid,
                            BinNumber = x.BinNumber,
                            items = x.items,
                            manufacturername = x.manufacturername,
                            ManufacturerNumber = x.ManufacturerNumber,
                            SupplierName = x.SupplierName,
                            SupplierNumber = x.SupplierNumber,
                            OldBarcodeString = x.BarcodeString,
                        }).ToList();

            }
        }

        //public BarcodeMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        //{
        //    return GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.ID == id).SingleOrDefault();
        //}
        
        /// <summary>
        /// Get Paged Records from the Technician Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<BarcodeMasterDTO> GetAllDeletedRecords(Int64 RoomID, Int64 CompanyID)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                /*return (from x in context.Database.SqlQuery<BarcodeMasterDTO>(@"SELECT A.*, B.UserName AS CreatedByName, C.UserName AS UpdatedByName 
                                                                                ,R.RoomName AS RoomName, M.ModuleName AS ModuleName
                                                                                FROM BarcodeMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
					                                                                                 LEFT OUTER JOIN UserMaster C on A.UpdatedBy = C.ID 
					                                                                                 LEFT OUTER JOIN Room R on A.RoomID = R.ID
					                                                                                 LEFT OUTER JOIN ModuleMaster M ON A.ModuleGuid = M.GUID  AND ISNULL(M.IsDeleted,0) = 0 
                                                                                WHERE ISNULL(A.IsDeleted,0) =1 AND  A.RoomID = " + RoomID + @" AND A.CompanyID = " + CompanyID) */

                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return (from x in context.Database.SqlQuery<BarcodeMasterDTO>("exec [GetBarcodeDeletedRecords] @RoomID,@CompanyID", params1)
                        select new BarcodeMasterDTO
                        {
                            ID = x.ID,
                            CreatedOn = x.CreatedOn,
                            UpdatedOn = x.UpdatedOn,
                            CreatedBy = x.CreatedBy,
                            UpdatedBy = x.UpdatedBy,
                            IsDeleted = x.IsDeleted,
                            GUID = x.GUID,
                            UpdatedByName = x.UpdatedByName,
                            Action = string.Empty,
                            BarcodeString = x.BarcodeString,
                            CompanyID = x.CompanyID,
                            CreatedByName = x.CreatedByName,
                            HistoryID = 0,
                            IsArchived = x.IsArchived,
                            ModuleGUID = x.ModuleGUID,
                            ModuleName = x.ModuleName,
                            RefGUID = x.RefGUID,
                            RefNumber = string.Empty,
                            RoomID = x.RoomID,
                            RoomName = x.RoomName,

                        }).ToList();

            }
        }


        /// <summary>
        /// Get Paged Records from the Technician Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<BarcodeMasterDTO> GetAllActiveRecordsByModuleID(string ModuleGuid, Int64 RoomID, Int64 CompanyID)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                /*return (from x in context.Database.SqlQuery<BarcodeMasterDTO>(@"SELECT A.*, B.UserName AS CreatedByName, C.UserName AS UpdatedByName 
                                                                                ,R.RoomName AS RoomName, M.ModuleName AS ModuleName,'' AS CreatedDate,'' As UpdatedDate
                                                                                FROM BarcodeMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
					                                                                                 LEFT OUTER JOIN UserMaster C on A.UpdatedBy = C.ID 
					                                                                                 LEFT OUTER JOIN Room R on A.RoomID = R.ID
					                                                                                 LEFT OUTER JOIN ModuleMaster M ON A.ModuleGuid = M.GUID  AND ISNULL(M.IsDeleted,0) = 0 
                                                                                                     LEFT OUTER JOIN BinMaster BM on A.binguid=BM.guid
                                                                                WHERE ISNULL(A.IsArchived,0)!=1 AND ISNULL(A.IsDeleted,0)!=1 AND ISNULL(bm.isdeleted,0) !=1
                                                                                AND A.RoomID = " + RoomID + @" AND A.CompanyID = " + CompanyID + @" AND A.ModuleGuid = '" + ModuleGuid + "'") */

                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ModuleGuid", ModuleGuid) };
                return (from x in context.Database.SqlQuery<BarcodeMasterDTO>("exec [GetBarcodeActiveRecordsByModuleID] @RoomID,@CompanyID,@ModuleGuid", params1)
                        select new BarcodeMasterDTO
                        {
                            ID = x.ID,
                            CreatedOn = x.CreatedOn,
                            UpdatedOn = x.UpdatedOn,
                            CreatedBy = x.CreatedBy,
                            UpdatedBy = x.UpdatedBy,
                            IsDeleted = x.IsDeleted,
                            GUID = x.GUID,
                            UpdatedByName = x.UpdatedByName,
                            Action = string.Empty,
                            BarcodeString = x.BarcodeString,
                            CompanyID = x.CompanyID,
                            CreatedByName = x.CreatedByName,
                            HistoryID = 0,
                            IsArchived = x.IsArchived,
                            ModuleGUID = x.ModuleGUID,
                            ModuleName = x.ModuleName,
                            RefGUID = x.RefGUID,
                            RefNumber = string.Empty,
                            RoomID = x.RoomID,
                            RoomName = x.RoomName,
                            UpdatedDate = x.UpdatedDate ?? DateTime.Now.ToString(),
                            CreatedDate = x.CreatedDate ?? DateTime.Now.ToString(),
                            BarcodeAdded = x.BarcodeAdded,
                            BinGuid = x.BinGuid
                        }).ToList();

            }
        }
        public Int64 Edit(BarcodeMasterDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                BarcodeMaster objBarCodemaster = context.BarcodeMasters.Where(b => b.ID == objDTO.ID).FirstOrDefault();
                objBarCodemaster.RefGuid = objDTO.RefGUID;
                objBarCodemaster.BarcodeString = objDTO.BarcodeString;
                objBarCodemaster.BinGuid = objDTO.BinGuid;
                objBarCodemaster.UpdatedBy = objDTO.UpdatedBy;
                objBarCodemaster.UpdatedOn = DateTimeUtility.DateTimeNow;

                objBarCodemaster.EditedFrom = objDTO.EditedFrom;
                objBarCodemaster.ReceivedOn = objDTO.ReceivedOn;
                context.SaveChanges();
                CacheHelper<IEnumerable<ItemMasterDTO>>.InvalidateCache();
                CacheHelper<IEnumerable<AssetMasterDTO>>.InvalidateCache();
                return objDTO.ID;
            }
        }
        public bool UpdateBarodeDataToItem(string OldBarCodeString, Int64 RoomId, Int64 CompanyId, Guid ItemGuid, string NewBarcodeString, Int64 UserId, string EditedFrom, string BarCodeAdded)
        {
            try
            {
                string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
                SqlHelper.ExecuteNonQuery(Connectionstring, "UpdateBarodeData", OldBarCodeString, RoomId, CompanyId, ItemGuid, NewBarcodeString, UserId, EditedFrom, BarCodeAdded);

                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public Int64 Insert(BarcodeMasterDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                BarcodeMaster obj = new BarcodeMaster()
                {
                    BarcodeString = objDTO.BarcodeString,
                    CompanyID = objDTO.CompanyID,
                    CreatedBy = objDTO.CreatedBy,
                    CreatedOn = DateTime.Now,
                    GUID = Guid.NewGuid(),
                    ID = 0,
                    IsArchived = false,
                    IsDeleted = false,
                    ModuleGuid = objDTO.ModuleGUID,
                    RefGuid = objDTO.RefGUID,
                    RoomID = objDTO.RoomID,
                    UpdatedBy = objDTO.UpdatedBy,
                    UpdatedOn = DateTime.Now,
                    BinGuid = objDTO.BinGuid,
                    BarcodeAdded = "Manual",
                    AddedFrom = objDTO.AddedFrom,
                    EditedFrom = objDTO.EditedFrom,
                    ReceivedOn = objDTO.ReceivedOn,
                    ReceivedOnWeb = objDTO.ReceivedOnWeb,
                };
                context.BarcodeMasters.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;


                if (objDTO.ModuleName.ToLower().Contains("item master"))
                {
                    //IEnumerable<ItemMasterDTO> ObjCache = CacheHelper<IEnumerable<ItemMasterDTO>>.GetCacheItem("Cached_ItemMaster_" + objDTO.CompanyID.ToString());
                    //if (ObjCache != null)
                    //{
                    //    List<ItemMasterDTO> objTemp = ObjCache.ToList();
                    //    ItemMasterDTO itemDTO = objTemp.SingleOrDefault(x => x.GUID == objDTO.RefGUID);
                    //    if (!string.IsNullOrEmpty(itemDTO.AppendedBarcodeString))
                    //        itemDTO.AppendedBarcodeString += ",";

                    //    itemDTO.AppendedBarcodeString += objDTO.BarcodeString;
                    //    objTemp.RemoveAll(i => i.GUID == objDTO.RefGUID);
                    //    ObjCache = objTemp.AsEnumerable();
                    //    List<ItemMasterDTO> tempC = new List<ItemMasterDTO>();
                    //    tempC.Add(itemDTO);
                    //    IEnumerable<ItemMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable()).AsEnumerable();
                    //    CacheHelper<IEnumerable<ItemMasterDTO>>.AddCacheItem("Cached_ItemMaster_" + objDTO.CompanyID.ToString(), NewCache);
                    //}
                }
                else if (objDTO.ModuleName.ToLower().Contains("kits"))
                {
                    //Get Cached-Media
                    //IEnumerable<KitMasterDTO> ObjCache = CacheHelper<IEnumerable<KitMasterDTO>>.GetCacheItem("Cached_KitMaster_" + objDTO.CompanyID.ToString());
                    //if (ObjCache != null)
                    //{
                    //    List<KitMasterDTO> objTemp = ObjCache.ToList();
                    //    KitMasterDTO kitDTO = objTemp.SingleOrDefault(x => x.GUID == objDTO.RefGUID);
                    //    if (!string.IsNullOrEmpty(kitDTO.AppendedBarcodeString))
                    //        kitDTO.AppendedBarcodeString += ",";
                    //    kitDTO.AppendedBarcodeString += objDTO.BarcodeString;
                    //    objTemp.RemoveAll(i => i.GUID == objDTO.RefGUID);
                    //    ObjCache = objTemp.AsEnumerable();

                    //    List<KitMasterDTO> tempC = new List<KitMasterDTO>();
                    //    tempC.Add(kitDTO);
                    //    IEnumerable<KitMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    //    CacheHelper<IEnumerable<KitMasterDTO>>.AppendToCacheItem("Cached_KitMaster_" + objDTO.CompanyID.ToString(), NewCache);
                    //}
                }
                else if (objDTO.ModuleName.ToLower().Contains("quick list permission"))
                {
                    //IEnumerable<QuickListMasterDTO> ObjCache = CacheHelper<IEnumerable<QuickListMasterDTO>>.GetCacheItem("Cached_QuickListMaster_" + objDTO.CompanyID.ToString());
                    //if (ObjCache != null)
                    //{
                        //List<QuickListMasterDTO> objTemp = ObjCache.ToList();
                        //QuickListMasterDTO orderDTO = objTemp.SingleOrDefault(x => x.GUID == objDTO.RefGUID);
                        //if (!string.IsNullOrEmpty(orderDTO.AppendedBarcodeString))
                        //    orderDTO.AppendedBarcodeString += ",";
                        //orderDTO.AppendedBarcodeString += objDTO.BarcodeString;
                        //objTemp.RemoveAll(i => i.GUID == objDTO.RefGUID);
                        //objTemp.Add(orderDTO);
                        //CacheHelper<IEnumerable<QuickListMasterDTO>>.AppendToCacheItem("Cached_QuickListMaster_" + objDTO.CompanyID.ToString(), objTemp.AsEnumerable<QuickListMasterDTO>());
                    //}

                }
                else if (objDTO.ModuleName.ToLower().Contains("tool master"))
                {
                    //IEnumerable<ToolMasterDTO> ObjCache = CacheHelper<IEnumerable<ToolMasterDTO>>.GetCacheItem("Cached_ToolMaster_" + objDTO.CompanyID.ToString());
                    //if (ObjCache != null)
                    //{
                        //List<ToolMasterDTO> objTemp = ObjCache.ToList();
                        //ToolMasterDTO orderDTO = objTemp.SingleOrDefault(x => x.GUID == objDTO.RefGUID);
                        //if (!string.IsNullOrEmpty(orderDTO.AppendedBarcodeString))
                        //    orderDTO.AppendedBarcodeString += ",";
                        //orderDTO.AppendedBarcodeString += objDTO.BarcodeString;
                        //objTemp.RemoveAll(i => i.GUID == objDTO.RefGUID);
                        //objTemp.Add(orderDTO);
                        //CacheHelper<IEnumerable<ToolMasterDTO>>.AppendToCacheItem("Cached_ToolMaster_" + objDTO.CompanyID.ToString(), objTemp.AsEnumerable<ToolMasterDTO>());
                    //}
                }
                return objDTO.ID;
            }
        }


        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteRecords(string IDs, Int64 userid, Int64 RoomID, Int64 CompanyId, string ModuleName)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //string strQuery = "";
                string[] strIds = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in strIds)
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        //strQuery += "UPDATE BarcodeMaster SET UpdatedOn = '" + DateTime.Now.ToString() + "' , UpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + " and lower(BarcodeAdded) ='manual';";

                        var params1 = new SqlParameter[] { new SqlParameter("@UserID", userid), new SqlParameter("@ID", Convert.ToInt64(item)) };
                        context.Database.ExecuteSqlCommand("exec [UpdateBarcodeRecord] @UserID,@ID", params1);
                    }
                }
                //context.Database.ExecuteSqlCommand(strQuery);

                //Enumerable<BarcodeMasterDTO> objBarCodeDTOList = GetAllDeletedRecords(RoomID, CompanyId);

                foreach (var item in strIds)
                {
                    //BarcodeMasterDTO objBarcodeDTO = objBarCodeDTOList.FirstOrDefault(x => x.ID == Int64.Parse(item));
                    if (Convert.ToString(ModuleName).ToLower() == "item master")
                    {
                        //IEnumerable<ItemMasterDTO> ObjCache = CacheHelper<IEnumerable<ItemMasterDTO>>.GetCacheItem("Cached_ItemMaster_" + CompanyId.ToString());
                        //if (ObjCache != null && ObjCache.Count() > 0)
                        //{
                        //    List<ItemMasterDTO> objTemp = ObjCache.ToList();
                        //    ItemMasterDTO itemDTO = objTemp.FirstOrDefault(i => i.AppendedBarcodeString.Contains(objBarcodeDTO.BarcodeString));
                        //    string[] ItemBarcodes = itemDTO.AppendedBarcodeString.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        //    objTemp.RemoveAll(i => i.ID == itemDTO.ID);
                        //    List<string> lstString = ItemBarcodes.ToList<string>();
                        //    lstString.RemoveAll(x => x == objBarcodeDTO.BarcodeString);

                        //    string s = string.Join(",", lstString.ToArray());
                        //    itemDTO.AppendedBarcodeString = s;
                        //    objTemp.Add(itemDTO);
                        //    ObjCache = objTemp.AsEnumerable();
                        //    CacheHelper<IEnumerable<ItemMasterDTO>>.AddCacheItem("Cached_ItemMaster_" + CompanyId.ToString(), ObjCache.AsEnumerable());
                        //}
                    }
                    else if (Convert.ToString(ModuleName).ToLower() == "kits")
                    {
                        //IEnumerable<KitMasterDTO> ObjCache = CacheHelper<IEnumerable<KitMasterDTO>>.GetCacheItem("Cached_KitMaster_" + CompanyId.ToString());
                        //if (ObjCache != null && ObjCache.Count() > 0)
                        //{
                            //List<KitMasterDTO> objTemp = ObjCache.ToList();
                            //KitMasterDTO itemDTO = objTemp.FirstOrDefault(i => i.AppendedBarcodeString.Contains(objBarcodeDTO.BarcodeString));
                            //string[] ItemBarcodes = itemDTO.AppendedBarcodeString.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            //objTemp.RemoveAll(i => i.ID == itemDTO.ID);
                            //List<string> lstString = ItemBarcodes.ToList<string>();
                            //lstString.RemoveAll(x => x == objBarcodeDTO.BarcodeString);

                            //string s = string.Join(",", lstString.ToArray());
                            //itemDTO.AppendedBarcodeString = s;
                            //objTemp.Add(itemDTO);
                            //CacheHelper<IEnumerable<KitMasterDTO>>.AddCacheItem("Cached_KitMaster_" + CompanyId.ToString(), objTemp.AsEnumerable());
                        //}
                    }
                    else if (Convert.ToString(ModuleName).ToLower() == "quick list permission")
                    {

                        //IEnumerable<QuickListMasterDTO> ObjCache = CacheHelper<IEnumerable<QuickListMasterDTO>>.GetCacheItem("Cached_QuickListMaster_" + CompanyId.ToString());
                        //if (ObjCache != null && ObjCache.Count() > 0)
                        //{
                            //List<QuickListMasterDTO> objTemp = ObjCache.ToList();
                            //QuickListMasterDTO itemDTO = objTemp.FirstOrDefault(i => i.AppendedBarcodeString.Contains(objBarcodeDTO.BarcodeString));
                            //string[] ItemBarcodes = itemDTO.AppendedBarcodeString.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            //objTemp.RemoveAll(i => i.ID == itemDTO.ID);
                            //List<string> lstString = ItemBarcodes.ToList<string>();
                            //lstString.RemoveAll(x => x == objBarcodeDTO.BarcodeString);
                            //string s = string.Join(",", lstString.ToArray());
                            //itemDTO.AppendedBarcodeString = s;
                            //objTemp.Add(itemDTO);

                            //CacheHelper<IEnumerable<QuickListMasterDTO>>.AppendToCacheItem("Cached_QuickListMaster_" + CompanyId.ToString(), objTemp.AsEnumerable<QuickListMasterDTO>());
                        //}

                    }
                    else if (Convert.ToString(ModuleName).ToLower() == "tool master")
                    {

                        //IEnumerable<ToolMasterDTO> ObjCache = CacheHelper<IEnumerable<ToolMasterDTO>>.GetCacheItem("Cached_ToolMaster_" + CompanyId.ToString());
                        //if (ObjCache != null && ObjCache.Count() > 0)
                        //{
                            //List<ToolMasterDTO> objTemp = ObjCache.ToList();
                            //ToolMasterDTO itemDTO = objTemp.FirstOrDefault(i => i.AppendedBarcodeString.Contains(objBarcodeDTO.BarcodeString));
                            //string[] ItemBarcodes = itemDTO.AppendedBarcodeString.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            //objTemp.RemoveAll(i => i.ID == itemDTO.ID);
                            //List<string> lstString = ItemBarcodes.ToList<string>();
                            //lstString.RemoveAll(x => x == objBarcodeDTO.BarcodeString);
                            //string s = string.Join(",", lstString.ToArray());
                            //itemDTO.AppendedBarcodeString = s;
                            //objTemp.Add(itemDTO);

                            //CacheHelper<IEnumerable<ToolMasterDTO>>.AppendToCacheItem("Cached_ToolMaster_" + CompanyId.ToString(), objTemp.AsEnumerable<ToolMasterDTO>());
                        //}
                    }
                }
                return true;
            }
        }
        public bool UnDeleteRecords(string IDs, Int64 userid, Int64 RoomID, Int64 CompanyId, string ModuleName)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //string strQuery = "";
                string[] strIds = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in strIds)
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        //strQuery += "UPDATE BarcodeMaster SET UpdatedOn = '" + DateTime.Now.ToString() + "' , UpdatedBy = " + userid.ToString() + ", IsDeleted=0 WHERE ID =" + item.ToString() + " and lower(BarcodeAdded) ='manual';";

                        var params1 = new SqlParameter[] { new SqlParameter("@UserID", userid), new SqlParameter("@ID", Convert.ToInt64(item)) };
                        context.Database.ExecuteSqlCommand("exec [UnDeleteBarcodeRecord] @UserID,@ID", params1);
                    }
                }
                //context.Database.ExecuteSqlCommand(strQuery);

                //IEnumerable<BarcodeMasterDTO> objBarCodeDTOList = GetAllDeletedRecords(RoomID, CompanyId);
                foreach (var item in strIds)
                {
                    //BarcodeMasterDTO objBarcodeDTO = objBarCodeDTOList.FirstOrDefault(x => x.ID == Int64.Parse(item));
                    if (Convert.ToString(ModuleName).ToLower() == "item master")
                    {
                        //IEnumerable<ItemMasterDTO> ObjCache = CacheHelper<IEnumerable<ItemMasterDTO>>.GetCacheItem("Cached_ItemMaster_" + CompanyId.ToString());
                        //if (ObjCache != null && ObjCache.Count() > 0)
                        //{
                        //    List<ItemMasterDTO> objTemp = ObjCache.ToList();
                        //    ItemMasterDTO itemDTO = objTemp.FirstOrDefault(i => i.AppendedBarcodeString.Contains(objBarcodeDTO.BarcodeString));
                        //    string[] ItemBarcodes = itemDTO.AppendedBarcodeString.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        //    objTemp.RemoveAll(i => i.ID == itemDTO.ID);
                        //    List<string> lstString = ItemBarcodes.ToList<string>();
                        //    lstString.RemoveAll(x => x == objBarcodeDTO.BarcodeString);

                        //    string s = string.Join(",", lstString.ToArray());
                        //    itemDTO.AppendedBarcodeString = s;
                        //    objTemp.Add(itemDTO);
                        //    ObjCache = objTemp.AsEnumerable();
                        //    CacheHelper<IEnumerable<ItemMasterDTO>>.AddCacheItem("Cached_ItemMaster_" + CompanyId.ToString(), ObjCache.AsEnumerable());
                        //}
                    }
                    else if (Convert.ToString(ModuleName).ToLower() == "kits")
                    {
                        //IEnumerable<KitMasterDTO> ObjCache = CacheHelper<IEnumerable<KitMasterDTO>>.GetCacheItem("Cached_KitMaster_" + CompanyId.ToString());
                        //if (ObjCache != null && ObjCache.Count() > 0)
                        //{
                            //List<KitMasterDTO> objTemp = ObjCache.ToList();
                            //KitMasterDTO itemDTO = objTemp.FirstOrDefault(i => i.AppendedBarcodeString.Contains(objBarcodeDTO.BarcodeString));
                            //string[] ItemBarcodes = itemDTO.AppendedBarcodeString.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            //objTemp.RemoveAll(i => i.ID == itemDTO.ID);
                            //List<string> lstString = ItemBarcodes.ToList<string>();
                            //lstString.RemoveAll(x => x == objBarcodeDTO.BarcodeString);

                            //string s = string.Join(",", lstString.ToArray());
                            //itemDTO.AppendedBarcodeString = s;
                            //objTemp.Add(itemDTO);
                            //CacheHelper<IEnumerable<KitMasterDTO>>.AddCacheItem("Cached_KitMaster_" + CompanyId.ToString(), objTemp.AsEnumerable());
                        //}
                    }
                    else if (Convert.ToString(ModuleName).ToLower() == "quick list permission")
                    {

                        //IEnumerable<QuickListMasterDTO> ObjCache = CacheHelper<IEnumerable<QuickListMasterDTO>>.GetCacheItem("Cached_QuickListMaster_" + CompanyId.ToString());
                        //if (ObjCache != null && ObjCache.Count() > 0)
                        //{
                            //List<QuickListMasterDTO> objTemp = ObjCache.ToList();
                            //QuickListMasterDTO itemDTO = objTemp.FirstOrDefault(i => i.AppendedBarcodeString.Contains(objBarcodeDTO.BarcodeString));
                            //string[] ItemBarcodes = itemDTO.AppendedBarcodeString.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            //objTemp.RemoveAll(i => i.ID == itemDTO.ID);
                            //List<string> lstString = ItemBarcodes.ToList<string>();
                            //lstString.RemoveAll(x => x == objBarcodeDTO.BarcodeString);
                            //string s = string.Join(",", lstString.ToArray());
                            //itemDTO.AppendedBarcodeString = s;
                            //objTemp.Add(itemDTO);

                            //CacheHelper<IEnumerable<QuickListMasterDTO>>.AppendToCacheItem("Cached_QuickListMaster_" + CompanyId.ToString(), objTemp.AsEnumerable<QuickListMasterDTO>());
                        //}

                    }
                    else if (Convert.ToString(ModuleName).ToLower() == "tool master")
                    {

                        //IEnumerable<ToolMasterDTO> ObjCache = CacheHelper<IEnumerable<ToolMasterDTO>>.GetCacheItem("Cached_ToolMaster_" + CompanyId.ToString());
                        //if (ObjCache != null && ObjCache.Count() > 0)
                        //{
                            ///List<ToolMasterDTO> objTemp = ObjCache.ToList();
                            //ToolMasterDTO itemDTO = objTemp.FirstOrDefault(i => i.AppendedBarcodeString.Contains(objBarcodeDTO.BarcodeString));
                            //string[] ItemBarcodes = itemDTO.AppendedBarcodeString.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            //objTemp.RemoveAll(i => i.ID == itemDTO.ID);
                            //List<string> lstString = ItemBarcodes.ToList<string>();
                            //lstString.RemoveAll(x => x == objBarcodeDTO.BarcodeString);
                            //string s = string.Join(",", lstString.ToArray());
                            //itemDTO.AppendedBarcodeString = s;
                            //objTemp.Add(itemDTO);

                            //CacheHelper<IEnumerable<ToolMasterDTO>>.AppendToCacheItem("Cached_ToolMaster_" + CompanyId.ToString(), objTemp.AsEnumerable<ToolMasterDTO>());
                        //}
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Get Coma seprated barcode string
        /// </summary>
        /// <param name="RefItemGuid"></param>
        /// <param name="ModuleName"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        public string GetComaSaperatedBarcodes(IEnumerable<BarcodeMasterDTO> BarcodeList, Guid? RefItemGuid, string ModuleName)
        {
            //if (RefItemGuid != null && !string.IsNullOrEmpty(ModuleName) && (ModuleName.ToLower().Equals("item master") || ModuleName.ToLower().Equals("kits")))
            if (RefItemGuid != null && !string.IsNullOrEmpty(ModuleName))
            {
                IEnumerable<BarcodeMasterDTO> lstBarcodeDTO = BarcodeList.Where(x => x.RefGUID == RefItemGuid);
                if (lstBarcodeDTO != null && lstBarcodeDTO.Count() > 0)
                {
                    string[] arrBarcode = (from x in lstBarcodeDTO
                                           select x.BarcodeString).ToArray<string>();
                    string barcodestring = string.Join(",", arrBarcode);
                    return barcodestring;
                }
            }
            return string.Empty;
        }

        public BarcodeMasterDTO GetBarcodeData(Guid ModuleGuid, string BarcodeValue, Int64 RoomId, Int64 CompanyID)
        {   
            BarcodeMasterDTO objBarcodeMasterDTO = new BarcodeMasterDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ModuleGuid", ModuleGuid),
                                                   new SqlParameter("@BarcodeValue", BarcodeValue),
                                                   new SqlParameter("@RoomID", RoomId),
                                                   new SqlParameter("@CompanyID", CompanyID)  };

                objBarcodeMasterDTO = (from x in context.Database.SqlQuery<BarcodeMasterDTO>("exec [GetBarcodeByModuleGUID]  @ModuleGuid,@BarcodeValue,@RoomID,@CompanyID", params1)
                                       select new BarcodeMasterDTO
                                       {
                                           ID = x.ID,
                                           CreatedOn = x.CreatedOn,
                                           UpdatedOn = x.UpdatedOn,
                                           CreatedBy = x.CreatedBy,
                                           UpdatedBy = x.UpdatedBy,
                                           IsDeleted = x.IsDeleted,
                                           GUID = x.GUID,
                                           UpdatedByName = string.Empty,
                                           Action = string.Empty,
                                           BarcodeString = x.BarcodeString,
                                           CompanyID = x.CompanyID,
                                           CreatedByName = string.Empty,
                                           HistoryID = 0,
                                           IsArchived = x.IsArchived,
                                           ModuleGUID = x.ModuleGUID,
                                           ModuleName = string.Empty,
                                           RefGUID = x.RefGUID,
                                           RefNumber = string.Empty,
                                           RoomID = x.RoomID,
                                           RoomName = x.RoomName,
                                           BinGuid = x.BinGuid
                                       }).FirstOrDefault();
            }

            return objBarcodeMasterDTO;
            //BarcodeMasterDTO objBarcode = new BarcodeMasterDTO();
            //IEnumerable<BarcodeMasterDTO> ListBarcode = null;
            //ListBarcode = GetAllRecords(RoomId, CompanyID);
            //if (ListBarcode != null)
            //{
            //    objBarcode = ListBarcode.ToList().Where(t => t.ModuleGUID == ModuleGuid && t.BarcodeString == BarcodeValue && t.IsDeleted == false).FirstOrDefault();
            //}
            //return objBarcode;
        }
        public List<BarcodeMasterDTO> GetBarcodeExport(long CompanyId, long RoomId)
        {
            List<BarcodeMasterDTO> lstBarcodeMastermain = new List<BarcodeMasterDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId), new SqlParameter("@CompanyID", CompanyId) };
                List<BarcodeMasterDTO> lstresult = context.Database.SqlQuery<BarcodeMasterDTO>("exec GetBarcodeExport @RoomID,@CompanyID", params1).ToList();
                lstBarcodeMastermain = (from im in lstresult
                                        select new BarcodeMasterDTO
                                        {
                                            items = im.items != null ? im.items : string.Empty,
                                            ModuleName = im.ModuleName != null ? im.ModuleName : string.Empty,
                                            BinNumber = im.BinNumber != null ? im.BinNumber.Replace("[|EmptyStagingBin|]", "") : string.Empty,
                                            ID = im.ID,
                                            BarcodeString = im.BarcodeString != null ? im.BarcodeString : string.Empty,
                                            GUID = im.GUID
                                        }).ToList();
            }
            return lstBarcodeMastermain;
        }
        public IEnumerable<BarcodeMasterDTO> GetBarcodeList(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string RoomDateFormat, Guid ModuleGuid, TimeZoneInfo CurrentTimeZone)
        {
            List<BarcodeMasterDTO> lstItems = new List<BarcodeMasterDTO>();
            TotalCount = 0;
            BarcodeMasterDTO objItemDTO = new BarcodeMasterDTO();

            string CreatedByName = null;
            string UpdatedByName = null;
            string RefGuid = null;
            string ModuleType = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string SupplierNumber = null;
            string Category = null;
            if (ModuleGuid != null)
                ModuleType = ModuleGuid.ToString();

            DataSet dsCart = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            //string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            if (Connectionstring == "")
            {
                return lstItems;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            sortColumnName = sortColumnName.Replace("CreatedDate", "CreatedOn").Replace("UpdatedDate", "UpdatedOn");
            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetBarcodeList", StartRowIndex, MaxRows, SearchTerm, sortColumnName, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, IsDeleted, IsArchived, RoomID, CompanyID, ModuleType, RefGuid, SupplierNumber, Category);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                if (Fields.Length > 2)
                {
                    if (!string.IsNullOrEmpty(Fields[2]))
                        SearchTerm = Fields[2];
                    else
                        SearchTerm = string.Empty;
                }
                else
                {
                    SearchTerm = string.Empty;
                }


                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    UpdatedByName = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    CreatedByName = FieldsPara[0].TrimEnd(',');
                }
                //if (!string.IsNullOrWhiteSpace(FieldsPara[81]))
                //{
                //    ModuleType = FieldsPara[81].TrimEnd(',');
                //}
                if (!string.IsNullOrWhiteSpace(FieldsPara[82]))
                {
                    RefGuid = FieldsPara[82].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[83]))
                {
                    SupplierNumber = FieldsPara[83].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[84]))
                {
                    Category = FieldsPara[84].TrimEnd(',');
                }
                if (ModuleGuid != null)
                    ModuleType = ModuleGuid.ToString();

                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetBarcodeList", StartRowIndex, MaxRows, SearchTerm, sortColumnName, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, IsDeleted, IsArchived, RoomID, CompanyID, ModuleType, RefGuid, SupplierNumber, Category);
            }
            else
            {
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetBarcodeList", StartRowIndex, MaxRows, SearchTerm, sortColumnName, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, IsDeleted, IsArchived, RoomID, CompanyID, ModuleType, RefGuid, SupplierNumber, Category);
            }
            if (dsCart != null && dsCart.Tables.Count > 0)
            {
                DataTable dtCart = dsCart.Tables[0];
                if (dtCart.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtCart.Rows[0]["TotalRecords"]);
                    lstItems = dtCart.AsEnumerable()
                    .Select(row => new BarcodeMasterDTO
                    {
                        ID = row.Field<long>("ID"),
                        items = row.Field<string>("Items"),
                        ModuleName = row.Field<string>("ModuleName"),
                        BinNumber = row.Field<string>("BinNumber"),
                        BarcodeAdded = row.Field<string>("BarcodeAdded"),
                        BarcodeString = row.Field<string>("BarcodeString"),
                        GUID = row.Field<Guid>("GUID"),
                        CreatedOn = row.Field<DateTime?>("CreatedOn").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("CreatedOn")) : Convert.ToDateTime(row.Field<DateTime?>("CreatedOn")),
                        UpdatedOn = row.Field<DateTime?>("UpdatedOn").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("UpdatedOn")) : Convert.ToDateTime(row.Field<DateTime?>("UpdatedOn")),
                        CreatedBy = row.Field<long>("CreatedBy"),
                        UpdatedBy = row.Field<long>("UpdatedBy"),
                        IsDeleted = (row.Field<bool?>("IsDeleted").HasValue ? row.Field<bool?>("IsDeleted") : false),
                        IsArchived = (row.Field<bool?>("IsArchived").HasValue ? row.Field<bool?>("IsArchived") : false),
                        CompanyID = row.Field<long>("CompanyID"),
                        RoomID = row.Field<long>("RoomID"),
                        CreatedByName = row.Field<string>("CreatedByName"),
                        UpdatedByName = row.Field<string>("UpdatedByName"),
                        RoomName = row.Field<string>("RoomName"),
                        ReceivedOn = row.Field<DateTime?>("ReceivedOn").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOn")) : row.Field<DateTime?>("ReceivedOn"),
                        ReceivedOnWeb = row.Field<DateTime?>("ReceivedOnWeb").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOnWeb")) : row.Field<DateTime?>("ReceivedOnWeb"),
                        AddedFrom = row.Field<string>("AddedFrom"),
                        EditedFrom = row.Field<string>("EditedFrom"),
                        Category = row.Field<string>("Category"),
                        Description = row.Field<string>("Description"),
                        SupplierName = row.Field<string>("SupplierName"),
                        SupplierNumber = row.Field<string>("SupplierNumber"),
                        manufacturername = row.Field<string>("manufacturername"),
                        ManufacturerNumber = row.Field<string>("ManufacturerNumber"),
                    }).ToList();
                }
            }
            return lstItems;
        }
        public string DuplicateCheck(Int64 barCodeId, string NewBarcodeString, Int64 RoomId, Int64 CompanyId)
        {
            string msg = "";
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var qry = (from em in context.BarcodeMasters
                           where em.BarcodeString == NewBarcodeString && (em.IsArchived ?? false) == false && (em.IsDeleted ?? false) == false && em.ID != barCodeId
                           && em.RoomID == RoomId && em.CompanyID == CompanyId
                           select em);
                if (qry.Any())
                {
                    msg = "duplicate";
                }
                else
                {
                    msg = "ok";
                }
            }
            return msg;
        }

        public string GetBarcodeStatus(string IDs, string deleted,long EnterPriseId,long CompanyId,long RoomId,long UserId)
        {
            DataSet ds = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            ds = SqlHelper.ExecuteDataset(EturnsConnection, "GetBarcodeStatus", IDs, deleted);
            DataTable dt = new DataTable();
            string msg = string.Empty;
            int Failcnt = 0;
            int Successcnt = 0;
            if (ds.Tables.Count > 0)
            {
                string currentCulture = "en-US";
                if (HttpContext.Current != null)
                {
                    if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                    {
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                        {
                            currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                        }

                    }
                }
                else
                {
                    eTurns.DTO.eTurnsRegionInfo objeTurnsRegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomId, CompanyId, UserId);
                    currentCulture = objeTurnsRegionInfo.CultureName;
                }
                string ResourceFileCommonBarcode = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResBarcodeMaster", currentCulture, EnterPriseId, CompanyId);
                dt = ds.Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["Status"].ToString() == "Success")
                    {
                        Successcnt += 1;
                    }
                    else if (dr["Status"].ToString() == "Fail")
                    {
                        Failcnt += 1;
                    }
                }
                if (Successcnt > 0)
                {
                    //msg = Successcnt + " record(s) deleted successfully.";
                    string MsgBarCodeDeleteUndeleteSuccess = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgBarCodeDeleteUndeleteSuccess", ResourceFileCommonBarcode,EnterPriseId,CompanyId,RoomId, "ResBarcodeMaster",currentCulture);
                    msg = Successcnt + MsgBarCodeDeleteUndeleteSuccess.Replace("{0}",deleted);
                }
                if (Failcnt > 0)
                {
                    string ResourceFileCommon = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResCommon", currentCulture, EnterPriseId, CompanyId);
                    string MsgRecordsUsedInOtherModule = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgRecordsUsedInOtherModule", ResourceFileCommon,EnterPriseId, CompanyId, RoomId, "ResBarcodeMaster", currentCulture);
                    
                    if (string.IsNullOrEmpty(msg))
                    {
                        msg = Failcnt + " " + MsgRecordsUsedInOtherModule;
                    }
                    else
                    {
                        msg = msg + " " + Failcnt + " "+ MsgRecordsUsedInOtherModule;
                    }
                }
            }
            return msg;

        }

        public IEnumerable<BarcodeMasterDTO> GetBarcodeByBarcodeStringRefGUID(Int64 RoomID, Int64 CompanyID, string BarcodeString, string RefGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), 
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@BarcodeString", BarcodeString),
                                                   new SqlParameter("@RefGUID", RefGUID) };

                return (from x in context.Database.SqlQuery<BarcodeMasterDTO>("exec [GetBarcodeByBarcodeStringRef] @RoomID,@CompanyID,@BarcodeString,@RefGUID", params1)
                        select new BarcodeMasterDTO
                        {
                            ID = x.ID,
                            CreatedOn = x.CreatedOn,
                            UpdatedOn = x.UpdatedOn,
                            CreatedBy = x.CreatedBy,
                            UpdatedBy = x.UpdatedBy,
                            IsDeleted = x.IsDeleted,
                            GUID = x.GUID,
                            UpdatedByName = x.UpdatedByName,
                            Action = string.Empty,
                            BarcodeString = x.BarcodeString,
                            CompanyID = x.CompanyID,
                            CreatedByName = x.CreatedByName,
                            HistoryID = 0,
                            IsArchived = x.IsArchived,
                            ModuleGUID = x.ModuleGUID,
                            ModuleName = x.ModuleName,
                            RefGUID = x.RefGUID,
                            RefNumber = string.Empty,
                            RoomID = x.RoomID,
                            RoomName = x.RoomName,
                            BarcodeAdded = x.BarcodeAdded,

                        }).ToList();

            }
        }

        public BarcodeMasterDTO GetBarcodeMasterByID(Int64 ID ,Int64 RoomID, Int64 CompanyID, bool Isdeleted, bool IsArchieved)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),  
                                                   new SqlParameter("@CompanyID", CompanyID), 
                                                   new SqlParameter("@Isdeleted", Isdeleted), 
                                                   new SqlParameter("@IsArchieved", IsArchieved),
                                                   new SqlParameter("@ID", ID)  };

                return (from x in context.Database.SqlQuery<BarcodeMasterDTO>("exec [BarcodeNarrowSearchByID]  @RoomID,@CompanyID,@Isdeleted,@IsArchieved,@ID", params1)
                        select new BarcodeMasterDTO
                        {
                            ID = x.ID,
                            CreatedOn = x.CreatedOn,
                            UpdatedOn = x.UpdatedOn,
                            CreatedBy = x.CreatedBy,
                            UpdatedBy = x.UpdatedBy,
                            IsDeleted = x.IsDeleted,
                            GUID = x.GUID,
                            UpdatedByName = x.UpdatedByName,
                            Action = string.Empty,
                            BarcodeString = x.BarcodeString,
                            CompanyID = x.CompanyID,
                            CreatedByName = x.CreatedByName,
                            HistoryID = 0,
                            IsArchived = x.IsArchived,
                            ModuleGUID = x.ModuleGUID,
                            ModuleName = x.ModuleName,
                            RefGUID = x.RefGUID,
                            RefNumber = string.Empty,
                            RoomID = x.RoomID,
                            RoomName = x.RoomName,
                            BarcodeAdded = x.BarcodeAdded,
                            BinGuid = x.BinGuid,
                            BinNumber = x.BinNumber,
                            items = x.items,
                            manufacturername = x.manufacturername,
                            ManufacturerNumber = x.ManufacturerNumber,
                            SupplierName = x.SupplierName,
                            SupplierNumber = x.SupplierNumber,
                            OldBarcodeString = x.BarcodeString,
                        }).FirstOrDefault();

            }
        }
        public void GenerateBarcodeForRoom(Int64 RoomId, string ModuleName,string AddedFrom)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomId)
                                                    ,new SqlParameter("@ModuleName", ModuleName)
                                                     ,new SqlParameter("@AddedFrom", AddedFrom)};
                context.Database.ExecuteSqlCommand("exec [GenerateBarcodeForRoom] @RoomId,@ModuleName,@AddedFrom", params1);

            }
        }

    }
}
