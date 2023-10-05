using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.API.Features.ConcernsType;
using ConcernsCaseWork.Data.Models;
using FluentAssertions;
using System;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Factories
{
	public class ConcernsTypeResponseFactoryTests
    {
        [Fact]
        public void ReturnsConcernsTypeResponse_WhenGivenAnConcernsType()
        {
            var concernType = new ConcernsType
            {
                Id = 2,
                Name = "test concerns type",
                Description = "a test concerns type",
                CreatedAt = new DateTime(2021, 04,07),
                UpdatedAt = new DateTime(2021, 04,07)
            };

            var expected = new ConcernsTypeResponse
            {
	            Id = concernType.Id,
                Name = concernType.Name,
                Description = concernType.Description,
                CreatedAt = concernType.CreatedAt,
                UpdatedAt = concernType.UpdatedAt
            };

            var result = ConcernsTypeResponseFactory.Create(concernType);
            result.Should().BeEquivalentTo(expected);
        }
    }
}