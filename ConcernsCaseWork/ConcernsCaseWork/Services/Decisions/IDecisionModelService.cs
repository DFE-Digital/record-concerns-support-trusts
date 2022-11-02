using ConcernsCaseWork.Models.CaseActions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Decisions
{
	public interface IDecisionModelService
	{
		public Task<List<DecisionModel>> GetDecisionsByUrn(long urn);
	}
}
