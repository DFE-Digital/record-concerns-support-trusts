using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.TRAMS.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages
{
	[Parallelizable(ParallelScope.All)]
	public class ClosedPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_Returns_ClosedCases()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<ClosedPageModel>>();

			var homeModels = HomePageFactory.BuildHomeModels();
			
			mockCaseModelService.Setup(c => c.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<StatusEnum>()))
				.ReturnsAsync(homeModels);
			
			var pageModel = SetupClosedPageModel(mockCaseModelService.Object, mockLogger.Object, true);
			
			// act
			await pageModel.OnGetAsync();
			var homeModel = pageModel.CasesClosed;
			
			// assert
			Assert.That(homeModel, Is.Not.Null);
			Assert.IsAssignableFrom<List<HomeModel>>(homeModel);
			Assert.That(homeModel.Count, Is.EqualTo(homeModels.Count));
			foreach (var expected in homeModel)
			{
				foreach (var actual in homeModels.Where(actual => expected.CaseUrn.Equals(actual.CaseUrn)))
				{
					Assert.That(expected.Closed, Is.EqualTo(actual.Closed));
					Assert.That(expected.Created, Is.EqualTo(actual.Created));
					Assert.That(expected.Updated, Is.EqualTo(actual.Updated));
					Assert.That(expected.Review, Is.EqualTo(actual.Review));
					Assert.That(expected.AcademyNames, Is.EqualTo(actual.AcademyNames));
					Assert.That(expected.CaseType, Is.EqualTo(actual.CaseType));
					Assert.That(expected.CaseSubType, Is.EqualTo(actual.CaseSubType));
					Assert.That(expected.CaseTypeDescription, Is.EqualTo($"{actual.CaseType}: {actual.CaseSubType}"));
					Assert.That(expected.CaseUrn, Is.EqualTo(actual.CaseUrn));
					Assert.That(expected.RagRating, Is.EqualTo(actual.RagRating));
					Assert.That(expected.TrustName, Is.EqualTo(actual.TrustName));
					Assert.That(expected.RagRatingCss, Is.EqualTo(actual.RagRatingCss));
				}
			}
			
			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("ClosedPageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}

		[Test]
		public async Task WhenOnGetAsync_ThrowsException()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<ClosedPageModel>>();
			
			mockCaseModelService.Setup(c => c.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<StatusEnum>()))
				.Throws<Exception>();

			var pageModel = SetupClosedPageModel(mockCaseModelService.Object, mockLogger.Object, true);

			// act
			await pageModel.OnGetAsync();
			var homeModel = pageModel.CasesClosed;

			// assert
			Assert.That(homeModel, Is.Not.Null);
			Assert.IsAssignableFrom<HomeModel[]>(homeModel);
			Assert.That(homeModel.Count, Is.EqualTo(0));
		}

		private static ClosedPageModel SetupClosedPageModel(
			ICaseModelService mockCaseModelService, ILogger<ClosedPageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new ClosedPageModel(mockCaseModelService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}