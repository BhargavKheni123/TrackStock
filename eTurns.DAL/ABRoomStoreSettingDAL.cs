using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DAL
{
    public class ABRoomStoreSettingDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]        
        public ABRoomStoreSettingDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }
        #endregion

        #region [Class Methods]
        public ABRoomStoreSettingDTO EditABRoomStoreSetting(ABRoomStoreSettingDTO objABRoomStoreSettingDTO)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", objABRoomStoreSettingDTO.ID), new SqlParameter("@ABLocale", objABRoomStoreSettingDTO.ABLocale ?? (object)DBNull.Value), new SqlParameter("@EmailAddress", objABRoomStoreSettingDTO.EmailAddress ?? (object)DBNull.Value), new SqlParameter("@ProductSearchFacets", objABRoomStoreSettingDTO.ProductSearchFacets ?? (object)DBNull.Value), new SqlParameter("@ProductSearchPageSize", objABRoomStoreSettingDTO.ProductSearchPageSize), new SqlParameter("@AbSupplierName", objABRoomStoreSettingDTO.AbSupplierName ?? (object)DBNull.Value), new SqlParameter("@ProductVariantLimit", objABRoomStoreSettingDTO.ProductVariantLimit), new SqlParameter("@ABSystemID", objABRoomStoreSettingDTO.ABSystemID ?? (object)DBNull.Value), new SqlParameter("@ABSystemPassword", objABRoomStoreSettingDTO.ABSystemPassword ?? (object)DBNull.Value), new SqlParameter("@ItemUDFforSeller", objABRoomStoreSettingDTO.ItemUDFforSeller ?? (object)DBNull.Value), new SqlParameter("@AmazonOrderEndpoint", objABRoomStoreSettingDTO.AmazonOrderEndpoint ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ABRoomStoreSettingDTO>("exec [EditABRoomStoreSetting] @ID,@ABLocale,@EmailAddress,@ProductSearchFacets,@ProductSearchPageSize,@AbSupplierName,@ProductVariantLimit,@ABSystemID,@ABSystemPassword,@ItemUDFforSeller,@AmazonOrderEndpoint", params1).FirstOrDefault();
            }
        }
        public ABRoomStoreSettingDTO GetABRoomStoreSetting(long CompanyID, long RoomID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomID", RoomID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ABRoomStoreSettingDTO>("exec [GetABRoomStoreSetting] @CompanyID,@RoomID", params1).FirstOrDefault();
            }
        }
        public ABRoomStoreSettingDTO InsertABRoomStoreSetting(ABRoomStoreSettingDTO objABRoomStoreSettingDTO)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", objABRoomStoreSettingDTO.CompanyID), new SqlParameter("@RoomID", objABRoomStoreSettingDTO.RoomID), new SqlParameter("@ABLocale", objABRoomStoreSettingDTO.ABLocale ?? (object)DBNull.Value), new SqlParameter("@ABCountryCode", objABRoomStoreSettingDTO.ABCountryCode ?? (object)DBNull.Value), new SqlParameter("@AuthCode", objABRoomStoreSettingDTO.AuthCode ?? (object)DBNull.Value), new SqlParameter("@EmailAddress", objABRoomStoreSettingDTO.EmailAddress ?? (object)DBNull.Value), new SqlParameter("@ProductSearchFacets", objABRoomStoreSettingDTO.ProductSearchFacets ?? (object)DBNull.Value), new SqlParameter("@MarketplaceID", objABRoomStoreSettingDTO.MarketplaceID ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ABRoomStoreSettingDTO>("exec [InsertABRoomStoreSetting] @CompanyID,@RoomID,@ABLocale,@ABCountryCode,@AuthCode,@EmailAddress,@ProductSearchFacets,@MarketplaceID", params1).FirstOrDefault();
            }
        }
        public ABRoomStoreSettingDTO InsertABRoomStoreSettingDefaults(long CompanyID, long RoomID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomID", RoomID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ABRoomStoreSettingDTO>("exec [InsertABRoomStoreSettingDefaults] @CompanyID,@RoomID", params1).FirstOrDefault();
            }
        }
        public ABRoomStoreSettingDTO EditABRoomStoreSettingAuthCode(long ID, string AuthCode)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@AuthCode", AuthCode ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ABRoomStoreSettingDTO>("exec [EditABRoomStoreSettingAuthCode] @ID,@AuthCode", params1).FirstOrDefault();
            }
        }
        public ABRoomStoreSettingDTO EditABRoomStoreSettingMPAndAuth(long ID, string AuthCode, string ABLocale, string ABCountryCode, string MarketplaceID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@AuthCode", AuthCode ?? (object)DBNull.Value), new SqlParameter("@ABLocale", ABLocale ?? (object)DBNull.Value), new SqlParameter("@ABCountryCode", ABCountryCode ?? (object)DBNull.Value), new SqlParameter("@MarketplaceID", MarketplaceID ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ABRoomStoreSettingDTO>("exec [EditABRoomStoreSettingMPAndAuth] @ID,@AuthCode,@ABLocale,@ABCountryCode,@MarketplaceID", params1).FirstOrDefault();
            }
        }
        public ABRoomStoreSettingDTO EditABRoomMPCountry(long ID, string ABCountryCode, string MarketplaceID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@ABCountryCode", ABCountryCode ?? (object)DBNull.Value), new SqlParameter("@MarketplaceID", MarketplaceID ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ABRoomStoreSettingDTO>("exec [EditABRoomMPCountry] @ID,@ABCountryCode,@MarketplaceID", params1).FirstOrDefault();
            }
        }
        public ABRoomStoreSettingDTO InsertABRoomStoreSettingDefaultsWithEmail(long CompanyID, long RoomID, string Email)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@Email", string.IsNullOrWhiteSpace(Email) ? (object)DBNull.Value : Email) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ABRoomStoreSettingDTO>("exec [InsertABRoomStoreSettingDefaultsWithEmail] @CompanyID,@RoomID,@Email", params1).FirstOrDefault();
            }
        }
        #endregion
    }
}
