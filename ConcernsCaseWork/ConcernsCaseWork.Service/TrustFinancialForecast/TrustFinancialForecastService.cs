using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Service.TrustFinancialForecast;

public class TrustFinancialForecastService : ConcernsAbstractService, ITrustFinancialForecastService
{
	private readonly ILogger<TrustFinancialForecastService> _logger;

	public TrustFinancialForecastService(IHttpClientFactory clientFactory, ILogger<TrustFinancialForecastService> logger, ICorrelationContext correlationContext) : base(clientFactory, logger, correlationContext)
	{
		_logger = Guard.Against.Null(logger);
	}
	
	public async Task<string> PostTrustFinancialForecast(CreateTrustFinancialForecastRequest createTrustFinancialForecast)
	{
		_logger.LogMethodEntered();

		_ = Guard.Against.Null(createTrustFinancialForecast);
		
		var response = await Post<CreateTrustFinancialForecastRequest, string>(
			$"/{EndpointsVersion}/concerns-cases/{createTrustFinancialForecast.CaseUrn}/trustfinancialforecast", 
			createTrustFinancialForecast);

		_logger.LogInformation("Trust Financial Forecast created. caseUrn: {CaseUrn}, Trust Financial Forecast Id:{TrustFinancialForecastId}", 
			createTrustFinancialForecast.CaseUrn, response);
		
		return response;
	}
}

