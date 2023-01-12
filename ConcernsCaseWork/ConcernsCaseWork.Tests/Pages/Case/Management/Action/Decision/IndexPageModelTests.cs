using AutoFixture;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Pages.Case.Management.Action.Decision;
using ConcernsCaseWork.Service.Decision;
using ConcernsCaseWork.Service.Permissions;
using ConcernsCaseWork.Shared.Tests.Factory;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.Decision
{
	[Parallelizable(ParallelScope.Fixtures)]
	public class IndexPageModelTests
	{
		private Mock<IDecisionService> _mockDecision;
		private readonly static Fixture _fixture = new();

		[SetUp]
		public void SetUp()
		{
			_mockDecision = new Mock<IDecisionService>();
		}

		[Test]
		public async Task OnGetAsync_Returns_Page()
		{
			var apiDecision = _fixture.Create<GetDecisionResponse>();
			apiDecision.ConcernsCaseUrn = 1;
			_mockDecision.Setup(m => m.GetDecision(1, 2)).ReturnsAsync(apiDecision);

			var pageModel = CreateCasePageModel();
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("decisionId", 2);

			await pageModel.OnGetAsync();

			pageModel.TempData[ErrorConstants.ErrorMessageKey].Should().BeNull();

			pageModel.Decision.ConcernsCaseUrn.Should().Be(apiDecision.ConcernsCaseUrn);
			pageModel.Decision.DecisionId.Should().Be(apiDecision.DecisionId);
			
			pageModel.ViewData[ViewDataConstants.Title].Should().Be("Decision");
		}

		[Test]
		public async Task OnGetAsync_WhenGetDecisionFails_ReturnsError()
		{
			_mockDecision.Setup(m => m.GetDecision(1, 2)).Throws(new Exception("Failed to retrieve decisions"));

			var pageModel = CreateCasePageModel();

			await pageModel.OnGetAsync();

			var result = pageModel.TempData[ErrorConstants.ErrorMessageKey];

			result.Should().Be(ErrorConstants.ErrorOnGetPage);
		}

		private IndexPageModel CreateCasePageModel()
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(true);

			var logger = new Mock<ILogger<IndexPageModel>>();

			var permissionsService = new Mock<ICasePermissionsService>();
			permissionsService.Setup(m => m.GetCasePermissions(It.IsAny<long>())).ReturnsAsync(_fixture.Create<GetCasePermissionsResponse>());

			return new IndexPageModel(_mockDecision.Object, permissionsService.Object, logger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}
