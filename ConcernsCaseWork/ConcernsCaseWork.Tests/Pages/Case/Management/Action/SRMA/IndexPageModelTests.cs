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
using Service.TRAMS.Cases;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.SRMA
{
	[Parallelizable(ParallelScope.All)]
	public class IndexPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_MissingCaseUrn_ThrowsException_ReturnPage()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var pageModel = SetupIndexPageModel(mockSrmaService.Object, mockLogger.Object);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
		}


		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

			mockSrmaService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupIndexPageModel(mockSrmaService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("srmaId", 1);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.IsNotNull(pageModel.SRMAModel);

			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::SRMA::IndexPageModel::OnGetAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}

		private static IndexPageModel SetupIndexPageModel(
			ISRMAService mockSrmaService,
			ILogger<IndexPageModel> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new IndexPageModel(mockSrmaService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}


}