using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case.Management.Concern;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Records;
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
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Concern
{
	[Parallelizable(ParallelScope.All)]
	public class ClosurePageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_Return_Page()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockLogger = new Mock<ILogger<ClosurePageModel>>();

			var caseModel = CaseFactory.BuildCaseModel();
			var recordModel = RecordFactory.BuildRecordModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			mockRecordModelService.Setup(r => r.GetRecordModelById(It.IsAny<long>(), It.IsAny<long>()))
				.ReturnsAsync(recordModel);
			mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);

			var pageModel = SetupClosurePageModel(
				mockCaseModelService.Object,
				mockRecordModelService.Object,
				mockTrustModelService.Object,
				mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("recordId", 1);

			// act
			await pageModel.OnGet();

			// assert
			Assert.IsNotNull(pageModel);
			Assert.IsNotNull(pageModel.TrustDetailsModel);
			Assert.IsNotNull(pageModel.TrustDetailsModel);
			Assert.IsNull(pageModel.TempData["Error.Message"]);

			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("ClosurePageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);

			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<long>()), Times.Once);
			mockRecordModelService.Verify(r => r.GetRecordModelById(It.IsAny<long>(), It.IsAny<long>()), Times.Once);
			mockTrustModelService.Verify(t => t.GetTrustByUkPrn(It.IsAny<string>()), Times.Once);
		}

		[Test]
		public async Task WhenOnGetAsync_CaseUrn_RecordIdIsNullOrEmpty_Returns_Page()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockLogger = new Mock<ILogger<ClosurePageModel>>();

			var pageModel = SetupClosurePageModel(
				mockCaseModelService.Object,
				mockRecordModelService.Object,
				mockTrustModelService.Object,
				mockLogger.Object);

			// act
			await pageModel.OnGet();

			// assert
			Assert.IsNotNull(pageModel);
			Assert.IsNull(pageModel.TrustDetailsModel);
			Assert.IsNull(pageModel.TrustDetailsModel);
			Assert.IsNotNull(pageModel.TempData["Error.Message"]);

			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("ClosurePageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);

			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<long>()), Times.Never);
			mockRecordModelService.Verify(r => r.GetRecordModelById(It.IsAny<long>(), It.IsAny<long>()), Times.Never);
			mockTrustModelService.Verify(t => t.GetTrustByUkPrn(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public async Task WhenOnGetAsync_RecordIdIsNullOrEmpty_Returns_Page()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockLogger = new Mock<ILogger<ClosurePageModel>>();

			var pageModel = SetupClosurePageModel(
				mockCaseModelService.Object,
				mockRecordModelService.Object,
				mockTrustModelService.Object,
				mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			// act
			await pageModel.OnGet();

			// assert
			Assert.IsNotNull(pageModel);
			Assert.IsNull(pageModel.TrustDetailsModel);
			Assert.IsNull(pageModel.TrustDetailsModel);
			Assert.IsNotNull(pageModel.TempData["Error.Message"]);

			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("ClosurePageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);

			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<long>()), Times.Never);
			mockRecordModelService.Verify(r => r.GetRecordModelById(It.IsAny<long>(), It.IsAny<long>()), Times.Never);
			mockTrustModelService.Verify(t => t.GetTrustByUkPrn(It.IsAny<string>()), Times.Never);
		}
		
		[Test]
		public async Task WhenOnGetCloseConcern_Return_ManagementPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockLogger = new Mock<ILogger<ClosurePageModel>>();

			var caseModel = CaseFactory.BuildCaseModel();
			var recordModel = RecordFactory.BuildRecordModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			var expectedUrl = $"/case/{caseModel.Urn}/management";

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			mockRecordModelService.Setup(r => r.GetRecordModelById(It.IsAny<long>(), It.IsAny<long>()))
				.ReturnsAsync(recordModel);
			mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);

			var pageModel = SetupClosurePageModel(
				mockCaseModelService.Object,
				mockRecordModelService.Object,
				mockTrustModelService.Object,
				mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseModel.Urn);
			routeData.Add("recordId", 1);

			// act
			var pageResponse = await pageModel.OnGetCloseConcern();
			var pageResponseInstance = pageResponse as RedirectResult;

			// assert
			Assert.IsNotNull(pageModel);
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			Assert.IsNotNull(pageResponseInstance);
			Assert.That(pageResponseInstance.Url, Is.EqualTo(expectedUrl));

			mockRecordModelService.Verify(r => r.PatchRecordStatus(It.IsAny<PatchRecordModel>()), Times.Once);
		}

		[Test]
		public async Task WhenOnGetCloseConcern_CaseUrn_RecordIdIsNullOrEmpty_Return_Page()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockLogger = new Mock<ILogger<ClosurePageModel>>();

			var pageModel = SetupClosurePageModel(
				mockCaseModelService.Object,
				mockRecordModelService.Object,
				mockTrustModelService.Object,
				mockLogger.Object);

			// act
			var pageResponse = await pageModel.OnGetCloseConcern();

			// assert
			Assert.NotNull(pageResponse);
			Assert.IsNotNull(pageModel);
			Assert.IsNull(pageModel.TrustDetailsModel);
			Assert.IsNull(pageModel.TrustDetailsModel);
			Assert.IsNotNull(pageModel.TempData["Error.Message"]);

			mockRecordModelService.Verify(r => r.PatchRecordStatus(It.IsAny<PatchRecordModel>()), Times.Never);
		}
		
		[Test]
		public void WhenOnGetCancel_Return_ManagementPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockLogger = new Mock<ILogger<ClosurePageModel>>();

			var caseModel = CaseFactory.BuildCaseModel();
			var recordModel = RecordFactory.BuildRecordModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			var expectedUrl = $"/case/{caseModel.Urn}/management";

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			mockRecordModelService.Setup(r => r.GetRecordModelById(It.IsAny<long>(), It.IsAny<long>()))
				.ReturnsAsync(recordModel);
			mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);

			var pageModel = SetupClosurePageModel(
				mockCaseModelService.Object,
				mockRecordModelService.Object,
				mockTrustModelService.Object,
				mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseModel.Urn);
			routeData.Add("recordId", 1);

			// act
			var pageResponse = pageModel.OnGetCancel();
			var pageResponseInstance = pageResponse as RedirectResult;

			// assert
			Assert.IsNotNull(pageModel);
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			Assert.IsNotNull(pageResponseInstance);
			Assert.That(pageResponseInstance.Url, Is.EqualTo(expectedUrl));
		}

		[Test]
		public void WhenOnGetCancel_CaseUrn_RecordIdIsNullOrEmpty_Return_Page()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockLogger = new Mock<ILogger<ClosurePageModel>>();

			var pageModel = SetupClosurePageModel(
				mockCaseModelService.Object,
				mockRecordModelService.Object,
				mockTrustModelService.Object,
				mockLogger.Object);

			// act
			var pageResponse = pageModel.OnGetCancel();

			// assert
			Assert.NotNull(pageResponse);
			Assert.IsNotNull(pageModel);
			Assert.IsNull(pageModel.TrustDetailsModel);
			Assert.IsNull(pageModel.TrustDetailsModel);
			Assert.IsNotNull(pageModel.TempData["Error.Message"]);
		}

		private static ClosurePageModel SetupClosurePageModel(
			ICaseModelService mockCaseModelService,
			IRecordModelService mockRecordModelService,
			ITrustModelService mockTrustModelService,
			ILogger<ClosurePageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new ClosurePageModel(mockCaseModelService, 
				mockRecordModelService, 
				mockTrustModelService, 
				mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}