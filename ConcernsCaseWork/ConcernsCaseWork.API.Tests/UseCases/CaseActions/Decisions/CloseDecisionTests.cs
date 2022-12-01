using AutoFixture;
using AutoFixture.Idioms;
using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.API.Factories.Concerns.Decisions;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.API.UseCases.CaseActions.Decisions;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.UseCases.CaseActions.Decisions;

public class CloseDecisionTests
{
	private readonly IFixture _fixture;

	public CloseDecisionTests()
	{
		_fixture = new Fixture();
		_fixture.Register(Mock.Of<IConcernsCaseGateway>);
		_fixture.Register(Mock.Of<ICloseDecisionResponseFactory>);
	}
	
	[Fact]
	public void CloseDecisions_Is_Assignable_To_IUseCaseAsync()
	{
		var sut = _fixture.Create<CloseDecision>();

		sut.Should()
			.BeAssignableTo<IUseCaseAsync<DecisionUseCaseRequestWrapper<CloseDecisionRequest>, CloseDecisionResponse>>();
	}

	[Fact]
	public async Task Execute_When_ConcernsCase_Not_Found_Throws_Exception()
	{
		// arrange 
		var mockCloseDecisionResponseFactory = new Mock<ICloseDecisionResponseFactory>();
		var mockGateWay = new Mock<IConcernsCaseGateway>();

		var cCase = default(ConcernsCase);
		var caseUrn = _fixture.Create<int>();
		var decisionId = _fixture.Create<int>();
		
		var request = CreateRequest(caseUrn, decisionId);

		mockGateWay.Setup(x => x.GetConcernsCaseByUrn(caseUrn, It.IsAny<bool>())).Returns(cCase);

		var sut = new CloseDecision(mockGateWay.Object, mockCloseDecisionResponseFactory.Object);
		
		// act
		var action = () => sut.Execute(request, CancellationToken.None);

		// assert
		(await action.Should().ThrowAsync<InvalidOperationException>()).And.Message.Should()
			.Be($"Concerns Case {caseUrn} not found");
	}
	
	[Fact]
	public async Task Execute_When_Decision_Not_Found_Throws_Exception()
	{
		// arrange
		var mockCloseDecisionResponseFactory = new Mock<ICloseDecisionResponseFactory>();
		var mockGateWay = new Mock<IConcernsCaseGateway>();

		var cCase = CreateOpenCase();
		var caseUrn = cCase.Urn;
		var decisionId = _fixture.Create<int>();
		
		var request = CreateRequest(caseUrn, decisionId);

		mockGateWay
			.Setup(x => x.GetConcernsCaseByUrn(caseUrn, true))
			.Returns(cCase);

		var sut = new CloseDecision(mockGateWay.Object, mockCloseDecisionResponseFactory.Object);
		
		// act
		var action = () => sut.Execute(request, CancellationToken.None);

		// assert
		(await action.Should().ThrowAsync<ArgumentOutOfRangeException>()).And.Message.Should()
			.Be($"Decision id {decisionId} not found in this case. Case urn {caseUrn} (Parameter 'decisionId')");
	}
	
	[Theory]
	[InlineData(null)]
	[InlineData(0)]
	public async Task Execute_When_DecisionId_Empty_Throws_Exception(int? decisionId)
	{
		// arrange
		var mockCloseDecisionResponseFactory = new Mock<ICloseDecisionResponseFactory>();
		var mockGateWay = new Mock<IConcernsCaseGateway>();

		var caseUrn = _fixture.Create<int>();
		
		var request = CreateRequest(caseUrn, decisionId);

		var sut = new CloseDecision(mockGateWay.Object, mockCloseDecisionResponseFactory.Object);
		
		// act
		var action = () => sut.Execute(request, CancellationToken.None);

		// assert
		(await action.Should().ThrowAsync<ArgumentOutOfRangeException>()).And.Message.Should()
			.Be($"Specified argument was out of the range of valid values. (Parameter 'DecisionId')");
	}
			
