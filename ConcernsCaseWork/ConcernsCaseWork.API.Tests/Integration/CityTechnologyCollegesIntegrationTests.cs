using AutoFixture;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.RequestModels.CaseActions.SRMA;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.Tests.Fixtures;
using ConcernsCaseWork.Data;
using ConcernsCaseWork.Data.Models;
using FizzWare.NBuilder;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
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
			_autoFixture.Customize<CityTechnologyCollege>(c => c.Without(w => w.Id));

		}

		protected CityTechnologyCollege BuildRecord()
		{
			return new CityTechnologyCollege()
			{
				Name = _autoFixture.Create<string>(),
				UKPRN = _autoFixture.Create<string>().Substring(0, 12),
				CompaniesHouseNumber = _autoFixture.Create<string>().Substring(0, 8),
				AddressLine1 = _autoFixture.Create<string>(),
				AddressLine2 = _autoFixture.Create<string>(),
				AddressLine3 = _autoFixture.Create<string>(),
				Town = _autoFixture.Create<string>(),
				County = _autoFixture.Create<string>(),
				Postcode = _autoFixture.Create<string>().Substring(0, 9),
			};
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
			var wrapper = await response.Content.ReadFromJsonAsync<List<CityTechnologyCollege>>();

			//Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			wrapper.Count.Should().Be(1);

			//Tidy
			context.CityTechnologyColleges.Remove(ctc);
			await context.SaveChangesAsync();
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


			context.CityTechnologyColleges.Remove(ctcA);
			context.CityTechnologyColleges.Remove(ctcB);

			await context.SaveChangesAsync();
		}


		[Fact]
		public async Task When_Get_IndividualItem_WhenNoneExist_Return_OK()
		{
			CityTechnologyCollege ctcA = _autoFixture.Create<CityTechnologyCollege>();
			var result = await _client.GetAsync($"/v2/citytechnologycolleges/ukprn/{ctcA.UKPRN}");
			result.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

	}
}
