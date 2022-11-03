using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.RequestModels.CaseActions.NTI.NoticeToImprove;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.NoticeToImprove;
using ConcernsCaseWork.Data.Models;
using FizzWare.NBuilder;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Factories
{
    public class NoticeToImproveFactoryTests
    {
       [Fact]
       public void CreateDBModel_ExpectedNoticeToImprove_WhenCreateNoticeToImproveRequestProvided()
        {
            var dtCreated = DateTime.Now;

            var details = new
            {
                CaseUrn = 123,
                DateStarted = dtCreated,
                Notes = "Notes",
                StatusId = 1,
                CreatedBy = "Test Name",
                CreatedAt = dtCreated,
                UpdatedAt = dtCreated,
                NoticeToImproveReasonsMapping = new List<int>() { 1, 3},
                NoticeToImproveConditionsMapping = new List<int>() { 1, 3},
                ClosedStatusId = 1,
                ClosedAt = dtCreated
            };

            var createNoticeToImprove = Builder<CreateNoticeToImproveRequest>
                .CreateNew()
                .With(r => r.CaseUrn = details.CaseUrn)
                .With(r => r.DateStarted = details.DateStarted)
                .With(r => r.Notes = details.Notes)
                .With(r => r.StatusId = details.StatusId)
                .With(r => r.NoticeToImproveReasonsMapping = details.NoticeToImproveReasonsMapping)
                .With(r => r.NoticeToImproveConditionsMapping = details.NoticeToImproveConditionsMapping)
                .With(r => r.CreatedBy = details.CreatedBy)
                .With(r => r.CreatedAt = details.CreatedAt)
                .With(r => r.UpdatedAt = details.UpdatedAt)
                .With(r => r.ClosedStatusId = details.ClosedStatusId)
                .With(r => r.ClosedAt = details.ClosedAt)
                .Build();


            var expectedNoticeToImprove = new NoticeToImprove
            {
                CaseUrn = details.CaseUrn,
                Notes = details.Notes,
                DateStarted = details.DateStarted,
                StatusId = details.StatusId,
                NoticeToImproveReasonsMapping = details.NoticeToImproveReasonsMapping.Select(r => {
                    return new NoticeToImproveReasonMapping()
                    {
                        NoticeToImproveReasonId = r
                    };
                }).ToList(),
                NoticeToImproveConditionsMapping = details.NoticeToImproveConditionsMapping.Select(c => {
                    return new NoticeToImproveConditionMapping()
                    {
                        NoticeToImproveConditionId = c
                    };
                }).ToList(),
                CreatedBy = details.CreatedBy,
                CreatedAt = details.CreatedAt,
                UpdatedAt = details.UpdatedAt,
                ClosedStatusId = details.ClosedStatusId,
                ClosedAt = details.ClosedAt
            };

            var response = NoticeToImproveFactory.CreateDBModel(createNoticeToImprove);
            response.Should().BeEquivalentTo(expectedNoticeToImprove);
        }

       [Fact]
       public void CreateDBModel_ExpectedNoticeToImprove_WhenPatchNoticeToImproveRequestProvided()
        {
            var dtCreated = DateTime.Now;

            var details = new
            {
                Id = 111,
                CaseUrn = 123,
                DateStarted = dtCreated,
                Notes = "Notes",
                StatusId = 1,
                CreatedBy = "Test Name",
                CreatedAt = dtCreated,
                UpdatedAt = dtCreated,
                NoticeToImproveReasonsMapping = new List<int>() { 1, 3 },
                NoticeToImproveConditionsMapping = new List<int>() { 1, 3 },
                ClosedStatusId = 1,
                ClosedAt = dtCreated,
                SumissionDecisionId = "100001",
                DateNTILifted = dtCreated,
                DateNTIClosed = dtCreated
            };


            var patchNoticeToImprove = Builder<PatchNoticeToImproveRequest>
                .CreateNew()
                .With(r => r.Id = details.Id)
                .With(r => r.CaseUrn = details.CaseUrn)
                .With(r => r.DateStarted = details.DateStarted)
                .With(r => r.Notes = details.Notes)
                .With(r => r.StatusId = details.StatusId)
                .With(r => r.NoticeToImproveReasonsMapping = details.NoticeToImproveReasonsMapping)
                .With(r => r.NoticeToImproveConditionsMapping = details.NoticeToImproveConditionsMapping)
                .With(r => r.CreatedBy = details.CreatedBy)
                .With(r => r.CreatedAt = details.CreatedAt)
                .With(r => r.UpdatedAt = details.UpdatedAt)
                .With(r => r.ClosedStatusId = details.ClosedStatusId)
                .With(r => r.ClosedAt = details.ClosedAt)
                .With(r => r.SumissionDecisionId = details.SumissionDecisionId)
                .With(r => r.DateNTILifted = details.DateNTILifted)
                .With(r => r.DateNTIClosed = details.DateNTIClosed)
                .Build();


            var expectedNoticeToImprove = new NoticeToImprove
            {
                Id = details.Id,
                CaseUrn = details.CaseUrn,
                Notes = details.Notes,
                DateStarted = details.DateStarted,
                StatusId = details.StatusId,
                NoticeToImproveReasonsMapping = details.NoticeToImproveReasonsMapping.Select(r => {
                    return new NoticeToImproveReasonMapping()
                    {
                        NoticeToImproveReasonId = r
                    };
                }).ToList(),
                NoticeToImproveConditionsMapping = details.NoticeToImproveConditionsMapping.Select(c => {
                    return new NoticeToImproveConditionMapping()
                    {
                        NoticeToImproveConditionId = c
                    };
                }).ToList(),
                CreatedBy = details.CreatedBy,
                CreatedAt = details.CreatedAt,
                UpdatedAt = details.UpdatedAt,
                ClosedStatusId = details.ClosedStatusId,
                ClosedAt = details.ClosedAt,
                SumissionDecisionId = details.SumissionDecisionId,
                DateNTILifted = details.DateNTILifted,
                DateNTIClosed = details.DateNTIClosed
            };

            var response = NoticeToImproveFactory.CreateDBModel(patchNoticeToImprove);
            response.Should().BeEquivalentTo(expectedNoticeToImprove);
        }

       [Fact]
       public void CreateResponse_ExpectedNTIWarningLetterResponse_WhenNTIWarningLetterProvided()
       {
           var dtCreated = DateTime.Now;

           var details = new
           {
               Id = 456,
               CaseUrn = 123,
               DateStarted = dtCreated,
               Notes = "Notes",
               StatusId = 1,
               CreatedBy = "Test Name",
               CreatedAt = dtCreated,
               UpdatedAt = dtCreated,
               NoticeToImproveReasonsMapping = new List<int>() { 1, 3 }.Select(r =>
               {
                   return new NoticeToImproveReasonMapping() { NoticeToImproveReasonId = r };
               }).ToList(),
               NoticeToImproveConditionsMapping = new List<int>() { 1, 3 }.Select(c =>
               {
                   return new NoticeToImproveConditionMapping() { NoticeToImproveConditionId = c };
               }).ToList(),
               ClosedStatusId = 1,
               ClosedAt = dtCreated,
               SumissionDecisionId = "100001",
               DateNTILifted = dtCreated,
               DateNTIClosed = dtCreated
           };


           var noticeToImprove = Builder<NoticeToImprove>
               .CreateNew()
                .With(r => r.Id = details.Id)
                .With(r => r.CaseUrn = details.CaseUrn)
                .With(r => r.DateStarted = details.DateStarted)
                .With(r => r.Notes = details.Notes)
                .With(r => r.StatusId = details.StatusId)
                .With(r => r.NoticeToImproveReasonsMapping = details.NoticeToImproveReasonsMapping)
                .With(r => r.NoticeToImproveConditionsMapping = details.NoticeToImproveConditionsMapping)
                .With(r => r.CreatedBy = details.CreatedBy)
                .With(r => r.CreatedAt = details.CreatedAt)
                .With(r => r.UpdatedAt = details.UpdatedAt)
                .With(r => r.ClosedStatusId = details.ClosedStatusId)
                .With(r => r.ClosedAt = details.ClosedAt)
                .With(r => r.SumissionDecisionId = details.SumissionDecisionId)
                .With(r => r.DateNTILifted = details.DateNTILifted)
                .With(r => r.DateNTIClosed = details.DateNTIClosed)
                .Build();

            var expectedNoticeToImprove = new NoticeToImproveResponse
            {
                Id = details.Id,
                CaseUrn = details.CaseUrn,
                Notes = details.Notes,
                DateStarted = details.DateStarted,
                StatusId = details.StatusId,
                NoticeToImproveReasonsMapping = details.NoticeToImproveReasonsMapping.Select(r => r.NoticeToImproveReasonId).ToList(),
                NoticeToImproveConditionsMapping = details.NoticeToImproveConditionsMapping.Select(r => r.NoticeToImproveConditionId).ToList(),
                CreatedBy = details.CreatedBy,
                CreatedAt = details.CreatedAt,
                UpdatedAt = details.UpdatedAt,
                ClosedStatusId = details.ClosedStatusId,
                ClosedAt = details.ClosedAt,
                SumissionDecisionId = details.SumissionDecisionId,
                DateNTILifted = details.DateNTILifted,
                DateNTIClosed = details.DateNTIClosed
            };

           var response = NoticeToImproveFactory.CreateResponse(noticeToImprove);
           response.Should().BeEquivalentTo(expectedNoticeToImprove);
       }
    }
}
