using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Features.Case
{
	public interface IGetCaseFilterParameters
	{
		Task<CaseFilterParameters> Execute();
	}

	public class GetCaseFilterParameters : IGetCaseFilterParameters
	{
		private readonly ICaseSummaryGateway _gateway;

		public GetCaseFilterParameters(ICaseSummaryGateway gateway)
		{
			_gateway = gateway;
		}

		public async Task<CaseFilterParameters> Execute()
		{
			return await _gateway.GetCaseFilterParameters();
		}
	}
}
