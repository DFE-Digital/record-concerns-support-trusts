using ConcernsCasework.Service.Base;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ConcernsCasework.Service.FinancialPlan
{
	public sealed class FinancialPlanStatusService : ConcernsAbstractService, IFinancialPlanStatusService
	{
		private readonly ILogger<FinancialPlanStatusService> _logger;

		public FinancialPlanStatusService(IHttpClientFactory clientFactory, ILogger<FinancialPlanStatusService> logger) : base(clientFactory, logger)
		{
			_logger = logger;
		}

		public async Task<IList<FinancialPlanStatusDto>> GetAllFinancialPlansStatusesAsync()
		{
			try
			{
				_logger.LogInformation("FinancialPlanStatusService::GetAllFinancialPlansStatusesAsync");
				
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{EndpointsVersion}/case-actions/financial-plan/all-statuses");
				var client = ClientFactory.CreateClient(HttpClientName);
				
				var response = await client.SendAsync(request);

				response.EnsureSuccessStatusCode();

				var content = await response.Content.ReadAsStringAsync();

				var apiWrapperRatingsDto = JsonConvert.DeserializeObject<ApiListWrapper<FinancialPlanStatusDto>>(content);

				if (apiWrapperRatingsDto is { Data: { } })
				{
					return apiWrapperRatingsDto.Data;
				}

				throw new Exception("Academies API error unwrap response");

			}
			catch (Exception ex)
			{
				_logger.LogError("FinancialPlanStatusService::GetAllFinancialPlansStatuses::Exception message::{Message}", ex.Message);

				throw;
			}
		}
		
		public async Task<IList<FinancialPlanStatusDto>> GetClosureFinancialPlansStatusesAsync()
		{
			try
			{
				_logger.LogInformation("FinancialPlanStatusService::GetClosureFinancialPlansStatusesAsync");
				
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{EndpointsVersion}/case-actions/financial-plan/closure-statuses");
				var client = ClientFactory.CreateClient(HttpClientName);
				
				var response = await client.SendAsync(request);

				response.EnsureSuccessStatusCode();

				var content = await response.Content.ReadAsStringAsync();

				var apiWrapperRatingsDto = JsonConvert.DeserializeObject<ApiListWrapper<FinancialPlanStatusDto>>(content);

				if (apiWrapperRatingsDto is { Data: { } })
				{
					return apiWrapperRatingsDto.Data;
				}

				throw new Exception("Academies API error unwrap response");

			}
			catch (Exception ex)
			{
				_logger.LogError("FinancialPlanStatusService::GetClosureFinancialPlansStatuses::Exception message::{Message}", ex.Message);

				throw;
			}
		}

		public async Task<IList<FinancialPlanStatusDto>> GetOpenFinancialPlansStatusesAsync()
		{
			try
			{
				_logger.LogInformation("FinancialPlanStatusService::GetOpenFinancialPlansStatusesAsync");
				
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{EndpointsVersion}/case-actions/financial-plan/open-statuses");
				var client = ClientFactory.CreateClient(HttpClientName);
				
				var response = await client.SendAsync(request);

				response.EnsureSuccessStatusCode();

				var content = await response.Content.ReadAsStringAsync();

				var apiWrapperRatingsDto = JsonConvert.DeserializeObject<ApiListWrapper<FinancialPlanStatusDto>>(content);

				if (apiWrapperRatingsDto is { Data: { } })
				{
					return apiWrapperRatingsDto.Data;
				}

				throw new Exception("Academies API error unwrap response");

			}
			catch (Exception ex)
			{
				_logger.LogError("FinancialPlanStatusService::GetOpenFinancialPlansStatuses::Exception message::{Message}", ex.Message);

				throw;
			}
		}
	}
}
