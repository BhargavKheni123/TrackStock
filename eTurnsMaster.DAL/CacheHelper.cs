using System;
using System.Web;

namespace eTurnsMaster.DAL
{
    public static class CacheHelper<T>
    {
        public static string[] MasterCacheKeyArray = { typeof(T).ToString() };

        public static string GetCacheKey(string cacheKey)
        {
            return string.Concat(MasterCacheKeyArray[0], "-", cacheKey);
        }

        public static void InvalidateCache()
        {
            HttpRuntime.Cache.Remove(MasterCacheKeyArray[0]);
        }

        public static T GetCacheItem(string rawKey)
        {
            return (T)HttpRuntime.Cache[GetCacheKey(rawKey)];
        }

        public static T AddCacheItem(string rawKey, object value)
        {
            System.Web.Caching.Cache DataCache = HttpRuntime.Cache;

            if (DataCache[MasterCacheKeyArray[0]] == null)
                DataCache[MasterCacheKeyArray[0]] = DateTime.UtcNow;

            System.Web.Caching.CacheDependency dependency = new System.Web.Caching.CacheDependency(null, MasterCacheKeyArray);
            DataCache.Insert(GetCacheKey(rawKey), value, dependency, DateTime.UtcNow.AddDays(60), System.Web.Caching.Cache.NoSlidingExpiration);

            return (T)value;
        }


        public static T AppendToCacheItem(string rawKey, object value)
        {
            System.Web.Caching.Cache DataCache = HttpRuntime.Cache;

            if (DataCache[MasterCacheKeyArray[0]] == null)
                DataCache[MasterCacheKeyArray[0]] = DateTime.UtcNow;

            System.Web.Caching.CacheDependency dependency = new System.Web.Caching.CacheDependency(null, MasterCacheKeyArray);
            DataCache.Insert(GetCacheKey(rawKey), value, dependency, DateTime.UtcNow.AddDays(60), System.Web.Caching.Cache.NoSlidingExpiration);

            //DataCache.Add(GetCacheKey(rawKey), value, dependency, DateTime.UtcNow.AddDays(60), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High,null);

            return (T)value;
        }

    }
}
