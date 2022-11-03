using ConcernsCaseWork.Redis.Base;
using ConcernsCaseWork.Service.Nti;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Nti
{
	public class NtiConditionsCachedService : CachedService, INtiConditionsCachedService
	{
		private const string CacheKey = "Nti.NoticeToImprove.Conditions";
		private readonly INtiConditionsService _ntiConditionsService;
		private readonly ILogger<NtiConditionsCachedService> _logger;

		public NtiConditionsCachedService(ICacheProvider cacheProvider,
			INtiConditionsService ntiConditionsService,
			ILogger<NtiConditionsCachedService> logger) : base(cacheProvider)
		{
			_ntiConditionsService = ntiConditionsService;
			_logger = logger;
		}

		public async Task<ICollection<NtiConditionDto>> GetAllConditionsAsync()
		{
			ICollection<NtiConditionDto> conditions = null;

			try
			{
				conditions = await GetData<ICollection<NtiConditionDto>>(CacheKey);
			}
			catch (Exception x)
			{
				_logger.LogError(x, "Error occured while trying to get Nti Conditions from cache");
				throw;
			}

			if (conditions == null || conditions.Count == 0)
			{
				conditions = await _ntiConditionsService.GetAllConditionsAsync();
				if (conditions?.Count > 0)
				{
					await StoreData(CacheKey, conditions);
				}
			}

			return conditions;
		}

	}
}
