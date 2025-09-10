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
    public class QuickBookTokenDetailDAL : eTurnsBaseDAL
    {
        public QuickBookTokenDetailDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        public List<QuickBookTokenDetailDTO> GetAllTokenDetail(bool IsForRefreshToken)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@IsForRefreshToken", IsForRefreshToken) };
                return context.Database.SqlQuery<QuickBookTokenDetailDTO>("exec [GetAllTokenDetail] @IsForRefreshToken", params1).ToList();
            }
        }

        public QuickBookTokenDetailDTO GetTokenDetail(Int64 EnterpriseID, Int64 CompanyID, Int64 RoomID)
        {
            var params1 = new SqlParameter[] {
                    new SqlParameter("@EnterpriseID", EnterpriseID),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@RoomID", RoomID) };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<QuickBookTokenDetailDTO>("exec [GetTokenDetail] @EnterpriseID,@CompanyID,@RoomID", params1).FirstOrDefault();
            }
        }

        public List<QuickBookTokenDetailDTO> InsertTokenDetail(QuickBookTokenDetailDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@EnterpriseID", objDTO.EnterpriseID),
                    new SqlParameter("@CompanyID", objDTO.CompanyID),
                    new SqlParameter("@RoomID", objDTO.RoomID),
                    new SqlParameter("@AccessToken", objDTO.AccessToken ?? (object)DBNull.Value),
                    new SqlParameter("@AccessTokenExpireDate", objDTO.AccessTokenExpireDate ?? (object)DBNull.Value),
                    new SqlParameter("@RefreshToken", objDTO.RefreshToken ?? (object)DBNull.Value),
                    new SqlParameter("@RefreshTokenExpireDate", objDTO.RefreshTokenExpireDate ?? (object)DBNull.Value),
                    new SqlParameter("@Code", objDTO.Code ?? (object)DBNull.Value),
                    new SqlParameter("@RealmCompanyID", objDTO.RealmCompanyID),
                    new SqlParameter("@Created", objDTO.Created),
                    new SqlParameter("@LastUpdated", objDTO.LastUpdated),
                    new SqlParameter("@CreatedBy", objDTO.CreatedBy),
                    new SqlParameter("@LastUpdatedBy", objDTO.LastUpdatedBy),
                    new SqlParameter("@AddedFrom", objDTO.AddedFrom),
                    new SqlParameter("@EditedFrom", objDTO.EditedFrom),
                    new SqlParameter("@IsDeleted", objDTO.IsDeleted),
                    new SqlParameter("@IsArchived", objDTO.IsArchived),
                    new SqlParameter("@QuickBookCompanyName", objDTO.QuickBookCompanyName) };

                return context.Database.SqlQuery<QuickBookTokenDetailDTO>("Exec [InsertTokenDetail] @EnterpriseID, @CompanyID, @RoomID, @AccessToken, @AccessTokenExpireDate, @RefreshToken, @RefreshTokenExpireDate, @Code, @RealmCompanyID, @Created, @LastUpdated, @CreatedBy, @LastUpdatedBy, @AddedFrom, @EditedFrom, @IsDeleted ,@IsArchived, @QuickBookCompanyName", params1).ToList();
            }

        }

        public List<QuickBookTokenDetailDTO> UpdateTokenDetail(QuickBookTokenDetailDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@EnterpriseID", objDTO.EnterpriseID),
                    new SqlParameter("@CompanyID", objDTO.CompanyID),
                    new SqlParameter("@RoomID", objDTO.RoomID),
                    new SqlParameter("@AccessToken", objDTO.AccessToken ?? (object)DBNull.Value),
                    new SqlParameter("@AccessTokenExpireDate", objDTO.AccessTokenExpireDate ?? (object)DBNull.Value),
                    new SqlParameter("@RefreshToken", objDTO.RefreshToken ?? (object)DBNull.Value),
                    new SqlParameter("@RefreshTokenExpireDate", objDTO.RefreshTokenExpireDate ?? (object)DBNull.Value),
                    new SqlParameter("@Code", objDTO.Code ?? (object)DBNull.Value),
                    new SqlParameter("@RealmCompanyID", objDTO.RealmCompanyID),
                    new SqlParameter("@LastUpdated", objDTO.LastUpdated),
                    new SqlParameter("@LastUpdatedBy", objDTO.LastUpdatedBy),
                    new SqlParameter("@EditedFrom", objDTO.EditedFrom),
                    new SqlParameter("@AuthError", objDTO.AuthError ?? (object)DBNull.Value),
                    new SqlParameter("@IsDeleted", objDTO.IsDeleted ?? (object)DBNull.Value),
                    new SqlParameter("@IsArchived", objDTO.IsArchived ?? (object)DBNull.Value),
                    new SqlParameter("@ID", objDTO.ID),
                    new SqlParameter("@QuickBookCompanyName", objDTO.QuickBookCompanyName ?? (object)DBNull.Value) };

                return context.Database.SqlQuery<QuickBookTokenDetailDTO>("Exec [UpdateTokenDetail] @EnterpriseID, @CompanyID, @RoomID, @AccessToken, " +
                    "@AccessTokenExpireDate, @RefreshToken, @RefreshTokenExpireDate, @Code, @RealmCompanyID, @LastUpdated, @LastUpdatedBy, @EditedFrom, @AuthError, @IsDeleted, @IsArchived, @ID, @QuickBookCompanyName", params1).ToList();
            }

        }

        public bool InsertQBLastSyncDetails(Int64 EnterpriseID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseID", EnterpriseID) };
                context.Database.ExecuteSqlCommand("exec [InsertQBLastSyncDetails] @EnterpriseID", params1);
                return true;
            }
        }
        
    }
}
