using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Models;
using System;
using FluentAssertions;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Factories
{
    public class ConcernsStatusResponseFactoryTests
    {
        [Fact]
        public void ReturnsConcernsStatusResponse_WhenGivenAnConcernsStatus()
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