	[Theory]
	[InlineData(null)]
	[InlineData(0)]
	public async Task Execute_When_CaseUrn_Empty_Throws_Exception(int? caseUrn)
	{
		// arrange
		var mockCloseDecisionResponseFactory = new Mock<ICloseDecisionResponseFactory>();
		var mockGateWay = new Mock<IConcernsCaseGateway>();

		var decisionId = _fixture.Create<int>();
		
		var request = CreateRequest(caseUrn, decisionId);

		var sut = new CloseDecision(mockGateWay.Object, mockCloseDecisionResponseFactory.Object);
		
		// act
		var action = () => sut.Execute(request, CancellationToken.None);

		// assert
		(await action.Should().ThrowAsync<ArgumentOutOfRangeException>()).And.Message.Should()
			.Be($"Specified argument was out of the range of valid values. (Parameter 'CaseUrn')");
	}

	[Fact]
	public async Task Execute_When_ConcernsCase_And_Decision_Found_Closes_Decision()
	{
		// arrange
		var mockCloseDecisionResponseFactory = new Mock<ICloseDecisionResponseFactory>();
		var mockGateWay = new Mock<IConcernsCaseGateway>();

		var cCase = CreateOpenCase();
		var decision = CreateOpenDecision();
		cCase.AddDecision(decision, _fixture.Create<DateTime>());
		
		var caseUrn = cCase.Urn;
		var decisionId = decision.DecisionId;
		
		var request = CreateRequest(caseUrn, decisionId);
		
		mockGateWay.Setup(x => x.GetConcernsCaseByUrn(caseUrn, true)).Returns(cCase);
		mockCloseDecisionResponseFactory.Setup(x => x.Create(caseUrn, decisionId))
			.Returns(new CloseDecisionResponse(caseUrn, (int)decisionId!));

		var sut = new CloseDecision(mockGateWay.Object, mockCloseDecisionResponseFactory.Object);
		
		// act
		var result = await sut.Execute(request, CancellationToken.None);

		// assert
		result.CaseUrn.Should().Be(caseUrn);
		result.DecisionId.Should().Be(decisionId);
	}
	
	[Fact]
	public void Constructor_Guards_Against_Null_Arguments()
	{
		var assertion = _fixture.Create<GuardClauseAssertion>();
		
		assertion.Verify(typeof(CloseDecision).GetConstructors());
	}

	[Fact]
	public void Methods_Guards_Against_Null_Arguments()
	{
		var assertion = _fixture.Create<GuardClauseAssertion>();

		assertion.Verify(typeof(CloseDecision).GetMethods());
	}

	private Decision CreateOpenDecision()
	{
		var decision = Decision.CreateNew(
			crmCaseNumber: new string(_fixture.CreateMany<char>(Decision.MaxCaseNumberLength).ToArray()),
			retrospectiveApproval: _fixture.Create<bool>(),
			submissionRequired: _fixture.Create<bool>(),
			submissionDocumentLink: new string(_fixture.CreateMany<char>(Decision.MaxUrlLength).ToArray()),
			receivedRequestDate: DateTimeOffset.Now,
			decisionTypes: new DecisionType[] { new DecisionType(Data.Enums.Concerns.DecisionType.NoticeToImprove) },
			totalAmountRequested: _fixture.Create<decimal>(),
			supportingNotes: new string(_fixture.CreateMany<char>(Decision.MaxSupportingNotesLength).ToArray()),
			DateTimeOffset.Now
		);

		decision.DecisionId = _fixture.Create<int>();
		return decision;
	}

	private ConcernsCase CreateOpenCase() 
		=> _fixture.Build<ConcernsCase>()
			.With(c => c.ClosedAt, (DateTime?)null)
			.With(c => c.ConcernsRecords, new List<ConcernsRecord>())
			.With(c => c.Rating, new ConcernsRating())
			.Create();

	private DecisionUseCaseRequestWrapper<CloseDecisionRequest> CreateRequest(int? caseUrn, int? decisionId) 
		=> _fixture.Build<DecisionUseCaseRequestWrapper<CloseDecisionRequest>>()
			.With(c => c.CaseUrn, caseUrn)
			.With(c => c.DecisionId, decisionId)
			.Create();
}