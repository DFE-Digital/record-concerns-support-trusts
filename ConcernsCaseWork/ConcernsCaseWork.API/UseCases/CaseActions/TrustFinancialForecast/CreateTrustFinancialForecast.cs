using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases.CaseActions.TrustFinancialForecast
{
    public class CreateTrustFinancialForecast : IUseCaseAsync<CreateTrustFinancialForecastRequest, int>
    {
        private readonly IConcernsCaseGateway _concernsCaseGateway;
        private readonly ITrustFinancialForecastGateway _trustFinancialForecastGateway;

        public CreateTrustFinancialForecast(IConcernsCaseGateway concernsCaseGateway, ITrustFinancialForecastGateway trustFinancialForecastGateway)
        {
            _concernsCaseGateway = concernsCaseGateway ?? throw new ArgumentNullException(nameof(concernsCaseGateway));
            _trustFinancialForecastGateway = trustFinancialForecastGateway ?? throw new ArgumentNullException(nameof(trustFinancialForecastGateway));
        }

        public async Task<int> Execute(CreateTrustFinancialForecastRequest request, CancellationToken cancellationToken)
        {
	        EnsureRequestIsValid(request);
		
	        await EnsureCaseExists(request.CaseUrn, cancellationToken);

	        var model = BuildTrustFinancialForecast(request);
	        
            return await _trustFinancialForecastGateway.Update(model, cancellationToken);
        }
        
        private static void EnsureRequestIsValid(CreateTrustFinancialForecastRequest request)
        {
	        if (request is null)
	        {
		        throw new ArgumentNullException(nameof(request));
	        }
		
	        if (!request.IsValid())
	        {
		        throw new ArgumentException("Request is not valid", nameof(request));
	        }
        }

        private async Task EnsureCaseExists(int caseUrn, CancellationToken cancellationToken)
        {
	        if (! await _concernsCaseGateway.CaseExists(caseUrn, cancellationToken))
	        {
		        throw new NotFoundException($"Concerns Case {caseUrn} not found");
	        }
        }

        private static Data.Models.TrustFinancialForecast BuildTrustFinancialForecast(CreateTrustFinancialForecastRequest request)
        {
	        var now = DateTimeOffset.Now;
	        
	        return new Data.Models.TrustFinancialForecast
	        {
		        CaseUrn = request.CaseUrn,
		        SRMAOfferedAfterTFF = request.SRMAOfferedAfterTFF,
		        ForecastingToolRanAt = request.ForecastingToolRanAt,
		        WasTrustResponseSatisfactory = request.WasTrustResponseSatisfactory,
		        Notes = request.Notes,
		        SFSOInitialReviewHappenedAt = request.SFSOInitialReviewHappenedAt,
		        TrustRespondedAt = request.TrustRespondedAt,
		        
		        CreatedAt = now,
		        UpdatedAt = now
	        };
        }
    }
}
