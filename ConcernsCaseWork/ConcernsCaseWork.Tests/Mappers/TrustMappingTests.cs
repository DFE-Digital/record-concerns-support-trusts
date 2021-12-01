using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Shared.Tests.Factory;
using NUnit.Framework;
using Service.TRAMS.Trusts;

namespace ConcernsCaseWork.Tests.Mappers
{
	[Parallelizable(ParallelScope.All)]
	public class TrustMappingTests
	{
		[Test]
		public void WhenFetchTrustName_Return_TrustName()
		{
			// arrange
			var trustDetailsDto = TrustFactory.BuildTrustDetailsDto();
			
			// act
			var actualTrustName = TrustMapping.FetchTrustName(trustDetailsDto);
			
			// assert
			Assert.That(actualTrustName, Is.EqualTo(trustDetailsDto.GiasData.GroupName));
		}
		
		[Test]
		public void WhenFetchTrustName_When_GiasIsNull_Return_Default_TrustName()
		{
			// arrange
			var trustDetailsDto = new TrustDetailsDto(
				null,
				IfdDataFactory.BuildIfdDataDto(),
				EstablishmentFactory.BuildListEstablishmentDto());
			
			// act
			var actualTrustName = TrustMapping.FetchTrustName(trustDetailsDto);
			
			// assert
			Assert.That(actualTrustName, Is.EqualTo("-"));
		}

		[Test]
		public void WhenFetchAcademies_Return_Academies()
		{
			// arrange
			var trustDetailsDto = TrustFactory.BuildTrustDetailsDto();
			
			// act
			var actualTrustName = TrustMapping.FetchAcademies(trustDetailsDto);
			
			// assert
			Assert.That(actualTrustName, Is.Not.Null);
		}
		
		[Test]
		public void WhenFetchAcademies_When_AcademiesIsNull_Return_Academies()
		{
			// arrange
			var trustDetailsDto = new TrustDetailsDto(null, null, null);
			
			// act
			var actualTrustName = TrustMapping.FetchAcademies(trustDetailsDto);
			
			// assert
			Assert.That(actualTrustName, Is.Empty);
		}
	}
}