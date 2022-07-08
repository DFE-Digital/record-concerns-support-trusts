using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.TRAMS.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.Nti
{
	public class NtiStatusesService : AbstractService, INtiStatusesService
	{
		private readonly ILogger<NtiReasonsService> _logger;

		public NtiStatusesService(IHttpClientFactory clientFactory, ILogger<NtiReasonsService> logger) : base(clientFactory)
		{
			_logger = logger;
		}

		public async Task<ICollection<NtiStatusDto>> GetAllStatuses()
		{
			try
			{
				_logger.LogInformation("NtiStatusesService::GetAllStatuses");

				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{EndpointsVersion}/case-actions/nti-under-consideration/all-statuses");

				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);

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
				_logger.LogError("NtiStatusesService::GetAllStatuses::Exception message::{Message}", ex.Message);
				throw;
			}
		}
	}
}
