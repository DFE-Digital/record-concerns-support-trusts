using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Service.Helpers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Nti
{
	public class NtiConditionsService : ConcernsAbstractService, INtiConditionsService
	{
		private readonly ILogger<NtiConditionsService> _logger;

		public NtiConditionsService(IHttpClientFactory httpClientFactory, ILogger<NtiConditionsService> logger, ICorrelationContext correlationContext) : base(httpClientFactory, logger, correlationContext)
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
				var client = CreateHttpClient();

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
