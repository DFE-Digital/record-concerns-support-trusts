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
	public class NtiWarningLetterStatusesService : AbstractService, INtiWarningLetterStatusesService
	{
		private readonly ILogger<NtiWarningLetterStatusesService> _logger;

		public NtiWarningLetterStatusesService(IHttpClientFactory clientFactory,
			ILogger<NtiWarningLetterStatusesService> logger) : base(clientFactory)
		{
			_logger = logger;
		}

		public async Task<ICollection<NtiWarningLetterStatusDto>> GetAllStatusesAsync()
		{
			try
			{
				_logger.LogInformation("NtiWarningLetterStatusesService::GetAllStatusesAsync");

				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{EndpointsVersion}/case-actions/nti-warning-letter/all-statuses");

				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);

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
				_logger.LogError("NtiWarningLetterStatusesService::GetAllStatusesAsync::Exception message::{Message}", ex.Message);
				throw;
			}
		}
	}
}
