using ConcernsCaseWork.API.RequestModels.CaseActions.NTI.NoticeToImprove;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.NoticeToImprove;
using ConcernsCaseWork.API.UseCases.CaseActions.NTI.NoticeToImprove;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.UseCases
{
    public class CreateNoticeToImproveTests
    {
	    [Fact]
        public void CreateNoticeToImprove_ShouldCreateAndReturnNoticeToImproveResponse_WhenGivenCreateNoticeToImproveRequest()
        {
			var caseUrn = 544;
			var now = DateTime.Now;
			var statusId = 1;
			var notes = "Notes";
			var createdBy = "Test User";
			var reasons = new List<int>() { 1, 3 };
			var conditions = new List<int>() { 1, 3 };

			var createNoticeToImproveRequest = Builder<CreateNoticeToImproveRequest>
	            .CreateNew()
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
				CaseUrn = caseUrn,
				StatusId = statusId,
				DateStarted = now,
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
				}).ToList()
			};

            var expectedResult = new NoticeToImproveResponse
            {
				CaseUrn = caseUrn,
				StatusId = statusId,
				DateStarted = now,
				CreatedAt = now,
				Notes = notes,
				CreatedBy = createdBy,
				NoticeToImproveReasonsMapping = reasons,
				NoticeToImproveConditionsMapping = conditions
			};
			
			var mockGateway = new Mock<INoticeToImproveGateway>();
            
            mockGateway.Setup(g => g.CreateNoticeToImprove(It.IsAny<NoticeToImprove>())).Returns(Task.FromResult(noticeToImproveDbModel));
            
            var useCase = new CreateNoticeToImprove(mockGateway.Object);
            
            var result = useCase.Execute(createNoticeToImproveRequest);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}