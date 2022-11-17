using AutoFixture;
using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
using FizzWare.NBuilder;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Factories;

public class CaseSummaryResponseFactoryTests
{
	private readonly IFixture _fixture = new Fixture();

	[Fact]
	public void ReturnsCaseSummaryResponse_WhenGivenACaseSummaryVmWithNoConcernsOrActionsOrDecisions()
	{
		// arrange
		var ratingName = _fixture.Create<string>();
		var caseSummaryVm = Builder<CaseSummaryVm>.CreateNew().Build();
		caseSummaryVm.Rating = new ConcernsRating { Name = ratingName };

		// act
		var result = CaseSummaryResponseFactory.Create(caseSummaryVm);

		// assert
		result.CaseUrn.Should().Be(caseSummaryVm.CaseUrn);
		result.CreatedAt.Should().Be(caseSummaryVm.CreatedAt);
		result.UpdatedAt.Should().Be(caseSummaryVm.UpdatedAt);
		result.CreatedBy.Should().Be(caseSummaryVm.CreatedBy);
		result.Rating.Name.Should().Be(caseSummaryVm.Rating.Name);
		result.StatusName.Should().Be(caseSummaryVm.StatusName);
		result.TrustUkPrn.Should().Be(caseSummaryVm.TrustUkPrn);
		result.ActiveConcerns.Should().BeEmpty();
		result.Decisions.Should().BeEmpty();
		result.FinancialPlanCases.Should().BeEmpty();
		result.NtisUnderConsideration.Should().BeEmpty();
		result.NoticesToImprove.Should().BeEmpty();
		result.NtiWarningLetters.Should().BeEmpty();
		result.SrmaCases.Should().BeEmpty();
	}

	[Fact]
	public void ReturnsCaseSummaryResponse_WhenGivenACaseSummaryVmWithConcernsAndActionsAndDecisions()
	{
		// arrange
		var ratingName = _fixture.Create<string>();
		var caseSummaryVm = Builder<CaseSummaryVm>.CreateNew().Build();
		caseSummaryVm.Rating = new ConcernsRating { Name = ratingName };
		caseSummaryVm.ActiveConcerns = BuildListConcerns();
		caseSummaryVm.Decisions = BuildListDecisions();
		caseSummaryVm.FinancialPlanCases = BuildListActions();
		caseSummaryVm.NoticesToImprove = BuildListActions();
		caseSummaryVm.NtiWarningLetters = BuildListActions();
		caseSummaryVm.NtisUnderConsideration = BuildListActions();
		caseSummaryVm.SrmaCases = BuildListActions();

		// act
		var result = CaseSummaryResponseFactory.Create(caseSummaryVm);

		// assert
		result.CaseUrn.Should().Be(caseSummaryVm.CaseUrn);
		result.CreatedAt.Should().Be(caseSummaryVm.CreatedAt);
		result.UpdatedAt.Should().Be(caseSummaryVm.UpdatedAt);
		result.CreatedBy.Should().Be(caseSummaryVm.CreatedBy);
		result.Rating.Name.Should().Be(caseSummaryVm.Rating.Name);
		result.StatusName.Should().Be(caseSummaryVm.StatusName);
		result.TrustUkPrn.Should().Be(caseSummaryVm.TrustUkPrn);
		result.ActiveConcerns.Should().NotBeEmpty();
		result.Decisions.Should().NotBeEmpty();
		result.FinancialPlanCases.Should().NotBeEmpty();
		result.NtisUnderConsideration.Should().NotBeEmpty();
		result.NoticesToImprove.Should().NotBeEmpty();
		result.NtiWarningLetters.Should().NotBeEmpty();
		result.SrmaCases.Should().NotBeEmpty();
		
		result.ActiveConcerns.Count().Should().Be(caseSummaryVm.ActiveConcerns.Count());
		result.Decisions.Count().Should().Be(caseSummaryVm.Decisions.Count());
		result.FinancialPlanCases.Count().Should().Be(caseSummaryVm.FinancialPlanCases.Count());
		result.NtisUnderConsideration.Count().Should().Be(caseSummaryVm.NtisUnderConsideration.Count());
		result.NoticesToImprove.Count().Should().Be(caseSummaryVm.NoticesToImprove.Count());
		result.NtiWarningLetters.Count().Should().Be(caseSummaryVm.NtiWarningLetters.Count());
		result.SrmaCases.Count().Should().Be(caseSummaryVm.SrmaCases.Count());
	}

	private List<CaseSummaryVm.Concern> BuildListConcerns()
		=> new List<CaseSummaryVm.Concern>
		{
			new(_fixture.Create<string>(), new ConcernsRating { Name = _fixture.Create<string>() }, _fixture.Create<DateTime>()),
			new(_fixture.Create<string>(), new ConcernsRating { Name = _fixture.Create<string>() }, _fixture.Create<DateTime>()),
			new(_fixture.Create<string>(), new ConcernsRating { Name = _fixture.Create<string>() }, _fixture.Create<DateTime>()),
			new(_fixture.Create<string>(), new ConcernsRating { Name = _fixture.Create<string>() }, _fixture.Create<DateTime>()),
			new(_fixture.Create<string>(), new ConcernsRating { Name = _fixture.Create<string>() }, _fixture.Create<DateTime>()),
			new(_fixture.Create<string>(), new ConcernsRating { Name = _fixture.Create<string>() }, _fixture.Create<DateTime>())
		};

	private List<Decision> BuildListDecisions()
		=> new List<Decision>
		{
			BuildDecision(),
			BuildDecision(),
			BuildDecision()
		};

	private Decision BuildDecision()
		=>
		Decision.CreateNew(
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

private List<CaseSummaryVm.Action> BuildListActions() => _fixture.CreateMany<CaseSummaryVm.Action>().ToList();
}