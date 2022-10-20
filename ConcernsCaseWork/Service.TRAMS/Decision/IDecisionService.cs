using System;
using System.Threading.Tasks;

namespace Service.TRAMS.Decision
{
	public interface IDecisionService
	{
		Task PostDecision(CreateDecisionDto createDecisionDto);
	}
}

