using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Case.Management.Action.FinancialPlan;
using ConcernsCaseWork.Services.FinancialPlan;
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

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.FinancialPlan;

[Parallelizable(ParallelScope.All)]
public class ClosedPageTests
{
	[Test]
	public async Task WhenOnGetAsync_ReturnsPageModel()
	{
		// arrange
		var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
		var mockLogger = new Mock<ILogger<ClosedPageModel>>();

		var caseUrn = 4;
		var financialPlanId = 6;
		var financialPlan = SetupFinancialPlanModel(financialPlanId, caseUrn, null);
		financialPlan.ClosedAt = DateTime.Now;

		mockFinancialPlanModelService.Setup(fp => fp.GetFinancialPlansModelById(caseUrn, financialPlanId, It.IsAny<string>()))
			.ReturnsAsync(financialPlan);

		var pageModel = SetupIndexPageModel(mockFinancialPlanModelService.Object, mockLogger.Object);

		var routeData = pageModel.RouteData.Values;
		routeData.Add("urn", caseUrn);
		routeData.Add("financialplanid", financialPlanId);

		// act
		var response = await pageModel.OnGetAsync();

		// assert
		mockLogger.Verify(
			m => m.Log(
				LogLevel.Information,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::FinancialPlan::ClosedPageModel::OnGetAsync")),
				null,
				It.IsAny<Func<It.IsAnyType, Exception, string>>()),
			Times.Once);

		Assert.Multiple(() =>
		{
			Assert.That(response, Is.InstanceOf<PageResult>());
			Assert.That(pageModel.FinancialPlanModel, Is.Not.Null);
			Assert.That(financialPlanId, Is.EqualTo(pageModel.FinancialPlanModel.Id));
			Assert.That(caseUrn, Is.EqualTo(pageModel.FinancialPlanModel.CaseUrn));
			Assert.That(pageModel.TempData["Error.Message"], Is.Null);
		});
	}

	[Test]
	public async Task WhenOnGetAsync_WhenFinancialPlanIsOpen_RedirectsToOpenPage()
	{
		// arrange
		var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
		var mockLogger = new Mock<ILogger<ClosedPageModel>>();

		var caseUrn = 4;
		var financialPlanId = 6;
		var financialPlan = SetupFinancialPlanModel(financialPlanId, caseUrn, null);

		mockFinancialPlanModelService.Setup(fp => fp.GetFinancialPlansModelById(caseUrn, financialPlanId, It.IsAny<string>()))
			.ReturnsAsync(financialPlan);

		var pageModel = SetupIndexPageModel(mockFinancialPlanModelService.Object, mockLogger.Object);

		var routeData = pageModel.RouteData.Values;
		routeData.Add("urn", caseUrn);
		routeData.Add("financialplanid", financialPlanId);

		// act
		var response = await pageModel.OnGetAsync();

		// assert
		mockLogger.Verify(
			m => m.Log(
				LogLevel.Information,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::FinancialPlan::ClosedPageModel::OnGetAsync")),
				null,
				It.IsAny<Func<It.IsAnyType, Exception, string>>()),
			Times.Once);

		Assert.Multiple(() =>
		{
			Assert.That(response, Is.InstanceOf<RedirectResult>());
			Assert.That(((RedirectResult)response).Url, Is.EqualTo($"/case/{caseUrn}/management/action/financialplan/{financialPlanId}"));
			Assert.That(pageModel.TempData["Error.Message"], Is.Null);
		});
	}

	private static FinancialPlanModel SetupFinancialPlanModel(long planId, long caseUrn, string statusName = "")
		=> new FinancialPlanModel(planId,
			caseUrn,
			DateTime.Now,
			null,
			null,
			String.Empty,
			new FinancialPlanStatusModel(statusName, 1, false),
			null);

	private static ClosedPageModel SetupIndexPageModel(
		IFinancialPlanModelService mockFinancialPlanModelService,
		ILogger<ClosedPageModel> mockLogger,
		bool isAuthenticated = false)
	{
		(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

		return new ClosedPageModel(mockFinancialPlanModelService, mockLogger)
		{
			PageContext = pageContext, TempData = tempData, Url = new UrlHelper(actionContext), MetadataProvider = pageContext.ViewData.ModelMetadata
		};
	}
}