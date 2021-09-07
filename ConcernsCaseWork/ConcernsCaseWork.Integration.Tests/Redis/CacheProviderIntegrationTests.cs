using ConcernsCaseWork.Integration.Tests.Factory;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Service.Redis.Base;
using Service.Redis.Configuration;
using Service.Redis.Models;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Integration.Tests.Redis
{
	[TestFixture]
	public class CacheProviderIntegrationTests
	{
		/// Testing the class requires a running Redis,
		/// startup is configured to use Redis with session storage.
		private IConfigurationRoot _configuration;
		private WebAppFactory _factory;
		
		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			_configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().ConfigurationJsonFile().Build();
			_factory = new WebAppFactory(_configuration);
		}
		
		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			_factory.Dispose();
		}
		
		[Test]
		public async Task WhenGetFromCache_IsSuccessful()
		{
			// arrange
			const int cacheTimeToLive = 120;
			
			var cacheTtl = _configuration.GetSection(CacheOptions.Cache).Get<CacheOptions>();
			Assert.NotNull(cacheTtl);
			
			var vCapConfiguration = JObject.Parse(_configuration["VCAP_SERVICES"]);
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
			var cacheProvider = new CacheProvider(redisCache, Options.Create(cacheTtl));

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
		
		[Test]
		public async Task WhenGetCasesStateDataFromCache_IsSuccessful()
		{
			// arrange
			const int cacheTimeToLive = 120;
			
			var cacheTtl = _configuration.GetSection(CacheOptions.Cache).Get<CacheOptions>();
			Assert.NotNull(cacheTtl);
			
			var vCapConfiguration = JObject.Parse(_configuration["VCAP_SERVICES"]);
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
			var cacheProvider = new CacheProvider(redisCache, Options.Create(cacheTtl));

			var cacheEntryOptions = new DistributedCacheEntryOptions()
				.SetSlidingExpiration(TimeSpan.FromSeconds(cacheTimeToLive));
			
			var caseStateData = new CasesStateData
			{
				TrustUkPrn = "999999"
			};
			
			// act
			await cacheProvider.SetCache("test@email.com", caseStateData, cacheEntryOptions);
			var cachedCaseStateData = await cacheProvider.GetFromCache<CasesStateData>("test@email.com");

			// assert
			Assert.That(cachedCaseStateData, Is.Not.Null);
			Assert.That(cachedCaseStateData, Is.InstanceOf<CasesStateData>());
			Assert.That(cachedCaseStateData.TrustUkPrn, Is.EqualTo(caseStateData.TrustUkPrn));

			// clean up
			await cacheProvider.ClearCache("test@email.com");
			cachedCaseStateData = await cacheProvider.GetFromCache<CasesStateData>("test@email.com");
			
			Assert.That(cachedCaseStateData, Is.Null);
		}
	}
}