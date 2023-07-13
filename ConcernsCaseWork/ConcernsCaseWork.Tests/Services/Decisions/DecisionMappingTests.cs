using AutoFixture;
using ConcernsCaseWork.API.Contracts.Decisions;
using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Services.Decisions;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ConcernsCaseWork.Tests.Services.Decisions
{
	[Parallelizable(ParallelScope.All)]
	internal class DecisionMappingTests
	{
		private readonly static Fixture _fixture = new();
		
		[Test]
		public void ToActionSummary_WhenDoesNotHaveOutcome_ReturnsCorrectModel()
		{
			var apiDecision = _fixture.Create<DecisionSummaryResponse>();
			apiDecision.Status = DecisionStatus.InProgress;
			apiDecision.CreatedAt = new DateTimeOffset(2021, 9, 5, 0, 0, 0, new TimeSpan());
			apiDecision.ClosedAt = new DateTimeOffset(2024, 8, 30, 0, 0, 0, new TimeSpan());
			apiDecision.Outcome = null;

			var result = DecisionMapping.ToActionSummary(apiDecision);

			result.StatusName.Should().Be("In progress");
			result.OpenedDate.Should().Be("05 September 2021");
			result.ClosedDate.Should().Be("30 August 2024");
			result.Name.Should().Be($"Decision: {apiDecision.Title}");
			result.RelativeUrl.Should().Be($"/case/{apiDecision.ConcernsCaseUrn}/management/action/decision/{apiDecision.DecisionId}");
			result.RawOpenedDate.Should().Be(apiDecision.CreatedAt);
			result.RawClosedDate.Should().Be(apiDecision.ClosedAt);
		}
		
		[Test]
		[TestCase(DecisionOutcomeStatus.Declined, "Declined")]
		[TestCase(DecisionOutcomeStatus.Approved, "Approved")]
		[TestCase(DecisionOutcomeStatus.Withdrawn, "Withdrawn")]
		[TestCase(DecisionOutcomeStatus.PartiallyApproved, "Partially approved")]
		[TestCase(DecisionOutcomeStatus.ApprovedWithConditions, "Approved with conditions")]
		public void ToActionSummary_WhenHasOutcome_ReturnsCorrectModel(DecisionOutcomeStatus outcomeStatus, string expectedStatusName)
		{
			var apiDecision = _fixture.Create<DecisionSummaryResponse>();
			apiDecision.Status = DecisionStatus.InProgress;
			apiDecision.Outcome = outcomeStatus;

			var result = DecisionMapping.ToActionSummary(apiDecision);

			result.StatusName.Should().Be(expectedStatusName);
		}

		[TestCase(null, "No")]
		[TestCase(false, "No")]
		[TestCase(true, "Yes")]
		public void ToViewDecisionModel_ReturnsCorrectModel(
			bool? booleanFlag,
			string booleanResolvedValue)
		{
			var apiDecision = _fixture.Create<GetDecisionResponse>();
			apiDecision.IsEditable = true;
			apiDecision.ConcernsCaseUrn = 2;
			apiDecision.DecisionId = 10;
			apiDecision.RetrospectiveApproval = booleanFlag;
			apiDecision.SubmissionRequired = booleanFlag;
			apiDecision.ReceivedRequestDate = new DateTimeOffset(2023, 1, 4, 0, 0, 0, new TimeSpan());
			apiDecision.TotalAmountRequested = 150000;
			apiDecision.DecisionTypeQuestions = new DecisionTypeQuestion[]
			{
				new DecisionTypeQuestion()
				{
					Id = DecisionType.NoticeToImprove
				},
				new DecisionTypeQuestion()
				{
					Id = DecisionType.RepayableFinancialSupport
				}
			};
			apiDecision.CreatedAt = new DateTimeOffset(2022, 12, 24, 0, 0, 0, new TimeSpan());
			apiDecision.ClosedAt = new DateTimeOffset(2022, 12, 25, 0, 0, 0, new TimeSpan());

			apiDecision.Outcome = new DecisionOutcome()
			{
				DecisionOutcomeId = 10007,
				TotalAmount = 15000,
				Authorizer = DecisionOutcomeAuthorizer.CounterSigningDeputyDirector,
				Status = DecisionOutcomeStatus.ApprovedWithConditions,
				DecisionMadeDate = new DateTimeOffset(2023, 5, 7, 0, 0, 0, new TimeSpan()),
				DecisionEffectiveFromDate = new DateTimeOffset(2023, 12, 13, 0, 0, 0, new TimeSpan()),
				BusinessAreasConsulted = new List<DecisionOutcomeBusinessArea>()
				{
					DecisionOutcomeBusinessArea.Capital,
					DecisionOutcomeBusinessArea.SchoolsFinancialSupportAndOversight
				}
			};

			var casePermissions = new GetCasePermissionsResponse() { Permissions = new List<CasePermission> { CasePermission.Edit } };

			var result = DecisionMapping.ToViewDecisionModel(apiDecision, casePermissions);

			result.DecisionId.Should().Be(apiDecision.DecisionId);
			result.ConcernsCaseUrn.Should().Be(apiDecision.ConcernsCaseUrn);
			result.CrmEnquiryNumber.Should().Be(apiDecision.CrmCaseNumber);
			result.RetrospectiveApproval.Should().Be(booleanResolvedValue);
			result.SubmissionRequired.Should().Be(booleanResolvedValue);
			result.SubmissionLink.Should().Be(apiDecision.SubmissionDocumentLink);
			result.EsfaReceivedRequestDate.Should().Be("04 January 2023");
			result.TotalAmountRequested.Should().Be("£150,000.00");
			result.DecisionTypes.Should().BeEquivalentTo(new List<string>() { "Notice to Improve (NTI)", "Repayable financial support" });
			result.SupportingNotes.Should().Be(apiDecision.SupportingNotes);
			result.EditLink.Should().Be("/case/2/management/action/decision/addOrUpdate/10");
			result.BackLink.Should().Be("/case/2/management");
			result.CreatedDate.Should().Be("24 December 2022");
			result.ClosedDate.Should().Be("25 December 2022");

			result.Outcome.Status.Should().Be("Approved with conditions");
			result.Outcome.Authorizer.Should().Be("Countersigning Deputy Director");
			result.Outcome.TotalAmount.Should().Be("£15,000.00");
			result.Outcome.DecisionMadeDate.Should().Be("07 May 2023");
			result.Outcome.DecisionEffectiveFromDate.Should().Be("13 December 2023");

			result.Outcome.BusinessAreasConsulted.Should().BeEquivalentTo(new List<string>() { "Capital", "SFSO (Schools Financial Support and Oversight)" });
			result.Outcome.EditLink.Should().Be("/case/2/management/action/decision/10/outcome/addOrUpdate/10007");
			result.IsEditable.Should().BeTrue();
		}
	
		[Test]
		public void ToDecisionModel_WhenNoPropertiesFilled_AndNoOutcome_ReturnsCorrectModel()
		{
			var apiDecision = new GetDecisionResponse();
			apiDecision.DecisionTypeQuestions = new DecisionTypeQuestion[] { };

			var result = DecisionMapping.ToViewDecisionModel(apiDecision, new GetCasePermissionsResponse());

			result.EsfaReceivedRequestDate.Should().BeEmpty();
			result.Outcome.Should().BeNull();
		}

		[TestCaseSource(nameof(DecisionPermissionTestCases))]
		public void ToDecisionModel_WhenNotEditable_ReturnsCorrectModel(
			bool isEditable,
			GetCasePermissionsResponse casePermissionsResponse)
		{
			var apiDecision = _fixture.Create<GetDecisionResponse>();
			apiDecision.IsEditable = isEditable;

			var result = DecisionMapping.ToViewDecisionModel(apiDecision, casePermissionsResponse);

			result.IsEditable.Should().BeFalse();
		}

		[Test]
		public void ToEditDecision_ReturnsCorrectModel()
		{
			var apiDecision = _fixture.Create<GetDecisionResponse>();

			var result = DecisionMapping.ToEditDecisionModel(apiDecision);

			result.ConcernsCaseUrn.Should().Be(apiDecision.ConcernsCaseUrn);
			result.CrmCaseNumber.Should().Be(apiDecision.CrmCaseNumber);
			result.DecisionTypeQuestions.Should().BeEquivalentTo(apiDecision.DecisionTypeQuestions);
			result.ReceivedRequestDate.Should().Be(apiDecision.ReceivedRequestDate);
			result.RetrospectiveApproval.Should().Be(apiDecision.RetrospectiveApproval);
			result.SubmissionDocumentLink.Should().Be(apiDecision.SubmissionDocumentLink);
			result.SupportingNotes.Should().Be(apiDecision.SupportingNotes);
			result.SubmissionRequired.Should().Be(apiDecision.SubmissionRequired);
			result.TotalAmountRequested.Should().Be(apiDecision.TotalAmountRequested);
		}

		[Test]
		public void ToEditDecision_When_NoPropertiesFilled_Returns_CorrectModel()
		{
			var apiDecision = new GetDecisionResponse();
			apiDecision.DecisionTypeQuestions = new DecisionTypeQuestion[] { };

			var result = DecisionMapping.ToEditDecisionModel(apiDecision);

			result.ReceivedRequestDate.Should().Be(null);
		}

		[Test]
		public void ToUpdateDecision_Returns_CorrectModel()
		{
			var createDecisionModel = _fixture.Create<CreateDecisionRequest>();
			createDecisionModel.SubmissionRequired = true;

			var result = DecisionMapping.ToUpdateDecision(createDecisionModel);

			result.Should().BeEquivalentTo(createDecisionModel, options =>
				options.Excluding(o => o.ConcernsCaseUrn)
			);
		}

		[TestCase(null)]
		[TestCase(false)]
		public void ToUpdateDecision_When_SubmissionNotRequired_Returns_NoSubmissionLink(bool? submissionRequired) {
			var createDecisionModel = _fixture.Create<CreateDecisionRequest>();
			createDecisionModel.SubmissionRequired = submissionRequired;

			var result = DecisionMapping.ToUpdateDecision(createDecisionModel);

			result.SubmissionDocumentLink.Should().BeNull();
		}

		[Test]
		public void ToCreateDecisionRequest_ReturnsCorrectModel()
		{
			var model = _fixture.Create<EditDecisionOutcomeModel>();
			model.DecisionMadeDate = new OptionalDateModel()
			{
				Day = "2",
				Month = "5",
				Year = "2022"
			};

			model.DecisionEffectiveFromDate = new OptionalDateModel()
			{
				Day = "6",
				Month = "12",
				Year = "2023"
			};

			var result = DecisionMapping.ToCreateDecisionOutcomeRequest(model);

			result.Status.Should().Be(model.Status);
			result.TotalAmount.Should().Be(model.TotalAmount);
			result.Authorizer.Should().Be(model.Authorizer);
			result.BusinessAreasConsulted.Should().BeEquivalentTo(model.BusinessAreasConsulted);

			result.DecisionMadeDate.Value.Day.Should().Be(2);
			result.DecisionMadeDate.Value.Month.Should().Be(5);
			result.DecisionMadeDate.Value.Year.Should().Be(2022);

			result.DecisionEffectiveFromDate.Value.Day.Should().Be(6);
			result.DecisionEffectiveFromDate.Value.Month.Should().Be(12);
			result.DecisionEffectiveFromDate.Value.Year.Should().Be(2023);
		}

		[Test]
		public void ToCreateDecisionRequest_MinimumFieldsFilled_ReturnsCorrectModel()
		{
			var model = new EditDecisionOutcomeModel()
			{
				Status = DecisionOutcomeStatus.Approved,
				DecisionEffectiveFromDate = new OptionalDateModel(),
				DecisionMadeDate = new OptionalDateModel()
			};

			var result = DecisionMapping.ToCreateDecisionOutcomeRequest(model);

			result.Status.Should().Be(model.Status);
			result.DecisionMadeDate.Should().BeNull();
			result.DecisionEffectiveFromDate.Should().BeNull();
		}

		[Test]
		public void ToEditDecisionOutcomeModel_ReturnsModel()
		{
			var model = new DecisionOutcome()
			{
				Status = DecisionOutcomeStatus.PartiallyApproved,
				TotalAmount = 100,
				DecisionMadeDate = new DateTimeOffset(2022, 11, 29, 0, 0, 0, new TimeSpan(0, 0, 0)),
				DecisionEffectiveFromDate = new DateTimeOffset(2022, 11, 29, 0, 0, 0, new TimeSpan(0, 0, 0)),
				Authorizer = DecisionOutcomeAuthorizer.G7,
				BusinessAreasConsulted = new List<DecisionOutcomeBusinessArea> { DecisionOutcomeBusinessArea.SchoolsFinancialSupportAndOversight, DecisionOutcomeBusinessArea.BusinessPartner }
			};

			var getDecisionResponse = _fixture.Create<GetDecisionResponse>();
			getDecisionResponse.Outcome = model;

			var result = DecisionMapping.ToEditDecisionOutcomeModel(getDecisionResponse);

			result.Status.Should().Be(model.Status);
			result.TotalAmount.Should().Be(model.TotalAmount);
			result.DecisionMadeDate.Day.Should().Be(model.DecisionMadeDate.Value.Day.ToString());
			result.DecisionMadeDate.Month.Should().Be(model.DecisionMadeDate.Value.Month.ToString());
			result.DecisionMadeDate.Year.Should().Be(model.DecisionMadeDate.Value.Year.ToString());

			result.DecisionEffectiveFromDate.Day.Should().Be(model.DecisionEffectiveFromDate.Value.Day.ToString());
			result.DecisionEffectiveFromDate.Month.Should().Be(model.DecisionEffectiveFromDate.Value.Month.ToString());
			result.DecisionEffectiveFromDate.Year.Should().Be(model.DecisionEffectiveFromDate.Value.Year.ToString());
			result.Authorizer.Should().Be(model.Authorizer);
			result.BusinessAreasConsulted.Should().BeEquivalentTo(model.BusinessAreasConsulted);
		}


		[Test]
		public void ToUpdateDecisionOutcome_ReturnsCorrectModel()
		{
			var model = _fixture.Create<EditDecisionOutcomeModel>();

			model.DecisionMadeDate = new OptionalDateModel()
			{
				Day = "2",
				Month = "5",
				Year = "2022"
			};

			model.DecisionEffectiveFromDate = new OptionalDateModel()
			{
				Day = "6",
				Month = "12",
				Year = "2023"
			};

			var result = DecisionMapping.ToUpdateDecisionOutcome(model);

			result.Status.Should().Be(model.Status);
			result.TotalAmount.Should().Be(model.TotalAmount);
			result.DecisionMadeDate.Value.Day.ToString().Should().Be(model.DecisionMadeDate.Day);
			result.DecisionMadeDate.Value.Month.ToString().Should().Be(model.DecisionMadeDate.Month);
			result.DecisionMadeDate.Value.Year.ToString().Should().Be(model.DecisionMadeDate.Year);

			result.DecisionEffectiveFromDate.Value.Day.ToString().Should().Be(model.DecisionEffectiveFromDate.Day);
			result.DecisionEffectiveFromDate.Value.Month.ToString().Should().Be(model.DecisionEffectiveFromDate.Month);
			result.DecisionEffectiveFromDate.Value.Year.ToString().Should().Be(model.DecisionEffectiveFromDate.Year);
			result.Authorizer.Should().Be(model.Authorizer);
			result.BusinessAreasConsulted.Should().BeEquivalentTo(model.BusinessAreasConsulted);
		}

		[Test]
		public void ToDecisionSummary_ReturnsCorrectModel()
		{
			var apiDecision = _fixture.Create<DecisionSummaryResponse>();

			var result = DecisionMapping.ToDecisionSummaryModel(apiDecision);

			result.ClosedAt.Should().Be(apiDecision.ClosedAt?.Date);
		}

		private static IEnumerable<TestCaseData> DecisionPermissionTestCases()
		{
			yield return new TestCaseData(false, new GetCasePermissionsResponse() { Permissions = new List<CasePermission>() { CasePermission.Edit } });
			yield return new TestCaseData(true, new GetCasePermissionsResponse());
		}
	}
}
