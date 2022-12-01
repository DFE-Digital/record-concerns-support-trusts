using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.API.Factories.Concerns.Decisions;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases.CaseActions.Decisions;

public class CloseDecision: IUseCaseAsync<DecisionUseCaseRequestWrapper<CloseDecisionRequest>, CloseDecisionResponse>
{
	private readonly IConcernsCaseGateway _concernsCaseGateway;
	private readonly ICloseDecisionResponseFactory _responseFactory;

	public CloseDecision(IConcernsCaseGateway concernsCaseGateway, ICloseDecisionResponseFactory responseFactory)
	{
		_concernsCaseGateway = concernsCaseGateway ?? throw new ArgumentNullException(nameof(concernsCaseGateway));
		_responseFactory = responseFactory ?? throw new ArgumentNullException(nameof(responseFactory));
	}
	
	public Task<CloseDecisionResponse> Execute(DecisionUseCaseRequestWrapper<CloseDecisionRequest> request, CancellationToken cancellationToken)
	{
		_ = request ?? throw new ArgumentNullException(nameof(request));
		_ = request.CaseUrn > 0 ? request.CaseUrn : throw new ArgumentOutOfRangeException(nameof(request.CaseUrn));
		_ = request.DecisionId > 0 ? request.DecisionId : throw new ArgumentOutOfRangeException(nameof(request.DecisionId));

		async Task<CloseDecisionResponse> DoWork() {
			cancellationToken.ThrowIfCancellationRequested();
			
			var concernsCase = _concernsCaseGateway.GetConcernsCaseByUrn(request.CaseUrn, withChangeTracking: true);

			if (concernsCase == null)
			{
				throw new InvalidOperationException($"Concerns Case {request.CaseUrn} not found");
			}

			concernsCase.CloseDecision((int)request.DecisionId!, request.Request.SupportingNotes, DateTimeOffset.Now);

			return _responseFactory.Create(request.CaseUrn, (int)request.DecisionId);
		}

		return DoWork();
	}
}