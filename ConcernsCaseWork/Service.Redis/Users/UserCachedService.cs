using Microsoft.Extensions.Caching.Distributed;
using Service.Redis.Base;
using Service.Redis.Models;
using System;
using System.Threading.Tasks;

namespace Service.Redis.Users
{
	public sealed class UserCachedService : IUserCachedService
	{
		private readonly IActiveDirectoryService _activeDirectoryService;
		private readonly ICacheProvider _cacheProvider;
		
		public UserCachedService(IActiveDirectoryService activeDirectoryService, ICacheProvider cacheProvider)
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

			if (userClaims == null) return null;
			
			var cacheEntryOptions = new DistributedCacheEntryOptions()
				.SetSlidingExpiration(TimeSpan.FromSeconds(_cacheProvider.CacheTimeToLive())); 
	                
			await _cacheProvider.SetCache(userCredentials.Email, userClaims, cacheEntryOptions);

			return userClaims;
		}
	}
}