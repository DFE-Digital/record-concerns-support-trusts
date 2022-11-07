using AutoFixture;
using ConcernsCaseWork.Data.Enums;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
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
            return Decision.CreateNew(
                crmCaseNumber: fixture.Create<string>().Substring(0, 20),
                retrospectiveApproval: fixture.Create<bool?>(),
                submissionRequired: fixture.Create<bool?>(),
                submissionDocumentLink: fixture.Create<string>(),
                receivedRequestDate: fixture.Create<DateTimeOffset>(),
                decisionTypes: fixture.CreateMany<DecisionType>().ToArray(),
                totalAmountRequested: fixture.Create<decimal>(),
                supportingNotes: fixture.Create<string>(),
                DateTimeOffset.Now);
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

            Action act = () => Decision.CreateNew(
                crmCaseNumber,
                null,
                null,
                submissionDocumentLink,
                DateTimeOffset.UtcNow,
                Array.Empty<DecisionType>(), amountRequested, supportingNotes, DateTimeOffset.Now);

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
                new DecisionType(Data.Enums.Concerns.DecisionType.NoticeToImprove) { DecisionId = decisionId },
                new DecisionType(Data.Enums.Concerns.DecisionType.OtherFinancialSupport){ DecisionId = decisionId }
            };

            var expectation = new
            {
                CrmCaseNumber = caseNumber,
                RetrospectiveApproval = true,
                SubmissionRequired = true,
                SubmissionDocumentLink = link,
                ReceivedRequestDate = fixture.Create<DateTimeOffset>(),
                DecisionTypes = decisionTypes,
                TotalAmountRequested = 13.5m,
                SupportingNotes = notes,
                CurrentDateTime = DateTimeOffset.UtcNow
            };

            var sut = Decision.CreateNew(
                expectation.CrmCaseNumber,
                expectation.RetrospectiveApproval,
                expectation.SubmissionRequired,
                expectation.SubmissionDocumentLink,
                expectation.ReceivedRequestDate,
                expectation.DecisionTypes,
                expectation.TotalAmountRequested,
                expectation.SupportingNotes,
                expectation.CurrentDateTime
            );

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
            var sut = Decision.CreateNew(
                "12345",
                true,
                true,
                "https://somewhere/somelink.doc",
                DateTimeOffset.UtcNow,
                fixture.CreateMany<DecisionType>(5).ToArray(),
                13.5m,
                "some notes",
                DateTimeOffset.UtcNow
            );

            sut.GetTitle().Should().Be("Multiple Decision Types");
        }

        [Fact]
        public void GetTitle_Maps_Zero_Decision_Types_To_Text()
        {
            var sut = Decision.CreateNew(
                "12345",
                true,
                true,
                "https://somewhere/somelink.doc",
                DateTimeOffset.UtcNow,
                null,
                13.5m,
                "some notes",
                DateTimeOffset.UtcNow
            );

            sut.GetTitle().Should().Be("No Decision Types");
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void GetTitle_When_One_DecisionType_Maps_To_DecisionType_Description(Data.Enums.Concerns.DecisionType decisionType)
        {
            var decisionTypes = new[]
            {
                new DecisionType(decisionType) { DecisionId = (int)decisionType },
            };

            var sut = Decision.CreateNew(
                "12345",
                true,
                true,
                "https://somewhere/somelink.doc",
                DateTimeOffset.UtcNow,
                decisionTypes,
                13.5m,
                "some notes",
                DateTimeOffset.UtcNow
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