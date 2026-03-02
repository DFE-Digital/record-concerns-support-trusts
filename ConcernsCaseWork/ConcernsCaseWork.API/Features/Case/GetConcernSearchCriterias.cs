using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Features.Case
{
	public interface IGetConcernSearchCriterias
	{
		CaseSearchParametersResponse Execute();
	}

	public class GetConcernSearchCriterias : IGetConcernSearchCriterias
	{
		private readonly IConcernsCaseGateway _concernsCaseGateway;

		public GetConcernSearchCriterias(IConcernsCaseGateway concernsCaseGateway)
		{
			_concernsCaseGateway = concernsCaseGateway;
		}

		public CaseSearchParametersResponse Execute()
		{
			var owners = _concernsCaseGateway.GetOwnersOfCases();
			var teamLeaders = _concernsCaseGateway.GetTeamLeadersOfCases();

			return new CaseSearchParametersResponse
			{
				CaseOwners = owners,
				TeamLeaders = teamLeaders
			};
		}
	}
}
