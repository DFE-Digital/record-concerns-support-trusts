using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.TRAMS.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.NtiWarningLetter
{
	public class NtiWarningLetterConditionsService : AbstractService, INtiWarningLetterConditionsService
	{
		private readonly ILogger<NtiWarningLetterConditionsService> _logger;

		public NtiWarningLetterConditionsService(IHttpClientFactory httpClientFactory,
			ILogger<NtiWarningLetterConditionsService> logger) : base(httpClientFactory)
		{
			_logger = logger;
		}

		public async Task<ICollection<NtiWarningLetterConditionDto>> GetAllConditionsAsync()
		{
			try
			{
				_logger.LogInformation("NtiWarningLetterConditionsService::GetAllConditionsAsync");

				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{EndpointsVersion}/case-actions/nti-warning-letter/all-conditions");

				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);

				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();

				// Read response content
				var content = await response.Content.ReadAsStringAsync();

				// Deserialize content to POCO
				var apiWrapperRatingsDto = JsonConvert.DeserializeObject<ApiListWrapper<NtiWarningLetterConditionDto>>(content);

				return apiWrapperRatingsDto.Data;
			}
			catch (Exception ex)
			{
				_logger.LogError("NtiWarningLetterConditionsService::GetAllConditionsAsync::Exception message::{Message}", ex.Message);
				throw;
			}
		}
	}
}
