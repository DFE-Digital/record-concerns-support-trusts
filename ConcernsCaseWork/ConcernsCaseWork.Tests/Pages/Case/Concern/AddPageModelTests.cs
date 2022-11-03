using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case.Concern;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Concern
{
	[Parallelizable(ParallelScope.All)]
	public class AddPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_Return_Page()
		{
			// arrange
			var mockLogger = new Mock<ILogger<AddPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();

			var createCaseModel = CaseFactory.BuildCreateCaseModel();
			createCaseModel.CreateRecordsModel = RecordFactory.BuildListCreateRecordModel();
			var userState = new UserState("tester.one") { TrustUkPrn = "trust-ukprn", CreateCaseModel = createCaseModel };
			var expected = TrustFactory.BuildTrustDetailsModel();

			mockUserStateCachedService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync(userState);
			mockTrustModelService.Setup(s => s.GetTrustByUkPrn(It.IsAny<string>())).ReturnsAsync(expected);

			var pageModel = SetupAddPageModel(mockTrustModelService.Object, mockUserStateCachedService.Object, mockLogger.Object, true);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel, Is.Not.Null);
			Assert.That(pageModel.TrustDetailsModel, Is.Not.Null);
			Assert.That(pageModel.CreateRecordsModel, Is.Not.Null);
			Assert.IsNull(pageModel.TempData["Error.Message"]);

			var trustDetailsModel = pageModel.TrustDetailsModel;
			var createRecordsModel = pageModel.CreateRecordsModel;

			Assert.IsAssignableFrom<TrustDetailsModel>(trustDetailsModel);
			Assert.IsAssignableFrom<List<CreateRecordModel>>(createRecordsModel);

			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("AddPageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);

			mockUserStateCachedService.Verify(c => c.GetData(It.IsAny<string>()), Times.Once);
			mockTrustModelService.Verify(s => s.GetTrustByUkPrn(It.IsAny<string>()), Times.Once);
		}

		[Test]
		public async Task WhenOnGetAsync_TrustUkprnIsNullOrEmpty_Return_Page()
		{
			// arrange
			var mockLogger = new Mock<ILogger<AddPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();

			var createCaseModel = CaseFactory.BuildCreateCaseModel();
			createCaseModel.CreateRecordsModel = RecordFactory.BuildListCreateRecordModel();
			var userState = new UserState("testing") { CreateCaseModel = createCaseModel };

			mockUserStateCachedService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync(userState);

			var pageModel = SetupAddPageModel(mockTrustModelService.Object, mockUserStateCachedService.Object, mockLogger.Object, true);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel, Is.Not.Null);
			Assert.IsNull(pageModel.TrustDetailsModel);
			Assert.IsNull(pageModel.CreateRecordsModel);
			Assert.IsNotNull(pageModel.TempData["Error.Message"]);

			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("AddPageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);

			mockUserStateCachedService.Verify(c => c.GetData(It.IsAny<string>()), Times.Once);
			mockTrustModelService.Verify(s => s.GetTrustByUkPrn(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public async Task WhenOnGetAsync_UserStateIsNull_Return_Page()
		{
			// arrange
			var mockLogger = new Mock<ILogger<AddPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();

			mockUserStateCachedService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync((UserState)null);

			var pageModel = SetupAddPageModel(mockTrustModelService.Object, mockUserStateCachedService.Object, mockLogger.Object, true);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel, Is.Not.Null);
			Assert.IsNull(pageModel.TrustDetailsModel);
			Assert.IsNull(pageModel.CreateRecordsModel);
			Assert.IsNotNull(pageModel.TempData["Error.Message"]);

			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("AddPageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);

			mockUserStateCachedService.Verify(c => c.GetData(It.IsAny<string>()), Times.Once);
			mockTrustModelService.Verify(s => s.GetTrustByUkPrn(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public async Task WhenOnGetCancel_Return_HomePage()
		{
			// arrange
			var mockLogger = new Mock<ILogger<AddPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();

			mockUserStateCachedService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync(new UserState("testing"));
			mockUserStateCachedService.Setup(c => c.StoreData(It.IsAny<string>(), It.IsAny<UserState>()));

			var pageModel = SetupAddPageModel(mockTrustModelService.Object, mockUserStateCachedService.Object, mockLogger.Object, true);

			// act
			var pageResponse = await pageModel.OnGetCancel();
			var pageResponseInstance = pageResponse as RedirectResult;

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			Assert.IsNotNull(pageResponseInstance);
			Assert.That(pageResponseInstance.Url, Is.EqualTo("/"));
		}

		[Test]
		public async Task WhenOnGetCancel_UserStateIsNull_Return_Page()
		{
			// arrange
			var mockLogger = new Mock<ILogger<AddPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();

			mockUserStateCachedService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync((UserState)null);
			mockUserStateCachedService.Setup(c => c.StoreData(It.IsAny<string>(), It.IsAny<UserState>()));

			var pageModel = SetupAddPageModel(mockTrustModelService.Object, mockUserStateCachedService.Object, mockLogger.Object, true);

			// act
			var pageResponse = await pageModel.OnGetCancel();
			var pageResponseInstance = pageResponse as PageResult;

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			Assert.IsNotNull(pageResponseInstance);
			Assert.IsNotNull(pageModel.TempData["Error.Message"]);
		}

		private static AddPageModel SetupAddPageModel(
			ITrustModelService mockTrustModelService,
			IUserStateCachedService mockUserStateCachedService,
			ILogger<AddPageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new AddPageModel(mockTrustModelService, mockUserStateCachedService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}