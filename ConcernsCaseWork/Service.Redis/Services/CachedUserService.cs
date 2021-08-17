using Microsoft.Extensions.Caching.Distributed;
using Service.Redis.Models;
using System;
using System.Threading.Tasks;

namespace Service.Redis.Services
{
	public sealed class CachedUserService : IUserService
	{
		private const int CacheTimeToLive = 120;
		private readonly UserService _userService;
		private readonly ICacheProvider _cacheProvider;
		
		public CachedUserService(UserService userService, ICacheProvider cacheProvider)
		{
			_userService = userService;
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

			userClaims = await _userService.GetUserAsync(userCredentials);
			
			var cacheEntryOptions = new DistributedCacheEntryOptions()
				.SetSlidingExpiration(TimeSpan.FromSeconds(CacheTimeToLive)); 
                
			await _cacheProvider.SetCache(userCredentials.Email, userClaims, cacheEntryOptions);

			return userClaims;
		}
	}
}