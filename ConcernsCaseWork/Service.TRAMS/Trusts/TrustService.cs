using Microsoft.Extensions.Logging;
using Service.TRAMS.Base;
using Service.TRAMS.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
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
		
		public async Task<IList<TrustSummaryDto>> GetTrustsByPagination(TrustSearch trustSearch)
		{
			try
			{
				_logger.LogInformation("TrustService::GetTrustsByPagination");

				// Create a request
				using var request = new HttpRequestMessage(HttpMethod.Get, BuildRequestUri(trustSearch));
				
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
				var trusts = JsonSerializer.Deserialize<IList<TrustSummaryDto>>(content, options);
				
				return trusts;
			}
			catch (Exception ex)
			{
				_logger.LogError($"TrustService::GetTrustsByPagination::Exception message::{ex.Message}");
			}

			return Array.Empty<TrustSummaryDto>();
		}

		public async Task<TrustDetailsDto> GetTrustByUkPrn(string ukPrn)
		{
			try
			{
				_logger.LogInformation("TrustService::GetTrustByUkPrn");

				// Create a request
				using var request = new HttpRequestMessage(HttpMethod.Get, $"/trust/{ukPrn}");
				
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
				var trustDetails = JsonSerializer.Deserialize<TrustDetailsDto>(content, options);
				
				return trustDetails;
			}
			catch (Exception ex)
			{
				_logger.LogError($"TrustService::GetTrustByUkPrn::Exception message::{ex.Message}");
				throw;
			}
		}
		
		public string BuildRequestUri(TrustSearch trustSearch)
		{
			var queryParams = HttpUtility.ParseQueryString(string.Empty);
			if (!string.IsNullOrEmpty(trustSearch.GroupName))
			{
				queryParams.Add("groupName", trustSearch.GroupName);
			}
			if (!string.IsNullOrEmpty(trustSearch.Ukprn))
			{
				queryParams.Add("ukprn", trustSearch.Ukprn);
			}
			if (!string.IsNullOrEmpty(trustSearch.CompaniesHouseNumber))
			{
				queryParams.Add("companiesHouseNumber", trustSearch.CompaniesHouseNumber);
			}
			queryParams.Add("page", trustSearch.Page.ToString());
			
			return $"/trusts?{HttpUtility.UrlEncode(queryParams.ToString())}";
		}
	}
}