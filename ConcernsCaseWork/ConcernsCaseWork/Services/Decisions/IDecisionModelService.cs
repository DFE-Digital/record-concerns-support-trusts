using ConcernsCaseWork.Models.CaseActions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Decisions
{
	public interface IDecisionModelService
	{
		public Task<List<ActionSummaryModel>> GetDecisionsByUrn(long urn);
	}
}
