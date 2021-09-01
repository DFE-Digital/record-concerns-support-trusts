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
			var mockTrustService = new Mock<ITrustSearchService>();
			var mockLogger = new Mock<ILogger<TrustModelService>>();
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();
			var trustSummaryDto = TrustFactory.CreateListTrustSummaryDto();

			mockTrustService.Setup(ts => ts.GetTrustsBySearchCriteria(It.IsAny<TrustSearch>())).ReturnsAsync(trustSummaryDto);

			// act
			var trustModelService = new TrustModelService(mockTrustService.Object, mapper, mockLogger.Object);
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
			var mockTrustService = new Mock<ITrustSearchService>();
			var mockLogger = new Mock<ILogger<TrustModelService>>();
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();

			mockTrustService.Setup(ts => ts.GetTrustsBySearchCriteria(It.IsAny<TrustSearch>())).ReturnsAsync(Array.Empty<TrustSummaryDto>());

			// act
			var trustModelService = new TrustModelService(mockTrustService.Object, mapper, mockLogger.Object);
			var trustsSummaryModel = await trustModelService.GetTrustsBySearchCriteria(It.IsAny<TrustSearch>());

			// assert
			Assert.IsAssignableFrom<List<TrustSummaryModel>>(trustsSummaryModel.ToList());
			Assert.That(trustsSummaryModel.Count, Is.EqualTo(0));
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
	}
}