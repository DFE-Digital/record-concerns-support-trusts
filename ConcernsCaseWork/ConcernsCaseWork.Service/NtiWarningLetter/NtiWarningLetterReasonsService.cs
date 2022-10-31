using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.NtiWarningLetter
{
	public class NtiWarningLetterReasonsService : ConcernsAbstractService, INtiWarningLetterReasonsService
	{
		private readonly ILogger<NtiWarningLetterReasonsService> _logger;

		public NtiWarningLetterReasonsService(IHttpClientFactory clientFactory, ILogger<NtiWarningLetterReasonsService> logger, ICorrelationContext correlationContext) : base(clientFactory, logger, correlationContext)
		{
			_logger = logger;
		}

		public async Task<ICollection<NtiWarningLetterReasonDto>> GetAllReasonsAsync()
		{
			try
			{
				_logger.LogInformation("NtiWarningLetterReasonsService::GetAllReasonsAsync");

				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{EndpointsVersion}/case-actions/nti-warning-letter/all-reasons");

				// Create http client
				var client = CreateHttpClient();

				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();

				// Read response content
				var content = await response.Content.ReadAsStringAsync();

				// Deserialize content to POCO
				var apiWrapperRatingsDto = JsonConvert.DeserializeObject<ApiListWrapper<NtiWarningLetterReasonDto>>(content);

				return apiWrapperRatingsDto.Data;
			}
			catch (Exception ex)
			{
				_logger.LogError("NtiWarningLetterReasonsService::GetAllReasonsAsync::Exception message::{Message}", ex.Message);
				throw;
			}
		}
	}
}