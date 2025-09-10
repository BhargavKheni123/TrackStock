using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public class ResourceDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        //public ResourceDAL(base.DataBaseName)
        //{

        //}

        public ResourceDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ResourceDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        /// <summary>
        /// GetCachedData
        /// </summary>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        public IEnumerable<ResourceLanguageDTO> GetCachedResourceLanguageData(Int64 EnterpriseID)
        {
            IEnumerable<ResourceLanguageDTO> lstResourceLanguageDTO = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstResourceLanguageDTO = (from u in context.Database.SqlQuery<ResourceLanguageDTO>("exec [GetResourceLanguage] ")
                                          select new ResourceLanguageDTO
                                          {
                                              ID = u.ID,
                                              Culture = u.Culture,
                                              Language = u.Language
                                          }).AsParallel().ToList();
            }
            return lstResourceLanguageDTO;
        }

        public IEnumerable<ResourceLanguageDTO> GeteTurnsLanguages()
        {
            List<ResourceLanguageDTO> lstLans = new List<ResourceLanguageDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstLans = (from u in context.Database.SqlQuery<ResourceLanguageDTO>("exec [GetResourceLanguage] ")
                           select new ResourceLanguageDTO
                           {
                               ID = u.ID,
                               Culture = u.Culture,
                               Language = u.Language
                           }).AsParallel().ToList();

            }


            return lstLans;
        }
        public IEnumerable<ResourceLanguageDTO> OnlyeTurnsLanguages()
        {
            List<ResourceLanguageDTO> lstLans = new List<ResourceLanguageDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstLans = (from u in context.Database.SqlQuery<ResourceLanguageDTO>("exec [GetBaseDBResourceLanguage] ")
                           select new ResourceLanguageDTO
                           {
                               ID = u.ID,
                               Culture = u.Culture,
                               Language = u.Language
                           }).AsParallel().ToList();

            }


            return lstLans;
        }


        public bool InsertLanguageeTurns(ResourceLanguageDTO objResourceLanguageDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //ResourceLaguage obj = new ResourceLaguage()
                //{
                //    ID = 0,
                //    Culture = objResourceLanguageDTO.Culture,
                //    Language = objResourceLanguageDTO.Language
                //};
                //context.ResourceLaguages.Add(obj);
                //context.SaveChanges();
                //objResourceLanguageDTO.ID = objResourceLanguageDTO.ID;
                string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
                SqlHelper.ExecuteNonQuery(Connectionstring, "InsertResourcelanguage", objResourceLanguageDTO.ID, objResourceLanguageDTO.Culture, objResourceLanguageDTO.Language);
                return true;
            }
        }

        /// <summary>
        /// GetCachedData
        /// </summary>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        public IEnumerable<ResourceModuleMasterDTO> GetCachedResourceModuleMasterData(Int64 CompanyID)
        {
            IEnumerable<ResourceModuleMasterDTO> lstResourceModuleMaster = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstResourceModuleMaster = (from u in context.Database.SqlQuery<ResourceModuleMasterDTO>("exec [GetResourceModuleMaster] ")
                                           select new ResourceModuleMasterDTO
                                           {
                                               ID = u.ID,
                                               ModuleName = u.ModuleName,
                                               ResModuleKey = u.ResModuleKey
                                           }).AsParallel().ToList();
            }

            return lstResourceModuleMaster;
        }


        public IEnumerable<ResourceModuleMasterDTO> GetResourceModuleMasterData()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<ResourceModuleMasterDTO> objResModule = (from u in context.Database.SqlQuery<ResourceModuleMasterDTO>("Exec [GetResourceModule]")
                                                                     select new ResourceModuleMasterDTO
                                                                     {
                                                                         ID = u.ID,
                                                                         ModuleName = u.ModuleName,
                                                                         ResModuleKey = u.ResModuleKey
                                                                     }).AsParallel().ToList();
                return objResModule;
            }
        }


        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ResourceModuleDetailsDTO> GetCachedResourceModuleDetailData(Int64 CompanyID, int ResourceModuleID)
        {
            IEnumerable<ResourceModuleDetailsDTO> lstResourceModuleDetails = null;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ResourceModuleID", ResourceModuleID) };

                lstResourceModuleDetails = (from u in context.Database.SqlQuery<ResourceModuleDetailsDTO>("Exec [GetResourceModuleDetailByModuleID] @ResourceModuleID", params1)
                                            select new ResourceModuleDetailsDTO
                                            {
                                                ID = u.ID,
                                                DisplayFileName = u.DisplayFileName,
                                                FileName = u.FileName,
                                                ResourceModuleID = u.ResourceModuleID,
                                                ResourceModuleName = u.ResourceModuleName,
                                                ResModuleDetailsKey = u.ResModuleDetailsKey
                                            }).AsParallel().ToList();
            }
            return lstResourceModuleDetails;
        }

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ResourceModuleDetailsDTO> GetCachedResourceModuleDetailData_Mobile(Int64 CompanyID, Int64 ResourceModuleID)
        {
            IEnumerable<ResourceModuleDetailsDTO> lstResourceModuleDetails = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ResourceModuleID", ResourceModuleID) };

                lstResourceModuleDetails = (from u in context.Database.SqlQuery<ResourceModuleDetailsDTO>("Exec [GetResourceModuleDetailMobileByModuleID] @ResourceModuleID", params1)
                                            select new ResourceModuleDetailsDTO
                                                             {
                                                                 ID = u.ID,
                                                                 DisplayPageName = u.DisplayPageName,
                                                                 FileName = u.FileName,
                                                                 ResourceModuleID = u.ResourceModuleID,
                                                                 ResourceModuleName = u.ResourceModuleName,
                                                                 ResMobModuleDetailsKey = u.ResMobModuleDetailsKey
                                                             }).AsParallel().ToList();
                
            }

            return lstResourceModuleDetails;
        }

        public IEnumerable<ResourceModuleDetailsDTO> GetResourceModuleDetailData_Mobile(Int64 ResourceModuleID) 
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ResourceModuleID", ResourceModuleID) };

                IEnumerable<ResourceModuleDetailsDTO> obj = (from u in context.Database.SqlQuery<ResourceModuleDetailsDTO>("Exec [GetResourceModuleDetailMobileByModuleID] @ResourceModuleID", params1)
                                                             select new ResourceModuleDetailsDTO
                                                             {
                                                                 ID = u.ID,
                                                                 DisplayPageName = u.DisplayPageName,
                                                                 FileName = u.FileName,
                                                                 ResourceModuleID = u.ResourceModuleID,
                                                                 ResourceModuleName = u.ResourceModuleName,
                                                                 PageName = u.PageName,
                                                                 ResMobModuleDetailsKey = u.ResMobModuleDetailsKey
                                                             }).AsParallel().ToList();
                return obj;

            }
        }


        public IEnumerable<ResourceModuleDetailsDTO> GetResourceModuleDetailData(Int64 ResourceModuleID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ResourceModuleID", ResourceModuleID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<ResourceModuleDetailsDTO> obj = (from u in context.Database.SqlQuery<ResourceModuleDetailsDTO>("Exec [GetResourceModuleDetailData] @ResourceModuleID", params1)
                                                             select new ResourceModuleDetailsDTO
                                                             {
                                                                 ID = u.ID,
                                                                 DisplayFileName = u.DisplayFileName,
                                                                 FileName = u.FileName,
                                                                 ResourceModuleID = u.ResourceModuleID,
                                                                 ResourceModuleName = u.ResourceModuleName,
                                                                 //PageName = u.PageName,
                                                                 ResMobModuleDetailsKey = u.ResMobModuleDetailsKey
                                                             }).AsParallel().ToList();
                return obj;

            }
        }

        public ResourceModuleDetailsDTO GetResourceModuleDetailDataByID(Int64 ModuleDetailID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ModuleDetailID", ModuleDetailID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ResourceModuleDetailsDTO obj = (from u in context.Database.SqlQuery<ResourceModuleDetailsDTO>("Exec [GetResourceModuleDetailDataByID] @ModuleDetailID", params1)
                                                select new ResourceModuleDetailsDTO
                                                {
                                                    ID = u.ID,
                                                    FileName = u.FileName,
                                                    DisplayFileName = u.DisplayFileName,
                                                    ResourceModuleID = u.ResourceModuleID,
                                                    ResourceModuleName = u.ResourceModuleName,
                                                    ResMobModuleDetailsKey = u.ResMobModuleDetailsKey
                                                }).AsParallel().FirstOrDefault();
                return obj;
            }
        }
        
        public IEnumerable<ResourceLanguageDTO> GetETurnsResourceLanguageData()
        {
            eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString);

            IEnumerable<ResourceLanguageDTO> obj = (from u in context.Database.SqlQuery<ResourceLanguageDTO>("Exec [GetBaseDBResourceLanguage]")
                                                    select new ResourceLanguageDTO
                                                    {
                                                        ID = u.ID,
                                                        Culture = u.Culture,
                                                        Language = u.Language
                                                    }).AsParallel().ToList();
            return obj;
        }
        public IEnumerable<ResourceLanguageDTO> GetETurnsResourceLanguage()
        {
            eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString);

            IEnumerable<ResourceLanguageDTO> obj = (from u in context.Database.SqlQuery<ResourceLanguageDTO>("exec [GetETurnsResourceLanguage]")
                                                    select new ResourceLanguageDTO
                                                    {
                                                        ID = u.ID,
                                                        Culture = u.Culture,
                                                        Language = u.Language
                                                    }).AsParallel().ToList();
            return obj;
        }

        public ResourceLanguageDTO GetResourceLanguageByID(long ID)
        {
            eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString);
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID) };

            ResourceLanguageDTO obj = (from u in context.Database.SqlQuery<ResourceLanguageDTO>("exec [GetResourceLanguageByID] @ID", params1)
                                       select new ResourceLanguageDTO
                                       {
                                           ID = u.ID,
                                           Culture = u.Culture,
                                           Language = u.Language
                                       }).AsParallel().FirstOrDefault();
            return obj;
        }

        public ResourceLanguageDTO GetResourceLanguageByCulture(string culture)
        {
            eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString);
            var params1 = new SqlParameter[] { new SqlParameter("@Culture", culture) };

            ResourceLanguageDTO obj = (from u in context.Database.SqlQuery<ResourceLanguageDTO>("exec [GetResourceLanguageByCulture] @Culture", params1)
                                       select new ResourceLanguageDTO
                                       {
                                           ID = u.ID,
                                           Culture = u.Culture,
                                           Language = u.Language
                                       }).AsParallel().FirstOrDefault();
            return obj;
        }

        public IEnumerable<ResourceLanguageDTO> GetResourceLanguageDataFromXML(string mappath)
        {
            DataSet objDataset = new DataSet();
            objDataset.ReadXml(mappath);

            IEnumerable<ResourceLanguageDTO> obj = objDataset.Tables[0].AsEnumerable()
                                                    .Select(row => new ResourceLanguageDTO
                                                    {
                                                        ID = 0,
                                                        Culture = row.Field<string>("Culture"),
                                                        Language = row.Field<string>("Language")
                                                    }).AsParallel().ToList();
            return obj;
        }
        public void AddResourceLanguageData(ResourceLanguageDTO objResourceLanguageDTO)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strCommand = "EXEC AddResourceLanguageData ";
                strCommand += "'" + objResourceLanguageDTO.Language + "'";
                strCommand += ",'" + objResourceLanguageDTO.Culture + "'";
                context.Database.ExecuteSqlCommand(strCommand);
            }
        }

        public void DeleteETurnsResourceLanguageData(ResourceLanguageDTO objResourceLanguageDTO)  
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ResourceLaguageID", objResourceLanguageDTO.ID) };
                context.Database.ExecuteSqlCommand("EXEC DeleteBaseDBEmailTemplateDetail @ResourceLaguageID", params1);

                var params2 = new SqlParameter[] { new SqlParameter("@ResourceLaguageID", objResourceLanguageDTO.ID) };
                context.Database.ExecuteSqlCommand("EXEC DeleteBaseDBMobileResources  @ResourceLaguageID", params2);

                var params3 = new SqlParameter[] { new SqlParameter("@ResourceLaguage", objResourceLanguageDTO.Language) };
                context.Database.ExecuteSqlCommand("EXEC DeleteBaseDBResourcesLanguage  @ResourceLaguage", params3);

            }
        }

        public IEnumerable<ResourceLanguageDTO> GetResourceLanguageData()
        {
            eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString);

            IEnumerable<ResourceLanguageDTO> obj = (from u in context.Database.SqlQuery<ResourceLanguageDTO>("EXEC [GetResourceLanguage] ")
                                                    select new ResourceLanguageDTO
                                                    {
                                                        ID = u.ID,
                                                        Culture = u.Culture,
                                                        Language = u.Language
                                                    }).AsParallel().ToList();
            return obj;
        }
         
        public void DeleteResourceLanguageData(ResourceLanguageDTO objResourceLanguageDTO, int CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //var params1 = new SqlParameter[] { new SqlParameter("@ResourceLaguageID", objResourceLanguageDTO.ID),
                //                                   new SqlParameter("@CompanyID", CompanyID) };
                //context.Database.ExecuteSqlCommand("EXEC DeleteMobileResources @ResourceLaguageID,@CompanyID", params1);

                
                var params2 = new SqlParameter[] { new SqlParameter("@ResourceLaguageID", objResourceLanguageDTO.ID),
                                                   new SqlParameter("@CompanyID", CompanyID) };
                context.Database.ExecuteSqlCommand("EXEC DeleteEmailTemplateDetail @ResourceLaguageID,@CompanyID", params2);

                if (CompanyID == 0)
                {
                    var params3 = new SqlParameter[] { new SqlParameter("@Language", objResourceLanguageDTO.Language) };
                    context.Database.ExecuteSqlCommand("EXEC DeleteResourceLaguages @Language", params3);
                }
            }
        }

        public void AddResource(string strCommand)
        {
            eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString);
            context.Database.ExecuteSqlCommand(strCommand);

        }

        public Int64 InsertResourceLanguage(ResourceLanguageDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Int64 InsertedID = 0;
                var params1 = new SqlParameter[] {
                    new SqlParameter("@ID", objDTO.ID),
                    new SqlParameter("@culture", objDTO.Culture),
                    new SqlParameter("@Language", objDTO.Language)
                };
                InsertedID = context.Database.SqlQuery<Int64>("EXEC [InsertResourcelanguageEnterpriseWise] @ID,@culture,@Language", params1).FirstOrDefault();
                return InsertedID;

                //ResourceLaguage obj = new ResourceLaguage();
                //obj.ID = 0;
                //obj.Language = objDTO.Language;
                //obj.Culture = objDTO.Culture;

                //context.ResourceLaguages.Add(obj);
                //context.SaveChanges();

                //objDTO.ID = obj.ID;
                //return obj.ID;
            }
        }

        /// <summary>
        /// GetCachedData
        /// </summary>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        public ResourceModuleMasterDTO GetResourceModuleByID(Int64 ModuleID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ModuleID", ModuleID) };

                ResourceModuleMasterDTO obj = (from u in context.Database.SqlQuery<ResourceModuleMasterDTO>("EXEC GetResourceModuleMasterByID  @ModuleID", params1)
                                               select new ResourceModuleMasterDTO
                                               {
                                                   ID = u.ID,
                                                   ModuleName = u.ModuleName
                                               }).FirstOrDefault();
                return obj;
            }
        }

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public ResourceModuleDetailsDTO GetResourceModuleDetail(Int64 ModuleID, string ResourcePageName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ModuleID", ModuleID), 
                                                   new SqlParameter("@ResourcePageName", ResourcePageName) };

                ResourceModuleDetailsDTO obj = (from u in context.Database.SqlQuery<ResourceModuleDetailsDTO>("EXEC GetResourceModuleDetailByModuleIDResPage  @ModuleID,@ResourcePageName", params1)
                                                select new ResourceModuleDetailsDTO
                                                {
                                                    ID = u.ID,
                                                    DisplayFileName = u.DisplayFileName,
                                                    FileName = u.FileName,
                                                    ResourceModuleID = u.ResourceModuleID,
                                                    ResourceModuleName = u.ResourceModuleName
                                                }).FirstOrDefault();
                return obj;
            }

        }

        public IEnumerable<MobileResourcesDTO> GetAllMobileResourceRecords()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<MobileResourcesDTO> obj = (from u in context.Database.SqlQuery<MobileResourcesDTO>("EXEC [GetAllMobileResource] ")
                                                       select new MobileResourcesDTO
                                                       {
                                                           ID = u.ID,
                                                           ResourceKey = u.ResourceKey,
                                                           ResourceValue = u.ResourceValue,
                                                           CompanyID = u.CompanyID,
                                                           CreatedBy = u.CreatedBy,
                                                           UpdatedBy = u.UpdatedBy,
                                                           CreatedOn = u.CreatedOn,
                                                           UpdatedOn = u.UpdatedOn,
                                                           CreatedByName = u.CreatedByName,
                                                           UpdatedByName = u.UpdatedByName,
                                                           LanguageID = u.LanguageID,
                                                           ResourcePageID = u.ResourcePageID,
                                                       }).AsParallel().ToList();
                return obj;
            }
        }

        /// <returns></returns>
        public bool Edit(MobileResourcesDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                MobileResource obj = context.MobileResources.FirstOrDefault(x => x.ID == objDTO.ID);
                obj.ID = objDTO.ID;

                obj.ResourceKey = objDTO.ResourceKey;
                obj.ResourceValue = objDTO.ResourceValue;
                obj.UpdatedBy = objDTO.UpdatedBy;
                obj.UpdatedOn = DateTimeUtility.DateTimeNow;


                context.Entry(obj).State = System.Data.Entity.EntityState.Unchanged;
                context.MobileResources.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                return true;
            }
        }

        public bool SaveRoomWiseEmailTemplate(long CompanyId, long RoomId, long UserID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {  

                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId),  
                                                   new SqlParameter("@CompanyID", CompanyId),
                                                   new SqlParameter("@UserID", UserID) };

                context.Database.ExecuteSqlCommand("EXEC [SaveRoomWiseEmailTemplate] @RoomID,@CompanyID,@UserID", params1);
                return true;
            }
        }
    }
}
