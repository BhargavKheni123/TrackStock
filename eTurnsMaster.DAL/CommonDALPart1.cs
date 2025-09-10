using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurnsMaster.DAL
{
    public partial class CommonMasterDAL : eTurnsMasterBaseDAL
    {
        public string UserDuplicateCheck(long ID, string UserEmail)
        {
            string msg = "";
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var qry = (from em in context.UserMasters
                           where em.Email == UserEmail && em.IsArchived == false && em.IsDeleted == false && em.ID != ID
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

        public string RoleDuplicateCheck(long ID, string Rolename, int UserType, long EnterpriseId)
        {
            if (UserType == 1)
            {
                string msg = "";
                using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    var qry = (from em in context.RoleMasters
                               where em.RoleName == Rolename && em.IsArchived == false && em.IsDeleted == false && em.ID != ID
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
            else
            {
                string msg = "";
                using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    var qry = (from em in context.RoleMasters
                               where em.RoleName == Rolename && em.IsArchived == false && em.IsDeleted == false && em.ID != ID && em.EnterpriseId == EnterpriseId
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
        }

        public Dictionary<string, int> GetNarrowDDData(string TableName, string TextFieldName, bool IsArchived, bool IsDeleted, int LoggedInUserType, long? EnterpriseId, long? CompanyId, long? RoomId)
        {
            Dictionary<string, int> ColUDFData = new Dictionary<string, int>();
            switch (TableName)
            {
                case "EnterpriseMaster":
                    if (TextFieldName == "LastUpdatedBy")
                    {
                        using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                        {

                            var lstsupps = (from ci in context.EnterpriseMasters
                                            join um in context.UserMasters on ci.LastUpdatedBy equals um.ID
                                            where ci.IsArchived == IsArchived && ci.IsDeleted == IsDeleted
                                            orderby um.UserName
                                            group ci by new { ci.LastUpdatedBy, um.UserName } into grpms
                                            select new
                                            {
                                                count = grpms.Count(),
                                                sid = grpms.Key.LastUpdatedBy,
                                                supname = grpms.Key.UserName
                                            });

                            return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                        }
                    }
                    else if (TextFieldName == "CreatedBy")
                    {
                        using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                        {

                            var lstsupps = (from ci in context.EnterpriseMasters
                                            join um in context.UserMasters on ci.CreatedBy equals um.ID
                                            where ci.IsArchived == IsArchived && ci.IsDeleted == IsDeleted
                                            orderby um.UserName
                                            group ci by new { ci.CreatedBy, um.UserName } into grpms
                                            select new
                                            {
                                                count = grpms.Count(),
                                                sid = grpms.Key.CreatedBy,
                                                supname = grpms.Key.UserName
                                            });
                            return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                        }
                    }
                    return ColUDFData;
                case "RoleMaster":
                    List<RoleMasterDTO> lstRoles = new RoleMasterDAL().GetAllRoles(LoggedInUserType, EnterpriseId, CompanyId, RoomId);
                    if (TextFieldName == "LastUpdatedBy")
                    {
                        using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                        {

                            var lstsupps = (from ci in lstRoles
                                            join um in context.UserMasters on ci.LastUpdatedBy equals um.ID
                                            where ci.IsArchived == IsArchived && ci.IsDeleted == IsDeleted
                                            orderby um.UserName
                                            group ci by new { ci.LastUpdatedBy, um.UserName } into grpms
                                            select new
                                            {
                                                count = grpms.Count(),
                                                sid = grpms.Key.LastUpdatedBy,
                                                supname = grpms.Key.UserName
                                            });

                            return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                        }
                    }
                    else if (TextFieldName == "CreatedBy")
                    {
                        using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                        {

                            var lstsupps = (from ci in lstRoles
                                            join um in context.UserMasters on ci.CreatedBy equals um.ID
                                            where ci.IsArchived == IsArchived && ci.IsDeleted == IsDeleted
                                            orderby um.UserName
                                            group ci by new { ci.CreatedBy, um.UserName } into grpms
                                            select new
                                            {
                                                count = grpms.Count(),
                                                sid = grpms.Key.CreatedBy,
                                                supname = grpms.Key.UserName
                                            });
                            return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                        }
                    }
                    return ColUDFData;
            }
            return ColUDFData;
        }

        public IEnumerable<object> GetPagedRecords(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string TableName, long ID)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                TotalCount = 0;

                switch (TableName)
                {
                    case "EnterpriseList":
                        TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(ID) FROM EnterpriseMaster_History WHERE ID = " + ID + "").ToList()[0]);
                        return (from u in context.ExecuteStoreQuery<EnterpriseDTO>(@"
                            SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName,'' AS CreatedDate,'' as UpdatedDate
                                FROM EnterpriseMaster_History as A 
                                LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID                                        
                                WHERE A.ID = " + ID + " order by A.HistoryID desc")
                                select new EnterpriseDTO
                                {
                                    ID = u.ID,
                                    HistoryID = u.HistoryID,
                                    Action = u.Action,
                                    Name = u.Name,
                                    Address = u.Address,
                                    City = u.City,
                                    State = u.State,
                                    PostalCode = u.PostalCode,
                                    Country = u.Country,
                                    ContactPhone = u.ContactPhone,
                                    ContactEmail = u.ContactEmail,
                                    SoftwareBasePrice = u.SoftwareBasePrice,
                                    MaxBinsPerBasePrice = u.MaxBinsPerBasePrice,
                                    CostPerBin = u.CostPerBin,
                                    DiscountPrice1 = u.DiscountPrice1,
                                    DiscountPrice2 = u.DiscountPrice2,
                                    DiscountPrice3 = u.DiscountPrice3,
                                    MaxSubscriptionTier1 = u.MaxSubscriptionTier1,
                                    MaxSubscriptionTier2 = u.MaxSubscriptionTier2,
                                    MaxSubscriptionTier3 = u.MaxSubscriptionTier3,
                                    MaxSubscriptionTier4 = u.MaxSubscriptionTier4,
                                    MaxSubscriptionTier5 = u.MaxSubscriptionTier5,
                                    MaxSubscriptionTier6 = u.MaxSubscriptionTier6,
                                    PriceSubscriptionTier1 = u.PriceSubscriptionTier1,
                                    PriceSubscriptionTier2 = u.PriceSubscriptionTier2,
                                    PriceSubscriptionTier3 = u.PriceSubscriptionTier3,
                                    PriceSubscriptionTier4 = u.PriceSubscriptionTier4,
                                    PriceSubscriptionTier5 = u.PriceSubscriptionTier5,
                                    PriceSubscriptionTier6 = u.PriceSubscriptionTier6,
                                    IncludeLicenseFees = u.IncludeLicenseFees,
                                    MaxLicenseTier1 = u.MaxLicenseTier1,
                                    MaxLicenseTier2 = u.MaxLicenseTier2,
                                    MaxLicenseTier3 = u.MaxLicenseTier3,
                                    MaxLicenseTier4 = u.MaxLicenseTier4,
                                    MaxLicenseTier5 = u.MaxLicenseTier5,
                                    MaxLicenseTier6 = u.MaxLicenseTier6,
                                    PriceLicenseTier1 = u.PriceLicenseTier1,
                                    PriceLicenseTier2 = u.PriceLicenseTier2,
                                    PriceLicenseTier3 = u.PriceLicenseTier3,
                                    PriceLicenseTier4 = u.PriceLicenseTier4,
                                    PriceLicenseTier5 = u.PriceLicenseTier5,
                                    PriceLicenseTier6 = u.PriceLicenseTier6,
                                    UDF1 = u.UDF1,
                                    UDF2 = u.UDF2,
                                    UDF3 = u.UDF3,
                                    UDF4 = u.UDF4,
                                    UDF5 = u.UDF5,
                                    GUID = u.GUID,
                                    Created = u.Created,
                                    Updated = u.Updated,
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    CreatedDate = u.CreatedDate,
                                    UpdatedDate = u.UpdatedDate,
                                }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
                    default:
                        return null;
                }
            }
        }

        public IEnumerable<eTurns.DTO.Resources.ResourceLanguageDTO> GetCachedResourceLanguageData(Int64 CompanyID)
        {
            IEnumerable<eTurns.DTO.Resources.ResourceLanguageDTO> ObjCache = CacheHelper<IEnumerable<eTurns.DTO.Resources.ResourceLanguageDTO>>.GetCacheItem("Cached_MasterLanguages_" + CompanyID);

            if (ObjCache == null || ObjCache.Count() <= 0)
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<eTurns.DTO.Resources.ResourceLanguageDTO> obj = (from u in context.ExecuteStoreQuery<eTurns.DTO.Resources.ResourceLanguageDTO>(@"SELECT * FROM Languages Order by Language")
                                                                                 select new eTurns.DTO.Resources.ResourceLanguageDTO
                                                                                 {
                                                                                     ID = u.ID,
                                                                                     Culture = u.Culture,
                                                                                     Language = u.Language
                                                                                 }).AsParallel().ToList();

                    ObjCache = CacheHelper<IEnumerable<eTurns.DTO.Resources.ResourceLanguageDTO>>.AddCacheItem("Cached_MasterLanguages_" + CompanyID, obj);
                }
            }

            return ObjCache;
        }

        public void DeleteSchedulerErrorLog(long ScheduleID, int ScheduleFor)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                IQueryable<SchedulerErrorLog> lstErrors = context.SchedulerErrorLogs.Where(t => t.SchedulerID == ScheduleID && t.ScheduleFor == ScheduleFor);
                if (lstErrors.Any())
                {
                    foreach (var item in lstErrors)
                    {
                        context.SchedulerErrorLogs.DeleteObject(item);
                    }
                    context.SaveChanges();
                }
            }
        }

        public List<SchedulerErrorLog> getLogByScheduleID(long ScheduleID, int ScheduleFor)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.SchedulerErrorLogs.Where(t => t.ScheduleFor == ScheduleFor && t.SchedulerID == ScheduleID).ToList();
            }
        }
    }
}
