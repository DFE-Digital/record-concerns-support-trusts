using ConcernsCaseWork.API.Contracts.ResponseModels.TrustFinancialForecasts;
using ConcernsCaseWork.Pages.Case.Management.Action.CaseActionCreateHelpers;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Service.TrustFinancialForecast;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.ActionCreateHelpersTests;

[Parallelizable(ParallelScope.All)]
public class TrustFinancialForecastCreateHelperTests
{
	[Test]
	public void TrustFinancialForecastCreateHelper_CanHandle_RespondsCorrectly()
	{
		// arrange
		var mockTrustFinancialForecastService = new Mock<ITrustFinancialForecastService>();
		var sut = new TrustFinancialForecastCreateHelper(mockTrustFinancialForecastService.Object);

		// act
		var expectedTrue = sut.CanHandle(CaseActionEnum.TrustFinancialForecast);
		var expectedFalse = sut.CanHandle(CaseActionEnum.FinancialPlan);

		// assert
		Assert.That(expectedTrue, Is.True);
		Assert.That(expectedFalse, Is.False);
	}

	[Test]
	public void TrustFinancialForecastCreateHelper_NewCaseActionAllowed_ClearToAdd()
	{
		// arrange
		var mockTrustFinancialForecastService = new Mock<ITrustFinancialForecastService>();
		var sut = new TrustFinancialForecastCreateHelper(mockTrustFinancialForecastService.Object);

		var caseActions = new [] {
			new TrustFinancialForecastResponse { TrustFinancialForecastId = 123, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-10) },
			new TrustFinancialForecastResponse { TrustFinancialForecastId = 124, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-8) },
			new TrustFinancialForecastResponse { TrustFinancialForecastId = 125, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-5) },
		}.AsEnumerable();

		mockTrustFinancialForecastService.Setup(svc => svc.GetAllForCase(888)).Returns(Task.FromResult(caseActions));

		// act
		var actual = sut.NewCaseActionAllowed(888).Result;

		// assert
		Assert.That(actual, Is.True);
	}

	[Test]
	public void TrustFinancialForecastCreateHelper_NewCaseActionAllowed_NotClearToAdd()
	{
		// arrange
		var mockTrustFinancialForecastService = new Mock<ITrustFinancialForecastService>();
		var sut = new TrustFinancialForecastCreateHelper(mockTrustFinancialForecastService.Object);

		var caseActions = new [] {
			new TrustFinancialForecastResponse { TrustFinancialForecastId = 123, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-10) },
			new TrustFinancialForecastResponse { TrustFinancialForecastId = 124, CaseUrn = 888 },
			new TrustFinancialForecastResponse { TrustFinancialForecastId = 125, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-5) },
		}.AsEnumerable();

		mockTrustFinancialForecastService.Setup(svc => svc.GetAllForCase(888)).Returns(Task.FromResult(caseActions));

		// act, assert
		Assert.ThrowsAsync<InvalidOperationException>(async Task () => await sut.NewCaseActionAllowed(888));
	}
}