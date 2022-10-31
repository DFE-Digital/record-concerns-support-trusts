using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.Decision
{
	public interface IDecisionService
	{
		Task<CreateDecisionResponseDto> PostDecision(CreateDecisionDto createDecisionDto);

		Task<List<GetDecisionResponseDto>> GetDecisionsByCaseUrn(long urn);
	}
}

