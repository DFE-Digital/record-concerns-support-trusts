using ConcernsCaseWork.Service.NtiUnderConsideration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.NtiUnderConsideration
{
	public interface INtiUnderConsiderationReasonsCachedService
	{
		Task ClearData();
		Task<ICollection<NtiUnderConsiderationReasonDto>> GetAllReasons();
	}
}
