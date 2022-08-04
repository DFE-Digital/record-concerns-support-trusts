	using ConcernsCaseWork.Models.CaseActions;
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
	using Service.TRAMS.FinancialPlan;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.FinancialPlan
	{
		[Parallelizable(ParallelScope.All)]
		public class ClosePageModelTests
		{
			[Test]
			public async Task WhenOnGetAsync_MissingCaseUrn_ThrowsException_ReturnPage()
			{
				// arrange
				var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
				var mockFinancialPlanStatusService = new Mock<IFinancialPlanStatusCachedService>();

				var mockLogger = new Mock<ILogger<ClosePageModel>>();

				var pageModel = SetupClosePageModel(mockFinancialPlanModelService.Object, mockFinancialPlanStatusService.Object, mockLogger.Object);

				// act
				await pageModel.OnGetAsync();

				// assert
				Assert.That(pageModel.TempData["Error.Message"],
					Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
			}

			[Test]
			public async Task WhenOnGetAsync_ReturnsPageModel()
			{
				// arrange
				var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
				var mockFinancialPlanStatusService = new Mock<IFinancialPlanStatusCachedService>();
				var mockLogger = new Mock<ILogger<ClosePageModel>>();

				var pageModel = SetupClosePageModel(mockFinancialPlanModelService.Object, mockFinancialPlanStatusService.Object, mockLogger.Object);

				var routeData = pageModel.RouteData.Values;
				routeData.Add("urn", 1);

				// act
				await pageModel.OnGetAsync();

				// assert
				mockLogger.Verify(
					m => m.Log(
						LogLevel.Information,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::FinancialPlan::ClosePageModel::OnGetAsync")),
						null,
						It.IsAny<Func<It.IsAnyType, Exception, string>>()),
					Times.Once);
			}

			[Test]
			[TestCase("1", "")]
			[TestCase("", "1")]
			[TestCase("", "")]
			public async Task WhenOnPostAsync_EmptyRouteValues_ThrowsException_ReturnsPage(string urn, string financialId)
			{
				// arrange
				var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
				var mockFinancialPlanStatusService = new Mock<IFinancialPlanStatusCachedService>();
				var mockLogger = new Mock<ILogger<ClosePageModel>>();

				var pageModel = SetupClosePageModel(mockFinancialPlanModelService.Object, mockFinancialPlanStatusService.Object, mockLogger.Object);
				
				var routeData = pageModel.RouteData.Values;
				routeData.Add("urn", urn);
				routeData.Add("financialplanid", financialId);

				// act
				var pageResponse = await pageModel.OnPostAsync();

				// assert
				Assert.That(pageResponse, Is.InstanceOf<PageResult>());
				var page = pageResponse as PageResult;

				Assert.That(page, Is.Not.Null);
				Assert.That(pageModel.TempData, Is.Not.Null);
				Assert.That(pageModel.TempData["Error.Message"],
					Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
			}
			
			[Test]
			public async Task WhenOnPostAsync_MissingFinancialPlanId_ThrowsException_ReturnsPage()
			{
				// arrange
				var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
				var mockFinancialPlanStatusService = new Mock<IFinancialPlanStatusCachedService>();
				var mockLogger = new Mock<ILogger<ClosePageModel>>();

				var pageModel = SetupClosePageModel(mockFinancialPlanModelService.Object, mockFinancialPlanStatusService.Object, mockLogger.Object);
				
				var routeData = pageModel.RouteData.Values;
				routeData.Add("urn", "1");
				routeData.Add("financialplanid", "");

				// act
				var pageResponse = await pageModel.OnPostAsync();

				// assert
				Assert.That(pageResponse, Is.InstanceOf<PageResult>());
				var page = pageResponse as PageResult;

				Assert.That(page, Is.Not.Null);
				Assert.That(pageModel.TempData, Is.Not.Null);
				Assert.That(pageModel.TempData["Error.Message"],
					Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
			}

			[Test]
			[TestCaseSource(nameof(GetListValidStatusNames))]
			public async Task WhenOnPostAsync_Valid_Calls_Patch_Method(string statusName)
			{
				// arrange
				var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
				var mockFinancialPlanStatusService = new Mock<IFinancialPlanStatusCachedService>();
				var mockLogger = new Mock<ILogger<ClosePageModel>>();
				
				mockFinancialPlanStatusService.Setup(fp => fp.GetClosureFinancialPlansStatusesAsync())
					.ReturnsAsync(GetListValidStatuses());

				var pageModel = SetupClosePageModel(mockFinancialPlanModelService.Object, mockFinancialPlanStatusService.Object, mockLogger.Object);

				var caseUrn = 1;
				var routeData = pageModel.RouteData.Values;
				routeData.Add("urn", caseUrn);
				routeData.Add("financialplanid", 1);

				pageModel.HttpContext.Request.Form = new FormCollection(
					new Dictionary<string, StringValues>
					{
						{ "status", statusName}
					});

				// act
				var pageResponse = await pageModel.OnPostAsync();

				// assert
				Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
				var result = pageResponse as RedirectResult;

				Assert.That(result, Is.Not.Null);
				Assert.That(result.Url, Is.EqualTo($"/case/{caseUrn}/management"));
				Assert.That(pageModel.TempData, Is.Not.Null);
				Assert.That(pageModel.TempData["Error.Message"], Is.Null);
				
				mockFinancialPlanModelService.Verify(f => f.PatchFinancialById(It.Is<PatchFinancialPlanModel>(fpm =>
					fpm.ClosedAt != null), It.IsAny<string>()), Times.Once);
			}

			private static ClosePageModel SetupClosePageModel(
				IFinancialPlanModelService mockFinancialPlanModelService,
				IFinancialPlanStatusCachedService mockFinancialPlanStatusService,
				ILogger<ClosePageModel> mockLogger,
				bool isAuthenticated = false)
			{
				(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

				return new ClosePageModel(mockFinancialPlanModelService, mockFinancialPlanStatusService, mockLogger)
				{
					PageContext = pageContext, TempData = tempData, Url = new UrlHelper(actionContext), MetadataProvider = pageContext.ViewData.ModelMetadata
				};
			}
			
			private static List<FinancialPlanStatusDto> GetListValidStatuses() => FinancialPlanStatusFactory.BuildListClosureFinancialPlanStatusDto().ToList();
			private static IEnumerable<string> GetListValidStatusNames() => GetListValidStatuses().Select(dto => dto.Name);
		} 
	}