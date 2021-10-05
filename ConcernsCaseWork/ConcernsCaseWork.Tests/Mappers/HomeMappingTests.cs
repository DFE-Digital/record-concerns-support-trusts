using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Shared.Tests.Factory;
using NUnit.Framework;
using Service.TRAMS.Records;
using Service.TRAMS.Status;
using Service.TRAMS.Type;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Tests.Mappers
{
	[Parallelizable(ParallelScope.All)]
	public class HomeMappingTests
	{
		[Test]
		public void WhenMap_ReturnsActive_Cases()
		{
			// arrange
			var casesDto = CaseFactory.BuildListCaseDto();
			var trustDetailsDto = TrustFactory.BuildListTrustDetailsDto();
			var recordsDto = new List<RecordDto> { RecordFactory.BuildRecordDto(), RecordFactory.BuildRecordDto(4) };
			var ratingsDto = RatingFactory.BuildListRatingDto();
			var typesDto = new List<TypeDto> { TypeFactory.BuildTypeDto() };
			var statusLiveDto = StatusFactory.BuildStatusDto(StatusEnum.Live.ToString(), 1);

			// act
			var activeCases = HomeMapping.Map(casesDto.Where(c => c.Status == 1).ToList(), trustDetailsDto,
				recordsDto, ratingsDto, typesDto, statusLiveDto);

			// assert
			Assert.That(activeCases, Is.Not.Null);
			Assert.That(activeCases.Count, Is.EqualTo(2));
		}
		
		[Test]
		public void WhenMap_ReturnsMonitoring_Cases()
		{
			// arrange
			var casesDto = CaseFactory.BuildListCaseDto();
			var trustDetailsDto = TrustFactory.BuildListTrustDetailsDto();
			var recordsDto = new List<RecordDto> { RecordFactory.BuildRecordDto(3), RecordFactory.BuildRecordDto(5) };
			var ratingsDto = RatingFactory.BuildListRatingDto();
			var typesDto = new List<TypeDto> { TypeFactory.BuildTypeDto() };
			var statusMonitoringDto = StatusFactory.BuildStatusDto(StatusEnum.Monitoring.ToString(), 2);

			// act
			var monitoringCases = HomeMapping.Map(casesDto.Where(c => c.Status == 2).ToList(), trustDetailsDto,
				recordsDto, ratingsDto, typesDto, statusMonitoringDto);

			// assert
			Assert.That(monitoringCases, Is.Not.Null);
			Assert.That(monitoringCases.Count, Is.EqualTo(2));
		}
		
		[Test]
		public void WhenMapCases_To_HomeModels()
		{
			// arrange
			var casesDto = CaseFactory.BuildListCaseDto();
			var trustDetailsDto = TrustFactory.BuildListTrustDetailsDto();
			var recordsDto = RecordFactory.BuildListRecordDto();
			var ratingsDto = RatingFactory.BuildListRatingDto();
			var typesDto = TypeFactory.BuildListTypeDto();
			var statusLiveDto = StatusFactory.BuildStatusDto(StatusEnum.Live.ToString(), 1);
			var statusMonitoringDto = StatusFactory.BuildStatusDto(StatusEnum.Monitoring.ToString(), 2);
			
			// act
			var activeCases = HomeMapping.Map(casesDto.Where(c => c.Status == 1).ToList(), trustDetailsDto,
				recordsDto, ratingsDto, typesDto, statusLiveDto);
			var monitoringCases = HomeMapping.Map(casesDto.Where(c => c.Status == 2).ToList(), trustDetailsDto,
				recordsDto, ratingsDto, typesDto, statusMonitoringDto);
			
			// assert
			Assert.That(activeCases, Is.Not.Null);
			Assert.That(monitoringCases, Is.Not.Null);
			Assert.That(activeCases.Count, Is.EqualTo(2));
			Assert.That(monitoringCases.Count, Is.EqualTo(2));
		}

		[TestCase("", "n/a")]
		[TestCase("n/a", "-")]
		[TestCase("Red-Plus", "Red Plus")]
		[TestCase("Red", "Red")]
		[TestCase("Red-Amber", "Red,Amber")]
		[TestCase("Amber-Green", "Amber,Green")]
		[TestCase("Amber", "Amber")]
		[TestCase("Green", "Green")]
		public void WhenFetchRag_ReturnsListRags(string rating, string expected)
		{
			// arrange
			var splitExpected = expected.Split(",").ToList();
			
			// act
			var rags = RagMapping.FetchRag(rating);

			// assert
			CollectionAssert.AreEqual(rags, splitExpected);
		}
		
		[TestCase("", "n/a")]
		[TestCase("n/a", "")]
		[TestCase("Red-Plus", "ragtag__redplus")]
		[TestCase("Red", "ragtag__red")]
		[TestCase("Red-Amber", "ragtag__red,ragtag__amber")]
		[TestCase("Amber-Green", "ragtag__amber,ragtag__green")]
		[TestCase("Amber", "ragtag__amber")]
		[TestCase("Green", "ragtag__green")]
		public void WhenFetchRagCss_ReturnsListRags(string rating, string expected)
		{
			// arrange
			var splitExpected = expected.Split(",").ToList();
			
			// act
			var rags = RagMapping.FetchRagCss(rating);

			// assert
			CollectionAssert.AreEqual(rags, splitExpected);
		}
	}
}