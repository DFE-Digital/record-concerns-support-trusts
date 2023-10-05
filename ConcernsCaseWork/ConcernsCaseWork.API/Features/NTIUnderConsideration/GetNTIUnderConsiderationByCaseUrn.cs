using ConcernsCaseWork.API.Contracts.NtiUnderConsideration;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Features.NTIUnderConsideration
{
	public class GetNTIUnderConsiderationByCaseUrn : IUseCase<int, List<NTIUnderConsiderationResponse>>
	{
		private readonly INTIUnderConsiderationGateway _gateway;

		public GetNTIUnderConsiderationByCaseUrn(INTIUnderConsiderationGateway gateway)
		{
			_gateway = gateway;
		}

		public List<NTIUnderConsiderationResponse> Execute(int caseUrn)
		{
			return ExecuteAsync(caseUrn).Result;
		}

		public async Task<List<NTIUnderConsiderationResponse>> ExecuteAsync(int caseUrn)
		{
			var considerations = await _gateway.GetNTIUnderConsiderationByCaseUrn(caseUrn);
			return considerations.Select(consideration => NTIUnderConsiderationFactory.CreateResponse(consideration)).ToList();
		}
	}
}