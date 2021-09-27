using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Base;
using Service.Redis.Cases;
using Service.Redis.Models;
using Service.Redis.Rating;
using Service.Redis.RecordRatingHistory;
using Service.Redis.Records;
using Service.Redis.Sequence;
using Service.Redis.Status;
using Service.Redis.Trusts;
using Service.Redis.Type;
using Service.TRAMS.Cases;
using Service.TRAMS.RecordRatingHistory;
using Service.TRAMS.Records;
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
			var statusLiveDto = StatusFactory.BuildStatusDto(StatusEnum.Live.ToString(), 1);
			var statusMonitoringDto = StatusFactory.BuildStatusDto(StatusEnum.Monitoring.ToString(), 2);
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
					Assert.That(expected.RagRating, Is.EqualTo(RagMapping.FetchRag(rating)));
					Assert.That(expected.RagRatingCss, Is.EqualTo(RagMapping.FetchRagCss(rating)));
				}
			}
		}
		
		[Test]
		public async Task WhenGetCasesByCaseworker_FetchFromTramsApi_ReturnEmptyCases()
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
			var statusLiveDto = StatusFactory.BuildStatusDto(StatusEnum.Live.ToString(), 1);
			var statusMonitoringDto = StatusFactory.BuildStatusDto(StatusEnum.Monitoring.ToString(), 2);
			
			mockCaseCachedService.Setup(cs => cs.GetCasesByCaseworker(It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(casesDto);
			mockStatusCachedService.SetupSequence(s => s.GetStatusByName(It.IsAny<string>())).ReturnsAsync(statusLiveDto).ReturnsAsync(statusMonitoringDto);
			mockCaseCachedService.Setup(cs => cs.GetCasesByCaseworker(It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(new List<CaseDto>());

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
			var statusLiveDto = StatusFactory.BuildStatusDto(StatusEnum.Live.ToString(), 1);
			var statusMonitoringDto = StatusFactory.BuildStatusDto(StatusEnum.Monitoring.ToString(), 2);
			
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
		
		[Test]
		public async Task WhenGetCasesByCaseworker_FetchFromCache_ReturnsCases()
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
			var statusLiveDto = StatusFactory.BuildStatusDto(StatusEnum.Live.ToString(), 1);
			var statusMonitoringDto = StatusFactory.BuildStatusDto(StatusEnum.Monitoring.ToString(), 2);
			var ratingsDto = RatingFactory.BuildListRatingDto();
			var typesDto = TypeFactory.BuildListTypeDto();
			var trustDto = TrustFactory.BuildTrustDetailsDto();
			
			mockStatusCachedService.SetupSequence(s => s.GetStatusByName(It.IsAny<string>())).ReturnsAsync(statusLiveDto).ReturnsAsync(statusMonitoringDto);
			mockRecordCachedService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<CaseDto>())).ReturnsAsync(recordsDto);
			mockRatingCachedService.Setup(r => r.GetRatings()).ReturnsAsync(ratingsDto);
			mockTypeCachedService.Setup(t => t.GetTypes()).ReturnsAsync(typesDto);
			mockTrustCachedService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>())).ReturnsAsync(trustDto);
			mockCaseCachedService.Setup(cs => cs.GetCasesByCaseworker(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(casesDto);

			var userState = new UserState
			{
				TrustUkPrn = "trust-ukprn",
				CasesDetails =
				{
					{ 1, new CaseWrapper { 
						CaseDto = CaseFactory.BuildCaseDto(), 
						Records =
						{
							{ 1, new RecordWrapper
							{
								RecordDto = RecordFactory.BuildRecordDto(), 
								RecordsRatingHistory = new List<RecordRatingHistoryDto>
								{
									new RecordRatingHistoryDto(DateTimeOffset.Now, 1, 1)
								}
							} }
						}
					} }
				}
			};
			mockCachedService.Setup(c => c.GetData<UserState>(It.IsAny<string>())).ReturnsAsync(userState);
			
			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object, mockCachedService.Object,  mockRecordRatingHistoryCachedService.Object, 
				mockStatusCachedService.Object, mockSequenceCachedService.Object, mockLogger.Object);
			(IList<HomeModel> activeCasesModel, IList<HomeModel> monitoringCasesModel) = await caseModelService.GetCasesByCaseworker(It.IsAny<string>());

			// assert
			Assert.IsAssignableFrom<List<HomeModel>>(activeCasesModel);
			Assert.IsAssignableFrom<List<HomeModel>>(monitoringCasesModel);
			Assert.That(activeCasesModel.Count, Is.EqualTo(1));
			Assert.That(monitoringCasesModel.Count, Is.EqualTo(0));
			
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
					Assert.That(expected.RagRating, Is.EqualTo(RagMapping.FetchRag(rating)));
					Assert.That(expected.RagRatingCss, Is.EqualTo(RagMapping.FetchRagCss(rating)));
				}
			}
		}
		
		[Test]
		public async Task WhenPostCase_ReturnsCaseModel()
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

			var caseDto = CaseFactory.BuildCaseDto();
			var recordDto = RecordFactory.BuildRecordDto();
			var statusLiveDto = StatusFactory.BuildStatusDto(StatusEnum.Live.ToString(), 1);
			var statusMonitoringDto = StatusFactory.BuildStatusDto(StatusEnum.Monitoring.ToString(), 2);
			var ratingDto = RatingFactory.BuildRatingDto();
			var typeDto = TypeFactory.BuildTypeDto();
			var createCaseModel = CaseFactory.BuildCreateCaseModel();
			var recordRatingHistoryDto = RecordRatingHistoryFactory.BuildRecordRatingHistoryDto();
			
			mockSequenceCachedService.Setup(s => s.Generator()).ReturnsAsync(1);
			mockStatusCachedService.SetupSequence(s => s.GetStatusByName(It.IsAny<string>())).ReturnsAsync(statusLiveDto).ReturnsAsync(statusMonitoringDto);
			mockTypeCachedService.Setup(t => t.GetTypeByNameAndDescription(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(typeDto);
			mockRatingCachedService.Setup(r => r.GetRatingByName(It.IsAny<string>())).ReturnsAsync(ratingDto);
			mockCaseCachedService.Setup(c => c.IsCasePrimary(It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(true);
			mockCaseCachedService.Setup(cs => cs.PostCase(It.IsAny<CreateCaseDto>())).ReturnsAsync(caseDto);
			mockRecordCachedService.Setup(r => r.PostRecordByCaseUrn(It.IsAny<CreateRecordDto>(), It.IsAny<string>())).ReturnsAsync(recordDto);
			mockRecordRatingHistoryCachedService.Setup(r => r.PostRecordRatingHistory(It.IsAny<RecordRatingHistoryDto>(), It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(recordRatingHistoryDto);
			
			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object, mockCachedService.Object,  mockRecordRatingHistoryCachedService.Object, 
				mockStatusCachedService.Object, mockSequenceCachedService.Object, mockLogger.Object);
			
			var actualCaseModel = await caseModelService.PostCase(createCaseModel);

			// assert
			Assert.IsAssignableFrom<CaseModel>(actualCaseModel);
			Assert.That(actualCaseModel, Is.Not.Null);
			Assert.That(actualCaseModel.CreatedAt, Is.EqualTo(caseDto.CreatedAt));
			Assert.That(actualCaseModel.UpdatedAt, Is.EqualTo(caseDto.UpdatedAt));
			Assert.That(actualCaseModel.ReviewAt, Is.EqualTo(caseDto.ReviewAt));
			Assert.That(actualCaseModel.ClosedAt, Is.EqualTo(caseDto.ClosedAt));
			Assert.That(actualCaseModel.CreatedBy, Is.EqualTo(caseDto.CreatedBy));
			Assert.That(actualCaseModel.Description, Is.EqualTo(caseDto.Description));
			Assert.That(actualCaseModel.CrmEnquiry, Is.EqualTo(caseDto.CrmEnquiry));
			Assert.That(actualCaseModel.TrustUkPrn, Is.EqualTo(caseDto.TrustUkPrn));
			Assert.That(actualCaseModel.ReasonAtReview, Is.EqualTo(caseDto.ReasonAtReview));
			Assert.That(actualCaseModel.DeEscalation, Is.EqualTo(caseDto.DeEscalation));
			Assert.That(actualCaseModel.Issue, Is.EqualTo(caseDto.Issue));
			Assert.That(actualCaseModel.CurrentStatus, Is.EqualTo(caseDto.CurrentStatus));
			Assert.That(actualCaseModel.NextSteps, Is.EqualTo(caseDto.NextSteps));
			Assert.That(actualCaseModel.CaseAim, Is.EqualTo(caseDto.CaseAim));
			Assert.That(actualCaseModel.DeEscalationPoint, Is.EqualTo(caseDto.DeEscalationPoint));
			Assert.That(actualCaseModel.DirectionOfTravel, Is.EqualTo(caseDto.DirectionOfTravel));
			Assert.That(actualCaseModel.Urn, Is.EqualTo(1));
			Assert.That(actualCaseModel.Status, Is.EqualTo(1));
			Assert.That(actualCaseModel.StatusName, Is.EqualTo(StatusEnum.Live.ToString()));
		}
		
		[Test]
		public void WhenPostCase_ThrowsException()
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
			
			var statusLiveDto = StatusFactory.BuildStatusDto(StatusEnum.Live.ToString(), 1);
			var statusMonitoringDto = StatusFactory.BuildStatusDto(StatusEnum.Monitoring.ToString(), 2);
			var ratingDto = RatingFactory.BuildRatingDto();
			var typeDto = TypeFactory.BuildTypeDto();
			var createCaseModel = CaseFactory.BuildCreateCaseModel();

			mockSequenceCachedService.Setup(s => s.Generator()).ReturnsAsync(1);
			mockStatusCachedService.SetupSequence(s => s.GetStatusByName(It.IsAny<string>())).ReturnsAsync(statusLiveDto).ReturnsAsync(statusMonitoringDto);
			mockTypeCachedService.Setup(t => t.GetTypeByNameAndDescription(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(typeDto);
			mockRatingCachedService.Setup(r => r.GetRatingByName(It.IsAny<string>())).ReturnsAsync(ratingDto);

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object, mockCachedService.Object,  mockRecordRatingHistoryCachedService.Object, 
				mockStatusCachedService.Object, mockSequenceCachedService.Object, mockLogger.Object);
			
			// assert
			Assert.ThrowsAsync<NullReferenceException>(() => caseModelService.PostCase(createCaseModel));
		}

		[Test]
		public async Task When_GetCaseByUrn_ReturnsCaseModel()
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
			
			var caseDto = CaseFactory.BuildCaseDto();

			mockCaseCachedService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseDto);
			mockTrustCachedService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(TrustFactory.BuildTrustDetailsDto());
			mockRecordCachedService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<CaseDto>()))
				.ReturnsAsync(RecordFactory.BuildListRecordDto());
			mockTypeCachedService.Setup(t => t.GetTypes()).ReturnsAsync(TypeFactory.BuildListTypeDto());
			mockRatingCachedService.Setup(r => r.GetRatings()).ReturnsAsync(RatingFactory.BuildListRatingDto());

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object, mockCachedService.Object,  mockRecordRatingHistoryCachedService.Object, 
				mockStatusCachedService.Object, mockSequenceCachedService.Object, mockLogger.Object);

			var actualCaseModel = await caseModelService.GetCaseByUrn("testing", 1);

			// assert
			Assert.IsAssignableFrom<CaseModel>(actualCaseModel);
			Assert.That(actualCaseModel, Is.Not.Null);
			Assert.That(actualCaseModel.CreatedAt, Is.EqualTo(caseDto.CreatedAt));
			Assert.That(actualCaseModel.UpdatedAt, Is.EqualTo(caseDto.UpdatedAt));
			Assert.That(actualCaseModel.ReviewAt, Is.EqualTo(caseDto.ReviewAt));
			Assert.That(actualCaseModel.ClosedAt, Is.EqualTo(caseDto.ClosedAt));
			Assert.That(actualCaseModel.CreatedBy, Is.EqualTo(caseDto.CreatedBy));
			Assert.That(actualCaseModel.Description, Is.EqualTo(caseDto.Description));
			Assert.That(actualCaseModel.CrmEnquiry, Is.EqualTo(caseDto.CrmEnquiry));
			Assert.That(actualCaseModel.TrustUkPrn, Is.EqualTo(caseDto.TrustUkPrn));
			Assert.That(actualCaseModel.ReasonAtReview, Is.EqualTo(caseDto.ReasonAtReview));
			Assert.That(actualCaseModel.DeEscalation, Is.EqualTo(caseDto.DeEscalation));
			Assert.That(actualCaseModel.Issue, Is.EqualTo(caseDto.Issue));
			Assert.That(actualCaseModel.CurrentStatus, Is.EqualTo(caseDto.CurrentStatus));
			Assert.That(actualCaseModel.NextSteps, Is.EqualTo(caseDto.NextSteps));
			Assert.That(actualCaseModel.CaseAim, Is.EqualTo(caseDto.CaseAim));
			Assert.That(actualCaseModel.DeEscalationPoint, Is.EqualTo(caseDto.DeEscalationPoint));
			Assert.That(actualCaseModel.DirectionOfTravel, Is.EqualTo(caseDto.DirectionOfTravel));
			Assert.That(actualCaseModel.Urn, Is.EqualTo(1));
			Assert.That(actualCaseModel.Status, Is.EqualTo(1));
		}
		
		[Test]
		public void When_GetCaseByUrn_ThrowsException()
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
			
			mockRecordCachedService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<CaseDto>()))
				.ReturnsAsync(RecordFactory.BuildListRecordDto());
			mockTypeCachedService.Setup(t => t.GetTypes()).ReturnsAsync(TypeFactory.BuildListTypeDto());
			mockRatingCachedService.Setup(r => r.GetRatings()).ReturnsAsync(RatingFactory.BuildListRatingDto());

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object, mockCachedService.Object,  mockRecordRatingHistoryCachedService.Object, 
				mockStatusCachedService.Object, mockSequenceCachedService.Object, mockLogger.Object);

			Assert.ThrowsAsync<NullReferenceException>(() => caseModelService.GetCaseByUrn("testing", 1));
		}
		
		[Test]
		public async Task WhenPatchConcernType_ReturnsTask()
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

			var caseDto = CaseFactory.BuildCaseDto();
			var recordsDto = RecordFactory.BuildListRecordDto();
			var typeDto = TypeFactory.BuildTypeDto();

			mockTypeCachedService.Setup(t => t.GetTypeByNameAndDescription(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(typeDto);
			mockCaseCachedService.Setup(cs => cs.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(caseDto);
			mockRecordCachedService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<CaseDto>())).ReturnsAsync(recordsDto);
			
			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object, mockCachedService.Object,  mockRecordRatingHistoryCachedService.Object, 
				mockStatusCachedService.Object, mockSequenceCachedService.Object, mockLogger.Object);
			
			await caseModelService.PatchConcernType(CaseFactory.BuildPatchCaseModel());

			// assert
			mockRecordCachedService.Verify(r => r.PatchRecordByUrn(It.IsAny<RecordDto>(), It.IsAny<string>()), Times.Once);
			mockCaseCachedService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Once);
		}
		
		[Test]
		public void WhenPatchConcernType_ThrowsException()
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

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object, mockCachedService.Object,  mockRecordRatingHistoryCachedService.Object, 
				mockStatusCachedService.Object, mockSequenceCachedService.Object, mockLogger.Object);
			
			Assert.ThrowsAsync<NullReferenceException>(() => caseModelService.PatchConcernType(CaseFactory.BuildPatchCaseModel()));

			// assert
			mockRecordCachedService.Verify(r => r.PatchRecordByUrn(It.IsAny<RecordDto>(), It.IsAny<string>()), Times.Never);
			mockCaseCachedService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Never);
		}
		
		[Test]
		public async Task WhenPatchRiskRating_ReturnsTask()
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

			var caseDto = CaseFactory.BuildCaseDto();
			var recordsDto = RecordFactory.BuildListRecordDto();
			var ratingDto = RatingFactory.BuildRatingDto();

			mockRatingCachedService.Setup(t => t.GetRatingByName(It.IsAny<string>())).ReturnsAsync(ratingDto);
			mockCaseCachedService.Setup(cs => cs.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(caseDto);
			mockRecordCachedService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<CaseDto>())).ReturnsAsync(recordsDto);
			
			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object, mockCachedService.Object,  mockRecordRatingHistoryCachedService.Object, 
				mockStatusCachedService.Object, mockSequenceCachedService.Object, mockLogger.Object);
			
			await caseModelService.PatchRiskRating(CaseFactory.BuildPatchCaseModel());

			// assert
			mockRecordCachedService.Verify(r => r.PatchRecordByUrn(It.IsAny<RecordDto>(), It.IsAny<string>()), Times.Once);
			mockCaseCachedService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Once);
			mockRecordRatingHistoryCachedService.Verify(r => 
				r.PostRecordRatingHistory(It.IsAny<RecordRatingHistoryDto>(), It.IsAny<string>(), It.IsAny<long>()), Times.Once);
		}
		
		[Test]
		public void WhenPatchRiskRating_ThrowsException()
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

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object, mockCachedService.Object,  mockRecordRatingHistoryCachedService.Object, 
				mockStatusCachedService.Object, mockSequenceCachedService.Object, mockLogger.Object);
			
			Assert.ThrowsAsync<NullReferenceException>(() => caseModelService.PatchRiskRating(CaseFactory.BuildPatchCaseModel()));

			// assert
			mockRecordCachedService.Verify(r => r.PatchRecordByUrn(It.IsAny<RecordDto>(), It.IsAny<string>()), Times.Never);
			mockCaseCachedService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Never);
			mockRecordRatingHistoryCachedService.Verify(r => 
				r.PostRecordRatingHistory(It.IsAny<RecordRatingHistoryDto>(), It.IsAny<string>(), It.IsAny<long>()), Times.Never);
		}
		
		[Test]
		public async Task WhenPatchClosure_ReturnsTask()
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

			var caseDto = CaseFactory.BuildCaseDto();
			var recordsDto = RecordFactory.BuildListRecordDto();
			var statusDto = StatusFactory.BuildStatusDto(StatusEnum.Live.ToString(), 1);

			mockStatusCachedService.Setup(t => t.GetStatusByName(It.IsAny<string>())).ReturnsAsync(statusDto);
			mockCaseCachedService.Setup(cs => cs.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(caseDto);
			mockRecordCachedService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<CaseDto>())).ReturnsAsync(recordsDto);
			
			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object, mockCachedService.Object,  mockRecordRatingHistoryCachedService.Object, 
				mockStatusCachedService.Object, mockSequenceCachedService.Object, mockLogger.Object);
			
			await caseModelService.PatchClosure(CaseFactory.BuildPatchCaseModel());

			// assert
			mockRecordCachedService.Verify(r => r.PatchRecordByUrn(It.IsAny<RecordDto>(), It.IsAny<string>()), Times.Once);
			mockCaseCachedService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Once);
		}
		
		[Test]
		public void WhenPatchClosure_ThrowsException()
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

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object, mockCachedService.Object,  mockRecordRatingHistoryCachedService.Object, 
				mockStatusCachedService.Object, mockSequenceCachedService.Object, mockLogger.Object);
			
			Assert.ThrowsAsync<ArgumentNullException>(() => caseModelService.PatchClosure(CaseFactory.BuildPatchCaseModel()));

			// assert
			mockRecordCachedService.Verify(r => r.PatchRecordByUrn(It.IsAny<RecordDto>(), It.IsAny<string>()), Times.Never);
			mockCaseCachedService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Never);
		}
		
		[Test]
		public async Task WhenPatchDirectionOfTravel_ReturnsTask()
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

			var caseDto = CaseFactory.BuildCaseDto();
			
			mockCaseCachedService.Setup(cs => cs.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(caseDto);

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object, mockCachedService.Object,  mockRecordRatingHistoryCachedService.Object, 
				mockStatusCachedService.Object, mockSequenceCachedService.Object, mockLogger.Object);
			
			await caseModelService.PatchDirectionOfTravel(CaseFactory.BuildPatchCaseModel());

			// assert
			mockCaseCachedService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Once);
		}
		
		[Test]
		public void WhenPatchDirectionOfTravel_ThrowsException()
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

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object, mockCachedService.Object,  mockRecordRatingHistoryCachedService.Object, 
				mockStatusCachedService.Object, mockSequenceCachedService.Object, mockLogger.Object);
			
			Assert.ThrowsAsync<NullReferenceException>(() => caseModelService.PatchDirectionOfTravel(CaseFactory.BuildPatchCaseModel()));

			// assert
			mockCaseCachedService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Never);
		}
	}
}