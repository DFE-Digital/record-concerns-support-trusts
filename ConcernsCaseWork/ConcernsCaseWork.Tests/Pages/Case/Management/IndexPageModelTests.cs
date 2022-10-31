using AutoFixture;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Case.Management;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Decisions;
using ConcernsCaseWork.Services.FinancialPlan;
using ConcernsCaseWork.Services.Nti;
using ConcernsCaseWork.Services.NtiWarningLetter;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Shared.Tests.Factory;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.NtiUnderConsideration;
using Service.Redis.Status;
using Service.TRAMS.Decision;
using Service.TRAMS.NtiUnderConsideration;
using Service.TRAMS.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management
{
	[Parallelizable(ParallelScope.Fixtures)]
	[TestFixture]
	public class IndexPageModelTests
	{
		private Mock<ICaseModelService> _mockCaseModelService = null;
		private Mock<ITrustModelService> _mockTrustModelService = null;
		private Mock<IRecordModelService> _mockRecordModelService = null;
		private Mock<IRatingModelService> _mockRatingModelService = null;
		private Mock<IStatusCachedService> _mockStatusCachedService = null;
		private Mock<ILogger<IndexPageModel>> _mockLogger = null;
		private Mock<ISRMAService> _mockSrmaService = null;
		private Mock<IFinancialPlanModelService> _mockFinancialPlanModelService = null;
		private Mock<INtiUnderConsiderationStatusesCachedService> _mockNtiStatusesCachedService = null;
		private Mock<INtiUnderConsiderationModelService> _mockNtiUnderConsiderationModelService = null;
		private Mock<INtiWarningLetterModelService> _mockNtiWLModelService = null;
		private Mock<INtiModelService> _mockNtiModelService = null;
		private Mock<IDecisionModelService> _mockDecisionService = null;

		private readonly static Fixture _fixture = new();

		[SetUp]
		public void SetUp()
		{
			_mockCaseModelService = new Mock<ICaseModelService>();
			_mockTrustModelService = new Mock<ITrustModelService>();
			_mockRecordModelService = new Mock<IRecordModelService>();
			_mockRatingModelService = new Mock<IRatingModelService>();
			_mockStatusCachedService = new Mock<IStatusCachedService>();
			_mockLogger = new Mock<ILogger<IndexPageModel>>();
			_mockSrmaService = new Mock<ISRMAService>();
			_mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			_mockNtiStatusesCachedService = new Mock<INtiUnderConsiderationStatusesCachedService>();
			_mockNtiUnderConsiderationModelService = new Mock<INtiUnderConsiderationModelService>();
			_mockNtiWLModelService = new Mock<INtiWarningLetterModelService>();
			_mockNtiModelService = new Mock<INtiModelService>();
			_mockDecisionService = new Mock<IDecisionModelService>();

		}

		[Test]
		public async Task WhenOnGetAsync_MissingCaseUrn_ThrowsException_ReturnPage()
		{
			// arrange
			var pageModel = SetupIndexPageModel();

			// act
			await pageModel.OnGetAsync();
			
			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
			
			_mockCaseModelService.Verify(c => 
				c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
			_mockTrustModelService.Verify(c => 
				c.GetTrustByUkPrn(It.IsAny<string>()), Times.Never);
			_mockCaseModelService.Verify(c => 
				c.GetCasesByTrustUkprn(It.IsAny<string>()), Times.Never);
		}
		
		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			var caseModel = CaseFactory.BuildCaseModel();
			var trustCasesModel = CaseFactory.BuildListTrustCasesModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			var recordsModel = RecordFactory.BuildListRecordModel();
			var closedStatusModel = StatusFactory.BuildStatusDto(StatusEnum.Close.ToString(), 3);
			var financialPlansModel = FinancialPlanFactory.BuildListFinancialPlanModel();
			var ntiUnderConsiderationModels = NTIUnderConsiderationFactory.BuildListNTIUnderConsiderationModel();
			var ntiWarningLetterModels = NTIWarningLetterFactory.BuildListNTIWarningLetterModels(3);
			var ntiModels = NTIFactory.BuildListNTIModel();

			SetupDefaultModels();
			
			_mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			_mockCaseModelService.Setup(c => c.GetCasesByTrustUkprn(It.IsAny<string>()))
				.ReturnsAsync(trustCasesModel);
			_mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);
			_mockRecordModelService.Setup(r => r.GetRecordsModelByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(recordsModel);
			_mockStatusCachedService.Setup(s => s.GetStatusByName(It.IsAny<string>()))
				.ReturnsAsync(closedStatusModel);
			_mockFinancialPlanModelService.Setup(fp => fp.GetFinancialPlansModelByCaseUrn(It.IsAny<long>(), It.IsAny<string>()))
				.ReturnsAsync(financialPlansModel);
			_mockNtiUnderConsiderationModelService.Setup(uc => uc.GetNtiUnderConsiderationsForCase(It.IsAny<long>()))
				.ReturnsAsync(ntiUnderConsiderationModels);
			_mockNtiWLModelService.Setup(wl => wl.GetNtiWarningLettersForCase(It.IsAny<long>()))
				.ReturnsAsync(ntiWarningLetterModels);
			_mockNtiModelService.Setup(nti => nti.GetNtisForCaseAsync(It.IsAny<long>()))
				.ReturnsAsync(ntiModels);

			
			var pageModel = SetupIndexPageModel();

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

			var expectedCountCaseActions = financialPlansModel.Count + ntiUnderConsiderationModels.Count + ntiWarningLetterModels.Count + ntiModels.Count;
			Assert.That(pageModel.CaseActions.Count, Is.EqualTo(expectedCountCaseActions));
			
			var expectedHasOpenCaseActions = financialPlansModel.Any(m => !m.ClosedAt.HasValue) 
			                                   || ntiUnderConsiderationModels.Any(m => !m.ClosedAt.HasValue) 
			                                   || ntiWarningLetterModels.Any(m => !m.ClosedAt.HasValue) 
			                                   || ntiModels.Any(m => !m.ClosedAt.HasValue);
			Assert.That(pageModel.HasOpenActions, Is.EqualTo(expectedHasOpenCaseActions));
			
			var expectedHasClosedCaseActions = financialPlansModel.Any(m => m.ClosedAt.HasValue) 
			                                 || ntiUnderConsiderationModels.Any(m => m.ClosedAt.HasValue) 
			                                 || ntiWarningLetterModels.Any(m => m.ClosedAt.HasValue) 
			                                 || ntiModels.Any(m => m.ClosedAt.HasValue);
			Assert.That(pageModel.HasClosedActions, Is.EqualTo(expectedHasClosedCaseActions));
			
			Assert.That(pageModel.TrustCasesModel, Is.Not.Null);
			Assert.That(pageModel.TrustCasesModel.Count, Is.EqualTo(1));

			var actualFirstTrustCaseModel = trustCasesModel.First();
			var expectedFirstTrustCaseModel = pageModel.TrustCasesModel.First();
			Assert.That(expectedFirstTrustCaseModel.CaseUrn, Is.EqualTo(actualFirstTrustCaseModel.CaseUrn));
			Assert.That(expectedFirstTrustCaseModel.RecordsModel, Is.EqualTo(actualFirstTrustCaseModel.RecordsModel));
			Assert.That(expectedFirstTrustCaseModel.Created, Is.EqualTo(actualFirstTrustCaseModel.Created));
			Assert.That(expectedFirstTrustCaseModel.RatingModel, Is.EqualTo(actualFirstTrustCaseModel.RatingModel));

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
		public async Task WhenOnGetAsync_WhenActiveDecisionsExist_ActiveDecisionsAreShownOnPage()
		{
			// arrange
			var urn = 3;

			SetupDefaultModels();

			var decisions = _fixture.CreateMany<DecisionModel>().ToList();

			_mockDecisionService.Setup(m => m.GetDecisionsByUrn(It.IsAny<long>())).ReturnsAsync(decisions);

			var pageModel = SetupIndexPageModel();
			pageModel.RouteData.Values.Add("urn", urn);

			// act
			var page = await pageModel.OnGetAsync();

			// assert
			PageLoadedWithoutError(pageModel);

			var result = pageModel.CaseActions.Where(c => c is DecisionModel).ToList();

			decisions.Should().BeEquivalentTo(decisions);
		}

		[Test]
		public async Task WhenOnGetAsync_WhenCaseIsClosed_RedirectsToClosedCasePage()
		{
			// arrange
			var closedStatusUrn = 3;

			var caseModel = CaseFactory.BuildCaseModel(statusUrn:closedStatusUrn);
			var trustCasesModel = CaseFactory.BuildListTrustCasesModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			var recordsModel = RecordFactory.BuildListRecordModel();
			var closedStatusModel = StatusFactory.BuildStatusDto(StatusEnum.Close.ToString(), closedStatusUrn);
			var financialPlansModel = FinancialPlanFactory.BuildListFinancialPlanModel();
			var ntiUnderConsiderationModels = NTIUnderConsiderationFactory.BuildListNTIUnderConsiderationModel();
			var ntiWarningLetterModels = NTIWarningLetterFactory.BuildListNTIWarningLetterModels(3);
			var ntiModels = NTIFactory.BuildListNTIModel();
			
			_mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			_mockCaseModelService.Setup(c => c.GetCasesByTrustUkprn(It.IsAny<string>()))
				.ReturnsAsync(trustCasesModel);
			_mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);
			_mockRecordModelService.Setup(r => r.GetRecordsModelByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(recordsModel);
			_mockStatusCachedService.Setup(s => s.GetStatusByName(StatusEnum.Close.ToString()))
				.ReturnsAsync(closedStatusModel);
			_mockFinancialPlanModelService.Setup(fp => fp.GetFinancialPlansModelByCaseUrn(It.IsAny<long>(), It.IsAny<string>()))
				.ReturnsAsync(financialPlansModel);
			_mockNtiUnderConsiderationModelService.Setup(uc => uc.GetNtiUnderConsiderationsForCase(It.IsAny<long>()))
				.ReturnsAsync(ntiUnderConsiderationModels);
			_mockNtiWLModelService.Setup(wl => wl.GetNtiWarningLettersForCase(It.IsAny<long>()))
				.ReturnsAsync(ntiWarningLetterModels);
			_mockNtiModelService.Setup(nti => nti.GetNtisForCaseAsync(It.IsAny<long>()))
				.ReturnsAsync(ntiModels);
			
			var pageModel = SetupIndexPageModel();

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			
			// act
			var result = await pageModel.OnGetAsync();
			
			// assert
			Assert.That(result, Is.TypeOf<RedirectResult>());
			Assert.That(((RedirectResult)result).Url, Is.EqualTo($"/case/{caseModel.Urn}/closed"));
		}
		
		[Test]
		public async Task WhenOnGetAsync_WithNoCaseActions_SetsHasOpenActionsAndHasClosedActionsToFalse()
		{
			// arrange
			var caseModel = CaseFactory.BuildCaseModel();
			var trustCasesModel = CaseFactory.BuildListTrustCasesModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			var recordsModel = RecordFactory.BuildListRecordModel();
			var closedStatusModel = StatusFactory.BuildStatusDto(StatusEnum.Close.ToString(), 3);

			_mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			_mockCaseModelService.Setup(c => c.GetCasesByTrustUkprn(It.IsAny<string>()))
				.ReturnsAsync(trustCasesModel);
			_mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);
			_mockRecordModelService.Setup(r => r.GetRecordsModelByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(recordsModel);
			_mockStatusCachedService.Setup(s => s.GetStatusByName(It.IsAny<string>()))
				.ReturnsAsync(closedStatusModel);
			_mockFinancialPlanModelService.Setup(fp => fp.GetFinancialPlansModelByCaseUrn(It.IsAny<long>(), It.IsAny<string>()))
				.ReturnsAsync(new List<FinancialPlanModel>());
			_mockNtiUnderConsiderationModelService.Setup(uc => uc.GetNtiUnderConsiderationsForCase(It.IsAny<long>()))
				.ReturnsAsync(new List<NtiUnderConsiderationModel>());
			_mockNtiWLModelService.Setup(wl => wl.GetNtiWarningLettersForCase(It.IsAny<long>()))
				.ReturnsAsync(new List<NtiWarningLetterModel>());
			_mockNtiModelService.Setup(nti => nti.GetNtisForCaseAsync(It.IsAny<long>()))
				.ReturnsAsync(new List<NtiModel>());

			var pageModel = SetupIndexPageModel();

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			
			// act
			await pageModel.OnGetAsync();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(pageModel.CaseActions.Count, Is.EqualTo(0));
				Assert.That(pageModel.HasOpenActions, Is.EqualTo(false));
				Assert.That(pageModel.HasClosedActions, Is.EqualTo(false));
			});
		}

		[Test]
		public async Task WhenOnGetAsync_WithOnlyOpenCaseActions_SetsHasOpenActionsToTrueAndHasClosedActionsToFalse()
		{
			// arrange
			var caseModel = CaseFactory.BuildCaseModel();
			var trustCasesModel = CaseFactory.BuildListTrustCasesModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			var recordsModel = RecordFactory.BuildListRecordModel();
			var closedStatusModel = StatusFactory.BuildStatusDto(StatusEnum.Close.ToString(), 3);
			var ntiWarningLetterModels = NTIWarningLetterFactory.BuildListNTIWarningLetterModels(3);

			_mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			_mockCaseModelService.Setup(c => c.GetCasesByTrustUkprn(It.IsAny<string>()))
				.ReturnsAsync(trustCasesModel);
			_mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);
			_mockRecordModelService.Setup(r => r.GetRecordsModelByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(recordsModel);
			_mockStatusCachedService.Setup(s => s.GetStatusByName(It.IsAny<string>()))
				.ReturnsAsync(closedStatusModel);
			_mockFinancialPlanModelService.Setup(fp => fp.GetFinancialPlansModelByCaseUrn(It.IsAny<long>(), It.IsAny<string>()))
				.ReturnsAsync(new List<FinancialPlanModel>());
			_mockNtiUnderConsiderationModelService.Setup(uc => uc.GetNtiUnderConsiderationsForCase(It.IsAny<long>()))
				.ReturnsAsync(new List<NtiUnderConsiderationModel>());
			_mockNtiWLModelService.Setup(wl => wl.GetNtiWarningLettersForCase(It.IsAny<long>()))
				.ReturnsAsync(ntiWarningLetterModels);
			_mockNtiModelService.Setup(nti => nti.GetNtisForCaseAsync(It.IsAny<long>()))
				.ReturnsAsync(new List<NtiModel>());

			var pageModel = SetupIndexPageModel();

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			
			// act
			await pageModel.OnGetAsync();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(pageModel.CaseActions.Count, Is.EqualTo(3));
				Assert.That(pageModel.HasOpenActions, Is.EqualTo(true));
				Assert.That(pageModel.HasClosedActions, Is.EqualTo(false));
			});
		}

		[Test]
		public async Task WhenOnGetAsync_WithOnlyClosedCaseActions_SetsHasOpenActionsToFalseAndHasClosedActionsToTrue()
		{
			// arrange
			var caseModel = CaseFactory.BuildCaseModel();
			var trustCasesModel = CaseFactory.BuildListTrustCasesModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			var recordsModel = RecordFactory.BuildListRecordModel();
			var closedStatusModel = StatusFactory.BuildStatusDto(StatusEnum.Close.ToString(), 3);
			var ntiWarningLetterModels = NTIWarningLetterFactory.BuildListNTIWarningLetterModels(3, DateTime.Now);

			_mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			_mockCaseModelService.Setup(c => c.GetCasesByTrustUkprn(It.IsAny<string>()))
				.ReturnsAsync(trustCasesModel);
			_mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);
			_mockRecordModelService.Setup(r => r.GetRecordsModelByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(recordsModel);
			_mockStatusCachedService.Setup(s => s.GetStatusByName(It.IsAny<string>()))
				.ReturnsAsync(closedStatusModel);
			_mockFinancialPlanModelService.Setup(fp => fp.GetFinancialPlansModelByCaseUrn(It.IsAny<long>(), It.IsAny<string>()))
				.ReturnsAsync(new List<FinancialPlanModel>());
			_mockNtiUnderConsiderationModelService.Setup(uc => uc.GetNtiUnderConsiderationsForCase(It.IsAny<long>()))
				.ReturnsAsync(new List<NtiUnderConsiderationModel>());
			_mockNtiWLModelService.Setup(wl => wl.GetNtiWarningLettersForCase(It.IsAny<long>()))
				.ReturnsAsync(ntiWarningLetterModels);
			_mockNtiModelService.Setup(nti => nti.GetNtisForCaseAsync(It.IsAny<long>()))
				.ReturnsAsync(new List<NtiModel>());

			var pageModel = SetupIndexPageModel();

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			
			// act
			await pageModel.OnGetAsync();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(pageModel.CaseActions.Count, Is.EqualTo(3));
				Assert.That(pageModel.HasOpenActions, Is.EqualTo(false));
				Assert.That(pageModel.HasClosedActions, Is.EqualTo(true));
			});
		}

		[Test]
		public async Task WhenOnGetAsync_WithOpenAndClosedCaseActions_SetsHasOpenActionsAndHasClosedActionsToTrue()
		{
			// arrange
			var caseModel = CaseFactory.BuildCaseModel();
			var trustCasesModel = CaseFactory.BuildListTrustCasesModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			var recordsModel = RecordFactory.BuildListRecordModel();
			var closedStatusModel = StatusFactory.BuildStatusDto(StatusEnum.Close.ToString(), 3);
			var ntiWarningLetterModels = NTIWarningLetterFactory.BuildListNTIWarningLetterModels(3);			
			var closedNtiModels = NTIFactory.BuildClosedListNTIModel();

			_mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			_mockCaseModelService.Setup(c => c.GetCasesByTrustUkprn(It.IsAny<string>()))
				.ReturnsAsync(trustCasesModel);
			_mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);
			_mockRecordModelService.Setup(r => r.GetRecordsModelByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(recordsModel);
			_mockStatusCachedService.Setup(s => s.GetStatusByName(It.IsAny<string>()))
				.ReturnsAsync(closedStatusModel);
			_mockFinancialPlanModelService.Setup(fp => fp.GetFinancialPlansModelByCaseUrn(It.IsAny<long>(), It.IsAny<string>()))
				.ReturnsAsync(new List<FinancialPlanModel>());
			_mockNtiUnderConsiderationModelService.Setup(uc => uc.GetNtiUnderConsiderationsForCase(It.IsAny<long>()))
				.ReturnsAsync(new List<NtiUnderConsiderationModel>());
			_mockNtiWLModelService.Setup(wl => wl.GetNtiWarningLettersForCase(It.IsAny<long>()))
				.ReturnsAsync(ntiWarningLetterModels);
			_mockNtiModelService.Setup(nti => nti.GetNtisForCaseAsync(It.IsAny<long>()))
				.ReturnsAsync(closedNtiModels);

			var pageModel = SetupIndexPageModel();

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			
			// act
			await pageModel.OnGetAsync();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(pageModel.CaseActions.Count, Is.EqualTo(5));
				Assert.That(pageModel.HasOpenActions, Is.EqualTo(true));
				Assert.That(pageModel.HasClosedActions, Is.EqualTo(true));
			});
		}

		[Test]
		public async Task WhenUserHasEditCasePrivileges_ShowEditActions_Return_False()
		{
			// arrange
			var caseModel = CaseFactory.BuildCaseModel();
			var trustCasesModel = CaseFactory.BuildListTrustCasesModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			var recordsModel = RecordFactory.BuildListRecordModel();

			_mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			_mockCaseModelService.Setup(c => c.GetCasesByTrustUkprn(It.IsAny<string>()))
				.ReturnsAsync(trustCasesModel);
			_mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);
			_mockRecordModelService.Setup(r => r.GetRecordsModelByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(recordsModel);

			var pageModel = SetupIndexPageModel();

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
			var caseModel = CaseFactory.BuildCaseModel("Tester");
			var trustCasesModel = CaseFactory.BuildListTrustCasesModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			var recordsModel = RecordFactory.BuildListRecordModel();
			var closeStatusModel = StatusFactory.BuildStatusDto(StatusEnum.Close.ToString(), 3);

			_mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			_mockCaseModelService.Setup(c => c.GetCasesByTrustUkprn(It.IsAny<string>()))
				.ReturnsAsync(trustCasesModel);
			_mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);
			_mockRecordModelService.Setup(r => r.GetRecordsModelByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(recordsModel);
			_mockStatusCachedService.Setup(s => s.GetStatusByName(It.IsAny<string>()))
				.ReturnsAsync(closeStatusModel);

			var pageModel = SetupIndexPageModel(isAuthenticated:true);
			
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
			var caseModel = CaseFactory.BuildCaseModel("Tester", 3);
			var trustCasesModel = CaseFactory.BuildListTrustCasesModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			var recordsModel = RecordFactory.BuildListRecordModel();
			var closeStatusModel = StatusFactory.BuildStatusDto(StatusEnum.Close.ToString(), 3);

			_mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			_mockCaseModelService.Setup(c => c.GetCasesByTrustUkprn(It.IsAny<string>()))
				.ReturnsAsync(trustCasesModel);
			_mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);
			_mockRecordModelService.Setup(r => r.GetRecordsModelByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(recordsModel);
			_mockStatusCachedService.Setup(s => s.GetStatusByName(It.IsAny<string>()))
				.ReturnsAsync(closeStatusModel);

			var pageModel = SetupIndexPageModel(isAuthenticated:true);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			// act
			await pageModel.OnGetAsync();
			var showEditActions = pageModel.IsEditableCase;

			// assert
			Assert.False(showEditActions);
		}

		private IndexPageModel SetupIndexPageModel(
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new IndexPageModel(_mockCaseModelService.Object, _mockTrustModelService.Object, _mockRecordModelService.Object, _mockRatingModelService.Object, 
				_mockStatusCachedService.Object, _mockSrmaService.Object, _mockFinancialPlanModelService.Object, _mockNtiUnderConsiderationModelService.Object, _mockNtiStatusesCachedService.Object,
				_mockNtiWLModelService.Object, _mockNtiModelService.Object,
				_mockLogger.Object, _mockDecisionService.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}

		private void SetupDefaultModels()
		{
			var urn = 3;
			var caseModel = CaseFactory.BuildCaseModel("Tester", urn);
			var closedStatusModel = StatusFactory.BuildStatusDto(StatusEnum.Close.ToString(), 1);
			var financialPlansModel = FinancialPlanFactory.BuildListFinancialPlanModel();
			var ntiUnderConsiderationModels = NTIUnderConsiderationFactory.BuildListNTIUnderConsiderationModel();
			var ntiWarningLetterModels = NTIWarningLetterFactory.BuildListNTIWarningLetterModels(3);
			var ntiModels = NTIFactory.BuildListNTIModel();

			_mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			_mockStatusCachedService.Setup(s => s.GetStatusByName(It.IsAny<string>()))
				.ReturnsAsync(closedStatusModel);
			_mockFinancialPlanModelService.Setup(fp => fp.GetFinancialPlansModelByCaseUrn(It.IsAny<long>(), It.IsAny<string>()))
				.ReturnsAsync(financialPlansModel);
			_mockNtiUnderConsiderationModelService.Setup(uc => uc.GetNtiUnderConsiderationsForCase(It.IsAny<long>()))
				.ReturnsAsync(ntiUnderConsiderationModels);
			_mockNtiWLModelService.Setup(wl => wl.GetNtiWarningLettersForCase(It.IsAny<long>()))
				.ReturnsAsync(ntiWarningLetterModels);
			_mockNtiModelService.Setup(nti => nti.GetNtisForCaseAsync(It.IsAny<long>()))
				.ReturnsAsync(ntiModels);
			_mockNtiStatusesCachedService.Setup(s => s.GetAllStatuses())
				.ReturnsAsync(new List<NtiUnderConsiderationStatusDto>());
			_mockDecisionService.Setup(m => m.GetDecisionsByUrn(It.IsAny<long>())).ReturnsAsync(new List<DecisionModel>());
		}

		private void PageLoadedWithoutError(IndexPageModel pageModel)
		{
			pageModel.TempData["Error.Message"].Should().BeNull();
		}
	}
}