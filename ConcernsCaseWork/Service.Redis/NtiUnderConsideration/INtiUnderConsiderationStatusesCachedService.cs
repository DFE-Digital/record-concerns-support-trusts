using ConcernsCasework.Service.NtiUnderConsideration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.NtiUnderConsideration
{
	public interface INtiUnderConsiderationStatusesCachedService
	{
		Task ClearData();
		Task<ICollection<NtiUnderConsiderationStatusDto>> GetAllStatuses();
	}
}
