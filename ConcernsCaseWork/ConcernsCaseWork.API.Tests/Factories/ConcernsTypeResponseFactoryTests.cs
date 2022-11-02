using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Models;
using System;
using FluentAssertions;
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
                UpdatedAt = new DateTime(2021, 04,07),
                Urn = 123,
            };

            var expected = new ConcernsTypeResponse
            {
                Name = concernType.Name,
                Description = concernType.Description,
                CreatedAt = concernType.CreatedAt,
                UpdatedAt = concernType.UpdatedAt,
                Urn = concernType.Urn
            };

            var result = ConcernsTypeResponseFactory.Create(concernType);
            result.Should().BeEquivalentTo(expected);
        }
    }
}