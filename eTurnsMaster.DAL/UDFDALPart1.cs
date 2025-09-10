using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurnsMaster.DAL
{
    public partial class UDFDAL : eTurnsMasterBaseDAL
    {
        public IEnumerable<UDFDTO> GetCachedDataeTurns()
        {
            //Get Cached-Media
            IEnumerable<UDFDTO> ObjCache = CacheHelper<IEnumerable<UDFDTO>>.GetCacheItem("Cached_UDF_eTurns");
            if (ObjCache == null)
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    string qry = @"select A.*,u1.UserName as CreatedByName,u2.username as UpdatedByName from " + MasterDbConnectionHelper.GetETurnsMasterDBName() + ".dbo.udf as A left outer join " + MasterDbConnectionHelper.GetETurnsMasterDBName() + ".dbo.UserMaster as u1 on A.CreatedBy= u1.id left outer join " + MasterDbConnectionHelper.GetETurnsMasterDBName() + ".dbo.UserMaster as u2 on A.LastUpdatedBy= u2.id";

                    IEnumerable<UDFDTO> obj = (from u in context.ExecuteStoreQuery<UDFDTO>(qry)
                                               select new UDFDTO
                                               {
                                                   EnterpriseId = u.EnterpriseId,
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
                                                   Created = u.Created,
                                                   Updated = u.Updated,
                                                   CreatedBy = u.CreatedBy,
                                                   LastUpdatedBy = u.LastUpdatedBy,
                                                   CreatedByName = u.CreatedByName,
                                                   UpdatedByName = u.UpdatedByName,
                                                   IsDeleted = u.IsDeleted,
                                                   UDFMaxLength = u.UDFMaxLength
                                               }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<UDFDTO>>.AddCacheItem("Cached_UDF_eTurns", obj);
                }
            }

            return ObjCache;
        }

        public IEnumerable<UDFDTO> GetPagedRecordseTurns(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 CompanyID, string UDFTableName, long EnterpriseID)
        {


            if (String.IsNullOrEmpty(SearchTerm))
            {
                //Get Cached-Media
                IEnumerable<UDFDTO> ObjCache = GetCachedDataeTurns().Where(t => t.UDFTableName == UDFTableName && t.EnterpriseId == EnterpriseID);
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                IEnumerable<UDFDTO> ObjCache = GetCachedDataeTurns();

                string[] dd = SearchTerm.Replace("STARTWITH#", "$").Split('$');
                string[] stringSeparators = new string[] { "[###]" };

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
                TotalCount = ObjCache.Where(t => t.UDFTableName == UDFTableName).Count();
                return ObjCache.Where(t => t.UDFTableName == UDFTableName).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                IEnumerable<UDFDTO> ObjCache = GetCachedDataeTurns();
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
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Unchanged);
                context.UDFs.Attach(obj);
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                context.SaveChanges();

                //Set UDFOptions deleted on UDF delete
                string sSQL = "UPDATE UDFOptions SET Updated = '" + DateTime.UtcNow.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE UDFID =" + Udfid.ToString() + ";";
                context.ExecuteStoreCommand(sSQL);

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
        /// Delete Multiple UDF Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyId)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        //Update UDF to be set as Deleted
                        strQuery += "UPDATE UDF SET Updated = '" + DateTime.UtcNow.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + "; ";
                        //Update UDFOptions to be set as Deleted
                        strQuery += "UPDATE UDFOptions SET Updated = '" + DateTime.UtcNow.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE UDFID =" + item.ToString() + "; ";
                    }
                }
                context.ExecuteStoreCommand(strQuery);

                //Get Cached-Media
                IEnumerable<UDFDTO> ObjCache = CacheHelper<IEnumerable<UDFDTO>>.GetCacheItem("Cached_UDF_Master");
                if (ObjCache != null)
                {
                    List<UDFDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
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
        public bool InsertDefaultUDFs(string UDFTableName, Int64 UserID, long EnterpriseId, int iUDFMaxLength = 200)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                string sSQL = "";
                string DateNow = System.DateTime.UtcNow.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String))));
                string UDFColumnName;
                // 5 - so the code will check from UDF1 to UDF5, if NOT exists will create them
                for (int i = 1; i <= 5; i++)
                {
                    UDFColumnName = "UDF" + i.ToString();
                    sSQL += " IF NOT EXISTS (SELECT ID FROM UDF WHERE UDFColumnName='" + UDFColumnName + "' AND UDFTableName='" + UDFTableName + "' AND EnterpriseId=" + EnterpriseId + ") ";
                    sSQL += " Begin ";
                    sSQL += "     INSERT INTO UDF (UDFTableName, UDFColumnName, created, Updated,CreatedBy,LastUpdatedBy,IsDeleted,GUID,EnterpriseId,UDFControlType,UDFOptionsCSV,UDFDefaultValue,UDFIsRequired,UDFIsSearchable,UDFMaxLength) ";
                    //sSQL += "     VALUES('" + UDFTableName + "','" + UDFColumnName + "', '" + DateNow + "', '" + DateNow + "', " + UserID + ", " + UserID + ",0,'" + Guid.NewGuid() + "'," + EnterpriseId + ")";
                    sSQL += "SELECT '" + UDFTableName + "', UDFColumnName, '" + DateNow + "', '" + DateNow + "'," + UserID + "," + UserID + ",IsDeleted,'" + Guid.NewGuid() + "',0,UDFControlType,UDFOptionsCSV,UDFDefaultValue,UDFIsRequired,UDFIsSearchable," + iUDFMaxLength + " FROM " + MasterDbConnectionHelper.GeteTurnsDBName() + ".dbo.UDF where companyid=0 and room=0 and udfTableName='" + UDFTableName + "' and UDFColumnName='" + UDFColumnName + "'";
                    sSQL += "     INSERT INTO UDFoptions (UDFID,UDFOption,created, Updated,CreatedBy,LastUpdatedBy,IsDeleted,GUID)";
                    sSQL += "     select @@IDENTITY,UDFOption, '" + DateNow + "', '" + DateNow + "'," + UserID + "," + UserID + ",IsDeleted,'" + Guid.NewGuid() + "' from " + MasterDbConnectionHelper.GeteTurnsDBName() + ".dbo.UDFoptions where udfid =(SELECT ID FROM UDF WHERE UDFColumnName='" + UDFColumnName + "' AND UDFTableName='" + UDFTableName + "' AND EnterpriseId=0)";
                    sSQL += " End ";
                }
                var iResult = context.ExecuteStoreCommand(sSQL);


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

    public partial class UDFOptionDAL : eTurnsMasterBaseDAL
    {
        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UDFOptionsDTO> GetCachedData()
        {
            //Get Cached-Media
            IEnumerable<UDFOptionsDTO> ObjCache = CacheHelper<IEnumerable<UDFOptionsDTO>>.GetCacheItem("Cached_UDFOptions_"); // + CompanyID.ToString());
            //if (ObjCache == null)
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<UDFOptionsDTO> obj = (from u in context.ExecuteStoreQuery<UDFOptionsDTO>(@"SELECT * from UDFOptions WHERE IsDeleted = 0 ")// AND CompanyID = " + CompanyID.ToString())
                                                      select new UDFOptionsDTO
                                                      {
                                                          ID = u.ID,
                                                          UDFID = u.UDFID,
                                                          UDFOption = u.UDFOption,
                                                          CompanyID = u.CompanyID,
                                                          GUID = u.GUID,
                                                          Created = u.Created,
                                                          Updated = u.Updated,
                                                          CreatedBy = u.CreatedBy
                                                      }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<UDFOptionsDTO>>.AddCacheItem("Cached_UDFOptions_", obj);// + CompanyID.ToString(), obj);
                }
            }

            return ObjCache;
        }


        public UDFOptionsDTO GetRecord(Int64 ID)
        {
            return GetCachedData().Single(t => t.ID == ID);
        }


        /// <summary>
        /// Get All Records By UDFID
        /// </summary>
        /// <param name="UDFID"></param>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        public IEnumerable<UDFOptionsDTO> GetUDFOptionsByUDF(Int64 UDFID)
        {
            return GetCachedData().Where(t => t.UDFID == UDFID);
        }

        /// <summary>
        /// Delete Record By ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool Delete(Int64 id, Int64 userid)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                string sSQL = "";
                sSQL += "UPDATE UDFOptions SET Updated = '" + DateTime.UtcNow.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + id.ToString() + ";";
                context.ExecuteStoreCommand(sSQL);

                ////Get Cached-Media
                IEnumerable<UDFOptionsDTO> ObjCache = CacheHelper<IEnumerable<UDFOptionsDTO>>.GetCacheItem("Cached_UDFOptions_");//+ obj.CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<UDFOptionsDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == id);
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<UDFOptionsDTO>>.AppendToCacheItem("Cached_UDFOptions_", ObjCache); //+ obj.CompanyID.ToString(), ObjCache);
                }

                return true;
            }
        }
    }
}

