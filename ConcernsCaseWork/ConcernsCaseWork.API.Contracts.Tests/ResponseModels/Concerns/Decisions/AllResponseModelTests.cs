using AutoFixture;
using AutoFixture.Idioms;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using FluentAssertions.Execution;
using System.Reflection;

namespace ConcernsCaseWork.API.Tests.ResponseModels.Concerns.Decisions
{
    public class AllResponseModelTests
    {
        [Fact]
        public void All_Constructors_Guard_Against_Null()
        {
            // Arrange
            var requestTypes = GetResponseTypes();
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
            var requestTypes = GetResponseTypes();
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
            var requestTypes = GetResponseTypes();
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

        private TypeInfo[] GetResponseTypes()
        {
            return typeof(CreateDecisionResponse).Assembly
                .DefinedTypes
                .Where(x =>
                    x.IsClass &&
                    x.Namespace != null
                    && x.Namespace.StartsWith(typeof(CreateDecisionResponse).Namespace))
                .ToArray();
        }
    }
}
