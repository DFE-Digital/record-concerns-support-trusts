using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.TRAMS.Nti;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Redis.NtiUnderConsideration
{
	public class NtiCachedService : CachedService, INtiCachedService
	{
		private readonly INtiService _ntiTramsService;
		private readonly INtiReasonsCachedService _reasonsCachedService;

		public NtiCachedService(INtiService ntiTramsService,
			ICacheProvider cacheProvider,
			INtiReasonsCachedService reasonsCachedService,
			ILogger<NtiCachedService> logger)
			: base(cacheProvider)
		{
			_ntiTramsService = ntiTramsService;
			_reasonsCachedService = reasonsCachedService;
		}

		public async Task<NtiDto> CreateNti(NtiDto nti)
		{
			var createdNti = await _ntiTramsService.CreateNti(nti);
			if (createdNti == null)
			{
				throw new InvalidOperationException("Created Nti record cannot be null");
			}

			await UpdateCachedNti(createdNti);
			return createdNti;
		}

		private async Task UpdateCachedNti(NtiDto nti)
		{
			if (nti.Id == default(long))
			{
				throw new InvalidOperationException($"Nti is not valid to be cached (Id:{nti.Id})");
			}

			var key = GetCacheKeyForNti(nti.Id);

			await StoreData<NtiDto>(key, nti);
			await ClearData(GetCacheKeyForNtisForCase(nti.CaseUrn));
		}

		private string GetCacheKeyForNti(long underConsiderationId)
		{
			return $"Nti:NtiId:{underConsiderationId}";
		}

		private string GetCacheKeyForNtisForCase(long caseUrn)
		{
			return $"NtisForCase:CaseUrn:{caseUrn}";
		}

		public async Task<ICollection<NtiDto>> GetNtisForCase(long caseUrn)
		{
			var cacheKey = GetCacheKeyForNtisForCase(caseUrn);
			var ntis = await GetData<ICollection<NtiDto>>(cacheKey);
			if (ntis == null || ntis.Count == 0)
			{
				ntis = await _ntiTramsService.GetNtisForCase(caseUrn);
				await StoreData<ICollection<NtiDto>>(cacheKey, ntis);
			}

			return ntis;
		}

		public async Task<NtiDto> PatchNti(NtiDto nti)
		{
			NtiDto patched = null;

			if (nti.Id == default(long))
			{
				throw new InvalidOperationException($"Nti doesn't have a valid Id");
			}

			patched = await _ntiTramsService.PatchNti(nti);
			if (patched?.Id != default(long))
			{
				var ntiCacheKey = GetCacheKeyForNti(nti.Id);
				await StoreData(ntiCacheKey, patched);

				var cacheKeyForCase = GetCacheKeyForNtisForCase(nti.CaseUrn);
				await ClearData(cacheKeyForCase);
			}

			return patched;
		}

		public async Task<NtiDto> GetNti(long ntiId)
		{
			var cacheKey = GetCacheKeyForNti(ntiId);
			var nti = await GetData<NtiDto>(cacheKey);
			if (nti == null || nti.Reasons == null)
			{
				nti = await _ntiTramsService.GetNti(ntiId);
				nti.Reasons = await GetReasons(nti);
				await StoreData<NtiDto>(cacheKey, nti);
			}

			return nti;
		}

		private async Task<List<NtiReasonDto>> GetReasons(NtiDto ntiDto)
		{
			var allReasons = await _reasonsCachedService.GetAllReasons();
			var reasonsForNti = new List<NtiReasonDto>();
			foreach (var reasonId in ntiDto.UnderConsiderationReasonsMapping)
			{
				var matchingReason = allReasons.SingleOrDefault(r => r.Id == reasonId) ?? throw new InvalidOperationException($"A matching NTI Under Consideration reason could not find with the Id:{reasonId}");
				reasonsForNti.Add(matchingReason);
			}

			return reasonsForNti;
		}
	}
}
