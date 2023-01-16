using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Pages.Case.Management.Action.Nti;
using ConcernsCaseWork.Redis.Nti;
using ConcernsCaseWork.Service.Nti;
using ConcernsCaseWork.Services.Nti;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.Nti
{
	[Parallelizable(ParallelScope.All)]
	public class AddPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_MissingCaseUrn_ThrowsException_ReturnPage()
		{
			// arrange

			var mockNtiModelService = new Mock<INtiModelService>();
			var mockNtiReasonsService = new Mock<INtiReasonsCachedService>();
			var mockNtiStatusesService = new Mock<INtiStatusesCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockNtiModelService, mockNtiReasonsService, mockNtiStatusesService, mockLogger);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
		}

		[Test]
		public async Task WhenOnGetAsync_PopulatesPageModel()
		{
			// arrange
			var caseUrn = 191L;
			var ntiId = 835;

			var mockNtiModelService = new Mock<INtiModelService>();
			var mockNtiReasonsService = new Mock<INtiReasonsCachedService>();
			var mockNtiStatusesService = new Mock<INtiStatusesCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			mockNtiStatusesService.Setup(svc => svc.GetAllStatusesAsync()).ReturnsAsync(new NtiStatusDto[] {
				new NtiStatusDto { Id = 1, Name = "Status 1" }
			});

			mockNtiReasonsService.Setup(svc => svc.GetAllReasonsAsync()).ReturnsAsync(new NtiReasonDto[] {
				new NtiReasonDto { Id = 1, Name = "Reason 1" }
			});
			
			var ntiModel = NTIFactory.BuildNTIModel();
			mockNtiModelService.Setup(n => n.GetNtiByIdAsync(ntiId))
				.ReturnsAsync(ntiModel);

			var pageModel = SetupAddPageModel(mockNtiModelService, mockNtiReasonsService, mockNtiStatusesService, mockLogger);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseUrn);
			routeData.Add("ntiid", ntiId);

			// act
			await pageModel.OnGetAsync();

			// assert
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::NTI::AddPageModel::OnGetAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);

			Assert.That(pageModel, Is.Not.Null);
			Assert.That(pageModel.CaseUrn, Is.EqualTo(caseUrn));
			Assert.That(pageModel.Statuses.Single().Text, Is.EqualTo("Status 1"));
			Assert.That(pageModel.Reasons.Single().Text, Is.EqualTo("Reason 1"));
			Assert.That(pageModel.CancelLinkUrl, Is.Not.Null);
		}

		[Test]
		public async Task WhenOnGetAsync_AndNtiIsClosed_RedirectsToClosedPage()
		{
			// arrange
			var caseUrn = 191L;
			var ntiId = 835;

			var mockNtiModelService = new Mock<INtiModelService>();
			var mockNtiReasonsService = new Mock<INtiReasonsCachedService>();
			var mockNtiStatusesService = new Mock<INtiStatusesCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			mockNtiStatusesService.Setup(svc => svc.GetAllStatusesAsync()).ReturnsAsync(new NtiStatusDto[] {
				new NtiStatusDto { Id = 1, Name = "Status 1" }
			});

			mockNtiReasonsService.Setup(svc => svc.GetAllReasonsAsync()).ReturnsAsync(new NtiReasonDto[] {
				new NtiReasonDto { Id = 1, Name = "Reason 1" }
			});
			
			var ntiModel = NTIFactory.BuildClosedNTIModel();
			mockNtiModelService.Setup(n => n.GetNtiByIdAsync(ntiId))
				.ReturnsAsync(ntiModel);

			var pageModel = SetupAddPageModel(mockNtiModelService, mockNtiReasonsService, mockNtiStatusesService, mockLogger);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseUrn);
			routeData.Add("ntiid", ntiId);

			// act
			var response = await pageModel.OnGetAsync();

			// assert
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::NTI::AddPageModel::OnGetAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
			
			Assert.Multiple(() =>
			{
				Assert.That(response, Is.InstanceOf<RedirectResult>());
				Assert.That(((RedirectResult)response).Url, Is.EqualTo($"/case/{caseUrn}/management/action/nti/{ntiId}"));
				Assert.That(pageModel.TempData["Error.Message"], Is.Null);
			});
		}

		[Test]
		public async Task WhenOnPostAsync_MissingRouteData_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockNtiReasonsService = new Mock<INtiReasonsCachedService>();
			var mockNtiStatusesService = new Mock<INtiStatusesCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockNtiModelService, mockNtiReasonsService, mockNtiStatusesService, mockLogger);

			// act
			var pageResponse = await pageModel.OnPostAsync(String.Empty);

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnPostPage));
		}

		private static AddPageModel SetupAddPageModel(Mock<INtiModelService> mockNtiModelService,
			Mock<INtiReasonsCachedService> mockNtiReasonsCachedService,
			Mock<INtiStatusesCachedService> mockNtiStatusesCachedService,
			Mock<ILogger<AddPageModel>> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new AddPageModel(mockNtiStatusesCachedService.Object, mockNtiReasonsCachedService.Object,
				mockNtiModelService.Object, mockLogger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}

}