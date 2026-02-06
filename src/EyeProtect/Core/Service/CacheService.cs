using System.Linq;
using System.Runtime.Caching;

namespace EyeProtect.Core.Service
{
    /// <summary>
    /// 内存缓存 Service
    /// </summary>
    public class CacheService : IService
    {
        private readonly ObjectCache cache;
        public CacheService()
        {
            cache = MemoryCache.Default;
        }
        public void Init()
        {

        }
        public object this[string key]
        {
            get
            {
                var result = cache.Get(key);
                if (result != null)
                {
                    cache.Remove(key);
                }
                return result;
            }

            set => cache.Set(key, value, null);
        }

        public void Dispose()
        {
            var keys = cache.Select(kvp => kvp.Key).ToList();
            foreach (var key in keys)
            {
                cache.Remove(key);
            }
        }
    }
}
