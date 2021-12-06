using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages;
using ConcernsCaseWork.Security;
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
	public class HomePageModelTests
	{
		[Test]
		public async Task WhenInstanceOfIndexPageOnGetAsync_ReturnCases()
		{
			// arrange
			var homeModels = HomePageFactory.BuildHomeModels();

			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockRbacManager = new Mock<IRbacManager>();
			var mockLogger = new Mock<ILogger<HomePageModel>>();
			mockCaseModelService.Setup(c => c.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<StatusEnum>()))
				.ReturnsAsync(homeModels);
			
			// act
			var homePageModel = SetupHomeModel(mockCaseModelService.Object, mockRbacManager.Object, mockLogger.Object);
			await homePageModel.OnGetAsync();
			
			// assert
			Assert.IsAssignableFrom<List<HomeModel>>(homePageModel.CasesActive);
			Assert.That(homePageModel.CasesActive.Count, Is.EqualTo(homeModels.Count));
			foreach (var expected in homePageModel.CasesActive)
			{
				foreach (var actual in homeModels.Where(actual => expected.CaseUrn.Equals(actual.CaseUrn)))
				{
					Assert.That(expected.Closed, Is.EqualTo(actual.Closed));
					Assert.That(expected.Created, Is.EqualTo(actual.Created));
					Assert.That(DateTimeOffset.FromUnixTimeMilliseconds(expected.CreatedUnixTime).ToString("dd-MM-yyyy"), Is.EqualTo(actual.Created));
					Assert.That(expected.Updated, Is.EqualTo(actual.Updated));
					Assert.That(DateTimeOffset.FromUnixTimeMilliseconds(expected.UpdatedUnixTime).ToString("dd-MM-yyyy"), Is.EqualTo(actual.Updated));
					Assert.That(expected.Review, Is.EqualTo(actual.Review));
					Assert.That(expected.AcademyNames, Is.EqualTo(actual.AcademyNames));
					Assert.That(expected.CaseType, Is.EqualTo(actual.CaseType));
					Assert.That(expected.CaseSubType, Is.EqualTo(actual.CaseSubType));
					Assert.That(expected.CaseTypeDescription, Is.EqualTo($"{actual.CaseType}: {actual.CaseSubType}"));
					Assert.That(expected.CaseUrn, Is.EqualTo(actual.CaseUrn));
					Assert.That(expected.RagRating, Is.EqualTo(actual.RagRating));
					Assert.That(expected.TrustName, Is.EqualTo(actual.TrustName));
					Assert.That(expected.TrustNameTitle, Is.EqualTo(actual.TrustName.ToTitle()));
					Assert.That(expected.RagRatingCss, Is.EqualTo(actual.RagRatingCss));
				}
			}
			
			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("HomePageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}

		[Test]
		public async Task WhenInstanceOfIndexPageOnGetAsync_ReturnEmptyCases()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockRbacManager = new Mock<IRbacManager>();
			var mockLogger = new Mock<ILogger<HomePageModel>>();
			var emptyList = new List<HomeModel>();
			
			mockCaseModelService.Setup(model => model.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<StatusEnum>()))
				.ReturnsAsync(emptyList);
			
			// act
			var indexModel = SetupHomeModel(mockCaseModelService.Object, mockRbacManager.Object, mockLogger.Object);
			await indexModel.OnGetAsync();
			
			// assert
			Assert.IsAssignableFrom<List<HomeModel>>(indexModel.CasesActive);
			Assert.That(indexModel.CasesActive.Count, Is.Zero);
			
			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("HomePageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}
		
		private static HomePageModel SetupHomeModel(ICaseModelService mockCaseModelService, IRbacManager mockRbacManager, ILogger<HomePageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new HomePageModel(mockCaseModelService,mockRbacManager, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}