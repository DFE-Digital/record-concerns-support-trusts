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
using Service.Redis.Records;
using Service.Redis.Sequence;
using Service.Redis.Status;
using Service.Redis.Trusts;
using Service.Redis.Type;
using Service.TRAMS.Cases;
using Service.TRAMS.Status;
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
		public async Task WhenGetCasesByCaseworker_FetchFromTramsApi_ReturnsCases()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockCachedService = new Mock<ICachedService>();
			var mockSequenceCachedService = new Mock<ISequenceCachedService>();
			var mockRecordRatingHistoryCachedService = new Mock<IRecordRatingHistoryCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();

			var casesDto = CaseFactory.BuildListCaseDto();
			var recordsDto = RecordFactory.BuildListRecordDto();
			var statusLiveDto = StatusFactory.BuildStatusDto(Status.Live.ToString(), 1);
			var statusMonitoringDto = StatusFactory.BuildStatusDto(Status.Monitoring.ToString(), 2);
			var ratingsDto = RatingFactory.BuildListRatingDto();
			var typesDto = TypeFactory.BuildListTypeDto();
			var trustDto = TrustFactory.BuildTrustDetailsDto();
			
			mockStatusCachedService.SetupSequence(s => s.GetStatusByName(It.IsAny<string>())).ReturnsAsync(statusLiveDto).ReturnsAsync(statusMonitoringDto);
			mockRecordCachedService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<CaseDto>())).ReturnsAsync(recordsDto);
			mockRatingCachedService.Setup(r => r.GetRatings()).ReturnsAsync(ratingsDto);
			mockTypeCachedService.Setup(t => t.GetTypes()).ReturnsAsync(typesDto);
			mockTrustCachedService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>())).ReturnsAsync(trustDto);
			mockCaseCachedService.Setup(cs => cs.GetCasesByCaseworker(It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(casesDto);
			
			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object, mockCachedService.Object,  mockRecordRatingHistoryCachedService.Object, 
				mockStatusCachedService.Object, mockSequenceCachedService.Object, mockLogger.Object);
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
					var establishmentNames = string.Join(",", trustDto.Establishments.Select(e => e.EstablishmentName));
					var recordDto = recordsDto.FirstOrDefault(r => r.CaseUrn.CompareTo(actual.Urn) == 0);
					Assert.IsNotNull(recordDto);

					var caseType = typesDto.FirstOrDefault(t => t.Urn.CompareTo(recordDto.TypeUrn) == 0);
					Assert.IsNotNull(caseType);
					
					var rating = ratingsDto.Where(r => r.Urn.CompareTo(recordDto.RatingUrn) == 0)
						.Select(r => r.Name)
						.First();
					Assert.IsNotNull(rating);
					
					Assert.That(expected.Created, Is.EqualTo(actual.CreatedAt.ToString("dd-MM-yyyy")));
					Assert.That(expected.Updated, Is.EqualTo(actual.UpdatedAt.ToString("dd-MM-yyyy")));
					Assert.That(expected.AcademyNames, Is.EqualTo(establishmentNames));
					Assert.That(expected.CaseType, Is.EqualTo(caseType.Name));
					Assert.That(expected.CaseSubType, Is.EqualTo(caseType.Description));
					Assert.That(expected.CaseUrn, Is.EqualTo(actual.Urn.ToString()));
					Assert.That(expected.TrustName, Is.EqualTo(HomeMapping.FetchTrustName(new List<TrustDetailsDto>{ trustDto }, actual)));
					Assert.That(expected.RagRating, Is.EqualTo(HomeMapping.FetchRag(rating)));
					Assert.That(expected.RagRatingCss, Is.EqualTo(HomeMapping.FetchRagCss(rating)));
				}
			}
		}
		
		[Test]
		public async Task WhenGetCasesByCaseworker_FetchFromTramsApi_ThrowsException_ReturnEmptyCases()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockCachedService = new Mock<ICachedService>();
			var mockSequenceCachedService = new Mock<ISequenceCachedService>();
			var mockRecordRatingHistoryCachedService = new Mock<IRecordRatingHistoryCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			
			var casesDto = CaseFactory.BuildListCaseDto();
			var statusLiveDto = StatusFactory.BuildStatusDto(Status.Live.ToString(), 1);
			var statusMonitoringDto = StatusFactory.BuildStatusDto(Status.Monitoring.ToString(), 2);
			
			mockCaseCachedService.Setup(cs => cs.GetCasesByCaseworker(It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(casesDto);
			mockStatusCachedService.SetupSequence(s => s.GetStatusByName(It.IsAny<string>())).ReturnsAsync(statusLiveDto).ReturnsAsync(statusMonitoringDto);
			mockCaseCachedService.Setup(cs => cs.GetCasesByCaseworker(It.IsAny<string>(), It.IsAny<string>()))
				.ThrowsAsync(new Exception());

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object, mockCachedService.Object,  mockRecordRatingHistoryCachedService.Object, 
				mockStatusCachedService.Object, mockSequenceCachedService.Object, mockLogger.Object);
			(IList<HomeModel> activeCasesModel, IList<HomeModel> monitoringCasesModel) = await caseModelService.GetCasesByCaseworker(It.IsAny<string>());

			// assert
			Assert.IsAssignableFrom<HomeModel[]>(activeCasesModel);
			Assert.IsAssignableFrom<HomeModel[]>(monitoringCasesModel);
			Assert.That(activeCasesModel.Count, Is.EqualTo(0));
			Assert.That(monitoringCasesModel.Count, Is.EqualTo(0));
		}
	}
}