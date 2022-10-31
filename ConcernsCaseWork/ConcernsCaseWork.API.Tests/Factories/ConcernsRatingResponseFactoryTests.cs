using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Models;
using System;
using FluentAssertions;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Factories
{
    public class ConcernsRatingResponseFactoryTests
    {
        [Fact]
        public void ReturnsConcernsRatingResponse_WhenGivenAnConcernsRating()
        {

            var concernsRating = new ConcernsRating
            {
                Id = 5,
                Name = "Test concerns rating",
                CreatedAt = new DateTime(2021, 10,07),
                UpdatedAt = new DateTime(2021, 10,07),
                Urn = 789
            };

            var expected = new ConcernsRatingResponse
            {
                Name = concernsRating.Name,
                CreatedAt = concernsRating.CreatedAt,
                UpdatedAt = concernsRating.UpdatedAt,
                Urn = concernsRating.Urn
            };

            var result = ConcernsRatingResponseFactory.Create(concernsRating);
            result.Should().BeEquivalentTo(expected);
        }
    }
}