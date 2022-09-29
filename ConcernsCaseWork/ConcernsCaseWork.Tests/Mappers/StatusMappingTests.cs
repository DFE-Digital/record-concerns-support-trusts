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
			var statusModel = StatusMapping.MapDtoToModel(statusesDto, statusDto.Id);

			// assert
			Assert.That(statusModel, Is.Not.Null);
			Assert.That(statusModel.Urn, Is.EqualTo(statusDto.Id));
			Assert.That(statusModel.Name, Is.EqualTo(statusDto.Name));
		}

		[Test]
		public void WhenMapDtoToModel_When_Urn_IsUknown_ReturnsStatusModel()
		{
			//arrange
			var statusesDto = StatusFactory.BuildListStatusDto();
			var firstStatusDto = statusesDto.First();

			// act
			var statusModel = StatusMapping.MapDtoToModel(statusesDto, 0);

			// assert
			Assert.That(statusModel, Is.Not.Null);
			Assert.That(statusModel.Urn, Is.EqualTo(firstStatusDto.Id));
			Assert.That(statusModel.Name, Is.EqualTo(firstStatusDto.Name));
		}

	}
}