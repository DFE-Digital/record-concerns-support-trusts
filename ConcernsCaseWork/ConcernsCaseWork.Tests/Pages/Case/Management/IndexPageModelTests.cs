using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Pages.Case.Management;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.FinancialPlan;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.NtiUnderConsideration;
using Service.Redis.Status;
using Service.TRAMS.Status;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management
{
	[Parallelizable(ParallelScope.All)]
	public class IndexPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_MissingCaseUrn_ThrowsException_ReturnPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
			var mockCaseHistoryModelService = new Mock<ICaseHistoryModelService>();
			var mockSrmaService = new Mock<ISRMAService>();
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockNtiStatusesCachedService = new Mock<INtiStatusesCachedService>();

			var pageModel = SetupIndexPageModel(mockCaseModelService.Object, mockTrustModelService.Object,
				mockCaseHistoryModelService.Object, mockRecordModelService.Object, mockRatingModelService.Object, mockStatusCachedService.Object, 
				mockSrmaService.Object, mockFinancialPlanModelService.Object, mockNtiModelService.Object, mockNtiStatusesCachedService.Object, mockLogger.Object);

			// act
			await pageModel.OnGetAsync();
			
			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
			
			mockCaseModelService.Verify(c => 
				c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
			mockCaseHistoryModelService.Verify(c => 
				c.GetCasesHistory(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
			mockTrustModelService.Verify(c => 
				c.GetTrustByUkPrn(It.IsAny<string>()), Times.Never);
			mockCaseModelService.Verify(c => 
				c.GetCasesByTrustUkprn(It.IsAny<string>()), Times.Never);
		}
		
		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
			var mockCaseHistoryModelService = new Mock<ICaseHistoryModelService>();
			var mockSrmaService = new Mock<ISRMAService>();
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockNtiStatusesCachedService = new Mock<INtiStatusesCachedService>();

			var caseModel = CaseFactory.BuildCaseModel();
			var trustCasesModel = CaseFactory.BuildListTrustCasesModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			var casesHistoryModel = CaseFactory.BuildListCasesHistoryModel();
			var recordsModel = RecordFactory.BuildListRecordModel();
			var closedStatusModel = StatusFactory.BuildStatusDto(StatusEnum.Close.ToString(), 3);
			var financialPlansModel = FinancialPlanFactory.BuildListFinancialPlanModel();
			
			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			mockCaseModelService.Setup(c => c.GetCasesByTrustUkprn(It.IsAny<string>()))
				.ReturnsAsync(trustCasesModel);
			mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);
			mockCaseHistoryModelService.Setup(c => c.GetCasesHistory(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(casesHistoryModel);
			mockRecordModelService.Setup(r => r.GetRecordsModelByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(recordsModel);
			mockStatusCachedService.Setup(s => s.GetStatusByName(It.IsAny<string>()))
				.ReturnsAsync(closedStatusModel);
			mockFinancialPlanModelService.Setup(fp => fp.GetFinancialPlansModelByCaseUrn(It.IsAny<long>(), It.IsAny<string>()))
				.ReturnsAsync(financialPlansModel);
			
			var pageModel = SetupIndexPageModel(mockCaseModelService.Object, mockTrustModelService.Object,
					mockCaseHistoryModelService.Object, mockRecordModelService.Object, mockRatingModelService.Object, mockStatusCachedService.Object, 
					mockSrmaService.Object, mockFinancialPlanModelService.Object, mockNtiModelService.Object, mockNtiStatusesCachedService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			
			// act
			await pageModel.OnGetAsync();
			
			// assert
			Assert.That(pageModel.CaseModel, Is.Not.Null);
			Assert.That(pageModel.CaseModel.Description, Is.EqualTo(caseModel.Description));
			Assert.That(pageModel.CaseModel.Issue, Is.EqualTo(caseModel.Issue));
			Assert.That(pageModel.CaseModel.StatusUrn, Is.EqualTo(caseModel.StatusUrn));
			Assert.That(pageModel.CaseModel.Urn, Is.EqualTo(caseModel.Urn));
			Assert.That(pageModel.CaseModel.ClosedAt, Is.EqualTo(caseModel.ClosedAt));
			Assert.That(pageModel.CaseModel.CreatedAt, Is.EqualTo(caseModel.CreatedAt));
			Assert.That(pageModel.CaseModel.CreatedBy, Is.EqualTo(caseModel.CreatedBy));
			Assert.That(pageModel.CaseModel.CrmEnquiry, Is.EqualTo(caseModel.CrmEnquiry));
			Assert.That(pageModel.CaseModel.CurrentStatus, Is.EqualTo(caseModel.CurrentStatus));
			Assert.That(pageModel.CaseModel.DeEscalation, Is.EqualTo(caseModel.DeEscalation));
			Assert.That(pageModel.CaseModel.NextSteps, Is.EqualTo(caseModel.NextSteps));
			Assert.That(pageModel.CaseModel.CaseAim, Is.EqualTo(caseModel.CaseAim));
			Assert.That(pageModel.CaseModel.DeEscalationPoint, Is.EqualTo(caseModel.DeEscalationPoint));
			Assert.That(pageModel.CaseModel.ReviewAt, Is.EqualTo(caseModel.ReviewAt));
			Assert.That(pageModel.CaseModel.StatusName, Is.EqualTo(caseModel.StatusName));
			Assert.That(pageModel.TrustDetailsModel.GiasData.GroupName, Is.EqualTo(trustDetailsModel.GiasData.GroupName));
			Assert.That(pageModel.TrustDetailsModel.GiasData.GroupNameTitle, Is.EqualTo(trustDetailsModel.GiasData.GroupName.ToTitle()));
			Assert.That(pageModel.TrustDetailsModel, Is.EqualTo(trustDetailsModel));
			Assert.True(pageModel.TrustDetailsModel.Establishments[0].EstablishmentWebsite.Contains("http"));
			Assert.That(pageModel.CaseModel.UpdatedAt, Is.EqualTo(caseModel.UpdatedAt));
			Assert.That(pageModel.CaseModel.DirectionOfTravel, Is.EqualTo(caseModel.DirectionOfTravel));
			Assert.That(pageModel.CaseModel.ReasonAtReview, Is.EqualTo(caseModel.ReasonAtReview));
			Assert.That(pageModel.CaseModel.TrustUkPrn, Is.EqualTo(caseModel.TrustUkPrn));
			
			Assert.That(pageModel.TrustCasesModel, Is.Not.Null);
			Assert.That(pageModel.TrustCasesModel.Count, Is.EqualTo(1));

			var actualFirstTrustCaseModel = trustCasesModel.First();
			var expectedFirstTrustCaseModel = pageModel.TrustCasesModel.First();
			Assert.That(expectedFirstTrustCaseModel.CaseUrn, Is.EqualTo(actualFirstTrustCaseModel.CaseUrn));
			Assert.That(expectedFirstTrustCaseModel.RecordsModel, Is.EqualTo(actualFirstTrustCaseModel.RecordsModel));
			Assert.That(expectedFirstTrustCaseModel.Created, Is.EqualTo(actualFirstTrustCaseModel.Created));
			Assert.That(expectedFirstTrustCaseModel.RatingModel, Is.EqualTo(actualFirstTrustCaseModel.RatingModel));

			Assert.That(pageModel.CasesHistoryModel, Is.Not.Null);
			Assert.That(pageModel.CasesHistoryModel.Count, Is.EqualTo(1));

			var actualFirstCaseHistoryModel = casesHistoryModel.First();
			var expectedFirstCaseHistoryModel = pageModel.CasesHistoryModel.First();
			Assert.That(expectedFirstCaseHistoryModel.CaseUrn, Is.EqualTo(actualFirstCaseHistoryModel.CaseUrn));
			Assert.That(expectedFirstCaseHistoryModel.Action, Is.EqualTo(actualFirstCaseHistoryModel.Action));
			Assert.That(expectedFirstCaseHistoryModel.Description, Is.EqualTo(actualFirstCaseHistoryModel.Description));
			Assert.That(expectedFirstCaseHistoryModel.Title, Is.EqualTo(actualFirstCaseHistoryModel.Title));
			Assert.That(expectedFirstCaseHistoryModel.Urn, Is.EqualTo(actualFirstCaseHistoryModel.Urn));
			Assert.That(expectedFirstCaseHistoryModel.CreatedAt, Is.EqualTo(actualFirstCaseHistoryModel.CreatedAt));

			var expectedRecordsModel = pageModel.CaseModel.RecordsModel;

			for (var index = 0; index < expectedRecordsModel.Count; ++index)
			{
				Assert.That(expectedRecordsModel.ElementAt(index).Urn, Is.EqualTo(recordsModel.ElementAt(index).Urn));
				Assert.That(expectedRecordsModel.ElementAt(index).CaseUrn, Is.EqualTo(recordsModel.ElementAt(index).CaseUrn));
				Assert.That(expectedRecordsModel.ElementAt(index).RatingUrn, Is.EqualTo(recordsModel.ElementAt(index).RatingUrn));
				Assert.That(expectedRecordsModel.ElementAt(index).StatusUrn, Is.EqualTo(recordsModel.ElementAt(index).StatusUrn));
				Assert.That(expectedRecordsModel.ElementAt(index).TypeUrn, Is.EqualTo(recordsModel.ElementAt(index).TypeUrn));
				
				var expectedRecordRatingModel = expectedRecordsModel.ElementAt(index).RatingModel;
				var actualRecordRatingModel = recordsModel.ElementAt(index).RatingModel;
				Assert.NotNull(expectedRecordRatingModel);
				Assert.NotNull(actualRecordRatingModel);
				Assert.That(expectedRecordRatingModel.Checked, Is.EqualTo(actualRecordRatingModel.Checked));
				Assert.That(expectedRecordRatingModel.Name, Is.EqualTo(actualRecordRatingModel.Name));
				Assert.That(expectedRecordRatingModel.Urn, Is.EqualTo(actualRecordRatingModel.Urn));
				Assert.That(expectedRecordRatingModel.RagRating, Is.EqualTo(actualRecordRatingModel.RagRating));
				Assert.That(expectedRecordRatingModel.RagRatingCss, Is.EqualTo(actualRecordRatingModel.RagRatingCss));
				
				var expectedRecordTypeModel = expectedRecordsModel.ElementAt(index).TypeModel;
				var actualRecordTypeModel = recordsModel.ElementAt(index).TypeModel;
				Assert.NotNull(expectedRecordTypeModel);
				Assert.NotNull(actualRecordTypeModel);
				Assert.That(expectedRecordTypeModel.Type, Is.EqualTo(actualRecordTypeModel.Type));
				Assert.That(expectedRecordTypeModel.SubType, Is.EqualTo(actualRecordTypeModel.SubType));
				Assert.That(expectedRecordTypeModel.TypeDisplay, Is.EqualTo(actualRecordTypeModel.TypeDisplay));
				Assert.That(expectedRecordTypeModel.TypesDictionary, Is.EqualTo(actualRecordTypeModel.TypesDictionary));

				var expectedRecordStatusModel = expectedRecordsModel.ElementAt(index).StatusModel;
				var actualRecordStatusModel = recordsModel.ElementAt(index).StatusModel;
				Assert.NotNull(expectedRecordStatusModel);
				Assert.NotNull(actualRecordTypeModel);
				Assert.That(expectedRecordStatusModel.Name, Is.EqualTo(actualRecordStatusModel.Name));
				Assert.That(expectedRecordStatusModel.Urn, Is.EqualTo(actualRecordStatusModel.Urn));
			}
		}

		[Test]
		public async Task WhenUserHasEditCasePrivileges_ShowEditActions_Return_False()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var closeStatusModel = StatusFactory.BuildStatusDto(StatusEnum.Close.ToString(), 3);
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
			var mockCaseHistoryModelService = new Mock<ICaseHistoryModelService>();
			var mockSrmaService = new Mock<ISRMAService>();
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockNtiStatusesCachedService = new Mock<INtiStatusesCachedService>();

			var caseModel = CaseFactory.BuildCaseModel();
			var trustCasesModel = CaseFactory.BuildListTrustCasesModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			var casesHistoryModel = CaseFactory.BuildListCasesHistoryModel();
			var recordsModel = RecordFactory.BuildListRecordModel();

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			mockCaseModelService.Setup(c => c.GetCasesByTrustUkprn(It.IsAny<string>()))
				.ReturnsAsync(trustCasesModel);
			mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);
			mockCaseHistoryModelService.Setup(c => c.GetCasesHistory(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(casesHistoryModel);
			mockRecordModelService.Setup(r => r.GetRecordsModelByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(recordsModel);

			var pageModel = SetupIndexPageModel(mockCaseModelService.Object, mockTrustModelService.Object,
				mockCaseHistoryModelService.Object, mockRecordModelService.Object, mockRatingModelService.Object, mockStatusCachedService.Object, 
				mockSrmaService.Object, mockFinancialPlanModelService.Object, mockNtiModelService.Object, mockNtiStatusesCachedService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			
			// act
			await pageModel.OnGetAsync();
			var showEditActions = pageModel.IsEditableCase;
			
			// assert
			Assert.False(showEditActions);
		}
		
		[Test]
		public async Task WhenUserHasEditCasePrivileges_ShowEditActions_Return_True()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
			var mockCaseHistoryModelService = new Mock<ICaseHistoryModelService>();
			var mockSrmaService = new Mock<ISRMAService>();
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockNtiStatusesCachedService = new Mock<INtiStatusesCachedService>();


			var caseModel = CaseFactory.BuildCaseModel("Tester");
			var trustCasesModel = CaseFactory.BuildListTrustCasesModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			var casesHistoryModel = CaseFactory.BuildListCasesHistoryModel();
			var recordsModel = RecordFactory.BuildListRecordModel();
			var closeStatusModel = StatusFactory.BuildStatusDto(StatusEnum.Close.ToString(), 3);

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			mockCaseModelService.Setup(c => c.GetCasesByTrustUkprn(It.IsAny<string>()))
				.ReturnsAsync(trustCasesModel);
			mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);
			mockCaseHistoryModelService.Setup(c => c.GetCasesHistory(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(casesHistoryModel);
			mockRecordModelService.Setup(r => r.GetRecordsModelByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(recordsModel);
			mockStatusCachedService.Setup(s => s.GetStatusByName(It.IsAny<string>()))
				.ReturnsAsync(closeStatusModel);

			var pageModel = SetupIndexPageModel(mockCaseModelService.Object, mockTrustModelService.Object,
				mockCaseHistoryModelService.Object, mockRecordModelService.Object, mockRatingModelService.Object, mockStatusCachedService.Object, 
				mockSrmaService.Object, mockFinancialPlanModelService.Object, mockNtiModelService.Object, mockNtiStatusesCachedService.Object, mockLogger.Object, isAuthenticated:true);
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			
			// act
			await pageModel.OnGetAsync();
			var showEditActions = pageModel.IsEditableCase;
			
			// assert
			Assert.True(showEditActions);
		}

		[Test]
		public async Task WhenCaseIsClosed_ShowEditActions_Return_False()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
			var mockCaseHistoryModelService = new Mock<ICaseHistoryModelService>();
			var mockSrmaService = new Mock<ISRMAService>();
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockNtiStatusesCachedService = new Mock<INtiStatusesCachedService>();

			var caseModel = CaseFactory.BuildCaseModel("Tester", 3);
			var trustCasesModel = CaseFactory.BuildListTrustCasesModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			var casesHistoryModel = CaseFactory.BuildListCasesHistoryModel();
			var recordsModel = RecordFactory.BuildListRecordModel();
			var closeStatusModel = StatusFactory.BuildStatusDto(StatusEnum.Close.ToString(), 3);

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			mockCaseModelService.Setup(c => c.GetCasesByTrustUkprn(It.IsAny<string>()))
				.ReturnsAsync(trustCasesModel);
			mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);
			mockCaseHistoryModelService.Setup(c => c.GetCasesHistory(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(casesHistoryModel);
			mockRecordModelService.Setup(r => r.GetRecordsModelByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(recordsModel);
			mockStatusCachedService.Setup(s => s.GetStatusByName(It.IsAny<string>()))
				.ReturnsAsync(closeStatusModel);

			var pageModel = SetupIndexPageModel(mockCaseModelService.Object, mockTrustModelService.Object,
				mockCaseHistoryModelService.Object, mockRecordModelService.Object, mockRatingModelService.Object, mockStatusCachedService.Object, 
				mockSrmaService.Object, mockFinancialPlanModelService.Object, mockNtiModelService.Object, mockNtiStatusesCachedService.Object, mockLogger.Object, isAuthenticated:true);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			// act
			await pageModel.OnGetAsync();
			var showEditActions = pageModel.IsEditableCase;

			// assert
			Assert.False(showEditActions);
		}


		private static IndexPageModel SetupIndexPageModel(
			ICaseModelService mockCaseModelService, 
			ITrustModelService mockTrustModelService,
			ICaseHistoryModelService mockCaseHistoryModelService,
			IRecordModelService mockRecordModelService,
			IRatingModelService mockRatingModelService,
			IStatusCachedService mockStatusCachedService,
			ISRMAService mockSrmaService, 
			IFinancialPlanModelService mockFinancialPlanModelService,
			INtiModelService mockNtiModelService,
			INtiStatusesCachedService mockNtiStatusesCachedService,
			ILogger<IndexPageModel> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new IndexPageModel(mockCaseModelService, mockTrustModelService, mockCaseHistoryModelService, mockRecordModelService, mockRatingModelService, 
				mockStatusCachedService, mockSrmaService, mockFinancialPlanModelService, mockNtiModelService, mockNtiStatusesCachedService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}