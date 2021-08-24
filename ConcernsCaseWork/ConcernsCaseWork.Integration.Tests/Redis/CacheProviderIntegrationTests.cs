using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Service.Redis.Base;
using Service.Redis.Models;
using StackExchange.Redis;
using System;
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
			var configuration = ConfigurationFactory.ConfigurationUserSecretsBuilder();
			
			var vCapConfiguration = JObject.Parse(configuration["VCAP_SERVICES"]);
			var redisCredentials = vCapConfiguration["redis"]?[0]?["credentials"];
			
			Assert.NotNull(redisCredentials);
			
			var password = (string)redisCredentials["password"];
			var host = (string)redisCredentials["host"];
			var port = (string)redisCredentials["port"];
			var tls = (bool)redisCredentials["tls_enabled"];

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