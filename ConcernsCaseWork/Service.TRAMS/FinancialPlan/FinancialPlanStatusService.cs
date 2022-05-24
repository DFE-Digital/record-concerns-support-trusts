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

		public async Task<IList<FinancialPlanStatusDto>> GetFinancialPlansStatuses()
		{
			try
			{
				_logger.LogInformation("FinancialPlanStatusService::GetFinancialPlansStatuses");

				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{EndpointsVersion}/concerns-ratings");

				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);

				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();

				// Read response content
				var content = await response.Content.ReadAsStringAsync();

				// Deserialize content to POCO
				var apiWrapperRatingsDto = JsonConvert.DeserializeObject<ApiListWrapper<FinancialPlanStatusDto>>(content);

				// Unwrap response
				if (apiWrapperRatingsDto is { Data: { } })
				{
					//return apiWrapperRatingsDto.Data;

					var now = DateTime.Now;

					var statuses = new List<FinancialPlanStatusDto>() 
					{
						new FinancialPlanStatusDto("AwaitingPlan", now, now, 1),
						new FinancialPlanStatusDto("ReturnToTrust", now, now, 2),
						new FinancialPlanStatusDto("ViablePlanReceived", now, now, 3),
						new FinancialPlanStatusDto("Abandoned", now, now, 4)
					};

					return statuses;
				}

				throw new Exception("Academies API error unwrap response");
			}
			catch (Exception ex)
			{
				_logger.LogError("FinancialPlanStatusService::GetFinancialPlansStatuses::Exception message::{Message}", ex.Message);

				throw;
			}
		}
	}
}
