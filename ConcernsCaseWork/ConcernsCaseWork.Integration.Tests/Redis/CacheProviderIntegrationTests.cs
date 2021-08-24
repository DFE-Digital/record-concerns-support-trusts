using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Service.Redis.Models;
using Service.Redis.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Integration.Tests.Redis
{
	[Parallelizable(ParallelScope.All)]
	public class CacheProviderIntegrationTests
	{
		[Test]
		public async Task WhenGetFromCache_IsSuccessful()
		{
			// arrange
			const int cacheTimeToLive = 120;
			var initialData = new Dictionary<string, string>
			{
				{ "VCAP_SERVICES:redis:0:credentials:host", "127.0.0.1" }, 
				{ "VCAP_SERVICES:redis:0:credentials:password", "password" }, 
				{ "VCAP_SERVICES:redis:0:credentials:port", "6379" }
			};
			
			var configuration = new ConfigurationBuilder().AddInMemoryCollection(initialData).Build();
			var redisCredential = configuration.GetSection("VCAP_SERVICES:redis:0");
			var host = redisCredential["credentials:host"];
			var password = redisCredential["credentials:password"];
			var port = redisCredential["credentials:port"];
			var tls = redisCredential["credentials:tls_enabled"] is { } && Boolean.Parse(redisCredential["credentials:tls_enabled"]);

			var redisConfigurationOptions = new ConfigurationOptions
			{
				Password = password,
				EndPoints = {$"{host}:{port}"},
				Ssl = tls
			};
			var redisCacheOptions = new RedisCacheOptions { ConfigurationOptions = redisConfigurationOptions };
			var redisCache = new RedisCache(redisCacheOptions);
			var cacheProvider = new CacheProvider(redisCache);

			var cacheEntryOptions = new DistributedCacheEntryOptions()
				.SetSlidingExpiration(TimeSpan.FromSeconds(cacheTimeToLive));
			
			var userClaims = new UserClaims
			{
				Email = "test@email.com", 
				Id = "test"
			};
			
			// act
			await cacheProvider.SetCache(userClaims.Email, userClaims, cacheEntryOptions);
			var cachedUserClaim = await cacheProvider.GetFromCache<UserClaims>(userClaims.Email);

			// assert
			Assert.That(cachedUserClaim, Is.Not.Null);
			Assert.That(cachedUserClaim, Is.InstanceOf<UserClaims>());
			Assert.That(cachedUserClaim.Email, Is.EqualTo(userClaims.Email));
			Assert.That(cachedUserClaim.Id, Is.EqualTo(userClaims.Id));
			
			// clean up
			await cacheProvider.ClearCache(cachedUserClaim.Email);
			cachedUserClaim = await cacheProvider.GetFromCache<UserClaims>(userClaims.Email);
			
			Assert.That(cachedUserClaim, Is.Null);
		}
	}
}