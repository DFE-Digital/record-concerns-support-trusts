using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.Data.Models;
using FluentAssertions;
using System;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Factories
{
	public class ConcernsStatusResponseFactoryTests
    {
        [Fact]
        public void Returns_ConcernsStatusResponse_WhenGiven_ConcernsStatus()
        {
            var concernsStatus = new ConcernsStatus
            {
                Id = 456,
                Name = "Test concerns status",
                CreatedAt = new DateTime(2021, 10,07),
                UpdatedAt = new DateTime(2021, 10,07)
            };

            var expected = new ConcernsStatusResponse
            {
                Name = concernsStatus.Name,
                CreatedAt = concernsStatus.CreatedAt,
                UpdatedAt = concernsStatus.UpdatedAt,
                Id = concernsStatus.Id
            };

            var result = ConcernsStatusResponseFactory.Create(concernsStatus);
            result.Should().BeEquivalentTo(expected);
        }
    }
}