using Microsoft.Extensions.Logging;
using Service.TRAMS.Base;
using Service.TRAMS.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service.TRAMS.Rating
{
	public sealed class RatingService : AbstractService, IRatingService
	{
		private readonly ILogger<RatingService> _logger;
		
		public RatingService(IHttpClientFactory clientFactory, ILogger<RatingService> logger) : base(clientFactory)
		{
			_logger = logger;
		}

		public async Task<IList<RatingDto>> GetRatings()
		{
			try
			{
				_logger.LogInformation("RatingService::GetRatings");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, "/ratings");
				
				// Create http client
				var client = ClientFactory.CreateClient("TramsClient");
				
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
				var ratingsDto = JsonSerializer.Deserialize<IList<RatingDto>>(content, options);

				return ratingsDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"RatingService::GetRatings::Exception message::{ex.Message}");
			}
			
			return Array.Empty<RatingDto>();
		}
	}
}