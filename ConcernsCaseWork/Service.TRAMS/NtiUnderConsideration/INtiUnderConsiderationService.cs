using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.NtiUnderConsideration
{
	public interface INtiUnderConsiderationService
	{
		Task<NtiUnderConsiderationDto> CreateNti(NtiUnderConsiderationDto ntiDto);
		Task<NtiUnderConsiderationDto> GetNti(long ntiId);
		Task<ICollection<NtiUnderConsiderationDto>> GetNtisForCase(long caseUrn);
		Task<NtiUnderConsiderationDto> PatchNti(NtiUnderConsiderationDto ntiDto);
	}
}
