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
using Newtonsoft.Json;

namespace eTurns.DAL
{
    public class ZohoDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        public ZohoDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        #endregion

        public ZohoDevAppSettingDTO GetZohoDevAppSetting()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ZohoDevAppSettingDTO>("exec [GetZohoDevAppSetting]").FirstOrDefault();
            }
        }

        public ZOHOAPIPathConfigDTO GetZohoAPIPathConfig(string ZohoAPISection, string ZohoAPIModuleName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ZohoAPISection", ZohoAPISection ?? (object)DBNull.Value), new SqlParameter("@ZohoAPIModuleName", ZohoAPIModuleName ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ZOHOAPIPathConfigDTO>("exec [GetZohoAPIPathConfig] @ZohoAPISection,@ZohoAPIModuleName", params1).FirstOrDefault();
            }
        }

        public ZohoTokenDetailDTO GetZohoTokenDetail()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ZohoTokenDetailDTO>("exec [GetZohoTokenDetail]").FirstOrDefault();
            }
        }

        public ZohoTokenDetailDTO EditZohoTokenDetailAccessCodeInfo(ZohoTokenDetailDTO zohoTokenDetailDTO)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", zohoTokenDetailDTO.ID), 
                                               new SqlParameter("@AccessToken", zohoTokenDetailDTO.AccessToken ?? (object)DBNull.Value), 
                                               new SqlParameter("@AccessTokenExpireDate", zohoTokenDetailDTO.AccessTokenExpireDate ?? (object)DBNull.Value), 
                                               new SqlParameter("@RefreshToken", zohoTokenDetailDTO.RefreshToken ?? (object)DBNull.Value), 
                                               new SqlParameter("@RefreshTokenExpireDate", zohoTokenDetailDTO.RefreshTokenExpireDate ?? (object)DBNull.Value), 
                                               //new SqlParameter("@UserID", objABTokenDetailDTO.LastUpdatedBy), 
                                               new SqlParameter("@EditedFrom", zohoTokenDetailDTO.EditedFrom ?? (object)DBNull.Value) };
            
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ZohoTokenDetailDTO>("exec [EditZohoTokenDetailAccessCodeInfo] @ID,@AccessToken,@AccessTokenExpireDate,@RefreshToken,@RefreshTokenExpireDate,@EditedFrom", params1).FirstOrDefault();
            }
        }


        public bool izZohoUser(long userID)
        {
            try
            {
                var param = new SqlParameter[] { new SqlParameter("@UserId", userID) };
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    return context.Database.SqlQuery<bool>("exec [CheckZoHoUser] @UserId", param).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }


    }
}
