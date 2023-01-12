using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Service.Context;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.NtiUnderConsideration
{
	public class NtiUnderConsiderationStatusesService : ConcernsAbstractService, INtiUnderConsiderationStatusesService
	{
		private readonly ILogger<NtiUnderConsiderationReasonsService> _logger;

		public NtiUnderConsiderationStatusesService(IHttpClientFactory clientFactory, ILogger<NtiUnderConsiderationReasonsService> logger, ICorrelationContext correlationContext, IUserContextService userContextService) : base(clientFactory, logger, correlationContext, userContextService)
		{
			_logger = logger;
		}

		public async Task<ICollection<NtiUnderConsiderationStatusDto>> GetAllStatuses()
		{
			try
			{
				_logger.LogInformation("NtiStatusesService::GetAllStatuses");

				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{EndpointsVersion}/case-actions/nti-under-consideration/all-statuses");

				// Create http client
				var client = CreateHttpClient();

				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();

				// Read response content
				var content = await response.Content.ReadAsStringAsync();

				// Deserialize content to POCO
				var apiWrapperRatingsDto = JsonConvert.DeserializeObject<ApiListWrapper<NtiUnderConsiderationStatusDto>>(content);

				return apiWrapperRatingsDto.Data;
			}
			catch (Exception ex)
			{
				_logger.LogError("NtiStatusesService::GetAllStatuses::Exception message::{Message}", ex.Message);
				throw;
			}
		}
	}
}
