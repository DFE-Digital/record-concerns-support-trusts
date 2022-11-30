using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.API.Factories.Concerns.Decisions;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases.CaseActions.Decisions;

public class CloseDecision: IUseCaseAsync<(int urn, int decisionId, CloseDecisionRequest request), CloseDecisionResponse>
{
	private readonly IConcernsCaseGateway _concernsCaseGateway;
	private readonly ICloseDecisionResponseFactory _responseFactory;

	public CloseDecision(IConcernsCaseGateway concernsCaseGateway, ICloseDecisionResponseFactory responseFactory)
	{
		_concernsCaseGateway = concernsCaseGateway ?? throw new ArgumentNullException(nameof(concernsCaseGateway));
		_responseFactory = responseFactory ?? throw new ArgumentNullException(nameof(responseFactory));
	}
	
	public Task<CloseDecisionResponse> Execute((int urn, int decisionId, CloseDecisionRequest request) request, CancellationToken cancellationToken)
	{
		_ = request.urn > 0 ? request.urn : throw new ArgumentOutOfRangeException(nameof(request.urn));
		_ = request.decisionId > 0 ? request.decisionId : throw new ArgumentOutOfRangeException(nameof(request.decisionId));
		_ = request.request ?? throw new ArgumentNullException(nameof(request.request));

		async Task<CloseDecisionResponse> DoWork() {
			cancellationToken.ThrowIfCancellationRequested();
			
			var concernsCase = _concernsCaseGateway.GetConcernsCaseByUrn(request.urn, withChangeTracking: true);

			if (concernsCase == null)
			{
				throw new InvalidOperationException($"Concerns Case {request.urn} not found");
			}

			concernsCase.CloseDecision(request.decisionId, request.request.SupportingNotes, DateTimeOffset.Now);

			await _concernsCaseGateway.UpdateExistingAsync(concernsCase);

			return _responseFactory.Create(request.urn, request.decisionId);
		}

		return DoWork();
	}
}