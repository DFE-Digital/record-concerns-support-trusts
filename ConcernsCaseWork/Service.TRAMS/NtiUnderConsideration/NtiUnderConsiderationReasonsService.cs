using ConcernsCaseWork.Logging;
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
	public class NtiUnderConsiderationReasonsService : AbstractService, INtiUnderConsiderationReasonsService
	{
		private readonly ILogger<NtiUnderConsiderationReasonsService> _logger;

		public NtiUnderConsiderationReasonsService(IHttpClientFactory clientFactory, ILogger<NtiUnderConsiderationReasonsService> logger, ICorrelationContext correlationContext) : base(clientFactory, logger, correlationContext)
		{
			_logger = logger;
		}

		public async Task<ICollection<NtiUnderConsiderationReasonDto>> GetAllReasons()
		{
			try
			{
				_logger.LogInformation("NtiReasonsService::GetAllReasons");

				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{_endpointsVersion}/case-actions/nti-under-consideration/all-reasons");

				// Create http client
				var client = CreateHttpClient();

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
