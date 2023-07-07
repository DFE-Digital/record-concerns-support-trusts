using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Trust;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Trusts;
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
			var mockLogger = new Mock<ILogger<OverviewPageModel>>();

			var pageModel = SetupOverviewPageModel(mockTrustModelService.Object, mockCaseSummaryService.Object, mockLogger.Object);

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
			var mockLogger = new Mock<ILogger<OverviewPageModel>>();
			var activeCaseSummaryModels = CaseSummaryModelFactory.BuildActiveCaseSummaryModels();
			var closedCaseSummaryModels = CaseSummaryModelFactory.BuildClosedCaseSummaryModels();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			var activeCaseSummaryGroupModel = new CaseSummaryGroupModel<ActiveCaseSummaryModel>()
			{
				Cases = activeCaseSummaryModels,
				Pagination = new PaginationModel()
			};

			var closedCaseSummaryGroupModel = new CaseSummaryGroupModel<ClosedCaseSummaryModel>()
			{
				Cases = closedCaseSummaryModels,
				Pagination = new PaginationModel()
			};

			mockCaseSummaryService.Setup(c => c.GetActiveCaseSummariesByTrust(It.IsAny<string>(), 1))
				.ReturnsAsync(activeCaseSummaryGroupModel);
			mockCaseSummaryService.Setup(c => c.GetClosedCaseSummariesByTrust(It.IsAny<string>(), 1))
				.ReturnsAsync(closedCaseSummaryGroupModel);
			mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>())).ReturnsAsync(trustDetailsModel);

			var pageModel = SetupOverviewPageModel(mockTrustModelService.Object, mockCaseSummaryService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("id", 1);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(pageModel.TrustOverviewModel.TrustDetailsModel, Is.Not.Null);

				Assert.That(pageModel.TrustOverviewModel.TrustDetailsModel.GiasData.GroupName, Is.EqualTo(trustDetailsModel.GiasData.GroupName));
				Assert.That(pageModel.TrustOverviewModel.TrustDetailsModel.GiasData.GroupNameTitle, Is.EqualTo(trustDetailsModel.GiasData.GroupName.ToTitle()));
				Assert.That(pageModel.TrustOverviewModel.TrustDetailsModel, Is.EqualTo(trustDetailsModel));
	
				Assert.That(pageModel.TrustOverviewModel.TrustDetailsModel.Establishments[0].EstablishmentWebsite, Does.Contain("http"));

				Assert.That(pageModel.TrustOverviewModel.ActiveCaseSummaryGroupModel.Cases, Is.EquivalentTo(activeCaseSummaryModels));
				Assert.That(pageModel.TrustOverviewModel.ClosedCaseSummaryGroupModel.Cases, Is.EquivalentTo(closedCaseSummaryModels));
			});
	}

		private static OverviewPageModel SetupOverviewPageModel(
			 ITrustModelService mockTrustModelService, ICaseSummaryService mockCaseSummaryService, ILogger<OverviewPageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new OverviewPageModel(mockTrustModelService, mockCaseSummaryService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}