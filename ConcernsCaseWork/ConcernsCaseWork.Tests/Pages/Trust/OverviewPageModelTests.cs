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
using System.Linq;
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
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockLogger = new Mock<ILogger<OverviewPageModel>>();

			var pageModel = SetupOverviewPageModel(mockTrustModelService.Object, mockCaseModelService.Object, mockTypeModelService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("id", "");

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
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockLogger = new Mock<ILogger<OverviewPageModel>>();
			var trustCasesModel = CaseFactory.BuildListTrustCasesModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();

			mockCaseModelService.Setup(c => c.GetCasesByTrustUkprn(It.IsAny<string>()))
				.ReturnsAsync(trustCasesModel);
			mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>())).ReturnsAsync(trustDetailsModel);

			var pageModel = SetupOverviewPageModel(mockTrustModelService.Object, mockCaseModelService.Object, mockTypeModelService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("id", 1);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TrustDetailsModel, Is.Not.Null);
			Assert.That(pageModel.TrustDetailsModel.GiasData.GroupName, Is.EqualTo(trustDetailsModel.GiasData.GroupName));
			Assert.That(pageModel.TrustDetailsModel.GiasData.GroupNameTitle, Is.EqualTo(trustDetailsModel.GiasData.GroupName.ToTitle()));
			Assert.That(pageModel.TrustDetailsModel, Is.EqualTo(trustDetailsModel));
			Assert.True(pageModel.TrustDetailsModel.Establishments[0].EstablishmentWebsite.Contains("http"));
			Assert.That(pageModel.TrustCasesModel, Is.Not.Null);
			Assert.That(pageModel.TrustCasesModel.Count, Is.EqualTo(1));

			var actualFirstTrustCaseModel = trustCasesModel.First();
			var expectedFirstTrustCaseModel = pageModel.TrustCasesModel.First();
			Assert.That(expectedFirstTrustCaseModel.CaseUrn, Is.EqualTo(actualFirstTrustCaseModel.CaseUrn));
			Assert.That(expectedFirstTrustCaseModel.RecordsModel, Is.EqualTo(actualFirstTrustCaseModel.RecordsModel));
			Assert.That(expectedFirstTrustCaseModel.Created, Is.EqualTo(actualFirstTrustCaseModel.Created));
			Assert.That(expectedFirstTrustCaseModel.RatingModel, Is.EqualTo(actualFirstTrustCaseModel.RatingModel));
		}

		private static OverviewPageModel SetupOverviewPageModel(
			 ITrustModelService mockTrustModelService, ICaseModelService mockCaseModelService, ITypeModelService mockTypeModelService, ILogger<OverviewPageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new OverviewPageModel(mockTrustModelService, mockCaseModelService, mockTypeModelService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}