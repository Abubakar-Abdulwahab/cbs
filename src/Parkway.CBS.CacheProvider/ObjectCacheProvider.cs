using System;
using System.Runtime.Caching;

namespace Parkway.CBS.CacheProvider
{
    public static class ObjectCacheProvider
    {
        static readonly ObjectCache cache = MemoryCache.Default;

        /// <summary>
        /// Try add object to cache with key.
        /// Object are kept in cache till the end of day unless a different time to live is passed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tenant"></param>
        /// <param name="key"></param>
        /// <param name="valueToCache"></param>
        /// <param name="ttl">time to live</param>
        public static bool TryCache<T>(string tenant, string key, T valueToCache, DateTime? ttl = null)
        {
            try
            {
                return cache.Add(GetKeyCombination(tenant, key), valueToCache, ttl == null ? DateTime.Now.AddDays(1).AddMilliseconds(-1) : ttl.Value);
            }
            catch (Exception) { return false; }
        }


        /// <summary>
        /// Get cached object, would return the cached object 
        /// or a null value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tenant"></param>
        /// <param name="key"></param>
        /// <returns>T</returns>
        public static T GetCachedObject<T>(string tenant, string key) where T : class
        {
            try
            {
                return (T)cache[GetKeyCombination(tenant, key)];
            }
            catch (Exception) { return null; }
        }


        /// <summary>
        /// Get key combination with prefix
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="key"></param>
        /// <returns>string</returns>
        private static string GetKeyCombination(string tenant, string key)
        {
            return $"{tenant}-{key}";
        }

        /// <summary>
        /// Remove an object from the cache
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="key"></param>
        /// <returns>bool</returns>
        public static bool RemoveCachedObject(string tenant, string key)
        {
            try
            {
                cache.Remove(GetKeyCombination(tenant, key));
                return true;
            }
            catch (Exception) { return false; }
        }

    }
}
