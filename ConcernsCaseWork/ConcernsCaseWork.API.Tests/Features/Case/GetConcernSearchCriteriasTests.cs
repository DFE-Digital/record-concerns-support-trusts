using ConcernsCaseWork.API.Features.Case;
using ConcernsCaseWork.Data.Gateways;
using Moq;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Features.Case
{
    public class GetConcernSearchCriteriasTests
    {
		[Fact]
		public void Execute_ReturnsExpectedCaseSearchParametersResponse()
		{
			// Arrange
			var mockGateway = new Mock<IConcernsCaseGateway>();
			var expectedOwners = new[] { "owner1", "owner2" };
			var expectedTeamLeaders = new[] { "leader1", "leader2" };

			mockGateway.Setup(g => g.GetOwnersOfCases()).Returns(expectedOwners);
			mockGateway.Setup(g => g.GetTeamLeadersOfCases()).Returns(expectedTeamLeaders);

			var sut = new GetConcernSearchCriterias(mockGateway.Object);

			// Act
			var result = sut.Execute();

			// Assert
			Assert.Equal(expectedOwners, result.CaseOwners);
			Assert.Equal(expectedTeamLeaders, result.TeamLeaders);
		}
	}
}
