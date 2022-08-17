using AutoMapper;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Shared.Tests.Factory;
using ConcernsCaseWork.Shared.Tests.Shared;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Trusts;
using ConcernsCasework.Service.Trusts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Services.Trusts
{
	[Parallelizable(ParallelScope.All)]
	public class TrustModelServiceTests
	{
		[Test]
		public async Task WhenGetTrustsBySearchCriteria_ReturnsTrusts()
		{
			// arrange
			var mockTrustSearchService = new Mock<ITrustSearchService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockLogger = new Mock<ILogger<TrustModelService>>();
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();
			var trustSummaryDto = TrustFactory.BuildListTrustSummaryDto();

			mockTrustSearchService.Setup(ts => ts.GetTrustsBySearchCriteria(It.IsAny<TrustSearch>())).ReturnsAsync(trustSummaryDto);

			// act
			var trustModelService = new TrustModelService(mockTrustSearchService.Object, mockTrustCachedService.Object, mapper, mockLogger.Object);
			var trustsSummaryModel = await trustModelService.GetTrustsBySearchCriteria(It.IsAny<TrustSearch>());

			// assert
			Assert.IsAssignableFrom<List<TrustSearchModel>>(trustsSummaryModel);
			Assert.That(trustsSummaryModel.Count, Is.EqualTo(trustSummaryDto.Count));
			foreach (var expected in trustsSummaryModel)
			{
				foreach (var actual in trustSummaryDto.Where(actual => expected.UkPrn.Equals(actual.UkPrn)))
				{
					Assert.That(expected.Establishments.Count, Is.EqualTo(actual.Establishments.Count));
					Assert.That(expected.Urn, Is.EqualTo(actual.Urn));
					Assert.That(expected.UkPrn, Is.EqualTo(actual.UkPrn));
					Assert.That(expected.GroupName, Is.EqualTo(actual.GroupName));
					Assert.That(expected.CompaniesHouseNumber, Is.EqualTo(actual.CompaniesHouseNumber));
					Assert.That(expected.DisplayName, Is.EqualTo(SharedBuilder.BuildDisplayName(actual)));
					Assert.That(expected.TrustType, Is.EqualTo(actual.TrustType));
					
					Assert.IsAssignableFrom<GroupContactAddressModel>(expected.GroupContactAddress);
					Assert.That(expected.GroupContactAddress, Is.Not.Null);
					Assert.That(expected.GroupContactAddress.County, Is.EqualTo(actual.GroupContactAddress.County));
					Assert.That(expected.GroupContactAddress.Locality, Is.EqualTo(actual.GroupContactAddress.Locality));
					Assert.That(expected.GroupContactAddress.Postcode, Is.EqualTo(actual.GroupContactAddress.Postcode));
					Assert.That(expected.GroupContactAddress.Street, Is.EqualTo(actual.GroupContactAddress.Street));
					Assert.That(expected.GroupContactAddress.Town, Is.EqualTo(actual.GroupContactAddress.Town));
					Assert.That(expected.GroupContactAddress.AdditionalLine, Is.EqualTo(actual.GroupContactAddress.AdditionalLine));
					Assert.That(expected.GroupContactAddress.DisplayAddress, Is.EqualTo(SharedBuilder.BuildDisplayAddress(actual.GroupContactAddress)));
					
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
		public async Task WhenGetTrustsBySearchCriteria_ThrowsException_ReturnEmptyTrusts()
		{
			// arrange
			var mockTrustSearchService = new Mock<ITrustSearchService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockLogger = new Mock<ILogger<TrustModelService>>();
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();

			mockTrustSearchService.Setup(ts => ts.GetTrustsBySearchCriteria(It.IsAny<TrustSearch>())).ReturnsAsync(Array.Empty<TrustSearchDto>());

			// act
			var trustModelService = new TrustModelService(mockTrustSearchService.Object, mockTrustCachedService.Object, mapper, mockLogger.Object);
			var trustsSummaryModel = await trustModelService.GetTrustsBySearchCriteria(It.IsAny<TrustSearch>());

			// assert
			Assert.IsAssignableFrom<List<TrustSearchModel>>(trustsSummaryModel.ToList());
			Assert.That(trustsSummaryModel.Count, Is.EqualTo(0));
		}
		
		[Test]
		public async Task WhenGetTrustByUkPrn_ReturnsTrust()
		{
			// arrange
			var mockTrustSearchService = new Mock<ITrustSearchService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockLogger = new Mock<ILogger<TrustModelService>>();
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();
			var trustDetailsDto = TrustFactory.BuildTrustDetailsDto();

			mockTrustCachedService.Setup(ts => ts.GetTrustByUkPrn(It.IsAny<string>())).ReturnsAsync(trustDetailsDto);

			// act
			var trustModelService = new TrustModelService(mockTrustSearchService.Object, mockTrustCachedService.Object, mapper, mockLogger.Object);
			var trustsDetailsModel = await trustModelService.GetTrustByUkPrn(It.IsAny<string>());

			// assert
			Assert.IsAssignableFrom<TrustDetailsModel>(trustsDetailsModel);
			Assert.That(trustsDetailsModel, Is.Not.Null);
			Assert.That(trustsDetailsModel.GiasData, Is.Not.Null);
			Assert.That(trustsDetailsModel.GiasData.GroupId, Is.EqualTo(trustDetailsDto.GiasData.GroupId));
			Assert.That(trustsDetailsModel.GiasData.GroupName, Is.EqualTo(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(trustDetailsDto.GiasData.GroupName)));
			Assert.That(trustsDetailsModel.GiasData.GroupTypeCode, Is.EqualTo(trustsDetailsModel.GiasData.GroupTypeCode));
			Assert.That(trustsDetailsModel.GiasData.UkPrn, Is.EqualTo(trustDetailsDto.GiasData.UkPrn));
			Assert.That(trustsDetailsModel.GiasData.CompaniesHouseNumber, Is.EqualTo(trustDetailsDto.GiasData.CompaniesHouseNumber));
			Assert.That(trustsDetailsModel.GiasData.GroupContactAddress, Is.Not.Null);
			Assert.That(trustsDetailsModel.GiasData.GroupContactAddress.County, Is.EqualTo(trustDetailsDto.GiasData.GroupContactAddress.County));
			Assert.That(trustsDetailsModel.GiasData.GroupContactAddress.Locality, Is.EqualTo(trustDetailsDto.GiasData.GroupContactAddress.Locality));
			Assert.That(trustsDetailsModel.GiasData.GroupContactAddress.Postcode, Is.EqualTo(trustDetailsDto.GiasData.GroupContactAddress.Postcode));
			Assert.That(trustsDetailsModel.GiasData.GroupContactAddress.Street, Is.EqualTo(trustDetailsDto.GiasData.GroupContactAddress.Street));
			Assert.That(trustsDetailsModel.GiasData.GroupContactAddress.Town, Is.EqualTo(trustDetailsDto.GiasData.GroupContactAddress.Town));
			Assert.That(trustsDetailsModel.GiasData.GroupContactAddress.AdditionalLine, Is.EqualTo(trustDetailsDto.GiasData.GroupContactAddress.AdditionalLine));
			Assert.That(trustsDetailsModel.GiasData.GroupContactAddress.DisplayAddress, Is.EqualTo(SharedBuilder.BuildDisplayAddress(trustDetailsDto.GiasData.GroupContactAddress)));
			
			// Ifd Data
			Assert.IsAssignableFrom<IfdDataModel>(trustsDetailsModel.IfdData);
			Assert.That(trustsDetailsModel.IfdData, Is.Not.Null);
			Assert.That(trustsDetailsModel.IfdData.TrustType, Is.EqualTo(trustDetailsDto.IfdData.TrustType));
			Assert.IsAssignableFrom<GroupContactAddressModel>(trustsDetailsModel.IfdData.GroupContactAddress);
			Assert.That(trustsDetailsModel.IfdData.GroupContactAddress, Is.Not.Null);
			Assert.That(trustsDetailsModel.IfdData.GroupContactAddress.County, Is.EqualTo(trustDetailsDto.IfdData.GroupContactAddress.County));
			Assert.That(trustsDetailsModel.IfdData.GroupContactAddress.Locality, Is.EqualTo(trustDetailsDto.IfdData.GroupContactAddress.Locality));
			Assert.That(trustsDetailsModel.IfdData.GroupContactAddress.Postcode, Is.EqualTo(trustDetailsDto.IfdData.GroupContactAddress.Postcode));
			Assert.That(trustsDetailsModel.IfdData.GroupContactAddress.Street, Is.EqualTo(trustDetailsDto.IfdData.GroupContactAddress.Street));
			Assert.That(trustsDetailsModel.IfdData.GroupContactAddress.Town, Is.EqualTo(trustDetailsDto.IfdData.GroupContactAddress.Town));
			Assert.That(trustsDetailsModel.IfdData.GroupContactAddress.AdditionalLine, Is.EqualTo(trustDetailsDto.IfdData.GroupContactAddress.AdditionalLine));
			Assert.That(trustsDetailsModel.IfdData.GroupContactAddress.DisplayAddress, Is.EqualTo(SharedBuilder.BuildDisplayAddress(trustDetailsDto.IfdData.GroupContactAddress)));
			Assert.That(trustsDetailsModel.IfdData.TrustContactPhoneNumber, Is.EqualTo(trustDetailsDto.IfdData.TrustContactPhoneNumber));

			// Establisment
			Assert.IsAssignableFrom<List<EstablishmentModel>>(trustsDetailsModel.Establishments);
			Assert.That(trustsDetailsModel.Establishments, Is.Not.Null);
			Assert.That(trustsDetailsModel.Establishments.Count, Is.EqualTo(1));
			var establishment = trustsDetailsModel.Establishments.First();
			var establishmentExpected = trustDetailsDto.Establishments.First();
			Assert.That(establishment.Urn, Is.EqualTo(establishmentExpected.Urn));
			Assert.That(establishment.EstablishmentName, Is.EqualTo(establishmentExpected.EstablishmentName));
			Assert.That(establishment.EstablishmentNumber, Is.EqualTo(establishmentExpected.EstablishmentNumber));
			Assert.That(establishment.LocalAuthorityCode, Is.EqualTo(establishmentExpected.LocalAuthorityCode));
			Assert.That(establishment.LocalAuthorityName, Is.EqualTo(establishmentExpected.LocalAuthorityName));
		}
	}
}