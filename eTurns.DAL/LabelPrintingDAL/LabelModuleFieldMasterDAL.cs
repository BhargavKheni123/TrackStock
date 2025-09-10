using eTurns.DTO.LabelPrinting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;

namespace eTurns.DAL.LabelPrintingDAL
{
    public class LabelModuleFieldMasterDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        //public LabelModuleFieldMasterDAL(base.DataBaseName)
        //{

        //}

        public LabelModuleFieldMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public LabelModuleFieldMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
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
        private IEnumerable<LabelModuleFieldMasterDTO> GetLabelModuleFieldMasterByModuleID(Int64 ModuleID)
        {
            List<LabelModuleFieldMasterDTO> lstLblModuleFieldMaster = new List<LabelModuleFieldMasterDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ModuleID", ModuleID) };

                lstLblModuleFieldMaster = (from u in context.Database.SqlQuery<LabelModuleFieldMasterDTO>("EXEC GetLabelModuleFieldMasterByModuleID @ModuleID", params1)
                                                              select new LabelModuleFieldMasterDTO
                                                              {
                                                                  ID = u.ID,
                                                                  FieldName = u.FieldName,
                                                                  FieldDisplayName = u.FieldDisplayName,
                                                                  IncludeInBarcode = u.IncludeInBarcode,
                                                                  ModuleID = u.ModuleID,
                                                                  FieldDisplayOrder = u.FieldDisplayOrder,
                                                              }).AsParallel().ToList();
                
            }
            return lstLblModuleFieldMaster;
        }

        /// <summary>
        /// Get FieldDisplayName From Resource
        /// And UDF From UDF Table Name column Name field
        /// </summary>
        /// <param name="FieldName"></param>
        /// <param name="ModuleID"></param>
        /// <returns></returns>
        public string GetFieldDisplayNameFromResource(string FieldName, Int64 ModuleID, string ResourceFileName, bool LoadRoomLevelResource)
        {
            string ReturnString = FieldName;
            if (!FieldName.Contains("UDF"))
            {
                ReturnString = eTurns.DTO.Resources.ResourceHelper.GetResourceValue("Field_" + FieldName, "ResLabelPrintingFields", false);
                if (ReturnString == "Field_" + FieldName)
                    ReturnString = FieldName;
            }
            else if (!string.IsNullOrEmpty(ResourceFileName))
            {
                ReturnString = eTurns.DTO.Resources.ResourceHelper.GetResourceValue(FieldName, ResourceFileName, LoadRoomLevelResource);
            }
            return ReturnString;
        }

        /// <summary>
        /// Get Paged Records from the LabelModuleFieldMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<LabelModuleFieldMasterDTO> GetRecordsModueWise(Int64 ModuleID, Int64 CompanyID, bool LoadRoomLevelResource)
        {
            //IEnumerable<LabelModuleFieldMasterDTO> lstFields = GetCachedData().Where(x => x.ModuleID == ModuleID);
            IEnumerable<LabelModuleFieldMasterDTO> lstFields = GetLabelModuleFieldMasterByModuleID(ModuleID);
            string stringResourceFile = "";
            switch (ModuleID)
            {
                case 1:
                    stringResourceFile = "ResAssetMaster";
                    break;
                case 2:
                    stringResourceFile = "ResItemMaster";
                    break;
                case 3:
                    stringResourceFile = "ResItemMaster";
                    break;
                case 4:
                    stringResourceFile = "ResOrder";
                    break;
                case 5:
                    stringResourceFile = "ResQuickList";
                    break;
                case 6:
                    stringResourceFile = "ResItemMaster";
                    break;
                case 7:
                    stringResourceFile = "ResMaterialStaging";
                    break;
                case 8:
                    stringResourceFile = "ResToolMaster";
                    break;
                case 9:
                    stringResourceFile = "ResTransfer";
                    break;
                case 10:
                    stringResourceFile = "ResTechnician";
                    break;
                default:
                    break;
            }


            foreach (var item in lstFields)
            {
                item.FieldDisplayName = GetFieldDisplayNameFromResource(item.FieldName, item.ModuleID, stringResourceFile, LoadRoomLevelResource);
            }
            return lstFields;
        }

        /// <summary>
        /// Insert Record in the DataBase LabelModuleFieldMaster
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        //public Int64 Insert(LabelModuleFieldMasterDTO objDTO)
        //{
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        LabelModuleFieldMaster obj = new LabelModuleFieldMaster();
        //        obj.ID = 0;
        //        obj.FieldName = objDTO.FieldName;
        //        obj.FieldDisplayName = objDTO.FieldDisplayName;
        //        obj.IncludeInBarcode = objDTO.IncludeInBarcode;
        //        obj.ModuleID = objDTO.ModuleID;
        //        obj.FieldDisplayOrder = objDTO.FieldDisplayOrder;

        //        context.LabelModuleFieldMasters.Add(obj);
        //        context.SaveChanges();
        //        objDTO.ID = obj.ID;


        //        if (objDTO.ID > 0)
        //        {
        //            //Get Cached-Media
        //            IEnumerable<LabelModuleFieldMasterDTO> ObjCache = CacheHelper<IEnumerable<LabelModuleFieldMasterDTO>>.GetCacheItem("Cached_LabelModuleFieldMaster");
        //            if (ObjCache != null)
        //            {
        //                List<LabelModuleFieldMasterDTO> tempC = new List<LabelModuleFieldMasterDTO>();
        //                tempC.Add(objDTO);

        //                IEnumerable<LabelModuleFieldMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
        //                CacheHelper<IEnumerable<LabelModuleFieldMasterDTO>>.AppendToCacheItem("Cached_LabelModuleFieldMaster", NewCache);
        //            }
        //        }

        //        return obj.ID;
        //    }

        //}

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        //public bool Edit(LabelModuleFieldMasterDTO objDTO)
        //{
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        LabelModuleFieldMaster obj = new LabelModuleFieldMaster();
        //        obj.ID = objDTO.ID;
        //        obj.FieldName = objDTO.FieldName;
        //        obj.FieldDisplayName = objDTO.FieldDisplayName;
        //        obj.IncludeInBarcode = objDTO.IncludeInBarcode;
        //        obj.ModuleID = objDTO.ModuleID;
        //        obj.FieldDisplayOrder = objDTO.FieldDisplayOrder;
        //        context.LabelModuleFieldMasters.Attach(obj);
        //        context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
        //        context.SaveChanges();

        //        //Get Cached-Media
        //        IEnumerable<LabelModuleFieldMasterDTO> ObjCache = CacheHelper<IEnumerable<LabelModuleFieldMasterDTO>>.GetCacheItem("Cached_LabelModuleFieldMaster");
        //        if (ObjCache != null)
        //        {
        //            List<LabelModuleFieldMasterDTO> objTemp = ObjCache.ToList();
        //            objTemp.RemoveAll(i => i.ID == objDTO.ID);
        //            ObjCache = objTemp.AsEnumerable();

        //            List<LabelModuleFieldMasterDTO> tempC = new List<LabelModuleFieldMasterDTO>();
        //            tempC.Add(objDTO);
        //            IEnumerable<LabelModuleFieldMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
        //            CacheHelper<IEnumerable<LabelModuleFieldMasterDTO>>.AppendToCacheItem("Cached_LabelModuleFieldMaster", NewCache);
        //        }


        //        return true;
        //    }
        //}

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        //public bool DeleteRecords(string IDs)
        //{
        //    using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        string strQuery = "";
        //        foreach (var item in IDs.Split(','))
        //        {
        //            if (!string.IsNullOrEmpty(item.Trim()))
        //            {
        //                strQuery += "Delete From LabelModuleFieldMaster  WHERE ID =" + item.ToString() + ";";
        //            }
        //        }
        //        context.Database.ExecuteSqlCommand(strQuery);


        //        //Get Cached-Media
        //        IEnumerable<LabelModuleFieldMasterDTO> ObjCache = CacheHelper<IEnumerable<LabelModuleFieldMasterDTO>>.GetCacheItem("Cached_LabelModuleFieldMaster");
        //        if (ObjCache != null)
        //        {
        //            List<LabelModuleFieldMasterDTO> objTemp = ObjCache.ToList();
        //            objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
        //            ObjCache = objTemp.AsEnumerable();
        //            CacheHelper<IEnumerable<LabelModuleFieldMasterDTO>>.AppendToCacheItem("Cached_LabelModuleFieldMaster", ObjCache);
        //        }

        //        return true;
        //    }
        //}

        /// <summary>
        /// Get Paged Records from the LabelModuleFieldMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        //public IEnumerable<LabelModuleFieldMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        //{
        //    //Get Cached-Media
        //    IEnumerable<LabelModuleFieldMasterDTO> ObjCache = GetCachedData();


        //    if (String.IsNullOrEmpty(SearchTerm))
        //    {

        //        TotalCount = ObjCache.Count();
        //        return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        //    }
        //    else
        //    {
        //        TotalCount = ObjCache.Where
        //           (
        //               t => t.ID.ToString().Contains(SearchTerm) ||
        //               (t.FieldDisplayName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //               (t.FieldName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
        //           ).Count();
        //        return ObjCache.Where
        //            (t => t.ID.ToString().Contains(SearchTerm) ||
        //                (t.FieldDisplayName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.FieldName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
        //            ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

        //    }
        //}

        /// <summary>
        /// Get Paged Records from the LabelModuleFieldMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        //public IEnumerable<LabelModuleFieldMasterDTO> GetAllRecords()
        //{
        //    return GetCachedData();
        //}

        /// <summary>
        /// Get Particullar Record from the LabelModuleFieldMaster by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        //public LabelModuleFieldMasterDTO GetRecord(Int64 id)
        //{
        //    return GetCachedData().Single(t => t.ID == id);
        //}

    }
}


