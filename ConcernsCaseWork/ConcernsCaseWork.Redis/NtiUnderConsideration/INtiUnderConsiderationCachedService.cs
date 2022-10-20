using ConcernsCaseWork.Service.NtiUnderConsideration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.NtiUnderConsideration
{
	public interface INtiUnderConsiderationCachedService
	{
		Task<NtiUnderConsiderationDto> CreateNti(NtiUnderConsiderationDto nti);
		Task<NtiUnderConsiderationDto> GetNti(long ntiId);
		Task<ICollection<NtiUnderConsiderationDto>> GetNtisForCase(long caseUrn);
		Task<NtiUnderConsiderationDto> PatchNti(NtiUnderConsiderationDto nti);
	}
}
