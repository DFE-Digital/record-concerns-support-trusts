using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.TRAMS.CaseActions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Redis.CaseActions
{
	public class CachedSRMAProvider : CachedService
	{
		private readonly ICacheProvider _cacheProvider;
		private readonly SRMAProvider _srmaProvider;
		private readonly ILogger<CachedSRMAProvider> _logger;

		public CachedSRMAProvider(ICacheProvider cacheProvider, SRMAProvider srmaProvider, ILogger<CachedSRMAProvider> logger) : base(cacheProvider)
		{
			_cacheProvider = cacheProvider;
			_srmaProvider = srmaProvider;
			_logger = logger;
		}

		public async Task<SRMADto> GetSRMAById(long srmaId)
		{
			var cacheKey = GetCacheKeyForSrmaBySrmaIdKey(srmaId);

			var srma = await GetData<SRMADto>(cacheKey);

			if (srma == null)
			{
				srma = await _srmaProvider.GetSRMAById(srmaId);

				if (srma != null)
				{
					await StoreData<SRMADto>(cacheKey, srma);
				}
			}

			return srma;
		}

		public async Task<SRMADto> SaveSRMA(SRMADto srma)
		{
			var created = await _srmaProvider.SaveSRMA(srma);
			if (created != null)
			{
				var cacheKey = GetCacheKeyForSrmaBySrmaIdKey(srma.Id);
				await StoreData<SRMADto>(cacheKey, created);
			}

			return created;
		}

		public async Task<List<SRMADto>> GetSRMAsForCase(long caseUrn)
		{
			var cacheKey = $"SRMAsForCase:CaseId:{caseUrn}";

			var srmas = await GetData<List<SRMADto>>(cacheKey);

			if(srmas == null || srmas.Count == 0)
			{
				srmas = await _srmaProvider.GetSRMAsForCase(caseUrn);
				
				if (srmas?.Count > 0)
				{
					await StoreData<List<SRMADto>>(cacheKey, srmas);
				}
			}

			return srmas;
		}

		public async Task<SRMADto> SetDateAccepted(long srmaId, DateTime? acceptedDate)
		{ 
			var patched = await _srmaProvider.SetDateAccepted(srmaId, acceptedDate);
			if (patched != null)
			{
				var cacheKey = GetCacheKeyForSrmaBySrmaIdKey(srmaId);
				await StoreData<SRMADto>(cacheKey, patched);
			}

			return patched;
		}

		private string GetCacheKeyForSrmaBySrmaIdKey(long srmaId)
		{
			return $"SRMA:SRMAId:{srmaId}";
		}
	}
}
