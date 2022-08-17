using ConcernsCaseWork.Extensions;
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
using Service.Redis.Ratings;
using Service.Redis.Records;
using Service.Redis.Status;
using Service.Redis.Trusts;
using Service.Redis.Types;
using ConcernsCasework.Service.Cases;
using ConcernsCasework.Service.Records;
using ConcernsCasework.Service.Status;
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
		public async Task WhenGetCasesByCaseworkerAndStatus_GroupCaseWorkers_Return_LiveCases()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();

			const string trustUkPrn = "trust-ukprn";
			
			// cases
			var casesDto = CaseFactory.BuildListCaseDto(trustUkPrn);
			var casesDtoLive = casesDto.Where(c => c.StatusUrn.CompareTo(1) == 0).ToList();
			
			// records
			var recordsDtoLiveCases = casesDtoLive.SelectMany(c => RecordFactory.BuildListRecordDtoByCaseUrn(c.Urn)).ToList();
			
			// status
			var statusLiveDto = StatusFactory.BuildStatusDto(StatusEnum.Live.ToString(), casesDtoLive.First().Urn);
			
			var ratingsDto = RatingFactory.BuildListRatingDto();
			var typesDto = TypeFactory.BuildListTypeDto();
			var trustDto = TrustFactory.BuildTrustDetailsDto();
			var statusesDto = StatusFactory.BuildListStatusDto();

			mockStatusCachedService.Setup(s => s.GetStatusByName(It.IsAny<string>()))
				.ReturnsAsync(statusLiveDto);
			mockStatusCachedService.Setup(s => s.GetStatuses())
				.ReturnsAsync(statusesDto);
			mockCaseCachedService.Setup(cs => cs.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(casesDtoLive);
			mockRatingCachedService.Setup(r => r.GetRatings())
				.ReturnsAsync(ratingsDto);
			mockTypeCachedService.Setup(t => t.GetTypes())
				.ReturnsAsync(typesDto);
			mockTrustCachedService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDto);
			mockRecordCachedService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(recordsDtoLiveCases);

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);

			var liveCasesModel = await caseModelService.GetCasesByCaseworkerAndStatus(new[] {"userA", "userB"}, StatusEnum.Live);

			// assert
			Assert.IsAssignableFrom<List<HomeModel>>(liveCasesModel);
			Assert.That(liveCasesModel.Count, Is.EqualTo(4));

			foreach (var expected in liveCasesModel)
			{
				foreach (var actual in casesDto.Where(actual => expected.CaseUrn.Equals(actual.Urn.ToString())))
				{
					Assert.That(expected.Closed, Is.EqualTo(actual.ClosedAt.ToString("dd-MM-yyyy")));
					Assert.That(expected.Created, Is.EqualTo(actual.CreatedAt.ToString("dd-MM-yyyy")));
					Assert.That(expected.Review, Is.EqualTo(actual.ReviewAt.ToString("dd-MM-yyyy")));
					Assert.That(expected.Updated, Is.EqualTo(actual.UpdatedAt.ToString("dd-MM-yyyy")));
					Assert.That(expected.ClosedUnixTime, Is.EqualTo(actual.ClosedAt.ToUnixTimeMilliseconds()));
					Assert.That(expected.CreatedUnixTime, Is.EqualTo(actual.CreatedAt.ToUnixTimeMilliseconds()));
					Assert.That(expected.ReviewUnixTime, Is.EqualTo(actual.ReviewAt.ToUnixTimeMilliseconds()));
					Assert.That(expected.UpdatedUnixTime, Is.EqualTo(actual.UpdatedAt.ToUnixTimeMilliseconds()));
					
					Assert.That(expected.CreatedBy, Is.EqualTo(actual.CreatedBy));
					Assert.That(expected.CaseUrn, Is.EqualTo(actual.Urn.ToString()));
					Assert.That(expected.TrustName, Is.EqualTo(TrustMapping.FetchTrustName(trustDto)));
					Assert.That(expected.TrustNameTitle, Is.EqualTo(TrustMapping.FetchTrustName(trustDto).ToTitle()));
					
					// Case rating
					var actualCaseRatingModel = RatingMapping.MapDtoToModel(ratingsDto, actual.RatingUrn);
					Assert.NotNull(expected.RatingModel);
					Assert.NotNull(actualCaseRatingModel);
					Assert.That(expected.RatingModel.Checked, Is.EqualTo(actualCaseRatingModel.Checked));
					Assert.That(expected.RatingModel.Name, Is.EqualTo(actualCaseRatingModel.Name));
					Assert.That(expected.RatingModel.Urn, Is.EqualTo(actualCaseRatingModel.Urn));
					Assert.That(expected.RatingModel.RagRating, Is.EqualTo(actualCaseRatingModel.RagRating));
					Assert.That(expected.RatingModel.RagRatingCss, Is.EqualTo(actualCaseRatingModel.RagRatingCss));

					// Records
					var actualRecordsModel = RecordMapping.MapDtoToModel(recordsDtoLiveCases, typesDto, ratingsDto, statusesDto);
					var expectedRecordsModel = expected.RecordsModel;
					
					Assert.NotNull(expectedRecordsModel);
					Assert.NotNull(actualRecordsModel);
					Assert.That(expectedRecordsModel.Count, Is.EqualTo(actualRecordsModel.Count));
					
					for (var index = 0; index < expectedRecordsModel.Count; ++index)
					{
						Assert.That(expectedRecordsModel.ElementAt(index).Urn, Is.EqualTo(actualRecordsModel.ElementAt(index).Urn));
						Assert.That(expectedRecordsModel.ElementAt(index).CaseUrn, Is.EqualTo(actualRecordsModel.ElementAt(index).CaseUrn));
						Assert.That(expectedRecordsModel.ElementAt(index).RatingUrn, Is.EqualTo(actualRecordsModel.ElementAt(index).RatingUrn));
						Assert.That(expectedRecordsModel.ElementAt(index).StatusUrn, Is.EqualTo(actualRecordsModel.ElementAt(index).StatusUrn));
						Assert.That(expectedRecordsModel.ElementAt(index).TypeUrn, Is.EqualTo(actualRecordsModel.ElementAt(index).TypeUrn));
						
						var expectedRecordRatingModel = expectedRecordsModel.ElementAt(index).RatingModel;
						var actualRecordRatingModel = actualRecordsModel.ElementAt(index).RatingModel;
						Assert.NotNull(expectedRecordRatingModel);
						Assert.NotNull(actualRecordRatingModel);
						Assert.That(expectedRecordRatingModel.Checked, Is.EqualTo(actualRecordRatingModel.Checked));
						Assert.That(expectedRecordRatingModel.Name, Is.EqualTo(actualRecordRatingModel.Name));
						Assert.That(expectedRecordRatingModel.Urn, Is.EqualTo(actualRecordRatingModel.Urn));
						Assert.That(expectedRecordRatingModel.RagRating, Is.EqualTo(actualRecordRatingModel.RagRating));
						Assert.That(expectedRecordRatingModel.RagRatingCss, Is.EqualTo(actualRecordRatingModel.RagRatingCss));
						
						var expectedRecordTypeModel = expectedRecordsModel.ElementAt(index).TypeModel;
						var actualRecordTypeModel = actualRecordsModel.ElementAt(index).TypeModel;
						Assert.NotNull(expectedRecordTypeModel);
						Assert.NotNull(actualRecordTypeModel);
						Assert.That(expectedRecordTypeModel.Type, Is.EqualTo(actualRecordTypeModel.Type));
						Assert.That(expectedRecordTypeModel.SubType, Is.EqualTo(actualRecordTypeModel.SubType));
						Assert.That(expectedRecordTypeModel.TypeDisplay, Is.EqualTo(actualRecordTypeModel.TypeDisplay));
						Assert.That(expectedRecordTypeModel.TypesDictionary, Is.EqualTo(actualRecordTypeModel.TypesDictionary));

						var expectedRecordStatusModel = expectedRecordsModel.ElementAt(index).StatusModel;
						var actualRecordStatusModel = actualRecordsModel.ElementAt(index).StatusModel;
						Assert.NotNull(expectedRecordStatusModel);
						Assert.NotNull(actualRecordTypeModel);
						Assert.That(expectedRecordStatusModel.Name, Is.EqualTo(actualRecordStatusModel.Name));
						Assert.That(expectedRecordStatusModel.Urn, Is.EqualTo(actualRecordStatusModel.Urn));
					}
				}
			}
		}
		
		[Test]
		public async Task WhenGetCasesByCaseworkerAndStatus_GroupCaseWorkers_Ordered_By_Descending_UpdatedDate_Return_LiveCases()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();

			const string trustUkPrn = "trust-ukprn";
			
			// cases
			var casesDto = CaseFactory.BuildListCaseDto(trustUkPrn);
			var casesDtoLive = casesDto.Where(c => c.StatusUrn.CompareTo(1) == 0).ToList();
			
			// records
			var recordsDtoLiveCases = casesDtoLive.SelectMany(c => RecordFactory.BuildListRecordDtoByCaseUrn(c.Urn)).ToList();
			
			// status
			var statusLiveDto = StatusFactory.BuildStatusDto(StatusEnum.Live.ToString(), casesDtoLive.First().Urn);
			
			var ratingsDto = RatingFactory.BuildListRatingDto();
			var typesDto = TypeFactory.BuildListTypeDto();
			var trustDto = TrustFactory.BuildTrustDetailsDto();
			var statusesDto = StatusFactory.BuildListStatusDto();

			mockStatusCachedService.Setup(s => s.GetStatusByName(It.IsAny<string>()))
				.ReturnsAsync(statusLiveDto);
			mockStatusCachedService.Setup(s => s.GetStatuses())
				.ReturnsAsync(statusesDto);
			mockCaseCachedService.Setup(cs => cs.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(casesDtoLive);
			mockRatingCachedService.Setup(r => r.GetRatings())
				.ReturnsAsync(ratingsDto);
			mockTypeCachedService.Setup(t => t.GetTypes())
				.ReturnsAsync(typesDto);
			mockTrustCachedService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDto);
			mockRecordCachedService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(recordsDtoLiveCases);

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);

			var liveCasesModel = await caseModelService.GetCasesByCaseworkerAndStatus(new[] {"userA", "userB"}, StatusEnum.Live);

			// assert
			Assert.IsAssignableFrom<List<HomeModel>>(liveCasesModel);
			Assert.That(liveCasesModel.Count, Is.EqualTo(4));

			var filteredHomeModel = liveCasesModel.SelectMany(c => casesDto.Where(homeModel => homeModel.Urn.ToString() == c.CaseUrn));
			var orderedHomeModel = filteredHomeModel.OrderByDescending(c => c.UpdatedAt.ToUnixTimeMilliseconds()).ToList();

			for (var index = 0; index < liveCasesModel.Count; ++index)
			{
				var expected = liveCasesModel.ElementAt(index).CaseUrn;
				var actual = orderedHomeModel.ElementAt(index).Urn.ToString();
					
				Assert.That(expected, Is.EqualTo(actual));
			}
		}
		
		[Test]
		public async Task WhenGetCasesByCaseworkerAndStatus_GroupCaseWorkers_ThrowsException_Return_EmptyCases()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();

			const string trustUkPrn = "trust-ukprn";
			
			// cases
			var casesDto = CaseFactory.BuildListCaseDto(trustUkPrn);
			var casesDtoLive = casesDto.Where(c => c.StatusUrn.CompareTo(1) == 0).ToList();
			
			// records
			var recordsDtoLiveCases = casesDtoLive.SelectMany(c => RecordFactory.BuildListRecordDtoByCaseUrn(c.Urn)).ToList();
			
			// status
			var statusLiveDto = StatusFactory.BuildStatusDto(StatusEnum.Live.ToString(), casesDtoLive.First().Urn);
			
			var ratingsDto = RatingFactory.BuildListRatingDto();
			var typesDto = TypeFactory.BuildListTypeDto();
			var trustDto = TrustFactory.BuildTrustDetailsDto();

			mockStatusCachedService.Setup(s => s.GetStatusByName(It.IsAny<string>()))
				.ReturnsAsync(statusLiveDto);
			mockCaseCachedService.Setup(cs => cs.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(casesDtoLive);
			mockRatingCachedService.Setup(r => r.GetRatings())
				.ReturnsAsync(ratingsDto);
			mockTypeCachedService.Setup(t => t.GetTypes())
				.ReturnsAsync(typesDto);
			mockTrustCachedService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDto);
			mockRecordCachedService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(recordsDtoLiveCases);

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);
			
			var liveCasesModel = await caseModelService.GetCasesByCaseworkerAndStatus(It.IsAny<string[]>(), StatusEnum.Live);
		
			// assert
			Assert.IsAssignableFrom<HomeModel[]>(liveCasesModel);
			Assert.That(liveCasesModel.Count, Is.EqualTo(0));
		}
		
		[Test]
		public async Task WhenGetCasesByCaseworker_FetchFromAcademiesApi_Return_LiveCases()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();

			const string trustUkPrn = "trust-ukprn";
			
			// cases
			var casesDto = CaseFactory.BuildListCaseDto(trustUkPrn);
			var casesDtoLive = casesDto.Where(c => c.StatusUrn.CompareTo(1) == 0).ToList();
			
			// records
			var recordsDtoLiveCases = casesDtoLive.SelectMany(c => RecordFactory.BuildListRecordDtoByCaseUrn(c.Urn)).ToList();
			
			// status
			var statusLiveDto = StatusFactory.BuildStatusDto(StatusEnum.Live.ToString(), casesDtoLive.First().Urn);
			var statusesDto = StatusFactory.BuildListStatusDto();
			var ratingsDto = RatingFactory.BuildListRatingDto();
			var typesDto = TypeFactory.BuildListTypeDto();
			var trustDto = TrustFactory.BuildTrustDetailsDto();

			mockStatusCachedService.Setup(s => s.GetStatusByName(It.IsAny<string>()))
				.ReturnsAsync(statusLiveDto);
			mockStatusCachedService.Setup(s => s.GetStatuses())
				.ReturnsAsync(statusesDto);
			mockCaseCachedService.Setup(cs => cs.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(casesDtoLive);
			mockRatingCachedService.Setup(r => r.GetRatings())
				.ReturnsAsync(ratingsDto);
			mockTypeCachedService.Setup(t => t.GetTypes())
				.ReturnsAsync(typesDto);
			mockTrustCachedService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDto);
			mockRecordCachedService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(recordsDtoLiveCases);

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);

			var monitoringCasesModel = await caseModelService.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), StatusEnum.Live);

			// assert
			Assert.IsAssignableFrom<List<HomeModel>>(monitoringCasesModel);
			Assert.That(monitoringCasesModel.Count, Is.EqualTo(2));

			foreach (var expected in monitoringCasesModel)
			{
				foreach (var actual in casesDto.Where(actual => expected.CaseUrn.Equals(actual.Urn.ToString())))
				{
					Assert.That(expected.Closed, Is.EqualTo(actual.ClosedAt.ToString("dd-MM-yyyy")));
					Assert.That(expected.Created, Is.EqualTo(actual.CreatedAt.ToString("dd-MM-yyyy")));
					Assert.That(expected.Review, Is.EqualTo(actual.ReviewAt.ToString("dd-MM-yyyy")));
					Assert.That(expected.Updated, Is.EqualTo(actual.UpdatedAt.ToString("dd-MM-yyyy")));
					Assert.That(expected.ClosedUnixTime, Is.EqualTo(actual.ClosedAt.ToUnixTimeMilliseconds()));
					Assert.That(expected.CreatedUnixTime, Is.EqualTo(actual.CreatedAt.ToUnixTimeMilliseconds()));
					Assert.That(expected.ReviewUnixTime, Is.EqualTo(actual.ReviewAt.ToUnixTimeMilliseconds()));
					Assert.That(expected.UpdatedUnixTime, Is.EqualTo(actual.UpdatedAt.ToUnixTimeMilliseconds()));
					Assert.That(expected.CreatedBy, Is.EqualTo(actual.CreatedBy));
					Assert.That(expected.CaseUrn, Is.EqualTo(actual.Urn.ToString()));
					Assert.That(expected.TrustName, Is.EqualTo(TrustMapping.FetchTrustName(trustDto)));
					Assert.That(expected.TrustNameTitle, Is.EqualTo(TrustMapping.FetchTrustName(trustDto).ToTitle()));
					
					// Case rating
					var actualCaseRatingModel = RatingMapping.MapDtoToModel(ratingsDto, actual.RatingUrn);
					Assert.NotNull(expected.RatingModel);
					Assert.NotNull(actualCaseRatingModel);
					Assert.That(expected.RatingModel.Checked, Is.EqualTo(actualCaseRatingModel.Checked));
					Assert.That(expected.RatingModel.Name, Is.EqualTo(actualCaseRatingModel.Name));
					Assert.That(expected.RatingModel.Urn, Is.EqualTo(actualCaseRatingModel.Urn));
					Assert.That(expected.RatingModel.RagRating, Is.EqualTo(actualCaseRatingModel.RagRating));
					Assert.That(expected.RatingModel.RagRatingCss, Is.EqualTo(actualCaseRatingModel.RagRatingCss));

					// Records
					var actualRecordsModel = RecordMapping.MapDtoToModel(recordsDtoLiveCases, typesDto, ratingsDto, statusesDto);
					var expectedRecordsModel = expected.RecordsModel;
					
					Assert.NotNull(expectedRecordsModel);
					Assert.NotNull(actualRecordsModel);
					Assert.That(expectedRecordsModel.Count, Is.EqualTo(actualRecordsModel.Count));
					
					for (var index = 0; index < expectedRecordsModel.Count; ++index)
					{
						Assert.That(expectedRecordsModel.ElementAt(index).Urn, Is.EqualTo(actualRecordsModel.ElementAt(index).Urn));
						Assert.That(expectedRecordsModel.ElementAt(index).CaseUrn, Is.EqualTo(actualRecordsModel.ElementAt(index).CaseUrn));
						Assert.That(expectedRecordsModel.ElementAt(index).RatingUrn, Is.EqualTo(actualRecordsModel.ElementAt(index).RatingUrn));
						Assert.That(expectedRecordsModel.ElementAt(index).StatusUrn, Is.EqualTo(actualRecordsModel.ElementAt(index).StatusUrn));
						Assert.That(expectedRecordsModel.ElementAt(index).TypeUrn, Is.EqualTo(actualRecordsModel.ElementAt(index).TypeUrn));
						
						var expectedRecordRatingModel = expectedRecordsModel.ElementAt(index).RatingModel;
						var actualRecordRatingModel = actualRecordsModel.ElementAt(index).RatingModel;
						Assert.NotNull(expectedRecordRatingModel);
						Assert.NotNull(actualRecordRatingModel);
						Assert.That(expectedRecordRatingModel.Checked, Is.EqualTo(actualRecordRatingModel.Checked));
						Assert.That(expectedRecordRatingModel.Name, Is.EqualTo(actualRecordRatingModel.Name));
						Assert.That(expectedRecordRatingModel.Urn, Is.EqualTo(actualRecordRatingModel.Urn));
						Assert.That(expectedRecordRatingModel.RagRating, Is.EqualTo(actualRecordRatingModel.RagRating));
						Assert.That(expectedRecordRatingModel.RagRatingCss, Is.EqualTo(actualRecordRatingModel.RagRatingCss));
						
						var expectedRecordTypeModel = expectedRecordsModel.ElementAt(index).TypeModel;
						var actualRecordTypeModel = actualRecordsModel.ElementAt(index).TypeModel;
						Assert.NotNull(expectedRecordTypeModel);
						Assert.NotNull(actualRecordTypeModel);
						Assert.That(expectedRecordTypeModel.Type, Is.EqualTo(actualRecordTypeModel.Type));
						Assert.That(expectedRecordTypeModel.SubType, Is.EqualTo(actualRecordTypeModel.SubType));
						Assert.That(expectedRecordTypeModel.TypeDisplay, Is.EqualTo(actualRecordTypeModel.TypeDisplay));
						Assert.That(expectedRecordTypeModel.TypesDictionary, Is.EqualTo(actualRecordTypeModel.TypesDictionary));

						var expectedRecordStatusModel = expectedRecordsModel.ElementAt(index).StatusModel;
						var actualRecordStatusModel = actualRecordsModel.ElementAt(index).StatusModel;
						Assert.NotNull(expectedRecordStatusModel);
						Assert.NotNull(actualRecordTypeModel);
						Assert.That(expectedRecordStatusModel.Name, Is.EqualTo(actualRecordStatusModel.Name));
						Assert.That(expectedRecordStatusModel.Urn, Is.EqualTo(actualRecordStatusModel.Urn));
					}
				}
			}
		}

		[Test]
		public async Task WhenGetCasesByCaseworker_FetchFromAcademiesApi_Return_MonitoringCases()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();

			const string trustUkPrn = "trust-ukprn";
			
			// cases
			var casesDto = CaseFactory.BuildListCaseDto(trustUkPrn);
			var casesDtoMonitoring = casesDto.Where(c => c.StatusUrn.CompareTo(2) == 0).ToList();
			
			// records
			var recordsDtoMonitoringCases = casesDtoMonitoring.SelectMany(c => RecordFactory.BuildListRecordDtoByCaseUrn(c.Urn)).ToList();
			
			// status
			var statusMonitoringDto = StatusFactory.BuildStatusDto(StatusEnum.Monitoring.ToString(), casesDtoMonitoring.First().Urn);
			var statusesDto = StatusFactory.BuildListStatusDto();
			var ratingsDto = RatingFactory.BuildListRatingDto();
			var typesDto = TypeFactory.BuildListTypeDto();
			var trustDto = TrustFactory.BuildTrustDetailsDto();

			mockStatusCachedService.Setup(s => s.GetStatusByName(It.IsAny<string>()))
				.ReturnsAsync(statusMonitoringDto);
			mockStatusCachedService.Setup(s => s.GetStatuses())
				.ReturnsAsync(statusesDto);
			mockCaseCachedService.Setup(cs => cs.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(casesDtoMonitoring);
			mockRatingCachedService.Setup(r => r.GetRatings())
				.ReturnsAsync(ratingsDto);
			mockTypeCachedService.Setup(t => t.GetTypes())
				.ReturnsAsync(typesDto);
			mockTrustCachedService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDto);
			mockRecordCachedService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(recordsDtoMonitoringCases);

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);

			var monitoringCasesModel = await caseModelService.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), StatusEnum.Monitoring);

			// assert
			Assert.IsAssignableFrom<List<HomeModel>>(monitoringCasesModel);
			Assert.That(monitoringCasesModel.Count, Is.EqualTo(2));

			foreach (var expected in monitoringCasesModel)
			{
				foreach (var actual in casesDto.Where(actual => expected.CaseUrn.Equals(actual.Urn.ToString())))
				{
					Assert.That(expected.Closed, Is.EqualTo(actual.ClosedAt.ToString("dd-MM-yyyy")));
					Assert.That(expected.Created, Is.EqualTo(actual.CreatedAt.ToString("dd-MM-yyyy")));
					Assert.That(expected.Review, Is.EqualTo(actual.ReviewAt.ToString("dd-MM-yyyy")));
					Assert.That(expected.Updated, Is.EqualTo(actual.UpdatedAt.ToString("dd-MM-yyyy")));
					Assert.That(expected.ClosedUnixTime, Is.EqualTo(actual.ClosedAt.ToUnixTimeMilliseconds()));
					Assert.That(expected.CreatedUnixTime, Is.EqualTo(actual.CreatedAt.ToUnixTimeMilliseconds()));
					Assert.That(expected.ReviewUnixTime, Is.EqualTo(actual.ReviewAt.ToUnixTimeMilliseconds()));
					Assert.That(expected.UpdatedUnixTime, Is.EqualTo(actual.UpdatedAt.ToUnixTimeMilliseconds()));
					Assert.That(expected.CreatedBy, Is.EqualTo(actual.CreatedBy));
					Assert.That(expected.CaseUrn, Is.EqualTo(actual.Urn.ToString()));
					Assert.That(expected.TrustName, Is.EqualTo(TrustMapping.FetchTrustName(trustDto)));
					Assert.That(expected.TrustNameTitle, Is.EqualTo(TrustMapping.FetchTrustName(trustDto).ToTitle()));
					
					// Case rating
					var actualCaseRatingModel = RatingMapping.MapDtoToModel(ratingsDto, actual.RatingUrn);
					Assert.NotNull(expected.RatingModel);
					Assert.NotNull(actualCaseRatingModel);
					Assert.That(expected.RatingModel.Checked, Is.EqualTo(actualCaseRatingModel.Checked));
					Assert.That(expected.RatingModel.Name, Is.EqualTo(actualCaseRatingModel.Name));
					Assert.That(expected.RatingModel.Urn, Is.EqualTo(actualCaseRatingModel.Urn));
					Assert.That(expected.RatingModel.RagRating, Is.EqualTo(actualCaseRatingModel.RagRating));
					Assert.That(expected.RatingModel.RagRatingCss, Is.EqualTo(actualCaseRatingModel.RagRatingCss));

					// Records
					var actualRecordsModel = RecordMapping.MapDtoToModel(recordsDtoMonitoringCases, typesDto, ratingsDto, statusesDto);
					var expectedRecordsModel = expected.RecordsModel;
					
					Assert.NotNull(expectedRecordsModel);
					Assert.NotNull(actualRecordsModel);
					Assert.That(expectedRecordsModel.Count, Is.EqualTo(actualRecordsModel.Count));
					
					for (var index = 0; index < expectedRecordsModel.Count; ++index)
					{
						Assert.That(expectedRecordsModel.ElementAt(index).Urn, Is.EqualTo(actualRecordsModel.ElementAt(index).Urn));
						Assert.That(expectedRecordsModel.ElementAt(index).CaseUrn, Is.EqualTo(actualRecordsModel.ElementAt(index).CaseUrn));
						Assert.That(expectedRecordsModel.ElementAt(index).RatingUrn, Is.EqualTo(actualRecordsModel.ElementAt(index).RatingUrn));
						Assert.That(expectedRecordsModel.ElementAt(index).StatusUrn, Is.EqualTo(actualRecordsModel.ElementAt(index).StatusUrn));
						Assert.That(expectedRecordsModel.ElementAt(index).TypeUrn, Is.EqualTo(actualRecordsModel.ElementAt(index).TypeUrn));
						
						var expectedRecordRatingModel = expectedRecordsModel.ElementAt(index).RatingModel;
						var actualRecordRatingModel = actualRecordsModel.ElementAt(index).RatingModel;
						Assert.NotNull(expectedRecordRatingModel);
						Assert.NotNull(actualRecordRatingModel);
						Assert.That(expectedRecordRatingModel.Checked, Is.EqualTo(actualRecordRatingModel.Checked));
						Assert.That(expectedRecordRatingModel.Name, Is.EqualTo(actualRecordRatingModel.Name));
						Assert.That(expectedRecordRatingModel.Urn, Is.EqualTo(actualRecordRatingModel.Urn));
						Assert.That(expectedRecordRatingModel.RagRating, Is.EqualTo(actualRecordRatingModel.RagRating));
						Assert.That(expectedRecordRatingModel.RagRatingCss, Is.EqualTo(actualRecordRatingModel.RagRatingCss));
						
						var expectedRecordTypeModel = expectedRecordsModel.ElementAt(index).TypeModel;
						var actualRecordTypeModel = actualRecordsModel.ElementAt(index).TypeModel;
						Assert.NotNull(expectedRecordTypeModel);
						Assert.NotNull(actualRecordTypeModel);
						Assert.That(expectedRecordTypeModel.Type, Is.EqualTo(actualRecordTypeModel.Type));
						Assert.That(expectedRecordTypeModel.SubType, Is.EqualTo(actualRecordTypeModel.SubType));
						Assert.That(expectedRecordTypeModel.TypeDisplay, Is.EqualTo(actualRecordTypeModel.TypeDisplay));
						Assert.That(expectedRecordTypeModel.TypesDictionary, Is.EqualTo(actualRecordTypeModel.TypesDictionary));

						var expectedRecordStatusModel = expectedRecordsModel.ElementAt(index).StatusModel;
						var actualRecordStatusModel = actualRecordsModel.ElementAt(index).StatusModel;
						Assert.NotNull(expectedRecordStatusModel);
						Assert.NotNull(actualRecordTypeModel);
						Assert.That(expectedRecordStatusModel.Name, Is.EqualTo(actualRecordStatusModel.Name));
						Assert.That(expectedRecordStatusModel.Urn, Is.EqualTo(actualRecordStatusModel.Urn));
					}
				}
			}
		}
		
		[Test]
		public async Task WhenGetCasesByCaseworker_FetchFromAcademiesApi_Return_CloseCases()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();

			const string trustUkPrn = "trust-ukprn";
			
			// cases
			var casesDto = CaseFactory.BuildListCaseDto(trustUkPrn);
			var casesDtoClosed = casesDto.Where(c => c.StatusUrn.CompareTo(3) == 0).ToList();
			
			// records
			var recordsDtoClosedCases = casesDtoClosed.SelectMany(c => RecordFactory.BuildListRecordDtoByCaseUrn(c.Urn)).ToList();
			
			// status
			var statusClosedDto = StatusFactory.BuildStatusDto(StatusEnum.Close.ToString(), casesDtoClosed.First().Urn);
			var statusesDto = StatusFactory.BuildListStatusDto();
			var ratingsDto = RatingFactory.BuildListRatingDto();
			var typesDto = TypeFactory.BuildListTypeDto();
			var trustDto = TrustFactory.BuildTrustDetailsDto();

			mockStatusCachedService.Setup(s => s.GetStatusByName(It.IsAny<string>()))
				.ReturnsAsync(statusClosedDto);
			mockStatusCachedService.Setup(s => s.GetStatuses())
				.ReturnsAsync(statusesDto);
			mockCaseCachedService.Setup(cs => cs.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(casesDtoClosed);
			mockRatingCachedService.Setup(r => r.GetRatings())
				.ReturnsAsync(ratingsDto);
			mockTypeCachedService.Setup(t => t.GetTypes())
				.ReturnsAsync(typesDto);
			mockTrustCachedService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDto);
			mockRecordCachedService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(recordsDtoClosedCases);

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);

			var monitoringCasesModel = await caseModelService.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), StatusEnum.Close);

			// assert
			Assert.IsAssignableFrom<List<HomeModel>>(monitoringCasesModel);
			Assert.That(monitoringCasesModel.Count, Is.EqualTo(1));

			foreach (var expected in monitoringCasesModel)
			{
				foreach (var actual in casesDto.Where(actual => expected.CaseUrn.Equals(actual.Urn.ToString())))
				{
					Assert.That(expected.Closed, Is.EqualTo(actual.ClosedAt.ToString("dd-MM-yyyy")));
					Assert.That(expected.Created, Is.EqualTo(actual.CreatedAt.ToString("dd-MM-yyyy")));
					Assert.That(expected.Review, Is.EqualTo(actual.ReviewAt.ToString("dd-MM-yyyy")));
					Assert.That(expected.Updated, Is.EqualTo(actual.UpdatedAt.ToString("dd-MM-yyyy")));
					Assert.That(expected.ClosedUnixTime, Is.EqualTo(actual.ClosedAt.ToUnixTimeMilliseconds()));
					Assert.That(expected.CreatedUnixTime, Is.EqualTo(actual.CreatedAt.ToUnixTimeMilliseconds()));
					Assert.That(expected.ReviewUnixTime, Is.EqualTo(actual.ReviewAt.ToUnixTimeMilliseconds()));
					Assert.That(expected.UpdatedUnixTime, Is.EqualTo(actual.UpdatedAt.ToUnixTimeMilliseconds()));
					Assert.That(expected.CreatedBy, Is.EqualTo(actual.CreatedBy));
					Assert.That(expected.CaseUrn, Is.EqualTo(actual.Urn.ToString()));
					Assert.That(expected.TrustName, Is.EqualTo(TrustMapping.FetchTrustName(trustDto)));
					Assert.That(expected.TrustNameTitle, Is.EqualTo(TrustMapping.FetchTrustName(trustDto).ToTitle()));
					
					// Case rating
					var actualCaseRatingModel = RatingMapping.MapDtoToModel(ratingsDto, actual.RatingUrn);
					Assert.NotNull(expected.RatingModel);
					Assert.NotNull(actualCaseRatingModel);
					Assert.That(expected.RatingModel.Checked, Is.EqualTo(actualCaseRatingModel.Checked));
					Assert.That(expected.RatingModel.Name, Is.EqualTo(actualCaseRatingModel.Name));
					Assert.That(expected.RatingModel.Urn, Is.EqualTo(actualCaseRatingModel.Urn));
					Assert.That(expected.RatingModel.RagRating, Is.EqualTo(actualCaseRatingModel.RagRating));
					Assert.That(expected.RatingModel.RagRatingCss, Is.EqualTo(actualCaseRatingModel.RagRatingCss));

					// Records
					var actualRecordsModel = RecordMapping.MapDtoToModel(recordsDtoClosedCases, typesDto, ratingsDto, statusesDto);
					var expectedRecordsModel = expected.RecordsModel;
					
					Assert.NotNull(expectedRecordsModel);
					Assert.NotNull(actualRecordsModel);
					Assert.That(expectedRecordsModel.Count, Is.EqualTo(actualRecordsModel.Count));
					
					for (var index = 0; index < expectedRecordsModel.Count; ++index)
					{
						Assert.That(expectedRecordsModel.ElementAt(index).Urn, Is.EqualTo(actualRecordsModel.ElementAt(index).Urn));
						Assert.That(expectedRecordsModel.ElementAt(index).CaseUrn, Is.EqualTo(actualRecordsModel.ElementAt(index).CaseUrn));
						Assert.That(expectedRecordsModel.ElementAt(index).RatingUrn, Is.EqualTo(actualRecordsModel.ElementAt(index).RatingUrn));
						Assert.That(expectedRecordsModel.ElementAt(index).StatusUrn, Is.EqualTo(actualRecordsModel.ElementAt(index).StatusUrn));
						Assert.That(expectedRecordsModel.ElementAt(index).TypeUrn, Is.EqualTo(actualRecordsModel.ElementAt(index).TypeUrn));
						
						var expectedRecordRatingModel = expectedRecordsModel.ElementAt(index).RatingModel;
						var actualRecordRatingModel = actualRecordsModel.ElementAt(index).RatingModel;
						Assert.NotNull(expectedRecordRatingModel);
						Assert.NotNull(actualRecordRatingModel);
						Assert.That(expectedRecordRatingModel.Checked, Is.EqualTo(actualRecordRatingModel.Checked));
						Assert.That(expectedRecordRatingModel.Name, Is.EqualTo(actualRecordRatingModel.Name));
						Assert.That(expectedRecordRatingModel.Urn, Is.EqualTo(actualRecordRatingModel.Urn));
						Assert.That(expectedRecordRatingModel.RagRating, Is.EqualTo(actualRecordRatingModel.RagRating));
						Assert.That(expectedRecordRatingModel.RagRatingCss, Is.EqualTo(actualRecordRatingModel.RagRatingCss));
						
						var expectedRecordTypeModel = expectedRecordsModel.ElementAt(index).TypeModel;
						var actualRecordTypeModel = actualRecordsModel.ElementAt(index).TypeModel;
						Assert.NotNull(expectedRecordTypeModel);
						Assert.NotNull(actualRecordTypeModel);
						Assert.That(expectedRecordTypeModel.Type, Is.EqualTo(actualRecordTypeModel.Type));
						Assert.That(expectedRecordTypeModel.SubType, Is.EqualTo(actualRecordTypeModel.SubType));
						Assert.That(expectedRecordTypeModel.TypeDisplay, Is.EqualTo(actualRecordTypeModel.TypeDisplay));
						Assert.That(expectedRecordTypeModel.TypesDictionary, Is.EqualTo(actualRecordTypeModel.TypesDictionary));

						var expectedRecordStatusModel = expectedRecordsModel.ElementAt(index).StatusModel;
						var actualRecordStatusModel = actualRecordsModel.ElementAt(index).StatusModel;
						Assert.NotNull(expectedRecordStatusModel);
						Assert.NotNull(actualRecordTypeModel);
						Assert.That(expectedRecordStatusModel.Name, Is.EqualTo(actualRecordStatusModel.Name));
						Assert.That(expectedRecordStatusModel.Urn, Is.EqualTo(actualRecordStatusModel.Urn));
					}
				}
			}
		}
		
		[Test]
		public async Task WhenGetCasesByCaseworker_FetchFromAcademiesApi_ReturnEmptyCases()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();
			
			var casesDto = CaseFactory.BuildListCaseDto();
			var statusLiveDto = StatusFactory.BuildStatusDto(StatusEnum.Live.ToString(), 1);
			
			mockCaseCachedService.Setup(cs => cs.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(casesDto);
			mockStatusCachedService.Setup(s => s.GetStatusByName(It.IsAny<string>())).ReturnsAsync(statusLiveDto);
			mockCaseCachedService.Setup(cs => cs.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(new List<CaseDto>());

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);
			
			var activeCasesModel = await caseModelService.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<StatusEnum>());

			// assert
			Assert.IsAssignableFrom<HomeModel[]>(activeCasesModel);
			Assert.That(activeCasesModel.Count, Is.EqualTo(0));
		}
		
		[Test]
		public async Task WhenGetCasesByCaseworker_FetchFromAcademiesApi_ThrowsException_ReturnEmptyCases()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();
			
			var casesDto = CaseFactory.BuildListCaseDto();
			var statusLiveDto = StatusFactory.BuildStatusDto(StatusEnum.Live.ToString(), 1);
			
			mockCaseCachedService.Setup(cs => cs.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(casesDto);
			mockStatusCachedService.Setup(s => s.GetStatusByName(It.IsAny<string>())).ReturnsAsync(statusLiveDto);
			mockCaseCachedService.Setup(cs => cs.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<long>()))
				.ThrowsAsync(new Exception());

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);
			
			var activeCasesModel = await caseModelService.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<StatusEnum>());

			// assert
			Assert.IsAssignableFrom<HomeModel[]>(activeCasesModel);
			Assert.That(activeCasesModel.Count, Is.EqualTo(0));
		}
		
		[Test]
		public async Task WhenGetCasesByCaseworker_StatusUnknown_ReturnsEmptyCases()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();

			var casesDto = CaseFactory.BuildListCaseDto();
			var recordsDto = RecordFactory.BuildListRecordDto();

			var statusCloseDto = StatusFactory.BuildStatusDto(StatusEnum.Unknown.ToString(), casesDto.First().Urn);
			var ratingsDto = RatingFactory.BuildListRatingDto();
			var typesDto = TypeFactory.BuildListOrphanTypeDto();
			var trustDto = TrustFactory.BuildTrustDetailsDto(casesDto.First().TrustUkPrn);

			mockStatusCachedService.Setup(s => s.GetStatusByName(It.IsAny<string>()))
				.ReturnsAsync(statusCloseDto);
			mockCaseCachedService.Setup(cs => cs.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(casesDto);
			mockRatingCachedService.Setup(r => r.GetRatings())
				.ReturnsAsync(ratingsDto);
			mockTypeCachedService.Setup(t => t.GetTypes())
				.ReturnsAsync(typesDto);
			mockTrustCachedService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDto);
			mockRecordCachedService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(recordsDto);

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);

			var closedCasesModel = await caseModelService.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), StatusEnum.Unknown);

			// assert
			Assert.IsAssignableFrom<HomeModel[]>(closedCasesModel);
			Assert.That(closedCasesModel.Count, Is.EqualTo(0));
		}

		[Test]
		public async Task WhenGetCasesByCaseworker_FetchFromCache_MissingCasesDto_ReturnsCases()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockCachedService = new Mock<ICachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();

			var casesDto = CaseFactory.BuildListCaseDto();
			var firstCaseDto = casesDto.First();
			var recordsDto = RecordFactory.BuildListRecordDto();
			var statusLiveDto = StatusFactory.BuildStatusDto(StatusEnum.Live.ToString(), 1);
			var statusMonitoringDto = StatusFactory.BuildStatusDto(StatusEnum.Monitoring.ToString(), 2);
			var ratingsDto = RatingFactory.BuildListRatingDto();
			var typesDto = TypeFactory.BuildListTypeDto();
			var trustDto = TrustFactory.BuildTrustDetailsDto(firstCaseDto.TrustUkPrn);
			var statusesDto = StatusFactory.BuildListStatusDto();
			
			mockStatusCachedService.SetupSequence(s => s.GetStatusByName(It.IsAny<string>())).ReturnsAsync(statusLiveDto).ReturnsAsync(statusMonitoringDto);
			mockStatusCachedService.Setup(s => s.GetStatuses()).ReturnsAsync(statusesDto);
			mockRecordCachedService.Setup(r => r.GetRecordsByCaseUrn(firstCaseDto.CreatedBy, firstCaseDto.Urn)).ReturnsAsync(recordsDto);
			mockRatingCachedService.Setup(r => r.GetRatings()).ReturnsAsync(ratingsDto);
			mockTypeCachedService.Setup(t => t.GetTypes()).ReturnsAsync(typesDto);
			mockTrustCachedService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>())).ReturnsAsync(trustDto);
			mockCaseCachedService.Setup(cs => cs.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(casesDto);

			var userState = new UserState
			{
				TrustUkPrn = firstCaseDto.TrustUkPrn
			};
			mockCachedService.Setup(c => c.GetData<UserState>(It.IsAny<string>())).ReturnsAsync(userState);

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);
			
			var activeCasesModel = await caseModelService.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<StatusEnum>());

			// assert
			Assert.IsAssignableFrom<List<HomeModel>>(activeCasesModel);
			Assert.That(activeCasesModel.Count, Is.EqualTo(casesDto.Count));
		}

		[Test]
		public async Task When_GetCasesByTrustUkprn_ReturnsEmptyListTrustCasesModel()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();
			
			mockCaseSearchService.Setup(c => c.GetCasesByCaseTrustSearch(It.IsAny<CaseTrustSearch>()))
				.ReturnsAsync(new List<CaseDto>());
			
			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);

			var actualTrustCasesModel = await caseModelService.GetCasesByTrustUkprn("testing");

			// assert
			Assert.IsAssignableFrom<TrustCasesModel[]>(actualTrustCasesModel);
			Assert.That(actualTrustCasesModel, Is.Not.Null);
			Assert.That(actualTrustCasesModel.Count, Is.EqualTo(0));
		}

		[Test]
		public async Task When_GetCasesByTrustUkprn_ReturnsListTrustCasesModel()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();

			var casesDto = CaseFactory.BuildListCaseDto();
			var recordsDto = RecordFactory.BuildListRecordDto();
			var ratingsDto = RatingFactory.BuildListRatingDto();
			var typesDto = TypeFactory.BuildListTypeDto();
			var statusesDto = StatusFactory.BuildListStatusDto();
			var monitoringStatus = StatusFactory.BuildStatusDto(StatusEnum.Monitoring.ToString(), 1);

			mockCaseSearchService.Setup(c => c.GetCasesByCaseTrustSearch(It.IsAny<CaseTrustSearch>()))
				.ReturnsAsync(casesDto);
			mockRecordCachedService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(recordsDto);
			mockRatingCachedService.Setup(r => r.GetRatings()).ReturnsAsync(ratingsDto);
			mockTypeCachedService.Setup(t => t.GetTypes()).ReturnsAsync(typesDto);
			mockStatusCachedService.Setup(s => s.GetStatuses())
				.ReturnsAsync(statusesDto);
			mockStatusCachedService.Setup(s => s.GetStatusByName(It.IsAny<string>()))
				.ReturnsAsync(monitoringStatus);

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);

			var actualTrustCasesModel = await caseModelService.GetCasesByTrustUkprn("testing");

			// assert
			Assert.IsAssignableFrom<List<TrustCasesModel>>(actualTrustCasesModel);
			Assert.That(actualTrustCasesModel, Is.Not.Null);
			Assert.That(actualTrustCasesModel.Count, Is.EqualTo(3));
		}

		[Test]
		public async Task When_GetCasesByTrustUkprn_MissingRecords_ReturnsEmptyListTrustCasesModel()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();

			var casesDto = CaseFactory.BuildListCaseDto();
			var ratingsDto = RatingFactory.BuildListRatingDto();
			var typesDto = TypeFactory.BuildListTypeDto();
			var statusesDto = StatusFactory.BuildListStatusDto();
			var monitoringStatus = StatusFactory.BuildStatusDto(StatusEnum.Monitoring.ToString(), 1);

			mockCaseSearchService.Setup(c => c.GetCasesByCaseTrustSearch(It.IsAny<CaseTrustSearch>()))
				.ReturnsAsync(casesDto);
			mockRecordCachedService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(new List<RecordDto>());
			mockRatingCachedService.Setup(r => r.GetRatings()).ReturnsAsync(ratingsDto);
			mockTypeCachedService.Setup(t => t.GetTypes()).ReturnsAsync(typesDto);
			mockStatusCachedService.Setup(s => s.GetStatuses())
				.ReturnsAsync(statusesDto);
			mockStatusCachedService.Setup(s => s.GetStatusByName(It.IsAny<string>()))
				.ReturnsAsync(monitoringStatus);

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);

			var actualTrustCasesModel = await caseModelService.GetCasesByTrustUkprn("testing");

			// assert
			Assert.IsAssignableFrom<TrustCasesModel[]>(actualTrustCasesModel);
			Assert.That(actualTrustCasesModel, Is.Not.Null);
			Assert.That(actualTrustCasesModel.Count, Is.EqualTo(0));
		}

		[Test]
		public void When_GetCasesByTrustUkprn_ThrowsException()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();
			
			mockCaseSearchService.Setup(c => c.GetCasesByCaseTrustSearch(It.IsAny<CaseTrustSearch>()))
				.Throws<Exception>();
			
			// act | assert
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);

			Assert.ThrowsAsync<Exception>(() => caseModelService.GetCasesByTrustUkprn("testing"));
		}		
		
		[Test]
		public async Task WhenPostCase_ReturnsCaseUrn()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();

			var caseDto = CaseFactory.BuildCaseDto();
			var recordDto = RecordFactory.BuildRecordDto();
			var statusLiveDto = StatusFactory.BuildStatusDto(StatusEnum.Live.ToString(), 1);
			var statusMonitoringDto = StatusFactory.BuildStatusDto(StatusEnum.Monitoring.ToString(), 2);
			var createCaseModel = CaseFactory.BuildCreateCaseModel();
			createCaseModel.CreateRecordsModel = RecordFactory.BuildListCreateRecordModel();

			mockStatusCachedService.SetupSequence(s => s.GetStatusByName(It.IsAny<string>())).ReturnsAsync(statusLiveDto).ReturnsAsync(statusMonitoringDto);
			mockCaseCachedService.Setup(cs => cs.PostCase(It.IsAny<CreateCaseDto>())).ReturnsAsync(caseDto);
			mockRecordCachedService.Setup(r => r.PostRecordByCaseUrn(It.IsAny<CreateRecordDto>(), It.IsAny<string>())).ReturnsAsync(recordDto);

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);
			
			var actualCaseUrn = await caseModelService.PostCase(createCaseModel);

			// assert
			Assert.That(actualCaseUrn, Is.Not.Null);
			Assert.That(actualCaseUrn, Is.EqualTo(caseDto.Urn));
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
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();
			
			var statusLiveDto = StatusFactory.BuildStatusDto(StatusEnum.Live.ToString(), 1);
			var statusMonitoringDto = StatusFactory.BuildStatusDto(StatusEnum.Monitoring.ToString(), 2);
			var createCaseModel = CaseFactory.BuildCreateCaseModel();
			
			mockStatusCachedService.SetupSequence(s => s.GetStatusByName(It.IsAny<string>())).ReturnsAsync(statusLiveDto).ReturnsAsync(statusMonitoringDto);

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);
			
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
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();
			
			var caseDto = CaseFactory.BuildCaseDto();

			mockCaseCachedService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseDto);
			mockRecordCachedService.Setup(r => r.GetRecordsByCaseUrn(caseDto.CreatedBy, caseDto.Urn))
				.ReturnsAsync(RecordFactory.BuildListRecordDto());
			mockTypeCachedService.Setup(t => t.GetTypes()).ReturnsAsync(TypeFactory.BuildListTypeDto());
			mockRatingCachedService.Setup(r => r.GetRatings()).ReturnsAsync(RatingFactory.BuildListRatingDto());

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);

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
			Assert.That(actualCaseModel.StatusUrn, Is.EqualTo(1));
			Assert.That(actualCaseModel.RatingUrn, Is.EqualTo(caseDto.RatingUrn));
			Assert.IsNull(actualCaseModel.RatingModel);
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
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();

			var caseDto = CaseFactory.BuildCaseDto();

			mockRecordCachedService.Setup(r => r.GetRecordsByCaseUrn(caseDto.CreatedBy, caseDto.Urn))
				.ReturnsAsync(RecordFactory.BuildListRecordDto());
			mockTypeCachedService.Setup(t => t.GetTypes()).ReturnsAsync(TypeFactory.BuildListTypeDto());
			mockRatingCachedService.Setup(r => r.GetRatings()).ReturnsAsync(RatingFactory.BuildListRatingDto());

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);

			Assert.ThrowsAsync<NullReferenceException>(() => caseModelService.GetCaseByUrn("testing", 1));
		}
		
		[Test]
		public async Task WhenPatchCaseRating_ReturnsTask()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();

			var caseDto = CaseFactory.BuildCaseDto();
			
			mockCaseCachedService.Setup(cs => cs.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(caseDto);
			
			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);
			
			await caseModelService.PatchCaseRating(CaseFactory.BuildPatchCaseModel());

			// assert
			mockCaseCachedService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Once);
		}
		
		[Test]
		public void WhenPatchCaseRating_ThrowsException()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();
			
			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);
			
			Assert.ThrowsAsync<NullReferenceException>(() => caseModelService.PatchCaseRating(CaseFactory.BuildPatchCaseModel()));

			// assert
			mockRecordCachedService.Verify(r => r.PatchRecordByUrn(It.IsAny<RecordDto>(), It.IsAny<string>()), Times.Never);
			mockCaseCachedService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Never);
		}

		[Test]
		public async Task WhenPatchRecordRating_ReturnsTask()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();

			var recordsDto = RecordFactory.BuildListRecordDto();

			mockRecordCachedService.Setup(rs => rs.GetRecordsByCaseUrn(It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(recordsDto);

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object,
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,
				mockStatusCachedService.Object, mockCaseSearchService.Object,
				mockCaseHistoryCachedService.Object, mockLogger.Object);

			await caseModelService.PatchRecordRating(RecordFactory.BuildPatchRecordModel());

			// assert
			mockRecordCachedService.Verify(r => r.PatchRecordByUrn(It.IsAny<RecordDto>(), It.IsAny<string>()), Times.Once);
		}

		[Test]
		public void WhenPatchRecordRating_ThrowsException()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object,
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,
				mockStatusCachedService.Object, mockCaseSearchService.Object,
				mockCaseHistoryCachedService.Object, mockLogger.Object);

			Assert.ThrowsAsync<ArgumentNullException>(() => caseModelService.PatchRecordRating(RecordFactory.BuildPatchRecordModel()));

			// assert
			mockCaseCachedService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Never);
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
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();

			var caseDto = CaseFactory.BuildCaseDto();
			var statusDto = StatusFactory.BuildStatusDto(StatusEnum.Live.ToString(), 1);

			mockStatusCachedService.Setup(t => t.GetStatusByName(It.IsAny<string>())).ReturnsAsync(statusDto);
			mockCaseCachedService.Setup(cs => cs.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(caseDto);
			mockCaseCachedService.Setup(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()));
			mockCaseHistoryCachedService.Setup(c => c.PostCaseHistory(It.IsAny<CreateCaseHistoryDto>(), It.IsAny<string>()));
			
			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);
			
			await caseModelService.PatchClosure(CaseFactory.BuildPatchCaseModel());

			// assert
			mockCaseHistoryCachedService.Verify(c => c.PostCaseHistory(It.IsAny<CreateCaseHistoryDto>(), It.IsAny<string>()), Times.Once);
			mockCaseCachedService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Once);
			mockStatusCachedService.Verify(t => t.GetStatusByName(It.IsAny<string>()), Times.Once);
			mockCaseCachedService.Verify(cs => cs.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Once);
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
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);
			
			Assert.ThrowsAsync<NullReferenceException>(() => caseModelService.PatchClosure(CaseFactory.BuildPatchCaseModel()));

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
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();

			var caseDto = CaseFactory.BuildCaseDto();
			
			mockCaseCachedService.Setup(cs => cs.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(caseDto);

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);
			
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
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);
			
			Assert.ThrowsAsync<NullReferenceException>(() => caseModelService.PatchDirectionOfTravel(CaseFactory.BuildPatchCaseModel()));

			// assert
			mockCaseCachedService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Never);
		}
		
		[Test]
		public async Task WhenPatchIssue_ReturnsTask()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();

			var caseDto = CaseFactory.BuildCaseDto();
			
			mockCaseCachedService.Setup(cs => cs.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(caseDto);

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);
			
			await caseModelService.PatchIssue(CaseFactory.BuildPatchCaseModel());

			// assert
			mockCaseCachedService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Once);
		}
		
		[Test]
		public void WhenPatchIssue_ThrowsException()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);
			
			Assert.ThrowsAsync<NullReferenceException>(() => caseModelService.PatchIssue(CaseFactory.BuildPatchCaseModel()));

			// assert
			mockCaseCachedService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Never);
		}
		
		[Test]
		public async Task WhenPatchCurrentStatus_ReturnsTask()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();

			var caseDto = CaseFactory.BuildCaseDto();
			
			mockCaseCachedService.Setup(cs => cs.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(caseDto);

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);
			
			await caseModelService.PatchCurrentStatus(CaseFactory.BuildPatchCaseModel());

			// assert
			mockCaseCachedService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Once);
		}
		
		[Test]
		public void WhenPatchCurrentStatus_ThrowsException()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);
			
			Assert.ThrowsAsync<NullReferenceException>(() => caseModelService.PatchCurrentStatus(CaseFactory.BuildPatchCaseModel()));

			// assert
			mockCaseCachedService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Never);
		}
		
		[Test]
		public async Task WhenPatchCaseAim_ReturnsTask()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();

			var caseDto = CaseFactory.BuildCaseDto();
			
			mockCaseCachedService.Setup(cs => cs.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(caseDto);

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);
			
			await caseModelService.PatchCaseAim(CaseFactory.BuildPatchCaseModel());

			// assert
			mockCaseCachedService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Once);
		}
		
		[Test]
		public void WhenPatchCaseAim_ThrowsException()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();
			
			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);
			
			Assert.ThrowsAsync<NullReferenceException>(() => caseModelService.PatchCaseAim(CaseFactory.BuildPatchCaseModel()));

			// assert
			mockCaseCachedService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Never);
		}
		
		[Test]
		public async Task WhenPatchDeEscalationPoint_ReturnsTask()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();

			var caseDto = CaseFactory.BuildCaseDto();
			
			mockCaseCachedService.Setup(cs => cs.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(caseDto);

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);
			
			await caseModelService.PatchDeEscalationPoint(CaseFactory.BuildPatchCaseModel());

			// assert
			mockCaseCachedService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Once);
		}
		
		[Test]
		public void WhenPatchDeEscalationPoint_ThrowsException()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);
			
			Assert.ThrowsAsync<NullReferenceException>(() => caseModelService.PatchDeEscalationPoint(CaseFactory.BuildPatchCaseModel()));

			// assert
			mockCaseCachedService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Never);
		}

		[Test]
		public async Task WhenPatchNextSteps_ReturnsTask()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();

			var caseDto = CaseFactory.BuildCaseDto();

			mockCaseCachedService.Setup(cs => cs.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(caseDto);

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);

			await caseModelService.PatchNextSteps(CaseFactory.BuildPatchCaseModel());

			// assert
			mockCaseCachedService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Once);
		}

		[Test]
		public void WhenPatchNextSteps_ThrowsException()
		{
			// arrange
			var mockCaseCachedService = new Mock<ICaseCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockRecordCachedService = new Mock<IRecordCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseSearchService = new Mock<ICaseSearchService>();
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();

			// act
			var caseModelService = new CaseModelService(mockCaseCachedService.Object, 
				mockTrustCachedService.Object, mockRecordCachedService.Object,
				mockRatingCachedService.Object, mockTypeCachedService.Object,  
				mockStatusCachedService.Object, mockCaseSearchService.Object, 
				mockCaseHistoryCachedService.Object, mockLogger.Object);

			Assert.ThrowsAsync<NullReferenceException>(() => caseModelService.PatchNextSteps(CaseFactory.BuildPatchCaseModel()));

			// assert
			mockCaseCachedService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Never);
		}
	}
}