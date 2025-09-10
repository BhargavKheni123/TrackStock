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
    public partial class CompanyMasterDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public CompanyMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public CompanyMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]

        public List<CompanyMasterDTO> GetAllCompanies()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CompanyMasterDTO>("exec [GetAllCompanies]").ToList();
            }
        }
        public CompanyMasterDTO GetCompanyByID(long ID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CompanyMasterDTO>("exec [GetCompanyByID] @ID", params1).FirstOrDefault();
            }
        }

        public CompanyMasterDTO GetCompanyByIDNormal(long ID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CompanyMasterDTO>("exec [GetCompanyByIDNormal] @ID", params1).FirstOrDefault();
            }
        }

        public CompanyMasterDTO GetCompanyByIDPlain(long ID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CompanyMasterDTO>("exec [GetCompanyByIDPlain] @ID", params1).FirstOrDefault();
            }
        }
        public List<CompanyMasterDTO> GetCompaniesByIds(string IDs)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@IDs", IDs ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CompanyMasterDTO>("exec [GetCompaniesByIds] @IDs", params1).ToList();
            }
        }
        public CompanyMasterDTO GetCompanyByName(string CompanyName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CompanyName", CompanyName ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CompanyMasterDTO>("exec [GetCompanyByName] @CompanyName", params1).FirstOrDefault();
            }
        }
        public List<CompanyMasterDTO> GetCompanyByNameSearch(string CompanyName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CompanyName", CompanyName ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CompanyMasterDTO>("exec [GetCompanyByNameSearch] @CompanyName", params1).ToList();
            }
        }
        public Int64 Insert(CompanyMasterDTO objDTO)
        {

            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //Assign & Mapping of the DTO Class property to Entity Class 
                var obj = new CompanyMaster
                {
                    ID = objDTO.ID,
                    Name = objDTO.Name,
                    CompanyNumber = objDTO.CompanyNumber,
                    Address = objDTO.Address,
                    City = objDTO.City,
                    State = objDTO.State,
                    PostalCode = objDTO.PostalCode,
                    Country = objDTO.Country,
                    ContactPhone = objDTO.ContactPhone,
                    ContactEmail = objDTO.ContactEmail,
                    Created = DateTimeUtility.DateTimeNow,
                    Updated = DateTimeUtility.DateTimeNow,
                    CreatedBy = objDTO.CreatedBy,
                    LastUpdatedBy = objDTO.LastUpdatedBy,
                    IsDeleted = (bool)objDTO.IsDeleted,
                    IsArchived = (bool)objDTO.IsArchived,
                    GUID = objDTO.GUID,
                    UDF1 = objDTO.UDF1,
                    UDF2 = objDTO.UDF2,
                    UDF3 = objDTO.UDF3,
                    UDF4 = objDTO.UDF4,
                    UDF5 = objDTO.UDF5,
                    CompanyLogo = objDTO.CompanyLogo,
                    IsActive = objDTO.IsActive
                };
                context.CompanyMasters.Add(obj);
                context.SaveChanges();

                //Commited By Esha. Moved this Code To Controller   

                //CompanyConfig objCompanyConfig = new CompanyConfig();
                //objCompanyConfig.CompanyID = obj.ID;
                //context.CompanyConfigs.Add(objCompanyConfig);
                //context.SaveChanges();

                return obj.ID;
            }
        }
        public CompanyMasterDTO Edit(CompanyMasterDTO objDTO)
        {
            CompanyMaster objCompanyMaster = new CompanyMaster();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objCompanyMaster = context.CompanyMasters.FirstOrDefault(t => t.ID == objDTO.ID);
                if (objCompanyMaster.IsActive != objDTO.IsActive)
                {
                    objDTO.IsStatusChanged = true;
                }
                if (objCompanyMaster != null)
                {
                    objCompanyMaster.Name = objDTO.Name;
                    objCompanyMaster.CompanyNumber = objDTO.CompanyNumber;
                    objCompanyMaster.Address = objDTO.Address;
                    objCompanyMaster.City = objDTO.City;
                    objCompanyMaster.State = objDTO.State;
                    objCompanyMaster.PostalCode = objDTO.PostalCode;
                    objCompanyMaster.Country = objDTO.Country;
                    objCompanyMaster.ContactPhone = objDTO.ContactPhone;
                    objCompanyMaster.ContactEmail = objDTO.ContactEmail;
                    objCompanyMaster.Updated = DateTimeUtility.DateTimeNow;
                    objCompanyMaster.LastUpdatedBy = objDTO.LastUpdatedBy;
                    objCompanyMaster.UDF1 = objDTO.UDF1;
                    objCompanyMaster.UDF2 = objDTO.UDF2;
                    objCompanyMaster.UDF3 = objDTO.UDF3;
                    objCompanyMaster.UDF4 = objDTO.UDF4;
                    objCompanyMaster.UDF5 = objDTO.UDF5;
                    objCompanyMaster.IsActive = objDTO.IsActive;
                    context.SaveChanges();
                }
                return objDTO;

            }
        }
        public bool UpdateIncludeCommonBOM(long CompanyId, bool Status)
        {
            bool retval = false;
            CompanyMaster objCompanyMaster = new CompanyMaster();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objCompanyMaster = context.CompanyMasters.FirstOrDefault(t => t.ID == CompanyId);

                if (objCompanyMaster != null)
                {
                    objCompanyMaster.IsIncludeCommonBOM = Status;
                    context.SaveChanges();
                    retval = true;
                }

            }
            return retval;
        }
        public string UpdateCommonBOM(long CompanyId, bool Status, long UserId)
        {
            string strError = "";
            try
            {
                if (!(CompanyId > 0))
                {
                    return "";
                }



                using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

                    SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
                    if (Status)
                    {
                        strError = Convert.ToString(SqlHelper.ExecuteScalar(EturnsConnection, "USP_BOM_CompanyCheckISInclude", CompanyId, UserId));
                        strError = "ok";
                    }
                    else
                    {
                        strError = Convert.ToString(SqlHelper.ExecuteScalar(EturnsConnection, "USP_BOM_CompanyUnCheckISInclude", CompanyId, UserId));
                    }



                }
                CacheHelper<IEnumerable<ItemMasterDTO>>.InvalidateCache();
                CacheHelper<IEnumerable<BinMasterDTO>>.InvalidateCache();
                CacheHelper<IEnumerable<CategoryMasterDTO>>.InvalidateCache();
                CacheHelper<IEnumerable<GLAccountMasterDTO>>.InvalidateCache();
                CacheHelper<IEnumerable<ManufacturerMasterDTO>>.InvalidateCache();
                CacheHelper<IEnumerable<UnitMasterDTO>>.InvalidateCache();
                CacheHelper<IEnumerable<SupplierMasterDTO>>.InvalidateCache();

                CacheHelper<IEnumerable<ItemManufacturerDetailsDTO>>.InvalidateCache();
                CacheHelper<IEnumerable<ItemSupplierDetailsDTO>>.InvalidateCache();
                CacheHelper<IEnumerable<InventoryClassificationMasterDTO>>.InvalidateCache();
                return strError;
            }
            catch
            {
                return strError;
            }

        }
        public string UpdateData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ColumnName", columnName),
                                                   new SqlParameter("@value", value),
                                                   new SqlParameter("@ID", id) };

                context.Database.ExecuteSqlCommand("exec [UpdateCompanyData] @ColumnName,@value,@ID", params1);
            }
            return value;
        }
        public bool updateLogoName(long Id, string fileName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CompanyMaster objCompanyMaster = context.CompanyMasters.FirstOrDefault(t => t.ID == Id);
                if (objCompanyMaster != null)
                {
                    objCompanyMaster.CompanyLogo = fileName;
                    context.SaveChanges();
                }
            }
            return true;
        }
        public List<CompanyMasterDTO> GetPagedCompanies(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, bool IsArchived, bool IsDeleted, string UserCompanies, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            List<CompanyMasterDTO> lstCompanies = new List<CompanyMasterDTO>();
            TotalCount = 0;
            CompanyMasterDTO objCompanyDto = new CompanyMasterDTO();
            DataSet dsCompanies = new DataSet();
            //string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            string Connectionstring = DbConnectionHelper.GeteTurnsMasterSQLConnectionString(DbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.GeneralReadWrite.ToString("F"));
            if (Connectionstring == "")
            {
                return lstCompanies;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            string CompanyCreaters = null;
            string CompanyUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string EnterpriseIds = null;
            string IsActive = null;

            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsCompanies = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedCompanies", StartRowIndex, MaxRows, SearchTerm, sortColumnName, CompanyCreaters, CompanyUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, UserCompanies, EnterpriseIds, IsActive);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                //SearchTerm = string.Empty;
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

                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    CompanyCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    CompanyUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    // CreatedDateFrom = Convert.ToDateTime(FieldsPara[2].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    // CreatedDateTo = Convert.ToDateTime(FieldsPara[2].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    //UpdatedDateFrom = Convert.ToDateTime(FieldsPara[3].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    // UpdatedDateTo = Convert.ToDateTime(FieldsPara[3].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + "','";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + "','";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + "','";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + "','";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + "','";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[56]))
                {
                    EnterpriseIds = FieldsPara[56].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[86]))
                {
                    IsActive = FieldsPara[86].TrimEnd(',');
                    if (IsActive == "0")
                        IsActive = "false";
                    else if (IsActive == "1")
                        IsActive = "true";
                    else
                        IsActive = null;
                }

                dsCompanies = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedCompanies", StartRowIndex, MaxRows, SearchTerm, sortColumnName, CompanyCreaters, CompanyUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, UserCompanies, EnterpriseIds, IsActive);
            }
            else
            {
                dsCompanies = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedCompanies", StartRowIndex, MaxRows, SearchTerm, sortColumnName, CompanyCreaters, CompanyUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, UserCompanies, EnterpriseIds, IsActive);
            }

            if (dsCompanies != null && dsCompanies.Tables.Count > 0)
            {
                DataTable dtCompanies = dsCompanies.Tables[0];
                if (dtCompanies.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtCompanies.Rows[0]["TotalRecords"]);
                }
                lstCompanies = (from cmny in dtCompanies.AsEnumerable()
                                select new CompanyMasterDTO
                                {
                                    Address = cmny.Field<string>("Address"),
                                    City = cmny.Field<string>("City"),
                                    CompanyLogo = cmny.Field<string>("CompanyLogo"),
                                    ContactEmail = cmny.Field<string>("ContactEmail"),
                                    ContactPhone = cmny.Field<string>("ContactPhone"),
                                    Country = cmny.Field<string>("Country"),
                                    Created = cmny.Field<DateTime>("Created"),
                                    CreatedBy = cmny.Field<long?>("CreatedBy"),
                                    CreatedByName = cmny.Field<string>("CreatedByName"),
                                    EnterPriseId = cmny.Field<long>("EnterpriseID"),
                                    EnterPriseName = cmny.Field<string>("EnterpriseName"),
                                    GUID = cmny.Field<Guid>("GUID"),
                                    ID = cmny.Field<long>("ID"),
                                    IsActive = cmny.Field<bool>("IsActive"),
                                    IsArchived = cmny.Field<bool?>("IsArchived"),
                                    IsDeleted = cmny.Field<bool?>("IsDeleted"),
                                    LastUpdatedBy = cmny.Field<long?>("LastUpdatedBy"),
                                    Name = cmny.Field<string>("Name"),
                                    PostalCode = cmny.Field<string>("PostalCode"),
                                    State = cmny.Field<string>("State"),
                                    UDF1 = cmny.Field<string>("UDF1"),
                                    UDF2 = cmny.Field<string>("UDF2"),
                                    UDF3 = cmny.Field<string>("UDF3"),
                                    UDF4 = cmny.Field<string>("UDF4"),
                                    UDF5 = cmny.Field<string>("UDF5"),
                                    Updated = cmny.Field<DateTime?>("Updated"),
                                    UpdatedByName = cmny.Field<string>("UpdatedByName"),
                                    NoOfRooms = cmny.Field<Int32>("NoOfRooms"),
                                    NoOfActiveRooms = cmny.Field<Int32>("NoOfActiveRooms"),
                                    CompanyNumber = cmny.Field<string>("CompanyNumber")
                                }).ToList();
            }
            return lstCompanies;
        }
        public bool GetIsIncludePermission(long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from x in context.CompanyMasters
                        where x.ID == CompanyId
                        select x.IsIncludeCommonBOM
                        ).FirstOrDefault();
            }
        }
        public List<CompanyMasterDTO> GetAllCompaniesByEnterpriseID(long EnterpriseID)
        {
            CommonDAL objCommonDAL = null;
            EnterpriseDTO objEnterpriseDTO = null;
            List<CompanyMasterDTO> lstAllCompanies = null;
            try
            {
                objCommonDAL = new CommonDAL(this.DataBaseName);
                objEnterpriseDTO = objCommonDAL.GetEnterpriseByID(EnterpriseID);
                string DBConnectionstring = DbConnectionHelper.GeteTurnsEntityFWConnectionString(objEnterpriseDTO.EnterpriseDBName, DbConnectionType.EFReadWrite.ToString("F"));
                lstAllCompanies = new List<CompanyMasterDTO>();
                using (var context = new eTurnsEntities(DBConnectionstring))
                {
                    lstAllCompanies = context.Database.SqlQuery<CompanyMasterDTO>("exec [GetAllCompanies]").ToList();
                }
                return lstAllCompanies;
            }
            catch
            {
                throw;
            }
            finally
            {
                objCommonDAL = null;
                objEnterpriseDTO = null;
                lstAllCompanies = null;
            }
        }
        public CompanyMasterDTO GetAllCompaniesByEnterpriseIDByCompanyName(long EnterpriseID, string CompanyName)
        {
            CommonDAL objCommonDAL = new CommonDAL(base.DataBaseName);
            EnterpriseDTO objEnterpriseDTO = objCommonDAL.GetEnterpriseByID(EnterpriseID);
            string DBConnectionstring = DbConnectionHelper.GeteTurnsEntityFWConnectionString(objEnterpriseDTO.EnterpriseDBName, DbConnectionType.EFReadWrite.ToString("F"));
            CompanyMasterDTO lstAllCompanies = new CompanyMasterDTO();
            var params1 = new SqlParameter[] { new SqlParameter("@CompanyName", CompanyName ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(DBConnectionstring))
            {
                lstAllCompanies = context.Database.SqlQuery<CompanyMasterDTO>("exec [GetCompanyByName] @CompanyName", params1).FirstOrDefault();
            }
            return lstAllCompanies;
        }


        public List<CompanyMasterDTO> GetAllCompaniesByEnterpriseIDByCompanyList(long EnterpriseID, string CompanyNames)
        {
            CommonDAL objCommonDAL = new CommonDAL(base.DataBaseName);
            EnterpriseDTO objEnterpriseDTO = objCommonDAL.GetEnterpriseByID(EnterpriseID);
            string DBConnectionstring = DbConnectionHelper.GeteTurnsEntityFWConnectionString(objEnterpriseDTO.EnterpriseDBName, DbConnectionType.EFReadWrite.ToString("F"));
            List<CompanyMasterDTO> lstAllCompanies = new List<CompanyMasterDTO>();
            var params1 = new SqlParameter[] { new SqlParameter("@CompanyNames", CompanyNames ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(DBConnectionstring))
            {
                lstAllCompanies = context.Database.SqlQuery<CompanyMasterDTO>("exec [GetCompanyByCompanyList] @CompanyNames", params1).ToList();
            }
            return lstAllCompanies;
        }

        public IEnumerable<CompanyMasterDTO> GetAllCompaniesFromETurnsMaster(bool IsArchived, bool IsDeleted, List<CompanyMasterDTO> lstCompanies, Int64 RoleID)
        {
            string UserCompanies = string.Empty;
            if (lstCompanies == null)
            {
                lstCompanies = new List<CompanyMasterDTO>();
            }

            long RoleId = -1;
            long.TryParse(Convert.ToString(RoleID), out RoleId);

            if (RoleId != -1)
                UserCompanies = string.Join(",", lstCompanies.Select(t => t.ID).ToArray());

            if (string.IsNullOrWhiteSpace(UserCompanies) && RoleId != -1)
            {
                UserCompanies = "0,0";
            }
            IEnumerable<CompanyMasterDTO> lstAllCompanies = null;
            var params1 = new SqlParameter[] { new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@UserCompanies", UserCompanies ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseETurnsMasterEntityConnectionString))
            {
                lstAllCompanies = context.Database.SqlQuery<CompanyMasterDTO>("exec dbo.GetAllCompanies @IsDeleted,@IsArchived,@UserCompanies", params1).ToList();
            }
            return lstAllCompanies;
        }

        public bool UnDeleteCompanyWiseRecords(string ID, Int64 userid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (!string.IsNullOrEmpty(ID.Trim()) && ID.Trim() != userid.ToString())
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@UserID", userid),
                                                       new SqlParameter("@ID", ID.Trim()) };

                    context.Database.ExecuteSqlCommand("exec [UnDeleteCompanyMasterByID]  @UserID,@ID", params1);
                }
            }
            return true;
        }

        public CompanyMasterDTO GetALLCompanyByID(long ID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CompanyMasterDTO>("exec [GetALLCompanyByID] @ID", params1).FirstOrDefault();
            }
        }
        public List<CompanyMasterDTO> GetCompanyMasterChangeLog(string IDs)
        {
            IDs = IDs.Replace(",", "");
            var params1 = new SqlParameter[] { new SqlParameter("@IDs", Convert.ToInt32(IDs)), new SqlParameter("@dbName", DataBaseName) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CompanyMasterDTO>("exec [GetCompanyMasterChangeLog] @IDs,@dbName", params1).ToList();
            }
        }

        public bool RemoveCompanyImage(Guid CompanyGUID, string EditedFrom, Int64 UserID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool bResult = false;
                try
                {
                    string strQuery = "EXEC [dbo].[RemoveCompanyImage] '" + Convert.ToString(CompanyGUID) + "','" + EditedFrom + "'," + UserID + "";
                    context.Database.ExecuteSqlCommand(strQuery);
                    bResult = true;
                }
                catch
                {
                    bResult = false;
                }
                return bResult;
            }
        }

        public List<CompanyMasterDTO> GetCompanyListByEnterpriseANDRoom(long EnterpriseID, string RoomName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseID", EnterpriseID), new SqlParameter("@RoomName", RoomName.Trim()) };
            using (var context = new eTurnsEntities(base.DataBaseETurnsMasterEntityConnectionString))
            {
                return context.Database.SqlQuery<CompanyMasterDTO>("exec [GetCompanyListByEnterpriseANDRoom] @EnterpriseID,@RoomName", params1).ToList();
            }
        }
        #endregion

    }

}


