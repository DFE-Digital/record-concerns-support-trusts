using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.UserContext;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Service.Trusts
{
	public interface ICityTechnologyCollegeService
	{
		Task<TrustSearchResponseDto> GetCollegeByPagination(string groupName);
		Task<TrustDetailsDto> GetCollegeByUkPrn(string ukPrn);
	}

	public class CityTechnologyCollegeService : ConcernsAbstractService, ICityTechnologyCollegeService
	{
		private const string _endpointVersion = "v2";
		private readonly string _defaultGroupTypeName = "City Technology College";

		public CityTechnologyCollegeService(IHttpClientFactory clientFactory,
			ILogger<CityTechnologyCollegeService> logger,
			ICorrelationContext correlationContext,
			IClientUserInfoService userInfoService) : base(clientFactory, logger, correlationContext, userInfoService)
		{

		}

		protected class CityTechnologyCollege
		{
			public string Name { get; set; }
			public string UKPRN { get; set; }
			public string CompaniesHouseNumber { get; set; }
			public string AddressLine1 { get; set; }
			public string AddressLine2 { get; set; }
			public string AddressLine3 { get; set; }
			public string County { get; set; }
			public string Town { get; set; }
			public string Postcode { get; set; }

		}

		public async Task<TrustSearchResponseDto> GetCollegeByPagination(string groupName)
		{
			// Create a request
			using var request = new HttpRequestMessage(HttpMethod.Get, $"/{_endpointVersion}/citytechnologycolleges?NameUKPRNCHNumber={groupName}");

			// Create http client
			var client = CreateHttpClient();

			// Execute request
			var response = await client.SendAsync(request);

			// Check status code
			response.EnsureSuccessStatusCode();

			// Read response content
			var result = await response.Content.ReadFromJsonAsync<List<CityTechnologyCollege>>();


			List<TrustSearchDto> trustObjectList = result.Select(f => new TrustSearchDto(f.UKPRN, string.Empty, f.Name, f.CompaniesHouseNumber, _defaultGroupTypeName, new GroupContactAddressDto(f.AddressLine1, f.AddressLine2, String.Empty, f.Town, f.County, f.Postcode))).ToList();

			return new TrustSearchResponseDto { NumberOfMatches = trustObjectList.Count, Trusts = trustObjectList };
		}

		public async Task<TrustDetailsDto> GetCollegeByUkPrn(string ukPrn)
		{
			// Create a request
			using var request = new HttpRequestMessage(HttpMethod.Get, $"/{_endpointVersion}/citytechnologycolleges/ukprn/{ukPrn}");

			// Create http client
			var client = CreateHttpClient();

			// Execute request
			var response = await client.SendAsync(request);

			// Check status code
			response.EnsureSuccessStatusCode();

			// Read response content
			var result = await response.Content.ReadFromJsonAsync<CityTechnologyCollege>();

			var trustDetails = new TrustDetailsDto()
			{
				IfdData = null,
				Establishments = null,
				GiasData = new GiasDataDto(result.UKPRN, string.Empty, result.Name,string.Empty, result.CompaniesHouseNumber, new GroupContactAddressDto(result.AddressLine1, result.AddressLine2, String.Empty, result.Town, result.County, result.Postcode), _defaultGroupTypeName)
			};

			return trustDetails;
		}
	}
}
