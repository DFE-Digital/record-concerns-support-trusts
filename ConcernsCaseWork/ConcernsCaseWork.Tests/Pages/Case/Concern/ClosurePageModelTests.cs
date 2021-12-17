using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case.Concern;
using ConcernsCaseWork.Pages.Case.Management.Concern;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Services.Types;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Base;
using Service.Redis.Models;
using Service.TRAMS.Status;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Concern
{
	[Parallelizable(ParallelScope.All)]
	public class ClosurePageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_Return_Page()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockStatusModelService = new Mock<IStatusService>();
			var mockLogger = new Mock<ILogger<ClosurePageModel>>();

			var caseModel = CaseFactory.BuildCaseModel();
			var recordModel = RecordFactory.BuildRecordModel();
			var ratingsmodel = RatingFactory.BuildListRatingModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			var typeModel = TypeFactory.BuildTypeModel();

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			mockRecordModelService.Setup(r => r.GetRecordModelByUrn(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>()))
				.ReturnsAsync(recordModel);
			mockRatingModelService.Setup(r => r.GetSelectedRatingsModelByUrn(It.IsAny<long>()))
				.ReturnsAsync(ratingsmodel);
			mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);
			mockTypeModelService.Setup(t => t.GetSelectedTypeModelByUrn(It.IsAny<long>()))
				.ReturnsAsync(typeModel);

			var pageModel = SetupClosurePageModel(
				mockCaseModelService.Object,
				mockRatingModelService.Object,
				mockRecordModelService.Object,
				mockTrustModelService.Object,
				mockTypeModelService.Object,
				mockStatusModelService.Object,
				mockLogger.Object);


			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("recordUrn", 1);

			// act
			await pageModel.OnGet();

			// assert
			Assert.IsNotNull(pageModel);
			Assert.IsNotNull(pageModel.TrustDetailsModel);
			Assert.IsNotNull(pageModel.RatingsModel);
			Assert.IsNotNull(pageModel.TrustDetailsModel);
			Assert.IsNotNull(pageModel.TypeModel);
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

			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Once);
			mockRecordModelService.Verify(r => r.GetRecordModelByUrn(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>()), Times.Once);
			mockRatingModelService.Verify(r => r.GetSelectedRatingsModelByUrn(It.IsAny<long>()), Times.Once);
			mockTrustModelService.Verify(t => t.GetTrustByUkPrn(It.IsAny<string>()), Times.Once);
			mockTypeModelService.Verify(t => t.GetSelectedTypeModelByUrn(It.IsAny<long>()), Times.Once);
		}

		[Test]
		public async Task WhenOnGetAsync_CaseUrn_RecordUrnIsNullOrEmpty_Returns_Page()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockStatusModelService = new Mock<IStatusService>();
			var mockLogger = new Mock<ILogger<ClosurePageModel>>();

			var pageModel = SetupClosurePageModel(
				mockCaseModelService.Object,
				mockRatingModelService.Object,
				mockRecordModelService.Object,
				mockTrustModelService.Object,
				mockTypeModelService.Object,
				mockStatusModelService.Object,
				mockLogger.Object);


			// act
			await pageModel.OnGet();

			// assert
			Assert.IsNotNull(pageModel);
			Assert.IsNull(pageModel.TrustDetailsModel);
			Assert.IsNull(pageModel.RatingsModel);
			Assert.IsNull(pageModel.TrustDetailsModel);
			Assert.IsNull(pageModel.TypeModel);
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

			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
			mockRecordModelService.Verify(r => r.GetRecordModelByUrn(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>()), Times.Never);
			mockRatingModelService.Verify(r => r.GetSelectedRatingsModelByUrn(It.IsAny<long>()), Times.Never);
			mockTrustModelService.Verify(t => t.GetTrustByUkPrn(It.IsAny<string>()), Times.Never);
			mockTypeModelService.Verify(t => t.GetSelectedTypeModelByUrn(It.IsAny<long>()), Times.Never);
		}

		[Test]
		public async Task WhenOnGetCloseConcern_Return_ManagementPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockStatusModelService = new Mock<IStatusService>();
			var mockLogger = new Mock<ILogger<ClosurePageModel>>();

			var caseModel = CaseFactory.BuildCaseModel();
			var recordModel = RecordFactory.BuildRecordModel();
			var ratingsmodel = RatingFactory.BuildListRatingModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			var typeModel = TypeFactory.BuildTypeModel();
			var statusesModel = StatusFactory.BuildListStatusDto();
			var expectedUrl = $"/case/{caseModel.Urn}/management";

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			mockRecordModelService.Setup(r => r.GetRecordModelByUrn(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>()))
				.ReturnsAsync(recordModel);
			mockRatingModelService.Setup(r => r.GetSelectedRatingsModelByUrn(It.IsAny<long>()))
				.ReturnsAsync(ratingsmodel);
			mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);
			mockTypeModelService.Setup(t => t.GetSelectedTypeModelByUrn(It.IsAny<long>()))
				.ReturnsAsync(typeModel);
			mockStatusModelService.Setup(s => s.GetStatuses())
				.ReturnsAsync(statusesModel);

			var pageModel = SetupClosurePageModel(
				mockCaseModelService.Object,
				mockRatingModelService.Object,
				mockRecordModelService.Object,
				mockTrustModelService.Object,
				mockTypeModelService.Object,
				mockStatusModelService.Object,
				mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseModel.Urn);
			routeData.Add("recordUrn", 1);

			// act
			var pageResponse = await pageModel.OnGetCloseConcern();
			var pageResponseInstance = pageResponse as RedirectResult;

			// assert
			Assert.IsNotNull(pageModel);
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			Assert.IsNotNull(pageResponseInstance);
			Assert.That(pageResponseInstance.Url, Is.EqualTo(expectedUrl));

			mockStatusModelService.Verify(s => s.GetStatuses(), Times.Once);
			mockRecordModelService.Verify(r => r.PatchRecordStatus(It.IsAny<PatchRecordModel>()), Times.Once);
		}

		[Test]
		public async Task WhenOnGetCloseConcern_CaseUrn_RecordUrnIsNullOrEmpty_Return_Page()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockStatusModelService = new Mock<IStatusService>();
			var mockLogger = new Mock<ILogger<ClosurePageModel>>();

			var pageModel = SetupClosurePageModel(
				mockCaseModelService.Object,
				mockRatingModelService.Object,
				mockRecordModelService.Object,
				mockTrustModelService.Object,
				mockTypeModelService.Object,
				mockStatusModelService.Object,
				mockLogger.Object);

			// act
			var pageResponse = await pageModel.OnGetCloseConcern();
			var pageResponseInstance = pageResponse as RedirectResult;

			// assert
			Assert.IsNotNull(pageModel);
			Assert.IsNull(pageModel.TrustDetailsModel);
			Assert.IsNull(pageModel.RatingsModel);
			Assert.IsNull(pageModel.TrustDetailsModel);
			Assert.IsNull(pageModel.TypeModel);
			Assert.IsNotNull(pageModel.TempData["Error.Message"]);

			mockStatusModelService.Verify(s => s.GetStatuses(), Times.Never);
			mockRecordModelService.Verify(r => r.PatchRecordStatus(It.IsAny<PatchRecordModel>()), Times.Never);
		}


		[Test]
		public void WhenOnGetCancel_Return_ManagementPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockStatusModelService = new Mock<IStatusService>();
			var mockLogger = new Mock<ILogger<ClosurePageModel>>();

			var caseModel = CaseFactory.BuildCaseModel();
			var recordModel = RecordFactory.BuildRecordModel();
			var ratingsmodel = RatingFactory.BuildListRatingModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			var typeModel = TypeFactory.BuildTypeModel();
			var expectedUrl = $"/case/{caseModel.Urn}/management";

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			mockRecordModelService.Setup(r => r.GetRecordModelByUrn(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>()))
				.ReturnsAsync(recordModel);
			mockRatingModelService.Setup(r => r.GetSelectedRatingsModelByUrn(It.IsAny<long>()))
				.ReturnsAsync(ratingsmodel);
			mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);
			mockTypeModelService.Setup(t => t.GetSelectedTypeModelByUrn(It.IsAny<long>()))
				.ReturnsAsync(typeModel);

			var pageModel = SetupClosurePageModel(
				mockCaseModelService.Object,
				mockRatingModelService.Object,
				mockRecordModelService.Object,
				mockTrustModelService.Object,
				mockTypeModelService.Object,
				mockStatusModelService.Object,
				mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseModel.Urn);
			routeData.Add("recordUrn", 1);

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
		public void WhenOnGetCancel_CaseUrn_RecordUrnIsNullOrEmpty_Return_Page()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockStatusModelService = new Mock<IStatusService>();
			var mockLogger = new Mock<ILogger<ClosurePageModel>>();

			var pageModel = SetupClosurePageModel(
				mockCaseModelService.Object,
				mockRatingModelService.Object,
				mockRecordModelService.Object,
				mockTrustModelService.Object,
				mockTypeModelService.Object,
				mockStatusModelService.Object,
				mockLogger.Object);

			// act
			var pageResponse = pageModel.OnGetCancel();

			// assert
			Assert.IsNotNull(pageModel);
			Assert.IsNull(pageModel.TrustDetailsModel);
			Assert.IsNull(pageModel.RatingsModel);
			Assert.IsNull(pageModel.TrustDetailsModel);
			Assert.IsNull(pageModel.TypeModel);
			Assert.IsNotNull(pageModel.TempData["Error.Message"]);
		}

		private static ClosurePageModel SetupClosurePageModel(
			ICaseModelService mockCaseModelService,
			IRatingModelService mockRatingModelService,
			IRecordModelService mockRecordModelService,
			ITrustModelService mockTrustModelService,
			ITypeModelService mockTypeModelService,
			IStatusService mockStatusService,
			ILogger<ClosurePageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new ClosurePageModel(mockCaseModelService, 
				mockRatingModelService, 
				mockRecordModelService, 
				mockTrustModelService, 
				mockTypeModelService, 
				mockStatusService, 
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