using ConcernsCaseWork.Redis.Base;
using ConcernsCaseWork.Service.NtiWarningLetter;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.NtiWarningLetter
{
	public class NtiWarningLetterCachedService : CachedService, INtiWarningLetterCachedService
	{
		public NtiWarningLetterCachedService(ICacheProvider cacheProvider) : base(cacheProvider)
		{
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
