using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models.Concerns.TeamCasework;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.UseCases
{
    public class GetConcernsCaseworkTeamTests
    {
        [Fact]
        public async Task GetConcernsCaseworkTeam_Implements_IGetConcernsCaseworkTeam()
        {
            typeof(GetConcernsCaseworkTeam).Should().BeAssignableTo<GetConcernsCaseworkTeam>();
        }

        [Fact]
        public async Task Execute_When_Team_Found_Returns_ConcernsCaseworkTeam()
        {
            var ownerId = "john.doe";
            var mockGateway = new Mock<IConcernsTeamCaseworkGateway>();
            var useCase = new GetConcernsCaseworkTeam(mockGateway.Object);

            mockGateway
            .Setup(g => g.GetByOwnerId(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ConcernsCaseworkTeam
            {
                Id = ownerId,
                TeamMembers = new List<ConcernsCaseworkTeamMember>
                {
                    new ConcernsCaseworkTeamMember { TeamMember = "user.one" } ,
                    new ConcernsCaseworkTeamMember { TeamMember = "user.two" } ,
                    new ConcernsCaseworkTeamMember { TeamMember = "user.three" }
                }
            });

            var sut = new GetConcernsCaseworkTeam(mockGateway.Object);
            var result = await sut.Execute(ownerId, CancellationToken.None);

            result.Should().NotBeNull();
            result.OwnerId.Should().Be(ownerId);
            result.TeamMembers.Length.Should().Be(3);
            result.TeamMembers.Should().Contain("user.one");
            result.TeamMembers.Should().Contain("user.two");
            result.TeamMembers.Should().Contain("user.three");
        }

        [Fact]
        public async Task Execute_When_Teamw_NotFound_Returns_Null()
        {
            var ownerId = "john.doe";
            var mockGateway = new Mock<IConcernsTeamCaseworkGateway>();
            var useCase = new GetConcernsCaseworkTeam(mockGateway.Object);

            mockGateway
                .Setup(g => g.GetByOwnerId(ownerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(default(ConcernsCaseworkTeam));

            var sut = new GetConcernsCaseworkTeam(mockGateway.Object);
            var result = await sut.Execute(ownerId, CancellationToken.None);

            result.Should().BeNull();
        }
    }
}
