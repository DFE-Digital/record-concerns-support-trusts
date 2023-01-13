using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Service.Helpers;
using ConcernsCaseWork.UserContext;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.NtiWarningLetter
{
	public class NtiWarningLetterStatusesService : ConcernsAbstractService, INtiWarningLetterStatusesService
	{
		private readonly ILogger<NtiWarningLetterStatusesService> _logger;

		public NtiWarningLetterStatusesService(IHttpClientFactory clientFactory, ILogger<NtiWarningLetterStatusesService> logger, ICorrelationContext correlationContext, IUserInfoService userInfoService) : base(clientFactory, logger, correlationContext, userInfoService)
		{
			_logger = logger;
		}

		public async Task<ICollection<NtiWarningLetterStatusDto>> GetAllStatusesAsync()
		{
			try
			{
				_logger.LogInformation($"{nameof(NtiWarningLetterStatusesService)}::{LoggingHelpers.EchoCallerName()}");

				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{EndpointsVersion}/case-actions/nti-warning-letter/all-statuses");

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
