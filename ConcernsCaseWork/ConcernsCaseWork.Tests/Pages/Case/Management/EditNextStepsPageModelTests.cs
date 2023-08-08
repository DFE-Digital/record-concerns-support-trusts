using AutoFixture;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case.Management;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Shared.Tests.Factory;
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
	public class EditNextStepsPageModelTests
	{
		private static Fixture _fixture = new();

		[Test]
		public async Task WhenOnGetAsync_ReturnsPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<EditNextStepsPageModel>>();

			var caseModel = CaseFactory.BuildCaseModel();

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<long>()))
				.ReturnsAsync(caseModel);

			var pageModel = SetupEditNextStepsPageModel(mockCaseModelService.Object, mockLogger.Object);
			
			// act
			var pageResponse = await pageModel.OnGetAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.NextSteps, Is.Not.Null);

			mockCaseModelService.Verify(c => 
				c.GetCaseByUrn(It.IsAny<long>()), Times.Once);
		}
		
		[Test]
		public async Task WhenOnPostEditNextSteps_RouteData_RequestForm_ReturnsToPreviousUrl()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<EditNextStepsPageModel>>();

			mockCaseModelService.Setup(c => c.PatchIssue(It.IsAny<PatchCaseModel>()));

			var pageModel = SetupEditNextStepsPageModel(mockCaseModelService.Object, mockLogger.Object);

			pageModel.CaseUrn = 1;

			pageModel.NextSteps = _fixture.Create<TextAreaUiComponent>();
			
			// act
			var pageResponse = await pageModel.OnPostEditNextSteps();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(page.Url, Is.EqualTo("/case/1/management"));
			
			mockCaseModelService.Verify(c => 
				c.PatchNextSteps(It.IsAny<PatchCaseModel>()), Times.Once);
			mockCaseModelService.Verify(c => 
				c.GetCaseByUrn(It.IsAny<long>()), Times.Never);
		}
		
		private static EditNextStepsPageModel SetupEditNextStepsPageModel(
			ICaseModelService mockCaseModelService, ILogger<EditNextStepsPageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new EditNextStepsPageModel(mockCaseModelService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}