using System;
using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
namespace eTurns.DAL
{
    public interface ICacheManager
    {
        T Get<T>(string key);
        void Set(string key, object data, int cacheTime);
        bool IsSet(string key);
        void Remove(string key);
        void RemoveByPattern(string pattern);
        void Clear();
    }
    public partial class MemoryCacheManager : ICacheManager
    {
        protected ObjectCache Cache
        {
            get
            {
                return MemoryCache.Default;
            }
        }

        public virtual T Get<T>(string key)
        {
            return (T)Cache[key];
        }
        public virtual void Set(string key, object data, int cacheTime)
        {
            if (data == null)
                return;

            var policy = new CacheItemPolicy { AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheTime) };
            Cache.Add(new CacheItem(key, data), policy);
        }

        public virtual bool IsSet(string key)
        {
            return (Cache.Contains(key));
        }
        public virtual void Remove(string key)
        {
            Cache.Remove(key);
        }
        public virtual void RemoveByPattern(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var keysToRemove = (from item in Cache where regex.IsMatch(item.Key) select item.Key).ToList();

            foreach (string key in keysToRemove)
            {
                Remove(key);
            }
        }
        public virtual void Clear()
        {
            foreach (var item in Cache)
                Remove(item.Key);
        }
    }
}