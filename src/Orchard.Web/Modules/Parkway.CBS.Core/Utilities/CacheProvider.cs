using System;
using System.Runtime.Caching;

namespace Parkway.CBS.Core.Utilities
{
    public class CacheProvider
    {
        static readonly ObjectCache cache = MemoryCache.Default;

        /// <summary>
        /// Try add object to cache with key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="valueToCache"></param>
        public static bool TryCache<T>(string prefix, string key, T valueToCache, DateTimeOffset expirationTime)
        {
            try
            {
                return cache.Add(GetKeyCombination(prefix, key), valueToCache, expirationTime);
            }
            catch (Exception) { return false; }
        }


        /// <summary>
        /// Get cached object, would return the cached object 
        /// or a null value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefix"></param>
        /// <param name="key"></param>
        /// <returns>T</returns>
        public static T GetCachedObject<T>(string prefix, string key) where T : class
        {
            try
            {
                return (T)cache[GetKeyCombination(prefix, key)];
            }
            catch (Exception) { return null; }
        }

        /// <summary>
        /// Remove cached object with specified key
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="key"></param>
        public static void RemoveCache<T>(string prefix, string key)
        {
            try
            {
                cache.Remove(GetKeyCombination(prefix, key));
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// Get key combination with prefix
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="key"></param>
        /// <returns>string</returns>
        private static string GetKeyCombination(string prefix, string key)
        {
            return $"{prefix}-{key}";
        }

    }
}