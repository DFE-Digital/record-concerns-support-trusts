using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Contracts.ResponseModels.TrustFinancialForecasts;
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
	
	public async Task<string> CreateTrustFinancialForecast(CreateTrustFinancialForecastRequest request)
	{
		_logger.LogMethodEntered();

		_ = Guard.Against.Null(request);
		
		var response = await Post<CreateTrustFinancialForecastRequest, string>(
			$"/{EndpointsVersion}/concerns-cases/{request.CaseUrn}/trustfinancialforecast", 
			request);

		_logger.LogInformation("Trust Financial Forecast created. caseUrn: {CaseUrn}, Trust Financial Forecast Id:{TrustFinancialForecastId}", 
			request.CaseUrn, response);
		
		return response;
	}
	
	public async Task<TrustFinancialForecastResponse> GetTrustFinancialForecastById(GetTrustFinancialForecastRequest request)
	{
		_logger.LogMethodEntered();

		return await Get<TrustFinancialForecastResponse>(
			$"/{EndpointsVersion}/concerns-cases/{request.CaseUrn}/trustfinancialforecast/{request.TrustFinancialForecastId}");
	}

	public async Task UpdateTrustFinancialForecast(UpdateTrustFinancialForecastRequest request)
	{
		_logger.LogMethodEntered();

		await Put<UpdateTrustFinancialForecastRequest, string>($"/{EndpointsVersion}/concerns-cases/{request.CaseUrn}/trustfinancialforecast/{request.TrustFinancialForecastId}", request);
	}
}

