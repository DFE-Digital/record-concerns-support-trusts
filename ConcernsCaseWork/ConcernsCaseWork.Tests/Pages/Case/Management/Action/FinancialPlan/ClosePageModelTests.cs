using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Case.Management.Action.FinancialPlan;
using ConcernsCaseWork.Redis.FinancialPlan;
using ConcernsCaseWork.Service.FinancialPlan;
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
				Is.EqualTo(ErrorConstants.ErrorOnGetPage));
		}

		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockFinancialPlanStatusService = new Mock<IFinancialPlanStatusCachedService>();
			var mockLogger = new Mock<ILogger<ClosePageModel>>();

			var pageModel = SetupClosePageModel(mockFinancialPlanModelService.Object, mockFinancialPlanStatusService.Object, mockLogger.Object);
			var validStatuses = GetListValidStatuses();
			
			var caseUrn = 4;
			var financialPlanId = 6;
							
			mockFinancialPlanModelService.Setup(fp => fp.GetFinancialPlansModelById(caseUrn, financialPlanId))
				.ReturnsAsync(SetupFinancialPlanModel(financialPlanId, caseUrn, null));

			mockFinancialPlanStatusService.Setup(fp => fp.GetClosureFinancialPlansStatusesAsync())
				.ReturnsAsync(validStatuses);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseUrn);
			routeData.Add("financialplanid", financialPlanId);

			Assert.IsNull(pageModel.FinancialPlanModel);
			
			// act
			var response = await pageModel.OnGetAsync();

			// assert
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::FinancialPlan::ClosePageModel::OnGetAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
			
			Assert.IsInstanceOf<PageResult>(response);
			Assert.IsNotNull(pageModel.FinancialPlanModel);
			Assert.That(financialPlanId, Is.EqualTo(pageModel.FinancialPlanModel.Id));
			Assert.That(caseUrn, Is.EqualTo(pageModel.FinancialPlanModel.CaseUrn));
			Assert.IsNotNull(pageModel.FinancialPlanStatuses);
			Assert.That(pageModel.FinancialPlanStatuses.Count(), Is.EqualTo(2));
			
			Assert.IsFalse(pageModel.FinancialPlanStatuses.Any(s => s.IsChecked));

			var testStatus1 = pageModel.FinancialPlanStatuses.First();
			Assert.IsTrue(validStatuses.Select(s => s.Description).Contains(testStatus1.Text));
			Assert.IsTrue(validStatuses.Select(s => s.Name).Contains(testStatus1.Id));
		
			var testStatus2 = pageModel.FinancialPlanStatuses.Last();
			Assert.IsTrue(validStatuses.Select(s => s.Description).Contains(testStatus2.Text));
			Assert.IsTrue(validStatuses.Select(s => s.Name).Contains(testStatus2.Id));
			
			Assert.IsNull(pageModel.TempData["Error.Message"]);
		}
		
		[Test]
		public async Task WhenOnGetAsync_WhenFinancialPlanIsClosed_ReturnsRedirectToClosedPage()
		{
		// arrange
		var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockFinancialPlanStatusService = new Mock<IFinancialPlanStatusCachedService>();
			var mockLogger = new Mock<ILogger<ClosePageModel>>();

			var pageModel = SetupClosePageModel(mockFinancialPlanModelService.Object, mockFinancialPlanStatusService.Object, mockLogger.Object);
			var validStatuses = GetListValidStatuses();
			
			var caseUrn = 4;
			var financialPlanId = 6;
			var financialPlan = SetupFinancialPlanModel(financialPlanId, caseUrn, null);
			financialPlan.ClosedAt = DateTime.Now;
							
			mockFinancialPlanModelService.Setup(fp => fp.GetFinancialPlansModelById(caseUrn, financialPlanId))
				.ReturnsAsync(financialPlan);

			mockFinancialPlanStatusService.Setup(fp => fp.GetClosureFinancialPlansStatusesAsync())
				.ReturnsAsync(validStatuses);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseUrn);
			routeData.Add("financialplanid", financialPlanId);

			Assert.IsNull(pageModel.FinancialPlanModel);
			
			// act
			var response = await pageModel.OnGetAsync();

			// assert
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::FinancialPlan::ClosePageModel::OnGetAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
			
			Assert.Multiple(() =>
			{
				Assert.That(response, Is.InstanceOf<RedirectResult>());
				Assert.That(((RedirectResult)response).Url, Is.EqualTo($"/case/{caseUrn}/management/action/financialplan/{financialPlanId}/closed"));
				Assert.That(pageModel.TempData["Error.Message"], Is.Null);
			});
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
				Is.EqualTo(ErrorConstants.ErrorOnPostPage));
			
			mockFinancialPlanModelService.Verify(f => f.PatchFinancialById(It.IsAny<PatchFinancialPlanModel>()), Times.Never);
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
			routeData.Add("financialplanid", 2);

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
				fpm.ClosedAt != null)), Times.Once);
		}
		
		[Test]
		public async Task WhenOnPostAsync_WithEmptyStatus_ReturnsError()
		{
			// arrange
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockFinancialPlanStatusService = new Mock<IFinancialPlanStatusCachedService>();
			var mockLogger = new Mock<ILogger<ClosePageModel>>();
			
			var caseUrn = 1;
			var financialPlanId = 2;
			
			mockFinancialPlanStatusService.Setup(fp => fp.GetClosureFinancialPlansStatusesAsync())
				.ReturnsAsync(GetListValidStatuses());
			
			mockFinancialPlanModelService.Setup(fp => fp.GetFinancialPlansModelById(caseUrn, financialPlanId))
				.ReturnsAsync(SetupFinancialPlanModel(caseUrn, financialPlanId, null));

			var pageModel = SetupClosePageModel(mockFinancialPlanModelService.Object, mockFinancialPlanStatusService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseUrn);
			routeData.Add("financialplanid", financialPlanId);

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "status", ""}
				});

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["FinancialPlan.Message"], Is.EqualTo("Please select a reason for closing the Financial Plan"));
			
			mockFinancialPlanModelService.Verify(f => f.PatchFinancialById(It.IsAny<PatchFinancialPlanModel>()), Times.Never);
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
		
		private static FinancialPlanModel SetupFinancialPlanModel(long planId, long caseUrn, string statusName = "")
			=> new FinancialPlanModel(planId, 
				caseUrn, 
				DateTime.Now, 
				null, 
				null, 
				String.Empty, 
				new FinancialPlanStatusModel(statusName, 1, false), 
				null);
	} 
}