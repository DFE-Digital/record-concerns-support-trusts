using ConcernsCaseWork.Redis.Base;
using ConcernsCaseWork.Service.Nti;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Nti
{
	public class NtiCachedService : CachedService, INtiCachedService
	{
		private const string High_Level_Cache_Key = "NoticeToImprove";
		private readonly INtiService _ntiService;
		private readonly ILogger<NtiCachedService> _logger;

		public NtiCachedService(ICacheProvider cacheProvider,
			INtiService ntiService,
			ILogger<NtiCachedService> logger) : base(cacheProvider)
		{
			_ntiService = ntiService;
			_logger = logger;
		}

		public async Task<NtiDto> CreateNtiAsync(NtiDto newNti)
		{
			var created = await _ntiService.CreateNtiAsync(newNti);

			await CacheNtiAsync(created);
			await ClearNtisForCaseFromCacheAsync(newNti.CaseUrn);

			return created;
		}


		public async Task<NtiDto> GetNtiAsync(long ntiId)
		{
			NtiDto nti = null;

			try
			{
				nti = await GetData<NtiDto>(CreateCacheKeyForNti(ntiId));
			}
			catch (Exception x)
			{
				_logger.LogError(x, "Error occured while trying to get Nti from cache");
				throw;
			}

			if (nti == null)
			{
				nti = await _ntiService.GetNtiAsync(ntiId);
				if (nti != null)
				{
					await CacheNtiAsync(nti);
				}
			}

			return nti;
		}

		public async Task<ICollection<NtiDto>> GetNtisForCaseAsync(long caseUrn)
		{
			ICollection<NtiDto> ntis = null;
			var cacheKey = CreateCacheKeyForNtisForCase(caseUrn);
			try
			{
				ntis = await GetData<ICollection<NtiDto>>(cacheKey);
			}
			catch (Exception x)
			{
				_logger.LogError(x, $"Error occured while trying to get Nti for case {caseUrn} from cache");
				throw;
			}

			if (ntis == null || ntis.Count == 0)
			{
				ntis = await _ntiService.GetNtisForCaseAsync(caseUrn);
				if (ntis != null)
				{
					await StoreData(cacheKey, ntis);
				}
			}

			return ntis;
		}

		public async Task<NtiDto> PatchNtiAsync(NtiDto nti)
		{
			var patched = await _ntiService.PatchNtiAsync(nti);
			if (patched != null)
			{
				await CacheNtiAsync(patched);
				await ClearNtisForCaseFromCacheAsync(patched.CaseUrn);
			}

			return patched;
		}

		private string CreateCacheKeyForNti(long ntiId)
		{
			return $"{High_Level_Cache_Key}:Nti:Id:{ntiId}";
		}

		private string CreateCacheKeyForNtisForCase(long caseUrn) 
		{
			return $"{High_Level_Cache_Key}:NtiForCase:CaseUrn:{caseUrn}";
		}

		private async Task CacheNtiAsync(NtiDto nti)
		{
			try
			{
				if (nti.Id == default(long))
				{
					throw new InvalidOperationException("Nti Id is invalid");
				}

				await StoreData(CreateCacheKeyForNti(nti.Id), nti);
			}
			catch (Exception x)
			{
				_logger.LogError(x, "Error occured while trying to add new Nti to cache");
				throw;
			}
		}

		private async Task ClearNtisForCaseFromCacheAsync(long caseUrn)
		{
			try
			{
				await ClearData(CreateCacheKeyForNtisForCase(caseUrn));
			}
			catch (Exception x)
			{
				_logger.LogError(x, $"Error occured while trying to remove Ntis for case {caseUrn}");
				throw;
			}
		}

		public async Task SaveNti(NtiDto nti, string continuationId)
		{
			if (string.IsNullOrWhiteSpace(continuationId))
			{
				throw new ArgumentNullException(nameof(continuationId));
			}

			await StoreData(continuationId, nti);
		}

		public async Task<NtiDto> GetNti(string continuationId)
		{
			if (string.IsNullOrWhiteSpace(continuationId))
			{
				throw new ArgumentNullException(nameof(continuationId));
			}

			return await GetData<NtiDto>(continuationId);
		}

		public async Task SaveNtiAsync(NtiDto nti, string continuationId)
		{
			if (string.IsNullOrWhiteSpace(continuationId))
			{
				throw new ArgumentNullException(nameof(continuationId));
			}

			await StoreData(continuationId, nti);
		}

		public async Task<NtiDto> GetNtiAsync(string continuationId)
		{
			if (string.IsNullOrWhiteSpace(continuationId))
			{
				throw new ArgumentNullException(nameof(continuationId));
			}

			return await GetData<NtiDto>(continuationId);
		}
	}
}

