using AutoFixture.Idioms;
using AutoFixture;
using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using FluentAssertions.Execution;
using System.Reflection;

namespace ConcernsCaseWork.API.Tests.RequestModels.Concerns.Decisions
{
    public class AllRequestModelTests
    {
        [Fact]
        public void All_Constructors_Guard_Against_Null()
        {
            // Arrange
            var requestTypes = GetRequestTypes();
            var fixture = new Fixture();

            foreach (var requestType in requestTypes)
            {
                using (var scope = new AssertionScope())
                {
                    // Act & Assert
                    var assertion = fixture.Create<GuardClauseAssertion>();

                    assertion.Verify(requestType.GetConstructors());
                }
            }
        }

        [Fact]
        public void Properties_Are_Initialized_By_Constructor()
        {
            // Arrange
            var requestTypes = GetRequestTypes();
            var fixture = new Fixture();

            foreach (var requestType in requestTypes)
            {
                using (var scope = new AssertionScope())
                {
                    // Act & Assert
                    var assertion = fixture.Create<ConstructorInitializedMemberAssertion>();
                    assertion.Verify(requestType.GetConstructors());
                }
            }
        }

        [Fact]
        public void Property_Setters_Work_As_Expected()
        {
            // Arrange
            var requestTypes = GetRequestTypes();
            var fixture = new Fixture();

            foreach (var requestType in requestTypes)
            {
                using (var scope = new AssertionScope())
                {
                    // Act & Assert
                    var assertion = fixture.Create<WritablePropertyAssertion>();

                    assertion.Verify(requestType.GetProperties());
                }
            }
        }

        private TypeInfo[] GetRequestTypes()
        {
            return typeof(CreateDecisionRequest).Assembly
                .DefinedTypes
                .Where(x =>
                    x.IsClass &&
                    x.Namespace != null
                    && x.Namespace.StartsWith(typeof(CreateDecisionRequest).Namespace))
                .ToArray();
        }
    }
}
