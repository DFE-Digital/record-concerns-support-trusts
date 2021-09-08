using AutoMapper;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Base;
using Service.Redis.Rating;
using Service.Redis.Type;
using Service.TRAMS.Cases;
using Service.TRAMS.Records;
using Service.TRAMS.Trusts;
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
			var mockCaseService = new Mock<ICaseService>();
			var mockTrustService = new Mock<ITrustService>();
			var mockRecordService = new Mock<IRecordService>();
			var mockRatingService = new Mock<IRatingCachedService>();
			var mockTypeService = new Mock<ITypeCachedService>();
			var mockCachedService = new Mock<ICachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();
			var casesDto = CaseDtoFactory.CreateListCaseDto();

			mockCaseService.Setup(cs => cs.GetCasesByCaseworker(It.IsAny<string>())).ReturnsAsync(casesDto);
			
			// act
			var caseModelService = new CaseModelService(mockCaseService.Object, mockTrustService.Object, mockRecordService.Object,
				mockRatingService.Object, mockTypeService.Object, mockCachedService.Object, mapper, mockLogger.Object);
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
			var mockCaseService = new Mock<ICaseService>();
			var mockTrustService = new Mock<ITrustService>();
			var mockRecordService = new Mock<IRecordService>();
			var mockRatingService = new Mock<IRatingCachedService>();
			var mockTypeService = new Mock<ITypeCachedService>();
			var mockCachedService = new Mock<ICachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();

			mockCaseService.Setup(cs => cs.GetCasesByCaseworker(It.IsAny<string>())).ThrowsAsync(new Exception());

			// act
			var caseModelService = new CaseModelService(mockCaseService.Object, mockTrustService.Object, mockRecordService.Object,
				mockRatingService.Object, mockTypeService.Object, mockCachedService.Object, mapper, mockLogger.Object);
			(IList<HomeModel> activeCasesModel, IList<HomeModel> monitoringCasesModel) = await caseModelService.GetCasesByCaseworker(It.IsAny<string>());

			// assert
			Assert.IsAssignableFrom<List<HomeModel>>(activeCasesModel);
			Assert.IsAssignableFrom<List<HomeModel>>(monitoringCasesModel);
			Assert.That(activeCasesModel.Count, Is.Empty);
			Assert.That(monitoringCasesModel.Count, Is.Empty);
		}
	}
}