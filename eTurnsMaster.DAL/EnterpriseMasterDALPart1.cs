using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eTurns.DTO;
using System.Data;
using Dynamite;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Collections;
using System.Configuration;
using System.Data.SqlClient;
using eTurns.DTO.Resources;
using System.Web;


namespace eTurnsMaster.DAL
{
    public partial class EnterpriseMasterDAL : eTurnsMasterBaseDAL
    {
        public List<EnterPriseSQLScriptsDTO> GetDbAllScripts(bool IsDeleted)
        {
            List<EnterPriseSQLScriptsDTO> lstScripts = new List<EnterPriseSQLScriptsDTO>();

            using (eTurns_MasterEntities MasterDb = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {

                lstScripts = (from esm in MasterDb.EnterpriseScriptsMasters
                                  //join umcrby in MasterDb.UserMasters on esm.CreatedBy equals umcrby.ID
                              select new EnterPriseSQLScriptsDTO
                              {
                                  CreatedBy = esm.CreatedBy,
                                  CreatedDate = esm.CreatedDate,
                                  ExecuitionSequence = esm.ExecuitionSequence,
                                  IsDeleted = esm.IsDeleted,
                                  IsMasterScript = esm.IsMasterScript,
                                  ScriptName = esm.ScriptName,
                                  ScriptText = esm.ScriptText,
                                  SQLScriptID = esm.SQLScriptID,
                                  UpdatedBy = esm.UpdatedBy,
                                  UpdatedDate = esm.UpdatedDate

                              }).OrderBy(t => t.SQLScriptID).ToList();
            }
            return lstScripts.Where(l => l.IsDeleted == IsDeleted).ToList();

        }
        public List<EnterPriseSQLScriptsDtlDTO> GetDbAllScripts(long SQlScriptID)
        {
            List<EnterPriseSQLScriptsDtlDTO> lstEnterPriseSQLScriptsDtlDTO = new List<EnterPriseSQLScriptsDtlDTO>();
            using (eTurns_MasterEntities MasterDb = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                lstEnterPriseSQLScriptsDtlDTO = (from em in MasterDb.EnterpriseMasters
                                                 join esd in MasterDb.EnterpriseSqlScriptsDetails on new { emid = em.ID, islatest = true, scid = SQlScriptID } equals new { emid = esd.EnterpriseID, islatest = esd.IsLatestExecution, scid = esd.SQLScriptID } into em_esd_join
                                                 from em_esd in em_esd_join.DefaultIfEmpty()
                                                 select new EnterPriseSQLScriptsDtlDTO
                                                 {
                                                     EnterpriseID = em.ID,
                                                     EnterPriseName = em.Name,
                                                     EnterpriseSqlScriptsDtlID = em_esd.EnterpriseSqlScriptsDtlID,
                                                     IsExecuitedSuccessfully = em_esd.IsExecuitedSuccessfully,
                                                     IsLatestExecution = em_esd.IsLatestExecution,
                                                     ReturnResult = em_esd.ReturnResult,
                                                     SQLScriptID = SQlScriptID
                                                 }).OrderBy(t => t.EnterpriseID).ToList();
            }
            return lstEnterPriseSQLScriptsDtlDTO;
        }
        public EnterpriseDTO GetEnterprise(Int64 id)
        {
            EnterpriseDTO objEnterpriseDTO = new EnterpriseDTO();
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                UserMaster objUserMasterDTO = new UserMaster();
                objUserMasterDTO = (from um in context.UserMasters
                                    join EM in context.EnterpriseMasters on um.ID equals EM.EnterpriseSuperAdmin// into UME

                                    where um.EnterpriseId == id && um.UserType == 2 && um.RoleId == -2 && um.IsDeleted == false// && (UMList == null || (UMList != null && UMList.EnterpriseSuperAdmin == um.ID))
                                    select um).FirstOrDefault();

                if (objUserMasterDTO == null)
                {
                    objUserMasterDTO = new UserMaster();
                }

                List<EnterpriseSuperAdmin> lstAll = context.UserMasters.Where(t => t.RoleId == -2 && t.UserType == 2 && t.EnterpriseId == id && t.IsDeleted == false).Select(t => new EnterpriseSuperAdmin() { ID = t.ID, Email = t.Email, IsDefault = false, UserName = t.UserName, IsEPSuperAdmin = true }).ToList();
                if (objUserMasterDTO != null && objUserMasterDTO.ID > 0)
                {
                    lstAll.ForEach(t =>
                    {
                        if (t.ID == objUserMasterDTO.ID)
                        {
                            t.IsDefault = true;
                        }
                    });
                }
                if (lstAll == null)
                {
                    lstAll = new List<EnterpriseSuperAdmin>();
                }
                string strAdmins = string.Join(",", lstAll.Select(t => t.UserName).ToArray());

                objEnterpriseDTO = (from x in context.EnterpriseMasters
                                    join um in context.UserMasters on x.CreatedBy equals um.ID into x_um_join
                                    from x_um in x_um_join.DefaultIfEmpty()
                                    join umup in context.UserMasters on x.LastUpdatedBy equals umup.ID into x_umup_join
                                    from x_umup in x_umup_join.DefaultIfEmpty()
                                    where x.ID == id
                                    select new EnterpriseDTO
                                    {
                                        ID = x.ID,
                                        Name = x.Name,
                                        Address = x.Address,
                                        City = x.City,
                                        State = x.State,
                                        PostalCode = x.PostalCode,
                                        Country = x.Country,
                                        ContactPhone = x.ContactPhone,
                                        ContactEmail = x.ContactEmail,
                                        SoftwareBasePrice = x.SoftwareBasePrice,
                                        MaxBinsPerBasePrice = x.MaxBinsPerBasePrice,
                                        CostPerBin = x.CostPerBin,
                                        DiscountPrice1 = x.DiscountPrice1,
                                        DiscountPrice2 = x.DiscountPrice2,
                                        DiscountPrice3 = x.DiscountPrice3,
                                        MaxSubscriptionTier1 = x.MaxSubscriptionTier1,
                                        MaxSubscriptionTier2 = x.MaxSubscriptionTier2,
                                        MaxSubscriptionTier3 = x.MaxSubscriptionTier3,
                                        MaxSubscriptionTier4 = x.MaxSubscriptionTier4,
                                        MaxSubscriptionTier5 = x.MaxSubscriptionTier5,
                                        MaxSubscriptionTier6 = x.MaxSubscriptionTier6,
                                        PriceSubscriptionTier1 = x.PriceSubscriptionTier1,
                                        PriceSubscriptionTier2 = x.PriceSubscriptionTier2,
                                        PriceSubscriptionTier3 = x.PriceSubscriptionTier3,
                                        PriceSubscriptionTier4 = x.PriceSubscriptionTier4,
                                        PriceSubscriptionTier5 = x.PriceSubscriptionTier5,
                                        PriceSubscriptionTier6 = x.PriceSubscriptionTier6,
                                        IncludeLicenseFees = x.IncludeLicenseFees,
                                        MaxLicenseTier1 = x.MaxLicenseTier1,
                                        MaxLicenseTier2 = x.MaxLicenseTier2,
                                        MaxLicenseTier3 = x.MaxLicenseTier3,
                                        MaxLicenseTier4 = x.MaxLicenseTier4,
                                        MaxLicenseTier5 = x.MaxLicenseTier5,
                                        MaxLicenseTier6 = x.MaxLicenseTier6,
                                        PriceLicenseTier1 = x.PriceLicenseTier1,
                                        PriceLicenseTier2 = x.PriceLicenseTier2,
                                        PriceLicenseTier3 = x.PriceLicenseTier3,
                                        PriceLicenseTier4 = x.PriceLicenseTier4,
                                        PriceLicenseTier5 = x.PriceLicenseTier5,
                                        PriceLicenseTier6 = x.PriceLicenseTier6,
                                        UDF1 = x.UDF1,
                                        UDF2 = x.UDF2,
                                        UDF3 = x.UDF3,
                                        UDF4 = x.UDF4,
                                        UDF5 = x.UDF5,
                                        GUID = x.GUID ?? Guid.Empty,
                                        Created = x.Created,
                                        Updated = x.Updated,
                                        CreatedByName = x_um.UserName,
                                        UpdatedByName = x_umup.UserName,
                                        IsArchived = x.IsArchived ?? false,
                                        IsDeleted = x.IsDeleted,
                                        CreatedBy = x.CreatedBy,
                                        LastUpdatedBy = x.LastUpdatedBy,
                                        EnterpriseUserEmail = objUserMasterDTO.Email,
                                        EnterpriseUserPassword = objUserMasterDTO.Password,
                                        UserName = objUserMasterDTO.UserName,
                                        EnterpriseUserID = objUserMasterDTO.ID,
                                        EnterpriseDBName = x.EnterpriseDBName,
                                        //EnterpriseDBConnectionString = x.EnterpriseDBConnectionString,
                                        EnterpriseSqlServerName = x.EnterpriseSqlServerName,
                                        EnterpriseSqlServerUserName = x.EnterpriseSqlServerUserName,
                                        EnterpriseSqlServerPassword = x.EnterpriseSqlServerPassword,
                                        EnterpriseLogo = x.EnterPriseLogo,
                                        IsActive = x.IsActive,
                                        EnterpriseSuperAdmin = x.EnterpriseSuperAdmin,
                                        //lstSuperAdmins = lstAll,
                                        //EnterpriseSuperAdmins = strAdmins
                                        EnterPriseDomainURL = x.EnterpriseDomainURL,
                                        ArchiveDbName = x.ArchiveDbName,
                                        ChangeLogDbName = x.ChangeLogDbName,
                                        HistoryDBName = x.HistoryDBName
                                    }).FirstOrDefault();
                if (objEnterpriseDTO != null)
                {
                    objEnterpriseDTO.lstSuperAdmins = lstAll;
                    objEnterpriseDTO.EnterpriseSuperAdmins = strAdmins;
                }

                return objEnterpriseDTO;

            }
        }

