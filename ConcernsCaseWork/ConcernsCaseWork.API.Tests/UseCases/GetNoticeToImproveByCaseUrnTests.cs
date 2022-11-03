using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.UseCases.CaseActions.NTI.NoticeToImprove;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.UseCases
{
    public class GetNoticeToImproveByCaseUrnTests
    {
        [Fact]
        public void GetNoticeToImproveByCaseUrn_ShouldReturnNoticeToImproveResponse_WhenGivenCaseUrn()
        {
            var caseUrn = 123;
            var reasonMappings = new List<NoticeToImproveReasonMapping>() { new NoticeToImproveReasonMapping() { NoticeToImproveReasonId = 1 } };
            var conditionMappings = new List<NoticeToImproveConditionMapping>() { new NoticeToImproveConditionMapping() { NoticeToImproveConditionId = 1 } };

            var matchingNoticeToImprove = new NoticeToImprove
            {
                CaseUrn = caseUrn,
                Notes = "Test NTI",
                NoticeToImproveReasonsMapping = reasonMappings,
                NoticeToImproveConditionsMapping = conditionMappings
            };


            var noticesToImprove = new List<NoticeToImprove>
            {
                matchingNoticeToImprove,
                new NoticeToImprove
                {
                    CaseUrn = 123,
                    Notes = "Test NTI 2",
                    NoticeToImproveReasonsMapping = reasonMappings,
                    NoticeToImproveConditionsMapping = conditionMappings

                },
                new NoticeToImprove
                {
                    CaseUrn = 345,
                    Notes = "Test NTI 3",
                    NoticeToImproveReasonsMapping = reasonMappings,
                    NoticeToImproveConditionsMapping = conditionMappings
                }
            };


            var expectedResult = NoticeToImproveFactory.CreateResponse(matchingNoticeToImprove);

            var mockNoticeToImproveGateway = new Mock<INoticeToImproveGateway>();
            mockNoticeToImproveGateway.Setup(g => g.GetNoticeToImproveByCaseUrn(caseUrn)).Returns(Task.FromResult((ICollection<NoticeToImprove>)noticesToImprove.Where(s => s.CaseUrn == caseUrn).ToList()));


            var useCase = new GetNoticeToImproveByCaseUrn(mockNoticeToImproveGateway.Object);

            var result = useCase.Execute(caseUrn);

            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            result.First().Should().BeEquivalentTo(expectedResult);
        }
    }
}
