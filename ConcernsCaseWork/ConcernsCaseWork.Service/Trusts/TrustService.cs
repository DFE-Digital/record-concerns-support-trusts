using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.Configuration;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
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
		private readonly IFeatureManager _featureManager;
		private readonly ICityTechnologyCollegeService _cityTechnologyCollegeService;

		private const string EndpointV3 = "v3";

		public TrustService(
			IHttpClientFactory clientFactory, 
			ILogger<TrustService> logger, 
			ICorrelationContext correlationContext, 
			IClientUserInfoService userInfoService,
			IFakeTrustService fakeTrustService,
			ICityTechnologyCollegeService cityTechnologyCollegeService,
			IFeatureManager featureManager) : base(clientFactory, logger, correlationContext, userInfoService)
		{
			_logger = logger;
			_fakeTrustService = fakeTrustService;
			_featureManager = featureManager;
			_cityTechnologyCollegeService = cityTechnologyCollegeService;
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

				var v3Enabled = await _featureManager.IsEnabledAsync(FeatureFlags.IsV3TrustSearchEnabled);
				var endpointVersion = v3Enabled ? EndpointV3 : EndpointsVersion;


				TrustDetailsDto cityTechnologyCollege = await CheckForCTCByUKPRN(ukPrn);
				if (cityTechnologyCollege != null)
				{
					return cityTechnologyCollege;
				}

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
		
		private async Task<TrustDetailsDto> CheckForCTCByUKPRN(string ukPrn)
		{
			TrustDetailsDto cityTechnologyCollege = null;

			bool ShouldCTCBeIncludedInTrustSearch = await _featureManager.IsEnabledAsync(FeatureFlags.IsCTCInTrustSearchEnabled); ;
			if (ShouldCTCBeIncludedInTrustSearch)
			{
				_logger.LogInformation($"TrustService::GetTrustByUkPrn Feature Flag ShouldCTCBeAddedToTrustSearch True. Starting Search for CTCs");


				try
				{
					cityTechnologyCollege = await _cityTechnologyCollegeService.GetCollegeByUkPrn(ukPrn);
					if (cityTechnologyCollege != null)
					{
						_logger.LogInformation($"TrustService::GetTrustByUkPrn Found CTC , returning {cityTechnologyCollege.GiasData.GroupName}");
					}
				}
				catch (Exception ex)
				{
					_logger.LogInformation($"TrustService::GetTrustByUkPrn An error occured searching for CTCs. Contining search from Trust List");
					_logger.LogError("TrustService::GetTrustByUkPrn::Exception message::{Message}", ex.Message);
				}
			}
			else
			{
				_logger.LogInformation($"TrustService::GetTrustByUkPrn Feature Flag ShouldCTCBeIncludedInTrustSearch False. Skipping Check from CTC list");
			}
			return cityTechnologyCollege;
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

				var v3Enabled = await _featureManager.IsEnabledAsync(FeatureFlags.IsV3TrustSearchEnabled);
				var endpointVersion = v3Enabled ? EndpointV3 : EndpointsVersion;

				Int32 maxResultsFromApi = maxRecordsPerPage;
				TrustSearchResponseDto ctcList = await Test(trustSearch.GroupName);
				if (ctcList != null)
				{
					maxResultsFromApi = maxRecordsPerPage - ctcList.Trusts.Count();
				}

				// Create a request
				var endpoint = $"/{endpointVersion}/trusts?{BuildRequestUri(trustSearch, maxResultsFromApi)}";
				var response = await GetByPagination<TrustSearchDto>(endpoint);

				//Combine the results of Trusts and CTC Searches
				List<TrustSearchDto> combinedMatches = new List<TrustSearchDto>();
				if (ctcList !=null && ctcList.Trusts.Count() >0)
				{
					combinedMatches.AddRange(ctcList.Trusts);
				}
				if (response.Data !=null)
				{
					combinedMatches.AddRange(response.Data);
				}

				return new TrustSearchResponseDto { NumberOfMatches = combinedMatches.Count(), Trusts = combinedMatches };
			}
			catch (Exception ex)
			{
				_logger.LogError("TrustService::GetTrustsByPagination::Exception message::{Message}", ex.Message);

				throw;
			}
		}

		private async Task<TrustSearchResponseDto> Test(string GroupName)
		{
			bool ShouldCTCBeIncludedInTrustSearch = await _featureManager.IsEnabledAsync(FeatureFlags.IsCTCInTrustSearchEnabled);

			TrustSearchResponseDto ctcList = null;

			if (ShouldCTCBeIncludedInTrustSearch)
			{
				_logger.LogInformation($"TrustService::GetTrustsByPagination Feature Flag ShouldCTCBeIncludedInTrustSearch True. Starting Search for CTCs");
				try
				{
					ctcList = await _cityTechnologyCollegeService.GetCollegeByPagination(GroupName);
					if (ctcList != null)
					{
						_logger.LogInformation($"TrustService::GetTrustsByPagination Found items in CTC list, returning {ctcList.Trusts.Count} results");
					}
				}
				catch (Exception ex)
				{
					_logger.LogInformation($"TrustService::GetTrustsByPagination An error occured searching for CTCs. Contining search from Trust List");
					_logger.LogError("TrustService::GetTrustsByPagination::Exception message::{Message}", ex.Message);
				}
			}
			else
			{
				_logger.LogInformation($"TrustService::GetTrustsByPagination Feature Flag ShouldCTCBeIncludedInTrustSearch False. Skipping Check from CTC list");
			}
			return ctcList;
		}

		private bool IsNumeric(string input)
		{
			return int.TryParse(input, out _ );
		}
	}
}