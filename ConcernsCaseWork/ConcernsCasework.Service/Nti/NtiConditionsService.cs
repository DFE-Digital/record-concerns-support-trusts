using ConcernsCasework.Service.Base;
using ConcernsCasework.Service.Helpers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ConcernsCasework.Service.Nti
{
	public class NtiConditionsService : ConcernsAbstractService, INtiConditionsService
	{
		private readonly ILogger<NtiConditionsService> _logger;

		public NtiConditionsService(IHttpClientFactory httpClientFactory,
			ILogger<NtiConditionsService> logger) : base(httpClientFactory, logger)
		{
			_logger = logger;
		}

		public async Task<ICollection<NtiConditionDto>> GetAllConditionsAsync()
		{
			try
			{
				_logger.LogInformation($"{nameof(NtiConditionsService)}::{LoggingHelpers.EchoCallerName()}");

				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{EndpointsVersion}/case-actions/notice-to-improve/all-conditions");

				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);

				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();

				// Read response content
				var content = await response.Content.ReadAsStringAsync();

				// Deserialize content to POCO
				var apiWrapperRatingsDto = JsonConvert.DeserializeObject<ApiListWrapper<NtiConditionDto>>(content);

				return apiWrapperRatingsDto.Data;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"{nameof(NtiConditionsService)}::{LoggingHelpers.EchoCallerName()}");
				throw;
			}
		}
	}
}
