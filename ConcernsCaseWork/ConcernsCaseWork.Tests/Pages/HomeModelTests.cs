using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages
{
	[Parallelizable(ParallelScope.All)]
	public class HomeModelTests
	{
		[Test]
		public async Task WhenInstanceOfIndexPageOnGetAsync_ReturnCases()
		{
			// arrange
			var homeModels = HomeModelFactory.BuildHomeModels();

			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<HomePageModel>>();
			mockCaseModelService.Setup(model => model.GetCasesByCaseworker(It.IsAny<string>())).ReturnsAsync((homeModels, homeModels));
			
			// act
			var homePageModel = new HomePageModel(mockCaseModelService.Object, mockLogger.Object);
			await homePageModel.OnGetAsync();
			
			// assert
			Assert.IsAssignableFrom<List<CaseModel>>(homePageModel.CasesActive);
			Assert.That(homePageModel.CasesActive.Count, Is.EqualTo(homeModels.Count));
			foreach (var expected in homePageModel.CasesActive)
			{
				foreach (var actual in homeModels.Where(actual => expected.CaseUrn.Equals(actual.CaseUrn)))
				{
					Assert.That(expected.Closed, Is.EqualTo(actual.Closed));
					Assert.That(expected.Created, Is.EqualTo(actual.Created));
					Assert.That(expected.Updated, Is.EqualTo(actual.Updated));
					Assert.That(expected.AcademyNames, Is.EqualTo(actual.AcademyNames));
					Assert.That(expected.CaseType, Is.EqualTo(actual.CaseType));
					Assert.That(expected.CaseUrn, Is.EqualTo(actual.CaseUrn));
					Assert.That(expected.RagRating, Is.EqualTo(actual.RagRating));
					Assert.That(expected.TrustName, Is.EqualTo(actual.TrustName));
					Assert.That(expected.CaseSubType, Is.EqualTo(actual.CaseSubType));
					Assert.That(expected.RagRatingCss, Is.EqualTo(actual.RagRatingCss));
				}
			}
			
			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("HomeModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}

		[Test]
		public async Task WhenInstanceOfIndexPageOnGetAsync_ReturnEmptyCases()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<HomePageModel>>();
			var emptyList = new List<HomeModel>();
			
			mockCaseModelService.Setup(model => model.GetCasesByCaseworker(It.IsAny<string>())).ReturnsAsync((emptyList, emptyList));
			
			// act
			var indexModel = new HomePageModel(mockCaseModelService.Object, mockLogger.Object);
			await indexModel.OnGetAsync();
			
			// assert
			Assert.IsAssignableFrom<List<CaseModel>>(indexModel.CasesActive);
			Assert.That(indexModel.CasesActive.Count, Is.Zero);
			
			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("HomeModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}
	}
}