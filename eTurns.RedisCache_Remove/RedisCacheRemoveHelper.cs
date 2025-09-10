using eTurns.DTO;
using eTurnsMaster.DAL;
using System;
using System.Collections.Generic;
namespace eTurns.RedisCache_Remove
{
    public class RedisCacheRemoveHelper
    {
        public void ProcessRemoveRedisCache()
        {
            RedisCacheKeyDAL objRedisCacheKeyDAL = new RedisCacheKeyDAL();
            try
            {
                List<RedisCacheKeyDTO> lstRedisKeyToRemove = objRedisCacheKeyDAL.GetRedisCacheKeysToRemove();
                foreach (RedisCacheKeyDTO objRedisCacheDTO in lstRedisKeyToRemove)
                {
                    try
                    {
                        string redisKey = objRedisCacheDTO.EnterpriseId.ToString() + "_" + objRedisCacheDTO.CompanyId.ToString() + "_" + objRedisCacheDTO.RoomId.ToString() + "_" + objRedisCacheDTO.CacheKeyName;
                        CommonFunctions.SaveLogInTextFile("eTurns.RedisCache_Remove >> ProcessRemoveRedisCache => Started: redisKey :  " + redisKey + " => On : " + System.DateTime.UtcNow, GeneralHelper.DoSendLogsInMail);
                        RedisCacheHelper.RemoveCacheValue(redisKey);
                        //objRedisCacheKeyDAL.UpdateRedisCacheKeyStatus(objRedisCacheDTO.Id);
                        CommonFunctions.SaveLogInTextFile("eTurns.RedisCache_Remove >> ProcessRemoveRedisCache => End: redisKey :  " + redisKey + " => On : " + System.DateTime.UtcNow, GeneralHelper.DoSendLogsInMail);

                    }
                    catch (Exception ex)
                    {
                        CommonFunctions.SaveExceptionInTextFile("FAIL : eTurns.RedisCache_Remove >> ProcessRemoveRedisCache : RemoveCacheValue >> ", ex, GeneralHelper.DoSendLogsInMail);
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.SaveExceptionInTextFile("FAIL : eTurns.RedisCache_Remove >> ProcessRemoveRedisCache : GetRedisCacheKeysToRemove() >> ", ex, GeneralHelper.DoSendLogsInMail);
            }
        }
    }
}
