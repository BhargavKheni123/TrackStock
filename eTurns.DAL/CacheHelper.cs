using System;
using System.Collections;
using System.Web;

namespace eTurns.DAL
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

        public static void InvalidateCacheByKey(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }
        public static void InvalidateUserUISettingsCacheByKey(string key)
        {
            HttpRuntime.Cache.Remove(string.Concat(MasterCacheKeyArray[0], "-", key));
        }

        public static void InvalidateCacheByKeyStartWith(string key)
        {
            IDictionaryEnumerator enumerator = System.Web.HttpRuntime.Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string keyvalues = (string)enumerator.Key;
                if (keyvalues.Contains(key))
                {
                    HttpRuntime.Cache.Remove(keyvalues);
                }
            }
            HttpRuntime.Cache.Remove(key);
        }
        public static T GetCacheItem(string rawKey)
        {
            return (T)HttpRuntime.Cache[GetCacheKey(rawKey)];
        }

        public static T AddCacheItem(string rawKey, object value)
        {
            if (HttpContext.Current != null)
            {
                System.Web.Caching.Cache DataCache = HttpRuntime.Cache;

                if (DataCache[MasterCacheKeyArray[0]] == null)
                    DataCache[MasterCacheKeyArray[0]] = DateTimeUtility.DateTimeNow;

                System.Web.Caching.CacheDependency dependency = new System.Web.Caching.CacheDependency(null, MasterCacheKeyArray);
                DataCache.Insert(GetCacheKey(rawKey), value, dependency, DateTime.Now.AddDays(60), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            return (T)value;
        }


        public static T AppendToCacheItem(string rawKey, object value)
        {
            System.Web.Caching.Cache DataCache = HttpRuntime.Cache;

            if (DataCache[MasterCacheKeyArray[0]] == null)
                DataCache[MasterCacheKeyArray[0]] = DateTimeUtility.DateTimeNow;

            System.Web.Caching.CacheDependency dependency = new System.Web.Caching.CacheDependency(null, MasterCacheKeyArray);
            DataCache.Insert(GetCacheKey(rawKey), value, dependency, DateTime.Now.AddDays(60), System.Web.Caching.Cache.NoSlidingExpiration);

            //DataCache.Add(GetCacheKey(rawKey), value, dependency, DateTime.Now.AddDays(60), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High,null);

            return (T)value;
        }

    }
}
