using AutoMapper;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Shared.Tests.Factory;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Tests.Mappers
{
	[Parallelizable(ParallelScope.All)]
	public class AutoMapperTrustsTests
	{
		[Test]
		public void ConvertFromTrustsSummaryDtoToTrustsSummaryModelIsValid()
		{
			// arrange
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();

			var trustsDto = TrustFactory.BuildListTrustSummaryDto();
			
			// act
			var trustsModel = mapper.Map<IList<TrustSummaryModel>>(trustsDto);
			
			// assert
			Assert.IsAssignableFrom<List<TrustSummaryModel>>(trustsModel);
			Assert.That(trustsModel.Count, Is.EqualTo(trustsDto.Count));
			foreach (var expected in trustsModel)
			{
				foreach (var actual in trustsDto.Where(actual => expected.UkPrn.Equals(actual.UkPrn)))
				{
					Assert.That(expected.Establishments.Count, Is.EqualTo(actual.Establishments.Count));
					Assert.That(expected.Urn, Is.EqualTo(actual.Urn));
					Assert.That(expected.UkPrn, Is.EqualTo(actual.UkPrn));
					Assert.That(expected.GroupName, Is.EqualTo(actual.GroupName));
					Assert.That(expected.CompaniesHouseNumber, Is.EqualTo(actual.CompaniesHouseNumber));
					
					foreach (var establishment in actual.Establishments)
					{
						foreach (var expectedEstablishment in expected.Establishments)
						{
							Assert.That(establishment.Name, Is.EqualTo(expectedEstablishment.Name));
							Assert.That(establishment.Urn, Is.EqualTo(expectedEstablishment.Urn));
							Assert.That(establishment.UkPrn, Is.EqualTo(expectedEstablishment.UkPrn));
						}
					}
				}
			}
		}
		
		[Test]
		public void ConvertFromTrustDetailsDtoToTrustsDetailsModelIsValid()
		{
			// arrange
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();

			var trustDetailsDto = TrustFactory.BuildTrustDetailsDto();
			
			// act
			var trustDetailsModel = mapper.Map<TrustDetailsModel>(trustDetailsDto);
			
			// assert
			Assert.IsAssignableFrom<TrustDetailsModel>(trustDetailsModel);
			Assert.That(trustDetailsModel.GiasData, Is.Not.Null);
			Assert.That(trustDetailsModel.GiasData.GroupId, Is.EqualTo(trustDetailsDto.GiasData.GroupId));
			Assert.That(trustDetailsModel.GiasData.GroupName, Is.EqualTo(trustDetailsDto.GiasData.GroupName));
			Assert.That(trustDetailsModel.GiasData.UkPrn, Is.EqualTo(trustDetailsDto.GiasData.UkPrn));
			Assert.That(trustDetailsModel.GiasData.CompaniesHouseNumber, Is.EqualTo(trustDetailsDto.GiasData.CompaniesHouseNumber));
			Assert.That(trustDetailsModel.GiasData.GroupContactAddress, Is.Not.Null);
			Assert.That(trustDetailsModel.GiasData.GroupContactAddress.County, Is.EqualTo(trustDetailsDto.GiasData.GroupContactAddress.County));
			Assert.That(trustDetailsModel.GiasData.GroupContactAddress.Locality, Is.EqualTo(trustDetailsDto.GiasData.GroupContactAddress.Locality));
			Assert.That(trustDetailsModel.GiasData.GroupContactAddress.Postcode, Is.EqualTo(trustDetailsDto.GiasData.GroupContactAddress.Postcode));
			Assert.That(trustDetailsModel.GiasData.GroupContactAddress.Street, Is.EqualTo(trustDetailsDto.GiasData.GroupContactAddress.Street));
			Assert.That(trustDetailsModel.GiasData.GroupContactAddress.Town, Is.EqualTo(trustDetailsDto.GiasData.GroupContactAddress.Town));
			Assert.That(trustDetailsModel.GiasData.GroupContactAddress.AdditionalLine, Is.EqualTo(trustDetailsDto.GiasData.GroupContactAddress.AdditionalLine));
		}
	}
}