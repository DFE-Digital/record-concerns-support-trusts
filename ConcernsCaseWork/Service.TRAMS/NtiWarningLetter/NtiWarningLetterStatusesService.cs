﻿using ConcernsCaseWork.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.TRAMS.Base;
using Service.TRAMS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.NtiWarningLetter
{
	public class NtiWarningLetterStatusesService : AbstractService, INtiWarningLetterStatusesService
	{
		private readonly ILogger<NtiWarningLetterStatusesService> _logger;

		public NtiWarningLetterStatusesService(IHttpClientFactory clientFactory, ILogger<NtiWarningLetterStatusesService> logger, ICorrelationContext correlationContext) : base(clientFactory, logger, correlationContext)
		{
			_logger = logger;
		}

		public async Task<ICollection<NtiWarningLetterStatusDto>> GetAllStatusesAsync()
		{
			try
			{
				_logger.LogInformation($"{nameof(NtiWarningLetterStatusesService)}::{LoggingHelpers.EchoCallerName()}");

				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{_endpointsVersion}/case-actions/nti-warning-letter/all-statuses");

				// Create http client
				var client = CreateHttpClient();

				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();

				// Read response content
				var content = await response.Content.ReadAsStringAsync();

				// Deserialize content to POCO
				var apiWrapperRatingsDto = JsonConvert.DeserializeObject<ApiListWrapper<NtiWarningLetterStatusDto>>(content);

				return apiWrapperRatingsDto.Data;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"{nameof(NtiWarningLetterStatusesService)}::{LoggingHelpers.EchoCallerName()}");
				throw;
			}
		}
	}
}
