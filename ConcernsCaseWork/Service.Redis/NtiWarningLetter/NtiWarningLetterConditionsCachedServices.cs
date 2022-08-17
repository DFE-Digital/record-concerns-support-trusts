using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using ConcernsCasework.Service.NtiWarningLetter;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Redis.NtiWarningLetter
{
	public class NtiWarningLetterConditionsCachedServices : CachedService, INtiWarningLetterConditionsCachedService
	{
		private readonly INtiWarningLetterConditionsService _ntiWarningLetterConditionsService;
		private readonly ILogger<NtiWarningLetterConditionsCachedServices> _logger;

		private const string CacheKey = "Nti.WarningLetter.Conditions";

		public NtiWarningLetterConditionsCachedServices(ICacheProvider cacheProvider,
			INtiWarningLetterConditionsService ntiWarningLetterConditionsService,
			ILogger<NtiWarningLetterConditionsCachedServices> logger) : base(cacheProvider)
		{
			_ntiWarningLetterConditionsService = ntiWarningLetterConditionsService;
			_logger = logger;
		}

		public async Task<ICollection<NtiWarningLetterConditionDto>> GetAllConditionsAsync()
		{
			ICollection<NtiWarningLetterConditionDto> conditions = null;

			try
			{
				conditions = await GetData<ICollection<NtiWarningLetterConditionDto>>(CacheKey);
			}
			catch (Exception x)
			{
				_logger.LogError(x, "Error occured while trying to get Nti Warning Letter Conditions from cache");
				throw;
			}

			if (conditions == null || conditions.Count == 0)
			{
				conditions = await _ntiWarningLetterConditionsService.GetAllConditionsAsync();
				if(conditions?.Count > 0)
				{
					await StoreData(CacheKey, conditions);
				}
			}

			return conditions;
		}
	}
}
