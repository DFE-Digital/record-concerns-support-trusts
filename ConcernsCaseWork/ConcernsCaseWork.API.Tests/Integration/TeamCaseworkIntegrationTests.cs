using AutoFixture;
using ConcernsCaseWork.API.RequestModels.Concerns.TeamCasework;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.Tests.Fixtures;
using ConcernsCaseWork.API.Tests.Helpers;
using ConcernsCaseWork.Data.Models;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Integration
{
	[Collection(ApiTestCollection.ApiTestCollectionName)]
	public class TeamCaseworkIntegrationTests: CaseWorkIntegrationTestsBase
	{


		public TeamCaseworkIntegrationTests(ApiTestFixture apiTestFixture):base(apiTestFixture)
		{

		}

		[Fact]
		public async Task When_Put_InvalidRequest_Returns_ValidationErrors()
		{
			var request = _fixture.Create<ConcernsCaseworkTeamUpdateRequest>();
			request.OwnerId = new string('a', 301);

			var result = await _client.PutAsync($"/v2/concerns-team-casework/owners/1", request.ConvertToJson());
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);


			var error = await result.Content.ReadAsStringAsync();
			error.Should().Contain("The field OwnerId must be a string with a maximum length of 300.");
		}

		[Fact]
		public async Task When_Put_InvalidQuery_Returns_ValidationErrors()
		{
			var request = _fixture.Create<ConcernsCaseworkTeamUpdateRequest>();
			var ownerId = new string('a', 301);

			var result = await _client.PutAsync($"/v2/concerns-team-casework/owners/{ownerId}", request.ConvertToJson());
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

			var error = await result.Content.ReadAsStringAsync();
			error.Should().Contain("The field ownerId must be a string with a maximum length of 300.");
		}



		[Fact]
		public async Task When_HasActiveCasesWithCaseActions_Returns_CorrectInformation_200()
		{
			//Arrange
			var request = await CreateTeam();
			var owner = request.OwnerId;

			//Act and Assert
			await HasActiveCasesWithCaseActions_Returns_CorrectInformation_200(request.TeamMembers.FirstOrDefault(), $"/v2/concerns-cases/summary/{owner}/active/team");
		}


		[Fact]
		public async Task When_HasActiveCases_Returns_AllCases_200()
		{
			//Arrange
			var arrangeResult = await CreateTeamAndCasesForOwnerAndTeamMember();
			var expectedCases = arrangeResult.expectedCases;
			var owner = arrangeResult.owner;

			//Act
			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/{owner}/active/team");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ActiveCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			//Assert
			wrapper.Paging.Should().BeNull();
			result.Should().HaveCount(10);
			CaseSummaryAssert.AssertCaseList(result.Cast<CaseSummaryResponse>().ToList(), expectedCases);
		}

		[Fact]
		public async Task When_HasNoActiveCases_Returns_Empty_200()
		{
			//Arrange
			var request = await CreateTeam();
			var owner = request.OwnerId;

			//Act
			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/{owner}/active/team");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ActiveCaseSummaryResponse>>();
			var result = wrapper.Data;
			
			//Assert
			result.Should().HaveCount(0);
		}

		[Fact]
		public async Task When_HasActiveCases_PaginationOnlyNext_Returns_200()
		{
			//Arrange
			var request = await CreateTeam();
			var owner = request.OwnerId;

			//Act And Assert
			await HasActiveCases_PaginationOnlyNext_Returns_200(request.OwnerId, request.TeamMembers.FirstOrDefault(), $"/v2/concerns-cases/summary/{owner}/active/team?page=1&count=2");
		}

		[Fact]
		public async Task When_HasActiveCases_PaginationNextAndPrevious_Returns_200()
		{
			//Arrange
			var arrangeResult = await CreateTeamAndCasesForOwnerAndTeamMember();
			var cases = arrangeResult.expectedCases;
			var owner = arrangeResult.owner;

			var expectedCases = cases.Skip(4).Take(2).ToList();

			//Act
			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/{owner}/active/team?page=3&count=2");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ActiveCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			//Assert
			result.Should().HaveCount(2);
			CaseSummaryAssert.AssertCaseList(result.Cast<CaseSummaryResponse>().ToList(), expectedCases);
			wrapper.Paging.RecordCount.Should().Be(10);
			wrapper.Paging.HasNext.Should().BeTrue();
			wrapper.Paging.HasPrevious.Should().BeTrue();
		}

		[Fact]
		public async Task When_HasActiveCases_PaginationPreviousOnly_Returns_200()
		{
			//Arrange
			var arrangeResult = await CreateTeamAndCasesForOwnerAndTeamMember();
			var cases = arrangeResult.expectedCases;
			var owner = arrangeResult.owner;

			var expectedCases = cases.Skip(8).Take(2).ToList();

			//Act
			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/{owner}/active/team?page=5&count=2");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ActiveCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			//Assert
			result.Should().HaveCount(2);
			CaseSummaryAssert.AssertCaseList(result.Cast<CaseSummaryResponse>().ToList(), expectedCases);
			wrapper.Paging.RecordCount.Should().Be(10);
			wrapper.Paging.HasNext.Should().BeFalse();
			wrapper.Paging.HasPrevious.Should().BeTrue();
		}

		[Fact]
		public async Task When_HasActiveCases_RequestPageGreaterThanAvailable_Returns_Empty_200()
		{
			//Arrange
			var arrangeResult = await CreateTeamAndCasesForOwnerAndTeamMember();
			var owner = arrangeResult.owner;

			//Act
			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/{owner}/active/team?page=6&count=2");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ClosedCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();
			
			//Assert
			result.Should().HaveCount(0);
		}

		protected async Task<ConcernsCaseworkTeamUpdateRequest> CreateTeam()
		{
			var request = _fixture.Create<ConcernsCaseworkTeamUpdateRequest>();

			var result = await _client.PutAsync($"/v2/concerns-team-casework/owners/{request.OwnerId}", request.ConvertToJson());

			return request;
		}

		public async Task<(List<ConcernsCase> expectedCases, string owner)> CreateTeamAndCasesForOwnerAndTeamMember()
		{
			var request = await CreateTeam();
			var owner = request.OwnerId;
			var differentOwner = request.TeamMembers.FirstOrDefault();
			await BulkCreateActiveCases(owner);
			var expectedCases = await BulkCreateActiveCases(differentOwner);

			return (expectedCases, owner);
		}
	}
}
