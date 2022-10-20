using ConcernsCaseWork.Service.Base;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.NtiUnderConsideration
{
	public class NtiUnderConsiderationReasonsService : ConcernsAbstractService, INtiUnderConsiderationReasonsService
	{
		private readonly ILogger<NtiUnderConsiderationReasonsService> _logger;

		public NtiUnderConsiderationReasonsService(IHttpClientFactory clientFactory, ILogger<NtiUnderConsiderationReasonsService> logger) : base(clientFactory, logger)
		{
			_logger = logger;
		}

		public async Task<ICollection<NtiUnderConsiderationReasonDto>> GetAllReasons()
		{
			try
			{
				_logger.LogInformation("NtiReasonsService::GetAllReasons");

				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{EndpointsVersion}/case-actions/nti-under-consideration/all-reasons");

				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);

				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();

				// Read response content
				var content = await response.Content.ReadAsStringAsync();

				// Deserialize content to POCO
				var apiWrapperRatingsDto = JsonConvert.DeserializeObject<ApiListWrapper<NtiUnderConsiderationReasonDto>>(content);

				return apiWrapperRatingsDto.Data;
			}
			catch (Exception ex)
			{
				_logger.LogError("NtiReasonsService::GetAllReasons::Exception message::{Message}", ex.Message);
				throw;
			}
		}
	}
}
