using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.Configuration;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.UserContext;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Newtonsoft.Json;
using System.Net;
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
		private const string EndpointV4 = "v4";

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
			_cityTechnologyCollegeService = cityTechnologyCollegeService;
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

				TrustDetailsDto cityTechnologyCollege = await CheckForCTCByUKPRN(ukPrn);

				if (cityTechnologyCollege != null)
				{
					return cityTechnologyCollege;
				}

				var isV4Enabled = await _featureManager.IsEnabledAsync(FeatureFlags.IsTrustSearchV4Enabled);
				var endpointVersion = isV4Enabled ? EndpointV4 : EndpointV3;

				var endpoint = $"/{endpointVersion}/trust/{ukPrn}";

				TrustDetailsDto result = null;

				if (isV4Enabled)
				{
					result = await GetTrustByUkPrnV4(endpoint);
				}
				else
				{
					result = await GetTrustByUkPrnV3(endpoint);
				}

				return result;
			}
			catch (Exception ex)
			{
				_logger.LogError("TrustService::GetTrustByUkPrn::Exception message::{Message}", ex.Message);
				throw;
			}
		}

		private async Task<TResponse> PerformGet<TResponse>(string endpoint)
		{
			var client = CreateHttpClient();

			using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

			var response = await client.SendAsync(request);

			response.EnsureSuccessStatusCode();

			var content = await response.Content.ReadAsStringAsync();

			var result = JsonConvert.DeserializeObject<TResponse>(content);

			return result;
		}

		private async Task<TrustDetailsDto> GetTrustByUkPrnV3(string endpoint)
		{
			var response = await PerformGet<ApiWrapper<TrustDetailsV3Dto>>(endpoint);

			var result = new TrustDetailsDto()
			{
				IfdData = response.Data.TrustData,
				Establishments = response.Data.Establishments,
				GiasData = response.Data.GiasData
			};

			return result;
		}

		private async Task<TrustDetailsDto> GetTrustByUkPrnV4(string ukPrn)
		{
			var trustDetailsResponse = await PerformGet<ApiWrapper<TrustDetailsV4Dto>>($"/{EndpointV4}/trust/{ukPrn}");

			var result = new TrustDetailsDto()
			{
				IfdData = new(),
				GiasData = new()
				{
					GroupName = trustDetailsResponse.Data.Name,
					UkPrn = trustDetailsResponse.Data.Ukprn,
					CompaniesHouseNumber = trustDetailsResponse.Data.CompaniesHouseNumber,
					GroupContactAddress = trustDetailsResponse.Data.Address,
					GroupType = trustDetailsResponse.Data.Type?.Name
				},
			};

			result.Establishments = await GetEstablishments(ukPrn);

			return result;
		}

		private async Task<List<EstablishmentDto>> GetEstablishments(string ukPrn)
		{
			var establishmentResponse = await PerformGet<List<EstablishmentV4Dto>>($"/v4/establishments/trust/{ukPrn}");

			var result = establishmentResponse.Select(e =>
			{
				return new EstablishmentDto()
				{
					Urn = e.Urn,
					EstablishmentNumber = e.EstablishmentNumber,
					EstablishmentName = e.EstablishmentName,
					HeadteacherTitle = e.HeadteacherTitle,
					HeadteacherFirstName = e.HeadteacherFirstName,
					HeadteacherLastName = e.HeadteacherLastName,
					EstablishmentType = e.EstablishmentType,
					Census = e.Census,
					SchoolWebsite = e.SchoolWebsite,
					SchoolCapacity = e.SchoolCapacity
				};
			}).ToList();

			return result;
		}

		private async Task<TrustDetailsDto> CheckForCTCByUKPRN(string ukPrn)
		{
			TrustDetailsDto cityTechnologyCollege = null;

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
				_logger.LogInformation($"TrustService::GetTrustByUkPrn An error occured searching for CTCs. Containing search from Trust List");
				_logger.LogError("TrustService::GetTrustByUkPrn::Exception message::{Message}", ex.Message);
			}

			return cityTechnologyCollege;
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

				Int32 maxResultsFromApi = maxRecordsPerPage;
				TrustSearchResponseDto ctcList = await SearchCtcs(trustSearch.GroupName);
				if (ctcList != null)
				{
					maxResultsFromApi = maxRecordsPerPage - ctcList.Trusts.Count();
				}

				var isV4Enabled = await _featureManager.IsEnabledAsync(FeatureFlags.IsTrustSearchV4Enabled);
				var endpointVersion = isV4Enabled ? EndpointV4 : EndpointV3;

				// Create a request
				var endpoint = $"/{endpointVersion}/trusts?{BuildRequestUri(trustSearch, maxResultsFromApi)}";

				ApiListWrapper<TrustSearchDto> response = null;

				if (isV4Enabled)
				{
					response = await SearchV4(endpoint);
				}
				else
				{
					response = await GetByPagination<TrustSearchDto>(endpoint);
				}

				//Combine the results of Trusts and CTC Searches
				List<TrustSearchDto> combinedMatches = new List<TrustSearchDto>();
				if (ctcList != null && ctcList.Trusts.Count() > 0)
				{
					combinedMatches.AddRange(ctcList.Trusts);
				}
				if (response.Data != null)
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

		private async Task<ApiListWrapper<TrustSearchDto>> SearchV4(string endpoint)
		{
			var response = await GetByPagination<TrustSearchV4Dto>(endpoint);

			var result = new ApiListWrapper<TrustSearchDto>()
			{
				Data = response.Data.Select(t =>
				{
					return new TrustSearchDto()
					{
						Urn = t.Urn,
						UkPrn = t.UkPrn,
						GroupName = t.GroupName,
						CompaniesHouseNumber = t.CompaniesHouseNumber,
						GroupContactAddress = t.GroupContactAddress,
						TrustType = t.TrustType
					};
				}).ToList(),
				Paging = response.Paging
			};

			return result;
		}

		private async Task<TrustSearchResponseDto> SearchCtcs(string groupName)
		{
			TrustSearchResponseDto ctcList = null;

			_logger.LogInformation($"TrustService::GetTrustsByPagination Feature Flag ShouldCTCBeIncludedInTrustSearch True. Starting Search for CTCs");
			try
			{
				ctcList = await _cityTechnologyCollegeService.GetCollegeByPagination(groupName);
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

			return ctcList;
		}

		private bool IsNumeric(string input)
		{
			return int.TryParse(input, out _);
		}
	}
}