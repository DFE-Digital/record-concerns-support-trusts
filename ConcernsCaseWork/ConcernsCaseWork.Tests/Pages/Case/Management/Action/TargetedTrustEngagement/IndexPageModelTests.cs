﻿using AutoFixture;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.API.Contracts.TargetedTrustEngagement;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Pages.Case.Management.Action.TargetedTrustEngagement;
using ConcernsCaseWork.Service.Permissions;
using ConcernsCaseWork.Service.TargetedTrustEngagement;
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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.TargetedTrustEngagement
{
	[Parallelizable(ParallelScope.Fixtures)]
	public class IndexPageModelTests
	{
		private Mock<ITargetedTrustEngagementService> _mockTargetedTrustEngagementService;
		private readonly static Fixture _fixture = new();

		[SetUp]
		public void SetUp()
		{
			_mockTargetedTrustEngagementService = new Mock<ITargetedTrustEngagementService>();
		}

		[Test]
		public async Task OnGetAsync_Returns_Page()
		{
			var apiTTe = _fixture.Create<GetTargetedTrustEngagementResponse>();
			apiTTe.CaseUrn = 1;
			apiTTe.ClosedAt = null;
			apiTTe.IsEditable = true;
			_mockTargetedTrustEngagementService.Setup(m => m.GetTargetedTrustEngagement(1, 2)).ReturnsAsync(apiTTe);

			var pageModel = CreateCasePageModel();
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("targetedtrustengagementId", 2);

			await pageModel.OnGetAsync();

			pageModel.TempData[ErrorConstants.ErrorMessageKey].Should().BeNull();

			pageModel.TargetedTrustEngagement.Id.Should().Be(apiTTe.Id.ToString());
			pageModel.TargetedTrustEngagement.IsEditable.Should().BeTrue();
			
			pageModel.ViewData[ViewDataConstants.Title].Should().Be("Targeted Trust Engagement");
		}

		[Test]
		public async Task OnGetAsync_WhenGetDecisionFails_ReturnsError()
		{
			_mockTargetedTrustEngagementService.Setup(m => m.GetTargetedTrustEngagement(1, 2)).Throws(new Exception("Failed to retrieve TTE"));

			var pageModel = CreateCasePageModel();

			await pageModel.OnGetAsync();

			var result = pageModel.TempData[ErrorConstants.ErrorMessageKey];

			result.Should().Be(ErrorConstants.ErrorOnGetPage);
		}

		private IndexPageModel CreateCasePageModel()
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(true);

			var logger = new Mock<ILogger<IndexPageModel>>();

			var casePermissions = new GetCasePermissionsResponse() { Permissions = new List<CasePermission>() { CasePermission.Edit } };
			var permissionsService = new Mock<ICasePermissionsService>();
			permissionsService.Setup(m => m.GetCasePermissions(It.IsAny<long>())).ReturnsAsync(casePermissions);

			return new IndexPageModel(_mockTargetedTrustEngagementService.Object, permissionsService.Object, logger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}
