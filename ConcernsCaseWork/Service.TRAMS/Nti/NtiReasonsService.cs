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
	public class NtiReasonsService : AbstractService, INtiReasonsService
	{
		private readonly ILogger<NtiReasonsService> _logger;

		public NtiReasonsService(IHttpClientFactory clientFactory, ILogger<NtiReasonsService> logger) : base(clientFactory)
		{
			_logger = logger;
		}

		public async Task<ICollection<NtiReasonDto>> GetAllReasons()
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
				var apiWrapperRatingsDto = JsonConvert.DeserializeObject<ApiListWrapper<NtiReasonDto>>(content);

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
