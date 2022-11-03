using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Models;
using System;
using FluentAssertions;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Factories
{
    public class ConcernsMeansOfReferralResponseFactoryTests
    {
        [Fact]
        public void ReturnsConcernsMeansOfReferralResponse_WhenGivenAnConcernsMeansOfReferral()
        {
            var concernMeansOfReferral = new ConcernsMeansOfReferral
            {
                Id = 2,
                Name = "test means of referral",
                Description = "some description of the means of referral",
                CreatedAt = new DateTime(2022, 07,27),
                UpdatedAt = new DateTime(2022, 07,28)
            };

            var expected = new ConcernsMeansOfReferralResponse
            {
                Name = concernMeansOfReferral.Name,
                Description = concernMeansOfReferral.Description,
                CreatedAt = concernMeansOfReferral.CreatedAt,
                UpdatedAt = concernMeansOfReferral.UpdatedAt,
                Id = concernMeansOfReferral.Id
            };

            var result = ConcernsMeansOfReferralResponseFactory.Create(concernMeansOfReferral);
            result.Should().BeEquivalentTo(expected);
        }
    }
}