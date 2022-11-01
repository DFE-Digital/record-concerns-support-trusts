using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.UseCases.CaseActions.FinancialPlan;
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
    public class GetFinancialPlansByCaseIdTests
    {
        [Fact]
        public void GetFinancialPlanByCaseUrn_ShouldReturnFinancialPlanResponse_WhenGivenCaseUrn()
        {
            var caseUrn = 123;

            var matchingFP = new FinancialPlanCase
            {
                CaseUrn = caseUrn,
                Notes = "test notes xyz"
            };

            var fps = new List<FinancialPlanCase> {
                matchingFP,

                new FinancialPlanCase {
                    CaseUrn = 234,
                    Notes = "Financial Plan 2"
                },
                new FinancialPlanCase {
                    CaseUrn = 874,
                    Notes = "Financial plan 3"
                }
            };

            var expectedResult = FinancialPlanFactory.CreateResponse(matchingFP);

            var mockFPGateway = new Mock<IFinancialPlanGateway>();
            mockFPGateway.Setup(g => g.GetFinancialPlansByCaseUrn(caseUrn)).Returns(Task.FromResult((ICollection<FinancialPlanCase>)fps.Where(s => s.CaseUrn == caseUrn).ToList()));

            var useCase = new GetFinancialPlanByCaseId(mockFPGateway.Object);

            var result = useCase.Execute(caseUrn);

            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result.First().Should().BeEquivalentTo(expectedResult);
        }
    }
}
