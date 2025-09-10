using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using eTurns.DAL;
using System.Data.Objects;
using eTurns.DTO;
using Dynamite;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Collections;
using System.Configuration;
using System.Web;
using eTurns.DTO.Resources;

namespace eTurns.DAL
{
    public partial class CompanyMasterDAL : eTurnsBaseDAL
    {
        public IEnumerable<CompanyMasterDTO> GetAllRecords()
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                return (from x in context.Database.SqlQuery<CompanyMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName,'' AS CreatedDate,'' AS UpdatedDate FROM CompanyMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE ISNULL(A.IsArchived,0)!=1 AND ISNULL(A.IsDeleted,0)!=1")
                        select new CompanyMasterDTO
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
                            Created = x.Created,
                            Updated = x.Updated,
                            CreatedBy = x.CreatedBy,
                            LastUpdatedBy = x.LastUpdatedBy,
                            IsDeleted = x.IsDeleted,
                            GUID = x.GUID,
                            IsActive = x.IsActive,
                            CreatedDate = x.CreatedDate,
                            UpdatedDate = x.UpdatedDate
                        }).ToList();

            }
        }
        public CompanyMasterDTO GetRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from x in context.CompanyMasters
                        join um in context.UserMasters on x.CreatedBy equals um.ID into x_um_join
                        from x_um in x_um_join.DefaultIfEmpty()
                        join umup in context.UserMasters on x.LastUpdatedBy equals umup.ID into x_umup_join
                        from x_umup in x_umup_join.DefaultIfEmpty()
                        where x.ID == id
                        select new CompanyMasterDTO
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
                            Created = x.Created,
                            Updated = x.Updated,
                            CreatedBy = x.CreatedBy,
                            LastUpdatedBy = x.LastUpdatedBy,
                            IsDeleted = x.IsDeleted,
                            GUID = x.GUID,
                            CreatedByName = x_um.UserName,
                            UpdatedByName = x_umup.UserName,
                            CompanyLogo = x.CompanyLogo,
                            IsActive = x.IsActive,
                            IsIncludeCommonBOM = x.IsIncludeCommonBOM
                        }).FirstOrDefault();
            }
        }
        public IEnumerable<CompanyMasterDTO> GetAllCompanies(bool IsArchived, bool IsDeleted, List<CompanyMasterDTO> lstCompanies)
        {
            long[] CompanyIds = new long[] { };
            if (lstCompanies == null)
            {
                lstCompanies = new List<CompanyMasterDTO>();
            }
            CompanyIds = lstCompanies.Select(t => t.ID).ToArray();
            IEnumerable<CompanyMasterDTO> lstAllCompanies = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (IsArchived == false && IsDeleted == false)
                {
                    lstAllCompanies = (from x in context.CompanyMasters
                                       join um in context.UserMasters on x.CreatedBy equals um.ID into x_um_join
                                       from x_um in x_um_join.DefaultIfEmpty()
                                       join umup in context.UserMasters on x.LastUpdatedBy equals umup.ID into x_umup_join
                                       from x_umup in x_umup_join.DefaultIfEmpty()
                                       where CompanyIds.Contains(x.ID) && (x.IsArchived ?? false) == false && (x.IsDeleted ?? false) == false
                                       select new CompanyMasterDTO
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
                                           Created = x.Created,
                                           Updated = x.Updated,
                                           CreatedBy = x.CreatedBy,
                                           LastUpdatedBy = x.LastUpdatedBy,
                                           IsDeleted = x.IsDeleted ?? false,
                                           GUID = x.GUID,
                                           CreatedByName = x_um.UserName,
                                           UpdatedByName = x_umup.UserName,
                                           IsArchived = x.IsArchived ?? false,
                                           IsActive = x.IsActive,
                                       }).ToList();
                }
                else
                {
                    if (IsArchived && IsDeleted)
                    {
                        lstAllCompanies = (from x in context.CompanyMasters
                                           join um in context.UserMasters on x.CreatedBy equals um.ID into x_um_join
                                           from x_um in x_um_join.DefaultIfEmpty()
                                           join umup in context.UserMasters on x.LastUpdatedBy equals umup.ID into x_umup_join
                                           from x_umup in x_umup_join.DefaultIfEmpty()
                                           where CompanyIds.Contains(x.ID) && (x.IsArchived ?? false) == true && (x.IsDeleted ?? false) == true
                                           select new CompanyMasterDTO
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
                                               Created = x.Created,
                                               Updated = x.Updated,
                                               CreatedBy = x.CreatedBy,
                                               LastUpdatedBy = x.LastUpdatedBy,
                                               IsDeleted = x.IsDeleted,
                                               GUID = x.GUID,
                                               CreatedByName = x_um.UserName,
                                               UpdatedByName = x_umup.UserName,
                                               IsActive = x.IsActive
                                           }).ToList();
                    }
                    else if (IsArchived)
                    {
                        lstAllCompanies = (from x in context.CompanyMasters
                                           join um in context.UserMasters on x.CreatedBy equals um.ID into x_um_join
                                           from x_um in x_um_join.DefaultIfEmpty()
                                           join umup in context.UserMasters on x.LastUpdatedBy equals umup.ID into x_umup_join
                                           from x_umup in x_umup_join.DefaultIfEmpty()
                                           where CompanyIds.Contains(x.ID) && (x.IsArchived ?? false) == true
                                           select new CompanyMasterDTO
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
                                               Created = x.Created,
                                               Updated = x.Updated,
                                               CreatedBy = x.CreatedBy,
                                               LastUpdatedBy = x.LastUpdatedBy,
                                               IsDeleted = x.IsDeleted,
                                               GUID = x.GUID,
                                               CreatedByName = x_um.UserName,
                                               UpdatedByName = x_umup.UserName,
                                               IsActive = x.IsActive
                                           }).ToList();
                    }
                    else if (IsDeleted)
                    {
                        lstAllCompanies = (from x in context.CompanyMasters
                                           join um in context.UserMasters on x.CreatedBy equals um.ID into x_um_join
                                           from x_um in x_um_join.DefaultIfEmpty()
                                           join umup in context.UserMasters on x.LastUpdatedBy equals umup.ID into x_umup_join
                                           from x_umup in x_umup_join.DefaultIfEmpty()
                                           where CompanyIds.Contains(x.ID) && (x.IsDeleted ?? false) == true
                                           select new CompanyMasterDTO
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
                                               Created = x.Created,
                                               Updated = x.Updated,
                                               CreatedBy = x.CreatedBy,
                                               LastUpdatedBy = x.LastUpdatedBy,
                                               IsDeleted = x.IsDeleted,
                                               GUID = x.GUID,
                                               CreatedByName = x_um.UserName,
                                               UpdatedByName = x_umup.UserName,
                                               IsActive = x.IsActive
                                           }).ToList();
                    }
                }
            }
            return lstAllCompanies;
        }
        public CompanyMasterDTO GetCompanYByEnterpriseIDAndCompanyID(long EnterpriseID, long CompanyID)
        {

            CompanyMasterDTO lstAllCompanies = new CompanyMasterDTO();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstAllCompanies = (from x in context.CompanyMasters
                                   join um in context.UserMasters on x.CreatedBy equals um.ID into x_um_join
                                   from x_um in x_um_join.DefaultIfEmpty()
                                   join umup in context.UserMasters on x.LastUpdatedBy equals umup.ID into x_umup_join
                                   from x_umup in x_umup_join.DefaultIfEmpty()
                                   where (x.IsArchived ?? false) == false && (x.IsDeleted ?? false) == false && x.IsActive == true
                                   && x.ID == CompanyID
                                   select new CompanyMasterDTO
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
                                       Created = x.Created,
                                       Updated = x.Updated,
                                       CreatedBy = x.CreatedBy,
                                       LastUpdatedBy = x.LastUpdatedBy,
                                       IsDeleted = x.IsDeleted ?? false,
                                       GUID = x.GUID,
                                       CreatedByName = x_um.UserName,
                                       UpdatedByName = x_umup.UserName,
                                       IsArchived = x.IsArchived ?? false,
                                       IsActive = x.IsActive

                                   }).FirstOrDefault();
            }
            return lstAllCompanies;
        }
        //public CompanyMasterDTO GetCompanyByName(string Name)
        //{
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        return (from x in context.CompanyMasters
        //                join um in context.UserMasters on x.CreatedBy equals um.ID into x_um_join
        //                from x_um in x_um_join.DefaultIfEmpty()
        //                join umup in context.UserMasters on x.LastUpdatedBy equals umup.ID into x_umup_join
        //                from x_umup in x_umup_join.DefaultIfEmpty()
        //                where x.Name == Name && (x.IsDeleted ?? false) == false
        //                select new CompanyMasterDTO
        //                {
        //                    ID = x.ID,
        //                    Name = x.Name,
        //                    Address = x.Address,
        //                    City = x.City,
        //                    State = x.State,
        //                    PostalCode = x.PostalCode,
        //                    Country = x.Country,
        //                    ContactPhone = x.ContactPhone,
        //                    ContactEmail = x.ContactEmail,
        //                    UDF1 = x.UDF1,
        //                    UDF2 = x.UDF2,
        //                    UDF3 = x.UDF3,
        //                    UDF4 = x.UDF4,
        //                    UDF5 = x.UDF5,
        //                    Created = x.Created,
        //                    Updated = x.Updated,
        //                    CreatedBy = x.CreatedBy,
        //                    LastUpdatedBy = x.LastUpdatedBy,
        //                    IsDeleted = x.IsDeleted,
        //                    GUID = x.GUID,
        //                    CreatedByName = x_um.UserName,
        //                    UpdatedByName = x_umup.UserName,
        //                    CompanyLogo = x.CompanyLogo,
        //                    IsActive = x.IsActive,
        //                    IsIncludeCommonBOM = x.IsIncludeCommonBOM
        //                }).FirstOrDefault();
        //    }
        //}
        public bool DeleteRecords(string IDs, int userid)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE CompanyMaster SET Updated = '" + DateTime.Now.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.Database.ExecuteSqlCommand(strQuery);
                return true;
            }
        }
        public IEnumerable<CompanyMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strSortinitializer = "";

                if (sortColumnName.ToUpper().Contains("CREATEDBYNAME"))
                {
                    strSortinitializer = "B";
                    sortColumnName = "UserName";
                }
                else if (sortColumnName.ToUpper().Contains("UPDATEDBYNAME"))
                {
                    strSortinitializer = "C";
                    sortColumnName = "UserName";
                }
                else
                {
                    strSortinitializer = "A";
                }

                strSortinitializer = strSortinitializer + "." + sortColumnName;


                if (String.IsNullOrEmpty(SearchTerm))
                {
                    TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM CompanyMaster WHERE IsDeleted!=1 AND IsArchived != 1").ToList()[0]);

                    return (from x in context.Database.SqlQuery<CompanyMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName,'' AS CreatedDate,'' AS UpdatedDate FROM CompanyMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 ORDER BY " + strSortinitializer)
                            select new CompanyMasterDTO
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
                                Created = x.Created,
                                Updated = x.Updated,
                                CreatedBy = x.CreatedBy,
                                LastUpdatedBy = x.LastUpdatedBy,
                                IsDeleted = x.IsDeleted,
                                GUID = x.GUID,
                                UDF1 = x.UDF1,
                                UDF2 = x.UDF2,
                                UDF3 = x.UDF3,
                                UDF4 = x.UDF4,
                                UDF5 = x.UDF5,
                                IsActive = x.IsActive,
                                CreatedDate = x.CreatedDate,
                                UpdatedDate = x.UpdatedDate
                            }).Skip(StartRowIndex).Take(MaxRows).ToList();
                }
                else if (SearchTerm.Contains("[###]"))
                {
                    string search = "";
                    string[] dd = SearchTerm.Replace("[###]", "$").Split('$');
                    string[] stringSeparators = new string[] { "[###]" };

                    if (dd != null && dd.Length > 0)
                    {
                        string[] Fields = dd[1].Split(stringSeparators, StringSplitOptions.None);
                        if (Fields != null && Fields.Length > 0)
                        {
                            // 6 counts for fields based on that prepare the search string
                            // 1 = createdBy, 2 = updatedby , 3 = CreatedFrom, 4 = CreatedTo, 5=UpdatedFrom,6=UpdatedTo
                            foreach (var item in Fields)
                            {
                                if (item.Length > 0)
                                {
                                    if (item.Contains("CreatedBy"))
                                    {
                                        search += " A.CreatedBy in (" + item.Split('#')[1] + ")";
                                    }
                                    if (item.Contains("UpdatedBy"))
                                    {
                                        if (search.Length > 0)
                                            search += " AND ";
                                        search += " A.LastUpdatedBy in (" + item.Split('#')[1] + ")";
                                    }
                                    if (item.Contains("DateCreatedFrom"))
                                    {
                                        if (search.Length > 0)
                                            search += " AND ";
                                        string[] CreatedDateFrom = item.Split('#');
                                        search += " (A.Created >= DATETIME('" + CreatedDateFrom[1] + "') AND A.Created <= DATETIME('" + CreatedDateFrom[3] + "'))";
                                    }
                                    if (item.Contains("DateUpdatedFrom"))
                                    {
                                        if (search.Length > 0)
                                            search += " AND ";
                                        string[] UpdatedDateFrom = item.Split('#');
                                        search += " (A.Updated >= DATETIME('" + UpdatedDateFrom[1] + "') AND A.Updated <= DATETIME('" + UpdatedDateFrom[3] + "'))";
                                    }
                                }
                            }
                        }
                    }

                    if (search.Length > 0)
                    {
                        search = " AND (" + search + " )";
                    }

                    TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(A.ID) FROM CompanyMaster as A WHERE A.IsDeleted!=1 AND A.IsArchived != 1 " + search + "").ToList()[0]);

                    return (from x in context.Database.SqlQuery<CompanyMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName,'' AS CreatedDate,'' AS UpdatedDate FROM CompanyMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 " + search + " ORDER BY " + strSortinitializer)
                            select new CompanyMasterDTO
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
                                Created = x.Created,
                                Updated = x.Updated,
                                CreatedBy = x.CreatedBy,
                                LastUpdatedBy = x.LastUpdatedBy,
                                IsDeleted = x.IsDeleted,
                                GUID = x.GUID,
                                UDF1 = x.UDF1,
                                UDF2 = x.UDF2,
                                UDF3 = x.UDF3,
                                UDF4 = x.UDF4,
                                UDF5 = x.UDF5,
                                IsActive = x.IsActive,
                                CreatedDate = x.CreatedDate,
                                UpdatedDate = x.UpdatedDate

                            }).Skip(StartRowIndex).Take(MaxRows).ToList();
                }
                else
                {
                    TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM CompanyMaster WHERE IsDeleted!=1 AND IsArchived != 1 AND ((Name like '%" + SearchTerm + "%') OR (ID like '%" + SearchTerm + "%') OR (Address like '%" + SearchTerm + "%') OR (City like '%" + SearchTerm + "%') OR (State like '%" + SearchTerm + "%') OR (PostalCode like '%" + SearchTerm + "%') OR (Country like '%" + SearchTerm + "%'))").ToList()[0]);
                    return (from x in context.Database.SqlQuery<CompanyMasterDTO>("SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName,'' AS CreatedDate,'' AS UpdatedDate FROM CompanyMaster  A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND ((A.Name like '%" + SearchTerm + "%') OR (A.ID like '%" + SearchTerm + "%') OR (A.Address like '%" + SearchTerm + "%') OR (A.City like '%" + SearchTerm + "%') OR (A.State like '%" + SearchTerm + "%') OR (A.PostalCode like '%" + SearchTerm + "%') OR (A.Country like '%" + SearchTerm + "%')) ORDER BY " + strSortinitializer)
                            select new CompanyMasterDTO
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
                                Created = x.Created,
                                Updated = x.Updated,
                                CreatedBy = x.CreatedBy,
                                LastUpdatedBy = x.LastUpdatedBy,
                                IsDeleted = x.IsDeleted,
                                GUID = x.GUID,
                                UDF1 = x.UDF1,
                                UDF2 = x.UDF2,
                                UDF3 = x.UDF3,
                                UDF4 = x.UDF4,
                                UDF5 = x.UDF5,
                                IsActive = x.IsActive,
                                CreatedDate = x.CreatedDate,
                                UpdatedDate = x.UpdatedDate
                            }).Skip(StartRowIndex).Take(MaxRows).ToList();
                }
            }
        }
        public List<CompanyMasterDTO> GetAllCompaniesByEnterpriseID(long EnterpriseID, string DBConnectionstring)
        {
            CommonDAL objCommonDAL = new CommonDAL(base.DataBaseName);
            EnterpriseDTO objEnterpriseDTO = objCommonDAL.GetEnterpriseByID(EnterpriseID);
            //  string DBConnectionstring = DbConnectionHelper.GeteTurnsEntityFWConnectionString(objEnterpriseDTO.EnterpriseDBName, DbConnectionType.EFReadWrite.ToString("F"));

            List<CompanyMasterDTO> lstAllCompanies = new List<CompanyMasterDTO>();

            using (var context = new eTurnsEntities(DBConnectionstring))
            {
                lstAllCompanies = (from x in context.CompanyMasters
                                   join um in context.UserMasters on x.CreatedBy equals um.ID into x_um_join
                                   from x_um in x_um_join.DefaultIfEmpty()
                                   join umup in context.UserMasters on x.LastUpdatedBy equals umup.ID into x_umup_join
                                   from x_umup in x_umup_join.DefaultIfEmpty()
                                   where (x.IsArchived ?? false) == false && (x.IsDeleted ?? false) == false && x.IsActive == true
                                   select new CompanyMasterDTO
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
                                       Created = x.Created,
                                       Updated = x.Updated,
                                       CreatedBy = x.CreatedBy,
                                       LastUpdatedBy = x.LastUpdatedBy,
                                       IsDeleted = x.IsDeleted ?? false,
                                       GUID = x.GUID,
                                       CreatedByName = x_um.UserName,
                                       UpdatedByName = x_umup.UserName,
                                       IsArchived = x.IsArchived ?? false,
                                       IsActive = x.IsActive


                                   }).ToList();
            }
            return lstAllCompanies;
        }
        public CompanyMasterDTO GetRecord(Int64 id, string DBConnectionstring)
        {
            using (var context = new eTurnsEntities(DBConnectionstring))
            {
                return (from x in context.CompanyMasters
                        join um in context.UserMasters on x.CreatedBy equals um.ID into x_um_join
                        from x_um in x_um_join.DefaultIfEmpty()
                        join umup in context.UserMasters on x.LastUpdatedBy equals umup.ID into x_umup_join
                        from x_umup in x_umup_join.DefaultIfEmpty()
                        where x.ID == id
                        select new CompanyMasterDTO
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
                            Created = x.Created,
                            Updated = x.Updated,
                            CreatedBy = x.CreatedBy,
                            LastUpdatedBy = x.LastUpdatedBy,
                            IsDeleted = x.IsDeleted,
                            GUID = x.GUID,
                            CreatedByName = x_um.UserName,
                            UpdatedByName = x_umup.UserName,
                            CompanyLogo = x.CompanyLogo,
                            IsActive = x.IsActive
                        }).FirstOrDefault();
            }
        }
        public IEnumerable<CompanyMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<CompanyMasterDTO> ObjCache = GetAllCompanies(IsArchived, IsDeleted, null).AsEnumerable<CompanyMasterDTO>();
            if (String.IsNullOrEmpty(SearchTerm))
            {
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                ObjCache = ObjCache.Where(t =>
                                          ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains((t.CreatedBy ?? 0).ToString())))
                                       && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains((t.LastUpdatedBy ?? 0).ToString())))
                                       && ((Fields[1].Split('@')[2] == "") || (t.Created.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                                       && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
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
                        (t.ContactEmail ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ContactPhone ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Country ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.CreatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    );
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
        }
        public bool Delete(Int64 id, Int64 userid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CompanyMaster obj = context.CompanyMasters.Single(t => t.ID == id);
                obj.IsDeleted = true;
                obj.IsArchived = false;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = userid;
                context.CompanyMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return true;
            }
        }

        public string UpdateData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "UPDATE CompanyMaster SET " + columnName + " = '" + value + "', Updated = GETUTCDATE() WHERE ID=" + id;
                context.ExecuteStoreCommand(strQuery);
            }
            return value;
        }

        public bool UnDeleteCompanyWiseRecords(string ID, Int64 userid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                if (!string.IsNullOrEmpty(ID.Trim()) && ID.Trim() != userid.ToString())
                {
                    strQuery += "UPDATE CompanyMaster SET Updated = '" + DateTimeUtility.DateTimeNow.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=0 WHERE ID =" + ID.Trim() + ";";
                }

                if (!string.IsNullOrEmpty(strQuery))
                    context.ExecuteStoreCommand(strQuery);
            }
            return true;
        }

    }
}