        public EnterpriseDTO GetEnterpriseByName(String Enterprise)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return (from x in context.EnterpriseMasters
                        join um in context.UserMasters on x.CreatedBy equals um.ID into x_um_join
                        from x_um in x_um_join.DefaultIfEmpty()
                        join umup in context.UserMasters on x.LastUpdatedBy equals umup.ID into x_umup_join
                        from x_umup in x_umup_join.DefaultIfEmpty()
                        where x.Name.Trim().ToUpper() == Enterprise.Trim().ToUpper() && x.IsDeleted == false
                        select new EnterpriseDTO
                        {
                            ID = x.ID,
                            Name = x.Name,
                            Address = x.Address,
                            City = x.City,
                            State = x.State,
                            PostalCode = x.PostalCode,
                            Country = x.Country,
                            ContactPhone = x.ContactPhone,
                            ContactEmail = x.ContactEmail,
                            SoftwareBasePrice = x.SoftwareBasePrice,
                            MaxBinsPerBasePrice = x.MaxBinsPerBasePrice,
                            CostPerBin = x.CostPerBin,
                            DiscountPrice1 = x.DiscountPrice1,
                            DiscountPrice2 = x.DiscountPrice2,
                            DiscountPrice3 = x.DiscountPrice3,
                            MaxSubscriptionTier1 = x.MaxSubscriptionTier1,
                            MaxSubscriptionTier2 = x.MaxSubscriptionTier2,
                            MaxSubscriptionTier3 = x.MaxSubscriptionTier3,
                            MaxSubscriptionTier4 = x.MaxSubscriptionTier4,
                            MaxSubscriptionTier5 = x.MaxSubscriptionTier5,
                            MaxSubscriptionTier6 = x.MaxSubscriptionTier6,
                            PriceSubscriptionTier1 = x.PriceSubscriptionTier1,
                            PriceSubscriptionTier2 = x.PriceSubscriptionTier2,
                            PriceSubscriptionTier3 = x.PriceSubscriptionTier3,
                            PriceSubscriptionTier4 = x.PriceSubscriptionTier4,
                            PriceSubscriptionTier5 = x.PriceSubscriptionTier5,
                            PriceSubscriptionTier6 = x.PriceSubscriptionTier6,
                            IncludeLicenseFees = x.IncludeLicenseFees,
                            MaxLicenseTier1 = x.MaxLicenseTier1,
                            MaxLicenseTier2 = x.MaxLicenseTier2,
                            MaxLicenseTier3 = x.MaxLicenseTier3,
                            MaxLicenseTier4 = x.MaxLicenseTier4,
                            MaxLicenseTier5 = x.MaxLicenseTier5,
                            MaxLicenseTier6 = x.MaxLicenseTier6,
                            PriceLicenseTier1 = x.PriceLicenseTier1,
                            PriceLicenseTier2 = x.PriceLicenseTier2,
                            PriceLicenseTier3 = x.PriceLicenseTier3,
                            PriceLicenseTier4 = x.PriceLicenseTier4,
                            PriceLicenseTier5 = x.PriceLicenseTier5,
                            PriceLicenseTier6 = x.PriceLicenseTier6,
                            UDF1 = x.UDF1,
                            UDF2 = x.UDF2,
                            UDF3 = x.UDF3,
                            UDF4 = x.UDF4,
                            UDF5 = x.UDF5,
                            GUID = x.GUID ?? Guid.Empty,
                            Created = x.Created,
                            Updated = x.Updated,
                            CreatedByName = x_um.UserName,
                            UpdatedByName = x_umup.UserName,
                            IsArchived = x.IsArchived ?? false,
                            IsDeleted = x.IsDeleted,
                            CreatedBy = x.CreatedBy,
                            LastUpdatedBy = x.LastUpdatedBy,
                            //EnterpriseUserEmail = objUserMasterDTO.Email,
                            //EnterpriseUserPassword = objUserMasterDTO.Password,
                            //UserName = objUserMasterDTO.UserName,
                            //EnterpriseUserID = objUserMasterDTO.ID,
                            EnterpriseDBName = x.EnterpriseDBName,
                            //EnterpriseDBConnectionString = x.EnterpriseDBConnectionString,
                            EnterpriseSqlServerName = x.EnterpriseSqlServerName,
                            EnterpriseSqlServerUserName = x.EnterpriseSqlServerUserName,
                            EnterpriseSqlServerPassword = x.EnterpriseSqlServerPassword,
                            EnterpriseLogo = x.EnterPriseLogo,
                            IsActive = x.IsActive,
                            EnterPriseDomainURL = x.EnterpriseDomainURL,
                            ArchiveDbName = x.ArchiveDbName,
                            ChangeLogDbName = x.ChangeLogDbName,
                            HistoryDBName = x.HistoryDBName
                        }).FirstOrDefault();

            }
        }

        public EnterpriseDTO GetEnterpriseByID(long EnterpriseID)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return (from x in context.EnterpriseMasters
                        join um in context.UserMasters on x.CreatedBy equals um.ID into x_um_join
                        from x_um in x_um_join.DefaultIfEmpty()
                        join umup in context.UserMasters on x.LastUpdatedBy equals umup.ID into x_umup_join
                        from x_umup in x_umup_join.DefaultIfEmpty()
                        where x.ID == EnterpriseID && x.IsDeleted == false
                        select new EnterpriseDTO
                        {
                            ID = x.ID,
                            Name = x.Name,
                            Address = x.Address,
                            City = x.City,
                            State = x.State,
                            PostalCode = x.PostalCode,
                            Country = x.Country,
                            ContactPhone = x.ContactPhone,
                            ContactEmail = x.ContactEmail,
                            SoftwareBasePrice = x.SoftwareBasePrice,
                            MaxBinsPerBasePrice = x.MaxBinsPerBasePrice,
                            CostPerBin = x.CostPerBin,
                            DiscountPrice1 = x.DiscountPrice1,
                            DiscountPrice2 = x.DiscountPrice2,
                            DiscountPrice3 = x.DiscountPrice3,
                            MaxSubscriptionTier1 = x.MaxSubscriptionTier1,
                            MaxSubscriptionTier2 = x.MaxSubscriptionTier2,
                            MaxSubscriptionTier3 = x.MaxSubscriptionTier3,
                            MaxSubscriptionTier4 = x.MaxSubscriptionTier4,
                            MaxSubscriptionTier5 = x.MaxSubscriptionTier5,
                            MaxSubscriptionTier6 = x.MaxSubscriptionTier6,
                            PriceSubscriptionTier1 = x.PriceSubscriptionTier1,
                            PriceSubscriptionTier2 = x.PriceSubscriptionTier2,
                            PriceSubscriptionTier3 = x.PriceSubscriptionTier3,
                            PriceSubscriptionTier4 = x.PriceSubscriptionTier4,
                            PriceSubscriptionTier5 = x.PriceSubscriptionTier5,
                            PriceSubscriptionTier6 = x.PriceSubscriptionTier6,
                            IncludeLicenseFees = x.IncludeLicenseFees,
                            MaxLicenseTier1 = x.MaxLicenseTier1,
                            MaxLicenseTier2 = x.MaxLicenseTier2,
                            MaxLicenseTier3 = x.MaxLicenseTier3,
                            MaxLicenseTier4 = x.MaxLicenseTier4,
                            MaxLicenseTier5 = x.MaxLicenseTier5,
                            MaxLicenseTier6 = x.MaxLicenseTier6,
                            PriceLicenseTier1 = x.PriceLicenseTier1,
                            PriceLicenseTier2 = x.PriceLicenseTier2,
                            PriceLicenseTier3 = x.PriceLicenseTier3,
                            PriceLicenseTier4 = x.PriceLicenseTier4,
                            PriceLicenseTier5 = x.PriceLicenseTier5,
                            PriceLicenseTier6 = x.PriceLicenseTier6,
                            UDF1 = x.UDF1,
                            UDF2 = x.UDF2,
                            UDF3 = x.UDF3,
                            UDF4 = x.UDF4,
                            UDF5 = x.UDF5,
                            GUID = x.GUID ?? Guid.Empty,
                            Created = x.Created,
                            Updated = x.Updated,
                            CreatedByName = x_um.UserName,
                            UpdatedByName = x_umup.UserName,
                            IsArchived = x.IsArchived ?? false,
                            IsDeleted = x.IsDeleted,
                            CreatedBy = x.CreatedBy,
                            LastUpdatedBy = x.LastUpdatedBy,
                            //EnterpriseUserEmail = objUserMasterDTO.Email,
                            //EnterpriseUserPassword = objUserMasterDTO.Password,
                            //UserName = objUserMasterDTO.UserName,
                            //EnterpriseUserID = objUserMasterDTO.ID,
                            EnterpriseDBName = x.EnterpriseDBName,
                            //EnterpriseDBConnectionString = x.EnterpriseDBConnectionString,
                            EnterpriseSqlServerName = x.EnterpriseSqlServerName,
                            EnterpriseSqlServerUserName = x.EnterpriseSqlServerUserName,
                            EnterpriseSqlServerPassword = x.EnterpriseSqlServerPassword,
                            EnterpriseLogo = x.EnterPriseLogo,
                            IsActive = x.IsActive,
                            EnterPriseDomainURL = x.EnterpriseDomainURL,
                            ArchiveDbName = x.ArchiveDbName,
                            ChangeLogDbName = x.ChangeLogDbName,
                            HistoryDBName = x.HistoryDBName
                        }).FirstOrDefault();

            }
        }

        public Dictionary<long, string> GetEnterpriseListWithDBName(List<long> EnterpriseIDs)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var enterprises = (from x in context.EnterpriseMasters
                                   where EnterpriseIDs.Contains(x.ID) && x.IsDeleted == false
                                   select new EnterpriseDTO
                                   {
                                       ID = x.ID,
                                       EnterpriseDBName = (!string.IsNullOrEmpty(x.EnterpriseDBName) ? x.EnterpriseDBName : string.Empty)
                                   }).ToList();

                Dictionary<long, string> enterpriseList = enterprises.ToDictionary(o => o.ID, o => o.EnterpriseDBName);
                return enterpriseList;
            }
        }

        public List<EnterpriseDTO> GetAllEnterpriseForExecution()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                string Qry = @"select EnterpriseDBName,name,2 as currentposition from [" + MasterDbConnectionHelper.GetETurnsMasterDBName() + "].[dbo].[EnterpriseMaster] where isnull(isdeleted,0) =0 union SELECT 'eTurnsdemo' as EnterpriseDBName,'eTurnsdemo' as name ,1 as currentposition union SELECT name as EnterpriseDBName,'eTurns' as name  ,1 as currentposition from sys.databases SD where  name like 'eTurns' union SELECT 'eTurnsmaster' as EnterpriseDBName,'eTurnsmaster' as name,1 as currentposition union SELECT 'eTurnsmasterdemo' as EnterpriseDBName,'eTurnsmasterdemo' as name,1 as currentposition order by currentposition,name asc";
                return (from x in context.ExecuteStoreQuery<EnterpriseDTO>(Qry)
                        select new EnterpriseDTO
                        {

                            Name = x.Name,
                            EnterpriseDBName = x.EnterpriseDBName,
                            ID = x.ID

                        }).ToList();

            }
        }

        public List<EnterpriseDTO> GetAllEnterpriseForExecutionWithDemo()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                string Qry = @"select EnterpriseDBName,name,2 as currentposition from [" + MasterDbConnectionHelper.GetETurnsMasterDBName() + "].[dbo].[EnterpriseMaster] where isnull(isdeleted,0) =0  order by currentposition,name asc";
                List<EnterpriseDTO> obj = (from x in context.ExecuteStoreQuery<EnterpriseDTO>(Qry)
                                           select new EnterpriseDTO
                                           {

                                               Name = x.Name,
                                               EnterpriseDBName = x.EnterpriseDBName,

                                           }).ToList();
                if (System.Configuration.ConfigurationManager.AppSettings["TemplateDbName"] != null)
                {
                    string EturnsName = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["TemplateDbName"]);
                    obj.Insert(0, new EnterpriseDTO { Name = EturnsName, EnterpriseDBName = EturnsName });

                }
                return obj;
            }
        }

        public List<EnterpriseDTO> GetAllEnterprise(bool? IsDeleted)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return (from x in context.EnterpriseMasters
                        join um in context.UserMasters on x.CreatedBy equals um.ID into x_um_join
                        from x_um in x_um_join.DefaultIfEmpty()
                        join umup in context.UserMasters on x.LastUpdatedBy equals umup.ID into x_umup_join
                        from x_umup in x_umup_join.DefaultIfEmpty()
                        where (IsDeleted == null || x.IsDeleted == IsDeleted)
                        select new EnterpriseDTO
                        {
                            ID = x.ID,
                            Name = x.Name,
                            Address = x.Address,
                            City = x.City,
                            State = x.State,
                            PostalCode = x.PostalCode,
                            Country = x.Country,
                            ContactPhone = x.ContactPhone,
                            ContactEmail = x.ContactEmail,
                            SoftwareBasePrice = x.SoftwareBasePrice,
                            MaxBinsPerBasePrice = x.MaxBinsPerBasePrice,
                            CostPerBin = x.CostPerBin,
                            DiscountPrice1 = x.DiscountPrice1,
                            DiscountPrice2 = x.DiscountPrice2,
                            DiscountPrice3 = x.DiscountPrice3,
                            MaxSubscriptionTier1 = x.MaxSubscriptionTier1,
                            MaxSubscriptionTier2 = x.MaxSubscriptionTier2,
                            MaxSubscriptionTier3 = x.MaxSubscriptionTier3,
                            MaxSubscriptionTier4 = x.MaxSubscriptionTier4,
                            MaxSubscriptionTier5 = x.MaxSubscriptionTier5,
                            MaxSubscriptionTier6 = x.MaxSubscriptionTier6,
                            PriceSubscriptionTier1 = x.PriceSubscriptionTier1,
                            PriceSubscriptionTier2 = x.PriceSubscriptionTier2,
                            PriceSubscriptionTier3 = x.PriceSubscriptionTier3,
                            PriceSubscriptionTier4 = x.PriceSubscriptionTier4,
                            PriceSubscriptionTier5 = x.PriceSubscriptionTier5,
                            PriceSubscriptionTier6 = x.PriceSubscriptionTier6,
                            IncludeLicenseFees = x.IncludeLicenseFees,
                            MaxLicenseTier1 = x.MaxLicenseTier1,
                            MaxLicenseTier2 = x.MaxLicenseTier2,
                            MaxLicenseTier3 = x.MaxLicenseTier3,
                            MaxLicenseTier4 = x.MaxLicenseTier4,
                            MaxLicenseTier5 = x.MaxLicenseTier5,
                            MaxLicenseTier6 = x.MaxLicenseTier6,
                            PriceLicenseTier1 = x.PriceLicenseTier1,
                            PriceLicenseTier2 = x.PriceLicenseTier2,
                            PriceLicenseTier3 = x.PriceLicenseTier3,
                            PriceLicenseTier4 = x.PriceLicenseTier4,
                            PriceLicenseTier5 = x.PriceLicenseTier5,
                            PriceLicenseTier6 = x.PriceLicenseTier6,
                            UDF1 = x.UDF1,
                            UDF2 = x.UDF2,
                            UDF3 = x.UDF3,
                            UDF4 = x.UDF4,
                            UDF5 = x.UDF5,
                            GUID = x.GUID ?? Guid.Empty,
                            Created = x.Created,
                            Updated = x.Updated,
                            CreatedByName = x_um.UserName,
                            UpdatedByName = x_umup.UserName,
                            IsArchived = x.IsArchived ?? false,
                            IsDeleted = x.IsDeleted,
                            CreatedBy = x.CreatedBy,
                            LastUpdatedBy = x.LastUpdatedBy,
                            EnterpriseDBName = x.EnterpriseDBName,
                            //EnterpriseDBConnectionString = x.EnterpriseDBConnectionString,
                            EnterpriseSqlServerName = x.EnterpriseSqlServerName,
                            EnterpriseSqlServerUserName = x.EnterpriseSqlServerUserName,
                            EnterpriseSqlServerPassword = x.EnterpriseSqlServerPassword,
                            IsActive = x.IsActive,
                            EnterPriseDomainURL = x.EnterpriseDomainURL,
                            ArchiveDbName = x.ArchiveDbName,
                            ChangeLogDbName = x.ChangeLogDbName,
                            HistoryDBName = x.HistoryDBName
                        }).ToList();
            }
        }

        public EnterPriseConfigDTO GetEnterpriseConfig(Int64 id)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return (from x in context.EnterPriseConfigs
                        where x.EnterpriseID == id
                        select new EnterPriseConfigDTO
                        {
                            ID = x.ID,
                            EnterPriseID = x.EnterpriseID,
                            ScheduleDaysBefore = x.ScheduleDaysBefore,
                            OperationalHoursBefore = x.OperationalHoursBefore,
                            MileageBefore = x.MileageBefore,
                            ProjectAmountExceed = x.ProjectAmountExceed,
                            ProjectItemQuantitExceed = x.ProjectItemQuantitExceed,
                            ProjectItemAmountExceed = x.ProjectItemAmountExceed,
                            CostDecimalPoints = x.CostDecimalPoints,
                            QuantityDecimalPoints = x.QuantityDecimalPoints,
                            DateFormat = x.DateFormat,
                            NOBackDays = x.NOBackDays,
                            NODaysAve = x.NODaysAve,
                            NOTimes = x.NOTimes,
                            MinPer = x.MinPer,
                            MaxPer = x.MaxPer,
                            CurrencySymbol = x.CurrencySymbol,
                            AEMTPndOrders = x.AEMTPndOrders,
                            AEMTPndRequisition = x.AEMTPndRequisition,
                            AEMTPndTransfer = x.AEMTPndTransfer,
                            AEMTSggstOrdMin = x.AEMTSggstOrdMin,
                            AEMTSggstOrdCrt = x.AEMTSggstOrdCrt,
                            AEMTAssetMntDue = x.AEMTAssetMntDue,
                            AEMTToolsMntDue = x.AEMTToolsMntDue,
                            AEMTItemStockOut = x.AEMTItemStockOut,
                            AEMTCycleCount = x.AEMTCycleCount,
                            AEMTCycleCntItmMiss = x.AEMTCycleCntItmMiss,
                            AEMTItemUsageRpt = x.AEMTItemUsageRpt,
                            AEMTItemReceiveRpt = x.AEMTItemReceiveRpt,
                            GridRefreshTimeInSecond = x.GridRefreshTimeInSecond,
                            DateFormatCSharp = x.DateFormatCSharp,
                            PasswordExpiryDays = x.PasswordExpiryDays,
                        }).FirstOrDefault();
            }
        }

        public DashboardParameterDTO GetDashboardParameterById(Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return (from x in context.DashboardParameters
                        where x.RoomId == RoomID && x.CompanyId == CompanyID
                        select new DashboardParameterDTO
                        {
                            ID = x.ID,
                            RoomId = x.RoomId ?? 0,
                            CompanyId = x.CompanyId,
                            CreatedOn = x.CreatedOn,
                            CreatedBy = x.CreatedBy,
                            UpdatedOn = x.UpdatedOn,
                            UpdatedBy = x.UpdatedBy,
                            TurnsMeasureMethod = x.TurnsMeasureMethod,
                            TurnsMonthsOfUsageToSample = x.TurnsMonthsOfUsageToSample,
                            AUDayOfUsageToSample = x.AUDayOfUsageToSample,
                            AUMeasureMethod = x.AUMeasureMethod,
                            AUDaysOfDailyUsage = x.AUDaysOfDailyUsage,
                            MinMaxMeasureMethod = x.MinMaxMeasureMethod,
                            MinMaxDayOfUsageToSample = x.MinMaxDayOfUsageToSample,
                            MinMaxDayOfAverage = x.MinMaxDayOfAverage,
                            MinMaxMinNumberOfTimesMax = x.MinMaxMinNumberOfTimesMax,
                            MinMaxOptValue1 = x.MinMaxOptValue1,
                            MinMaxOptValue2 = x.MinMaxOptValue2,
                            GraphFromMonth = x.GraphFromMonth,
                            GraphToMonth = x.GraphToMonth,
                            GraphFromYear = x.GraphFromYear,
                            GraphToYear = x.GraphToYear,
                            IsTrendingEnabled = x.IsTrendingEnabled,
                            PieChartmetricOn = x.PieChartmetricOn,
                            TurnsCalculatedStockRoomTurn = x.TurnsCalculatedStockRoomTurn,
                            AUCalculatedDailyUsageOverSample = x.AUCalculatedDailyUsageOverSample,
                            MinMaxCalculatedDailyUsageOverSample = x.MinMaxCalculatedDailyUsageOverSample,
                            MinMaxCalcAvgPullByDay = x.MinMaxCalcAvgPullByDay ?? 0,
                            MinMaxCalcualtedMax = x.MinMaxCalcualtedMax,
                            AutoClassification = x.AutoClassification,
                            MonthlyAverageUsage = x.MonthlyAverageUsage,
                            AnnualCarryingCostPercent = x.AnnualCarryingCostPercent,
                            LargestAnnualCashSavings = x.LargestAnnualCashSavings
                        }).FirstOrDefault();
            }
        }

        public bool InsertEnterpriseConfig(EnterPriseConfigDTO objDTO)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                //Assign & Mapping of the DTO Class property to Entity Class 
                var obj = new EnterPriseConfig
                {
                    ID = objDTO.ID,
                    EnterpriseID = objDTO.EnterPriseID,
                    ScheduleDaysBefore = objDTO.ScheduleDaysBefore,
                    OperationalHoursBefore = objDTO.OperationalHoursBefore,
                    MileageBefore = objDTO.MileageBefore,
                    ProjectAmountExceed = objDTO.ProjectAmountExceed,
                    ProjectItemQuantitExceed = objDTO.ProjectItemQuantitExceed,
                    ProjectItemAmountExceed = objDTO.ProjectItemAmountExceed,
                    CostDecimalPoints = objDTO.CostDecimalPoints,
                    QuantityDecimalPoints = objDTO.QuantityDecimalPoints,
                    DateFormat = objDTO.DateFormat,
                    NOBackDays = objDTO.NOBackDays,
                    NODaysAve = objDTO.NODaysAve,
                    NOTimes = objDTO.NOTimes,
                    MinPer = objDTO.MinPer,
                    MaxPer = objDTO.MaxPer,
                    CurrencySymbol = objDTO.CurrencySymbol,
                    AEMTPndOrders = objDTO.AEMTPndOrders,
                    AEMTPndRequisition = objDTO.AEMTPndRequisition,
                    AEMTPndTransfer = objDTO.AEMTPndTransfer,
                    AEMTSggstOrdMin = objDTO.AEMTSggstOrdMin,
                    AEMTSggstOrdCrt = objDTO.AEMTSggstOrdCrt,
                    AEMTAssetMntDue = objDTO.AEMTAssetMntDue,
                    AEMTToolsMntDue = objDTO.AEMTToolsMntDue,
                    AEMTItemStockOut = objDTO.AEMTItemStockOut,
                    AEMTCycleCount = objDTO.AEMTCycleCount,
                    AEMTCycleCntItmMiss = objDTO.AEMTCycleCntItmMiss,
                    AEMTItemUsageRpt = objDTO.AEMTItemUsageRpt,
                    AEMTItemReceiveRpt = objDTO.AEMTItemReceiveRpt,
                    GridRefreshTimeInSecond = objDTO.GridRefreshTimeInSecond,
                    DateFormatCSharp = objDTO.DateFormatCSharp,
                    PasswordExpiryDays = objDTO.PasswordExpiryDays,
                    PasswordExpiryWarningDays = objDTO.PasswordExpiryWarningDays,
                    PreviousLastAllowedPWD = objDTO.PreviousLastAllowedPWD,

                };

                context.AddToEnterPriseConfigs(obj);
                context.SaveChanges();
            }
            return true;
        }

        public EnterpriseDTO Insert(EnterpriseDTO objDTO)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                //Assign & Mapping of the DTO Class property to Entity Class 
                var obj = new EnterpriseMaster
                {
                    ID = objDTO.ID,
                    Name = objDTO.Name,
                    Address = objDTO.Address,
                    City = objDTO.City,
                    State = objDTO.State,
                    PostalCode = objDTO.PostalCode,
                    Country = objDTO.Country,
                    ContactPhone = objDTO.ContactPhone,
                    ContactEmail = objDTO.ContactEmail,
                    SoftwareBasePrice = objDTO.SoftwareBasePrice,
                    MaxBinsPerBasePrice = objDTO.MaxBinsPerBasePrice,
                    CostPerBin = objDTO.CostPerBin,
                    DiscountPrice1 = objDTO.DiscountPrice1,
                    DiscountPrice2 = objDTO.DiscountPrice2,
                    DiscountPrice3 = objDTO.DiscountPrice3,
                    MaxSubscriptionTier1 = objDTO.MaxSubscriptionTier1,
                    MaxSubscriptionTier2 = objDTO.MaxSubscriptionTier2,
                    MaxSubscriptionTier3 = objDTO.MaxSubscriptionTier3,
                    MaxSubscriptionTier4 = objDTO.MaxSubscriptionTier4,
                    MaxSubscriptionTier5 = objDTO.MaxSubscriptionTier5,
                    MaxSubscriptionTier6 = objDTO.MaxSubscriptionTier6,
                    PriceSubscriptionTier1 = objDTO.PriceSubscriptionTier1,
                    PriceSubscriptionTier2 = objDTO.PriceSubscriptionTier2,
                    PriceSubscriptionTier3 = objDTO.PriceSubscriptionTier3,
                    PriceSubscriptionTier4 = objDTO.PriceSubscriptionTier4,
                    PriceSubscriptionTier5 = objDTO.PriceSubscriptionTier5,
                    PriceSubscriptionTier6 = objDTO.PriceSubscriptionTier6,
                    IncludeLicenseFees = objDTO.IncludeLicenseFees,
                    MaxLicenseTier1 = objDTO.MaxLicenseTier1,
                    MaxLicenseTier2 = objDTO.MaxLicenseTier2,
                    MaxLicenseTier3 = objDTO.MaxLicenseTier3,
                    MaxLicenseTier4 = objDTO.MaxLicenseTier4,
                    MaxLicenseTier5 = objDTO.MaxLicenseTier5,
                    MaxLicenseTier6 = objDTO.MaxLicenseTier6,
                    PriceLicenseTier1 = objDTO.PriceLicenseTier1,
                    PriceLicenseTier2 = objDTO.PriceLicenseTier2,
                    PriceLicenseTier3 = objDTO.PriceLicenseTier3,
                    PriceLicenseTier4 = objDTO.PriceLicenseTier4,
                    PriceLicenseTier5 = objDTO.PriceLicenseTier5,
                    PriceLicenseTier6 = objDTO.PriceLicenseTier6,
                    UDF1 = objDTO.UDF1,
                    UDF2 = objDTO.UDF2,
                    UDF3 = objDTO.UDF3,
                    UDF4 = objDTO.UDF4,
                    UDF5 = objDTO.UDF5,
                    GUID = Guid.NewGuid(),
                    Updated = DateTime.UtcNow,
                    Created = DateTime.UtcNow,
                    CreatedBy = objDTO.CreatedBy,
                    LastUpdatedBy = objDTO.LastUpdatedBy,
                    IsDeleted = false,
                    IsArchived = false,
                    IsActive = objDTO.IsActive,
                    EnterpriseDomainURL = objDTO.EnterPriseDomainURL

                };
                context.AddToEnterpriseMasters(obj);
                context.SaveChanges();
                List<EnterPriseSQLScriptsDTO> lstScripts = new List<EnterPriseSQLScriptsDTO>();
                lstScripts = (from src in context.EnterpriseScriptsMasters
                              where src.IsDeleted == false
                              select new EnterPriseSQLScriptsDTO
                              {
                                  CreatedBy = src.CreatedBy,
                                  CreatedByName = string.Empty,
                                  CreatedDate = src.CreatedDate,
                                  ExecuitionSequence = src.ExecuitionSequence,
                                  IsDeleted = src.IsDeleted,
                                  IsMasterScript = src.IsMasterScript,
                                  ScriptName = src.ScriptName,
                                  ScriptText = src.ScriptText,
                                  SQLScriptID = src.SQLScriptID,
                                  UpdatedBy = src.UpdatedBy,
                                  UpdatedByName = string.Empty,
                                  UpdatedDate = src.UpdatedDate
                              }).OrderBy(t => t.SQLScriptID).ToList();
                foreach (var item in lstScripts)
                {
                    ExecuitScriptOnEnterprises(item.SQLScriptID, obj.ID.ToString());
                }
                //ExecuitScriptOnEnterprises(1, obj.ID.ToString());
                //ExecuitScriptOnEnterprises(2, obj.ID.ToString());
                //ExecuitScriptOnEnterprises(3, obj.ID.ToString());
                //ExecuitScriptOnEnterprises(4, obj.ID.ToString());


                EnterpriseMasterDAL objEnterprise = new EnterpriseMasterDAL();
                //EnterpriseDTO objEnterPrise = new EnterpriseDTO();
                //objEnterPrise = objEnterprise.GetEnterprise(obj.ID);
                string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(obj.EnterpriseDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
                Int64 EnterpriseID = obj.ID;
                SqlHelper.ExecuteNonQuery(EnterpriseDbConnection, "InsertEnterpriseConfig", EnterpriseID);
                for (int i = 1; i < 4; i++)
                {
                    EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new EnterPriseUserMasterDAL();
                    UserMasterDTO objUserMasterDTO = new UserMasterDTO();
                    objUserMasterDTO.CompanyID = 0;
                    objUserMasterDTO.ConfirmPassword = "password";
                    objUserMasterDTO.Created = DateTime.UtcNow;
                    objUserMasterDTO.CreatedBy = objDTO.CreatedBy;
                    objUserMasterDTO.CreatedByName = objDTO.CreatedByName;
                    objUserMasterDTO.Email = objDTO.EnterpriseUserEmail;
                    objUserMasterDTO.EnterpriseDbName = string.Empty;
                    objUserMasterDTO.EnterpriseId = obj.ID;
                    objUserMasterDTO.GUID = Guid.NewGuid();
                    objUserMasterDTO.IsArchived = false;
                    objUserMasterDTO.IsDeleted = false;
                    objUserMasterDTO.LastUpdatedBy = obj.LastUpdatedBy;
                    objUserMasterDTO.Password = objDTO.EnterpriseUserPassword;
                    objUserMasterDTO.Phone = objDTO.ContactPhone;
                    objUserMasterDTO.RoleName = "Enterprise Role";
                    objUserMasterDTO.Updated = DateTime.UtcNow;

                    if (i == 1)
                    {
                        objUserMasterDTO.UserType = 2;
                        objUserMasterDTO.UserName = objDTO.UserName;
                        objUserMasterDTO.RoleID = -2;
                    }
                    else if (i == 2)
                    {
                        objUserMasterDTO.UserType = 4;
                        objUserMasterDTO.UserName = "Enterprise System User";
                        objUserMasterDTO.RoleID = -4;
                    }
                    else if (i == 3)
                    {
                        objUserMasterDTO.UserType = 5;
                        objUserMasterDTO.UserName = "eTurns User";
                        objUserMasterDTO.RoleID = -5;
                    }
                    objUserMasterDTO = objEnterPriseUserMasterDAL.SaveUseronMasterDB(objUserMasterDTO);

                    Int64 NewUserID = objUserMasterDTO.ID;

                    // Insert into Enterprise Config File

                    if (i == 1)
                    {

                        {

                            EnterpriseMaster objEnterpriseMaster = context.EnterpriseMasters.Where(t => t.ID == obj.ID).FirstOrDefault();
                            if (objEnterpriseMaster != null)
                            {
                                objEnterpriseMaster.EnterpriseSuperAdmin = NewUserID;
                            }
                            context.SaveChanges();

                        }
                    }
                    InsertUserInChildDB(NewUserID, objUserMasterDTO, obj.ID);
                }

                return GetEnterprise(obj.ID);
            }
        }

        public void UpdateEnterpriseSuperUser(Int64 NewUserID, UserMasterDTO objUserMasterDTO, EnterpriseDTO objDTO)
        {
            EnterpriseMasterDAL objEnterprise = new EnterpriseMasterDAL();
            EnterpriseDTO obj = new EnterpriseDTO();
            obj = objEnterprise.GetEnterpriseByIdPlain(objDTO.ID);
            string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(obj.EnterpriseDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                UserMaster objUserMaster = context.UserMasters.FirstOrDefault(t => t.ID == objUserMasterDTO.ID);
                objUserMaster.UserName = objDTO.UserName;
                if (objDTO.IsPasswordChanged == "1")
                {
                    objUserMaster.Password = objDTO.EnterpriseUserPassword;
                }
                if (objDTO.IsEmailChanged == "1")
                {
                    objUserMaster.Email = objDTO.EnterpriseUserEmail;
                }
                context.SaveChanges();
            }
            bool IsPasswordChanged = false;
            bool IsEmailChanged = false;

            if (objDTO.IsPasswordChanged == "1" && objDTO.IsEmailChanged == "1")
            {
                SqlHelper.ExecuteNonQuery(EnterpriseDbConnection, CommandType.Text, "UPDATE USERMASTER SET Email='" + objDTO.EnterpriseUserEmail + "', PASSWORD='" + objDTO.EnterpriseUserPassword + "' where ID='" + objUserMasterDTO.ID + "'");
            }
            else if (objDTO.IsPasswordChanged == "1")
            {
                SqlHelper.ExecuteNonQuery(EnterpriseDbConnection, CommandType.Text, "UPDATE USERMASTER SET  PASSWORD='" + objDTO.EnterpriseUserPassword + "' where ID='" + objUserMasterDTO.ID + "'");
            }
            else if (objDTO.IsEmailChanged == "1")
            {
                SqlHelper.ExecuteNonQuery(EnterpriseDbConnection, CommandType.Text, "UPDATE USERMASTER SET Email='" + objDTO.EnterpriseUserEmail + "' where ID='" + objUserMasterDTO.ID + "'");
            }
            
        }

        /// <summary>
        /// Delete Particullar Record
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool Delete(Int64 id, Int64 userid)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                EnterpriseMaster obj = context.EnterpriseMasters.Single(t => t.ID == id);
                obj.IsDeleted = true;
                obj.IsArchived = false;
                obj.Updated = DateTime.UtcNow;
                obj.LastUpdatedBy = userid;
                context.EnterpriseMasters.Attach(obj);
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                context.SaveChanges();
                return true;
            }
        }

        public bool DeleteRecords(string IDs, int userid)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE EnterpriseMaster SET Updated = '" + DateTime.UtcNow.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
                        strQuery += "UPDATE RoleMaster SET Updated = '" + DateTime.UtcNow.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE UserType > 1 AND EnterpriseId =" + item.ToString() + ";";
                        strQuery += "UPDATE userMaster SET Updated = '" + DateTime.UtcNow.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE UserType > 1 AND EnterpriseId =" + item.ToString() + ";";
                        strQuery += "DELETE FROM RoleModuledetails  WHERE EnterpriseID =" + item.ToString() + ";";
                        strQuery += "DELETE FROM UserRoleDetails WHERE EnterpriseID =" + item.ToString() + ";"; ;
                        strQuery += "DELETE FROM UserRoomAccess  WHERE EnterpriseID =" + item.ToString() + ";";
                        strQuery += "DELETE FROM RoleRoomAccess  WHERE EnterpriseID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);
                return true;
            }
        }

        public bool UnDeleteRecords(string IDs, int userid)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE EnterpriseMaster SET Updated = '" + DateTime.UtcNow.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=0 WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);
                return true;
            }
        }

        public string UpdateData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "UPDATE EnterpriseMster SET " + columnName + " = '" + value + "', Updated = DATETIME ('" + System.DateTime.UtcNow.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String)))) + "') WHERE ID=" + id;
                context.ExecuteStoreCommand(strQuery);
            }
            return value;
        }

        public List<EnterpriseDTO> GetEnterPriseByUser(long UserId, long RoleId, long UserType)
        {
            List<EnterpriseDTO> lstEnterprise = new List<EnterpriseDTO>();

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                if (UserType == 1)
                {
                    if (RoleId == -1)
                    {
                        lstEnterprise = (from x in context.ExecuteStoreQuery<EnterpriseDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName FROM EnterpriseMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1")
                                         select new EnterpriseDTO
                                         {
                                             ID = x.ID,
                                             Name = x.Name,
                                             Address = x.Address,
                                             City = x.City,
                                             State = x.State,
                                             PostalCode = x.PostalCode,
                                             Country = x.Country,
                                             ContactPhone = x.ContactPhone,
                                             ContactEmail = x.ContactEmail,
                                             SoftwareBasePrice = x.SoftwareBasePrice,
                                             MaxBinsPerBasePrice = x.MaxBinsPerBasePrice,
                                             CostPerBin = x.CostPerBin,
                                             DiscountPrice1 = x.DiscountPrice1,
                                             DiscountPrice2 = x.DiscountPrice2,
                                             DiscountPrice3 = x.DiscountPrice3,
                                             MaxSubscriptionTier1 = x.MaxSubscriptionTier1,
                                             MaxSubscriptionTier2 = x.MaxSubscriptionTier2,
                                             MaxSubscriptionTier3 = x.MaxSubscriptionTier3,
                                             MaxSubscriptionTier4 = x.MaxSubscriptionTier4,
                                             MaxSubscriptionTier5 = x.MaxSubscriptionTier5,
                                             MaxSubscriptionTier6 = x.MaxSubscriptionTier6,
                                             PriceSubscriptionTier1 = x.PriceSubscriptionTier1,
                                             PriceSubscriptionTier2 = x.PriceSubscriptionTier2,
                                             PriceSubscriptionTier3 = x.PriceSubscriptionTier3,
                                             PriceSubscriptionTier4 = x.PriceSubscriptionTier4,
                                             PriceSubscriptionTier5 = x.PriceSubscriptionTier5,
                                             PriceSubscriptionTier6 = x.PriceSubscriptionTier6,
                                             IncludeLicenseFees = x.IncludeLicenseFees,
                                             MaxLicenseTier1 = x.MaxLicenseTier1,
                                             MaxLicenseTier2 = x.MaxLicenseTier2,
                                             MaxLicenseTier3 = x.MaxLicenseTier3,
                                             MaxLicenseTier4 = x.MaxLicenseTier4,
                                             MaxLicenseTier5 = x.MaxLicenseTier5,
                                             MaxLicenseTier6 = x.MaxLicenseTier6,
                                             PriceLicenseTier1 = x.PriceLicenseTier1,
                                             PriceLicenseTier2 = x.PriceLicenseTier2,
                                             PriceLicenseTier3 = x.PriceLicenseTier3,
                                             PriceLicenseTier4 = x.PriceLicenseTier4,
                                             PriceLicenseTier5 = x.PriceLicenseTier5,
                                             PriceLicenseTier6 = x.PriceLicenseTier6,
                                             UDF1 = x.UDF1,
                                             UDF2 = x.UDF2,
                                             UDF3 = x.UDF3,
                                             UDF4 = x.UDF4,
                                             UDF5 = x.UDF5,
                                             GUID = x.GUID,
                                             Created = x.Created,
                                             Updated = x.Updated,
                                             CreatedByName = x.CreatedByName,
                                             UpdatedByName = x.UpdatedByName,
                                             EnterpriseDBName = x.EnterpriseDBName,
                                             //EnterpriseDBConnectionString = x.EnterpriseDBConnectionString,
                                             IsArchived = x.IsArchived ?? false,
                                             IsDeleted = x.IsDeleted,
                                             CreatedBy = x.CreatedBy,
                                             LastUpdatedBy = x.LastUpdatedBy,
                                             EnterPriseDomainURL = x.EnterPriseDomainURL,
                                             ArchiveDbName = x.ArchiveDbName,
                                             ChangeLogDbName = x.ChangeLogDbName,
                                             HistoryDBName = x.HistoryDBName
                                         }).ToList();
                    }
                    else
                    {
                        lstEnterprise = (from x in context.ExecuteStoreQuery<EnterpriseDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName FROM EnterpriseMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.ID IN (select distinct EnterpriseId from UserRoleDetails where UserID=" + UserId + ")")
                                         select new EnterpriseDTO
                                         {
                                             ID = x.ID,
                                             Name = x.Name,
                                             Address = x.Address,
                                             City = x.City,
                                             State = x.State,
                                             PostalCode = x.PostalCode,
                                             Country = x.Country,
                                             ContactPhone = x.ContactPhone,
                                             ContactEmail = x.ContactEmail,
                                             SoftwareBasePrice = x.SoftwareBasePrice,
                                             MaxBinsPerBasePrice = x.MaxBinsPerBasePrice,
                                             CostPerBin = x.CostPerBin,
                                             DiscountPrice1 = x.DiscountPrice1,
                                             DiscountPrice2 = x.DiscountPrice2,
                                             DiscountPrice3 = x.DiscountPrice3,
                                             MaxSubscriptionTier1 = x.MaxSubscriptionTier1,
                                             MaxSubscriptionTier2 = x.MaxSubscriptionTier2,
                                             MaxSubscriptionTier3 = x.MaxSubscriptionTier3,
                                             MaxSubscriptionTier4 = x.MaxSubscriptionTier4,
                                             MaxSubscriptionTier5 = x.MaxSubscriptionTier5,
                                             MaxSubscriptionTier6 = x.MaxSubscriptionTier6,
                                             PriceSubscriptionTier1 = x.PriceSubscriptionTier1,
                                             PriceSubscriptionTier2 = x.PriceSubscriptionTier2,
                                             PriceSubscriptionTier3 = x.PriceSubscriptionTier3,
                                             PriceSubscriptionTier4 = x.PriceSubscriptionTier4,
                                             PriceSubscriptionTier5 = x.PriceSubscriptionTier5,
                                             PriceSubscriptionTier6 = x.PriceSubscriptionTier6,
                                             IncludeLicenseFees = x.IncludeLicenseFees,
                                             MaxLicenseTier1 = x.MaxLicenseTier1,
                                             MaxLicenseTier2 = x.MaxLicenseTier2,
                                             MaxLicenseTier3 = x.MaxLicenseTier3,
                                             MaxLicenseTier4 = x.MaxLicenseTier4,
                                             MaxLicenseTier5 = x.MaxLicenseTier5,
                                             MaxLicenseTier6 = x.MaxLicenseTier6,
                                             PriceLicenseTier1 = x.PriceLicenseTier1,
                                             PriceLicenseTier2 = x.PriceLicenseTier2,
                                             PriceLicenseTier3 = x.PriceLicenseTier3,
                                             PriceLicenseTier4 = x.PriceLicenseTier4,
                                             PriceLicenseTier5 = x.PriceLicenseTier5,
                                             PriceLicenseTier6 = x.PriceLicenseTier6,
                                             UDF1 = x.UDF1,
                                             UDF2 = x.UDF2,
                                             UDF3 = x.UDF3,
                                             UDF4 = x.UDF4,
                                             UDF5 = x.UDF5,
                                             GUID = x.GUID,
                                             Created = x.Created,
                                             Updated = x.Updated,
                                             CreatedByName = x.CreatedByName,
                                             UpdatedByName = x.UpdatedByName,
                                             EnterpriseDBName = x.EnterpriseDBName,
                                             EnterPriseDomainURL = x.EnterPriseDomainURL,
                                             ArchiveDbName = x.ArchiveDbName,
                                             ChangeLogDbName = x.ChangeLogDbName,
                                             HistoryDBName = x.HistoryDBName
                                         }).ToList();
                    }
                }
                else
                {
                    lstEnterprise = (from x in context.ExecuteStoreQuery<EnterpriseDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName FROM EnterpriseMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1")

                                     select new EnterpriseDTO
                                     {
                                         ID = x.ID,
                                         Name = x.Name,
                                         Address = x.Address,
                                         City = x.City,
                                         State = x.State,
                                         PostalCode = x.PostalCode,
                                         Country = x.Country,
                                         ContactPhone = x.ContactPhone,
                                         ContactEmail = x.ContactEmail,
                                         SoftwareBasePrice = x.SoftwareBasePrice,
                                         MaxBinsPerBasePrice = x.MaxBinsPerBasePrice,
                                         CostPerBin = x.CostPerBin,
                                         DiscountPrice1 = x.DiscountPrice1,
                                         DiscountPrice2 = x.DiscountPrice2,
                                         DiscountPrice3 = x.DiscountPrice3,
                                         MaxSubscriptionTier1 = x.MaxSubscriptionTier1,
                                         MaxSubscriptionTier2 = x.MaxSubscriptionTier2,
                                         MaxSubscriptionTier3 = x.MaxSubscriptionTier3,
                                         MaxSubscriptionTier4 = x.MaxSubscriptionTier4,
                                         MaxSubscriptionTier5 = x.MaxSubscriptionTier5,
                                         MaxSubscriptionTier6 = x.MaxSubscriptionTier6,
                                         PriceSubscriptionTier1 = x.PriceSubscriptionTier1,
                                         PriceSubscriptionTier2 = x.PriceSubscriptionTier2,
                                         PriceSubscriptionTier3 = x.PriceSubscriptionTier3,
                                         PriceSubscriptionTier4 = x.PriceSubscriptionTier4,
                                         PriceSubscriptionTier5 = x.PriceSubscriptionTier5,
                                         PriceSubscriptionTier6 = x.PriceSubscriptionTier6,
                                         IncludeLicenseFees = x.IncludeLicenseFees,
                                         MaxLicenseTier1 = x.MaxLicenseTier1,
                                         MaxLicenseTier2 = x.MaxLicenseTier2,
                                         MaxLicenseTier3 = x.MaxLicenseTier3,
                                         MaxLicenseTier4 = x.MaxLicenseTier4,
                                         MaxLicenseTier5 = x.MaxLicenseTier5,
                                         MaxLicenseTier6 = x.MaxLicenseTier6,
                                         PriceLicenseTier1 = x.PriceLicenseTier1,
                                         PriceLicenseTier2 = x.PriceLicenseTier2,
                                         PriceLicenseTier3 = x.PriceLicenseTier3,
                                         PriceLicenseTier4 = x.PriceLicenseTier4,
                                         PriceLicenseTier5 = x.PriceLicenseTier5,
                                         PriceLicenseTier6 = x.PriceLicenseTier6,
                                         UDF1 = x.UDF1,
                                         UDF2 = x.UDF2,
                                         UDF3 = x.UDF3,
                                         UDF4 = x.UDF4,
                                         UDF5 = x.UDF5,
                                         GUID = x.GUID,
                                         Created = x.Created,
                                         Updated = x.Updated,
                                         CreatedByName = x.CreatedByName,
                                         UpdatedByName = x.UpdatedByName,
                                         EnterpriseDBName = x.EnterpriseDBName,
                                         EnterPriseDomainURL = x.EnterPriseDomainURL,
                                         ArchiveDbName = x.ArchiveDbName,
                                         ChangeLogDbName = x.ChangeLogDbName,
                                         HistoryDBName = x.HistoryDBName
                                     }).ToList();
                }
                return lstEnterprise;
            }

        }

        public List<long> GetDistinctMasterCompanyID(long UserId, long RoleId, long UserType, long EnterPriceID)
        {
            List<long> lstCompany = new List<long>();
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                if (UserType == 1)
                {
                    if (RoleId == -1)
                    {
                        lstCompany = (from x in context.ExecuteStoreQuery<long>(@"SELECT distinct CompanyId from UserRoleDetails WHERE EnterpriseId = " + EnterPriceID)
                                      select x).ToList();
                    }
                    else
                    {
                        lstCompany = (from x in context.ExecuteStoreQuery<long>(@"SELECT distinct CompanyId from UserRoleDetails WHERE EnterpriseId = " + EnterPriceID + " AND UserID=" + UserId)
                                      select x).ToList();
                    }
                }
                else
                {
                    lstCompany = (from x in context.ExecuteStoreQuery<long>(@"SELECT distinct CompanyId from UserRoleDetails WHERE EnterpriseId = " + EnterPriceID + " AND UserID=" + UserId)
                                  select x).ToList();
                }
                return lstCompany;
            }

        }

        public List<long> GetDistinctMasterRoomID(long UserId, long RoleId, long UserType, long EnterPriceID, long CompanyID)
        {
            List<long> lstCompany = new List<long>();
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                if (UserType == 1)
                {
                    if (RoleId == -1)
                    {
                        lstCompany = (from x in context.ExecuteStoreQuery<long>(@"SELECT distinct RoomId from UserRoleDetails WHERE EnterpriseId = " + EnterPriceID + " AND CompanyId=" + CompanyID)
                                      select x).ToList();
                    }
                    else
                    {
                        lstCompany = (from x in context.ExecuteStoreQuery<long>(@"SELECT distinct RoomId from UserRoleDetails WHERE EnterpriseId = " + EnterPriceID + " AND CompanyId=" + CompanyID + " AND UserID=" + UserId)
                                      select x).ToList();
                    }
                }
                else
                {
                    lstCompany = (from x in context.ExecuteStoreQuery<long>(@"SELECT distinct RoomId from UserRoleDetails WHERE EnterpriseId = " + EnterPriceID + " AND CompanyId=" + CompanyID + " AND UserID=" + UserId)
                                  select x).ToList();
                }
                return lstCompany;
            }

        }

        public List<CompanyMasterDTO> GetCompaniesOfEnterPrise(long[] EnterpriseIds)
        {
            List<CompanyMasterDTO> lstCompanies = new List<CompanyMasterDTO>();
            foreach (long eid in EnterpriseIds)
            {
                EnterpriseDTO objEnterprise = new EnterpriseDTO();
                objEnterprise = GetEnterpriseByIdPlain(eid);
                if (objEnterprise != null)
                {
                    DataSet dsCompanies = new DataSet();
                    string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(objEnterprise.EnterpriseDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));

                    dsCompanies = SqlHelper.ExecuteDataset(EnterpriseDbConnection, CommandType.Text, "select * from CompanyMaster");
                    if (dsCompanies != null && dsCompanies.Tables.Count > 0)
                    {
                        long templong = 0;
                        bool tempbool = false;
                        DataTable dtcompanies = dsCompanies.Tables[0];
                        if (dtcompanies.Rows.Count > 0)
                        {
                            foreach (DataRow drcompay in dtcompanies.Rows)
                            {
                                CompanyMasterDTO objCompanyMasterDTO = new CompanyMasterDTO();
                                if (dtcompanies.Columns.Contains("ID"))
                                {
                                    templong = 0;
                                    long.TryParse(Convert.ToString(drcompay["ID"]), out templong);
                                    objCompanyMasterDTO.ID = templong;
                                }
                                if (dtcompanies.Columns.Contains("Name"))
                                {
                                    objCompanyMasterDTO.Name = Convert.ToString(drcompay["Name"]);
                                }
                                if (dtcompanies.Columns.Contains("IsDeleted"))
                                {
                                    tempbool = false;
                                    bool.TryParse(Convert.ToString(drcompay["IsDeleted"]), out tempbool);
                                    objCompanyMasterDTO.IsDeleted = tempbool;
                                }
                                if (dtcompanies.Columns.Contains("IsActive"))
                                {
                                    tempbool = false;
                                    bool.TryParse(Convert.ToString(drcompay["IsActive"]), out tempbool);
                                    objCompanyMasterDTO.IsActive = tempbool;
                                }
                                //IsActive;IsDeleted
                                objCompanyMasterDTO.EnterPriseId = eid;
                                objCompanyMasterDTO.EnterPriseName = objEnterprise.Name;
                                lstCompanies.Add(objCompanyMasterDTO);
                            }
                        }

                    }
                }

            }
            return lstCompanies;
        }

        public List<RoleCompanyMasterDTO> GetCompaniesOfEnterPriseNew(long[] EnterpriseIds)
        {
            using (var contect = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return (from cm in contect.MstCompanyMasters
                        join em in contect.EnterpriseMasters on cm.EnterpriseID equals em.ID
                        where EnterpriseIds.Contains(cm.EnterpriseID) && (cm.IsDeleted ?? false) == false
                        select new RoleCompanyMasterDTO
                        {
                            ID = cm.CompanyID ?? 0,
                            Name = cm.Name,
                            IsDeleted = cm.IsDeleted,
                            IsActive = cm.IsActive,
                            EnterPriseId = em.ID,
                            EnterPriseName = em.Name
                        }).ToList();
            }
        }

        public List<RoomDTO> GetRoomsByCompany(List<CompanyMasterDTO> lstCompanies)
        {
            List<RoomDTO> lstRooms = new List<RoomDTO>();
            long[] EnterpriseIds = lstCompanies.Select(t => t.EnterPriseId).Distinct().ToArray();
            foreach (long eid in EnterpriseIds)
            {
                string strCompanies = string.Join(",", lstCompanies.Where(t => t.EnterPriseId == eid).Select(t => t.ID).ToArray());
                EnterpriseDTO objEnterprise = new EnterpriseDTO();
                objEnterprise = GetEnterprise(eid);
                if (objEnterprise != null)
                {
                    if (!string.IsNullOrWhiteSpace(objEnterprise.EnterpriseDBName))
                    {
                        DataSet dsRooms = new DataSet();
                        string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(objEnterprise.EnterpriseDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
                        dsRooms = SqlHelper.ExecuteDataset(EnterpriseDbConnection, CommandType.Text, " SELECT rm.[ID] as RoomId,rm.[RoomName],cm.ID as CompanyId,cm.Name as CompanyName,rm.[CompanyName] as compName,rm.[ContactName],rm.[StreetAddress],rm.[City],rm.[State],rm.[PostalCode],rm.[Country],rm.[PhoneNo],rm.[Email],rm.[InvoiceBranch],rm.[CustomerNumber],rm.[BlanketPO],rm.[IsConsignment],rm.[IseVMI],rm.[IsTax1Parts],rm.[IsTax1Labor],rm.[Tax1Name],rm.[Tax1Rate],rm.[IsTax2Parts],rm.[IsTax2Labor],rm.[Tax2Name],rm.[Tax2Rate],rm.[ReplineshmentRoom],rm.[IsTrending],rm.[SourceOfTrending],rm.[TrendingFormula],rm.[TrendingFormulaType],rm.[TrendingFormulaDays],rm.[TrendingFormulaOverDays],rm.[SuggestedOrder],rm.[SuggestedTransfer],rm.[AverageUsageFormula],rm.[MethodOfValuingInventory],rm.[AutoCreateTransferFrequency],rm.[AutoCreateTransferTime],rm.[AutoCreateTransferSubmit],rm.[IsActive],rm.[LicenseBilled],rm.[NextCountNo],rm.[NextOrderNo],rm.[NextRequisitionNo],rm.[NextStagingNo],rm.[NextTransferNo],rm.[NextWorkOrderNo],rm.[RoomGrouping],rm.[Created],rm.[Updated],rm.[CreatedBy],rm.[LastUpdatedBy],rm.[Room],rm.[IsDeleted],rm.[MaxOrderSize],rm.[HighPO],rm.[HighJob],rm.[HighTransfer],rm.[HighCount],rm.[GlobMarkupParts],rm.[GlobMarkupLabor],rm.[UniqueID],rm.[CostCenter],rm.[GXPRConsJob],rm.[IsArchived],rm.[GUID],rm.[IsTax2onTax1],rm.[TrendingFormulaAvgDays],rm.[TrendingFormulaCounts],rm.[TransferFrequencyOption],rm.[TransferFrequencyDays],rm.[TransferFrequencyMonth],rm.[TransferFrequencyNumber],rm.[TransferFrequencyWeek],rm.[TransferFrequencyMainOption],rm.[TrendingSampleSize],rm.[TrendingSampleSizeDivisor],rm.[AverageUsageSampleSize],rm.[AverageUsageSampleSizeDivisor],rm.[AverageUsageTransactions],rm.[UDF1],rm.[UDF2],rm.[UDF3],rm.[UDF4],rm.[UDF5],rm.[UDF6],rm.[UDF7],rm.[UDF8],rm.[UDF9],rm.[UDF10],rm.[CompanyID],rm.[DefaultSupplierID],rm.[NextAssetNo],rm.[NextBinNo],rm.[NextKitNo],rm.[NextItemNo],rm.[NextProjectSpendNo],rm.[NextToolNo],rm.[InventoryConsuptionMethod],rm.[ReplenishmentType],rm.[DefaultBinID],rm.[IsRoomActive] from Room as rm inner join CompanyMaster as cm on rm.CompanyID = cm.ID Where rm.CompanyID in (" + strCompanies + ") ");
                        if (dsRooms != null && dsRooms.Tables.Count > 0)
                        {
                            long templong = 0;
                            bool tempbool = false;
                            DataTable dtRooms = dsRooms.Tables[0];
                            if (dtRooms.Rows.Count > 0)
                            {
                                foreach (DataRow drcompay in dtRooms.Rows)
                                {
                                    RoomDTO objRoomDTO = new RoomDTO();
                                    if (dtRooms.Columns.Contains("RoomId"))
                                    {
                                        templong = 0;
                                        long.TryParse(Convert.ToString(drcompay["RoomId"]), out templong);
                                        objRoomDTO.ID = templong;
                                    }
                                    if (dtRooms.Columns.Contains("CompanyId"))
                                    {
                                        templong = 0;
                                        long.TryParse(Convert.ToString(drcompay["CompanyId"]), out templong);
                                        objRoomDTO.CompanyID = templong;
                                    }
                                    if (dtRooms.Columns.Contains("RoomName"))
                                    {
                                        objRoomDTO.RoomName = Convert.ToString(drcompay["RoomName"]);
                                    }
                                    if (dtRooms.Columns.Contains("IsDeleted"))
                                    {
                                        tempbool = false;
                                        bool.TryParse(Convert.ToString(drcompay["IsDeleted"]), out tempbool);
                                        objRoomDTO.IsDeleted = tempbool;

                                    }
                                    if (dtRooms.Columns.Contains("IsRoomActive"))
                                    {
                                        tempbool = false;
                                        bool.TryParse(Convert.ToString(drcompay["IsRoomActive"]), out tempbool);
                                        objRoomDTO.IsRoomActive = tempbool;
                                    }
                                    if (dtRooms.Columns.Contains("CompanyName"))
                                    {
                                        objRoomDTO.CompanyName = Convert.ToString(drcompay["CompanyName"]);
                                    }
                                    objRoomDTO.EnterpriseName = objEnterprise.Name;

                                    objRoomDTO.EnterpriseId = eid;
                                    lstRooms.Add(objRoomDTO);
                                }
                            }
                        }
                    }
                }

            }
            return lstRooms;
        }

        public List<RoleDetailsRoom> GetRoomsByCompanyNew(List<CompanyMasterDTO> lstCompanies)
        {
            long[] eps = lstCompanies.Select(t => t.EnterPriseId).Distinct().ToArray();
            long[] cps = lstCompanies.Select(t => t.ID).Distinct().ToArray();
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return (from rm in context.MstRooms
                        join cm in context.MstCompanyMasters on new { rm.EnterpriseID, rm.CompanyID } equals new { cm.EnterpriseID, cm.CompanyID }
                        where eps.Contains(rm.EnterpriseID) && cps.Contains(rm.CompanyID ?? 0) && cm.IsDeleted == false && rm.IsDeleted == false
                        select new RoleDetailsRoom
                        {
                            ID = rm.RoomID ?? 0,
                            CompanyID = rm.CompanyID ?? 0,
                            EnterpriseId = rm.EnterpriseID,
                            IsDeleted = rm.IsDeleted,
                            RoomName = rm.RoomName,
                            IsRoomActive = rm.IsRoomActive,
                            CompanyName = cm.Name

                        }).ToList();

            }
        }

        public IEnumerable<EnterpriseDTO> GetAllEnterprises(bool IsArchived, bool IsDeleted)
        {
            IEnumerable<EnterpriseDTO> lstAllEnterprises = null;
            try
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    if (IsArchived == false && IsDeleted == false)
                    {
                        lstAllEnterprises = (from x in context.EnterpriseMasters
                                             join um in context.UserMasters on x.CreatedBy equals um.ID into x_um_join
                                             from x_um in x_um_join.DefaultIfEmpty()
                                             join umup in context.UserMasters on x.LastUpdatedBy equals umup.ID into x_umup_join
                                             from x_umup in x_umup_join.DefaultIfEmpty()
                                             where (x.IsArchived ?? false) == false && x.IsDeleted == false
                                             select new EnterpriseDTO
                                             {
                                                 ID = x.ID,
                                                 Name = x.Name,
                                                 Address = x.Address,
                                                 City = x.City,
                                                 State = x.State,
                                                 PostalCode = x.PostalCode,
                                                 Country = x.Country,
                                                 ContactPhone = x.ContactPhone,
                                                 ContactEmail = x.ContactEmail,
                                                 SoftwareBasePrice = x.SoftwareBasePrice,
                                                 MaxBinsPerBasePrice = x.MaxBinsPerBasePrice,
                                                 CostPerBin = x.CostPerBin,
                                                 DiscountPrice1 = x.DiscountPrice1,
                                                 DiscountPrice2 = x.DiscountPrice2,
                                                 DiscountPrice3 = x.DiscountPrice3,
                                                 MaxSubscriptionTier1 = x.MaxSubscriptionTier1,
                                                 MaxSubscriptionTier2 = x.MaxSubscriptionTier2,
                                                 MaxSubscriptionTier3 = x.MaxSubscriptionTier3,
                                                 MaxSubscriptionTier4 = x.MaxSubscriptionTier4,
                                                 MaxSubscriptionTier5 = x.MaxSubscriptionTier5,
                                                 MaxSubscriptionTier6 = x.MaxSubscriptionTier6,
                                                 PriceSubscriptionTier1 = x.PriceSubscriptionTier1,
                                                 PriceSubscriptionTier2 = x.PriceSubscriptionTier2,
                                                 PriceSubscriptionTier3 = x.PriceSubscriptionTier3,
                                                 PriceSubscriptionTier4 = x.PriceSubscriptionTier4,
                                                 PriceSubscriptionTier5 = x.PriceSubscriptionTier5,
                                                 PriceSubscriptionTier6 = x.PriceSubscriptionTier6,
                                                 IncludeLicenseFees = x.IncludeLicenseFees,
                                                 MaxLicenseTier1 = x.MaxLicenseTier1,
                                                 MaxLicenseTier2 = x.MaxLicenseTier2,
                                                 MaxLicenseTier3 = x.MaxLicenseTier3,
                                                 MaxLicenseTier4 = x.MaxLicenseTier4,
                                                 MaxLicenseTier5 = x.MaxLicenseTier5,
                                                 MaxLicenseTier6 = x.MaxLicenseTier6,
                                                 PriceLicenseTier1 = x.PriceLicenseTier1,
                                                 PriceLicenseTier2 = x.PriceLicenseTier2,
                                                 PriceLicenseTier3 = x.PriceLicenseTier3,
                                                 PriceLicenseTier4 = x.PriceLicenseTier4,
                                                 PriceLicenseTier5 = x.PriceLicenseTier5,
                                                 PriceLicenseTier6 = x.PriceLicenseTier6,
                                                 UDF1 = x.UDF1,
                                                 UDF2 = x.UDF2,
                                                 UDF3 = x.UDF3,
                                                 UDF4 = x.UDF4,
                                                 UDF5 = x.UDF5,
                                                 GUID = x.GUID ?? Guid.Empty,
                                                 Created = x.Created,
                                                 Updated = x.Updated,
                                                 CreatedByName = x_um.UserName,
                                                 UpdatedByName = x_umup.UserName,
                                                 IsArchived = x.IsArchived ?? false,
                                                 IsDeleted = x.IsDeleted,
                                                 CreatedBy = x.CreatedBy,
                                                 LastUpdatedBy = x.LastUpdatedBy,
                                                 EnterpriseDBName = x.EnterpriseDBName,
                                                 //EnterpriseDBConnectionString = x.EnterpriseDBConnectionString,
                                                 EnterpriseSqlServerName = x.EnterpriseSqlServerName,
                                                 EnterpriseSqlServerUserName = x.EnterpriseSqlServerUserName,
                                                 EnterpriseSqlServerPassword = x.EnterpriseSqlServerPassword,
                                                 EnterpriseLogo = x.EnterPriseLogo,
                                                 EnterPriseDomainURL = x.EnterpriseDomainURL,
                                                 ArchiveDbName = x.ArchiveDbName,
                                                 ChangeLogDbName = x.ChangeLogDbName,
                                                 HistoryDBName = x.HistoryDBName
                                             }).ToList();
                    }
                    else
                    {
                        if (IsArchived && IsDeleted)
                        {
                            lstAllEnterprises = (from x in context.EnterpriseMasters
                                                 join um in context.UserMasters on x.CreatedBy equals um.ID into x_um_join
                                                 from x_um in x_um_join.DefaultIfEmpty()
                                                 join umup in context.UserMasters on x.LastUpdatedBy equals umup.ID into x_umup_join
                                                 from x_umup in x_umup_join.DefaultIfEmpty()
                                                 where (x.IsArchived ?? false) == true && x.IsDeleted == true
                                                 select new EnterpriseDTO
                                                 {
                                                     ID = x.ID,
                                                     Name = x.Name,
                                                     Address = x.Address,
                                                     City = x.City,
                                                     State = x.State,
                                                     PostalCode = x.PostalCode,
                                                     Country = x.Country,
                                                     ContactPhone = x.ContactPhone,
                                                     ContactEmail = x.ContactEmail,
                                                     SoftwareBasePrice = x.SoftwareBasePrice,
                                                     MaxBinsPerBasePrice = x.MaxBinsPerBasePrice,
                                                     CostPerBin = x.CostPerBin,
                                                     DiscountPrice1 = x.DiscountPrice1,
                                                     DiscountPrice2 = x.DiscountPrice2,
                                                     DiscountPrice3 = x.DiscountPrice3,
                                                     MaxSubscriptionTier1 = x.MaxSubscriptionTier1,
                                                     MaxSubscriptionTier2 = x.MaxSubscriptionTier2,
                                                     MaxSubscriptionTier3 = x.MaxSubscriptionTier3,
                                                     MaxSubscriptionTier4 = x.MaxSubscriptionTier4,
                                                     MaxSubscriptionTier5 = x.MaxSubscriptionTier5,
                                                     MaxSubscriptionTier6 = x.MaxSubscriptionTier6,
                                                     PriceSubscriptionTier1 = x.PriceSubscriptionTier1,
                                                     PriceSubscriptionTier2 = x.PriceSubscriptionTier2,
                                                     PriceSubscriptionTier3 = x.PriceSubscriptionTier3,
                                                     PriceSubscriptionTier4 = x.PriceSubscriptionTier4,
                                                     PriceSubscriptionTier5 = x.PriceSubscriptionTier5,
                                                     PriceSubscriptionTier6 = x.PriceSubscriptionTier6,
                                                     IncludeLicenseFees = x.IncludeLicenseFees,
                                                     MaxLicenseTier1 = x.MaxLicenseTier1,
                                                     MaxLicenseTier2 = x.MaxLicenseTier2,
                                                     MaxLicenseTier3 = x.MaxLicenseTier3,
                                                     MaxLicenseTier4 = x.MaxLicenseTier4,
                                                     MaxLicenseTier5 = x.MaxLicenseTier5,
                                                     MaxLicenseTier6 = x.MaxLicenseTier6,
                                                     PriceLicenseTier1 = x.PriceLicenseTier1,
                                                     PriceLicenseTier2 = x.PriceLicenseTier2,
                                                     PriceLicenseTier3 = x.PriceLicenseTier3,
                                                     PriceLicenseTier4 = x.PriceLicenseTier4,
                                                     PriceLicenseTier5 = x.PriceLicenseTier5,
                                                     PriceLicenseTier6 = x.PriceLicenseTier6,
                                                     UDF1 = x.UDF1,
                                                     UDF2 = x.UDF2,
                                                     UDF3 = x.UDF3,
                                                     UDF4 = x.UDF4,
                                                     UDF5 = x.UDF5,
                                                     GUID = x.GUID ?? Guid.Empty,
                                                     Created = x.Created,
                                                     Updated = x.Updated,
                                                     CreatedByName = x_um.UserName,
                                                     UpdatedByName = x_umup.UserName,
                                                     EnterpriseDBName = x.EnterpriseDBName,
                                                     //EnterpriseDBConnectionString = x.EnterpriseDBConnectionString,
                                                     IsArchived = x.IsArchived ?? false,
                                                     IsDeleted = x.IsDeleted,
                                                     CreatedBy = x.CreatedBy,
                                                     LastUpdatedBy = x.LastUpdatedBy,
                                                     EnterPriseDomainURL = x.EnterpriseDomainURL,
                                                     ArchiveDbName = x.ArchiveDbName,
                                                     ChangeLogDbName = x.ChangeLogDbName,
                                                     HistoryDBName = x.HistoryDBName
                                                 }).ToList();
                        }
                        else if (IsArchived)
                        {
                            lstAllEnterprises = (from x in context.EnterpriseMasters
                                                 join um in context.UserMasters on x.CreatedBy equals um.ID into x_um_join
                                                 from x_um in x_um_join.DefaultIfEmpty()
                                                 join umup in context.UserMasters on x.LastUpdatedBy equals umup.ID into x_umup_join
                                                 from x_umup in x_umup_join.DefaultIfEmpty()
                                                 where (x.IsArchived ?? false) == true
                                                 select new EnterpriseDTO
                                                 {
                                                     ID = x.ID,
                                                     Name = x.Name,
                                                     Address = x.Address,
                                                     City = x.City,
                                                     State = x.State,
                                                     PostalCode = x.PostalCode,
                                                     Country = x.Country,
                                                     ContactPhone = x.ContactPhone,
                                                     ContactEmail = x.ContactEmail,
                                                     SoftwareBasePrice = x.SoftwareBasePrice,
                                                     MaxBinsPerBasePrice = x.MaxBinsPerBasePrice,
                                                     CostPerBin = x.CostPerBin,
                                                     DiscountPrice1 = x.DiscountPrice1,
                                                     DiscountPrice2 = x.DiscountPrice2,
                                                     DiscountPrice3 = x.DiscountPrice3,
                                                     MaxSubscriptionTier1 = x.MaxSubscriptionTier1,
                                                     MaxSubscriptionTier2 = x.MaxSubscriptionTier2,
                                                     MaxSubscriptionTier3 = x.MaxSubscriptionTier3,
                                                     MaxSubscriptionTier4 = x.MaxSubscriptionTier4,
                                                     MaxSubscriptionTier5 = x.MaxSubscriptionTier5,
                                                     MaxSubscriptionTier6 = x.MaxSubscriptionTier6,
                                                     PriceSubscriptionTier1 = x.PriceSubscriptionTier1,
                                                     PriceSubscriptionTier2 = x.PriceSubscriptionTier2,
                                                     PriceSubscriptionTier3 = x.PriceSubscriptionTier3,
                                                     PriceSubscriptionTier4 = x.PriceSubscriptionTier4,
                                                     PriceSubscriptionTier5 = x.PriceSubscriptionTier5,
                                                     PriceSubscriptionTier6 = x.PriceSubscriptionTier6,
                                                     IncludeLicenseFees = x.IncludeLicenseFees,
                                                     MaxLicenseTier1 = x.MaxLicenseTier1,
                                                     MaxLicenseTier2 = x.MaxLicenseTier2,
                                                     MaxLicenseTier3 = x.MaxLicenseTier3,
                                                     MaxLicenseTier4 = x.MaxLicenseTier4,
                                                     MaxLicenseTier5 = x.MaxLicenseTier5,
                                                     MaxLicenseTier6 = x.MaxLicenseTier6,
                                                     PriceLicenseTier1 = x.PriceLicenseTier1,
                                                     PriceLicenseTier2 = x.PriceLicenseTier2,
                                                     PriceLicenseTier3 = x.PriceLicenseTier3,
                                                     PriceLicenseTier4 = x.PriceLicenseTier4,
                                                     PriceLicenseTier5 = x.PriceLicenseTier5,
                                                     PriceLicenseTier6 = x.PriceLicenseTier6,
                                                     UDF1 = x.UDF1,
                                                     UDF2 = x.UDF2,
                                                     UDF3 = x.UDF3,
                                                     UDF4 = x.UDF4,
                                                     UDF5 = x.UDF5,
                                                     GUID = x.GUID ?? Guid.Empty,
                                                     Created = x.Created,
                                                     Updated = x.Updated,
                                                     CreatedByName = x_um.UserName,
                                                     UpdatedByName = x_umup.UserName,
                                                     EnterpriseDBName = x.EnterpriseDBName,
                                                     //EnterpriseDBConnectionString = x.EnterpriseDBConnectionString,
                                                     IsArchived = x.IsArchived ?? false,
                                                     IsDeleted = x.IsDeleted,
                                                     CreatedBy = x.CreatedBy,
                                                     LastUpdatedBy = x.LastUpdatedBy,
                                                     EnterPriseDomainURL = x.EnterpriseDomainURL,
                                                     ArchiveDbName = x.ArchiveDbName,
                                                     ChangeLogDbName = x.ChangeLogDbName,
                                                     HistoryDBName = x.HistoryDBName
                                                 }).ToList();
                        }
                        else if (IsDeleted)
                        {
                            lstAllEnterprises = (from x in context.EnterpriseMasters
                                                 join um in context.UserMasters on x.CreatedBy equals um.ID into x_um_join
                                                 from x_um in x_um_join.DefaultIfEmpty()
                                                 join umup in context.UserMasters on x.LastUpdatedBy equals umup.ID into x_umup_join
                                                 from x_umup in x_umup_join.DefaultIfEmpty()
                                                 where x.IsDeleted == true
                                                 select new EnterpriseDTO
                                                 {
                                                     ID = x.ID,
                                                     Name = x.Name,
                                                     Address = x.Address,
                                                     City = x.City,
                                                     State = x.State,
                                                     PostalCode = x.PostalCode,
                                                     Country = x.Country,
                                                     ContactPhone = x.ContactPhone,
                                                     ContactEmail = x.ContactEmail,
                                                     SoftwareBasePrice = x.SoftwareBasePrice,
                                                     MaxBinsPerBasePrice = x.MaxBinsPerBasePrice,
                                                     CostPerBin = x.CostPerBin,
                                                     DiscountPrice1 = x.DiscountPrice1,
                                                     DiscountPrice2 = x.DiscountPrice2,
                                                     DiscountPrice3 = x.DiscountPrice3,
                                                     MaxSubscriptionTier1 = x.MaxSubscriptionTier1,
                                                     MaxSubscriptionTier2 = x.MaxSubscriptionTier2,
                                                     MaxSubscriptionTier3 = x.MaxSubscriptionTier3,
                                                     MaxSubscriptionTier4 = x.MaxSubscriptionTier4,
                                                     MaxSubscriptionTier5 = x.MaxSubscriptionTier5,
                                                     MaxSubscriptionTier6 = x.MaxSubscriptionTier6,
                                                     PriceSubscriptionTier1 = x.PriceSubscriptionTier1,
                                                     PriceSubscriptionTier2 = x.PriceSubscriptionTier2,
                                                     PriceSubscriptionTier3 = x.PriceSubscriptionTier3,
                                                     PriceSubscriptionTier4 = x.PriceSubscriptionTier4,
                                                     PriceSubscriptionTier5 = x.PriceSubscriptionTier5,
                                                     PriceSubscriptionTier6 = x.PriceSubscriptionTier6,
                                                     IncludeLicenseFees = x.IncludeLicenseFees,
                                                     MaxLicenseTier1 = x.MaxLicenseTier1,
                                                     MaxLicenseTier2 = x.MaxLicenseTier2,
                                                     MaxLicenseTier3 = x.MaxLicenseTier3,
                                                     MaxLicenseTier4 = x.MaxLicenseTier4,
                                                     MaxLicenseTier5 = x.MaxLicenseTier5,
                                                     MaxLicenseTier6 = x.MaxLicenseTier6,
                                                     PriceLicenseTier1 = x.PriceLicenseTier1,
                                                     PriceLicenseTier2 = x.PriceLicenseTier2,
                                                     PriceLicenseTier3 = x.PriceLicenseTier3,
                                                     PriceLicenseTier4 = x.PriceLicenseTier4,
                                                     PriceLicenseTier5 = x.PriceLicenseTier5,
                                                     PriceLicenseTier6 = x.PriceLicenseTier6,
                                                     UDF1 = x.UDF1,
                                                     UDF2 = x.UDF2,
                                                     UDF3 = x.UDF3,
                                                     UDF4 = x.UDF4,
                                                     UDF5 = x.UDF5,
                                                     GUID = x.GUID ?? Guid.Empty,
                                                     Created = x.Created,
                                                     Updated = x.Updated,
                                                     CreatedByName = x_um.UserName,
                                                     UpdatedByName = x_umup.UserName,
                                                     EnterpriseDBName = x.EnterpriseDBName,
                                                     //EnterpriseDBConnectionString = x.EnterpriseDBConnectionString,
                                                     IsArchived = x.IsArchived ?? false,
                                                     IsDeleted = x.IsDeleted,
                                                     CreatedBy = x.CreatedBy,
                                                     LastUpdatedBy = x.LastUpdatedBy,
                                                     EnterPriseDomainURL = x.EnterpriseDomainURL,
                                                     ArchiveDbName = x.ArchiveDbName,
                                                     ChangeLogDbName = x.ChangeLogDbName,
                                                     HistoryDBName = x.HistoryDBName
                                                 }).ToList();
                        }
                    }
                }
                return lstAllEnterprises;
            }
            finally
            {
                lstAllEnterprises = null;
            }

        }

        public List<EnterpriseDTO> GetEnterprises(long[] Ids)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return (from x in context.EnterpriseMasters
                        join um in context.UserMasters on x.CreatedBy equals um.ID into x_um_join
                        from x_um in x_um_join.DefaultIfEmpty()
                        join umup in context.UserMasters on x.LastUpdatedBy equals umup.ID into x_umup_join
                        from x_umup in x_umup_join.DefaultIfEmpty()
                        where Ids.Contains(x.ID)
                        select new EnterpriseDTO
                        {
                            ID = x.ID,
                            Name = x.Name,
                            Address = x.Address,
                            City = x.City,
                            State = x.State,
                            PostalCode = x.PostalCode,
                            Country = x.Country,
                            ContactPhone = x.ContactPhone,
                            ContactEmail = x.ContactEmail,
                            SoftwareBasePrice = x.SoftwareBasePrice,
                            MaxBinsPerBasePrice = x.MaxBinsPerBasePrice,
                            CostPerBin = x.CostPerBin,
                            DiscountPrice1 = x.DiscountPrice1,
                            DiscountPrice2 = x.DiscountPrice2,
                            DiscountPrice3 = x.DiscountPrice3,
                            MaxSubscriptionTier1 = x.MaxSubscriptionTier1,
                            MaxSubscriptionTier2 = x.MaxSubscriptionTier2,
                            MaxSubscriptionTier3 = x.MaxSubscriptionTier3,
                            MaxSubscriptionTier4 = x.MaxSubscriptionTier4,
                            MaxSubscriptionTier5 = x.MaxSubscriptionTier5,
                            MaxSubscriptionTier6 = x.MaxSubscriptionTier6,
                            PriceSubscriptionTier1 = x.PriceSubscriptionTier1,
                            PriceSubscriptionTier2 = x.PriceSubscriptionTier2,
                            PriceSubscriptionTier3 = x.PriceSubscriptionTier3,
                            PriceSubscriptionTier4 = x.PriceSubscriptionTier4,
                            PriceSubscriptionTier5 = x.PriceSubscriptionTier5,
                            PriceSubscriptionTier6 = x.PriceSubscriptionTier6,
                            IncludeLicenseFees = x.IncludeLicenseFees,
                            MaxLicenseTier1 = x.MaxLicenseTier1,
                            MaxLicenseTier2 = x.MaxLicenseTier2,
                            MaxLicenseTier3 = x.MaxLicenseTier3,
                            MaxLicenseTier4 = x.MaxLicenseTier4,
                            MaxLicenseTier5 = x.MaxLicenseTier5,
                            MaxLicenseTier6 = x.MaxLicenseTier6,
                            PriceLicenseTier1 = x.PriceLicenseTier1,
                            PriceLicenseTier2 = x.PriceLicenseTier2,
                            PriceLicenseTier3 = x.PriceLicenseTier3,
                            PriceLicenseTier4 = x.PriceLicenseTier4,
                            PriceLicenseTier5 = x.PriceLicenseTier5,
                            PriceLicenseTier6 = x.PriceLicenseTier6,
                            UDF1 = x.UDF1,
                            UDF2 = x.UDF2,
                            UDF3 = x.UDF3,
                            UDF4 = x.UDF4,
                            UDF5 = x.UDF5,
                            GUID = x.GUID ?? Guid.Empty,
                            Created = x.Created,
                            Updated = x.Updated,
                            CreatedByName = x_um.UserName,
                            UpdatedByName = x_umup.UserName,
                            IsArchived = x.IsArchived ?? false,
                            IsDeleted = x.IsDeleted,
                            CreatedBy = x.CreatedBy,
                            LastUpdatedBy = x.LastUpdatedBy,
                            EnterpriseDBName = x.EnterpriseDBName,
                            //EnterpriseDBConnectionString = x.EnterpriseDBConnectionString,
                            EnterpriseSqlServerName = x.EnterpriseSqlServerName,
                            EnterpriseSqlServerUserName = x.EnterpriseSqlServerUserName,
                            EnterpriseSqlServerPassword = x.EnterpriseSqlServerPassword,
                            IsActive = x.IsActive,
                            EnterPriseDomainURL = x.EnterpriseDomainURL,
                            ArchiveDbName = x.ArchiveDbName,
                            ChangeLogDbName = x.ChangeLogDbName,
                            HistoryDBName = x.HistoryDBName
                        }).ToList();
            }

        }

        public List<UserAccessDTO> GetUserAccessWithNames(long UserId)
        {
            List<UserAccessDTO> UserRoomAccess = new UserMasterDAL().GetUserRoomAccessByUserIdPlain(UserId);
            List<UserAccessDTO> lstUserAccess = new List<UserAccessDTO>();
            EnterpriseDTO objEnterprise = new EnterpriseDTO();
            UserAccessDTO objUserAccessDTO = new UserAccessDTO();

            if (UserRoomAccess != null && UserRoomAccess.Count > 0)
            {
                string enterpriseIds = string.Join(",", UserRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId > 0).Select(t => t.EnterpriseId).Distinct().ToArray());
                var enterprises = GetEnterprisesByIds(enterpriseIds);

                foreach (var item in UserRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId > 0).Select(t => t.EnterpriseId).Distinct())
                {
                    objEnterprise = new EnterpriseDTO();
                    objEnterprise = enterprises.Where(e => e.ID == item).FirstOrDefault();  //GetEnterpriseByIdPlain(item);

                    if (objEnterprise != null && objEnterprise.ID > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(objEnterprise.EnterpriseDBName))
                        {
                            DataSet dsRooms = new DataSet();
                            string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(objEnterprise.EnterpriseDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
                            string Rmids = string.Join(",", UserRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId > 0 && t.EnterpriseId == item).Select(t => t.RoomId).ToArray());
                            dsRooms = SqlHelper.ExecuteDataset(EnterpriseDbConnection, "GetCompaniesAndRoomsByRoomIdsNormal", Rmids);

                            if (dsRooms != null && dsRooms.Tables.Count > 0)
                            {

                                DataTable dtRooms = dsRooms.Tables[0];
                                if (dtRooms.Rows.Count > 0)
                                {
                                    foreach (DataRow drcompay in dtRooms.Rows)
                                    {
                                        objUserAccessDTO = new UserAccessDTO();
                                        objUserAccessDTO.EnterpriseId = objEnterprise.ID;
                                        objUserAccessDTO.EnterpriseName = objEnterprise.Name;
                                        objUserAccessDTO.IsEnterpriseActive = objEnterprise.IsActive;
                                        if (dtRooms.Columns.Contains("RoomId"))
                                        {
                                            long templong = 0;
                                            long.TryParse(Convert.ToString(drcompay["RoomId"]), out templong);
                                            objUserAccessDTO.RoomId = templong;
                                        }
                                        if (dtRooms.Columns.Contains("CompanyId"))
                                        {
                                            long templong = 0;
                                            long.TryParse(Convert.ToString(drcompay["CompanyId"]), out templong);
                                            objUserAccessDTO.CompanyId = templong;
                                        }
                                        if (dtRooms.Columns.Contains("RoomName"))
                                        {
                                            objUserAccessDTO.RoomName = Convert.ToString(drcompay["RoomName"]);
                                        }

                                        if (dtRooms.Columns.Contains("CompanyName"))
                                        {
                                            objUserAccessDTO.CompanyName = Convert.ToString(drcompay["CompanyName"]);
                                        }
                                        if (dtRooms.Columns.Contains("IsRoomActive"))
                                        {
                                            objUserAccessDTO.IsRoomActive = Convert.ToBoolean(drcompay["IsRoomActive"]);
                                        }
                                        if (dtRooms.Columns.Contains("IsCompanyActive"))
                                        {
                                            objUserAccessDTO.IsCompanyActive = Convert.ToBoolean(drcompay["IsCompanyActive"]);
                                        }
                                        lstUserAccess.Add(objUserAccessDTO);
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var item in UserRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId == 0).Select(t => t.EnterpriseId).Distinct())
                {
                    objEnterprise = new EnterpriseDTO();
                    objEnterprise = enterprises.Where(e => e.ID == item).FirstOrDefault();  //objEnterprise = GetEnterpriseByIdPlain(item);

                    if (objEnterprise != null && objEnterprise.ID > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(objEnterprise.EnterpriseDBName))
                        {
                            DataSet dsCompanies = new DataSet();
                            string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(objEnterprise.EnterpriseDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
                            string cmids = string.Join(",", UserRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId == 0 && t.EnterpriseId == item).Select(t => t.CompanyId).ToArray());
                            dsCompanies = SqlHelper.ExecuteDataset(EnterpriseDbConnection, "GetCompaniesByIdsNormal", cmids);

                            if (dsCompanies != null && dsCompanies.Tables.Count > 0)
                            {
                                DataTable dtRooms = dsCompanies.Tables[0];
                                if (dtRooms.Rows.Count > 0)
                                {
                                    foreach (DataRow drcompay in dtRooms.Rows)
                                    {
                                        objUserAccessDTO = new UserAccessDTO();
                                        objUserAccessDTO.EnterpriseId = objEnterprise.ID;
                                        objUserAccessDTO.EnterpriseName = objEnterprise.Name;
                                        objUserAccessDTO.IsEnterpriseActive = objEnterprise.IsActive;
                                        //if (dtRooms.Columns.Contains("RoomId"))
                                        //{
                                        //    long templong = 0;
                                        //    long.TryParse(Convert.ToString(drcompay["RoomId"]), out templong);
                                        //    objUserAccessDTO.ID = templong;
                                        //}
                                        if (dtRooms.Columns.Contains("CompanyId"))
                                        {
                                            long templong = 0;
                                            long.TryParse(Convert.ToString(drcompay["CompanyId"]), out templong);
                                            objUserAccessDTO.CompanyId = templong;
                                        }
                                        //if (dtRooms.Columns.Contains("RoomName"))
                                        //{
                                        //    objUserAccessDTO.RoomName = Convert.ToString(drcompay["RoomName"]);
                                        //}

                                        if (dtRooms.Columns.Contains("CompanyName"))
                                        {
                                            objUserAccessDTO.CompanyName = Convert.ToString(drcompay["CompanyName"]);
                                        }
                                        //if (dtRooms.Columns.Contains("IsRoomActive"))
                                        //{
                                        //    objUserAccessDTO.IsRoomActive = Convert.ToBoolean(drcompay["IsRoomActive"]);
                                        //}
                                        if (dtRooms.Columns.Contains("IsCompanyActive"))
                                        {
                                            objUserAccessDTO.IsCompanyActive = Convert.ToBoolean(drcompay["IsCompanyActive"]);
                                        }
                                        lstUserAccess.Add(objUserAccessDTO);
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var item in UserRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId == 0 && t.RoomId == 0).Select(t => t.EnterpriseId).Distinct())
                {
                    objEnterprise = new EnterpriseDTO();
                    objEnterprise = enterprises.Where(e => e.ID == item).FirstOrDefault();  // //objEnterprise = GetEnterpriseByIdPlain(item);
                    objUserAccessDTO = new UserAccessDTO();

                    if (objEnterprise != null && objEnterprise.ID > 0)
                    {
                        objUserAccessDTO.EnterpriseId = objEnterprise.ID;
                        objUserAccessDTO.EnterpriseName = objEnterprise.Name;
                        objUserAccessDTO.IsEnterpriseActive = objEnterprise.IsActive;
                        lstUserAccess.Add(objUserAccessDTO);
                    }
                }
            }
            return lstUserAccess;
        }

        public List<UserAccessDTO> GetRoleAccessWithNames(long RoleId)
        {
            List<UserAccessDTO> RoleRoomAccess = new UserMasterDAL().GetRoleRoomAccessByRoleIdPlain(RoleId);
            List<UserAccessDTO> lstUserAccess = new List<UserAccessDTO>();
            EnterpriseDTO objEnterprise = new EnterpriseDTO();
            UserAccessDTO objUserAccessDTO = new UserAccessDTO();

            if (RoleRoomAccess != null && RoleRoomAccess.Count > 0)
            {
                foreach (var item in RoleRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId > 0).Select(t => t.EnterpriseId).Distinct())
                {
                    objEnterprise = new EnterpriseDTO();
                    objEnterprise = GetEnterpriseByIdPlain(item);

                    if (objEnterprise != null)
                    {
                        if (!string.IsNullOrWhiteSpace(objEnterprise.EnterpriseDBName))
                        {
                            DataSet dsRooms = new DataSet();
                            string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(objEnterprise.EnterpriseDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
                            string Rmids = string.Join(",", RoleRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId > 0 && t.EnterpriseId == item).Select(t => t.RoomId).ToArray());
                            dsRooms = SqlHelper.ExecuteDataset(EnterpriseDbConnection, CommandType.Text, "select rm.ID as RoomId,rm.IsRoomActive,rm.RoomName,cm.ID as CompanyId,cm.Name as CompanyName,cm.IsActive as IsCompanyActive,isnull(rm.IsDeleted,0) as IsRoomDeleted,isnull(cm.IsDeleted,0) as IsCompanyDeleted from Room as rm inner join CompanyMaster as cm on rm.CompanyID = cm.ID Where rm.ID in (" + Rmids + ")");

                            if (dsRooms != null && dsRooms.Tables.Count > 0)
                            {
                                DataTable dtRooms = dsRooms.Tables[0];

                                if (dtRooms.Rows.Count > 0)
                                {
                                    foreach (DataRow drcompay in dtRooms.Rows)
                                    {
                                        objUserAccessDTO = new UserAccessDTO();
                                        objUserAccessDTO.EnterpriseId = objEnterprise.ID;
                                        objUserAccessDTO.EnterpriseName = objEnterprise.Name;
                                        objUserAccessDTO.IsEnterpriseActive = objEnterprise.IsActive;

                                        if (dtRooms.Columns.Contains("RoomId"))
                                        {
                                            long templong = 0;
                                            long.TryParse(Convert.ToString(drcompay["RoomId"]), out templong);
                                            objUserAccessDTO.RoomId = templong;
                                        }
                                        if (dtRooms.Columns.Contains("CompanyId"))
                                        {
                                            long templong = 0;
                                            long.TryParse(Convert.ToString(drcompay["CompanyId"]), out templong);
                                            objUserAccessDTO.CompanyId = templong;
                                        }
                                        if (dtRooms.Columns.Contains("RoomName"))
                                        {
                                            objUserAccessDTO.RoomName = Convert.ToString(drcompay["RoomName"]);
                                        }

                                        if (dtRooms.Columns.Contains("CompanyName"))
                                        {
                                            objUserAccessDTO.CompanyName = Convert.ToString(drcompay["CompanyName"]);
                                        }
                                        if (dtRooms.Columns.Contains("IsRoomActive"))
                                        {
                                            objUserAccessDTO.IsRoomActive = Convert.ToBoolean(drcompay["IsRoomActive"]);
                                        }
                                        if (dtRooms.Columns.Contains("IsCompanyActive"))
                                        {
                                            objUserAccessDTO.IsCompanyActive = Convert.ToBoolean(drcompay["IsCompanyActive"]);
                                        }
                                        lstUserAccess.Add(objUserAccessDTO);
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var item in RoleRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId == 0).Select(t => t.EnterpriseId).Distinct())
                {
                    objEnterprise = new EnterpriseDTO();
                    objEnterprise = GetEnterpriseByIdPlain(item);
                    if (objEnterprise != null)
                    {
                        if (!string.IsNullOrWhiteSpace(objEnterprise.EnterpriseDBName))
                        {
                            DataSet dsCompanies = new DataSet();
                            string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(objEnterprise.EnterpriseDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
                            string cmids = string.Join(",", RoleRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId == 0 && t.EnterpriseId == item).Select(t => t.CompanyId).ToArray());
                            dsCompanies = SqlHelper.ExecuteDataset(EnterpriseDbConnection, CommandType.Text, "Select ID as CompanyId,Name as CompanyName,IsActive as IsCompanyActive,ISNULL(isdeleted,0) as IsCompanyDeleted from CompanyMaster Where ID in (" + cmids + ")");
                            if (dsCompanies != null && dsCompanies.Tables.Count > 0)
                            {
                                DataTable dtRooms = dsCompanies.Tables[0];
                                if (dtRooms.Rows.Count > 0)
                                {
                                    foreach (DataRow drcompay in dtRooms.Rows)
                                    {
                                        objUserAccessDTO = new UserAccessDTO();
                                        objUserAccessDTO.EnterpriseId = objEnterprise.ID;
                                        objUserAccessDTO.EnterpriseName = objEnterprise.Name;
                                        objUserAccessDTO.IsEnterpriseActive = objEnterprise.IsActive;
                                        //if (dtRooms.Columns.Contains("RoomId"))
                                        //{
                                        //    long templong = 0;
                                        //    long.TryParse(Convert.ToString(drcompay["RoomId"]), out templong);
                                        //    objUserAccessDTO.ID = templong;
                                        //}
                                        if (dtRooms.Columns.Contains("CompanyId"))
                                        {
                                            long templong = 0;
                                            long.TryParse(Convert.ToString(drcompay["CompanyId"]), out templong);
                                            objUserAccessDTO.CompanyId = templong;
                                        }
                                        //if (dtRooms.Columns.Contains("RoomName"))
                                        //{
                                        //    objUserAccessDTO.RoomName = Convert.ToString(drcompay["RoomName"]);
                                        //}

                                        if (dtRooms.Columns.Contains("CompanyName"))
                                        {
                                            objUserAccessDTO.CompanyName = Convert.ToString(drcompay["CompanyName"]);
                                        }
                                        //if (dtRooms.Columns.Contains("IsRoomActive"))
                                        //{
                                        //    objUserAccessDTO.IsRoomActive = Convert.ToBoolean(drcompay["IsRoomActive"]);
                                        //}
                                        if (dtRooms.Columns.Contains("IsCompanyActive"))
                                        {
                                            objUserAccessDTO.IsCompanyActive = Convert.ToBoolean(drcompay["IsCompanyActive"]);
                                        }
                                        lstUserAccess.Add(objUserAccessDTO);
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var item in RoleRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId == 0 && t.RoomId == 0).Select(t => t.EnterpriseId).Distinct())
                {

                    objEnterprise = new EnterpriseDTO();
                    objEnterprise = GetEnterpriseByIdPlain(item);
                    objUserAccessDTO = new UserAccessDTO();
                    if (objEnterprise != null)
                    {
                        objUserAccessDTO.EnterpriseId = objEnterprise.ID;
                        objUserAccessDTO.EnterpriseName = objEnterprise.Name;
                        objUserAccessDTO.IsEnterpriseActive = objEnterprise.IsActive;
                        lstUserAccess.Add(objUserAccessDTO);
                    }
                }
            }
            return lstUserAccess;
        }

        public List<UserAccessDTO> GetUserAccessWithNames(List<UserAccessDTO> UserRoomAccess)
        {
            List<UserAccessDTO> lstUserAccess = new List<UserAccessDTO>();
            EnterpriseDTO objEnterprise = new EnterpriseDTO();
            UserAccessDTO objUserAccessDTO = new UserAccessDTO();

            if (UserRoomAccess != null && UserRoomAccess.Count > 0)
            {
                foreach (var item in UserRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId > 0).Select(t => t.EnterpriseId).Distinct())
                {
                    objEnterprise = new EnterpriseDTO();
                    objEnterprise = GetEnterpriseByIdPlain(item);

                    if (objEnterprise != null)
                    {
                        if (!string.IsNullOrWhiteSpace(objEnterprise.EnterpriseDBName))
                        {
                            DataSet dsRooms = new DataSet();
                            string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(objEnterprise.EnterpriseDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
                            string Rmids = string.Join(",", UserRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId > 0 && t.EnterpriseId == item).Select(t => t.RoomId).ToArray());
                            dsRooms = SqlHelper.ExecuteDataset(EnterpriseDbConnection, CommandType.Text, "select rm.ID as RoomId,rm.IsRoomActive,rm.RoomName,cm.ID as CompanyId,cm.Name as CompanyName,cm.IsActive as IsCompanyActive,isnull(rm.IsDeleted,0) as IsRoomDeleted,isnull(cm.IsDeleted,0) as IsCompanyDeleted from Room as rm inner join CompanyMaster as cm on rm.CompanyID = cm.ID Where rm.ID in (" + Rmids + ")");
                            if (dsRooms != null && dsRooms.Tables.Count > 0)
                            {
                                DataTable dtRooms = dsRooms.Tables[0];
                                if (dtRooms.Rows.Count > 0)
                                {
                                    foreach (DataRow drcompay in dtRooms.Rows)
                                    {
                                        objUserAccessDTO = new UserAccessDTO();
                                        objUserAccessDTO.EnterpriseId = objEnterprise.ID;
                                        objUserAccessDTO.EnterpriseName = objEnterprise.Name;
                                        objUserAccessDTO.IsEnterpriseActive = objEnterprise.IsActive;
                                        if (dtRooms.Columns.Contains("RoomId"))
                                        {
                                            long templong = 0;
                                            long.TryParse(Convert.ToString(drcompay["RoomId"]), out templong);
                                            objUserAccessDTO.RoomId = templong;
                                        }
                                        if (dtRooms.Columns.Contains("CompanyId"))
                                        {
                                            long templong = 0;
                                            long.TryParse(Convert.ToString(drcompay["CompanyId"]), out templong);
                                            objUserAccessDTO.CompanyId = templong;
                                        }
                                        if (dtRooms.Columns.Contains("RoomName"))
                                        {
                                            objUserAccessDTO.RoomName = Convert.ToString(drcompay["RoomName"]);
                                        }

                                        if (dtRooms.Columns.Contains("CompanyName"))
                                        {
                                            objUserAccessDTO.CompanyName = Convert.ToString(drcompay["CompanyName"]);
                                        }
                                        if (dtRooms.Columns.Contains("IsRoomActive"))
                                        {
                                            objUserAccessDTO.IsRoomActive = Convert.ToBoolean(drcompay["IsRoomActive"]);
                                        }
                                        if (dtRooms.Columns.Contains("IsCompanyActive"))
                                        {
                                            objUserAccessDTO.IsCompanyActive = Convert.ToBoolean(drcompay["IsCompanyActive"]);
                                        }
                                        lstUserAccess.Add(objUserAccessDTO);
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var item in UserRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId == 0).Select(t => t.EnterpriseId).Distinct())
                {
                    objEnterprise = new EnterpriseDTO();
                    objEnterprise = GetEnterpriseByIdPlain(item);
                    if (objEnterprise != null)
                    {
                        if (!string.IsNullOrWhiteSpace(objEnterprise.EnterpriseDBName))
                        {
                            DataSet dsCompanies = new DataSet();
                            string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(objEnterprise.EnterpriseDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
                            string cmids = string.Join(",", UserRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId == 0 && t.EnterpriseId == item).Select(t => t.CompanyId).ToArray());
                            dsCompanies = SqlHelper.ExecuteDataset(EnterpriseDbConnection, CommandType.Text, "Select ID as CompanyId,Name as CompanyName,IsActive as IsCompanyActive,ISNULL(isdeleted,0) as IsCompanyDeleted from CompanyMaster Where ID in (" + cmids + ")");
                            if (dsCompanies != null && dsCompanies.Tables.Count > 0)
                            {
                                DataTable dtRooms = dsCompanies.Tables[0];
                                if (dtRooms.Rows.Count > 0)
                                {
                                    foreach (DataRow drcompay in dtRooms.Rows)
                                    {
                                        objUserAccessDTO = new UserAccessDTO();
                                        objUserAccessDTO.EnterpriseId = objEnterprise.ID;
                                        objUserAccessDTO.EnterpriseName = objEnterprise.Name;
                                        objUserAccessDTO.IsEnterpriseActive = objEnterprise.IsActive;
                                        //if (dtRooms.Columns.Contains("RoomId"))
                                        //{
                                        //    long templong = 0;
                                        //    long.TryParse(Convert.ToString(drcompay["RoomId"]), out templong);
                                        //    objUserAccessDTO.ID = templong;
                                        //}
                                        if (dtRooms.Columns.Contains("CompanyId"))
                                        {
                                            long templong = 0;
                                            long.TryParse(Convert.ToString(drcompay["CompanyId"]), out templong);
                                            objUserAccessDTO.CompanyId = templong;
                                        }
                                        //if (dtRooms.Columns.Contains("RoomName"))
                                        //{
                                        //    objUserAccessDTO.RoomName = Convert.ToString(drcompay["RoomName"]);
                                        //}

                                        if (dtRooms.Columns.Contains("CompanyName"))
                                        {
                                            objUserAccessDTO.CompanyName = Convert.ToString(drcompay["CompanyName"]);
                                        }
                                        //if (dtRooms.Columns.Contains("IsRoomActive"))
                                        //{
                                        //    objUserAccessDTO.IsRoomActive = Convert.ToBoolean(drcompay["IsRoomActive"]);
                                        //}
                                        if (dtRooms.Columns.Contains("IsCompanyActive"))
                                        {
                                            objUserAccessDTO.IsCompanyActive = Convert.ToBoolean(drcompay["IsCompanyActive"]);
                                        }
                                        lstUserAccess.Add(objUserAccessDTO);
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var item in UserRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId == 0 && t.RoomId == 0).Select(t => t.EnterpriseId).Distinct())
                {
                    objEnterprise = new EnterpriseDTO();
                    objEnterprise = GetEnterpriseByIdPlain(item);
                    objUserAccessDTO = new UserAccessDTO();
                    if (objEnterprise != null)
                    {
                        objUserAccessDTO.EnterpriseId = objEnterprise.ID;
                        objUserAccessDTO.EnterpriseName = objEnterprise.Name;
                        objUserAccessDTO.IsEnterpriseActive = objEnterprise.IsActive;
                        lstUserAccess.Add(objUserAccessDTO);
                    }
                }
            }
            return lstUserAccess;
        }

        public ForgotPasswordRequest GetForgotPasswordRequest(Guid Token)
        {
            ForgotPasswordRequest objForgotPasswordRequest = null;
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                objForgotPasswordRequest = context.ForgotPasswordRequests.FirstOrDefault(t => t.RequestToken == Token);
            }
            return objForgotPasswordRequest;
        }

        public List<RoomDTO> GetAllRooms(long? EnterpriseID)
        {
            List<RoomDTO> lstRooms = new List<RoomDTO>();
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                lstRooms = (from u in context.MstRooms
                            join cm in context.MstCompanyMasters on new { u.CompanyID, u.EnterpriseID } equals new { cm.CompanyID, cm.EnterpriseID }
                            join em in context.EnterpriseMasters on new { eid = u.EnterpriseID } equals new { eid = em.ID }
                            join uc in context.UserMasters on u.CreatedBy equals uc.ID into rm_uc_join
                            from rm_uc in rm_uc_join.DefaultIfEmpty()
                            join uu in context.UserMasters on u.LastUpdatedBy equals uu.ID into rm_uu_join
                            from rm_uu in rm_uu_join.DefaultIfEmpty()
                            where u.IsDeleted == false && (cm.IsDeleted ?? false) == false && em.IsDeleted == false && (EnterpriseID == null || u.EnterpriseID == EnterpriseID)
                            select new RoomDTO
                            {
                                ID = u.RoomID ?? 0,
                                RoomName = u.RoomName,
                                CompanyName = cm.Name,
                                ContactName = u.ContactName,
                                streetaddress = u.StreetAddress,
                                City = u.City,
                                State = u.State,
                                PostalCode = u.PostalCode,
                                Country = u.Country,
                                Email = u.Email,
                                PhoneNo = u.PhoneNo,
                                InvoiceBranch = u.InvoiceBranch,
                                CustomerNumber = u.CustomerNumber,
                                BlanketPO = u.BlanketPO,
                                IsConsignment = u.IsConsignment,
                                IsTrending = u.IsTrending,
                                SourceOfTrending = u.SourceOfTrending,
                                TrendingFormula = u.TrendingFormula,
                                TrendingFormulaType = u.TrendingFormulaType,
                                TrendingFormulaDays = u.TrendingFormulaDays,
                                TrendingFormulaOverDays = u.TrendingFormulaOverDays,
                                TrendingFormulaAvgDays = u.TrendingFormulaAvgDays,
                                TrendingFormulaCounts = u.TrendingFormulaCounts,
                                SuggestedOrder = u.SuggestedOrder,
                                SuggestedTransfer = u.SuggestedTransfer,
                                ReplineshmentRoom = u.ReplineshmentRoom,
                                ReplenishmentType = u.ReplenishmentType,
                                IseVMI = u.IseVMI,
                                MaxOrderSize = u.MaxOrderSize,
                                HighPO = u.HighPO,
                                HighJob = u.HighJob,
                                HighTransfer = u.HighTransfer,
                                HighCount = u.HighCount,
                                GlobMarkupParts = u.GlobMarkupParts,
                                GlobMarkupLabor = u.GlobMarkupLabor,
                                IsTax1Parts = u.IsTax1Parts,
                                IsTax1Labor = u.IsTax1Labor,
                                Tax1name = u.Tax1Name,
                                Tax1Rate = u.Tax1Rate,
                                IsTax2Parts = u.IsTax2Parts,
                                IsTax2Labor = u.IsTax2Labor,
                                tax2name = u.Tax2Name,
                                Tax2Rate = u.Tax2Rate,
                                IsTax2onTax1 = u.IsTax2onTax1 ?? false,
                                GXPRConsJob = u.GXPRConsJob,
                                CostCenter = u.CostCenter,
                                UniqueID = u.UniqueID,
                                Created = u.Created ?? DateTime.MinValue,
                                Updated = u.Updated,
                                CreatedBy = u.CreatedBy,
                                LastUpdatedBy = u.LastUpdatedBy,
                                IsDeleted = u.IsDeleted,
                                IsArchived = u.IsArchived ?? false,
                                CreatedByName = rm_uc.UserName,
                                UpdatedByName = rm_uu.UserName,
                                MethodOfValuingInventory = u.MethodOfValuingInventory,
                                AutoCreateTransferFrequency = u.AutoCreateTransferFrequency,
                                AutoCreateTransferTime = u.AutoCreateTransferTime,
                                AutoCreateTransferSubmit = u.AutoCreateTransferSubmit,
                                IsActive = u.IsActive,
                                LicenseBilled = u.LicenseBilled,
                                NextCountNo = u.NextCountNo,
                                IsProjectSpendMandatory = u.IsProjectSpendMandatory,
                                IsAverageUsageBasedOnPull = u.IsAverageUsageBasedOnPull,
                                SlowMovingValue = u.SlowMovingValue,
                                FastMovingValue = u.FastMovingValue,
                                POAutoSequence = u.POAutoSequence,
                                NextOrderNo = u.NextOrderNo,
                                NextRequisitionNo = u.NextRequisitionNo,
                                NextStagingNo = u.NextStagingNo,
                                NextTransferNo = u.NextTransferNo,
                                NextWorkOrderNo = u.NextWorkOrderNo,
                                RoomGrouping = u.RoomGrouping,
                                TransferFrequencyOption = u.TransferFrequencyOption,
                                TransferFrequencyDays = u.TransferFrequencyDays,
                                TransferFrequencyMonth = u.TransferFrequencyMonth,
                                TransferFrequencyNumber = u.TransferFrequencyNumber,
                                TransferFrequencyWeek = u.TransferFrequencyWeek,
                                TransferFrequencyMainOption = u.TransferFrequencyMainOption,
                                TrendingSampleSize = u.TrendingSampleSize,
                                TrendingSampleSizeDivisor = u.TrendingSampleSizeDivisor,
                                AverageUsageTransactions = u.AverageUsageTransactions,
                                AverageUsageSampleSize = u.AverageUsageSampleSize,
                                AverageUsageSampleSizeDivisor = u.AverageUsageSampleSizeDivisor,
                                GUID = u.GUID ?? Guid.Empty,
                                CompanyID = u.CompanyID,
                                UDF1 = u.UDF1,
                                UDF2 = u.UDF2,
                                UDF3 = u.UDF3,
                                UDF4 = u.UDF4,
                                UDF5 = u.UDF5,
                                DefaultSupplierID = u.DefaultSupplierID,
                                NextAssetNo = u.NextAssetNo,
                                NextBinNo = u.NextBinNo,
                                NextKitNo = u.NextKitNo,
                                NextItemNo = u.NextItemNo,
                                NextProjectSpendNo = u.NextProjectSpendNo,
                                NextToolNo = u.NextToolNo,
                                DefaultBinID = u.DefaultBinID,
                                IsRoomActive = u.IsRoomActive,
                                RequestedXDays = u.RequestedXDays ?? 0,
                                RequestedYDays = u.RequestedYDays ?? 0,
                                InventoryConsuptionMethod = u.InventoryConsuptionMethod == null ? "" : u.InventoryConsuptionMethod,
                                DefaultBinName = u.DefaultBinName,
                                BaseOfInventory = u.BaseOfInventory,
                                eVMIWaitCommand = u.eVMIWaitCommand,
                                eVMIWaitPort = u.eVMIWaitPort,
                                CountAutoSequence = u.CountAutoSequence,
                                ShelfLifeleadtimeOrdRpt = u.ShelfLifeleadtimeOrdRpt,
                                LeadTimeOrdRpt = u.LeadTimeOrdRpt,
                                EnterpriseName = em.Name,
                                EnterpriseId = u.EnterpriseID,
                                StagingAutoSequence = u.StagingAutoSequence,
                                TransferAutoSequence = u.TransferAutoSequence,
                                WorkOrderAutoSequence = u.WorkOrderAutoSequence,
                                WarnUserOnAssigningNonDefaultBin = u.WarnUserOnAssigningNonDefaultBin,
                                MaintenanceDueNoticeDays = u.MaintenanceDueNoticeDays,
                                DefaultRequisitionRequiredDays = u.DefaultRequisitionRequiredDays,
                                AttachingWOWithRequisition = u.AttachingWOWithRequisition,
                                PreventMaxOrderQty = u.PreventMaxOrderQty,
                                DefaultCountType = u.DefaultCountType,
                                IsWOSignatureRequired = u.IsWOSignatureRequired,
                                IsIgnoreCreditRule = u.IsIgnoreCreditRule,
                                IsAllowRequisitionDuplicate = u.IsAllowRequisitionDuplicate ?? true,
                                IsAllowOrderDuplicate = u.IsAllowOrderDuplicate,
                                IsAllowWorkOrdersDuplicate = u.IsAllowWorkOrdersDuplicate,
                                ToolCountAutoSequence = u.ToolCountAutoSequence,
                                NextToolCountNo = u.NextToolCountNo,

                                ForceSupplierFilter = u.ForceSupplierFilter,
                                SuggestedReturn = u.SuggestedReturn
                            }).ToList();
            }
            return lstRooms;
        }

        public List<UserAccessDTO> GetSuperUserPermissions(UserMasterDTO objDTO)
        {
            List<UserAccessDTO> lstPermissions = new List<UserAccessDTO>();
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                lstPermissions = (from ura in context.UserRoomAccesses
                                  join em in context.EnterpriseMasters on ura.EnterpriseId equals em.ID into ura_em_join
                                  from ura_em in ura_em_join.DefaultIfEmpty()
                                  join cm in context.MstCompanyMasters on new { cid = ura.CompanyId, eid = ura.EnterpriseId } equals new { cid = (cm.CompanyID ?? 0), eid = cm.EnterpriseID } into ura_cm_join
                                  from ura_cm in ura_cm_join.DefaultIfEmpty()
                                  join rm in context.MstRooms on new { rid = ura.RoomId, cid = ura.CompanyId, eid = ura.EnterpriseId } equals new { rid = (rm.RoomID ?? 0), cid = (rm.CompanyID ?? 0), eid = rm.EnterpriseID } into ura_rm_join
                                  from ura_rm in ura_rm_join.DefaultIfEmpty()
                                  where ura.UserId == objDTO.ID && ura_em.IsDeleted == false && ura_cm.IsDeleted == false && ura_rm.IsDeleted == false
                                  select new UserAccessDTO()
                                  {
                                      CompanyId = ura.CompanyId,
                                      CompanyName = ura_cm.Name,
                                      EnterpriseId = ura.EnterpriseId,
                                      EnterpriseName = ura_em.Name,
                                      ID = ura.ID,
                                      IsCompanyActive = true,
                                      IsRoomActive = ura_rm.IsRoomActive,
                                      IsEnterpriseActive = true,
                                      RoleId = objDTO.RoleID,
                                      RoleName = objDTO.RoleName,
                                      //  RoomId = ura.ID,
                                      RoomId = ura.RoomId,
                                      RoomName = ura_rm.RoomName,
                                      UserId = objDTO.ID,
                                      CompanyLogo = string.Empty,
                                      UserName = objDTO.UserName,
                                      UserType = objDTO.UserType
                                  }).ToList();
            }
            return lstPermissions;
        }
        public Dictionary<string, string> GetColumnListWithNUllRef(string DataBaseName, string TableName)
        {

            Dictionary<string, string> TableList = new Dictionary<string, string>();
            //string EnterpriseDbConnection = GetEnterpriseConnectionstring(DataBaseName);
            string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(DataBaseName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
            DataTable dt = SqlHelper.ExecuteDataset(EnterpriseDbConnection, "GetColumnsWithNullaRef", DataBaseName, TableName).Tables[0];
            TableList = dt.AsEnumerable().ToDictionary<DataRow, string, string>(row => row.Field<string>(0),
                                  row => row.Field<string>(1));
            return TableList;
        }
        public void SetEnterpriseSuperAdmin(EnterpriseDTO objDTO)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                eTurnsMaster.DAL.UserMaster objCurrentEnterpriseSU = context.UserMasters.FirstOrDefault(t => t.EnterpriseId == objDTO.ID && t.RoleId == -2 && t.UserType == 2 && t.IsDeleted == false);
                if (objCurrentEnterpriseSU != null && objDTO.UserName != objCurrentEnterpriseSU.UserName)
                {
                    objCurrentEnterpriseSU.IsDeleted = true;
                    objCurrentEnterpriseSU.RoleId = 0;
                    objCurrentEnterpriseSU.UserType = 3;

                    eTurnsMaster.DAL.UserMaster ExistingUser = context.UserMasters.FirstOrDefault(t => t.EnterpriseId == objDTO.ID && t.UserName == objDTO.UserName && t.IsDeleted == false);
                    if (ExistingUser != null)
                    {
                        ExistingUser.RoleId = -2;
                        ExistingUser.UserType = 2;
                        ExistingUser.RoomId = 0;
                        ExistingUser.CompanyID = 0;
                        if (objDTO.IsPasswordChanged == "1")
                        {
                            ExistingUser.Password = objDTO.EnterpriseUserPassword;
                        }
                    }
                    else
                    {
                        EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new EnterPriseUserMasterDAL();
                        UserMasterDTO objUserMasterDTO = new UserMasterDTO();
                        objUserMasterDTO.CompanyID = 0;
                        objUserMasterDTO.ConfirmPassword = "password";
                        objUserMasterDTO.Created = DateTime.UtcNow;
                        objUserMasterDTO.CreatedBy = objDTO.CreatedBy;
                        objUserMasterDTO.CreatedByName = objDTO.CreatedByName;
                        objUserMasterDTO.Email = objDTO.EnterpriseUserEmail;
                        objUserMasterDTO.EnterpriseDbName = string.Empty;
                        objUserMasterDTO.EnterpriseId = objDTO.ID;
                        objUserMasterDTO.GUID = Guid.NewGuid();
                        objUserMasterDTO.IsArchived = false;
                        objUserMasterDTO.IsDeleted = false;
                        objUserMasterDTO.LastUpdatedBy = objDTO.LastUpdatedBy;
                        objUserMasterDTO.Password = objDTO.EnterpriseUserPassword;
                        objUserMasterDTO.Phone = objDTO.ContactPhone;
                        objUserMasterDTO.RoleName = "Enterprise Role";
                        objUserMasterDTO.Updated = DateTime.UtcNow;
                        objUserMasterDTO.UserType = 2;
                        objUserMasterDTO.UserName = objDTO.UserName;
                        objUserMasterDTO.RoleID = -2;
                        objUserMasterDTO = objEnterPriseUserMasterDAL.SaveUseronMasterDB(objUserMasterDTO);
                        Int64 NewUserID = objUserMasterDTO.ID;

                        {



                        }
                        InsertUserInChildDB(NewUserID, objUserMasterDTO, objDTO.ID);

                    }
                    context.SaveChanges();


                }
                else
                {
                    UserMasterDTO objUserMaster = new UserMasterDTO();//new UserMasterDAL().GetUserByID(objDTO.EnterpriseUserID);
                    UpdateEnterpriseSuperUser(objUserMaster.ID, objUserMaster, objDTO);
                }
            }
        }
        public List<EnterpriseDTO> GetAllEnterprises(bool? IsDeleted)
        {
            List<EnterpriseDTO> lstAllEnterprises = null;
            try
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {

                    lstAllEnterprises = (from x in context.EnterpriseMasters
                                         join um in context.UserMasters on x.CreatedBy equals um.ID into x_um_join
                                         from x_um in x_um_join.DefaultIfEmpty()
                                         join umup in context.UserMasters on x.LastUpdatedBy equals umup.ID into x_umup_join
                                         from x_umup in x_umup_join.DefaultIfEmpty()
                                         where (IsDeleted == null || x.IsDeleted == IsDeleted)
                                         select new EnterpriseDTO
                                         {
                                             ID = x.ID,
                                             Name = x.Name,
                                             Address = x.Address,
                                             City = x.City,
                                             State = x.State,
                                             PostalCode = x.PostalCode,
                                             Country = x.Country,
                                             ContactPhone = x.ContactPhone,
                                             ContactEmail = x.ContactEmail,
                                             SoftwareBasePrice = x.SoftwareBasePrice,
                                             MaxBinsPerBasePrice = x.MaxBinsPerBasePrice,
                                             CostPerBin = x.CostPerBin,
                                             DiscountPrice1 = x.DiscountPrice1,
                                             DiscountPrice2 = x.DiscountPrice2,
                                             DiscountPrice3 = x.DiscountPrice3,
                                             MaxSubscriptionTier1 = x.MaxSubscriptionTier1,
                                             MaxSubscriptionTier2 = x.MaxSubscriptionTier2,
                                             MaxSubscriptionTier3 = x.MaxSubscriptionTier3,
                                             MaxSubscriptionTier4 = x.MaxSubscriptionTier4,
                                             MaxSubscriptionTier5 = x.MaxSubscriptionTier5,
                                             MaxSubscriptionTier6 = x.MaxSubscriptionTier6,
                                             PriceSubscriptionTier1 = x.PriceSubscriptionTier1,
                                             PriceSubscriptionTier2 = x.PriceSubscriptionTier2,
                                             PriceSubscriptionTier3 = x.PriceSubscriptionTier3,
                                             PriceSubscriptionTier4 = x.PriceSubscriptionTier4,
                                             PriceSubscriptionTier5 = x.PriceSubscriptionTier5,
                                             PriceSubscriptionTier6 = x.PriceSubscriptionTier6,
                                             IncludeLicenseFees = x.IncludeLicenseFees,
                                             MaxLicenseTier1 = x.MaxLicenseTier1,
                                             MaxLicenseTier2 = x.MaxLicenseTier2,
                                             MaxLicenseTier3 = x.MaxLicenseTier3,
                                             MaxLicenseTier4 = x.MaxLicenseTier4,
                                             MaxLicenseTier5 = x.MaxLicenseTier5,
                                             MaxLicenseTier6 = x.MaxLicenseTier6,
                                             PriceLicenseTier1 = x.PriceLicenseTier1,
                                             PriceLicenseTier2 = x.PriceLicenseTier2,
                                             PriceLicenseTier3 = x.PriceLicenseTier3,
                                             PriceLicenseTier4 = x.PriceLicenseTier4,
                                             PriceLicenseTier5 = x.PriceLicenseTier5,
                                             PriceLicenseTier6 = x.PriceLicenseTier6,
                                             UDF1 = x.UDF1,
                                             UDF2 = x.UDF2,
                                             UDF3 = x.UDF3,
                                             UDF4 = x.UDF4,
                                             UDF5 = x.UDF5,
                                             GUID = x.GUID ?? Guid.Empty,
                                             Created = x.Created,
                                             Updated = x.Updated,
                                             CreatedByName = x_um.UserName,
                                             UpdatedByName = x_umup.UserName,
                                             IsArchived = x.IsArchived ?? false,
                                             IsDeleted = x.IsDeleted,
                                             CreatedBy = x.CreatedBy,
                                             LastUpdatedBy = x.LastUpdatedBy,
                                             EnterpriseDBName = x.EnterpriseDBName,
                                             //EnterpriseDBConnectionString = x.EnterpriseDBConnectionString,
                                             EnterpriseSqlServerName = x.EnterpriseSqlServerName,
                                             EnterpriseSqlServerUserName = x.EnterpriseSqlServerUserName,
                                             EnterpriseSqlServerPassword = x.EnterpriseSqlServerPassword,
                                             EnterpriseLogo = x.EnterPriseLogo,
                                             EnterPriseDomainURL = x.EnterpriseDomainURL,
                                             ArchiveDbName = x.ArchiveDbName,
                                             ChangeLogDbName = x.ChangeLogDbName,
                                             HistoryDBName = x.HistoryDBName
                                         }).ToList();


                }
                return lstAllEnterprises;
            }
            finally
            {
                lstAllEnterprises = null;
            }

        }

        public long GetEnterpriseIdByUserID(Int64 Userid)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                long enterpriseid = context.UserMasters.Where(u => u.ID == Userid).FirstOrDefault().EnterpriseId;
                return enterpriseid;
            }
        }

        public List<EnterpriseDTO> GetEnterprisesByName(string EnterpriseNames)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseNames", EnterpriseNames ?? (object)DBNull.Value) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.ExecuteStoreQuery<EnterpriseDTO>("exec [GetEnterprisesByNames] @EnterpriseNames", params1).ToList();
            }
        }

        public EnterpriseDTO GetEnterpriseByUserID(Int64 UserId)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                EnterpriseDTO oEnterprise = new EnterpriseDTO();
                oEnterprise = (from x in context.EnterpriseMasters
                               join um in context.UserMasters on x.ID equals um.EnterpriseId
                               join umcr in context.UserMasters on x.CreatedBy equals umcr.ID into x_um_join
                               from x_um in x_um_join.DefaultIfEmpty()
                               join umup in context.UserMasters on x.LastUpdatedBy equals umup.ID into x_umup_join
                               from x_umup in x_umup_join.DefaultIfEmpty()
                               where um.ID == UserId
                               select new EnterpriseDTO
                               {
                                   ID = x.ID,
                                   Name = x.Name,
                                   Address = x.Address,
                                   City = x.City,
                                   State = x.State,
                                   PostalCode = x.PostalCode,
                                   Country = x.Country,
                                   ContactPhone = x.ContactPhone,
                                   ContactEmail = x.ContactEmail,
                                   SoftwareBasePrice = x.SoftwareBasePrice,
                                   MaxBinsPerBasePrice = x.MaxBinsPerBasePrice,
                                   CostPerBin = x.CostPerBin,
                                   DiscountPrice1 = x.DiscountPrice1,
                                   DiscountPrice2 = x.DiscountPrice2,
                                   DiscountPrice3 = x.DiscountPrice3,
                                   MaxSubscriptionTier1 = x.MaxSubscriptionTier1,
                                   MaxSubscriptionTier2 = x.MaxSubscriptionTier2,
                                   MaxSubscriptionTier3 = x.MaxSubscriptionTier3,
                                   MaxSubscriptionTier4 = x.MaxSubscriptionTier4,
                                   MaxSubscriptionTier5 = x.MaxSubscriptionTier5,
                                   MaxSubscriptionTier6 = x.MaxSubscriptionTier6,
                                   PriceSubscriptionTier1 = x.PriceSubscriptionTier1,
                                   PriceSubscriptionTier2 = x.PriceSubscriptionTier2,
                                   PriceSubscriptionTier3 = x.PriceSubscriptionTier3,
                                   PriceSubscriptionTier4 = x.PriceSubscriptionTier4,
                                   PriceSubscriptionTier5 = x.PriceSubscriptionTier5,
                                   PriceSubscriptionTier6 = x.PriceSubscriptionTier6,
                                   IncludeLicenseFees = x.IncludeLicenseFees,
                                   MaxLicenseTier1 = x.MaxLicenseTier1,
                                   MaxLicenseTier2 = x.MaxLicenseTier2,
                                   MaxLicenseTier3 = x.MaxLicenseTier3,
                                   MaxLicenseTier4 = x.MaxLicenseTier4,
                                   MaxLicenseTier5 = x.MaxLicenseTier5,
                                   MaxLicenseTier6 = x.MaxLicenseTier6,
                                   PriceLicenseTier1 = x.PriceLicenseTier1,
                                   PriceLicenseTier2 = x.PriceLicenseTier2,
                                   PriceLicenseTier3 = x.PriceLicenseTier3,
                                   PriceLicenseTier4 = x.PriceLicenseTier4,
                                   PriceLicenseTier5 = x.PriceLicenseTier5,
                                   PriceLicenseTier6 = x.PriceLicenseTier6,
                                   UDF1 = x.UDF1,
                                   UDF2 = x.UDF2,
                                   UDF3 = x.UDF3,
                                   UDF4 = x.UDF4,
                                   UDF5 = x.UDF5,
                                   GUID = x.GUID ?? Guid.Empty,
                                   Created = x.Created,
                                   Updated = x.Updated,
                                   CreatedByName = x_um.UserName,
                                   UpdatedByName = x_umup.UserName,
                                   IsArchived = x.IsArchived ?? false,
                                   IsDeleted = x.IsDeleted,
                                   CreatedBy = x.CreatedBy,
                                   LastUpdatedBy = x.LastUpdatedBy,
                                   //EnterpriseUserEmail = objUserMasterDTO.Email,
                                   //EnterpriseUserPassword = objUserMasterDTO.Password,
                                   //UserName = objUserMasterDTO.UserName,
                                   //EnterpriseUserID = objUserMasterDTO.ID,
                                   EnterpriseDBName = x.EnterpriseDBName,
                                   //EnterpriseDBConnectionString = x.EnterpriseDBConnectionString,
                                   EnterpriseSqlServerName = x.EnterpriseSqlServerName,
                                   EnterpriseSqlServerUserName = x.EnterpriseSqlServerUserName,
                                   EnterpriseSqlServerPassword = x.EnterpriseSqlServerPassword,
                                   EnterpriseLogo = x.EnterPriseLogo,
                                   IsActive = x.IsActive,
                                   EnterpriseSuperAdmin = x.EnterpriseSuperAdmin,
                                   EnterPriseDomainURL = x.EnterpriseDomainURL,
                                   ArchiveDbName = x.ArchiveDbName,
                                   ChangeLogDbName = x.ChangeLogDbName,
                                   HistoryDBName = x.HistoryDBName
                               }).FirstOrDefault();

                if (oEnterprise != null)
                {
                    UserMaster objUserMasterDTO = new UserMaster();
                    objUserMasterDTO = (from um in context.UserMasters
                                        join EM in context.EnterpriseMasters on um.ID equals EM.EnterpriseSuperAdmin// into UME
                                        where um.EnterpriseId == oEnterprise.ID && um.UserType == 2 && um.RoleId == -2 && um.IsDeleted == false// && (UMList == null || (UMList != null && UMList.EnterpriseSuperAdmin == um.ID))
                                        select um).FirstOrDefault();

                    if (objUserMasterDTO == null)
                    {
                        objUserMasterDTO = new UserMaster();
                    }

                    oEnterprise.EnterpriseUserEmail = objUserMasterDTO.Email;
                    oEnterprise.EnterpriseUserPassword = objUserMasterDTO.Password;
                    oEnterprise.UserName = objUserMasterDTO.UserName;
                    oEnterprise.EnterpriseUserID = objUserMasterDTO.ID;
                }
                return oEnterprise;
            }
        }

        public List<MstCompanyMaster> GetCompaniesByName(string Companies, long EnterpriseID, string EnterpriseName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseID", EnterpriseID ?? (object)DBNull.Value), new SqlParameter("@EnterpriseName", EnterpriseName ?? (object)DBNull.Value), new SqlParameter("@CompanyNames", Companies ?? (object)DBNull.Value) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.ExecuteStoreQuery<MstCompanyMaster>("exec [GetCompaniesByName] @EnterpriseID,@EnterpriseName,@CompanyNames", params1).ToList();
            }
        }

        public List<EnterpriseDTO> GetAllEnterprisesWithDBName()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var enterprises = (from x in context.EnterpriseMasters
                                   where x.IsDeleted == false
                                   select new EnterpriseDTO
                                   {
                                       ID = x.ID,
                                       EnterpriseDBName = (!string.IsNullOrEmpty(x.EnterpriseDBName) ? x.EnterpriseDBName : string.Empty)
                                   }).ToList();
                return enterprises;
            }
        }

        public string GetEnterpriseDBNameByID(long EnterpriseID)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return (from x in context.EnterpriseMasters
                        join um in context.UserMasters on x.CreatedBy equals um.ID into x_um_join
                        from x_um in x_um_join.DefaultIfEmpty()
                        join umup in context.UserMasters on x.LastUpdatedBy equals umup.ID into x_umup_join
                        from x_umup in x_umup_join.DefaultIfEmpty()
                        where x.ID == EnterpriseID && x.IsDeleted == false
                        select x.EnterpriseDBName
                        ).FirstOrDefault();

            }
        }

        public IEnumerable<EnterpriseDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, bool IsArchived, bool IsDeleted, List<long> EnterpriseFilter, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            IEnumerable<EnterpriseDTO> ObjCache = GetAllEnterprises(IsArchived, IsDeleted).AsEnumerable<EnterpriseDTO>();
            if (EnterpriseFilter != null && EnterpriseFilter.Count > 0)
            {
                ObjCache = ObjCache.Where(t => EnterpriseFilter.Contains(t.ID));
            }
            string newSearchValue = string.Empty;

            if (String.IsNullOrEmpty(SearchTerm))
            {
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {

                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                if (Fields.Length > 2)
                {
                    if (!string.IsNullOrEmpty(Fields[2]))
                        newSearchValue = Fields[2];
                    else
                        newSearchValue = string.Empty;
                }
                else
                {
                    newSearchValue = string.Empty;
                }

                ObjCache = ObjCache.Where(t =>
                                          ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains((t.CreatedBy ?? 0).ToString())))
                                       && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains((t.LastUpdatedBy ?? 0).ToString())))
                                       && ((Fields[1].Split('@')[2] == "") || (t.Created >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone) && t.Created <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone)))
                                       && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone) && t.Updated.Value <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone)))
                                       );

                ObjCache = ObjCache.Where
                   (
                       t => t.Name.IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.Address ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.City ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.State ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.PostalCode ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.Country ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.SoftwareBasePrice).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.MaxBinsPerBasePrice).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.CostPerBin).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.DiscountPrice1).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.DiscountPrice2).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.DiscountPrice3).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.MaxSubscriptionTier1).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.MaxSubscriptionTier2).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.MaxSubscriptionTier3).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.MaxSubscriptionTier4).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.MaxSubscriptionTier5).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.MaxSubscriptionTier6).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.PriceSubscriptionTier1).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.PriceSubscriptionTier2).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.PriceSubscriptionTier3).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.PriceSubscriptionTier4).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.PriceSubscriptionTier5).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.PriceSubscriptionTier6).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.IncludeLicenseFees).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.MaxLicenseTier1).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.MaxLicenseTier2).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.MaxLicenseTier3).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.MaxLicenseTier4).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.MaxLicenseTier5).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.MaxLicenseTier6).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.PriceLicenseTier1).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.PriceLicenseTier2).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.PriceLicenseTier3).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.PriceLicenseTier4).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.PriceLicenseTier5).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.PriceLicenseTier6).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.Created).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.Updated).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.ContactEmail ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.ContactPhone ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.UDF1 ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.UDF2 ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.UDF3 ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.UDF4 ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.UDF5 ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.Country ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.CreatedByName ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.UpdatedByName ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.CostPerBin.HasValue ? (t.CostPerBin ?? 0).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 : false) ||
                       (t.DiscountPrice1.HasValue ? (t.DiscountPrice1 ?? 0).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 : false) ||
                       (t.DiscountPrice2.HasValue ? (t.DiscountPrice2 ?? 0).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 : false)
                   );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                ObjCache = ObjCache.Where
                    (
                        t => t.Name.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Address ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.City ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.State ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.PostalCode ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.Country ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.SoftwareBasePrice).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.MaxBinsPerBasePrice).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.CostPerBin).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.DiscountPrice1).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.DiscountPrice2).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.DiscountPrice3).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.MaxSubscriptionTier1).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.MaxSubscriptionTier2).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.MaxSubscriptionTier3).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.MaxSubscriptionTier4).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.MaxSubscriptionTier5).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.MaxSubscriptionTier6).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.PriceSubscriptionTier1).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.PriceSubscriptionTier2).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.PriceSubscriptionTier3).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.PriceSubscriptionTier4).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.PriceSubscriptionTier5).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.PriceSubscriptionTier6).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.IncludeLicenseFees).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.MaxLicenseTier1).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.MaxLicenseTier2).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.MaxLicenseTier3).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.MaxLicenseTier4).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.MaxLicenseTier5).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.MaxLicenseTier6).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.PriceLicenseTier1).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.PriceLicenseTier2).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.PriceLicenseTier3).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.PriceLicenseTier4).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.PriceLicenseTier5).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.PriceLicenseTier6).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.Created).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                       (t.Updated).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ContactEmail ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ContactPhone ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Country ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.CreatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.CostPerBin.HasValue ? (t.CostPerBin ?? 0).ToString().IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 : false) ||
                        (t.DiscountPrice1.HasValue ? (t.DiscountPrice1 ?? 0).ToString().IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 : false) ||
                        (t.DiscountPrice2.HasValue ? (t.DiscountPrice2 ?? 0).ToString().IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 : false)
                    );
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
        }
    }
}
