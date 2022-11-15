using AutoFixture;
using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.Services.Decisions;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Tests.Services.Decisions
{
	[Parallelizable(ParallelScope.All)]
	internal class DecisionMappingTests
	{
		private readonly static Fixture _fixture = new();

		[Test]
		public void ToActionSummary_ReturnsCorrectModel()
		{
			var apiDecision = _fixture.Create<DecisionSummaryResponse>();
			apiDecision.DecisionStatus = DecisionStatus.InProgress;
			apiDecision.CreatedAt = new DateTimeOffset(2023, 1, 4, 0, 0, 0, new TimeSpan());
			apiDecision.ClosedAt = new DateTimeOffset(2023, 2, 24, 0, 0, 0, new TimeSpan());

			var result = DecisionMapping.ToActionSummary(apiDecision);

			result.StatusName.Should().Be("In progress");
			result.OpenedDate.Should().Be("04-01-2023");
			result.ClosedDate.Should().Be("24-02-2023");
			result.Name.Should().Be($"Decision: {apiDecision.Title}");
			result.RelativeUrl.Should().Be($"/case/{apiDecision.ConcernsCaseUrn}/management/action/decision/{apiDecision.DecisionId}");
		}

		[TestCase(null, "No")]
		[TestCase(false, "No")]
		[TestCase(true, "Yes")]
		public void ToDecisionModel_ReturnsCorrectModel(
			bool? booleanFlag,
			string booleanResolvedValue)
		{
			var apiDecision = _fixture.Create<GetDecisionResponse>();
			apiDecision.ConcernsCaseUrn = 2;
			apiDecision.DecisionId = 10;
			apiDecision.RetrospectiveApproval = booleanFlag;
			apiDecision.SubmissionRequired = booleanFlag;
			apiDecision.ReceivedRequestDate = new DateTimeOffset(2023, 1, 4, 0, 0, 0, new TimeSpan());
			apiDecision.TotalAmountRequested = 150000;
			apiDecision.DecisionTypes = new DecisionType[]
			{
				DecisionType.NoticeToImprove,
				DecisionType.RepayableFinancialSupport
			};

			var result = DecisionMapping.ToDecisionModel(apiDecision);

			result.DecisionId.Should().Be(apiDecision.DecisionId);
			result.ConcernsCaseUrn.Should().Be(apiDecision.ConcernsCaseUrn);
			result.CrmEnquiryNumber.Should().Be(apiDecision.CrmCaseNumber);
			result.RetrospectiveApproval.Should().Be(booleanResolvedValue);
			result.SubmissionRequired.Should().Be(booleanResolvedValue);
			result.SubmissionLink.Should().Be(apiDecision.SubmissionDocumentLink);
			result.EsfaReceivedRequestDate.Should().Be("04-01-2023");
			result.TotalAmountRequested.Should().Be("£150,000.00");
			result.DecisionTypes.Should().BeEquivalentTo(new List<string>() { "Notice to Improve (NTI)", "Repayable financial support" });
			result.SupportingNotes.Should().Be(apiDecision.SupportingNotes);
			result.EditLink.Should().Be("/case/2/management/action/decision/addOrUpdate/10");
			result.BackLink.Should().Be("/case/2/management");
		}

		[Test]
		public void ToDecisionModel_WhenNoPropertiesFilled_ReturnsCorrectModel()
		{
			var apiDecision = new GetDecisionResponse();
			apiDecision.DecisionTypes = new DecisionType[] { };

			var result = DecisionMapping.ToDecisionModel(apiDecision);

			result.EsfaReceivedRequestDate.Should().BeNull();
		}

		[Test]
		public void ToEditDecision_ReturnsCorrectModel()
		{
			var apiDecision = _fixture.Create<GetDecisionResponse>();

			var result = DecisionMapping.ToEditDecisionModel(apiDecision);

			result.ConcernsCaseUrn.Should().Be(apiDecision.ConcernsCaseUrn);
			result.CrmCaseNumber.Should().Be(apiDecision.CrmCaseNumber);
			result.DecisionTypes.Should().BeEquivalentTo(apiDecision.DecisionTypes);
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
			apiDecision.DecisionTypes = new DecisionType[] { };

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
	}
}
