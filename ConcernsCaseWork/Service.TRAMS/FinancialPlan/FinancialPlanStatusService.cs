using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.TRAMS.Base;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Service.TRAMS.FinancialPlan
{
	public sealed class FinancialPlanStatusService : AbstractService, IFinancialPlanStatusService
	{
		private readonly ILogger<FinancialPlanStatusService> _logger;

		public FinancialPlanStatusService(IHttpClientFactory clientFactory, ILogger<FinancialPlanStatusService> logger) : base(clientFactory)
		{
			_logger = logger;
		}

		public async Task<IList<FinancialPlanStatusDto>> GetAllFinancialPlansStatuses()
		{
			try
			{
				_logger.LogInformation("FinancialPlanStatusService::GetAllFinancialPlansStatuses");

				return await HttpClientWrapper<FinancialPlanStatusDto>.GetData($"/{EndpointsVersion}/case-actions/financial-plan/all-statuses", ClientFactory);
			}
			catch (Exception ex)
			{
				_logger.LogError("FinancialPlanStatusService::GetAllFinancialPlansStatuses::Exception message::{Message}", ex.Message);

				throw;
			}
		}
		
		public async Task<IList<FinancialPlanStatusDto>> GetClosureFinancialPlansStatuses()
		{
			try
			{
				_logger.LogInformation("FinancialPlanStatusService::GetClosureFinancialPlansStatuses");

				return await HttpClientWrapper<FinancialPlanStatusDto>.GetData($"/{EndpointsVersion}/case-actions/financial-plan/closure-statuses", ClientFactory);
			}
			catch (Exception ex)
			{
				_logger.LogError("FinancialPlanStatusService::GetClosureFinancialPlansStatuses::Exception message::{Message}", ex.Message);

				throw;
			}
		}
				
		public async Task<IList<FinancialPlanStatusDto>> GetOpenFinancialPlansStatuses()
		{
			try
			{
				_logger.LogInformation("FinancialPlanStatusService::GetOpenFinancialPlansStatuses");

				return await HttpClientWrapper<FinancialPlanStatusDto>.GetData($"/{EndpointsVersion}/case-actions/financial-plan/open-statuses", ClientFactory);
			}
			catch (Exception ex)
			{
				_logger.LogError("FinancialPlanStatusService::GetOpenFinancialPlansStatuses::Exception message::{Message}", ex.Message);

				throw;
			}
		}

		private static class HttpClientWrapper<T> 
		{
			public static async Task<IList<T>> GetData(string relativeUrl, IHttpClientFactory clientFactory)
			{
				var request = new HttpRequestMessage(HttpMethod.Get, relativeUrl);
				var client = clientFactory.CreateClient(HttpClientName);
				
				var response = await client.SendAsync(request);

				response.EnsureSuccessStatusCode();

				var content = await response.Content.ReadAsStringAsync();

				var apiWrapperRatingsDto = JsonConvert.DeserializeObject<ApiListWrapper<T>>(content);

				if (apiWrapperRatingsDto is { Data: { } })
				{
					return apiWrapperRatingsDto.Data;
				}

				throw new Exception("Academies API error unwrap response");
			}
		}
	}
}
