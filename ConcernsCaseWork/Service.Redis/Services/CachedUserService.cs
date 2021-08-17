using Microsoft.Extensions.Caching.Distributed;
using Service.Redis.Models;
using System;
using System.Threading.Tasks;

namespace Service.Redis.Services
{
	public sealed class CachedUserService : ICachedUserService
	{
		private const int CacheTimeToLive = 120;
		private readonly IActiveDirectoryService _activeDirectoryService;
		private readonly ICacheProvider _cacheProvider;
		
		public CachedUserService(IActiveDirectoryService activeDirectoryService, ICacheProvider cacheProvider)
		{
			_activeDirectoryService = activeDirectoryService;
			_cacheProvider = cacheProvider;
		}
		
		/// <summary>
		/// Cached user key: email
		/// </summary>
		/// <param name="userCredentials"></param>
		/// <returns></returns>
		public async Task<UserClaims> GetUserAsync(UserCredentials userCredentials)
		{
			var userClaims = await _cacheProvider.GetFromCache<UserClaims>(userCredentials.Email);
			if (userClaims != null) return userClaims;

			userClaims = await _activeDirectoryService.GetUserAsync(userCredentials);
			
			var cacheEntryOptions = new DistributedCacheEntryOptions()
				.SetSlidingExpiration(TimeSpan.FromSeconds(CacheTimeToLive)); 
                
			await _cacheProvider.SetCache(userCredentials.Email, userClaims, cacheEntryOptions);

			return userClaims;
		}
	}
}