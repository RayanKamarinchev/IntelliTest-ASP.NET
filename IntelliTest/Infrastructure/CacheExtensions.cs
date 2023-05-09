using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace IntelliTest.Infrastructure
{
    public static class CacheExtensions
    {
        public static Task SetAsync<T>(this IDistributedCache cache, string key, T value)
        {
            return SetAsync(cache, key, value, new DistributedCacheEntryOptions());
        }
        public static Task SetAsync<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, GetJsonSerializerOptions()));
            return cache.SetAsync(key, bytes, options);
        }
        public static bool TryGetValue<T>(this IDistributedCache cache, string key, out T? value)
        {
            byte[] val = cache.Get(key);
            if (val == null)
            {
                value = default;
                return false;
            }
            value = JsonSerializer.Deserialize<T>(val, GetJsonSerializerOptions());
            return true;
        }

        public static void SetAsync<T>(this IMemoryCache cache, string key, T value)
        {
            SetAsync(cache, key, value, new MemoryCacheEntryOptions());
        }
        public static void SetAsync<T> (this IMemoryCache cache, string key, T value, MemoryCacheEntryOptions options)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, GetJsonSerializerOptions()));
            cache.Set(key, bytes, options);
        }
        public static bool TryGetValue<T>(this IMemoryCache cache, string key, out T? value)
        {
            byte[]? val = (byte[]?)cache.Get(key);
            if (val == null)
            {
                value = default;
                return false;
            }
            value = JsonSerializer.Deserialize<T>(val, GetJsonSerializerOptions());
            return true;
        }

        private static JsonSerializerOptions GetJsonSerializerOptions()
        {
            return new JsonSerializerOptions()
            {
                PropertyNamingPolicy = null,
                WriteIndented = true,
                AllowTrailingCommas = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
        }
    }
}
