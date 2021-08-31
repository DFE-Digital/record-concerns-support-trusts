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

			var trustsDto = TrustDtoFactory.CreateListTrustSummaryDto();
			
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
	}
}