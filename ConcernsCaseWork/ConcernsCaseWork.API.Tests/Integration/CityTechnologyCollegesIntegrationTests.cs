using AutoFixture;
using ConcernsCaseWork.API.Tests.Fixtures;
using ConcernsCaseWork.API.Tests.Helpers;
using ConcernsCaseWork.Data;
using ConcernsCaseWork.Data.Models;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Integration
{
	[Collection(ApiTestCollection.ApiTestCollectionName)]
	public class CityTechnologyCollegesIntegrationTests
	{
		private readonly Fixture _autoFixture;
		private readonly HttpClient _client;
		private readonly ApiTestFixture _testFixture;

		public CityTechnologyCollegesIntegrationTests(ApiTestFixture apiTestFixture)
		{
			_client = apiTestFixture.Client;
			_autoFixture = new Fixture();
			_testFixture = apiTestFixture;
		}

		public class CityTechnologyCollegeResponse
		{
			public int Id { get; set; }
			public string Name { get; set; }
			public string UKPRN { get; set; }
			public string CompaniesHouseNumber { get; set; }

			public string AddressLine1 { get; set; }
			public string AddressLine2 { get; set; }
			public string AddressLine3 { get; set; }
			public string Town { get; set; }
			public string County { get; set; }
			public string Postcode { get; set; }
		}

		protected CityTechnologyCollege BuildRecord()
		{
			var result = CityTechnologyCollege.Create(_autoFixture.Create<string>(), _autoFixture.Create<string>().Substring(0, 12), _autoFixture.Create<string>().Substring(0, 8));
			result.ChangeAddress(_autoFixture.Create<string>(), _autoFixture.Create<string>(), _autoFixture.Create<string>(), _autoFixture.Create<string>(), _autoFixture.Create<string>(), _autoFixture.Create<string>().Substring(0, 9));
			return result;
		}

		[Fact]
		public async Task When_CreateNewCTC_Return_OK_And_ResourceUrl()
		{
			//Arrange
			CityTechnologyCollege ctc = BuildRecord();
			
			//Act
			HttpResponseMessage postResponse = await _client.PostAsync("/v2/citytechnologycolleges", ctc.ConvertToJson());
			var createdUrl = postResponse.Headers.Location;
			var getResponse = await _client.GetAsync(createdUrl);
			var actual = await getResponse.Content.ReadFromJsonAsync<CityTechnologyCollegeResponse>();


			//Assert
			postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
			actual.Should().BeEquivalentTo(ctc);
		}

		[Fact]
		public async Task When_GetList_Return_OK_And_Created_Items()
		{
			//Arrange
			await using ConcernsDbContext context = _testFixture.GetContext();

			CityTechnologyCollege ctc = BuildRecord();

			context.CityTechnologyColleges.Add(ctc);
			await context.SaveChangesAsync();

			//Act
			var response = await _client.GetAsync($"/v2/citytechnologycolleges");
			var wrapper = await response.Content.ReadFromJsonAsync<List<CityTechnologyCollegeResponse>>();

			//Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			wrapper.Count.Should().BeGreaterThan(0);
			var actual = wrapper.SingleOrDefault(f => f.Id == ctc.Id);
			actual.Should().BeEquivalentTo(ctc);
		}

		[Fact]
		public async Task When_GetList_Return_OK_And_Created_ItemsWithQueryStrings()
		{
			//Arrange
			await using ConcernsDbContext context = _testFixture.GetContext();

			CityTechnologyCollege ctc = BuildRecord();
			context.CityTechnologyColleges.Add(ctc);
			await context.SaveChangesAsync();


			//Act
			var response = await _client.GetAsync($"/v2/citytechnologycolleges/?NameUKPRNCHNumber=test");
			var wrapper = await response.Content.ReadFromJsonAsync<List<CityTechnologyCollegeResponse>>();

			//Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			wrapper.Count.Should().Be(0);
		}


		[Fact]
		public async Task When_Get_IndividualItem_Return_OK()
		{
			//Arrange
			await using ConcernsDbContext context = _testFixture.GetContext();

			CityTechnologyCollege ctcA = BuildRecord();
			CityTechnologyCollege ctcB = BuildRecord();

			context.CityTechnologyColleges.AddRange(ctcA, ctcB);
			await context.SaveChangesAsync();

			//Act
			var result = await _client.GetAsync($"/v2/citytechnologycolleges/ukprn/{ctcA.UKPRN}");

			//Asset
			result.StatusCode.Should().Be(HttpStatusCode.OK);
		}


		[Fact]
		public async Task When_Get_IndividualItem_WhenNoneExist_Return_OK()
		{
			string urkprn = "123465789";
			var result = await _client.GetAsync($"/v2/citytechnologycolleges/ukprn/{urkprn}");
			result.StatusCode.Should().Be(HttpStatusCode.NoContent);
		}

	}
}
