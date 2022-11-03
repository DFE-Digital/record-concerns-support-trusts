using ConcernsCaseWork.Service.NtiUnderConsideration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.NtiUnderConsideration
{
	public interface INtiUnderConsiderationStatusesCachedService
	{
		Task ClearData();
		Task<ICollection<NtiUnderConsiderationStatusDto>> GetAllStatuses();
	}
}
