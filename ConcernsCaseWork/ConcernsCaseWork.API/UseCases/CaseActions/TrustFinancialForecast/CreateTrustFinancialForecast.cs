using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;

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

			var cc = GetCase(request.CaseUrn);
		

	        var model = BuildTrustFinancialForecast(request);
	        
            var result =  await _trustFinancialForecastGateway.Update(model, cancellationToken);

			cc.CaseLastUpdatedAt = model.CreatedAt.DateTime;

			await _concernsCaseGateway.UpdateExistingAsync(cc);

			return result;
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

		private ConcernsCase GetCase(int caseUrn)
		{
			var cc = _concernsCaseGateway.GetConcernsCaseByUrn(caseUrn);
			if (cc == null)
			{
				throw new NotFoundException($"Concerns Case {caseUrn} not found");
			}
			return cc;
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
