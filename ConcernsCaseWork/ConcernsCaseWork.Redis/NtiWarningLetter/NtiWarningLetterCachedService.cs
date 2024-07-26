using ConcernsCaseWork.Redis.Base;
using ConcernsCaseWork.Service.NtiWarningLetter;
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
			await StoreData(continuationId, ntiWarningLetter);
		}

		public async Task<NtiWarningLetterDto> GetNtiWarningLetter(string continuationId)
		{
			return await GetData<NtiWarningLetterDto>(continuationId);
		}
	}
}
