using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.API.Factories.Concerns.Decisions;
using ConcernsCaseWork.Data.Exceptions;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases.CaseActions.Decisions;

public class CloseDecision: IUseCaseAsync<DecisionUseCaseRequestParams<CloseDecisionRequest>, CloseDecisionResponse>
{
	private readonly IConcernsCaseGateway _concernsCaseGateway;
	private readonly ICloseDecisionResponseFactory _responseFactory;

	public CloseDecision(IConcernsCaseGateway concernsCaseGateway, ICloseDecisionResponseFactory responseFactory)
	{
		_concernsCaseGateway = concernsCaseGateway ?? throw new ArgumentNullException(nameof(concernsCaseGateway));
		_responseFactory = responseFactory ?? throw new ArgumentNullException(nameof(responseFactory));
	}
	
	public Task<CloseDecisionResponse> Execute(DecisionUseCaseRequestParams<CloseDecisionRequest> request, CancellationToken cancellationToken)
	{
		_ = request ?? throw new ArgumentNullException(nameof(request));
		_ = request.CaseUrn > 0 ? request.CaseUrn : throw new ArgumentOutOfRangeException(nameof(request.CaseUrn));
		_ = request.DecisionId > 0 ? request.DecisionId : throw new ArgumentOutOfRangeException(nameof(request.DecisionId));

		async Task<CloseDecisionResponse> DoWork() {
			cancellationToken.ThrowIfCancellationRequested();
			
			var concernsCase = _concernsCaseGateway.GetConcernsCaseByUrn(request.CaseUrn, withChangeTracking: true);

			if (concernsCase == null)
			{
				throw new NotFoundException($"Concerns Case {request.CaseUrn} not found");
			}

			try
			{
				concernsCase.CloseDecision((int)request.DecisionId!, request.Request.SupportingNotes, DateTimeOffset.Now);

				await _concernsCaseGateway.UpdateExistingAsync(concernsCase);

				return _responseFactory.Create(request.CaseUrn, (int)request.DecisionId);
			}
			catch (EntityNotFoundException ex)
			{
				throw new NotFoundException(ex.Message);
			}
			catch (StateChangeNotAllowedException ex)
			{
				throw new OperationNotCompletedException(ex.Message);
			}
		}

		return DoWork();
	}
}