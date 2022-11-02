using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.UseCases.CaseActions.NTI.NoticeToImprove;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;


namespace ConcernsCaseWork.API.Tests.UseCases
{
    public class GetNoticeToImproveByIdTests
    {
        [Fact]
        public void GetNoticeToImproveById_ShouldReturnNoticeToImproveResponse_WhenGivenNoticeToImproveId()
        {
            var noticeToImproveId = 544;
            var reasonMappings = new List<NoticeToImproveReasonMapping>() { new NoticeToImproveReasonMapping() { NoticeToImproveReasonId = 1 } };
            var conditionMappings = new List<NoticeToImproveConditionMapping>() { new NoticeToImproveConditionMapping() { NoticeToImproveConditionId = 1 } };


            var noticeToImprove = new NoticeToImprove
            {
                Id = noticeToImproveId,
                Notes = "test notice to improve",
                NoticeToImproveReasonsMapping = reasonMappings,
                NoticeToImproveConditionsMapping = conditionMappings
            };

            var expectedResult = NoticeToImproveFactory.CreateResponse(noticeToImprove);

            var mockGateway = new Mock<INoticeToImproveGateway>();
            mockGateway.Setup(g => g.GetNoticeToImproveById(noticeToImproveId)).Returns(Task.FromResult(noticeToImprove));

            var useCase = new GetNoticeToImproveById(mockGateway.Object);
            var result = useCase.Execute(noticeToImproveId);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
        }

    }
}
