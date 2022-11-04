using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.UseCases.CaseActions.NTI.UnderConsideration;
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
    public class GetNTIUnderConsiderationByCaseUrnTests
    {
        [Fact]
        public void GetNTIUnderConsiderationyCaseUrn_ShouldReturnNTIUnderConsiderationResponse_WhenGivenCaseUrn()
        {
            var caseUrn = 123;

            var matchingConsideration = new NTIUnderConsideration
            {
                CaseUrn = caseUrn,
                Notes = "Test consideration",
                UnderConsiderationReasonsMapping = new List<NTIUnderConsiderationReasonMapping>() { new NTIUnderConsiderationReasonMapping() { NTIUnderConsiderationReasonId = 1 } }
            };

            var considerations = new List<NTIUnderConsideration>
            {
                matchingConsideration,
                new NTIUnderConsideration
                {
                    CaseUrn = 123,
                    Notes = "Test consideration 2",
                    UnderConsiderationReasonsMapping = new List<NTIUnderConsiderationReasonMapping>() { new NTIUnderConsiderationReasonMapping() { NTIUnderConsiderationReasonId = 1 } }
                },
                new NTIUnderConsideration
                {
                    CaseUrn = 345,
                    Notes = "Test consideration 2",
                    UnderConsiderationReasonsMapping = new List<NTIUnderConsiderationReasonMapping>() { new NTIUnderConsiderationReasonMapping() { NTIUnderConsiderationReasonId = 1 } }
                }
            };

            var expectedResult = NTIUnderConsiderationFactory.CreateResponse(matchingConsideration);

            var mockNTIUnderConsiderationGateway = new Mock<INTIUnderConsiderationGateway>();
            mockNTIUnderConsiderationGateway.Setup(g => g.GetNTIUnderConsiderationByCaseUrn(caseUrn)).Returns(Task.FromResult((ICollection<NTIUnderConsideration>)considerations.Where(s => s.CaseUrn == caseUrn).ToList()));

            var useCase = new GetNTIUnderConsiderationByCaseUrn(mockNTIUnderConsiderationGateway.Object);

            var result = useCase.Execute(caseUrn);

            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            result.First().Should().BeEquivalentTo(expectedResult);
        }
    }
}
