using eTurns.DTO.Helper;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTurnsMaster.DAL
{
    public static class RedisCacheHelper 
    {
        private static readonly ConnectionMultiplexer _redisConnection;
        private static readonly IDatabase _db;
        private static string  redisEndpoint = eTurnsAppConfig.redisCacheEndPoint;
        private static IServer _server;

        static RedisCacheHelper()
        {
            _redisConnection = ConnectionMultiplexer.Connect(redisEndpoint);
            _db = _redisConnection.GetDatabase();
            _server = _redisConnection.GetServer(_redisConnection.GetEndPoints()[0]);
        }


        public static void SetCacheValue(string key, string value, TimeSpan? expiry = null)
        {
            _db.StringSet(key, value);
        }

        public static string GetCacheValue(string key)
        {
            return _db.StringGet(key);
        }

        public static bool RemoveCacheValue(string key)
        {
            string pattern = key + ":*"; // This will match keys like Key:0, Key:1, etc.
            var keys = _server.Keys(pattern: pattern);
            // Delete matching keys
            foreach (var keyss in keys)
            {
                _db.KeyDelete(keyss); // Delete the key from Redis
            }
            return _db.KeyDelete(key);
        }

        // Save list to Redis as JSON
        public static void SaveList<T>(string key, List<T> list)
        {
            var jsonData = JsonConvert.SerializeObject(list);
            _db.StringSet(key, jsonData,TimeSpan.FromMinutes(10)); // Save for 10 minutes
        }

        // Retrieve list from Redis
        public static List<T> RetrieveList<T>(string key)
        {
            var jsonData = _db.StringGet(key);
            if (jsonData.IsNullOrEmpty) return new List<T>();
            return JsonConvert.DeserializeObject<List<T>>(jsonData);
        }

        public static void Dispose()
        {
            _redisConnection.Dispose();
        }
    }
}