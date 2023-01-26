using Ardalis.GuardClauses;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Redis.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Base
{
	public sealed class CacheProvider : ICacheProvider
	{
		private readonly IDistributedCache _cache;
		private readonly ILogger<CacheProvider> _logger;
		private readonly int _cacheTtl;

		public CacheProvider(IDistributedCache cache, IOptions<CacheOptions> options, ILogger<CacheProvider> logger)
		{
			_cache = Guard.Against.Null(cache);
			_cacheTtl = Guard.Against.Null(options.Value.TimeToLive);
			_logger = Guard.Against.Null(logger);
		}

		public int CacheTimeToLive()
		{
			return _cacheTtl;
		}

		public async Task<T> GetFromCache<T>(string key) where T : class
		{
			_logger.LogMethodEntered();
			Guard.Against.NullOrWhiteSpace(key);
			try
			{
				var cachedData = await _cache.GetStringAsync(key);
				return cachedData == null ? null : JsonConvert.DeserializeObject<T>(cachedData);
			}
			catch (Exception e)
			{
#if DEBUG
				if (Debugger.IsAttached)
				{
					Debugger.Break();
				}
#endif
				_logger.LogErrorMsg(e);
				// Do not bubble up exception. Return null instead.
				return null;
			}
		}

		public async Task SetCache<T>(string key, T value, DistributedCacheEntryOptions options) where T : class
		{
			_logger.LogMethodEntered();
			Guard.Against.NullOrWhiteSpace(key);

			// do not store null values for cache, it's unnecessary
			if (value == null)
			{
				await ClearCache(key);
				return;
			}

			try
			{
				var user = JsonConvert.SerializeObject(value);
				await _cache.SetStringAsync(key, user, options);
			}
			catch (Exception e)
			{
#if DEBUG
				if (Debugger.IsAttached)
				{
					Debugger.Break();
				}
#endif
				// Do not bubble up the exception.
				_logger.LogErrorMsg(e);
			}
		}

		public async Task ClearCache(string key)
		{
			_logger.LogMethodEntered();
			Guard.Against.NullOrWhiteSpace(key);

			try
			{
				await _cache.RemoveAsync(key);

			}
			catch (Exception e)
			{
#if DEBUG
				if (Debugger.IsAttached)
				{
					Debugger.Break();
				}
#endif
				// Do not bubble up the exception.
				_logger.LogErrorMsg(e);
			}
		}
	}
}