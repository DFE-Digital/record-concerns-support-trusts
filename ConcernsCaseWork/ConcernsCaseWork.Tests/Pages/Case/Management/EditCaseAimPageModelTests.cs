using AutoFixture;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case.Management;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Shared.Tests.Factory;
using ConcernsCaseWork.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management
{
	[Parallelizable(ParallelScope.All)]
	public class EditCaseAimPageModelTests
	{
		private static Fixture _fixture = new();

		[Test]
		public async Task WhenOnGetAsync_ReturnsPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<EditCaseAimPageModel>>();

			var caseModel = CaseFactory.BuildCaseModel();

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<long>()))
				.ReturnsAsync(caseModel);

			var pageModel = SetupEditCaseAimPageModel(mockCaseModelService.Object, mockLogger.Object);

			// act
			var pageResponse = await pageModel.OnGetAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.CaseAim, Is.Not.Null);

			mockCaseModelService.Verify(c => 
				c.GetCaseByUrn(It.IsAny<long>()), Times.Once);
		}
		
		[Test]
		public async Task WhenOnPostEditIssue_RouteData_ReturnsToPreviousUrl()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<EditCaseAimPageModel>>();

			mockCaseModelService.Setup(c => c.PatchCaseAim(It.IsAny<PatchCaseModel>()));

			var pageModel = SetupEditCaseAimPageModel(mockCaseModelService.Object, mockLogger.Object);

			pageModel.CaseUrn = 1;

			pageModel.CaseAim = _fixture.Create<TextAreaUiComponent>();
			
			// act
			var pageResponse = await pageModel.OnPostEditCaseAim();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(page.Url, Is.EqualTo("/case/1/management"));
			
			mockCaseModelService.Verify(c => c.PatchCaseAim(It.IsAny<PatchCaseModel>()), Times.Once);
		}
		
		private static EditCaseAimPageModel SetupEditCaseAimPageModel(
			ICaseModelService mockCaseModelService, ILogger<EditCaseAimPageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new EditCaseAimPageModel(mockCaseModelService, mockLogger,MockTelemetry.CreateMockTelemetryClient())
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}