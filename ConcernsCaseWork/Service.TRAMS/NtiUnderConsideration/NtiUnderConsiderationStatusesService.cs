using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.TRAMS.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.NtiUnderConsideration
{
	public class NtiUnderConsiderationStatusesService : AbstractService, INtiUnderConsiderationStatusesService
	{
		private readonly ILogger<NtiUnderConsiderationReasonsService> _logger;

		public NtiUnderConsiderationStatusesService(IHttpClientFactory clientFactory, ILogger<NtiUnderConsiderationReasonsService> logger) : base(clientFactory)
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
				var client = ClientFactory.CreateClient(HttpClientName);

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
