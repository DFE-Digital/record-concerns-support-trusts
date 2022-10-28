using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Pages.Case.Management.Action.SRMA;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.SRMA.Edit
{
	[Parallelizable(ParallelScope.All)]
	public class EditDateOfVisitPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_ReturnsPage()
		{
			// arrange 
			var mockSRMAModelService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<EditDateOfVisitPageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

			mockSRMAModelService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupDateOfVisitPageModel(mockSRMAModelService.Object, mockLogger.Object);
			var routeData = pageModel.RouteData.Values;
			routeData.Add("caseUrn", srmaModel.CaseUrn);
			routeData.Add("srmaId", srmaModel.Id);

			// act
			var pageResponse = await pageModel.OnGetAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.SRMAModel, Is.Not.Null);
		}
		
		[Test]
		public async Task WhenOnGetAsync_AndSrmaIsOpen_RedirectsToClosedPage()
		{
			// arrange 
			var mockSRMAModelService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<EditDateOfVisitPageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed, closedAt:DateTime.Now);

			mockSRMAModelService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupDateOfVisitPageModel(mockSRMAModelService.Object, mockLogger.Object);
			var routeData = pageModel.RouteData.Values;
			routeData.Add("caseUrn", srmaModel.CaseUrn);
			routeData.Add("srmaId", srmaModel.Id);

			// act
			var pageResponse = await pageModel.OnGetAsync();
			
			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;

			Assert.That(page?.Url, Is.EqualTo($"/case/{srmaModel.CaseUrn}/management/action/srma/{srmaModel.Id}/closed"));
			Assert.That(pageModel.TempData["Error.Message"], Is.Null);
		}

		[Test]
		public async Task WhenOnGetAsync_MissingRouteData_CaseUrn_ThrowsException_ReturnsPage()
		{
			// arrange 
			var mockSRMAModelService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<EditDateOfVisitPageModel>>();

			var pageModel = SetupDateOfVisitPageModel(mockSRMAModelService.Object, mockLogger.Object);

			// act 
			var pageResponse = await pageModel.OnGetAsync();
			var routeData = pageModel.RouteData.Values;
			routeData.Add("SrmaId", 1);

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.IsNull(pageModel.SRMAModel);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));

			mockSRMAModelService.Verify(s =>
				s.GetSRMAById(It.IsAny<long>()), Times.Never);
		}

		[Test]
		public async Task WhenOnGetAsync_MissingRouteData_SrmaID_ThrowsException_ReturnsPage()
		{
			// arrange 
			var mockSRMAModelService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<EditDateOfVisitPageModel>>();

			var pageModel = SetupDateOfVisitPageModel(mockSRMAModelService.Object, mockLogger.Object);
			var routeData = pageModel.RouteData.Values;
			routeData.Add("caseUrn", 1);

			// act 
			var pageResponse = await pageModel.OnGetAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.IsNull(pageModel.SRMAModel);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));

			mockSRMAModelService.Verify(s =>
				s.GetSRMAById(It.IsAny<long>()), Times.Never);
		}

		[Test]
		public async Task WhenOnPostAsync_MissingRouteData_ThrowsException_ReturnsPage()
		{
			// arrange 
			var mockSRMAModelService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<EditDateOfVisitPageModel>>();

			var pageModel = SetupDateOfVisitPageModel(mockSRMAModelService.Object, mockLogger.Object);

			// act 
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.IsNull(pageModel.SRMAModel);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));

			mockSRMAModelService.Verify(s =>
				s.GetSRMAById(It.IsAny<long>()), Times.Never);
		}

		[Test]
		public async Task WhenOnPostAsync_RouteData_RequestForm_Return_To_SRMA_Page()
		{
			// arrange 
			var mockSRMAModelService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<EditDateOfVisitPageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

			var pageModel = SetupDateOfVisitPageModel(mockSRMAModelService.Object, mockLogger.Object);
			var routeData = pageModel.RouteData.Values;
			routeData.Add("caseUrn", srmaModel.CaseUrn);
			routeData.Add("srmaId", srmaModel.Id);

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "start-dtr-day", new StringValues("07") },
					{ "start-dtr-month", new StringValues("04") },
					{ "start-dtr-year", new StringValues("2022") },
					{ "end-dtr-day", new StringValues("07") },
					{ "end-dtr-month", new StringValues("04") },
					{ "end-dtr-year", new StringValues("2022") },
				});

			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(page.Url, Is.EqualTo($"/case/{srmaModel.CaseUrn}/management/action/srma/{srmaModel.Id}"));

		}

		[Test]
		public async Task WhenOnPostAsync_RouteData_RequestForm_InvalidStartDate_ThrowsApplicationException_Page()
		{
			// arrange 
			var mockSRMAModelService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<EditDateOfVisitPageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

			var pageModel = SetupDateOfVisitPageModel(mockSRMAModelService.Object, mockLogger.Object);
			var routeData = pageModel.RouteData.Values;
			routeData.Add("caseUrn", srmaModel.CaseUrn);
			routeData.Add("srmaId", srmaModel.Id);

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "start-dtr-day", new StringValues("00") },
					{ "start-dtr-month", new StringValues("04") },
					{ "start-dtr-year", new StringValues("2022") },
					{ "end-dtr-day", new StringValues("07") },
					{ "end-dtr-month", new StringValues("04") },
					{ "end-dtr-year", new StringValues("2022") },
				});

			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.IsNull(pageModel.SRMAModel);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["SRMA.Message"], Is.EqualTo("Start date 00-04-2022 is an invalid date"));
		}

		[Test]
		public async Task WhenOnPostAsync_RouteData_RequestForm_InvalidEndDate_ThrowsApplicationException_Page()
		{
			// arrange 
			var mockSRMAModelService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<EditDateOfVisitPageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

			var pageModel = SetupDateOfVisitPageModel(mockSRMAModelService.Object, mockLogger.Object);
			var routeData = pageModel.RouteData.Values;
			routeData.Add("caseUrn", srmaModel.CaseUrn);
			routeData.Add("srmaId", srmaModel.Id);

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "start-dtr-day", new StringValues("01") },
					{ "start-dtr-month", new StringValues("04") },
					{ "start-dtr-year", new StringValues("2022") },
					{ "end-dtr-day", new StringValues("00") },
					{ "end-dtr-month", new StringValues("04") },
					{ "end-dtr-year", new StringValues("2022") },
				});

			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.IsNull(pageModel.SRMAModel);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["SRMA.Message"], Is.EqualTo("End date 00-04-2022 is an invalid date"));
		}

		[Test]
		public async Task WhenOnPostAsync_RouteData_RequestForm_EndDate_Is_Before_StartDate_ThrowsApplicationException_Page()
		{
			// arrange 
			var mockSRMAModelService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<EditDateOfVisitPageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

			var pageModel = SetupDateOfVisitPageModel(mockSRMAModelService.Object, mockLogger.Object);
			var routeData = pageModel.RouteData.Values;
			routeData.Add("caseUrn", srmaModel.CaseUrn);
			routeData.Add("srmaId", srmaModel.Id);

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "start-dtr-day", new StringValues("03") },
					{ "start-dtr-month", new StringValues("04") },
					{ "start-dtr-year", new StringValues("2022") },
					{ "end-dtr-day", new StringValues("02") },
					{ "end-dtr-month", new StringValues("04") },
					{ "end-dtr-year", new StringValues("2022") },
				});

			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.IsNull(pageModel.SRMAModel);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["SRMA.Message"], Is.EqualTo("Please ensure end date is same as or after start date."));
		}

		private static EditDateOfVisitPageModel SetupDateOfVisitPageModel(
		ISRMAService mockSrmaModelService, ILogger<EditDateOfVisitPageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new EditDateOfVisitPageModel(mockSrmaModelService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};

		}
	}
}
