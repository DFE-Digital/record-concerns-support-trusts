using ConcernsCaseWork.API.Controllers;
using ConcernsCaseWork.API.RequestModels.Concerns.TeamCasework;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.ResponseModels.Concerns.TeamCasework;
using ConcernsCaseWork.API.UseCases;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Controllers
{
    public class ConcernsTeamCaseworkControllerTests
    {
        private readonly Mock<ILogger<ConcernsTeamCaseworkController>> _mockLogger = new Mock<ILogger<ConcernsTeamCaseworkController>>();

        [Fact]
        public async Task Get_Returns200_When_Successfully_Fetched_Data()
        {
            // arrange
            var expectedOwnerId = "john.smith";
            var expectedData = new ConcernsCaseworkTeamResponse() { OwnerId = expectedOwnerId, TeamMembers = new[] { "john.doe", "jane.doe", "fred.flintstone" } };
            
            var getTeamCommand = new Mock<IGetConcernsCaseworkTeam>();
            getTeamCommand.Setup(x => x.Execute(expectedOwnerId, It.IsAny<CancellationToken>())).ReturnsAsync(expectedData);

            var getTeamOwnersCommand = new Mock<IGetConcernsCaseworkTeamOwners>();

            var updateCommand = new Mock<IUpdateConcernsCaseworkTeam>();

            var controller = new ConcernsTeamCaseworkController(
                _mockLogger.Object,
                getTeamCommand.Object,
                getTeamOwnersCommand.Object,
                updateCommand.Object
            );

            // act
            var actionResult = await controller.GetTeam("john.smith", CancellationToken.None);
            var expectedResponse = new ApiSingleResponseV2<ConcernsCaseworkTeamResponse>(expectedData);

            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            okResult.StatusCode.Value.Should().Be(StatusCodes.Status200OK);
            (okResult.Value as ApiSingleResponseV2<ConcernsCaseworkTeamResponse>).Should().NotBeNull();
            ((ApiSingleResponseV2<ConcernsCaseworkTeamResponse>)okResult.Value).Data.Should().BeEquivalentTo(expectedData);
        }


        [Fact]
        public async Task Get_ReturnsNoContent_When_No_Data_Available()
        {
            // arrange
            var expectedOwnerId = "john.smith";
            var expectedData = new ConcernsCaseworkTeamResponse() { OwnerId = expectedOwnerId, TeamMembers = new[] { "john.doe", "jane.doe", "fred.flintstone" } };

            var getTeamCommand = new Mock<IGetConcernsCaseworkTeam>();
            getTeamCommand.Setup(x => x.Execute(expectedOwnerId, It.IsAny<CancellationToken>())).ReturnsAsync(default(ConcernsCaseworkTeamResponse));

            var getTeamOwnersCommand = new Mock<IGetConcernsCaseworkTeamOwners>();

            var updateCommand = new Mock<IUpdateConcernsCaseworkTeam>();

            var controller = new ConcernsTeamCaseworkController(
                _mockLogger.Object,
                getTeamCommand.Object,
                getTeamOwnersCommand.Object,
                updateCommand.Object
            );

            // act
            var actionResult = await controller.GetTeam("john.smith", CancellationToken.None);
            Assert.IsType<NoContentResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetTeamOwners_Returns_200_And_Data_When_Data_Exists()
        {
            // arrange
            var expectedData = new[] { "john.doe", "jane.doe", "fred.flintstone" };

            var getTeamOwnersCommand = new Mock<IGetConcernsCaseworkTeamOwners>();
            getTeamOwnersCommand.Setup(x => x.Execute(CancellationToken.None)).ReturnsAsync(expectedData);


            var updateCommand = new Mock<IUpdateConcernsCaseworkTeam>();

            var controller = new ConcernsTeamCaseworkController(
                _mockLogger.Object,
                Mock.Of<IGetConcernsCaseworkTeam>(),
                getTeamOwnersCommand.Object,
                updateCommand.Object
            );

            // act
            var actionResult = await controller.GetTeamOwners(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            okResult.StatusCode.Value.Should().Be(StatusCodes.Status200OK);
            (okResult.Value as ApiSingleResponseV2<string[]>).Should().NotBeNull();
            ((ApiSingleResponseV2<string[]>)okResult.Value).Data.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public async Task GetTeamOwners_Returns_200_When_No_Data_Exists()
        {
            // arrange
            var getTeamOwnersCommand = new Mock<IGetConcernsCaseworkTeamOwners>();
            getTeamOwnersCommand.Setup(x => x.Execute(CancellationToken.None)).ReturnsAsync(default(string[]));

            var updateCommand = new Mock<IUpdateConcernsCaseworkTeam>();

            var controller = new ConcernsTeamCaseworkController(
                _mockLogger.Object,
                Mock.Of<IGetConcernsCaseworkTeam>(),
                getTeamOwnersCommand.Object,
                updateCommand.Object
            );

            // act
            var actionResult = await controller.GetTeamOwners(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            okResult.StatusCode.Value.Should().Be(StatusCodes.Status200OK);
            (okResult.Value as ApiSingleResponseV2<string[]>).Should().NotBeNull();
            ((ApiSingleResponseV2<string[]>)okResult.Value).Data.Should().BeEquivalentTo(Array.Empty<string>());
        }

        [Fact]
        public async Task Put_ReturnsBadRequest_When_OwnerId_Differs_From_Model()
        {
            // arrange            
            var getTeamCommand = new Mock<IGetConcernsCaseworkTeam>();
            var updateTeamCommand = new Mock<IUpdateConcernsCaseworkTeam>();
            var getTeamOwnersCommand = new Mock<IGetConcernsCaseworkTeamOwners>();

            var controller = new ConcernsTeamCaseworkController(
                _mockLogger.Object,
                getTeamCommand.Object,
                getTeamOwnersCommand.Object,
                updateTeamCommand.Object
            );

            var updateModel = new ConcernsCaseworkTeamUpdateRequest
            {
                OwnerId = "different.ownerId",
                TeamMembers = new[] { "Barny.Rubble" }
            };

            // act
            var actionResult = await controller.Put("john.smith", updateModel, CancellationToken.None);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task Put_ReturnsBadRequest_When_Model_IsNull()
        {
            // arrange
            var getTeamCommand = new Mock<IGetConcernsCaseworkTeam>();
            var updateTeamCommand = new Mock<IUpdateConcernsCaseworkTeam>();
            var getTeamOwnersCommand = new Mock<IGetConcernsCaseworkTeamOwners>();

            var controller = new ConcernsTeamCaseworkController(
                _mockLogger.Object,
                getTeamCommand.Object,
                getTeamOwnersCommand.Object,
                updateTeamCommand.Object
            );

            // act
            var actionResult = await controller.Put("john.smith", null, CancellationToken.None);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task Put_ReturnsOK_When_UpdateCommand_Executed()
        {
            // arrange
            var expectedOwnerId = "john.smith";
            var expectedModel = new ConcernsCaseworkTeamUpdateRequest() { OwnerId = expectedOwnerId, TeamMembers = new[] { "john.doe", "jane.doe", "fred.flintstone" } };

            var getTeamCommand = new Mock<IGetConcernsCaseworkTeam>();
            var updateTeamCommand = new Mock<IUpdateConcernsCaseworkTeam>();
            var getTeamOwnersCommand = new Mock<IGetConcernsCaseworkTeamOwners>();

            updateTeamCommand.Setup(x => x.Execute(expectedModel, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ConcernsCaseworkTeamResponse { OwnerId = expectedModel.OwnerId, TeamMembers = expectedModel.TeamMembers });

            var controller = new ConcernsTeamCaseworkController(
                _mockLogger.Object,
                getTeamCommand.Object,
                getTeamOwnersCommand.Object,
                updateTeamCommand.Object
            );

            // act
            var actionResult = await controller.Put(expectedOwnerId, expectedModel, CancellationToken.None);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            (okResult.Value as ApiSingleResponseV2<ConcernsCaseworkTeamResponse>).Should().NotBeNull();
            ((ApiSingleResponseV2<ConcernsCaseworkTeamResponse>)okResult.Value).Data.Should().BeEquivalentTo(expectedModel);
        }
    }
}
