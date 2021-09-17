using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Shared.Tests.Factory;
using NUnit.Framework;
using Service.TRAMS.Status;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Tests.Mappers
{
	[Parallelizable(ParallelScope.All)]
	public class HomeMappingTests
	{
		[Test]
		public void WhenMapCases_To_HomeModels()
		{
			// arrange
			var casesDto = CaseFactory.BuildListCaseDto();
			var trustsDetailsDto = TrustFactory.BuildListTrustDetailsDto();
			var recordsDto = RecordFactory.BuildListRecordDto();
			var ratingsDto = RatingFactory.BuildListRatingDto();
			var typesDto = TypeFactory.BuildListTypeDto();
			var statusLiveDto = StatusFactory.BuildStatusDto(Status.Live.ToString(), 1);
			var statusMonitoringDto = StatusFactory.BuildStatusDto(Status.Monitoring.ToString(), 2);

			// act
			(IList<HomeModel> activeCases, IList<HomeModel> monitoringCases) = HomeMapping.Map(casesDto, trustsDetailsDto,
				recordsDto, ratingsDto, typesDto, statusLiveDto, statusMonitoringDto);
			
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
			var rags = HomeMapping.FetchRag(rating);

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
			var rags = HomeMapping.FetchRagCss(rating);

			// assert
			CollectionAssert.AreEqual(rags, splitExpected);
		}
	}
}