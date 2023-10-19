using AutoFixture;
using ConcernsCaseWork.API.Features.Case;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.Data.Models.Decisions;
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
	public void ReturnsActiveCaseSummaryResponse_WhenGivenAnActiveCaseSummaryVmWithNoConcernsOrActionsOrDecisions()
	{
		// arrange
		var ratingName = _fixture.Create<string>();
		var caseSummaryVm = Builder<ActiveCaseSummaryVm>.CreateNew().Build();
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
		result.TrustFinancialForecasts.Should().BeEmpty();
	}

	[Fact]
	public void ReturnsActiveCaseSummaryResponse_WhenGivenAnActiveCaseSummaryVmWithConcernsAndActionsAndDecisions()
	{
		// arrange
		var ratingName = _fixture.Create<string>();
		var caseSummaryVm = Builder<ActiveCaseSummaryVm>.CreateNew().Build();
		caseSummaryVm.Rating = new ConcernsRating { Name = ratingName };
		caseSummaryVm.ActiveConcerns = BuildListConcerns();
		caseSummaryVm.Decisions = BuildListDecisions();
		caseSummaryVm.FinancialPlanCases = BuildListActions();
		caseSummaryVm.NoticesToImprove = BuildListActions();
		caseSummaryVm.NtiWarningLetters = BuildListActions();
		caseSummaryVm.NtisUnderConsideration = BuildListActions();
		caseSummaryVm.SrmaCases = BuildListActions();
		caseSummaryVm.TrustFinancialForecasts = BuildListActions();

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
		result.TrustFinancialForecasts.Should().NotBeEmpty();
		
		result.ActiveConcerns.Count().Should().Be(caseSummaryVm.ActiveConcerns.Count());
		result.Decisions.Count().Should().Be(caseSummaryVm.Decisions.Count());
		result.FinancialPlanCases.Count().Should().Be(caseSummaryVm.FinancialPlanCases.Count());
		result.NtisUnderConsideration.Count().Should().Be(caseSummaryVm.NtisUnderConsideration.Count());
		result.NoticesToImprove.Count().Should().Be(caseSummaryVm.NoticesToImprove.Count());
		result.NtiWarningLetters.Count().Should().Be(caseSummaryVm.NtiWarningLetters.Count());
		result.SrmaCases.Count().Should().Be(caseSummaryVm.SrmaCases.Count());
		result.TrustFinancialForecasts.Count().Should().Be(caseSummaryVm.TrustFinancialForecasts.Count());
	}
	
	[Fact]
	public void ReturnsClosedCaseSummaryResponse_WhenGivenAClosedCaseSummaryVmWithNoConcernsOrActionsOrDecisions()
	{
		// arrange
		var caseSummaryVm = Builder<ClosedCaseSummaryVm>.CreateNew().Build();

		// act
		var result = CaseSummaryResponseFactory.Create(caseSummaryVm);

		// assert
		result.CaseUrn.Should().Be(caseSummaryVm.CaseUrn);
		result.CreatedAt.Should().Be(caseSummaryVm.CreatedAt);
		result.UpdatedAt.Should().Be(caseSummaryVm.UpdatedAt);
		result.CreatedBy.Should().Be(caseSummaryVm.CreatedBy);
		result.StatusName.Should().Be(caseSummaryVm.StatusName);
		result.TrustUkPrn.Should().Be(caseSummaryVm.TrustUkPrn);
		result.ClosedAt.Should().Be(caseSummaryVm.ClosedAt);
		result.ClosedConcerns.Should().BeEmpty();
		result.Decisions.Should().BeEmpty();
		result.FinancialPlanCases.Should().BeEmpty();
		result.NtisUnderConsideration.Should().BeEmpty();
		result.NoticesToImprove.Should().BeEmpty();
		result.NtiWarningLetters.Should().BeEmpty();
		result.SrmaCases.Should().BeEmpty();
		result.TrustFinancialForecasts.Should().BeEmpty();
	}

	[Fact]
	public void ReturnsClosedCaseSummaryResponse_WhenGivenAClosedCaseSummaryVmWithConcernsAndActionsAndDecisions()
	{
		// arrange
		var caseSummaryVm = Builder<ClosedCaseSummaryVm>.CreateNew().Build();
		caseSummaryVm.ClosedConcerns = BuildListConcerns();
		caseSummaryVm.Decisions = BuildListDecisions();
		caseSummaryVm.FinancialPlanCases = BuildListActions();
		caseSummaryVm.NoticesToImprove = BuildListActions();
		caseSummaryVm.NtiWarningLetters = BuildListActions();
		caseSummaryVm.NtisUnderConsideration = BuildListActions();
		caseSummaryVm.SrmaCases = BuildListActions();
		caseSummaryVm.TrustFinancialForecasts = BuildListActions();

		// act
		var result = CaseSummaryResponseFactory.Create(caseSummaryVm);

		// assert
		result.CaseUrn.Should().Be(caseSummaryVm.CaseUrn);
		result.CreatedAt.Should().Be(caseSummaryVm.CreatedAt);
		result.UpdatedAt.Should().Be(caseSummaryVm.UpdatedAt);
		result.CreatedBy.Should().Be(caseSummaryVm.CreatedBy);
		result.StatusName.Should().Be(caseSummaryVm.StatusName);
		result.TrustUkPrn.Should().Be(caseSummaryVm.TrustUkPrn);
		result.ClosedAt.Should().Be(caseSummaryVm.ClosedAt);
		result.Decisions.Should().NotBeEmpty();
		result.FinancialPlanCases.Should().NotBeEmpty();
		result.NtisUnderConsideration.Should().NotBeEmpty();
		result.NoticesToImprove.Should().NotBeEmpty();
		result.NtiWarningLetters.Should().NotBeEmpty();
		result.SrmaCases.Should().NotBeEmpty();
		result.TrustFinancialForecasts.Should().NotBeEmpty();
		
		result.ClosedConcerns.Count().Should().Be(caseSummaryVm.ClosedConcerns.Count());
		result.Decisions.Count().Should().Be(caseSummaryVm.Decisions.Count());
		result.FinancialPlanCases.Count().Should().Be(caseSummaryVm.FinancialPlanCases.Count());
		result.NtisUnderConsideration.Count().Should().Be(caseSummaryVm.NtisUnderConsideration.Count());
		result.NoticesToImprove.Count().Should().Be(caseSummaryVm.NoticesToImprove.Count());
		result.NtiWarningLetters.Count().Should().Be(caseSummaryVm.NtiWarningLetters.Count());
		result.SrmaCases.Count().Should().Be(caseSummaryVm.SrmaCases.Count());
		result.TrustFinancialForecasts.Count().Should().Be(caseSummaryVm.TrustFinancialForecasts.Count());
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
		Decision.CreateNew(new DecisionParameters()
		{
			CrmCaseNumber = new string(_fixture.CreateMany<char>(Decision.MaxCaseNumberLength).ToArray()),
			RetrospectiveApproval = _fixture.Create<bool>(),
			SubmissionRequired = _fixture.Create<bool>(),
			SubmissionDocumentLink = new string(_fixture.CreateMany<char>(Decision.MaxUrlLength).ToArray()),
			ReceivedRequestDate = DateTimeOffset.Now,
			DecisionTypes = new DecisionType[] { new (Contracts.Decisions.DecisionType.NoticeToImprove, Contracts.Decisions.DrawdownFacilityAgreed.No, Contracts.Decisions.FrameworkCategory.FacilitatingTransferFinanciallyAgreed) },
			TotalAmountRequested = _fixture.Create<decimal>(),
			SupportingNotes = new string(_fixture.CreateMany<char>(Decision.MaxSupportingNotesLength).ToArray()),
			Now = DateTimeOffset.Now
		});

	private List<CaseSummaryVm.Action> BuildListActions() => _fixture.CreateMany<CaseSummaryVm.Action>().ToList();
}