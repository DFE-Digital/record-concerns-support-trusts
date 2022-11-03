using ConcernsCaseWork.API.Factories.Concerns.Decisions;
using ConcernsCaseWork.API.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases.CaseActions.Decisions
{
    public class GetDecisions : IUseCaseAsync<GetDecisionsRequest, DecisionSummaryResponse[]>
    {
        private readonly ILogger<GetDecisions> _logger;
        private readonly IConcernsCaseGateway _gateway;
        private readonly IGetDecisionsSummariesFactory _getDecisionsSummariesFactory;

        public GetDecisions(ILogger<GetDecisions> logger, IConcernsCaseGateway gateway, IGetDecisionsSummariesFactory getDecisionsSummariesFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
            _getDecisionsSummariesFactory = getDecisionsSummariesFactory ?? throw new ArgumentNullException(nameof(getDecisionsSummariesFactory));
        }

        public Task<DecisionSummaryResponse[]> Execute(GetDecisionsRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            async Task<DecisionSummaryResponse[]> DoWork()
            {
                cancellationToken.ThrowIfCancellationRequested();
                var concernsCase = _gateway.GetConcernsCaseById(request.ConcernsCaseUrn);

                if (concernsCase == null)
                {
                    return default;
                }

                return _getDecisionsSummariesFactory.Create(request.ConcernsCaseUrn, concernsCase.Decisions.AsEnumerable());
            }

            return DoWork();
        }
    }
}
