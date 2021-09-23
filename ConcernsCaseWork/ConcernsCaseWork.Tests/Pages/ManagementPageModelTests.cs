using ConcernsCaseWork.Pages.Case;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Type;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
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
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockLogger = new Mock<ILogger<ManagementPageModel>>();

			var pageModel = SetupManagementPageModel(mockCaseModelService.Object, mockTypeModelService.Object, mockLogger.Object);

			// act
			await pageModel.OnGetAsync();
			
			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
		}
		
		[Test]
		public async Task WhenOnGetAsync_MissingCaseUrn_ReturnsPageModel()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockLogger = new Mock<ILogger<ManagementPageModel>>();

			var caseModel = CaseFactory.BuildCaseModel();

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(caseModel);
			
			var pageModel = SetupManagementPageModel(mockCaseModelService.Object, mockTypeModelService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("id", 1);
			
			// act
			await pageModel.OnGetAsync();
			
			// assert
			Assert.That(pageModel.CaseModel, Is.Not.Null);
			Assert.That(pageModel.CaseModel.Description, Is.EqualTo(caseModel.Description));
			Assert.That(pageModel.CaseModel.Issue, Is.EqualTo(caseModel.Issue));
			Assert.That(pageModel.CaseModel.Status, Is.EqualTo(caseModel.Status));
			Assert.That(pageModel.CaseModel.Urn, Is.EqualTo(caseModel.Urn));
			Assert.That(pageModel.CaseModel.CaseType, Is.EqualTo(caseModel.CaseType));
			Assert.That(pageModel.CaseModel.ClosedAt, Is.EqualTo(caseModel.ClosedAt));
			Assert.That(pageModel.CaseModel.CreatedAt, Is.EqualTo(caseModel.CreatedAt));
			Assert.That(pageModel.CaseModel.CreatedBy, Is.EqualTo(caseModel.CreatedBy));
			Assert.That(pageModel.CaseModel.CrmEnquiry, Is.EqualTo(caseModel.CrmEnquiry));
			Assert.That(pageModel.CaseModel.CurrentStatus, Is.EqualTo(caseModel.CurrentStatus));
			Assert.That(pageModel.CaseModel.DeEscalation, Is.EqualTo(caseModel.DeEscalation));
			Assert.That(pageModel.CaseModel.NextSteps, Is.EqualTo(caseModel.NextSteps));
			Assert.That(pageModel.CaseModel.RagRating, Is.EqualTo(caseModel.RagRating));
			Assert.That(pageModel.CaseModel.CaseAim, Is.EqualTo(caseModel.CaseAim));
			Assert.That(pageModel.CaseModel.DeEscalationPoint, Is.EqualTo(caseModel.DeEscalationPoint));
			Assert.That(pageModel.CaseModel.ReviewAt, Is.EqualTo(caseModel.ReviewAt));
			Assert.That(pageModel.CaseModel.StatusName, Is.EqualTo(caseModel.StatusName));
			Assert.That(pageModel.CaseModel.TrustName, Is.EqualTo(caseModel.TrustName));
			Assert.That(pageModel.CaseModel.UpdatedAt, Is.EqualTo(caseModel.UpdatedAt));
			Assert.That(pageModel.CaseModel.CaseSubType, Is.EqualTo(caseModel.CaseSubType));
			Assert.That(pageModel.CaseModel.DirectionOfTravel, Is.EqualTo(caseModel.DirectionOfTravel));
			Assert.That(pageModel.CaseModel.RagRatingCss, Is.EqualTo(caseModel.RagRatingCss));
			Assert.That(pageModel.CaseModel.ReasonAtReview, Is.EqualTo(caseModel.ReasonAtReview));
			Assert.That(pageModel.CaseModel.TrustUkPrn, Is.EqualTo(caseModel.TrustUkPrn));
		}
		
		private static ManagementPageModel SetupManagementPageModel(
			ICaseModelService mockCaseModelService, ITypeModelService mockTypeModelService, ILogger<ManagementPageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new ManagementPageModel(mockCaseModelService, mockTypeModelService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}