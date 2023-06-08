using Ardalis.GuardClauses;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Service.Configuration;
using ConcernsCaseWork.UserContext;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Newtonsoft.Json;
using System.Web;

namespace ConcernsCaseWork.Service.Trusts
{
	public sealed class TrustService : TramsAbstractService, ITrustService
	{
		private readonly ILogger<TrustService> _logger;
		private readonly IFakeTrustService _fakeTrustService;
		IFeatureManager _featureManager;

		private const string EndpointV3 = "v3";

		public TrustService(
			IHttpClientFactory clientFactory, 
			ILogger<TrustService> logger, 
			ICorrelationContext correlationContext, 
			IClientUserInfoService userInfoService,
			IFakeTrustService fakeTrustService,
			IFeatureManager featureManager) : base(clientFactory, logger, correlationContext, userInfoService)
		{
			_logger = logger;
			_fakeTrustService = fakeTrustService;
			_featureManager = featureManager;
		}

		public string BuildRequestUri(TrustSearch trustSearch, int maxRecordsPerPage)
		{
			var queryParams = HttpUtility.ParseQueryString(string.Empty);
			if (!string.IsNullOrEmpty(trustSearch.GroupName))
			{
				queryParams.Add("groupName", trustSearch.GroupName);
			}
			if (IsNumeric(trustSearch.Ukprn))
			{
				queryParams.Add("ukprn", trustSearch.Ukprn);
			}
			if (!string.IsNullOrEmpty(trustSearch.CompaniesHouseNumber))
			{
				queryParams.Add("companiesHouseNumber", trustSearch.CompaniesHouseNumber);
			}
			queryParams.Add("page", trustSearch.Page.ToString());
			queryParams.Add("count", maxRecordsPerPage.ToString());
			queryParams.Add("includeEstablishments", false.ToString());

			return HttpUtility.UrlEncode(queryParams.ToString());
		}

		public async Task<TrustDetailsDto> GetTrustByUkPrn(string ukPrn)
		{
			try
			{
				_logger.LogInformation("TrustService::GetTrustByUkPrn");

				var fakeTrust = _fakeTrustService.GetTrustByUkPrn(ukPrn);

				if (fakeTrust != null)
				{
					_logger.LogInformation($"TrustService::GetTrustByUkPrn Found fake trust, returning {fakeTrust.GiasData.GroupName}");
					return fakeTrust;
				}

				var v3Enabled = await _featureManager.IsEnabledAsync(FeatureFlags.V3TrustSearch);
				var endpointVersion = v3Enabled ? EndpointV3 : EndpointsVersion;

				// Create a request
				using var request = new HttpRequestMessage(HttpMethod.Get, $"/{endpointVersion}/trust/{ukPrn}");

				// Create http client
				var client = CreateHttpClient();

				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();

				// Read response content
				var content = await response.Content.ReadAsStringAsync();

				var apiWrapperTrustDetails = ProcessSearchByUkPrnResponse(content, v3Enabled);

				return apiWrapperTrustDetails;
			}
			catch (Exception ex)
			{
				_logger.LogError("TrustService::GetTrustByUkPrn::Exception message::{Message}", ex.Message);
				throw;
			}
		}

		private static TrustDetailsDto ProcessSearchByUkPrnResponse(string content, bool v3Enabled)
		{
			if (!v3Enabled)
			{
				return JsonConvert.DeserializeObject<ApiWrapper<TrustDetailsDto>>(content).Data;
			}

			var v3Response = JsonConvert.DeserializeObject<ApiWrapper<TrustDetailsV3Dto>>(content);
			var v2Response = new TrustDetailsDto()
			{
				IfdData = v3Response.Data.TrustData,
				Establishments = v3Response.Data.Establishments,
				GiasData = v3Response.Data.GiasData
			};

			return v2Response;
		}

		public async Task<TrustSearchResponseDto> GetTrustsByPagination(TrustSearch trustSearch, int maxRecordsPerPage)
		{
			Guard.Against.Null(trustSearch);
			Guard.Against.NegativeOrZero(maxRecordsPerPage);

			try
			{
				_logger.LogInformation("TrustService::GetTrustsByPagination");

				var fakeTrust = _fakeTrustService.GetTrustsByPagination(trustSearch.GroupName);

				if (fakeTrust != null)
				{
					_logger.LogInformation($"TrustService::GetTrustsByPagination Found fake trust, returning {fakeTrust.Trusts.Count} results");
					return fakeTrust;
				}

				var v3Enabled = await _featureManager.IsEnabledAsync(FeatureFlags.V3TrustSearch);
				var endpointVersion = v3Enabled ? EndpointV3 : EndpointsVersion; 

				// Create a request
				var endpoint = $"/{endpointVersion}/trusts?{BuildRequestUri(trustSearch, maxRecordsPerPage)}";

				var response = await GetByPagination<TrustSearchDto>(endpoint);

				return new TrustSearchResponseDto { NumberOfMatches = response.Paging.RecordCount, Trusts = response.Data };
			}
			catch (Exception ex)
			{
				_logger.LogError("TrustService::GetTrustsByPagination::Exception message::{Message}", ex.Message);

				throw;
			}
		}

		private bool IsNumeric(string input)
		{
			return int.TryParse(input, out _ );
		}
	}
}