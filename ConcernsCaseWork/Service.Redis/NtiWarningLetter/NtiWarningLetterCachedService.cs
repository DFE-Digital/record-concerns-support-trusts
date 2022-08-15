using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.TRAMS.NtiWarningLetter;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Redis.NtiWarningLetter
{
	public class NtiWarningLetterCachedService : CachedService, INtiWarningLetterCachedService
	{
		private readonly INtiWarningLetterService _ntiWarningLetterService;
		private readonly ILogger<NtiWarningLetterCachedService> _logger;

		private const string High_Level_Cache_Key = "NtiWarningLetter";

		public NtiWarningLetterCachedService(ICacheProvider cacheProvider,
			INtiWarningLetterService ntiWarningLetterService,
			ILogger<NtiWarningLetterCachedService> logger) : base(cacheProvider)
		{
			_ntiWarningLetterService = ntiWarningLetterService;
			_logger = logger;
		}

		public async Task<NtiWarningLetterDto> CreateNtiWarningLetterAsync(NtiWarningLetterDto newNtiWarningLetter)
		{
			var created = await _ntiWarningLetterService.CreateNtiWarningLetterAsync(newNtiWarningLetter);

			await CacheNtiWLAsync(created);
			await ClearNtiWLForCaseFromCacheAsync(newNtiWarningLetter.CaseUrn);

			return created;
		}


		public async Task<NtiWarningLetterDto> GetNtiWarningLetterAsync(long ntiWarningLetterId)
		{
			NtiWarningLetterDto nti = null;

			try
			{
				nti = await GetData<NtiWarningLetterDto>(CreateCacheKeyForNtiWarningLetter(ntiWarningLetterId));
			}
			catch (Exception x)
			{
				_logger.LogError(x, "Error occured while trying to get Nti Warning Letter from cache");
				throw;
			}

			if (nti == null)
			{
				nti = await _ntiWarningLetterService.GetNtiWarningLetterAsync(ntiWarningLetterId);
				if (nti != null)
				{
					await CacheNtiWLAsync(nti);
				}
			}

			return nti;
		}

		public async Task<ICollection<NtiWarningLetterDto>> GetNtiWarningLettersForCaseAsync(long caseUrn)
		{
			ICollection<NtiWarningLetterDto> ntis = null;
			var cacheKey = CreateCacheKeyForNtiWarningLettersForCase(caseUrn);
			try
			{
				ntis = await GetData<ICollection<NtiWarningLetterDto>>(cacheKey);
			}
			catch (Exception x)
			{
				_logger.LogError(x, $"Error occured while trying to get Nti Warning Letters for case {caseUrn} from cache");
				throw;
			}

			if (ntis == null)
			{
				ntis = await _ntiWarningLetterService.GetNtiWarningLettersForCaseAsync(caseUrn);
				if (ntis != null)
				{
					await StoreData(cacheKey, ntis);
				}
			}

			return ntis;
		}

		public async Task<NtiWarningLetterDto> PatchNtiWarningLetterAsync(NtiWarningLetterDto ntiWarningLetter)
		{
			var patched = await _ntiWarningLetterService.PatchNtiWarningLetterAsync(ntiWarningLetter);
			if (patched != null)
			{
				await CacheNtiWLAsync(patched);
				await ClearNtiWLForCaseFromCacheAsync(patched.CaseUrn);
			}

			return patched;
		}

		private string CreateCacheKeyForNtiWarningLetter(long ntiWarningLetterId)
		{
			return $"{High_Level_Cache_Key}:NtiWarningLetter:Id:{ntiWarningLetterId}";
		}

		private string CreateCacheKeyForNtiWarningLettersForCase(long caseUrn)
		{
			return $"{High_Level_Cache_Key}:NtiWarningLetter:Case:{caseUrn}";
		}

		private async Task CacheNtiWLAsync(NtiWarningLetterDto ntiWL)
		{
			try
			{
				if (ntiWL.Id == default(long))
				{
					throw new InvalidOperationException("Nti Warning Letter Id is invalid");
				}

				await StoreData(CreateCacheKeyForNtiWarningLetter(ntiWL.Id), ntiWL);
			}
			catch (Exception x)
			{
				_logger.LogError(x, "Error occured while trying to add new Nti Warning Letter to cache");
				throw;
			}
		}

		private async Task ClearNtiWLForCaseFromCacheAsync(long caseUrn)
		{
			try
			{
				await ClearData(CreateCacheKeyForNtiWarningLettersForCase(caseUrn));
			}
			catch (Exception x)
			{
				_logger.LogError(x, $"Error occured while trying to remove Nti Warning Letters for case {caseUrn}");
				throw;
			}
		}

		public async Task SaveNtiWarningLetter(NtiWarningLetterDto ntiWarningLetter, string continuationId)
		{
			if (string.IsNullOrWhiteSpace(continuationId))
			{
				throw new ArgumentNullException(nameof(continuationId));
			}

			await StoreData(continuationId, ntiWarningLetter);
		}

		public async Task<NtiWarningLetterDto> GetNtiWarningLetter(string continuationId)
		{
			if (string.IsNullOrWhiteSpace(continuationId))
			{
				throw new ArgumentNullException(nameof(continuationId));
			}

			return await GetData<NtiWarningLetterDto>(continuationId);
		}
	}
}
