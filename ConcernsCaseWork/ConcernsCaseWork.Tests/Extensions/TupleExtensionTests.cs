using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Shared.Tests.Factory;
using NUnit.Framework;
using Service.TRAMS.Cases;
using System.Collections.Generic;

namespace ConcernsCaseWork.Tests.Extensions
{
	[Parallelizable(ParallelScope.All)]
	public class TupleExtensionTests
	{
		[Test]
		public void WhenSplit_ReturnCases()
		{
			// arrange
			var casesDto = CaseFactory.BuildListCaseDto();

			// act
			(IList<CaseDto> activeCasesDto, IList<CaseDto> monitoringCasesDto) = casesDto.Split(
				caseDto => caseDto.StatusUrn.CompareTo(1) == 0, 
				caseDto => caseDto.StatusUrn.CompareTo(2) == 0);

			// assert
			Assert.That(activeCasesDto, Is.Not.Null);
			Assert.That(monitoringCasesDto, Is.Not.Null);
			Assert.That(activeCasesDto.Count, Is.EqualTo(2));
			Assert.That(monitoringCasesDto.Count, Is.EqualTo(2));
		}
	}
}