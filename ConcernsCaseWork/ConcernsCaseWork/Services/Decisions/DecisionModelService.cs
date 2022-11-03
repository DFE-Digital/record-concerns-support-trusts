using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Service.Decision;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Decisions
{
	public class DecisionModelService : IDecisionModelService
	{
		private readonly IDecisionService _decisionService;

		public DecisionModelService(IDecisionService decisionService)
		{
			_decisionService = decisionService;
		}

		public async Task<List<ActionSummaryModel>> GetDecisionsByUrn(long urn)
		{
			var decisions = await _decisionService.GetDecisionsByCaseUrn(urn);


			return decisions.Select(d => DecisionMapping.ToActionSummary(d)).ToList();
		}
	}
}
