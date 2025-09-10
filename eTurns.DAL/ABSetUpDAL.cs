using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace eTurns.DAL
{
    public class ABSetUpDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        public ABSetUpDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        #endregion

        #region [Class Methods]
        public ABTokenDetailDTO EditABTokenDetailAccessCodeInfo(ABTokenDetailDTO objABTokenDetailDTO)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", objABTokenDetailDTO.ID), new SqlParameter("@AccessToken", objABTokenDetailDTO.AccessToken ?? (object)DBNull.Value), new SqlParameter("@AccessTokenExpireDate", objABTokenDetailDTO.AccessTokenExpireDate ?? (object)DBNull.Value), new SqlParameter("@RefreshToken", objABTokenDetailDTO.RefreshToken ?? (object)DBNull.Value), new SqlParameter("@RefreshTokenExpireDate", objABTokenDetailDTO.RefreshTokenExpireDate ?? (object)DBNull.Value), new SqlParameter("@UserID", objABTokenDetailDTO.LastUpdatedBy), new SqlParameter("@EditedFrom", objABTokenDetailDTO.EditedFrom ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ABTokenDetailDTO>("exec [EditABTokenDetailAccessCodeInfo] @ID,@AccessToken,@AccessTokenExpireDate,@RefreshToken,@RefreshTokenExpireDate,@UserID,@EditedFrom", params1).FirstOrDefault();
            }
        }
        public ABTokenDetailDTO GetABTokenDetailByAuthCode(string ABAppAuthCode)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ABAppAuthCode", ABAppAuthCode ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ABTokenDetailDTO>("exec [GetABTokenDetailByAuthCode] @ABAppAuthCode", params1).FirstOrDefault();
            }
        }
        public ABTokenDetailDTO InsertABTokenDetail(ABTokenDetailDTO objABTokenDetailDTO)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ABAppAuthCode", objABTokenDetailDTO.ABAppAuthCode ?? (object)DBNull.Value), new SqlParameter("@AccessToken", objABTokenDetailDTO.AccessToken ?? (object)DBNull.Value), new SqlParameter("@AccessTokenExpireDate", objABTokenDetailDTO.AccessTokenExpireDate ?? (object)DBNull.Value), new SqlParameter("@RefreshToken", objABTokenDetailDTO.RefreshToken ?? (object)DBNull.Value), new SqlParameter("@RefreshTokenExpireDate", objABTokenDetailDTO.RefreshTokenExpireDate ?? (object)DBNull.Value), new SqlParameter("@Created", objABTokenDetailDTO.Created), new SqlParameter("@LastUpdated", objABTokenDetailDTO.LastUpdated), new SqlParameter("@CreatedBy", objABTokenDetailDTO.CreatedBy), new SqlParameter("@LastUpdatedBy", objABTokenDetailDTO.LastUpdatedBy), new SqlParameter("@AddedFrom", objABTokenDetailDTO.AddedFrom ?? (object)DBNull.Value), new SqlParameter("@EditedFrom", objABTokenDetailDTO.EditedFrom ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ABTokenDetailDTO>("exec [InsertABTokenDetail] @ABAppAuthCode,@AccessToken,@AccessTokenExpireDate,@RefreshToken,@RefreshTokenExpireDate,@Created,@LastUpdated,@CreatedBy,@LastUpdatedBy,@AddedFrom,@EditedFrom", params1).FirstOrDefault();
            }
        }
        public ABDevAppSettingDTO GetABDevAppSetting()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ABDevAppSettingDTO>("exec [GetABDevAppSetting]").FirstOrDefault();
            }
        }
        public List<ABMarketPlaceDTO> GetAllABMarketPlace()
        {
            var params1 = new SqlParameter[] { };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ABMarketPlaceDTO>("exec [GetAllABMarketPlace] ", params1).ToList();
            }
        }
        public ABMarketPlaceDTO GetABMarketPlaceByMPID(string MarketplaceID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@MarketplaceID", MarketplaceID ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ABMarketPlaceDTO>("exec [GetABMarketPlaceByMPID] @MarketplaceID", params1).FirstOrDefault();
            }
        }
        public ABMarketPlaceDTO GetABMarketPlaceByDomain(string OAuthAuthorizationURI)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@OAuthAuthorizationURI", OAuthAuthorizationURI ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ABMarketPlaceDTO>("exec [GetABMarketPlaceByDomain] @OAuthAuthorizationURI", params1).FirstOrDefault();
            }
        }
        public ABMarketPlaceDTO GetABMarketPlaceByRegionCode(string MarketplaceID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RegionCode", MarketplaceID ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ABMarketPlaceDTO>("exec [GetABMarketPlaceByRegionCode] @RegionCode", params1).FirstOrDefault();
            }
        }
        public ABAPIPathConfigDTO GetABAPIPathConfig(string ABAPISection, string ABAPIModuleName, string ABAPIOperation)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ABAPISection", ABAPISection ?? (object)DBNull.Value), new SqlParameter("@ABAPIModuleName", ABAPIModuleName ?? (object)DBNull.Value), new SqlParameter("@ABAPIOperation", ABAPIOperation ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ABAPIPathConfigDTO>("exec [GetABAPIPathConfig] @ABAPISection,@ABAPIModuleName,@ABAPIOperation", params1).FirstOrDefault();
            }
        }

        public List<ZohoReponseDTO> FetchZohoReponseForProcessing()
        {
            //List<ZohoReponseDTO> lstReturn = new List<ZohoReponseDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<ZohoReponseDTO> lstZohoReponseDTO = context.Database.SqlQuery<ZohoReponseDTO>("exec [" + DbConnectionHelper.GeteTurnsLoggingDBName() + "].[dbo].[FetchZohoReponseForProcessing]").ToList();
                if (lstZohoReponseDTO != null && lstZohoReponseDTO.Count > 0)
                {
                    foreach (var zohoReponseDTO in lstZohoReponseDTO)
                    {
                        if (!string.IsNullOrWhiteSpace(zohoReponseDTO.RespData))
                        {
                            try
                            {
                                SubscriptionInfo objData = new SubscriptionInfo();
                                objData = JsonConvert.DeserializeObject<SubscriptionInfo>(zohoReponseDTO.RespData);
                                if (objData != null)
                                {
                                    zohoReponseDTO.subscriptionInfo = objData;
                                    zohoReponseDTO.subscription = objData.data.subscription;
                                    zohoReponseDTO.isValid = true;
                                    //objData.ID = zohoReponseDTO.ID;
                                    //objData.IsStarted = zohoReponseDTO.IsStarted;
                                    //objData.TimeStarted = zohoReponseDTO.TimeStarted;
                                    //objData.IsCompleted = zohoReponseDTO.IsCompleted;
                                    //objData.TimeCompleted = zohoReponseDTO.TimeCompleted;
                                    //objData.IsException = zohoReponseDTO.IsException;
                                    //objData.TimeException = zohoReponseDTO.TimeException;
                                    //objData.ErrorException = zohoReponseDTO.ErrorException;
                                    //objData.Created = zohoReponseDTO.Created;
                                    //objData.Updated = zohoReponseDTO.Updated;
                                    //objData.EnterpriseID = zohoReponseDTO.EnterpriseID;
                                    //objData.CompanyID = zohoReponseDTO.CompanyID;
                                    //objData.RoomID = zohoReponseDTO.RoomID;
                                    //objData.RespData = zohoReponseDTO.RespData;

                                    //lstReturn.Add(objData);
                                }
                                else
                                {
                                    zohoReponseDTO.isValid = false;
                                }
                            }
                            catch
                            {
                                zohoReponseDTO.isValid = false;
                            }
                        }
                    }
                }
                return lstZohoReponseDTO;
            }
        }

        public ZohoReponseDTO FetchZohoReponseForProcessingRoleAndUser(long EnterpriseId, long CompanyId, long RoomId)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseId", EnterpriseId), new SqlParameter("@CompanyId", CompanyId), new SqlParameter("@RoomId", RoomId) };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var zohoReponseDTO = context.Database.SqlQuery<ZohoReponseDTO>("exec [" + DbConnectionHelper.GeteTurnsLoggingDBName() + "].[dbo].[FetchZohoReponseForProcessingRoleAndUser] @EnterpriseId,@CompanyId,@RoomId ", params1).FirstOrDefault();

                if (zohoReponseDTO != null && zohoReponseDTO.ID > 0)
                {
                    if (!string.IsNullOrWhiteSpace(zohoReponseDTO.RespData) && !string.IsNullOrEmpty(zohoReponseDTO.RespData))
                    {
                        try
                        {
                            SubscriptionInfo objData = new SubscriptionInfo();
                            objData = JsonConvert.DeserializeObject<SubscriptionInfo>(zohoReponseDTO.RespData);

                            if (objData != null)
                            {
                                zohoReponseDTO.subscriptionInfo = objData;
                                zohoReponseDTO.subscription = objData.data.subscription;
                                zohoReponseDTO.isValid = true;
                            }
                            else
                            {
                                zohoReponseDTO.isValid = false;
                            }
                        }
                        catch
                        {
                            zohoReponseDTO.isValid = false;
                        }
                    }
                    else
                    {
                        zohoReponseDTO.isValid = false;
                    }

                }

                return zohoReponseDTO;
            }
        }

        public ZohoReponseDTO SetABECRCreateCompletedWithSuccess(long ID, long? EnterPriseID, long? CompanyID, long? RoomID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID),
                                               new SqlParameter("@EnterPriseID", EnterPriseID ?? (object)DBNull.Value),
                                               new SqlParameter("@CompanyID", CompanyID ?? (object)DBNull.Value),
                                               new SqlParameter("@RoomID", RoomID ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ZohoReponseDTO>("exec [" + DbConnectionHelper.GeteTurnsLoggingDBName() + "].[dbo].[SetABECRCreateCompletedWithSuccess] @ID,@EnterPriseID,@CompanyID,@RoomID", params1).FirstOrDefault();
            }
        }
        public ZohoReponseDTO SetABECRCreateCompletedWithError(long ID, string ErrorException, long? EnterPriseID, long? CompanyID, long? RoomID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@ErrorException", ErrorException ?? (object)DBNull.Value),
                                               new SqlParameter("@EnterPriseID", EnterPriseID ?? (object)DBNull.Value),
                                               new SqlParameter("@CompanyID", CompanyID ?? (object)DBNull.Value),
                                               new SqlParameter("@RoomID", RoomID ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ZohoReponseDTO>("exec [" + DbConnectionHelper.GeteTurnsLoggingDBName() + "].[dbo].[SetABECRCreateCompletedWithError] @ID,@ErrorException,@EnterPriseID,@CompanyID,@RoomID", params1).FirstOrDefault();
            }
        }
        public ZohoReponseDTO UpdateABECRCreateWithException(long ID, string ErrorException, long? EnterPriseID, long? CompanyID, long? RoomID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@ErrorException", ErrorException ?? (object)DBNull.Value),
                                               new SqlParameter("@EnterPriseID", EnterPriseID ?? (object)DBNull.Value),
                                               new SqlParameter("@CompanyID", CompanyID ?? (object)DBNull.Value),
                                               new SqlParameter("@RoomID", RoomID ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ZohoReponseDTO>("exec [" + DbConnectionHelper.GeteTurnsLoggingDBName() + "].[dbo].[UpdateABECRCreateWithException] @ID,@ErrorException,@EnterPriseID,@CompanyID,@RoomID", params1).FirstOrDefault();
            }
        }
        public ZohoReponseDTO UpdateABECRCreateWithUserIDRoleID(long ID, long RoleID, long UserID, string CustomerJson)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID),
                                               new SqlParameter("@RoleID", RoleID),
                                               new SqlParameter("@UserID", UserID),
                                               new SqlParameter("@CustomerJson", CustomerJson ?? (object)DBNull.Value)
                                             };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ZohoReponseDTO>("exec [" + DbConnectionHelper.GeteTurnsLoggingDBName() + "].[dbo].[UpdateABECRCreateWithUserIDRoleID] @ID,@RoleID,@UserID,@CustomerJson", params1).FirstOrDefault();
            }
        }


        public GetABAppRequestLogDTO InsertGetABAppRequestLog(GetABAppRequestLogDTO objGetABAppRequestLogDTO)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UserID", objGetABAppRequestLogDTO.UserID), new SqlParameter("@UserName", objGetABAppRequestLogDTO.UserName ?? (object)DBNull.Value), new SqlParameter("@amazon_callback_uri", objGetABAppRequestLogDTO.amazon_callback_uri ?? (object)DBNull.Value), new SqlParameter("@amazon_state", objGetABAppRequestLogDTO.amazon_state ?? (object)DBNull.Value), new SqlParameter("@applicationId", objGetABAppRequestLogDTO.applicationId ?? (object)DBNull.Value), new SqlParameter("@Created", objGetABAppRequestLogDTO.Created), new SqlParameter("@Updated", objGetABAppRequestLogDTO.Updated), new SqlParameter("@EnterpriseID", objGetABAppRequestLogDTO.EnterpriseID), new SqlParameter("@CompanyID", objGetABAppRequestLogDTO.CompanyID), new SqlParameter("@RoomID", objGetABAppRequestLogDTO.RoomID), new SqlParameter("@auth_code", objGetABAppRequestLogDTO.auth_code ?? (object)DBNull.Value), new SqlParameter("@eTurns_state", objGetABAppRequestLogDTO.eTurns_state ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<GetABAppRequestLogDTO>("exec [InsertGetABAppRequestLog] @UserID,@UserName,@amazon_callback_uri,@amazon_state,@applicationId,@Created,@Updated,@EnterpriseID,@CompanyID,@RoomID,@auth_code,@eTurns_state", params1).FirstOrDefault();
            }
        }
        public GetABAppRequestLogDTO InsertOrUpdateGetABAppRequestLogByUserID(GetABAppRequestLogDTO objGetABAppRequestLogDTO)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UserID", objGetABAppRequestLogDTO.UserID), new SqlParameter("@UserName", objGetABAppRequestLogDTO.UserName ?? (object)DBNull.Value), new SqlParameter("@amazon_callback_uri", objGetABAppRequestLogDTO.amazon_callback_uri ?? (object)DBNull.Value), new SqlParameter("@amazon_state", objGetABAppRequestLogDTO.amazon_state ?? (object)DBNull.Value), new SqlParameter("@applicationId", objGetABAppRequestLogDTO.applicationId ?? (object)DBNull.Value), new SqlParameter("@Created", objGetABAppRequestLogDTO.Created), new SqlParameter("@Updated", objGetABAppRequestLogDTO.Updated), new SqlParameter("@EnterpriseID", objGetABAppRequestLogDTO.EnterpriseID), new SqlParameter("@CompanyID", objGetABAppRequestLogDTO.CompanyID), new SqlParameter("@RoomID", objGetABAppRequestLogDTO.RoomID), new SqlParameter("@auth_code", objGetABAppRequestLogDTO.auth_code ?? (object)DBNull.Value), new SqlParameter("@eTurns_state", objGetABAppRequestLogDTO.eTurns_state ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<GetABAppRequestLogDTO>("exec [InsertOrUpdateGetABAppRequestLogByUserID] @UserID,@UserName,@amazon_callback_uri,@amazon_state,@applicationId,@Created,@Updated,@EnterpriseID,@CompanyID,@RoomID,@auth_code,@eTurns_state", params1).FirstOrDefault();
            }
        }
        public GetABAppRequestLogDTO UpdateGetABAppRequestLogAuthCode(string auth_code, string eTurns_state)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@auth_code", auth_code ?? (object)DBNull.Value), new SqlParameter("@eTurns_state", eTurns_state ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<GetABAppRequestLogDTO>("exec [UpdateGetABAppRequestLogAuthCode] @auth_code,@eTurns_state", params1).FirstOrDefault();
            }
        }

        public ZohoReponseDTO FetchZohoReponseByUser(long UserID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UserID", UserID) };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ZohoReponseDTO>("exec [" + DbConnectionHelper.GeteTurnsLoggingDBName() + "].[dbo].[FetchZohoReponseByUser] @UserID", params1).FirstOrDefault();
            }
        }
        public GetABAppRequestLogDTO FetchGetABAppRequestLog(long UserID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UserID", UserID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<GetABAppRequestLogDTO>("exec [FetchGetABAppRequestLog] @UserID", params1).FirstOrDefault();
            }
        }
        public GetABAppRequestLogDTO UpdateGetABAppRequestLog(GetABAppRequestLogDTO objGetABAppRequestLogDTO)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UserID", objGetABAppRequestLogDTO.UserID), 
                                               new SqlParameter("@EnterpriseID", objGetABAppRequestLogDTO.EnterpriseID), 
                                               new SqlParameter("@CompanyID", objGetABAppRequestLogDTO.CompanyID), 
                                               new SqlParameter("@RoomID", objGetABAppRequestLogDTO.RoomID),
                                               new SqlParameter("@UserName", objGetABAppRequestLogDTO.UserName ?? (object)DBNull.Value),
                                               new SqlParameter("@eTurns_state", objGetABAppRequestLogDTO.eTurns_state ?? (object)DBNull.Value) };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<GetABAppRequestLogDTO>("exec [UpdateGetABAppRequestLog] @UserID,@EnterpriseID,@CompanyID,@RoomID,@UserName,@eTurns_state", params1).FirstOrDefault();
            }
        }

        public List<ZohoInvoiceToeTurnsResponseDTO> FetchZohoInvoiceResponseForProcessing()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<ZohoInvoiceToeTurnsResponseDTO> lstZohoReponseDTO = context.Database.SqlQuery<ZohoInvoiceToeTurnsResponseDTO>("exec [" + DbConnectionHelper.GeteTurnsLoggingDBName() + "].[dbo].[FetchZohoInvoiceResponseForProcessing]").ToList();
                
                if (lstZohoReponseDTO != null && lstZohoReponseDTO.Count > 0)
                {
                    foreach (var zohoReponseDTO in lstZohoReponseDTO)
                    {
                        if (!string.IsNullOrWhiteSpace(zohoReponseDTO.RespData))
                        {
                            try
                            {
                                ZohoInvoiceToeTurnsResponseInfo objData = new ZohoInvoiceToeTurnsResponseInfo();
                                objData = JsonConvert.DeserializeObject<ZohoInvoiceToeTurnsResponseInfo>(zohoReponseDTO.RespData);
                                
                                if (objData != null)
                                {
                                    zohoReponseDTO.InvoiceInfo = objData;
                                    zohoReponseDTO.Invoice = objData.data.invoice;
                                    zohoReponseDTO.isValid = true;
                                }
                                else
                                {
                                    zohoReponseDTO.isValid = false;
                                }
                            }
                            catch
                            {
                                zohoReponseDTO.isValid = false;
                            }
                        }
                    }
                }
                return lstZohoReponseDTO;
            }
        }

        public void SetZohoInvoiceCreateCompletedWithSuccess(long ID, string QBInvoiceId)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID),
                                               new SqlParameter("@QBInvoiceId", QBInvoiceId ?? (object)DBNull.Value)
                                             };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("exec [" + DbConnectionHelper.GeteTurnsLoggingDBName() + "].[dbo].[SetZohoInvoiceCreateCompletedWithSuccess] @ID,@QBInvoiceId", params1);
            }
        }

        public void UpdateZohoInvoiceCreateWithException(long ID, string ErrorException)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID), 
                                               new SqlParameter("@ErrorException", ErrorException ?? (object)DBNull.Value)
                                             };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("exec [" + DbConnectionHelper.GeteTurnsLoggingDBName() + "].[dbo].[UpdateZohoInvoiceCreateWithException] @ID,@ErrorException", params1);
            }
        }

        #endregion

    }
}
