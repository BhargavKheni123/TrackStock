using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace eTurns.DAL
{
    public class EnterpriseDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        //public EnterpriseDAL(base.DataBaseName)
        //{

        //}

        public EnterpriseDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public EnterpriseDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]
        public EnterpriseDTO GetEnterprise(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from x in context.EnterpriseMasters
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
                            IsActive = x.IsActive,
                            EnterpriseSuperAdmin = x.EnterpriseSuperAdmin,
                            EnterPriseDomainURL = x.EnterpriseDomainURL,
                            ArchiveDbName = x.ArchiveDbName,
                            ChangeLogDbName = x.ChangeLogDbName,
                            HistoryDBName = x.HistoryDBName,
                            AllowABIntegration = x.AllowABIntegration
                        }).FirstOrDefault();

            }
        }
        public EnterpriseDTO GetEnterpriseByDbName(string DBName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                return (from x in context.EnterpriseMasters
                        join um in context.UserMasters on x.CreatedBy equals um.ID into x_um_join
                        from x_um in x_um_join.DefaultIfEmpty()
                        join umup in context.UserMasters on x.LastUpdatedBy equals umup.ID into x_umup_join
                        from x_umup in x_umup_join.DefaultIfEmpty()
                        where x.EnterpriseDBName == DBName
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
                            IsActive = x.IsActive,
                            EnterPriseDomainURL = x.EnterpriseDomainURL,
                            ArchiveDbName = x.ArchiveDbName,
                            ChangeLogDbName = x.ChangeLogDbName,
                            HistoryDBName = x.HistoryDBName,
                            AllowABIntegration = x.AllowABIntegration
                        }).FirstOrDefault();

            }
        }
        public EnterpriseDTO Insert(EnterpriseDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
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
                    UDF1 = objDTO.UDF1,
                    UDF2 = objDTO.UDF2,
                    UDF3 = objDTO.UDF3,
                    UDF4 = objDTO.UDF4,
                    UDF5 = objDTO.UDF5,
                    GUID = Guid.NewGuid(),
                    Updated = DateTime.Now,
                    Created = DateTime.Now,
                    CreatedBy = objDTO.CreatedBy,
                    LastUpdatedBy = objDTO.LastUpdatedBy,
                    IsDeleted = false,
                    IsArchived = false,
                    IsActive = objDTO.IsActive,
                    EnterpriseDBName = objDTO.EnterpriseDBName,
                    //EnterpriseDBConnectionString = objDTO.EnterpriseDBConnectionString,
                    EnterPriseLogo = objDTO.EnterpriseLogo,
                    EnterpriseSuperAdmin = objDTO.EnterpriseSuperAdmin,
                    EnterpriseDomainURL = objDTO.EnterPriseDomainURL,
                    ArchiveDbName = objDTO.ArchiveDbName,
                    ChangeLogDbName = objDTO.ChangeLogDbName,
                    HistoryDBName = objDTO.HistoryDBName,
                    AllowABIntegration = objDTO.AllowABIntegration,
                };
                context.EnterpriseMasters.Add(obj);
                context.SaveChanges();
                //var objEnterpriseConfig = new EnterPriseConfig
                //{
                //    EnterpriseID=obj.ID,
                //    PreviousLastAllowedPWD = 5,
                //    PasswordExpiryDays = 180,
                //    PasswordExpiryWarningDays = 30,
                //    DisplayAgreePopupDays = 180,
                //    NotAllowedCharacter = "'",
                //};
                //context.EnterPriseConfigs(objEnterpriseConfig);
                //context.SaveChanges();
                CacheHelper<ResourceLanguageDTO>.InvalidateCache();
                return GetEnterprise(obj.ID);


            }
        }
        public EnterpriseDTO Edit(EnterpriseDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                EnterpriseMaster obj = context.EnterpriseMasters.FirstOrDefault(t => t.ID == objDTO.ID);
                
                if (obj != null)
                {
                    var abIntegrationFlagBeforeUpdate = obj.AllowABIntegration;

                    obj.ID = objDTO.ID;
                    obj.Name = objDTO.Name;
                    obj.Address = objDTO.Address;
                    obj.City = objDTO.City;
                    obj.State = objDTO.State;
                    obj.PostalCode = objDTO.PostalCode;
                    obj.Country = objDTO.Country;
                    obj.ContactPhone = objDTO.ContactPhone;
                    obj.ContactEmail = objDTO.ContactEmail;
                    obj.UDF1 = objDTO.UDF1;
                    obj.UDF2 = objDTO.UDF2;
                    obj.UDF3 = objDTO.UDF3;
                    obj.UDF4 = objDTO.UDF4;
                    obj.UDF5 = objDTO.UDF5;
                    obj.Updated = DateTime.Now;
                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    obj.IsActive = objDTO.IsActive;
                    obj.EnterpriseSuperAdmin = objDTO.EnterpriseSuperAdmin;
                    obj.AllowABIntegration = objDTO.AllowABIntegration;

                    context.SaveChanges();
                    CacheHelper<ResourceLanguageDTO>.InvalidateCache();

                    if (abIntegrationFlagBeforeUpdate && !objDTO.AllowABIntegration)
                    {
                        OffABIntegrationForAllRoomsOfEnterprise();
                    }

                    return GetEnterprise(obj.ID);
                }
                else
                {
                    return GetEnterprise(objDTO.ID);
                }
            }
        }
        public string GetEnterpriseDBName(Int64 EnterPriseId)
        {
            string EnterpriseDBName = string.Empty;
            base.DataBaseName = DbConnectionHelper.GetETurnsMasterDBName();

            EnterpriseDTO oEnterpriseDTO = GetEnterprise(EnterPriseId);
            if (oEnterpriseDTO != null)
                EnterpriseDBName = oEnterpriseDTO.EnterpriseDBName;

            return EnterpriseDBName;
        }
        public EnterpriseDTO GetEnterpriseFromeTurnsMasterDB(Int64 EnterPriseId)
        {
            string EnterpriseDBName = string.Empty;
            base.DataBaseName = DbConnectionHelper.GetETurnsMasterDBName();

            EnterpriseDTO oEnterpriseDTO = GetEnterprise(EnterPriseId);

            return oEnterpriseDTO;
        }

        public EnterpriseDTO GetEnterpriseByEnterpriseName(string EnterpriseName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                return (from x in context.EnterpriseMasters
                        join um in context.UserMasters on x.CreatedBy equals um.ID into x_um_join
                        from x_um in x_um_join.DefaultIfEmpty()
                        join umup in context.UserMasters on x.LastUpdatedBy equals umup.ID into x_umup_join
                        from x_umup in x_umup_join.DefaultIfEmpty()
                        where x.Name == EnterpriseName
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
                            IsActive = x.IsActive,
                            EnterPriseDomainURL = x.EnterpriseDomainURL,
                            ArchiveDbName = x.ArchiveDbName,
                            ChangeLogDbName = x.ChangeLogDbName,
                            HistoryDBName = x.HistoryDBName,
                            AllowABIntegration = x.AllowABIntegration
                        }).FirstOrDefault();

            }
        }

        public List<ResourceLanguageDTO> CheckLanguageChangebyHistoryDate(DateTime HistoryDate)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@HistoryDate", HistoryDate) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ResourceLanguageDTO>("exec [CheckLanguageChangebyHistoryDate] @HistoryDate", params1).ToList();
            }
        }

        public void OffABIntegrationForAllRoomsOfEnterprise()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("exec [OffABIntegrationForAllRoomsOfEnterprise]");
            }
        }

        #endregion
    }
}
