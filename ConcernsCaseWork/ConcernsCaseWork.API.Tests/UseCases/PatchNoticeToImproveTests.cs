using ConcernsCaseWork.API.RequestModels.CaseActions.NTI.NoticeToImprove;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.NoticeToImprove;
using ConcernsCaseWork.API.UseCases.CaseActions.NTI.NoticeToImprove;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using Xunit;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.API.Tests.UseCases
{
    public class PatchNoticeToImproveTests
    {
        [Fact]
        public void ShouldPatchNoticeToImproveAndReturnNoticeToImproveResponse_WhenGivenPatchNoticeToImproveRequest()
        {
            var id = 123;
            var caseUrn = 544;
            var now = DateTime.Now;
            var statusId = 1;
            var notes = "Notes";
            var createdBy = "Test User";
            var reasons = new List<int>() { 1, 3 };
            var conditions = new List<int>() { 1, 3 };

            var patchNoticeToImproveRequest = Builder<PatchNoticeToImproveRequest>
                .CreateNew()
                .With(r => r.Id = id)
                .With(r => r.CaseUrn = caseUrn)
                .With(r => r.StatusId = statusId)
                .With(r => r.CreatedAt = now)
                .With(r => r.DateStarted = now)
                .With(r => r.Notes = notes) 
                .With(r => r.CreatedBy = createdBy)
                .With(r => r.NoticeToImproveReasonsMapping = reasons)
                .With(r => r.NoticeToImproveConditionsMapping = conditions)
                .Build();

            var noticeToImproveDbModel = new NoticeToImprove
            {
                Id = id,
                CaseUrn = caseUrn,
                CreatedAt = now,
                Notes = notes,
                CreatedBy = createdBy,
                NoticeToImproveReasonsMapping = reasons.Select(r => {
                    return new NoticeToImproveReasonMapping()
                    {
                        NoticeToImproveReasonId = r
                    };
                }).ToList(),
                NoticeToImproveConditionsMapping = conditions.Select(c => {
                    return new NoticeToImproveConditionMapping()
                    {
                        NoticeToImproveConditionId = c
                    };
                }).ToList(),
            };

            var expectedResult = new NoticeToImproveResponse
            {
                Id = id,
                CaseUrn = caseUrn,
                CreatedAt = now,
                Notes = notes,
                CreatedBy = createdBy,
                NoticeToImproveReasonsMapping = reasons,
                NoticeToImproveConditionsMapping = conditions
            };

            var mockGateway = new Mock<INoticeToImproveGateway>();
            mockGateway.Setup(g => g.PatchNoticeToImprove(It.IsAny<NoticeToImprove>())).Returns(Task.FromResult(noticeToImproveDbModel));

            var useCase = new PatchNoticeToImprove(mockGateway.Object);
            var result = useCase.Execute(patchNoticeToImproveRequest);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}