using ConcernsCaseWork.API.RequestModels.Concerns.TeamCasework;
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
    public class UpdateConcernsCaseworkTeamTests
    {
        [Fact]
        public async Task GetConcernsCaseworkTeam_Implements_IGetConcernsCaseworkTeam()
        {
            typeof(UpdateConcernsCaseworkTeam).Should().BeAssignableTo<IUpdateConcernsCaseworkTeam>();
        }

        [Fact]
        public async Task Execute_When_Team_Found_Performs_Update()
        {
            var ownerId = "john.doe";
            var mockGateway = new Mock<IConcernsTeamCaseworkGateway>();

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

            var updateRequest = new ConcernsCaseworkTeamUpdateRequest()
            {
                OwnerId = ownerId,
                TeamMembers = new string[]
                {
                    "user.one",
                    "user.three"
                }
            };

            // Act
            var sut = new UpdateConcernsCaseworkTeam(mockGateway.Object);
            var result = await sut.Execute(updateRequest, CancellationToken.None);

            result.Should().NotBeNull();
            result.OwnerId.Should().Be(ownerId);
            result.TeamMembers.Length.Should().Be(2);
            result.TeamMembers.Should().Contain("user.one");
            result.TeamMembers.Should().Contain("user.three");

            mockGateway.Verify(x => x.UpdateCaseworkTeam(It.Is<ConcernsCaseworkTeam>(c => c.Id == ownerId && c.TeamMembers.Count == 2), It.IsAny<CancellationToken>()), Times.Once);
            mockGateway.Verify(x => x.AddCaseworkTeam(It.IsAny<ConcernsCaseworkTeam>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Execute_When_Team_Not_Found_Performs_Add()
        {
            var ownerId = "john.doe";
            var mockGateway = new Mock<IConcernsTeamCaseworkGateway>();

            mockGateway
            .Setup(g => g.GetByOwnerId(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(default(ConcernsCaseworkTeam));

            var updateRequest = new ConcernsCaseworkTeamUpdateRequest()
            {
                OwnerId = ownerId,
                TeamMembers = new string[]
                {
                    "user.one",
                    "user.three"
                }
            };

            // Act
            var sut = new UpdateConcernsCaseworkTeam(mockGateway.Object);
            var result = await sut.Execute(updateRequest, CancellationToken.None);

            result.Should().NotBeNull();
            result.OwnerId.Should().Be(ownerId);
            result.TeamMembers.Length.Should().Be(2);
            result.TeamMembers.Should().Contain("user.one");
            result.TeamMembers.Should().Contain("user.three");

            // verify add was called.
            mockGateway.Verify(x => x.UpdateCaseworkTeam(It.IsAny<ConcernsCaseworkTeam>(), It.IsAny<CancellationToken>()), Times.Never);
            mockGateway.Verify(x => x.AddCaseworkTeam(It.Is<ConcernsCaseworkTeam>(c => c.Id == ownerId && c.TeamMembers.Count == 2), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
