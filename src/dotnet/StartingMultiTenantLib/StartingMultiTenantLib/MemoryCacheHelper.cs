using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Text;

namespace SeaEveritMicroService.Services
{
    public class MemoryCacheHelper
    {
        private static ObjectCache _cache = MemoryCache.Default;

        public static void Remove(string key) {
            _cache.Remove(key);
        }

        public static bool Add(string key, int miliSec) {
            return _cache.Add(key, 1, DateTimeOffset.UtcNow.AddMilliseconds(miliSec));
        }

        public static void Set(string key, int miliSec, string value = "1") {
            _cache.Set(key, value, DateTimeOffset.UtcNow.AddMilliseconds(miliSec));
        }

        public static void Set<T>(string key, int miliSec, T value) where T : class {
            _cache.Set(key, value, DateTimeOffset.UtcNow.AddMilliseconds(miliSec));
        }

        public static bool Contains(string key, out string value) {
            bool result = _cache.Contains(key);
            value = string.Empty;
            if (result) {
                object valueObj = _cache.Get(key);
                if (valueObj != null) {
                    value = valueObj.ToString();
                }
            }

            return result;
        }

        public static bool Contains<T>(string key, out T value) where T : class {
            value = default(T);
            bool result = _cache.Contains(key);
            if (result) {
                object valueObj = _cache.Get(key);
                if (valueObj != null && valueObj is T) {
                    value = valueObj as T;
                }
            }

            return result;
        }
    }
}