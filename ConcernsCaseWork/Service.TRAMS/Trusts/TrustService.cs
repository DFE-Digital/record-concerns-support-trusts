using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.TRAMS.Base;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Service.TRAMS.Trusts
{
	public sealed class TrustService : AbstractService, ITrustService
	{
		private readonly ILogger<TrustService> _logger;
		
		public TrustService(IHttpClientFactory clientFactory, ILogger<TrustService> logger) : base(clientFactory)
		{
			_logger = logger;
		}
		
		public async Task<ApiListWrapper<TrustSearchDto>> GetTrustsByPagination(TrustSearch trustSearch)
		{
			try
			{
				_logger.LogInformation("TrustService::GetTrustsByPagination");

				// Create a request
				using var request = new HttpRequestMessage(HttpMethod.Get, $"/{EndpointsVersion}/trusts?{BuildRequestUri(trustSearch)}");
				
				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);
					
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();

				// Read response content
				var content = await response.Content.ReadAsStringAsync();

				// Deserialize content to POCO
				var apiWrapperTrusts = JsonConvert.DeserializeObject<ApiListWrapper<TrustSearchDto>>(content);
				
				return apiWrapperTrusts;
			}
			catch (Exception ex)
			{
				_logger.LogError("TrustService::GetTrustsByPagination::Exception message::{Message}", ex.Message);
				
				throw;
			}
		}

		public async Task<TrustDetailsDto> GetTrustByUkPrn(string ukPrn)
		{
			try
			{
				_logger.LogInformation("TrustService::GetTrustByUkPrn");

				// Create a request
				using var request = new HttpRequestMessage(HttpMethod.Get, $"/{EndpointsVersion}/trust/{ukPrn}");
				
				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);
					
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();

				// Read response content
				var content = await response.Content.ReadAsStringAsync();

				// Deserialize content to POCO
				var apiWrapperTrustDetails = JsonConvert.DeserializeObject<ApiWrapper<TrustDetailsDto>>(content);
				
				// Unwrap response
				if (apiWrapperTrustDetails is { Data: { } })
				{
					return apiWrapperTrustDetails.Data;
				}

				throw new Exception("Academies API error unwrap response");
			}
			catch (Exception ex)
			{
				_logger.LogError("TrustService::GetTrustByUkPrn::Exception message::{Message}", ex.Message);
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
			
			return HttpUtility.UrlEncode(queryParams.ToString());
		}
	}
}