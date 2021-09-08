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
			var cases = CaseModelFactory.CreateCaseModels();

			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<HomePageModel>>();
			mockCaseModelService.Setup(model => model.GetCasesByCaseworker(It.IsAny<string>())).ReturnsAsync(cases, cases);
			
			// act
			var indexModel = new HomePageModel(mockCaseModelService.Object, mockLogger.Object);
			await indexModel.OnGetAsync();
			
			// assert
			Assert.IsAssignableFrom<List<CaseModel>>(indexModel.CasesActive);
			Assert.That(indexModel.CasesActive.Count, Is.EqualTo(cases.Count));
			foreach (var expected in indexModel.CasesActive)
			{
				foreach (var actual in cases.Where(actual => expected.Id.Equals(actual.Id)))
				{
					Assert.That(expected.Description, Is.EqualTo(actual.Description));
					Assert.That(expected.Issue, Is.EqualTo(actual.Issue));
					Assert.That(expected.Status, Is.EqualTo(actual.Status));
					Assert.That(expected.Urn, Is.EqualTo(actual.Urn));
					Assert.That(expected.ClosedAt, Is.EqualTo(actual.ClosedAt));
					Assert.That(expected.CreatedAt, Is.EqualTo(actual.CreatedAt));
					Assert.That(expected.CreatedBy, Is.EqualTo(actual.CreatedBy));
					Assert.That(expected.CrmEnquiry, Is.EqualTo(actual.CrmEnquiry));
					Assert.That(expected.CurrentStatus, Is.EqualTo(actual.CurrentStatus));
					Assert.That(expected.DeEscalation, Is.EqualTo(actual.DeEscalation));
					Assert.That(expected.NextSteps, Is.EqualTo(actual.NextSteps));
					Assert.That(expected.ResolutionStrategy, Is.EqualTo(actual.ResolutionStrategy));
					Assert.That(expected.ReviewAt, Is.EqualTo(actual.ReviewAt));
					Assert.That(expected.UpdateAt, Is.EqualTo(actual.UpdateAt));
					Assert.That(expected.DirectionOfTravel, Is.EqualTo(actual.DirectionOfTravel));
					Assert.That(expected.ReasonAtReview, Is.EqualTo(actual.ReasonAtReview));
					Assert.That(expected.TrustUkPrn, Is.EqualTo(actual.TrustUkPrn));
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
			mockCaseModelService.Setup(model => model.GetCasesByCaseworker(It.IsAny<string>())).ReturnsAsync(new List<CaseModel>());
			
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
		
		[Test]
		public void WhenInstanceOfIndexPageCheck_ReturnRags()
		{
			// act
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<HomePageModel>>();
			var indexModel = new HomePageModel(mockCaseModelService.Object, mockLogger.Object);
			
			// assert
			Assert.That(indexModel.Rags.Count, Is.EqualTo(5));
			Assert.That(indexModel.Rags, Is.EqualTo(new Dictionary<int, string>(5)
			{
				{0, "-"}, {1, "Red"}, {2, "Red | Amber"}, {3, "Amber | Green"}, {4, "Red Plus"}
			}));
		}
		
		[Test]
		public void WhenInstanceOfIndexPageCheck_ReturnRagCss()
		{
			// act
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<HomePageModel>>();
			var indexModel = new HomePageModel(mockCaseModelService.Object, mockLogger.Object);
			
			// assert
			Assert.That(indexModel.RagsCss.Count, Is.EqualTo(5));
			Assert.That(indexModel.RagsCss, Is.EqualTo(new Dictionary<int, string>(5)
			{
				{0, ""}, {1, "ragtag__red"}, {2, "ragtag__redamber"}, {3, "ragtag__ambergreen"}, {4, "ragtag__redplus"}
			}));
		}
	}
}