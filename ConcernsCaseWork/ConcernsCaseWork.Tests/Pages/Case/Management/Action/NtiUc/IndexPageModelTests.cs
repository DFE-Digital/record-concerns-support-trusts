using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Case.Management.Action.NtiUnderConsideration;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.NtiUnderConsideration;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.NtiUc
{
	[Parallelizable(ParallelScope.All)]
	public class IndexPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_MissingCaseUrn_ThrowsException_ReturnPage()
		{
			// arrange
			Mock<INtiUnderConsiderationModelService> ntiUnderConsiderationModel = new Mock<INtiUnderConsiderationModelService>();
			Mock<ILogger<IndexPageModel>> mockLogger = new Mock<ILogger<IndexPageModel>>();

			var pageModel = SetupIndexPageModel(ntiUnderConsiderationModel, mockLogger);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
		}

		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			Mock<INtiUnderConsiderationModelService> mockNtiUnderConsiderationModelService = new Mock<INtiUnderConsiderationModelService>();
			Mock<ILogger<IndexPageModel>> mockLogger = new Mock<ILogger<IndexPageModel>>();

			var ntiUnderConsiderationModel = NTIUnderConsiderationFactory.BuildNTIUnderConsiderationModel();

			mockNtiUnderConsiderationModelService.Setup(n => n.GetNtiUnderConsideration(ntiUnderConsiderationModel.Id))
				.ReturnsAsync(ntiUnderConsiderationModel);

			var expectedModel = Copy(ntiUnderConsiderationModel);

			var pageModel = SetupIndexPageModel(mockNtiUnderConsiderationModelService, mockLogger);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("ntiUnderConsiderationId", ntiUnderConsiderationModel.Id);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.IsNotNull(pageModel.NTIUnderConsiderationModel);
			Assert.AreEqual(JsonConvert.SerializeObject(expectedModel), JsonConvert.SerializeObject(ntiUnderConsiderationModel));
			
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::NTI-UC::IndexPageModel::OnGetAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}
		
		[Test]
		public async Task WhenOnGetAsync_StatusIsClosed_ReturnsPageModel()
		{
			// arrange
			Mock<INtiUnderConsiderationModelService> mockNtiUnderConsiderationModelService = new Mock<INtiUnderConsiderationModelService>();
			Mock<ILogger<IndexPageModel>> mockLogger = new Mock<ILogger<IndexPageModel>>();
			
			var ntiUnderConsiderationModel = NTIUnderConsiderationFactory.BuildClosedNTIUnderConsiderationModel();
			var expectedModel = Copy(ntiUnderConsiderationModel);

			mockNtiUnderConsiderationModelService.Setup(n => n.GetNtiUnderConsideration(ntiUnderConsiderationModel.Id))
				.ReturnsAsync(ntiUnderConsiderationModel);

			var pageModel = SetupIndexPageModel(mockNtiUnderConsiderationModelService, mockLogger);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("ntiUnderConsiderationId", ntiUnderConsiderationModel.Id);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.IsNotNull(pageModel.NTIUnderConsiderationModel);
			Assert.AreEqual(JsonConvert.SerializeObject(expectedModel), JsonConvert.SerializeObject(ntiUnderConsiderationModel));

			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::NTI-UC::IndexPageModel::OnGetAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}

		[Test]
		public async Task WhenOnGetAsync_MissingNTIUnderConsideration_ThrowsException_ReturnPage()
		{
			// arrange
			Mock<INtiUnderConsiderationModelService> ntiUnderConsiderationModel = new Mock<INtiUnderConsiderationModelService>();
			Mock<ILogger<IndexPageModel>> mockLogger = new Mock<ILogger<IndexPageModel>>();

			var pageModel = SetupIndexPageModel(ntiUnderConsiderationModel, mockLogger);

			var routeData = pageModel.RouteData.Values;

			routeData.Add("urn", 1);
			routeData.Add("ntiUnderConsiderationId", 1);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
		}


		private static IndexPageModel SetupIndexPageModel(
			Mock<INtiUnderConsiderationModelService> mockNtiModelService,
			Mock<ILogger<IndexPageModel>> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new IndexPageModel(mockNtiModelService.Object, mockLogger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}

		private static NtiUnderConsiderationModel Copy(NtiUnderConsiderationModel model)
			=> JsonConvert.DeserializeObject<NtiUnderConsiderationModel>(JsonConvert.SerializeObject(model));
	}
}