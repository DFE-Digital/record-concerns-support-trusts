using AutoFixture.Idioms;
using AutoFixture;
using ConcernsCaseWork.API.Factories.Concerns.Decisions;
using ConcernsCaseWork.API.UseCases.CaseActions.Decisions;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Factories.Concerns.Decisions
{
public class AllDecisionFactoriesTests
    {
        [Fact]
        public void All_Constructors_Guard_Against_Null()
        {
            // Arrange
            var requestTypes = GetTypes();
            var fixture = CreateFixture();

            foreach (var typeInfo in requestTypes)
            {
                using (var scope = new AssertionScope())
                {
                    scope.AddReportable("type", typeInfo.FullName);

                    // Act & Assert
                    var assertion = fixture.Create<GuardClauseAssertion>();

                    assertion.Verify(typeInfo.GetConstructors());
                }
            }
        }

        [Fact]
        public void All_Methods_Guard_Against_Null()
        {
            // Arrange
            var requestTypes = GetTypes();
            var fixture = CreateFixture();

            foreach (var typeInfo in requestTypes)
            {
                using (var scope = new AssertionScope())
                {
                    scope.AddReportable("type", typeInfo.FullName);

                    // Act & Assert
                    var assertion = fixture.Create<GuardClauseAssertion>();

                    assertion.Verify(typeInfo.GetMethods());
                }
            }
        }

        [Fact]
        public void Properties_Are_Initialized_By_Constructor()
        {
            // Arrange
            var requestTypes = GetTypes();
            var fixture = CreateFixture();

            foreach (var typeInfo in requestTypes)
            {
                using (var scope = new AssertionScope())
                {
                    scope.AddReportable("type", typeInfo.FullName);

                    // Act & Assert
                    var assertion = fixture.Create<ConstructorInitializedMemberAssertion>();
                    assertion.Verify(typeInfo.GetConstructors());
                }
            }
        }

        [Fact]
        public void Property_Setters_Work_As_Expected()
        {
            // Arrange
            var requestTypes = GetTypes();
            var fixture = CreateFixture();

            foreach (var typeInfo in requestTypes)
            {
                using (var scope = new AssertionScope())
                {
                    scope.AddReportable("type", typeInfo.FullName);

                    // Act & Assert
                    var assertion = fixture.Create<WritablePropertyAssertion>();

                    assertion.Verify(typeInfo.GetProperties());
                }
            }
        }

        private TypeInfo[] GetTypes()
        {
            // THe typeToFind is the key type used to find classes within the same namespace.
            var typeToFind = typeof(CreateDecisionResponseFactory);
            return typeToFind.Assembly
                .DefinedTypes
                .Where(x =>
                    x.IsClass &&
                    x.Namespace != null
                    && x.Namespace.StartsWith(typeToFind.Namespace))
                .ToArray();
        }

        private Fixture CreateFixture(
            ILogger<GetDecisions> logger = null,
            IConcernsCaseGateway gateway = null,
            IGetDecisionsSummariesFactory decisionsSummariesFactory = null)
        {
            var fixture = new Fixture();
            fixture.Register(() => logger ?? Mock.Of<ILogger<GetDecisions>>());
            fixture.Register(() => gateway ?? Mock.Of<IConcernsCaseGateway>());
            fixture.Register(() => decisionsSummariesFactory ?? Mock.Of<IGetDecisionsSummariesFactory>());

            fixture.Customize<Decision>(sb => sb.FromFactory(() =>
            {
                var decision = Decision.CreateNew(
                    crmCaseNumber: new string(fixture.CreateMany<char>(Decision.MaxCaseNumberLength).ToArray()),
                    retrospectiveApproval: fixture.Create<bool>(),
                    submissionRequired: fixture.Create<bool>(),
                    submissionDocumentLink: new string(fixture.CreateMany<char>(Decision.MaxUrlLength).ToArray()),
                    receivedRequestDate: DateTimeOffset.Now,
                    decisionTypes: new DecisionType[] { new DecisionType(Data.Enums.Concerns.DecisionType.NoticeToImprove) },
                    totalAmountRequested: fixture.Create<decimal>(),
                    supportingNotes: new string(fixture.CreateMany<char>(Decision.MaxSupportingNotesLength).ToArray()),
                    DateTimeOffset.Now
                );
                decision.DecisionId = fixture.Create<int>();
                return decision;
            }));

            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            return fixture;
        }
    }
}
