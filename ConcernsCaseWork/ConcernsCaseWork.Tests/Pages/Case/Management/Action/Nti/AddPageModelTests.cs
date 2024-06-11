using ConcernsCaseWork.API.Contracts.NoticeToImprove;
using ConcernsCaseWork.Pages.Case.Management.Action.Nti;
using ConcernsCaseWork.Services.Nti;
using ConcernsCaseWork.Shared.Tests.Factory;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.Nti
{
	[Parallelizable(ParallelScope.All)]
	public class AddPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_PopulatesPageModel()
		{
			// arrange
			var caseUrn = 191;
			var ntiId = 835;

			var mockNtiModelService = new Mock<INtiModelService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();
			
			var ntiModel = NTIFactory.BuildNTIModel();
			mockNtiModelService.Setup(n => n.GetNtiByIdAsync(ntiId))
				.ReturnsAsync(ntiModel);

			var pageModel = SetupAddPageModel(mockNtiModelService, mockLogger);

			var routeData = pageModel.RouteData.Values;
			pageModel.CaseUrn = caseUrn;
			pageModel.NtiId = ntiId;

			// act
			var pageResult = await pageModel.OnGetAsync();

			pageResult.Should().BeAssignableTo<PageResult>();

			Assert.That(pageModel, Is.Not.Null);
			Assert.That(pageModel.CaseUrn, Is.EqualTo(caseUrn));
			Assert.That(pageModel.NtiStatus.SelectedId, Is.EqualTo((int)NtiStatus.IssuedNTI));
			Assert.That(pageModel.Reasons.First().Text, Is.EqualTo("Cash flow problems"));
			Assert.That(pageModel.CancelLinkUrl, Is.Not.Null);
		}

		[Test]
		public async Task WhenOnGetAsync_AndNtiIsClosed_RedirectsToClosedPage()
		{
			// arrange
			var caseUrn = 191;
			var ntiId = 835;

			var mockNtiModelService = new Mock<INtiModelService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();
			
			var ntiModel = NTIFactory.BuildClosedNTIModel();
			mockNtiModelService.Setup(n => n.GetNtiByIdAsync(ntiId))
				.ReturnsAsync(ntiModel);

			var pageModel = SetupAddPageModel(mockNtiModelService, mockLogger);

			var routeData = pageModel.RouteData.Values;
			pageModel.CaseUrn = caseUrn;
			pageModel.NtiId = ntiId;

			// act
			var response = await pageModel.OnGetAsync();
			
			Assert.Multiple(() =>
			{
				Assert.That(response, Is.InstanceOf<RedirectResult>());
				Assert.That(((RedirectResult)response).Url, Is.EqualTo($"/case/{caseUrn}/management/action/nti/{ntiId}"));
				Assert.That(pageModel.TempData["Error.Message"], Is.Null);
			});
		}

		private static AddPageModel SetupAddPageModel(Mock<INtiModelService> mockNtiModelService,
			Mock<ILogger<AddPageModel>> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new AddPageModel(
				mockNtiModelService.Object, 
				mockLogger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}

}