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
	public class NtiStatusesService : AbstractService, INtiStatusesService
	{
		private readonly ILogger<NtiStatusesService> _logger;

		public NtiStatusesService(IHttpClientFactory httpClientFactory, 
			ILogger<NtiStatusesService> logger) : base(httpClientFactory)
		{
			_logger = logger;
		}

		public async Task<ICollection<NtiStatusDto>> GetNtiStatusesAsync()
		{
			try
			{
				_logger.LogInformation($"{nameof(NtiStatusesService)}::{LoggingHelpers.EchoCallerName()}");

				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{EndpointsVersion}/case-actions/notice-to-improve/all-statuses");

				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);

				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();

				// Read response content
				var content = await response.Content.ReadAsStringAsync();

				// Deserialize content to POCO
				var apiWrapperRatingsDto = JsonConvert.DeserializeObject<ApiListWrapper<NtiStatusDto>>(content);

				return apiWrapperRatingsDto.Data;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"{nameof(NtiStatusesService)}::{LoggingHelpers.EchoCallerName()}");
				throw;
			}
		}
	}
}