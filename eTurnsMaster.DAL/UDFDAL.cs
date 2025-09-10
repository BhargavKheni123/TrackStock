using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Data.SqlClient;

namespace eTurnsMaster.DAL
{
    public partial class UDFDAL : eTurnsMasterBaseDAL
    {

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UDFDTO> GetCachedData()
        {
            //Get Cached-Media
            IEnumerable<UDFDTO> ObjCache = CacheHelper<IEnumerable<UDFDTO>>.GetCacheItem("Cached_UDF_Master");
            if (ObjCache == null)
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<UDFDTO> obj = (from u in context.UDFs
                                               join cr in context.UserMasters on u.CreatedBy equals cr.ID into u_cr_join
                                               from u_cr in u_cr_join.DefaultIfEmpty()
                                               join up in context.UserMasters on u.LastUpdatedBy equals up.ID into u_up_join
                                               from u_up in u_up_join.DefaultIfEmpty()
                                               select new UDFDTO
                                               {
                                                   EnterpriseId = u.EnterpriseId ?? 0,
                                                   ID = u.ID,
                                                   CompanyID = 0,
                                                   Room = 0,
                                                   UDFTableName = u.UDFTableName,
                                                   UDFColumnName = u.UDFColumnName,
                                                   UDFControlType = u.UDFControlType,
                                                   UDFDefaultValue = u.UDFDefaultValue,
                                                   UDFOptionsCSV = u.UDFOptionsCSV,
                                                   UDFIsRequired = u.UDFIsRequired,
                                                   UDFIsSearchable = u.UDFIsSearchable,
                                                   Created = u.Created ?? DateTime.MinValue,
                                                   Updated = u.Updated,
                                                   CreatedBy = u.CreatedBy,
                                                   LastUpdatedBy = u.LastUpdatedBy,
                                                   CreatedByName = u_cr.UserName,
                                                   UpdatedByName = u_up.UserName,
                                                   IsDeleted = u.IsDeleted,
                                                   UDFMaxLength = u.UDFMaxLength

                                               }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<UDFDTO>>.AddCacheItem("Cached_UDF_Master", obj);
                }
            }

            return ObjCache;
        }
        
        /// <summary>
        /// GetAllRecords
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UDFDTO> GetAllRecords(string UDFTableName)
        {
            return GetCachedData().Where(t => (t.UDFTableName ?? string.Empty).Trim().ToLower() == (UDFTableName ?? string.Empty).Trim().ToLower()).OrderBy("UDFColumnName ASC");
        }

