using ConcernsCaseWork.API.Factories.Concerns.Decisions;
using ConcernsCaseWork.API.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases.CaseActions.Decisions
{
    public class CreateDecision : IUseCaseAsync<CreateDecisionRequest, CreateDecisionResponse>
    {
        private readonly IConcernsCaseGateway _concernsCaseGateway;
        private readonly IDecisionFactory _decisionFactory;
        private readonly ICreateDecisionResponseFactory _createDecisionResponseFactory;

        public CreateDecision(IConcernsCaseGateway concernsCaseGateway, IDecisionFactory decisionFactory, ICreateDecisionResponseFactory createDecisionResponseFactory)
        {
            _concernsCaseGateway = concernsCaseGateway ?? throw new ArgumentNullException(nameof(concernsCaseGateway));
            _decisionFactory = decisionFactory ?? throw new ArgumentNullException(nameof(decisionFactory));
            _createDecisionResponseFactory = createDecisionResponseFactory ?? throw new ArgumentNullException(nameof(createDecisionResponseFactory));
        }

        public Task<CreateDecisionResponse> Execute(CreateDecisionRequest request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            if (!request.IsValid())
            {
                throw new ArgumentException("Request is not valid", nameof(request));
            }

            async Task<CreateDecisionResponse> DoWork()
            {
                var concernsCase = _concernsCaseGateway.GetConcernsCaseByUrn(request.ConcernsCaseUrn) ??
                                   throw new InvalidOperationException(
                                       $"The concerns case for urn {request.ConcernsCaseUrn}, was not found");

                var decision = _decisionFactory.CreateDecision(request);
                concernsCase.AddDecision(decision, DateTimeOffset.Now);

                cancellationToken.ThrowIfCancellationRequested();

                _concernsCaseGateway.SaveConcernsCase(concernsCase);

                return _createDecisionResponseFactory.Create(concernsCase.Urn, decision.DecisionId);
            }

            return DoWork();
        }
    }
}
