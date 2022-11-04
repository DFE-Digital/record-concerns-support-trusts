using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.UseCases.CaseActions.NTI.WarningLetter;
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
    public class GetNTIWarningLetterByCaseUrnTests
    {
        [Fact]
        public void GetNTIWarningLetteryCaseUrn_ShouldReturnNTIWarningLetterResponse_WhenGivenCaseUrn()
        {
            var caseUrn = 123;
            var reasonMappings = new List<NTIWarningLetterReasonMapping>() { new NTIWarningLetterReasonMapping() { NTIWarningLetterReasonId = 1 } };
            var conditionMappings = new List<NTIWarningLetterConditionMapping>() { new NTIWarningLetterConditionMapping() { NTIWarningLetterConditionId = 1 } };

            var matchingWarningLetter = new NTIWarningLetter
            {
                CaseUrn = caseUrn,
                Notes = "Test warning letter",
                WarningLetterReasonsMapping = reasonMappings,
                WarningLetterConditionsMapping = conditionMappings
            };

            var warningLetters = new List<NTIWarningLetter>
            {
                matchingWarningLetter,
                new NTIWarningLetter
                {
                    CaseUrn = 123,
                    Notes = "Test warning letter 2",
                    WarningLetterReasonsMapping = reasonMappings,
                    WarningLetterConditionsMapping = conditionMappings
                },
                new NTIWarningLetter
                {
                    CaseUrn = 345,
                    Notes = "Test warning letter 3",
                    WarningLetterReasonsMapping = reasonMappings,
                    WarningLetterConditionsMapping = conditionMappings
                }
            };

            var expectedResult = NTIWarningLetterFactory.CreateResponse(matchingWarningLetter);

            var mockNTIWarningLetterGateway = new Mock<INTIWarningLetterGateway>();
            mockNTIWarningLetterGateway.Setup(g => g.GetNTIWarningLetterByCaseUrn(caseUrn)).Returns(Task.FromResult((ICollection<NTIWarningLetter>)warningLetters.Where(s => s.CaseUrn == caseUrn).ToList()));

            var useCase = new GetNTIWarningLetterByCaseUrn(mockNTIWarningLetterGateway.Object);

            var result = useCase.Execute(caseUrn);

            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            result.First().Should().BeEquivalentTo(expectedResult);
        }
    }
}
