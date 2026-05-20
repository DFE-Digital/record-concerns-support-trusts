using AutoFixture;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Shared.Tests.Factory;
using ConcernsCaseWork.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case
{
	[Parallelizable(ParallelScope.All)]
	public class RatingPageModelTests
	{
		private static Fixture _fixture = new();

		[Test]
		public async Task WhenOnGetAsync_Return_Page()
		{
			// arrange
			var mockLogger = new Mock<ILogger<RatingPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockUserStateService = new Mock<IUserStateCachedService>();

			var createCaseModel = CaseFactory.BuildCreateCaseModel();
			createCaseModel.CreateRecordsModel = RecordFactory.BuildListCreateRecordModel();
			var userState = new UserState("testing") { TrustUkPrn = "trust-ukprn", CreateCaseModel = createCaseModel };
			var expected = TrustFactory.BuildTrustDetailsModel();
			
			mockUserStateService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync(userState);
			mockTrustModelService.Setup(s => s.GetTrustByUkPrn(It.IsAny<string>())).ReturnsAsync(expected);
			
			var pageModel = SetupRatingPageModel(mockTrustModelService.Object, 
				mockUserStateService.Object,
				mockLogger.Object, true);
			
			// act
			await pageModel.OnGetAsync();
			
			// assert
			Assert.That(pageModel, Is.Not.Null);
			Assert.That(pageModel.TrustDetailsModel, Is.Not.Null);
			Assert.That(pageModel.CreateRecordsModel, Is.Not.Null);
			Assert.IsNull(pageModel.TempData["Error.Message"]);
			
			var trustDetailsPageModel = pageModel.TrustDetailsModel;
			var createRecordsPageModel = pageModel.CreateRecordsModel;
			
			Assert.IsAssignableFrom<TrustDetailsModel>(trustDetailsPageModel);
			Assert.IsAssignableFrom<List<CreateRecordModel>>(createRecordsPageModel);
			
			mockUserStateService.Verify(c => c.GetData(It.IsAny<string>()), Times.Once);
			mockTrustModelService.Verify(s => s.GetTrustByUkPrn(It.IsAny<string>()), Times.Once);
		}
		
		[Test]
		public async Task WhenOnGetAsync_TrustUkprnIsNullOrEmpty_Return_Page()
		{
			// arrange
			var mockLogger = new Mock<ILogger<RatingPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockUserStateService = new Mock<IUserStateCachedService>();

			var createCaseModel = CaseFactory.BuildCreateCaseModel();
			createCaseModel.CreateRecordsModel = RecordFactory.BuildListCreateRecordModel();
			var userState = new UserState("testing") { CreateCaseModel = createCaseModel };
			var expected = TrustFactory.BuildTrustDetailsModel();
			
			mockUserStateService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync(userState);
			mockTrustModelService.Setup(s => s.GetTrustByUkPrn(It.IsAny<string>())).ReturnsAsync(expected);
			
			var pageModel = SetupRatingPageModel(mockTrustModelService.Object, 
				mockUserStateService.Object, 
				mockLogger.Object, true);
			
			// act
			await pageModel.OnGetAsync();
			
			// assert
			Assert.That(pageModel, Is.Not.Null);
			Assert.IsNull(pageModel.TrustDetailsModel);
			Assert.IsNull(pageModel.CreateRecordsModel);
			Assert.IsNotNull(pageModel.TempData["Error.Message"]);
			
			mockUserStateService.Verify(c => c.GetData(It.IsAny<string>()), Times.Once);
			mockTrustModelService.Verify(s => s.GetTrustByUkPrn(It.IsAny<string>()), Times.Never);
		}		
		
		[Test]
		public async Task WhenOnGetAsync_UserStateIsNull_Return_Page()
		{
			// arrange
			var mockLogger = new Mock<ILogger<RatingPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();
			
			mockUserStateCachedService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync((UserState)null);
			
			var pageModel = SetupRatingPageModel(mockTrustModelService.Object, 
				mockUserStateCachedService.Object, 
				mockLogger.Object, true);
			
			// act
			await pageModel.OnGetAsync();
			
			// assert
			Assert.That(pageModel, Is.Not.Null);
			Assert.IsNull(pageModel.TrustDetailsModel);
			Assert.IsNull(pageModel.CreateRecordsModel);
			Assert.IsNotNull(pageModel.TempData["Error.Message"]);
			
			mockUserStateCachedService.Verify(c => c.GetData(It.IsAny<string>()), Times.Once);
			mockTrustModelService.Verify(s => s.GetTrustByUkPrn(It.IsAny<string>()), Times.Never);
		}		

		[Test]
		public async Task WhenOnPostAsync_RedirectToPage_Details()
		{
			// arrange
			var mockLogger = new Mock<ILogger<RatingPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();
			
			var expected = CaseFactory.BuildCreateCaseModel();
			var userState = new UserState("testing") { TrustUkPrn = "trust-ukprn", CreateCaseModel = expected };
			
			mockUserStateCachedService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync(userState);
			
			var pageModel = SetupRatingPageModel(mockTrustModelService.Object, 
				mockUserStateCachedService.Object, 
				mockLogger.Object, true);

			pageModel.RiskToTrust = _fixture.Create<RadioButtonsUiComponent>();
			pageModel.RiskToTrust.SelectedId = 1;
			pageModel.YesCheckedRagRational = false;

			// act
			var pageResponse = await pageModel.OnPostAsync();
			var pageResponseInstance = pageResponse as RedirectToPageResult;
			
			// assert
			Assert.That(pageResponseInstance, Is.Not.Null);
			Assert.That(pageResponseInstance.PageName, Is.EqualTo("details"));

			mockUserStateCachedService.Verify(c => c.GetData(It.IsAny<string>()), Times.Once);
			mockUserStateCachedService.Verify(c => c.StoreData(It.IsAny<string>(), It.IsAny<UserState>()), Times.Once);
		}
		
		[Test]
		public async Task WhenOnPostAsync_And_RatingId_Invalid_Raise_Exception()
		{
			// arrange
			var mockLogger = new Mock<ILogger<RatingPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();
			
			var expected = CaseFactory.BuildCreateCaseModel();
			var userState = new UserState("testing") { TrustUkPrn = "trust-ukprn", CreateCaseModel = expected };
			
			mockUserStateCachedService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync(userState);
			
			var pageModel = SetupRatingPageModel(mockTrustModelService.Object, 
				mockUserStateCachedService.Object, 
				mockLogger.Object, true);

			pageModel.RiskToTrust = _fixture.Create<RadioButtonsUiComponent>();
			pageModel.RiskToTrust.SelectedId = 101;
			pageModel.YesCheckedRagRational = false;

			// act
			_ = await pageModel.OnPostAsync();
			
			// assert
			Assert.IsNotNull(pageModel.TempData["Error.Message"]);
		}

		[Test]
		public async Task WhenOnPostAsync_And_RatingRational_Is_Yes_WithNoCommentry_Fails_Validation()
		{
			// arrange
			var mockLogger = new Mock<ILogger<RatingPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();

			var expected = CaseFactory.BuildCreateCaseModel();
			var userState = new UserState("testing") { TrustUkPrn = "trust-ukprn", CreateCaseModel = expected };

			mockUserStateCachedService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync(userState);

			var pageModel = SetupRatingPageModel(mockTrustModelService.Object,
				mockUserStateCachedService.Object,
				mockLogger.Object, true);

			pageModel.RiskToTrust = _fixture.Create<RadioButtonsUiComponent>();
			pageModel.RiskToTrust.SelectedId = 1;
			pageModel.YesCheckedRagRational = true;

			// act
			_ = await pageModel.OnPostAsync();

			// Assert
			Assert.Multiple(() =>
			{
				Assert.That(pageModel.ModelState.IsValid, Is.False);
				Assert.That(pageModel.ModelState.ContainsKey(nameof(pageModel.RatingRationalCommentary)), Is.True);
			});
			var error = pageModel.ModelState[nameof(pageModel.RatingRationalCommentary)].Errors[0];
			Assert.That(error.ErrorMessage, Is.EqualTo("You must enter a RAG rationale commentary"));
		}

		[Test]
		public async Task WhenOnPostAsync_And_RatingRational_Is_Yes_WithCommentry_Too_Long_Fails_Validation()
		{
			// arrange
			var mockLogger = new Mock<ILogger<RatingPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();

			var expected = CaseFactory.BuildCreateCaseModel();
			var userState = new UserState("testing") { TrustUkPrn = "trust-ukprn", CreateCaseModel = expected };

			mockUserStateCachedService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync(userState);

			var pageModel = SetupRatingPageModel(mockTrustModelService.Object,
				mockUserStateCachedService.Object,
				mockLogger.Object, true);

			pageModel.RiskToTrust = _fixture.Create<RadioButtonsUiComponent>();
			pageModel.RiskToTrust.SelectedId = 1;
			pageModel.YesCheckedRagRational = true;
			pageModel.RatingRationalCommentary = "Lorem ipsum dolor sit amet consectetur adipiscing elit. Blandit quis suspendisse aliquet nisi sodales consequat magna. Sem placerat in id cursus mi pretium tellus. Finibus facilisis dapibus etiam interdum tortor ligula congue. Sed diam urna tempor pulvinar vivamus fringilla lacus. Porta elementum a enim euismod quam justo lectus. Nisl malesuada lacinia integer nunc posuere ut hendrerit. Imperdiet mollis nullam volutpat porttitor ullamcorper rutrum gravida. Ad litora torquent per conubia nostra inceptos himenaeos. Ornare sagittis vehicula praesent dui felis venenatis ultrices. Dis parturient montes nascetur ridiculus mus donec rhoncus. Potenti ultricies habitant morbi senectus netus suscipit auctor. Maximus eget fermentum odio phasellus non purus est. Platea dictumst lorem ipsum dolor sit amet consectetur. Dictum risus blandit quis suspendisse aliquet nisi sodales. Vitae pellentesque sem placerat in id cursus mi. Luctus nibh finibus facilisis dapibus etiam interdum tortor. Eu aenean sed diam urna tempor pulvinar vivamus. Tincidunt nam porta elementum a enim euismod quam. Iaculis massa nisl malesuada lacinia integer nunc posuere. Velit aliquam imperdiet mollis nullam volutpat porttitor ullamcorper. Taciti sociosqu ad litora torquent per conubia nostra. Primis vulputate ornare sagittis vehicula praesent dui felis. Et magnis dis parturient montes nascetur ridiculus mus. Accumsan maecenas potenti ultricies habitant morbi senectus netus. Mattis scelerisque maximus eget fermentum odio phasellus non. Hac habitasse platea dictumst lorem ipsum dolor sit. Vestibulum fusce dictum risus blandit quis suspendisse aliquet. Ex sapien vitae pellentesque sem placerat in id. Neque at luctus nibh finibus facilisis dapibus etiam. Tempus leo eu aenean sed diam urna tempor. Viverra ac tincidunt nam porta elementum a enim. Bibendum egestas iaculis massa nisl malesuada lacinia integer. Arcu dignissim velit aliquam imperdiet mollis nullam volutpat. Class aptent taciti sociosqu ad litora torquent per. Turpis fames primis vulputate ornare sagittis vehicula praesent. Natoque penatibus et magnis dis parturient montes nascetur. Feugiat tristique accumsan maecenas potenti ultricies habitant morbi. Nulla molestie mattis scelerisque maximus eget fermentum odio. Cubilia curae hac habitasse platea dictumst lorem ipsum. Mauris pharetra vestibulum fusce dictum risus blandit quis. Quisque faucibus ex sapien vitae pellentesque sem placerat. Ante condimentum neque at luctus nibh finibus facilisis. Duis convallis tempus leo eu aenean sed diam. Sollicitudin erat viverra ac tincidunt nam porta elementum. Nec metus bibendum egestas iaculis massa nisl malesuada. Commodo augue arcu dignissim velit aliquam imperdiet mollis. Semper vel class aptent taciti sociosqu ad litora. Cras eleifend turpis fames primis vulputate ornare sagittis. Orci varius natoque penatibus et magnis dis parturient. Proin libero feugiat tristique accumsan maecenas potenti ultricies. Eros lobortis nulla molestie mattis scelerisque maximus eget. Curabitur facilisi cubilia curae hac habitasse platea dictumst. Efficitur laoreet mauris pharetra vestibulum fusce dictum risus. Adipiscing elit quisque faucibus ex sapien vitae pellentesque. Consequat magna ante condimentum neque at luctus nibh. Pretium tellus duis convallis tempus leo eu aenean. Ligula congue sollicitudin erat viverra ac tincidunt nam. Fringilla lacus nec metus bibendum egestas iaculis massa. Justo lectus commodo augue arcu dignissim velit aliquam. Ut hendrerit semper vel class aptent taciti sociosqu. Rutrum gravida cras eleifend turpis fames primis vulputate. Inceptos himenaeos orci varius natoque penatibus et magnis. Venenatis ultrices proin libero feugiat tristique accumsan maecenas. Donec rhoncus eros lobortis nulla molestie mattis scelerisque. Suscipit auctor curabitur facilisi cubilia curae hac habitasse. Purus est efficitur laoreet mauris pharetra vestibulum fusce. Amet consectetur adipiscing elit quisque faucibus ex sapien. Nisi sodales consequat magna ante condimentum neque at. Cursus mi pretium tellus duis convallis tempus leo. Interdum tortor ligula congue sollicitudin erat viverra ac. Pulvinar vivamus fringilla lacus nec metus bibendum egestas. Euismod quam justo lectus commodo augue arcu dignissim. Nunc posuere ut hendrerit semper vel class aptent. Porttitor ullamcorper rutrum gravida cras eleifend turpis fames. Conubia nostra inceptos himenaeos orci varius natoque penatibus. Dui felis venenatis ultrices proin libero feugiat tristique. Ridiculus mus donec rhoncus eros lobortis nulla molestie. Senectus netus suscipit auctor curabitur facilisi cubilia curae. Phasellus non purus est efficitur laoreet mauris pharetra. Dolor sit amet consectetur adipiscing elit quisque faucibus. Suspendisse aliquet nisi sodales consequat magna ante condimentum. In id cursus mi pretium tellus duis convallis. Dapibus etiam interdum tortor ligula congue sollicitudin erat. Urna tempor pulvinar vivamus fringilla lacus nec metus. Aenim euismod quam justo lectus commodo augue. Lacinia integer nunc posuere ut hendrerit semper vel. Nullam volutpat porttitor ullamcorper rutrum gravida cras eleifend. Torquent per conubia nostra inceptos himenaeos orci varius. Vehicula praesent dui felis venenatis ultrices proin libero. Montes nascetur ridiculus mus donec rhoncus eros lobortis. Habitant morbi senectus netus suscipit auctor curabitur facilisi. Fermentum odio phasellus non purus est efficitur laoreet. Lorem ipsum dolor sit amet consectetur adipiscing elit. Blandit quis suspendisse aliquet nisi sodales consequat magna. Sem placerat in id cursus mi pretium tellus. Finibus facilisis dapibus etiam interdum tortor ligula congue. Sed diam urna tempor pulvinar vivamus fringilla lacus. Porta elementum a enim euismod quam justo lectus. Nisl malesuada lacinia integer nunc posuere ut hendrerit.";

			// act
			_ = await pageModel.OnPostAsync();

			// Assert
			Assert.Multiple(() =>
			{
				Assert.That(pageModel.ModelState.IsValid, Is.False);
				Assert.That(pageModel.ModelState.ContainsKey("RationalCommentaryMaxLength"), Is.True);
			});
			var error = pageModel.ModelState["RationalCommentaryMaxLength"].Errors[0];
			Assert.That(error.ErrorMessage, Is.EqualTo($"You have {pageModel.RatingRationalCommentary.Length - RatingPageModel.RationYesCommentaryMaxLength} characters too many."));
		}

		private static RatingPageModel SetupRatingPageModel(
			ITrustModelService mockTrustModelService, 
			IUserStateCachedService mockUserStateCachedService,
			ILogger<RatingPageModel> mockLogger, bool isAuthenticated = false)
		{
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			mockClaimsPrincipalHelper.Setup(x => x.GetPrincipalName(It.IsAny<ClaimsPrincipal>())).Returns("Tester");

			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new RatingPageModel(mockTrustModelService, mockUserStateCachedService,
				mockLogger, mockClaimsPrincipalHelper.Object,MockTelemetry.CreateMockTelemetryClient())
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}