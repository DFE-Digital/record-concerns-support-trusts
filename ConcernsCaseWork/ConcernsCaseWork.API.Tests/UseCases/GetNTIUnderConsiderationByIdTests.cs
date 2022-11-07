using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.UseCases.CaseActions.NTI.UnderConsideration;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.UseCases
{
    public class GetNTIUnderConsiderationByIdTests
    {
        [Fact]
        public void GetNTIUnderConsiderationById_ShouldReturnNTIUnderConsiderationResponse_WhenGivenNTIUnderConsiderationId()
        {
            var considerationId = 544;

            var consideration = new NTIUnderConsideration
            {
                Id = considerationId,
                Notes = "test consideration",
                UnderConsiderationReasonsMapping = new List<NTIUnderConsiderationReasonMapping>() { new NTIUnderConsiderationReasonMapping() { NTIUnderConsiderationReasonId = 1 } }
            };

            var expectedResult = NTIUnderConsiderationFactory.CreateResponse(consideration);

            var mockConsiderationGateway = new Mock<INTIUnderConsiderationGateway>();
            mockConsiderationGateway.Setup(g => g.GetNTIUnderConsiderationById(considerationId)).Returns(Task.FromResult(consideration));

            var useCase = new GetNTIUnderConsiderationById(mockConsiderationGateway.Object);
            var result = useCase.Execute(considerationId);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
