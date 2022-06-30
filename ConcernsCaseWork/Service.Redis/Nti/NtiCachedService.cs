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
		private readonly INtiService _ntiTramsService;

		public NtiCachedService(INtiService ntiTramsService,
			ICacheProvider cacheProvider,
			ILogger<NtiCachedService> logger)
			:base(cacheProvider)
		{
			_ntiTramsService = ntiTramsService;
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

			var key = GetCacheKeyForNti(nti);

			await StoreData<NtiDto>(key, nti);
			await ClearData(GetCacheKeyForNtisForCase(nti.CaseUrn));
		}

		private string GetCacheKeyForNti(NtiDto nti)
		{
			return $"Nti:NtiId:{nti.Id}";
		}

		private string GetCacheKeyForNtisForCase(long caseUrn)
		{
			return $"NtisForCase:CaseUrn:{caseUrn}";
		}

		public async Task<ICollection<NtiDto>> GetNtisForCase(long caseUrn)
		{
			var cacheKey = GetCacheKeyForNtisForCase(caseUrn);
			var ntis = await GetData<ICollection<NtiDto>>(cacheKey);
			if(ntis==null || ntis.Count == 0)
			{
				ntis = await _ntiTramsService.GetNtisForCase(caseUrn);
				await StoreData<ICollection<NtiDto>>(cacheKey, ntis);
			}

			return ntis;
		}


		public async Task<NtiDto> GetNTIUnderConsiderationById(long underConsiderationId)
		{
			var key = $"Nti:NtiId:{underConsiderationId}";
			var ntiUnderConsideration = await GetData<NtiDto>(key);


			if (ntiUnderConsideration == null)
			{
				ntiUnderConsideration = await _ntiTramsService.GetNTIUnderConsiderationById(underConsiderationId);
				await StoreData<NtiDto>(key, ntiUnderConsideration);
			}

			return ntiUnderConsideration;


		}
	}
}
