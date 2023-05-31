﻿using Ardalis.GuardClauses;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.UserContext;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Web;

namespace ConcernsCaseWork.Service.Trusts
{
	public sealed class TrustService : TramsAbstractService, ITrustService
	{
		private readonly ILogger<TrustService> _logger;
		private readonly string TrustEndpointVersion = "v3";

		public TrustService(IHttpClientFactory clientFactory, ILogger<TrustService> logger, ICorrelationContext correlationContext, IClientUserInfoService userInfoService) : base(clientFactory, logger, correlationContext, userInfoService)
		{
			_logger = logger;
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

				// Create a request
				using var request = new HttpRequestMessage(HttpMethod.Get, $"/{TrustEndpointVersion}/trust/{ukPrn}");

				// Create http client
				var client = CreateHttpClient();

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

		public async Task<TrustSearchResponseDto> GetTrustsByPagination(TrustSearch trustSearch, int maxRecordsPerPage)
		{
			Guard.Against.Null(trustSearch);
			Guard.Against.NegativeOrZero(maxRecordsPerPage);

			try
			{
				_logger.LogInformation("TrustService::GetTrustsByPagination");

				// Create a request
				var endpoint = $"/{TrustEndpointVersion}/trusts?{BuildRequestUri(trustSearch, maxRecordsPerPage)}";

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