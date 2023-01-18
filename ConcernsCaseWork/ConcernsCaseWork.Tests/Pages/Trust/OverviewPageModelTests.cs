using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Pages.Trust;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Services.Types;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Trust
{
	[Parallelizable(ParallelScope.All)]
	public class OverviewPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_MissingCaseUrn_ThrowsException()
		{
			// arrange
			var mockCaseSummaryService = new Mock<ICaseSummaryService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockLogger = new Mock<ILogger<OverviewPageModel>>();

			var pageModel = SetupOverviewPageModel(mockTrustModelService.Object, mockCaseSummaryService.Object, mockTypeModelService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("id", "");

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnGetPage));
		}

		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			var mockCaseSummaryService = new Mock<ICaseSummaryService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockLogger = new Mock<ILogger<OverviewPageModel>>();
			var activeCaseSummaryModels = CaseSummaryModelFactory.BuildActiveCaseSummaryModels();
			var closedCaseSummaryModels = CaseSummaryModelFactory.BuildClosedCaseSummaryModels();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();

			mockCaseSummaryService.Setup(c => c.GetActiveCaseSummariesByTrust(It.IsAny<string>()))
				.ReturnsAsync(activeCaseSummaryModels);
			mockCaseSummaryService.Setup(c => c.GetClosedCaseSummariesByTrust(It.IsAny<string>()))
				.ReturnsAsync(closedCaseSummaryModels);
			mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>())).ReturnsAsync(trustDetailsModel);

			var pageModel = SetupOverviewPageModel(mockTrustModelService.Object, mockCaseSummaryService.Object, mockTypeModelService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("id", 1);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(pageModel.TrustDetailsModel, Is.Not.Null);

				Assert.That(pageModel.TrustDetailsModel.GiasData.GroupName, Is.EqualTo(trustDetailsModel.GiasData.GroupName));
				Assert.That(pageModel.TrustDetailsModel.GiasData.GroupNameTitle, Is.EqualTo(trustDetailsModel.GiasData.GroupName.ToTitle()));
				Assert.That(pageModel.TrustDetailsModel, Is.EqualTo(trustDetailsModel));
	
				Assert.That(pageModel.TrustDetailsModel.Establishments[0].EstablishmentWebsite, Does.Contain("http"));

				Assert.That(pageModel.ActiveCases, Is.EquivalentTo(activeCaseSummaryModels));
				Assert.That(pageModel.ClosedCases, Is.EquivalentTo(closedCaseSummaryModels));
			});
	}

		private static OverviewPageModel SetupOverviewPageModel(
			 ITrustModelService mockTrustModelService, ICaseSummaryService mockCaseSummaryService, ITypeModelService mockTypeModelService, ILogger<OverviewPageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new OverviewPageModel(mockTrustModelService, mockCaseSummaryService, mockTypeModelService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}