using ConcernsCaseWork.Redis.Base;
using ConcernsCaseWork.Service.Nti;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Nti
{
	public class NtiCachedService : CachedService, INtiCachedService
	{
		public NtiCachedService(ICacheProvider cacheProvider) : base(cacheProvider)
		{
		}

		public async Task<NtiDto> GetNtiAsync(string continuationId)
		{
			return await GetData<NtiDto>(continuationId);
		}

		public async Task SaveNtiAsync(NtiDto nti, string continuationId)
		{
			await StoreData(continuationId, nti);
		}
	}
}