        public IEnumerable<UDFDTO> GetAllRecords(string UDFTableName, long ID)
        {
            return GetCachedData().Where(t => t.UDFTableName == UDFTableName && t.EnterpriseId == ID).OrderBy("UDFColumnName ASC");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="StartRowIndex"></param>
        /// <param name="MaxRows"></param>
        /// <param name="TotalCount"></param>
        /// <param name="SearchTerm"></param>
        /// <param name="sortColumnName"></param>
        /// <param name="CompanyID"></param>
        /// <param name="UDFTableName"></param>
        /// <returns></returns>
        public IEnumerable<UDFDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 CompanyID, string UDFTableName, long EnterpriseID)
        {
            if (String.IsNullOrEmpty(SearchTerm))
            {
                //Get Cached-Media
                IEnumerable<UDFDTO> ObjCache = GetCachedData().Where(t => t.UDFTableName == UDFTableName && t.EnterpriseId == EnterpriseID);
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                IEnumerable<UDFDTO> ObjCache = GetCachedData();

                string[] stringSeparators = new string[] { "[###]" };
                string[] dd = SearchTerm.Replace("STARTWITH#", "$").Split(stringSeparators, StringSplitOptions.None);
                string searchStr = string.Empty;


                if (dd != null && dd.Length > 0)
                {
                    string[] Fields = dd[1].Split(stringSeparators, StringSplitOptions.None);
                    // 6 counts for fields based on that prepare the search string
                    // 1 = createdBy, 2 = updatedby , 3 = CreatedFrom, 4 = CreatedTo, 5=UpdatedFrom,6=UpdatedTo
                    foreach (var item in Fields)
                    {
                        if (item.Length > 0)
                        {
                            if (item.Contains("CreatedBy"))
                            {
                                ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.CreatedByName.ToString()));
                            }
                            else if (item.Contains("UpdatedBy"))
                            {
                                ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UpdatedByName.ToString()));
                            }
                            else if (item.Contains("DateCreatedFrom"))
                            {
                                ObjCache = ObjCache.Where(t => t.Created.Date >= Convert.ToDateTime(item.Split('#')[1]).Date && t.Created.Date <= Convert.ToDateTime(item.Split('#')[3]).Date);
                            }
                            else if (item.Contains("DateUpdatedFrom"))
                            {
                                ObjCache = ObjCache.Where(t => t.Updated.Value.Date >= Convert.ToDateTime(item.Split('#')[1]).Date && t.Updated.Value.Date <= Convert.ToDateTime(item.Split('#')[3]).Date);
                            }
                        }
                    }
                }
                
                if (dd.Length > 2)
                {
                    if (!string.IsNullOrEmpty(dd[2]))
                        searchStr = dd[2];
                    else
                        searchStr = string.Empty;
                }
                if(searchStr != string.Empty)
                {
                    TotalCount = ObjCache.Where(t => t.UDFTableName == UDFTableName && t.EnterpriseId == EnterpriseID && ((t.UDFColumnName != null && t.UDFColumnName.ToLower().Contains(searchStr.ToLower())) || (t.UDFDisplayColumnName != null && t.UDFDisplayColumnName.ToLower().Contains(searchStr.ToLower())))).Count();
                    return ObjCache.Where(t => t.UDFTableName == UDFTableName && t.EnterpriseId == EnterpriseID && ((t.UDFColumnName != null && t.UDFColumnName.ToLower().Contains(searchStr.ToLower())) || (t.UDFDisplayColumnName != null && t.UDFDisplayColumnName.ToLower().Contains(searchStr.ToLower())))).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
                }
                else
                {
                    TotalCount = ObjCache.Where(t => t.UDFTableName == UDFTableName && t.EnterpriseId == EnterpriseID).Count();
                    return ObjCache.Where(t => t.UDFTableName == UDFTableName && t.EnterpriseId == EnterpriseID).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
                }
    
            }
            else
            {
                //Get Cached-Media
                IEnumerable<UDFDTO> ObjCache = GetCachedData();
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        t.UDFColumnName.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0).Count();
                return ObjCache.Where(t => t.UDFTableName == UDFTableName).Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        t.UDFColumnName.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }

        /// <summary>
        /// Get UDF Record By ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public UDFDTO GetRecord(Int64 ID, Int64 CompanyID)
        {
            return GetCachedData().Single(t => t.ID == ID);
        }

        /// <summary>
        /// Insert UDF Record
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(UDFDTO objDTO)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                UDF obj = new UDF();
                obj.ID = 0;

                obj.UDFTableName = objDTO.UDFTableName;
                obj.UDFColumnName = objDTO.UDFColumnName;
                obj.UDFControlType = objDTO.UDFControlType;
                obj.UDFDefaultValue = objDTO.UDFDefaultValue;
                obj.UDFOptionsCSV = objDTO.UDFOptionsCSV;
                obj.UDFIsRequired = objDTO.UDFIsRequired;
                obj.UDFIsSearchable = objDTO.UDFIsSearchable;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Updated = DateTime.UtcNow;
                obj.Created = DateTime.UtcNow;
                obj.IsDeleted = false;
                obj.UDFMaxLength = objDTO.UDFMaxLength ?? 200;
                obj.GUID = Guid.NewGuid();
                context.UDFs.Add(obj);
                context.SaveChanges();

                objDTO.ID = obj.ID;
                if (objDTO.ID > 0)
                {
                    //Get Cached-Media
                    IEnumerable<UDFDTO> ObjCache = CacheHelper<IEnumerable<UDFDTO>>.GetCacheItem("Cached_UDF_Master");
                    if (ObjCache != null)
                    {
                        List<UDFDTO> tempC = new List<UDFDTO>();
                        tempC.Add(objDTO);

                        IEnumerable<UDFDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                        CacheHelper<IEnumerable<UDFDTO>>.AppendToCacheItem("Cached_UDF_Master", NewCache);
                    }
                }


                return obj.ID;
            }
        }
        
        public Int64 InserteTurns(UDFDTO objDTO)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                UDF obj = new UDF();
                obj.ID = 0;

                obj.UDFTableName = objDTO.UDFTableName;
                obj.UDFColumnName = objDTO.UDFColumnName;
                obj.UDFControlType = objDTO.UDFControlType;
                obj.UDFDefaultValue = objDTO.UDFDefaultValue;
                obj.UDFOptionsCSV = objDTO.UDFOptionsCSV;
                obj.UDFIsRequired = objDTO.UDFIsRequired;
                obj.UDFIsSearchable = objDTO.UDFIsSearchable;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Updated = DateTime.UtcNow;
                obj.Created = DateTime.UtcNow;
                obj.IsDeleted = false;
                obj.GUID = Guid.NewGuid();
                obj.UDFMaxLength = objDTO.UDFMaxLength ?? 200;
                context.UDFs.Add(obj);
                context.SaveChanges();

                objDTO.ID = obj.ID;
                if (objDTO.ID > 0)
                {
                    //Get Cached-Media
                    IEnumerable<UDFDTO> ObjCache = CacheHelper<IEnumerable<UDFDTO>>.GetCacheItem("Cached_UDF_Master");
                    if (ObjCache != null)
                    {
                        List<UDFDTO> tempC = new List<UDFDTO>();
                        tempC.Add(objDTO);

                        IEnumerable<UDFDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                        CacheHelper<IEnumerable<UDFDTO>>.AppendToCacheItem("Cached_UDF_Master", NewCache);
                    }
                }


                return obj.ID;
            }
        }

        /// <summary>
        /// Update UDF Record By ID
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(UDFDTO objDTO)
        {
            objDTO.Updated = DateTime.UtcNow;

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                UDF obj = new UDF();
                obj.ID = objDTO.ID;
                obj.UDFTableName = objDTO.UDFTableName;
                obj.UDFColumnName = objDTO.UDFColumnName;
                obj.UDFControlType = objDTO.UDFControlType;
                obj.UDFDefaultValue = objDTO.UDFDefaultValue;
                obj.UDFOptionsCSV = objDTO.UDFOptionsCSV;
                obj.UDFIsRequired = objDTO.UDFIsRequired;
                obj.UDFIsSearchable = objDTO.UDFIsSearchable;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Updated = objDTO.Updated;
                obj.Created = objDTO.Created;
                obj.IsDeleted = objDTO.IsDeleted;
                obj.EnterpriseId = objDTO.EnterpriseId;
                obj.GUID = Guid.NewGuid();
                obj.UDFMaxLength = objDTO.UDFMaxLength ?? 200;
                context.UDFs.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                if (obj != null && obj.IsDeleted)
                    Delete(obj.ID, obj.CreatedBy.GetValueOrDefault(0));


                //Get Cached-Media
                IEnumerable<UDFDTO> ObjCache = CacheHelper<IEnumerable<UDFDTO>>.GetCacheItem("Cached_UDF_Master");
                if (ObjCache != null)
                {
                    List<UDFDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == objDTO.ID);
                    ObjCache = objTemp.AsEnumerable();

                    List<UDFDTO> tempC = new List<UDFDTO>();
                    tempC.Add(objDTO);
                    IEnumerable<UDFDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    CacheHelper<IEnumerable<UDFDTO>>.AppendToCacheItem("Cached_UDF_Master", NewCache);
                }

                return true;
            }
        }


        /// <summary>
        /// Delete Tool Record By ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool Delete(Int64 Udfid, Int64 userid)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                UDF obj = context.UDFs.Single(t => t.ID == Udfid);
                obj.IsDeleted = true;
                obj.Updated = DateTime.UtcNow;
                obj.LastUpdatedBy = userid;
                context.Entry(obj).State = System.Data.Entity.EntityState.Unchanged;
                context.UDFs.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                //Set UDFOptions deleted on UDF delete
                var DeleteRMDParam = new SqlParameter[] { new SqlParameter("@UserID", userid), new SqlParameter("@UDFID", Udfid) };
                context.Database.ExecuteSqlCommand("exec [DeleteUDFOptionsByUDFIds] @UserID,@UDFID", DeleteRMDParam);
                //string sSQL = "UPDATE UDFOptions SET Updated = '" + DateTime.UtcNow.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE UDFID =" + Udfid.ToString() + ";";
                //context.Database.ExecuteSqlCommand(sSQL);

                //Get Cached-Media
                IEnumerable<UDFDTO> ObjCache = CacheHelper<IEnumerable<UDFDTO>>.GetCacheItem("Cached_UDF_Master");
                if (ObjCache != null)
                {
                    List<UDFDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == Udfid);
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<UDFDTO>>.AppendToCacheItem("Cached_UDF_Master", ObjCache);
                }


                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UDFTableName"></param>
        /// <param name="CompanyID"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public bool InsertDefaultUDFs(string UDFTableName, long UserID, long EnterpriseId,int iUDFMaxLength = 200)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var eTurnsDBName = MasterDbConnectionHelper.GeteTurnsDBName();
                var SqlParam = new SqlParameter[] {       
                                                          new SqlParameter("@UDFTableName", UDFTableName), 
                                                          new SqlParameter("@EnterpriseId", EnterpriseId) ,
                                                          new SqlParameter("@UserID", UserID),
                                                          new SqlParameter("@UDFMaxLength", iUDFMaxLength),
                                                          new SqlParameter("@eTurnsDBName", eTurnsDBName)                                                          
                                                   };
                var iResult = context.Database.ExecuteSqlCommand("exec [InsertDefaultUDFs] @UDFTableName,@EnterpriseId,@UserID,@UDFMaxLength,@eTurnsDBName", SqlParam);

                if (iResult > 0)
                {
                    //Get Cached-Media
                    IEnumerable<UDFDTO> ObjCache = CacheHelper<IEnumerable<UDFDTO>>.GetCacheItem("Cached_UDF_Master");
                    if (ObjCache != null)
                    {
                        IEnumerable<UDFDTO> obj = (from u in context.UDFs
                                                   join cr in context.UserMasters on u.CreatedBy equals cr.ID into u_cr_join
                                                   from u_cr in u_cr_join.DefaultIfEmpty()
                                                   join up in context.UserMasters on u.LastUpdatedBy equals up.ID into u_up_join
                                                   from u_up in u_up_join.DefaultIfEmpty()
                                                   select new UDFDTO
                                                   {
                                                       EnterpriseId = u.EnterpriseId ?? 0,
                                                       ID = u.ID,
                                                       CompanyID = 0,
                                                       Room = 0,
                                                       UDFTableName = u.UDFTableName,
                                                       UDFColumnName = u.UDFColumnName,
                                                       UDFControlType = u.UDFControlType,
                                                       UDFDefaultValue = u.UDFDefaultValue,
                                                       UDFOptionsCSV = u.UDFOptionsCSV,
                                                       UDFIsRequired = u.UDFIsRequired,
                                                       UDFIsSearchable = u.UDFIsSearchable,
                                                       Created = u.Created ?? DateTime.MinValue,
                                                       Updated = u.Updated,
                                                       CreatedBy = u.CreatedBy,
                                                       LastUpdatedBy = u.LastUpdatedBy,
                                                       CreatedByName = u_cr.UserName,
                                                       UpdatedByName = u_up.UserName,
                                                       IsDeleted = u.IsDeleted,
                                                       UDFMaxLength = u.UDFMaxLength
                                                   }).AsParallel().ToList(); ;
                        ObjCache = CacheHelper<IEnumerable<UDFDTO>>.AddCacheItem("Cached_UDF_Master", obj);
                    }
                }

                return true;
            }
        }
    }

    public class UDFOptionDAL : eTurnsMasterBaseDAL
    {
        public UDFOptionsDTO GetUDFOptionByIDPlain(long OptionID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@OptionID", OptionID) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UDFOptionsDTO>("exec [GetUDFOptionByIDPlain] @OptionID", params1).FirstOrDefault();
            }
        }

        /// <summary>
        /// Get All Records By UDFID
        /// </summary>
        /// <param name="UDFID"></param>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        public IEnumerable<UDFOptionsDTO> GetUDFOptionsByUDFIDPlain(long UdfId)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UdfId", UdfId) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UDFOptionsDTO>("exec [GetUDFOptionsByUDFIDPlain] @UdfId", params1).ToList();
            }
        }

        /// <summary>
        /// Insert Record By ID
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public Int64 Insert(UDFOptionsDTO objDTO)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                UDFOption obj = new UDFOption();
                obj.ID = 0;
                obj.UDFID = objDTO.UDFID;
                obj.UDFOption1 = objDTO.UDFOption;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Updated = DateTime.UtcNow;
                obj.Created = DateTime.UtcNow;
                obj.IsDeleted = false;
                obj.GUID = Guid.NewGuid();
                //obj.CompanyID = objDTO.CompanyID;
                context.UDFOptions.Add(obj);
                context.SaveChanges();

                objDTO.ID = obj.ID;
                if (objDTO.ID > 0)
                {
                    //Get Cached-Media
                    IEnumerable<UDFOptionsDTO> ObjCache = CacheHelper<IEnumerable<UDFOptionsDTO>>.GetCacheItem("Cached_UDFOptions_"); // + objDTO.CompanyID.ToString());
                    if (ObjCache != null)
                    {
                        List<UDFOptionsDTO> tempC = new List<UDFOptionsDTO>();
                        tempC.Add(objDTO);

                        IEnumerable<UDFOptionsDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                        CacheHelper<IEnumerable<UDFOptionsDTO>>.AppendToCacheItem("Cached_UDFOptions_", NewCache);// + objDTO.CompanyID.ToString(), NewCache);
                    }
                }

                return obj.ID;
            }
        }

        /// <summary>
        /// Update Record By ID
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(UDFOptionsDTO objDTO)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                UDFOption obj = new UDFOption();
                obj.ID = objDTO.ID;
                obj.UDFID = objDTO.UDFID;
                obj.UDFOption1 = objDTO.UDFOption;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Updated = DateTime.UtcNow;
                obj.Created = objDTO.Created;
                obj.IsDeleted = false;
                obj.GUID = objDTO.GUID;
                context.UDFOptions.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();


                //Get Cached-Media
                IEnumerable<UDFOptionsDTO> ObjCache = CacheHelper<IEnumerable<UDFOptionsDTO>>.GetCacheItem("Cached_UDFOptions_"); // + objDTO.CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<UDFOptionsDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == objDTO.ID);
                    ObjCache = objTemp.AsEnumerable();

                    List<UDFOptionsDTO> tempC = new List<UDFOptionsDTO>();
                    tempC.Add(objDTO);
                    IEnumerable<UDFOptionsDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    CacheHelper<IEnumerable<UDFOptionsDTO>>.AppendToCacheItem("Cached_UDFOptions_", NewCache); // + objDTO.CompanyID.ToString(), NewCache);
                }

                return true;
            }
        }

        public bool Edit(Int64 ID, string UDFOption, Int64 UserID, Int64 CompanyID)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                UDFDAL objUDFDAL = new UDFDAL();
                UDFOptionDAL objUDFOptionsDAL = new UDFOptionDAL();

                //Update udf options
                UDFOptionsDTO objDTO = GetUDFOptionByIDPlain(ID);
                objDTO.UDFOption = UDFOption;
                objDTO.Updated = DateTime.UtcNow;

                objDTO.LastUpdatedBy = UserID;
                objUDFOptionsDAL.Edit(objDTO);

                ////update udf it self
                //UDFDTO objUDFDTO = objUDFDAL.GetRecord(objDTO.UDFID, CompanyID);
                //objUDFDTO.UDFDefaultValue = UDFOption;
                //objUDFDTO.Updated = System.DateTime.UtcNow;
                //objUDFDAL.Edit(objUDFDTO);

                return true;
            }
        }

    }
}
