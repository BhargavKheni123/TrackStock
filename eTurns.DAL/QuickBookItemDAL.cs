using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTurns.DTO;
using System.Data;
using Dynamite;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Collections;
using System.Data.SqlClient;
using eTurns.DTO.Resources;
using System.Web;
using System.Transactions;

namespace eTurns.DAL
{
    public class QuickBookItemDAL : eTurnsBaseDAL
    {
        public QuickBookItemDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        public List<QuickBookItemDTO> GetQuickBookItemByCompanyRoomID(Int64 CompanyID, Int64 RoomID, bool? IsProcess)
        {
            var params1 = new SqlParameter[] {
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@IsProcess", IsProcess ?? (object)DBNull.Value),
            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<QuickBookItemDTO> obj = (from u in context.Database.SqlQuery<QuickBookItemDTO>("Exec [GetQuickBookItemByCompanyRoomID] @CompanyID,@RoomID,@IsProcess", params1)
                                              select new QuickBookItemDTO
                                              {
                                                  ID = u.ID,
                                                  GUID = u.GUID,
                                                  ItemGUID = u.ItemGUID,
                                                  IsProcess = u.IsProcess,
                                                  IsSuccess = u.IsSuccess,
                                                  ErrorDescription = u.ErrorDescription,
                                                  CompanyID = u.CompanyID,
                                                  RoomID = u.RoomID,
                                                  Action = u.Action,
                                                  IsDeleted = u.IsDeleted,
                                                  IsArchived = u.IsArchived,
                                                  Created = u.Created,
                                                  LastUpdated = u.LastUpdated,
                                                  CreatedBy = u.CreatedBy,
                                                  LastUpdatedBy = u.LastUpdatedBy,
                                                  AddedFrom = u.AddedFrom,
                                                  EditedFrom = u.EditedFrom,
                                                  ItemNumber = u.ItemNumber,
                                                  Description = u.Description,
                                                  LongDescription = u.LongDescription,
                                                  CategoryID = u.CategoryID,
                                                  CategoryName = u.CategoryName,
                                                  Cost = u.Cost,
                                                  OnHandQuantity = u.OnHandQuantity,
                                                  QuickBookItemID = u.QuickBookItemID,
                                                  WhatWhereAction = u.WhatWhereAction,
                                                  ImagePath = u.ImagePath,
                                                  ItemImageExternalURL = u.ItemImageExternalURL,
                                                  ImageType = u.ImageType,
                                                  ItemMasterID = u.ItemMasterID,
                                                  ItemUniqueNumber = u.ItemUniqueNumber,
                                                  SellPrice = u.SellPrice,
                                                  MinimumQuantity = u.MinimumQuantity,
                                                  MaximumQuantity = u.MaximumQuantity,
                                                  CriticalQuantity = u.CriticalQuantity,
                                                  OnOrderQuantity = u.OnOrderQuantity,
                                                  Taxable = u.Taxable
                                              }).ToList();

                return obj;
            }

        }

        public List<QuickBookItemDTO> UpdateQuickBookItem(QuickBookItemDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@GUID", objDTO.GUID),
                    new SqlParameter("@ItemGUID", objDTO.ItemGUID),
                    new SqlParameter("@IsProcess", objDTO.IsProcess ?? (object)DBNull.Value),
                    new SqlParameter("@IsSuccess", objDTO.IsSuccess ?? (object)DBNull.Value),
                    new SqlParameter("@ErrorDescription", objDTO.ErrorDescription ?? (object)DBNull.Value),
                    new SqlParameter("@CompanyID", objDTO.CompanyID),
                    new SqlParameter("@RoomID", objDTO.RoomID),
                    new SqlParameter("@IsDeleted", objDTO.IsDeleted ?? (object)DBNull.Value),
                    new SqlParameter("@IsArchived", objDTO.IsArchived ?? (object)DBNull.Value),
                    new SqlParameter("@LastUpdated", objDTO.LastUpdated ?? (object)DBNull.Value),
                    new SqlParameter("@LastUpdatedBy", objDTO.LastUpdatedBy),
                    new SqlParameter("@EditedFrom", objDTO.EditedFrom),
                    new SqlParameter("@QuickBookItemID", objDTO.QuickBookItemID ?? (object)DBNull.Value),
                    new SqlParameter("@ItemFullyQualifiedName", objDTO.ItemFullyQualifiedName ?? (object)DBNull.Value),
                    new SqlParameter("@QBJSON", objDTO.QBJSON ?? (object)DBNull.Value)};

                return context.Database.SqlQuery<QuickBookItemDTO>("Exec [UpdateQuickBookItem] @GUID,@ItemGUID,@IsProcess,@IsSuccess,@ErrorDescription,@CompanyID,@RoomID,@IsDeleted,@IsArchived,@LastUpdated,@LastUpdatedBy,@EditedFrom,@QuickBookItemID,@ItemFullyQualifiedName,@QBJSON", params1).ToList();
            }

        }

        public List<QuickBookItemDTO> InsertItemQuickBookSetup(Int64 EnterpriseID, Int64 CompanyID, Int64 RoomID, Int64 UserID)
        {
            // Insert Item when new room setup to Quickbook
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@EnterpriseID", EnterpriseID),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@UserID", UserID) };

                return context.Database.SqlQuery<QuickBookItemDTO>("Exec [InsertItemQuickBookSetup] @EnterpriseID,@CompanyID,@RoomID,@UserID", params1).ToList();
            }

        }

        public List<QuickBookItemDTO> InsertQuickBookItem(Guid ItemGUID, Int64 EnterpriseID, Int64 CompanyID, Int64 RoomID, string Action, bool? IsDeleted, Int64 UserID, string AddedFrom, Int64? OnHandQuantity, string WhatWhereAction)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@ItemGUID", ItemGUID),
                    new SqlParameter("@EnterpriseID", EnterpriseID),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@Action", Action),
                    new SqlParameter("@IsDeleted", IsDeleted ?? (object)DBNull.Value),
                    new SqlParameter("@UserID", UserID),
                    new SqlParameter("@AddedFrom", AddedFrom),
                    new SqlParameter("@OnHandQuantity", OnHandQuantity ?? (object)DBNull.Value),
                    new SqlParameter("@WhatWhereAction", WhatWhereAction ?? (object)DBNull.Value) };

                return context.Database.SqlQuery<QuickBookItemDTO>("Exec [InsertQuickBookItem] @ItemGUID,@EnterpriseID,@CompanyID,@RoomID,@Action,@IsDeleted,@UserID,@AddedFrom,@OnHandQuantity,@WhatWhereAction", params1).ToList();
            }

        }

    }
}
