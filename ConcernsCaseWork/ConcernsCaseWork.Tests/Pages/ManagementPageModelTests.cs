using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Pages.Case;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Rating;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Services.Type;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages
{
	[Parallelizable(ParallelScope.All)]
	public class ManagementPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_MissingCaseUrn_ThrowsException()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockLogger = new Mock<ILogger<ManagementPageModel>>();
			var mockCaseHistoryModelService = new Mock<ICaseHistoryModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();

			var pageModel = SetupManagementPageModel(mockCaseModelService.Object, mockTrustModelService.Object,
				mockCaseHistoryModelService.Object, mockTypeModelService.Object, mockRecordModelService.Object, mockRatingModelService.Object, mockLogger.Object);

				// act
			await pageModel.OnGetAsync();
			
			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
		}
		
		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockLogger = new Mock<ILogger<ManagementPageModel>>();
			var mockCaseHistoryModelService = new Mock<ICaseHistoryModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			
			var caseModel = CaseFactory.BuildCaseModel();
			var trustCasesModel = CaseFactory.BuildListTrustCasesModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			var casesHistoryModel = CaseFactory.BuildListCasesHistoryModel();

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			mockCaseModelService.Setup(c => c.GetCasesByTrustUkprn(It.IsAny<string>()))
				.ReturnsAsync(trustCasesModel);
			mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);
			mockCaseHistoryModelService.Setup(c => c.GetCasesHistory(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(casesHistoryModel);
			
			var pageModel = SetupManagementPageModel(mockCaseModelService.Object, mockTrustModelService.Object,
					mockCaseHistoryModelService.Object, mockTypeModelService.Object, mockRecordModelService.Object, mockRatingModelService.Object,  mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("id", 1);
			
			// act
			await pageModel.OnGetAsync();
			
			// assert
			Assert.That(pageModel.CaseModel, Is.Not.Null);
			Assert.That(pageModel.CaseModel.Description, Is.EqualTo(caseModel.Description));
			Assert.That(pageModel.CaseModel.Issue, Is.EqualTo(caseModel.Issue));
			Assert.That(pageModel.CaseModel.StatusUrn, Is.EqualTo(caseModel.StatusUrn));
			Assert.That(pageModel.CaseModel.Urn, Is.EqualTo(caseModel.Urn));
			// Assert.That(pageModel.CaseModel.CaseType, Is.EqualTo(caseModel.CaseType));
			Assert.That(pageModel.CaseModel.ClosedAt, Is.EqualTo(caseModel.ClosedAt));
			Assert.That(pageModel.CaseModel.CreatedAt, Is.EqualTo(caseModel.CreatedAt));
			Assert.That(pageModel.CaseModel.CreatedBy, Is.EqualTo(caseModel.CreatedBy));
			Assert.That(pageModel.CaseModel.CrmEnquiry, Is.EqualTo(caseModel.CrmEnquiry));
			Assert.That(pageModel.CaseModel.CurrentStatus, Is.EqualTo(caseModel.CurrentStatus));
			Assert.That(pageModel.CaseModel.DeEscalation, Is.EqualTo(caseModel.DeEscalation));
			Assert.That(pageModel.CaseModel.NextSteps, Is.EqualTo(caseModel.NextSteps));
			//Assert.That(pageModel.CaseModel.RagRating, Is.EqualTo(caseModel.RagRating)); //TODOEA
			Assert.That(pageModel.CaseModel.CaseAim, Is.EqualTo(caseModel.CaseAim));
			Assert.That(pageModel.CaseModel.DeEscalationPoint, Is.EqualTo(caseModel.DeEscalationPoint));
			Assert.That(pageModel.CaseModel.ReviewAt, Is.EqualTo(caseModel.ReviewAt));
			Assert.That(pageModel.CaseModel.StatusName, Is.EqualTo(caseModel.StatusName));
			Assert.That(pageModel.TrustDetailsModel.GiasData.GroupName, Is.EqualTo(trustDetailsModel.GiasData.GroupName));
			Assert.That(pageModel.TrustDetailsModel.GiasData.GroupNameTitle, Is.EqualTo(trustDetailsModel.GiasData.GroupName.ToTitle()));
			Assert.That(pageModel.TrustDetailsModel, Is.EqualTo(trustDetailsModel));
			Assert.True(pageModel.TrustDetailsModel.Establishments[0].EstablishmentWebsite.Contains("http"));
			Assert.That(pageModel.CaseModel.UpdatedAt, Is.EqualTo(caseModel.UpdatedAt));
			// Assert.That(pageModel.CaseModel.CaseSubType, Is.EqualTo(caseModel.CaseSubType));
			Assert.That(pageModel.CaseModel.DirectionOfTravel, Is.EqualTo(caseModel.DirectionOfTravel));
			//Assert.That(pageModel.CaseModel.RagRatingCss, Is.EqualTo(caseModel.RagRatingCss)); //TODOEA
			Assert.That(pageModel.CaseModel.ReasonAtReview, Is.EqualTo(caseModel.ReasonAtReview));
			Assert.That(pageModel.CaseModel.TrustUkPrn, Is.EqualTo(caseModel.TrustUkPrn));
			
			Assert.That(pageModel.TrustCasesModel, Is.Not.Null);
			Assert.That(pageModel.TrustCasesModel.Count, Is.EqualTo(1));

			var actualFirstTrustCaseModel = trustCasesModel.First();
			var expectedFirstTrustCaseModel = pageModel.TrustCasesModel.First();
			Assert.That(expectedFirstTrustCaseModel.CaseUrn, Is.EqualTo(actualFirstTrustCaseModel.CaseUrn));
			Assert.That(expectedFirstTrustCaseModel.RagRating, Is.EqualTo(actualFirstTrustCaseModel.RagRating));
			Assert.That(expectedFirstTrustCaseModel.RagRatingCss, Is.EqualTo(actualFirstTrustCaseModel.RagRatingCss));
			Assert.That(expectedFirstTrustCaseModel.Closed, Is.EqualTo(actualFirstTrustCaseModel.Closed));
			Assert.That(expectedFirstTrustCaseModel.CaseTypeDescription, Is.EqualTo(actualFirstTrustCaseModel.CaseTypeDescription));
			Assert.That(expectedFirstTrustCaseModel.StatusDescription, Is.EqualTo(actualFirstTrustCaseModel.StatusDescription));
			
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
		}
		
		private static ManagementPageModel SetupManagementPageModel(
			ICaseModelService mockCaseModelService, 
			ITrustModelService mockTrustModelService,
			ICaseHistoryModelService mockCaseHistoryModelService,
			ITypeModelService mockTypeModelService,
			IRecordModelService mockRecordModelService,
			IRatingModelService mockRatingModelService,
			ILogger<ManagementPageModel> mockLogger, 
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new ManagementPageModel(mockCaseModelService, mockTrustModelService, mockCaseHistoryModelService, mockTypeModelService, mockRecordModelService, mockRatingModelService, mockLogger)

			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}