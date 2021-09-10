using AutoMapper;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Base;
using Service.Redis.Cases;
using Service.Redis.Rating;
using Service.Redis.RecordRatingHistory;
using Service.Redis.Status;
using Service.Redis.Trusts;
using Service.Redis.Type;
using Service.TRAMS.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Services.Cases
{
	[Parallelizable(ParallelScope.All)]
	public class CaseModelServiceTests
	{
		[Test]
		public async Task WhenGetCasesByCaseworker_ReturnsCases()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordService = new Mock<IRecordService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockCachedService = new Mock<ICachedService>();
			var mockRecordRatingHistoryCachedService = new Mock<IRecordRatingHistoryCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();
			var casesDto = CaseDtoFactory.CreateListCaseDto();

			mockCaseCachedService.Setup(cs => cs.GetCasesByCaseworker(It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(casesDto);
			
			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, mockTrustCachedService.Object, mockRecordService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object, mockCachedService.Object,  mockRecordRatingHistoryCachedService.Object, 
				mockStatusCachedService.Object, mapper, mockLogger.Object);
			(IList<HomeModel> activeCasesModel, IList<HomeModel> monitoringCasesModel) = await caseModelService.GetCasesByCaseworker(It.IsAny<string>());

			// assert
			Assert.IsAssignableFrom<List<HomeModel>>(activeCasesModel);
			Assert.IsAssignableFrom<List<HomeModel>>(monitoringCasesModel);
			Assert.That(activeCasesModel.Count, Is.EqualTo(2));
			Assert.That(monitoringCasesModel.Count, Is.EqualTo(2));
			
			foreach (var expected in activeCasesModel)
			{
				foreach (var actual in casesDto.Where(actual => expected.CaseUrn.Equals(actual.Urn.ToString())))
				{
					Assert.That(expected.Created, Is.EqualTo(actual.CreatedAt.ToString("dd-MM-yyyy")));
					Assert.That(expected.Updated, Is.EqualTo(actual.UpdateAt.ToString("dd-MM-yyyy")));
					Assert.That(expected.AcademyNames, Is.Empty);
					Assert.That(expected.CaseType, Is.Empty);
					Assert.That(expected.CaseUrn, Is.EqualTo(actual.Urn));
					Assert.That(expected.RagRating, Is.Empty);
					Assert.That(expected.TrustName, Is.Empty);
					Assert.That(expected.CaseSubType, Is.Empty);
					Assert.That(expected.RagRatingCss, Is.Empty);
				}
			}
		}
		
		[Test]
		public async Task WhenGetCasesByCaseworker_ThrowsException_ReturnEmptyCases()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordService = new Mock<IRecordService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockCachedService = new Mock<ICachedService>();
			var mockRecordRatingHistoryCachedService = new Mock<IRecordRatingHistoryCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();

			mockCaseCachedService.Setup(cs => cs.GetCasesByCaseworker(It.IsAny<string>(), It.IsAny<string>()))
				.ThrowsAsync(new Exception());

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, mockTrustCachedService.Object, mockRecordService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object, mockCachedService.Object,  mockRecordRatingHistoryCachedService.Object, 
				mockStatusCachedService.Object, mapper, mockLogger.Object);
			(IList<HomeModel> activeCasesModel, IList<HomeModel> monitoringCasesModel) = await caseModelService.GetCasesByCaseworker(It.IsAny<string>());

			// assert
			Assert.IsAssignableFrom<List<HomeModel>>(activeCasesModel);
			Assert.IsAssignableFrom<List<HomeModel>>(monitoringCasesModel);
			Assert.That(activeCasesModel.Count, Is.Empty);
			Assert.That(monitoringCasesModel.Count, Is.Empty);
		}
	}
}