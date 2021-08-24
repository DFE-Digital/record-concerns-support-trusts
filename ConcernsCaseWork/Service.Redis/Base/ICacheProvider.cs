using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;

namespace Service.Redis.Base
{
	public interface ICacheProvider
	{
		int CacheTimeToLive();
		Task<T> GetFromCache<T>(string key) where T : class;
		Task SetCache<T>(string key, T value, DistributedCacheEntryOptions options) where T : class;
		Task ClearCache(string key);
	}
}