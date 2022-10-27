using ConcernsCaseWork.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.TRAMS.Base;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Service.TRAMS.Status
{
	public sealed class StatusService : AbstractService, IStatusService
	{
		private readonly ILogger<StatusService> _logger;
		
		public StatusService(IHttpClientFactory clientFactory, ILogger<StatusService> logger, ICorrelationContext correlationContext) : base(clientFactory, logger, correlationContext)
		{
			_logger = logger;
		}

		public async Task<IList<StatusDto>> GetStatuses()
		{
			try
			{
				_logger.LogInformation("StatusService::GetStatuses");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{EndpointsVersion}/concerns-statuses");
				
				// Create http client
				var client = CreateHttpClient();
				
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POCO
				var apiWrapperStatusesDto = JsonConvert.DeserializeObject<ApiListWrapper<StatusDto>>(content);

				// Unwrap response
				if (apiWrapperStatusesDto is { Data: { } })
				{
					return apiWrapperStatusesDto.Data;
				}

				throw new Exception("Academies API error unwrap response");
			}
			catch (Exception ex)
			{
				_logger.LogError("StatusService::GetStatuses::Exception message::{Message}", ex.Message);

				throw;
			}
		}
	}
}