using ConcernsCaseWork.API.Factories.Concerns.Decisions;
using ConcernsCaseWork.API.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases.CaseActions.Decisions
{
	public class GetDecision: IUseCaseAsync<GetDecisionRequest, GetDecisionResponse>
	{
		private readonly IConcernsCaseGateway _concernsCaseGateway;
		private readonly IGetDecisionResponseFactory _responseFactory;

		public GetDecision(IConcernsCaseGateway concernsCaseGateway, IGetDecisionResponseFactory responseFactory)
		{
			_concernsCaseGateway = concernsCaseGateway ?? throw new ArgumentNullException(nameof(concernsCaseGateway));
			_responseFactory = responseFactory ?? throw new ArgumentNullException(nameof(responseFactory));
		}
		public Task<GetDecisionResponse> Execute(GetDecisionRequest request, CancellationToken cancellationToken)
		{
			_ = request ?? throw new ArgumentNullException(nameof(request));

			async Task<GetDecisionResponse> DoWork()
			{
				var concernsCase = _concernsCaseGateway.GetConcernsCaseByUrn(request.ConcernsCaseUrn);
				var decision = concernsCase?.Decisions.FirstOrDefault(x => x.DecisionId == request.DecisionId);

				return decision != null ? _responseFactory.Create(concernsCase.Urn, decision) : null;
			}

			return DoWork();
		}
	}
}