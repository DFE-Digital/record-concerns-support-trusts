using System.Threading.Tasks;

namespace Service.TRAMS.Decision
{
	public interface IDecisionService
	{
		Task<CreateDecisionResponseDto> PostDecision(CreateDecisionDto createDecisionDto);
	}
}

