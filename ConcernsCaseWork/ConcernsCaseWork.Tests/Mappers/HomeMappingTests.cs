using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Shared.Tests.Factory;
using NUnit.Framework;
using Service.TRAMS.Status;
using System.Collections.Generic;

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
	}
}