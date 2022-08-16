using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.TRAMS.Nti;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Redis.Nti
{
	public class NtiCachedService : CachedService, INtiCachedService
	{
		private const string High_Level_Cache_Key = "NoticeToImprove";
		private readonly INtiService _ntiService;
		private readonly ILogger<NtiCachedService> _logger;

		public NtiCachedService(ICacheProvider cacheProvider, 
			INtiService ntiService, ILogger<NtiCachedService> logger) : base(cacheProvider)
		{
			_ntiService = ntiService;
			_logger = logger;
		}

		public async Task<ICollection<NtiDto>> GetNtisForCase(long caseUrn)
		{
			ICollection<NtiDto> ntis = null;
			var cacheKey = CreateCacheKeyForNtiWarningLettersForCase(caseUrn);
			try
			{
				ntis = await GetData<ICollection<NtiDto>>(cacheKey);
			}
			catch (Exception x)
			{
				_logger.LogError(x, $"Error occured while trying to get Ntis for case {caseUrn} from cache");
				throw;
			}

			if (ntis == null)
			{
				ntis = await _ntiService.GetNtisForCaseAsync(caseUrn);
				if (ntis != null)
				{
					await StoreData(cacheKey, ntis);
				}
			}

			return ntis;
		}

		private string CreateCacheKeyForNtiWarningLettersForCase(long caseUrn)
		{
			return $"{High_Level_Cache_Key}:NtisForCase:Case:{caseUrn}";
		}
	}
}

