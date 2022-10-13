using System;
using System.Threading.Tasks;

namespace Service.TRAMS.Decision
{
	public interface IDecisionService
	{
		Task<DecisionDto> PostDecision(DecisionDto createDecisionDto);
	}
}

