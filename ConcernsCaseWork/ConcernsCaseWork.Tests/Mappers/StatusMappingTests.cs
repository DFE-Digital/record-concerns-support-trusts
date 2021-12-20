using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Shared.Tests.Factory;
using NUnit.Framework;
using Service.TRAMS.Trusts;
using System.Linq;

namespace ConcernsCaseWork.Tests.Mappers
{
	[Parallelizable(ParallelScope.All)]
	public class StatusMappingTests
	{
		[Test]
		public void WhenMapDtoToModel_ReturnsStatusModel()
		{
			//arrange
			var statusesDto = StatusFactory.BuildListStatusDto();
			var statusDto = statusesDto[1];

			// act
			var statusModel = StatusMapping.MapDtoToModel(statusesDto, statusDto.Urn);

			//// assert
			Assert.That(statusModel, Is.Not.Null);
		}

		[Test]
		public void WhenMapDtoToModel_When_Urn_IsUknown_ReturnsStatusModel()
		{
			//arrange
			var statusesDto = StatusFactory.BuildListStatusDto();

			// act
			var statusModel = StatusMapping.MapDtoToModel(statusesDto, 0);

			//// assert
			Assert.That(statusModel, Is.Not.Null);
		}

	}
}