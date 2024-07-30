using AutoFixture;
using ConcernsCaseWork.API.Contracts.Srma;
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Pages.Case.Management.Action.SRMA;
using ConcernsCaseWork.Service.Permissions;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Shared.Tests.Factory;
using ConcernsCaseWork.Shared.Tests.MockHelpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.SRMA
{
	[Parallelizable(ParallelScope.All)]
	public class IndexPageModelTests
	{
		private readonly IFixture _fixture;
		
		public IndexPageModelTests()
		{
			_fixture = new Fixture();
		}

		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockCasePermissionsService = new Mock<ICasePermissionsService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

			mockSrmaService.Setup(s => s.GetSRMAViewModel(1, It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupIndexPageModel(mockSrmaService.Object, mockCasePermissionsService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			pageModel.CaseUrn = 1;
			pageModel.SrmaId = 1;

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.IsNotNull(pageModel.SRMAModel);

			mockLogger.VerifyLogErrorWasNotCalled();
		}
		
		[Test]
		public async Task WhenOnGetAsync_AndSrmaIsClosed_RedirectsToClosedPage()
		{
			// arrange
			var caseUrn = 1;
			var srmaId = 1;
			
			var mockSrmaService = new Mock<ISRMAService>();
			var mockCasePermissionsService = new Mock<ICasePermissionsService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed, closedAt: DateTime.Now);
			
			mockSrmaService.Setup(s => s.GetSRMAViewModel(caseUrn, It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupIndexPageModel(mockSrmaService.Object, mockCasePermissionsService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			pageModel.CaseUrn = caseUrn;
			pageModel.SrmaId = srmaId;

			// act
			var response = await pageModel.OnGetAsync();

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(response, Is.InstanceOf<RedirectResult>());
				Assert.That(((RedirectResult)response).Url, Is.EqualTo($"/case/{caseUrn}/management/action/srma/{srmaId}/closed"));
				Assert.That(pageModel.TempData["Error.Message"], Is.Null);
			});
		}

		[TestCase("complete")]
		[TestCase("decline")]
		[TestCase("cancel")]
		public async Task WhenOnPost_SRMA_IsValid_RedirectToResolvePage(string action)
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockCasePermissionsService = new Mock<ICasePermissionsService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed, SRMAReasonOffered.OfferLinked);

			mockSrmaService.Setup(s => s.GetSRMAViewModel(1, It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupIndexPageModel(mockSrmaService.Object, mockCasePermissionsService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			pageModel.CaseUrn = 1;
			pageModel.SrmaId = 1;

			// act
			var pageResponse = await pageModel.OnPost(action);
			var pageResponseInstance = pageResponse as RedirectResult;

			// assert
			Assert.NotNull(pageResponseInstance);
			Assert.That(pageResponseInstance.Url, Is.EqualTo($"/case/1/management/action/srma/1/resolve/{action}"));
		}

		[Test]
		public void WhenOnPost_UnrecognisedAction_ThrowsError()
		{
			var mockSrmaService = new Mock<ISRMAService>();
			var mockCasePermissionsService = new Mock<ICasePermissionsService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed, SRMAReasonOffered.OfferLinked);

			mockSrmaService.Setup(s => s.GetSRMAViewModel(1, It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupIndexPageModel(mockSrmaService.Object, mockCasePermissionsService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			pageModel.CaseUrn = 1;
			pageModel.SrmaId = 1;

			var exception = Assert.ThrowsAsync<Exception>(async () =>
			{
				await pageModel.OnPost("unknown");
			});

			exception.Message.Should().Be("Unrecognised action unknown");
		}

		private static IndexPageModel SetupIndexPageModel(
			ISRMAService mockSrmaService,
			ICasePermissionsService mockCasePermissionsService,
			ILogger<IndexPageModel> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new IndexPageModel(mockSrmaService, mockCasePermissionsService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}

}