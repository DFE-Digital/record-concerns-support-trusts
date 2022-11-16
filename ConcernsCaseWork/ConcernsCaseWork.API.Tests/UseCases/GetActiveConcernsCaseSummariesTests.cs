using AutoFixture;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.UseCases
{
    public class GetActiveConcernsCaseSummariesByOwnerTests
    {
	    private readonly IFixture _fixture = new Fixture();
        [Fact]
        public async Task Execute_ShouldReturnListOfConcernsCaseResponsesForAGivenOwnerId()
        {
	        // arrange
            var ownerId = _fixture.Create<string>();
            var gateway = new Mock<ICaseSummaryGateway>();
            
            var cases = Builder<CaseSummaryVm>.CreateListOfSize(10)
                .All()
                .With(c => c.CreatedBy = ownerId)
                .With(c => c.Rating = Builder<ConcernsRating>.CreateNew().Build())
                .Build();

            gateway.Setup(g => g.GetActiveCaseSummaries(ownerId)).ReturnsAsync(cases);
            
            var sut = new GetActiveConcernsCaseSummariesByOwner(gateway.Object);

            // act
            var result = await sut.Execute(ownerId);

            // assert
            result.Should().HaveCount(10);
        }

        [Fact]
        public async Task Execute_ShouldReturnEmptyListWhenNoConcernsCases()
        {
	        // arrange
	        var ownerId = _fixture.Create<string>();
	        var gateway = new Mock<ICaseSummaryGateway>();

	        var cases = new List<CaseSummaryVm>();

	        gateway.Setup(g => g.GetActiveCaseSummaries(ownerId)).ReturnsAsync(cases);

	        var sut = new GetActiveConcernsCaseSummariesByOwner(gateway.Object);
	        
	        // act
	        var result = await sut.Execute(ownerId);

	        // assert
	        result.Should().BeEmpty();
        }
    }
}