using AutoMapper;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Shared.Tests.Factory;
using NUnit.Framework;

namespace ConcernsCaseWork.Tests.Models
{
	[Parallelizable(ParallelScope.All)]
	public class TrustDetailsModelTests
	{
		[Test]
		public void Calculate_Trust_Pupils_Capacity_Percentage()
		{
			// arrange
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();
			var trustDetailsDto = TrustFactory.BuildTrustDetailsDto();

			double expectedTotalNumberOfPupils = 100;
			double expectedTotalSchoolCapacity = 1000;
			double expectedPupilCapacityPercentage = 10;

			// act
			var trustDetailsModel = mapper.Map<TrustDetailsModel>(trustDetailsDto);

			// assert
			Assert.That(trustDetailsModel.TotalPupils, Is.EqualTo(expectedTotalNumberOfPupils));
			Assert.That(trustDetailsModel.TotalPupilCapacity, Is.EqualTo(expectedTotalSchoolCapacity));
			Assert.That(trustDetailsModel.PupilCapacityPercentage, Is.EqualTo(expectedPupilCapacityPercentage));
		}

	}
}
