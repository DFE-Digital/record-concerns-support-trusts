using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Contracts.ResponseModels.TrustFinancialForecasts;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.UserContext;
using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Service.TrustFinancialForecast;

public class TrustFinancialForecastService : ConcernsAbstractService, ITrustFinancialForecastService
{
	private readonly ILogger<TrustFinancialForecastService> _logger;

	public TrustFinancialForecastService(IHttpClientFactory clientFactory, ILogger<TrustFinancialForecastService> logger, ICorrelationContext correlationContext, IClientUserInfoService clientUserInfoService)
		: base(clientFactory, logger, correlationContext, clientUserInfoService)
	{
		_logger = Guard.Against.Null(logger);
	}

	public async Task<string> Create(CreateTrustFinancialForecastRequest request)
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

	public async Task<TrustFinancialForecastResponse> GetById(GetTrustFinancialForecastByIdRequest request)
	{
		_logger.LogMethodEntered();

		return await Get<TrustFinancialForecastResponse>(
			$"/{EndpointsVersion}/concerns-cases/{request.CaseUrn}/trustfinancialforecast/{request.TrustFinancialForecastId}");
	}

	public async Task Update(UpdateTrustFinancialForecastRequest request)
	{
		_logger.LogMethodEntered();

		await Put<UpdateTrustFinancialForecastRequest, string>($"/{EndpointsVersion}/concerns-cases/{request.CaseUrn}/trustfinancialforecast/{request.TrustFinancialForecastId}", request);
	}

	public async Task<IEnumerable<TrustFinancialForecastResponse>> GetAllForCase(int caseUrn)
	{
		_logger.LogMethodEntered();

		return await Get<IEnumerable<TrustFinancialForecastResponse>>($"/{EndpointsVersion}/concerns-cases/{caseUrn}/trustfinancialforecast");
	}

	public async Task Close(CloseTrustFinancialForecastRequest request)
	{
		_logger.LogMethodEntered();

		await Patch<CloseTrustFinancialForecastRequest, string>($"/{EndpointsVersion}/concerns-cases/{request.CaseUrn}/trustfinancialforecast/{request.TrustFinancialForecastId}", request);
	}
}

