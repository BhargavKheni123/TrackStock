using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.ComponentModel.Design;
using static eTurns.DAL.OrderMasterDAL;
using static eTurns.DTO.RoleMasterDTO_BKP;
namespace eTurns.DAL
{
    public partial class CommonDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        //public CommonDAL(base.DataBaseName)
        //{

        //}

        public CommonDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public CommonDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion
        string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];


        public List<T> GetListOfRecord<T>(string columnList, string tableName, string condition, string sortColumn) where T : class
        {
            var params1 = new SqlParameter[] { new SqlParameter("@columnList", columnList), new SqlParameter("@tableName", tableName), new SqlParameter("@condition", condition), new SqlParameter("@sortColumn", sortColumn) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<T>("exec [GetListOfRecord] @columnList,@tableName,@condition,@sortColumn", params1).ToList();
            }
        }

        public T GetSingleRecord<T>(string columnList, string tableName, string condition, string sortColumn) where T : class
        {
            var params1 = new SqlParameter[] { new SqlParameter("@columnList", columnList), new SqlParameter("@tableName", tableName), new SqlParameter("@condition", condition), new SqlParameter("@sortColumn", sortColumn) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<T>("exec [GetSingleRecord] @columnList,@tableName,@condition,@sortColumn", params1).FirstOrDefault();
            }
        }

        private string GetSessionKey(long EnterPriceID, Int64 CompanyID, Int64 RoomID, Int64 OrderID = 0)
        {
            string strKey = "OrderLineItem_" + EnterPriceID + "_" + CompanyID + "_" + RoomID;
            return strKey;
        }

        public IEnumerable<object> GetPagedRecords(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string TableName, long ID, long? CompanyId, long? EnterpriseID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                TotalCount = 0;

                switch (TableName)
                {
                    case "RoomList":
                        var params1 = new SqlParameter[] { new SqlParameter("@Id", ID), new SqlParameter("@CompanyID", CompanyId), new SqlParameter("@EnterpriseId", EnterpriseID) };
                        var roomHistory = context.Database.SqlQuery<RoomHistoryDTO>("exec GetRoomHistoryData @Id,@CompanyID,@EnterpriseId", params1).ToList();
                        if (roomHistory != null && roomHistory.Any())
                        {
                            TotalCount = roomHistory.Count();
                        }
                        return roomHistory;
                    case "CompanyList":
                        var paramCmp = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@dbName", DataBaseName) };
                        List<CompanyMasterDTO> lstComp = (from x in context.Database.SqlQuery<CompanyMasterDTO>("exec [GetCompanyMasterChangeLog] @ID,@dbName", paramCmp)
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
                                                              CreatedByName = x.CreatedByName,
                                                              UpdatedByName = x.UpdatedByName,
                                                              IsArchived = x.IsArchived ?? false,
                                                              CreatedDate = x.CreatedDate,
                                                              UpdatedDate = x.UpdatedDate,
                                                              IsActive = x.IsActive,
                                                              IsStatusChanged = x.IsStatusChanged,
                                                              Action = x.Action,
                                                              HistoryID = x.HistoryID,
                                                              TotalRecords = x.TotalRecords,
                                                              CompanyNumber = x.CompanyNumber
                                                          }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstComp != null && lstComp.Count > 0)
                        {
                            TotalCount = lstComp.FirstOrDefault().TotalRecords ?? 0;
                        }

                        return lstComp;
                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM CompanyMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //return (from x in context.Database.SqlQuery<CompanyMasterDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName   ,'' AS CreatedDate,'' AS UpdatedDate                          
                    //        FROM CompanyMaster_History as A 
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID                                                                
                    //        WHERE A.ID = " + ID + " order by A.HistoryID desc")
                    //        select new CompanyMasterDTO
                    //        {
                    //            ID = x.ID,
                    //            Name = x.Name,
                    //            Address = x.Address,
                    //            City = x.City,
                    //            State = x.State,
                    //            PostalCode = x.PostalCode,
                    //            Country = x.Country,
                    //            ContactPhone = x.ContactPhone,
                    //            ContactEmail = x.ContactEmail,
                    //            UDF1 = x.UDF1,
                    //            UDF2 = x.UDF2,
                    //            UDF3 = x.UDF3,
                    //            UDF4 = x.UDF4,
                    //            UDF5 = x.UDF5,
                    //            Created = x.Created,
                    //            Updated = x.Updated,
                    //            CreatedBy = x.CreatedBy,
                    //            LastUpdatedBy = x.LastUpdatedBy,
                    //            IsDeleted = x.IsDeleted ?? false,
                    //            GUID = x.GUID,
                    //            CreatedByName = x.CreatedByName,
                    //            UpdatedByName = x.UpdatedByName,
                    //            IsArchived = x.IsArchived ?? false,
                    //            CreatedDate = x.CreatedDate,
                    //            UpdatedDate = x.UpdatedDate,
                    //            IsActive = x.IsActive,
                    //            IsStatusChanged = x.IsStatusChanged,
                    //            Action = x.Action,
                    //            HistoryID = x.HistoryID,

                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();


                    case "TechnicianList":
                        var paramTechnician = new SqlParameter[] { new SqlParameter("@ID", ID) };
                        List<TechnicianMasterDTO> lstTechnician = (from u in context.Database.SqlQuery<TechnicianMasterDTO>("exec [GetTechnicianMasterHistory] @ID", paramTechnician)
                                                                   select new TechnicianMasterDTO
                                                                   {
                                                                       ID = u.ID,
                                                                       HistoryID = u.HistoryID,
                                                                       Technician = u.Technician,
                                                                       Created = u.Created,
                                                                       Updated = u.Updated,
                                                                       //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                                                                       //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                                                                       //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                                                                       CreatedByName = u.CreatedByName,
                                                                       UpdatedByName = u.UpdatedByName,
                                                                       RoomName = u.RoomName,
                                                                       Action = u.Action,
                                                                       UDF1 = u.UDF1,
                                                                       UDF2 = u.UDF2,
                                                                       UDF3 = u.UDF3,
                                                                       UDF4 = u.UDF4,
                                                                       UDF5 = u.UDF5,
                                                                       CreatedDate = u.CreatedDate,
                                                                       UpdatedDate = u.UpdatedDate,
                                                                       TotalRecords = u.TotalRecords,
                                                                   }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstTechnician != null && lstTechnician.Count > 0)
                        {
                            TotalCount = lstTechnician.FirstOrDefault().TotalRecords;
                        }

                        return lstTechnician;

                    #region Old Code
                    //    TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM TechnicianMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //    return (from u in context.Database.SqlQuery<TechnicianMasterDTO>(@"
                    //            SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName  ,'' AS CreatedDate,'' AS UpdatedDate                              
                    //            FROM TechnicianMaster_History as A 
                    //            LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //            LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //            LEFT OUTER JOIN Room D on A.Room = D.ID                                 
                    //            WHERE A.ID = " + ID + " order by A.HistoryID desc")
                    //            select new TechnicianMasterDTO
                    //            {
                    //                ID = u.ID,
                    //                HistoryID = u.HistoryID,
                    //                Technician = u.Technician,
                    //                Created = u.Created,
                    //                Updated = u.Updated,
                    //                //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                    //                //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                    //                //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                    //                CreatedByName = u.CreatedByName,
                    //                UpdatedByName = u.UpdatedByName,
                    //                RoomName = u.RoomName,
                    //                Action = u.Action,
                    //                UDF1 = u.UDF1,
                    //                UDF2 = u.UDF2,
                    //                UDF3 = u.UDF3,
                    //                UDF4 = u.UDF4,
                    //                UDF5 = u.UDF5,
                    //                CreatedDate = u.CreatedDate,
                    //                UpdatedDate = u.UpdatedDate,
                    //            }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList(); 
                    #endregion


                    case "BinList":
                        var paramBin = new SqlParameter[] { new SqlParameter("@ID", ID) };
                        List<BinMasterDTO> lstBin = (from u in context.Database.SqlQuery<BinMasterDTO>("exec [GetBinMasterHistory] @ID", paramBin)
                                                     select new BinMasterDTO
                                                     {
                                                         ID = u.ID,
                                                         HistoryID = u.HistoryID,
                                                         Action = u.Action,
                                                         BinNumber = u.BinNumber,
                                                         Created = u.Created,
                                                         LastUpdated = u.LastUpdated,
                                                         //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy ?? 0)).UserName.ToString(),
                                                         //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy ?? 0)).UserName.ToString(),
                                                         //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID ?? 0), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                                                         CreatedByName = u.CreatedByName,
                                                         UpdatedByName = u.UpdatedByName,
                                                         RoomName = u.RoomName,
                                                         CreatedBy = u.CreatedBy,
                                                         LastUpdatedBy = u.LastUpdatedBy,
                                                         CompanyID = u.CompanyID,
                                                         IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                         IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                         UDF1 = u.UDF1,
                                                         UDF2 = u.UDF2,
                                                         UDF3 = u.UDF3,
                                                         UDF4 = u.UDF4,
                                                         UDF5 = u.UDF5,
                                                         CreatedDate = u.CreatedDate,
                                                         UpdatedDate = u.UpdatedDate,
                                                         AddedFrom = u.AddedFrom,
                                                         EditedFrom = u.EditedFrom,
                                                         ReceivedOn = u.ReceivedOn,
                                                         ReceivedOnWeb = u.ReceivedOnWeb,
                                                         IsStagingHeader = u.IsStagingHeader,//u.IsStagingHeader,
                                                         IsStagingLocation = u.IsStagingLocation,//u.IsStagingLocation.ToString() == "1" ? true : false,
                                                         IsDefault = u.IsDefault,
                                                         MaterialStagingGUID = u.MaterialStagingGUID,
                                                         Room = u.Room,
                                                         GUID = u.GUID,
                                                         ItemGUID = u.ItemGUID,
                                                         MinimumQuantity = u.MinimumQuantity,
                                                         MaximumQuantity = u.MaximumQuantity,
                                                         CriticalQuantity = u.CriticalQuantity,
                                                         TotalRecords = u.TotalRecords,
                                                     }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstBin != null && lstBin.Count > 0)
                        {
                            TotalCount = lstBin.FirstOrDefault().TotalRecords ?? 0;
                        }
                        return lstBin;

                    #region Old Code
                    //    TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM BinMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //    return (from u in context.Database.SqlQuery<BinMasterDTO>(@"
                    //            SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,'' AS CreatedDate,'' AS UpdatedDate                               
                    //            FROM BinMaster_History as A 
                    //            LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //            LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //            LEFT OUTER JOIN Room D on A.Room = D.ID                                 
                    //            WHERE A.ID = " + ID + " order by A.HistoryID desc")
                    //            select new BinMasterDTO
                    //            {
                    //                ID = u.ID,
                    //                HistoryID = u.HistoryID,
                    //                Action = u.Action,
                    //                BinNumber = u.BinNumber,
                    //                Created = u.Created,
                    //                LastUpdated = u.LastUpdated,
                    //                //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy ?? 0)).UserName.ToString(),
                    //                //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy ?? 0)).UserName.ToString(),
                    //                //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID ?? 0), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                    //                CreatedByName = u.CreatedByName,
                    //                UpdatedByName = u.UpdatedByName,
                    //                RoomName = u.RoomName,
                    //                CreatedBy = u.CreatedBy,
                    //                LastUpdatedBy = u.LastUpdatedBy,
                    //                CompanyID = u.CompanyID,
                    //                IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                    //                IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                    //                UDF1 = u.UDF1,
                    //                UDF2 = u.UDF2,
                    //                UDF3 = u.UDF3,
                    //                UDF4 = u.UDF4,
                    //                UDF5 = u.UDF5,
                    //                CreatedDate = u.CreatedDate,
                    //                UpdatedDate = u.UpdatedDate,
                    //                AddedFrom = u.AddedFrom,
                    //                EditedFrom = u.EditedFrom,
                    //                ReceivedOn = u.ReceivedOn,
                    //                ReceivedOnWeb = u.ReceivedOnWeb,
                    //                IsStagingHeader = u.IsStagingHeader,//u.IsStagingHeader,
                    //                IsStagingLocation = u.IsStagingLocation,//u.IsStagingLocation.ToString() == "1" ? true : false,
                    //                IsDefault = u.IsDefault,
                    //                MaterialStagingGUID = u.MaterialStagingGUID,
                    //                Room = u.Room,
                    //                GUID = u.GUID,
                    //                ItemGUID = u.ItemGUID,
                    //                MinimumQuantity = u.MinimumQuantity,
                    //                MaximumQuantity = u.MaximumQuantity,
                    //                CriticalQuantity = u.CriticalQuantity,


                    //            }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList(); 
                    #endregion
                    case "CategoryList":
                        var paramC = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@dbName", DataBaseName) };
                        List<CategoryMasterDTO> lstCat = (from u in context.Database.SqlQuery<CategoryMasterDTO>("exec [GetCategoryMasterChangeLog] @ID,@dbName", paramC)
                                                          select new CategoryMasterDTO
                                                          {
                                                              ID = u.ID,
                                                              HistoryID = u.HistoryID,
                                                              Action = u.Action,
                                                              Category = u.Category,
                                                              Created = u.Created,
                                                              Updated = u.Updated,
                                                              //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                                                              //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                                                              //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                                                              CreatedByName = u.CreatedByName,
                                                              UpdatedByName = u.UpdatedByName,
                                                              RoomName = u.RoomName,
                                                              CreatedBy = u.CreatedBy,
                                                              LastUpdatedBy = u.LastUpdatedBy,
                                                              CompanyID = u.CompanyID,
                                                              IsArchived = u.IsArchived,
                                                              IsDeleted = u.IsDeleted,
                                                              GUID = u.GUID,
                                                              UDF1 = u.UDF1,
                                                              UDF2 = u.UDF2,
                                                              UDF3 = u.UDF3,
                                                              UDF4 = u.UDF4,
                                                              UDF5 = u.UDF5,
                                                              CreatedDate = u.CreatedDate,
                                                              UpdatedDate = u.UpdatedDate,
                                                              AddedFrom = u.AddedFrom,
                                                              EditedFrom = u.EditedFrom,
                                                              ReceivedOn = u.ReceivedOn,
                                                              ReceivedOnWeb = u.ReceivedOnWeb,
                                                              Count = u.Count
                                                          }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstCat != null && lstCat.Count > 0)
                        {
                            TotalCount = lstCat.FirstOrDefault().Count ?? 0;
                        }

                        return lstCat;
                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM CategoryMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<CategoryMasterDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,'' AS CreatedDate,'' AS UpdatedDate                                
                    //        FROM CategoryMaster_History as A 
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.Room = D.ID                                 
                    //        WHERE A.ID = " + ID + " order by A.HistoryID desc")
                    //        select new CategoryMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            Category = u.Category,
                    //            Created = u.Created,
                    //            Updated = u.Updated,
                    //            //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                    //            //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                    //            //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            CompanyID = u.CompanyID,
                    //            IsArchived = u.IsArchived,
                    //            IsDeleted = u.IsDeleted,
                    //            GUID = u.GUID,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
                    case "CustomerList":
                        var paramCM = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@dbName", DataBaseName) };
                        List<CustomerMasterDTO> lstCust = (from u in context.Database.SqlQuery<CustomerMasterDTO>("exec [GetCustomerMasterChangeLog] @ID,@dbName", paramCM)
                                                           select new CustomerMasterDTO
                                                           {
                                                               ID = u.ID,
                                                               HistoryID = u.HistoryID,
                                                               Action = u.Action,
                                                               Customer = u.Customer,
                                                               Account = u.Account,
                                                               Address = u.Address,
                                                               City = u.City,
                                                               State = u.State,
                                                               Country = u.Country,
                                                               ZipCode = u.ZipCode,
                                                               Contact = u.Contact,
                                                               Email = u.Email,
                                                               Phone = u.Phone,
                                                               Created = u.Created,
                                                               CreatedBy = u.CreatedBy,
                                                               LastUpdatedBy = u.LastUpdatedBy,
                                                               Room = u.Room,
                                                               Updated = u.Updated,
                                                               //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                                                               //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                                                               //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                                                               CreatedByName = u.CreatedByName,
                                                               UpdatedByName = u.UpdatedByName,
                                                               RoomName = u.RoomName,
                                                               CompanyID = u.CompanyID,
                                                               IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                               IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                               //GUID = u.GUID,
                                                               UDF1 = u.UDF1,
                                                               UDF2 = u.UDF2,
                                                               UDF3 = u.UDF3,
                                                               UDF4 = u.UDF4,
                                                               UDF5 = u.UDF5,
                                                               CreatedDate = u.CreatedDate,
                                                               UpdatedDate = u.UpdatedDate,
                                                               AddedFrom = u.AddedFrom,
                                                               EditedFrom = u.EditedFrom,
                                                               ReceivedOn = u.ReceivedOn,
                                                               ReceivedOnWeb = u.ReceivedOnWeb,
                                                               TotalRecords = u.TotalRecords
                                                           }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstCust != null && lstCust.Count > 0)
                        {
                            TotalCount = lstCust.FirstOrDefault().TotalRecords ?? 0;
                        }

                        return lstCust;
                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM CustomerMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<CustomerMasterDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName   ,'' AS CreatedDate,'' AS UpdatedDate                             
                    //        FROM CustomerMaster_History as A 
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.Room = D.ID                                 
                    //        WHERE A.ID = " + ID + " order by A.HistoryID desc")
                    //        select new CustomerMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            Customer = u.Customer,
                    //            Account = u.Account,
                    //            Address = u.Address,
                    //            City = u.City,
                    //            State = u.State,
                    //            Country = u.Country,
                    //            ZipCode = u.ZipCode,
                    //            Contact = u.Contact,
                    //            Email = u.Email,
                    //            Phone = u.Phone,
                    //            Created = u.Created,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            Room = u.Room,
                    //            Updated = u.Updated,
                    //            //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                    //            //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                    //            //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            CompanyID = u.CompanyID,
                    //            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                    //            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                    //            //GUID = u.GUID,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                    case "FTPList":
                        var paramFTP = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@dbName", DataBaseName) };
                        List<FTPMasterDTO> lstFTP = (from u in context.Database.SqlQuery<FTPMasterDTO>("exec [GetFTPMasterChangeLog] @ID,@dbName", paramFTP)
                                                     select new FTPMasterDTO
                                                     {
                                                         ID = u.ID,
                                                         HistoryID = u.HistoryID,
                                                         Action = u.Action,
                                                         SFtpName = u.SFtpName,
                                                         ServerAddress = u.ServerAddress,
                                                         UserName = u.UserName,
                                                         Password = u.Password,
                                                         Port = u.Port,
                                                         Created = u.Created,
                                                         CreatedBy = u.CreatedBy,
                                                         //LastUpdatedBy = u.LastUpdatedBy,
                                                         UpdatedBy = u.UpdatedBy,
                                                         RoomId = u.RoomId,
                                                         LastUpdated = u.LastUpdated,
                                                         CreatedByName = u.CreatedByName,
                                                         UpdatedByName = u.UpdatedByName,
                                                         RoomName = u.RoomName,
                                                         CompanyId = u.CompanyId,
                                                         IsDeleted = u.IsDeleted,
                                                         IsArchived = u.IsArchived,
                                                         //GUID = u.GUID,
                                                         CreatedDate = u.CreatedDate,
                                                         UpdatedDate = u.UpdatedDate,
                                                         AddedFrom = u.AddedFrom,
                                                         EditedFrom = u.EditedFrom,
                                                         TotalRecords = u.TotalRecords
                                                     }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstFTP != null && lstFTP.Count > 0)
                        {
                            TotalCount = lstFTP.FirstOrDefault().TotalRecords ?? 0;
                        }

                        return lstFTP;
                    // FTP History End
                    case "EnterpriseList":
                        var paramEnterprise = new SqlParameter[] { new SqlParameter("@ID", ID) };
                        List<EnterpriseDTO> lstEnterprise = (from u in context.Database.SqlQuery<EnterpriseDTO>("exec [GetEnterpriseHistory] @ID", paramEnterprise)
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
                                                                 UDF1 = u.UDF1,
                                                                 UDF2 = u.UDF2,
                                                                 UDF3 = u.UDF3,
                                                                 UDF4 = u.UDF4,
                                                                 UDF5 = u.UDF5,
                                                                 GUID = u.GUID,
                                                                 //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                                                                 //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                                                                 CreatedByName = u.CreatedByName,
                                                                 UpdatedByName = u.UpdatedByName,
                                                                 CreatedDate = u.CreatedDate,
                                                                 UpdatedDate = u.UpdatedDate,
                                                                 TotalRecords = u.TotalRecords,
                                                             }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstEnterprise != null && lstEnterprise.Count > 0)
                        {
                            TotalCount = lstEnterprise.FirstOrDefault().TotalRecords ?? 0;
                        }

                        return lstEnterprise;
                    #region Old EnterpriseList
                    //case "EnterpriseList":
                    //    TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM Enterprise_History WHERE ID = " + ID + "").ToList()[0]);
                    //    return (from u in context.Database.SqlQuery<EnterpriseDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName,'' AS CreatedDate,'' AS UpdatedDate
                    //            FROM Enterprise_History as A 
                    //            LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //            LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID                                        
                    //            WHERE A.ID = " + ID + " order by A.HistoryID desc")
                    //            select new EnterpriseDTO
                    //            {
                    //                ID = u.ID,
                    //                HistoryID = u.HistoryID,
                    //                Action = u.Action,
                    //                Name = u.Name,
                    //                Address = u.Address,
                    //                City = u.City,
                    //                State = u.State,
                    //                PostalCode = u.PostalCode,
                    //                Country = u.Country,
                    //                ContactPhone = u.ContactPhone,
                    //                ContactEmail = u.ContactEmail,
                    //                SoftwareBasePrice = u.SoftwareBasePrice,
                    //                MaxBinsPerBasePrice = u.MaxBinsPerBasePrice,
                    //                CostPerBin = u.CostPerBin,
                    //                DiscountPrice1 = u.DiscountPrice1,
                    //                DiscountPrice2 = u.DiscountPrice2,
                    //                DiscountPrice3 = u.DiscountPrice3,
                    //                MaxSubscriptionTier1 = u.MaxSubscriptionTier1,
                    //                MaxSubscriptionTier2 = u.MaxSubscriptionTier2,
                    //                MaxSubscriptionTier3 = u.MaxSubscriptionTier3,
                    //                MaxSubscriptionTier4 = u.MaxSubscriptionTier4,
                    //                MaxSubscriptionTier5 = u.MaxSubscriptionTier5,
                    //                MaxSubscriptionTier6 = u.MaxSubscriptionTier6,
                    //                PriceSubscriptionTier1 = u.PriceSubscriptionTier1,
                    //                PriceSubscriptionTier2 = u.PriceSubscriptionTier2,
                    //                PriceSubscriptionTier3 = u.PriceSubscriptionTier3,
                    //                PriceSubscriptionTier4 = u.PriceSubscriptionTier4,
                    //                PriceSubscriptionTier5 = u.PriceSubscriptionTier5,
                    //                PriceSubscriptionTier6 = u.PriceSubscriptionTier6,
                    //                IncludeLicenseFees = u.IncludeLicenseFees,
                    //                MaxLicenseTier1 = u.MaxLicenseTier1,
                    //                MaxLicenseTier2 = u.MaxLicenseTier2,
                    //                MaxLicenseTier3 = u.MaxLicenseTier3,
                    //                MaxLicenseTier4 = u.MaxLicenseTier4,
                    //                MaxLicenseTier5 = u.MaxLicenseTier5,
                    //                MaxLicenseTier6 = u.MaxLicenseTier6,
                    //                PriceLicenseTier1 = u.PriceLicenseTier1,
                    //                PriceLicenseTier2 = u.PriceLicenseTier2,
                    //                PriceLicenseTier3 = u.PriceLicenseTier3,
                    //                PriceLicenseTier4 = u.PriceLicenseTier4,
                    //                PriceLicenseTier5 = u.PriceLicenseTier5,
                    //                PriceLicenseTier6 = u.PriceLicenseTier6,
                    //                UDF1 = u.UDF1,
                    //                UDF2 = u.UDF2,
                    //                UDF3 = u.UDF3,
                    //                UDF4 = u.UDF4,
                    //                UDF5 = u.UDF5,
                    //                GUID = u.GUID,
                    //                //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                    //                //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                    //                CreatedByName = u.CreatedByName,
                    //                UpdatedByName = u.UpdatedByName,
                    //                CreatedDate = u.CreatedDate,
                    //                UpdatedDate = u.UpdatedDate,
                    //            }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList(); 
                    #endregion
                    case "FreightTypeList":
                        var paramFreight = new SqlParameter[] { new SqlParameter("@ID", ID) ,
                                                                new SqlParameter("@StartRowIndex", StartRowIndex) ,
                                                                new SqlParameter("@MaxRows", MaxRows)
                                                               };
                        var lstFreight = context.Database.SqlQuery<FreightTypeMasterDTO>("exec [GetFreightTypeListHistoryByIdFull] @ID,@StartRowIndex,@MaxRows", paramFreight).ToList();
                        TotalCount = 0;

                        if (lstFreight != null && lstFreight.Any() && lstFreight.Count > 0)
                        {
                            TotalCount = lstFreight.FirstOrDefault().TotalRecords ?? 0;
                        }
                        return lstFreight;
                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM FreightTypeMaster_History WHERE ID = " + ID + "").ToList()[0]);

                    //return (from u in context.Database.SqlQuery<FreightTypeMasterDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName ,'' AS CreatedDate,'' AS UpdatedDate                               
                    //        FROM FreightTypeMaster_History as A 
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.Room = D.ID                                 
                    //        WHERE A.ID = " + ID + " order by A.HistoryID desc")
                    //        select new FreightTypeMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            FreightType = u.FreightType,
                    //            Created = u.Created,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            Room = u.Room,
                    //            LastUpdated = u.LastUpdated,
                    //            //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                    //            //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                    //            //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            //GUID = u.GUID,
                    //            CompanyID = u.CompanyID,
                    //            IsArchived = u.IsArchived,
                    //            IsDeleted = u.IsDeleted,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                    case "GLAccountList":
                        var paramGL = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@dbName", DataBaseName) };
                        List<GLAccountMasterDTO> lstGL = (from u in context.Database.SqlQuery<GLAccountMasterDTO>("exec [GetGLAccountMasterChangeLog] @ID,@dbName", paramGL)
                                                          select new GLAccountMasterDTO
                                                          {
                                                              ID = u.ID,
                                                              HistoryID = u.HistoryID,
                                                              Action = u.Action,
                                                              GLAccount = u.GLAccount,
                                                              Description = u.Description,
                                                              Created = u.Created,
                                                              Updated = u.Updated,
                                                              CreatedBy = u.CreatedBy,
                                                              LastUpdatedBy = u.LastUpdatedBy,
                                                              //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                                                              //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                                                              //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                                                              CreatedByName = u.CreatedByName,
                                                              UpdatedByName = u.UpdatedByName,
                                                              RoomName = u.RoomName,
                                                              GUID = u.GUID,
                                                              CompanyID = u.CompanyID,
                                                              IsArchived = u.IsArchived,
                                                              IsDeleted = u.IsDeleted,
                                                              UDF1 = u.UDF1,
                                                              UDF2 = u.UDF2,
                                                              UDF3 = u.UDF3,
                                                              UDF4 = u.UDF4,
                                                              UDF5 = u.UDF5,
                                                              CreatedDate = u.CreatedDate,
                                                              UpdatedDate = u.UpdatedDate,
                                                              TotalRecords = u.TotalRecords
                                                          }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstGL != null && lstGL.Count > 0)
                        {
                            TotalCount = lstGL.FirstOrDefault().TotalRecords ?? 0;
                        }

                        return lstGL;
                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM GLAccountMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<GLAccountMasterDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,'' AS CreatedDate,'' AS UpdatedDate                                
                    //        FROM GLAccountMaster_History as A 
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.Room = D.ID                                 
                    //        WHERE A.ID = " + ID + " order by A.HistoryID desc")
                    //        select new GLAccountMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            GLAccount = u.GLAccount,
                    //            Description = u.Description,
                    //            Created = u.Created,
                    //            Updated = u.Updated,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                    //            //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                    //            //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            GUID = u.GUID,
                    //            CompanyID = u.CompanyID,
                    //            IsArchived = u.IsArchived,
                    //            IsDeleted = u.IsDeleted,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
                    case "GXPRConsigmentJobList":
                        TotalCount = 0;
                        return new List<GXPRConsigmentJobMasterDTO>();
                    // Below code is commented bcause it's not being called.

                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM GXPRConsigmentJobMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<GXPRConsigmentJobMasterDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName    ,'' AS CreatedDate,'' AS UpdatedDate                            
                    //        FROM  GXPRConsigmentJobMaster_History as A 
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.Room = D.ID                                 
                    //        WHERE A.ID = " + ID + " order by A.HistoryID desc")
                    //        select new GXPRConsigmentJobMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            GXPRConsigmentJob = u.GXPRConsigmentJob,
                    //            Created = u.Created,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            Room = u.Room,
                    //            Updated = u.Updated,
                    //            //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                    //            //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                    //            //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            CompanyID = u.CompanyID,
                    //            IsArchived = u.IsArchived,
                    //            IsDeleted = u.IsDeleted,
                    //            GUID = u.GUID,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
                    case "JobTypeList":
                        TotalCount = 0;
                        return new List<JobTypeMasterDTO>();
                    // Below code is commented bcause jobtype page is not being used.
                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM JobTypeMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<JobTypeMasterDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS LastUpdatedByName, D.RoomName ,'' AS CreatedDate,'' AS UpdatedDate                               
                    //        FROM  JobTypeMaster_History as A 
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.Room = D.ID                                 
                    //        WHERE A.ID = " + ID + " order by A.HistoryID desc")
                    //        select new JobTypeMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            JobType = u.JobType,
                    //            Created = u.Created,
                    //            CreatedBy = u.CreatedBy,
                    //            //GUID = u.GUID,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            Room = u.Room,
                    //            LastUpdated = u.LastUpdated,
                    //            //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                    //            //LastUpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                    //            //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                    //            CreatedByName = u.CreatedByName,
                    //            LastUpdatedByName = u.LastUpdatedByName,
                    //            RoomName = u.RoomName,
                    //            CompanyID = u.CompanyID,
                    //            IsArchived = u.IsArchived,
                    //            IsDeleted = u.IsDeleted,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
                    case "LocationList":
                        // GetLocationMasterChangeLog
                        var paramLM = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@dbName", DataBaseName) };
                        List<LocationMasterDTO> lstLM = (from u in context.Database.SqlQuery<LocationMasterDTO>("exec [GetLocationMasterChangeLog] @ID,@dbName", paramLM)
                                                         select new LocationMasterDTO
                                                         {
                                                             ID = u.ID,
                                                             HistoryID = u.HistoryID,
                                                             Action = u.Action,
                                                             Location = u.Location,
                                                             Created = u.Created,
                                                             LastUpdated = u.LastUpdated,
                                                             CreatedBy = u.CreatedBy,
                                                             LastUpdatedBy = u.LastUpdatedBy,
                                                             //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                                                             //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                                                             //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                                                             CreatedByName = u.CreatedByName,
                                                             UpdatedByName = u.UpdatedByName,
                                                             RoomName = u.RoomName,
                                                             GUID = u.GUID,
                                                             CompanyID = u.CompanyID,
                                                             IsArchived = u.IsArchived,
                                                             IsDeleted = u.IsDeleted,
                                                             UDF1 = u.UDF1,
                                                             UDF2 = u.UDF2,
                                                             UDF3 = u.UDF3,
                                                             UDF4 = u.UDF4,
                                                             UDF5 = u.UDF5,
                                                             CreatedDate = u.CreatedDate,
                                                             UpdatedDate = u.UpdatedDate,
                                                             AddedFrom = u.AddedFrom,
                                                             EditedFrom = u.EditedFrom,
                                                             ReceivedOn = u.ReceivedOn,
                                                             ReceivedOnWeb = u.ReceivedOnWeb,
                                                             TotalRecords = u.TotalRecords
                                                         }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstLM != null && lstLM.Count > 0)
                        {
                            TotalCount = lstLM.FirstOrDefault().TotalRecords ?? 0;
                        }

                        return lstLM;
                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM LocationMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<LocationMasterDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName ,'' AS CreatedDate,'' AS UpdatedDate                               
                    //        FROM  LocationMaster_History as A 
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.Room = D.ID                                 
                    //        WHERE A.ID = " + ID + " order by A.HistoryID desc")
                    //        select new LocationMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            Location = u.Location,
                    //            Created = u.Created,
                    //            LastUpdated = u.LastUpdated,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                    //            //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                    //            //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            GUID = u.GUID,
                    //            CompanyID = u.CompanyID,
                    //            IsArchived = u.IsArchived,
                    //            IsDeleted = u.IsDeleted,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
                    case "ManufacturerList":
                        var paramMM = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@dbName", DataBaseName) };
                        List<ManufacturerMasterDTO> lstMM = (from u in context.Database.SqlQuery<ManufacturerMasterDTO>("exec [GetManufacturerMasterChangeLog] @ID,@dbName", paramMM)
                                                             select new ManufacturerMasterDTO
                                                             {
                                                                 ID = u.ID,
                                                                 HistoryID = u.HistoryID,
                                                                 Action = u.Action,
                                                                 Manufacturer = u.Manufacturer,
                                                                 Created = u.Created,
                                                                 Updated = u.Updated,
                                                                 CreatedBy = u.CreatedBy,
                                                                 LastUpdatedBy = u.LastUpdatedBy,
                                                                 //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                                                                 //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                                                                 //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                                                                 CreatedByName = u.CreatedByName,
                                                                 UpdatedByName = u.UpdatedByName,
                                                                 RoomName = u.RoomName,
                                                                 GUID = u.GUID,
                                                                 CompanyID = u.CompanyID,
                                                                 IsArchived = u.IsArchived,
                                                                 IsDeleted = u.IsDeleted,
                                                                 UDF1 = u.UDF1,
                                                                 UDF2 = u.UDF2,
                                                                 UDF3 = u.UDF3,
                                                                 UDF4 = u.UDF4,
                                                                 UDF5 = u.UDF5,
                                                                 CreatedDate = u.CreatedDate,
                                                                 UpdatedDate = u.UpdatedDate,
                                                                 AddedFrom = u.AddedFrom,
                                                                 EditedFrom = u.EditedFrom,
                                                                 ReceivedOn = u.ReceivedOn,
                                                                 ReceivedOnWeb = u.ReceivedOnWeb,
                                                                 TotalRecords = u.TotalRecords
                                                             }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstMM != null && lstMM.Count > 0)
                        {
                            TotalCount = lstMM.FirstOrDefault().TotalRecords ?? 0;
                        }

                        return lstMM;
                    case "WrittenOfCategoryList":
                        var paramWOC = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@dbName", DataBaseName) };
                        List<WrittenOfCategoryDTO> lstchangeLog = (from u in context.Database.SqlQuery<WrittenOfCategoryDTO>("exec [GetWrittenOffCategoryChangeLog] @ID,@dbName", paramWOC)
                                                                   select new WrittenOfCategoryDTO
                                                                   {
                                                                       ID = u.ID,
                                                                       HistoryID = u.HistoryID,
                                                                       Action = u.Action,
                                                                       WrittenOffCategory = u.WrittenOffCategory,
                                                                       Created = u.Created,
                                                                       Updated = u.Updated,
                                                                       CreatedBy = u.CreatedBy,
                                                                       LastUpdatedBy = u.LastUpdatedBy,
                                                                       CreatedByName = u.CreatedByName,
                                                                       UpdatedByName = u.UpdatedByName,
                                                                       GUID = u.GUID,
                                                                       CompanyID = u.CompanyID,
                                                                       IsArchived = u.IsArchived,
                                                                       IsDeleted = u.IsDeleted,
                                                                       CreatedDate = u.CreatedDate,
                                                                       UpdatedDate = u.UpdatedDate,
                                                                       AddedFrom = u.AddedFrom,
                                                                       EditedFrom = u.EditedFrom,
                                                                       ReceivedOn = u.ReceivedOn,
                                                                       ReceivedOnWeb = u.ReceivedOnWeb,
                                                                       TotalRecords = u.TotalRecords
                                                                   }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstchangeLog != null && lstchangeLog.Count > 0)
                        {
                            TotalCount = lstchangeLog.FirstOrDefault().TotalRecords ?? 0;
                        }

                        return lstchangeLog;
                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM ManufacturerMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<ManufacturerMasterDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,'' AS CreatedDate,'' AS UpdatedDate                                
                    //        FROM  ManufacturerMaster_History as A 
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.Room = D.ID                                 
                    //        WHERE A.ID = " + ID + " order by A.HistoryID desc")
                    //        select new ManufacturerMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            Manufacturer = u.Manufacturer,
                    //            Created = u.Created,
                    //            Updated = u.Updated,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                    //            //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                    //            //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            GUID = u.GUID,
                    //            CompanyID = u.CompanyID,
                    //            IsArchived = u.IsArchived,
                    //            IsDeleted = u.IsDeleted,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
                    case "MeasurementTermList":
                        TotalCount = 0;
                        return new List<MeasurementTermMasterDTO>();
                    // Below code is commented bcause MeasurementTermList page is not being used.

                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM MeasurementTermMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<MeasurementTermMasterDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName ,'' AS CreatedDate,'' AS UpdatedDate                               
                    //        FROM  MeasurementTermMaster_History as A 
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.Room = D.ID                                 
                    //        WHERE A.ID = " + ID + " order by A.HistoryID desc")
                    //        select new MeasurementTermMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            MeasurementTerm = u.MeasurementTerm,
                    //            Created = u.Created,
                    //            Updated = u.Updated,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                    //            //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                    //            //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            CompanyID = u.CompanyID,
                    //            IsArchived = u.IsArchived,
                    //            IsDeleted = u.IsDeleted,
                    //            GUID = u.GUID,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
                    case "ProjectList":
                        var paramProject = new SqlParameter[] { new SqlParameter("@ID", ID) };
                        List<ProjectMasterDTO> lstProject = (from u in context.Database.SqlQuery<ProjectMasterDTO>("exec [GetProjectMasterHistory] @ID", paramProject)
                                                             select new ProjectMasterDTO
                                                             {
                                                                 ID = u.ID,
                                                                 HistoryID = u.HistoryID,
                                                                 Action = u.Action,
                                                                 ProjectSpendName = u.ProjectSpendName,
                                                                 DollarLimitAmount = u.DollarLimitAmount,
                                                                 DollarUsedAmount = u.DollarUsedAmount,
                                                                 Description = u.Description,
                                                                 CompanyID = u.CompanyID,
                                                                 Created = u.Created,
                                                                 Updated = u.Updated,
                                                                 CreatedBy = u.CreatedBy,
                                                                 LastUpdatedBy = u.LastUpdatedBy,
                                                                 CreatedByName = u.CreatedByName,
                                                                 UpdatedByName = u.UpdatedByName,
                                                                 RoomName = u.RoomName,
                                                                 IsArchived = u.IsArchived,
                                                                 IsDeleted = u.IsDeleted,
                                                                 GUID = u.GUID,
                                                                 UDF1 = u.UDF1,
                                                                 UDF2 = u.UDF2,
                                                                 UDF3 = u.UDF3,
                                                                 UDF4 = u.UDF4,
                                                                 UDF5 = u.UDF5,
                                                                 WhatWhereAction = u.WhatWhereAction,
                                                                 CreatedDate = u.CreatedDate,
                                                                 UpdatedDate = u.UpdatedDate,
                                                                 ReceivedOn = u.ReceivedOn,
                                                                 ReceivedOnWeb = u.ReceivedOnWeb,
                                                                 AddedFrom = u.AddedFrom,
                                                                 EditedFrom = u.EditedFrom,
                                                                 TotalRecords = u.TotalRecords,
                                                             }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstProject != null && lstProject.Count > 0)
                        {
                            TotalCount = lstProject.FirstOrDefault().TotalRecords;
                        }

                        return lstProject;
                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM ProjectMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<ProjectMasterDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName  ,'' AS CreatedDate,'' AS UpdatedDate                              
                    //        FROM ProjectMaster_History as A 
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.Room = D.ID                                 
                    //        WHERE A.ID = " + ID + " order by A.HistoryID desc")
                    //        select new ProjectMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            ProjectSpendName = u.ProjectSpendName,
                    //            DollarLimitAmount = u.DollarLimitAmount,
                    //            DollarUsedAmount = u.DollarUsedAmount,
                    //            Description = u.Description,
                    //            CompanyID = u.CompanyID,
                    //            Created = u.Created,
                    //            Updated = u.Updated,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            IsArchived = u.IsArchived,
                    //            IsDeleted = u.IsDeleted,
                    //            GUID = u.GUID,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            WhatWhereAction = u.WhatWhereAction,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
                    case "ShipViaList":
                        var paramShipVia = new SqlParameter[] { new SqlParameter("@ID", ID) };
                        List<ShipViaDTO> lstShipVia = (from u in context.Database.SqlQuery<ShipViaDTO>("exec [GetShipViaMasterHistory] @ID", paramShipVia)
                                                       select new ShipViaDTO
                                                       {
                                                           ID = u.ID,
                                                           HistoryID = u.HistoryID,
                                                           Action = u.Action,
                                                           ShipVia = u.ShipVia,
                                                           Created = u.Created,
                                                           Updated = u.Updated,
                                                           //CreatedByName = u.CreatedBy.HasValue ? new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(): string.Empty,
                                                           //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                                                           //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                                                           CreatedByName = u.CreatedByName,
                                                           UpdatedByName = u.UpdatedByName,
                                                           RoomName = u.RoomName,
                                                           CreatedBy = u.CreatedBy,
                                                           LastUpdatedBy = u.LastUpdatedBy,
                                                           GUID = u.GUID,
                                                           CompanyID = u.CompanyID,
                                                           IsDeleted = u.IsDeleted,
                                                           IsArchived = u.IsArchived,
                                                           UDF1 = u.UDF1,
                                                           UDF2 = u.UDF2,
                                                           UDF3 = u.UDF3,
                                                           UDF4 = u.UDF4,
                                                           UDF5 = u.UDF5,
                                                           CreatedDate = u.CreatedDate,
                                                           UpdatedDate = u.UpdatedDate,
                                                           AddedFrom = u.AddedFrom,
                                                           EditedFrom = u.EditedFrom,
                                                           ReceivedOn = u.ReceivedOn,
                                                           ReceivedOnWeb = u.ReceivedOnWeb,
                                                           TotalRecords = u.TotalRecords
                                                       }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstShipVia != null && lstShipVia.Count > 0)
                        {
                            TotalCount = lstShipVia.FirstOrDefault().TotalRecords;
                        }

                        return lstShipVia;
                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM ShipViaMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<ShipViaDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName  ,'' AS CreatedDate,'' AS UpdatedDate                              
                    //        FROM  ShipViaMaster_History as A 
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.Room = D.ID                                 
                    //        WHERE A.ID = " + ID + " order by A.HistoryID desc")
                    //        select new ShipViaDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            ShipVia = u.ShipVia,
                    //            Created = u.Created,
                    //            Updated = u.Updated,
                    //            //CreatedByName = u.CreatedBy.HasValue ? new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(): string.Empty,
                    //            //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                    //            //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            GUID = u.GUID,
                    //            CompanyID = u.CompanyID,
                    //            IsDeleted = u.IsDeleted,
                    //            IsArchived = u.IsArchived,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
                    case "SupplierBlankePOList":
                    case "SupplierList":
                        var paramSupp = new SqlParameter[] { new SqlParameter("@ID", ID) };
                        List<SupplierMasterDTO> lstSupp = (from u in context.Database.SqlQuery<SupplierMasterDTO>("exec [GetSupplierMasterHistory] @ID", paramSupp)
                                                           select new SupplierMasterDTO
                                                           {
                                                               ID = u.ID,
                                                               HistoryID = u.HistoryID,
                                                               Action = u.Action,
                                                               SupplierName = u.SupplierName,
                                                               Description = u.Description,
                                                               AccountNo = u.AccountNo,
                                                               ReceiverID = u.ReceiverID,
                                                               Address = u.Address,
                                                               City = u.City,
                                                               State = u.State,
                                                               ZipCode = u.ZipCode,
                                                               Country = u.Country,
                                                               Contact = u.Contact,
                                                               Phone = u.Phone,
                                                               Fax = u.Fax,
                                                               Email = u.Email,
                                                               IsEmailPOInBody = u.IsEmailPOInBody,
                                                               IsEmailPOInPDF = u.IsEmailPOInPDF,
                                                               IsEmailPOInCSV = u.IsEmailPOInCSV,
                                                               IsEmailPOInX12 = u.IsEmailPOInX12,
                                                               Created = u.Created,
                                                               LastUpdated = u.LastUpdated,
                                                               CreatedBy = u.CreatedBy,
                                                               LastUpdatedBy = u.LastUpdatedBy,
                                                               Room = u.Room,
                                                               GUID = u.GUID,
                                                               IsDeleted = u.IsDeleted,
                                                               IsArchived = u.IsArchived,
                                                               //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                                                               //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                                                               //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                                                               CreatedByName = u.CreatedByName,
                                                               UpdatedByName = u.UpdatedByName,
                                                               RoomName = u.RoomName,
                                                               CompanyID = u.CompanyID,
                                                               UDF1 = u.UDF1,
                                                               UDF2 = u.UDF2,
                                                               UDF3 = u.UDF3,
                                                               UDF4 = u.UDF4,
                                                               UDF5 = u.UDF5,
                                                               CreatedDate = u.CreatedDate,
                                                               UpdatedDate = u.UpdatedDate,
                                                               AddedFrom = u.AddedFrom,
                                                               EditedFrom = u.EditedFrom,
                                                               ReceivedOn = u.ReceivedOn,
                                                               ReceivedOnWeb = u.ReceivedOnWeb,
                                                               DefaultOrderRequiredDays = u.DefaultOrderRequiredDays,
                                                               BranchNumber = u.BranchNumber,
                                                               MaximumOrderSize = u.MaximumOrderSize,
                                                               IsSendtoVendor = u.IsSendtoVendor,
                                                               IsVendorReturnAsn = u.IsVendorReturnAsn,
                                                               IsSupplierReceivesKitComponents = u.IsSupplierReceivesKitComponents,
                                                               TotalRecords = u.TotalRecords
                                                           }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstSupp != null && lstSupp.Count > 0)
                        {
                            TotalCount = lstSupp.FirstOrDefault().TotalRecords;
                        }

                        return lstSupp;
                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM SupplierMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<SupplierMasterDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName  ,'' AS CreatedDate,'' AS UpdatedDate                              
                    //        FROM  SupplierMaster_History as A 
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.Room = D.ID 
                    //        WHERE A.ID = " + ID + " order by A.HistoryID desc")
                    //        select new SupplierMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            SupplierName = u.SupplierName,
                    //            Description = u.Description,
                    //            AccountNo = u.AccountNo,
                    //            ReceiverID = u.ReceiverID,
                    //            Address = u.Address,
                    //            City = u.City,
                    //            State = u.State,
                    //            ZipCode = u.ZipCode,
                    //            Country = u.Country,
                    //            Contact = u.Contact,
                    //            Phone = u.Phone,
                    //            Fax = u.Fax,
                    //            Email = u.Email,
                    //            IsEmailPOInBody = u.IsEmailPOInBody,
                    //            IsEmailPOInPDF = u.IsEmailPOInPDF,
                    //            IsEmailPOInCSV = u.IsEmailPOInCSV,
                    //            IsEmailPOInX12 = u.IsEmailPOInX12,
                    //            Created = u.Created,
                    //            LastUpdated = u.LastUpdated,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            Room = u.Room,
                    //            GUID = u.GUID,
                    //            IsDeleted = u.IsDeleted,
                    //            IsArchived = u.IsArchived,
                    //            //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                    //            //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                    //            //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            CompanyID = u.CompanyID,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb,
                    //            DefaultOrderRequiredDays = u.DefaultOrderRequiredDays,
                    //            BranchNumber = u.BranchNumber,
                    //            MaximumOrderSize = u.MaximumOrderSize,
                    //            IsSendtoVendor = u.IsSendtoVendor,
                    //            IsVendorReturnAsn = u.IsVendorReturnAsn,
                    //            IsSupplierReceivesKitComponents = u.IsSupplierReceivesKitComponents

                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
                    case "ToolCategoryList":
                        //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM  WHERE ID = " + ID + "").ToList()[0]);
                        var paramToolCatagory = new SqlParameter[] { new SqlParameter("@ID", ID) };
                        List<ToolCategoryMasterDTO> lstToolCategory = (from u in context.Database.SqlQuery<ToolCategoryMasterDTO>("exec [GetToolCategoryMasterHistory] @ID", paramToolCatagory)
                                                                       select new ToolCategoryMasterDTO
                                                                       {
                                                                           ID = u.ID,
                                                                           HistoryID = u.HistoryID,
                                                                           Action = u.Action,
                                                                           ToolCategory = u.ToolCategory,
                                                                           Created = u.Created,
                                                                           Updated = u.Updated,
                                                                           //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                                                                           //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                                                                           //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                                                                           CreatedByName = u.CreatedByName,
                                                                           UpdatedByName = u.UpdatedByName,
                                                                           RoomName = u.RoomName,
                                                                           LastUpdatedBy = u.LastUpdatedBy,
                                                                           CreatedBy = u.CreatedBy,
                                                                           Room = u.Room,
                                                                           GUID = u.GUID,
                                                                           CompanyID = u.CompanyID,
                                                                           IsDeleted = u.IsDeleted,
                                                                           IsArchived = u.IsArchived,
                                                                           UDF1 = u.UDF1,
                                                                           UDF2 = u.UDF2,
                                                                           UDF3 = u.UDF3,
                                                                           UDF4 = u.UDF4,
                                                                           UDF5 = u.UDF5,
                                                                           CreatedDate = u.CreatedDate,
                                                                           UpdatedDate = u.UpdatedDate,
                                                                           AddedFrom = u.AddedFrom,
                                                                           EditedFrom = u.EditedFrom,
                                                                           ReceivedOn = u.ReceivedOn,
                                                                           ReceivedOnWeb = u.ReceivedOnWeb,
                                                                           TotalRecords = u.TotalRecords
                                                                       }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstToolCategory != null && lstToolCategory.Count > 0)
                        {
                            TotalCount = lstToolCategory.FirstOrDefault().TotalRecords;
                        }

                        return lstToolCategory;
                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM ToolCategoryMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<ToolCategoryMasterDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName ,'' AS CreatedDate,'' AS UpdatedDate                               
                    //        FROM  ToolCategoryMaster_History as A 
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.Room = D.ID                                 
                    //        WHERE A.ID = " + ID + " order by A.HistoryID desc")
                    //        select new ToolCategoryMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            ToolCategory = u.ToolCategory,
                    //            Created = u.Created,
                    //            Updated = u.Updated,
                    //            //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                    //            //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                    //            //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            CreatedBy = u.CreatedBy,
                    //            Room = u.Room,
                    //            GUID = u.GUID,
                    //            CompanyID = u.CompanyID,
                    //            IsDeleted = u.IsDeleted,
                    //            IsArchived = u.IsArchived,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
                    case "AssetCategoryList":
                        var param = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@dbName", DataBaseName) };
                        List<AssetCategoryMasterDTO> lstcats = context.Database.SqlQuery<AssetCategoryMasterDTO>("exec [GetAssetCategoryMasterChangeLog] @ID,@dbName", param).ToList();
                        TotalCount = 0;
                        if (lstcats != null && lstcats.Count > 0)
                        {
                            TotalCount = lstcats.First().TotalRecords;
                        }

                        return lstcats;
                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM AssetCategoryMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<AssetCategoryMasterDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,'' AS CreatedDate,'' As UpdatedDate                                
                    //        FROM  AssetCategoryMaster_History as A 
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.Room = D.ID                                 
                    //        WHERE A.ID = " + ID + " order by A.HistoryID desc")
                    //        select new AssetCategoryMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            AssetCategory = u.AssetCategory,
                    //            Created = u.Created,
                    //            Updated = u.Updated,
                    //            //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                    //            //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                    //            //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            CreatedBy = u.CreatedBy,
                    //            Room = u.Room,
                    //            GUID = u.GUID,
                    //            CompanyID = u.CompanyID,
                    //            IsDeleted = u.IsDeleted,
                    //            IsArchived = u.IsArchived,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
                    case "ToolList":
                        var paramToolList = new SqlParameter[] { new SqlParameter("@ID", ID) };
                        List<ToolMasterDTO> lstToolMaster = (from u in context.Database.SqlQuery<ToolMasterDTO>("exec [GetToolMasterHistory] @ID", paramToolList)
                                                             select new ToolMasterDTO
                                                             {
                                                                 ID = u.ID,
                                                                 HistoryID = u.HistoryID,
                                                                 Action = u.Action,
                                                                 ToolName = u.ToolName,
                                                                 Serial = u.Serial,
                                                                 Description = u.Description,
                                                                 Cost = u.Cost,
                                                                 Quantity = u.Quantity,
                                                                 IsCheckedOut = u.IsCheckedOut,
                                                                 IsGroupOfItems = u.IsGroupOfItems,
                                                                 ToolCategoryID = u.ToolCategoryID,
                                                                 ToolCategory = u.ToolCategory,
                                                                 Created = u.Created,
                                                                 Updated = u.Updated,
                                                                 CreatedBy = u.CreatedBy,
                                                                 LastUpdatedBy = u.LastUpdatedBy,
                                                                 CreatedByName = u.CreatedByName,
                                                                 UpdatedByName = u.UpdatedByName,
                                                                 Room = u.Room,
                                                                 RoomName = u.RoomName,
                                                                 Location = u.Location,
                                                                 LocationID = u.LocationID,
                                                                 GUID = u.GUID,
                                                                 CompanyID = u.CompanyID,
                                                                 IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                                 IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                                 UDF1 = u.UDF1,
                                                                 UDF2 = u.UDF2,
                                                                 UDF3 = u.UDF3,
                                                                 UDF4 = u.UDF4,
                                                                 UDF5 = u.UDF5,
                                                                 CreatedDate = u.CreatedDate,
                                                                 UpdatedDate = u.UpdatedDate,
                                                                 AddedFrom = u.AddedFrom,
                                                                 EditedFrom = u.EditedFrom,
                                                                 ReceivedOn = u.ReceivedOn,
                                                                 ReceivedOnWeb = u.ReceivedOnWeb,
                                                                 TotalRecords = u.TotalRecords
                                                             }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstToolMaster != null && lstToolMaster.Count > 0)
                        {
                            TotalCount = lstToolMaster.FirstOrDefault().TotalRecords ?? 0;
                        }

                        return lstToolMaster;

                    #region Old
                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM ToolMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<ToolMasterDTO>(
                    //            @"SELECT 
                    //            A.ID,A.HistoryID,A.Action,A.ToolName, A.Serial, A.Description, A.Cost, A.IsDeleted, A.IsArchived
                    //            ,A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5,A.CreatedBy,A.LastUpdatedBy,A.Quantity
                    //            ,A.IsCheckedOut,A.IsGroupOfItems, A.Created, A.Updated, A.ToolCategoryID
                    //            ,A.LocationID,A.Room,A.CompanyId
                    //            ,B.UserName AS 'CreatedByName'
                    //            ,C.UserName AS 'UpdatedByName'
                    //            ,D.RoomName, E.ToolCategory 
                    //            ,L.Location  ,'' AS CreatedDate,'' AS UpdatedDate,A.AddedFrom,A.EditedFrom,A.ReceivedOn,A.ReceivedOnWeb
                    //            FROM ToolMaster_History A 
                    //            LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID 
                    //            LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //            LEFT OUTER JOIN Room D on A.Room = D.ID 
                    //            LEFT OUTER JOIN ToolCategoryMaster E on A.ToolCategoryID = E.ID  
                    //            LEFT OUTER JOIN LocationMaster L on A.LocationID = L.ID
                    //            WHERE A.ID = " + ID + " order by HistoryID desc"
                    //            )
                    //        select new ToolMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            ToolName = u.ToolName,
                    //            Serial = u.Serial,
                    //            Description = u.Description,
                    //            Cost = u.Cost,
                    //            Quantity = u.Quantity,
                    //            IsCheckedOut = u.IsCheckedOut,
                    //            IsGroupOfItems = u.IsGroupOfItems,
                    //            ToolCategoryID = u.ToolCategoryID,
                    //            ToolCategory = u.ToolCategory,
                    //            Created = u.Created,
                    //            Updated = u.Updated,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            Room = u.Room,
                    //            RoomName = u.RoomName,
                    //            Location = u.Location,
                    //            LocationID = u.LocationID,
                    //            GUID = u.GUID,
                    //            CompanyID = u.CompanyID,
                    //            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                    //            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList(); 
                    #endregion
                    case "UnitList":
                        var paramUnit = new SqlParameter[] { new SqlParameter("@ID", ID) };
                        List<UnitMasterDTO> lstUnitList = (from u in context.Database.SqlQuery<UnitMasterDTO>("exec [GetUnitHistory] @ID", paramUnit)
                                                           select new UnitMasterDTO
                                                           {
                                                               ID = u.ID,
                                                               HistoryID = u.HistoryID,
                                                               Action = u.Action,
                                                               Unit = u.Unit,
                                                               Created = u.Created,
                                                               CreatedBy = u.CreatedBy,
                                                               LastUpdatedBy = u.LastUpdatedBy,
                                                               Room = u.Room,
                                                               Updated = u.Updated,
                                                               Description = u.Description,
                                                               //EngineModel = u.EngineModel,
                                                               //EngineSerialNo = u.EngineSerialNo,
                                                               //GUID = u.GUID,
                                                               //Make = u.Make,
                                                               //MarkupLabour = u.MarkupLabour,
                                                               //MarkupParts = u.MarkupParts,
                                                               //Model = u.Model,
                                                               //Odometer = u.Odometer,
                                                               //OdometerUpdate = u.OdometerUpdate,
                                                               //OpHours = u.OpHours,
                                                               //OpHoursUpdate = u.OpHoursUpdate,
                                                               //Plate = u.Plate,
                                                               //SerialNo = u.SerialNo,
                                                               //Year = u.Year,
                                                               IsArchived = u.IsArchived,
                                                               IsDeleted = u.IsDeleted,
                                                               //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                                                               //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                                                               //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                                                               CreatedByName = u.CreatedByName,
                                                               UpdatedByName = u.UpdatedByName,
                                                               RoomName = u.RoomName,
                                                               CompanyID = u.CompanyID,
                                                               CreatedDate = u.CreatedDate,
                                                               UpdatedDate = u.UpdatedDate,
                                                               AddedFrom = u.AddedFrom,
                                                               EditedFrom = u.EditedFrom,
                                                               ReceivedOn = u.ReceivedOn,
                                                               ReceivedOnWeb = u.ReceivedOnWeb,
                                                               TotalRecords = u.TotalRecords
                                                           }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstUnitList != null && lstUnitList.Count > 0)
                        {
                            TotalCount = lstUnitList.FirstOrDefault().TotalRecords;
                        }

                        return lstUnitList;

                    #region Old Code
                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM UnitMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<UnitMasterDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName ,'' AS CreatedDate,'' AS UpdatedDate                               
                    //        FROM  UnitMaster_History as A 
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.Room = D.ID                                 
                    //        WHERE A.ID = " + ID + " order by A.HistoryID desc")
                    //        select new UnitMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            Unit = u.Unit,
                    //            Created = u.Created,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            Room = u.Room,
                    //            Updated = u.Updated,
                    //            Description = u.Description,
                    //            //EngineModel = u.EngineModel,
                    //            //EngineSerialNo = u.EngineSerialNo,
                    //            //GUID = u.GUID,
                    //            //Make = u.Make,
                    //            //MarkupLabour = u.MarkupLabour,
                    //            //MarkupParts = u.MarkupParts,
                    //            //Model = u.Model,
                    //            //Odometer = u.Odometer,
                    //            //OdometerUpdate = u.OdometerUpdate,
                    //            //OpHours = u.OpHours,
                    //            //OpHoursUpdate = u.OpHoursUpdate,
                    //            //Plate = u.Plate,
                    //            //SerialNo = u.SerialNo,
                    //            //Year = u.Year,
                    //            IsArchived = u.IsArchived,
                    //            IsDeleted = u.IsDeleted,
                    //            //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                    //            //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                    //            //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            CompanyID = u.CompanyID,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList(); 
                    #endregion
                    case "ItemList":
                        //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM  WHERE ID = " + ID + "").ToList()[0]);
                        var paramItemlist = new SqlParameter[] { new SqlParameter("@ID", ID) };
                        List<ItemMasterDTO> lstItemMaster = (from u in context.Database.SqlQuery<ItemMasterDTO>("exec [GetItemMasterHistory] @ID", paramItemlist)
                                                             select new ItemMasterDTO
                                                             {
                                                                 ID = u.ID,
                                                                 HistoryID = u.HistoryID,
                                                                 Action = u.Action,
                                                                 ItemNumber = u.ItemNumber,
                                                                 ManufacturerID = u.ManufacturerID,
                                                                 ManufacturerNumber = u.ManufacturerNumber,
                                                                 ManufacturerName = u.ManufacturerName,
                                                                 SupplierID = u.SupplierID,
                                                                 SupplierPartNo = u.SupplierPartNo,
                                                                 SupplierName = u.SupplierName,
                                                                 UPC = u.UPC == null ? "" : u.UPC,
                                                                 UNSPSC = u.UNSPSC == null ? "" : u.UNSPSC,
                                                                 Description = u.Description,
                                                                 LongDescription = u.LongDescription,
                                                                 CategoryID = u.CategoryID.GetValueOrDefault(0),
                                                                 GLAccountID = u.GLAccountID.GetValueOrDefault(0),
                                                                 UOMID = u.UOMID,
                                                                 PricePerTerm = u.PricePerTerm.GetValueOrDefault(0),
                                                                 DefaultReorderQuantity = u.DefaultReorderQuantity,
                                                                 DefaultPullQuantity = u.DefaultPullQuantity,
                                                                 Cost = u.Cost.GetValueOrDefault(0),
                                                                 Markup = u.Markup.GetValueOrDefault(0),
                                                                 SellPrice = u.SellPrice.GetValueOrDefault(0),
                                                                 ExtendedCost = u.ExtendedCost.GetValueOrDefault(0),
                                                                 LeadTimeInDays = u.LeadTimeInDays.GetValueOrDefault(0),
                                                                 Trend = u.Trend,
                                                                 Taxable = u.Taxable,
                                                                 Consignment = u.Consignment,
                                                                 StagedQuantity = u.StagedQuantity.GetValueOrDefault(0),
                                                                 InTransitquantity = u.InTransitquantity.GetValueOrDefault(0),
                                                                 OnOrderQuantity = u.OnOrderQuantity.GetValueOrDefault(0),
                                                                 OnReturnQuantity = u.OnReturnQuantity.GetValueOrDefault(0),
                                                                 OnTransferQuantity = u.OnTransferQuantity.GetValueOrDefault(0),
                                                                 SuggestedOrderQuantity = u.SuggestedOrderQuantity.GetValueOrDefault(0),
                                                                 SuggestedTransferQuantity = u.SuggestedTransferQuantity.GetValueOrDefault(0),
                                                                 RequisitionedQuantity = u.RequisitionedQuantity.GetValueOrDefault(0),
                                                                 AverageUsage = u.AverageUsage.GetValueOrDefault(0),
                                                                 Turns = u.Turns.GetValueOrDefault(0),
                                                                 OnHandQuantity = u.OnHandQuantity == null ? 0 : u.OnHandQuantity,//u.OnHandQuantity.GetValueOrDefault(0),
                                                                 CriticalQuantity = u.CriticalQuantity,
                                                                 MinimumQuantity = u.MinimumQuantity,
                                                                 MaximumQuantity = u.MaximumQuantity,
                                                                 WeightPerPiece = u.WeightPerPiece.GetValueOrDefault(0),
                                                                 ItemUniqueNumber = u.ItemUniqueNumber,
                                                                 //TransferOrPurchase = u.TransferOrPurchase,
                                                                 IsPurchase = u.IsPurchase,
                                                                 IsTransfer = u.IsTransfer,
                                                                 DefaultLocation = u.DefaultLocation,
                                                                 InventoryClassification = u.InventoryClassification,
                                                                 SerialNumberTracking = u.SerialNumberTracking,
                                                                 LotNumberTracking = u.LotNumberTracking,
                                                                 DateCodeTracking = u.DateCodeTracking,
                                                                 ItemType = u.ItemType,
                                                                 ImagePath = u.ImagePath,
                                                                 UDF1 = u.UDF1,
                                                                 UDF2 = u.UDF2,
                                                                 UDF3 = u.UDF3,
                                                                 UDF4 = u.UDF4,
                                                                 UDF5 = u.UDF5,
                                                                 GUID = u.GUID,
                                                                 Created = u.Created,
                                                                 Updated = u.Updated,
                                                                 CreatedBy = u.CreatedBy,
                                                                 LastUpdatedBy = u.LastUpdatedBy,
                                                                 IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                                 IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                                 CompanyID = u.CompanyID,
                                                                 Room = u.Room.GetValueOrDefault(0),
                                                                 CreatedByName = u.CreatedByName,
                                                                 UpdatedByName = u.UpdatedByName,
                                                                 RoomName = u.RoomName,
                                                                 IsLotSerialExpiryCost = u.IsLotSerialExpiryCost,
                                                                 CategoryName = u.CategoryName,
                                                                 Unit = u.Unit,
                                                                 GLAccount = u.GLAccount,
                                                                 ItemTypeName = u.ItemTypeName,
                                                                 CreatedDate = u.CreatedDate,
                                                                 UpdatedDate = u.UpdatedDate,
                                                                 AddedFrom = u.AddedFrom,
                                                                 EditedFrom = u.EditedFrom,
                                                                 ReceivedOn = u.ReceivedOn,
                                                                 ReceivedOnWeb = u.ReceivedOnWeb,
                                                                 OnOrderInTransitQuantity = u.OnOrderInTransitQuantity.GetValueOrDefault(0),
                                                                 TotalRecords = u.TotalRecords
                                                             }).AsParallel().ToList();

                        TotalCount = 0;
                        if (lstItemMaster != null && lstItemMaster.Count > 0)
                        {
                            TotalCount = lstItemMaster.FirstOrDefault().TotalRecords;
                        }

                        return lstItemMaster;
                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM ItemMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<ItemMasterDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,
                    //        M.Manufacturer as ManufacturerName,S.SupplierName, C1.Category AS CategoryName,U1.Unit,G1.GLAccount,'' AS CreatedDate,'' AS UpdatedDate
                    //        FROM ItemMaster_History A 
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.Room = D.ID 
                    //        LEFT OUTER join ManufacturerMaster M on m.id = A.ManufacturerID 
                    //        left outer join SupplierMaster S on S.id = A.SupplierID 
                    //        LEFT OUTER join CategoryMaster C1 on C1.id = A.CategoryID
                    //        LEFT OUTER join UnitMaster U1 on U1.id = A.UOMID
                    //        LEFT OUTER join GLAccountMaster G1 on G1.id = A.GLAccountID
                    //        WHERE A.ID = " + ID + " Order by A.HistoryID desc"
                    //        )
                    //        select new ItemMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            ItemNumber = u.ItemNumber,
                    //            ManufacturerID = u.ManufacturerID,
                    //            ManufacturerNumber = u.ManufacturerNumber,
                    //            ManufacturerName = u.ManufacturerName,
                    //            SupplierID = u.SupplierID,
                    //            SupplierPartNo = u.SupplierPartNo,
                    //            SupplierName = u.SupplierName,
                    //            UPC = u.UPC == null ? "" : u.UPC,
                    //            UNSPSC = u.UNSPSC == null ? "" : u.UNSPSC,
                    //            Description = u.Description,
                    //            LongDescription = u.LongDescription,
                    //            CategoryID = u.CategoryID.GetValueOrDefault(0),
                    //            GLAccountID = u.GLAccountID.GetValueOrDefault(0),
                    //            UOMID = u.UOMID,
                    //            PricePerTerm = u.PricePerTerm.GetValueOrDefault(0),
                    //            DefaultReorderQuantity = u.DefaultReorderQuantity,
                    //            DefaultPullQuantity = u.DefaultPullQuantity,
                    //            Cost = u.Cost.GetValueOrDefault(0),
                    //            Markup = u.Markup.GetValueOrDefault(0),
                    //            SellPrice = u.SellPrice.GetValueOrDefault(0),
                    //            ExtendedCost = u.ExtendedCost.GetValueOrDefault(0),
                    //            LeadTimeInDays = u.LeadTimeInDays.GetValueOrDefault(0),
                    //            Trend = u.Trend,
                    //            Taxable = u.Taxable,
                    //            Consignment = u.Consignment,
                    //            StagedQuantity = u.StagedQuantity.GetValueOrDefault(0),
                    //            InTransitquantity = u.InTransitquantity.GetValueOrDefault(0),
                    //            OnOrderQuantity = u.OnOrderQuantity.GetValueOrDefault(0),
                    //            OnReturnQuantity = u.OnReturnQuantity.GetValueOrDefault(0),
                    //            OnTransferQuantity = u.OnTransferQuantity.GetValueOrDefault(0),
                    //            SuggestedOrderQuantity = u.SuggestedOrderQuantity.GetValueOrDefault(0),
                    //            SuggestedTransferQuantity = u.SuggestedTransferQuantity.GetValueOrDefault(0),
                    //            RequisitionedQuantity = u.RequisitionedQuantity.GetValueOrDefault(0),
                    //            AverageUsage = u.AverageUsage.GetValueOrDefault(0),
                    //            Turns = u.Turns.GetValueOrDefault(0),
                    //            OnHandQuantity = u.OnHandQuantity == null ? 0 : u.OnHandQuantity,//u.OnHandQuantity.GetValueOrDefault(0),
                    //            CriticalQuantity = u.CriticalQuantity,
                    //            MinimumQuantity = u.MinimumQuantity,
                    //            MaximumQuantity = u.MaximumQuantity,
                    //            WeightPerPiece = u.WeightPerPiece.GetValueOrDefault(0),
                    //            ItemUniqueNumber = u.ItemUniqueNumber,
                    //            //TransferOrPurchase = u.TransferOrPurchase,
                    //            IsPurchase = u.IsPurchase,
                    //            IsTransfer = u.IsTransfer,
                    //            DefaultLocation = u.DefaultLocation,
                    //            InventoryClassification = u.InventoryClassification,
                    //            SerialNumberTracking = u.SerialNumberTracking,
                    //            LotNumberTracking = u.LotNumberTracking,
                    //            DateCodeTracking = u.DateCodeTracking,
                    //            ItemType = u.ItemType,
                    //            ImagePath = u.ImagePath,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            GUID = u.GUID,
                    //            Created = u.Created,
                    //            Updated = u.Updated,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                    //            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                    //            CompanyID = u.CompanyID,
                    //            Room = u.Room.GetValueOrDefault(0),
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            IsLotSerialExpiryCost = u.IsLotSerialExpiryCost,
                    //            CategoryName = u.CategoryName,
                    //            Unit = u.Unit,
                    //            GLAccount = u.GLAccount,
                    //            ItemTypeName = u.ItemTypeName,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb,
                    //            OnOrderInTransitQuantity = u.OnOrderInTransitQuantity.GetValueOrDefault(0),

                    //        }).AsParallel().ToList();
                    case "QuickList":
                        TotalCount = 0;
                        return new List<QuickListMasterDTO>();
                    // Below code is commented bcause this code is not being called.

                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM QuickListMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //UserMasterDAL UMDAL = new UserMasterDAL(base.DataBaseName);
                    //return (from u in context.Database.SqlQuery<QuickListMasterDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,                                
                    //        (SELECT Count(ID) From QuickListItems_History E WHERE A.GUID= E.QuickListGUID) as NoOfItems ,'' AS CreatedDate,'' AS UpdatedDate
                    //        FROM  QuickListMaster_History as A 
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.Room = D.ID                                 
                    //        WHERE A.ID = " + ID + " order by A.HistoryID desc")
                    //        select new QuickListMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            Name = u.Name,
                    //            Comment = u.Comment,
                    //            CompanyID = u.CompanyID,
                    //            IsDeleted = u.IsDeleted.GetValueOrDefault(false),
                    //            IsArchived = u.IsArchived.GetValueOrDefault(false),
                    //            Type = u.Type,
                    //            Created = u.Created,
                    //            LastUpdated = u.LastUpdated,
                    //            //CreatedByName = UMDAL.GetRecord(Convert.ToInt64(u.CreatedBy)).UserName,
                    //            //UpdatedByName = UMDAL.GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName,
                    //            //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), false, false).RoomName.ToString(),
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            IsHistory = true,
                    //            Room = u.Room,
                    //            GUID = u.GUID,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            NoOfItems = u.NoOfItems,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                    case "PullList":
                        TotalCount = 0;
                        return new List<PullMasterDTO>();
                    // Below code is commented bcause this code is not being called.

                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM PullMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<PullMasterDTO>(@"
                    //        SELECT A.*, 
                    //        B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,P.ProjectSpendName as ProjectName,
                    //        I.ItemNumber,I.DefaultPullQuantity,I.OnHandQuantity,I.CategoryID,I.ManufacturerID,I.SupplierID  ,'' AS CreatedDate,'' AS UpdatedDate
                    //        FROM PullMaster_History AS A 
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.Room = D.ID 
                    //        LEFT OUTER  JOIN ProjectMaster P on P.GUID = A.ProjectSpendGUID 
                    //        LEFT OUTER  JOIN ItemMaster I on I.GUID = A.ItemGUID
                    //        WHERE A.ID = " + ID + " order by HistoryID desc")
                    //        select new PullMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            ProjectName = u.ProjectName,
                    //            DefaultPullQuantity = u.DefaultPullQuantity,
                    //            OnHandQuantity = u.OnHandQuantity,
                    //            ItemNumber = u.ItemNumber,
                    //            PoolQuantity = u.PoolQuantity,
                    //            SerialNumber = u.SerialNumber,
                    //            LotNumber = u.LotNumber,
                    //            DateCode = u.DateCode,
                    //            BinID = u.BinID,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            GUID = u.GUID,
                    //            Created = u.Created,
                    //            Updated = u.Updated,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            IsDeleted = u.IsDeleted,
                    //            IsArchived = u.IsArchived,
                    //            CompanyID = u.CompanyID,
                    //            Room = u.Room,
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                    //            //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                    //            //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
                    case "MaterialStaging":
                        TotalCount = 0;
                        return new List<MaterialStagingDTO>();
                    // Below code is commented bcause this code is not being called.

                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM MaterialStaging_History WHERE ID = " + ID + "").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<MaterialStagingDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName
                    //        ,L.BinNumber as 'BinName'  ,'' AS CreatedDate,'' AS UpdatedDate
                    //        FROM MaterialStaging_History AS A
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.Room = D.ID
                    //        left outer join BinMaster L on A.BinID = L.ID
                    //        WHERE A.ID = " + ID + " order by HinstoryID desc")
                    //        select new MaterialStagingDTO
                    //        {
                    //            ID = u.ID,
                    //            HinstoryID = u.HinstoryID,
                    //            Action = u.Action,
                    //            StagingName = u.StagingName,
                    //            StagingLocationName = u.StagingLocationName,//new BinMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.BinID),u.Room.GetValueOrDefault(0), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).BinNumber.ToString(),
                    //            Description = u.Description,
                    //            BinID = u.BinID,
                    //            GUID = u.GUID,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            CompanyID = u.CompanyID,
                    //            Room = u.Room,
                    //            IsDeleted = u.IsDeleted,
                    //            IsArchived = u.IsArchived,
                    //            Created = u.Created,
                    //            Updated = u.Updated,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
                    case "OrderList":
                        var paramOrder = new SqlParameter[] { new SqlParameter("@ID", ID) };
                        List<OrderMasterDTO> lstOrderMaster = (from u in context.Database.SqlQuery<OrderMasterDTO>("exec [GetOrderMasterHistory] @ID", paramOrder)
                                                               select new OrderMasterDTO
                                                               {
                                                                   ID = u.ID,
                                                                   HistoryID = u.HistoryID,
                                                                   Action = u.Action,
                                                                   OrderNumber = u.OrderNumber,
                                                                   ReleaseNumber = u.ReleaseNumber,
                                                                   ShipVia = u.ShipVia,
                                                                   Supplier = u.Supplier,
                                                                   StagingName = u.StagingName,
                                                                   Comment = u.Comment,
                                                                   RequiredDate = u.RequiredDate,
                                                                   OrderStatus = u.OrderStatus,
                                                                   OrderStatusText = ResOrder.GetOrderStatusText(((eTurns.DTO.OrderStatus)u.OrderStatus).ToString()),
                                                                   RejectionReason = u.RejectionReason,
                                                                   CustomerID = u.CustomerID,
                                                                   CustomerGUID = u.CustomerGUID,
                                                                   PackSlipNumber = u.PackSlipNumber,
                                                                   ShippingTrackNumber = u.ShippingTrackNumber,
                                                                   Created = u.Created,
                                                                   LastUpdated = u.LastUpdated,
                                                                   CreatedBy = u.CreatedBy,
                                                                   LastUpdatedBy = u.LastUpdatedBy,
                                                                   Room = u.Room,
                                                                   IsDeleted = u.IsDeleted,
                                                                   IsArchived = u.IsArchived,
                                                                   CompanyID = u.CompanyID,
                                                                   GUID = u.GUID,
                                                                   UDF1 = u.UDF1,
                                                                   UDF2 = u.UDF2,
                                                                   UDF3 = u.UDF3,
                                                                   UDF4 = u.UDF4,
                                                                   UDF5 = u.UDF5,
                                                                   //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                                                                   //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                                                                   //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                                                                   CreatedByName = u.CreatedByName,
                                                                   UpdatedByName = u.UpdatedByName,
                                                                   RoomName = u.RoomName,
                                                                   //SupplierName = u.Supplier.GetValueOrDefault(0) > 0 ? new SupplierMasterDAL(base.DataBaseName).GetRecord(u.Supplier.GetValueOrDefault(0), u.Room.GetValueOrDefault(0), u.CompanyID.GetValueOrDefault(0)).SupplierName : "",
                                                                   //ShipViaName = u.ShipVia.GetValueOrDefault(0) > 0 ? new ShipViaDAL(base.DataBaseName).GetRecord(u.ShipVia.GetValueOrDefault(0), u.Room.GetValueOrDefault(0), u.CompanyID.GetValueOrDefault(0)).ShipVia : "",
                                                                   SupplierName = u.SupplierName,
                                                                   ShipViaName = u.ShipViaName,
                                                                   CustomerName = u.CustomerName,
                                                                   WhatWhereAction = u.WhatWhereAction,
                                                                   CreatedDate = u.CreatedDate,
                                                                   UpdatedDate = u.UpdatedDate,
                                                                   AddedFrom = u.AddedFrom,
                                                                   EditedFrom = u.EditedFrom,
                                                                   ReceivedOn = u.ReceivedOn,
                                                                   ReceivedOnWeb = u.ReceivedOnWeb,
                                                                   TotalRecords = u.TotalRecords
                                                               }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstOrderMaster != null && lstOrderMaster.Count > 0)
                        {
                            TotalCount = lstOrderMaster.FirstOrDefault().TotalRecords;
                        }

                        return lstOrderMaster;

                    #region Old Code
                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM OrderMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<OrderMasterDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,ISNULL(S.SupplierName,'') AS SupplierName,ISNULL(SH.ShipVia,'') AS ShipViaName,ISNULL(CM.Customer,'') AS CustomerName,ISNULL(BM.StagingName,'') AS StagingName  ,'' AS CreatedDate,'' AS UpdatedDate                            
                    //        FROM  OrderMaster_History as A 
                    //        LEFT OUTER JOIN SupplierMaster S on A.Supplier= S.ID 
                    //        LEFT OUTER JOIN ShipViaMaster SH on A.ShipVia= SH.ID 
                    //        LEFT OUTER JOIN CustomerMaster CM on A.CustomerGUID= CM.GUID 
                    //        LEFT OUTER JOIN MaterialStaging BM on A.MaterialStagingGUID = BM.GUID
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.Room = D.ID                                 
                    //        WHERE A.ID = " + ID + " order by A.HistoryID desc")
                    //        select new OrderMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            OrderNumber = u.OrderNumber,
                    //            ReleaseNumber = u.ReleaseNumber,
                    //            ShipVia = u.ShipVia,
                    //            Supplier = u.Supplier,
                    //            StagingName = u.StagingName,
                    //            Comment = u.Comment,
                    //            RequiredDate = u.RequiredDate,
                    //            OrderStatus = u.OrderStatus,
                    //            OrderStatusText = ResOrder.GetOrderStatusText(((eTurns.DTO.OrderStatus)u.OrderStatus).ToString()),
                    //            RejectionReason = u.RejectionReason,
                    //            CustomerID = u.CustomerID,
                    //            CustomerGUID = u.CustomerGUID,
                    //            PackSlipNumber = u.PackSlipNumber,
                    //            ShippingTrackNumber = u.ShippingTrackNumber,
                    //            Created = u.Created,
                    //            LastUpdated = u.LastUpdated,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            Room = u.Room,
                    //            IsDeleted = u.IsDeleted,
                    //            IsArchived = u.IsArchived,
                    //            CompanyID = u.CompanyID,
                    //            GUID = u.GUID,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                    //            //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                    //            //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            //SupplierName = u.Supplier.GetValueOrDefault(0) > 0 ? new SupplierMasterDAL(base.DataBaseName).GetRecord(u.Supplier.GetValueOrDefault(0), u.Room.GetValueOrDefault(0), u.CompanyID.GetValueOrDefault(0)).SupplierName : "",
                    //            //ShipViaName = u.ShipVia.GetValueOrDefault(0) > 0 ? new ShipViaDAL(base.DataBaseName).GetRecord(u.ShipVia.GetValueOrDefault(0), u.Room.GetValueOrDefault(0), u.CompanyID.GetValueOrDefault(0)).ShipVia : "",
                    //            SupplierName = u.SupplierName,
                    //            ShipViaName = u.ShipViaName,
                    //            CustomerName = u.CustomerName,
                    //            WhatWhereAction = u.WhatWhereAction,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList(); 
                    #endregion
                    case "cartlist":
                        TotalCount = 0;
                        return new List<CartItemDTO>();
                    // Below code is commented bcause this code is not being called.

                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM CartItem_history WHERE ID = " + ID + "").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<CartItemDTO>(@"SELECT cih.*,im.ItemNumber,um_cr.UserName as CreatedByName,um_up.UserName as UpdatedByName,'' AS CreatedDate,'' AS UpdatedDate  FROM CartItem_history cih inner join ItemMaster as im on cih.ItemGUID = im.GUID  left join UserMaster as um_cr on cih.CreatedBy = um_cr.ID left join UserMaster as um_up on cih.LastUpdatedBy = um_up.ID WHERE cih.ID = " + ID + " order by cih.HistoryID desc")
                    //        select new CartItemDTO
                    //        {
                    //            ID = u.ID,

                    //            ItemNumber = u.ItemNumber,
                    //            ItemGUID = u.ItemGUID,
                    //            Quantity = u.Quantity,
                    //            Status = u.Status,
                    //            ReplenishType = u.ReplenishType,
                    //            IsKitComponent = u.IsKitComponent,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            GUID = u.GUID,
                    //            Created = u.Created,
                    //            Updated = u.Updated,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            IsDeleted = u.IsDeleted,
                    //            IsArchived = u.IsArchived,
                    //            CompanyID = u.CompanyID,
                    //            Room = u.Room,
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb
                    //        }).Skip(StartRowIndex).Take(MaxRows).ToList();
                    case "KitList":
                        TotalCount = 0;
                        return new List<KitMasterDTO>();
                    // Below code is commented bcause this code is not being called.

                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM KitMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<KitMasterDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName ,'' AS CreatedDate,'' AS UpdatedDate                               
                    //        FROM  KitMaster_History as A 
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.Room = D.ID                                 
                    //        WHERE A.ID = " + ID + " order by A.HistoryID desc")
                    //        select new KitMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            KitPartNumber = u.KitPartNumber,
                    //            Description = u.Description,
                    //            ReOrderType = u.ReOrderType,
                    //            KitCategory = u.KitCategory,
                    //            AvailableKitQuantity = u.AvailableKitQuantity,
                    //            AvailableWIPKit = u.AvailableWIPKit,
                    //            KitDemand = u.KitDemand,
                    //            KitCost = u.KitCost,
                    //            KitSellPrice = u.KitSellPrice,
                    //            MinimumKitQuantity = u.MinimumKitQuantity,
                    //            MaximumKitQuantity = u.MaximumKitQuantity,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            GUID = u.GUID,
                    //            Created = u.Created,
                    //            Updated = u.Updated,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            IsDeleted = u.IsDeleted,
                    //            IsArchived = u.IsArchived,
                    //            CompanyID = u.CompanyID,
                    //            Room = u.Room,
                    //            //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                    //            //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                    //            //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb

                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
                    case "VenderMaster":
                        var paramVender = new SqlParameter[] { new SqlParameter("@ID", ID) };
                        List<VenderMasterDTO> lstVenderMaster = (from u in context.Database.SqlQuery<VenderMasterDTO>("exec [GetVenderMasterHistory] @ID", paramVender)
                                                                 select new VenderMasterDTO
                                                                 {
                                                                     ID = u.ID,
                                                                     HistoryID = u.HistoryID,
                                                                     Action = u.Action,
                                                                     Vender = u.Vender,
                                                                     Created = u.Created,
                                                                     Updated = u.Updated,
                                                                     //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                                                                     //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                                                                     //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                                                                     CreatedByName = u.CreatedByName,
                                                                     UpdatedByName = u.UpdatedByName,
                                                                     RoomName = u.RoomName,
                                                                     CreatedBy = u.CreatedBy,
                                                                     LastUpdatedBy = u.LastUpdatedBy,
                                                                     CompanyID = u.CompanyID,
                                                                     IsArchived = u.IsArchived,
                                                                     IsDeleted = u.IsDeleted,
                                                                     UDF1 = u.UDF1,
                                                                     UDF2 = u.UDF2,
                                                                     UDF3 = u.UDF3,
                                                                     UDF4 = u.UDF4,
                                                                     UDF5 = u.UDF5,
                                                                     CreatedDate = u.CreatedDate,
                                                                     UpdatedDate = u.UpdatedDate,
                                                                     TotalRecords = u.TotalRecords
                                                                 }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstVenderMaster != null && lstVenderMaster.Count > 0)
                        {
                            TotalCount = lstVenderMaster.FirstOrDefault().TotalRecords;
                        }

                        return lstVenderMaster;
                    #region Old Code
                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM VenderMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<VenderMasterDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName ,'' AS CreatedDate,'' AS UpdatedDate                               
                    //        FROM  VenderMaster_History as A 
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.Room = D.ID                                 
                    //        WHERE A.ID = " + ID + " order by A.HistoryID desc")
                    //        select new VenderMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            Vender = u.Vender,
                    //            Created = u.Created,
                    //            Updated = u.Updated,
                    //            //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                    //            //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                    //            //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            CompanyID = u.CompanyID,
                    //            IsArchived = u.IsArchived,
                    //            IsDeleted = u.IsDeleted,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList(); 
                    #endregion
                    case "RequisitionMasterList":
                        TotalCount = 0;
                        return new List<RequisitionMasterDTO>();
                    // Below code is commented bcause this code is not being called.

                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM RequisitionMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<RequisitionMasterDTO>(@"
                    //       SELECT A.*, B.UserName AS 'CreatedByName', CM.Customer,
                    //       C.UserName AS UpdatedByName, D.RoomName ,'' AS CreatedDate,'' AS UpdatedDate FROM RequisitionMaster_History A 
                    //    LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //    LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //    LEFT OUTER JOIN Room D on A.Room = D.ID 
                    //    LEFT OUTER JOIN CustomerMaster CM on A.CustomerGUID = CM.GUID 
                    //    WHERE A.ID = " + ID + " order by HistoryID desc")
                    //        select new RequisitionMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            RequisitionNumber = u.RequisitionNumber,
                    //            Description = u.Description,

                    //            RequiredDate = u.RequiredDate,
                    //            NumberofItemsrequisitioned = u.NumberofItemsrequisitioned,
                    //            TotalCost = u.TotalCost,
                    //            TotalSellPrice = u.TotalSellPrice,
                    //            CustomerID = u.CustomerID,
                    //            CustomerGUID = u.CustomerGUID,
                    //            Customer = u.Customer,
                    //            ProjectSpendGUID = u.ProjectSpendGUID,
                    //            RequisitionStatus = u.RequisitionStatus,
                    //            RequisitionType = u.RequisitionType,
                    //            BillingAccountID = u.BillingAccountID,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            GUID = u.GUID,
                    //            Created = u.Created,
                    //            Updated = u.Updated,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                    //            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                    //            CompanyID = u.CompanyID,
                    //            Room = u.Room,
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
                    case "WOMasterList":
                        TotalCount = 0;
                        return new List<WorkOrderDTO>();
                    // Below code is commented bcause this code is not being called.

                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM WorkOrder_History WHERE ID = " + ID + "").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<WorkOrderDTO>(@"
                    //                SELECT 
                    //                A.*, B.UserName AS 'CreatedByName',C.UserName AS UpdatedByName, D.RoomName ,'' AS CreatedDate,'' AS UpdatedDate                                               
                    //                FROM WorkOrder_History A
                    //                LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //                LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //                LEFT OUTER JOIN Room D on A.Room = D.ID 
                    //                WHERE A.ID = " + ID + " order by A.HistoryID desc"
                    //                )
                    //        select new WorkOrderDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            WOName = u.WOName,
                    //            SubJob = u.SubJob,
                    //            TechnicianID = u.TechnicianID,
                    //            Technician = u.Technician,
                    //            CustomerID = u.CustomerID,
                    //            CustomerGUID = u.CustomerGUID,
                    //            Customer = u.Customer,
                    //            AssetGUID = u.AssetGUID,
                    //            AssetName = u.AssetName,
                    //            //Asset = u.AssetName,
                    //            ToolGUID = u.ToolGUID,
                    //            ToolName = u.ToolName,
                    //            //Tool = u.ToolName,
                    //            Odometer_OperationHours = u.Odometer_OperationHours.GetValueOrDefault(0),
                    //            UsedItems = u.UsedItems.GetValueOrDefault(0),
                    //            UsedItemsCost = u.UsedItemsCost.GetValueOrDefault(0),
                    //            JobTypeID = u.JobTypeID,
                    //            GXPRConsigmentJobID = u.GXPRConsigmentJobID,
                    //            WOType = u.WOType,
                    //            WOStatus = u.WOStatus,
                    //            Created = u.Created,
                    //            Updated = u.Updated,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            Room = u.Room,
                    //            IsDeleted = u.IsDeleted,
                    //            IsArchived = u.IsArchived,
                    //            CompanyID = u.CompanyID,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            UsedItemsSellPrice = u.UsedItemsSellPrice
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
                    case "TransferList":
                        var paramTransfer = new SqlParameter[] { new SqlParameter("@ID", ID) };
                        List<TransferMasterDTO> lstTransfer = (from u in context.Database.SqlQuery<TransferMasterDTO>("exec [GetTransferMasterHistory] @ID", paramTransfer)
                                                               select new TransferMasterDTO
                                                               {
                                                                   ID = u.ID,
                                                                   HistoryID = u.HistoryID,
                                                                   Action = u.Action,
                                                                   TransferNumber = u.TransferNumber,
                                                                   Comment = u.Comment,
                                                                   RequireDate = u.RequireDate,
                                                                   TransferStatus = u.TransferStatus,
                                                                   RequestTypeName = Enum.Parse(typeof(RequestType), u.RequestType.ToString()).ToString(),
                                                                   //ReplenishingRoomName = new RoomDAL(base.DataBaseName).GetRecord(u.ReplenishingRoomID, u.CompanyID, false, false).RoomName.ToString(),//need to check the false,false values passed here
                                                                   ReplenishingRoomName = u.ReplenishingRoomName,
                                                                   TransferStatusName = ResTransfer.GetTransferStatusText(((eTurns.DTO.TransferStatus)u.TransferStatus).ToString()),
                                                                   Created = u.Created,
                                                                   Updated = u.Updated,
                                                                   CreatedBy = u.CreatedBy,
                                                                   LastUpdatedBy = u.LastUpdatedBy,
                                                                   RoomID = u.RoomID,
                                                                   IsDeleted = u.IsDeleted,
                                                                   IsArchived = u.IsArchived,
                                                                   CompanyID = u.CompanyID,
                                                                   GUID = u.GUID,
                                                                   UDF1 = u.UDF1,
                                                                   UDF2 = u.UDF2,
                                                                   UDF3 = u.UDF3,
                                                                   UDF4 = u.UDF4,
                                                                   UDF5 = u.UDF5,
                                                                   CreatedByName = u.CreatedByName,
                                                                   UpdatedByName = u.UpdatedByName,
                                                                   RoomName = u.RoomName,
                                                                   StagingName = u.StagingName,
                                                                   WhatWhereAction = u.WhatWhereAction,
                                                                   CreatedDate = u.CreatedDate,
                                                                   UpdatedDate = u.UpdatedDate,
                                                                   AddedFrom = u.AddedFrom,
                                                                   EditedFrom = u.EditedFrom,
                                                                   ReceivedOn = u.ReceivedOn,
                                                                   ReceivedOnWeb = u.ReceivedOnWeb,
                                                                   TotalRecords = u.TotalRecords
                                                               }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstTransfer != null && lstTransfer.Count > 0)
                        {
                            TotalCount = lstTransfer.FirstOrDefault().TotalRecords;
                        }

                        return lstTransfer;

                    #region Old Code
                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM TransferMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<TransferMasterDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, RPR.RoomName AS ReplenishingRoomName,
                    //               '' AS CreatedDate,'' AS UpdatedDate ,MS.StagingName
                    //        FROM  TransferMaster_History as A 
                    //        LEFT OUTER JOIN Room RPR ON A.ReplenishingRoomID = RPR.ID
                    //        LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.RoomID = D.ID           
                    //        LEFT OUTER JOIN MaterialStaging MS ON (MS.ID= A.StagingID OR MS.Guid=A.MaterialStagingGUID)                      
                    //        WHERE A.ID = " + ID + " order by A.HistoryID desc")
                    //        select new TransferMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            TransferNumber = u.TransferNumber,
                    //            Comment = u.Comment,
                    //            RequireDate = u.RequireDate,
                    //            TransferStatus = u.TransferStatus,
                    //            RequestTypeName = Enum.Parse(typeof(RequestType), u.RequestType.ToString()).ToString(),
                    //            //ReplenishingRoomName = new RoomDAL(base.DataBaseName).GetRecord(u.ReplenishingRoomID, u.CompanyID, false, false).RoomName.ToString(),//need to check the false,false values passed here
                    //            ReplenishingRoomName = u.ReplenishingRoomName,
                    //            TransferStatusName = ResTransfer.GetTransferStatusText(((eTurns.DTO.TransferStatus)u.TransferStatus).ToString()),
                    //            Created = u.Created,
                    //            Updated = u.Updated,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            RoomID = u.RoomID,
                    //            IsDeleted = u.IsDeleted,
                    //            IsArchived = u.IsArchived,
                    //            CompanyID = u.CompanyID,
                    //            GUID = u.GUID,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            StagingName = u.StagingName,
                    //            WhatWhereAction = u.WhatWhereAction,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb,
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList(); 
                    #endregion
                    case "AssetList":
                        var paramA = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@dbName", DataBaseName) };
                        List<AssetMasterDTO> lstassets = (from u in context.Database.SqlQuery<AssetMasterDTO>("exec [GetAssetMasterChangeLog] @ID,@dbName", paramA)
                                                          select new AssetMasterDTO
                                                          {
                                                              ID = u.ID,
                                                              HistoryID = u.HistoryID,
                                                              Action = u.Action,
                                                              AssetName = u.AssetName,
                                                              Description = u.Description,
                                                              Make = u.Make,
                                                              Model = u.Model,
                                                              Serial = u.Serial,
                                                              ToolCategoryID = u.ToolCategoryID,
                                                              ToolCategory = u.ToolCategory,
                                                              PurchaseDate = u.PurchaseDate,
                                                              PurchasePrice = u.PurchasePrice,
                                                              DepreciatedValue = u.DepreciatedValue,
                                                              SuggestedMaintenanceDate = u.SuggestedMaintenanceDate,
                                                              Created = u.Created,
                                                              CreatedBy = u.CreatedBy,
                                                              Updated = u.Updated,
                                                              LastUpdatedBy = u.LastUpdatedBy,
                                                              Room = u.Room,
                                                              IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                              IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                              GUID = u.GUID,
                                                              CompanyID = u.CompanyID,
                                                              UDF1 = u.UDF1,
                                                              UDF2 = u.UDF2,
                                                              UDF3 = u.UDF3,
                                                              UDF4 = u.UDF4,
                                                              UDF5 = u.UDF5,
                                                              CreatedByName = u.CreatedByName,
                                                              UpdatedByName = u.UpdatedByName,
                                                              RoomName = u.RoomName,
                                                              CreatedDate = u.CreatedDate,
                                                              UpdatedDate = u.UpdatedDate,
                                                              AddedFrom = u.AddedFrom,
                                                              EditedFrom = u.EditedFrom,
                                                              ReceivedOn = u.ReceivedOn,
                                                              ReceivedOnWeb = u.ReceivedOnWeb,
                                                              TotalRecords = u.TotalRecords
                                                          }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstassets != null && lstassets.Count > 0)
                        {
                            TotalCount = lstassets.FirstOrDefault().TotalRecords ?? 0;
                        }

                        return lstassets;
                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM AssetMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<AssetMasterDTO>(@"SELECT A.*, TC.ToolCategory, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,'' AS CreatedDate,'' AS UpdatedDate FROM AssetMaster_History A 
                    //LEFT OUTER  JOIN ToolCategoryMaster TC on A.ToolCategoryID = TC.ID  
                    //LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.ID = " + ID + " order by HistoryID desc")
                    //        select new AssetMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            AssetName = u.AssetName,
                    //            Description = u.Description,
                    //            Make = u.Make,
                    //            Model = u.Model,
                    //            Serial = u.Serial,
                    //            ToolCategoryID = u.ToolCategoryID,
                    //            ToolCategory = u.ToolCategory,
                    //            PurchaseDate = u.PurchaseDate,
                    //            PurchasePrice = u.PurchasePrice,
                    //            DepreciatedValue = u.DepreciatedValue,
                    //            SuggestedMaintenanceDate = u.SuggestedMaintenanceDate,
                    //            Created = u.Created,
                    //            CreatedBy = u.CreatedBy,
                    //            Updated = u.Updated,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            Room = u.Room,
                    //            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                    //            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                    //            GUID = u.GUID,
                    //            CompanyID = u.CompanyID,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
                    case "CostUOMList":
                        var paramCst = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@dbName", DataBaseName) };
                        List<CostUOMMasterDTO> lstCst = (from u in context.Database.SqlQuery<CostUOMMasterDTO>("exec [GetCostUOMMasterChangeLog] @ID,@dbName", paramCst)
                                                         select new CostUOMMasterDTO
                                                         {
                                                             ID = u.ID,
                                                             HistoryID = u.HistoryID,
                                                             Action = u.Action,
                                                             CostUOM = u.CostUOM,
                                                             CostUOMValue = u.CostUOMValue,
                                                             Created = u.Created,
                                                             Updated = u.Updated,
                                                             //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                                                             //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                                                             //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                                                             CreatedByName = u.CreatedByName,
                                                             UpdatedByName = u.UpdatedByName,
                                                             RoomName = u.RoomName,
                                                             CreatedBy = u.CreatedBy,
                                                             LastUpdatedBy = u.LastUpdatedBy,
                                                             CompanyID = u.CompanyID,
                                                             IsArchived = u.IsArchived,
                                                             IsDeleted = u.IsDeleted,
                                                             GUID = u.GUID,
                                                             UDF1 = u.UDF1,
                                                             UDF2 = u.UDF2,
                                                             UDF3 = u.UDF3,
                                                             UDF4 = u.UDF4,
                                                             UDF5 = u.UDF5,
                                                             CreatedDate = u.CreatedDate,
                                                             UpdatedDate = u.UpdatedDate,
                                                             AddedFrom = u.AddedFrom,
                                                             EditedFrom = u.EditedFrom,
                                                             ReceivedOn = u.ReceivedOn,
                                                             ReceivedOnWeb = u.ReceivedOnWeb,
                                                             TotalRecords = u.TotalRecords,
                                                             isForBOM = u.isForBOM,
                                                         }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstCst != null && lstCst.Count > 0)
                        {
                            TotalCount = lstCst.FirstOrDefault().TotalRecords ?? 0;
                        }

                        return lstCst;
                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM CostUOMMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<CostUOMMasterDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName ,'' AS CreatedDate,'' AS UpdatedDate                         
                    //        FROM CostUOMMaster_History as A 
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.Room = D.ID                                 
                    //        WHERE A.ID = " + ID + " order by A.HistoryID desc")
                    //        select new CostUOMMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            CostUOM = u.CostUOM,
                    //            CostUOMValue = u.CostUOMValue,
                    //            Created = u.Created,
                    //            Updated = u.Updated,
                    //            //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                    //            //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                    //            //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            CompanyID = u.CompanyID,
                    //            IsArchived = u.IsArchived,
                    //            IsDeleted = u.IsDeleted,
                    //            GUID = u.GUID,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
                    case "InventoryClassificationList":
                        var paramIC = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@dbName", DataBaseName) };
                        List<InventoryClassificationMasterDTO> lstICst = (from u in context.Database.SqlQuery<InventoryClassificationMasterDTO>("exec [GetInventoryClassificationMasterChangeLog] @ID,@dbName", paramIC)
                                                                          select new InventoryClassificationMasterDTO
                                                                          {
                                                                              ID = u.ID,
                                                                              HistoryID = u.HistoryID,
                                                                              Action = u.Action,
                                                                              InventoryClassification = u.InventoryClassification,
                                                                              BaseOfInventory = u.BaseOfInventory,
                                                                              RangeStart = u.RangeStart,
                                                                              RangeEnd = u.RangeEnd,
                                                                              Created = u.Created,
                                                                              Updated = u.Updated,
                                                                              //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                                                                              //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                                                                              //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                                                                              CreatedByName = u.CreatedByName,
                                                                              UpdatedByName = u.UpdatedByName,
                                                                              RoomName = u.RoomName,
                                                                              CreatedBy = u.CreatedBy,
                                                                              LastUpdatedBy = u.LastUpdatedBy,
                                                                              CompanyID = u.CompanyID,
                                                                              IsArchived = u.IsArchived,
                                                                              IsDeleted = u.IsDeleted,
                                                                              GUID = u.GUID,
                                                                              UDF1 = u.UDF1,
                                                                              UDF2 = u.UDF2,
                                                                              UDF3 = u.UDF3,
                                                                              UDF4 = u.UDF4,
                                                                              UDF5 = u.UDF5,
                                                                              CreatedDate = u.CreatedDate,
                                                                              UpdatedDate = u.UpdatedDate,
                                                                              AddedFrom = u.AddedFrom,
                                                                              EditedFrom = u.EditedFrom,
                                                                              ReceivedOn = u.ReceivedOn,
                                                                              ReceivedOnWeb = u.ReceivedOnWeb,
                                                                              TotalRecords = u.TotalRecords
                                                                          }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstICst != null && lstICst.Count > 0)
                        {
                            TotalCount = lstICst.FirstOrDefault().TotalRecords;
                        }

                        return lstICst;
                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM InventoryClassificationMaster_History WHERE ID = " + ID + "").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<InventoryClassificationMasterDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName ,'' AS CreatedDate,'' AS UpdatedDate                               
                    //        FROM InventoryClassificationMaster_History as A 
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.Room = D.ID                                 
                    //        WHERE A.ID = " + ID + " order by A.HistoryID desc")
                    //        select new InventoryClassificationMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            InventoryClassification = u.InventoryClassification,
                    //            BaseOfInventory = u.BaseOfInventory,
                    //            RangeStart = u.RangeStart,
                    //            RangeEnd = u.RangeEnd,
                    //            Created = u.Created,
                    //            Updated = u.Updated,
                    //            //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                    //            //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                    //            //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            CompanyID = u.CompanyID,
                    //            IsArchived = u.IsArchived,
                    //            IsDeleted = u.IsDeleted,
                    //            GUID = u.GUID,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
                    case "PermissionTemplateList":
                        var paramPT = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@dbName", DataBaseName) };
                        List<PermissionTemplateDTO> lstPTHistory = (from u in context.Database.SqlQuery<PermissionTemplateDTO>("exec [GetPermissionTemplateMasterChangeLog] @ID,@dbName", paramPT)
                                                                          select new PermissionTemplateDTO
                                                                          {
                                                                              ID = u.ID,
                                                                              HistoryID = u.HistoryID,
                                                                              Action = u.Action,
                                                                              TemplateName = u.TemplateName,
                                                                              Description = u.Description,
                                                                              Created = u.Created,
                                                                              Updated = u.Updated,
                                                                              //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                                                                              //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                                                                              //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                                                                              CreatedByName = u.CreatedByName,
                                                                              UpdatedByName = u.UpdatedByName,
                                                                        
                                                                              //CreatedBy = u.CreatedBy,
                                                                              IsDeleted = u.IsDeleted,
                                                                              CreatedDate = u.CreatedDate,
                                                                              UpdatedDate = u.UpdatedDate,
                                                                              TotalRecords = u.TotalRecords
                                                                          }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstPTHistory != null && lstPTHistory.Count > 0)
                        {
                            TotalCount = lstPTHistory.FirstOrDefault().TotalRecords;
                        }

                        return lstPTHistory;
                    default:
                        return null;
                }
            }
        }

        public IEnumerable<object> GetPagedRecordsGUID(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string TableName, string GUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                TotalCount = 0;

                switch (TableName)
                {
                    case "QuickList":
                        var paramQuicklist = new SqlParameter[] { new SqlParameter("@GUID", GUID) };
                        UserMasterDAL UMDAL = new UserMasterDAL(base.DataBaseName);
                        List<QuickListMasterDTO> lstQuicklist = (from u in context.Database.SqlQuery<QuickListMasterDTO>("exec [GetQuickListMasterHistoryByGUID] @GUID", paramQuicklist)
                                                                 select new QuickListMasterDTO
                                                                 {
                                                                     ID = u.ID,
                                                                     HistoryID = u.HistoryID,
                                                                     Action = u.Action,
                                                                     Name = u.Name,
                                                                     Comment = u.Comment,
                                                                     CompanyID = u.CompanyID,
                                                                     IsDeleted = u.IsDeleted.GetValueOrDefault(false),
                                                                     IsArchived = u.IsArchived.GetValueOrDefault(false),
                                                                     Type = u.Type,
                                                                     Created = u.Created,
                                                                     LastUpdated = u.LastUpdated,
                                                                     //CreatedByName = UMDAL.GetRecord(Convert.ToInt64(u.CreatedBy)).UserName,
                                                                     //UpdatedByName = UMDAL.GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName,
                                                                     //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), false, false).RoomName.ToString(),
                                                                     CreatedByName = u.CreatedByName,
                                                                     UpdatedByName = u.UpdatedByName,
                                                                     RoomName = u.RoomName,
                                                                     CreatedBy = u.CreatedBy,
                                                                     LastUpdatedBy = u.LastUpdatedBy,
                                                                     IsHistory = true,
                                                                     Room = u.Room,
                                                                     GUID = u.GUID,
                                                                     UDF1 = u.UDF1,
                                                                     UDF2 = u.UDF2,
                                                                     UDF3 = u.UDF3,
                                                                     UDF4 = u.UDF4,
                                                                     UDF5 = u.UDF5,
                                                                     NoOfItems = u.NoOfItems,
                                                                     WhatWhereAction = u.WhatWhereAction,
                                                                     CreatedDate = u.CreatedDate,
                                                                     UpdatedDate = u.UpdatedDate,
                                                                     ReceivedOn = u.ReceivedOn,
                                                                     ReceivedOnWeb = u.ReceivedOnWeb,
                                                                     AddedFrom = u.AddedFrom,
                                                                     EditedFrom = u.EditedFrom,
                                                                     TotalRecords = u.TotalRecords,
                                                                 }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstQuicklist != null && lstQuicklist.Count > 0)
                        {
                            TotalCount = lstQuicklist.FirstOrDefault().TotalRecords;
                        }

                        return lstQuicklist;

                    #region Old Code
                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM QuickListMaster_History WHERE GUID = '" + GUID + "'").ToList()[0]);
                    //UserMasterDAL UMDAL = new UserMasterDAL(base.DataBaseName);
                    //return (from u in context.Database.SqlQuery<QuickListMasterDTO>(@"
                    //    SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,'' AS CreatedDate,'' As UpdatedDAte,                                
                    //    (SELECT Count(ID) From QuickListItems_History E WHERE A.GUID= E.QuickListGUID) as NoOfItems 
                    //    FROM  QuickListMaster_History as A 
                    //    LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //    LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //    LEFT OUTER JOIN Room D on A.Room = D.ID                                 
                    //    WHERE A.GUID = '" + GUID + "' order by A.HistoryID desc")
                    //        select new QuickListMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            Name = u.Name,
                    //            Comment = u.Comment,
                    //            CompanyID = u.CompanyID,
                    //            IsDeleted = u.IsDeleted.GetValueOrDefault(false),
                    //            IsArchived = u.IsArchived.GetValueOrDefault(false),
                    //            Type = u.Type,
                    //            Created = u.Created,
                    //            LastUpdated = u.LastUpdated,
                    //            //CreatedByName = UMDAL.GetRecord(Convert.ToInt64(u.CreatedBy)).UserName,
                    //            //UpdatedByName = UMDAL.GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName,
                    //            //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), false, false).RoomName.ToString(),
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            IsHistory = true,
                    //            Room = u.Room,
                    //            GUID = u.GUID,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            NoOfItems = u.NoOfItems,
                    //            WhatWhereAction = u.WhatWhereAction,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList(); 
                    #endregion
                    case "WOMasterList":
                        var paramWO = new SqlParameter[] { new SqlParameter("@GUID", GUID) };
                        List<WorkOrderDTO> lstWO = (from u in context.Database.SqlQuery<WorkOrderDTO>("exec [GetWorkOrderHistoryByGUID] @GUID", paramWO)
                                                    select new WorkOrderDTO
                                                    {
                                                        ID = u.ID,
                                                        HistoryID = u.HistoryID,
                                                        Action = u.Action,
                                                        WOName = u.WOName,
                                                        SubJob = u.SubJob,
                                                        TechnicianID = u.TechnicianID,
                                                        Technician = u.Technician,
                                                        CustomerID = u.CustomerID,
                                                        CustomerGUID = u.CustomerGUID,
                                                        Customer = u.Customer,
                                                        AssetGUID = u.AssetGUID,
                                                        AssetName = u.AssetName,
                                                        //Asset = u.AssetName,
                                                        ToolGUID = u.ToolGUID,
                                                        ToolName = u.ToolName,
                                                        Odometer_OperationHours = u.Odometer_OperationHours.GetValueOrDefault(0),
                                                        UsedItems = u.UsedItems.GetValueOrDefault(0),
                                                        UsedItemsCost = u.UsedItemsCost.GetValueOrDefault(0),
                                                        JobTypeID = u.JobTypeID,
                                                        GXPRConsigmentJobID = u.GXPRConsigmentJobID,
                                                        WOType = u.WOType,
                                                        WOStatus = u.WOStatus,
                                                        Created = u.Created,
                                                        Updated = u.Updated,
                                                        CreatedBy = u.CreatedBy,
                                                        LastUpdatedBy = u.LastUpdatedBy,
                                                        Room = u.Room,
                                                        IsDeleted = u.IsDeleted,
                                                        IsArchived = u.IsArchived,
                                                        CompanyID = u.CompanyID,
                                                        UDF1 = u.UDF1,
                                                        UDF2 = u.UDF2,
                                                        UDF3 = u.UDF3,
                                                        UDF4 = u.UDF4,
                                                        UDF5 = u.UDF5,
                                                        CreatedByName = u.CreatedByName,
                                                        UpdatedByName = u.UpdatedByName,
                                                        RoomName = u.RoomName,
                                                        WhatWhereAction = u.WhatWhereAction,
                                                        Description = u.Description,
                                                        CreatedDate = u.CreatedDate,
                                                        UpdatedDate = u.UpdatedDate,
                                                        ReceivedOn = u.ReceivedOn,
                                                        ReceivedOnWeb = u.ReceivedOnWeb,
                                                        AddedFrom = u.AddedFrom,
                                                        EditedFrom = u.EditedFrom,
                                                        ReleaseNumber = u.ReleaseNumber,
                                                        UsedItemsSellPrice = u.UsedItemsSellPrice,
                                                        TotalRecords = u.TotalRecords
                                                    }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstWO != null && lstWO.Count > 0)
                        {
                            TotalCount = lstWO.FirstOrDefault().TotalRecords ?? 0;
                        }

                        return lstWO;

                    #region Old Code
                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM WorkOrder_History WHERE GUID = '" + GUID + "'").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<WorkOrderDTO>(@"
                    //                SELECT 
                    //                A.HistoryID,A.Action,A.ID,A.GUID,A.WOName,A.TechnicianID,A.Technician,A.CustomerID,A.Customer,
                    //                A.AssetGUID,A.ToolGUID,A.WOStatus,A.Odometer_OperationHours,A.Created,
                    //                A.Updated,A.CreatedBy,A.LastUpdatedBy,A.Room,A.IsDeleted,A.IsArchived,A.CompanyID,
                    //                A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5,A.UsedItems,A.UsedItemsCost,A.UsedItemsSellPrice,A.WOType,A.ReleaseNumber,
                    //                B.UserName AS 'CreatedByName',C.UserName AS UpdatedByName, D.RoomName
                    //                ,T.ToolName,AST.AssetName,A.WhatWhereAction , A.Description,'' AS CreatedDate,'' As UpdatedDAte,A.CustomerGUID
                    //                ,A.[ReceivedOnWeb],A.[ReceivedOn],A.[AddedFrom],A.[EditedFrom]
                    //                FROM WorkOrder_History A
                    //                LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //                LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //                LEFT OUTER JOIN Room D on A.Room = D.ID 
                    //                LEFT OUTER JOIN toolmaster T on A.ToolGUID = T.GUID
                    //                LEFT OUTER JOIN CustomerMaster CM on A.CustomerGUID = CM.GUID 
                    //                LEFT OUTER JOIN AssetMaster AST on A.AssetGUID = AST.GUID
                    //                WHERE A.GUID = '" + GUID + "' order by A.HistoryID desc"
                    //                )
                    //        select new WorkOrderDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            WOName = u.WOName,
                    //            SubJob = u.SubJob,
                    //            TechnicianID = u.TechnicianID,
                    //            Technician = u.Technician,
                    //            CustomerID = u.CustomerID,
                    //            CustomerGUID = u.CustomerGUID,
                    //            Customer = u.Customer,
                    //            AssetGUID = u.AssetGUID,
                    //            AssetName = u.AssetName,
                    //            //Asset = u.AssetName,
                    //            ToolGUID = u.ToolGUID,
                    //            ToolName = u.ToolName,
                    //            Odometer_OperationHours = u.Odometer_OperationHours.GetValueOrDefault(0),
                    //            UsedItems = u.UsedItems.GetValueOrDefault(0),
                    //            UsedItemsCost = u.UsedItemsCost.GetValueOrDefault(0),
                    //            JobTypeID = u.JobTypeID,
                    //            GXPRConsigmentJobID = u.GXPRConsigmentJobID,
                    //            WOType = u.WOType,
                    //            WOStatus = u.WOStatus,
                    //            Created = u.Created,
                    //            Updated = u.Updated,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            Room = u.Room,
                    //            IsDeleted = u.IsDeleted,
                    //            IsArchived = u.IsArchived,
                    //            CompanyID = u.CompanyID,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            WhatWhereAction = u.WhatWhereAction,
                    //            Description = u.Description,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            ReleaseNumber = u.ReleaseNumber,
                    //            UsedItemsSellPrice = u.UsedItemsSellPrice
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList(); 
                    #endregion
                    case "RequisitionMasterList":
                        var paramReq = new SqlParameter[] { new SqlParameter("@GUID", GUID) };
                        List<RequisitionMasterDTO> lstReqlist = (from u in context.Database.SqlQuery<RequisitionMasterDTO>("exec [GetRequisitionMasterHistoryByGUID] @GUID", paramReq)
                                                                 select new RequisitionMasterDTO
                                                                 {
                                                                     ID = u.ID,
                                                                     HistoryID = u.HistoryID,
                                                                     Action = u.Action,
                                                                     RequisitionNumber = u.RequisitionNumber,
                                                                     Description = u.Description,
                                                                     RequiredDate = u.RequiredDate,
                                                                     NumberofItemsrequisitioned = u.NumberofItemsrequisitioned,
                                                                     TotalCost = u.TotalCost,
                                                                     TotalSellPrice = u.TotalSellPrice,
                                                                     CustomerID = u.CustomerID,
                                                                     CustomerGUID = u.CustomerGUID,
                                                                     Customer = u.Customer,
                                                                     ProjectSpendGUID = u.ProjectSpendGUID,
                                                                     RequisitionStatus = u.RequisitionStatus,
                                                                     RequisitionType = u.RequisitionType,
                                                                     BillingAccountID = u.BillingAccountID,
                                                                     UDF1 = u.UDF1,
                                                                     UDF2 = u.UDF2,
                                                                     UDF3 = u.UDF3,
                                                                     UDF4 = u.UDF4,
                                                                     UDF5 = u.UDF5,
                                                                     GUID = u.GUID,
                                                                     Created = u.Created,
                                                                     Updated = u.Updated,
                                                                     CreatedBy = u.CreatedBy,
                                                                     LastUpdatedBy = u.LastUpdatedBy,
                                                                     IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                                     IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                                     CompanyID = u.CompanyID,
                                                                     Room = u.Room,
                                                                     CreatedByName = u.CreatedByName,
                                                                     UpdatedByName = u.UpdatedByName,
                                                                     RoomName = u.RoomName,
                                                                     WhatWhereAction = u.WhatWhereAction,
                                                                     CreatedDate = u.CreatedDate,
                                                                     UpdatedDate = u.UpdatedDate,
                                                                     ReceivedOn = u.ReceivedOn,
                                                                     ReceivedOnWeb = u.ReceivedOnWeb,
                                                                     AddedFrom = u.AddedFrom,
                                                                     EditedFrom = u.EditedFrom,
                                                                     ReleaseNumber = u.ReleaseNumber,
                                                                     TotalRecords = u.TotalRecords
                                                                 }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstReqlist != null && lstReqlist.Count > 0)
                        {
                            TotalCount = lstReqlist.FirstOrDefault().TotalRecords;
                        }

                        return lstReqlist;

                    #region Old Code
                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM RequisitionMaster_History WHERE GUID = '" + GUID + "'").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<RequisitionMasterDTO>(@"
                    //       SELECT A.*, B.UserName AS 'CreatedByName', CM.Customer,
                    //       C.UserName AS UpdatedByName, D.RoomName,'' AS CreatedDate,'' As UpdatedDAte FROM RequisitionMaster_History A 
                    //    LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //    LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //    LEFT OUTER JOIN Room D on A.Room = D.ID 
                    //    LEFT OUTER JOIN CustomerMaster CM on A.CustomerGUID = CM.GUID 
                    //    WHERE A.GUID = '" + GUID + "' order by HistoryID desc")
                    //        select new RequisitionMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            RequisitionNumber = u.RequisitionNumber,
                    //            Description = u.Description,
                    //            RequiredDate = u.RequiredDate,
                    //            NumberofItemsrequisitioned = u.NumberofItemsrequisitioned,
                    //            TotalCost = u.TotalCost,
                    //            TotalSellPrice = u.TotalSellPrice,
                    //            CustomerID = u.CustomerID,
                    //            CustomerGUID = u.CustomerGUID,
                    //            Customer = u.Customer,
                    //            ProjectSpendGUID = u.ProjectSpendGUID,
                    //            RequisitionStatus = u.RequisitionStatus,
                    //            RequisitionType = u.RequisitionType,
                    //            BillingAccountID = u.BillingAccountID,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            GUID = u.GUID,
                    //            Created = u.Created,
                    //            Updated = u.Updated,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                    //            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                    //            CompanyID = u.CompanyID,
                    //            Room = u.Room,
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            WhatWhereAction = u.WhatWhereAction,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            ReleaseNumber = u.ReleaseNumber
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList(); 
                    #endregion
                    case "KitList":
                        TotalCount = 0;
                        return new List<KitMasterDTO>();
                    // Below code is commented bcause this code is not being called.

                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM KitMaster_History WHERE GUID = '" + GUID + "'").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<KitMasterDTO>(@"SELECT *, UC.UserName AS CreatedByName, UU.UserName AS UpdatedByName,R.RoomName AS RoomName,'' AS CreatedDate,'' As UpdatedDAte FROM KitMaster_History A Left outer join UserMaster UC ON A.CreatedBy = UC.ID
                    //                                                      Left outer join UserMaster UU ON A.LastUpdatedBy = UU.ID
                    //                                                      Left outer join Room R ON A.Room = R.ID Where A.GUID = '" + GUID + "' order by HistoryID desc")
                    //        select new KitMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            KitPartNumber = u.KitPartNumber,
                    //            Description = u.Description,
                    //            ReOrderType = u.ReOrderType,
                    //            KitCategory = u.KitCategory,
                    //            AvailableKitQuantity = u.AvailableKitQuantity,
                    //            AvailableWIPKit = u.AvailableWIPKit,
                    //            KitDemand = u.KitDemand,
                    //            KitCost = u.KitCost,
                    //            KitSellPrice = u.KitSellPrice,
                    //            MinimumKitQuantity = u.MinimumKitQuantity,
                    //            MaximumKitQuantity = u.MaximumKitQuantity,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            GUID = u.GUID,
                    //            Created = u.Created,
                    //            Updated = u.Updated,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            IsDeleted = u.IsDeleted,
                    //            IsArchived = u.IsArchived,
                    //            CompanyID = u.CompanyID,
                    //            Room = u.Room,
                    //            CreatedByName = u.CreatedByName,// new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                    //            UpdatedByName = u.UpdatedByName,// new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                    //            RoomName = u.RoomName,/// new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb

                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
                    case "ItemList":
                        TotalCount = 0;
                        return new List<ItemMasterDTO>();
                    // Below code is commented because this code is not being used.

                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM ItemMaster_History WHERE GUID = '" + GUID + "'").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<ItemMasterDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,
                    //        M.Manufacturer as ManufacturerName,S.SupplierName, C1.Category AS CategoryName,U1.Unit,G1.GLAccount,'' AS CreatedDate,'' As UpdatedDAte
                    //        FROM ItemMaster_History A 
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.Room = D.ID 
                    //        LEFT OUTER join ManufacturerMaster M on m.id = A.ManufacturerID 
                    //        left outer join SupplierMaster S on S.id = A.SupplierID 
                    //        LEFT OUTER join CategoryMaster C1 on C1.id = A.CategoryID
                    //        LEFT OUTER join UnitMaster U1 on U1.id = A.UOMID
                    //        LEFT OUTER join GLAccountMaster G1 on G1.id = A.GLAccountID
                    //        WHERE A.GUID = '" + GUID + "' Order by A.HistoryID desc"
                    //        )
                    //        select new ItemMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            ItemNumber = u.ItemNumber,
                    //            ManufacturerID = u.ManufacturerID,
                    //            ManufacturerNumber = u.ManufacturerNumber,
                    //            ManufacturerName = u.ManufacturerName,
                    //            SupplierID = u.SupplierID,
                    //            SupplierPartNo = u.SupplierPartNo,
                    //            SupplierName = u.SupplierName,
                    //            UPC = u.UPC == null ? "" : u.UPC,
                    //            UNSPSC = u.UNSPSC == null ? "" : u.UNSPSC,
                    //            Description = u.Description,
                    //            LongDescription = u.LongDescription,
                    //            CategoryID = u.CategoryID.GetValueOrDefault(0),
                    //            GLAccountID = u.GLAccountID.GetValueOrDefault(0),
                    //            UOMID = u.UOMID,
                    //            PricePerTerm = u.PricePerTerm.GetValueOrDefault(0),
                    //            DefaultReorderQuantity = u.DefaultReorderQuantity,
                    //            DefaultPullQuantity = u.DefaultPullQuantity,
                    //            Cost = u.Cost.GetValueOrDefault(0),
                    //            Markup = u.Markup.GetValueOrDefault(0),
                    //            SellPrice = u.SellPrice.GetValueOrDefault(0),
                    //            ExtendedCost = u.ExtendedCost.GetValueOrDefault(0),
                    //            LeadTimeInDays = u.LeadTimeInDays.GetValueOrDefault(0),
                    //            Trend = u.Trend,
                    //            Taxable = u.Taxable,
                    //            Consignment = u.Consignment,
                    //            StagedQuantity = u.StagedQuantity.GetValueOrDefault(0),
                    //            InTransitquantity = u.InTransitquantity.GetValueOrDefault(0),
                    //            OnOrderQuantity = u.OnOrderQuantity.GetValueOrDefault(0),
                    //            OnReturnQuantity = u.OnReturnQuantity.GetValueOrDefault(0),
                    //            OnTransferQuantity = u.OnTransferQuantity.GetValueOrDefault(0),
                    //            SuggestedOrderQuantity = u.SuggestedOrderQuantity.GetValueOrDefault(0),
                    //            SuggestedTransferQuantity = u.SuggestedTransferQuantity.GetValueOrDefault(0),
                    //            RequisitionedQuantity = u.RequisitionedQuantity.GetValueOrDefault(0),
                    //            AverageUsage = u.AverageUsage.GetValueOrDefault(0),
                    //            Turns = u.Turns.GetValueOrDefault(0),
                    //            OnHandQuantity = u.OnHandQuantity == null ? 0 : u.OnHandQuantity,//u.OnHandQuantity.GetValueOrDefault(0),
                    //            CriticalQuantity = u.CriticalQuantity,
                    //            MinimumQuantity = u.MinimumQuantity,
                    //            MaximumQuantity = u.MaximumQuantity,
                    //            WeightPerPiece = u.WeightPerPiece.GetValueOrDefault(0),
                    //            ItemUniqueNumber = u.ItemUniqueNumber,
                    //            //TransferOrPurchase = u.TransferOrPurchase,
                    //            IsPurchase = u.IsPurchase,
                    //            IsTransfer = u.IsTransfer,
                    //            DefaultLocation = u.DefaultLocation,
                    //            InventoryClassification = u.InventoryClassification,
                    //            SerialNumberTracking = u.SerialNumberTracking,
                    //            LotNumberTracking = u.LotNumberTracking,
                    //            DateCodeTracking = u.DateCodeTracking,
                    //            ItemType = u.ItemType,
                    //            ImagePath = u.ImagePath,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            GUID = u.GUID,
                    //            Created = u.Created,
                    //            Updated = u.Updated,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                    //            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                    //            CompanyID = u.CompanyID,
                    //            Room = u.Room.GetValueOrDefault(0),
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            IsLotSerialExpiryCost = u.IsLotSerialExpiryCost,
                    //            CategoryName = u.CategoryName,
                    //            Unit = u.Unit,
                    //            GLAccount = u.GLAccount,
                    //            ItemTypeName = u.ItemTypeName,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb,
                    //            OnOrderInTransitQuantity = u.OnOrderInTransitQuantity.GetValueOrDefault(0),

                    //        }).AsParallel().ToList();
                    case "MaterialStaging":
                        var paramMS = new SqlParameter[] { new SqlParameter("@GUID", GUID), new SqlParameter("@dbName", DataBaseName) };
                        List<MaterialStagingDTO> lstMS = (from u in context.Database.SqlQuery<MaterialStagingDTO>("exec [GetMaterialStagingChangeLog] @GUID,@dbName", paramMS)
                                                          select new MaterialStagingDTO
                                                          {
                                                              ID = u.ID,
                                                              HinstoryID = u.HinstoryID,
                                                              Action = u.Action,
                                                              StagingName = u.StagingName,
                                                              StagingLocationName = u.StagingLocationName,//new BinMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.BinID),u.Room.GetValueOrDefault(0), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).BinNumber.ToString(),
                                                              Description = u.Description,
                                                              BinID = u.BinID,
                                                              GUID = u.GUID,
                                                              UDF1 = u.UDF1,
                                                              UDF2 = u.UDF2,
                                                              UDF3 = u.UDF3,
                                                              UDF4 = u.UDF4,
                                                              UDF5 = u.UDF5,
                                                              CompanyID = u.CompanyID,
                                                              Room = u.Room,
                                                              IsDeleted = u.IsDeleted,
                                                              IsArchived = u.IsArchived,
                                                              Created = u.Created,
                                                              Updated = u.Updated,
                                                              CreatedBy = u.CreatedBy,
                                                              LastUpdatedBy = u.LastUpdatedBy,
                                                              CreatedByName = u.CreatedByName,
                                                              UpdatedByName = u.UpdatedByName,
                                                              RoomName = u.RoomName,
                                                              WhatWhereAction = u.WhatWhereAction,
                                                              CreatedDate = u.CreatedDate,
                                                              UpdatedDate = u.UpdatedDate,
                                                              BinName = u.StagingLocationName != null ? u.StagingLocationName.Replace("[|EmptyStagingBin|]", "") : string.Empty,
                                                              AddedFrom = u.AddedFrom,
                                                              EditedFrom = u.EditedFrom,
                                                              ReceivedOn = u.ReceivedOn,
                                                              ReceivedOnWeb = u.ReceivedOnWeb,
                                                              TotalRecords = u.TotalRecords

                                                          }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstMS != null && lstMS.Count > 0)
                        {
                            TotalCount = lstMS.FirstOrDefault().TotalRecords ?? 0;
                        }

                        return lstMS;
                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM MaterialStaging_History WHERE GUID = '" + GUID + "'").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<MaterialStagingDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName
                    //        ,L.BinNumber as 'BinName'  ,'' AS CreatedDate,'' As UpdatedDAte
                    //        FROM MaterialStaging_History AS A
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.Room = D.ID
                    //        left outer join BinMaster L on A.BinID = L.ID
                    //        WHERE A.GUID = '" + GUID + "' order by HinstoryID desc")
                    //        select new MaterialStagingDTO
                    //        {
                    //            ID = u.ID,
                    //            HinstoryID = u.HinstoryID,
                    //            Action = u.Action,
                    //            StagingName = u.StagingName,
                    //            StagingLocationName = u.StagingLocationName,//new BinMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.BinID),u.Room.GetValueOrDefault(0), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).BinNumber.ToString(),
                    //            Description = u.Description,
                    //            BinID = u.BinID,
                    //            GUID = u.GUID,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            CompanyID = u.CompanyID,
                    //            Room = u.Room,
                    //            IsDeleted = u.IsDeleted,
                    //            IsArchived = u.IsArchived,
                    //            Created = u.Created,
                    //            Updated = u.Updated,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            WhatWhereAction = u.WhatWhereAction,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            BinName = u.StagingLocationName,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb

                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
                    case "cartlist":
                        var paramCart = new SqlParameter[] { new SqlParameter("@GUID", GUID) };
                        List<CartItemDTO> lstCart = (from u in context.Database.SqlQuery<CartItemDTO>("exec [GetCartHistoryByGUID] @GUID", paramCart)
                                                     select new CartItemDTO
                                                     {
                                                         ID = u.ID,
                                                         ItemNumber = u.ItemNumber,
                                                         ItemGUID = u.ItemGUID,
                                                         Quantity = u.Quantity,
                                                         Status = u.Status,
                                                         ReplenishType = u.ReplenishType,
                                                         IsKitComponent = u.IsKitComponent,
                                                         UDF1 = u.UDF1,
                                                         UDF2 = u.UDF2,
                                                         UDF3 = u.UDF3,
                                                         UDF4 = u.UDF4,
                                                         UDF5 = u.UDF5,
                                                         GUID = u.GUID,
                                                         Created = u.Created,
                                                         Updated = u.Updated,
                                                         CreatedBy = u.CreatedBy,
                                                         LastUpdatedBy = u.LastUpdatedBy,
                                                         IsDeleted = u.IsDeleted,
                                                         IsArchived = u.IsArchived,
                                                         CompanyID = u.CompanyID,
                                                         Room = u.Room,
                                                         CreatedByName = u.CreatedByName,
                                                         UpdatedByName = u.UpdatedByName,
                                                         RoomName = u.RoomName,
                                                         WhatWhereAction = u.WhatWhereAction,
                                                         CreatedDate = u.CreatedDate,
                                                         UpdatedDate = u.UpdatedDate,
                                                         AddedFrom = u.AddedFrom,
                                                         EditedFrom = u.EditedFrom,
                                                         ReceivedOn = u.ReceivedOn,
                                                         ReceivedOnWeb = u.ReceivedOnWeb,
                                                         TotalRecords = u.TotalRecords
                                                     }).Skip(StartRowIndex).Take(MaxRows).ToList();
                        TotalCount = 0;
                        if (lstCart != null && lstCart.Count > 0)
                        {
                            TotalCount = lstCart.FirstOrDefault().TotalRecords ?? 0;
                        }

                        return lstCart;

                    #region Old Code
                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM CartItem_history WHERE GUID = '" + GUID + "'").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<CartItemDTO>(@"SELECT cih.*,im.ItemNumber,um_cr.UserName as CreatedByName,um_up.UserName as UpdatedByName,'' AS CreatedDate,'' As UpdatedDAte  FROM CartItem_history cih inner join ItemMaster as im on cih.ItemGUID = im.GUID  left join UserMaster as um_cr on cih.CreatedBy = um_cr.ID left join UserMaster as um_up on cih.LastUpdatedBy = um_up.ID WHERE cih.GUID = '" + GUID + "' order by cih.HistoryID desc")
                    //        select new CartItemDTO
                    //        {
                    //            ID = u.ID,
                    //            ItemNumber = u.ItemNumber,
                    //            ItemGUID = u.ItemGUID,
                    //            Quantity = u.Quantity,
                    //            Status = u.Status,
                    //            ReplenishType = u.ReplenishType,
                    //            IsKitComponent = u.IsKitComponent,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            GUID = u.GUID,
                    //            Created = u.Created,
                    //            Updated = u.Updated,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            IsDeleted = u.IsDeleted,
                    //            IsArchived = u.IsArchived,
                    //            CompanyID = u.CompanyID,
                    //            Room = u.Room,
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            WhatWhereAction = u.WhatWhereAction,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb
                    //        }).Skip(StartRowIndex).Take(MaxRows).ToList(); 
                    #endregion
                    case "PullList":
                        TotalCount = 0;
                        return new List<PullMasterDTO>();
                    // Below code is commented because this code is not being used.

                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM PullMaster_History WHERE GUID = '" + GUID + "'").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<PullMasterDTO>(@"
                    //        SELECT A.*, 
                    //        B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,P.ProjectSpendName as ProjectName,
                    //        I.ItemNumber,I.DefaultPullQuantity,I.OnHandQuantity,I.CategoryID,I.ManufacturerID,I.SupplierID  ,'' AS CreatedDate,'' As UpdatedDAte
                    //        FROM PullMaster_History AS A 
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.Room = D.ID 
                    //        LEFT OUTER  JOIN ProjectMaster P on P.GUID = A.ProjectSpendGUID 
                    //        LEFT OUTER  JOIN ItemMaster I on I.GUID = A.ItemGUID
                    //        WHERE A.GUID = '" + GUID + "' order by HistoryID desc")
                    //        select new PullMasterDTO
                    //        {
                    //            ID = u.ID,
                    //            HistoryID = u.HistoryID,
                    //            Action = u.Action,
                    //            ProjectName = u.ProjectName,
                    //            DefaultPullQuantity = u.DefaultPullQuantity,
                    //            OnHandQuantity = u.OnHandQuantity,
                    //            ItemNumber = u.ItemNumber,
                    //            PoolQuantity = u.PoolQuantity,
                    //            SerialNumber = u.SerialNumber,
                    //            LotNumber = u.LotNumber,
                    //            DateCode = u.DateCode,
                    //            BinID = u.BinID,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            GUID = u.GUID,
                    //            Created = u.Created,
                    //            Updated = u.Updated,
                    //            CreatedBy = u.CreatedBy,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            IsDeleted = u.IsDeleted,
                    //            IsArchived = u.IsArchived,
                    //            CompanyID = u.CompanyID,
                    //            Room = u.Room,
                    //            CreatedByName = u.CreatedByName,
                    //            UpdatedByName = u.UpdatedByName,
                    //            RoomName = u.RoomName,
                    //            WhatWhereAction = u.WhatWhereAction,
                    //            PULLCost = u.PULLCost,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            //CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.CreatedBy)).UserName.ToString(),
                    //            //UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.LastUpdatedBy)).UserName.ToString(),
                    //            //RoomName = new RoomDAL(base.DataBaseName).GetRecord(Convert.ToInt64(u.Room), Convert.ToInt64(u.CompanyID), (bool)u.IsArchived, (bool)u.IsDeleted).RoomName.ToString(),
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
                    case "InventoryCount":
                        var paramICnt = new SqlParameter[] { new SqlParameter("@GUID", GUID), new SqlParameter("@dbName", DataBaseName) };
                        List<InventoryCountDTO> lstIcnt = (from u in context.Database.SqlQuery<InventoryCountDTO>("exec [GetInventoryCountChangeLog] @GUID,@dbName", paramICnt)
                                                           select new InventoryCountDTO
                                                           {
                                                               HistoryID = u.HistoryID,
                                                               CompanyId = u.CompanyId,
                                                               CompleteCauseCountGUID = u.CompleteCauseCountGUID,
                                                               CountCompletionDate = u.CountCompletionDate,
                                                               CountDate = u.CountDate,
                                                               CountItemDescription = u.CountItemDescription,
                                                               CountName = u.CountName,
                                                               CountStatus = u.CountStatus,
                                                               CountType = u.CountType,
                                                               Created = u.Created,
                                                               CreatedBy = u.CreatedBy,
                                                               CreatedByName = u.CreatedByName,
                                                               GUID = u.GUID,
                                                               ID = u.ID,
                                                               IsApplied = u.IsApplied,
                                                               IsArchived = u.IsArchived,
                                                               IsAutomatedCompletion = u.IsAutomatedCompletion,
                                                               IsClosed = u.IsClosed,
                                                               IsDeleted = u.IsDeleted,
                                                               LastUpdatedBy = u.LastUpdatedBy,
                                                               RoomId = u.RoomId,
                                                               RoomName = u.RoomName,
                                                               TotalItemsWithinCount = u.TotalItemsWithinCount,
                                                               UDF1 = u.UDF1,
                                                               UDF2 = u.UDF2,
                                                               UDF3 = u.UDF3,
                                                               UDF4 = u.UDF4,
                                                               UDF5 = u.UDF5,
                                                               Updated = u.Updated,
                                                               UpdatedByName = u.UpdatedByName,
                                                               Year = u.Year,
                                                               CreatedDate = u.CreatedDate,
                                                               UpdatedDate = u.UpdatedDate,
                                                               AddedFrom = u.AddedFrom,
                                                               EditedFrom = u.EditedFrom,
                                                               ReceivedOn = u.ReceivedOn,
                                                               ReceivedOnWeb = u.ReceivedOnWeb,
                                                               TotalRecords = u.TotalRecords,
                                                               ReleaseNumber = u.ReleaseNumber
                                                           }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();

                        TotalCount = 0;
                        if (lstIcnt != null && lstIcnt.Count > 0)
                        {
                            TotalCount = lstIcnt.FirstOrDefault().TotalRecords ?? 0;
                        }

                        return lstIcnt;
                    //TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM InventoryCount_History WHERE GUID = '" + GUID + "'").ToList()[0]);
                    //return (from u in context.Database.SqlQuery<InventoryCountDTO>(@"
                    //        SELECT A.*,B.UserName AS 'CreatedByName',C.UserName AS UpdatedByName,D.RoomName,'' AS CreatedDate,'' As UpdatedDAte FROM InventoryCount_History AS A 
                    //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    //        LEFT OUTER JOIN Room D on A.RoomId = D.ID
                    //        WHERE A.GUID = '" + GUID + "' order by HistoryID desc")
                    //        select new InventoryCountDTO
                    //        {
                    //            HistoryID = u.HistoryID,
                    //            CompanyId = u.CompanyId,
                    //            CompleteCauseCountGUID = u.CompleteCauseCountGUID,
                    //            CountCompletionDate = u.CountCompletionDate,
                    //            CountDate = u.CountDate,
                    //            CountItemDescription = u.CountItemDescription,
                    //            CountName = u.CountName,
                    //            CountStatus = u.CountStatus,
                    //            CountType = u.CountType,
                    //            Created = u.Created,
                    //            CreatedBy = u.CreatedBy,
                    //            CreatedByName = u.CreatedByName,
                    //            GUID = u.GUID,
                    //            ID = u.ID,
                    //            IsApplied = u.IsApplied,
                    //            IsArchived = u.IsArchived,
                    //            IsAutomatedCompletion = u.IsAutomatedCompletion,
                    //            IsClosed = u.IsClosed,
                    //            IsDeleted = u.IsDeleted,
                    //            LastUpdatedBy = u.LastUpdatedBy,
                    //            RoomId = u.RoomId,
                    //            RoomName = u.RoomName,
                    //            TotalItemsWithinCount = u.TotalItemsWithinCount,
                    //            UDF1 = u.UDF1,
                    //            UDF2 = u.UDF2,
                    //            UDF3 = u.UDF3,
                    //            UDF4 = u.UDF4,
                    //            UDF5 = u.UDF5,
                    //            Updated = u.Updated,
                    //            UpdatedByName = u.UpdatedByName,
                    //            Year = u.Year,
                    //            CreatedDate = u.CreatedDate,
                    //            UpdatedDate = u.UpdatedDate,
                    //            AddedFrom = u.AddedFrom,
                    //            EditedFrom = u.EditedFrom,
                    //            ReceivedOn = u.ReceivedOn,
                    //            ReceivedOnWeb = u.ReceivedOnWeb
                    //        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// Check Duplicate 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ActionMode"></param>
        /// <param name="ID"></param>
        /// <param name="TableName"></param>
        /// <param name="FieldName"></param>
        /// <param name="RoomID"></param>
        /// <param name="EnterpriseID"></param>
        /// <returns></returns>
        public string DuplicateCheck(string Name, string ActionMode, Int64 ID, string TableName, string FieldName, Int64 RoomID, Int64 CompanyID)
        {
            Name = (Name ?? string.Empty).Replace("'", "''").ToLower();
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    string WhereCond = "";

                    if (ActionMode == "add")
                    {
                        if (TableName == "ItemLocationDetails")
                        {
                            WhereCond = " ltrim(rtrim(lower(" + FieldName + "))) = '" + Name + "' and CompanyID =" + CompanyID + " and IsDeleted = 0 and IsArchived = 0 ";
                        }
                        else
                        {
                            WhereCond = " ltrim(rtrim(lower( " + FieldName + "))) = '" + Name + "' and Room =" + RoomID + " and CompanyID =" + CompanyID + " and IsDeleted = 0 and IsArchived = 0 ";
                        }
                    }
                    else
                    {
                        if (TableName == "ItemLocationDetails")
                        {
                            WhereCond = " ltrim(rtrim(lower(" + FieldName + "))) = '" + Name + "' and ID = " + ID + " and CompanyID =" + CompanyID + " ";
                        }
                        else
                        {
                            WhereCond = " ltrim(rtrim(lower(" + FieldName + "))) = '" + Name + "' and ID = " + ID + " and Room =" + RoomID + " and CompanyID =" + CompanyID + " ";
                        }
                    }
                    //var data = context.ChkDuplicate(TableName, "ID", WhereCond).ToList();
                    var params1 = new SqlParameter[] { new SqlParameter("@TableName", TableName), new SqlParameter("@PkID", "ID"), new SqlParameter("@WhereCond", WhereCond) };
                    var data = (from u in context.Database.SqlQuery<int?>("exec [ChkDuplicate] @TableName,@PkID,@WhereCond", params1) select u).ToList(); //A.IsDeleted!=1 AND A.IsArchived != 1 AND 

                    string Msg = "";
                    foreach (var item in data)
                    {
                        if (item.Value == 0 && ActionMode == "add")
                            Msg = "ok";
                        else if (item.Value == 0 && ActionMode == "edit")
                        {
                            WhereCond = " " + FieldName + " = '" + Name + "' and Room =" + RoomID + " and CompanyID =" + CompanyID + " and IsDeleted = 0 and IsArchived = 0 ";
                            params1 = new SqlParameter[] { new SqlParameter("@TableName", TableName), new SqlParameter("@PkID", "ID"), new SqlParameter("@WhereCond", WhereCond) };
                            data = (from u in context.Database.SqlQuery<int?>("exec [ChkDuplicate] @TableName,@PkID,@WhereCond", params1) select u).ToList(); //A.IsDeleted!=1 AND A.IsArchived != 1 AND 
                            //data = context.ChkDuplicate(TableName, "ID", WhereCond).ToList();
                            foreach (var item1 in data)
                            {
                                if (item1.Value == 0)
                                    Msg = "ok";
                                else
                                    Msg = "duplicate";
                            }
                        }
                        else
                        {
                            if (ActionMode == "edit" && item.Value == 1)
                                Msg = "ok";
                            else
                                Msg = "duplicate";
                        }
                    }
                    return Msg;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public long GetBOMIDsByName(Int64 RoomID, Int64 CompanyID, bool? IsForBom, string Name, string Type)
        {
            Name = Name == null ? Name : Name.Replace("'", "''");
            var params1 = new SqlParameter[] {new SqlParameter("@RoomID", RoomID),new SqlParameter("@CompanyID", CompanyID)
                ,new SqlParameter("@IsForBom", IsForBom ?? (object)DBNull.Value)
                ,new SqlParameter("@Name", Name ?? string.Empty)
                ,new SqlParameter("@Type", Type ?? string.Empty)
                };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<long>("exec GetBOMIDsByName @RoomID,@CompanyID,@IsForBom,@Name,@Type", params1).FirstOrDefault();
            }

        }
        /// <summary>
        /// Check Duplicate 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ActionMode"></param>
        /// <param name="ID"></param>
        /// <param name="TableName"></param>
        /// <param name="FieldName"></param>
        /// <param name="RoomID"></param>
        /// <param name="EnterpriseID"></param>
        /// <returns></returns>
        public string CheckItemDuplication(string Name, string ActionMode, long ID, string TableName, string FieldName, long RoomID,
                                         long CompanyID, int ItemType)
        {
            Name = (Name ?? string.Empty).Replace("'", "''").ToLower();
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    string WhereCond = "";

                    if (ActionMode == "add")
                    {
                        WhereCond = " ltrim(rtrim(lower( " + FieldName + "))) = '" + Name + "' and ItemType =" + ItemType + "  and Room =" + RoomID + " and CompanyID =" + CompanyID + " and IsDeleted = 0 and IsArchived = 0 ";
                    }
                    else
                    {
                        WhereCond = " ltrim(rtrim(lower(" + FieldName + "))) = '" + Name + "' and ID = " + ID + " and ItemType =" + ItemType + " and Room =" + RoomID + " and CompanyID =" + CompanyID + " ";
                    }
                    //var data = context.ChkDuplicate(TableName, "ID", WhereCond).ToList();
                    var params1 = new SqlParameter[] { new SqlParameter("@TableName", TableName), new SqlParameter("@PkID", "ID"), new SqlParameter("@WhereCond", WhereCond) };
                    var data = (from u in context.Database.SqlQuery<int?>("exec [ChkDuplicate] @TableName,@PkID,@WhereCond", params1) select u).ToList(); //A.IsDeleted!=1 AND A.IsArchived != 1 AND 

                    string Msg = "";
                    foreach (var item in data)
                    {
                        if (item.Value == 0 && ActionMode == "add")
                            Msg = "ok";
                        else if (item.Value == 0 && ActionMode == "edit")
                        {
                            WhereCond = " " + FieldName + " = '" + Name + "' and ItemType =" + ItemType + " and Room =" + RoomID + " and CompanyID =" + CompanyID + " and IsDeleted = 0 and IsArchived = 0 ";
                            params1 = new SqlParameter[] { new SqlParameter("@TableName", TableName), new SqlParameter("@PkID", "ID"), new SqlParameter("@WhereCond", WhereCond) };
                            data = (from u in context.Database.SqlQuery<int?>("exec [ChkDuplicate] @TableName,@PkID,@WhereCond", params1) select u).ToList(); //A.IsDeleted!=1 AND A.IsArchived != 1 AND 
                            //data = context.ChkDuplicate(TableName, "ID", WhereCond).ToList();
                            foreach (var item1 in data)
                            {
                                if (item1.Value == 0)
                                    Msg = "ok";
                                else
                                    Msg = "duplicate";
                            }
                        }
                        else
                        {
                            if (ActionMode == "edit" && item.Value == 1)
                                Msg = "ok";
                            else
                                Msg = "duplicate";
                        }
                    }
                    return Msg;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public string DuplicateCheck(string Name, string ActionMode, Int64 ID, string TableName, string FieldName, string ExtraWhereCondition, Int64 RoomID, Int64 CompanyID)
        {
            Name = Name.Replace("'", "''");
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    string WhereCond = "";

                    if (ActionMode == "add")
                        WhereCond = " " + FieldName + " = '" + Name + "' and " + ExtraWhereCondition + " and Room =" + RoomID + " and CompanyID =" + CompanyID + " and IsDeleted = 0 and IsArchived = 0 ";
                    else
                        WhereCond = " " + FieldName + " = '" + Name + "' and " + ExtraWhereCondition + " and ID = " + ID + " and Room =" + RoomID + " and CompanyID =" + CompanyID + " ";

                    //var data = context.ChkDuplicate(TableName, "ID", WhereCond).ToList();
                    var params1 = new SqlParameter[] { new SqlParameter("@TableName", TableName), new SqlParameter("@PkID", "ID"), new SqlParameter("@WhereCond", WhereCond) };
                    var data = (from u in context.Database.SqlQuery<int?>("exec [ChkDuplicate] @TableName,@PkID,@WhereCond", params1) select u).ToList(); //A.IsDeleted!=1 AND A.IsArchived != 1 AND 

                    string Msg = "";
                    foreach (var item in data)
                    {
                        if (item.Value == 0 && ActionMode == "add")
                            Msg = "ok";
                        else if (item.Value == 0 && ActionMode == "edit")
                        {
                            WhereCond = " " + FieldName + " = '" + Name + "' and Room =" + RoomID + " and CompanyID =" + CompanyID + " and IsDeleted = 0 and IsArchived = 0 ";
                            // data = context.ChkDuplicate(TableName, "ID", WhereCond).ToList();
                            params1 = new SqlParameter[] { new SqlParameter("@TableName", TableName), new SqlParameter("@PkID", "ID"), new SqlParameter("@WhereCond", WhereCond) };
                            data = (from u in context.Database.SqlQuery<int?>("exec [ChkDuplicate] @TableName,@PkID,@WhereCond", params1) select u).ToList(); //A.IsDeleted!=1 AND A.IsArchived != 1 AND 

                            foreach (var item1 in data)
                            {
                                if (item1.Value == 0)
                                    Msg = "ok";
                                else
                                    Msg = "duplicate";
                            }
                        }
                        else
                        {
                            if (ActionMode == "edit" && item.Value == 1)
                                Msg = "ok";
                            else
                                Msg = "duplicate";
                        }
                    }
                    return Msg;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public string DuplicateCheck(string Name, string ActionMode, Int64 ID, string TableName, string FieldName)
        {
            Name = Name.Replace("'", "''");
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    string WhereCond = "";

                    if (ActionMode == "add")
                        WhereCond = " " + FieldName + " = '" + Name + "' and IsDeleted = 0 and IsArchived = 0 ";
                    else
                        WhereCond = " " + FieldName + " = '" + Name + "' and ID = " + ID + " ";

                    //var data = context.ChkDuplicate(TableName, "ID", WhereCond).ToList();
                    var params1 = new SqlParameter[] { new SqlParameter("@TableName", TableName), new SqlParameter("@PkID", "ID"), new SqlParameter("@WhereCond", WhereCond) };
                    var data = (from u in context.Database.SqlQuery<int?>("exec [ChkDuplicate] @TableName,@PkID,@WhereCond", params1) select u).ToList(); //A.IsDeleted!=1 AND A.IsArchived != 1 AND 

                    string Msg = "";
                    foreach (var item in data)
                    {
                        if (item.Value == 0 && ActionMode == "add")
                            Msg = "ok";
                        else if (item.Value == 0 && ActionMode == "edit")
                        {
                            WhereCond = " " + FieldName + " = '" + Name + "' and IsDeleted = 0 and IsArchived = 0 ";
                            // data = context.ChkDuplicate(TableName, "ID", WhereCond).ToList();
                            params1 = new SqlParameter[] { new SqlParameter("@TableName", TableName), new SqlParameter("@PkID", "ID"), new SqlParameter("@WhereCond", WhereCond) };
                            data = (from u in context.Database.SqlQuery<int?>("exec [ChkDuplicate] @TableName,@PkID,@WhereCond", params1) select u).ToList(); //A.IsDeleted!=1 AND A.IsArchived != 1 AND 

                            foreach (var item1 in data)
                            {
                                if (item1.Value == 0)
                                    Msg = "ok";
                                else
                                    Msg = "duplicate";
                            }
                        }
                        else
                        {
                            if (ActionMode == "edit" && item.Value == 1)
                                Msg = "ok";
                            else
                                Msg = "duplicate";
                        }
                    }
                    return Msg;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public string DuplicateUFDOptionCheck(string Name, string ActionMode, Int64 ID, string TableName, string FieldName, Int64 _UDFID)
        {
            Name = Name.Replace("'", "''");
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    string WhereCond = "";

                    if (ActionMode == "add")
                        WhereCond = " " + FieldName + " = '" + Name + "' and IsDeleted = 0 and UDFID =" + _UDFID + "";
                    else
                        WhereCond = " " + FieldName + " = '" + Name + "' and ID = " + ID + " and UDFID =" + _UDFID + "";

                    //var data = context.ChkDuplicate(TableName, "ID", WhereCond).ToList();
                    var params1 = new SqlParameter[] { new SqlParameter("@TableName", TableName), new SqlParameter("@PkID", "ID"), new SqlParameter("@WhereCond", WhereCond) };
                    var data = (from u in context.Database.SqlQuery<int?>("exec [ChkDuplicate] @TableName,@PkID,@WhereCond", params1) select u).ToList(); //A.IsDeleted!=1 AND A.IsArchived != 1 AND 

                    string Msg = "";
                    foreach (var item in data)
                    {
                        if (item.Value == 0 && ActionMode == "add")
                            Msg = "ok";
                        else if (item.Value == 0 && ActionMode == "edit")
                        {
                            WhereCond = " " + FieldName + " = '" + Name + "' and IsDeleted = 0 and UDFID =" + _UDFID + "";
                            // data = context.ChkDuplicate(TableName, "ID", WhereCond).ToList();
                            params1 = new SqlParameter[] { new SqlParameter("@TableName", TableName), new SqlParameter("@PkID", "ID"), new SqlParameter("@WhereCond", WhereCond) };
                            data = (from u in context.Database.SqlQuery<int?>("exec [ChkDuplicate] @TableName,@PkID,@WhereCond", params1) select u).ToList(); //A.IsDeleted!=1 AND A.IsArchived != 1 AND 

                            foreach (var item1 in data)
                            {
                                if (item1.Value == 0)
                                    Msg = "ok";
                                else
                                    Msg = "duplicate";
                            }
                        }
                        else
                        {
                            if (ActionMode == "edit" && item.Value == 1)
                                Msg = "ok";
                            else
                                Msg = "duplicate";
                        }
                    }
                    return Msg;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public List<UDFBulkItem> DuplicateUFDOptionCheckWithList(List<UDFBulkItem> lstUDF, string ActionMode, Int64 ID, string TableName)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var udfTable = new DataTable();
                    udfTable.Columns.Add("UDFID", typeof(Int64));
                    udfTable.Columns.Add("UDFOption", typeof(string));
                    udfTable.Columns.Add("UDFColumnName", typeof(string));

                    foreach (var item in lstUDF)
                    {
                        udfTable.Rows.Add(item.UDFID, item.UDFOption, item.UDFColumnName);
                    }

                    var parameters = new[]
                    {
                        new SqlParameter("@TableName", TableName),
                        new SqlParameter("@PkID", "ID"),
                        new SqlParameter("@UDFList", SqlDbType.Structured)
                        {
                            TypeName = "UDFList_BulkType",
                            Value = udfTable
                        }
                    };
                    var result = context.Database.SqlQuery<UDFBulkItem>("EXEC ChkDuplicateWithUDFList @TableName, @PkID, @UDFList", parameters).ToList();
                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public string DuplicateCheckForCreditPull(string Name, string ActionMode, Int64 ID, string TableName, string FieldName, Int64 RoomID, Int64 CompanyID, string ExtraWhereCondtion)
        {
            Name = Name.Replace("'", "''");
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    string WhereCond = "";
                    string Msg = "";
                    WhereCond = " " + FieldName + " = '" + Name + "' and Room =" + RoomID + " and CompanyID =" + CompanyID;
                    //var dataCheckAvailability = context.ChkDuplicate(TableName, "ID", WhereCond).ToList();
                    var params1 = new SqlParameter[] { new SqlParameter("@TableName", TableName), new SqlParameter("@PkID", "ID"), new SqlParameter("@WhereCond", WhereCond) };
                    var dataCheckAvailability = (from u in context.Database.SqlQuery<int?>("exec [ChkDuplicate] @TableName,@PkID,@WhereCond", params1) select u).ToList(); //A.IsDeleted!=1 AND A.IsArchived != 1 AND 

                    foreach (var item1 in dataCheckAvailability)
                    {
                        if (item1.Value > 0)
                        {
                            WhereCond = " " + FieldName + " = '" + Name + "' and Room =" + RoomID + " and CompanyID =" + CompanyID + " " + ExtraWhereCondtion;
                            // var data = context.ChkDuplicate(TableName, "ID", WhereCond).ToList();
                            params1 = new SqlParameter[] { new SqlParameter("@TableName", TableName), new SqlParameter("@PkID", "ID"), new SqlParameter("@WhereCond", WhereCond) };
                            var data = (from u in context.Database.SqlQuery<int?>("exec [ChkDuplicate] @TableName,@PkID,@WhereCond", params1) select u).ToList(); //A.IsDeleted!=1 AND A.IsArchived != 1 AND 

                            foreach (var item in data)
                            {
                                if (item.Value == 1)
                                    Msg = "ok";
                                else
                                    Msg = "duplicate";
                            }
                        }
                        else
                            Msg = "ok";
                    }
                    return Msg;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public bool UpdateItemQuentity(Guid ItemGUID, Int64 LocationID, double Quentity, Int64 RoomID, Int64 CompanyID, Int64 UpdateBy, int UpdateType, long SessionUserId, long EnterpriseId)
        {
            if (Quentity == 0 || ItemGUID != Guid.Empty || LocationID < 0 || RoomID <= 0 || CompanyID <= 0)
                return false;

            int Qty = int.Parse(Quentity.ToString());
            ItemLocationDetailsDAL ItemLocationDetailDAL = new ItemLocationDetailsDAL(base.DataBaseName);
            //ItemLocationDetailsDTO ObjItemLocationDTO = ItemLocationDetailDAL.GetCachedData(ItemGUID, RoomID, CompanyID).Where(x => x.BinID == LocationID).SingleOrDefault();
            ItemLocationDetailsDTO ObjItemLocationDTO = ItemLocationDetailDAL.GetItemLocationDetail(ItemGUID, RoomID, CompanyID, LocationID, string.Empty, string.Empty, string.Empty, null, null, string.Empty).FirstOrDefault();
            if (ObjItemLocationDTO != null)
            {
                ObjItemLocationDTO.CustomerOwnedQuantity = ObjItemLocationDTO.CustomerOwnedQuantity.Value + (Qty * UpdateType);
                ItemLocationDetailDAL.Edit(ObjItemLocationDTO);

            }


            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            ItemMasterDTO objItemDTO = objItemDAL.GetItemWithoutJoins(null, ItemGUID);

            if (objItemDTO != null)
            {
                objItemDTO.OnHandQuantity = objItemDTO.OnHandQuantity + (Qty * UpdateType);
                objItemDTO.WhatWhereAction = "Common";
                objItemDAL.Edit(objItemDTO, SessionUserId, EnterpriseId);
            }

            ItemLocationQTYDAL objLocQTY = new ItemLocationQTYDAL(base.DataBaseName);
            //ItemLocationQTYDTO lstLocDTO = objLocQTY.GetAllRecords(RoomID, CompanyID).Where(x => x.BinID == LocationID && x.ItemGUID == ItemGUID).SingleOrDefault();
            ItemLocationQTYDTO lstLocDTO = objLocQTY.GetItemLocationQTY(RoomID, CompanyID, LocationID, Convert.ToString(ItemGUID)).FirstOrDefault();

            if (lstLocDTO != null)
            {
                lstLocDTO.Quantity = lstLocDTO.Quantity + (Qty * UpdateType);
                //objLocQTY.Edit(lstLocDTO);
                objLocQTY.UpdateOnHandQuantity(lstLocDTO.ItemGUID.Value, lstLocDTO.Room.GetValueOrDefault(0), lstLocDTO.CompanyID.GetValueOrDefault(0), SessionUserId, EnterpriseId);
            }

            #region "Item Quantity Deduction Cache"
            //Get Cached-Media
            //IEnumerable<ItemMasterDTO> ObjCacheItem = CacheHelper<IEnumerable<ItemMasterDTO>>.GetCacheItem("Cached_ItemMaster_" + objItemDTO.CompanyID.ToString());
            //if (ObjCacheItem != null)
            //{
            //List<ItemMasterDTO> objTemp = ObjCacheItem.ToList();
            //objTemp.RemoveAll(i => i.ID == objItemDTO.ID);
            //ObjCacheItem = objTemp.AsEnumerable();

            //List<ItemMasterDTO> tempC = new List<ItemMasterDTO>();
            //tempC.Add(objItemDTO);
            //IEnumerable<ItemMasterDTO> NewCache = ObjCacheItem.Concat(tempC.AsEnumerable()).AsEnumerable();
            //CacheHelper<IEnumerable<ItemMasterDTO>>.AddCacheItem("Cached_ItemMaster_" + objItemDTO.CompanyID.ToString(), NewCache);
            //}
            #endregion
            return true;
        }

        public string DeleteRecordsItem(string TableName, string IDs, Int64 Roomid, Int64 CompanyId)
        {
            try
            {
                if (!(Roomid > 0 && CompanyId > 0))
                {
                    return "";
                }

                string strError = "";
                string strDeleteIds = "";
                //string strBlankMsg = "";
                using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    foreach (var item in IDs.Split(','))
                    {
                        if (!string.IsNullOrEmpty(item.Trim()))
                        {
                            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
                            ItemMasterDTO objdto = objItemMasterDAL.GetItemWithoutJoins(null, Guid.Parse(item));
                            if ((objdto.StagedQuantity != null && objdto.StagedQuantity != 0) ||
                                (objdto.InTransitquantity != null && objdto.InTransitquantity != 0) ||
                                (objdto.OnOrderQuantity != null && objdto.OnOrderQuantity != 0) ||
                                (objdto.OnTransferQuantity != null && objdto.OnTransferQuantity != 0) ||
                                (objdto.RequisitionedQuantity != null && objdto.RequisitionedQuantity != 0) ||
                                (objdto.OnHandQuantity != null && objdto.OnHandQuantity != 0) ||
                                (objdto.PackingQuantity != null && objdto.PackingQuantity != 0) ||
(objdto.OnReturnQuantity != null && objdto.OnReturnQuantity != 0) ||
                                (objdto.OnOrderInTransitQuantity != null && objdto.OnOrderInTransitQuantity != 0))

                            {
                                strError = "Cannot delete Item,Beacause It is used in another module.";
                            }
                            else
                            {
                                strDeleteIds = strDeleteIds == "" ? item : strDeleteIds + "," + item;
                                string strUpdateOnHand = "  UPDATE ItemMaster SET IsDeleted = 1 WHERE CompanyID = " + CompanyId.ToString() + " AND Room = " + Roomid.ToString() + " AND Guid =  '" + item.ToString() + "'";
                                context.Database.ExecuteSqlCommand(strUpdateOnHand);
                                strError = "ok";
                            }

                        }
                    }
                }

                if (strDeleteIds != "")
                {

                    //IEnumerable<ItemMasterDTO> ObjCache = CacheHelper<IEnumerable<ItemMasterDTO>>.GetCacheItem("Cached_ItemMaster_" + CompanyId.ToString());
                    //if (ObjCache != null)
                    //{
                    //    List<ItemMasterDTO> objTemp = ObjCache.ToList();
                    //    objTemp.RemoveAll(i => strDeleteIds.Split(',').Contains(i.GUID.ToString()));
                    //    ObjCache = objTemp.AsEnumerable();
                    //    CacheHelper<IEnumerable<ItemMasterDTO>>.AppendToCacheItem("Cached_ItemMaster_" + CompanyId.ToString(), ObjCache);
                    //}

                }
                return strError;
            }
            catch (EntitySqlException ex)
            {
                return ex.Message;
            }
        }

        public bool CheckIfAnyBinIsDefault(string Ids, out string BinNumbers)
        {
            BinNumbers = "";
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<long?> lstBinId = Ids.Split(',').Where(x => !String.IsNullOrEmpty(x)).Select(x => (long?)Convert.ToInt64(x)).ToList();
                List<string> lstReturn = (from B in context.BinMasters
                                          where (lstBinId.Contains(B.ID) || lstBinId.Contains(B.ParentBinId))
                                                && B.IsDefault == true && B.IsDeleted == false
                                          select B.BinNumber).ToList();

                if (lstReturn == null || lstReturn.Count <= 0)
                {
                    BinNumbers = "";
                    return false;
                }
                else
                {
                    BinNumbers = String.Join(",", lstReturn.FirstOrDefault());
                    return true;
                }
            }
        }

        public bool CheckIfAnyBinIsDefaultbySP(string Ids, out string BinNumbers)
        {
            List<BinMasterDTO> lstBinDto = new List<BinMasterDTO>();
            BinNumbers = "";
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Ids", Ids) };

                lstBinDto = (from u in context.Database.SqlQuery<BinMasterDTO>("EXEC dbo.CheckIfAnyBinIsDefault @Ids", params1)
                             select new BinMasterDTO
                             {
                                 BinNumber = u.BinNumber
                             }).AsParallel().ToList();

                // return context.Database.SqlQuery<BinMasterDTO>("exec [GetMaterialStagingDetailChangeLog] @MaterialStagingGUID,@dbName", paramsMSD1).ToList();
            }
            if (lstBinDto == null || lstBinDto.Count <= 0)
            {
                BinNumbers = "";
                return false;
            }
            else
            {
                BinNumbers = String.Join(",", lstBinDto[0].BinNumber);
                return true;
            }
        }

        public string DeleteModulewise(string Ids, string ModuleName, bool IsGUID, long UserID, long EnterpriseID, long CompanyID, long RoomID)
        {
            DataSet ds = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            ds = SqlHelper.ExecuteDataset(EturnsConnection, "USP_ModulewiseDelete", Ids, ModuleName, IsGUID, UserID, "");
            DataTable dt = new DataTable();
            string msg = string.Empty;
            int Failcnt = 0;
            int Successcnt = 0;
            if (ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["Status"].ToString() == "Success")
                    {
                        Successcnt += 1;
                    }
                    else if (dr["Status"].ToString() == "Fail")
                    {
                        Failcnt += 1;
                    }
                }
                string currentCulture = "en-US";
                if (HttpContext.Current != null)
                {
                    if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                    {
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                        {
                            currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                        }

                    }
                }
                else
                {
                    eTurns.DTO.eTurnsRegionInfo objeTurnsRegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomID, CompanyID, UserID);
                    currentCulture = objeTurnsRegionInfo.CultureName;
                }
                string ResourceFileCommon = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResCommon", currentCulture, EnterpriseID, CompanyID);
                if (Successcnt > 0)
                {
                    string MsgRecordsDeleteSuccess = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgRecordsDeleteSuccess", ResourceFileCommon, EnterpriseID, CompanyID, RoomID, "ResCommon", currentCulture);
                    msg = Successcnt + " " + MsgRecordsDeleteSuccess; //string.Format(ResCommon.RecordDeletedSuccessfully, objDeletedItems.Length)
                }
                if (Failcnt > 0)
                {
                    string MsgRecordsUsedInOtherModule = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgRecordsUsedInOtherModule", ResourceFileCommon, EnterpriseID, CompanyID, RoomID, "ResCommon", currentCulture);
                    if (string.IsNullOrEmpty(msg))
                    {
                        msg = MsgRecordsUsedInOtherModule.Replace("{0}", Convert.ToString(Failcnt));
                    }
                    else
                    {
                        msg = msg + " " + MsgRecordsUsedInOtherModule.Replace("{0}", Convert.ToString(Failcnt)); ;
                    }
                }
            }

            return msg;
        }
        public ModuleDeleteDTO DeleteModulewiseForBOM(string Ids, string ModuleName, bool IsGUID, long UserID, long EnterpriseID, long CompanyID, long RoomID)
        {
            ModuleDeleteDTO objModuleDeleteDTO = new ModuleDeleteDTO();
            DataSet ds = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            ds = SqlHelper.ExecuteDataset(EturnsConnection, "USP_ModulewiseDelete", Ids, ModuleName, IsGUID, UserID, "");
            DataTable dt = new DataTable();
            string msg = string.Empty;
            int Failcnt = 0;
            int Successcnt = 0;
            string ItemUsedRoom = ""; //WI-8331

            if (ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["Status"].ToString() == "Success")
                    {
                        Successcnt += 1;
                    }
                    else if (dr["Status"].ToString() == "Fail")
                    {
                        Failcnt += 1;
                        ItemUsedRoom = dr["RoomName"].ToString();
                    }
                }
                string currentCulture = "en-US";
                if (HttpContext.Current != null)
                {
                    if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                    {
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                        {
                            currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                        }

                    }
                }
                else
                {
                    eTurns.DTO.eTurnsRegionInfo objeTurnsRegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomID, CompanyID, UserID);
                    currentCulture = (objeTurnsRegionInfo != null ? objeTurnsRegionInfo.CultureName : currentCulture);
                }
                string ResourceFileCommon = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResCommon", currentCulture, EnterpriseID, CompanyID);
                if (Successcnt > 0)
                {
                    msg = Successcnt + " " + "Success"; //string.Format(ResCommon.RecordDeletedSuccessfully, objDeletedItems.Length)
                }
                if (Failcnt > 0)
                {
                    string MsgRecordsUsedInOtherModule = "Record used in other module";
                    if (string.IsNullOrEmpty(msg))
                    {
                        msg = MsgRecordsUsedInOtherModule.Replace("{0}", Convert.ToString(Failcnt));
                    }
                    else
                    {
                        msg = msg + " " + MsgRecordsUsedInOtherModule.Replace("{0}", Convert.ToString(Failcnt)); ;
                    }

                    if (!string.IsNullOrEmpty(ItemUsedRoom))
                    {
                        msg = msg + " in Room :- " + ItemUsedRoom;
                    }
                }
            }

            List<DeleteStatusDTO> lstDeleteStatusDTO = new List<DeleteStatusDTO>();

            foreach (DataRow dr in dt.Rows)
            {
                DeleteStatusDTO obj = new DeleteStatusDTO();
                obj.RowNum = Convert.ToInt64(dr["RowNum"].ToString());
                obj.Id = dr["Id"].ToString();
                obj.Status = dr["Status"].ToString();
                lstDeleteStatusDTO.Add(obj);
            }
            objModuleDeleteDTO.CommonMessage = msg;
            objModuleDeleteDTO.SuccessItems = lstDeleteStatusDTO.Where(t => t.Status == "Success").ToList();
            objModuleDeleteDTO.FailureItems = lstDeleteStatusDTO.Where(t => t.Status == "Fail").ToList();

            return objModuleDeleteDTO;
        }

        public string DeleteModulewiseForService(string Ids, string ModuleName, bool IsGUID, long UserID, out string status, string EditedFrom, long EnterpriseID, long CompanyID, long RoomID)
        {
            status = string.Empty;
            DataSet ds = new DataSet();
            //string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            ds = SqlHelper.ExecuteDataset(EturnsConnection, "USP_ModulewiseDelete", Ids, ModuleName, IsGUID, UserID, EditedFrom);
            DataTable dt = new DataTable();
            string msg = string.Empty;
            int Failcnt = 0;
            int Successcnt = 0;
            if (ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["Status"].ToString() == "Success")
                    {
                        Successcnt += 1;
                    }
                    else if (dr["Status"].ToString() == "Fail")
                    {
                        Failcnt += 1;
                    }
                }
                string currentCulture = "en-US";
                string ResourceFileCommon = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResCommon", currentCulture, EnterpriseID, CompanyID);
                if (Successcnt > 0)
                {
                    string MsgRecordsDeleteSuccess = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgRecordsDeleteSuccess", ResourceFileCommon, EnterpriseID, CompanyID, RoomID, "ResCommon", currentCulture);
                    msg = Successcnt + " " + MsgRecordsDeleteSuccess; //string.Format(ResCommon.RecordDeletedSuccessfully, objDeletedItems.Length)
                    status = "Success";
                }
                if (Failcnt > 0)
                {
                    string MsgRecordsUsedInOtherModule = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgRecordsUsedInOtherModule", ResourceFileCommon, EnterpriseID, CompanyID, RoomID, "ResCommon", currentCulture);
                    if (string.IsNullOrEmpty(msg))
                    {
                        msg = MsgRecordsUsedInOtherModule.Replace("{0}", Convert.ToString(Failcnt));
                    }
                    else
                    {
                        msg = msg + " " + MsgRecordsUsedInOtherModule.Replace("{0}", Convert.ToString(Failcnt));
                    }

                    status = "fail";
                }


            }

            return msg;
        }

        public ModuleDeleteDTO DeleteModulewise(string Ids, string ModuleName, bool IsGUID, long UserID, bool combined, long EnterpriseID, long CompanyID, long RoomID)
        {
            ModuleDeleteDTO objModuleDeleteDTO = new ModuleDeleteDTO();
            DataSet ds = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            ds = SqlHelper.ExecuteDataset(EturnsConnection, "USP_ModulewiseDelete", Ids, ModuleName, IsGUID, UserID, "");
            DataTable dt = new DataTable();
            string msg = string.Empty;
            int Failcnt = 0;
            int Successcnt = 0;
            if (ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["Status"].ToString() == "Success")
                    {
                        Successcnt += 1;
                    }
                    else if (dr["Status"].ToString() == "Fail")
                    {
                        Failcnt += 1;
                    }
                }
                string currentCulture = "en-US";
                if (HttpContext.Current != null)
                {
                    if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                    {
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                        {
                            currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                        }

                    }
                }
                else
                {
                    eTurns.DTO.eTurnsRegionInfo objeTurnsRegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomID, CompanyID, UserID);
                    currentCulture = objeTurnsRegionInfo.CultureName;
                }
                string ResourceFileCommon = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResCommon", currentCulture, EnterpriseID, CompanyID);
                if (Successcnt > 0)
                {
                    string MsgRecordsDeleteSuccess = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgRecordsDeleteSuccess", ResourceFileCommon, EnterpriseID, CompanyID, RoomID, "ResCommon", currentCulture);
                    msg = Successcnt + " " + MsgRecordsDeleteSuccess; //string.Format(ResCommon.RecordDeletedSuccessfully, objDeletedItems.Length)
                }
                if (Failcnt > 0)
                {
                    string MsgRecordsUsedInOtherModule = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgRecordsUsedInOtherModule", ResourceFileCommon, EnterpriseID, CompanyID, RoomID, "ResCommon", currentCulture);
                    if (string.IsNullOrEmpty(msg))
                    {
                        msg = MsgRecordsUsedInOtherModule.Replace("{0}", Convert.ToString(Failcnt));
                    }
                    else
                    {
                        msg = msg + " " + MsgRecordsUsedInOtherModule.Replace("{0}", Convert.ToString(Failcnt));
                    }
                }
                List<DeleteStatusDTO> lstDeleteStatusDTO = new List<DeleteStatusDTO>();

                foreach (DataRow dr in dt.Rows)
                {
                    DeleteStatusDTO obj = new DeleteStatusDTO();
                    obj.RowNum = Convert.ToInt64(dr["RowNum"].ToString());
                    obj.Id = dr["Id"].ToString();
                    obj.Status = dr["Status"].ToString();
                    lstDeleteStatusDTO.Add(obj);
                }
                objModuleDeleteDTO.CommonMessage = msg;
                objModuleDeleteDTO.SuccessItems = lstDeleteStatusDTO.Where(t => t.Status == "Success").ToList();
                objModuleDeleteDTO.FailureItems = lstDeleteStatusDTO.Where(t => t.Status == "Fail").ToList();
            }

            return objModuleDeleteDTO;
        }


        public ModuleUnDeleteDTO UnDeleteModulewise(string Ids, string ModuleName, bool IsGUID, long UserID, bool combined, long EnterpriseID, long CompanyID, long RoomID)
        {
            ModuleUnDeleteDTO objModuleUnDeleteDTO = new ModuleUnDeleteDTO();
            DataSet ds = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            ds = SqlHelper.ExecuteDataset(EturnsConnection, "USP_ModulewiseUnDelete", Ids, ModuleName, IsGUID, UserID);
            DataTable dt = new DataTable();
            string msg = string.Empty;
            int Failcnt = 0;
            int Successcnt = 0;
            if (ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["Status"].ToString() == "Success")
                    {
                        Successcnt += 1;
                    }
                    else if (dr["Status"].ToString() == "Fail")
                    {
                        Failcnt += 1;
                    }
                }
                string currentCulture = "en-US";
                if (HttpContext.Current != null)
                {
                    if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                    {
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                        {
                            currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                        }

                    }
                }
                else
                {
                    eTurns.DTO.eTurnsRegionInfo objeTurnsRegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomID, CompanyID, UserID);
                    currentCulture = objeTurnsRegionInfo.CultureName;
                }
                string ResourceFileCommon = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResCommon", currentCulture, EnterpriseID, CompanyID);
                if (Successcnt > 0)
                {
                    string MsgUndeleteSuccess = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgUndeleteSuccess", ResourceFileCommon, EnterpriseID, CompanyID, RoomID, "ResCommon", currentCulture);
                    msg = Successcnt + " " + MsgUndeleteSuccess;
                }
                if (Failcnt > 0)
                {
                    string MsgUndeleteFailure = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgUndeleteFailure", ResourceFileCommon, EnterpriseID, CompanyID, RoomID, "ResCommon", currentCulture);
                    if (string.IsNullOrEmpty(msg))
                    {
                        msg = MsgUndeleteFailure.Replace("{0}", Convert.ToString(Failcnt));
                    }
                    else
                    {
                        msg = msg + " " + MsgUndeleteFailure.Replace("{0}", Convert.ToString(Failcnt));
                    }
                }
                List<UnDeleteStatusDTO> lstUnDeleteStatusDTO = new List<UnDeleteStatusDTO>();

                foreach (DataRow dr in dt.Rows)
                {
                    UnDeleteStatusDTO obj = new UnDeleteStatusDTO();
                    obj.RowNum = Convert.ToInt64(dr["RowNum"].ToString());
                    obj.Id = dr["Id"].ToString();
                    obj.Status = dr["Status"].ToString();
                    lstUnDeleteStatusDTO.Add(obj);
                }
                objModuleUnDeleteDTO.CommonMessage = msg;
                objModuleUnDeleteDTO.SuccessItems = lstUnDeleteStatusDTO.Where(t => t.Status == "Success").ToList();
                objModuleUnDeleteDTO.FailureItems = lstUnDeleteStatusDTO.Where(t => t.Status == "Fail").ToList();
            }

            return objModuleUnDeleteDTO;
        }

        public List<DeleteStatusDTO> DeleteModulewiseRetList(string Ids, string ModuleName, bool IsGUID, long UserID)
        {
            DataSet ds = new DataSet();
            //string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            string Connectionstring = base.DataBaseConnectionString;
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            ds = SqlHelper.ExecuteDataset(EturnsConnection, "USP_ModulewiseDelete", Ids, ModuleName, IsGUID, UserID, "");
            DataTable dt = new DataTable();

            List<DeleteStatusDTO> lstDeleteStatusDTO = new List<DeleteStatusDTO>();
            if (ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    DeleteStatusDTO obj = new DeleteStatusDTO();
                    obj.RowNum = Convert.ToInt64(dr["RowNum"].ToString());
                    obj.Id = dr["Id"].ToString();
                    obj.Status = dr["Status"].ToString();
                    lstDeleteStatusDTO.Add(obj);
                }
            }

            return lstDeleteStatusDTO;
        }

        public List<DeleteStatusDTO> UnDeleteModulewiseRetList(string Ids, string ModuleName, bool IsGUID, long UserID)
        {
            DataSet ds = new DataSet();
            string Connectionstring = base.DataBaseConnectionString;
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            ds = SqlHelper.ExecuteDataset(EturnsConnection, "USP_ModulewiseUnDelete", Ids, ModuleName, IsGUID, UserID);
            DataTable dt = new DataTable();

            List<DeleteStatusDTO> lstDeleteStatusDTO = new List<DeleteStatusDTO>();
            if (ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    DeleteStatusDTO obj = new DeleteStatusDTO();
                    obj.RowNum = Convert.ToInt64(dr["RowNum"].ToString());
                    obj.Id = dr["Id"].ToString();
                    obj.Status = dr["Status"].ToString();
                    lstDeleteStatusDTO.Add(obj);
                }
            }

            return lstDeleteStatusDTO;
        }

        public string DeleteRecords(string TableName, string IDs, Int64 Roomid, Int64 CompanyId)
        {
            try
            {
                if (!(Roomid > 0 && CompanyId > 0))
                {
                    return "";
                }


                string strError = "";
                string strDeleteIds = "";
                string strBlankMsg = "";
                using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {

                    foreach (var item in IDs.Split(','))
                    {
                        if (!string.IsNullOrEmpty(item.Trim()))
                        {
                            try
                            {
                                //context.DeleteStatusChange("Delete", TableName, item);
                                var params1 = new SqlParameter[] { new SqlParameter("@Flag", "Delete"), new SqlParameter("@TableName", TableName), new SqlParameter("@Id", item) };
                                context.Database.ExecuteSqlCommand("exec [DeleteStatusChange] @Flag,@TableName,@Id", params1);
                                strDeleteIds = strDeleteIds == "" ? item : strDeleteIds + "," + item;
                            }
                            catch (Exception)
                            {
                                if (TableName.ToLower() == "itemmaster")
                                {
                                    ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
                                    var result = objItemMasterDAL.GetItemWithoutJoins(null, Guid.Parse(item));
                                    if (result != null && result.ItemNumber != "")
                                    {
                                        strError = strError == "" ? result.ItemNumber : strError + ", " + result.ItemNumber;
                                    }
                                    else
                                    {
                                        strBlankMsg += "error";
                                        //strError = strError == "" ? item : strError + "," + item;
                                    }
                                }
                                else if (TableName.ToLower() == "binmaster")
                                {
                                    BinMasterDAL objBinMasterDal = new BinMasterDAL(base.DataBaseName);
                                    var result = objBinMasterDal.GetBinByID(Convert.ToInt64(item), Roomid, CompanyId);
                                    //var result = objBinMasterDal.GetItemLocation( Roomid, CompanyId, false, false,Guid.Empty, Convert.ToInt64(item),null,null).FirstOrDefault();
                                    if (result != null && result.BinNumber != "")
                                    {
                                        strError = strError == "" ? result.BinNumber : strError + ", " + result.BinNumber;
                                    }
                                    else
                                    {
                                        strBlankMsg += "error";
                                        //strError = strError == "" ? item : strError + "," + item;
                                    }
                                }
                                else if (TableName.ToLower() == "unitmaster")
                                {
                                    UnitMasterDAL objBinMasterDal = new UnitMasterDAL(base.DataBaseName);
                                    var result = objBinMasterDal.GetUnitByIDPlain(Convert.ToInt64(item));
                                    if (result != null && result.Unit != "")
                                    {
                                        strError = strError == "" ? result.Unit : strError + ", " + result.Unit;
                                    }
                                    else
                                    {
                                        strBlankMsg += "error";
                                        //strError = strError == "" ? item : strError + "," + item;
                                    }
                                }
                                else if (TableName.ToLower() == "bomunitmaster")
                                {
                                    UnitMasterDAL objBinMasterDal = new UnitMasterDAL(base.DataBaseName);
                                    var result = objBinMasterDal.GetUnitByIDPlain(Convert.ToInt64(item));
                                    if (result != null && result.Unit != "")
                                    {
                                        strError = strError == "" ? result.Unit : strError + ", " + result.Unit;
                                    }
                                    else
                                    {
                                        strBlankMsg += "error";
                                        //strError = strError == "" ? item : strError + "," + item;
                                    }
                                }
                                else if (TableName.ToLower() == "locationmaster")
                                {
                                    LocationMasterDAL objBinMasterDal = new LocationMasterDAL(base.DataBaseName);
                                    var result = objBinMasterDal.GetLocationByIDPlain(Convert.ToInt64(item), Roomid, CompanyId);
                                    if (result != null && result.Location != "")
                                    {
                                        strError = strError == "" ? result.Location : strError + ", " + result.Location;
                                    }
                                    else
                                    {
                                        strBlankMsg += "error";
                                        //strError = strError == "" ? item : strError + "," + item;
                                    }
                                }
                                else if (TableName.ToLower() == "categorymaster")
                                {
                                    CategoryMasterDAL objBinMasterDal = new CategoryMasterDAL(base.DataBaseName);
                                    var result = objBinMasterDal.GetCategoryByCatID(Convert.ToInt64(item));
                                    if (result != null && result.Category != "")
                                    {
                                        strError = strError == "" ? result.Category : strError + ", " + result.Category;
                                    }
                                    else
                                    {
                                        strBlankMsg += "error";
                                        //strError = strError == "" ? item : strError + "," + item;
                                    }
                                }
                                else if (TableName.ToLower() == "bomcategorymaster")
                                {
                                    CategoryMasterDAL objBinMasterDal = new CategoryMasterDAL(base.DataBaseName);
                                    var result = objBinMasterDal.GetCategoryByCatID(Convert.ToInt64(item));
                                    if (result != null && result.Category != "")
                                    {
                                        strError = strError == "" ? result.Category : strError + ", " + result.Category;
                                    }
                                    else
                                    {
                                        strBlankMsg += "error";
                                        //strError = strError == "" ? item : strError + "," + item;
                                    }
                                }
                                else if (TableName.ToLower() == "CostUOMMaster")
                                {
                                    CostUOMMasterDAL objBinMasterDal = new CostUOMMasterDAL(base.DataBaseName);
                                    var result = objBinMasterDal.GetCostUOMByID(Convert.ToInt64(item));
                                    if (result != null && result.CostUOM != "")
                                    {
                                        strError = strError == "" ? result.CostUOM : strError + ", " + result.CostUOM;
                                    }
                                    else
                                    {
                                        strBlankMsg += "error";
                                        //strError = strError == "" ? item : strError + "," + item;
                                    }
                                }
                                else if (TableName.ToLower() == "InventoryClassificationMaster")
                                {
                                    InventoryClassificationMasterDAL objBinMasterDal = new InventoryClassificationMasterDAL(base.DataBaseName);
                                    var result = objBinMasterDal.GetInventoryClassificationByIDPlain(Convert.ToInt64(item));
                                    if (result != null && result.InventoryClassification != "")
                                    {
                                        strError = strError == "" ? result.InventoryClassification : strError + ", " + result.InventoryClassification;
                                    }
                                    else
                                    {
                                        strBlankMsg += "error";
                                        //strError = strError == "" ? item : strError + "," + item;
                                    }
                                }
                                else if (TableName.ToLower() == "bominventoryclassificationmaster")
                                {
                                    InventoryClassificationMasterDAL objBinMasterDal = new InventoryClassificationMasterDAL(base.DataBaseName);
                                    var result = objBinMasterDal.GetInventoryClassificationByIDPlain(Convert.ToInt64(item));
                                    if (result != null && result.InventoryClassification != "")
                                    {
                                        strError = strError == "" ? result.InventoryClassification : strError + ", " + result.InventoryClassification;
                                    }
                                    else
                                    {
                                        strBlankMsg += "error";
                                        //strError = strError == "" ? item : strError + "," + item;
                                    }
                                }
                                else if (TableName.ToLower() == "customermaster")
                                {
                                    CustomerMasterDAL objBinMasterDal = new CustomerMasterDAL(base.DataBaseName);
                                    var result = objBinMasterDal.GetCustomerByID(Convert.ToInt64(item));
                                    if (result != null && result.Customer != "")
                                    {
                                        strError = strError == "" ? result.Customer : strError + ", " + result.Customer;
                                    }
                                    else
                                    {
                                        strBlankMsg += "error";
                                        //strError = strError == "" ? item : strError + "," + item;
                                    }
                                }
                                else if (TableName.ToLower() == "suppliermaster")
                                {
                                    SupplierMasterDAL objBinMasterDal = new SupplierMasterDAL(base.DataBaseName);
                                    var result = objBinMasterDal.GetSupplierByIDPlain(Convert.ToInt64(item));
                                    if (result != null && result.SupplierName != "")
                                    {
                                        strError = strError == "" ? result.SupplierName : strError + ", " + result.SupplierName;
                                    }
                                    else
                                    {
                                        strBlankMsg += "error";
                                        //strError = strError == "" ? item : strError + "," + item;
                                    }
                                }
                                else if (TableName.ToLower() == "bomsuppliermaster")
                                {
                                    SupplierMasterDAL objBinMasterDal = new SupplierMasterDAL(base.DataBaseName);
                                    var result = objBinMasterDal.GetSupplierByIDPlain(Convert.ToInt64(item));
                                    if (result != null && result.SupplierName != "")
                                    {
                                        strError = strError == "" ? result.SupplierName : strError + ", " + result.SupplierName;
                                    }
                                    else
                                    {
                                        strBlankMsg += "error";
                                        //strError = strError == "" ? item : strError + "," + item;
                                    }
                                }
                                else
                                {
                                    strBlankMsg += "error";
                                }
                            }
                        }
                    }
                }
                if (strDeleteIds != "")
                {
                    if (TableName == ImportMastersDTO.TableName.BinMaster.ToString())
                    {
                        //IEnumerable<BinMasterDTO> ObjCache = CacheHelper<IEnumerable<BinMasterDTO>>.GetCacheItem("Cached_BinMaster_" + CompanyId.ToString());
                        //if (ObjCache != null)
                        //{
                        //    List<BinMasterDTO> objTemp = ObjCache.ToList();
                        //    objTemp.RemoveAll(i => strDeleteIds.Split(',').Contains(i.ID.ToString()));
                        //    ObjCache = objTemp.AsEnumerable();
                        //    CacheHelper<IEnumerable<BinMasterDTO>>.AppendToCacheItem("Cached_BinMaster_" + CompanyId.ToString(), ObjCache);
                        //}
                    }
                    else if (TableName == ImportMastersDTO.TableName.CategoryMaster.ToString())
                    {
                        //IEnumerable<CategoryMasterDTO> ObjCache = CacheHelper<IEnumerable<CategoryMasterDTO>>.GetCacheItem("Cached_CategoryMaster_" + CompanyId.ToString());
                        //if (ObjCache != null)
                        //{
                        //    List<CategoryMasterDTO> objTemp = ObjCache.ToList();
                        //    objTemp.RemoveAll(i => strDeleteIds.Split(',').Contains(i.ID.ToString()));
                        //    ObjCache = objTemp.AsEnumerable();
                        //    CacheHelper<IEnumerable<CategoryMasterDTO>>.AppendToCacheItem("Cached_CategoryMaster_" + CompanyId.ToString(), ObjCache);
                        //}
                    }
                    //else if (TableName == ImportMastersDTO.TableName.CostUOMMaster.ToString())
                    //{
                    //    IEnumerable<CostUOMMasterDTO> ObjCache = CacheHelper<IEnumerable<CostUOMMasterDTO>>.GetCacheItem("Cached_CostUOMMaster_" + CompanyId.ToString());
                    //    if (ObjCache != null)
                    //    {
                    //        List<CostUOMMasterDTO> objTemp = ObjCache.ToList();
                    //        objTemp.RemoveAll(i => strDeleteIds.Split(',').Contains(i.ID.ToString()));
                    //        ObjCache = objTemp.AsEnumerable();
                    //        CacheHelper<IEnumerable<CostUOMMasterDTO>>.AppendToCacheItem("Cached_CostUOMMaster_" + CompanyId.ToString(), ObjCache);
                    //    }
                    //}
                    else if (TableName == ImportMastersDTO.TableName.InventoryClassificationMaster.ToString())
                    {
                        //IEnumerable<InventoryClassificationMasterDTO> ObjCache = CacheHelper<IEnumerable<InventoryClassificationMasterDTO>>.GetCacheItem("Cached_InventoryClassificationMaster_" + CompanyId.ToString());
                        //if (ObjCache != null)
                        //{
                        //    List<InventoryClassificationMasterDTO> objTemp = ObjCache.ToList();
                        //    objTemp.RemoveAll(i => strDeleteIds.Split(',').Contains(i.ID.ToString()));
                        //    ObjCache = objTemp.AsEnumerable();
                        //    CacheHelper<IEnumerable<InventoryClassificationMasterDTO>>.AppendToCacheItem("Cached_InventoryClassificationMaster_" + CompanyId.ToString(), ObjCache);
                        //}
                    }
                    else if (TableName == ImportMastersDTO.TableName.CustomerMaster.ToString())
                    {
                        //IEnumerable<CustomerMasterDTO> ObjCache = CacheHelper<IEnumerable<CustomerMasterDTO>>.GetCacheItem("Cached_CustomerMaster_" + CompanyId.ToString());
                        //if (ObjCache != null)
                        //{
                        //    List<CustomerMasterDTO> objTemp = ObjCache.ToList();
                        //    objTemp.RemoveAll(i => strDeleteIds.Split(',').Contains(i.ID.ToString()));
                        //    ObjCache = objTemp.AsEnumerable();
                        //    CacheHelper<IEnumerable<CustomerMasterDTO>>.AppendToCacheItem("Cached_CustomerMaster_" + CompanyId.ToString(), ObjCache);
                        //}
                    }
                    else if (TableName == ImportMastersDTO.TableName.FreightTypeMaster.ToString())
                    {
                        //IEnumerable<FreightTypeMasterDTO> ObjCache = CacheHelper<IEnumerable<FreightTypeMasterDTO>>.GetCacheItem("Cached_FreightTypeMaster_" + CompanyId.ToString());
                        //if (ObjCache != null)
                        //{
                        //    List<FreightTypeMasterDTO> objTemp = ObjCache.ToList();
                        //    objTemp.RemoveAll(i => strDeleteIds.Split(',').Contains(i.ID.ToString()));
                        //    ObjCache = objTemp.AsEnumerable();
                        //    CacheHelper<IEnumerable<FreightTypeMasterDTO>>.AppendToCacheItem("Cached_FreightTypeMaster_" + CompanyId.ToString(), ObjCache);
                        //}

                    }
                    else if (TableName == ImportMastersDTO.TableName.GLAccountMaster.ToString())
                    {
                        //IEnumerable<GLAccountMasterDTO> ObjCache = CacheHelper<IEnumerable<GLAccountMasterDTO>>.GetCacheItem("Cached_GLAccountMaster_" + CompanyId.ToString());
                        //if (ObjCache != null)
                        //{
                        //    List<GLAccountMasterDTO> objTemp = ObjCache.ToList();
                        //    objTemp.RemoveAll(i => strDeleteIds.Split(',').Contains(i.ID.ToString()));
                        //    ObjCache = objTemp.AsEnumerable();
                        //    CacheHelper<IEnumerable<GLAccountMasterDTO>>.AppendToCacheItem("Cached_GLAccountMaster_" + CompanyId.ToString(), ObjCache);
                        //}
                    }
                    else if (TableName == ImportMastersDTO.TableName.GXPRConsigmentJobMaster.ToString())
                    {
                        IEnumerable<GXPRConsigmentJobMasterDTO> ObjCache = CacheHelper<IEnumerable<GXPRConsigmentJobMasterDTO>>.GetCacheItem("Cached_GXPRConsigmentJobMaster_" + CompanyId.ToString());
                        if (ObjCache != null)
                        {
                            List<GXPRConsigmentJobMasterDTO> objTemp = ObjCache.ToList();
                            objTemp.RemoveAll(i => strDeleteIds.Split(',').Contains(i.ID.ToString()));
                            ObjCache = objTemp.AsEnumerable();
                            CacheHelper<IEnumerable<GXPRConsigmentJobMasterDTO>>.AppendToCacheItem("Cached_GXPRConsigmentJobMaster_" + CompanyId.ToString(), ObjCache);
                        }
                    }
                    else if (TableName == ImportMastersDTO.TableName.JobTypeMaster.ToString())
                    {
                        //IEnumerable<JobTypeMasterDTO> ObjCache = CacheHelper<IEnumerable<JobTypeMasterDTO>>.GetCacheItem("Cached_JobTypeMaster_" + CompanyId.ToString());
                        //if (ObjCache != null)
                        //{
                        //    List<JobTypeMasterDTO> objTemp = ObjCache.ToList();
                        //    objTemp.RemoveAll(i => strDeleteIds.Split(',').Contains(i.ID.ToString()));
                        //    ObjCache = objTemp.AsEnumerable();
                        //    CacheHelper<IEnumerable<JobTypeMasterDTO>>.AppendToCacheItem("Cached_JobTypeMaster_" + CompanyId.ToString(), ObjCache);
                        //}
                    }
                    else if (TableName == ImportMastersDTO.TableName.SupplierMaster.ToString())
                    {
                        //IEnumerable<SupplierMasterDTO> ObjCache = CacheHelper<IEnumerable<SupplierMasterDTO>>.GetCacheItem("Cached_SupplierMaster_" + CompanyId.ToString());
                        //if (ObjCache != null)
                        //{
                        //    List<SupplierMasterDTO> objTemp = ObjCache.ToList();
                        //    objTemp.RemoveAll(i => strDeleteIds.Split(',').Contains(i.ID.ToString()));
                        //    ObjCache = objTemp.AsEnumerable();
                        //    CacheHelper<IEnumerable<SupplierMasterDTO>>.AppendToCacheItem("Cached_SupplierMaster_" + CompanyId.ToString(), ObjCache);
                        //}
                    }
                    else if (TableName == ImportMastersDTO.TableName.ShipViaMaster.ToString())
                    {
                        //IEnumerable<ShipViaDTO> ObjCache = CacheHelper<IEnumerable<ShipViaDTO>>.GetCacheItem("Cached_ShipVia_" + CompanyId.ToString());
                        //if (ObjCache != null)
                        //{
                        //    List<ShipViaDTO> objTemp = ObjCache.ToList();
                        //    objTemp.RemoveAll(i => strDeleteIds.Split(',').Contains(i.ID.ToString()));
                        //    ObjCache = objTemp.AsEnumerable();
                        //    CacheHelper<IEnumerable<ShipViaDTO>>.AppendToCacheItem("Cached_ShipVia_" + CompanyId.ToString(), ObjCache);
                        //}
                    }
                    else if (TableName == ImportMastersDTO.TableName.TechnicianMaster.ToString())
                    {
                        //IEnumerable<TechnicianMasterDTO> ObjCache = CacheHelper<IEnumerable<TechnicianMasterDTO>>.GetCacheItem("Cached_TechnicianMaster_" + CompanyId.ToString());
                        //if (ObjCache != null)
                        //{
                        //    List<TechnicianMasterDTO> objTemp = ObjCache.ToList();
                        //    objTemp.RemoveAll(i => strDeleteIds.Split(',').Contains(i.ID.ToString()));
                        //    ObjCache = objTemp.AsEnumerable();
                        //    CacheHelper<IEnumerable<TechnicianMasterDTO>>.AppendToCacheItem("Cached_TechnicianMaster_" + CompanyId.ToString(), ObjCache);
                        //}
                    }
                    else if (TableName == ImportMastersDTO.TableName.UnitMaster.ToString())
                    {
                        //IEnumerable<UnitMasterDTO> ObjCache = CacheHelper<IEnumerable<UnitMasterDTO>>.GetCacheItem("Cached_UnitMaster_" + CompanyId.ToString());
                        //if (ObjCache != null)
                        //{
                        //    List<UnitMasterDTO> objTemp = ObjCache.ToList();
                        //    objTemp.RemoveAll(i => strDeleteIds.Split(',').Contains(i.ID.ToString()));
                        //    ObjCache = objTemp.AsEnumerable();
                        //    CacheHelper<IEnumerable<UnitMasterDTO>>.AppendToCacheItem("Cached_UnitMaster_" + CompanyId.ToString(), ObjCache);
                        //}
                    }
                    else if (TableName == ImportMastersDTO.TableName.ManufacturerMaster.ToString())
                    {
                        //IEnumerable<ManufacturerMasterDTO> ObjCache = CacheHelper<IEnumerable<ManufacturerMasterDTO>>.GetCacheItem("Cached_ManufacturerMaster_" + CompanyId.ToString());
                        //if (ObjCache != null)
                        //{
                        //    List<ManufacturerMasterDTO> objTemp = ObjCache.ToList();
                        //    objTemp.RemoveAll(i => strDeleteIds.Split(',').Contains(i.ID.ToString()));
                        //    ObjCache = objTemp.AsEnumerable();
                        //    CacheHelper<IEnumerable<ManufacturerMasterDTO>>.AppendToCacheItem("Cached_ManufacturerMaster_" + CompanyId.ToString(), ObjCache);
                        //}
                    }
                    else if (TableName == ImportMastersDTO.TableName.MeasurementTermMaster.ToString())
                    {
                        IEnumerable<MeasurementTermMasterDTO> ObjCache = CacheHelper<IEnumerable<MeasurementTermMasterDTO>>.GetCacheItem("Cached_MeasurementTermMaster_" + CompanyId.ToString());
                        if (ObjCache != null)
                        {
                            List<MeasurementTermMasterDTO> objTemp = ObjCache.ToList();
                            objTemp.RemoveAll(i => strDeleteIds.Split(',').Contains(i.ID.ToString()));
                            ObjCache = objTemp.AsEnumerable();
                            CacheHelper<IEnumerable<MeasurementTermMasterDTO>>.AppendToCacheItem("Cached_MeasurementTermMaster_" + CompanyId.ToString(), ObjCache);
                        }
                    }
                    else if (TableName == ImportMastersDTO.TableName.ItemMaster.ToString())
                    {
                        //IEnumerable<ItemMasterDTO> ObjCache = CacheHelper<IEnumerable<ItemMasterDTO>>.GetCacheItem("Cached_ItemMaster_" + CompanyId.ToString());
                        //if (ObjCache != null)
                        //{
                        //    List<ItemMasterDTO> objTemp = ObjCache.ToList();
                        //    objTemp.RemoveAll(i => strDeleteIds.Split(',').Contains(i.GUID.ToString()));
                        //    ObjCache = objTemp.AsEnumerable();
                        //    CacheHelper<IEnumerable<ItemMasterDTO>>.AppendToCacheItem("Cached_ItemMaster_" + CompanyId.ToString(), ObjCache);
                        //}
                    }
                }
                if (strBlankMsg == "")
                {
                    return strError != "" ? "(" + strError + ")" : "ok";
                }
                else
                {
                    return "";
                }
            }
            catch (EntitySqlException ex)
            {
                return ex.Message;
            }
        }

        public ResponseMessage CheckQuantityByLocation(Int64 LocationID, Guid ItemGUID, double Quantity, Int64 RoomID, Int64 CompanyID, long EnterPriseID, long UserID)
        {

            ResponseMessage response = new ResponseMessage();

            //ItemLocationQTYDTO lstLocDTO = new ItemLocationQTYDAL(base.DataBaseName).GetAllRecords(RoomID, CompanyID).Where(x => x.BinID == LocationID && x.ItemGUID == ItemGUID).SingleOrDefault();
            ItemLocationQTYDTO lstLocDTO = new ItemLocationQTYDAL(base.DataBaseName).GetItemLocationQTY(RoomID, CompanyID, LocationID, Convert.ToString(ItemGUID)).FirstOrDefault();
            BinMasterDTO objBINDTO = new BinMasterDAL(base.DataBaseName).GetBinByID(LocationID, RoomID, CompanyID);
            //BinMasterDTO objBINDTO = new BinMasterDAL(base.DataBaseName).GetItemLocation( RoomID, CompanyID, false, false,Guid.Empty, LocationID,null,null).FirstOrDefault();
            ItemMasterDTO ItemDTO = new ItemMasterDAL(base.DataBaseName).GetItemWithoutJoins(null, ItemGUID);
            string LocationName = objBINDTO.BinNumber;
            string currentCulture = "en-US";
            if (HttpContext.Current != null)
            {
                if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                {
                    if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                    {
                        currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                    }

                }
            }
            else
            {
                eTurns.DTO.eTurnsRegionInfo objeTurnsRegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomID, CompanyID, UserID);
                currentCulture = objeTurnsRegionInfo.CultureName;
            }
            string ResourceFileKitMaster = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResKitMaster", currentCulture, EnterPriseID, CompanyID);
            string ResourcePullMaster = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResPullMaster", currentCulture, EnterPriseID, CompanyID);
            if (Quantity <= 0)
            {
                string MsgQuantityGreaterZero = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgQuantityGreaterZero", ResourceFileKitMaster, EnterPriseID, CompanyID, RoomID, "ResKitMaster", currentCulture);
                response.Message = MsgQuantityGreaterZero;
                response.IsSuccess = false;

            }
            else if (lstLocDTO != null)
            {
                Double AvailableQuantity = 0;
                if (ItemDTO.Consignment)
                {
                    AvailableQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) + lstLocDTO.ConsignedQuantity.GetValueOrDefault(0);
                    string NotEnoughCustAndConsQtyForLocation = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotEnoughCustAndConsQtyForLocation", ResourcePullMaster, EnterPriseID, CompanyID, RoomID, "ResPullMaster", currentCulture);
                    response.Message = NotEnoughCustAndConsQtyForLocation.Replace("{0}", LocationName).Replace("{1}", lstLocDTO.Quantity.ToString());
                }
                else
                {
                    AvailableQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0);
                    string NotEnoughCustomerQuantityForLocation = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotEnoughCustomerQuantityForLocation", ResourcePullMaster, EnterPriseID, CompanyID, RoomID, "ResPullMaster", currentCulture);
                    response.Message = NotEnoughCustomerQuantityForLocation.Replace("{0}", LocationName).Replace("{1}", Convert.ToString(lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0)));

                }

                if (Quantity <= AvailableQuantity)
                {
                    string MsgQuantityAvailable = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgQuantityAvailable", ResourceFileKitMaster, EnterPriseID, CompanyID, RoomID, "ResKitMaster", currentCulture);
                    response.Message = MsgQuantityAvailable;
                    response.IsSuccess = true;
                }
                else
                {
                    response.IsSuccess = false;
                }
            }
            else
            {
                string NotEnoughQuantityForLocation = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotEnoughQuantityForLocation", ResourcePullMaster, EnterPriseID, CompanyID, RoomID, "ResPullMaster", currentCulture);
                response.Message = NotEnoughQuantityForLocation.Replace("{0}", LocationName).Replace("{1}", Convert.ToString(0));
                response.IsSuccess = false;
            }

            return response;
        }

        public ResponseMessage CheckQuantityByStagingLocation(Guid MaterialStagingGUID, Int64 StagingBinID, Guid ItemGUID, double Quantity, Int64 RoomID, Int64 CompanyID, Int64 EnterpriseId, Int64 UserId)
        {
            ResponseMessage response = new ResponseMessage();
            MaterialStagingDetailDAL objMSDetailDAL = new MaterialStagingDetailDAL(base.DataBaseName);
            //MaterialStagingDetailDTO objMSDetailDTO = objMSDetailDAL.GetAllRecords(RoomID, CompanyID).Where(x => x.MaterialStagingGUID == MaterialStagingGUID && x.ItemGUID == ItemGUID && x.StagingBinID == StagingBinID && x.IsDeleted == false && x.IsArchived == false).FirstOrDefault();
            MaterialStagingDetailDTO objMSDetailDTO = objMSDetailDAL.GetMaterialStagingDetailbyItemGUIDStagingBINID(Convert.ToString(MaterialStagingGUID), Convert.ToString(ItemGUID), StagingBinID, RoomID, CompanyID, false, false).FirstOrDefault();

            BinMasterDTO objBINDTO = new BinMasterDAL(base.DataBaseName).GetBinByID(StagingBinID, RoomID, CompanyID);
            //            BinMasterDTO objBINDTO = new BinMasterDAL(base.DataBaseName).GetItemLocation( RoomID, CompanyID, false, false,Guid.Empty, StagingBinID,null,null).FirstOrDefault();

            string currentCulture = "en-US";
            if (HttpContext.Current != null)
            {
                if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                {
                    if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                    {
                        currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                    }

                }
            }
            else
            {
                eTurns.DTO.eTurnsRegionInfo objeTurnsRegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomID, CompanyID, UserId);
                currentCulture = objeTurnsRegionInfo.CultureName;
            }
            string ResourceFileKitMaster = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResKitMaster", currentCulture, EnterpriseId, CompanyID);
            string ResourcePullMaster = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResPullMaster", currentCulture, EnterpriseId, CompanyID);
            if (Quantity <= 0)
            {
                string MsgQuantityGreaterZero = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgQuantityGreaterZero", ResourceFileKitMaster, EnterpriseId, CompanyID, RoomID, "ResKitMaster", currentCulture);
                response.Message = MsgQuantityGreaterZero;
                response.IsSuccess = false;
            }
            else if (objMSDetailDTO != null)
            {
                Double AvailableQuantity = objMSDetailDTO.Quantity;
                if (Quantity <= AvailableQuantity)
                {
                    string MsgQuantityAvailable = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgQuantityAvailable", ResourceFileKitMaster, EnterpriseId, CompanyID, RoomID, "ResKitMaster", currentCulture);
                    response.Message = MsgQuantityAvailable;
                    response.IsSuccess = true;
                }
                else
                {
                    response.IsSuccess = false;
                    string NotEnoughQuantityForLocation = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotEnoughQuantityForLocation", ResourcePullMaster, EnterpriseId, CompanyID, RoomID, "ResPullMaster", currentCulture);
                    response.Message = NotEnoughQuantityForLocation.Replace("{0}", objBINDTO.BinNumber).Replace("{1}", objMSDetailDTO.Quantity.ToString());
                }
            }
            else
            {
                string NotEnoughQuantityForLocation = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotEnoughQuantityForLocation", ResourcePullMaster, EnterpriseId, CompanyID, RoomID, "ResPullMaster", currentCulture);
                response.Message = NotEnoughQuantityForLocation.Replace("{0}", objBINDTO.BinNumber).Replace("{1}", Convert.ToString("0"));
                response.IsSuccess = false;
            }

            return response;
        }

        public List<BinMasterDTO> GetLocationWithQuantity(Guid ItemGUID, Int64 RoomID, Int64 CompanyID)
        {

            List<BinMasterDTO> lstBins = new List<BinMasterDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var result = (from A in context.ItemLocationDetails
                              join E in context.ItemMasters on new { ItemGUID = (A.ItemGUID.GetValueOrDefault(Guid.Empty)) } equals new { ItemGUID = E.GUID } into E_join
                              from E in E_join.DefaultIfEmpty()
                              join F in context.BinMasters on new { BinID = ((Int64?)A.BinID ?? 0) } equals new { BinID = F.ID } into F_join
                              from F in F_join.DefaultIfEmpty()
                              where F.IsStagingLocation == false && A.ItemGUID == ItemGUID && F.Room == RoomID && F.CompanyID == CompanyID &&
                                 A.Room == RoomID && A.CompanyID == CompanyID && E.Room == RoomID && E.CompanyID == CompanyID
                              group new { F, E, A } by new { F.ID, F.BinNumber } into g
                              select new
                              {
                                  ID = g.Key.ID,
                                  BinNumber = g.Key.BinNumber,
                                  Quantity = g.Sum(p => (((System.Boolean?)p.E.Consignment ?? false) == true ? (((System.Double?)p.A.CustomerOwnedQuantity ?? 0) + ((System.Double?)p.A.ConsignedQuantity ?? 0)) : (((System.Double?)p.A.CustomerOwnedQuantity ?? 0))))
                              }
                              );

                if (result != null && result.Count() > 0)
                {
                    foreach (var item in result)
                    {
                        lstBins.Add(new BinMasterDTO() { ID = item.ID, BinNumber = item.BinNumber + " (" + Int64.Parse(item.Quantity.ToString()) + ")" });
                    }
                }

            }

            return lstBins;

        }

        /// <summary>
        /// Get Narrow Search Data
        /// </summary>
        /// <param name="PageName"></param>
        /// <param name="CompanyID"></param>
        /// <param name="RoomID"></param>
        /// <param name="IsArchived"></param>
        /// <param name="IsDeleted"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        public NarrowSearchData GetNarrowSearchDataFromCache(string PageName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted, string Status, List<long> SupplierIds, string MainFilter = "", OrderType OrdType = OrderType.Order, ToolAssetOrderType ToolAssetOrderTypeOrdType = ToolAssetOrderType.Order)
        {
            NarrowSearchData objNarrowSearch = null;

            #region OrderMaster
            if (PageName.ToUpper() == "ORDERMASTER")
            {
                objNarrowSearch = new NarrowSearchData();
                OrderMasterDAL objOrderDAL = null;
                List<CommonDTO> StatusByList = null;
                List<CommonDTO> ReuiredDateList = null;
                List<CommonDTO> lstUDF1 = null;
                List<CommonDTO> lstUDF2 = null;
                List<CommonDTO> lstUDF3 = null;
                List<CommonDTO> lstUDF4 = null;
                List<CommonDTO> lstUDF5 = null;

                try
                {
                    objOrderDAL = new OrderMasterDAL(base.DataBaseName);
                    IEnumerable<CommonDTO> lstCommonDTO = objOrderDAL.DB_GetOrderNarrowSearchData(CompanyID, RoomID, IsArchived, IsDeleted, Status, SupplierIds, OrdType);
                    objNarrowSearch.SupplierByList = lstCommonDTO.Where(t => t.PageName == "SupplierByCount").ToList();
                    StatusByList = lstCommonDTO.Where(t => t.PageName == "OrderStatusByCount").ToList();
                    ReuiredDateList = lstCommonDTO.Where(t => t.PageName == "RequiredDateByCount").ToList();
                    objNarrowSearch.CreatedByList = lstCommonDTO.Where(t => t.PageName == "CreatedByCount").ToList();
                    objNarrowSearch.UpdatedByList = lstCommonDTO.Where(t => t.PageName == "LastUpdatedByCount").ToList();
                    objNarrowSearch.VendorList = lstCommonDTO.Where(t => t.PageName == "OrderShippingVendorByCount" && t.Text != null).ToList();
                    objNarrowSearch.PackslipNumberList = lstCommonDTO.Where(t => t.PageName == "OrderPackslipNumberByCount" && t.Text != null).ToList();
                    lstUDF1 = lstCommonDTO.Where(t => t.PageName == "UDF1ByCount").ToList();
                    lstUDF2 = lstCommonDTO.Where(t => t.PageName == "UDF2ByCount").ToList();
                    lstUDF3 = lstCommonDTO.Where(t => t.PageName == "UDF3ByCount").ToList();
                    lstUDF4 = lstCommonDTO.Where(t => t.PageName == "UDF4ByCount").ToList();
                    lstUDF5 = lstCommonDTO.Where(t => t.PageName == "UDF5ByCount").ToList();

                    StatusByList.ForEach(x => x.Text = ResOrder.GetOrderStatusText(Enum.Parse(typeof(OrderStatus), x.Text, true).ToString()));
                    objNarrowSearch.StatusByList = StatusByList;

                    if (lstUDF1.Count > 0)
                        objNarrowSearch.UDF1List = lstUDF1;
                    if (lstUDF2.Count > 0)
                        objNarrowSearch.UDF2List = lstUDF2;
                    if (lstUDF3.Count > 0)
                        objNarrowSearch.UDF3List = lstUDF3;
                    if (lstUDF4.Count > 0)
                        objNarrowSearch.UDF4List = lstUDF4;
                    if (lstUDF5.Count > 0)
                        objNarrowSearch.UDF5List = lstUDF5;

                    objNarrowSearch.RequiredDateList = ReuiredDateList;
                    List<CommonDTO> changeOrdList = new List<CommonDTO>();
                    changeOrdList.Add(new CommonDTO() { Text = "Changed Orders", ID = 1, Count = 0 });
                    changeOrdList.Add(new CommonDTO() { Text = "Changeble Orders", ID = 2, Count = 0 });
                    objNarrowSearch.ChangeOrderList = changeOrdList;

                    return objNarrowSearch;
                }
                finally
                {
                    objOrderDAL = null;
                    StatusByList = null;
                    objNarrowSearch = null;
                    ReuiredDateList = null;
                    lstUDF1 = null;
                    lstUDF2 = null;
                    lstUDF3 = null;
                    lstUDF4 = null;
                    lstUDF5 = null;
                    objNarrowSearch = null;
                }

            }
            #endregion
            #region ToolAssetOrderMaster
            if (PageName.ToUpper() == "TOOLASSETORDERMASTER")
            {
                objNarrowSearch = new NarrowSearchData();
                ToolAssetOrderMasterDAL objOrderDAL = null;
                List<CommonDTO> StatusByList = null;
                List<CommonDTO> ReuiredDateList = null;
                List<CommonDTO> lstUDF1 = null;
                List<CommonDTO> lstUDF2 = null;
                List<CommonDTO> lstUDF3 = null;
                List<CommonDTO> lstUDF4 = null;
                List<CommonDTO> lstUDF5 = null;
                try
                {
                    objOrderDAL = new ToolAssetOrderMasterDAL(base.DataBaseName);
                    IEnumerable<CommonDTO> lstCommonDTO = objOrderDAL.DB_GetOrderNarrowSearchData(CompanyID, RoomID, IsArchived, IsDeleted, Status, ToolAssetOrderTypeOrdType);
                    StatusByList = lstCommonDTO.Where(t => t.PageName == "OrderStatusByCount").ToList();
                    ReuiredDateList = lstCommonDTO.Where(t => t.PageName == "RequiredDateByCount").ToList();
                    objNarrowSearch.CreatedByList = lstCommonDTO.Where(t => t.PageName == "CreatedByCount").ToList();
                    objNarrowSearch.UpdatedByList = lstCommonDTO.Where(t => t.PageName == "LastUpdatedByCount").ToList();
                    objNarrowSearch.VendorList = lstCommonDTO.Where(t => t.PageName == "OrderShippingVendorByCount" && t.Text != null).ToList();
                    lstUDF1 = lstCommonDTO.Where(t => t.PageName == "UDF1ByCount").ToList();
                    lstUDF2 = lstCommonDTO.Where(t => t.PageName == "UDF2ByCount").ToList();
                    lstUDF3 = lstCommonDTO.Where(t => t.PageName == "UDF3ByCount").ToList();
                    lstUDF4 = lstCommonDTO.Where(t => t.PageName == "UDF4ByCount").ToList();
                    lstUDF5 = lstCommonDTO.Where(t => t.PageName == "UDF5ByCount").ToList();

                    StatusByList.ForEach(x => x.Text = ResOrder.GetOrderStatusText(Enum.Parse(typeof(OrderStatus), x.Text, true).ToString()));
                    objNarrowSearch.StatusByList = StatusByList;

                    if (lstUDF1.Count > 0)
                        objNarrowSearch.UDF1List = lstUDF1;
                    if (lstUDF2.Count > 0)
                        objNarrowSearch.UDF2List = lstUDF2;
                    if (lstUDF3.Count > 0)
                        objNarrowSearch.UDF3List = lstUDF3;
                    if (lstUDF4.Count > 0)
                        objNarrowSearch.UDF4List = lstUDF4;
                    if (lstUDF5.Count > 0)
                        objNarrowSearch.UDF5List = lstUDF5;

                    objNarrowSearch.RequiredDateList = ReuiredDateList;
                    List<CommonDTO> changeOrdList = new List<CommonDTO>();
                    changeOrdList.Add(new CommonDTO() { Text = "Changed Orders", ID = 1, Count = 0 });
                    changeOrdList.Add(new CommonDTO() { Text = "Changeble Orders", ID = 2, Count = 0 });

                    objNarrowSearch.ChangeOrderList = changeOrdList;

                    return objNarrowSearch;



                }
                finally
                {
                    objOrderDAL = null;
                    StatusByList = null;
                    objNarrowSearch = null;
                    ReuiredDateList = null;
                    lstUDF1 = null;
                    lstUDF2 = null;
                    lstUDF3 = null;
                    lstUDF4 = null;
                    lstUDF5 = null;
                    objNarrowSearch = null;
                }

            }
            # endregion
            #region TransferMaster

            if (PageName.ToUpper() == "TRANSFERMASTER")
            {
                objNarrowSearch = new NarrowSearchData();
                TransferMasterDAL objTransferDAL = null;
                List<CommonDTO> ItemLocationList = null;
                List<CommonDTO> CreatedByList = null;
                List<CommonDTO> UpdateByList = null;
                List<CommonDTO> StatusByList = null;
                List<CommonDTO> ReuiredDateList = null;
                List<CommonDTO> lstUDF1 = null;
                List<CommonDTO> lstUDF2 = null;
                List<CommonDTO> lstUDF3 = null;
                List<CommonDTO> lstUDF4 = null;
                List<CommonDTO> lstUDF5 = null;
                try
                {
                    objTransferDAL = new TransferMasterDAL(base.DataBaseName);
                    IEnumerable<CommonDTO> lstCommonDTO = objTransferDAL.DB_GetTransferNarrowSearchData(CompanyID, RoomID, IsArchived, IsDeleted, Status, MainFilter);
                    StatusByList = lstCommonDTO.Where(t => t.PageName == "TransferStatusByCount").ToList();
                    CreatedByList = lstCommonDTO.Where(t => t.PageName == "CreatedByCount").ToList();
                    UpdateByList = lstCommonDTO.Where(t => t.PageName == "LastUpdatedByCount").ToList();
                    lstUDF1 = lstCommonDTO.Where(t => t.PageName == "UDF1ByCount").ToList();
                    lstUDF2 = lstCommonDTO.Where(t => t.PageName == "UDF2ByCount").ToList();
                    lstUDF3 = lstCommonDTO.Where(t => t.PageName == "UDF3ByCount").ToList();
                    lstUDF4 = lstCommonDTO.Where(t => t.PageName == "UDF4ByCount").ToList();
                    lstUDF5 = lstCommonDTO.Where(t => t.PageName == "UDF5ByCount").ToList();
                    ItemLocationList = lstCommonDTO.Where(t => t.PageName == "ItemLocationByCount").ToList();
                    objNarrowSearch.CreatedByList = CreatedByList;
                    objNarrowSearch.UpdatedByList = UpdateByList;
                    objNarrowSearch.ItemLocationList = ItemLocationList;
                    StatusByList.ForEach(x => x.Text = ResOrder.GetOrderStatusText(Enum.Parse(typeof(TransferStatus), x.Text, true).ToString()));
                    objNarrowSearch.StatusByList = StatusByList;
                    UDFDAL _repository = new UDFDAL(base.DataBaseName);
                    IEnumerable<UDFDTO> DataFromDB = _repository.GetUDFsByUDFTableNamePlain("TransferMaster", RoomID, CompanyID);

                    if (ISUDFForNarrowSearch(DataFromDB, "UDF1"))
                    {
                        objNarrowSearch.UDF1List = lstUDF1;
                    }
                    if (ISUDFForNarrowSearch(DataFromDB, "UDF2"))
                    {
                        objNarrowSearch.UDF2List = lstUDF2;
                    }
                    if (ISUDFForNarrowSearch(DataFromDB, "UDF3"))
                    {
                        objNarrowSearch.UDF3List = lstUDF3;
                    }
                    if (ISUDFForNarrowSearch(DataFromDB, "UDF4"))
                    {
                        objNarrowSearch.UDF4List = lstUDF4;
                    }
                    if (ISUDFForNarrowSearch(DataFromDB, "UDF5"))
                    {
                        objNarrowSearch.UDF5List = lstUDF5;
                    }

                    if (lstUDF1.Count > 0)
                        objNarrowSearch.UDF1List = lstUDF1;
                    if (lstUDF2.Count > 0)
                        objNarrowSearch.UDF2List = lstUDF2;
                    if (lstUDF3.Count > 0)
                        objNarrowSearch.UDF3List = lstUDF3;
                    if (lstUDF4.Count > 0)
                        objNarrowSearch.UDF4List = lstUDF4;
                    if (lstUDF5.Count > 0)
                        objNarrowSearch.UDF5List = lstUDF5;

                    ReuiredDateList = new List<CommonDTO>();
                    ReuiredDateList.Add(new CommonDTO() { Text = "> 3 weeks", ID = 1, Count = 0 }); //ResOrder.MoreThanThreeWeeks
                    ReuiredDateList.Add(new CommonDTO() { Text = "2-3 weeks", ID = 2, Count = 0 }); //ResOrder.TwoToThreeWeeks
                    ReuiredDateList.Add(new CommonDTO() { Text = "Next weeks", ID = 3, Count = 0 }); //ResOrder.NextWeek
                    ReuiredDateList.Add(new CommonDTO() { Text = "This weeks", ID = 4, Count = 0 }); //ResOrder.ThisWeek
                    objNarrowSearch.RequiredDateList = ReuiredDateList;
                    return objNarrowSearch;
                }
                finally
                {
                    objTransferDAL = null;
                    CreatedByList = null;
                    UpdateByList = null;
                    StatusByList = null;
                    objNarrowSearch = null;
                    ReuiredDateList = null;
                    lstUDF1 = null;
                    lstUDF2 = null;
                    lstUDF3 = null;
                    lstUDF4 = null;
                    lstUDF5 = null;
                    objNarrowSearch = null;
                }

            }
            #endregion

            #region QuickList

            if (PageName.ToUpper() == "QUICKLISTMASTER")
            {
                objNarrowSearch = new NarrowSearchData();
                List<CommonDTO> CreatedByList = null;
                List<CommonDTO> UpdateByList = null;
                List<CommonDTO> lstUDF1 = null;
                List<CommonDTO> lstUDF2 = null;
                List<CommonDTO> lstUDF3 = null;
                List<CommonDTO> lstUDF4 = null;
                List<CommonDTO> lstUDF5 = null;
                try
                {

                    QuickListDAL objQuickList = new QuickListDAL(base.DataBaseName);
                    IEnumerable<CommonDTO> lstCommonDTO = objQuickList.DB_GetQuickListNarrowSearchData(CompanyID, RoomID, IsArchived, IsDeleted);
                    UDFDAL _repository = new UDFDAL(base.DataBaseName);
                    IEnumerable<UDFDTO> DataFromDB = _repository.GetUDFsByUDFTableNamePlain("QuickListMaster", RoomID, CompanyID);


                    CreatedByList = lstCommonDTO.Where(t => t.PageName == "CreatedByCount").ToList();
                    UpdateByList = lstCommonDTO.Where(t => t.PageName == "LastUpdatedByCount").ToList();
                    lstUDF1 = lstCommonDTO.Where(t => t.PageName == "UDF1ByCount").ToList();
                    lstUDF2 = lstCommonDTO.Where(t => t.PageName == "UDF2ByCount").ToList();
                    lstUDF3 = lstCommonDTO.Where(t => t.PageName == "UDF3ByCount").ToList();
                    lstUDF4 = lstCommonDTO.Where(t => t.PageName == "UDF4ByCount").ToList();
                    lstUDF5 = lstCommonDTO.Where(t => t.PageName == "UDF5ByCount").ToList();
                    objNarrowSearch.CreatedByList = CreatedByList;
                    objNarrowSearch.UpdatedByList = UpdateByList;

                    if (ISUDFForNarrowSearch(DataFromDB, "UDF1"))
                    {
                        objNarrowSearch.UDF1List = lstUDF1;
                    }
                    if (ISUDFForNarrowSearch(DataFromDB, "UDF2"))
                    {
                        objNarrowSearch.UDF2List = lstUDF2;
                    }
                    if (ISUDFForNarrowSearch(DataFromDB, "UDF3"))
                    {
                        objNarrowSearch.UDF3List = lstUDF3;
                    }
                    if (ISUDFForNarrowSearch(DataFromDB, "UDF4"))
                    {
                        objNarrowSearch.UDF4List = lstUDF4;
                    }
                    if (ISUDFForNarrowSearch(DataFromDB, "UDF5"))
                    {
                        objNarrowSearch.UDF5List = lstUDF5;
                    }

                    if (lstUDF1.Count > 0)
                        objNarrowSearch.UDF1List = lstUDF1;
                    if (lstUDF2.Count > 0)
                        objNarrowSearch.UDF2List = lstUDF2;
                    if (lstUDF3.Count > 0)
                        objNarrowSearch.UDF3List = lstUDF3;
                    if (lstUDF4.Count > 0)
                        objNarrowSearch.UDF4List = lstUDF4;
                    if (lstUDF5.Count > 0)
                        objNarrowSearch.UDF5List = lstUDF5;

                    return objNarrowSearch;
                }
                finally
                {
                    CreatedByList = null;
                    UpdateByList = null;
                    objNarrowSearch = null;
                    lstUDF1 = null;
                    lstUDF2 = null;
                    lstUDF3 = null;
                    lstUDF4 = null;
                    lstUDF5 = null;
                    objNarrowSearch = null;
                }

            }
            #endregion

            #region Label Printing

            if (PageName.ToUpper() == "LABELPRINTING")
            {
                objNarrowSearch = new NarrowSearchData();
                List<CommonDTO> LabelOnlyBaseList = null;
                List<CommonDTO> CreatedByList = null;
                List<CommonDTO> UpdateByList = null;
                List<CommonDTO> LabelModuleList = null;
                try
                {

                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {

                        CreatedByList = (from tmp in context.LabelFieldModuleTemplateDetails
                                         join um in context.UserMasters on tmp.CreatedBy equals um.ID
                                         where tmp.CreatedBy > 0
                                         && tmp.IsArchived == IsArchived && tmp.IsDeleted == IsDeleted
                                         && (tmp.CompanyID == CompanyID || tmp.CompanyID == -1)
                                         orderby um.UserName
                                         group tmp by new { tmp.CreatedBy, um.UserName } into grpCreatedBy
                                         where grpCreatedBy.Count() > 0
                                         orderby grpCreatedBy.Key.UserName
                                         select new CommonDTO
                                         {
                                             ID = grpCreatedBy.Key.CreatedBy,
                                             Text = grpCreatedBy.Key.UserName,
                                             Count = grpCreatedBy.Count()
                                         }).ToList<CommonDTO>();

                        objNarrowSearch.CreatedByList = CreatedByList;

                        UpdateByList = (from tmp in context.LabelFieldModuleTemplateDetails
                                        join um in context.UserMasters on tmp.UpdatedBy equals um.ID
                                        where tmp.UpdatedBy > 0
                                        && tmp.IsArchived == IsArchived && tmp.IsDeleted == IsDeleted
                                        && (tmp.CompanyID == CompanyID || tmp.CompanyID == -1)
                                        orderby um.UserName
                                        group tmp by new { tmp.UpdatedBy, um.UserName } into grpUpdatedBy
                                        where grpUpdatedBy.Count() > 0
                                        orderby grpUpdatedBy.Key.UserName
                                        select new CommonDTO
                                        {
                                            ID = grpUpdatedBy.Key.UpdatedBy,
                                            Text = grpUpdatedBy.Key.UserName,
                                            Count = grpUpdatedBy.Count()
                                        }).ToList<CommonDTO>();

                        objNarrowSearch.UpdatedByList = UpdateByList;

                        LabelModuleList = (from tmp in context.LabelFieldModuleTemplateDetails
                                           join um in context.LabelModuleMasters on tmp.ModuleID equals um.ID
                                           where tmp.ModuleID > 0
                                           && tmp.IsArchived == IsArchived && tmp.IsDeleted == IsDeleted
                                          && (tmp.CompanyID == CompanyID || tmp.CompanyID == -1)
                                           orderby um.ModuleName
                                           group tmp by new { tmp.ModuleID, um.ModuleName } into grpUpdatedBy
                                           where grpUpdatedBy.Count() > 0
                                           orderby grpUpdatedBy.Key.ModuleName
                                           select new CommonDTO
                                           {
                                               ID = grpUpdatedBy.Key.ModuleID,
                                               Text = grpUpdatedBy.Key.ModuleName,
                                               Count = grpUpdatedBy.Count()
                                           }).ToList<CommonDTO>();

                        if (LabelModuleList != null && LabelModuleList.Count > 0)
                        {
                            LabelModuleList.ForEach(t => t.Text = ResourceRead.GetResourceValue(t.Text, "ResModuleName"));
                        }

                        objNarrowSearch.LabelModuleList = LabelModuleList;

                        LabelOnlyBaseList = new List<CommonDTO>();

                        int CntBase = (from tmp in context.LabelFieldModuleTemplateDetails
                                       join um in context.LabelModuleMasters on tmp.ModuleID equals um.ID
                                       where tmp.ModuleID > 0
                                       && tmp.IsArchived == IsArchived && tmp.IsDeleted == IsDeleted
                                       && tmp.CompanyID == 0
                                       select tmp.ID).Count();

                        int CntAll = (from tmp in context.LabelFieldModuleTemplateDetails
                                      join um in context.LabelModuleMasters on tmp.ModuleID equals um.ID
                                      where tmp.ModuleID > 0
                                      && tmp.IsArchived == IsArchived && tmp.IsDeleted == IsDeleted
                                        && (tmp.CompanyID == CompanyID || tmp.CompanyID == -1)
                                      select tmp.ID).Count();
                        CntAll += CntBase;

                        CntBase = context.LabelFieldModuleTemplateDetails.Where(x => x.CompanyID == 0).Count();

                        LabelOnlyBaseList.Add(new CommonDTO() { ID = 11, Text = "All", Count = CntAll });
                        LabelOnlyBaseList.Add(new CommonDTO() { ID = 22, Text = "Only Base", Count = CntBase });

                        objNarrowSearch.LabelOnlyBaseList = LabelOnlyBaseList;
                    }
                    return objNarrowSearch;
                }
                finally
                {
                    CreatedByList = null;
                    UpdateByList = null;
                    objNarrowSearch = null;
                    LabelModuleList = null;
                }

            }
            #endregion

            #region "BOM Item Master"
            if (PageName.ToUpper() == "BOMMASTERLIST")
            {
                objNarrowSearch = new NarrowSearchData();
                List<CommonDTO> CreatedByList = null;
                List<CommonDTO> UpdateByList = null;
                List<CommonDTO> SupplierByList = null;
                List<CommonDTO> CategoryByList = null;
                List<CommonDTO> ManufacturerByList = null;
                List<CommonDTO> ItemTypeByList = null;
                BOMItemMasterDAL objBOMItemDAL = new BOMItemMasterDAL(base.DataBaseName);
                #region GET DATA

                Dictionary<string, int> ColUDFData = new Dictionary<string, int>();
                IEnumerable<BOMItemDTO> lstBomItems = objBOMItemDAL.GetAllRecords(RoomID, CompanyID);
                #region"CreatedBy"

                CreatedByList = (from tmp in lstBomItems.Where(t => t.CreatedBy.GetValueOrDefault(0) > 0)
                                 group tmp by new { tmp.CreatedBy, tmp.CreatedByName } into grpCreatedBy
                                 where grpCreatedBy.Count() > 0
                                 orderby grpCreatedBy.Key.CreatedByName
                                 select new CommonDTO
                                 {
                                     ID = grpCreatedBy.Key.CreatedBy.GetValueOrDefault(0),
                                     Text = grpCreatedBy.Key.CreatedByName,
                                     Count = grpCreatedBy.Count()
                                 }).ToList<CommonDTO>();

                objNarrowSearch.CreatedByList = CreatedByList;
                #endregion
                #region"UpdatedBy"
                UpdateByList = (from tmp in lstBomItems.Where(t => t.LastUpdatedBy.GetValueOrDefault(0) > 0)
                                group tmp by new { tmp.LastUpdatedBy, tmp.UpdatedByName } into grpUpdatedBy
                                where grpUpdatedBy.Count() > 0
                                orderby grpUpdatedBy.Key.UpdatedByName
                                select new CommonDTO
                                {
                                    ID = grpUpdatedBy.Key.LastUpdatedBy.GetValueOrDefault(0),
                                    Text = grpUpdatedBy.Key.UpdatedByName,
                                    Count = grpUpdatedBy.Count()
                                }).ToList<CommonDTO>();

                objNarrowSearch.UpdatedByList = UpdateByList;
                #endregion
                #region"Supplier"
                SupplierByList = (from tmp in lstBomItems.Where(t => t.IsArchived == IsArchived && t.IsDeleted == IsDeleted && t.SupplierID.GetValueOrDefault(0) > 0)
                                  group tmp by new { tmp.SupplierID, tmp.SupplierName } into grpSupplierBy
                                  where grpSupplierBy.Count() > 0
                                  orderby grpSupplierBy.Key.SupplierName
                                  select new CommonDTO
                                  {
                                      ID = grpSupplierBy.Key.SupplierID.GetValueOrDefault(0),
                                      Text = grpSupplierBy.Key.SupplierName,
                                      Count = grpSupplierBy.Count()
                                  }).ToList<CommonDTO>();

                objNarrowSearch.SupplierByList = SupplierByList;
                #endregion
                #region"Category"
                CategoryByList = (from tmp in lstBomItems.Where(t => t.IsArchived == IsArchived && t.IsDeleted == IsDeleted && t.CategoryID.GetValueOrDefault(0) > 0)
                                  group tmp by new { tmp.CategoryID, tmp.CategoryName } into grpCategoryBy
                                  where grpCategoryBy.Count() > 0
                                  orderby grpCategoryBy.Key.CategoryName
                                  select new CommonDTO
                                  {
                                      ID = grpCategoryBy.Key.CategoryID.GetValueOrDefault(0),
                                      Text = grpCategoryBy.Key.CategoryName,
                                      Count = grpCategoryBy.Count()
                                  }).ToList<CommonDTO>();

                objNarrowSearch.CategoryByList = CategoryByList;
                #endregion
                #region"Manufacture"
                ManufacturerByList = (from tmp in lstBomItems.Where(t => t.IsArchived == IsArchived && t.IsDeleted == IsDeleted && t.ManufacturerID.GetValueOrDefault(0) > 0)
                                      group tmp by new { tmp.ManufacturerID, tmp.ManufacturerName } into grpManufactureBy
                                      where grpManufactureBy.Count() > 0
                                      orderby grpManufactureBy.Key.ManufacturerName
                                      select new CommonDTO
                                      {
                                          ID = grpManufactureBy.Key.ManufacturerID.GetValueOrDefault(0),
                                          Text = grpManufactureBy.Key.ManufacturerName,
                                          Count = grpManufactureBy.Count()
                                      }).ToList<CommonDTO>();

                objNarrowSearch.ManufactureByList = ManufacturerByList;
                #endregion
                #region"ItemType"
                ItemTypeByList = (from tmp in lstBomItems.Where(t => t.IsArchived == IsArchived && t.IsDeleted == IsDeleted && t.ItemType > 0)
                                  group tmp by new { tmp.ItemType, tmp.ItemTypeName } into grpItemTypeeBy
                                  where grpItemTypeeBy.Count() > 0
                                  orderby grpItemTypeeBy.Key.ItemTypeName
                                  select new CommonDTO
                                  {
                                      ID = grpItemTypeeBy.Key.ItemType,
                                      Text = "Item",
                                      Count = grpItemTypeeBy.Count()
                                  }).ToList<CommonDTO>();

                objNarrowSearch.ItemTypeByList = ItemTypeByList;
                #endregion


                #endregion
            }

            #endregion

            #region Catalog Report

            if (PageName.ToUpper() == "CATALOGREPORT")
            {
                objNarrowSearch = new NarrowSearchData();
                List<CommonDTO> CreatedByList = null;
                List<CommonDTO> UpdateByList = null;
                try
                {

                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {

                        CreatedByList = (from tmp in context.CatalogReportDetails
                                         join um in context.UserMasters on tmp.CreatedBy equals um.ID
                                         where tmp.CreatedBy > 0
                                         && tmp.IsArchived == IsArchived && tmp.IsDeleted == IsDeleted
                                         && (tmp.CompanyID == CompanyID || tmp.CompanyID == -1)
                                         orderby um.UserName
                                         group tmp by new { tmp.CreatedBy, um.UserName } into grpCreatedBy
                                         where grpCreatedBy.Count() > 0
                                         orderby grpCreatedBy.Key.UserName
                                         select new CommonDTO
                                         {
                                             ID = grpCreatedBy.Key.CreatedBy,
                                             Text = grpCreatedBy.Key.UserName,
                                             Count = grpCreatedBy.Count()
                                         }).ToList<CommonDTO>();

                        objNarrowSearch.CreatedByList = CreatedByList;

                        UpdateByList = (from tmp in context.CatalogReportDetails
                                        join um in context.UserMasters on tmp.UpdatedBy equals um.ID
                                        where tmp.UpdatedBy > 0
                                        && tmp.IsArchived == IsArchived && tmp.IsDeleted == IsDeleted
                                        && (tmp.CompanyID == CompanyID || tmp.CompanyID == -1)
                                        orderby um.UserName
                                        group tmp by new { tmp.UpdatedBy, um.UserName } into grpUpdatedBy
                                        where grpUpdatedBy.Count() > 0
                                        orderby grpUpdatedBy.Key.UserName
                                        select new CommonDTO
                                        {
                                            ID = grpUpdatedBy.Key.UpdatedBy,
                                            Text = grpUpdatedBy.Key.UserName,
                                            Count = grpUpdatedBy.Count()
                                        }).ToList<CommonDTO>();

                        objNarrowSearch.UpdatedByList = UpdateByList;

                    }
                    return objNarrowSearch;
                }
                finally
                {
                    CreatedByList = null;
                    UpdateByList = null;
                    objNarrowSearch = null;
                }

            }
            #endregion

            #region ItemLocationPollListMaster
            if (PageName.ToUpper() == "ITEMLOCATIONPOLLLISTMASTER")
            {
                objNarrowSearch = new NarrowSearchData();
                List<CommonDTO> CreatedByList = null;
                List<CommonDTO> UpdateByList = null;
                try
                {

                    ItemLocationPollRequestDAL ILPRDAL = new ItemLocationPollRequestDAL(base.DataBaseName);
                    IEnumerable<CommonDTO> lstCommonDTO = ILPRDAL.DB_GetItemLocationPollRequestListNarrowSearchData(CompanyID, RoomID, IsArchived, IsDeleted);
                    UDFDAL _repository = new UDFDAL(base.DataBaseName);

                    CreatedByList = lstCommonDTO.Where(t => t.PageName == "CreatedByCount").ToList();
                    UpdateByList = lstCommonDTO.Where(t => t.PageName == "UpdatedByCount").ToList();
                    objNarrowSearch.CreatedByList = CreatedByList;
                    objNarrowSearch.UpdatedByList = UpdateByList;

                    return objNarrowSearch;
                }
                finally
                {
                    CreatedByList = null;
                    UpdateByList = null;
                    objNarrowSearch = null;
                    objNarrowSearch = null;
                }

            }
            #endregion




            #region ItemWeightPerPieceListMaster
            if (PageName.ToUpper() == "ITEMWEIGHTPERPIECELISTMASTER")
            {
                objNarrowSearch = new NarrowSearchData();
                List<CommonDTO> CreatedByList = null;
                List<CommonDTO> UpdateByList = null;
                try
                {

                    ItemWeightRequestDAL ILPRDAL = new ItemWeightRequestDAL(base.DataBaseName);
                    IEnumerable<CommonDTO> lstCommonDTO = ILPRDAL.DB_GetItemWeightPerPieceRequestListNarrowSearchData(CompanyID, RoomID, IsArchived, IsDeleted);
                    UDFDAL _repository = new UDFDAL(base.DataBaseName);

                    CreatedByList = lstCommonDTO.Where(t => t.PageName == "CreatedByCount").ToList();
                    UpdateByList = lstCommonDTO.Where(t => t.PageName == "UpdatedByCount").ToList();
                    objNarrowSearch.CreatedByList = CreatedByList;
                    objNarrowSearch.UpdatedByList = UpdateByList;

                    return objNarrowSearch;
                }
                finally
                {
                    CreatedByList = null;
                    UpdateByList = null;
                    objNarrowSearch = null;
                    objNarrowSearch = null;
                }

            }
            #endregion

            #region RESETREQUESTLISTMASTER
            if (PageName.ToUpper() == "RESETREQUESTLISTMASTER")
            {
                objNarrowSearch = new NarrowSearchData();
                List<CommonDTO> CreatedByList = null;
                List<CommonDTO> UpdateByList = null;
                try
                {

                    EVMIResetRequestDAL objeVMIRequestDAL = new EVMIResetRequestDAL(base.DataBaseName);
                    IEnumerable<CommonDTO> lstCommonDTO = objeVMIRequestDAL.DB_GetResetRequestListNarrowSearchData(CompanyID, RoomID, IsArchived, IsDeleted);
                    UDFDAL _repository = new UDFDAL(base.DataBaseName);

                    CreatedByList = lstCommonDTO.Where(t => t.PageName == "CreatedByCount").ToList();
                    UpdateByList = lstCommonDTO.Where(t => t.PageName == "UpdatedByCount").ToList();
                    objNarrowSearch.CreatedByList = CreatedByList;
                    objNarrowSearch.UpdatedByList = UpdateByList;

                    return objNarrowSearch;
                }
                finally
                {
                    CreatedByList = null;
                    UpdateByList = null;
                    objNarrowSearch = null;
                    objNarrowSearch = null;
                }

            }
            #endregion

            #region SHELFREQUESTLISTMASTER
            if (PageName.ToUpper() == "SHELFREQUESTLISTMASTER")
            {
                objNarrowSearch = new NarrowSearchData();
                List<CommonDTO> CreatedByList = null;
                List<CommonDTO> UpdateByList = null;
                try
                {

                    eVMIShelfRequestDAL objeVMIShelfDAL = new eVMIShelfRequestDAL(base.DataBaseName);
                    IEnumerable<CommonDTO> lstCommonDTO = objeVMIShelfDAL.DB_GetShelfRequestListNarrowSearchData(CompanyID, RoomID, IsArchived, IsDeleted);
                    UDFDAL _repository = new UDFDAL(base.DataBaseName);

                    CreatedByList = lstCommonDTO.Where(t => t.PageName == "CreatedByCount").ToList();
                    UpdateByList = lstCommonDTO.Where(t => t.PageName == "UpdatedByCount").ToList();
                    objNarrowSearch.CreatedByList = CreatedByList;
                    objNarrowSearch.UpdatedByList = UpdateByList;

                    return objNarrowSearch;
                }
                finally
                {
                    CreatedByList = null;
                    UpdateByList = null;
                    objNarrowSearch = null;
                    objNarrowSearch = null;
                }

            }
            #endregion

            #region EVMICOMCOMMONLISTMASTER
            if (PageName.ToUpper() == "EVMICOMCOMMONLISTMASTER")
            {
                objNarrowSearch = new NarrowSearchData();
                List<CommonDTO> CreatedByList = null;
                List<CommonDTO> UpdateByList = null;
                List<CommonDTO> RequestTypeList = null;
                try
                {

                    eVMICOMCommonRequestDAL objDAL = new eVMICOMCommonRequestDAL(base.DataBaseName);
                    IEnumerable<CommonDTO> lstCommonDTO = objDAL.DB_GeteVMICOMRequestListNarrowSearchData(CompanyID, RoomID, IsArchived, IsDeleted);
                    UDFDAL _repository = new UDFDAL(base.DataBaseName);

                    CreatedByList = lstCommonDTO.Where(t => t.PageName == "CreatedByCount").ToList();
                    UpdateByList = lstCommonDTO.Where(t => t.PageName == "UpdatedByCount").ToList();
                    RequestTypeList = lstCommonDTO.Where(t => t.PageName == "RequestType").ToList();


                    objNarrowSearch.CreatedByList = CreatedByList;
                    objNarrowSearch.UpdatedByList = UpdateByList;
                    objNarrowSearch.eVMICOMCommonRequestType = RequestTypeList;
                    return objNarrowSearch;
                }
                finally
                {
                    CreatedByList = null;
                    UpdateByList = null;
                    RequestTypeList = null;
                    objNarrowSearch = null;
                    objNarrowSearch = null;
                }

            }
            #endregion
            #region eVMICalibrationWeightListMaster
            if (PageName.ToUpper() == "EVMICALIBRATIONWEIGHTLISTMASTER")
            {
                objNarrowSearch = new NarrowSearchData();
                List<CommonDTO> CreatedByList = null;
                List<CommonDTO> UpdateByList = null;
                try
                {

                    eVMICalibrationWeightRequestDAL objDAL = new eVMICalibrationWeightRequestDAL(base.DataBaseName);
                    IEnumerable<CommonDTO> lstCommonDTO = objDAL.DB_GeteVMICalibrationWeightRequestNarrowSearchData(CompanyID, RoomID, IsArchived, IsDeleted);
                    UDFDAL _repository = new UDFDAL(base.DataBaseName);

                    CreatedByList = lstCommonDTO.Where(t => t.PageName == "CreatedByCount").ToList();
                    UpdateByList = lstCommonDTO.Where(t => t.PageName == "UpdatedByCount").ToList();
                    objNarrowSearch.CreatedByList = CreatedByList;
                    objNarrowSearch.UpdatedByList = UpdateByList;

                    return objNarrowSearch;
                }
                finally
                {
                    CreatedByList = null;
                    UpdateByList = null;
                    objNarrowSearch = null;
                    objNarrowSearch = null;
                }

            }
            #endregion

            return objNarrowSearch;
        }

        public bool ISUDFForNarrowSearch(IEnumerable<UDFDTO> DataFromDB, string UDFName)
        {
            bool IsSearchble = false;
            foreach (var i in DataFromDB)
            {
                if (i.UDFIsSearchable.GetValueOrDefault(false) && (i.UDFControlType == "Dropdown" || i.UDFControlType == "Dropdown Editable" || i.UDFControlType == "Textbox") && i.UDFColumnName == UDFName)
                {
                    IsSearchble = true;
                    break;
                }
            }
            return IsSearchble;
        }

        /// <summary>
        /// Get Paged Records from the Bin Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public List<CommonDTO> GetLocationListWithQuntity_OldNotUsed(Guid ItemGUID, Int64 RoomID, Int64 CompanyId)
        {
            ItemLocationDetailsDAL objdal = new ItemLocationDetailsDAL(base.DataBaseName);
            List<ItemLocationDetailsDTO> objDetailDTO = objdal.GetAllRecords(RoomID, CompanyId, ItemGUID, null, "ID ASC").Where(x => x.IsDeleted == false && x.IsArchived == false && (x.CustomerOwnedQuantity.GetValueOrDefault(0) > 0 || x.ConsignedQuantity.GetValueOrDefault(0) > 0)).ToList();
            List<CommonDTO> objNewList = new List<CommonDTO>();
            if (objDetailDTO != null && objDetailDTO.Count() > 0)
            {
                ItemMasterDTO itemDTO = new ItemMasterDAL(base.DataBaseName).GetItemWithoutJoins(null, objDetailDTO[0].ItemGUID);

                foreach (var item in objDetailDTO)
                {
                    //ItemLocationQTYDTO lstLocDTO = new ItemLocationQTYDAL(base.DataBaseName).GetAllRecords(RoomID, CompanyId).Where(x => x.BinID == item.BinID && x.ItemGUID == ItemGUID && x.Quantity > 0).SingleOrDefault();
                    ItemLocationQTYDTO lstLocDTO = new ItemLocationQTYDAL(base.DataBaseName).GetItemLocationPositiveQTYByItemGUID(RoomID, CompanyId, item.BinID, ItemGUID).FirstOrDefault();

                    if (lstLocDTO != null)
                    {
                        CommonDTO objCommon = new CommonDTO();
                        objCommon.ID = item.BinID.GetValueOrDefault(0);
                        objCommon.Text = item.BinNumber;
                        if (itemDTO.Consignment)
                        {
                            objCommon.Count = int.Parse(Math.Floor(lstLocDTO.ConsignedQuantity.GetValueOrDefault(0)).ToString()) + int.Parse(Math.Floor(lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0)).ToString());
                        }
                        else
                        {
                            objCommon.Count = int.Parse(Math.Floor(lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0)).ToString());
                        }
                        if (objCommon.Count > 0)
                        {
                            objCommon.Text = objCommon.Text;
                            objNewList.Add(objCommon);
                        }
                    }
                }
            }

            List<CommonDTO> objReturnList = (from x in objNewList
                                             group x by new { x.ID, x.Text } into grp
                                             select new CommonDTO
                                             {
                                                 ID = Convert.ToInt64(grp.Key.ID),
                                                 Text = grp.Key.Text + " (" + grp.Sum(x => x.Count) + ")"
                                             }).ToList();




            return objReturnList;
        }

        /// <summary>
        /// Get Paged Records from the Bin Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public List<CommonDTO> GetLocationListWithQuntity(Guid ItemGUID, Int64 RoomID, Int64 CompanyId)
        {
            ItemLocationDetailsDAL objdal = new ItemLocationDetailsDAL(base.DataBaseName);
            //List<ItemLocationQTYDTO> objDetailDTO = new ItemLocationQTYDAL(base.DataBaseName).GetAllRecords(RoomID, CompanyId).Where(x => x.ItemGUID == ItemGUID && x.Quantity > 0).ToList();
            List<ItemLocationQTYDTO> objDetailDTO = new ItemLocationQTYDAL(base.DataBaseName).GetItemLocationPositiveQTYByItemGUID(RoomID, CompanyId, null, ItemGUID).ToList();

            List<CommonDTO> objNewList = new List<CommonDTO>();
            if (objDetailDTO != null && objDetailDTO.Count() > 0)
            {
                ItemMasterDTO itemDTO = new ItemMasterDAL(base.DataBaseName).GetItemWithoutJoins(null, objDetailDTO[0].ItemGUID);

                foreach (var item in objDetailDTO)
                {
                    CommonDTO objCommon = new CommonDTO();
                    objCommon.ID = item.BinID;
                    objCommon.Text = item.BinNumber;
                    if (itemDTO.Consignment)
                    {
                        objCommon.Count = int.Parse(Math.Floor(item.ConsignedQuantity.GetValueOrDefault(0)).ToString()) + int.Parse(Math.Floor(item.CustomerOwnedQuantity.GetValueOrDefault(0)).ToString());
                    }
                    else
                    {
                        objCommon.Count = int.Parse(Math.Floor(item.CustomerOwnedQuantity.GetValueOrDefault(0)).ToString());
                    }
                    if (objCommon.Count > 0)
                    {
                        objCommon.Text = objCommon.Text;
                        objNewList.Add(objCommon);
                    }
                }

            }

            List<CommonDTO> objReturnList = (from x in objNewList
                                             group x by new { x.ID, x.Text } into grp
                                             select new CommonDTO
                                             {
                                                 ID = Convert.ToInt64(grp.Key.ID),
                                                 Text = grp.Key.Text + " (" + grp.Sum(x => x.Count) + ")"
                                             }).ToList();




            return objReturnList;
        }

        /// <summary>
        /// Get Status name for all the tables
        /// </summary>
        /// <param name="TableName">DataBase Table Name</param>
        /// <param name="StatusName">Status Name</param>
        /// <param name="CompanyID"></param>
        /// <param name="RoomID"></param>
        /// <returns>Status Count</returns>
        public List<CommonDTO> GetTabStatusCount(string TableName, string StatusName, Int64 CompanyID, Int64 RoomID, List<long> SupplierIds, string MainFilter = "false")
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strSupplierIDs = string.Empty;
                if (SupplierIds != null && SupplierIds.Any())
                {
                    strSupplierIDs = string.Join(",", SupplierIds);
                }

                var paramStatusCount = new SqlParameter[] { new SqlParameter("@TableName", TableName),
                                                            new SqlParameter("@StatusName", StatusName),
                                                            new SqlParameter("@RoomID", RoomID),
                                                            new SqlParameter("@CompanyID", CompanyID),
                                                            new SqlParameter("@SupplierIDs", strSupplierIDs),
                                                            new SqlParameter("@MainFilter", MainFilter)};

                return (from u in context.Database.SqlQuery<CommonDTO>("exec [GetTabStatusCount] @TableName,@StatusName,@RoomID,@CompanyID,@SupplierIDs,@MainFilter", paramStatusCount)
                        select new CommonDTO
                        {
                            ID = u.ID,
                            Text = u.Text.ToString(),
                            Count = u.Count
                        }).ToList();
            }
        }

        public string RoleDuplicateCheck(long ID, string Rolename)
        {
            string msg = "";
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
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

        public string CompanyDuplicateCheck(long ID, string CompanyName)
        {
            string msg = "";
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.CommandTimeout = 3600;
                var qry = (from em in context.CompanyMasters
                           where em.Name == CompanyName && em.IsArchived == false && em.IsDeleted == false && em.ID != ID
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

        public bool DeleteDuplicateRecords(Int64 CompanyID, Int64 UserID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool bResult = false;
                try
                {
                    context.Database.CommandTimeout = 3600;
                    string strUpdateOnHand = "EXEC [dbo].[DeleteDuplicateRecords] " + CompanyID + ", " + UserID;
                    context.Database.ExecuteSqlCommand(strUpdateOnHand);
                    bResult = true;
                }
                catch
                {
                    throw;
                    //bResult = false;
                }
                return bResult;
            }
        }

        public bool UpdateOnHandQtyAfterSynch(Int64 CompanyID, Int64 RoomID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool bResult = false;
                try
                {
                    context.Database.CommandTimeout = 3600;
                    string strUpdateOnHand = "EXEC [dbo].[UpdateOnHandQtyAfterSynch] " + CompanyID + ", " + RoomID;
                    context.Database.ExecuteSqlCommand(strUpdateOnHand);
                    bResult = true;
                }
                catch
                {
                    bResult = false;
                    throw;
                }
                return bResult;
            }
        }

        public bool UpdatePDATransactionQtyAfterSynch(Int64 CompanyID, Int64 RoomID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool bResult = false;
                try
                {
                    context.Database.CommandTimeout = 3600;
                    string strUpdateQty = "EXEC [dbo].[AT_UpdatePDATransactionQtyAfterSynch] " + CompanyID + ", " + RoomID;
                    context.Database.ExecuteSqlCommand(strUpdateQty);
                    bResult = true;
                }
                catch
                {
                    bResult = false;
                }
                return bResult;
            }
        }

        public bool ItemLocationDetialsRecalcAfterSynch(Int64 CompanyID, Int64 RoomID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool bResult = false;
                try
                {
                    context.Database.CommandTimeout = 3600;
                    string strUpdateOnHand = "EXEC [dbo].[ItemLocationQuantityAfterSync] " + RoomID + ", " + CompanyID;
                    context.Database.ExecuteSqlCommand(strUpdateOnHand);
                    bResult = true;
                }
                catch
                {
                    bResult = false;
                    throw;
                }
                return bResult;
            }
        }

        public bool PullMasterDetailsRecalcAfterSynch(Int64 CompanyID, Int64 RoomID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool bResult = false;
                try
                {
                    context.Database.CommandTimeout = 3600;
                    string strUpdateOnHand = "EXEC [dbo].[PullMasterDetailsRecalcAfterSynch] " + RoomID + ", " + CompanyID;
                    context.Database.ExecuteSqlCommand(strUpdateOnHand);
                    bResult = true;
                }
                catch
                {
                    bResult = false;
                    throw;
                }
                return bResult;
            }
        }

        public bool UpdateConsumeModuleDataAfterSynch(Int64 CompanyID, Int64 RoomID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool bResult = false;
                try
                {
                    context.Database.CommandTimeout = 3600;
                    string strUpdateConsume = "EXEC [dbo].[UpdateConsumeModuleDataAfterSynch] " + CompanyID + ", " + RoomID;
                    context.Database.ExecuteSqlCommand(strUpdateConsume);
                    bResult = true;
                }
                catch
                {
                    bResult = false;
                    throw;
                }
                return bResult;
            }
        }

        public bool UpdateToolAssetDataAfterSynch(Int64 CompanyID, Int64 RoomID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool bResult = false;
                try
                {
                    context.Database.CommandTimeout = 3600;
                    string strUpdateTools = "EXEC [dbo].[UpdateToolAssetDataAfterSynch] " + CompanyID + ", " + RoomID;
                    context.Database.ExecuteSqlCommand(strUpdateTools);
                    bResult = true;

                    string strAutoCartUpdate = "EXEC [dbo].[UpdateAutoCartAfterSynch] " + CompanyID + ", " + RoomID;
                    context.Database.ExecuteSqlCommand(strAutoCartUpdate);
                    bResult = true;

                }
                catch
                {
                    bResult = false;
                    throw;
                }
                return bResult;
            }
        }

        public bool UpdateConsignedPullBilling(Int64 CompanyID, Int64 RoomID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool bResult = false;
                try
                {
                    context.Database.CommandTimeout = 3600;
                    string stConsignedPullBilling = "EXEC [dbo].[UpdateConsignedPullBilling] " + RoomID + ", " + CompanyID;
                    context.Database.ExecuteSqlCommand(stConsignedPullBilling);
                    bResult = true;
                }
                catch
                {
                    bResult = false;
                    throw;
                }
                return bResult;
            }
        }

        public bool TransmitApprovedOrderAfterSync(Int64 CompanyID, Int64 RoomID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool bResult = false;
                try
                {
                    context.Database.CommandTimeout = 3600;
                    string strUpdateOrder = "EXEC [dbo].[TransmitApprovedOrderAfterSync] " + RoomID + ", " + CompanyID;
                    context.Database.ExecuteSqlCommand(strUpdateOrder);
                    bResult = true;
                }
                catch
                {
                    bResult = false;
                    throw;
                }
                return bResult;
            }
        }

        public List<ItemQuantityDetail> GetOnOrderDetailFromCache(Guid ItemGuid, Int64 RoomID, Int64 CompanyID)
        {
            List<ItemQuantityDetail> objQty = new List<ItemQuantityDetail>();

            OrderDetailsDAL objOrderDetailDAL = new OrderDetailsDAL(base.DataBaseName);
            OrderMasterDAL objOrderMasterDAL = new OrderMasterDAL(base.DataBaseName);
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
            List<OrderDetailsDTO> objOrderDetailDTO = objOrderDetailDAL.GetOrderDetailByRoomPlain(RoomID, CompanyID).Where(x => x.IsCloseItem.GetValueOrDefault(false) == false
                                                                                                                            && x.ItemGUID.GetValueOrDefault(Guid.Empty) == ItemGuid
                                                                                                                            && x.RequestedQuantity.GetValueOrDefault(0) > 0
                                                                                                                            && x.RequestedQuantity.GetValueOrDefault(0) > x.ReceivedQuantity.GetValueOrDefault(0)).ToList();
            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();

            objItemMasterDTO = objItemMasterDAL.GetItemWithoutJoins(null, ItemGuid); //.Where(x => x.GUID == ItemGuid).FirstOrDefault();

            foreach (var item in objOrderDetailDTO)
            {
                OrderMasterDTO orderDTO = objOrderMasterDAL.GetOrderByGuidPlain(item.OrderGUID.GetValueOrDefault(Guid.Empty));
                if (orderDTO.OrderType == (int)OrderType.Order && orderDTO.OrderStatus >= (int)OrderStatus.UnSubmitted && orderDTO.OrderStatus < (int)OrderStatus.Closed)
                {
                    if (orderDTO.StagingID.GetValueOrDefault(0) <= 0)
                    {
                        double qty = 0;
                        if (item.ApprovedQuantity.GetValueOrDefault(0) > 0)
                        {
                            qty = item.ApprovedQuantity.GetValueOrDefault(0) - item.ReceivedQuantity.GetValueOrDefault(0);
                        }
                        else if (item.RequestedQuantity.GetValueOrDefault(0) > 0)
                        {
                            qty = item.RequestedQuantity.GetValueOrDefault(0) - item.ReceivedQuantity.GetValueOrDefault(0);
                        }

                        if (qty <= 0)
                            qty = 0;

                        ItemQuantityDetail itemQty = new ItemQuantityDetail();
                        itemQty.Name = orderDTO.OrderNumber;
                        itemQty.qty = qty;
                        itemQty.ID = orderDTO.ID.ToString();
                        itemQty.status = orderDTO.OrderStatus.ToString();
                        itemQty.requirdate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy");
                        itemQty.ItemNo = objItemMasterDTO.ItemNumber;
                        itemQty.ReleaseNumber = orderDTO.ReleaseNumber;
                        itemQty.BinNumber = item.BinName;
                        itemQty.ItemQuantityType = "OnOrderQuantity";
                        objQty.Add(itemQty);
                    }
                }
            }


            return objQty;
        }
        public List<ItemQuantityDetail> GetOnQuoteDetailFromCache(Guid ItemGuid, Int64 RoomID, Int64 CompanyID)
        {
            List<ItemQuantityDetail> objQty = new List<ItemQuantityDetail>();

            QuoteDetailDAL objQuoteDetailDAL = new QuoteDetailDAL(base.DataBaseName);
            QuoteMasterDAL objQuoteMasterDAL = new QuoteMasterDAL(base.DataBaseName);
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
            List<QuoteDetailDTO> objQuoteDetailDTO = objQuoteDetailDAL.GetQuoteDetailByRoomPlain(RoomID, CompanyID).Where(x => x.IsCloseItem.GetValueOrDefault(false) == false
                                                                                                                            && x.ItemGUID == ItemGuid
                                                                                                                            && x.RequestedQuantity.GetValueOrDefault(0) > 0
                                                                                                                           ).ToList();
            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();

            objItemMasterDTO = objItemMasterDAL.GetItemWithoutJoins(null, ItemGuid); //.Where(x => x.GUID == ItemGuid).FirstOrDefault();

            foreach (var item in objQuoteDetailDTO)
            {
                QuoteMasterDTO quoteDTO = objQuoteMasterDAL.GetQuoteByGuidPlain(item.QuoteGUID);
                if (quoteDTO.QuoteStatus >= (int)QuoteStatus.UnSubmitted && quoteDTO.QuoteStatus < (int)QuoteStatus.Closed)
                {

                    {
                        double qty = 0;
                        if (item.ApprovedQuantity.GetValueOrDefault(0) > 0)
                        {
                            qty = item.ApprovedQuantity.GetValueOrDefault(0);
                        }
                        else if (item.RequestedQuantity.GetValueOrDefault(0) > 0)
                        {
                            qty = item.RequestedQuantity.GetValueOrDefault(0);
                        }

                        if (qty <= 0)
                            qty = 0;

                        ItemQuantityDetail itemQty = new ItemQuantityDetail();
                        itemQty.Name = quoteDTO.QuoteNumber;
                        itemQty.qty = qty;
                        itemQty.ID = quoteDTO.ID.ToString();
                        itemQty.status = quoteDTO.QuoteStatus.ToString();
                        itemQty.requirdate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy");
                        itemQty.ItemNo = objItemMasterDTO.ItemNumber;
                        itemQty.ReleaseNumber = quoteDTO.ReleaseNumber;
                        itemQty.BinNumber = item.BinName;
                        objQty.Add(itemQty);
                    }
                }
            }


            return objQty;
        }

        public List<ItemQuantityDetail> GetOnOrderDetailFromCache(Guid ItemGuid, Int64 RoomID, Int64 CompanyID, Int64 BinID)
        {
            List<ItemQuantityDetail> objQty = new List<ItemQuantityDetail>();

            OrderDetailsDAL objOrderDetailDAL = new OrderDetailsDAL(base.DataBaseName);
            OrderMasterDAL objOrderMasterDAL = new OrderMasterDAL(base.DataBaseName);
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
            List<OrderDetailsDTO> objOrderDetailDTO = objOrderDetailDAL.GetOrderDetailByItemGUID(RoomID, CompanyID, ItemGuid).Where(x => x.IsCloseItem.GetValueOrDefault(false) == false
                                                                                                                            && x.Bin.GetValueOrDefault(0) == BinID
                                                                                                                            && x.RequestedQuantity.GetValueOrDefault(0) > 0
                                                                                                                            && x.RequestedQuantity.GetValueOrDefault(0) > x.ReceivedQuantity.GetValueOrDefault(0)).ToList();
            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();

            objItemMasterDTO = objItemMasterDAL.GetItemWithoutJoins(null, ItemGuid); //.Where(x => x.GUID == ItemGuid).FirstOrDefault();

            foreach (var item in objOrderDetailDTO)
            {
                OrderMasterDTO orderDTO = objOrderMasterDAL.GetOrderByGuidPlain(item.OrderGUID.GetValueOrDefault(Guid.Empty));
                if (orderDTO.OrderType == (int)OrderType.Order && orderDTO.OrderStatus >= (int)OrderStatus.UnSubmitted && orderDTO.OrderStatus < (int)OrderStatus.Closed)
                {
                    if (orderDTO.StagingID.GetValueOrDefault(0) <= 0)
                    {
                        double qty = 0;
                        if (item.ApprovedQuantity.GetValueOrDefault(0) > 0)
                        {
                            qty = item.ApprovedQuantity.GetValueOrDefault(0) - item.ReceivedQuantity.GetValueOrDefault(0);
                        }
                        else if (item.RequestedQuantity.GetValueOrDefault(0) > 0)
                        {
                            qty = item.RequestedQuantity.GetValueOrDefault(0) - item.ReceivedQuantity.GetValueOrDefault(0);
                        }

                        if (qty <= 0)
                            qty = 0;

                        ItemQuantityDetail itemQty = new ItemQuantityDetail();
                        itemQty.Name = orderDTO.OrderNumber;
                        itemQty.qty = qty;
                        itemQty.ID = orderDTO.ID.ToString();
                        itemQty.status = orderDTO.OrderStatus.ToString();
                        itemQty.requirdate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy");
                        itemQty.ItemNo = objItemMasterDTO.ItemNumber;
                        itemQty.ReleaseNumber = orderDTO.ReleaseNumber;
                        itemQty.BinNumber = item.BinName;
                        objQty.Add(itemQty);
                    }
                }
            }


            return objQty;
        }

        public List<ItemQuantityDetail> GetOnOrderDetailInTransitFromCache(Guid ItemGuid, Int64 RoomID, Int64 CompanyID)
        {
            List<ItemQuantityDetail> objQty = new List<ItemQuantityDetail>();

            OrderDetailsDAL objOrderDetailDAL = new OrderDetailsDAL(base.DataBaseName);
            OrderMasterDAL objOrderMasterDAL = new OrderMasterDAL(base.DataBaseName);
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
            List<OrderDetailsDTO> objOrderDetailDTO = objOrderDetailDAL.GetOrderDetailByRoomPlain(RoomID, CompanyID).Where(x => x.IsCloseItem.GetValueOrDefault(false) == false
                                                                                                                        && x.ItemGUID.GetValueOrDefault(Guid.Empty) == ItemGuid
                                                                                                                        && x.RequestedQuantity.GetValueOrDefault(0) > 0
                                                                                                                        && x.RequestedQuantity.GetValueOrDefault(0) > x.ReceivedQuantity.GetValueOrDefault(0)).ToList();
            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();

            objItemMasterDTO = objItemMasterDAL.GetItemWithoutJoins(null, ItemGuid); //.Where(x => x.GUID == ItemGuid).FirstOrDefault();

            foreach (var item in objOrderDetailDTO)
            {
                OrderMasterDTO orderDTO = objOrderMasterDAL.GetOrderByGuidPlain(item.OrderGUID.GetValueOrDefault(Guid.Empty));
                if (orderDTO.OrderType == (int)OrderType.Order && orderDTO.OrderStatus >= (int)OrderStatus.Transmitted && orderDTO.OrderStatus < (int)OrderStatus.Closed)
                {
                    if (orderDTO.StagingID.GetValueOrDefault(0) <= 0)
                    {
                        double qty = 0;
                        if (item.ApprovedQuantity.GetValueOrDefault(0) > 0)
                        {
                            qty = item.ApprovedQuantity.GetValueOrDefault(0) - item.ReceivedQuantity.GetValueOrDefault(0);
                        }
                        else if (item.RequestedQuantity.GetValueOrDefault(0) > 0)
                        {
                            qty = item.RequestedQuantity.GetValueOrDefault(0) - item.ReceivedQuantity.GetValueOrDefault(0);
                        }

                        if (qty <= 0)
                            qty = 0;

                        ItemQuantityDetail itemQty = new ItemQuantityDetail();
                        itemQty.Name = orderDTO.OrderNumber;
                        itemQty.qty = qty;
                        itemQty.ID = orderDTO.ID.ToString();
                        itemQty.status = orderDTO.OrderStatus.ToString();
                        itemQty.requirdate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy");
                        itemQty.ItemNo = objItemMasterDTO.ItemNumber;
                        itemQty.ReleaseNumber = orderDTO.ReleaseNumber;
                        itemQty.BinNumber = item.BinName;
                        objQty.Add(itemQty);
                    }
                }
            }

            return objQty;
        }

        public List<ItemQuantityDetail> GetOnOrderDetailInTransitFromCache(Guid ItemGuid, Int64 RoomID, Int64 CompanyID, Int64 BinID)
        {
            List<ItemQuantityDetail> objQty = new List<ItemQuantityDetail>();

            OrderDetailsDAL objOrderDetailDAL = new OrderDetailsDAL(base.DataBaseName);
            OrderMasterDAL objOrderMasterDAL = new OrderMasterDAL(base.DataBaseName);
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
            List<OrderDetailsDTO> objOrderDetailDTO = objOrderDetailDAL.GetOrderDetailByItemGUID(RoomID, CompanyID, ItemGuid).Where(x => x.IsCloseItem.GetValueOrDefault(false) == false
                                                                                                                        && x.Bin.GetValueOrDefault(0) == BinID
                                                                                                                        && x.RequestedQuantity.GetValueOrDefault(0) > 0
                                                                                                                        && x.RequestedQuantity.GetValueOrDefault(0) > x.ReceivedQuantity.GetValueOrDefault(0)).ToList();
            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();

            objItemMasterDTO = objItemMasterDAL.GetItemWithoutJoins(null, ItemGuid); //.Where(x => x.GUID == ItemGuid).FirstOrDefault();

            foreach (var item in objOrderDetailDTO)
            {
                OrderMasterDTO orderDTO = objOrderMasterDAL.GetOrderByGuidPlain(item.OrderGUID.GetValueOrDefault(Guid.Empty));
                if (orderDTO.OrderType == (int)OrderType.Order && orderDTO.OrderStatus >= (int)OrderStatus.Transmitted && orderDTO.OrderStatus < (int)OrderStatus.Closed)
                {
                    if (orderDTO.StagingID.GetValueOrDefault(0) <= 0)
                    {
                        double qty = 0;
                        if (item.ApprovedQuantity.GetValueOrDefault(0) > 0)
                        {
                            qty = item.ApprovedQuantity.GetValueOrDefault(0) - item.ReceivedQuantity.GetValueOrDefault(0);
                        }
                        else if (item.RequestedQuantity.GetValueOrDefault(0) > 0)
                        {
                            qty = item.RequestedQuantity.GetValueOrDefault(0) - item.ReceivedQuantity.GetValueOrDefault(0);
                        }

                        if (qty <= 0)
                            qty = 0;

                        ItemQuantityDetail itemQty = new ItemQuantityDetail();
                        itemQty.Name = orderDTO.OrderNumber;
                        itemQty.qty = qty;
                        itemQty.ID = orderDTO.ID.ToString();
                        itemQty.status = orderDTO.OrderStatus.ToString();
                        itemQty.requirdate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy");
                        itemQty.ItemNo = objItemMasterDTO.ItemNumber;
                        itemQty.ReleaseNumber = orderDTO.ReleaseNumber;
                        itemQty.BinNumber = item.BinName;
                        objQty.Add(itemQty);
                    }
                }
            }

            return objQty;
        }

        public List<ItemQuantityDetail> GetOnReturnOrderDetailFromCache(Guid ItemGuid, Int64 RoomID, Int64 CompanyID)
        {
            List<ItemQuantityDetail> objQty = new List<ItemQuantityDetail>();
            OrderDetailsDAL objOrderDetailDAL = new OrderDetailsDAL(base.DataBaseName);
            OrderMasterDAL objOrderMasterDAL = new OrderMasterDAL(base.DataBaseName);
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
            List<OrderDetailsDTO> objOrderDetailDTO = objOrderDetailDAL.GetOrderDetailByRoomPlain(RoomID, CompanyID).Where(x => x.ItemGUID.GetValueOrDefault(Guid.Empty) == ItemGuid
                                                                                                                && x.RequestedQuantity.GetValueOrDefault(0) > 0
                                                                                                                && x.RequestedQuantity.GetValueOrDefault(0) > x.ReceivedQuantity.GetValueOrDefault(0)).ToList();
            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();

            objItemMasterDTO = objItemMasterDAL.GetItemWithoutJoins(null, ItemGuid);

            foreach (var item in objOrderDetailDTO)
            {
                OrderMasterDTO orderDTO = objOrderMasterDAL.GetOrderByGuidPlain(item.OrderGUID.GetValueOrDefault(Guid.Empty));

                if (orderDTO.OrderType == (int)OrderType.RuturnOrder && orderDTO.OrderStatus >= (int)OrderStatus.UnSubmitted && orderDTO.OrderStatus < (int)OrderStatus.Closed)
                {
                    if (orderDTO.StagingID.GetValueOrDefault(0) <= 0)
                    {
                        double qty = 0;
                        if (item.ApprovedQuantity.GetValueOrDefault(0) > 0)
                        {
                            qty = item.ApprovedQuantity.GetValueOrDefault(0) - item.ReceivedQuantity.GetValueOrDefault(0);
                        }
                        else if (item.RequestedQuantity.GetValueOrDefault(0) > 0)
                        {
                            qty = item.RequestedQuantity.GetValueOrDefault(0) - item.ReceivedQuantity.GetValueOrDefault(0);
                        }

                        if (qty <= 0)
                            qty = 0;

                        ItemQuantityDetail itemQty = new ItemQuantityDetail();
                        itemQty.Name = orderDTO.OrderNumber;
                        itemQty.qty = qty;
                        itemQty.ID = orderDTO.ID.ToString();
                        itemQty.status = orderDTO.OrderStatus.ToString();
                        itemQty.requirdate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy");
                        itemQty.ItemNo = objItemMasterDTO.ItemNumber;
                        itemQty.ReleaseNumber = orderDTO.ReleaseNumber;
                        itemQty.BinNumber = item.BinName;
                        itemQty.ItemQuantityType = "ReturnOrderQuantity";
                        objQty.Add(itemQty);
                    }
                }
            }
            return objQty;
        }

        public List<ItemQuantityDetail> GetOnReturnOrderDetailFromCache(Guid ItemGuid, Int64 RoomID, Int64 CompanyID, Int64 BinID)
        {
            List<ItemQuantityDetail> objQty = new List<ItemQuantityDetail>();
            OrderDetailsDAL objOrderDetailDAL = new OrderDetailsDAL(base.DataBaseName);
            OrderMasterDAL objOrderMasterDAL = new OrderMasterDAL(base.DataBaseName);
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
            List<OrderDetailsDTO> objOrderDetailDTO = objOrderDetailDAL.GetOrderDetailByItemGUID(RoomID, CompanyID, ItemGuid).Where(x => x.ItemGUID.GetValueOrDefault(Guid.Empty) == ItemGuid && x.Bin.GetValueOrDefault(0) == BinID
                                                                                                                && x.RequestedQuantity.GetValueOrDefault(0) > 0
                                                                                                                && x.RequestedQuantity.GetValueOrDefault(0) > x.ReceivedQuantity.GetValueOrDefault(0)).ToList();
            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();

            objItemMasterDTO = objItemMasterDAL.GetItemWithoutJoins(null, ItemGuid);

            foreach (var item in objOrderDetailDTO)
            {
                OrderMasterDTO orderDTO = objOrderMasterDAL.GetOrderByGuidPlain(item.OrderGUID.GetValueOrDefault(Guid.Empty));

                if (orderDTO.OrderType == (int)OrderType.RuturnOrder && orderDTO.OrderStatus >= (int)OrderStatus.UnSubmitted && orderDTO.OrderStatus < (int)OrderStatus.Closed)
                {
                    if (orderDTO.StagingID.GetValueOrDefault(0) <= 0)
                    {
                        double qty = 0;
                        if (item.ApprovedQuantity.GetValueOrDefault(0) > 0)
                        {
                            qty = item.ApprovedQuantity.GetValueOrDefault(0) - item.ReceivedQuantity.GetValueOrDefault(0);
                        }
                        else if (item.RequestedQuantity.GetValueOrDefault(0) > 0)
                        {
                            qty = item.RequestedQuantity.GetValueOrDefault(0) - item.ReceivedQuantity.GetValueOrDefault(0);
                        }

                        if (qty <= 0)
                            qty = 0;

                        ItemQuantityDetail itemQty = new ItemQuantityDetail();
                        itemQty.Name = orderDTO.OrderNumber;
                        itemQty.qty = qty;
                        itemQty.ID = orderDTO.ID.ToString();
                        itemQty.status = orderDTO.OrderStatus.ToString();
                        itemQty.requirdate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy");
                        itemQty.ItemNo = objItemMasterDTO.ItemNumber;
                        itemQty.ReleaseNumber = orderDTO.ReleaseNumber;
                        itemQty.BinNumber = item.BinName;
                        objQty.Add(itemQty);
                    }
                }
            }
            return objQty;
        }

        public List<ItemQuantityDetail> GetOnRequisitionDetailFromCache(Guid ItemGuid, Int64 RoomID, Int64 CompanyID)
        {
            RequisitionDetailsDAL objDetailDAL = new RequisitionDetailsDAL(base.DataBaseName);
            return objDetailDAL.GetItemRequistionQtyDetail(ItemGuid, RoomID, CompanyID);
            //List<ItemQuantityDetail> objQty = new List<ItemQuantityDetail>();

            //RequisitionDetailsDAL objDetailDAL = new RequisitionDetailsDAL(base.DataBaseName);
            //RequisitionMasterDAL objMasterDAL = new RequisitionMasterDAL(base.DataBaseName);
            ////List<RequisitionDetailsDTO> objDetailDTO = objDetailDAL.GetAllRecords(RoomID, CompanyID).Where(x => x.IsDeleted.GetValueOrDefault(false) == false && x.IsArchived.GetValueOrDefault(false) == false && x.ItemGUID.GetValueOrDefault(Guid.Empty) == ItemGuid && x.QuantityRequisitioned.GetValueOrDefault(0) > 0).ToList();
            //List<RequisitionDetailsDTO> objDetailDTO = objDetailDAL.GetRecordByItemID(ItemGuid, RoomID, CompanyID, false, false).Where(x => x.QuantityRequisitioned.GetValueOrDefault(0) > 0).ToList();
            //foreach (var item in objDetailDTO)
            //{
            //    RequisitionMasterDTO orderDTO = objMasterDAL.GetRequisitionByGUIDPlain(item.RequisitionGUID.GetValueOrDefault(Guid.Empty));
            //    //if (orderDTO.RequisitionStatus.ToLower() != "closed")
            //    if (orderDTO.RequisitionStatus.ToLower().Equals("approved"))
            //    {
            //        double qty = 0;
            //        if (item.QuantityApproved.GetValueOrDefault(0) > 0)
            //        {
            //            qty = item.QuantityApproved.GetValueOrDefault(0) - item.QuantityPulled.GetValueOrDefault(0);
            //        }
            //        else if (item.QuantityRequisitioned.GetValueOrDefault(0) > 0)
            //        {
            //            qty = item.QuantityRequisitioned.GetValueOrDefault(0) - item.QuantityPulled.GetValueOrDefault(0);
            //        }

            //        if (qty <= 0)
            //            qty = 0;

            //        ItemQuantityDetail itemQty = new ItemQuantityDetail();
            //        itemQty.Name = orderDTO.RequisitionNumber;
            //        itemQty.qty = qty;
            //        itemQty.ID = orderDTO.ID.ToString();
            //        itemQty.status = orderDTO.RequisitionStatus.ToString();
            //        itemQty.GUID = orderDTO.GUID;
            //        //itemQty.requirdate = orderDTO.RequiredDate;
            //        itemQty.requirdate = (orderDTO.RequiredDate.HasValue && HttpContext.Current != null && HttpContext.Current.Session != null) ? Convert.ToDateTime(orderDTO.RequiredDate.Value).ToString(Convert.ToString(HttpContext.Current.Session["RoomDateFormat"])) : string.Empty;
            //        objQty.Add(itemQty);
            //    }
            //}


            //return objQty;
        }

        public List<ItemQuantityDetail> GetOnRequisitionDetailFromCache(Guid ItemGuid, Int64 RoomID, Int64 CompanyID, Int64 BinID)
        {
            RequisitionDetailsDAL objDetailDAL = new RequisitionDetailsDAL(base.DataBaseName);
            return objDetailDAL.GetItemRequistionQtyDetail(ItemGuid, RoomID, CompanyID, BinID);
        }

        public List<ItemQuantityDetail> GetOnStageQuantityDetailFromCache(Guid ItemGuid, Int64 RoomID, Int64 CompanyID)
        {
            List<ItemQuantityDetail> objQty = new List<ItemQuantityDetail>();

            MaterialStagingDetailDAL objDetailDAL = new MaterialStagingDetailDAL(base.DataBaseName);
            //List<MaterialStagingDetailDTO> objDetailDTO = objDetailDAL.GetAllRecordsRoomItemWise(RoomID, CompanyID, false, false, ItemGuid).Where(x => x.Quantity > 0).ToList();
            List<MaterialStagingDetailDTO> objDetailDTO = objDetailDAL.GetAllRecordsRoomItemWisePositiveQTY(RoomID, CompanyID, false, false, ItemGuid).ToList();

            List<MaterialStagingDetailDTO> objReturnList = (from x in objDetailDTO
                                                            group x by new { x.StagingBinID, x.StagingBinName } into grp
                                                            select new MaterialStagingDetailDTO
                                                            {
                                                                StagingBinID = Convert.ToInt64(grp.Key.StagingBinID),
                                                                StagingBinName = grp.Key.StagingBinName.Equals("[|EmptyStagingBin|]") ? "" : grp.Key.StagingBinName,
                                                                Quantity = grp.Sum(x => x.Quantity),
                                                            }).ToList();

            foreach (var item in objReturnList)
            {
                ItemQuantityDetail itemQty = new ItemQuantityDetail();
                itemQty.Name = item.StagingBinName;
                itemQty.qty = item.Quantity;
                itemQty.ID = item.StagingBinID.ToString();
                objQty.Add(itemQty);

            }


            return objQty;
        }

        public List<CartItemDTO> GetAllCarts(Guid ItemGuid, Int64 RoomID, Int64 CompanyID)
        {
            List<CartItemDTO> lstcarts = new List<CartItemDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstcarts = (from ci in context.CartItems
                            where (ci.IsDeleted ?? false) == false && (ci.IsArchived ?? false) == false && ci.ItemGUID == ItemGuid && ci.Room == RoomID && ci.CompanyID == CompanyID
                            select new CartItemDTO
                            {
                                ID = ci.ID,
                                Quantity = ci.Quantity
                            }).ToList();
            }
            return lstcarts;
        }

        public List<ItemQuantityDetail> GetOnTransferQtyDetailFromCache(Guid ItemGuid, Int64 RoomID, Int64 CompanyID, List<long> SupplierIds)
        {
            List<ItemQuantityDetail> objQty = new List<ItemQuantityDetail>();

            TransferDetailDAL objDetailDAL = new TransferDetailDAL(base.DataBaseName);
            TransferMasterDAL objMasterDAL = new TransferMasterDAL(base.DataBaseName);
            //List<TransferDetailDTO> objOrderDetailDTO = objDetailDAL.GetAllRecords(RoomID, CompanyID, false, false, SupplierIds).Where(x => x.ItemGUID == ItemGuid && x.RequestedQuantity > 0 && x.RequestedQuantity > x.ReceivedQuantity.GetValueOrDefault(0)).ToList();
            List<TransferDetailDTO> objOrderDetailDTO = objDetailDAL.GetTransferDetailDataByItemGUIDNormal(RoomID, CompanyID, SupplierIds, ItemGuid).ToList();

            foreach (var item in objOrderDetailDTO)
            {
                TransferMasterDTO orderDTO = objMasterDAL.GetTransferByGuidPlain(item.TransferGUID);
                if (orderDTO.RequestType == (int)RequestType.In && orderDTO.TransferStatus < (int)TransferStatus.Closed)
                {
                    ItemQuantityDetail itemQty = new ItemQuantityDetail();
                    itemQty.Name = orderDTO.TransferNumber;
                    itemQty.qty = (item.RequestedQuantity - item.ReceivedQuantity.GetValueOrDefault(0));
                    itemQty.ID = orderDTO.ID.ToString();
                    itemQty.status = orderDTO.TransferStatus.ToString();
                    itemQty.requirdate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy");
                    itemQty.requestType = orderDTO.RequestType.ToString();
                    objQty.Add(itemQty);
                }
            }


            return objQty;
        }

        public List<ItemQuantityDetail> GetOnTransferQtyDetailFromCacheWithBin(Guid ItemGuid, Int64 RoomID, Int64 CompanyID, List<long> SupplierIds, Int64 BinID)
        {
            List<ItemQuantityDetail> objQty = new List<ItemQuantityDetail>();

            TransferDetailDAL objDetailDAL = new TransferDetailDAL(base.DataBaseName);
            TransferMasterDAL objMasterDAL = new TransferMasterDAL(base.DataBaseName);
            //List<TransferDetailDTO> objOrderDetailDTO = objDetailDAL.GetAllRecords(RoomID, CompanyID, false, false, SupplierIds).Where(x => x.ItemGUID == ItemGuid && x.Bin.GetValueOrDefault(0) == BinID && x.RequestedQuantity > 0 && x.RequestedQuantity > x.ReceivedQuantity.GetValueOrDefault(0)).ToList();
            List<TransferDetailDTO> objOrderDetailDTO = objDetailDAL.GetTransferDetailDataByItemGUIDAndBinNormal(RoomID, CompanyID, SupplierIds, ItemGuid, BinID).ToList();

            foreach (var item in objOrderDetailDTO)
            {
                TransferMasterDTO orderDTO = objMasterDAL.GetTransferByGuidPlain(item.TransferGUID);
                if (orderDTO.RequestType == (int)RequestType.In && orderDTO.TransferStatus < (int)TransferStatus.Closed)
                {
                    ItemQuantityDetail itemQty = new ItemQuantityDetail();
                    itemQty.Name = orderDTO.TransferNumber;
                    itemQty.qty = (item.RequestedQuantity - item.ReceivedQuantity.GetValueOrDefault(0));
                    itemQty.ID = orderDTO.ID.ToString();
                    itemQty.status = orderDTO.TransferStatus.ToString();
                    itemQty.requirdate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy");
                    itemQty.requestType = orderDTO.RequestType.ToString();
                    objQty.Add(itemQty);
                }
            }


            return objQty;
        }

        public List<ItemQuantityDetail> GetOnTransferQtyInTransitDetailFromCache(Guid ItemGuid, Int64 RoomID, Int64 CompanyID, List<long> SupplierIds)
        {
            List<ItemQuantityDetail> objQty = new List<ItemQuantityDetail>();

            TransferDetailDAL objDetailDAL = new TransferDetailDAL(base.DataBaseName);
            TransferMasterDAL objMasterDAL = new TransferMasterDAL(base.DataBaseName);
            //List<TransferDetailDTO> objOrderDetailDTO = objDetailDAL.GetAllRecords(RoomID, CompanyID, false, false, SupplierIds).Where(x => x.ItemGUID == ItemGuid && x.RequestedQuantity > 0 && x.RequestedQuantity > x.ReceivedQuantity.GetValueOrDefault(0)).ToList();
            List<TransferDetailDTO> objOrderDetailDTO = objDetailDAL.GetTransferDetailDataByItemGUIDNormal(RoomID, CompanyID, SupplierIds, ItemGuid).ToList();

            foreach (var item in objOrderDetailDTO)
            {
                TransferMasterDTO orderDTO = objMasterDAL.GetTransferByGuidPlain(item.TransferGUID);
                if (orderDTO.RequestType == (int)RequestType.In && orderDTO.TransferStatus >= (int)TransferStatus.Transmitted && orderDTO.TransferStatus < (int)TransferStatus.Closed)
                {
                    ItemQuantityDetail itemQty = new ItemQuantityDetail();
                    itemQty.Name = orderDTO.TransferNumber;
                    itemQty.qty = (item.RequestedQuantity - item.ReceivedQuantity.GetValueOrDefault(0));
                    itemQty.ID = orderDTO.ID.ToString();
                    itemQty.status = orderDTO.TransferStatus.ToString();
                    itemQty.requirdate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy");
                    itemQty.requestType = orderDTO.RequestType.ToString();
                    objQty.Add(itemQty);
                }
            }


            return objQty;
        }

        public List<ItemQuantityDetail> GetOnTransferQtyInTransitDetailFromCache(Guid ItemGuid, Int64 RoomID, Int64 CompanyID, List<long> SupplierIds, Int64 BinID)
        {
            List<ItemQuantityDetail> objQty = new List<ItemQuantityDetail>();

            TransferDetailDAL objDetailDAL = new TransferDetailDAL(base.DataBaseName);
            TransferMasterDAL objMasterDAL = new TransferMasterDAL(base.DataBaseName);
            //List<TransferDetailDTO> objOrderDetailDTO = objDetailDAL.GetAllRecords(RoomID, CompanyID, false, false, SupplierIds).Where(x => x.ItemGUID == ItemGuid && x.Bin.GetValueOrDefault(0) == BinID && x.RequestedQuantity > 0 && x.RequestedQuantity > x.ReceivedQuantity.GetValueOrDefault(0)).ToList();
            List<TransferDetailDTO> objOrderDetailDTO = objDetailDAL.GetTransferDetailDataByItemGUIDAndBinNormal(RoomID, CompanyID, SupplierIds, ItemGuid, BinID).ToList();

            foreach (var item in objOrderDetailDTO)
            {
                TransferMasterDTO orderDTO = objMasterDAL.GetTransferByGuidPlain(item.TransferGUID);
                if (orderDTO.RequestType == (int)RequestType.In && orderDTO.TransferStatus >= (int)TransferStatus.Transmitted && orderDTO.TransferStatus < (int)TransferStatus.Closed)
                {
                    ItemQuantityDetail itemQty = new ItemQuantityDetail();
                    itemQty.Name = orderDTO.TransferNumber;
                    itemQty.qty = (item.RequestedQuantity - item.ReceivedQuantity.GetValueOrDefault(0));
                    itemQty.ID = orderDTO.ID.ToString();
                    itemQty.status = orderDTO.TransferStatus.ToString();
                    itemQty.requirdate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy");
                    itemQty.requestType = orderDTO.RequestType.ToString();
                    objQty.Add(itemQty);
                }
            }


            return objQty;
        }

        public List<ItemQuantityDetail> GetInTransitQtyDetailFromCache(Guid ItemGuid, Int64 RoomID, Int64 CompanyID, List<long> SupplierIds)
        {
            List<ItemQuantityDetail> objQty = new List<ItemQuantityDetail>();

            TransferDetailDAL objDetailDAL = new TransferDetailDAL(base.DataBaseName);
            TransferMasterDAL objMasterDAL = new TransferMasterDAL(base.DataBaseName);
            //List<TransferDetailDTO> objOrderDetailDTO = objDetailDAL.GetAllRecords(RoomID, CompanyID, false, false, SupplierIds).Where(x => x.ItemGUID == ItemGuid && x.RequestedQuantity > 0 && x.RequestedQuantity > x.ReceivedQuantity.GetValueOrDefault(0)).ToList();
            List<TransferDetailDTO> objOrderDetailDTO = objDetailDAL.GetTransferDetailDataByItemGUIDNormal(RoomID, CompanyID, SupplierIds, ItemGuid).ToList();

            foreach (var item in objOrderDetailDTO)
            {
                TransferMasterDTO orderDTO = objMasterDAL.GetTransferByGuidPlain(item.TransferGUID);
                if (orderDTO.RequestType == (int)RequestType.Out)
                {
                    ItemQuantityDetail itemQty = new ItemQuantityDetail();
                    itemQty.Name = orderDTO.TransferNumber;
                    itemQty.qty = (item.RequestedQuantity - item.ReceivedQuantity.GetValueOrDefault(0));
                    itemQty.ID = orderDTO.ID.ToString();
                    itemQty.status = orderDTO.TransferStatus.ToString();
                    itemQty.requirdate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy");
                    itemQty.requestType = orderDTO.RequestType.ToString();
                    objQty.Add(itemQty);
                }
            }


            return objQty;
        }

        public void GenerateDashboardData()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("EXEC [dbo].[InventoryDailyUpdates]");
            }
        }

        /// <summary>
        /// Get Status name for all the tables
        /// </summary>
        /// <param name="TableName">DataBase Table Name</param>
        /// <param name="StatusName">Status Name</param>
        /// <param name="CompanyID"></param>
        /// <param name="RoomID"></param>
        /// <returns>Status Count</returns>
        public List<RedCountDTO> GetRedCountByModuleType(string ModuleType, Int64 RoomID, Int64 CompanyID, List<long> SupplierIds, bool AllowUserToOrderConsnedItems = true)
        {
            if (CompanyID > 0 && RoomID > 0)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    string strSupplierIds = string.Empty;

                    if (SupplierIds != null && SupplierIds.Any())
                    {
                        strSupplierIds = string.Join(",", SupplierIds);
                    }

                    var params1 = new SqlParameter[] { new SqlParameter("@ModuleType", ModuleType),
                                                       new SqlParameter("@RoomID", RoomID),
                                                       new SqlParameter("@CompanyID", CompanyID),
                                                        new SqlParameter("@SupplierIds", strSupplierIds),
                                                        new SqlParameter("@AllowUserToOrderConsnedItems", ((!AllowUserToOrderConsnedItems) ? false: (object)DBNull.Value))

                                                       };
                    context.Database.CommandTimeout = 3600;
                    List<RedCountDTO> RedCountList = context.Database.SqlQuery<RedCountDTO>("EXEC dbo.GetRedCircleCount @ModuleType,@RoomID,@CompanyID,@SupplierIds,@AllowUserToOrderConsnedItems", params1).ToList();

                    return RedCountList;
                }
            }
            else
            {
                return new List<RedCountDTO>();
            }

        }

        #region Get Or Insert ID

        /// <summary>
        /// ShipVai
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="userID"></param>
        /// <param name="roomID"></param>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public Int64? GetOrInsertShipVaiIDByName(string Name, Int64 userID, Int64 roomID, Int64 companyID)
        {
            Int64? ID = null;
            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrWhiteSpace(Name))
            {
                ShipViaDAL objDAL = new ShipViaDAL(base.DataBaseName);
                ShipViaDTO dto = objDAL.GetShipViaByNamePlain(Name, roomID, companyID);
                if (dto != null && dto.ID > 0)
                {
                    ID = dto.ID;
                }
                else
                {
                    ShipViaDTO DTO = new ShipViaDTO()
                    {
                        ShipVia = Name,
                        Room = roomID,
                        CompanyID = companyID,
                        CreatedBy = userID,
                        LastUpdatedBy = userID,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow,
                        IsArchived = false,
                        IsDeleted = false,
                    };
                    ID = objDAL.Insert(DTO);
                }
            }
            return ID;
        }

        /// <summary>
        /// Customer
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="userID"></param>
        /// <param name="roomID"></param>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public Int64? GetOrInsertCustomerIDByName(string Name, Int64 userID, Int64 roomID, Int64 companyID)
        {
            Int64? ID = null;

            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrWhiteSpace(Name))
            {
                CustomerMasterDAL objDAL = new CustomerMasterDAL(base.DataBaseName);
                CustomerMasterDTO dtoList = objDAL.GetCustomerByName(Name, roomID, companyID);
                if (dtoList != null)
                {
                    ID = dtoList.ID;
                }
                else
                {
                    CustomerMasterDTO DTO = new CustomerMasterDTO() { Customer = Name, Room = roomID, CompanyID = companyID, CreatedBy = userID, Created = DateTime.Now, IsArchived = false, IsDeleted = false, };
                    if (DTO != null)
                    {
                        DTO.AddedFrom = "Web";
                        DTO.EditedFrom = "Web";
                        DTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        DTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    }
                    ID = objDAL.Insert(DTO);
                }
            }
            return ID;
        }


        public CustomerMasterDTO GetOrInsertCustomerGUIDByName(string Name, Int64 userID, Int64 roomID, Int64 companyID)
        {
            Guid? GuID = null;
            CustomerMasterDTO objCustomerMasterDTO = null;
            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrWhiteSpace(Name))
            {
                CustomerMasterDAL objDAL = new CustomerMasterDAL(base.DataBaseName);
                objCustomerMasterDTO = objDAL.GetCustomerByName(Name, roomID, companyID);
                if (objCustomerMasterDTO != null)
                {
                    GuID = objCustomerMasterDTO.GUID;
                }
                else
                {
                    objCustomerMasterDTO = new CustomerMasterDTO() { Customer = Name.Trim(), Room = roomID, CompanyID = companyID, CreatedBy = userID, Created = DateTime.Now, IsArchived = false, IsDeleted = false, };
                    objCustomerMasterDTO.AddedFrom = "Web";
                    objCustomerMasterDTO.EditedFrom = "Web";
                    objCustomerMasterDTO.GUID = Guid.NewGuid();
                    objCustomerMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objCustomerMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objCustomerMasterDTO.ID = objDAL.Insert(objCustomerMasterDTO);
                }
            }
            return objCustomerMasterDTO;
        }
        public ToolMasterDTO GetToolGUIDByName(string Name, Int64 roomID, Int64 companyID)
        {
            Guid? GuID = null;
            ToolMasterDTO objToolMasterDTO = null;
            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrWhiteSpace(Name))
            {
                ToolMasterDAL objDAL = new ToolMasterDAL(base.DataBaseName);
                objToolMasterDTO = objDAL.GetToolByName(Name, roomID, companyID);
                if (objToolMasterDTO != null)
                {
                    GuID = objToolMasterDTO.GUID;
                }
                else
                {
                    return null;
                }
            }
            return objToolMasterDTO;
        }
        public AssetMasterDTO GetAssetGUIDByName(string Name, Int64 roomID, Int64 companyID)
        {
            Guid? GuID = null;
            AssetMasterDTO objAssetMasterDTO = null;
            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrWhiteSpace(Name))
            {
                AssetMasterDAL objDAL = new AssetMasterDAL(base.DataBaseName);
                objAssetMasterDTO = objDAL.GetAssetByName(Name, roomID, companyID);
                if (objAssetMasterDTO != null)
                {
                    GuID = objAssetMasterDTO.GUID;
                }
                else
                {
                    return null;
                }
            }
            return objAssetMasterDTO;
        }

        public TechnicianMasterDTO GetOrInsertTechnician(string TechnicianCode, string TechnicianName, long RoomID, long CompanyID, long UserID)
        {
            TechnicialMasterDAL objDAL = new TechnicialMasterDAL(base.DataBaseName);
            TechnicianMasterDTO objTechnicianMasterDTO = objDAL.GetTechnicianByCodePlain(TechnicianCode, RoomID, CompanyID);
            if (objTechnicianMasterDTO == null)
            {
                objTechnicianMasterDTO = new TechnicianMasterDTO();
                objTechnicianMasterDTO.TechnicianCode = TechnicianCode;
                objTechnicianMasterDTO.Technician = TechnicianName;
                objTechnicianMasterDTO.Room = RoomID;
                objTechnicianMasterDTO.CompanyID = CompanyID;
                objTechnicianMasterDTO.CreatedBy = UserID;
                objTechnicianMasterDTO.LastUpdatedBy = UserID;
                objTechnicianMasterDTO.Created = DateTimeUtility.DateTimeNow;
                objTechnicianMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                objTechnicianMasterDTO.GUID = Guid.NewGuid();
                objTechnicianMasterDTO.IsArchived = false;
                objTechnicianMasterDTO.IsDeleted = false;
                Int64 TechnicanID = objDAL.Insert(objTechnicianMasterDTO);
                if (TechnicanID > 0)
                {
                    return objTechnicianMasterDTO;
                }
                else
                {
                    return null;
                }
            }

            return objTechnicianMasterDTO;


        }

        /// <summary>
        /// MaterialStaging
        /// </summary>
        /// <param name="stagingName"></param>
        /// <param name="userID"></param>
        /// <param name="roomID"></param>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public Int64? GetOrInsertMaterialStagingIDByName(string stagingName, Int64 userID, Int64 roomID, Int64 companyID)
        {
            Int64? MSID = null;
            if (!string.IsNullOrEmpty(stagingName) && !string.IsNullOrWhiteSpace(stagingName))
            {
                MaterialStagingDAL objDAL = new MaterialStagingDAL(base.DataBaseName);
                //IEnumerable<MaterialStagingDTO> MSDTOList = objDAL.GetAllRecords(roomID, companyID, false, false).Where(x => x.StagingName == stagingName && x.StagingStatus == 1);
                IEnumerable<MaterialStagingDTO> MSDTOList = objDAL.GetMaterialStaging(roomID, companyID, false, false, stagingName, 1);
                if (MSDTOList != null && MSDTOList.Count() > 0)
                {
                    MSID = MSDTOList.FirstOrDefault().ID;
                }
                else
                {
                    MaterialStagingDTO msDTO = new MaterialStagingDTO() { StagingName = stagingName, Room = roomID, CompanyID = companyID, CreatedBy = userID, LastUpdatedBy = userID, Updated = DateTimeUtility.DateTimeNow, Created = DateTimeUtility.DateTimeNow, IsArchived = false, IsDeleted = false, StagingStatus = 1 };
                    msDTO.AddedFrom = "Web";
                    msDTO.EditedFrom = "Web";
                    msDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    msDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    MSID = objDAL.Insert(msDTO);
                }
            }
            return MSID;
        }

        public Guid? GetOrInsertMaterialStagingGUIDByName(string stagingName, Int64 userID, Int64 roomID, Int64 companyID)
        {
            Guid? MSGUID = null;
            if (!string.IsNullOrEmpty(stagingName) && !string.IsNullOrWhiteSpace(stagingName))
            {
                MaterialStagingDAL objDAL = new MaterialStagingDAL(base.DataBaseName);
                //IEnumerable<MaterialStagingDTO> MSDTOList = objDAL.GetAllRecords(roomID, companyID, false, false).Where(x => x.StagingName == stagingName);
                IEnumerable<MaterialStagingDTO> MSDTOList = objDAL.GetMaterialStaging(roomID, companyID, false, false, stagingName, null);
                if (MSDTOList != null && MSDTOList.Count() > 0)
                {
                    MSGUID = MSDTOList.FirstOrDefault().GUID;
                }
                else
                {
                    MaterialStagingDTO msDTO = new MaterialStagingDTO() { StagingName = stagingName, Room = roomID, CompanyID = companyID, CreatedBy = userID, Created = DateTime.Now, IsArchived = false, IsDeleted = false, StagingStatus = 1 };
                    msDTO.AddedFrom = "Web";
                    msDTO.EditedFrom = "Web";
                    msDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    msDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    Int64 MSID = objDAL.Insert(msDTO);
                    MaterialStagingDTO msDTONew = objDAL.GetRecord(MSID, roomID, companyID);
                    if (msDTO != null)
                        MSGUID = msDTONew.GUID;
                }
            }
            return MSGUID;
        }

        /// <summary>
        /// Vendor
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="userID"></param>
        /// <param name="roomID"></param>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public Int64? GetOrInsertVendorIDByName(string Name, Int64 userID, Int64 roomID, Int64 companyID)
        {
            Int64? ID = null;
            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrWhiteSpace(Name))
            {
                VenderMasterDAL objDAL = new VenderMasterDAL(base.DataBaseName);
                VenderMasterDTO dto = objDAL.GetVenderByNamePlain(Name, roomID, companyID);
                if (dto != null && dto.ID > 0)
                {
                    ID = dto.ID;
                }
                else
                {
                    VenderMasterDTO DTO = new VenderMasterDTO()
                    {
                        Vender = Name,
                        Room = roomID,
                        CompanyID = companyID,
                        CreatedBy = userID,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow,
                        IsArchived = false,
                        IsDeleted = false,
                        LastUpdatedBy = userID

                    };
                    ID = objDAL.Insert(DTO);
                }
            }
            return ID;
        }

        public Guid? GetOrInsertSupplierAccountByName(string Name, Int64 userID, Int64 roomID, Int64 companyID, Int64 supplierID)
        {
            Guid? SuppAccGUID = Guid.Empty;
            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrWhiteSpace(Name))
            {
                SupplierAccountDetailsDAL objDAL = new SupplierAccountDetailsDAL(base.DataBaseName);
                IEnumerable<SupplierAccountDetailsDTO> dtoList = objDAL.GetAllAccountsBySupplierID(supplierID, roomID, companyID).Where(x => x.AccountNo == Name);
                if (dtoList != null && dtoList.Count() > 0)
                {
                    SuppAccGUID = dtoList.FirstOrDefault().GUID;
                }
                else
                {
                    List<SupplierAccountDetailsDTO> lstSupplierAccountDetails = new List<SupplierAccountDetailsDTO>();
                    lstSupplierAccountDetails = objDAL.GetAllAccountsBySupplierID(supplierID, roomID, companyID).ToList();

                    //bool IsDefault = false;
                    SupplierAccountDetailsDTO objSuppAccountDefault = (from m in lstSupplierAccountDetails
                                                                       where m.SupplierID == supplierID && m.Room == roomID && m.CompanyID == companyID && m.IsDefault == true
                                                                       select m).FirstOrDefault();
                    //if (objSuppAccountDefault == null)
                    //    IsDefault = true;

                    //Guid NewGUID = Guid.NewGuid();
                    SupplierAccountDetailsDTO oSupplierAccountDetailsDTO = new SupplierAccountDetailsDTO();
                    oSupplierAccountDetailsDTO.SupplierID = supplierID;
                    oSupplierAccountDetailsDTO.AccountNo = Name;
                    oSupplierAccountDetailsDTO.AccountName = Name;
                    oSupplierAccountDetailsDTO.IsDefault = objSuppAccountDefault == null ? true : false;
                    oSupplierAccountDetailsDTO.Created = DateTimeUtility.DateTimeNow;
                    oSupplierAccountDetailsDTO.CreatedBy = userID;
                    oSupplierAccountDetailsDTO.Updated = DateTimeUtility.DateTimeNow;
                    oSupplierAccountDetailsDTO.LastUpdatedBy = userID;
                    oSupplierAccountDetailsDTO.Room = roomID;
                    oSupplierAccountDetailsDTO.CompanyID = companyID;
                    oSupplierAccountDetailsDTO.IsArchived = false;
                    oSupplierAccountDetailsDTO.IsDeleted = false;
                    oSupplierAccountDetailsDTO.GUID = Guid.NewGuid();
                    objDAL.Insert(oSupplierAccountDetailsDTO);
                    Int64 NewInsertID = 0;
                    NewInsertID = oSupplierAccountDetailsDTO.ID;
                    if (NewInsertID > 0)
                    {
                        SuppAccGUID = oSupplierAccountDetailsDTO.GUID;
                    }
                }
            }
            return SuppAccGUID;
        }


        /// <summary>
        /// Vendor
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="userID"></param>
        /// <param name="roomID"></param>
        /// <param name="companyID"></param>
        /// <returns></returns>
        //public Int64? GetOrInsertBinIDByName(string Name, Int64 userID, Int64 roomID, Int64 companyID, bool IsStagingLoc = false)
        //{
        //    Int64? ID = null;
        //    if (!string.IsNullOrEmpty(Name) && !string.IsNullOrWhiteSpace(Name))
        //    {
        //        //string BinName = Name.Substring(0, Name.IndexOf("(")).Trim();
        //        BinMasterDAL objDAL = new BinMasterDAL(base.DataBaseName);
        //        IEnumerable<BinMasterDTO> dtoList = objDAL.GetAllRecords(roomID, companyID, false, false).Where(x => x.BinNumber == Name);
        //        if (dtoList != null && dtoList.Count() > 0)
        //        {
        //            ID = dtoList.FirstOrDefault().ID;
        //        }
        //        else
        //        {
        //            BinMasterDTO DTO = new BinMasterDTO() { BinNumber = Name, Room = roomID, CompanyID = companyID, CreatedBy = userID, Created = DateTime.Now, IsArchived = false, IsDeleted = false, IsStagingLocation = IsStagingLoc };
        //            ID = objDAL.Insert(DTO);
        //        }
        //    }
        //    return ID;
        //}


        public Int64? GetOrInsertBinIDByNameItemGuid(Guid ItemGUID, string Name, Int64 userID, Int64 roomID, Int64 companyID, bool IsStagingLoc = false)
        {
            Int64? ID = null;
            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrWhiteSpace(Name))
            {
                //string BinName = Name.Substring(0, Name.IndexOf("(")).Trim();

                BinMasterDAL objItemLocationDetailsDAL = new BinMasterDAL(base.DataBaseName);
                BinMasterDTO objBinMasterDTO = objItemLocationDetailsDAL.GetItemBinPlain(ItemGUID, Name, roomID, companyID, userID, IsStagingLoc);
                ID = objBinMasterDTO.ID;
                //BinMasterDAL objDAL = new BinMasterDAL(base.DataBaseName);
                //IEnumerable<BinMasterDTO> dtoList = objDAL.GetAllRecords(roomID, companyID, false, false).Where(x => x.BinNumber == Name && x.ItemGUID == ItemGUID && x.IsStagingLocation == IsStagingLoc);
                //if (dtoList != null && dtoList.Count() > 0)
                //{
                //    ID = dtoList.FirstOrDefault().ID;
                //}
                //else
                //{
                //    BinMasterDTO DTO = new BinMasterDTO() { ItemGUID = ItemGUID, BinNumber = Name, Room = roomID, CompanyID = companyID, CreatedBy = userID, Created = DateTime.Now, LastUpdatedBy = userID, LastUpdated = DateTime.Now, IsArchived = false, IsDeleted = false, IsStagingLocation = IsStagingLoc, CriticalQuantity = 0, MinimumQuantity = 0, MaximumQuantity = 0 };
                //    ID = objDAL.Insert(DTO);
                //}
            }
            return ID;
        }

        /// <summary>
        /// Vendor
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="userID"></param>
        /// <param name="roomID"></param>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public Int64? GetOrInsertBinIDByName(Guid ItemGuid, string Name, Int64 userID, Int64 roomID, Int64 companyID, bool IsStagingLoc = false)
        {
            BinMasterDAL objItemLocQtyDAL = new BinMasterDAL(base.DataBaseName);

            Int64? BinID = GetOrInsertBinIDByNameItemGuid(ItemGuid, Name, userID, roomID, companyID, IsStagingLoc);

            if (!IsStagingLoc)
            {
                //BinMasterDTO itmLocLvQty = objItemLocQtyDAL.GetCachedDataLocationQty(ItemGuid, BinID.GetValueOrDefault(0), roomID, companyID);
                BinMasterDTO itmLocLvQty = objItemLocQtyDAL.GetBinByID(BinID.GetValueOrDefault(0), roomID, companyID);
                if (itmLocLvQty == null)
                {
                    itmLocLvQty = new BinMasterDTO()
                    {
                        ID = BinID.GetValueOrDefault(0),
                        BinNumber = Name,
                        ItemGUID = ItemGuid,
                        CreatedBy = userID,
                        CompanyID = companyID,
                        Room = roomID,
                        LastUpdatedBy = userID,
                        Created = DateTime.Now,
                        LastUpdated = DateTime.Now,
                        GUID = Guid.NewGuid(),
                        IsStagingHeader = false,
                        IsStagingLocation = false,
                        CriticalQuantity = 0,
                        MinimumQuantity = 0,
                        MaximumQuantity = 0,
                        SuggestedOrderQuantity = 0,
                        AddedFrom = "Web",
                        EditedFrom = "Web",
                        ReceivedOn = DateTimeUtility.DateTimeNow,
                        ReceivedOnWeb = DateTimeUtility.DateTimeNow
                    };

                    objItemLocQtyDAL.Insert(itmLocLvQty);
                }
            }
            return BinID;
        }


        #endregion

        public EnterpriseDTO GetEnterpriseByID(long enterpriseID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseETurnsMasterEntityConnectionString)) //using (var context = new eTurns_MasterEntities(base.DataBaseETurnsMasterEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Id", enterpriseID) };
                return context.Database.SqlQuery<EnterpriseDTO>("exec [GetEnterpriseByIdPlain] @Id", params1).FirstOrDefault();
            }
        }

        public void UpdateTurnsAverageUsage(Guid ItemGuid, Int64 RoomID, Int64 CompanyID, double? PullQty, double? PullCost, double? OrderQty, long SessionUserId)
        {
            #region "Update Turns and Average Usage"
            ItemMasterDAL objItem = new ItemMasterDAL(base.DataBaseName);
            ItemMasterDTO ItemDTO = objItem.GetItemWithoutJoins(null, ItemGuid);
            InventoryDashboardDTO InvDashDTO = new InventoryDashboardDTO();//objItem.GetHeaderCountByItemID(RoomID, CompanyID, ItemGuid.ToString());
            RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
            CommonDAL objCommonDAL = new CommonDAL(base.DataBaseName);
            //RoomDTO objRoomDTO = objRoomDAL.GetRoomByIDPlain(RoomID);
            string columnList = "ID,RoomName,IsAverageUsageBasedOnPull,TrendingFormulaType";
            RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
            if (ItemDTO != null)
            {
                if (InvDashDTO != null)
                    ItemDTO.Turns = Convert.ToDouble(InvDashDTO.Turns.GetValueOrDefault(0));

                if (objRoomDTO != null && PullQty.GetValueOrDefault(0) > 0 && objRoomDTO.IsAverageUsageBasedOnPull)
                {
                    double ItemInventoryValue = ItemDTO.OnHandQuantity.GetValueOrDefault(0) * ItemDTO.Cost.GetValueOrDefault(1);
                    double TotalQtyPUlled = PullQty.GetValueOrDefault(0) * PullCost.GetValueOrDefault(1);
                    double AverageUsage = TotalQtyPUlled / (ItemInventoryValue / PullQty.GetValueOrDefault(1));
                    ItemDTO.AverageUsage = AverageUsage;
                    ItemDTO.WhatWhereAction = "Pull";
                    objItem.Edit(ItemDTO, SessionUserId, 0);
                }
                else if (objRoomDTO != null && objRoomDTO.TrendingFormulaType.GetValueOrDefault(0) == 3 && PullQty.GetValueOrDefault(0) <= 0 && OrderQty.GetValueOrDefault(0) > 0)
                {
                    // Implement Order Quanity Average usage.
                    // 365 No of days and 90 include days to calculate average usage they will come from dashboard setting or may be room setting
                    double AverageUsage = 365 * (ItemDTO.OnOrderQuantity.GetValueOrDefault(0) / 90);
                    ItemDTO.AverageUsage = AverageUsage;
                    ItemDTO.WhatWhereAction = "Update Average Usag by Order Qty.";
                    objItem.Edit(ItemDTO, SessionUserId, 0);
                }

            }
            #endregion
        }

        public static string GeteTurnsImage(string path, string imagePath)
        {
            string str = string.Empty;

            str = @"<a href='" + path + @"' title=""E Turns Powered""> <img alt=""E Turns Powered"" src='" + path + imagePath + @"' style=""border: 0px currentColor; border-image: none;"" /></a>";
            return str;
        }

        public string CheckDuplicateSerialNumbers(string SrNumber, int ID, Int64 RoomID, Int64 CompanyID, Guid ItemGUID)
        {
            string msg = "";
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new List<SqlParameter> { new SqlParameter("@SrNumber", SrNumber),
                                                       new SqlParameter("@RoomID", RoomID),
                                                       new SqlParameter("@CompanyID", CompanyID),
                                                        new SqlParameter("@ItemGUID", ItemGUID),
                                                       };

                msg = context.Database.SqlQuery<string>("EXEC dbo.uspCheckDuplicateSrNo @SrNumber,@RoomID,@CompanyID,@ItemGUID"
                    , params1.ToArray()).FirstOrDefault<string>();


                //var qry = (from em in context.ItemLocationDetails
                //           where em.Room == RoomID && em.CompanyID == CompanyID
                //           && em.IsDeleted == false && em.IsArchived == false
                //           && em.ItemGUID == ItemGUID && em.ID != ID
                //           && ((em.ConsignedQuantity ?? 0) + (em.CustomerOwnedQuantity ?? 0)) > 0
                //           select em);

                //var MSqry = (from em in context.MaterialStagingPullDetails
                //             where em.Room == RoomID && em.CompanyID == CompanyID
                //             && em.IsDeleted == false && em.IsArchived == false
                //             && em.ItemGUID == ItemGUID
                //             && ((em.ConsignedQuantity ?? 0) + (em.CustomerOwnedQuantity ?? 0)) > 0
                //             select em);

                //if (qry.Any(x => x.SerialNumber.ToLower().Trim() == SrNumber.ToLower().Trim())
                //   || MSqry.Any(x => x.SerialNumber.ToLower().Trim() == SrNumber.ToLower().Trim()))
                //{
                //    msg = "duplicate";
                //}
                //else
                //{
                //    msg = "ok";
                //}

            }
            return msg;
        }

        public string CheckDuplicateLotAndExpiration(string LotNumber, string Expiration, DateTime ExpirationDate, int ID, Int64 RoomID, Int64 CompanyID, Guid ItemGUID, Int64 UserID, Int64 EnterpriseID, bool IsStagingLocation = false)
        {
            string msg = "";

            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var qry = (from em in context.ItemLocationDetails
                           where em.Room == RoomID && em.CompanyID == CompanyID
                           && em.IsDeleted == false && em.IsArchived == false
                           && em.ItemGUID == ItemGUID && em.ID != ID
                           select em).ToList();

                string currentCulture = "en-US";
                if (HttpContext.Current != null)
                {
                    if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                    {
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                        {
                            currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                        }

                    }
                }
                else
                {
                    eTurns.DTO.eTurnsRegionInfo objeTurnsRegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomID, CompanyID, UserID);
                    currentCulture = objeTurnsRegionInfo.CultureName;
                }
                string ResourceFilePullMaster = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResPullMaster", currentCulture, EnterpriseID, CompanyID);
                string MsgExpirationLotAvailable = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgExpirationLotAvailable", ResourceFilePullMaster, EnterpriseID, CompanyID, RoomID, "ResPullMaster", currentCulture);
                if (qry.Any(x => (x.LotNumber ?? string.Empty).ToLower().Trim() == (LotNumber ?? string.Empty).ToLower().Trim()))
                {
                    //&& (x.Expiration ?? string.Empty) != Expiration
                    var availableLot = qry.Where(x => (x.LotNumber ?? string.Empty).ToLower().Trim() == (LotNumber ?? string.Empty).ToLower().Trim()
                                                   && (x.ExpirationDate.GetValueOrDefault(DateTime.MinValue) != DateTime.MinValue)
                                                   && (x.ExpirationDate.GetValueOrDefault(DateTime.MinValue).Date != ExpirationDate.Date)).FirstOrDefault();

                    if (availableLot != null && availableLot.ID > 0)
                    {
                        msg = MsgExpirationLotAvailable.Replace("{0}", availableLot.LotNumber).Replace("{1}", availableLot.Expiration);
                    }
                    else
                    {
                        msg = "ok";
                    }
                }
                else
                {
                    msg = "ok";
                }

                if (msg == "ok")
                {
                    var qryMS = (from em in context.MaterialStagingPullDetails
                                 where em.Room == RoomID && em.CompanyID == CompanyID
                                 && em.IsDeleted == false && em.IsArchived == false
                                 && em.ItemGUID == ItemGUID && em.ID != ID
                                 select em).ToList();
                    if (qryMS.Any(x => (x.LotNumber ?? string.Empty).ToLower().Trim() == (LotNumber ?? string.Empty).ToLower().Trim()))
                    {
                        //&& (x.Expiration ?? string.Empty) != Expiration
                        var availableLot = qryMS.Where(x => (x.LotNumber ?? string.Empty).ToLower().Trim() == (LotNumber ?? string.Empty).ToLower().Trim()
                                                       && (x.ExpirationDate.GetValueOrDefault(DateTime.MinValue) != DateTime.MinValue)
                                                       && (x.ExpirationDate.GetValueOrDefault(DateTime.MinValue).Date != ExpirationDate.Date)).FirstOrDefault();

                        if (availableLot != null && availableLot.ID > 0)
                        {
                            msg = MsgExpirationLotAvailable.Replace("{0}", availableLot.LotNumber).Replace("{1}", availableLot.Expiration);
                        }
                        else
                        {
                            msg = "ok";
                        }
                    }
                    else
                    {
                        msg = "ok";
                    }
                }
            }

            return msg;
        }


        public string CheckDuplicateSerialNumbers(string SrNumber, Guid ItemLocationGUID, Int64 RoomID, Int64 CompanyID, Guid ItemGUID)
        {
            string msg = "";
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var qry = (from em in context.ItemLocationDetails
                           where em.Room == RoomID && em.CompanyID == CompanyID
                           && em.IsDeleted == false && em.IsArchived == false
                           && em.ItemGUID == ItemGUID && em.GUID != ItemLocationGUID
                           select em);
                if (qry.Any(x => x.SerialNumber.ToLower().Trim() == SrNumber.ToLower().Trim()))
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

        public string CheckDuplicateSerialNumbersForMove(string SrNumber, Guid ItemLocationGUID, Int64 RoomID, Int64 CompanyID, Guid ItemGUID)
        {
            string msg = "";
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var qry = (from em in context.ItemLocationDetails
                           where em.Room == RoomID && em.CompanyID == CompanyID
                           && em.IsDeleted == false && em.IsArchived == false
                           && em.ItemGUID == ItemGUID && em.GUID != ItemLocationGUID
                           && ((em.ConsignedQuantity ?? 0) + (em.CustomerOwnedQuantity ?? 0)) > 0
                           select em);
                if (qry.Any(x => x.SerialNumber.ToLower().Trim() == SrNumber.ToLower().Trim()))
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

        public string CheckDuplicateMSSerialNumbers(string SrNumber, Int64 RoomID, Int64 CompanyID, Guid ItemGUID)
        {
            string msg = "";
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var qry = (from em in context.MaterialStagingPullDetails
                           where em.Room == RoomID && em.CompanyID == CompanyID
                           && em.IsDeleted == false && em.IsArchived == false
                           && em.ItemGUID == ItemGUID
                           && ((em.ConsignedQuantity ?? 0) + (em.CustomerOwnedQuantity ?? 0)) > 0
                           select em);
                if (qry.Any(x => x.SerialNumber.ToLower().Trim() == SrNumber.ToLower().Trim()))
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

        /// <summary>
        /// Get Apha Numeric UniqueKey
        /// </summary>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        private string GetAphaNumericUniqueKey(int maxSize)
        {
            char[] chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        public string GetItemUniqueIdByRoom(Int64 RoomId, Int64 CompanyId)
        {
            string strUniqueId = GetAphaNumericUniqueKey(6);

            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{
            //    var duplicates = context.ItemMasters.Where(x => x.Room == RoomId && x.CompanyID == CompanyId && x.ItemUniqueNumber == strUniqueId).Select(x => x.ItemUniqueNumber);
            //    if (duplicates != null && duplicates.Count() > 0)
            //        GetItemUniqueIdByRoom(RoomId, CompanyId);
            //}

            return strUniqueId;
        }

        public string UniqueItemId(int ItemNumber = 10)
        {
            //return String.Format("{0:0000000000}", (DateTime.Now.Ticks / 10) % 1000000000);
            //Random ran = new Random((int)DateTime.Now.Ticks);
            //int key = ran.Next(1000000000, int.MaxValue);
            //return String.Format("{0:0000000000}", key);
            Random r = null;
            RandomNumberGenerator rng = null;
            byte[] bytes = null;
            try
            {
                System.Threading.Thread.Sleep(101);
                r = new Random();
                int idx = r.Next(0, 9);
                bytes = new byte[25];
                rng = RandomNumberGenerator.Create();
                rng.GetBytes(bytes);
                Int64 random = Math.Abs(BitConverter.ToInt64(bytes, idx) % 10000000000);
                return String.Format("{0:D10}", random);
            }
            catch (Exception)
            {
                r = new Random((int)DateTime.Now.Ticks);
                int key = r.Next(1000000000, int.MaxValue);
                return String.Format("{0:0000000000}", key);

                //throw;
            }
            finally
            {
                rng.Dispose();
                r = null;
                bytes = null;


            }
        }

        public static Dictionary<string, Regex> GetRegexDictionary()
        {

            Dictionary<string, Regex> dic = new Dictionary<string, Regex>();

            dic.Add("NNNNN", new Regex(@"^[0-9]{1,9}$"));

            dic.Add("M-d-yyyy", new Regex(@"^([1-9]|0[1-9]|1[0-2])[-]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-](1[9][0-9][0-9]|2[0][0-9][0-9])$"));
            dic.Add("M/d/yyyy", new Regex(@"^([1-9]|0[1-9]|1[0-2])[/]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[/](1[9][0-9][0-9]|2[0][0-9][0-9])$"));
            dic.Add("M.d.yyyy", new Regex(@"^([1-9]|0[1-9]|1[0-2])[.]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[.](1[9][0-9][0-9]|2[0][0-9][0-9])$"));

            dic.Add("d-M-yyyy", new Regex(@"^([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([1-9]|0[1-9]|1[0-2])[-](1[9][0-9][0-9]|2[0][0-9][0-9])$"));
            dic.Add("d/M/yyyy", new Regex(@"^([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[/]([1-9]|0[1-9]|1[0-2])[/](1[9][0-9][0-9]|2[0][0-9][0-9])$"));
            dic.Add("d.M.yyyy", new Regex(@"^([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[.]([1-9]|0[1-9]|1[0-2])[.](1[9][0-9][0-9]|2[0][0-9][0-9])$"));

            dic.Add("yyyy-M-d", new Regex(@"^(1[9][0-9][0-9]|2[0][0-9][0-9])[-]([1-9]|0[1-9]|1[0-2])[-]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])$"));
            dic.Add("yyyy/M/d", new Regex(@"^(1[9][0-9][0-9]|2[0][0-9][0-9])[/]([1-9]|0[1-9]|1[0-2])[/]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])$"));
            dic.Add("yyyy.M.d", new Regex(@"^(1[9][0-9][0-9]|2[0][0-9][0-9])[.]([1-9]|0[1-9]|1[0-2])[.]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])$"));

            dic.Add("yyyy-d-M", new Regex(@"^(1[9][0-9][0-9]|2[0][0-9][0-9])[-]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([1-9]|0[1-9]|1[0-2])$"));
            dic.Add("yyyy/d/M", new Regex(@"^(1[9][0-9][0-9]|2[0][0-9][0-9])[/]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[/]([1-9]|0[1-9]|1[0-2])$"));
            dic.Add("yyyy.d.M", new Regex(@"^(1[9][0-9][0-9]|2[0][0-9][0-9])[.]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[.]([1-9]|0[1-9]|1[0-2])$"));

            dic.Add("M-d-yyyy-NNNNN", new Regex(@"^([1-9]|0[1-9]|1[0-2])[-]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-](1[9][0-9][0-9]|2[0][0-9][0-9])[-]([0-9]{1,5})$"));
            dic.Add("M/d/yyyy-NNNNN", new Regex(@"^([1-9]|0[1-9]|1[0-2])[/]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[/](1[9][0-9][0-9]|2[0][0-9][0-9])[-]([0-9]{1,5})$"));
            dic.Add("M.d.yyyy-NNNNN", new Regex(@"^([1-9]|0[1-9]|1[0-2])[.]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[.](1[9][0-9][0-9]|2[0][0-9][0-9])[-]([0-9]{1,5})$"));

            dic.Add("d-M-yyyy-NNNNN", new Regex(@"^([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([1-9]|0[1-9]|1[0-2])[-](1[9][0-9][0-9]|2[0][0-9][0-9])[-]([0-9]{1,5})$"));
            dic.Add("d/M/yyyy-NNNNN", new Regex(@"^([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[/]([1-9]|0[1-9]|1[0-2])[/](1[9][0-9][0-9]|2[0][0-9][0-9])[-]([0-9]{1,5})$"));
            dic.Add("d.M.yyyy-NNNNN", new Regex(@"^([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[.]([1-9]|0[1-9]|1[0-2])[.](1[9][0-9][0-9]|2[0][0-9][0-9])[-]([0-9]{1,5})$"));

            dic.Add("yyyy-M-d-NNNNN", new Regex(@"^(1[9][0-9][0-9]|2[0][0-9][0-9])[-]([1-9]|0[1-9]|1[0-2])[-]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([0-9]{1,5})$"));
            dic.Add("yyyy/M/d-NNNNN", new Regex(@"^(1[9][0-9][0-9]|2[0][0-9][0-9])[/]([1-9]|0[1-9]|1[0-2])[/]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([0-9]{1,5})$"));
            dic.Add("yyyy.M.d-NNNNN", new Regex(@"^(1[9][0-9][0-9]|2[0][0-9][0-9])[.]([1-9]|0[1-9]|1[0-2])[.]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([0-9]{1,5})$"));

            dic.Add("yyyy-d-M-NNNNN", new Regex(@"^(1[9][0-9][0-9]|2[0][0-9][0-9])[-]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([1-9]|0[1-9]|1[0-2])[-]([0-9]{1,5})$"));
            dic.Add("yyyy/d/M-NNNNN", new Regex(@"^(1[9][0-9][0-9]|2[0][0-9][0-9])[/]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[/]([1-9]|0[1-9]|1[0-2])[-]([0-9]{1,5})$"));
            dic.Add("yyyy.d.M-NNNNN", new Regex(@"^(1[9][0-9][0-9]|2[0][0-9][0-9])[.]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[.]([1-9]|0[1-9]|1[0-2])[-]([0-9]{1,5})$"));


            dic.Add("M-d-yy", new Regex(@"^([1-9]|0[1-9]|1[0-2])[-]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([0-9]{2})$"));
            dic.Add("M/d/yy", new Regex(@"^([1-9]|0[1-9]|1[0-2])[/]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[/]([0-9]{2})$"));
            dic.Add("M.d.yy", new Regex(@"^([1-9]|0[1-9]|1[0-2])[.]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[.]([0-9]{2})$"));

            dic.Add("d-M-yy", new Regex(@"^([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([1-9]|0[1-9]|1[0-2])[-]([0-9]{2})$"));
            dic.Add("d/M/yy", new Regex(@"^([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[/]([1-9]|0[1-9]|1[0-2])[/]([0-9]{2})$"));
            dic.Add("d.M.yy", new Regex(@"^([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[.]([1-9]|0[1-9]|1[0-2])[.]([0-9]{2})$"));

            dic.Add("yy-M-d", new Regex(@"^([0-9]{2})[-]([1-9]|0[1-9]|1[0-2])[-]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])$"));
            dic.Add("yy/M/d", new Regex(@"^([0-9]{2})[/]([1-9]|0[1-9]|1[0-2])[/]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])$"));
            dic.Add("yy.M.d", new Regex(@"^([0-9]{2})[.]([1-9]|0[1-9]|1[0-2])[.]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])$"));

            dic.Add("yy-d-M", new Regex(@"^([0-9]{2})[-]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([1-9]|0[1-9]|1[0-2])$"));
            dic.Add("yy/d/M", new Regex(@"^([0-9]{2})[/]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[/]([1-9]|0[1-9]|1[0-2])$"));
            dic.Add("yy.d.M", new Regex(@"^([0-9]{2})[.]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[.]([1-9]|0[1-9]|1[0-2])$"));

            dic.Add("M-d-yy-NNNNN", new Regex(@"^([1-9]|0[1-9]|1[0-2])[-]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([0-9]{2})[-]([0-9]{1,5})$"));
            dic.Add("M/d/yy-NNNNN", new Regex(@"^([1-9]|0[1-9]|1[0-2])[/]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[/]([0-9]{2})[-]([0-9]{1,5})$"));
            dic.Add("M.d.yy-NNNNN", new Regex(@"^([1-9]|0[1-9]|1[0-2])[.]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[.]([0-9]{2})[-]([0-9]{1,5})$"));

            dic.Add("d-M-yy-NNNNN", new Regex(@"^([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([1-9]|0[1-9]|1[0-2])[-]([0-9]{2})[-]([0-9]{1,5})$"));
            dic.Add("d/M/yy-NNNNN", new Regex(@"^([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[/]([1-9]|0[1-9]|1[0-2])[/]([0-9]{2})[-]([0-9]{1,5})$"));
            dic.Add("d.M.yy-NNNNN", new Regex(@"^([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[.]([1-9]|0[1-9]|1[0-2])[.]([0-9]{2})[-]([0-9]{1,5})$"));

            dic.Add("yy-M-d-NNNNN", new Regex(@"^([0-9]{2})[-]([1-9]|0[1-9]|1[0-2])[-]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([0-9]{1,5})$"));
            dic.Add("yy/M/d-NNNNN", new Regex(@"^([0-9]{2})[/]([1-9]|0[1-9]|1[0-2])[/]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([0-9]{1,5})$"));
            dic.Add("yy.M.d-NNNNN", new Regex(@"^([0-9]{2})[.]([1-9]|0[1-9]|1[0-2])[.]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([0-9]{1,5})$"));

            dic.Add("yy-d-M-NNNNN", new Regex(@"^([0-9]{2})[-]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([1-9]|0[1-9]|1[0-2])[-]([0-9]{1,5})$"));
            dic.Add("yy/d/M-NNNNN", new Regex(@"^([0-9]{2})[/]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[/]([1-9]|0[1-9]|1[0-2])[-]([0-9]{1,5})$"));
            dic.Add("yy.d.M-NNNNN", new Regex(@"^([0-9]{2})[.]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[.]([1-9]|0[1-9]|1[0-2])[-]([0-9]{1,5})$"));


            return dic;
        }

        public static string GetSortingString(string StringToSort)
        {
            Dictionary<string, Regex> dic = CommonDAL.GetRegexDictionary();
            string inputString = StringToSort;
            string returnString = StringToSort;
            try
            {
                foreach (var item in dic)
                {
                    if (item.Value.IsMatch(inputString))
                    {
                        Match mat = item.Value.Match(inputString);
                        if (!string.IsNullOrEmpty(mat.Value))
                        {
                            string strkey = item.Key;

                            if (item.Key.StartsWith("NNNNN"))
                            {
                                returnString = mat.Value.PadLeft(13, '0');
                            }
                            else if (item.Key.EndsWith("-NNNNN"))
                            {
                                int idx = mat.Value.LastIndexOf("-");
                                string strDateVal = mat.Value.Substring(0, idx);
                                string strnumber = mat.Value.Remove(0, idx + 1);

                                DateTime dtval = DateTime.ParseExact(strDateVal, item.Key.Substring(0, item.Key.LastIndexOf("-")), new CultureInfo("en-US"));
                                returnString = dtval.ToString("yyyyMMdd") + strnumber.PadLeft(5, '0');
                            }
                            else if (item.Key.EndsWith("#NNNNN"))
                            {
                                int idx = mat.Value.LastIndexOf("#");
                                string strDateVal = mat.Value.Substring(0, idx);
                                string strnumber = mat.Value.Remove(0, idx);

                                DateTime dtval = DateTime.ParseExact(strDateVal, item.Key.Substring(0, item.Key.LastIndexOf("#")), new CultureInfo("en-US"));
                                returnString = dtval.ToString("yyyyMMdd") + strnumber.PadLeft(5, '0');
                            }
                            else if (item.Key.Contains("yyyy"))
                            {
                                DateTime dtval = DateTime.ParseExact(mat.Value, item.Key, new CultureInfo("en-US"));
                                returnString = dtval.ToString("yyyyMMdd") + "00000";
                            }
                            else if (item.Key.Contains("yy"))
                            {
                                DateTime dtval = DateTime.ParseExact(mat.Value, item.Key, new CultureInfo("en-US"));
                                returnString = dtval.ToString("yyyyMMdd") + "00000";
                            }
                            break;
                        }
                    }
                }
                return returnString;
            }
            catch (Exception)
            {
                return returnString;
            }
        }

        public static string GeteTurnsDatabase()
        {
            try
            {
                return ConfigurationManager.AppSettings["TemplateDbName"].ToString();
            }
            catch
            {
                return "eTurns";
            }
        }

        public IEnumerable<T> GetDataForReportFilterList<T>(string SpName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate,
                                        Int64? UserID, string[] OrderStatus = null, string QOHFilters = "", string OnlyExirationItems = "",
                                        bool IsRoomIdFilter = true, string QuantityType = "", string[] arrTransferStatus = null,
                                        string[] arrTransferRequestTypes = null, string OnlyExpirationItems = "", string DaysToApproveOrder = "",
                                        string DaysUntilItemExpires = "", string ProjectExpirationDate = "", string CountAppliedFilter = "", int FilterDateOn = 0,
                                        bool _isRunWithReportConnection = false, string[] arrItemIsActive = null,
                                        string DateCreatedEarlier = null, string DateActiveLater = null, string _usageType = "Consolidate",
                                        string AUDayOfUsageToSample = null, string AUMeasureMethod = null, string MinMaxDayOfAverage = null,
                                        string MinMaxMinNumberOfTimesMax = null, string[] WOStatus = null, int DecimalPointFromConfig = 0,
                                        string reportModuleName = "", string SelectedCartType = null, string Includestockedouttools = null,
                                        string Days = null, string ItemTypeFilter = null, bool IsAllowedZeroPullUsage = false) where T : class
        {
            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {
                context.Database.CommandTimeout = 3600;
                string compIDs = string.Empty;
                string roomIDs = string.Empty;
                string itemIsActive = string.Empty;
                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }
                if (arrItemIsActive != null)
                {
                    for (int i = 0; i < arrItemIsActive.Length; i++)
                    {
                        if (itemIsActive.Length > 0)
                            itemIsActive += ",";

                        itemIsActive += arrItemIsActive[i];
                    }
                }
                string strQuery = "Exec  " + SpName + " ";
                if (!SpName.ToLower().Equals("RPT_SerialNumberList".ToLower()))
                {
                    if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                        strQuery += "'" + Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                    else
                        strQuery += "NULL";

                    if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                        strQuery += ",'" + Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                    else
                        strQuery += ", NULL";
                }
                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {
                        if (SpName.ToLower().Equals("RPT_SerialNumberList".ToLower()))
                        {
                            strQuery += "'" + roomIDs + "'";
                        }
                        else
                        {
                            strQuery += ",'" + roomIDs + "'";
                        }
                    }
                    else
                        strQuery += ", NULL";
                }

                if (!string.IsNullOrEmpty(compIDs))
                    strQuery += ",'" + compIDs + "'";
                else
                    strQuery += ", NULL";
                if (SpName.ToLower().Equals("RPT_SerialNumberList".ToLower()))
                    strQuery += ",NULL , NULL, NULL, NULL";
                else
                    strQuery += ",NULL , NULL, NULL, NULL, NULL";

                if (UserID.GetValueOrDefault(0) > 0)
                    strQuery += "," + UserID;
                else if (SpName.ToLower() != ("RPT_GetToolAuditTrail_Data".ToLower()) && SpName.ToLower() != ("RPT_GetToolAuditTrail_Trans".ToLower()))
                    strQuery += ", NULL";

                if (OrderStatus != null && OrderStatus.Length > 0)
                {
                    string Ordstatus = "";
                    for (int i = 0; i < OrderStatus.Length; i++)
                    {
                        if (Ordstatus.Length > 0)
                            Ordstatus += ",";

                        Ordstatus += OrderStatus[i];
                    }
                    if (SpName == "RPT_GetRequisitions" || SpName == "RPT_GetRequisitionWithLineItems")
                    {
                        strQuery += ",@RequisitionStatus='" + Ordstatus + "'";
                    }
                    else if (SpName == "RPT_GetQuote" || SpName == "RPT_Quote")
                    {
                        strQuery += ",@QuoteStatus='" + Ordstatus + "'";
                    }
                    else
                    {
                        strQuery += ",@OrderStatus='" + Ordstatus + "'";
                    }
                }

                if (WOStatus != null && WOStatus.Length > 0)
                {
                    string _WOstatus = "";
                    for (int i = 0; i < WOStatus.Length; i++)
                    {
                        if (_WOstatus.Length > 0)
                            _WOstatus += ",";

                        _WOstatus += WOStatus[i];
                    }
                    strQuery += ",@WOStatus='" + _WOstatus + "'";
                }

                if (arrTransferStatus != null && arrTransferStatus.Length > 0)
                {
                    string Trfstatus = "";
                    for (int i = 0; i < arrTransferStatus.Length; i++)
                    {
                        if (Trfstatus.Length > 0)
                            Trfstatus += ",";

                        Trfstatus += arrTransferStatus[i];
                    }
                    strQuery += ",@TransferStatus='" + Trfstatus + "'";
                }

                if (arrTransferRequestTypes != null && arrTransferRequestTypes.Length > 0)
                {
                    string TrfReqType = "";
                    for (int i = 0; i < arrTransferRequestTypes.Length; i++)
                    {
                        if (TrfReqType.Length > 0)
                            TrfReqType += ",";

                        TrfReqType += arrTransferRequestTypes[i];
                    }
                    strQuery += ",@RequestType='" + TrfReqType + "'";
                }



                if (!string.IsNullOrEmpty(QOHFilters))
                {
                    if (SpName == "RPT_GetTools" || SpName == "RPT_GetWrittenOffTools")
                        strQuery += ",@OnlyAvailable='" + QOHFilters + "'";
                    else
                        strQuery += ",@QOHFilter='" + QOHFilters + "'";
                }

                if (!string.IsNullOrEmpty(OnlyExirationItems))
                    strQuery += ",@OnlyExirationItems='" + OnlyExirationItems + "'";

                if (!string.IsNullOrEmpty(QuantityType))
                    strQuery += ",@QuantityType='" + QuantityType + "'";



                if (!string.IsNullOrEmpty(OnlyExpirationItems))
                    strQuery += ",@OnlyExirationItems='" + OnlyExpirationItems + "'";

                if (!string.IsNullOrEmpty(DaysToApproveOrder))
                    strQuery += ",@DaysToApproveOrder='" + Convert.ToInt32(DaysToApproveOrder) + "'";

                if (!string.IsNullOrEmpty(DaysUntilItemExpires))
                    strQuery += ",@DaysUntilItemExpires='" + Convert.ToInt32(DaysUntilItemExpires) + "'";
                if (!string.IsNullOrEmpty(ProjectExpirationDate) && Convert.ToDateTime(ProjectExpirationDate) > DateTime.MinValue)
                    strQuery += ",@ProjectedExpirationDate='" + Convert.ToDateTime(ProjectExpirationDate).ToString("yyyy-MM-dd HH:mm:ss") + "'";

                if (!string.IsNullOrEmpty(CountAppliedFilter))
                    strQuery += ",@AppliedFilter='" + CountAppliedFilter + "'";

                if (!string.IsNullOrEmpty(reportModuleName) && (reportModuleName == "Consume_Pull" || reportModuleName == "Not Consume_Pull"))
                {
                    if (!((SpName ?? string.Empty).ToLower().Contains("rpt_cumulative_pullsummary")))
                    {
                        strQuery += ",@FilterDateOn='" + FilterDateOn + "'";
                    }
                }

                if (SpName.ToLower().Equals("RPT_InStockByBin".ToLower())
                    || SpName.ToLower().Equals("RPT_InStockByBin_Margin".ToLower())
                    || SpName.ToLower().Equals("RPT_InStockByBinWithQty".ToLower())
                    || SpName.ToLower().Equals("RPT_InStockByActivity".ToLower())
                    || SpName.ToLower().Equals("RPT_OutStockItem".ToLower())
                    || SpName.ToLower().Equals("RPT_PreciseDemandPlanning".ToLower())
                    || SpName.ToLower().Equals("RPT_PreciseDemandPlanningByItem".ToLower())
                    || SpName.ToLower().Equals("RPT_InventoryStockOut".ToLower())
                    || SpName.ToLower().Equals("RPT_InStockForSerialLotDateCode".ToLower())
                    || SpName.ToLower().Equals("RPT_GetPullSummary".ToLower())
                    || SpName.ToLower().Equals("RPT_GetPullSummary_RangeData".ToLower())
                    || SpName.ToLower().Equals("RPT_GetPullSummary_ForSchedule".ToLower())
                    || SpName.ToLower().Equals("RPT_GetPullSummaryByQuarter_RangeData".ToLower())
                    || SpName.ToLower().Equals("RPT_GetPullSummaryByQuarter".ToLower())
                    || SpName.ToLower().Equals("RPT_GetPullSummaryByQuarter_ForSchedule".ToLower())
                    )
                {
                    if (!string.IsNullOrEmpty(itemIsActive))
                        strQuery += ",@ItemIsActive='" + itemIsActive + "'";
                    else
                        strQuery += ",@ItemIsActive=NULL";
                }
                if (SpName.ToLower().Equals("RPT_OutStockItem".ToLower())
                    || SpName.ToLower().Equals("RPT_InventoryStockOut".ToLower()))
                {
                    if (!string.IsNullOrEmpty(DateCreatedEarlier))
                        strQuery += ",@DateCreatedEarlier='" + DateCreatedEarlier + "'";
                    else
                        strQuery += ",@DateCreatedEarlier=NULL";

                    if (!string.IsNullOrEmpty(DateActiveLater))
                        strQuery += ",@DateActiveLater='" + DateActiveLater + "'";
                    else
                        strQuery += ",@DateActiveLater=NULL";
                }

                if (SpName.ToLower().Equals("RPT_GetPullSummary".ToLower()) || SpName.ToLower().Equals("RPT_GetPullSummaryByQuarter".ToLower()) || SpName.ToLower().Equals("RPT_GetPullSummaryByQuarter_ForSchedule".ToLower()))
                {
                    if (!string.IsNullOrEmpty(_usageType))
                        strQuery += ",@UsageType='" + _usageType + "'";
                }

                if (SpName.ToLower().Equals("RPT_GetPullSummaryByQuarter".ToLower()) || SpName.ToLower().Equals("RPT_GetPullSummaryByQuarter_ForSchedule".ToLower()))
                {
                    if (IsAllowedZeroPullUsage)
                        strQuery += ",@IsAllowedZeroPullUsage=1";
                    else
                        strQuery += ",@IsAllowedZeroPullUsage=0";
                }

                if (SpName.ToLower().Equals("RPT_PreciseDemandPlanning".ToLower()))
                {
                    if (!string.IsNullOrEmpty(AUDayOfUsageToSample))
                        strQuery += ",@AUDayOfUsageToSample='" + AUDayOfUsageToSample + "'";
                    else
                        strQuery += ",@AUDayOfUsageToSample=NULL";

                    if (!string.IsNullOrEmpty(AUMeasureMethod) && AUMeasureMethod != "0")
                        strQuery += ",@AUMeasureMethod='" + AUMeasureMethod + "'";
                    else
                        strQuery += ",@AUMeasureMethod=NULL";

                    if (!string.IsNullOrEmpty(MinMaxDayOfAverage))
                        strQuery += ",@MinMaxDayOfAverage='" + MinMaxDayOfAverage + "'";
                    else
                        strQuery += ",@MinMaxDayOfAverage=NULL";

                    if (!string.IsNullOrEmpty(MinMaxMinNumberOfTimesMax))
                        strQuery += ",@MinMaxMinNumberOfTimesMax='" + MinMaxMinNumberOfTimesMax + "'";
                    else
                        strQuery += ",@MinMaxMinNumberOfTimesMax=NULL";

                    strQuery += ",@DecimalPointFromConfig='" + DecimalPointFromConfig + "'";
                }
                if (SpName.ToLower().Equals("RPT_PreciseDemandPlanningByItem".ToLower()))
                {
                    if (!string.IsNullOrEmpty(AUDayOfUsageToSample))
                        strQuery += ",@MinMaxDayOfUsageToSample='" + AUDayOfUsageToSample + "'";
                    else
                        strQuery += ",@MinMaxDayOfUsageToSample=NULL";

                    if (!string.IsNullOrEmpty(AUMeasureMethod) && AUMeasureMethod != "0")
                        strQuery += ",@MinMaxMeasureMethod='" + AUMeasureMethod + "'";
                    else
                        strQuery += ",@MinMaxMeasureMethod=NULL";

                    if (!string.IsNullOrEmpty(MinMaxDayOfAverage))
                        strQuery += ",@MinMaxDayOfAverage='" + MinMaxDayOfAverage + "'";
                    else
                        strQuery += ",@MinMaxDayOfAverage=NULL";

                    if (!string.IsNullOrEmpty(MinMaxMinNumberOfTimesMax))
                        strQuery += ",@MinMaxMinNumberOfTimesMax='" + MinMaxMinNumberOfTimesMax + "'";
                    else
                        strQuery += ",@MinMaxMinNumberOfTimesMax=NULL";

                    strQuery += ",@DecimalPointFromConfig='" + DecimalPointFromConfig + "'";
                }
                if (SpName.ToLower().Equals("RPT_GetCartItems".ToLower()))
                {
                    strQuery += ",@CartType='" + SelectedCartType + "'";
                }

                if (SpName.ToLower().Equals("RPT_GetReturnItemCandidates".ToLower())
                    )
                {
                    if (!string.IsNullOrEmpty(Days))
                        strQuery += ",@Days='" + Days + "'";
                    else
                        strQuery += ",@Days=NULL";
                }

                if (SpName.ToLower().Equals("RPT_InStockForSerialLotDateCode".ToLower())
                    )
                {
                    if (!string.IsNullOrEmpty(Days))
                        strQuery += ",@ItemTypeFilter='" + ItemTypeFilter + "'";
                    else
                        strQuery += ",@ItemTypeFilter=NULL";
                }

                //IEnumerable<PullMasterViewDTO> obj = (from u in context.Database.SqlQuery<PullMasterViewDTO>(@"SELECT A.*,");
                //IEnumerable<PullMasterViewDTO> ienPullView = (from u in context.PullMasters
                //                                              join i in context.ItemMasters on u.ItemGUID equals i.GUID into PI_join
                //                                              from u_i in PI_join.DefaultIfEmpty()
                //                                              where CompanyIDs.Contains(u.CompanyID ?? 0) && RoomIDs.Contains(u.Room ?? 0)
                context.Database.CommandTimeout = 3600;

                IEnumerable<T> ienPullView = context.Database.SqlQuery<T>(strQuery);

                return ienPullView.ToList<T>();

            }


        }

        public IEnumerable<T> GetScheduleFilterDataForTotalPulledReport<T>(string SpName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, string RangeFieldName
            , Int64? UserID, string UserSupplierIds, string QuantityType = "", int FilterDateOn = 0, bool _isRunWithReportConnection = false) where T : class
        {
            string connectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                connectionString = base.DataBaseEntityConnectionStringForReport;
            }

            string startdate = null, enddate = null, roomids = null, companyids = null;

            if (CompanyIDs != null && CompanyIDs.Count() > 0)
            {
                companyids = string.Join(",", CompanyIDs);
            }
            if (RoomIDs != null && RoomIDs.Count() > 0)
            {
                roomids = string.Join(",", RoomIDs);
            }
            if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) != DateTime.MinValue)
            {
                startdate = StartDate;
            }
            if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) != DateTime.MinValue)
            {
                enddate = EndDate;
            }
            string userId = string.Empty;

            if (UserID.HasValue)
            {
                userId = Convert.ToString(UserID);
            }

            using (var context = new eTurnsEntities(connectionString))
            {
                var params1 = new SqlParameter[]
                {
                    new SqlParameter("@RangeFieldName", RangeFieldName ?? (object)DBNull.Value ),
                    new SqlParameter("@StartDate", startdate ?? (object)DBNull.Value),
                    new SqlParameter("@EndDate", enddate ?? (object)DBNull.Value),
                    new SqlParameter("@RoomIDs", roomids ?? (object)DBNull.Value),
                    new SqlParameter("@CompanyIDs", companyids ?? (object)DBNull.Value),
                    new SqlParameter("@UserID", userId ?? (object)DBNull.Value),
                    new SqlParameter("@SupplierIDs", UserSupplierIds ?? (object)DBNull.Value),
                    new SqlParameter("@QuantityType", (QuantityType ?? (object)DBNull.Value)),
                    new SqlParameter("@FilterDateOn", FilterDateOn)
               };

                IEnumerable<T> ienPullView = context.Database.SqlQuery<T>("exec " + SpName + " @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID,@SupplierIDs,@QuantityType,@FilterDateOn", params1);
                return ienPullView.ToList<T>();
            }

        }

        public IEnumerable<T> GetDataForReportFilterList<T>(string SQLQuery) where T : class
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<T> lstData = context.Database.SqlQuery<T>(SQLQuery);
                return lstData.ToList();
            }


        }

        public long? GetOrInsertSupplierAccount(string Name, long RoomID, long CompanyID, long UserID)
        {
            long? SupplierID = null;
            long? SupplierAcID = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Room objRoom = context.Rooms.Where(m => m.ID == RoomID).FirstOrDefault();
                if (objRoom != null)
                {
                    SupplierID = objRoom.DefaultSupplierID;
                }
                if ((SupplierID ?? 0) > 0)
                {
                    SupplierAccountDetail objSupplierAccountDetail = context.SupplierAccountDetails.Where(f => f.Room == RoomID && f.CompanyID == CompanyID && f.SupplierID == SupplierID && f.AccountName == Name && (f.IsDeleted) == false).FirstOrDefault();
                    if (objSupplierAccountDetail != null)
                    {
                        SupplierAcID = objSupplierAccountDetail.ID;
                    }
                    else
                    {
                        objSupplierAccountDetail = new SupplierAccountDetail();
                        objSupplierAccountDetail.AccountName = Name;
                        objSupplierAccountDetail.AccountNo = string.Empty;
                        objSupplierAccountDetail.CompanyID = CompanyID;
                        objSupplierAccountDetail.Created = DateTimeUtility.DateTimeNow;
                        objSupplierAccountDetail.CreatedBy = UserID;
                        objSupplierAccountDetail.GUID = Guid.NewGuid();
                        objSupplierAccountDetail.ID = 0;
                        objSupplierAccountDetail.IsArchived = false;
                        objSupplierAccountDetail.IsDefault = false;
                        objSupplierAccountDetail.IsDeleted = false;
                        objSupplierAccountDetail.LastUpdatedBy = UserID;
                        objSupplierAccountDetail.Room = RoomID;
                        objSupplierAccountDetail.SupplierID = SupplierID;
                        objSupplierAccountDetail.Updated = DateTimeUtility.DateTimeNow;
                        context.SupplierAccountDetails.Add(objSupplierAccountDetail);
                        context.SaveChanges();
                        SupplierAcID = objSupplierAccountDetail.ID;
                    }
                }


            }
            return SupplierAcID;
        }

        public ProjectMasterDTO GetOrInsertProject(string ProjectName, long RoomID, long CompanyID, long UserID, string EditedFrom)
        {
            ProjectMasterDTO objProjectMasterDTO = new ProjectMasterDTO();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objProjectMasterDTO = context.ProjectMasters.Where(t => (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false && t.Room == RoomID && t.CompanyID == CompanyID && t.ProjectSpendName == ProjectName).Select(u => new ProjectMasterDTO()
                {
                    ID = u.ID,
                    ProjectSpendName = u.ProjectSpendName,
                    Description = u.Description,
                    DollarLimitAmount = u.DollarLimitAmount,
                    DollarUsedAmount = u.DollarUsedAmount,
                    TrackAllUsageAgainstThis = u.TrackAllUsageAgainstThis ?? false,
                    IsClosed = u.IsClosed ?? false,
                    CompanyID = u.CompanyID,
                    Created = u.Created,
                    Updated = u.Updated,
                    CreatedBy = u.CreatedBy,
                    LastUpdatedBy = u.LastUpdatedBy,
                    Room = u.Room,
                    IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                    IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                    GUID = u.GUID,
                    UDF1 = u.UDF1,
                    UDF2 = u.UDF2,
                    UDF3 = u.UDF3,
                    UDF4 = u.UDF4,
                    UDF5 = u.UDF5,
                    ReceivedOn = u.ReceivedOn,
                    ReceivedOnWeb = u.ReceivedOnWeb,
                    AddedFrom = u.AddedFrom,
                    EditedFrom = u.EditedFrom,
                }).FirstOrDefault();

                if (objProjectMasterDTO != null)
                {
                    return objProjectMasterDTO;
                }
                else
                {
                    objProjectMasterDTO = new ProjectMasterDTO() { Action = "insert", AddedFrom = string.IsNullOrWhiteSpace(EditedFrom) ? "web" : EditedFrom, CompanyID = CompanyID, Created = DateTimeUtility.DateTimeNow, CreatedBy = UserID, Description = string.Empty, EditedFrom = string.IsNullOrWhiteSpace(EditedFrom) ? "web" : EditedFrom, GUID = Guid.NewGuid(), ID = 0, IsArchived = false, IsClosed = false, IsDeleted = false, LastUpdatedBy = UserID, ProjectSpendName = ProjectName, ReceivedOn = DateTimeUtility.DateTimeNow, ReceivedOnWeb = DateTimeUtility.DateTimeNow, Room = RoomID, Updated = DateTimeUtility.DateTimeNow, WhatWhereAction = "from edi svc", TrackAllUsageAgainstThis = false, DollarLimitAmount = 0, DollarUsedAmount = 0 };
                    ProjectMasterDAL objProjectMasterDAL = new ProjectMasterDAL(base.DataBaseName);
                    objProjectMasterDTO.ID = objProjectMasterDAL.Insert(objProjectMasterDTO);
                    return objProjectMasterDTO;

                }
            }
        }

        public UserSyncHistoryDTO InsertUserSyncHist(UserSyncHistoryDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@SyncByUserID", objDTO.SyncByUserID), new SqlParameter("@SyncStep", (objDTO.SyncStep ?? string.Empty)), new SqlParameter("@SyncTime", objDTO.SyncTime), new SqlParameter("@CompanyID", objDTO.CompanyID), new SqlParameter("@RoomID", objDTO.RoomID), new SqlParameter("@BuildNo", (objDTO.BuildNo ?? string.Empty)), new SqlParameter("@DeviceName", (objDTO.DeviceName ?? string.Empty)), new SqlParameter("@SyncTransactionID", objDTO.SyncTransactionID), new SqlParameter("@ErrorDescription ", (objDTO.ErrorDescription ?? string.Empty)) };
                context.Database.ExecuteSqlCommand("exec InsertUserSyncHistory @SyncByUserID,@SyncStep,@SyncTime,@CompanyID,@RoomID,@BuildNo,@DeviceName,@SyncTransactionID,@ErrorDescription", params1);
            }
            return objDTO;
        }
        public List<UserSyncHistoryDTO> GetUserSyncHistoryForaDay(DateTime FromDate, DateTime ToDate)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {




                //string Qry = @"Select USH.*,UM.UserName,CM.Name as CompanyName from UserSyncHistory as USH
                //                inner join CompanyMaster as CM on USH.CompanyID = CM.ID
                //                inner join UserMaster as UM on USH.SyncByUserID = UM.ID
                //                Where CONVERT(date,SyncTime) between '" + FromDate.ToString("yyyy-MM-dd") + "' and '" + ToDate.ToString("yyyy-MM-dd") + "' Order by SyncTransactionID ASC";
                string Qry = " EXEC [UserSyncHistoryToday]";


                return (from u in context.Database.SqlQuery<UserSyncHistoryDTO>(Qry)
                        select new UserSyncHistoryDTO
                        {
                            BuildNo = u.BuildNo,
                            CompanyID = u.CompanyID,
                            DeviceName = u.DeviceName,
                            ErrorDescription = u.ErrorDescription,
                            ID = u.ID,
                            RoomID = u.RoomID,
                            SyncByUserID = u.SyncByUserID,
                            SyncStep = u.SyncStep,
                            SyncTime = u.SyncTime,
                            SyncTransactionID = u.SyncTransactionID,
                            UserName = u.UserName,
                            CompanyName = u.CompanyName
                        }).AsParallel().ToList();
            }
        }

        public List<UserSyncHistoryDTO> GetFailedUserSyncHistoryForaDay(int Hours)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string Qry = " EXEC [FailedUserSyncs] " + Hours;

                return (from u in context.Database.SqlQuery<UserSyncHistoryDTO>(Qry)
                        select new UserSyncHistoryDTO
                        {
                            BuildNo = u.BuildNo,
                            CompanyID = u.CompanyID,
                            DeviceName = u.DeviceName,
                            ErrorDescription = u.ErrorDescription,
                            ID = u.ID,
                            RoomID = u.RoomID,
                            SyncByUserID = u.SyncByUserID,
                            SyncStep = u.SyncStep,
                            SyncTime = u.SyncTime,
                            SyncTransactionID = u.SyncTransactionID,
                            UserName = u.UserName,
                            CompanyName = u.CompanyName
                        }).AsParallel().ToList();
            }
        }

        public void InsertIntoSyncProcessed(UserSyncHistoryDTO objUserSyncHistoryDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string Qry = "EXEC [InsertIntoSyncProcessed] '" + objUserSyncHistoryDTO.SyncTransactionID + "'";
                context.Database.ExecuteSqlCommand(Qry);
            }
        }
        public bool CheckMismatchesNow(UserSyncHistoryDTO objUserSyncHistoryDTO)
        {
            bool HasMisMatches = false;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //string Qry = "EXEC RPT_CheckMismatches @CompanyIDs='" + objUserSyncHistoryDTO.CompanyID + "'";
                SqlConnection EturnsConnection = new SqlConnection(base.DataBaseConnectionString);
                //SqlHelper.ExecuteDataset(EturnsConnection, CommandType.StoredProcedure, "RPT_CheckMismatches", objUserSyncHistoryDTO.CompanyID, DBNull.Value, DBNull.Value);
                //DataSet ds = SqlHelper.ExecuteDataset(EturnsConnection, "USP_ModulewiseDelete", Ids, ModuleName, IsGUID, UserID);
                DataSet ds = SqlHelper.ExecuteDataset(EturnsConnection, "RPT_CheckMismatches", objUserSyncHistoryDTO.CompanyID.ToString(), DBNull.Value, DBNull.Value);
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        HasMisMatches = true;
                    }
                }
                //context.Database.ExecuteSqlCommand(Qry);
            }
            return HasMisMatches;
        }
        public UserSyncHistoryDTO UpdateUserLastSyncDetails(UserSyncHistoryDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@SyncByUserID", objDTO.SyncByUserID), new SqlParameter("@RoomIDs", (objDTO.RoomIDs ?? string.Empty)), new SqlParameter("@PDABuildVersion", (objDTO.BuildNo ?? string.Empty)) };
                context.Database.ExecuteSqlCommand("EXEC UpdateUserLastSyncDetails @SyncByUserID,@RoomIDs,@PDABuildVersion", params1);
            }
            return objDTO;
        }
        public string GetDecryptValue(string UDFOption)
        {
            try
            {
                //string EncryptionKey = "eturns@@123";
                //byte[] cipherBytes = Convert.FromBase64String(UDFOption);
                //using (Aes encryptor = Aes.Create())
                //{
                //    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                //    encryptor.Key = pdb.GetBytes(32);
                //    encryptor.IV = pdb.GetBytes(16);
                //    using (MemoryStream ms = new MemoryStream())
                //    {
                //        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                //        {
                //            cs.Write(cipherBytes, 0, cipherBytes.Length);
                //            cs.Close();
                //        }
                //        UDFOption = Encoding.Unicode.GetString(ms.ToArray());
                //    }
                //}
                //return UDFOption;
                if (!string.IsNullOrWhiteSpace(UDFOption))
                {
                    RC2CryptoServiceProvider rc2CSP = new RC2CryptoServiceProvider();

                    byte[] key = new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76, 0x35, 0x84, 0x47 };
                    byte[] IV = new byte[] { 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64 };

                    ICryptoTransform decryptor = rc2CSP.CreateDecryptor(key, IV);

                    byte[] encrypted = Convert.FromBase64String(UDFOption);
                    MemoryStream msDecrypt = new MemoryStream(encrypted);
                    CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                    StringBuilder roundtrip = new StringBuilder();

                    int b = 0;

                    do
                    {
                        b = csDecrypt.ReadByte();

                        if (b != -1)
                        {
                            roundtrip.Append((char)b);
                        }

                    } while (b != -1);


                    return roundtrip.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch
            {
                return UDFOption;
            }
        }
        public string GetEncryptValue(string UDFOption)
        {
            try
            {
                //string EncryptionKey = "eturns@@123";
                //byte[] clearBytes = Encoding.Unicode.GetBytes(UDFOption);
                //using (Aes encryptor = Aes.Create())
                //{
                //    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                //    encryptor.Key = pdb.GetBytes(32);
                //    encryptor.IV = pdb.GetBytes(16);
                //    using (MemoryStream ms = new MemoryStream())
                //    {
                //        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                //        {
                //            cs.Write(clearBytes, 0, clearBytes.Length);
                //            cs.Close();
                //        }
                //        UDFOption = Convert.ToBase64String(ms.ToArray());
                //    }
                //}
                //return UDFOption;
                RC2CryptoServiceProvider rc2CSP = new RC2CryptoServiceProvider();

                byte[] key = new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76, 0x35, 0x84, 0x47 };
                byte[] IV = new byte[] { 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64 };

                ICryptoTransform encryptor = rc2CSP.CreateEncryptor(key, IV);

                MemoryStream msEncrypt = new MemoryStream();
                CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);


                byte[] toEncrypt = Encoding.ASCII.GetBytes(UDFOption);

                csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
                csEncrypt.FlushFinalBlock();

                byte[] encrypted = msEncrypt.ToArray();
                UDFOption = Convert.ToBase64String(encrypted);

                return UDFOption;
            }
            catch
            {
                return UDFOption;
            }
        }
        public void InsertItemWithDefaultValue(string ItemNumber, Int64 RoomId, Int64 CompanyID, string UniqueNumber, Int64 UserId)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemNumber", ItemNumber), new SqlParameter("@RoomIDs", RoomId), new SqlParameter("@CompanyID", (CompanyID)), new SqlParameter("@ItemUniqueNumber", (UniqueNumber)), new SqlParameter("@UserId", (UserId)) };
                context.Database.ExecuteSqlCommand("EXEC InsertItemDefaultValue @ItemNumber,@RoomIDs,@CompanyID,@ItemUniqueNumber,@UserId", params1);
            }

        }

        public static UserMasterDTO GeteTurnsSystemUser(string EnterpriseDbName)
        {
            UserMasterDTO objeturnsUserMasterDTO = new UserMasterDTO();
            eTurns.DAL.UserMasterDAL objUserMasterDAL = new eTurns.DAL.UserMasterDAL(EnterpriseDbName);
            objeturnsUserMasterDTO = objUserMasterDAL.GetSystemUser(5);
            return objeturnsUserMasterDTO;
        }

        public static UserMasterDTO GetEnterpriseSystemUser(string EnterpriseDbName)
        {
            UserMasterDTO objeturnsUserMasterDTO = new UserMasterDTO();
            eTurns.DAL.UserMasterDAL objUserMasterDAL = new eTurns.DAL.UserMasterDAL(EnterpriseDbName);
            objeturnsUserMasterDTO = objUserMasterDAL.GetSystemUser(5);
            return objeturnsUserMasterDTO;
        }

        public SupplierMasterDTO GetSupplierRecord(string SupplierName, long RoomID, long CompanyID, string EnterpriseDbName)
        {
            SupplierMasterDAL objDAL = new SupplierMasterDAL(EnterpriseDbName);
            SupplierMasterDTO obj = null;
            obj = objDAL.GetSupplierByNamePlain(RoomID, CompanyID, false, SupplierName);
            return obj;
        }


        public void InsertIntoActionMethod(string insertScript)
        {

            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{
            //    context.Database.ExecuteSqlCommand(insertScript);
            //}

        }


        public void SaveNotificationError(ReportSchedulerErrorDTO objReportSchedulerError)
        {
            //string MasterDbConnectionString = ConfigurationManager.ConnectionStrings["eTurnsMasterDbConnection"].ConnectionString.ToString();
            string MasterDbConnectionString = DbConnectionHelper.GeteTurnsMasterSQLConnectionString(DbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsMasterConnection = new SqlConnection(MasterDbConnectionString);
            DataSet dsCart = new DataSet();
            int retval = SqlHelper.ExecuteNonQuery(EturnsMasterConnection, "INSERTReportSchedulerError", objReportSchedulerError.NotificationID, objReportSchedulerError.Exception, objReportSchedulerError.ScheduleFor, objReportSchedulerError.RoomID, objReportSchedulerError.CompanyID, objReportSchedulerError.EnterpriseID, objReportSchedulerError.UserID);
        }

        public string CheckDuplicateToolSerialNumbers(string SrNumber, int ID, Int64 RoomID, Int64 CompanyID, Guid ToolGUID)
        {
            string msg = "";
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var qry = (from em in context.ToolAssetQuantityDetails
                           where em.RoomID == RoomID && em.CompanyID == CompanyID
                           && em.IsDeleted == false && em.IsArchived == false
                           && em.ToolGUID == ToolGUID && em.ID != ID
                           // && ((em.Quantity ?? 0)) > 0
                           select em);
                if (qry.Any(x => x.SerialNumber.ToLower().Trim() == SrNumber.ToLower().Trim()))
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


        public List<OLEDBConnectionInfo> GetAllConnectionparams()
        {
            List<OLEDBConnectionInfo> lstOLEDBConnectionsdecrypted = CacheHelper<List<OLEDBConnectionInfo>>.GetCacheItem("Connectionparams");
            if (lstOLEDBConnectionsdecrypted == null || lstOLEDBConnectionsdecrypted.Count == 0)
            {
                string EnKey = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["Enkey"]);
                SecHelper objSecHelper = new SecHelper();
                List<OLEDBConnectionInfo> lstOLEDBConnections = getCons();
                if (lstOLEDBConnections != null && lstOLEDBConnections.Count > 0)
                {
                    try
                    {
                        lstOLEDBConnectionsdecrypted = (from t in lstOLEDBConnections
                                                        select new OLEDBConnectionInfo
                                                        {
                                                            ID = t.ID,
                                                            ConectionType = t.ConectionType,
                                                            APP = objSecHelper.DecryptData(t.APP, EnKey),
                                                            ApplicationIntent = objSecHelper.DecryptData(t.ApplicationIntent, EnKey),
                                                            AppDatabase = objSecHelper.DecryptData(t.AppDatabase, EnKey),
                                                            MarsConn = objSecHelper.DecryptData(t.MarsConn, EnKey),
                                                            PacketSize = objSecHelper.DecryptData(t.PacketSize, EnKey),
                                                            PWD = objSecHelper.DecryptData(t.PWD, EnKey),
                                                            Server = objSecHelper.DecryptData(t.Server, EnKey),
                                                            Timeout = objSecHelper.DecryptData(t.Timeout, EnKey),
                                                            Trusted_Connection = objSecHelper.DecryptData(t.Trusted_Connection, EnKey),
                                                            UID = objSecHelper.DecryptData(t.UID, EnKey),
                                                            FailoverPartner = objSecHelper.DecryptData(t.FailoverPartner, EnKey),
                                                            PersistSensitive = objSecHelper.DecryptData(t.PersistSensitive, EnKey),
                                                            MultiSubnetFailover = objSecHelper.DecryptData(t.MultiSubnetFailover, EnKey),
                                                            Created = t.Created,
                                                            Updated = t.Updated,
                                                            CreatedBy = t.CreatedBy,
                                                            UpdatedBy = t.UpdatedBy,
                                                            GUID = t.GUID

                                                        }).ToList();
                        CacheHelper<List<OLEDBConnectionInfo>>.InvalidateCache();
                        CacheHelper<List<OLEDBConnectionInfo>>.AppendToCacheItem("Connectionparams", lstOLEDBConnectionsdecrypted);
                        return lstOLEDBConnectionsdecrypted;
                    }
                    catch
                    {
                        return lstOLEDBConnections;
                    }
                }
                return lstOLEDBConnections;
            }
            else
            {
                return lstOLEDBConnectionsdecrypted;
            }
        }
        public void SaveConnectionParams(List<OLEDBConnectionInfo> lstOLEDBConnections)
        {
            SecHelper objSecHelper = new SecHelper();
            List<OLEDBConnectionInfo> lstOLEDBConnectionsEncrypted = new List<OLEDBConnectionInfo>();
            if (lstOLEDBConnections != null && lstOLEDBConnections.Count > 0)
            {
                string EnKey = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["Enkey"]);
                OLEDBConnectionInfo objOLEDBConnectionInfo;
                foreach (var item in lstOLEDBConnections)
                {
                    objOLEDBConnectionInfo = new OLEDBConnectionInfo();
                    objOLEDBConnectionInfo.ID = item.ID;
                    objOLEDBConnectionInfo.ConectionType = objSecHelper.EncryptData((item.ConectionType ?? string.Empty), EnKey);
                    objOLEDBConnectionInfo.APP = objSecHelper.EncryptData(item.APP ?? string.Empty, EnKey);
                    objOLEDBConnectionInfo.ApplicationIntent = objSecHelper.EncryptData(item.ApplicationIntent ?? string.Empty, EnKey);
                    objOLEDBConnectionInfo.AppDatabase = objSecHelper.EncryptData(item.AppDatabase ?? string.Empty, EnKey);
                    objOLEDBConnectionInfo.MarsConn = objSecHelper.EncryptData(item.MarsConn ?? string.Empty, EnKey);
                    objOLEDBConnectionInfo.PacketSize = objSecHelper.EncryptData(item.PacketSize ?? string.Empty, EnKey);
                    objOLEDBConnectionInfo.PWD = objSecHelper.EncryptData(item.PWD ?? string.Empty, EnKey);
                    objOLEDBConnectionInfo.Server = objSecHelper.EncryptData(item.Server ?? string.Empty, EnKey);
                    objOLEDBConnectionInfo.Timeout = objSecHelper.EncryptData(item.Timeout ?? string.Empty, EnKey);
                    objOLEDBConnectionInfo.Trusted_Connection = objSecHelper.EncryptData(item.Trusted_Connection ?? string.Empty, EnKey);
                    objOLEDBConnectionInfo.UID = objSecHelper.EncryptData(item.UID ?? string.Empty, EnKey);
                    objOLEDBConnectionInfo.FailoverPartner = objSecHelper.EncryptData(item.FailoverPartner ?? string.Empty, EnKey);
                    objOLEDBConnectionInfo.PersistSensitive = objSecHelper.EncryptData(item.PersistSensitive ?? string.Empty, EnKey);
                    objOLEDBConnectionInfo.MultiSubnetFailover = objSecHelper.EncryptData(item.MultiSubnetFailover ?? string.Empty, EnKey);
                    objOLEDBConnectionInfo.UpdatedBy = item.UpdatedBy;
                    objOLEDBConnectionInfo.Updated = DateTime.UtcNow;
                    lstOLEDBConnectionsEncrypted.Add(objOLEDBConnectionInfo);
                }
                lstOLEDBConnections = SaveCons(lstOLEDBConnectionsEncrypted);
                List<OLEDBConnectionInfo> lstOLEDBConnectionsdecrypted = (from t in lstOLEDBConnections
                                                                          select new OLEDBConnectionInfo
                                                                          {
                                                                              ID = t.ID,
                                                                              ConectionType = t.ConectionType,
                                                                              APP = objSecHelper.DecryptData(t.APP, EnKey),
                                                                              ApplicationIntent = objSecHelper.DecryptData(t.ApplicationIntent, EnKey),
                                                                              AppDatabase = objSecHelper.DecryptData(t.AppDatabase, EnKey),
                                                                              MarsConn = objSecHelper.DecryptData(t.MarsConn, EnKey),
                                                                              PacketSize = objSecHelper.DecryptData(t.PacketSize, EnKey),
                                                                              PWD = objSecHelper.DecryptData(t.PWD, EnKey),
                                                                              Server = objSecHelper.DecryptData(t.Server, EnKey),
                                                                              Timeout = objSecHelper.DecryptData(t.Timeout, EnKey),
                                                                              Trusted_Connection = objSecHelper.DecryptData(t.Trusted_Connection, EnKey),
                                                                              UID = objSecHelper.DecryptData(t.UID, EnKey),
                                                                              FailoverPartner = objSecHelper.DecryptData(t.FailoverPartner, EnKey),
                                                                              PersistSensitive = objSecHelper.DecryptData(t.PersistSensitive, EnKey),
                                                                              MultiSubnetFailover = objSecHelper.DecryptData(t.MultiSubnetFailover, EnKey),
                                                                              Created = t.Created,
                                                                              Updated = t.Updated,
                                                                              CreatedBy = t.CreatedBy,
                                                                              UpdatedBy = t.UpdatedBy,
                                                                              GUID = t.GUID

                                                                          }).ToList();

                CacheHelper<List<OLEDBConnectionInfo>>.InvalidateCache();

                CacheHelper<List<OLEDBConnectionInfo>>.AppendToCacheItem("Connectionparams", lstOLEDBConnectionsdecrypted);

            }

        }


        public List<OLEDBConnectionInfo> getCons()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionStringeTurns))
            {
                return context.Database.SqlQuery<OLEDBConnectionInfo>("EXEC " + DbConnectionHelper.GetETurnsMasterDBName() + ".dbo.[getCons]").ToList();
            }
        }
        public List<OLEDBConnectionInfo> SaveCons(List<OLEDBConnectionInfo> lstUpdates)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (lstUpdates != null && lstUpdates.Count > 0)
                {
                    foreach (var item in lstUpdates)
                    {
                        var params1 = new SqlParameter[] { new SqlParameter("@ID", item.ID), new SqlParameter("@APP", item.APP ?? (object)DBNull.Value), new SqlParameter("@ApplicationIntent", item.ApplicationIntent ?? (object)DBNull.Value), new SqlParameter("@AppDatabase", item.AppDatabase ?? (object)DBNull.Value), new SqlParameter("@MarsConn", item.MarsConn ?? (object)DBNull.Value), new SqlParameter("@PacketSize", item.PacketSize ?? (object)DBNull.Value), new SqlParameter("@PWD", item.PWD ?? (object)DBNull.Value), new SqlParameter("@Server", item.Server ?? (object)DBNull.Value), new SqlParameter("@Timeout", item.Timeout ?? (object)DBNull.Value), new SqlParameter("@Trusted_Connection", item.Trusted_Connection ?? (object)DBNull.Value), new SqlParameter("@UID", item.UID ?? (object)DBNull.Value), new SqlParameter("@FailoverPartner", item.FailoverPartner ?? (object)DBNull.Value), new SqlParameter("@PersistSensitive", item.PersistSensitive ?? (object)DBNull.Value), new SqlParameter("@MultiSubnetFailover", item.MultiSubnetFailover ?? (object)DBNull.Value), new SqlParameter("@UserID", item.UpdatedBy) };
                        context.Database.ExecuteSqlCommand("exec " + DbConnectionHelper.GetETurnsMasterDBName() + ".dbo.[SaveCon] @ID,@APP,@ApplicationIntent,@AppDatabase,@MarsConn,@PacketSize,@PWD,@Server,@Timeout,@Trusted_Connection,@UID,@FailoverPartner,@PersistSensitive,@MultiSubnetFailover,@UserID", params1);
                    }
                }
            }
            return getCons();
        }


        #region WI-4691 - If a user deletes a room and there is only one room under the company, delete the company too

        public Int64 CheckCompanyRoomCount(Int64 CompanyID)
        {
            Int64 RoomCount = 0;
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var qry = (from em in context.Rooms
                           where em.CompanyID == CompanyID
                           && em.IsDeleted == false && em.IsArchived == false
                           select em.ID).ToList();
                RoomCount = qry.Count;
            }
            return RoomCount;
        }
        #endregion

        public List<OrderMasterItemsMain> InsertOrderImport(List<OrderMasterItemsMain> LstOrderItemsMain, Int64 RoomID, Int64 CompanyID, string EnterPriseDBName, Int64 UserID, string RoomDateFormat, CultureInfo RoomCulture, List<UserRoleModuleDetailsDTO> objUserRoleModuleDetailsDTO, long SessionUserId, long EnterPriceID, string TableName)
        {
            List<OrderMasterItemsMain> lstOrderMaster = LstOrderItemsMain;
            SupplierMasterDAL objSuppDAL = new SupplierMasterDAL(EnterPriseDBName);
            RoomDAL objRoomDAL = new RoomDAL(EnterPriseDBName);
            CommonDAL objCommonDAL = new CommonDAL(EnterPriseDBName);
            OrderMasterDAL obj = new OrderMasterDAL(EnterPriseDBName);
            AutoSequenceDAL objAutoSeqDAL = new AutoSequenceDAL(EnterPriseDBName);
            OrderMasterDAL objOrderDAL = new OrderMasterDAL(EnterPriseDBName);
            string strOrderSuccess = "Success";
            string strOrderFail = "Fail";
            ItemMasterDAL ItemDAL = new ItemMasterDAL(EnterPriseDBName);
            OrderDetailsDAL objDAL = new OrderDetailsDAL(EnterPriseDBName);
            string columnList = "ID,RoomName,PreventMaxOrderQty,DefaultSupplierID,IsAllowOrderDuplicate,AllowABIntegration";
            RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
            BinMasterDAL binDAL = new BinMasterDAL(EnterPriseDBName);
            List<BinMasterDTO> lstOfOrderLineItemBin = new List<BinMasterDTO>();
            string strBinNumbers = string.Join(",", LstOrderItemsMain.Where(x => (x.Bin ?? string.Empty) != string.Empty).Select(b => b.Bin).Distinct());
            lstOfOrderLineItemBin = binDAL.GetAllBinMastersByBinList(strBinNumbers, RoomID, CompanyID);
            MaterialStagingDAL objMsDAL = new MaterialStagingDAL(EnterPriseDBName);
            UDFDAL objUDFDAL = new UDFDAL(EnterPriseDBName);
            IEnumerable<UDFDTO> UDFDataFromDB_Order = objUDFDAL.GetRequiredUDFsByUDFTableNamePlain("OrderMaster", RoomID, CompanyID);
            IEnumerable<UDFDTO> UDFDataFromDB_OrderDtl = objUDFDAL.GetRequiredUDFsByUDFTableNamePlain("OrderDetails", RoomID, CompanyID);
            Dictionary<Guid, double> itemsOrderQtyForItemMinMax = new Dictionary<Guid, double>();
            Dictionary<string, double> itemsOrderQtyForBinMinMax = new Dictionary<string, double>();
            Dictionary<Guid, double> itemsreturnOrderQtyForItem = new Dictionary<Guid, double>();

            string currentCulture = "en-US";
            if (HttpContext.Current != null)
            {
                if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                {
                    if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                    {
                        currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                    }

                }
            }
            else
            {
                eTurns.DTO.eTurnsRegionInfo objeTurnsRegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomID, CompanyID, UserID);
                currentCulture = objeTurnsRegionInfo.CultureName;
            }
            string ResourceSupplierMaster = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResSupplierMaster", currentCulture, EnterPriceID, CompanyID);
            string ResourceOrderMaster = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResOrder", currentCulture, EnterPriceID, CompanyID);
            string ResImportMasters = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResImportMasters", currentCulture, EnterPriceID, CompanyID);
            string ResourceItemMaster = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResItemMaster", currentCulture, EnterPriceID, CompanyID);
            string ResourceBinMaster = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResBin", currentCulture, EnterPriceID, CompanyID);
            string ResourceRecieveOrderDetails = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResReceiveOrderDetails", currentCulture, EnterPriceID, CompanyID);

            #region validate
            foreach (OrderMasterItemsMain Ord in lstOrderMaster)
            {
                Ord.OrderGUID = Guid.NewGuid();
                SupplierMasterDTO SuppDTo = null;
                if (TableName.ToLower() == "returnorders")
                {
                    Ord.OrderStatus = "Closed";
                    Ord.OrderType = "returnorder";
                }

                #region Supplier blank validation
                if (Ord.Supplier == string.Empty || Ord.Supplier == null)
                {
                    string MsgSupplierNotAllowBlank = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgSupplierNotAllowBlank", ResourceSupplierMaster, EnterPriceID, CompanyID, RoomID, "ResSupplierMaster", currentCulture);
                    Ord.Status = strOrderFail;
                    Ord.Reason = MsgSupplierNotAllowBlank;
                    continue;
                }
                #endregion

                #region Order number validation
                if (Ord.OrderNumber == string.Empty || Ord.OrderNumber == null)
                {
                    string OrderNumberValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("OrderNumberValidation", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                    Ord.Status = strOrderFail;
                    Ord.Reason = OrderNumberValidation;
                    continue;
                }
                #endregion

                #region Required date blank validation
                if (Ord.RequiredDate == string.Empty || Ord.RequiredDate == null)
                {
                    string RequiredDateValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("RequiredDateValidation", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                    Ord.Reason = RequiredDateValidation;
                    if (TableName != "" && TableName.ToLower() == "returnorders")
                    {
                        string ReturnedDateValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("ReturnedDateValidation", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                        Ord.Reason = ReturnedDateValidation;
                    }
                    Ord.Status = strOrderFail;
                    continue;
                }
                #endregion

                #region Order Status Blank validation
                if (Ord.OrderStatus == string.Empty || Ord.OrderStatus == null)
                {
                    string OrderStatusNotBlank = ResourceRead.GetResourceValueByKeyAndFullFilePath("OrderStatusNotBlank", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                    Ord.Status = strOrderFail;
                    Ord.Reason = OrderStatusNotBlank;
                    continue;
                }
                #endregion

                #region Order Type Blank validation
                if (Ord.OrderType == string.Empty || Ord.OrderType == null)
                {
                    string OrderTypeBlankValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("OrderTypeBlankValidation", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                    Ord.Status = strOrderFail;
                    Ord.Reason = OrderTypeBlankValidation;
                    continue;
                }
                #endregion

                #region Order Status validation
                // Order Status
                if (Ord.OrderStatus != string.Empty)
                {
                    int Status = GetOrderStatus(Ord.OrderStatus);
                    if (Status == 0)
                    {
                        string ValidOrderStatusValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("ValidOrderStatusValidation", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                        Ord.Status = strOrderFail;
                        Ord.Reason = ValidOrderStatusValidation;
                        continue;
                    }
                }
                #endregion

                #region Order Type validation
                // Order Type
                if (Ord.OrderType != string.Empty)
                {
                    int OrderType = GetOrderType(Ord.OrderType);
                    if (OrderType == 0)
                    {
                        string ValidOrderTypeValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("ValidOrderTypeValidation", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                        Ord.Reason = ValidOrderTypeValidation;
                        Ord.Status = strOrderFail;
                        continue;
                    }
                }
                #endregion

                #region Item number blank validation
                if (string.IsNullOrWhiteSpace(Ord.ItemNumber) && Ord.ItemNumber == string.Empty)
                {
                    string ItemNumberEmptyValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("ItemNumberEmptyValidation", ResourceItemMaster, EnterPriceID, CompanyID, RoomID, "ResItemMaster", currentCulture);
                    Ord.Status = strOrderFail;
                    Ord.Reason = ItemNumberEmptyValidation;
                    continue;
                }
                #endregion

                #region Bin not allow empty validation
                if (string.IsNullOrWhiteSpace(Ord.Bin) && Ord.Bin == string.Empty)
                {
                    string BinNotAllowEmpty = ResourceRead.GetResourceValueByKeyAndFullFilePath("BinNotAllowEmpty", ResourceBinMaster, EnterPriceID, CompanyID, RoomID, "ResBin", currentCulture);
                    Ord.Status = strOrderFail;
                    Ord.Reason = BinNotAllowEmpty;
                    continue;
                }
                #endregion

                #region Requested quantity validation 
                if (Ord.RequestedQty != null && (double)Ord.RequestedQty <= 0)
                {
                    string MsgRequestedQtyValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgRequestedQtyValidation", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                    Ord.Status = strOrderFail;
                    Ord.Reason = MsgRequestedQtyValidation;
                    continue;
                }
                #endregion

                #region UDF Validation
                string OrdersUDFRequier = string.Empty;
                //CheckUDFIsRequired("OrderMaster", Ord.OrderUDF1, Ord.OrderUDF2, Ord.OrderUDF3, Ord.OrderUDF4, Ord.OrderUDF5, out OrdersUDFRequier, CompanyID, RoomID);
                CommonUtilityHelper.CheckUDFIsRequired(UDFDataFromDB_Order, Ord.OrderUDF1, Ord.OrderUDF2, Ord.OrderUDF3, Ord.OrderUDF4, Ord.OrderUDF5, out OrdersUDFRequier, EnterPriceID, CompanyID, RoomID, currentCulture, null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
                if (!string.IsNullOrEmpty(OrdersUDFRequier))
                {
                    Ord.Status = strOrderFail;
                    Ord.Reason = OrdersUDFRequier;
                    continue;
                }

                #region Order detail UDF validation
                string ordDetailUDFReq = string.Empty;
                //CheckUDFIsRequired("OrderDetails", Ord.LineItemUDF1, Ord.LineItemUDF2, Ord.LineItemUDF3, Ord.LineItemUDF4, Ord.LineItemUDF5, out ordDetailUDFReq, CompanyID, RoomID);
                CommonUtilityHelper.CheckUDFIsRequired(UDFDataFromDB_OrderDtl, Ord.LineItemUDF1, Ord.LineItemUDF2, Ord.LineItemUDF3, Ord.LineItemUDF4, Ord.LineItemUDF5, out ordDetailUDFReq, EnterPriceID, CompanyID, RoomID, currentCulture, null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
                if (!string.IsNullOrEmpty(ordDetailUDFReq))
                {
                    Ord.Status = strOrderFail;
                    Ord.Reason = ordDetailUDFReq;
                    continue;
                }
                #endregion
                #endregion

                #region Supplier blank and available validation
                if (Ord.Supplier == string.Empty || Ord.Supplier == null)
                {
                    if (objRoomDTO.DefaultSupplierID != null && Convert.ToInt64(objRoomDTO.DefaultSupplierID) > 0)
                    {
                        SuppDTo = objSuppDAL.GetSupplierByIDPlain(objRoomDTO.DefaultSupplierID.GetValueOrDefault(0));
                        if (SuppDTo != null)
                            Ord.Supplier = SuppDTo.SupplierName;
                        else
                        {
                            string MsgSupplierNotFound = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgSupplierNotFound", ResourceSupplierMaster, EnterPriceID, CompanyID, RoomID, "ResSupplierMaster", currentCulture);
                            Ord.Status = strOrderFail;
                            Ord.Reason = MsgSupplierNotFound;
                            continue;
                        }
                    }
                    else
                    {
                        string MsgSupplierNotAllowBlank = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgSupplierNotAllowBlank", ResourceSupplierMaster, EnterPriceID, CompanyID, RoomID, "ResSupplierMaster", currentCulture);
                        Ord.Status = strOrderFail;
                        Ord.Reason = MsgSupplierNotAllowBlank;
                        continue;
                    }
                }
                SupplierMasterDTO objSuppDTO = objSuppDAL.GetSupplierByNamePlain(RoomID, CompanyID, false, Ord.Supplier);
                if (objSuppDTO == null)
                {
                    string MsgSupplierNotFound = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgSupplierNotFound", ResourceSupplierMaster, EnterPriceID, CompanyID, RoomID, "ResSupplierMaster", currentCulture);
                    Ord.Status = strOrderFail;
                    Ord.Reason = MsgSupplierNotFound;
                    continue;
                }
                #endregion

                #region Order number duplication and related validation
                string orderNumber = Ord.OrderNumber;
                if (orderNumber == string.Empty)
                {
                    AutoOrderNumberGenerate objAutoNumber = objAutoSeqDAL.GetNextOrderNumber(RoomID, CompanyID, objSuppDTO.ID, EnterPriceID);
                    orderNumber = objAutoNumber.OrderNumber;
                    //if (objAutoNumber.IsBlanketPO && (orderNumber == string.Empty || orderNumber == null))
                    //{
                    //    IEnumerable<SupplierBlanketPODetailsDTO> objSuppBlnkPOList = null;
                    //    objSuppBlnkPOList = (from x in objAutoNumber.BlanketPOs
                    //                         where x != null
                    //                         && Convert.ToDateTime(x.StartDate.GetValueOrDefault(DateTime.MinValue).ToShortDateString()) <= Convert.ToDateTime(DateTimeUtility.DateTimeNow.ToShortDateString())
                    //                         && Convert.ToDateTime(x.Enddate.GetValueOrDefault(DateTime.MinValue).ToShortDateString()) >= Convert.ToDateTime(DateTimeUtility.DateTimeNow.ToShortDateString())
                    //                         select x);
                    //    if (objSuppBlnkPOList != null && objSuppBlnkPOList.Count() > 0)
                    //    {
                    //        orderNumber = objSuppBlnkPOList.FirstOrDefault().BlanketPO;
                    //    }
                    //}
                }

                if (Ord.OrderNumber == string.Empty && orderNumber != string.Empty && orderNumber != null)
                {
                    Ord.OrderNumber = orderNumber;
                }

                if (!string.IsNullOrEmpty(orderNumber) && orderNumber != null)
                {
                    if (orderNumber.Length > 22)
                    {
                        string OrderNumberLengthUpto22Char = ResourceRead.GetResourceValueByKeyAndFullFilePath("OrderNumberLengthUpto22Char", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                        Ord.Status = strOrderFail;
                        Ord.Reason = OrderNumberLengthUpto22Char; //ResOrder.OrderNumberLengthUpto22Char
                        continue;
                    }
                }

                // Order Duplicate check
                string strOK = string.Empty;
                if (objRoomDTO.IsAllowOrderDuplicate != true)
                {
                    if (obj.IsOrderNumberDuplicateById(orderNumber, 0, RoomID, CompanyID))
                    {
                        strOK = "duplicate";
                    }
                }
                if (strOK == "duplicate")
                {
                    string DuplicateOrderNumberValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("DuplicateOrderNumberValidation", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                    Ord.Status = strOrderFail;
                    Ord.Reason = DuplicateOrderNumberValidation;
                    continue;
                }
                #endregion

                #region Required Date validation and Format validation
                //RequireDate
                //if (Ord.RequiredDate == string.Empty || Ord.RequiredDate == null)
                //{
                //    if (objSuppDTO != null && objSuppDTO.DefaultOrderRequiredDays != null && Convert.ToInt64(objSuppDTO.DefaultOrderRequiredDays) > 0)
                //    {
                //        DateTime dt = DateTimeUtility.DateTimeNow;
                //        dt = dt.AddDays(Convert.ToInt64(objSuppDTO.DefaultOrderRequiredDays));
                //        Ord.RequiredDate = dt.ToString(RoomDateFormat);
                //        //DateTime.TryParseExact(Ord.RequiredDate, RoomDateFormat, RoomCulture, System.Globalization.DateTimeStyles.None, out dt);
                //    }
                //}
                if (Ord.RequiredDate != string.Empty && Ord.RequiredDate != null && Ord.RequiredDate != "")
                {
                    DateTime dt;
                    string _expirationDt = Ord.RequiredDate;
                    DateTime.TryParseExact(_expirationDt, RoomDateFormat, RoomCulture, System.Globalization.DateTimeStyles.None, out dt);
                    if (dt != DateTime.MinValue)
                    {
                        //obj.RequiredDate = dt.ToString(SessionHelper.RoomDateFormat);
                    }
                    else
                    {
                        string DateShouldBeInFormat = ResourceRead.GetResourceValueByKeyAndFullFilePath("DateShouldBeInFormat", ResImportMasters, EnterPriceID, CompanyID, RoomID, "ResImportMasters", currentCulture);
                        Ord.Status = strOrderFail;
                        Ord.Reason = DateShouldBeInFormat.Replace("{0}", "RequiredDate").Replace("{1}", RoomDateFormat);
                        continue;
                    }
                }
                //else
                //{
                //    string RequiredDateValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("RequiredDateValidation", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                //    Ord.Reason = RequiredDateValidation;
                //    if (TableName != "" && TableName.ToLower() == "returnorders")
                //    {
                //        string ReturnedDateValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("ReturnedDateValidation", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                //        Ord.Reason = ReturnedDateValidation;
                //    }
                //    Ord.Status = strOrderFail;
                //    continue;
                //}
                #endregion

                #region Itemnumber validation
                if (TableName.ToLower() == "returnorders" && (!string.IsNullOrEmpty(Ord.ItemNumber)))
                {
                    ItemMasterDTO ItemDetails = ItemDAL.GetRecordByItemNumber(Ord.ItemNumber, RoomID, CompanyID);
                    if (ItemDetails == null)
                    {
                        string MsgItemNotExistValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgItemNotExistValidation", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                        Ord.Status = strOrderFail;
                        Ord.Reason = MsgItemNotExistValidation;
                        continue;
                    }
                    if (ItemDetails != null && TableName.ToLower() == "returnorders")
                    {
                        if (ItemDetails.SerialNumberTracking == true || ItemDetails.LotNumberTracking == true || ItemDetails.DateCodeTracking == true || ItemDetails.ItemType != 1)
                        {
                            string MsgItemNotAllowed = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgItemNotAllowed", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                            Ord.Status = strOrderFail;
                            Ord.Reason = MsgItemNotAllowed;
                            continue;
                        }
                    }
                }
                #endregion

                #region Item not exist validation
                ItemMasterDTO ItemDTO = ItemDAL.GetRecordByItemNumber(Ord.ItemNumber, RoomID, CompanyID);
                if (ItemDTO == null)
                {
                    string MsgItemNotExistValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgItemNotExistValidation", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                    Ord.Status = strOrderFail;
                    Ord.Reason = MsgItemNotExistValidation;
                    continue;
                }
                #endregion

                Int64? binID = null;
                bool isStaginHaveDefaultLocation = false;
                if (Ord.StagingName != null && Ord.StagingName != string.Empty)
                {
                    Guid? MaterialStagingGUID = GetOrInsertMaterialStagingGUIDByName(Ord.StagingName, UserID, RoomID, CompanyID);
                    if (MaterialStagingGUID != Guid.Empty)
                    {
                        MaterialStagingDTO objMSDTO = null;
                        objMSDTO = objMsDAL.GetRecord(MaterialStagingGUID.Value, RoomID, CompanyID);
                        if (!string.IsNullOrEmpty(objMSDTO.StagingLocationName))
                        {
                            isStaginHaveDefaultLocation = true;
                        }
                    }
                }
                BinMasterDTO BinDTO = new BinMasterDTO();
                if (TableName.ToLower() == "returnorders")
                {
                    BinMasterDTO objBinMasterDTO = binDAL.GetBinByBinNumberPlain(Ord.Bin, RoomID, CompanyID, isStaginHaveDefaultLocation);
                    if (objBinMasterDTO == null)
                    {
                        string MsgEnterValidBinName = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgEnterValidBinName", ResourceBinMaster, EnterPriceID, CompanyID, RoomID, "ResBin", currentCulture);
                        Ord.Status = strOrderFail;
                        Ord.Reason = MsgEnterValidBinName;
                        continue;
                    }
                    else
                    {
                        var objParentBinMasterDTO = binDAL.GetItemBinByParentBinPlain(ItemDTO.GUID, objBinMasterDTO.ID, RoomID, CompanyID, isStaginHaveDefaultLocation);
                        if (objParentBinMasterDTO != null)
                        {
                            binID = objParentBinMasterDTO.ID;
                        }
                        else
                        {
                            binID = objBinMasterDTO.ID;
                        }
                        BinDTO = binDAL.GetAllRecordsItemBinWise(ItemDTO.GUID, RoomID, CompanyID, (Int64)binID).FirstOrDefault();
                    }
                }
                else
                {
                    binID = binDAL.GetOrInsertBinIDByName(ItemDTO.GUID, Ord.Bin, UserID, RoomID, CompanyID, isStaginHaveDefaultLocation);
                    BinDTO = binDAL.GetAllRecordsItemBinWise(ItemDTO.GUID, RoomID, CompanyID, (Int64)binID).FirstOrDefault();
                }

                if (ItemDTO.OrderUOMValue == null || ItemDTO.OrderUOMValue <= 0)
                    ItemDTO.OrderUOMValue = 1;

                if (Ord.RequestedQty != null && Ord.RequestedQty > 0)
                {
                    double Modulo = 0;
                    if (BinDTO != null && BinDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                    {
                        if (ItemDTO.IsAllowOrderCostuom)
                            Modulo = ((Ord.RequestedQty ?? 0) * (ItemDTO.OrderUOMValue ?? 1)) % (BinDTO.DefaultReorderQuantity ?? 1);
                        else
                            Modulo = (Ord.RequestedQty ?? 0) % (BinDTO.DefaultReorderQuantity ?? 1);
                        if (Modulo != 0)
                        {
                            string RequestedQtyNotMatchedWithLocationDefaultReOrderQty = ResourceRead.GetResourceValueByKeyAndFullFilePath("RequestedQtyNotMatchedWithLocationDefaultReOrderQty", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                            Ord.Status = strOrderFail;
                            Ord.Reason = string.Format(RequestedQtyNotMatchedWithLocationDefaultReOrderQty, BinDTO.BinNumber, BinDTO.DefaultReorderQuantity, ItemDTO.ItemNumber);//string.Format(ResOrder.RequestedQtyNotMatchedWithLocationDefaultReOrderQty, objBinMasterDTO.BinNumber, binDefault, objItemMasterDTO.ItemNumber)
                            continue;
                        }
                    }
                    else if (ItemDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                    {
                        if (ItemDTO.IsAllowOrderCostuom)
                            Modulo = ((Ord.RequestedQty ?? 0) * (ItemDTO.OrderUOMValue ?? 1)) % (ItemDTO.DefaultReorderQuantity ?? 1);
                        else
                            Modulo = (Ord.RequestedQty ?? 0) % (ItemDTO.DefaultReorderQuantity ?? 1);
                        if (Modulo != 0)
                        {
                            string RequestedQtyNotMatchedWithDefaultReOrderQty = ResourceRead.GetResourceValueByKeyAndFullFilePath("RequestedQtyNotMatchedWithDefaultReOrderQty", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                            Ord.Status = strOrderFail;
                            Ord.Reason = string.Format(RequestedQtyNotMatchedWithDefaultReOrderQty, ItemDTO.DefaultReorderQuantity, ItemDTO.ItemNumber);//string.Format(ResOrder.RequestedQtyNotMatchedWithDefaultReOrderQty, objItemMasterDTO.DefaultReorderQuantity, objItemMasterDTO.ItemNumber)
                            continue;
                        }
                    }
                }

                if (Ord.ApprovedQty != null && Ord.ApprovedQty > 0)
                {
                    double Modulo = 0;
                    if (BinDTO != null && BinDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                    {
                        if (ItemDTO.IsAllowOrderCostuom)
                            Modulo = ((Ord.ApprovedQty ?? 0) * (ItemDTO.OrderUOMValue ?? 1)) % (BinDTO.DefaultReorderQuantity ?? 1);
                        else
                            Modulo = (Ord.ApprovedQty ?? 0) % (BinDTO.DefaultReorderQuantity ?? 1);
                        if (Modulo != 0)
                        {
                            string MsgApprovedQuantityValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgApprovedQuantityValidation", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                            Ord.Status = strOrderFail;
                            Ord.Reason = string.Format(MsgApprovedQuantityValidation, BinDTO.BinNumber, BinDTO.DefaultReorderQuantity, ItemDTO.ItemNumber);
                            continue;
                        }
                    }
                    else if (ItemDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                    {
                        if (ItemDTO.IsAllowOrderCostuom)
                            Modulo = ((Ord.ApprovedQty ?? 0) * (ItemDTO.OrderUOMValue ?? 1)) % (ItemDTO.DefaultReorderQuantity ?? 1);
                        else
                            Modulo = (Ord.ApprovedQty ?? 0) % (ItemDTO.DefaultReorderQuantity ?? 1);
                        if (Modulo != 0)
                        {
                            string MsgDefaultReorderQuantity = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgDefaultReorderQuantity", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                            Ord.Status = strOrderFail;
                            Ord.Reason = string.Format(MsgDefaultReorderQuantity, ItemDTO.DefaultReorderQuantity, ItemDTO.ItemNumber);
                            continue;
                        }
                    }
                }



                // Validate Prevent Max On order if set in Room level
                int orderType = (!string.IsNullOrEmpty(Ord.OrderType) ? Ord.OrderType == OrderType.Order.ToString() ? (int)OrderType.Order : 0 : 0);
                if (orderType == (int)OrderType.Order && objRoomDTO.PreventMaxOrderQty == (int)PreventMaxOrderQty.OnOrder && Ord.OrderStatus != OrderStatus.Closed.ToString())
                {
                    var isOrderStatusSubmit = !string.IsNullOrEmpty(Ord.OrderStatus) ? (GetOrderStatus(Ord.OrderStatus) >= (int)OrderStatus.Approved) : false;
                    var isOrderStatusHigherThanSubmit = !string.IsNullOrEmpty(Ord.OrderStatus) ? (GetOrderStatus(Ord.OrderStatus) > (int)OrderStatus.Approved) ? true : false : false;

                    if (ItemDTO.IsItemLevelMinMaxQtyRequired.HasValue && ItemDTO.IsItemLevelMinMaxQtyRequired.Value)
                    {
                        var tmpItemOnOrderQty = ItemDTO.OnOrderQuantity.GetValueOrDefault(0);
                        double itemOrderQtySoFar = 0;

                        if (itemsOrderQtyForItemMinMax.ContainsKey(ItemDTO.GUID))
                        {
                            itemOrderQtySoFar += itemsOrderQtyForItemMinMax[ItemDTO.GUID];
                        }
                        if (ItemDTO.MaximumQuantity.HasValue && ItemDTO.MaximumQuantity.Value > 0 && (tmpItemOnOrderQty + (isOrderStatusSubmit ? Ord.ApprovedQty.GetValueOrDefault(0) : Ord.RequestedQty.GetValueOrDefault(0))) > ItemDTO.MaximumQuantity.Value)
                        {
                            string ItemNotAddedMaxQtyReached = ResourceRead.GetResourceValueByKeyAndFullFilePath("ItemNotAddedMaxQtyReached", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                            Ord.Status = strOrderFail;
                            Ord.Reason = string.Format(ItemNotAddedMaxQtyReached, ItemDTO.ItemNumber); //string.Format(ResOrder.ItemNotAddedMaxQtyReached, objItemMasterDTO.ItemNumber);
                            continue;
                        }
                        else
                        {
                            itemsOrderQtyForItemMinMax[ItemDTO.GUID] = (
                                        itemsOrderQtyForItemMinMax.ContainsKey(ItemDTO.GUID)
                                        ? (itemsOrderQtyForItemMinMax[ItemDTO.GUID] + (isOrderStatusSubmit ? Ord.ApprovedQty.GetValueOrDefault(0) : Ord.RequestedQty.GetValueOrDefault(0)))
                                        : (isOrderStatusSubmit ? Ord.ApprovedQty.GetValueOrDefault(0) : Ord.RequestedQty.GetValueOrDefault(0))
                                        );
                        }
                    }
                    else
                    {
                        var maxQtyAtBinLevel = lstOfOrderLineItemBin.Where(e => e.BinNumber.Equals(Ord.Bin) && e.ItemGUID.GetValueOrDefault(Guid.Empty) == ItemDTO.GUID).FirstOrDefault();
                        var tmpBinId = (maxQtyAtBinLevel != null && maxQtyAtBinLevel.ID > 0) ? maxQtyAtBinLevel.ID : 0;
                        var onOrderQtyAtBin = objDAL.GetOrderdQtyOfItemBinWise(RoomID, CompanyID, ItemDTO.GUID, tmpBinId);
                        var tmponOrderQtyAtBin = onOrderQtyAtBin;
                        double itemOrderQtySoFar = 0;
                        if (itemsOrderQtyForBinMinMax.ContainsKey(Convert.ToString(ItemDTO.GUID) + "_" + Ord.Bin))
                        {
                            itemOrderQtySoFar += itemsOrderQtyForBinMinMax[Convert.ToString(ItemDTO.GUID) + "_" + Ord.Bin];
                        }

                        if (maxQtyAtBinLevel != null && maxQtyAtBinLevel.MaximumQuantity.HasValue && maxQtyAtBinLevel.MaximumQuantity.Value > 0
                            && (tmponOrderQtyAtBin + itemOrderQtySoFar + (isOrderStatusSubmit ? Ord.ApprovedQty.GetValueOrDefault(0) : Ord.RequestedQty.GetValueOrDefault(0))) > maxQtyAtBinLevel.MaximumQuantity.Value)
                        {
                            string ItemNotAddedBinMaxQtyReached = ResourceRead.GetResourceValueByKeyAndFullFilePath("ItemNotAddedBinMaxQtyReached", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                            Ord.Status = strOrderFail;
                            Ord.Reason = string.Format(ItemNotAddedBinMaxQtyReached, ItemDTO.ItemNumber); //string.Format(ResOrder.ItemNotAddedBinMaxQtyReached, objItemMasterDTO.ItemNumber)
                        }
                        else
                        {
                            itemsOrderQtyForBinMinMax[Convert.ToString(ItemDTO.GUID) + "_" + Ord.Bin] =
                                    (itemsOrderQtyForBinMinMax.ContainsKey(Convert.ToString(ItemDTO.GUID) + "_" + Ord.Bin)
                                    ? (itemsOrderQtyForBinMinMax[Convert.ToString(ItemDTO.GUID) + "_" + Ord.Bin] + (isOrderStatusSubmit ? Ord.ApprovedQty.GetValueOrDefault(0) : Ord.RequestedQty.GetValueOrDefault(0)))
                                    : (isOrderStatusSubmit ? Ord.ApprovedQty.GetValueOrDefault(0) : Ord.RequestedQty.GetValueOrDefault(0))
                                    );
                        }
                    }
                }
                #region Blanket PO Validation
                //WI-8417
                if (Ord.OrderGUID.HasValue && orderType == (int)OrderType.Order && GetOrderStatus(Ord.OrderStatus) >= (int)OrderStatus.Approved && GetOrderStatus(Ord.OrderStatus) != (int)OrderStatus.Closed)
                {
                    List<OrderDetailsDTO> lstOfItems = new List<OrderDetailsDTO>();
                    string validationMsgForOrder = string.Empty;
                    double ItemCost = 0;
                    int ItemCostUOMValue = 0;

                    if (ItemDTO != null)
                    {
                        if (ItemDTO.Consignment)
                        {
                            ItemCost = ItemDTO.Cost.GetValueOrDefault(0);
                        }
                        else
                        {
                            if (Ord.OrderCost == null)
                            {
                                ItemCost = ItemDTO.Cost.GetValueOrDefault(0);
                            }
                            else
                            {
                                if (objRoomDTO.AllowABIntegration)
                                {
                                    ProductDetailsDAL objProductDetailsDAL = new ProductDetailsDAL(EnterPriseDBName);
                                    Int64 ABItemMappingID = objProductDetailsDAL.CheckItemAddedFromAB(ItemDTO.SupplierPartNo, ItemDTO.GUID, CompanyID, RoomID);
                                    if (ABItemMappingID > 0)
                                    {
                                        ItemCost = ItemDTO.Cost.GetValueOrDefault(0);
                                    }
                                    else
                                    {
                                        ItemCost = Ord.OrderCost.GetValueOrDefault(0);
                                    }
                                }
                                else
                                {
                                    ItemCost = Ord.OrderCost.GetValueOrDefault(0);
                                }
                            }
                        }

                        if (ItemDTO.CostUOMValue == null || ItemDTO.CostUOMValue.GetValueOrDefault(0) <= 0)
                        {
                            ItemCostUOMValue = 1;
                        }
                        else
                        {
                            ItemCostUOMValue = ItemDTO.CostUOMValue.GetValueOrDefault(1);
                        }
                    }
                    else
                    {
                        ItemCostUOMValue = 1;
                    }

                    lstOfItems.Add(new OrderDetailsDTO
                    {
                        ApprovedQuantity = Ord.ApprovedQty.GetValueOrDefault(0),
                        ItemGUID = ItemDTO.GUID,
                        OrderLineItemExtendedCost = (Ord.ApprovedQty.GetValueOrDefault(0) * (ItemCost / ItemCostUOMValue))
                    });
                    var isValidOrderLineItems = ValidateOrderItemOnSupplierBlanketPO(Ord.OrderGUID ?? Guid.Empty, lstOfItems, RoomID, CompanyID, EnterPriceID, UserID, out validationMsgForOrder);

                    if (!isValidOrderLineItems)
                    {
                        Ord.Status = strOrderFail;
                        Ord.Reason = validationMsgForOrder.Replace("<br/>", "");
                        continue;
                    }
                }
                #endregion
                // ISClose Item
                //if (Ord.IsCloseItem != string.Empty)
                //{
                //    bool IsCloseValidate = GetIsCloseItem(Ord.IsCloseItem);
                //    if (IsCloseValidate == false)
                //    {
                //        Ord.Status = strOrderFail;
                //        Ord.Reason = "provide valid IsCloseItem";
                //        continue;
                //    }
                //}

                if (TableName.ToLower() == "returnorders")
                {
                    // List<OrderMasterItemsMain> lstItems = lstOrderMaster.ToList();

                    //foreach (OrderMasterItemsMain item in Ord)
                    //{
                    ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
                    List<ItemLocationDetailsDTO> ObjItemLocation = objLocationDAL.GetAllRecordsAvailableAt((Int64)binID, RoomID, CompanyID, ItemDTO.GUID, null, "CustomerOwnedQuantity DESC, ConsignedQuantity DESC, ID ASC").ToList();
                    double onHandQuantity = 0.0;
                    foreach (var itemlocation in ObjItemLocation)
                    {
                        if (ItemDTO.Consignment)
                        {
                            onHandQuantity = onHandQuantity + (itemlocation.ConsignedQuantity.GetValueOrDefault(0) + (Double)itemlocation.CustomerOwnedQuantity.GetValueOrDefault(0));
                        }
                        else
                        {
                            onHandQuantity = onHandQuantity + (Double)itemlocation.CustomerOwnedQuantity.GetValueOrDefault(0);
                        }
                    }

                    if (!itemsreturnOrderQtyForItem.ContainsKey(ItemDTO.GUID))
                    {
                        itemsreturnOrderQtyForItem.Add(ItemDTO.GUID, onHandQuantity);
                    }
                    else if (itemsreturnOrderQtyForItem.ContainsKey(ItemDTO.GUID))
                    {
                        itemsreturnOrderQtyForItem[ItemDTO.GUID] = onHandQuantity;
                    }

                    if (itemsreturnOrderQtyForItem.ContainsKey(ItemDTO.GUID))
                    {
                        double temponHandQuantity = itemsreturnOrderQtyForItem[ItemDTO.GUID];
                        if (Ord.RequestedQty > temponHandQuantity)
                        {
                            //Quantity validation
                            string BinHaveNotSufficientQtyToReturn = ResourceRead.GetResourceValueByKeyAndFullFilePath("BinHaveNotSufficientQtyToReturn", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                            Ord.Status = strOrderFail;
                            Ord.Reason = string.Format(BinHaveNotSufficientQtyToReturn, ItemDTO.BinNumber);
                            continue;
                        }
                        else
                        {
                            var transonhandquantity = itemsreturnOrderQtyForItem[ItemDTO.GUID];
                            itemsreturnOrderQtyForItem[ItemDTO.GUID] = transonhandquantity - (double)Ord.RequestedQty;
                        }
                    }

                    //}

                    //check Order Cost
                    if (Ord.OrderCost == null || Ord.OrderCost < 0)
                    {
                        string ReturnOrderCostRequired = ResourceRead.GetResourceValueByKeyAndFullFilePath("ReturnOrderCostRequired", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                        Ord.Status = strOrderFail;
                        Ord.Reason = ReturnOrderCostRequired;
                        continue;
                    }

                }
            }
            #endregion


            #region Get Unique Order
            List<OrderMasterItemsMain> lstOrdHeader = (from t in lstOrderMaster
                                                       where t.Status != strOrderFail
                                                       select new OrderMasterItemsMain
                                                       {
                                                           OrderGUID = t.OrderGUID,
                                                           Supplier = t.Supplier,
                                                           OrderNumber = t.OrderNumber,
                                                           RequiredDate = t.RequiredDate,
                                                           OrderStatus = t.OrderStatus,
                                                           StagingName = t.StagingName,
                                                           OrderComment = t.OrderComment,
                                                           CustomerName = t.CustomerName,
                                                           PackSlipNumber = t.PackSlipNumber,
                                                           ShippingTrackNumber = t.ShippingTrackNumber,
                                                           OrderUDF1 = t.OrderUDF1,
                                                           OrderUDF2 = t.OrderUDF2,
                                                           OrderUDF3 = t.OrderUDF3,
                                                           OrderUDF4 = t.OrderUDF4,
                                                           OrderUDF5 = t.OrderUDF5,
                                                           ShipVia = t.ShipVia,
                                                           OrderType = t.OrderType,
                                                           ShippingVendor = t.ShippingVendor,
                                                           SalesOrder = t.SalesOrder,
                                                           //AccountNumber = t.AccountNumber,
                                                           SupplierAccount = t.SupplierAccount,
                                                           Reason = t.Reason,
                                                           Status = t.Status
                                                       }).OrderBy(x => x.Supplier).GroupBy(x => new { x.Supplier, x.OrderNumber, x.RequiredDate, x.OrderStatus, x.StagingName, x.OrderComment, x.CustomerName, x.PackSlipNumber, x.ShippingTrackNumber, x.OrderUDF1, x.OrderUDF2, x.OrderUDF3, x.OrderUDF4, x.OrderUDF5, x.ShipVia, x.OrderType, x.ShippingVendor, x.SupplierAccount, x.SalesOrder }).Select(x => new OrderMasterItemsMain
                                                       {
                                                           OrderGUID = x.First().OrderGUID,
                                                           Supplier = x.Key.Supplier,
                                                           OrderNumber = x.Key.OrderNumber,
                                                           RequiredDate = x.Key.RequiredDate,
                                                           OrderStatus = x.Key.OrderStatus,
                                                           StagingName = x.Key.StagingName,
                                                           OrderComment = x.Key.OrderComment,
                                                           CustomerName = x.Key.CustomerName,
                                                           PackSlipNumber = x.Key.PackSlipNumber,
                                                           ShippingTrackNumber = x.Key.ShippingTrackNumber,
                                                           OrderUDF1 = x.Key.OrderUDF1,
                                                           OrderUDF2 = x.Key.OrderUDF2,
                                                           OrderUDF3 = x.Key.OrderUDF3,
                                                           OrderUDF4 = x.Key.OrderUDF4,
                                                           OrderUDF5 = x.Key.OrderUDF5,
                                                           ShipVia = x.Key.ShipVia,
                                                           OrderType = x.Key.OrderType,
                                                           ShippingVendor = x.Key.ShippingVendor,
                                                           //AccountNumber = x.Key.AccountNumber,
                                                           SupplierAccount = x.Key.SupplierAccount,
                                                           SalesOrder = x.Key.SalesOrder
                                                       }).ToList();
            #endregion

            #region Create Order Header
            List<SupplierMasterDTO> lstSupplierDTO = new List<SupplierMasterDTO>();
            if (lstOrdHeader != null && lstOrdHeader.Count > 0)
            {
                string strSupplierList = string.Join(",", lstOrdHeader.Where(x => (x.Supplier ?? string.Empty) != string.Empty).Select(sp => sp.Supplier).Distinct().ToArray());
                lstSupplierDTO = objSuppDAL.GetSupplierByNAMEsPlain(strSupplierList, RoomID, CompanyID);
            }
            foreach (OrderMasterItemsMain objOrdHeader in lstOrdHeader)
            {
                SupplierMasterDTO objSuppDTO = null;
                if (lstSupplierDTO != null && lstSupplierDTO.Count > 0)
                {
                    objSuppDTO = lstSupplierDTO.Where(x => x.Room == RoomID && x.CompanyID == CompanyID && (x.SupplierName ?? string.Empty).ToLower() == (objOrdHeader.Supplier ?? string.Empty).ToLower()).FirstOrDefault();
                }

                if (objSuppDTO != null && objSuppDTO.ID > 0)
                {
                    OrderMasterDTO objOrdDTO = new OrderMasterDTO();
                    string orderNumber = objOrdHeader.OrderNumber;
                    if (orderNumber == string.Empty)
                    {
                        AutoOrderNumberGenerate objAutoNumber = objAutoSeqDAL.GetNextOrderNumber(RoomID, CompanyID, objSuppDTO.ID, EnterPriceID);
                        orderNumber = objAutoNumber.OrderNumber;
                        //if (objAutoNumber.IsBlanketPO && (orderNumber == string.Empty || orderNumber == null))
                        //{
                        //    IEnumerable<SupplierBlanketPODetailsDTO> objSuppBlnkPOList = null;
                        //    objSuppBlnkPOList = (from x in objAutoNumber.BlanketPOs
                        //                         where x != null
                        //                         && Convert.ToDateTime(x.StartDate.GetValueOrDefault(DateTime.MinValue).ToShortDateString()) <= Convert.ToDateTime(DateTimeUtility.DateTimeNow.ToShortDateString())
                        //                         && Convert.ToDateTime(x.Enddate.GetValueOrDefault(DateTime.MinValue).ToShortDateString()) >= Convert.ToDateTime(DateTimeUtility.DateTimeNow.ToShortDateString())
                        //                         select x);
                        //    if (objSuppBlnkPOList != null && objSuppBlnkPOList.Count() > 0)
                        //    {
                        //        orderNumber = objSuppBlnkPOList.FirstOrDefault().BlanketPO;
                        //    }
                        //}
                    }
                    //if (orderNumber != null && (!string.IsNullOrEmpty(orderNumber)))
                    //    orderNumber = orderNumber.Length > 22 ? orderNumber.Substring(0, 22) : orderNumber;

                    if (!string.IsNullOrEmpty(orderNumber) && orderNumber != null)
                    {
                        if (orderNumber.Length > 22)
                        {
                            string OrderNumberLengthUpto22Char = ResourceRead.GetResourceValueByKeyAndFullFilePath("OrderNumberLengthUpto22Char", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                            objOrdHeader.Status = strOrderFail;
                            objOrdHeader.Reason = OrderNumberLengthUpto22Char; //ResOrder.OrderNumberLengthUpto22Char
                            continue;
                        }
                    }

                    if (orderNumber == string.Empty || orderNumber == null)
                    {
                        string OrderNumberValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("OrderNumberValidation", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                        objOrdHeader.Status = strOrderFail;
                        objOrdHeader.Reason = OrderNumberValidation;
                        continue;
                    }
                    else
                        objOrdDTO.OrderNumber = orderNumber;

                    // Order Duplicate check
                    string strOK = string.Empty;
                    if (objRoomDTO.IsAllowOrderDuplicate != true)
                    {
                        if (obj.IsOrderNumberDuplicateById(orderNumber, 0, RoomID, CompanyID))
                        {
                            strOK = "duplicate";
                        }
                    }
                    if (strOK == "duplicate")
                    {
                        string MsgOrderNumberDuplicate = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgOrderNumberDuplicate", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                        objOrdHeader.Status = strOrderFail;
                        objOrdHeader.Reason = MsgOrderNumberDuplicate;
                        continue;
                    }

                    int ReleaseNo = 1;
                    if (!string.IsNullOrWhiteSpace(orderNumber))
                    {
                        var maximumReleaseNumberByOrderNo = objOrderDAL.GetMaximumReleaseNoByOrderNumber(RoomID, CompanyID, orderNumber, OrderType.Order);
                        if (maximumReleaseNumberByOrderNo > 0)
                            ReleaseNo = maximumReleaseNumberByOrderNo + 1;
                    }

                    objOrdDTO.ReleaseNumber = ReleaseNo.ToString();
                    objOrdDTO.Supplier = objSuppDTO.ID;
                    objOrdDTO.StagingID = GetOrInsertMaterialStagingIDByName(objOrdHeader.StagingName, UserID, RoomID, CompanyID);
                    objOrdDTO.Comment = objOrdHeader.OrderComment;
                    objOrdDTO.RequiredDate = DateTime.ParseExact(objOrdHeader.RequiredDate, RoomDateFormat, RoomCulture);
                    // Order Status
                    if (objOrdHeader.OrderStatus != string.Empty)
                    {
                        objOrdDTO.OrderStatus = GetOrderStatus(objOrdHeader.OrderStatus);
                        if (objOrdDTO.OrderStatus == 0)
                        {
                            string ValidOrderStatusValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("ValidOrderStatusValidation", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                            objOrdHeader.Status = strOrderFail;
                            objOrdHeader.Reason = ValidOrderStatusValidation;
                            continue;
                        }
                    }
                    else
                    {
                        string OrderStatusNotBlank = ResourceRead.GetResourceValueByKeyAndFullFilePath("OrderStatusNotBlank", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                        objOrdHeader.Status = strOrderFail;
                        objOrdHeader.Reason = OrderStatusNotBlank;
                        continue;
                    }

                    // Ord Status set submit to Approve
                    if (objUserRoleModuleDetailsDTO != null && objUserRoleModuleDetailsDTO.Count() > 0 && objOrdDTO.OrderStatus == (int)OrderStatus.Submitted)
                    {
                        if (objUserRoleModuleDetailsDTO.Where(i => i.ModuleID == 76).Any())
                        {
                            if (objUserRoleModuleDetailsDTO.Where(i => i.ModuleID == 76).FirstOrDefault().IsChecked == true)
                            {
                                objOrdDTO.OrderStatus = (int)OrderStatus.Approved;
                            }
                        }
                    }

                    // Ord Status set Approve to Transmit
                    SupplierMasterDTO objSuppMast = new SupplierMasterDTO();
                    objSuppMast = objSuppDAL.GetSupplierByIDPlain(objOrdDTO.Supplier.GetValueOrDefault(0));
                    if (objOrdDTO.OrderStatus == (int)OrderStatus.Approved && objSuppMast != null && !objSuppMast.IsSendtoVendor)
                    {
                        objOrdDTO.OrderStatus = (int)OrderStatus.Transmitted;
                    }

                    // Order Type
                    if (objOrdHeader.OrderType != string.Empty)
                    {
                        objOrdDTO.OrderType = GetOrderType(objOrdHeader.OrderType);
                        if (objOrdDTO.OrderType == 0)
                        {
                            string ValidOrderTypeValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("ValidOrderTypeValidation", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                            objOrdHeader.Status = strOrderFail;
                            objOrdHeader.Reason = ValidOrderTypeValidation;
                            continue;
                        }
                    }
                    else
                    {
                        string OrderTypeBlankValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("OrderTypeBlankValidation", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                        objOrdHeader.Status = strOrderFail;
                        objOrdHeader.Reason = OrderTypeBlankValidation;
                        continue;
                    }

                    objOrdDTO.CustomerID = null;
                    CustomerMasterDTO objCustomerMasterDTO = GetOrInsertCustomerGUIDByName(objOrdHeader.CustomerName, UserID, RoomID, CompanyID);
                    if (objCustomerMasterDTO != null)
                    {
                        objOrdDTO.CustomerGUID = objCustomerMasterDTO.GUID;
                        objOrdDTO.CustomerID = objCustomerMasterDTO.ID;
                    }
                    objOrdDTO.PackSlipNumber = objOrdHeader.PackSlipNumber;
                    objOrdDTO.ShippingTrackNumber = objOrdHeader.ShippingTrackNumber;
                    objOrdDTO.Created = DateTimeUtility.DateTimeNow;
                    objOrdDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objOrdDTO.CreatedBy = UserID;
                    objOrdDTO.LastUpdatedBy = UserID;
                    objOrdDTO.Room = RoomID;
                    objOrdDTO.IsDeleted = false;
                    objOrdDTO.IsArchived = false;
                    objOrdDTO.CompanyID = CompanyID;
                    objOrdDTO.GUID = objOrdHeader.OrderGUID ?? Guid.NewGuid();

                    objOrdDTO.UDF1 = objOrdHeader.OrderUDF1;
                    objOrdDTO.UDF2 = objOrdHeader.OrderUDF2;
                    objOrdDTO.UDF3 = objOrdHeader.OrderUDF3;
                    objOrdDTO.UDF4 = objOrdHeader.OrderUDF4;
                    objOrdDTO.UDF5 = objOrdHeader.OrderUDF5;
                    string OrdersUDFRequier = string.Empty;
                    CheckUDFIsRequired("OrderMaster", objOrdDTO.UDF1, objOrdDTO.UDF2, objOrdDTO.UDF3, objOrdDTO.UDF4, objOrdDTO.UDF5, out OrdersUDFRequier, CompanyID, RoomID, EnterPriceID, UserID);
                    if (!string.IsNullOrEmpty(OrdersUDFRequier))
                    {
                        objOrdHeader.Status = strOrderFail;
                        objOrdHeader.Reason = OrdersUDFRequier;
                        continue;
                    }

                    objOrdDTO.ShipVia = GetOrInsertShipVaiIDByName(objOrdHeader.ShipVia, UserID, RoomID, CompanyID);
                    objOrdDTO.OrderDate = DateTimeUtility.DateTimeNow;
                    objOrdDTO.ShippingVendor = GetOrInsertVendorIDByName(objOrdHeader.ShippingVendor, UserID, RoomID, CompanyID);
                    //objOrdDTO.AccountNumber = objOrdHeader.AccountNumber;
                    objOrdDTO.SupplierAccountGuid = GetOrInsertSupplierAccountByName(objOrdHeader.SupplierAccount, UserID, RoomID, CompanyID, objSuppDTO.ID);
                    if (objOrdDTO.SupplierAccountGuid == Guid.Empty)
                        objOrdDTO.SupplierAccountGuid = null;
                    objOrdDTO.WhatWhereAction = "Order Import";
                    objOrdDTO.MaterialStagingGUID = GetOrInsertMaterialStagingGUIDByName(objOrdHeader.StagingName, UserID, RoomID, CompanyID);
                    objOrdDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objOrdDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objOrdDTO.AddedFrom = "Import";
                    objOrdDTO.EditedFrom = "Import";
                    objOrdDTO.IsEDIOrder = false;
                    objOrdDTO.SalesOrder = objOrdHeader.SalesOrder;

                    if (objRoomDTO.POAutoSequence.GetValueOrDefault(0) == 0)
                    {
                        var countOfOrder = obj.GetCountOfOrderByOrderNumber(RoomID, CompanyID, objOrdHeader.OrderNumber);
                        objOrdDTO.ReleaseNumber = Convert.ToString(countOfOrder + 1);
                    }
                    if (objSuppDTO.POAutoSequence.GetValueOrDefault(0) == 1 && objSuppDTO.NextOrderNo.ToString().Trim() == orderNumber.ToString().Trim())
                    {
                        objOrdDTO.ReleaseNumber = Convert.ToString(Convert.ToInt32(objSuppDTO.POAutoNrReleaseNumber) + 1);
                    }
                    if (objOrdDTO.OrderStatus == (int)OrderStatus.Submitted)
                    {
                        objOrdDTO.RequesterID = UserID;
                        objOrdHeader.RequesterID = UserID;
                    }
                    else if (objOrdDTO.OrderStatus == (int)OrderStatus.Approved
                        || objOrdDTO.OrderStatus == (int)OrderStatus.Transmitted)
                    {
                        objOrdDTO.ApproverID = UserID;
                        objOrdHeader.ApproverID = UserID;
                        objOrdDTO.RequesterID = UserID;
                        objOrdHeader.RequesterID = UserID;
                    }

                    objOrdDTO = obj.InsertOrder(objOrdDTO, SessionUserId);

                    long ReturnVal = objOrdDTO.ID;
                    if (ReturnVal > 0)
                    {
                        objOrdHeader.OrderGUID = objOrdDTO.GUID;
                        objOrdHeader.Status = strOrderSuccess;
                    }
                    else
                    {
                        objOrdHeader.Status = strOrderFail;
                    }
                }
                else
                {
                    string MsgSupplierNotAllowBlank = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgSupplierNotAllowBlank", ResourceSupplierMaster, EnterPriceID, CompanyID, RoomID, "ResSupplierMaster", currentCulture);
                    objOrdHeader.Status = strOrderFail;
                    objOrdHeader.Reason = MsgSupplierNotAllowBlank;
                }
            }
            #endregion

            #region Update Order created GUID
            // lstOrderMaster, lstOrdHeader
            foreach (var OrdHdr in lstOrdHeader)
            {
                foreach (var obj1 in (lstOrderMaster.Where(t => t.Supplier == OrdHdr.Supplier && t.OrderNumber == OrdHdr.OrderNumber && t.RequiredDate == OrdHdr.RequiredDate && t.OrderStatus == OrdHdr.OrderStatus &&
                                                                    t.StagingName == OrdHdr.StagingName && t.OrderComment == OrdHdr.OrderComment && t.CustomerName == OrdHdr.CustomerName && t.PackSlipNumber == OrdHdr.PackSlipNumber &&
                                                                    t.ShippingTrackNumber == OrdHdr.ShippingTrackNumber && t.OrderUDF1 == OrdHdr.OrderUDF1 && t.OrderUDF2 == OrdHdr.OrderUDF2 && t.OrderUDF3 == OrdHdr.OrderUDF3 && t.OrderUDF4 == OrdHdr.OrderUDF4 && t.OrderUDF5 == OrdHdr.OrderUDF5 &&
                                                                    t.ShipVia == OrdHdr.ShipVia && t.OrderType == OrdHdr.OrderType && t.ShippingVendor == OrdHdr.ShippingVendor && t.SupplierAccount == OrdHdr.SupplierAccount)))
                {
                    if (obj1.Status != strOrderFail)
                    {
                        obj1.OrderGUID = OrdHdr.OrderGUID;
                        obj1.Status = OrdHdr.Status;
                        obj1.Reason = OrdHdr.Reason;
                    }
                }
            }
            #endregion

            #region Insert Order Items
            IEnumerable<string> failedOrderListNumber = Enumerable.Empty<string>();
            foreach (OrderMasterItemsMain objOrdHeader in lstOrdHeader)
            {
                List<OrderDetailsDTO> lstordDetailsForItemCostUpdate = new List<OrderDetailsDTO>();
                if (objOrdHeader.OrderGUID != Guid.Empty && objOrdHeader.Status == strOrderSuccess)
                {
                    List<OrderMasterItemsMain> lstItems = lstOrderMaster.Where(x => x.OrderGUID == objOrdHeader.OrderGUID && x.Status != strOrderFail).ToList();
                    failedOrderListNumber = lstOrderMaster.Where(x => x.OrderGUID == null && x.Status == strOrderFail).Select(x => x.OrderNumber).ToList().Distinct();
                    foreach (OrderMasterItemsMain item in lstItems)
                    {
                        if (string.IsNullOrWhiteSpace(item.ItemNumber) && item.ItemNumber == string.Empty)
                        {
                            string ItemNumberEmptyValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("ItemNumberEmptyValidation", ResourceItemMaster, EnterPriceID, CompanyID, RoomID, "ResItemMaster", currentCulture);
                            item.Status = strOrderFail;
                            item.Reason = ItemNumberEmptyValidation;
                            continue;
                        }
                        if (string.IsNullOrWhiteSpace(item.Bin) && item.Bin == string.Empty)
                        {
                            string BinNotAllowEmpty = ResourceRead.GetResourceValueByKeyAndFullFilePath("BinNotAllowEmpty", ResourceBinMaster, EnterPriceID, CompanyID, RoomID, "ResBin", currentCulture);
                            item.Status = strOrderFail;
                            item.Reason = BinNotAllowEmpty;
                            continue;
                        }
                        if (item.RequestedQty != null && (double)item.RequestedQty <= 0)
                        {
                            string MsgRequestedQtyValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgRequestedQtyValidation", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                            item.Reason = MsgRequestedQtyValidation;
                            if (TableName.ToLower() == "returnorders")
                            {
                                string MsgRequestedReturnedQtyValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgRequestedReturnedQtyValidation", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                                item.Reason = MsgRequestedReturnedQtyValidation;
                            }
                            item.Status = strOrderFail;
                            continue;
                        }

                        ItemMasterDTO ItemDTO = ItemDAL.GetRecordByItemNumber(item.ItemNumber, RoomID, CompanyID);
                        if (ItemDTO == null)
                        {
                            string MsgItemNotExistValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgItemNotExistValidation", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                            item.Status = strOrderFail;
                            item.Reason = MsgItemNotExistValidation;
                            continue;
                        }

                        if (ItemDTO.OrderUOMValue == null || ItemDTO.OrderUOMValue <= 0)
                            ItemDTO.OrderUOMValue = 1;

                        OrderDetailsDAL objOrderDetailsDAL = new OrderDetailsDAL(EnterPriseDBName);

                        SupplierBlanketPODetailsDAL objSupBlnaPODAL = new SupplierBlanketPODetailsDAL(EnterPriseDBName);

                        MaterialStagingDTO objMSDTO = null;
                        bool isStaginHaveDefaultLocation = false;
                        string binName = string.Empty;
                        Int64? binID = null;
                        OrderMasterDTO ODMDTO = objOrderDAL.GetOrderByGuidPlain(Guid.Parse(item.OrderGUID.ToString()));
                        OrderDetailsDTO ordDTO = new OrderDetailsDTO();
                        BinMasterDTO BinDTO = null;

                        if (ODMDTO.MaterialStagingGUID != null)
                        {
                            objMSDTO = new MaterialStagingDAL(EnterPriseDBName).GetRecord(ODMDTO.MaterialStagingGUID.Value, RoomID, CompanyID);
                            if (!string.IsNullOrEmpty(objMSDTO.StagingLocationName))
                            {
                                isStaginHaveDefaultLocation = true;
                                binName = objMSDTO.StagingLocationName;
                            }
                        }
                        binID = binDAL.GetOrInsertBinIDByName(ItemDTO.GUID, item.Bin, UserID, RoomID, CompanyID, isStaginHaveDefaultLocation);
                        BinDTO = binDAL.GetAllRecordsItemBinWise(ItemDTO.GUID, RoomID, CompanyID, (Int64)binID).FirstOrDefault();

                        if (item.RequestedQty != null && item.RequestedQty > 0)
                        {
                            double Modulo = 0;
                            if (BinDTO != null && BinDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                            {
                                if (ItemDTO.IsAllowOrderCostuom)
                                    Modulo = ((item.RequestedQty ?? 0) * (ItemDTO.OrderUOMValue ?? 1)) % (BinDTO.DefaultReorderQuantity ?? 1);
                                else
                                    Modulo = (item.RequestedQty ?? 0) % (BinDTO.DefaultReorderQuantity ?? 1);
                                if (Modulo != 0)
                                {
                                    string RequestedQtyNotMatchedWithLocationDefaultReOrderQty = ResourceRead.GetResourceValueByKeyAndFullFilePath("RequestedQtyNotMatchedWithLocationDefaultReOrderQty", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                                    item.Status = strOrderFail;
                                    item.Reason = string.Format(RequestedQtyNotMatchedWithLocationDefaultReOrderQty, BinDTO.BinNumber, BinDTO.DefaultReorderQuantity, ItemDTO.ItemNumber);//string.Format(ResOrder.RequestedQtyNotMatchedWithLocationDefaultReOrderQty, objBinMasterDTO.BinNumber, binDefault, objItemMasterDTO.ItemNumber)
                                    continue;
                                }
                            }
                            else if (ItemDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                            {
                                if (ItemDTO.IsAllowOrderCostuom)
                                    Modulo = ((item.RequestedQty ?? 0) * (ItemDTO.OrderUOMValue ?? 1)) % (ItemDTO.DefaultReorderQuantity ?? 1);
                                else
                                    Modulo = (item.RequestedQty ?? 0) % (ItemDTO.DefaultReorderQuantity ?? 1);
                                if (Modulo != 0)
                                {
                                    string RequestedQtyNotMatchedWithDefaultReOrderQty = ResourceRead.GetResourceValueByKeyAndFullFilePath("RequestedQtyNotMatchedWithDefaultReOrderQty", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                                    item.Status = strOrderFail;
                                    item.Reason = string.Format(RequestedQtyNotMatchedWithDefaultReOrderQty, ItemDTO.DefaultReorderQuantity, ItemDTO.ItemNumber); //string.Format(ResOrder.RequestedQtyNotMatchedWithDefaultReOrderQty, objItemMasterDTO.DefaultReorderQuantity, objItemMasterDTO.ItemNumber)
                                    continue;
                                }
                            }
                        }

                        if (item.ApprovedQty != null && item.ApprovedQty > 0)
                        {
                            double Modulo = 0;
                            if (BinDTO != null && BinDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                            {
                                if (ItemDTO.IsAllowOrderCostuom)
                                    Modulo = ((item.ApprovedQty ?? 0) * (ItemDTO.OrderUOMValue ?? 1)) % (BinDTO.DefaultReorderQuantity ?? 1);
                                else
                                    Modulo = (item.ApprovedQty ?? 0) % (BinDTO.DefaultReorderQuantity ?? 1);
                                if (Modulo != 0)
                                {
                                    string MsgApprovedQuantityValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgApprovedQuantityValidation", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                                    item.Status = strOrderFail;
                                    item.Reason = string.Format(MsgApprovedQuantityValidation, BinDTO.BinNumber, BinDTO.DefaultReorderQuantity, ItemDTO.ItemNumber);
                                    continue;
                                }
                            }
                            else if (ItemDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                            {
                                if (ItemDTO.IsAllowOrderCostuom)
                                    Modulo = ((item.ApprovedQty ?? 0) * (ItemDTO.OrderUOMValue ?? 1)) % (ItemDTO.DefaultReorderQuantity ?? 1);
                                else
                                    Modulo = (item.ApprovedQty ?? 0) % (ItemDTO.DefaultReorderQuantity ?? 1);
                                if (Modulo != 0)
                                {
                                    string MsgDefaultReorderQuantity = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgDefaultReorderQuantity", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                                    item.Status = strOrderFail;
                                    item.Reason = string.Format(MsgDefaultReorderQuantity, ItemDTO.DefaultReorderQuantity, ItemDTO.ItemNumber);
                                    continue;
                                }
                            }
                        }
                        if (TableName.ToLower() == "returnorders" && ordDTO.IsPackslipMandatoryAtReceive && (string.IsNullOrEmpty(item.PackSlipNumber) || string.IsNullOrWhiteSpace(item.PackSlipNumber)))
                        {
                            string MsgProvidePackslipNumber = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgProvidePackslipNumber", ResourceRecieveOrderDetails, EnterPriceID, CompanyID, RoomID, "ResReceiveOrderDetails", currentCulture);
                            item.Status = strOrderFail;
                            item.Reason = MsgProvidePackslipNumber;
                            continue;
                        }
                        if (TableName.ToLower() == "returnorders")
                        {
                            var requestedQuantity = item.RequestedQty;
                            //if (ordDTO.StagingID.GetValueOrDefault(0) > 0)
                            //{
                            //    MaterialStagingDetailDAL objMSDetailDAL = new MaterialStagingDetailDAL(base.DataBaseName);
                            //    var msDtlDTO = objMSDetailDAL.GetMaterialStagingDetailbyItemGUIDANDStagingBINID(objMSDTO.GUID, (Int64)binID, ordDTO.ItemGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                            //    if (msDtlDTO == null || msDtlDTO.Quantity < requestedQuantity)
                            //    {
                            //        string StagingBinHaveNotSufficientQtyToReturn = ResourceRead.GetResourceValueByKeyAndFullFilePath("StagingBinHaveNotSufficientQtyToReturn", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                            //        item.Status = strOrderFail;
                            //        item.Reason = string.Format(StagingBinHaveNotSufficientQtyToReturn,ItemDTO.BinNumber);
                            //        continue;
                            //    }

                            //}
                            //else{

                            //}

                            ItemLocationQTYDAL objLocQtyDAL = new ItemLocationQTYDAL(base.DataBaseName);
                            ItemLocationQTYDTO objItemLocQtyDTO = objLocQtyDAL.GetRecordByBinItem(ItemDTO.GUID, (Int64)binID, RoomID, CompanyID);
                            if (objItemLocQtyDTO == null || objItemLocQtyDTO.Quantity < requestedQuantity)
                            {
                                string BinHaveNotSufficientQtyToReturn = ResourceRead.GetResourceValueByKeyAndFullFilePath("BinHaveNotSufficientQtyToReturn", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                                item.Status = strOrderFail;
                                item.Reason = string.Format(BinHaveNotSufficientQtyToReturn, ItemDTO.BinNumber);
                                continue;
                            }


                        }

                        ordDTO.OrderGUID = ODMDTO.GUID;
                        ordDTO.ItemGUID = ItemDTO.GUID;
                        ordDTO.Bin = binID;
                        ordDTO.BinName = item.Bin;
                        ordDTO.RequestedQuantity = Convert.ToDouble(item.RequestedQty);
                        ordDTO.RequiredDate = ODMDTO.RequiredDate;
                        if (item.ReceivedQty != null)
                            ordDTO.ReceivedQuantity = Convert.ToDouble(item.ReceivedQty);
                        ordDTO.Created = DateTimeUtility.DateTimeNow;
                        ordDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        ordDTO.CreatedBy = UserID;
                        ordDTO.LastUpdatedBy = UserID;
                        ordDTO.Room = RoomID;
                        ordDTO.IsDeleted = false;
                        ordDTO.IsArchived = false;
                        ordDTO.CompanyID = CompanyID;
                        ordDTO.GUID = Guid.NewGuid();
                        ordDTO.ASNNumber = item.ASNNumber;
                        if (item.ApprovedQty != null || TableName.ToLower() == "returnorders")
                            ordDTO.ApprovedQuantity = (TableName.ToLower() == "returnorders") ? Convert.ToDouble(item.RequestedQty) : Convert.ToDouble(item.ApprovedQty);
                        ordDTO.IsEDISent = false;
                        if (item.InTransitQty != null)
                            ordDTO.InTransitQuantity = Convert.ToDouble(item.InTransitQty);
                        ordDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        ordDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        ordDTO.AddedFrom = "Import";
                        ordDTO.EditedFrom = "Import";
                        //if (item.IsCloseItem != string.Empty && item.IsCloseItem != null)
                        //{
                        //    if (item.IsCloseItem == "0")
                        //        ordDTO.IsCloseItem = false;
                        //    else if (item.IsCloseItem == "1")
                        //        ordDTO.IsCloseItem = true;
                        //    else
                        //        ordDTO.IsCloseItem = Convert.ToBoolean(item.IsCloseItem);
                        //}
                        ordDTO.IsCloseItem = item.IsCloseItem;
                        ordDTO.LineNumber = item.LineNumber;
                        ordDTO.ControlNumber = item.ControlNumber;
                        ordDTO.Comment = item.ItemComment;
                        ordDTO.UDF1 = item.LineItemUDF1;
                        ordDTO.UDF2 = item.LineItemUDF2;
                        ordDTO.UDF3 = item.LineItemUDF3;
                        ordDTO.UDF4 = item.LineItemUDF4;
                        ordDTO.UDF5 = item.LineItemUDF5;
                        ordDTO.SupplierName = item.Supplier;

                        SupplierMasterDTO objSuppDTOfordetails = objSuppDAL.GetSupplierByNamePlain(RoomID, CompanyID, false, item.Supplier);
                        if (objSuppDTOfordetails != null)
                        {
                            ordDTO.SupplierID = objSuppDTOfordetails.ID;
                        }
                        string ordDetailUDFReq = string.Empty;
                        CheckUDFIsRequired("OrderDetails", ordDTO.UDF1, ordDTO.UDF2, ordDTO.UDF3, ordDTO.UDF4, ordDTO.UDF5, out ordDetailUDFReq, CompanyID, RoomID, EnterPriceID, UserID);
                        if (!string.IsNullOrEmpty(ordDetailUDFReq))
                        {
                            item.Status = strOrderFail;
                            item.Reason = ordDetailUDFReq;
                            continue;
                        }
                        if (ODMDTO != null && ItemDTO != null && ordDTO != null)
                        {
                            CostUOMMasterDTO costUOM = new CostUOMMasterDAL(EnterPriseDBName).GetCostUOMByID(ItemDTO.CostUOMID.GetValueOrDefault(0));
                            if (costUOM == null)
                                costUOM = new CostUOMMasterDTO() { CostUOMValue = 1 };

                            if (costUOM.CostUOMValue == null || costUOM.CostUOMValue <= 0)
                            {
                                costUOM.CostUOMValue = 1;
                            }

                            #region WI-6215 and Other Relevant order cost related jira

                            if (ItemDTO.Consignment)
                            {
                                ordDTO.ItemCost = ItemDTO.Cost.GetValueOrDefault(0);
                            }
                            else
                            {
                                if (item.OrderCost == null)
                                {
                                    ordDTO.ItemCost = ItemDTO.Cost.GetValueOrDefault(0);
                                }
                                else
                                {
                                    if (objRoomDTO.AllowABIntegration)
                                    {
                                        ProductDetailsDAL objProductDetailsDAL = new ProductDetailsDAL(EnterPriseDBName);
                                        Int64 ABItemMappingID = objProductDetailsDAL.CheckItemAddedFromAB(ItemDTO.SupplierPartNo, ItemDTO.GUID, CompanyID, RoomID);
                                        if (ABItemMappingID > 0)
                                        {
                                            ordDTO.ItemCost = ItemDTO.Cost.GetValueOrDefault(0);
                                        }
                                        else
                                        {
                                            ordDTO.ItemCost = item.OrderCost.GetValueOrDefault(0);
                                        }
                                    }
                                    else
                                    {
                                        ordDTO.ItemCost = item.OrderCost.GetValueOrDefault(0);
                                    }
                                }
                            }

                            if (ordDTO.ItemCostUOMValue == null
                                || ordDTO.ItemCostUOMValue.GetValueOrDefault(0) <= 0)
                            {
                                if (ItemDTO != null)
                                {
                                    if (ItemDTO.CostUOMValue == null
                                        || ItemDTO.CostUOMValue.GetValueOrDefault(0) <= 0)
                                    {
                                        ordDTO.ItemCostUOMValue = 1;
                                    }
                                    else
                                    {
                                        ordDTO.ItemCostUOMValue = ItemDTO.CostUOMValue.GetValueOrDefault(1);
                                    }
                                }
                                else
                                {
                                    ordDTO.ItemCostUOMValue = 1;
                                }
                            }
                            if (ordDTO.ItemMarkup == null
                                || ordDTO.ItemMarkup.GetValueOrDefault(0) == 0)
                            {
                                ordDTO.ItemMarkup = ItemDTO.Markup.GetValueOrDefault(0);
                            }
                            if (ordDTO.ItemMarkup.GetValueOrDefault(0) > 0)
                            {
                                ordDTO.ItemSellPrice = ordDTO.ItemCost.GetValueOrDefault(0) + ((ordDTO.ItemCost.GetValueOrDefault(0) * ordDTO.ItemMarkup.GetValueOrDefault(0)) / 100);
                            }
                            else
                            {
                                ordDTO.ItemSellPrice = ordDTO.ItemCost.GetValueOrDefault(0);
                            }
                            #endregion

                            //ordDTO.OrderLineItemExtendedCost = double.Parse(Convert.ToString((ODMDTO.OrderStatus <= 2 ? (ordDTO.RequestedQuantity.GetValueOrDefault(0) * (ItemDTO.Cost.GetValueOrDefault(0) / costUOM.CostUOMValue.GetValueOrDefault(1)))
                            //                                            : (ordDTO.ApprovedQuantity.GetValueOrDefault(0) * (ItemDTO.Cost.GetValueOrDefault(0) / costUOM.CostUOMValue.GetValueOrDefault(1))))));

                            //ordDTO.OrderLineItemExtendedPrice = double.Parse(Convert.ToString((ODMDTO.OrderStatus <= 2 ? (ordDTO.RequestedQuantity.GetValueOrDefault(0) * (ItemDTO.SellPrice.GetValueOrDefault(0) / costUOM.CostUOMValue.GetValueOrDefault(1)))
                            //                                            : (ordDTO.ApprovedQuantity.GetValueOrDefault(0) * (ItemDTO.SellPrice.GetValueOrDefault(0) / costUOM.CostUOMValue.GetValueOrDefault(1))))));


                            OrderUOMMasterDTO OrderUOM = new OrderUOMMasterDAL(EnterPriseDBName).GetRecord(ItemDTO.OrderUOMID.GetValueOrDefault(0), ItemDTO.Room.GetValueOrDefault(0), ItemDTO.CompanyID.GetValueOrDefault(0), false, false);
                            if (OrderUOM == null)
                                OrderUOM = new OrderUOMMasterDTO() { OrderUOMValue = 1 };

                            if (OrderUOM.OrderUOMValue == null || OrderUOM.OrderUOMValue <= 0)
                            {
                                OrderUOM.OrderUOMValue = 1;
                            }

                            if (ordDTO.RequestedQuantity != null && ordDTO.RequestedQuantity > 0 && ItemDTO.IsAllowOrderCostuom)
                            {
                                ordDTO.RequestedQuantityUOM = ordDTO.RequestedQuantity;
                                ordDTO.RequestedQuantity = ordDTO.RequestedQuantity * OrderUOM.OrderUOMValue;
                            }
                            else
                                ordDTO.RequestedQuantityUOM = ordDTO.RequestedQuantity;

                            if (ordDTO.ApprovedQuantity != null && ordDTO.ApprovedQuantity > 0 && ItemDTO.IsAllowOrderCostuom)
                            {
                                ordDTO.ApprovedQuantityUOM = ordDTO.ApprovedQuantity;
                                ordDTO.ApprovedQuantity = ordDTO.ApprovedQuantity * OrderUOM.OrderUOMValue;
                            }
                            else
                                ordDTO.ApprovedQuantityUOM = ordDTO.ApprovedQuantity;

                            if (ordDTO.ReceivedQuantity != null && ordDTO.ReceivedQuantity > 0 && ItemDTO.IsAllowOrderCostuom)
                            {
                                ordDTO.ReceivedQuantityUOM = ordDTO.ReceivedQuantity;
                                ordDTO.ReceivedQuantity = ordDTO.ReceivedQuantity * OrderUOM.OrderUOMValue;
                            }
                            else
                                ordDTO.ReceivedQuantityUOM = ordDTO.ReceivedQuantity;

                            if (ordDTO.InTransitQuantity != null && ordDTO.InTransitQuantity > 0 && ItemDTO.IsAllowOrderCostuom)
                            {
                                ordDTO.InTransitQuantityUOM = ordDTO.InTransitQuantity;
                                ordDTO.InTransitQuantity = ordDTO.InTransitQuantity * OrderUOM.OrderUOMValue;
                            }
                            else
                                ordDTO.InTransitQuantityUOM = ordDTO.InTransitQuantity;

                            if (ordDTO.ItemCostUOMValue == null
                                    || ordDTO.ItemCostUOMValue.GetValueOrDefault(0) <= 0)
                            {
                                ordDTO.ItemCostUOMValue = 1;
                            }
                            if (ordDTO != null)
                            {
                                if (TableName.ToLower() == "returnorders")
                                {
                                    ordDTO.OrderLineItemExtendedCost = double.Parse(Convert.ToString((ODMDTO.OrderStatus <= 2 ? (ordDTO.RequestedQuantity.GetValueOrDefault(0) * (item.OrderCost) / ordDTO.ItemCostUOMValue.GetValueOrDefault(1))
                                                                            : (ordDTO.ApprovedQuantity.GetValueOrDefault(0) * (item.OrderCost / ordDTO.ItemCostUOMValue.GetValueOrDefault(1))))));
                                }
                                else
                                {
                                    ordDTO.OrderLineItemExtendedCost = double.Parse(Convert.ToString((ODMDTO.OrderStatus <= 2 ? (ordDTO.RequestedQuantity.GetValueOrDefault(0) * (ordDTO.ItemCost.GetValueOrDefault(0) / ordDTO.ItemCostUOMValue.GetValueOrDefault(1)))
                                                                            : (ordDTO.ApprovedQuantity.GetValueOrDefault(0) * (ordDTO.ItemCost.GetValueOrDefault(0) / ordDTO.ItemCostUOMValue.GetValueOrDefault(1))))));
                                }


                                ordDTO.OrderLineItemExtendedPrice = double.Parse(Convert.ToString((ODMDTO.OrderStatus <= 2 ? (ordDTO.RequestedQuantity.GetValueOrDefault(0) * (ordDTO.ItemSellPrice.GetValueOrDefault(0) / ordDTO.ItemCostUOMValue.GetValueOrDefault(1)))
                                                                            : (ordDTO.ApprovedQuantity.GetValueOrDefault(0) * (ordDTO.ItemSellPrice.GetValueOrDefault(0) / ordDTO.ItemCostUOMValue.GetValueOrDefault(1))))));

                            }


                        }

                        ordDTO = objOrderDetailsDAL.Insert(ordDTO, SessionUserId, EnterPriceID);

                        long ReturnVal = ordDTO.ID;
                        if (ReturnVal > 0)
                        {
                            objOrdHeader.Status = strOrderSuccess;
                            if (ODMDTO.OrderType.GetValueOrDefault(0) == (int)OrderType.Order
                                && !ItemDTO.Consignment
                                && (ODMDTO.OrderStatus == (int)OrderStatus.Approved
                                    || ODMDTO.OrderStatus == (int)OrderStatus.Transmitted)
                                )
                            {
                                lstordDetailsForItemCostUpdate.Add(ordDTO);
                            }
                        }
                        else
                            objOrdHeader.Status = strOrderFail;

                        //WI-8417 JKP
                        if (ODMDTO.OrderType.GetValueOrDefault(0) == (int)OrderType.Order && ODMDTO.OrderStatus >= (int)OrderStatus.Approved)
                        {
                            try
                            {
                                objOrderDetailsDAL.UpdateOrderUsedTotalValueBPO(ordDTO.ItemGUID.GetValueOrDefault(), ordDTO.ApprovedQuantity.GetValueOrDefault(0), ordDTO.OrderLineItemExtendedCost.GetValueOrDefault(0), "order");
                            }
                            catch (Exception ex) { }
                        }

                        //Order return Return button functionality
                        if (ODMDTO.OrderType.GetValueOrDefault(0) == (int)OrderType.RuturnOrder && (!failedOrderListNumber.Contains(item.OrderNumber)))
                        {
                            #region "Lot and other type logic"
                            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
                            ReceivedOrderTransferDetailDAL objRecvOrdDetailDAL = new ReceivedOrderTransferDetailDAL(base.DataBaseName);
                            OrderDetailsDAL orderDetailsDAL = new OrderDetailsDAL(base.DataBaseName);
                            ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
                            ItemLocationQTYDTO lstLocDTO = new ItemLocationQTYDAL(base.DataBaseName).GetItemLocationQTY(RoomID, CompanyID, binID, Convert.ToString(ordDTO.ItemGUID)).FirstOrDefault();
                            //List<ItemLocationDetailsDTO> ObjItemLocation = objLocationDAL.GetAllRecords(RoomID, CompanyID, Guid.Parse(objReturnQty.ItemGUID), null, "ID ASC").Where(x => x.BinID == objReturnQty.LocationID).OrderBy("CustomerOwnedQuantity DESC").ToList();
                            List<ItemLocationDetailsDTO> ObjItemLocation = objLocationDAL.GetAllRecordsAvailableAt((Int64)binID, RoomID, CompanyID, ItemDTO.GUID, null, "CustomerOwnedQuantity DESC, ConsignedQuantity DESC, ID ASC").ToList();
                            Double takenQunatity = 0;
                            double CosigneQty = 0;
                            double CustomerQty = 0;
                            foreach (var itemoil in ObjItemLocation)
                            {
                                Double loopCurrentTakenCustomer = 0;
                                Double loopCurrentTakenConsignment = 0;
                                if (takenQunatity == ordDTO.RequestedQuantity)
                                {
                                    break;
                                }
                                itemoil.OrderDetailGUID = ordDTO.GUID;
                                if (ItemDTO.Consignment)
                                {
                                    #region "Consignment Pull"
                                    if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0 && itemoil.ConsignedQuantity.GetValueOrDefault(0) >= (ordDTO.RequestedQuantity - takenQunatity))
                                    {
                                        loopCurrentTakenConsignment = (Double)ordDTO.RequestedQuantity - takenQunatity;
                                        itemoil.ConsignedQuantity = (Double)itemoil.ConsignedQuantity.GetValueOrDefault(0) - (ordDTO.RequestedQuantity - takenQunatity);
                                        takenQunatity += ((Double)ordDTO.RequestedQuantity - takenQunatity);
                                    }
                                    else if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) >= (ordDTO.RequestedQuantity - takenQunatity))
                                    {
                                        loopCurrentTakenCustomer = ((Double)ordDTO.RequestedQuantity - takenQunatity);
                                        itemoil.CustomerOwnedQuantity = (Double)itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - ((Double)ordDTO.RequestedQuantity - takenQunatity);
                                        takenQunatity += ((Double)ordDTO.RequestedQuantity - takenQunatity);
                                    }
                                    else
                                    {

                                        if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                                        {
                                            takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                            loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                            itemoil.CustomerOwnedQuantity = 0;
                                        }

                                        if (itemoil.ConsignedQuantity.GetValueOrDefault(0) >= (ordDTO.RequestedQuantity - takenQunatity))
                                        {
                                            loopCurrentTakenConsignment = ((Double)ordDTO.RequestedQuantity - takenQunatity);
                                            itemoil.ConsignedQuantity = itemoil.ConsignedQuantity.GetValueOrDefault(0) - ((Double)ordDTO.RequestedQuantity - takenQunatity);
                                            takenQunatity += (Double)ordDTO.RequestedQuantity - takenQunatity;
                                        }
                                        else
                                        {
                                            loopCurrentTakenConsignment = itemoil.ConsignedQuantity.GetValueOrDefault(0);
                                            takenQunatity += itemoil.ConsignedQuantity.GetValueOrDefault(0);
                                            itemoil.ConsignedQuantity = 0;
                                        }
                                    }

                                    #endregion
                                }
                                else
                                {
                                    #region "Customer own Pull"
                                    if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) >= (ordDTO.RequestedQuantity - takenQunatity))
                                    {
                                        loopCurrentTakenCustomer = ((Double)ordDTO.RequestedQuantity - takenQunatity);
                                        itemoil.CustomerOwnedQuantity = (Double)itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - ((Double)ordDTO.RequestedQuantity - takenQunatity);
                                        takenQunatity += ((Double)ordDTO.RequestedQuantity - takenQunatity);
                                    }
                                    else
                                    {
                                        loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                        takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                        itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - takenQunatity;

                                    }
                                    #endregion
                                }
                                itemoil.EditedFrom = "Import";
                                itemoil.ReceivedOn = DateTimeUtility.DateTimeNow;

                                objLocationDAL.Edit(itemoil);

                                ReceivedOrderTransferDetailDTO objReceiveOrdTrnfDetailDTO = new ReceivedOrderTransferDetailDTO()
                                {
                                    Action = string.Empty,
                                    BinNumber = itemoil.BinNumber,
                                    Cost = itemoil.Cost,
                                    CreatedByName = string.Empty,
                                    CriticalQuantity = itemoil.CriticalQuantity,
                                    DateCodeTracking = itemoil.DateCodeTracking,
                                    eVMISensorID = itemoil.eVMISensorID,
                                    eVMISensorPort = itemoil.eVMISensorPort,
                                    Expiration = itemoil.Expiration,
                                    ExpirationDate = itemoil.ExpirationDate,
                                    GUID = Guid.NewGuid(),
                                    HistoryID = 0,
                                    ID = 0,
                                    IsCreditPull = true,
                                    ItemLocationDetailGUID = itemoil.GUID,
                                    ItemNumber = itemoil.ItemNumber,
                                    ItemType = itemoil.ItemType,
                                    KitDetailGUID = itemoil.KitDetailGUID,
                                    LotNumber = (!string.IsNullOrWhiteSpace(itemoil.LotNumber)) ? itemoil.LotNumber.Trim() : string.Empty,
                                    LotNumberTracking = itemoil.LotNumberTracking,
                                    MaximumQuantity = itemoil.MaximumQuantity,
                                    MeasurementID = itemoil.MeasurementID,
                                    MinimumQuantity = itemoil.MinimumQuantity,
                                    mode = "ReturnOrder",
                                    Received = itemoil.Received,
                                    ReceivedDate = itemoil.ReceivedDate,
                                    RoomName = itemoil.RoomName,
                                    TransferDetailGUID = itemoil.TransferDetailGUID,
                                    UDF1 = itemoil.UDF1,
                                    UDF2 = itemoil.UDF2,
                                    UDF3 = itemoil.UDF3,
                                    UDF4 = itemoil.UDF4,
                                    UDF5 = itemoil.UDF5,
                                    UDF6 = itemoil.UDF6,
                                    UDF7 = itemoil.UDF7,
                                    UDF8 = itemoil.UDF8,
                                    UDF9 = itemoil.UDF9,
                                    UDF10 = itemoil.UDF10,
                                    BinID = binID,
                                    ItemGUID = ItemDTO.GUID,
                                    OrderDetailGUID = ordDTO.GUID,
                                    UpdatedByName = itemoil.UpdatedByName,
                                    Created = DateTime.Now,
                                    Updated = DateTime.Now,
                                    CreatedBy = UserID,
                                    LastUpdatedBy = UserID,
                                    CompanyID = CompanyID,
                                    Room = RoomID,
                                    IsArchived = false,
                                    IsDeleted = false,
                                    SerialNumberTracking = itemoil.SerialNumberTracking,
                                    SerialNumber = (!string.IsNullOrWhiteSpace(itemoil.SerialNumber)) ? itemoil.SerialNumber.Trim() : string.Empty,
                                    CustomerOwnedQuantity = loopCurrentTakenCustomer,
                                    ConsignedQuantity = loopCurrentTakenConsignment,
                                    AddedFrom = "Web",
                                    EditedFrom = "Web",
                                    ReceivedOn = DateTimeUtility.DateTimeNow,
                                    ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                                };

                                objRecvOrdDetailDAL.Insert(objReceiveOrdTrnfDetailDTO);
                            }
                            #endregion

                            #region "ItemLocation Quantity Deduction"

                            ItemDTO.OnHandQuantity = ItemDTO.OnHandQuantity - ordDTO.RequestedQuantity;
                            if (ItemDTO.Consignment)
                            {
                                //Both's sum we have available.
                                if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) == 0)
                                {
                                    CosigneQty = (Double)ordDTO.RequestedQuantity;
                                    lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity.GetValueOrDefault(0) - ordDTO.RequestedQuantity;
                                    lstLocDTO.Quantity = lstLocDTO.Quantity - (Double)ordDTO.RequestedQuantity;
                                }
                                else if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) >= ordDTO.RequestedQuantity)
                                {
                                    CustomerQty = (Double)ordDTO.RequestedQuantity;
                                    lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) - ordDTO.RequestedQuantity;
                                    lstLocDTO.Quantity = lstLocDTO.Quantity - (Double)ordDTO.RequestedQuantity;
                                }
                                else if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) < ordDTO.RequestedQuantity)
                                {
                                    Double cstqty = (Double)ordDTO.RequestedQuantity - (Double)lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0); // -((Double)objDTO.TempPullQTY - cstqty);
                                    Double consqty = cstqty;// objReturnQty.Quantity - cstqty;

                                    lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity - consqty;
                                    CustomerQty = (Double)lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0); ;
                                    CosigneQty = consqty;
                                    lstLocDTO.CustomerOwnedQuantity = 0;
                                    lstLocDTO.Quantity = lstLocDTO.Quantity - (CustomerQty + CosigneQty);
                                }
                            }
                            else
                            {
                                lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) - (Double)ordDTO.RequestedQuantity;
                                lstLocDTO.Quantity = lstLocDTO.Quantity - (Double)ordDTO.RequestedQuantity;
                                CustomerQty = (Double)ordDTO.RequestedQuantity;
                            }
                            #endregion

                            #region "Saving Location QTY data"
                            ItemDTO.LastUpdatedBy = UserID;
                            ItemDTO.WhatWhereAction = "Return Order";
                            ItemDTO.IsOnlyFromItemUI = false;
                            ItemDTO.EditedFrom = "import";
                            ItemDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objItemDAL.Edit(ItemDTO, SessionUserId, EnterPriceID);
                            ItemLocationQTYDAL objLocQTY = new ItemLocationQTYDAL(base.DataBaseName);
                            List<ItemLocationQTYDTO> lstUpdate = new List<ItemLocationQTYDTO>();
                            lstUpdate.Add(lstLocDTO);
                            objLocQTY.Save(lstUpdate, SessionUserId, EnterPriceID);

                            #endregion

                            #region "Update ItemMaster and Order Detail table"


                            OrderDetailsDTO ordDetailDTO = orderDetailsDAL.GetOrderDetailByGuidPlain(ordDTO.GUID, RoomID, CompanyID);
                            ordDetailDTO.ReceivedQuantity = ordDetailDTO.ReceivedQuantity.GetValueOrDefault(0) + (Double)ordDTO.RequestedQuantity;
                            ordDetailDTO.ReceivedQuantityUOM = ordDetailDTO.ReceivedQuantity;
                            ordDetailDTO.LastUpdatedBy = UserID;
                            ordDetailDTO.EditedFrom = "Import";
                            ordDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            orderDetailsDAL.Edit(ordDetailDTO, SessionUserId, EnterPriceID);
                            orderDetailsDAL.UpdateOrderStatusByReceive(ordDetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty), ordDetailDTO.Room.GetValueOrDefault(0), ordDetailDTO.CompanyID.GetValueOrDefault(0), ordDetailDTO.LastUpdatedBy.GetValueOrDefault(0), false);


                            //response.IsSuccess = true;
                            //string msgQuantityReturnedSuccessfully = ResourceRead.GetResourceValueByKeyAndFullFilePath("QuantityReturnedSuccessfully", orderMasterResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResOrder", CultureCode);
                            //response.Message = msgQuantityReturnedSuccessfully;

                            //WI-8417 JKP
                            try
                            {
                              objOrderDetailsDAL.UpdateOrderUsedTotalValueBPO(ordDetailDTO.ItemGUID.GetValueOrDefault(), ordDetailDTO.ApprovedQuantity.GetValueOrDefault(0), ordDetailDTO.OrderLineItemExtendedCost.GetValueOrDefault(0), "returnorder");
                            }
                            catch (Exception ex) { }
                            #endregion
                        }

                    }
                }



                if (lstordDetailsForItemCostUpdate != null
                   && lstordDetailsForItemCostUpdate.Count > 0)
                {
                    OrderMasterDAL orderMasterDAL = new OrderMasterDAL(base.DataBaseName);
                    DataTable dtOrdDetails = objDAL.GetOrderDetailTableFromList(lstordDetailsForItemCostUpdate);
                    orderMasterDAL.Ord_UpdateItemCostBasedonOrderDetailCost(UserID, "WebImport-OrderApprove", RoomID, CompanyID, dtOrdDetails);
                }
                
            }
            #endregion

            if (TableName.ToLower() == "returnorders" && failedOrderListNumber.Any())
            {
                //Failure item list
                var lstSuccessOrderNumbers = lstOrderMaster.Where(x => x.OrderGUID != null).Select(x => x.OrderNumber).ToList().Distinct();
                OrderMasterDAL orderMasterDAL = new OrderMasterDAL(base.DataBaseName);
                foreach (var item in lstOrderMaster)
                {
                    if (item.Status == strOrderFail && (lstSuccessOrderNumbers.Contains(item.OrderNumber)))
                    {
                        var updatableOrderID = lstOrderMaster.Where(x => x.OrderNumber == item.OrderNumber && x.OrderGUID != null).Select(x => x.OrderGUID).FirstOrDefault();
                        orderMasterDAL.UpdateOrderStatusbyGUID(updatableOrderID, (int)OrderStatus.Transmitted);
                    }
                }
            }

            return lstOrderMaster;
        }

        public bool ValidateOrderItemOnSupplierBlanketPO(Guid OrderGUID, List<OrderDetailsDTO> lstDetails, long RoomID, long CompanyID, long EnterPriceID, long UserID, out string message)
        {
            message = string.Empty;
            int MaxApproveQtyValidation = 0;
            int MaxItemCostValidation = 0;
            OrderMasterDAL orderMasterDAL = new OrderMasterDAL(base.DataBaseName);
            if (lstDetails != null && lstDetails.Count > 0)
            {
                string currentCulture = "en-US";
                if (HttpContext.Current != null)
                {
                    if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                    {
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                        {
                            currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                        }

                    }
                }
                else
                {
                    eTurns.DTO.eTurnsRegionInfo objeTurnsRegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomID, CompanyID, UserID);
                    currentCulture = objeTurnsRegionInfo.CultureName;
                }
                string ResourceOrderMaster = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResOrder", currentCulture, EnterPriceID, CompanyID);

                List<OrderDetailsDTO> lstOfItems = lstDetails.Select(x => new OrderDetailsDTO
                {
                    ItemGUID = x.ItemGUID,
                    ApprovedQuantity = x.ApprovedQuantity
                }).ToList();

                var validationResults = orderMasterDAL.ValidateOrderItemsQtyOnSupplierBlanketPO(OrderGUID, lstOfItems, RoomID, CompanyID);

                foreach (var item in validationResults)
                {
                    if (!item.IsValid)
                    {
                        string ApproveQuantityShouldbelessthanMaxQuantityLimitInSBPO = ResourceRead.GetResourceValueByKeyAndFullFilePath("ApproveQuantityShouldbelessthanMaxQuantityLimitInSBPO", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);

                        //var msg = "<br/> " + string.Format(ResOrder.ApproveQuantityShouldbelessthanMaxQuantityLimitInSBPO, item.ItemNumber);
                        var msg = "<br/> " + string.Format(ApproveQuantityShouldbelessthanMaxQuantityLimitInSBPO, item.ItemNumber);
                        message = (message == "" ? msg : message + msg);
                        MaxApproveQtyValidation++;
                        continue;
                    }
                }

                if (MaxApproveQtyValidation > 0)
                {
                    return false;
                }

                List<OrderDetailsDTO> lstOrderDetailsCost = lstDetails.Select(x => new OrderDetailsDTO
                {
                    ItemGUID = x.ItemGUID,
                    OrderLineItemExtendedCost = x.OrderLineItemExtendedCost
                }).ToList();

                var validationCostResults = orderMasterDAL.ValidateOrderItemsCostOnSupplierBlanketPO(OrderGUID, lstOrderDetailsCost, RoomID, CompanyID);

                foreach (var item in validationCostResults)
                {
                    if (!item.IsValid)
                    {
                        string OrderLineItemExtendedCostShouldbelessthanMaxLimitInSBPO = ResourceRead.GetResourceValueByKeyAndFullFilePath("OrderLineItemExtendedCostShouldbelessthanMaxLimitInSBPO", ResourceOrderMaster, EnterPriceID, CompanyID, RoomID, "ResOrder", currentCulture);
                        //var msg = "<br/> " + string.Format(ResOrder.OrderLineItemExtendedCostShouldbelessthanMaxLimitInSBPO, item.ItemNumber);
                        var msg = "<br/> " + string.Format(OrderLineItemExtendedCostShouldbelessthanMaxLimitInSBPO, item.ItemNumber);
                        message = (message == "" ? msg : message + msg);
                        MaxItemCostValidation++;
                        continue;
                    }
                }

                if (MaxItemCostValidation > 0)
                {
                    return false;
                }
            }
            return true;
        }

        public int GetOrderStatus(string strOrdStatus)
        {
            int Status = 0;
            if (strOrdStatus.ToLower() == OrderStatus.UnSubmitted.ToString().ToLower())
                Status = (int)OrderStatus.UnSubmitted;
            else if (strOrdStatus.ToLower() == OrderStatus.Submitted.ToString().ToLower())
                Status = (int)OrderStatus.Submitted;
            else if (strOrdStatus.ToLower() == OrderStatus.Approved.ToString().ToLower())
                Status = (int)OrderStatus.Approved;
            else if (strOrdStatus.ToLower() == OrderStatus.Transmitted.ToString().ToLower())
                Status = (int)OrderStatus.Transmitted;
            else if (strOrdStatus.ToLower() == OrderStatus.TransmittedIncomplete.ToString().ToLower())
                Status = (int)OrderStatus.TransmittedIncomplete;
            else if (strOrdStatus.ToLower() == OrderStatus.TransmittedPastDue.ToString().ToLower())
                Status = (int)OrderStatus.TransmittedPastDue;
            else if (strOrdStatus.ToLower() == OrderStatus.TransmittedInCompletePastDue.ToString().ToLower())
                Status = (int)OrderStatus.TransmittedInCompletePastDue;
            else if (strOrdStatus.ToLower() == OrderStatus.Closed.ToString().ToLower())
                Status = (int)OrderStatus.Closed;

            return Status;
        }

        public int GetOrderType(string strOrdType)
        {
            int Type = 0;
            if (strOrdType.ToLower() == OrderType.Order.ToString().ToLower())
                Type = (int)OrderType.Order;
            else if (strOrdType.ToLower() == "returnorder")
                Type = (int)OrderType.RuturnOrder;
            return Type;
        }

        public bool GetIsCloseItem(string strIsClose)
        {
            bool IsClose = false;
            if (strIsClose.ToLower() == "true")
                IsClose = true;
            else if (strIsClose.ToLower() == "false")
                IsClose = true;
            else if (strIsClose.ToLower() == "1")
                IsClose = true;
            else if (strIsClose.ToLower() == "0")
                IsClose = true;
            return IsClose;
        }

        public bool CheckItemCostUOM(Int64 CostUOMID, Int64 DefaultReorderQTY)
        {
            bool Flag = false;
            CostUOMMasterDAL objCostUOMDAL = new CostUOMMasterDAL(base.DataBaseName);
            CostUOMMasterDTO CostUOMDTO = objCostUOMDAL.GetCostUOMByID(CostUOMID);
            if (CostUOMDTO != null)
            {
                int intCostUOMValue = (int)CostUOMDTO.CostUOMValue;
                if ((DefaultReorderQTY % intCostUOMValue) == 0)
                {
                    Flag = true;
                }
            }
            return Flag;
        }

        public bool CheckItemCostUOMWithMinQty(Int64 CostUOMID, Int64 MinimumQTY)
        {
            bool Flag = false;
            CostUOMMasterDAL objCostUOMDAL = new CostUOMMasterDAL(base.DataBaseName);
            CostUOMMasterDTO CostUOMDTO = objCostUOMDAL.GetCostUOMByID(CostUOMID);
            if (CostUOMDTO != null)
            {
                int intCostUOMValue = (int)CostUOMDTO.CostUOMValue;
                if (MinimumQTY >= intCostUOMValue)
                {
                    Flag = true;
                }
            }
            return Flag;
        }
        public List<QuoteMasterItemsMain> InsertQuoteImport(List<QuoteMasterItemsMain> LstQuoteItemsMain, Int64 RoomID, Int64 CompanyID, string EnterPriseDBName, Int64 UserID, string RoomDateFormat, CultureInfo RoomCulture, List<UserRoleModuleDetailsDTO> objUserRoleModuleDetailsDTO, long SessionUserId, long EnterpriseID)
        {
            List<QuoteMasterItemsMain> lstQuoteMaster = LstQuoteItemsMain;
            SupplierMasterDAL objSuppDAL = new SupplierMasterDAL(EnterPriseDBName);
            RoomDAL objRoomDAL = new RoomDAL(EnterPriseDBName);
            CommonDAL objCommonDAL = new CommonDAL(EnterPriseDBName);
            QuoteMasterDAL obj = new QuoteMasterDAL(EnterPriseDBName);
            AutoSequenceDAL objAutoSeqDAL = new AutoSequenceDAL(EnterPriseDBName);
            QuoteMasterDAL objQuoteDAL = new QuoteMasterDAL(EnterPriseDBName);
            string strQuoteSuccess = "Success";
            string strQuoteFail = "Fail";
            ItemMasterDAL ItemDAL = new ItemMasterDAL(EnterPriseDBName);
            QuoteDetailDAL objDAL = new QuoteDetailDAL(EnterPriseDBName);
            string columnList = "ID,RoomName,IsAllowQuoteDuplicate";
            RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
            BinMasterDAL binDAL = new BinMasterDAL(EnterPriseDBName);
            List<BinMasterDTO> lstOfOrderLineItemBin = new List<BinMasterDTO>();
            string strBinNumbers = string.Join(",", LstQuoteItemsMain.Where(x => (x.Bin ?? string.Empty) != string.Empty).Select(b => b.Bin).Distinct());
            lstOfOrderLineItemBin = binDAL.GetAllBinMastersByBinList(strBinNumbers, RoomID, CompanyID);

            UDFDAL objUDFDAL = new UDFDAL(EnterPriseDBName);
            IEnumerable<UDFDTO> UDFDataFromDB_Quote = objUDFDAL.GetRequiredUDFsByUDFTableNamePlain("QuoteMaster", RoomID, CompanyID);
            IEnumerable<UDFDTO> UDFDataFromDB_QuoteDtl = objUDFDAL.GetRequiredUDFsByUDFTableNamePlain("QuoteDetails", RoomID, CompanyID);
            Dictionary<Guid, double> itemsQuoteQtyForItemMinMax = new Dictionary<Guid, double>();
            Dictionary<string, double> itemsQuoteQtyForBinMinMax = new Dictionary<string, double>();
            List<SupplierMasterDTO> lstSupplier = new List<SupplierMasterDTO>();
            #region validate
            List<SupplierMasterDTO> lstSupplierDTO = new List<SupplierMasterDTO>();
            string currentCulture = "en-US";
            if (HttpContext.Current != null)
            {
                if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                {
                    if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                    {
                        currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                    }

                }
            }
            else
            {
                eTurns.DTO.eTurnsRegionInfo objeTurnsRegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomID, CompanyID, UserID);
                currentCulture = objeTurnsRegionInfo.CultureName;
            }
            string ResourceSupplierMaster = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResSupplierMaster", currentCulture, EnterpriseID, CompanyID);
            string ResourceQuoteMaster = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResQuoteMaster", currentCulture, EnterpriseID, CompanyID);
            string ResourceOrderMaster = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResOrder", currentCulture, EnterpriseID, CompanyID);
            string ResourceItemMaster = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResItemMaster", currentCulture, EnterpriseID, CompanyID);
            string ResourceBinMaster = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResBin", currentCulture, EnterpriseID, CompanyID);
            if (lstQuoteMaster != null && lstQuoteMaster.Count > 0)
            {
                string strSupplierList = string.Join(",", lstQuoteMaster.Where(x => (x.SupplierName.Trim() ?? string.Empty) != string.Empty).Select(sp => sp.SupplierName.Trim()).Distinct().ToArray());
                lstSupplierDTO = objSuppDAL.GetSupplierByNAMEsPlain(strSupplierList, RoomID, CompanyID);
            }
            foreach (QuoteMasterItemsMain Qut in lstQuoteMaster)
            {
                SupplierMasterDTO SuppDTo = null;

                string SupplierIDS = string.Empty;
                foreach (string Supp in Qut.SupplierName.Split(','))
                {
                    if (!string.IsNullOrWhiteSpace(Supp))
                    {
                        SupplierMasterDTO objSuppDTO = null;
                        if (lstSupplierDTO != null && lstSupplierDTO.Count > 0)
                        {
                            objSuppDTO = lstSupplierDTO.Where(x => x.Room == RoomID && x.CompanyID == CompanyID && (x.SupplierName.Trim() ?? string.Empty).ToLower() == (Supp.Trim() ?? string.Empty).ToLower()).FirstOrDefault();
                        }
                        if (objSuppDTO != null)
                        {
                            if (lstSupplier.Where(t => t.ID == objSuppDTO.ID).Count() == 0)
                            {
                                if (!string.IsNullOrWhiteSpace(SupplierIDS))
                                {
                                    SupplierIDS = SupplierIDS + "," + Convert.ToString(objSuppDTO.ID);
                                }
                                else
                                {
                                    SupplierIDS = Convert.ToString(objSuppDTO.ID);
                                }
                                lstSupplier.Add(objSuppDTO);
                            }
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(SupplierIDS))
                {
                    Qut.QuoteSupplierIdsCSV = SupplierIDS;
                }

                if (lstSupplier == null || lstSupplier.Count == 0)
                {
                    string MsgSupplierNotFound = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgSupplierNotFound", ResourceSupplierMaster, EnterpriseID, CompanyID, RoomID, "ResSupplierMaster", currentCulture);
                    Qut.Status = strQuoteFail;
                    Qut.Reason = MsgSupplierNotFound;
                    continue;
                }
                lstSupplier = new List<SupplierMasterDTO>();
                string quoteNumber = Qut.QuoteNumber;
                long SupplierID = 0;
                try
                {
                    List<string> SupplierIDs = new List<string>();
                    if (!string.IsNullOrWhiteSpace(SupplierIDS))
                    {
                        foreach (string item in SupplierIDS.Split(','))
                        {
                            if (item != "" && (!SupplierIDs.Contains(item)))
                            {
                                SupplierIDs.Add(item);
                            }
                        }
                    }
                    if (SupplierIDs.Count == 1)
                    {
                        SupplierID = Convert.ToInt64(SupplierIDs[0]);
                    }
                    else
                    {
                        SupplierID = 0;
                    }

                }
                catch (Exception ex)
                {
                    SupplierID = 0;
                }
                //if (quoteNumber == string.Empty)
                //{
                //    AutoQuoteNumberGenerate objAutoNumber = objAutoSeqDAL.GetNextQuoteNumber(RoomID, CompanyID, EnterpriseID, SupplierID);
                //    quoteNumber = objAutoNumber.QuoteNumber;
                //    //if (objAutoNumber.IsBlanketPO && (orderNumber == string.Empty || orderNumber == null))
                //    //{
                //    //    IEnumerable<SupplierBlanketPODetailsDTO> objSuppBlnkPOList = null;
                //    //    objSuppBlnkPOList = (from x in objAutoNumber.BlanketPOs
                //    //                         where x != null
                //    //                         && Convert.ToDateTime(x.StartDate.GetValueOrDefault(DateTime.MinValue).ToShortDateString()) <= Convert.ToDateTime(DateTimeUtility.DateTimeNow.ToShortDateString())
                //    //                         && Convert.ToDateTime(x.Enddate.GetValueOrDefault(DateTime.MinValue).ToShortDateString()) >= Convert.ToDateTime(DateTimeUtility.DateTimeNow.ToShortDateString())
                //    //                         select x);
                //    //    if (objSuppBlnkPOList != null && objSuppBlnkPOList.Count() > 0)
                //    //    {
                //    //        orderNumber = objSuppBlnkPOList.FirstOrDefault().BlanketPO;
                //    //    }
                //    //}
                //}

                if (quoteNumber == string.Empty || quoteNumber == null)
                {
                    string MsgQuoteNumberValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgQuoteNumberValidation", ResourceQuoteMaster, EnterpriseID, CompanyID, RoomID, "ResQuoteMaster", currentCulture);
                    Qut.Status = strQuoteFail;
                    Qut.Reason = MsgQuoteNumberValidation;
                    continue;
                }
                //else if (Qut.QuoteNumber == string.Empty && quoteNumber != string.Empty && quoteNumber != null)
                //{
                //    Qut.QuoteNumber = quoteNumber;
                //}

                if (!string.IsNullOrEmpty(quoteNumber) && quoteNumber != null)
                {
                    if (quoteNumber.Length > 22)
                    {
                        string QuoteNumberLengthUpto22Char = ResourceRead.GetResourceValueByKeyAndFullFilePath("QuoteNumberLengthUpto22Char", ResourceQuoteMaster, EnterpriseID, CompanyID, RoomID, "ResQuoteMaster", currentCulture);
                        Qut.Status = strQuoteFail;
                        Qut.Reason = QuoteNumberLengthUpto22Char; //ResQuoteMaster.QuoteNumberLengthUpto22Char
                        continue;
                    }
                }

                // Order Duplicate check
                string strOK = string.Empty;
                if (objRoomDTO.IsAllowQuoteDuplicate != true)
                {
                    if (obj.IsQuoteNumberDuplicateById(quoteNumber, 0, RoomID, CompanyID))
                    {
                        strOK = "duplicate";
                    }
                }
                if (strOK == "duplicate")
                {
                    string MsgDuplicateQuoteNumber = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgDuplicateQuoteNumber", ResourceQuoteMaster, EnterpriseID, CompanyID, RoomID, "ResQuoteMaster", currentCulture);
                    Qut.Status = strQuoteFail;
                    Qut.Reason = MsgDuplicateQuoteNumber;
                    continue;
                }

                if (Qut.RequiredDate != string.Empty && Qut.RequiredDate != null && Qut.RequiredDate != "")
                {
                    DateTime dt;
                    string _expirationDt = Qut.RequiredDate;
                    DateTime.TryParseExact(_expirationDt, RoomDateFormat, RoomCulture, System.Globalization.DateTimeStyles.None, out dt);
                    if (dt != DateTime.MinValue)
                    {
                        //obj.RequiredDate = dt.ToString(SessionHelper.RoomDateFormat);
                    }
                    else
                    {
                        string ResImportMasters = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResImportMasters", currentCulture, EnterpriseID, CompanyID);
                        string DateShouldBeInFormat = ResourceRead.GetResourceValueByKeyAndFullFilePath("DateShouldBeInFormat", ResImportMasters, EnterpriseID, CompanyID, RoomID, "ResImportMasters", currentCulture);
                        Qut.Status = strQuoteFail;
                        Qut.Reason = string.Format(DateShouldBeInFormat, "RequiredDate", RoomDateFormat);
                        continue;
                    }
                }
                else
                {

                    string RequiredDateValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("RequiredDateValidation", ResourceOrderMaster, EnterpriseID, CompanyID, RoomID, "ResOrder", currentCulture);
                    Qut.Status = strQuoteFail;
                    Qut.Reason = RequiredDateValidation;
                    continue;
                }
                // Order Status
                if (Qut.QuoteStatus != string.Empty)
                {
                    int Status = GetOrderStatus(Qut.QuoteStatus);
                    if (Status == 0)
                    {
                        string MsgValidQuoteStatus = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgValidQuoteStatus", ResourceQuoteMaster, EnterpriseID, CompanyID, RoomID, "ResQuoteMaster", currentCulture);
                        Qut.Status = strQuoteFail;
                        Qut.Reason = MsgValidQuoteStatus;
                        continue;
                    }
                }
                else
                {
                    string MsgQuoteStatusBlank = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgQuoteStatusBlank", ResourceQuoteMaster, EnterpriseID, CompanyID, RoomID, "ResQuoteMaster", currentCulture);
                    Qut.Status = strQuoteFail;
                    Qut.Reason = MsgQuoteStatusBlank;
                    continue;
                }



                string QuoteUDFRequier = string.Empty;
                //CheckUDFIsRequired("OrderMaster", Ord.OrderUDF1, Ord.OrderUDF2, Ord.OrderUDF3, Ord.OrderUDF4, Ord.OrderUDF5, out OrdersUDFRequier, CompanyID, RoomID);
                CommonUtilityHelper.CheckUDFIsRequired(UDFDataFromDB_Quote, Qut.QuoteUDF1, Qut.QuoteUDF2, Qut.QuoteUDF3, Qut.QuoteUDF4, Qut.QuoteUDF5, out QuoteUDFRequier, EnterpriseID, CompanyID, RoomID, currentCulture, null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
                if (!string.IsNullOrEmpty(QuoteUDFRequier))
                {
                    Qut.Status = strQuoteFail;
                    Qut.Reason = QuoteUDFRequier;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(Qut.ItemNumber) && Qut.ItemNumber == string.Empty)
                {
                    string ItemNumberEmptyValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("ItemNumberEmptyValidation", ResourceItemMaster, EnterpriseID, CompanyID, RoomID, "ResItemMaster", currentCulture);
                    Qut.Status = strQuoteFail;
                    Qut.Reason = ItemNumberEmptyValidation;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(Qut.Bin) && Qut.Bin == string.Empty)
                {
                    string BinNotAllowEmpty = ResourceRead.GetResourceValueByKeyAndFullFilePath("BinNotAllowEmpty", ResourceBinMaster, EnterpriseID, CompanyID, RoomID, "ResBin", currentCulture);
                    Qut.Status = strQuoteFail;
                    Qut.Reason = BinNotAllowEmpty;
                    continue;
                }
                if (Qut.RequestedQty != null && (double)Qut.RequestedQty <= 0)
                {
                    string MsgRequestedQtyValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgRequestedQtyValidation", ResourceOrderMaster, EnterpriseID, CompanyID, RoomID, "ResOrder", currentCulture);
                    Qut.Status = strQuoteFail;
                    Qut.Reason = MsgRequestedQtyValidation;
                    continue;
                }
                ItemMasterDTO ItemDTO = ItemDAL.GetRecordByItemNumber(Qut.ItemNumber, RoomID, CompanyID);
                if (ItemDTO == null)
                {
                    string MsgItemNotExistValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgItemNotExistValidation", ResourceOrderMaster, EnterpriseID, CompanyID, RoomID, "ResOrder", currentCulture);
                    Qut.Status = strQuoteFail;
                    Qut.Reason = MsgItemNotExistValidation;
                    continue;
                }

                Int64? binID = null;

                bool isStaginHaveDefaultLocation = false;
                binID = binDAL.GetOrInsertBinIDByName(ItemDTO.GUID, Qut.Bin, UserID, RoomID, CompanyID, isStaginHaveDefaultLocation);
                BinMasterDTO BinDTO = binDAL.GetAllRecordsItemBinWise(ItemDTO.GUID, RoomID, CompanyID, (Int64)binID).FirstOrDefault();

                if (ItemDTO.OrderUOMValue == null || ItemDTO.OrderUOMValue <= 0)
                    ItemDTO.OrderUOMValue = 1;

                if (Qut.RequestedQty != null && Qut.RequestedQty > 0)
                {
                    double Modulo = 0;
                    if (BinDTO != null && BinDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                    {
                        if (ItemDTO.IsAllowOrderCostuom)
                            Modulo = ((Qut.RequestedQty ?? 0) * (ItemDTO.OrderUOMValue ?? 1)) % (BinDTO.DefaultReorderQuantity ?? 1);
                        else
                            Modulo = (Qut.RequestedQty ?? 0) % (BinDTO.DefaultReorderQuantity ?? 1);
                        if (Modulo != 0)
                        {
                            string RequestedQtyNotMatchedWithLocationDefaultReOrderQty = ResourceRead.GetResourceValueByKeyAndFullFilePath("RequestedQtyNotMatchedWithLocationDefaultReOrderQty", ResourceOrderMaster, EnterpriseID, CompanyID, RoomID, "ResOrder", currentCulture);
                            Qut.Status = strQuoteFail;
                            Qut.Reason = string.Format(RequestedQtyNotMatchedWithLocationDefaultReOrderQty, BinDTO.BinNumber, BinDTO.DefaultReorderQuantity, ItemDTO.ItemNumber);//string.Format(ResOrder.RequestedQtyNotMatchedWithLocationDefaultReOrderQty, objBinMasterDTO.BinNumber, binDefault, objItemMasterDTO.ItemNumber)
                            continue;
                        }
                    }
                    else if (ItemDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                    {
                        if (ItemDTO.IsAllowOrderCostuom)
                            Modulo = ((Qut.RequestedQty ?? 0) * (ItemDTO.OrderUOMValue ?? 1)) % (ItemDTO.DefaultReorderQuantity ?? 1);
                        else
                            Modulo = (Qut.RequestedQty ?? 0) % (ItemDTO.DefaultReorderQuantity ?? 1);
                        if (Modulo != 0)
                        {
                            string RequestedQtyNotMatchedWithDefaultReOrderQty = ResourceRead.GetResourceValueByKeyAndFullFilePath("RequestedQtyNotMatchedWithDefaultReOrderQty", ResourceOrderMaster, EnterpriseID, CompanyID, RoomID, "ResOrder", currentCulture);
                            Qut.Status = strQuoteFail;
                            Qut.Reason = string.Format(RequestedQtyNotMatchedWithDefaultReOrderQty, ItemDTO.DefaultReorderQuantity, ItemDTO.ItemNumber);//string.Format(ResOrder.RequestedQtyNotMatchedWithDefaultReOrderQty, objItemMasterDTO.DefaultReorderQuantity, objItemMasterDTO.ItemNumber)
                            continue;
                        }
                    }
                }

                if (Qut.ApprovedQty != null && Qut.ApprovedQty > 0)
                {
                    double Modulo = 0;
                    if (BinDTO != null && BinDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                    {
                        if (ItemDTO.IsAllowOrderCostuom)
                            Modulo = ((Qut.ApprovedQty ?? 0) * (ItemDTO.OrderUOMValue ?? 1)) % (BinDTO.DefaultReorderQuantity ?? 1);
                        else
                            Modulo = (Qut.ApprovedQty ?? 0) % (BinDTO.DefaultReorderQuantity ?? 1);
                        if (Modulo != 0)
                        {
                            string ApprovedQtyNotMatchedWithLocationDefaultReOrderQty = ResourceRead.GetResourceValueByKeyAndFullFilePath("ApprovedQtyNotMatchedWithLocationDefaultReOrderQty", ResourceOrderMaster, EnterpriseID, CompanyID, RoomID, "ResOrder", currentCulture);
                            Qut.Status = strQuoteFail;
                            Qut.Reason = string.Format(ApprovedQtyNotMatchedWithLocationDefaultReOrderQty, BinDTO.BinNumber, BinDTO.DefaultReorderQuantity, ItemDTO.ItemNumber);
                            continue;
                        }
                    }
                    else if (ItemDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                    {
                        if (ItemDTO.IsAllowOrderCostuom)
                            Modulo = ((Qut.ApprovedQty ?? 0) * (ItemDTO.OrderUOMValue ?? 1)) % (ItemDTO.DefaultReorderQuantity ?? 1);
                        else
                            Modulo = (Qut.ApprovedQty ?? 0) % (ItemDTO.DefaultReorderQuantity ?? 1);
                        if (Modulo != 0)
                        {
                            string MsgDefaultReorderQuantity = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgDefaultReorderQuantity", ResourceOrderMaster, EnterpriseID, CompanyID, RoomID, "ResOrder", currentCulture);
                            Qut.Status = strQuoteFail;
                            Qut.Reason = string.Format(MsgDefaultReorderQuantity, ItemDTO.DefaultReorderQuantity, ItemDTO.ItemNumber);
                            continue;
                        }
                    }
                }

                string ordDetailUDFReq = string.Empty;
                //CheckUDFIsRequired("OrderDetails", Ord.LineItemUDF1, Ord.LineItemUDF2, Ord.LineItemUDF3, Ord.LineItemUDF4, Ord.LineItemUDF5, out ordDetailUDFReq, CompanyID, RoomID);
                CommonUtilityHelper.CheckUDFIsRequired(UDFDataFromDB_QuoteDtl, Qut.LineItemUDF1, Qut.LineItemUDF2, Qut.LineItemUDF3, Qut.LineItemUDF4, Qut.LineItemUDF5, out ordDetailUDFReq, EnterpriseID, CompanyID, RoomID, currentCulture, null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
                if (!string.IsNullOrEmpty(ordDetailUDFReq))
                {
                    Qut.Status = strQuoteFail;
                    Qut.Reason = ordDetailUDFReq;
                    continue;
                }




            }
            #endregion


            #region Get Unique Quote
            List<QuoteMasterItemsMain> lstQutHeader = (from t in lstQuoteMaster
                                                       where t.Status != strQuoteFail
                                                       select new QuoteMasterItemsMain
                                                       {

                                                           QuoteNumber = t.QuoteNumber,
                                                           RequiredDate = t.RequiredDate,
                                                           QuoteStatus = t.QuoteStatus,
                                                           QuoteSupplierIdsCSV = t.QuoteSupplierIdsCSV,
                                                           QuoteComment = t.QuoteComment,
                                                           CustomerName = t.CustomerName,
                                                           PackSlipNumber = t.PackSlipNumber,
                                                           ShippingTrackNumber = t.ShippingTrackNumber,
                                                           QuoteUDF1 = t.QuoteUDF1,
                                                           QuoteUDF2 = t.QuoteUDF2,
                                                           QuoteUDF3 = t.QuoteUDF3,
                                                           QuoteUDF4 = t.QuoteUDF4,
                                                           QuoteUDF5 = t.QuoteUDF5,
                                                           ShipVia = t.ShipVia,

                                                           ShippingVendor = t.ShippingVendor,
                                                           //AccountNumber = t.AccountNumber,
                                                           SupplierAccount = t.SupplierAccount,
                                                           Reason = t.Reason,
                                                           Status = t.Status
                                                       }).OrderBy(x => x.QuoteNumber).GroupBy(x => new { x.QuoteNumber, x.RequiredDate, x.QuoteStatus, x.QuoteComment, x.CustomerName, x.PackSlipNumber, x.ShippingTrackNumber, x.QuoteUDF1, x.QuoteUDF2, x.QuoteUDF3, x.QuoteUDF4, x.QuoteUDF5, x.ShipVia, x.ShippingVendor, x.SupplierAccount, x.QuoteSupplierIdsCSV }).Select(x => new QuoteMasterItemsMain
                                                       {

                                                           QuoteNumber = x.Key.QuoteNumber,
                                                           RequiredDate = x.Key.RequiredDate,
                                                           QuoteStatus = x.Key.QuoteStatus,
                                                           QuoteSupplierIdsCSV = x.Key.QuoteSupplierIdsCSV,
                                                           QuoteComment = x.Key.QuoteComment,
                                                           CustomerName = x.Key.CustomerName,
                                                           PackSlipNumber = x.Key.PackSlipNumber,
                                                           ShippingTrackNumber = x.Key.ShippingTrackNumber,
                                                           QuoteUDF1 = x.Key.QuoteUDF1,
                                                           QuoteUDF2 = x.Key.QuoteUDF2,
                                                           QuoteUDF3 = x.Key.QuoteUDF3,
                                                           QuoteUDF4 = x.Key.QuoteUDF4,
                                                           QuoteUDF5 = x.Key.QuoteUDF5,
                                                           ShipVia = x.Key.ShipVia,

                                                           ShippingVendor = x.Key.ShippingVendor,
                                                           //AccountNumber = x.Key.AccountNumber,
                                                           SupplierAccount = x.Key.SupplierAccount
                                                       }).ToList();
            #endregion

            #region Create Quote Header

            foreach (QuoteMasterItemsMain objQutHeader in lstQutHeader)
            {



                QuoteMasterDTO objQutDTO = new QuoteMasterDTO();
                string quoteNumber = objQutHeader.QuoteNumber;
                if (quoteNumber == string.Empty)
                {

                    AutoQuoteNumberGenerate objAutoNumber = objAutoSeqDAL.GetNextQuoteNumber(RoomID, CompanyID, EnterpriseID,0, null);
                    quoteNumber = objAutoNumber.QuoteNumber;

                }
                //if (orderNumber != null && (!string.IsNullOrEmpty(orderNumber)))
                //    orderNumber = orderNumber.Length > 22 ? orderNumber.Substring(0, 22) : orderNumber;

                if (!string.IsNullOrEmpty(quoteNumber) && quoteNumber != null)
                {
                    if (quoteNumber.Length > 22)
                    {
                        string QuoteNumberLengthUpto22Char = ResourceRead.GetResourceValueByKeyAndFullFilePath("QuoteNumberLengthUpto22Char", ResourceQuoteMaster, EnterpriseID, CompanyID, RoomID, "ResQuoteMaster", currentCulture);
                        objQutHeader.Status = strQuoteFail;
                        objQutHeader.Reason = QuoteNumberLengthUpto22Char;
                        continue;
                    }
                }

                if (quoteNumber == string.Empty || quoteNumber == null)
                {
                    string MsgQuoteNumberValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgQuoteNumberValidation", ResourceQuoteMaster, EnterpriseID, CompanyID, RoomID, "ResQuoteMaster", currentCulture);
                    objQutHeader.Status = strQuoteFail;
                    objQutHeader.Reason = MsgQuoteNumberValidation;
                    continue;
                }
                else
                    objQutDTO.QuoteNumber = quoteNumber;

                // Order Duplicate check
                string strOK = string.Empty;
                if (objRoomDTO.IsAllowOrderDuplicate != true)
                {
                    if (obj.IsQuoteNumberDuplicateById(quoteNumber, 0, RoomID, CompanyID))
                    {
                        strOK = "duplicate";
                    }
                }
                if (strOK == "duplicate")
                {
                    string MsgQuoteNumberDuplicate = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgQuoteNumberDuplicate", ResourceQuoteMaster, EnterpriseID, CompanyID, RoomID, "ResQuoteMaster", currentCulture);
                    objQutHeader.Status = strQuoteFail;
                    objQutHeader.Reason = MsgQuoteNumberDuplicate;
                    continue;
                }

                int ReleaseNo = 1;
                if (!string.IsNullOrWhiteSpace(quoteNumber))
                {
                    var maximumReleaseNumberByOrderNo = objQuoteDAL.GetMaximumReleaseNoByQuoteNumber(RoomID, CompanyID, quoteNumber);
                    if (maximumReleaseNumberByOrderNo > 0)
                        ReleaseNo = maximumReleaseNumberByOrderNo + 1;
                }

                objQutDTO.ReleaseNumber = ReleaseNo.ToString();
                objQutDTO.Comment = objQutHeader.QuoteComment;
                objQutDTO.RequiredDate = DateTime.ParseExact(objQutHeader.RequiredDate, RoomDateFormat, RoomCulture);
                // Order Status
                if (objQutHeader.QuoteStatus != string.Empty)
                {
                    objQutDTO.QuoteStatus = GetQuoteStatus(objQutHeader.QuoteStatus);
                    if (objQutDTO.QuoteStatus == 0)
                    {
                        string MsgValidQuoteStatus = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgValidQuoteStatus", ResourceQuoteMaster, EnterpriseID, CompanyID, RoomID, "ResQuoteMaster", currentCulture);
                        objQutHeader.Status = strQuoteFail;
                        objQutHeader.Reason = MsgValidQuoteStatus;
                        continue;
                    }
                }
                else
                {
                    string MsgQuoteStatusBlank = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgQuoteStatusBlank", ResourceQuoteMaster, EnterpriseID, CompanyID, RoomID, "ResQuoteMaster", currentCulture);
                    objQutHeader.Status = strQuoteFail;
                    objQutHeader.Reason = MsgQuoteStatusBlank;
                    continue;
                }

                // Ord Status set submit to Approve
                if (objUserRoleModuleDetailsDTO != null && objUserRoleModuleDetailsDTO.Count() > 0 && objQutDTO.QuoteStatus == (int)QuoteStatus.Submitted)
                {
                    if (objUserRoleModuleDetailsDTO.Where(i => i.ModuleID == 149).Any())
                    {
                        if (objUserRoleModuleDetailsDTO.Where(i => i.ModuleID == 149).FirstOrDefault().IsChecked == true)
                        {
                            objQutDTO.QuoteStatus = (int)QuoteStatus.Approved;
                        }
                    }
                }


                //objQutDTO.CustomerID = null;
                //CustomerMasterDTO objCustomerMasterDTO = GetOrInsertCustomerGUIDByName(objQutHeader.CustomerName, UserID, RoomID, CompanyID);
                //if (objCustomerMasterDTO != null)
                //{
                //    objQutDTO.CustomerGUID = objCustomerMasterDTO.GUID;
                //    objQutDTO.CustomerID = objCustomerMasterDTO.ID;
                //    objQutDTO.CustomerAddress = objCustomerMasterDTO.Address;
                //}
                // objQutDTO.PackSlipNumber = objQutHeader.PackSlipNumber;
                // objQutDTO.ShippingTrackNumber = objQutHeader.ShippingTrackNumber;
                objQutDTO.Created = DateTimeUtility.DateTimeNow;
                objQutDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                objQutDTO.CreatedBy = UserID;
                objQutDTO.LastUpdatedBy = UserID;
                objQutDTO.Room = RoomID;
                objQutDTO.IsDeleted = false;
                objQutDTO.IsArchived = false;
                objQutDTO.CompanyID = CompanyID;
                objQutDTO.GUID = Guid.NewGuid();

                objQutDTO.UDF1 = objQutHeader.QuoteUDF1;
                objQutDTO.UDF2 = objQutHeader.QuoteUDF2;
                objQutDTO.UDF3 = objQutHeader.QuoteUDF3;
                objQutDTO.UDF4 = objQutHeader.QuoteUDF4;
                objQutDTO.UDF5 = objQutHeader.QuoteUDF5;
                objQutDTO.QuoteSupplierIdsCSV = objQutHeader.QuoteSupplierIdsCSV;
                string OrdersUDFRequier = string.Empty;
                CheckUDFIsRequired("QuoteMaster", objQutDTO.UDF1, objQutDTO.UDF2, objQutDTO.UDF3, objQutDTO.UDF4, objQutDTO.UDF5, out OrdersUDFRequier, CompanyID, RoomID, EnterpriseID, UserID);
                if (!string.IsNullOrEmpty(OrdersUDFRequier))
                {
                    objQutHeader.Status = strQuoteFail;
                    objQutHeader.Reason = OrdersUDFRequier;
                    continue;
                }

                //objQutDTO.ShipVia = GetOrInsertShipVaiIDByName(objQutHeader.ShipVia, UserID, RoomID, CompanyID);
                objQutDTO.QuoteDate = DateTimeUtility.DateTimeNow;
                // objQutDTO.ShippingVendor = GetOrInsertVendorIDByName(objQutHeader.ShippingVendor, UserID, RoomID, CompanyID);
                //   objOrdDTO.SupplierAccountGuid = GetOrInsertSupplierAccountByName(objOrdHeader.SupplierAccount, UserID, RoomID, CompanyID, objSuppDTO.ID);


                objQutDTO.WhatWhereAction = "Quote Import";

                objQutDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objQutDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                objQutDTO.AddedFrom = "Import";
                objQutDTO.EditedFrom = "Import";
                objQutDTO.IsEDIQuote = false;


                if (objRoomDTO.POAutoSequence.GetValueOrDefault(0) == 0)
                {
                    var countOfOrder = obj.GetCountOfQuoteByQuoteNumber(RoomID, CompanyID, objQutHeader.QuoteNumber);
                    objQutDTO.ReleaseNumber = Convert.ToString(countOfOrder + 1);
                }

                if (objQutDTO.QuoteStatus == (int)QuoteStatus.Submitted)
                {
                    objQutDTO.RequesterID = UserID;
                    objQutHeader.RequesterID = UserID;
                }
                else if (objQutDTO.QuoteStatus == (int)QuoteStatus.Approved
                    || objQutDTO.QuoteStatus == (int)QuoteStatus.Transmitted)
                {
                    objQutDTO.ApproverID = UserID;
                    objQutHeader.ApproverID = UserID;
                    objQutDTO.RequesterID = UserID;
                    objQutHeader.RequesterID = UserID;
                }

                objQutDTO = obj.InsertQuoteMaster(objQutDTO);

                long ReturnVal = objQutDTO.ID;
                if (ReturnVal > 0)
                {
                    objQutHeader.QuoteGUID = objQutDTO.GUID;
                    objQutHeader.Status = strQuoteSuccess;
                }
                else
                {
                    objQutHeader.Status = strQuoteFail;
                }

            }
            #endregion

            #region Update Order created GUID
            // lstOrderMaster, lstOrdHeader
            foreach (var QutHdr in lstQutHeader)
            {
                foreach (var obj1 in (lstQuoteMaster.Where(t => t.QuoteNumber == QutHdr.QuoteNumber && t.RequiredDate == QutHdr.RequiredDate && t.QuoteStatus == QutHdr.QuoteStatus &&
                                                                      t.QuoteComment == QutHdr.QuoteComment && t.CustomerName == QutHdr.CustomerName && t.PackSlipNumber == QutHdr.PackSlipNumber &&
                                                                    t.ShippingTrackNumber == QutHdr.ShippingTrackNumber && t.QuoteUDF1 == QutHdr.QuoteUDF1 && t.QuoteUDF2 == QutHdr.QuoteUDF2 && t.QuoteUDF3 == QutHdr.QuoteUDF3 && t.QuoteUDF4 == QutHdr.QuoteUDF4 && t.QuoteUDF5 == QutHdr.QuoteUDF5 &&
                                                                    t.ShipVia == QutHdr.ShipVia && t.ShippingVendor == QutHdr.ShippingVendor && t.SupplierAccount == QutHdr.SupplierAccount)))
                {
                    if (obj1.Status != strQuoteFail)
                    {
                        obj1.QuoteGUID = QutHdr.QuoteGUID;
                        obj1.Status = QutHdr.Status;
                        obj1.Reason = QutHdr.Reason;
                    }
                }
            }
            #endregion

            #region Insert Order Items
            foreach (QuoteMasterItemsMain objQutHeader in lstQutHeader)
            {
                List<QuoteDetailDTO> lstqutDetailsForItemCostUpdate = new List<QuoteDetailDTO>();
                if (objQutHeader.QuoteGUID != Guid.Empty && objQutHeader.Status == strQuoteSuccess)
                {
                    List<QuoteMasterItemsMain> lstItems = lstQuoteMaster.Where(x => x.QuoteGUID == objQutHeader.QuoteGUID && x.Status != strQuoteFail).ToList();
                    foreach (QuoteMasterItemsMain item in lstItems)
                    {
                        if (string.IsNullOrWhiteSpace(item.ItemNumber) && item.ItemNumber == string.Empty)
                        {
                            string ItemNumberEmptyValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("ItemNumberEmptyValidation", ResourceItemMaster, EnterpriseID, CompanyID, RoomID, "ResItemMaster", currentCulture);
                            item.Status = strQuoteFail;
                            item.Reason = ItemNumberEmptyValidation;
                            continue;
                        }
                        if (string.IsNullOrWhiteSpace(item.Bin) && item.Bin == string.Empty)
                        {
                            string BinNotAllowEmpty = ResourceRead.GetResourceValueByKeyAndFullFilePath("BinNotAllowEmpty", ResourceBinMaster, EnterpriseID, CompanyID, RoomID, "ResBin", currentCulture);
                            item.Status = strQuoteFail;
                            item.Reason = BinNotAllowEmpty;
                            continue;
                        }
                        if (item.RequestedQty != null && (double)item.RequestedQty <= 0)
                        {
                            string MsgRequestedQtyValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgRequestedQtyValidation", ResourceOrderMaster, EnterpriseID, CompanyID, RoomID, "ResOrder", currentCulture);
                            item.Status = strQuoteFail;
                            item.Reason = MsgRequestedQtyValidation;
                            continue;
                        }

                        ItemMasterDTO ItemDTO = ItemDAL.GetRecordByItemNumber(item.ItemNumber, RoomID, CompanyID);
                        if (ItemDTO == null)
                        {
                            string MsgItemNotExistValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgItemNotExistValidation", ResourceOrderMaster, EnterpriseID, CompanyID, RoomID, "ResOrder", currentCulture);
                            item.Status = strQuoteFail;
                            item.Reason = MsgItemNotExistValidation;
                            continue;
                        }

                        if (ItemDTO.OrderUOMValue == null || ItemDTO.OrderUOMValue <= 0)
                            ItemDTO.OrderUOMValue = 1;

                        QuoteDetailDAL objQuoteDetailDAL = new QuoteDetailDAL(EnterPriseDBName);

                        SupplierBlanketPODetailsDAL objSupBlnaPODAL = new SupplierBlanketPODetailsDAL(EnterPriseDBName);

                        MaterialStagingDTO objMSDTO = null;
                        bool isStaginHaveDefaultLocation = false;
                        string binName = string.Empty;
                        Int64? binID = null;
                        QuoteMasterDTO ODMDTO = objQuoteDAL.GetQuoteByGuidPlain(Guid.Parse(item.QuoteGUID.ToString()));
                        QuoteDetailDTO qutDTO = new QuoteDetailDTO();
                        BinMasterDTO BinDTO = null;

                        if (ODMDTO.MaterialStagingGUID != null)
                        {
                            objMSDTO = new MaterialStagingDAL(EnterPriseDBName).GetRecord(ODMDTO.MaterialStagingGUID.Value, RoomID, CompanyID);
                            if (!string.IsNullOrEmpty(objMSDTO.StagingLocationName))
                            {
                                isStaginHaveDefaultLocation = true;
                                binName = objMSDTO.StagingLocationName;
                            }
                        }
                        binID = binDAL.GetOrInsertBinIDByName(ItemDTO.GUID, item.Bin, UserID, RoomID, CompanyID, isStaginHaveDefaultLocation);
                        BinDTO = binDAL.GetAllRecordsItemBinWise(ItemDTO.GUID, RoomID, CompanyID, (Int64)binID).FirstOrDefault();

                        if (item.RequestedQty != null && item.RequestedQty > 0)
                        {
                            double Modulo = 0;
                            if (BinDTO != null && BinDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                            {
                                if (ItemDTO.IsAllowOrderCostuom)
                                    Modulo = ((item.RequestedQty ?? 0) * (ItemDTO.OrderUOMValue ?? 1)) % (BinDTO.DefaultReorderQuantity ?? 1);
                                else
                                    Modulo = (item.RequestedQty ?? 0) % (BinDTO.DefaultReorderQuantity ?? 1);
                                if (Modulo != 0)
                                {
                                    string RequestedQtyNotMatchedWithLocationDefaultReOrderQty = ResourceRead.GetResourceValueByKeyAndFullFilePath("RequestedQtyNotMatchedWithLocationDefaultReOrderQty", ResourceOrderMaster, EnterpriseID, CompanyID, RoomID, "ResOrder", currentCulture);
                                    item.Status = strQuoteFail;
                                    item.Reason = string.Format(RequestedQtyNotMatchedWithLocationDefaultReOrderQty, BinDTO.BinNumber, BinDTO.DefaultReorderQuantity, ItemDTO.ItemNumber);//string.Format(ResOrder.RequestedQtyNotMatchedWithLocationDefaultReOrderQty, objBinMasterDTO.BinNumber, binDefault, objItemMasterDTO.ItemNumber)
                                    continue;
                                }
                            }
                            else if (ItemDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                            {
                                if (ItemDTO.IsAllowOrderCostuom)
                                    Modulo = ((item.RequestedQty ?? 0) * (ItemDTO.OrderUOMValue ?? 1)) % (ItemDTO.DefaultReorderQuantity ?? 1);
                                else
                                    Modulo = (item.RequestedQty ?? 0) % (ItemDTO.DefaultReorderQuantity ?? 1);
                                if (Modulo != 0)
                                {
                                    string RequestedQtyNotMatchedWithDefaultReOrderQty = ResourceRead.GetResourceValueByKeyAndFullFilePath("RequestedQtyNotMatchedWithDefaultReOrderQty", ResourceOrderMaster, EnterpriseID, CompanyID, RoomID, "ResOrder", currentCulture);
                                    item.Status = strQuoteFail;
                                    item.Reason = string.Format(RequestedQtyNotMatchedWithDefaultReOrderQty, ItemDTO.DefaultReorderQuantity, ItemDTO.ItemNumber);//string.Format(ResOrder.RequestedQtyNotMatchedWithDefaultReOrderQty, objItemMasterDTO.DefaultReorderQuantity, objItemMasterDTO.ItemNumber)
                                    continue;
                                }
                            }
                        }

                        if (item.ApprovedQty != null && item.ApprovedQty > 0)
                        {
                            double Modulo = 0;
                            if (BinDTO != null && BinDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                            {
                                if (ItemDTO.IsAllowOrderCostuom)
                                    Modulo = ((item.ApprovedQty ?? 0) * (ItemDTO.OrderUOMValue ?? 1)) % (BinDTO.DefaultReorderQuantity ?? 1);
                                else
                                    Modulo = (item.ApprovedQty ?? 0) % (BinDTO.DefaultReorderQuantity ?? 1);
                                if (Modulo != 0)
                                {
                                    string MsgApprovedQuantityValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgApprovedQuantityValidation", ResourceOrderMaster, EnterpriseID, CompanyID, RoomID, "ResOrder", currentCulture);
                                    item.Status = strQuoteFail;
                                    item.Reason = string.Format(MsgApprovedQuantityValidation, BinDTO.BinNumber, BinDTO.DefaultReorderQuantity, ItemDTO.ItemNumber);
                                    continue;
                                }
                            }
                            else if (ItemDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                            {
                                if (ItemDTO.IsAllowOrderCostuom)
                                    Modulo = ((item.ApprovedQty ?? 0) * (ItemDTO.OrderUOMValue ?? 1)) % (ItemDTO.DefaultReorderQuantity ?? 1);
                                else
                                    Modulo = (item.ApprovedQty ?? 0) % (ItemDTO.DefaultReorderQuantity ?? 1);
                                if (Modulo != 0)
                                {
                                    string MsgDefaultReorderQuantity = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgDefaultReorderQuantity", ResourceOrderMaster, EnterpriseID, CompanyID, RoomID, "ResOrder", currentCulture);
                                    item.Status = strQuoteFail;
                                    item.Reason = string.Format(MsgDefaultReorderQuantity, ItemDTO.DefaultReorderQuantity, ItemDTO.ItemNumber);
                                    continue;
                                }
                            }
                        }

                        qutDTO.QuoteGUID = ODMDTO.GUID;
                        qutDTO.ItemGUID = ItemDTO.GUID;
                        qutDTO.BinID = binID;
                        qutDTO.BinName = item.Bin;
                        qutDTO.RequestedQuantity = Convert.ToDouble(item.RequestedQty);
                        qutDTO.RequiredDate = ODMDTO.RequiredDate;

                        qutDTO.Created = DateTimeUtility.DateTimeNow;
                        qutDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        qutDTO.CreatedBy = UserID;
                        qutDTO.LastUpdatedBy = UserID;
                        qutDTO.Room = RoomID;
                        qutDTO.IsDeleted = false;

                        qutDTO.CompanyID = CompanyID;
                        qutDTO.GUID = Guid.NewGuid();
                        qutDTO.ASNNumber = item.ASNNumber;
                        if (item.ApprovedQty != null)
                            qutDTO.ApprovedQuantity = Convert.ToDouble(item.ApprovedQty);
                        qutDTO.IsEDISent = false;
                        if (item.InTransitQty != null)
                            qutDTO.InTransitQuantity = Convert.ToDouble(item.InTransitQty);
                        qutDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        qutDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        qutDTO.AddedFrom = "Import";
                        qutDTO.EditedFrom = "Import";

                        qutDTO.IsCloseItem = item.IsCloseItem;
                        qutDTO.LineNumber = item.LineNumber;
                        qutDTO.ControlNumber = item.ControlNumber;
                        qutDTO.Comment = item.ItemComment;
                        qutDTO.UDF1 = item.LineItemUDF1;
                        qutDTO.UDF2 = item.LineItemUDF2;
                        qutDTO.UDF3 = item.LineItemUDF3;
                        qutDTO.UDF4 = item.LineItemUDF4;
                        qutDTO.UDF5 = item.LineItemUDF5;

                        string ordDetailUDFReq = string.Empty;
                        CheckUDFIsRequired("QuoteDetails", qutDTO.UDF1, qutDTO.UDF2, qutDTO.UDF3, qutDTO.UDF4, qutDTO.UDF5, out ordDetailUDFReq, CompanyID, RoomID, EnterpriseID, UserID);
                        if (!string.IsNullOrEmpty(ordDetailUDFReq))
                        {
                            item.Status = strQuoteFail;
                            item.Reason = ordDetailUDFReq;
                            continue;
                        }
                        if (ODMDTO != null && ItemDTO != null && qutDTO != null)
                        {
                            CostUOMMasterDTO costUOM = new CostUOMMasterDAL(EnterPriseDBName).GetCostUOMByID(ItemDTO.CostUOMID.GetValueOrDefault(0));
                            if (costUOM == null)
                                costUOM = new CostUOMMasterDTO() { CostUOMValue = 1 };

                            if (costUOM.CostUOMValue == null || costUOM.CostUOMValue <= 0)
                            {
                                costUOM.CostUOMValue = 1;
                            }

                            #region WI-6215 and Other Relevant order cost related jira

                            if (ItemDTO.Consignment)
                            {
                                qutDTO.ItemCost = ItemDTO.Cost.GetValueOrDefault(0);
                            }
                            else
                            {
                                if (item.QuoteCost == null)
                                {
                                    qutDTO.ItemCost = ItemDTO.Cost.GetValueOrDefault(0);
                                }
                                else
                                {
                                    qutDTO.ItemCost = item.QuoteCost.GetValueOrDefault(0);
                                }
                            }

                            if (qutDTO.ItemCostUOMValue == null
                                || qutDTO.ItemCostUOMValue.GetValueOrDefault(0) <= 0)
                            {
                                if (ItemDTO != null)
                                {
                                    if (ItemDTO.CostUOMValue == null
                                        || ItemDTO.CostUOMValue.GetValueOrDefault(0) <= 0)
                                    {
                                        qutDTO.ItemCostUOMValue = 1;
                                    }
                                    else
                                    {
                                        qutDTO.ItemCostUOMValue = ItemDTO.CostUOMValue.GetValueOrDefault(1);
                                    }
                                }
                                else
                                {
                                    qutDTO.ItemCostUOMValue = 1;
                                }
                            }
                            if (qutDTO.ItemMarkup == null
                                || qutDTO.ItemMarkup.GetValueOrDefault(0) == 0)
                            {
                                qutDTO.ItemMarkup = ItemDTO.Markup.GetValueOrDefault(0);
                            }
                            if (qutDTO.ItemMarkup.GetValueOrDefault(0) > 0)
                            {
                                qutDTO.ItemSellPrice = qutDTO.ItemCost.GetValueOrDefault(0) + ((qutDTO.ItemCost.GetValueOrDefault(0) * qutDTO.ItemMarkup.GetValueOrDefault(0)) / 100);
                            }
                            else
                            {
                                qutDTO.ItemSellPrice = qutDTO.ItemCost.GetValueOrDefault(0);
                            }
                            #endregion



                            OrderUOMMasterDTO OrderUOM = new OrderUOMMasterDAL(EnterPriseDBName).GetRecord(ItemDTO.OrderUOMID.GetValueOrDefault(0), ItemDTO.Room.GetValueOrDefault(0), ItemDTO.CompanyID.GetValueOrDefault(0), false, false);
                            if (OrderUOM == null)
                                OrderUOM = new OrderUOMMasterDTO() { OrderUOMValue = 1 };

                            if (OrderUOM.OrderUOMValue == null || OrderUOM.OrderUOMValue <= 0)
                            {
                                OrderUOM.OrderUOMValue = 1;
                            }

                            if (qutDTO.RequestedQuantity != null && qutDTO.RequestedQuantity > 0 && ItemDTO.IsAllowOrderCostuom)
                            {
                                qutDTO.RequestedQuantityUOM = qutDTO.RequestedQuantity;
                                qutDTO.RequestedQuantity = qutDTO.RequestedQuantity * OrderUOM.OrderUOMValue;
                            }
                            else
                                qutDTO.RequestedQuantityUOM = qutDTO.RequestedQuantity;

                            if (qutDTO.ApprovedQuantity != null && qutDTO.ApprovedQuantity > 0 && ItemDTO.IsAllowOrderCostuom)
                            {
                                qutDTO.ApprovedQuantityUOM = qutDTO.ApprovedQuantity;
                                qutDTO.ApprovedQuantity = qutDTO.ApprovedQuantity * OrderUOM.OrderUOMValue;
                            }
                            else
                                qutDTO.ApprovedQuantityUOM = qutDTO.ApprovedQuantity;


                            if (qutDTO.InTransitQuantity != null && qutDTO.InTransitQuantity > 0 && ItemDTO.IsAllowOrderCostuom)
                            {
                                qutDTO.InTransitQuantityUOM = qutDTO.InTransitQuantity;
                                qutDTO.InTransitQuantity = qutDTO.InTransitQuantity * OrderUOM.OrderUOMValue;
                            }
                            else
                                qutDTO.InTransitQuantityUOM = qutDTO.InTransitQuantity;

                            if (qutDTO.ItemCostUOMValue == null
                                    || qutDTO.ItemCostUOMValue.GetValueOrDefault(0) <= 0)
                            {
                                qutDTO.ItemCostUOMValue = 1;
                            }
                            if (qutDTO != null)
                            {
                                qutDTO.QuoteLineItemExtendedCost = double.Parse(Convert.ToString((ODMDTO.QuoteStatus <= 2 ? (qutDTO.RequestedQuantity.GetValueOrDefault(0) * (qutDTO.ItemCost.GetValueOrDefault(0) / qutDTO.ItemCostUOMValue.GetValueOrDefault(1)))
                                                                            : (qutDTO.ApprovedQuantity.GetValueOrDefault(0) * (qutDTO.ItemCost.GetValueOrDefault(0) / qutDTO.ItemCostUOMValue.GetValueOrDefault(1))))));

                                qutDTO.QuoteLineItemExtendedPrice = double.Parse(Convert.ToString((ODMDTO.QuoteStatus <= 2 ? (qutDTO.RequestedQuantity.GetValueOrDefault(0) * (qutDTO.ItemSellPrice.GetValueOrDefault(0) / qutDTO.ItemCostUOMValue.GetValueOrDefault(1)))
                                                                            : (qutDTO.ApprovedQuantity.GetValueOrDefault(0) * (qutDTO.ItemSellPrice.GetValueOrDefault(0) / qutDTO.ItemCostUOMValue.GetValueOrDefault(1))))));

                            }


                        }

                        qutDTO = objDAL.Insert(qutDTO, SessionUserId, EnterpriseID);

                        long ReturnVal = qutDTO.ID;
                        if (ReturnVal > 0)
                        {
                            objQutHeader.Status = strQuoteSuccess;
                            if (!ItemDTO.Consignment
                                && (ODMDTO.QuoteStatus == (int)QuoteStatus.Approved
                                    || ODMDTO.QuoteStatus == (int)QuoteStatus.Transmitted)
                                )
                            {
                                lstqutDetailsForItemCostUpdate.Add(qutDTO);
                            }
                        }
                        else
                            objQutHeader.Status = strQuoteFail;

                    }
                }

                if (lstqutDetailsForItemCostUpdate != null
                   && lstqutDetailsForItemCostUpdate.Count > 0)
                {
                    QuoteMasterDAL quoteMasterDAL = new QuoteMasterDAL(base.DataBaseName);

                    DataTable dtOrdDetails = objDAL.GetQuoteDetailTableFromList(lstqutDetailsForItemCostUpdate);
                    quoteMasterDAL.Qut_UpdateItemCostBasedonQuoteDetailCost(UserID, "WebImport-OrderApprove", RoomID, CompanyID, dtOrdDetails);
                }
            }
            #endregion


            return lstQuoteMaster;
        }
        public int GetQuoteStatus(string strQutStatus)
        {
            int Status = 0;
            if (strQutStatus.ToLower() == QuoteStatus.UnSubmitted.ToString().ToLower())
                Status = (int)QuoteStatus.UnSubmitted;
            else if (strQutStatus.ToLower() == QuoteStatus.Submitted.ToString().ToLower())
                Status = (int)QuoteStatus.Submitted;
            else if (strQutStatus.ToLower() == QuoteStatus.Approved.ToString().ToLower())
                Status = (int)QuoteStatus.Approved;
            else if (strQutStatus.ToLower() == QuoteStatus.Transmitted.ToString().ToLower())
                Status = (int)QuoteStatus.Transmitted;
            else if (strQutStatus.ToLower() == QuoteStatus.TransmittedIncomplete.ToString().ToLower())
                Status = (int)QuoteStatus.TransmittedIncomplete;
            else if (strQutStatus.ToLower() == QuoteStatus.TransmittedPastDue.ToString().ToLower())
                Status = (int)QuoteStatus.TransmittedPastDue;
            else if (strQutStatus.ToLower() == QuoteStatus.TransmittedInCompletePastDue.ToString().ToLower())
                Status = (int)QuoteStatus.TransmittedInCompletePastDue;
            else if (strQutStatus.ToLower() == QuoteStatus.Closed.ToString().ToLower())
                Status = (int)QuoteStatus.Closed;

            return Status;
        }
        public string ValidateToolCategory(RequisitionMasterDTO objReqDTO, string EnterpriseDBName, long EnterPriseID, long CompanyID, long RoomID, long UserID)
        {
            string strMsg = string.Empty;
            RequisitionMasterDAL objRequisitionMasterDAL = new RequisitionMasterDAL(EnterpriseDBName);
            bool IsDuplicateToolExist = false;
            IsDuplicateToolExist = objRequisitionMasterDAL.CheckDuplicateToolRequisition(objReqDTO.GUID.ToString(), objReqDTO.Room.GetValueOrDefault(0), objReqDTO.CompanyID.GetValueOrDefault(0));
            string currentCulture = "en-US";
            if (HttpContext.Current != null)
            {
                if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                {
                    if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                    {
                        currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                    }

                }
            }
            else
            {
                eTurns.DTO.eTurnsRegionInfo objeTurnsRegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomID, CompanyID, UserID);
                currentCulture = objeTurnsRegionInfo.CultureName;
            }
            string ResourceRequisitionMaster = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResRequisitionMaster", currentCulture, EnterPriseID, CompanyID);
            if (IsDuplicateToolExist)
            {
                string MsgDuplicateRequisition = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgDuplicateRequisition", ResourceRequisitionMaster, EnterPriseID, CompanyID, RoomID, "ResRequisitionMaster", currentCulture);
                strMsg = MsgDuplicateRequisition;
                return strMsg;
            }

            RequisitionDetailsDAL objRequisitionDetailsDAL = new RequisitionDetailsDAL(EnterpriseDBName);
            List<RequisitionDetailsDTO> lstReqDetails = objRequisitionDetailsDAL.GetReqLinesByReqGUIDToolAndCategory(objReqDTO.GUID, objReqDTO.Room.GetValueOrDefault(0), objReqDTO.CompanyID.GetValueOrDefault(0)).ToList();
            foreach (RequisitionDetailsDTO objReqDetails in lstReqDetails)
            {
                if (objReqDetails.ItemGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty && objReqDetails.ToolGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty
                                     && objReqDetails.ToolCategoryID.GetValueOrDefault(0) > 0)
                {
                    string MsgToolAgainstCategoryRequisition = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgToolAgainstCategoryRequisition", ResourceRequisitionMaster, EnterPriseID, CompanyID, RoomID, "ResRequisitionMaster", currentCulture);
                    strMsg = MsgToolAgainstCategoryRequisition;
                    break;
                }
            }
            return strMsg;
        }

        //public SiteSettingInfo GetSiteSettingInfo()
        //{
        //    SiteSettingInfo objSiteSetting = new SiteSettingInfo();
        //    string PathFile = System.Configuration.ConfigurationManager.AppSettings["JsonSiteSettingFilePath"];
        //    string strJson = System.IO.File.ReadAllText(PathFile);
        //    objSiteSetting = Newtonsoft.Json.JsonConvert.DeserializeObject<SiteSettingInfo>(strJson);
        //    return objSiteSetting;
        //}

        //public void SaveSiteSettingInfo(SiteSettingInfo objSiteSetting)
        //{
        //    if (objSiteSetting != null)
        //    {
        //        string PathFile = System.Configuration.ConfigurationManager.AppSettings["JsonSiteSettingFilePath"];
        //        string JSONData = Newtonsoft.Json.JsonConvert.SerializeObject(objSiteSetting);
        //        System.IO.File.WriteAllText(PathFile, JSONData);


        //        SiteSettingHelper.LoginWithSelection = objSiteSetting.LoginWithSelection;
        //        SiteSettingHelper.ChartLabelCharSize = objSiteSetting.ChartLabelCharSize;
        //        SiteSettingHelper.DelayRedCount = objSiteSetting.DelayRedCount;
        //        SiteSettingHelper.EnableOptimizations = objSiteSetting.EnableOptimizations;
        //        SiteSettingHelper.ItemDetailsNew = objSiteSetting.ItemDetailsNew;
        //        SiteSettingHelper.AcceptLicence = objSiteSetting.AcceptLicence;
        //        SiteSettingHelper.ILQImportNew = objSiteSetting.ILQImportNew;
        //        SiteSettingHelper.RoleCodeImprove = objSiteSetting.RoleCodeImprove;
        //        SiteSettingHelper.ItemDetailsChange = objSiteSetting.ItemDetailsChange;
        //        SiteSettingHelper.OldResultView = objSiteSetting.OldResultView;
        //        SiteSettingHelper.OldUserMasterUDF = objSiteSetting.OldUserMasterUDF;
        //        SiteSettingHelper.ReleaseNumber = objSiteSetting.ReleaseNumber;
        //        SiteSettingHelper.OrderNumberDateFormat = objSiteSetting.OrderNumberDateFormat;
        //        SiteSettingHelper.NoImage = objSiteSetting.NoImage;
        //        SiteSettingHelper.InventoryPhoto = objSiteSetting.InventoryPhoto;
        //        SiteSettingHelper.AssetPhoto = objSiteSetting.AssetPhoto;
        //        SiteSettingHelper.ToolPhoto = objSiteSetting.ToolPhoto;
        //        SiteSettingHelper.InventoryLink2 = objSiteSetting.InventoryLink2;
        //        SiteSettingHelper.SupplierPhoto = objSiteSetting.SupplierPhoto;
        //        SiteSettingHelper.WorkOrderFilePaths = objSiteSetting.WorkOrderFilePaths;
        //        SiteSettingHelper.AllowedCharacter = objSiteSetting.AllowedCharacter;
        //        SiteSettingHelper.XMLFilePath = objSiteSetting.XMLFilePath;
        //        SiteSettingHelper.ProjectSpendLimitPercentage = objSiteSetting.ProjectSpendLimitPercentage;
        //        SiteSettingHelper.EnableNewCountPopup = objSiteSetting.EnableNewCountPopup;
        //        SiteSettingHelper.ApplyChangePassword = objSiteSetting.ApplyChangePassword;
        //        SiteSettingHelper.NewBaseResourceFileName = objSiteSetting.NewBaseResourceFileName;
        //        SiteSettingHelper.BOMInventoryPhoto = objSiteSetting.BOMInventoryPhoto;
        //        SiteSettingHelper.BOMInventoryLink2 = objSiteSetting.BOMInventoryLink2;
        //        SiteSettingHelper.ItemBinBarcodeNewPage = objSiteSetting.ItemBinBarcodeNewPage;
        //        SiteSettingHelper.UDFMaxLength = objSiteSetting.UDFMaxLength;
        //        SiteSettingHelper.ExceptActionsForNoRoom = objSiteSetting.ExceptActionsForNoRoom;
        //        SiteSettingHelper.AccessQryUserNames = objSiteSetting.AccessQryUserNames;
        //        SiteSettingHelper.AccessQryRoleIds = objSiteSetting.AccessQryRoleIds;
        //        SiteSettingHelper.RunWithReportConnection = objSiteSetting.RunWithReportConnection;
        //        SiteSettingHelper.ApplyNewAuthorization = objSiteSetting.ApplyNewAuthorization;
        //        SiteSettingHelper.ValidateAntiForgeryTokenOnAllPosts = objSiteSetting.ValidateAntiForgeryTokenOnAllPosts;
        //        SiteSettingHelper.MethodsToIgnoreXSRF = objSiteSetting.MethodsToIgnoreXSRF;
        //        SiteSettingHelper.SaveNewAuthorizationError = objSiteSetting.SaveNewAuthorizationError;
        //        SiteSettingHelper.SaveAntiForgeryError = objSiteSetting.SaveAntiForgeryError;
        //        SiteSettingHelper.IsShowGlobalReprotBuilder = objSiteSetting.IsShowGlobalReprotBuilder;
        //        SiteSettingHelper.CustomShortDatePatterns = objSiteSetting.CustomShortDatePatterns;
        //        SiteSettingHelper.MethodsToCheckIsInsert = objSiteSetting.MethodsToCheckIsInsert;
        //        SiteSettingHelper.IsNewNotification = objSiteSetting.IsNewNotification;
        //        SiteSettingHelper.ControllerName = objSiteSetting.ControllerName;
        //        SiteSettingHelper.ModuleName = objSiteSetting.ModuleName;
        //        SiteSettingHelper.ModuleMasterName = objSiteSetting.ModuleMasterName;
        //        SiteSettingHelper.AllowEnterpriseRoomForNN = objSiteSetting.AllowEnterpriseRoomForNN;
        //        SiteSettingHelper.LoadEnterpriseGridOrdering = objSiteSetting.LoadEnterpriseGridOrdering;
        //        SiteSettingHelper.ValidatePhoneNumber = objSiteSetting.ValidatePhoneNumber;
        //        SiteSettingHelper.ProductEnvironment = objSiteSetting.ProductEnvironment;
        //        SiteSettingHelper.OnlyIfRoomAvailable = objSiteSetting.OnlyIfRoomAvailable;
        //        SiteSettingHelper.decimalPointFromConfig = objSiteSetting.decimalPointFromConfig;
        //        SiteSettingHelper.ForWardDatesEnterpriseID = objSiteSetting.ForWardDatesEnterpriseID;
        //        SiteSettingHelper.NewRangeDataFill = objSiteSetting.NewRangeDataFill;
        //        SiteSettingHelper.LoadNarrowDataCount = objSiteSetting.LoadNarrowDataCount;
        //        SiteSettingHelper.DisplayInventoryValueforReplenish = objSiteSetting.DisplayInventoryValueforReplenish;
        //        SiteSettingHelper.RoomReportGrid = objSiteSetting.RoomReportGrid;
        //        SiteSettingHelper.RoomReportGridCompanyID = objSiteSetting.RoomReportGridCompanyID;
        //        SiteSettingHelper.RoomReportGridRoomID = objSiteSetting.RoomReportGridRoomID;
        //        SiteSettingHelper.AllowedPullQtyEdit = objSiteSetting.AllowedPullQtyEdit;
        //        SiteSettingHelper.WorkOrderAllowedFileExtension = objSiteSetting.WorkOrderAllowedFileExtension;
        //        SiteSettingHelper.ResourceRead = objSiteSetting.ResourceRead;
        //        SiteSettingHelper.ResourceSave = objSiteSetting.ResourceSave;

        //    }
        //}

        public List<UserWiseRoomsAccessDetailsDTO> AddNewRoomPermissions(long EnterpriseId, long CompanyID, long RoomId, long UserId, long RoleId, long UserType, string EnterPriseDBName)
        {
            List<UserWiseRoomsAccessDetailsDTO> lstPermissions = new List<UserWiseRoomsAccessDetailsDTO>();

            if (UserType == 1 && RoleId > 0)
            {
                using (eTurnsEntities context = new eTurnsEntities(base.DataBaseETurnsMasterEntityConnectionString))
                //using (var context = new eTurns_MasterEntities(MasterDbConnectionHelper.GeteTurnsEntityFWConnectionString(MasterDbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.EFReadWrite.ToString("F"))))
                {
                    UserRoleDetail objUserRoleDetail = new UserRoleDetail();
                    RoleModuleDetail objRoleModuleDetail = new RoleModuleDetail();
                    foreach (var t in context.ModuleMasters.ToList())
                    {
                        UserRoleDetail oUserRoleDetail = context.UserRoleDetails.Where(x => x.UserID == UserId && x.RoleID == RoleId && x.ModuleID == t.ID && x.EnterpriseID == EnterpriseId && x.CompanyID == CompanyID && x.RoomId == RoomId).FirstOrDefault();
                        if (oUserRoleDetail == null)
                        {
                            objUserRoleDetail = new UserRoleDetail();
                            objUserRoleDetail.CompanyID = CompanyID;
                            objUserRoleDetail.EnterpriseID = EnterpriseId;
                            objUserRoleDetail.GUID = Guid.NewGuid();
                            objUserRoleDetail.ID = 0;
                            objUserRoleDetail.IsChecked = true;
                            objUserRoleDetail.IsDelete = true;
                            objUserRoleDetail.IsInsert = true;
                            objUserRoleDetail.IsUpdate = true;
                            objUserRoleDetail.IsView = true;
                            objUserRoleDetail.ModuleID = t.ID;
                            objUserRoleDetail.ModuleValue = t.Value;
                            objUserRoleDetail.RoleID = RoleId;
                            objUserRoleDetail.RoomId = RoomId;
                            objUserRoleDetail.ShowArchived = true;
                            objUserRoleDetail.ShowDeleted = true;
                            objUserRoleDetail.ShowUDF = true;
                            objUserRoleDetail.UserID = UserId;
                            context.UserRoleDetails.Add(objUserRoleDetail);
                        }

                        RoleModuleDetail objDetails = context.RoleModuleDetails.Where(x => x.RoleId == RoleId && x.ModuleID == t.ID && x.EnterpriseID == EnterpriseId && x.CompanyID == CompanyID && x.RoomId == RoomId).FirstOrDefault();
                        if (objDetails == null)
                        {
                            objRoleModuleDetail = new RoleModuleDetail();
                            objRoleModuleDetail.CompanyID = CompanyID;
                            objRoleModuleDetail.EnterpriseID = EnterpriseId;
                            objRoleModuleDetail.GUID = Guid.NewGuid();
                            objRoleModuleDetail.ID = 0;
                            objRoleModuleDetail.IsChecked = true;
                            objRoleModuleDetail.IsDelete = true;
                            objRoleModuleDetail.IsInsert = true;
                            objRoleModuleDetail.IsUpdate = true;
                            objRoleModuleDetail.IsView = true;
                            objRoleModuleDetail.ModuleID = t.ID;
                            objRoleModuleDetail.ModuleValue = t.Value;
                            objRoleModuleDetail.RoleId = RoleId;
                            objRoleModuleDetail.RoomId = RoomId;
                            objRoleModuleDetail.ShowArchived = true;
                            objRoleModuleDetail.ShowDeleted = true;
                            objRoleModuleDetail.ShowUDF = true;
                            context.RoleModuleDetails.Add(objRoleModuleDetail);
                        }
                    }
                    UserRoomAccess objUserRoomAccess = context.UserRoomAccesses.FirstOrDefault(t => t.EnterpriseId == EnterpriseId && t.CompanyId == CompanyID && (t.RoomId == RoomId || t.RoomId == 0) && t.UserId == UserId);
                    if (objUserRoomAccess == null)
                    {
                        objUserRoomAccess = new UserRoomAccess();
                        objUserRoomAccess.CompanyId = CompanyID;
                        objUserRoomAccess.EnterpriseId = EnterpriseId;
                        objUserRoomAccess.ID = 0;
                        objUserRoomAccess.RoleId = RoleId;
                        objUserRoomAccess.RoomId = RoomId;
                        objUserRoomAccess.UserId = UserId;
                        context.UserRoomAccesses.Add(objUserRoomAccess);
                    }
                    else
                    {
                        objUserRoomAccess.RoomId = RoomId;
                    }
                    RoleRoomAccess objRoleRoomAccess = context.RoleRoomAccesses.FirstOrDefault(t => t.EnterpriseId == EnterpriseId && t.CompanyId == CompanyID && (t.RoomId == RoomId || t.RoomId == 0) && t.RoleId == RoleId);
                    if (objRoleRoomAccess == null)
                    {
                        objRoleRoomAccess = new RoleRoomAccess();
                        objRoleRoomAccess.CompanyId = CompanyID;
                        objRoleRoomAccess.EnterpriseId = EnterpriseId;
                        objRoleRoomAccess.ID = 0;
                        objRoleRoomAccess.RoleId = RoleId;
                        objRoleRoomAccess.RoomId = RoomId;
                        context.RoleRoomAccesses.Add(objRoleRoomAccess);
                    }
                    else
                    {
                        objRoleRoomAccess.RoomId = RoomId;
                    }
                    context.SaveChanges();
                }

                //if (UserId == UserID)
                //{
                //eTurnsMaster.DAL.UserMasterDAL objUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL();
                List<UserRoleModuleDetailsDTO> lstUserRoleModuleDetailsDTO = GetUserRoleModuleDetailsRecord(UserId, RoleId, UserType);
                string strRoomList = string.Empty;
                lstPermissions = ConvertUserPermissions(lstUserRoleModuleDetailsDTO, RoleId, ref strRoomList);
                //}
            }
            else if ((UserType == 2 && RoleId > 0) || UserType == 3)
            {
                using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
                {
                    UserRoleDetail objUserRoleDetail = new UserRoleDetail();
                    RoleModuleDetail objRoleModuleDetail = new RoleModuleDetail();
                    foreach (var t in context.ModuleMasters.ToList())
                    {
                        UserRoleDetail oUserRoleDetail = context.UserRoleDetails.Where(x => x.UserID == UserId && x.RoleID == RoleId && x.ModuleID == t.ID && x.EnterpriseID == EnterpriseId && x.CompanyID == CompanyID && x.RoomId == RoomId).FirstOrDefault();
                        if (oUserRoleDetail == null)
                        {
                            objUserRoleDetail = new UserRoleDetail();
                            objUserRoleDetail.CompanyID = CompanyID;
                            objUserRoleDetail.EnterpriseID = EnterpriseId;
                            objUserRoleDetail.GUID = Guid.NewGuid();
                            objUserRoleDetail.ID = 0;
                            objUserRoleDetail.IsChecked = true;
                            objUserRoleDetail.IsDelete = true;
                            objUserRoleDetail.IsInsert = true;
                            objUserRoleDetail.IsUpdate = true;
                            objUserRoleDetail.IsView = true;
                            objUserRoleDetail.ModuleID = t.ID;
                            objUserRoleDetail.ModuleValue = t.Value;
                            objUserRoleDetail.RoleID = RoleId;
                            objUserRoleDetail.RoomId = RoomId;
                            objUserRoleDetail.ShowArchived = true;
                            objUserRoleDetail.ShowDeleted = true;
                            objUserRoleDetail.ShowUDF = true;
                            objUserRoleDetail.UserID = UserId;
                            if (t.ID == 117 ||
                                t.ID == 118)
                            {
                            }
                            else
                                context.UserRoleDetails.Add(objUserRoleDetail);
                        }

                        eTurns.DAL.RoleModuleDetail objDetails = context.RoleModuleDetails.Where(x => x.RoleId == RoleId && x.ModuleID == t.ID && x.EnterpriseID == EnterpriseId && x.CompanyID == CompanyID && x.RoomId == RoomId).FirstOrDefault();
                        if (objDetails == null)
                        {
                            objRoleModuleDetail = new eTurns.DAL.RoleModuleDetail();
                            objRoleModuleDetail.CompanyID = CompanyID;
                            objRoleModuleDetail.EnterpriseID = EnterpriseId;
                            objRoleModuleDetail.GUID = Guid.NewGuid();
                            objRoleModuleDetail.ID = 0;
                            objRoleModuleDetail.IsChecked = true;
                            objRoleModuleDetail.IsDelete = true;
                            objRoleModuleDetail.IsInsert = true;
                            objRoleModuleDetail.IsUpdate = true;
                            objRoleModuleDetail.IsView = true;
                            objRoleModuleDetail.ModuleID = t.ID;
                            objRoleModuleDetail.ModuleValue = t.Value;
                            objRoleModuleDetail.RoleId = RoleId;
                            objRoleModuleDetail.RoomId = RoomId;
                            objRoleModuleDetail.ShowArchived = true;
                            objRoleModuleDetail.ShowDeleted = true;
                            objRoleModuleDetail.ShowUDF = true;
                            if (t.ID == 117 ||
                                t.ID == 118)
                            {
                            }
                            else
                                context.RoleModuleDetails.Add(objRoleModuleDetail);
                        }
                    }

                    eTurns.DAL.UserRoomAccess objUserRoomAccess = context.UserRoomAccesses.FirstOrDefault(t => t.EnterpriseId == EnterpriseId && t.CompanyId == CompanyID && (t.RoomId == RoomId || t.RoomId == 0) && t.UserId == UserId);
                    if (objUserRoomAccess == null)
                    {
                        objUserRoomAccess = new eTurns.DAL.UserRoomAccess();
                        objUserRoomAccess.CompanyId = CompanyID;
                        objUserRoomAccess.EnterpriseId = EnterpriseId;
                        objUserRoomAccess.ID = 0;
                        objUserRoomAccess.RoleId = RoleId;
                        objUserRoomAccess.RoomId = RoomId;
                        objUserRoomAccess.UserId = UserId;
                        context.UserRoomAccesses.Add(objUserRoomAccess);
                    }
                    else
                    {
                        objUserRoomAccess.RoomId = RoomId;
                    }
                    eTurns.DAL.RoleRoomAccess objRoleRoomAccess = context.RoleRoomAccesses.FirstOrDefault(t => t.EnterpriseId == EnterpriseId && t.CompanyId == CompanyID && (t.RoomId == RoomId || t.RoomId == 0) && t.RoleId == RoleId);
                    if (objRoleRoomAccess == null)
                    {
                        objRoleRoomAccess = new eTurns.DAL.RoleRoomAccess();
                        objRoleRoomAccess.CompanyId = CompanyID;
                        objRoleRoomAccess.EnterpriseId = EnterpriseId;
                        objRoleRoomAccess.ID = 0;
                        objRoleRoomAccess.RoleId = RoleId;
                        objRoleRoomAccess.RoomId = RoomId;
                        context.RoleRoomAccesses.Add(objRoleRoomAccess);
                    }
                    else
                    {
                        objRoleRoomAccess.RoomId = RoomId;
                    }
                    context.SaveChanges();
                }
                eTurns.DAL.UserMasterDAL objUserMasterDAL = new eTurns.DAL.UserMasterDAL(EnterPriseDBName);
                List<UserRoleModuleDetailsDTO> lstUserRoleModuleDetailsDTO = objUserMasterDAL.GetUserRoleModuleDetailsByUserIdAndRoleId(UserId, RoleId);
                string strRoomList = string.Empty;
                lstPermissions = objUserMasterDAL.ConvertUserPermissions(lstUserRoleModuleDetailsDTO, RoleId, ref strRoomList);
            }
            return lstPermissions;
        }

        /// <summary>
        /// Get Particullar Record from the User Master by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public List<UserRoleModuleDetailsDTO> GetUserRoleModuleDetailsRecord(long UserID, long RoleID, long UserType)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UserID", UserID), new SqlParameter("@RoleID", RoleID), new SqlParameter("@UserType", UserType) };
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseETurnsMasterEntityConnectionString))
            {
                return context.Database.SqlQuery<UserRoleModuleDetailsDTO>("exec GetUserRoleModuleDetailsRecord @UserID,@RoleID,@UserType", params1).ToList();
            }
        }

        public List<UserWiseRoomsAccessDetailsDTO> ConvertUserPermissions(List<UserRoleModuleDetailsDTO> objData, Int64 RoleID, ref string RoomLists)
        {
            List<EnterpriseDTO> lstEnterprises = new List<EnterpriseDTO>();
            List<UserWiseRoomsAccessDetailsDTO> objRooms = new List<UserWiseRoomsAccessDetailsDTO>();

            if (RoleID == -1)
            {
                UserWiseRoomsAccessDetailsDTO obj = new UserWiseRoomsAccessDetailsDTO();
                obj.PermissionList = objData;
                obj.RoomID = 0;
                obj.CompanyId = 0;
                obj.EnterpriseId = 0;
                objRooms.Add(obj);
            }
            else
            {
                UserWiseRoomsAccessDetailsDTO objRoleRooms;
                List<UserRoleModuleDetailsDTO> cps;
                var objTempPermissionList = objData.GroupBy(element => new { element.EnteriseId, element.CompanyId, element.RoomId, element.EnterpriseName, element.CompanyName, element.RoomName, element.IsEnterpriseActive, element.IsCompanyActive, element.IsRoomActive }).OrderBy(g => g.Key.EnteriseId);
                RoomLists = string.Join(",", objTempPermissionList.Select(t => (t.Key.EnteriseId + "_" + t.Key.CompanyId + "_" + t.Key.RoomId + "_" + t.Key.RoomName)).ToArray());

                foreach (var grpData in objTempPermissionList)
                {
                    objRoleRooms = new UserWiseRoomsAccessDetailsDTO();
                    objRoleRooms.EnterpriseId = grpData.Key.EnteriseId;
                    objRoleRooms.IsEnterpriseActive = grpData.Key.IsEnterpriseActive;
                    objRoleRooms.CompanyId = grpData.Key.CompanyId;
                    objRoleRooms.RoomID = grpData.Key.RoomId;
                    objRoleRooms.RoleID = RoleID;
                    cps = new List<UserRoleModuleDetailsDTO>();
                    cps = objData.Where(t => t.RoomId == grpData.Key.RoomId && t.CompanyId == grpData.Key.CompanyId && t.EnteriseId == grpData.Key.EnteriseId).ToList();

                    if (cps != null)
                    {
                        objRoleRooms.PermissionList = cps;
                    }
                    else
                    {
                        cps = new List<UserRoleModuleDetailsDTO>();
                    }

                    if (objRoleRooms.RoomID > 0)
                    {
                        RolePermissionInfo objRolePermissionInfo = new RolePermissionInfo();
                        objRolePermissionInfo.EnterPriseId = grpData.Key.EnteriseId;
                        objRolePermissionInfo.CompanyId = grpData.Key.CompanyId;
                        objRolePermissionInfo.RoomId = grpData.Key.RoomId;
                        objRoleRooms.RoomName = grpData.Key.RoomName;
                        objRoleRooms.IsRoomActive = grpData.Key.IsRoomActive;
                        objRoleRooms.CompanyName = grpData.Key.CompanyName;
                        objRoleRooms.IsCompanyActive = grpData.Key.IsCompanyActive;
                        objRoleRooms.EnterpriseName = grpData.Key.EnterpriseName;
                    }
                    objRooms.Add(objRoleRooms);
                }
            }

            return objRooms;
        }

        public void InsertSSOAuthResp(string RespData)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RespData", RespData ?? (object)DBNull.Value) };
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("EXEC [" + DbConnectionHelper.GeteTurnsLoggingDBName() + "].[dbo].[InsertSSOAuthResp] @RespData", params1);
            }
        }
        public void InsertZohoReponseToeTurns(string RespData, string AllHeaders, bool IsValid)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RespData", RespData ?? (object)DBNull.Value),
                                               new SqlParameter("@AllHeaders", AllHeaders ?? (object)DBNull.Value),
                                               new SqlParameter("@IsValid", IsValid)};
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("EXEC [" + DbConnectionHelper.GeteTurnsLoggingDBName() + "].[dbo].[InsertZohoReponseToeTurns] @RespData,@AllHeaders,@IsValid", params1);
            }
        }

        public void InsertZohoInvoiceResponseToeTurns(string RespData, string AllHeaders, bool IsValid)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RespData", RespData ?? (object)DBNull.Value),
                                               new SqlParameter("@AllHeaders", AllHeaders ?? (object)DBNull.Value),
                                               new SqlParameter("@IsValid", IsValid)};
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("EXEC [" + DbConnectionHelper.GeteTurnsLoggingDBName() + "].[dbo].[InsertZohoInvoiceResponseToeTurns] @RespData,@AllHeaders,@IsValid", params1);
            }
        }

        public bool HasSpecialDomain(string CurrentDomainURL, long EnterpriseID)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseETurnsMasterEntityConnectionString))
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@CurrentDomainName", CurrentDomainURL),
                                  new SqlParameter("@EnterpriseID", EnterpriseID) };
                    string Result = context.Database.SqlQuery<string>("EXEC CheckForDomain @CurrentDomainName,@EnterpriseID", params1).FirstOrDefault();
                    if (Result == "yes")
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public void InsertOktaSSOAuthResp(string enterprise, string eTurnsUserEmailId, string queryString, string AllClaims, string RespData)
        {
            var params1 = new SqlParameter[] {
                new SqlParameter("@enterprise", enterprise  ?? (object)DBNull.Value),
                new SqlParameter("@eTurnsUserEmailId", eTurnsUserEmailId ?? (object)DBNull.Value),
                new SqlParameter("@QueryString", queryString  ?? (object)DBNull.Value),
                new SqlParameter("@AllClaims", AllClaims ?? (object)DBNull.Value),
                new SqlParameter("@RespData", RespData ?? (object)DBNull.Value)
            };
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("EXEC [" + DbConnectionHelper.GeteTurnsLoggingDBName() + "].[dbo].[InsertOktaSSOAuthResp] @enterprise, @eTurnsUserEmailId,@QueryString,@AllClaims,@RespData", params1);
            }
        }
    }
}