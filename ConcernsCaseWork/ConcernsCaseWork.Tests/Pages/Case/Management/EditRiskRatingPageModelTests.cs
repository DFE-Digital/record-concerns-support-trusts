using AutoFixture;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case.Management;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Records;
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

namespace ConcernsCaseWork.Tests.Pages.Case.Management
{
	[Parallelizable(ParallelScope.All)]
	public class EditRiskRatingPageModelTests
	{
		private static Fixture _fixture = new();

		[Test]
		public async Task WhenOnGet_ReturnsPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<EditRatingPageModel>>();

			var caseModel = CaseFactory.BuildCaseModel();
			var recordModel = RecordFactory.BuildRecordModel();

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<long>()))
				.ReturnsAsync(caseModel);

			var pageModel = SetupEditRiskRatingPageModel(mockCaseModelService.Object, mockLogger.Object);

			pageModel.CaseUrn = 1;
			
			// act
			var pageResponse = await pageModel.OnGet();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
		}

		[Test]
		public async Task WhenOnPostEditRiskRating_When_Commentary_Not_Provided_Fails_Validation()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<EditRatingPageModel>>();

			mockCaseModelService.Setup(c => c.PatchCaseRating(It.IsAny<PatchCaseModel>()));

			var pageModel = SetupEditRiskRatingPageModel(mockCaseModelService.Object, mockLogger.Object);

			pageModel.CaseUrn = 1;

			pageModel.RiskToTrust = _fixture.Create<RadioButtonsUiComponent>();
			pageModel.RiskToTrust.SelectedId = 1;

			// act
			var pageResponse = await pageModel.OnPostEditRiskRating();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());

			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);

			Assert.That(pageModel.ModelState.IsValid, Is.False);
			Assert.That(pageModel.ModelState.Keys.Count(), Is.EqualTo(1));
			Assert.That(pageModel.ModelState.First().Key, Is.EqualTo("RationalCommentary"));
			Assert.That(pageModel.ModelState.First().Value?.Errors.Single().ErrorMessage, Is.EqualTo("You must enter a RAG rationale commentary"));

			mockCaseModelService.Verify(c => c.PatchCaseRating(It.IsAny<PatchCaseModel>()), Times.Never);
		}

		[Test]
		public async Task WhenOnPostEditRiskRating_RouteData_RequestForm_ReturnsPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<EditRatingPageModel>>();

			mockCaseModelService.Setup(c => c.PatchCaseRating(It.IsAny<PatchCaseModel>()));

			var pageModel = SetupEditRiskRatingPageModel(mockCaseModelService.Object, mockLogger.Object);

			pageModel.RiskToTrust = _fixture.Create<RadioButtonsUiComponent>();
			pageModel.RiskToTrust.SelectedId = 1;

			pageModel.RatingRationalCommentary = "This is my RAG commentary ....";

			pageModel.CaseUrn = 1;
			
			// act
			var pageResponse = await pageModel.OnPostEditRiskRating();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(page.Url, Is.EqualTo("/case/1/management"));
		}

		private static EditRatingPageModel SetupEditRiskRatingPageModel(
			ICaseModelService mockCaseModelService, ILogger<EditRatingPageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new EditRatingPageModel(mockCaseModelService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}