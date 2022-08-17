using ConcernsCaseWork.Pages.Case.Management.Action.FinancialPlan;
using ConcernsCaseWork.Services.FinancialPlan;
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
using Service.Redis.FinancialPlan;
using Service.Redis.Models;
using ConcernsCasework.Service.FinancialPlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.FinancialPlan
{
	[Parallelizable(ParallelScope.All)]
	public class AddPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockFinancialPlanStatusService = new Mock<IFinancialPlanStatusCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockFinancialPlanModelService.Object, mockFinancialPlanStatusService.Object, mockLogger.Object);
			var validStatuses = GetListValidStatuses();
			
			mockFinancialPlanStatusService.Setup(fp => fp.GetOpenFinancialPlansStatusesAsync())
				.ReturnsAsync(validStatuses);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 4);

			// act
			var response = await pageModel.OnGetAsync();

			// assert
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::FinancialPlan::AddPageModel::OnGetAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
			
			Assert.IsInstanceOf<PageResult>(response);
			Assert.IsNotNull(pageModel.FinancialPlanStatuses);
			Assert.AreEqual(2, pageModel.FinancialPlanStatuses.Count());
			Assert.IsFalse(pageModel.FinancialPlanStatuses.Any(s => s.IsChecked));

			var testStatus1 = pageModel.FinancialPlanStatuses.First();
			Assert.IsTrue(validStatuses.Select(s => s.Description).Contains(testStatus1.Text));
			Assert.IsTrue(validStatuses.Select(s => s.Name).Contains(testStatus1.Id));
			
			var testStatus2 = pageModel.FinancialPlanStatuses.Last();
			Assert.IsTrue(validStatuses.Select(s => s.Description).Contains(testStatus2.Text));
			Assert.IsTrue(validStatuses.Select(s => s.Name).Contains(testStatus2.Id));
		}

		[Test]
		public async Task WhenOnPostAsync_MissingCaseUrn_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockFinancialPlanStatusService = new Mock<IFinancialPlanStatusCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockFinancialPlanModelService.Object, mockFinancialPlanStatusService.Object, mockLogger.Object);

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
		}

		[Test]
		public async Task WhenOnPostAsync_Invalid_DatePlanRequested_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockFinancialPlanStatusService = new Mock<IFinancialPlanStatusCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockFinancialPlanModelService.Object, mockFinancialPlanStatusService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "dtr-day-plan-requested", new StringValues("00") },
					{ "dtr-month-plan-requested", new StringValues("00") },
					{ "dtr-year-plan-requested", new StringValues("0000") },
				});

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["FinancialPlan.Message"], Is.EqualTo("Plan requested 00-00-0000 is an invalid date"));
		}

		[Test]
		public async Task WhenOnPostAsync_Invalid_ViablePlanReceivedDate_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockFinancialPlanStatusService = new Mock<IFinancialPlanStatusCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockFinancialPlanModelService.Object, mockFinancialPlanStatusService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "dtr-day-viable-plan", new StringValues("00") },
					{ "dtr-month-viable-plan", new StringValues("00") },
					{ "dtr-year-viable-plan", new StringValues("0000") },
				});

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["FinancialPlan.Message"], Is.EqualTo("Viable plan 00-00-0000 is an invalid date"));
		}
		[Test]
		public async Task WhenOnPostAsync_PartialViablePlanDate_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockFinancialPlanStatusService = new Mock<IFinancialPlanStatusCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockFinancialPlanModelService.Object, mockFinancialPlanStatusService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "dtr-day-viable-plan", new StringValues("02") },
					{ "dtr-month-viable-plan", new StringValues("04") },
				});

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["FinancialPlan.Message"], Is.EqualTo("Viable plan 02-04- is an invalid date"));
		}
		
		[Test]
		public async Task WhenOnPostAsync_Partial_RequestedDate_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockFinancialPlanStatusService = new Mock<IFinancialPlanStatusCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockFinancialPlanModelService.Object, mockFinancialPlanStatusService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "dtr-day-plan-requested", new StringValues("02") },
					{ "dtr-month-plan-requested", new StringValues("04") },
				});

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["FinancialPlan.Message"], Is.EqualTo("Plan requested 02-04- is an invalid date"));
		}
		
		[Test]
		public async Task WhenOnPostAsync_WithValidDatePlanRequested_Succeeds()
		{
			// arrange
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockFinancialPlanStatusService = new Mock<IFinancialPlanStatusCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var statuses = FinancialPlanStatusFactory.BuildListOpenFinancialPlanStatusDto();
			var caseUrn = 1L;
			
			mockFinancialPlanStatusService.Setup(s => s.GetOpenFinancialPlansStatusesAsync()).ReturnsAsync(statuses);
			
			var pageModel = SetupAddPageModel(mockFinancialPlanModelService.Object, mockFinancialPlanStatusService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseUrn);

			var day = 2;
			var month = 4;
			var year = 2022;

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "dtr-day-plan-requested", new StringValues($"0{day}") },
					{ "dtr-month-plan-requested", new StringValues($"0{month}") },
					{ "dtr-year-plan-requested", new StringValues(year.ToString()) }
				});

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			mockFinancialPlanModelService.Verify(f => f.PostFinancialPlanByCaseUrn(It.Is<CreateFinancialPlanModel>(fpm =>
					fpm.DatePlanRequested == new DateTime(year, month, day) &&
					fpm.DateViablePlanReceived == null), 
				It.IsAny<string>()), Times.Once);
			
			Assert.IsNotNull(pageResponse);
			Assert.IsNull(pageModel.TempData["Error.Message"]);
		}
		
		[Test]
		public async Task WhenOnPostAsync_WithValidDateViablePlanReceived_Succeeds()
		{
			// arrange
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockFinancialPlanStatusService = new Mock<IFinancialPlanStatusCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var statuses = FinancialPlanStatusFactory.BuildListOpenFinancialPlanStatusDto();
			var caseUrn = 1L;
			
			var pageModel = SetupAddPageModel(mockFinancialPlanModelService.Object, mockFinancialPlanStatusService.Object, mockLogger.Object);
			
			mockFinancialPlanStatusService.Setup(s => s.GetOpenFinancialPlansStatusesAsync()).ReturnsAsync(statuses);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseUrn);

			var day = 28;
			var month = 12;
			var year = 2024;

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "dtr-day-viable-plan", new StringValues(day.ToString()) },
					{ "dtr-month-viable-plan", new StringValues(month.ToString()) },
					{ "dtr-year-viable-plan", new StringValues(year.ToString()) }
				});

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			mockFinancialPlanModelService.Verify(f => 
				f.PostFinancialPlanByCaseUrn(It.Is<CreateFinancialPlanModel>(fpm => 
						fpm.DateViablePlanReceived == new DateTime(year, month, day) &&
				        fpm.DatePlanRequested == null), 
				It.IsAny<string>()), Times.Once);
			
			Assert.IsNotNull(pageResponse);
			Assert.IsNull(pageModel.TempData["Error.Message"]);
		}

		private static AddPageModel SetupAddPageModel(
			IFinancialPlanModelService mockFinancialPlanModelService, 
			IFinancialPlanStatusCachedService mockFinancialPlanStatusService,
			ILogger<AddPageModel> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new AddPageModel(mockFinancialPlanModelService, mockFinancialPlanStatusService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
		
		private static List<FinancialPlanStatusDto> GetListValidStatuses() => FinancialPlanStatusFactory.BuildListOpenFinancialPlanStatusDto().ToList();
	}


}