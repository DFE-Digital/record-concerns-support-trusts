using ConcernsCaseWork.Models.CaseActions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	public interface ISRMAService
	{
		public Task SaveSRMA(SRMA srma);
		public Task<IEnumerable<SRMA>> GetSRMAsForCase(long caseUrn);
	}
}