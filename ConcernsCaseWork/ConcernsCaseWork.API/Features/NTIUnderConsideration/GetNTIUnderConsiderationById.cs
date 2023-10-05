using ConcernsCaseWork.API.Contracts.NtiUnderConsideration;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Features.NTIUnderConsideration
{
	public class GetNTIUnderConsiderationById : IUseCase<long, NTIUnderConsiderationResponse>
	{
		private readonly INTIUnderConsiderationGateway _gateway;

		public GetNTIUnderConsiderationById(INTIUnderConsiderationGateway gateway)
		{
			_gateway = gateway;
		}

		public NTIUnderConsiderationResponse Execute(long underConsiderationId)
		{
			return ExecuteAsync(underConsiderationId).Result;
		}

		public async Task<NTIUnderConsiderationResponse> ExecuteAsync(long underConsiderationId)
		{
			var consideration = await _gateway.GetNTIUnderConsiderationById(underConsiderationId);
			return consideration == null ? null : NTIUnderConsiderationFactory.CreateResponse(consideration);
		}
	}
}
