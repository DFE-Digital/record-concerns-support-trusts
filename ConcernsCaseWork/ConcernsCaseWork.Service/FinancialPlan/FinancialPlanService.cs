using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Context;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Text;

namespace ConcernsCaseWork.Service.FinancialPlan
{
	public sealed class FinancialPlanService : ConcernsAbstractService, IFinancialPlanService
	{
		private readonly ILogger<FinancialPlanService> _logger;

		public FinancialPlanService(IHttpClientFactory clientFactory, ILogger<FinancialPlanService> logger, ICorrelationContext correlationContext, IUserContextService userContextService) : base(clientFactory, logger, correlationContext, userContextService)
		{
			_logger = logger;
		}

		public async Task<IList<FinancialPlanDto>> GetFinancialPlansByCaseUrn(long caseUrn)
		{
			try
			{
				_logger.LogInformation("FinancialPlanService::GetFinancialPlansByCaseUrn");

				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get,
					$"/{EndpointsVersion}/case-actions/financial-plan/case/{caseUrn}");

				// Create http client
				var client = CreateHttpClient();

				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();

				// Read response content
				var content = await response.Content.ReadAsStringAsync();

				// Deserialize content to POJO
				var apiWrapperFinancialPlansDto = JsonConvert.DeserializeObject<ApiListWrapper<FinancialPlanDto>>(content);

				// Unwrap response
				if (apiWrapperFinancialPlansDto is { Data: { } })
				{
					return apiWrapperFinancialPlansDto.Data;
				}

				throw new Exception("Academies API error unwrap response");
			}
			catch (Exception ex)
			{
				_logger.LogError("FinancialPlanService::GetFinancialPlansByCaseUrn::Exception message::{Message}", ex.Message);
			}

			return Array.Empty<FinancialPlanDto>();
		}

		public async Task<FinancialPlanDto> GetFinancialPlansById(long caseUrn, long financialPlanId)
		{
			try
			{
				_logger.LogInformation("FinancialPlanService::GetFinancialPlansById");

				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get,
					$"/{EndpointsVersion}/case-actions/financial-plan/{financialPlanId}");

				// Create http client
				var client = CreateHttpClient();

				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();

				// Read response content
				var content = await response.Content.ReadAsStringAsync();

				// Deserialize content to POJO
				var apiWrapperFinancialPlanDto = JsonConvert.DeserializeObject<ApiWrapper<FinancialPlanDto>>(content);

				// Unwrap response
				if (apiWrapperFinancialPlanDto is { Data: { } })
				{
					return apiWrapperFinancialPlanDto.Data;
				}

				throw new Exception("Academies API error unwrap response");
			}
			catch (Exception ex)
			{
				_logger.LogError("FinancialPlanService::GetFinancialPlansById::Exception message::{Message}", ex.Message);
			}

			return null;
		}

		public async Task<FinancialPlanDto> PostFinancialPlanByCaseUrn(CreateFinancialPlanDto createFinancialPlanDto)
		{
			try
			{
				_logger.LogInformation("FinancialPlanService::PostFinancialPlanByCaseUrn");

				// Create a request
				var request = new StringContent(
					JsonConvert.SerializeObject(createFinancialPlanDto),
					Encoding.UTF8,
					MediaTypeNames.Application.Json);

				// Create http client
				var client = CreateHttpClient();

				// Execute request
				var response = await client.PostAsync(
					$"/{EndpointsVersion}/case-actions/financial-plan", request);

				// Check status code
				response.EnsureSuccessStatusCode();

				// Read response content
				var content = await response.Content.ReadAsStringAsync();

				// Deserialize content to POJO
				var apiWrapperFinancialPlanDto = JsonConvert.DeserializeObject<ApiWrapper<FinancialPlanDto>>(content);

				// Unwrap response
				if (apiWrapperFinancialPlanDto is { Data: { } })
				{
					return apiWrapperFinancialPlanDto.Data;
				}

				throw new Exception("Academies API error unwrap response");
			}
			catch (Exception ex)
			{
				_logger.LogError("FinancialPlanService::PostFinancialPlanByCaseUrn::Exception message::{Message}", ex.Message);
			}

			return null;
		}

		public async Task<FinancialPlanDto> PatchFinancialPlanById(FinancialPlanDto financialPlanDto)
		{
			try
			{
				_logger.LogInformation("FinancialPlanService::PatchFinancialPlanById");

				// Create a request
				var request = new StringContent(
					JsonConvert.SerializeObject(financialPlanDto),
					Encoding.UTF8,
					MediaTypeNames.Application.Json);

				// Create http client
				var client = CreateHttpClient();

				// Execute request
				var response = await client.PatchAsync(
					$"/{EndpointsVersion}/case-actions/financial-plan", request);

				// Check status code
				response.EnsureSuccessStatusCode();

				// Read response content
				var content = await response.Content.ReadAsStringAsync();

				// Deserialize content to POJO
				var apiWrapperFinancialPlanDto = JsonConvert.DeserializeObject<ApiWrapper<FinancialPlanDto>>(content);

				// Unwrap response
				if (apiWrapperFinancialPlanDto is { Data: { } })
				{
					return apiWrapperFinancialPlanDto.Data;
				}

				throw new Exception("Academies API error unwrap response");
			}
			catch (Exception ex)
			{
				_logger.LogError("FinancialPlanService::PatchFinancialPlanById::Exception message::{Message}", ex.Message);
			}

			return null;
		}
	}
}
