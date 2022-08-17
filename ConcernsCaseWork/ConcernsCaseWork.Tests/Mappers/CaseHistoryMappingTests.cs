using ConcernsCaseWork.Mappers;
using NUnit.Framework;
using ConcernsCasework.Service.Cases;
using System;

namespace ConcernsCaseWork.Tests.Mappers
{
	[Parallelizable(ParallelScope.All)]
	public class CaseHistoryMappingTests
	{
		[TestCase(CaseHistoryEnum.Case)]
		[TestCase(CaseHistoryEnum.Concern)]
		[TestCase(CaseHistoryEnum.RiskRating)]
		[TestCase(CaseHistoryEnum.DirectionOfTravel)]
		[TestCase(CaseHistoryEnum.Issue)]
		[TestCase(CaseHistoryEnum.CurrentStatus)]
		[TestCase(CaseHistoryEnum.CaseAim)]
		[TestCase(CaseHistoryEnum.DeEscalationPoint)]
		[TestCase(CaseHistoryEnum.NextSteps)]
		[TestCase(CaseHistoryEnum.ClosedForMonitoring)]
		[TestCase(CaseHistoryEnum.Closed)]
		public void WhenBuildCaseHistoryDto_Returns_ValidDto(CaseHistoryEnum caseHistoryEnum)
		{
			// act
			var createCaseHistoryDto = CaseHistoryMapping.BuildCaseHistoryDto(caseHistoryEnum, 1);
			
			// assert
			Assert.That(createCaseHistoryDto, Is.Not.Null);
			Assert.That(createCaseHistoryDto.Action, Is.Not.Null);
			Assert.That(createCaseHistoryDto.Description, Is.Not.Null);
			Assert.That(createCaseHistoryDto.Title, Is.Not.Null);
			Assert.That(createCaseHistoryDto.CaseUrn, Is.Not.Null);
			Assert.That(createCaseHistoryDto.CreatedAt, Is.Not.Null);
		}

		[Test]
		public void WhenBuildCaseHistoryDto_Throws_Exception()
		{
			// act | assert
			Assert.Throws<Exception>(() => CaseHistoryMapping.BuildCaseHistoryDto(CaseHistoryEnum.Fnti, 1));
		}
	}
}