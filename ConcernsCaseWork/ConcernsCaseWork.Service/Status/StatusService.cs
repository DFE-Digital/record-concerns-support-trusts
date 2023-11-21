﻿using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.UserContext;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Status
{
	public sealed class StatusService : ConcernsAbstractService, IStatusService
	{
		private readonly ILogger<StatusService> _logger;

		public StatusService(IHttpClientFactory clientFactory, ILogger<StatusService> logger, ICorrelationContext correlationContext, IClientUserInfoService userInfoService) : base(clientFactory, logger, correlationContext, userInfoService)
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