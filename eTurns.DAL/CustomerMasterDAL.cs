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
    public partial class CustomerMasterDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]
        public CustomerMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public CustomerMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]
        public CustomerMasterDTO GetCustomerByID(long CustomerID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CustomerID", CustomerID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CustomerMasterDTO>("exec [GetCustomerByID] @CustomerID", params1).FirstOrDefault();
            }
        }
        public List<CustomerMasterDTO> GetCustomersByName(string CustomerName, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CustomerName", CustomerName ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CustomerMasterDTO>("exec [GetCustomersByName] @CustomerName,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public CustomerMasterDTO GetCustomerByName(string CustomerName, long RoomID, long CompanyID)
        {
            return GetCustomersByName(CustomerName, RoomID, CompanyID).OrderByDescending(t => t.ID).FirstOrDefault();
        }
        public List<CustomerMasterDTO> GetCustomersByNameSearch(string CustomerName, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CustomerName", CustomerName ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CustomerMasterDTO>("exec [GetCustomersByNameSearch] @CustomerName,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<CustomerMasterDTO> GetCustomersByRoomID(long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CustomerMasterDTO>("exec [GetCustomersByRoomID] @RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<CustomerMasterDTO> GetCustomersByIDs(string CustIds, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CustIds", CustIds ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CustomerMasterDTO>("exec [GetCustomersByIDs] @CustIds,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public CustomerMasterDTO GetCustomerByGUID(Guid CustomerGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CustomerGUID", CustomerGUID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CustomerMasterDTO>("exec [GetCustomerByGUID] @CustomerGUID", params1).FirstOrDefault();
            }
        }

        public List<NarrowSearchDTO> GetCustomerListNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetCustomerListNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey", params1).ToList();
            }
        }
        public Int64 Insert(CustomerMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CustomerMaster obj = new CustomerMaster();
                obj.ID = 0;
                obj.GUID = objDTO.GUID;// Guid.NewGuid();

                obj.Customer = objDTO.Customer;
                obj.Account = objDTO.Account;
                obj.Address = objDTO.Address;
                obj.City = objDTO.City;
                obj.State = objDTO.State;
                obj.Country = objDTO.Country;
                obj.ZipCode = objDTO.ZipCode;
                obj.Contact = objDTO.Contact;
                obj.Email = objDTO.Email;
                obj.Phone = objDTO.Phone;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.Updated = objDTO.Updated;
                obj.Created = objDTO.Created;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.Remarks = objDTO.Remarks;
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;
                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                context.CustomerMasters.Add(obj);
                context.SaveChanges();

                objDTO.ID = obj.ID;
                //if (objDTO.ID > 0)
                //{
                //    //Get Cached-Media
                //    IEnumerable<CustomerMasterDTO> ObjCache = CacheHelper<IEnumerable<CustomerMasterDTO>>.GetCacheItem("Cached_CustomerMaster_" + objDTO.CompanyID.ToString());
                //    if (ObjCache != null)
                //    {
                //        List<CustomerMasterDTO> tempC = new List<CustomerMasterDTO>();
                //        tempC.Add(objDTO);

                //        IEnumerable<CustomerMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                //        CacheHelper<IEnumerable<CustomerMasterDTO>>.AppendToCacheItem("Cached_CustomerMaster_" + objDTO.CompanyID.ToString(), NewCache);
                //    }
                //}


                return obj.ID;

            }
        }
        public bool Edit(CustomerMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CustomerMaster obj = new CustomerMaster();
                obj.ID = objDTO.ID;
                obj.Customer = objDTO.Customer;
                obj.Account = objDTO.Account;
                obj.Address = objDTO.Address;
                obj.City = objDTO.City;
                obj.State = objDTO.State;
                obj.Country = objDTO.Country;
                obj.ZipCode = objDTO.ZipCode;
                obj.Contact = objDTO.Contact;
                obj.Email = objDTO.Email;
                obj.Phone = objDTO.Phone;

                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.Updated = objDTO.Updated;
                obj.Created = objDTO.Created;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.GUID = objDTO.GUID;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;

                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.Remarks = objDTO.Remarks;
                obj.AddedFrom = objDTO.AddedFrom;
                if (objDTO.IsOnlyFromItemUI)
                {
                    obj.EditedFrom = objDTO.EditedFrom;
                    obj.ReceivedOn = objDTO.ReceivedOn;
                }
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                context.CustomerMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                ////Get Cached-Media
                //IEnumerable<CustomerMasterDTO> ObjCache = CacheHelper<IEnumerable<CustomerMasterDTO>>.GetCacheItem("Cached_CustomerMaster_" + objDTO.CompanyID.ToString());
                //if (ObjCache != null)
                //{
                //    List<CustomerMasterDTO> objTemp = ObjCache.ToList();
                //    objTemp.RemoveAll(i => i.ID == objDTO.ID);
                //    ObjCache = objTemp.AsEnumerable();

                //    List<CustomerMasterDTO> tempC = new List<CustomerMasterDTO>();
                //    tempC.Add(objDTO);
                //    IEnumerable<CustomerMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                //    CacheHelper<IEnumerable<CustomerMasterDTO>>.AppendToCacheItem("Cached_CustomerMaster_" + objDTO.CompanyID.ToString(), NewCache);
                //}



                return true;
            }

        }

        public List<CustomerMasterDTO> GetPagedCustomerMaster(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64? Room, Int64? CompanyID, Boolean? IsArchived, Boolean? IsDeleted, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            string CreatedByName = "";
            string UpdatedByName = "";
            string CreatedDateFrom = "";
            string CreatedDateTo = "";
            string UpdatedDateFrom = "";
            string UpdatedDateTo = "";
            string UDF1 = "";
            string UDF2 = "";
            string UDF3 = "";
            string UDF4 = "";
            string UDF5 = "";
            if ((SearchTerm ?? string.Empty).Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');

                if (Fields.Length > 2)
                {
                    if (Fields[2] != null)
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
                    CreatedByName = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    UpdatedByName = FieldsPara[1].TrimEnd(',');
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
            }
            else
            {
                SearchTerm = "";
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@StartRowIndex", StartRowIndex),
                    new SqlParameter("@MaxRows", MaxRows),
                    new SqlParameter("@SearchTerm", SearchTerm ?? (object)DBNull.Value),
                    new SqlParameter("@sortColumnName", sortColumnName ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedFrom", CreatedDateFrom),
                    new SqlParameter("@CreatedTo", CreatedDateTo),
                    new SqlParameter("@UpdatedFrom", UpdatedDateFrom),
                    new SqlParameter("@UpdatedTo", UpdatedDateTo),
                    new SqlParameter("@CreatedBy", CreatedByName),
                    new SqlParameter("@LastUpdatedBy", UpdatedByName),
                    new SqlParameter("@Room", Room),
                    new SqlParameter("@IsDeleted", IsDeleted),
                    new SqlParameter("@IsArchived", IsArchived),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@UDF1", UDF1),
                    new SqlParameter("@UDF2", UDF2),
                    new SqlParameter("@UDF3", UDF3),
                    new SqlParameter("@UDF4", UDF4),
                    new SqlParameter("@UDF5", UDF5)
                };

                List<CustomerMasterDTO> lstcats = context.Database.SqlQuery<CustomerMasterDTO>("exec [GetPagedCustomerMaster] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@CreatedFrom,@CreatedTo,@UpdatedFrom,@UpdatedTo,@CreatedBy,@LastUpdatedBy,@Room,@IsDeleted,@IsArchived,@CompanyID,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5", params1).ToList();
                TotalCount = 0;
                if (lstcats != null && lstcats.Count > 0)
                {
                    TotalCount = lstcats.First().TotalRecords ?? 0;
                }

                return lstcats;
            }
        }
        public List<CustomerMasterDTO> GetCustomerMasterChangeLog(string IDs)
        {
            IDs = IDs.Replace(",", "");
            var params1 = new SqlParameter[] { new SqlParameter("@ID", IDs), new SqlParameter("@dbName", DataBaseName) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CustomerMasterDTO>("exec [GetCustomerMasterChangeLog] @ID,@dbName", params1).ToList();
            }
        }
        #endregion
    }
}
