using AutoFixture;
using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.Tests.Fixtures;
using ConcernsCaseWork.API.Tests.Helpers;
using ConcernsCaseWork.Data;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Integration;

public class CloseDecisionIntegrationTests : IClassFixture<ApiTestFixture>
{
	private HttpClient _client;
	private Fixture _fixture;
	private ApiTestFixture _apiTestFixture;

	public CloseDecisionIntegrationTests(ApiTestFixture apiTestFixture)
	{
		_apiTestFixture = apiTestFixture;
		_client = apiTestFixture.Client;
		_fixture = new();
	}

	[Fact]
	public async Task When_Patch_Returns_200Response()
	{
		// arrange
		var cCase = await CreateCase();
		var decisionId = await CreateDecisionWithOutcome(cCase.Id);

		var request = new CloseDecisionRequest()
		{
			SupportingNotes = _fixture.Create<string>()
		};
		
		// act
		var result = await _client
			.PatchAsync($"/v2/concerns-cases/{cCase.Urn}/decisions/{decisionId}/close", 
			request.ConvertToJson());

		// assert
		result.StatusCode.Should().Be(HttpStatusCode.OK);

		var response = await result.Content.ReadFromJsonAsync<ApiSingleResponseV2<CloseDecisionResponse>>();
		response.Data.DecisionId.Should().Be(decisionId);
		response.Data.CaseUrn.Should().Be(cCase.Urn);

		var dbDecision = GetContext().Decisions.Single(d => d.DecisionId == decisionId);
		dbDecision.ClosedAt.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromMinutes(1));
		dbDecision.SupportingNotes.Should().Be(request.SupportingNotes);
	}
	
	[Fact]
	public async Task When_Patch_AndDecisionIsClosed_ReturnsBadRequest()
	{
		// arrange
		var cCase = await CreateCase();
		var decisionId = await CreateClosedDecision(cCase.Id);

		var request = new CloseDecisionRequest()
		{
			SupportingNotes = _fixture.Create<string>()
		};
		
		// act
		var result = await _client
			.PatchAsync($"/v2/concerns-cases/{cCase.Urn}/decisions/{decisionId}/close", 
				request.ConvertToJson());

		result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

		var error = await result.Content.ReadFromJsonAsync<HttpErrorMessage>();
		error.Message.Should().Be($"Operation not completed: Decision with id {decisionId} cannot be closed as it is already closed.");
		error.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
	}

	[Fact]
	public async Task When_Patch_With_No_Outcome_Returns_BadRequest()
	{
		// arrange
		var cCase = await CreateCase();
		var decisionId = await CreateDecision(cCase.Id);

		var request = new CloseDecisionRequest()
		{
			SupportingNotes = _fixture.Create<string>()
		};
		
		// act
		var result = await _client
			.PatchAsync($"/v2/concerns-cases/{cCase.Urn}/decisions/{decisionId}/close", 
				request.ConvertToJson());

		result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

		var error = await result.Content.ReadFromJsonAsync<HttpErrorMessage>();
		error.Message.Should().Be($"Operation not completed: Decision with id {decisionId} cannot be closed as it does not have an Outcome.");
		error.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
	}
	
	[Theory]
	[InlineData(2001, -1, -2)]
	public async Task When_PatchWithInvalidRequest_Returns_400Response(int notesLength, int caseUrn, int decisionId)
	{
		// arrange
		var request = new CloseDecisionRequest()
		{
			SupportingNotes = _fixture.Create<string>().PadRight(notesLength, 'g')
		};

		var result = await _client.PatchAsync($"/v2/concerns-cases/{caseUrn}/decisions/{decisionId}/close", request.ConvertToJson());

		result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

		var error = await result.Content.ReadFromJsonAsync<ValidationErrors>();
		error.Should().NotBeNull();
		error!.Title.Should().Be("One or more validation errors occurred.");
		error.Status.Should().Be((int)HttpStatusCode.BadRequest);
		error.Errors.Single().Key.Should().Be("SupportingNotes");
		error.Errors.Single().Value.Single().Should().Be("Notes must be 2000 characters or less");
	}

	[Fact]
	public async Task When_PatchWithMissingCase_Returns_404Response()
	{
		var caseId = _fixture.Create<int>();
		
		var request = new CloseDecisionRequest()
		{
			SupportingNotes = _fixture.Create<string>()
		};

		var result = await _client.PatchAsync($"/v2/concerns-cases/{caseId}/decisions/{_fixture.Create<int>()}/close", request.ConvertToJson());

		result.StatusCode.Should().Be(HttpStatusCode.NotFound);

		var error = await result.Content.ReadFromJsonAsync<HttpErrorMessage>();
		error.Message.Should().Be($"Not Found: Concerns Case {caseId} not found");
		error.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
	}

	[Fact]
	public async Task When_PatchWithMissingDecision_Returns_404Response()
	{
		var request = new CloseDecisionRequest()
		{
			SupportingNotes = _fixture.Create<string>()
		};

		var cCase = await CreateCase();
		var cCaseId = cCase.Id;
		var decisionId = _fixture.Create<int>();

		var result = await _client.PatchAsync($"/v2/concerns-cases/{cCaseId}/decisions/{decisionId}/close", request.ConvertToJson());

		result.StatusCode.Should().Be(HttpStatusCode.NotFound);

		var error = await result.Content.ReadFromJsonAsync<HttpErrorMessage>();
		error.Message.Should().Be($"Not Found: Decision with id {decisionId} not found");
		error.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
	}

	private async Task<ConcernsCase> CreateCase()
	{
		var cCase = new ConcernsCase()
		{
			RatingId = 1,
			StatusId = 1
		};

		using (var ctxt = GetContext())
		{
			ctxt.ConcernsCase.Add(cCase);
			await ctxt.SaveChangesAsync();
		}

		return cCase;
	}

	private async Task<int> CreateDecision(int cCaseId)
	{
		var decision = BuildDecision();
		decision.ConcernsCaseId = cCaseId;
		decision.Outcome = null;
		
		using (var ctxt = GetContext())
		{
			ctxt.Decisions.Add(decision);
			await ctxt.SaveChangesAsync();
		}

		return decision.DecisionId;
	}
	
	private async Task<int> CreateDecisionWithOutcome(int cCaseId)
	{
		var decision = BuildDecision();
		decision.ConcernsCaseId = cCaseId;
		decision.Outcome = new Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome.DecisionOutcome();
		
		using (var ctxt = GetContext())
		{
			ctxt.Decisions.Add(decision);
			await ctxt.SaveChangesAsync();
		}

		return decision.DecisionId;
	}
		
	private async Task<int> CreateClosedDecision(int cCaseId)
	{
		var decision = BuildDecision();
		decision.ConcernsCaseId = cCaseId;
		decision.ClosedAt = DateTimeOffset.Now;
		
		using (var ctxt = GetContext())
		{
			ctxt.Decisions.Add(decision);
			await ctxt.SaveChangesAsync();
		}

		return decision.DecisionId;
	}
	
	private static Decision BuildDecision() => Decision.CreateNew("123456", false, false, "", new DateTimeOffset(), new DecisionType[] { }, 200, "Notes!", new DateTimeOffset());
	
	private ConcernsDbContext GetContext() => _apiTestFixture.GetContext();

	private record HttpErrorMessage(int StatusCode, string Message);

	private record ValidationErrors(string Type, string Title, int Status, string TraceId, Dictionary<string, string[]> Errors);
}