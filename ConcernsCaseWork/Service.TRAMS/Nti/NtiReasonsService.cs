using ConcernsCaseWork.Logging;
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

namespace Service.TRAMS.Nti
{
	public class NtiReasonsService : AbstractService, INtiReasonsService
	{
		private readonly ILogger<NtiReasonsService> _logger;

		public NtiReasonsService(IHttpClientFactory httpClientFactory, ILogger<NtiReasonsService> logger, ICorrelationContext correlationContext) : base(httpClientFactory, logger, correlationContext)
		{
			_logger = logger;
		}

		public async Task<ICollection<NtiReasonDto>> GetNtiReasonsAsync()
		{
			try
			{
				_logger.LogInformation($"{nameof(NtiReasonsService)}::{LoggingHelpers.EchoCallerName()}");

				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{_endpointsVersion}/case-actions/notice-to-improve/all-reasons");

				// Create http client
				var client = CreateHttpClient();

				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();

				// Read response content
				var content = await response.Content.ReadAsStringAsync();

				// Deserialize content to POCO
				var apiWrapperRatingsDto = JsonConvert.DeserializeObject<ApiListWrapper<NtiReasonDto>>(content);

				return apiWrapperRatingsDto.Data;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"{nameof(NtiReasonsService)}::{LoggingHelpers.EchoCallerName()}");
				throw;
			}
		}
	}
}
