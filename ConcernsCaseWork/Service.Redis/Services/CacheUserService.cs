using Service.Redis.Models;
using System.Threading.Tasks;

namespace Service.Redis.Services
{
	public sealed class CacheUserService : ICacheUserService
	{
		private readonly ICacheProvider _cacheProvider;
		
		public CacheUserService(ICacheProvider cacheProvider)
		{
			_cacheProvider = cacheProvider;
		}
		
		public async Task<UserClaims> GetCachedUser(string key)
		{
			return await _cacheProvider.GetFromCache<UserClaims>(key);
		}

		public async Task ClearCache(string key)
		{
			await _cacheProvider.ClearCache(key);
		}
	}
}