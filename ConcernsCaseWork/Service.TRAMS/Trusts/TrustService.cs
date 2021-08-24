using Microsoft.Extensions.Logging;
using Service.TRAMS.Base;
using Service.TRAMS.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Service.TRAMS.Trusts
{
	public sealed class TrustService : AbstractService, ITrustService
	{
		private readonly ILogger<TrustService> _logger;
		
		public TrustService(IHttpClientFactory clientFactory, ILogger<TrustService> logger) : base(clientFactory)
		{
			_logger = logger;
		}
		
		public async Task<IList<TrustDto>> GetTrustsByPagination(int page = 1)
		{
			try
			{
				_logger.LogInformation("TrustService::GetTrustsByPagination");

				// Create a request
				using var request = new HttpRequestMessage(HttpMethod.Get, $"/trusts?page={page}");
				
				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);
					
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();

				// Read response content
				var content = await response.Content.ReadAsStringAsync();

				// Deserialize content to POJO
				var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
				var trusts = JsonSerializer.Deserialize<IList<TrustDto>>(content, options);
				
				return trusts;
			}
			catch (Exception ex)
			{
				_logger.LogError($"TrustService::GetTrustsByPagination::Exception message::{ex.Message}");
			}

			return Array.Empty<TrustDto>();
		}
	}
}