using ConcernsCasework.Service.Base;
using ConcernsCasework.Service.Helpers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ConcernsCasework.Service.Nti
{
	public class NtiReasonsService : ConcernsAbstractService, INtiReasonsService
	{
		private readonly ILogger<NtiReasonsService> _logger;

		public NtiReasonsService(IHttpClientFactory httpClientFactory, 
			ILogger<NtiReasonsService> logger) : base(httpClientFactory, logger)
		{
			_logger = logger;
		}

		public async Task<ICollection<NtiReasonDto>> GetNtiReasonsAsync()
		{
			try
			{
				_logger.LogInformation($"{nameof(NtiReasonsService)}::{LoggingHelpers.EchoCallerName()}");

				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{EndpointsVersion}/case-actions/notice-to-improve/all-reasons");

				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);

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
