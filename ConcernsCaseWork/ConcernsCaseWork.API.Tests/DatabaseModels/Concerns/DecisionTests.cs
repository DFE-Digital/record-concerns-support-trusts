using AutoFixture;
using ConcernsCaseWork.Data.Enums;
using ConcernsCaseWork.Data.Models.Decisions;
using FluentAssertions;
using System;
using System.Collections.Generic;
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
        [InlineData(-1000, "caseNumber", "notes", "link", "totalAmountRequested")]
        [InlineData(1000, "_maxString_", "notes", "link", "crmCaseNumber")]
        [InlineData(1000, "caseNumber", "_maxString_", "link", "supportingNotes")]
        [InlineData(1000, "caseNumber", "notes", "_maxString_", "submissionDocumentLink")]
        public void CreateNew_With_Invalid_Arguments_Throws_Exception(decimal amountRequested, string crmCaseNumber, string supportingNotes, string submissionDocumentLink, string expectedParamName)
        {
            var fixture = new Fixture();
            crmCaseNumber = crmCaseNumber == "_maxString_" ? CreateFixedLengthString(fixture, Decision.MaxCaseNumberLength + 1) : crmCaseNumber;
            supportingNotes = supportingNotes == "_maxString_" ? CreateFixedLengthString(fixture, Decision.MaxSupportingNotesLength + 1) : supportingNotes;
            submissionDocumentLink = submissionDocumentLink == "_maxString_" ? CreateFixedLengthString(fixture, Decision.MaxUrlLength + 1) : submissionDocumentLink;

			Action act = () => Decision.CreateNew(new DecisionParameters()
				{
					CrmCaseNumber = crmCaseNumber,
					RetrospectiveApproval = null,
					SubmissionRequired = null,
					SubmissionDocumentLink = submissionDocumentLink,
					ReceivedRequestDate = DateTimeOffset.UtcNow,
					DecisionTypes = Array.Empty<DecisionType>(),
					TotalAmountRequested = amountRequested,
					SupportingNotes = supportingNotes,
					Now = DateTimeOffset.Now
				}
			);

            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be(expectedParamName);
        }

        private string CreateFixedLengthString(Fixture fixture, int size)
        {
            return new string(fixture.CreateMany<char>(size).ToArray());
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
                new DecisionType(Data.Enums.Concerns.DecisionType.NoticeToImprove, Contracts.Decisions.DrawdownFacilityAgreed.No, Contracts.Decisions.FrameworkCategory.FacilitatingTransferEducationallyTriggered) { DecisionId = decisionId },
                new DecisionType(Data.Enums.Concerns.DecisionType.OtherFinancialSupport, Contracts.Decisions.DrawdownFacilityAgreed.PaymentUnderExistingArrangement, Contracts.Decisions.FrameworkCategory.BuildingFinancialCapability){ DecisionId = decisionId }
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
            sut.Status.Should().Be(Data.Enums.Concerns.DecisionStatus.InProgress);
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

        [Theory]
        [MemberData(nameof(TestData))]
        public void GetTitle_When_One_DecisionType_Maps_To_DecisionType_Description(Data.Enums.Concerns.DecisionType decisionType)
        {
            var decisionTypes = new[]
            {
                new DecisionType(decisionType, API.Contracts.Decisions.DrawdownFacilityAgreed.PaymentUnderExistingArrangement, API.Contracts.Decisions.FrameworkCategory.BuildingFinancialCapability) { DecisionId = (int)decisionType },
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

            sut.GetTitle().Should().Be(decisionType.GetDescription());
        }

        public static IEnumerable<object[]> TestData
        {
            get
            {
                foreach (var enumValue in Enum.GetValues(typeof(Data.Enums.Concerns.DecisionType)))
                {
                    yield return new object[] { (Data.Enums.Concerns.DecisionType)enumValue };
                }
            }
        }
    }
}