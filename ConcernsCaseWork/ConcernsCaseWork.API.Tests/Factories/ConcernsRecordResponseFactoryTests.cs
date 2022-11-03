using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Models;
using System;
using FizzWare.NBuilder;
using FluentAssertions;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Factories
{
    public class ConcernsRecordResponseFactoryTests
    {
        [Fact]
        public void ReturnsConcernsRecordResponse_WhenGivenAnConcernsRecord()
        {
            var concernCase = Builder<ConcernsCase>.CreateNew().Build();
            var concernType = Builder<ConcernsType>.CreateNew().Build();
            var concernRating = Builder<ConcernsRating>.CreateNew().Build();
            var concernMeansOfReferral = Builder<ConcernsMeansOfReferral>.CreateNew().Build();
            
            var concernsRecord = new ConcernsRecord
            {
                Id = 1,
                CreatedAt = new DateTime(2021, 05, 14),
                UpdatedAt = new DateTime(2021, 07, 20),
                ReviewAt = new DateTime(2021, 06, 19),
                ClosedAt = new DateTime(2021, 12, 03),
                Name = "Test record",
                Description = "Test record desc",
                Reason = "Test reason",
                CaseId = 2,
                TypeId = 3,
                RatingId = 5,
                StatusId = 23,
                ConcernsCase = concernCase,
                ConcernsType = concernType,
                ConcernsRating = concernRating,
                ConcernsMeansOfReferral = concernMeansOfReferral
            };

            var expected = new ConcernsRecordResponse
            {
	            Id = concernsRecord.Id,
                CreatedAt = concernsRecord.CreatedAt,
                UpdatedAt = concernsRecord.UpdatedAt,
                ReviewAt = concernsRecord.ReviewAt,
                ClosedAt = concernsRecord.ClosedAt,
                Name = concernsRecord.Name,
                Description = concernsRecord.Description,
                Reason = concernsRecord.Reason,
                StatusId = concernsRecord.StatusId,
                TypeId = concernType.Id,
                CaseUrn = concernCase.Id,
                RatingId = concernRating.Id,
                MeansOfReferralId = concernMeansOfReferral.Id
            };

            var result = ConcernsRecordResponseFactory.Create(concernsRecord);
            result.Should().BeEquivalentTo(expected);
        }
    }
}