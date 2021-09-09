using Microsoft.Extensions.Logging;
using Service.TRAMS.Base;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Numerics;
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
				var request = new HttpRequestMessage(HttpMethod.Get, $"{EndpointsVersion}/ratings");
				
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
			
			// TODO replace return when TRAMS API endpoints are live
			return new List<RatingDto>
			{
				new RatingDto("n/a", DateTime.Now, DateTime.Now, new BigInteger(1)),
				new RatingDto("Red-Plus", DateTime.Now, DateTime.Now, new BigInteger(2)),
				new RatingDto("Red", DateTime.Now, DateTime.Now, new BigInteger(3)),
				new RatingDto("Red-Amber", DateTime.Now, DateTime.Now, new BigInteger(4)),
				new RatingDto("Amber-Green", DateTime.Now, DateTime.Now, new BigInteger(5))
			};

			//return Array.Empty<RatingDto>();
		}
	}
}