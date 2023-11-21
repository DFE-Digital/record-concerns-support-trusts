using AutoFixture;
using ConcernsCaseWork.Data.Models.Decisions;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace ConcernsCaseWork.API.Tests.DatabaseModels.Concerns
{
	public class DecisionTests
    {
        [Fact]
        public void Can_Create_New_Decision()
        {
            var fixture = new Fixture();
            var sut = CreateRandomDecision(fixture);
            sut.Should().NotBeNull();
        }

        private Decision CreateRandomDecision(Fixture fixture)
        {
            return Decision.CreateNew(new DecisionParameters()
			{
				CrmCaseNumber = fixture.Create<string>().Substring(0, 20),
				RetrospectiveApproval = fixture.Create<bool?>(),
				SubmissionRequired = fixture.Create<bool?>(),
				SubmissionDocumentLink = fixture.Create<string>(),
				ReceivedRequestDate = fixture.Create<DateTimeOffset>(),
				DecisionTypes = fixture.CreateMany<DecisionType>().ToArray(),
				TotalAmountRequested = fixture.Create<decimal>(),
				SupportingNotes = fixture.Create<string>(),
				Now = DateTimeOffset.Now
			});
        }

        [Theory]
        [InlineData("caseNumber", "notes", "link")]
        [InlineData(null, null, null)]
        public void CreateNew_Sets_Properties(string caseNumber, string notes, string link)
        {
            var fixture = new Fixture();
            var decisionId = fixture.Create<int>();

            var decisionTypes = new[]
            {
                new DecisionType(Contracts.Decisions.DecisionType.NoticeToImprove, Contracts.Decisions.DrawdownFacilityAgreed.No, Contracts.Decisions.FrameworkCategory.FacilitatingTransferEducationallyTriggered) { DecisionId = decisionId },
                new DecisionType(Contracts.Decisions.DecisionType.OtherFinancialSupport, Contracts.Decisions.DrawdownFacilityAgreed.PaymentUnderExistingArrangement, Contracts.Decisions.FrameworkCategory.BuildingFinancialCapability){ DecisionId = decisionId }
            };

            var expectation = new
            {
                CrmCaseNumber = caseNumber,
				HasCrmCase = true,
                RetrospectiveApproval = true,
                SubmissionRequired = true,
                SubmissionDocumentLink = link,
                ReceivedRequestDate = fixture.Create<DateTimeOffset>(),
                DecisionTypes = decisionTypes,
                TotalAmountRequested = 13.5m,
                SupportingNotes = notes,
                CurrentDateTime = DateTimeOffset.UtcNow
            };

            var sut = Decision.CreateNew(new DecisionParameters()
			{
				CrmCaseNumber = expectation.CrmCaseNumber,
				HasCrmCase = expectation.HasCrmCase,
				RetrospectiveApproval = expectation.RetrospectiveApproval,
				SubmissionRequired = expectation.SubmissionRequired,
				SubmissionDocumentLink = expectation.SubmissionDocumentLink,
				ReceivedRequestDate = expectation.ReceivedRequestDate,
				DecisionTypes = expectation.DecisionTypes,
				TotalAmountRequested = expectation.TotalAmountRequested,
				SupportingNotes = expectation.SupportingNotes,
				Now = expectation.CurrentDateTime
			});

            sut.Should().BeEquivalentTo(expectation, cfg => cfg.Excluding(e => e.CurrentDateTime));
            sut.CreatedAt.Should().Be(expectation.CurrentDateTime);
            sut.UpdatedAt.Should().Be(expectation.CurrentDateTime);
            sut.DecisionId.Should().Be(0, "DecisionId should be assigned by the database");
        }

        [Fact]
        public void CreateNew_Sets_Status_To_InProgress()
        {
            var fixture = new Fixture();
            var sut = CreateRandomDecision(fixture);
            sut.Status.Should().Be(Contracts.Decisions.DecisionStatus.InProgress);
        }

        [Fact]
        public void GetTitle_Maps_Multiple_Decision_Types_To_Text()
        {
            var fixture = new Fixture();
            var sut = Decision.CreateNew(new DecisionParameters()
				{
					CrmCaseNumber = "12345",
					RetrospectiveApproval = true,
					SubmissionRequired = true,
					SubmissionDocumentLink = "https://somewhere/somelink.doc",
					ReceivedRequestDate = DateTimeOffset.UtcNow,
					DecisionTypes = fixture.CreateMany<DecisionType>(5).ToArray(),
					TotalAmountRequested = 13.5m,
					SupportingNotes = "some notes",
					Now = DateTimeOffset.UtcNow
				}

            );

            sut.GetTitle().Should().Be("Multiple Decision Types");
        }

        [Fact]
        public void GetTitle_Maps_Zero_Decision_Types_To_Text()
        {
            var sut = Decision.CreateNew(new DecisionParameters()
				{
					CrmCaseNumber = "12345",
					RetrospectiveApproval = true,
					SubmissionRequired = true,
					SubmissionDocumentLink = "https://somewhere/somelink.doc",
					ReceivedRequestDate = DateTimeOffset.UtcNow,
					DecisionTypes = null,
					TotalAmountRequested = 13.5m,
					SupportingNotes = "some notes",
					Now = DateTimeOffset.UtcNow
				}
            );

            sut.GetTitle().Should().Be("No Decision Types");
        }

        [Fact]
        public void GetTitle_When_One_DecisionType_Maps_To_DecisionType_Description()
        {
			var decisionType = Contracts.Decisions.DecisionType.RepayableFinancialSupport;

            var decisionTypes = new[]
            {
                new DecisionType(decisionType, Contracts.Decisions.DrawdownFacilityAgreed.PaymentUnderExistingArrangement, Contracts.Decisions.FrameworkCategory.BuildingFinancialCapability) { DecisionId = (int)decisionType },
            };

            var sut = Decision.CreateNew(new DecisionParameters()
				{
					CrmCaseNumber = "12345",
					RetrospectiveApproval = true,
					SubmissionRequired = true,
					SubmissionDocumentLink = "https://somewhere/somelink.doc",
					ReceivedRequestDate = DateTimeOffset.UtcNow,
					DecisionTypes = decisionTypes,
					TotalAmountRequested = 13.5m,
					SupportingNotes = "some notes",
					Now = DateTimeOffset.UtcNow
				}
            );

            sut.GetTitle().Should().Be("Repayable financial support");
        }
    }
}