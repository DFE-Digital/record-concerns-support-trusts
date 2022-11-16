using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.API.Factories.Concerns.Decisions;
using ConcernsCaseWork.Data.Gateways;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.API.UseCases.CaseActions.Decisions
{
public class UpdateDecision : IUseCaseAsync<(int urn, int decisionId, UpdateDecisionRequest details), UpdateDecisionResponse>
    {
        private readonly ILogger<UpdateDecision> _logger;
        private readonly IDecisionFactory _decisionFactory;
        private readonly IConcernsCaseGateway _concernsCaseGateway;
        private readonly IUpdateDecisionResponseFactory _responseFactory;

        public UpdateDecision(ILogger<UpdateDecision> logger,
            IDecisionFactory decisionFactory,
            IConcernsCaseGateway concernsCaseGateway,
            IUpdateDecisionResponseFactory responseFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _decisionFactory = decisionFactory ?? throw new ArgumentNullException(nameof(decisionFactory));
            _concernsCaseGateway = concernsCaseGateway ?? throw new ArgumentNullException((nameof(concernsCaseGateway)));
            _responseFactory = responseFactory ?? throw new ArgumentNullException(nameof(responseFactory));
        }

        public Task<UpdateDecisionResponse> Execute((int urn, int decisionId, UpdateDecisionRequest details) request, CancellationToken cancellationToken)
        {
            _ = request.urn > 0 ? request.urn : throw new ArgumentOutOfRangeException(nameof(request.urn));
            _ = request.decisionId > 0 ? request.decisionId : throw new ArgumentOutOfRangeException(nameof(request.decisionId));
            _ = request.details ?? throw new ArgumentNullException(nameof(request.details));

            async Task<UpdateDecisionResponse> DoWork() {
                cancellationToken.ThrowIfCancellationRequested();

                var decisionUpdates = _decisionFactory.CreateDecision(request.details);

                var concernsCase = _concernsCaseGateway.GetConcernsCaseByUrn(request.urn, withChangeTracking: true);

                if (concernsCase == null)
                {
                    throw new InvalidOperationException($"Concerns Case {request.urn} not found");
                }

                concernsCase.UpdateDecision(request.decisionId, decisionUpdates, DateTimeOffset.Now);

                await _concernsCaseGateway.UpdateExistingAsync(concernsCase);

                return _responseFactory.Create(request.urn, request.decisionId);
            }

            return DoWork();
        }
    }
}
