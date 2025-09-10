using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public class SolumTokenDetailDAL : eTurnsBaseDAL
    {
        public SolumTokenDetailDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        /// <summary>
        /// This method is used to get all token detail
        /// </summary>
        /// <param name="IsForRefreshToken">True in case want to fetch all tokens which are required to update</param>
        /// <returns></returns>
        public List<SolumTokenDetailDTO> GetAllTokenDetail(bool IsForRefreshToken)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@IsForRefreshToken", IsForRefreshToken) };
                return context.Database.SqlQuery<SolumTokenDetailDTO>("exec [GetAllSolumTokenDetail] @IsForRefreshToken", params1).ToList();
            }
        }

        public List<SolumStore> GetAllSolumStore()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SolumStore>("exec [GetAllSolumStore]").ToList();
            }
        }

        /// <summary>
        /// This method is used to insert token detail
        /// </summary>
        /// <param name="objDTO"></param>
        public void InsertTokenDetail(SolumTokenDetailDTO objDTO)
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
                    new SqlParameter("@Created", objDTO.Created),
                    new SqlParameter("@LastUpdated", objDTO.LastUpdated),
                    new SqlParameter("@CreatedBy", objDTO.CreatedBy),
                    new SqlParameter("@LastUpdatedBy", objDTO.LastUpdatedBy),
                    new SqlParameter("@AddedFrom", objDTO.AddedFrom),
                    new SqlParameter("@EditedFrom", objDTO.EditedFrom),
                    new SqlParameter("@IsDeleted", objDTO.IsDeleted),
                    new SqlParameter("@IsArchived", objDTO.IsArchived)
                    };

                context.Database.ExecuteSqlCommand("Exec [InsertSolumTokenDetail] @EnterpriseID, @CompanyID, @RoomID, @AccessToken, @AccessTokenExpireDate, @RefreshToken, @RefreshTokenExpireDate, @Created, @LastUpdated, @CreatedBy, @LastUpdatedBy, @AddedFrom, @EditedFrom, @IsDeleted ,@IsArchived", params1);
            }

        }

        /// <summary>
        /// This method is used to updated token detail(refresk token)
        /// </summary>
        /// <param name="objDTO"></param>
        public void UpdateTokenDetail(SolumTokenDetailDTO objDTO)
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
                    new SqlParameter("@LastUpdated", objDTO.LastUpdated),
                    new SqlParameter("@LastUpdatedBy", objDTO.LastUpdatedBy),
                    new SqlParameter("@EditedFrom", objDTO.EditedFrom),
                    new SqlParameter("@AuthError", objDTO.AuthError ?? (object)DBNull.Value),
                    new SqlParameter("@IsDeleted", objDTO.IsDeleted ?? (object)DBNull.Value),
                    new SqlParameter("@IsArchived", objDTO.IsArchived ?? (object)DBNull.Value),
                    new SqlParameter("@ID", objDTO.ID)
                    };

                context.Database.ExecuteSqlCommand("Exec [UpdateSolumTokenDetail] @EnterpriseID, @CompanyID, @RoomID, @AccessToken, " +
                    "@AccessTokenExpireDate, @RefreshToken, @RefreshTokenExpireDate, @LastUpdated, @LastUpdatedBy, @EditedFrom, @AuthError, @IsDeleted, @IsArchived, @ID", params1);
            }

        }

        /// <summary>
        /// This method is used to get the specific token detail 
        /// </summary>
        /// <param name="EnterpriseID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="RoomID"></param>
        /// <returns></returns>
        public SolumTokenDetailDTO GetTokenDetail(long? EnterpriseID, long? CompanyID, long? RoomID)
        {
            var params1 = new SqlParameter[] {
                    new SqlParameter("@EnterpriseID", EnterpriseID ?? (object)DBNull.Value),
                    new SqlParameter("@CompanyID", CompanyID ?? (object)DBNull.Value),
                    new SqlParameter("@RoomID", RoomID ?? (object)DBNull.Value) };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SolumTokenDetailDTO>("exec [GetSolumTokenDetail] @EnterpriseID,@CompanyID,@RoomID", params1).FirstOrDefault();
            }
        }

        public SolumStore GetSolumStoreByRoomId(long EnterpriseId, long CompanyId, long RoomId)
        {
            var params1 = new SqlParameter[] {
                    new SqlParameter("@EnterpriseId", EnterpriseId),
                    new SqlParameter("@CompanyId", CompanyId),
                    new SqlParameter("@RoomId", RoomId) };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SolumStore>("exec [GetSolumStoreByRoomId] @EnterpriseId,@CompanyId,@RoomId", params1).FirstOrDefault();
            }
        }

        public void InsertUpdateSolumStore(SolumTokenDetailDTO objDTO, string companyCode, string storeCode)
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
                    new SqlParameter("@CompanyName", companyCode),
                    new SqlParameter("@StationCode", storeCode),
                    new SqlParameter("@Created", objDTO.Created),
                    new SqlParameter("@LastUpdated", objDTO.LastUpdated),
                    new SqlParameter("@CreatedBy", objDTO.CreatedBy),
                    new SqlParameter("@LastUpdatedBy", objDTO.LastUpdatedBy),
                    new SqlParameter("@AddedFrom", objDTO.AddedFrom),
                    new SqlParameter("@EditedFrom", objDTO.EditedFrom),
                    new SqlParameter("@IsDeleted", objDTO.IsDeleted),
                    new SqlParameter("@IsArchived", objDTO.IsArchived)
                    };

                context.Database.ExecuteSqlCommand("Exec [InsertUpdateSolumStore] @EnterpriseID, @CompanyID, @RoomID, @AccessToken, @AccessTokenExpireDate, @RefreshToken, @RefreshTokenExpireDate, @CompanyName, @StationCode, @Created, @LastUpdated, @CreatedBy, @LastUpdatedBy, @AddedFrom, @EditedFrom, @IsDeleted ,@IsArchived", params1);
            }

        }
        public void DeleteSolumStoreAndTokenDetail(long EnterpriseId, long CompanyId, long RoomId,long UserID, bool IsDeleted,string EditedFrom)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@EnterpriseID", EnterpriseId),
                    new SqlParameter("@CompanyID", CompanyId),
                    new SqlParameter("@RoomID", RoomId),
                    new SqlParameter("@LastUpdated", DateTimeUtility.DateTimeNow),
                    new SqlParameter("@LastUpdatedBy", UserID),
                    new SqlParameter("@IsDeleted", IsDeleted),
                    new SqlParameter("@EditedFrom", EditedFrom)
                    };

                context.Database.ExecuteSqlCommand("Exec [DeleteSolumStoreAndTokenDetail] @EnterpriseID, @CompanyID, @RoomID, @LastUpdated, @LastUpdatedBy, @EditedFrom, @IsDeleted", params1);
            }

        }
    }
}
