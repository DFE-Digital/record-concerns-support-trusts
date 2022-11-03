using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;
using System.Collections.Generic;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using Xunit;

namespace ConcernsCaseWork.API.Tests.UseCases
{
    public class GetConcernsCaseByTrustUkPrnTests
    {
        [Fact]
        public void Execute_ShouldReturnListOfConcernsCaseResponses()
        {
            var mockGateway = new Mock<IConcernsCaseGateway>();
            var useCase = new GetConcernsCaseByTrustUkprn(mockGateway.Object);

            mockGateway
                .Setup(g => g.GetConcernsCaseByTrustUkprn(It.IsAny<string>(), 1, 50))
                .Returns(new List<ConcernsCase>());
            
            var result = useCase.Execute("1000", 1, 50);

            result.Should().BeOfType<List<ConcernsCaseResponse>>();
        }
        
        [Fact]
        public void Execute_WhenConcernsCaseIsNotFound_ReturnsEmptyList()
        {
            var mockGateway = new Mock<IConcernsCaseGateway>();
            var useCase = new GetConcernsCaseByTrustUkprn(mockGateway.Object);

            mockGateway
                .Setup(g => g.GetConcernsCaseByTrustUkprn(It.IsAny<string>(), 1, 50))
                .Returns(new List<ConcernsCase>());

            var result = useCase.Execute("1000", 1, 50);

            result.Should().BeEmpty();
        }

        [Fact]
        public void Execute_WhenConcernsCasesAreFound_ReturnsAllItemsFound()
        {
            var mockGateway = new Mock<IConcernsCaseGateway>();
            var useCase = new GetConcernsCaseByTrustUkprn(mockGateway.Object);
            var status = Builder<ConcernsStatus>.CreateNew().Build();
            var concernsCases = Builder<ConcernsCase>.CreateListOfSize(20).Build();

            mockGateway
                .Setup(g => g.GetConcernsCaseByTrustUkprn(It.IsAny<string>(), 1, 50))
                .Returns(concernsCases);

            var result = useCase.Execute("1000", 1, 50);

            result.Count.Should().Be(concernsCases.Count);
        }
    }
}