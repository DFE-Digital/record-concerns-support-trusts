using Service.TRAMS.NtiUnderConsideration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Redis.NtiUnderConsideration
{
	public interface INtiUnderConsiderationCachedService
	{
		Task<NtiUnderConsiderationDto> CreateNti(NtiUnderConsiderationDto nti);
		Task<NtiUnderConsiderationDto> GetNti(long ntiId);
		Task<ICollection<NtiUnderConsiderationDto>> GetNtisForCase(long caseUrn);
		Task<NtiUnderConsiderationDto> PatchNti(NtiUnderConsiderationDto nti);
	}
}
