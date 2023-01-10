using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.Data.Models;
using System;
using FizzWare.NBuilder;
using FluentAssertions;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Factories
{
    public class ConcernsRecordFactoryTests
    {
        [Fact]
        public void Create_ReturnsConcernsRecord_WhenGivenAnConcernsRecordRequest()
        {
            var concernsCase = Builder<ConcernsCase>.CreateNew().Build();
            var concernsType = Builder<ConcernsType>.CreateNew().Build();
            var concernsRating = Builder<ConcernsRating>.CreateNew().Build();
            var concernsMeansOfReferral = Builder<ConcernsMeansOfReferral>.CreateNew().Build();
            
            var recordRequest = new ConcernsRecordRequest
            {
                CreatedAt = new DateTime(2021, 05, 14),
                UpdatedAt = new DateTime(2021, 05, 14),
                ReviewAt = new DateTime(2021, 07, 05),
                ClosedAt = new DateTime(2021, 07, 05), // ClosedAt is not relevant when creating a new concerns record - this value is ignored
                Name = "Test concerns record",
                Description = "Test concerns record desc",
                Reason = "Test concern",
                CaseUrn = 1,
                TypeId = 2,
                RatingId = 3,
                StatusId = 1
            };

            var expected = new ConcernsRecord
            {
                CreatedAt = recordRequest.CreatedAt,
                UpdatedAt = recordRequest.UpdatedAt,
                ReviewAt = recordRequest.ReviewAt,
                ClosedAt = null,
                Name = recordRequest.Name,
                Description = recordRequest.Description,
                Reason = recordRequest.Reason,
                ConcernsCase = concernsCase,
                ConcernsType = concernsType,
                ConcernsRating = concernsRating,
                StatusId = recordRequest.StatusId,
                ConcernsMeansOfReferral = concernsMeansOfReferral
            };

            var result = ConcernsRecordFactory.Create(recordRequest, concernsCase, concernsType, concernsRating, concernsMeansOfReferral);
            result.Should().BeEquivalentTo(expected);
        }
        
        [Fact]
        public void Update_ReturnsConcernsRecord_WhenGivenAnConcernsRecordRequest()
        {
	        var concernsCase = Builder<ConcernsCase>.CreateNew().Build();
	        var concernsType = Builder<ConcernsType>.CreateNew().Build();
	        var concernsRating = Builder<ConcernsRating>.CreateNew().Build();
	        var concernsMeansOfReferral = Builder<ConcernsMeansOfReferral>.CreateNew().Build();
	        var originalConcernsRecord = Builder<ConcernsRecord>.CreateNew().Build();
            
	        var recordRequest = new ConcernsRecordRequest
	        {
		        CreatedAt = new DateTime(2021, 05, 14),
		        UpdatedAt = new DateTime(2021, 05, 14),
		        ReviewAt = new DateTime(2021, 07, 05),
		        ClosedAt = new DateTime(2021, 07, 05), // ClosedAt IS relevant when updating new concerns record - this value is used
		        Name = "Test concerns record",
		        Description = "Test concerns record desc",
		        Reason = "Test concern",
		        CaseUrn = 1,
		        TypeId = 2,
		        RatingId = 3,
		        StatusId = 1
	        };

	        var expected = new ConcernsRecord
	        {
		        CaseId = originalConcernsRecord.CaseId,
		        Id = originalConcernsRecord.Id,
		        CreatedAt = recordRequest.CreatedAt,
		        UpdatedAt = recordRequest.UpdatedAt,
		        ReviewAt = recordRequest.ReviewAt,
		        ClosedAt = recordRequest.ClosedAt,
		        Name = recordRequest.Name,
		        Description = recordRequest.Description,
		        Reason = recordRequest.Reason,
		        ConcernsCase = concernsCase,
		        ConcernsType = concernsType,
		        ConcernsRating = concernsRating,
		        ConcernsMeansOfReferral = concernsMeansOfReferral,
		        StatusId = recordRequest.StatusId,
		        TypeId = recordRequest.TypeId,
		        RatingId = recordRequest.RatingId,
		        MeansOfReferralId = recordRequest.MeansOfReferralId
	        };

	        var result = ConcernsRecordFactory.Update(originalConcernsRecord, recordRequest, concernsCase, concernsType, concernsRating, concernsMeansOfReferral);
	        result.Should().BeEquivalentTo(expected);
        }
    }
}