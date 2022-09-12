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
	public class NtiWarningLetterReasonsService : AbstractService, INtiWarningLetterReasonsService
	{
		private readonly ILogger<NtiWarningLetterReasonsService> _logger;

		public NtiWarningLetterReasonsService(IHttpClientFactory clientFactory,
			ILogger<NtiWarningLetterReasonsService> logger) : base(clientFactory, logger)
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
				var client = ClientFactory.CreateClient(HttpClientName);

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