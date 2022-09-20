using ConcernsCasework.Service.NtiUnderConsideration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.NtiUnderConsideration
{
	public interface INtiUnderConsiderationReasonsCachedService
	{
		Task ClearData();
		Task<ICollection<NtiUnderConsiderationReasonDto>> GetAllReasons();
	}
}
