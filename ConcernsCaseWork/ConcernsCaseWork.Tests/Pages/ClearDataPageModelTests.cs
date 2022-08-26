﻿using ConcernsCaseWork.Pages;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Base;
using Service.Redis.NtiUnderConsideration;
using Service.Redis.NtiWarningLetter;
using Service.Redis.Ratings;
using Service.Redis.Status;
using Service.Redis.Teams;
using Service.Redis.Trusts;
using Service.Redis.Types;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages
{
	[Parallelizable(ParallelScope.All)]
	public class ClearDataPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_ReturnsHomePage_ClearCache()
		{
			// arrange
			const string ExpectedUserIdentity = "Tester";
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockCachedService = new Mock<ICachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockNTIUnderConsiderationStatusesCachedService = new Mock<INtiUnderConsiderationStatusesCachedService>();
			var mockNTIUnderConsiderationReasonsCachedService = new Mock<INtiUnderConsiderationReasonsCachedService>();
			var mockNTIWarningLetterReasonCachedService = new Mock<INtiWarningLetterReasonsCachedService>();
			var mockNTIWarningLetterStatusesCachedService = new Mock<INtiWarningLetterStatusesCachedService>();
			var mockTeamsCachedService = new Mock<ITeamsCachedService>();
			var mockLogger = new Mock<ILogger<ClearDataPageModel>>();
			
			var pageModel = SetupClearDataModel(
				mockStatusCachedService.Object,
				mockRatingCachedService.Object,
				mockTypeCachedService.Object,
				mockCachedService.Object,
				mockTrustCachedService.Object,
				mockNTIUnderConsiderationStatusesCachedService.Object,
				mockNTIUnderConsiderationReasonsCachedService.Object,
				mockNTIWarningLetterReasonCachedService.Object,
				mockNTIWarningLetterStatusesCachedService.Object,
				mockTeamsCachedService.Object,
				mockLogger.Object,
				true);

			// act
			var response = await pageModel.OnGetAsync();
			
			Assert.IsInstanceOf(typeof(RedirectToPageResult), response);
			var redirectResult = response as RedirectToPageResult;
			
			Assert.That(redirectResult, Is.Not.Null);
			Assert.That(redirectResult.PageName, Is.Not.Null);
			Assert.That(redirectResult.PageName, Is.EqualTo("home"));
			
			mockStatusCachedService.Verify(c => c.ClearData(), Times.Once);
			mockRatingCachedService.Verify(c => c.ClearData(), Times.Once);
			mockTypeCachedService.Verify(c => c.ClearData(), Times.Once);
			mockCachedService.Verify(c => c.ClearData(It.IsAny<string>()), Times.Once);
			mockTrustCachedService.Verify(c => c.ClearData(), Times.Once);
			mockTeamsCachedService.Verify(c => c.ClearData(ExpectedUserIdentity), Times.Once);
		}
		
		[Test]
		public async Task WhenOnGetAsync_ReturnsHomePage_NotAuthenticated()
		{
			// arrange
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockCachedService = new Mock<ICachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockNTIUnderConsiderationStatusesCachedService = new Mock<INtiUnderConsiderationStatusesCachedService>();
			var mockNTIUnderConsiderationReasonsCachedService = new Mock<INtiUnderConsiderationReasonsCachedService>();
			var mockNTIWarningLetterReasonCachedService = new Mock<INtiWarningLetterReasonsCachedService>();
			var mockNTIWarningLetterStatusesCachedService = new Mock<INtiWarningLetterStatusesCachedService>();
			var mockTeamsCachedService = new Mock<ITeamsCachedService>();

			var mockLogger = new Mock<ILogger<ClearDataPageModel>>();
			
			var pageModel = SetupClearDataModel(mockStatusCachedService.Object,
				mockRatingCachedService.Object,
				mockTypeCachedService.Object,
				mockCachedService.Object,
				mockTrustCachedService.Object,
				mockNTIUnderConsiderationStatusesCachedService.Object,
				mockNTIUnderConsiderationReasonsCachedService.Object,
				mockNTIWarningLetterReasonCachedService.Object,
				mockNTIWarningLetterStatusesCachedService.Object,
				mockTeamsCachedService.Object,
				mockLogger.Object);

			// act
			var response = await pageModel.OnGetAsync();
			
			Assert.IsInstanceOf(typeof(RedirectToPageResult), response);
			var redirectResult = response as RedirectToPageResult;
			
			Assert.That(redirectResult, Is.Not.Null);
			Assert.That(redirectResult.PageName, Is.Not.Null);
			Assert.That(redirectResult.PageName, Is.EqualTo("home"));
			
			mockStatusCachedService.Verify(c => c.ClearData(), Times.Never);
			mockRatingCachedService.Verify(c => c.ClearData(), Times.Never);
			mockTypeCachedService.Verify(c => c.ClearData(), Times.Never);
			mockCachedService.Verify(c => c.ClearData(It.IsAny<string>()), Times.Never);
			mockTrustCachedService.Verify(c => c.ClearData(), Times.Never);
			mockTeamsCachedService.Verify(c => c.ClearData(It.IsAny<string>()), Times.Never);
		}
		
		private static ClearDataPageModel SetupClearDataModel(
			IStatusCachedService mockStatusCachedService,
			IRatingCachedService mockRatingCachedService,
			ITypeCachedService mockTypeCachedService,
			ICachedService mockCachedService,
			ITrustCachedService mockTrustCachedService,
			INtiUnderConsiderationStatusesCachedService mockNTIUnderConsiderationStatusesCachedService,
			INtiUnderConsiderationReasonsCachedService mockNTIUnderConsiderationReasonsCachedService,
			INtiWarningLetterReasonsCachedService mockNTIWarningLetterReasonCachedService,
			INtiWarningLetterStatusesCachedService mockNTIWarningLetterStatusesCachedService,
			ITeamsCachedService mockTeamsCachedService,
			ILogger<ClearDataPageModel> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new ClearDataPageModel(
				mockCachedService,
				mockTypeCachedService,
				mockStatusCachedService,
				mockRatingCachedService,
				mockTrustCachedService,
				mockNTIUnderConsiderationStatusesCachedService,
				mockNTIUnderConsiderationReasonsCachedService,
				mockNTIWarningLetterReasonCachedService,
				mockNTIWarningLetterStatusesCachedService,
				mockTeamsCachedService,
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