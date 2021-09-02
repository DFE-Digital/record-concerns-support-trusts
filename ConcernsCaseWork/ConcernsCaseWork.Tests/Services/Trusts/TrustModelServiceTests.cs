using AutoMapper;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.TRAMS.Models;
using Service.TRAMS.Trusts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			var mockTrustService = new Mock<ITrustService>();
			var mockLogger = new Mock<ILogger<TrustModelService>>();
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();
			var trustSummaryDto = TrustFactory.CreateListTrustSummaryDto();

			mockTrustSearchService.Setup(ts => ts.GetTrustsBySearchCriteria(It.IsAny<TrustSearch>())).ReturnsAsync(trustSummaryDto);

			// act
			var trustModelService = new TrustModelService(mockTrustSearchService.Object, mockTrustService.Object, mapper, mockLogger.Object);
			var trustsSummaryModel = await trustModelService.GetTrustsBySearchCriteria(It.IsAny<TrustSearch>());

			// assert
			Assert.IsAssignableFrom<List<TrustSummaryModel>>(trustsSummaryModel);
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
					Assert.That(expected.DisplayName, Is.EqualTo(BuildDisplayName(actual)));
					
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
			var mockTrustService = new Mock<ITrustService>();
			var mockLogger = new Mock<ILogger<TrustModelService>>();
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();

			mockTrustSearchService.Setup(ts => ts.GetTrustsBySearchCriteria(It.IsAny<TrustSearch>())).ReturnsAsync(Array.Empty<TrustSummaryDto>());

			// act
			var trustModelService = new TrustModelService(mockTrustSearchService.Object, mockTrustService.Object, mapper, mockLogger.Object);
			var trustsSummaryModel = await trustModelService.GetTrustsBySearchCriteria(It.IsAny<TrustSearch>());

			// assert
			Assert.IsAssignableFrom<List<TrustSummaryModel>>(trustsSummaryModel.ToList());
			Assert.That(trustsSummaryModel.Count, Is.EqualTo(0));
		}
		
		[Test]
		public async Task WhenGetTrustByUkPrn_ReturnsTrust()
		{
			// arrange
			var mockTrustSearchService = new Mock<ITrustSearchService>();
			var mockTrustService = new Mock<ITrustService>();
			var mockLogger = new Mock<ILogger<TrustModelService>>();
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();
			var trustDetailsDto = TrustFactory.CreateTrustDetailsDto();

			mockTrustService.Setup(ts => ts.GetTrustByUkPrn(It.IsAny<string>())).ReturnsAsync(trustDetailsDto);

			// act
			var trustModelService = new TrustModelService(mockTrustSearchService.Object, mockTrustService.Object, mapper, mockLogger.Object);
			var trustsDetailsModel = await trustModelService.GetTrustByUkPrn(It.IsAny<string>());

			// assert
			Assert.IsAssignableFrom<TrustDetailsModel>(trustsDetailsModel);
			Assert.That(trustsDetailsModel, Is.Not.Null);
			Assert.That(trustsDetailsModel.GiasData, Is.Not.Null);
			Assert.That(trustsDetailsModel.GiasData.GroupId, Is.EqualTo(trustDetailsDto.GiasData.GroupId));
			Assert.That(trustsDetailsModel.GiasData.GroupName, Is.EqualTo(trustDetailsDto.GiasData.GroupName));
			Assert.That(trustsDetailsModel.GiasData.UkPrn, Is.EqualTo(trustDetailsDto.GiasData.UkPrn));
			Assert.That(trustsDetailsModel.GiasData.CompaniesHouseNumber, Is.EqualTo(trustDetailsDto.GiasData.CompaniesHouseNumber));
			Assert.That(trustsDetailsModel.GiasData.GroupContactAddress, Is.Not.Null);
			Assert.That(trustsDetailsModel.GiasData.GroupContactAddress.County, Is.EqualTo(trustDetailsDto.GiasData.GroupContactAddress.County));
			Assert.That(trustsDetailsModel.GiasData.GroupContactAddress.Locality, Is.EqualTo(trustDetailsDto.GiasData.GroupContactAddress.Locality));
			Assert.That(trustsDetailsModel.GiasData.GroupContactAddress.Postcode, Is.EqualTo(trustDetailsDto.GiasData.GroupContactAddress.Postcode));
			Assert.That(trustsDetailsModel.GiasData.GroupContactAddress.Street, Is.EqualTo(trustDetailsDto.GiasData.GroupContactAddress.Street));
			Assert.That(trustsDetailsModel.GiasData.GroupContactAddress.Town, Is.EqualTo(trustDetailsDto.GiasData.GroupContactAddress.Town));
			Assert.That(trustsDetailsModel.GiasData.GroupContactAddress.AdditionalLine, Is.EqualTo(trustDetailsDto.GiasData.GroupContactAddress.AdditionalLine));
			Assert.That(trustsDetailsModel.GiasData.GroupContactAddress.DisplayAddress, Is.EqualTo(DisplayAddress(trustDetailsDto.GiasData.GroupContactAddress)));
		}

		private static string BuildDisplayName(TrustSummaryDto trustSummaryDto)
		{
			var sb = new StringBuilder();
			sb.Append(string.IsNullOrEmpty(trustSummaryDto.GroupName) ? "-".PadRight(2) : trustSummaryDto.GroupName);
			sb.Append(",").Append(" ");
			sb.Append(string.IsNullOrEmpty(trustSummaryDto.UkPrn) ? "-".PadRight(2) : trustSummaryDto.UkPrn);
			sb.Append(",").Append(" ");
			sb.Append(string.IsNullOrEmpty(trustSummaryDto.CompaniesHouseNumber) ? "-".PadRight(2) : trustSummaryDto.CompaniesHouseNumber);
				
			return sb.ToString();
		}
		
		private static string DisplayAddress(GroupContactAddressDto groupContactAddressDto)
		{
			var sb = new StringBuilder();
			sb.Append(string.IsNullOrEmpty(groupContactAddressDto.Street) ? "-".PadRight(2) : groupContactAddressDto.Street);
			sb.Append(",").Append(" ");
			sb.Append(string.IsNullOrEmpty(groupContactAddressDto.AdditionalLine) ? "-".PadRight(2) : groupContactAddressDto.AdditionalLine);
			sb.Append(",").Append(" ");
			sb.Append(string.IsNullOrEmpty(groupContactAddressDto.Locality) ? "-".PadRight(2) : groupContactAddressDto.Locality);
			sb.Append(",").Append(" ");
			sb.Append(string.IsNullOrEmpty(groupContactAddressDto.Postcode) ? "-".PadRight(2) : groupContactAddressDto.Postcode);
				
			return sb.ToString();
		}
	}
}