using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Models;
using System;
using FluentAssertions;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Factories
{
    public class ConcernsCaseResponseFactoryTests
    {
        [Fact]
        public void ReturnsConcernsCaseResponse_WhenGivenAnConcernsCase()
        {
            var concernsCase = new ConcernsCase
            {
                Id = 123,
                CreatedAt = new DateTime(2021, 10,07),
                UpdatedAt = new DateTime(2021, 10,07),
                ReviewAt = new DateTime(2021, 10,07),
                ClosedAt = new DateTime(2021, 10,07),
                CreatedBy = "12345",
                Description = "Test description",
                CrmEnquiry = "9876",
                TrustUkprn = "7654893",
                ReasonAtReview = "34567",
                DeEscalation = new DateTime(2021, 10,07),
                Issue = "564378",
                CurrentStatus = "87960",
                CaseAim = "0129",
                DeEscalationPoint = "20394",
                NextSteps = "next steps",
                CaseHistory = "case history",
                DirectionOfTravel = "Direction",
                Territory = Territory.National_Operations,
                Urn = 109,
                StatusId = 123,
                RatingId = 345,
                TrustCompaniesHouseNumber = "12345678"
            };

            var expected = new ConcernsCaseResponse
            {
                CreatedAt = concernsCase.CreatedAt,
                UpdatedAt = concernsCase.UpdatedAt,
                ReviewAt = concernsCase.ReviewAt,
                ClosedAt = concernsCase.ClosedAt,
                CreatedBy = concernsCase.CreatedBy,
                Description = concernsCase.Description,
                CrmEnquiry = concernsCase.CrmEnquiry,
                TrustUkprn = concernsCase.TrustUkprn,
                ReasonAtReview = concernsCase.ReasonAtReview,
                DeEscalation = concernsCase.DeEscalation,
                Issue = concernsCase.Issue,
                CurrentStatus = concernsCase.CurrentStatus,
                CaseAim = concernsCase.CaseAim,
                DeEscalationPoint = concernsCase.DeEscalationPoint,
                NextSteps = concernsCase.NextSteps,
                CaseHistory = concernsCase.CaseHistory,
                DirectionOfTravel = concernsCase.DirectionOfTravel,
                Territory = concernsCase.Territory,
                Urn = concernsCase.Urn,
                StatusId = concernsCase.StatusId,
                RatingId = concernsCase.RatingId,
                TrustCompaniesHouseNumber = concernsCase.TrustCompaniesHouseNumber
            };

            var result = ConcernsCaseResponseFactory.Create(concernsCase);
            result.Should().BeEquivalentTo(expected);
        }
    }
}