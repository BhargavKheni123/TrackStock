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

namespace eTurns.DAL
{
    public class EnterpriseDAL
    {

        /// <summary>
        /// Get Paged Records from the Technician Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<EnterpriseDTO> GetAllRecords()
        {

            using (var context = new eTurnsEntities())
            {

                return (from x in context.ExecuteStoreQuery<EnterpriseDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName FROM Enterprise A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1")
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
                            UpdatedByName = x.UpdatedByName
                        }).ToList();

            }
        }

        /// <summary>
        /// Get Paged Records from the Technician Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<EnterpriseDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName)
        {
            using (var context = new eTurnsEntities())
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
                    TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(ID) FROM Enterprise WHERE IsDeleted!=1 AND IsArchived != 1").ToList()[0]);

                    return (from x in context.ExecuteStoreQuery<EnterpriseDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName FROM Enterprise A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 ORDER BY " + strSortinitializer)
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
                                UpdatedByName = x.UpdatedByName
                            }).Skip(StartRowIndex).Take(MaxRows).ToList();
                }
                else if (SearchTerm.Contains("STARTWITH#"))
                {
                    string search = "";
                    string[] dd = SearchTerm.Replace("STARTWITH#", "$").Split('$');
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

                    TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(A.ID) FROM Enterprise as A WHERE A.IsDeleted!=1 AND A.IsArchived != 1 " + search + "").ToList()[0]);

                    return (from x in context.ExecuteStoreQuery<EnterpriseDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName FROM Enterprise A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 " + search + " ORDER BY " + strSortinitializer)
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
                                UpdatedByName = x.UpdatedByName
                            }).Skip(StartRowIndex).Take(MaxRows).ToList();
                }
                else
                {
                    TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(ID) FROM Enterprise WHERE IsDeleted!=1 AND IsArchived != 1 AND ((Name like '%" + SearchTerm + "%') OR (ID like '%" + SearchTerm + "%') OR (Address like '%" + SearchTerm + "%') OR (City like '%" + SearchTerm + "%') OR (State like '%" + SearchTerm + "%') OR (PostalCode like '%" + SearchTerm + "%') OR (Country like '%" + SearchTerm + "%'))").ToList()[0]);
                    return (from x in context.ExecuteStoreQuery<EnterpriseDTO>("SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName FROM Enterprise  A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND ((A.Name like '%" + SearchTerm + "%') OR (A.ID like '%" + SearchTerm + "%') OR (A.Address like '%" + SearchTerm + "%') OR (A.City like '%" + SearchTerm + "%') OR (A.State like '%" + SearchTerm + "%') OR (A.PostalCode like '%" + SearchTerm + "%') OR (A.Country like '%" + SearchTerm + "%')) ORDER BY " + strSortinitializer)
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
                                UpdatedByName = x.UpdatedByName
                            }).Skip(StartRowIndex).Take(MaxRows).ToList();
                }
            }
        }

        /// <summary>
        /// Get Particullar Record from the Technician Master by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public EnterpriseDTO GetRecord(Int64 id)
        {
            using (var context = new eTurnsEntities())
            {
                return (from x in context.ExecuteStoreQuery<EnterpriseDTO>(@"SELECT A.*, A.LastUpdatedBy, A.Updated, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName FROM Enterprise A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.ID=" + id.ToString())
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
                            UpdatedByName = x.UpdatedByName
                        }).SingleOrDefault();
            }
        }

        /// <summary>
        /// Insert Record in the DataBase Technician Master
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(EnterpriseDTO objDTO)
        {
            using (var context = new eTurnsEntities())
            {
                //Assign & Mapping of the DTO Class property to Entity Class 
                var obj = new Enterprise
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
                    Updated = DateTime.Now,
                    Created = DateTime.Now,
                    CreatedBy = objDTO.CreatedBy,
                    LastUpdatedBy = objDTO.LastUpdatedBy,
                    IsDeleted = false,
                    IsArchived = false,
                };
                context.AddToEnterprises(obj);
                context.SaveChanges();
                return obj.ID;
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
            using (var context = new eTurnsEntities())
            {
                Enterprise obj = context.Enterprises.Single(t => t.ID == id);
                obj.IsDeleted = true;
                obj.IsArchived = false;
                obj.Updated = DateTime.Now;
                obj.LastUpdatedBy = userid;
                context.Enterprises.Attach(obj);
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                context.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(EnterpriseDTO objDTO)
        {
            using (var context = new eTurnsEntities())
            {
                Enterprise obj = new Enterprise();
                obj.ID = objDTO.ID;
                obj.Name = objDTO.Name;
                obj.Address = objDTO.Address;
                obj.City = objDTO.City;
                obj.State = objDTO.State;
                obj.PostalCode = objDTO.PostalCode;
                obj.Country = objDTO.Country;
                obj.ContactPhone = objDTO.ContactPhone;
                obj.ContactEmail = objDTO.ContactEmail;
                obj.SoftwareBasePrice = objDTO.SoftwareBasePrice;
                obj.MaxBinsPerBasePrice = objDTO.MaxBinsPerBasePrice;
                obj.CostPerBin = objDTO.CostPerBin;
                obj.DiscountPrice1 = objDTO.DiscountPrice1;
                obj.DiscountPrice2 = objDTO.DiscountPrice2;
                obj.DiscountPrice3 = objDTO.DiscountPrice3;
                obj.MaxSubscriptionTier1 = objDTO.MaxSubscriptionTier1;
                obj.MaxSubscriptionTier2 = objDTO.MaxSubscriptionTier2;
                obj.MaxSubscriptionTier3 = objDTO.MaxSubscriptionTier3;
                obj.MaxSubscriptionTier4 = objDTO.MaxSubscriptionTier4;
                obj.MaxSubscriptionTier5 = objDTO.MaxSubscriptionTier5;
                obj.MaxSubscriptionTier6 = objDTO.MaxSubscriptionTier6;
                obj.PriceSubscriptionTier1 = objDTO.PriceSubscriptionTier1;
                obj.PriceSubscriptionTier2 = objDTO.PriceSubscriptionTier2;
                obj.PriceSubscriptionTier3 = objDTO.PriceSubscriptionTier3;
                obj.PriceSubscriptionTier4 = objDTO.PriceSubscriptionTier4;
                obj.PriceSubscriptionTier5 = objDTO.PriceSubscriptionTier5;
                obj.PriceSubscriptionTier6 = objDTO.PriceSubscriptionTier6;
                obj.IncludeLicenseFees = objDTO.IncludeLicenseFees;
                obj.MaxLicenseTier1 = objDTO.MaxLicenseTier1;
                obj.MaxLicenseTier2 = objDTO.MaxLicenseTier2;
                obj.MaxLicenseTier3 = objDTO.MaxLicenseTier3;
                obj.MaxLicenseTier4 = objDTO.MaxLicenseTier4;
                obj.MaxLicenseTier5 = objDTO.MaxLicenseTier5;
                obj.MaxLicenseTier6 = objDTO.MaxLicenseTier6;
                obj.PriceLicenseTier1 = objDTO.PriceLicenseTier1;
                obj.PriceLicenseTier2 = objDTO.PriceLicenseTier2;
                obj.PriceLicenseTier3 = objDTO.PriceLicenseTier3;
                obj.PriceLicenseTier4 = objDTO.PriceLicenseTier4;
                obj.PriceLicenseTier5 = objDTO.PriceLicenseTier5;
                obj.PriceLicenseTier6 = objDTO.PriceLicenseTier6;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.Created = objDTO.Created;
                obj.Updated = DateTime.Now;
                obj.IsDeleted = objDTO.IsDeleted;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsArchived = false;
                obj.GUID = objDTO.GUID;
                context.Enterprises.Attach(obj);
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                context.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteRecords(string IDs, int userid)
        {
            using (eTurnsEntities context = new eTurnsEntities())
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE Enterprise SET Updated = '" + DateTime.Now.ToString()  + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);
                return true;
            }
        }

        /// <summary>
        /// Update Data - Grid Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="rowId"></param>
        /// <param name="columnPosition"></param>
        /// <param name="columnId"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string UpdateData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            using (eTurnsEntities context = new eTurnsEntities())
            {
                string strQuery = "UPDATE Enterprise SET " + columnName + " = '" + value + "', Updated = DATETIME ('" + System.DateTime.Now.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String)))) + "') WHERE ID=" + id;
                context.ExecuteStoreCommand(strQuery);
            }
            return value;
        }

        /// <summary>
        /// Duplicate Checking for the Technician Master
        /// </summary>
        /// <param name="TechnicianName"></param>
        /// <param name="ActionMode"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        //public string DuplicateCheck(string EnterpriseName, string ActionMode, Int64 ID)
        //{

        //    EnterpriseName = EnterpriseName.Replace("'", "''");

        //    using (eTurnsEntities context = new eTurnsEntities())
        //    {
        //        try
        //        {
        //            //var data = context.TechnicianMasters.Count(x => x.Technician == TechnicianName);                   
        //            string WhereCond = "";

        //            if (ActionMode == "add")
        //                //data = context.ChkDuplicate("TechnicianMaster", "ID", " Technician = '" + TechnicianName + "'");
        //                WhereCond = " Name = '" + EnterpriseName + "'";
        //            else
        //                //data = context.ChkDuplicate("TechnicianMaster", "ID", " Technician = '" + TechnicianName + "' and ID = " + ID + "");
        //                WhereCond = " Name = '" + EnterpriseName + "' and ID = " + ID + "";

        //            var data = context.ChkDuplicate("Enterprise", "ID", WhereCond);
        //            string Msg = "";
        //            foreach (var item in data)
        //            {
        //                if (item.Value == 0 && ActionMode == "add")
        //                    Msg = "ok";
        //                else if (item.Value == 0 && ActionMode == "edit")
        //                {
        //                    WhereCond = " Name = '" + EnterpriseName + "'";
        //                    data = context.ChkDuplicate("Enterprise", "ID", WhereCond);
        //                    foreach (var item1 in data)
        //                    {
        //                        if (item1.Value == 0)
        //                            Msg = "ok";
        //                        else
        //                            Msg = "duplicate";
        //                    }
        //                }
        //                else
        //                {
        //                    if (ActionMode == "edit" && item.Value == 1)
        //                        Msg = "ok";
        //                    else
        //                        Msg = "duplicate";
        //                }
        //            }
        //            return Msg;
        //        }
        //        catch (Exception ex)
        //        {
        //            throw;
        //        }
        //    }
        //}
    }
}
