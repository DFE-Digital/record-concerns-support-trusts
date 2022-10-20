using ConcernsCaseWork.Service.Base;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Ratings
{
	public sealed class RatingService : ConcernsAbstractService, IRatingService
	{
		private readonly ILogger<RatingService> _logger;
		
		public RatingService(IHttpClientFactory clientFactory, ILogger<RatingService> logger) : base(clientFactory, logger)
		{
			_logger = logger;
		}

		public async Task<IList<RatingDto>> GetRatings()
		{
			try
			{
				_logger.LogInformation("RatingService::GetRatings");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{EndpointsVersion}/concerns-ratings");
				
				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);
				
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POCO
				var apiWrapperRatingsDto = JsonConvert.DeserializeObject<ApiListWrapper<RatingDto>>(content);

				// Unwrap response
				if (apiWrapperRatingsDto is { Data: { } })
				{
					return apiWrapperRatingsDto.Data;
				}

				throw new Exception("Academies API error unwrap response");
			}
			catch (Exception ex)
			{
				_logger.LogError("RatingService::GetRatings::Exception message::{Message}", ex.Message);

				throw;
			}
		}
	}
}