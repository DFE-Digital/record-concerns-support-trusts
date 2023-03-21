using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Shared.Tests.Factory;
using ConcernsCaseWork.Shared.Tests.MockHelpers;
using ConcernsCaseWork.Tests.Helpers;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.W3C;
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case
{
	[Parallelizable(ParallelScope.All)]
	public class ClosedPageModelTests
	{
		
		private static ConcurrentQueue<ITelemetry> TelemetryItems { get; } = new ConcurrentQueue<ITelemetry>();
		
		[Test]
		public async Task WhenOnGetAsync_Returns_ClosedCases()
		{
			// arrange
			var mockCaseSummaryService = new Mock<ICaseSummaryService>();
			var mockLogger = new Mock<ILogger<ClosedPageModel>>();

			var caseSummaryModels = CaseSummaryModelFactory.BuildClosedCaseSummaryModels();

			mockCaseSummaryService.Setup(c => c.GetClosedCaseSummariesByCaseworker(It.IsAny<string>()))
				.ReturnsAsync(caseSummaryModels);

			var sut = SetupClosedPageModel(mockCaseSummaryService.Object, mockLogger.Object, true);

			// act
			await sut.OnGetAsync();
			var closedCases = sut.ClosedCases;

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(closedCases, Is.Not.Null);
				Assert.IsAssignableFrom<List<ClosedCaseSummaryModel>>(closedCases);
				Assert.That(closedCases.Count, Is.EqualTo(caseSummaryModels.Count));

				foreach (var expected in closedCases)
				{
					foreach (var actual in caseSummaryModels.Where(actual => expected.CaseUrn.Equals(actual.CaseUrn)))
					{
						Assert.That(expected.ClosedAt, Is.EqualTo(actual.ClosedAt));
						Assert.That(expected.CreatedAt, Is.EqualTo(actual.CreatedAt));
						Assert.That(expected.UpdatedAt, Is.EqualTo(actual.UpdatedAt));
						Assert.That(expected.CreatedBy, Is.EqualTo(actual.CreatedBy));
						Assert.That(expected.CaseUrn, Is.EqualTo(actual.CaseUrn));
						Assert.That(expected.TrustName, Is.EqualTo(actual.TrustName));
					}
				}
			});

			// Verify ILogger
			mockLogger.VerifyLogInformationWasCalled("ClosedPageModel");
		}

		[Test]
		public async Task WhenOnGetAsync_ThrowsException()
		{
			// arrange
			var mockCaseSummaryService = new Mock<ICaseSummaryService>();
			var mockLogger = new Mock<ILogger<ClosedPageModel>>();
			var telemetryClient = CreateMockTelemetryClient();
			mockCaseSummaryService.Setup(c => c.GetClosedCaseSummariesByCaseworker(It.IsAny<string>()))
				.Throws<Exception>();

			var pageModel = SetupClosedPageModel(mockCaseSummaryService.Object, mockLogger.Object ,true);

			// act
			await pageModel.OnGetAsync();
			var closedCases = pageModel.ClosedCases;

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(closedCases, Is.Not.Null);
				Assert.IsAssignableFrom<List<ClosedCaseSummaryModel>>(closedCases);
				Assert.That(closedCases, Is.Empty);
			});
		}

		private static ClosedPageModel SetupClosedPageModel(
			ICaseSummaryService mockCaseSummaryService, ILogger<ClosedPageModel> mockLogger,bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			var telemetryClient = CreateMockTelemetryClient();
			return new ClosedPageModel(mockCaseSummaryService, Mock.Of<IClaimsPrincipalHelper>(),mockLogger,telemetryClient)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
		
		
		private static TelemetryClient CreateMockTelemetryClient()
		{
			var telemetryConfiguration = new TelemetryConfiguration
			{
				ConnectionString = "InstrumentationKey=" + Guid.NewGuid().ToString(),
				TelemetryChannel = new StubTelemetryChannel(TelemetryItems.Enqueue)
			};

			// TODO: Add telemetry initializers and processors if/as necessary.
			return new TelemetryClient(telemetryConfiguration);
		}
	}
}