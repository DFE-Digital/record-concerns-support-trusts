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
			: base(cacheProvider)
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


		public async Task<NtiDto> GetNTIUnderConsiderationById(long underConsiderationId)
		{
			var key = GetCacheKeyForNti(underConsiderationId);
			var ntiUnderConsideration = await GetData<NtiDto>(key);


			if (ntiUnderConsideration == null)
			{
				ntiUnderConsideration = await _ntiTramsService.GetNTIUnderConsiderationById(underConsiderationId);
				await StoreData<NtiDto>(key, ntiUnderConsideration);
			}

			return ntiUnderConsideration;


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
				var ntiCacheKey = GetCacheKeyForNti(nti);
				await StoreData(ntiCacheKey, patched);

				var cacheKeyForCase = GetCacheKeyForNtisForCase(nti.CaseUrn);
				await ClearData(cacheKeyForCase);
			}

			return patched;
		}

		public async Task<NtiDto> GetNti(long ntiId)
		{
			var cacheKey = GetCacheKeyForNti(new NtiDto { Id = ntiId });
			var nti = await GetData<NtiDto>(cacheKey);
			if(nti == null)
			{
				nti = await _ntiTramsService.GetNti(ntiId);
				await StoreData<NtiDto>(cacheKey, nti);	
			}

			return nti;
		}
	}
}
