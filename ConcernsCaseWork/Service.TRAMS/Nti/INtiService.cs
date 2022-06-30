using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.Nti
{
	public interface INtiService
	{
		Task<NtiDto> CreateNti(NtiDto ntiDto);
		Task<ICollection<NtiDto>> GetNtisForCase(long caseUrn);
		Task<NtiDto> GetNTIUnderConsiderationById(long underConsiderationId);
	}
}
