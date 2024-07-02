using ConcernsCaseWork.Service.Nti;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Nti
{
	public interface INtiCachedService
	{
		Task SaveNtiAsync(NtiDto nti, string continuationId);
		Task<NtiDto> GetNtiAsync(string continuationId);
	}
}
