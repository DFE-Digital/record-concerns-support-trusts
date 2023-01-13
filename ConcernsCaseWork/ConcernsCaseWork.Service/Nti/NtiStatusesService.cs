using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Service.Helpers;
using ConcernsCaseWork.UserContext;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Nti
{
	public class NtiStatusesService : ConcernsAbstractService, INtiStatusesService
	{
		private readonly ILogger<NtiStatusesService> _logger;

		public NtiStatusesService(IHttpClientFactory httpClientFactory, ILogger<NtiStatusesService> logger, ICorrelationContext correlationContext, IUserInfoService userInfoService) : base(httpClientFactory, logger, correlationContext, userInfoService)
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
				var client = CreateHttpClient();

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