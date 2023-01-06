using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;
using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using Xunit;

namespace ConcernsCaseWork.API.Tests.UseCases
{
    public class GetConcernsCasesByOwnerIdTests
    {
        [Fact]
        public void Execute_ShouldReturnListOfConcernsCaseResponsesForAGivenOwnerId()
        {
            var ownerId = "76832";
            var gateway = new Mock<IConcernsCaseGateway>();
            
            var cases = Builder<ConcernsCase>.CreateListOfSize(10)
                .All()
                .With(c => c.CreatedBy = ownerId)
                .Build();

            gateway.Setup(g =>
                    g.GetConcernsCasesByOwnerId(It.IsAny<string>(), null, 1, 50))
                .Returns(cases);

            var expected = cases.Select(ConcernsCaseResponseFactory.Create).ToList();

            var useCase = new GetConcernsCasesByOwnerId(gateway.Object);

            var result = useCase.Execute(ownerId, null, 1, 50);
            result.Should().BeEquivalentTo(expected);
        }
        
        [Fact]
        public void Execute_ShouldReturnListOfConcernsCaseResponsesFilteredByStatusWhenSupplied()
        {
            var ownerId = "564372";
            var statusId = 123;
            var gateway = new Mock<IConcernsCaseGateway>();
            
            var cases = Builder<ConcernsCase>.CreateListOfSize(5)
                .All()
                .With(c => c.CreatedBy = ownerId)
                .With(c => c.StatusId = statusId)
                .Build();

            gateway.Setup(g =>
                    g.GetConcernsCasesByOwnerId(It.IsAny<string>(), statusId, 1, 50))
                .Returns(cases);

            var expected = cases.Select(ConcernsCaseResponseFactory.Create).ToList();

            var useCase = new GetConcernsCasesByOwnerId(gateway.Object);

            var result = useCase.Execute(ownerId, statusId, 1, 50);
            result.Should().BeEquivalentTo(expected);
        }
        
        [Fact]
        public void Execute_ShouldReturnEmptyListWhenNoConcernsCasesAreFound()
        {
            var ownerId = "96784";
            var statusId = 567;
            var gateway = new Mock<IConcernsCaseGateway>();
            

            gateway.Setup(g =>
                    g.GetConcernsCasesByOwnerId(It.IsAny<string>(), statusId, 1, 50))
                .Returns(new List<ConcernsCase>());
            

            var useCase = new GetConcernsCasesByOwnerId(gateway.Object);

            var result = useCase.Execute(ownerId, statusId, 1, 50);
            result.Should().BeEquivalentTo(new List<ConcernsCaseResponse>());
        }
    }
}