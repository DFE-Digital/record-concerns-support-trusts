using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.Data.Models;
using System;
using FizzWare.NBuilder;
using FluentAssertions;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Factories
{
    public class ConcernsCaseFactoryTests
    {
        [Fact]
        public void ReturnsConcernsCase_WhenGivenAnConcernsCaseRequest()
        {
            var request = Builder<ConcernCaseRequest>.CreateNew()
                .With(c => c.CreatedAt = new DateTime(2022,10,13))
                .With(c => c.UpdatedAt = new DateTime(2022,06,07))
                .With(c => c.ReviewAt = new DateTime(2022,07,10))
                .With(c => c.CreatedBy = "7654")
                .With(c => c.Description = " Test Description for case")
                .With(c => c.CrmEnquiry = "3456")
                .With(c => c.TrustUkprn = "17654")
                .With(c => c.ReasonAtReview = "Test concerns")
                .With(c => c.DeEscalation = new DateTime(2022,04,01))
                .With(c => c.Issue = "Here is the issue")
                .With(c => c.CurrentStatus = "Case status")
                .With(c => c.CaseAim = "Here is the aim")
                .With(c => c.DeEscalationPoint = "Point of de-escalation")
                .With(c => c.NextSteps = "Here are the next steps")
                .With(c => c.DirectionOfTravel = "Up")
                .With(c => c.StatusUrn = 3)
                .With(c => c.RatingUrn = 3)
                .Build();

            var expected = new ConcernsCase
            {
                CreatedAt = request.CreatedAt,
                UpdatedAt = request.UpdatedAt,
                ReviewAt = request.ReviewAt,
                ClosedAt = request.ClosedAt,
                CreatedBy = request.CreatedBy,
                Description = request.Description,
                CrmEnquiry = request.CrmEnquiry,
                TrustUkprn = request.TrustUkprn,
                ReasonAtReview = request.ReasonAtReview,
                DeEscalation = request.DeEscalation,
                Issue = request.Issue,
                CurrentStatus = request.CurrentStatus,
                CaseAim = request.CaseAim,
                DeEscalationPoint = request.DeEscalationPoint,
                NextSteps = request.NextSteps,
                DirectionOfTravel = request.DirectionOfTravel,
                StatusUrn = request.StatusUrn,
                RatingUrn = request.RatingUrn
            };

            var result = ConcernsCaseFactory.Create(request);
            result.Should().BeEquivalentTo(expected);
        }
    }
}