﻿using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Service.Helpers;
using ConcernsCaseWork.UserContext;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.NtiWarningLetter
{
	public class NtiWarningLetterConditionsService : ConcernsAbstractService, INtiWarningLetterConditionsService
	{
		private readonly ILogger<NtiWarningLetterConditionsService> _logger;

		public NtiWarningLetterConditionsService(IHttpClientFactory httpClientFactory, ILogger<NtiWarningLetterConditionsService> logger, ICorrelationContext correlationContext, IClientUserInfoService userInfoService) : base(httpClientFactory, logger, correlationContext, userInfoService)
		{
			_logger = logger;
		}

		public async Task<ICollection<NtiWarningLetterConditionDto>> GetAllConditionsAsync()
		{
			try
			{
				_logger.LogInformation($"{nameof(NtiWarningLetterConditionsService)}::{LoggingHelpers.EchoCallerName()}");

				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{EndpointsVersion}/case-actions/nti-warning-letter/all-conditions");

				// Create http client
				var client = CreateHttpClient();

				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();

				// Read response content
				var content = await response.Content.ReadAsStringAsync();

				// Deserialize content to POCO
				var apiWrapperRatingsDto = JsonConvert.DeserializeObject<ApiListWrapper<NtiWarningLetterConditionDto>>(content);

				return apiWrapperRatingsDto.Data;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"{nameof(NtiWarningLetterConditionsService)}::{LoggingHelpers.EchoCallerName()}");
				throw;
			}
		}
	}
}
