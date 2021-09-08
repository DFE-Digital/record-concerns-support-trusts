using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Shared.Tests.Factory;
using ConcernsCaseWork.Shared.Tests.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Services;
using Service.TRAMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages
{
	[Parallelizable(ParallelScope.All)]
	public class IndexModelTests
	{
		[TestCase("")]
		[TestCase("a")]
		[TestCase("as")]
		public async Task WhenOnGetTrustsSearchResult_ReturnEmptyPartialPage(string searchQuery)
		{
			// arrange
			var mockLogger = new Mock<ILogger<IndexModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockCasesCachedService = new Mock<ICachedService>();
			
			var pageModel = SetupIndexModel(mockTrustModelService.Object, mockCasesCachedService.Object, mockLogger.Object, true);
			
			// act
			var response = await pageModel.OnGetTrustsSearchResult(searchQuery);
			
			// assert
			Assert.IsInstanceOf(typeof(PartialViewResult), response);
			var partialPage = response as PartialViewResult;
			
			Assert.That(partialPage, Is.Not.Null);
			Assert.That(partialPage.Model, Is.InstanceOf<IList<TrustSummaryModel>>());
			
			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("IndexModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}
		
		[Test]
		public async Task WhenOnGetTrustsSearchResult_ReturnResultsToPartialPage()
		{
			// arrange
			var mockLogger = new Mock<ILogger<IndexModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockCasesCachedService = new Mock<ICachedService>();
			var trustSummaryModel = TrustFactory.CreateListTrustSummaryModel();

			mockTrustModelService.Setup(t => t.GetTrustsBySearchCriteria(It.IsAny<TrustSearch>())).ReturnsAsync(trustSummaryModel);
			var pageModel = SetupIndexModel(mockTrustModelService.Object, mockCasesCachedService.Object, mockLogger.Object, true);
			
			// act
			var response = await pageModel.OnGetTrustsSearchResult("north");
			
			// assert
			Assert.IsInstanceOf(typeof(PartialViewResult), response);
			var partialPage = response as PartialViewResult;
			
			Assert.That(partialPage, Is.Not.Null);
			Assert.That(partialPage.Model, Is.InstanceOf<IList<TrustSummaryModel>>());
			
			foreach (var expected in (IList<TrustSummaryModel>)partialPage.Model)
			{
				foreach (var actual in trustSummaryModel.Where(actual => expected.UkPrn.Equals(actual.UkPrn)))
				{
					Assert.That(expected.Establishments.Count, Is.EqualTo(actual.Establishments.Count));
					Assert.That(expected.Urn, Is.EqualTo(actual.Urn));
					Assert.That(expected.UkPrn, Is.EqualTo(actual.UkPrn));
					Assert.That(expected.GroupName, Is.EqualTo(actual.GroupName));
					Assert.That(expected.CompaniesHouseNumber, Is.EqualTo(actual.CompaniesHouseNumber));
					Assert.That(expected.DisplayName, Is.EqualTo(SharedBuilder.BuildDisplayName(actual)));
					
					foreach (var establishment in actual.Establishments)
					{
						foreach (var expectedEstablishment in expected.Establishments)
						{
							Assert.That(establishment.Name, Is.EqualTo(expectedEstablishment.Name));
							Assert.That(establishment.Urn, Is.EqualTo(expectedEstablishment.Urn));
							Assert.That(establishment.UkPrn, Is.EqualTo(expectedEstablishment.UkPrn));
						}
					}
				}
			}
			
			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("IndexModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}
		
		[Test]
		public async Task WhenOnGetTrustsSearchResult_ReturnStatusCodeException()
		{
			// arrange
			var mockLogger = new Mock<ILogger<IndexModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockCasesCachedService = new Mock<ICachedService>();

			mockTrustModelService.Setup(t => t.GetTrustsBySearchCriteria(It.IsAny<TrustSearch>())).Throws<Exception>();
			var pageModel = SetupIndexModel(mockTrustModelService.Object, mockCasesCachedService.Object, mockLogger.Object, true);
			
			// act
			var response = await pageModel.OnGetTrustsSearchResult("north");
			
			// assert
			Assert.That(response, Is.TypeOf<ObjectResult>());
			var objectResponse = response as ObjectResult;
			Assert.That(objectResponse?.StatusCode, Is.EqualTo(500));
			
			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("IndexModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
			
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("IndexModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}
		
		[Test]
		public void WhenOnGetSelectedTrust_ReturnJsonResultWithRedirectUrl()
		{
			// arrange
			var mockLogger = new Mock<ILogger<IndexModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockCasesCachedService = new Mock<ICachedService>();
			
			var pageModel = SetupIndexModel(mockTrustModelService.Object, mockCasesCachedService.Object, mockLogger.Object, true);
			
			// act
			var response = pageModel.OnGetSelectedTrust("selectedTrust");
			
			// assert
			Assert.IsInstanceOf(typeof(ObjectResult), response);
			var jsonPageResponse = response as ObjectResult;
			
			Assert.That(jsonPageResponse, Is.Not.Null);

			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("IndexModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}
		
		[TestCase("")]
		[TestCase(null)]
		[TestCase("a")]
		[TestCase("as")]
		public void WhenOnGetSelectedTrust_ReturnStatusCodeException(string selectedTrust)
		{
			// arrange
			var mockLogger = new Mock<ILogger<IndexModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockCasesCachedService = new Mock<ICachedService>();

			mockTrustModelService.Setup(t => t.GetTrustsBySearchCriteria(It.IsAny<TrustSearch>())).Throws<Exception>();
			var pageModel = SetupIndexModel(mockTrustModelService.Object, mockCasesCachedService.Object, mockLogger.Object, true);
			
			// act
			var response = pageModel.OnGetSelectedTrust(selectedTrust);
			
			// assert
			Assert.That(response, Is.TypeOf<ObjectResult>());
			var objectResponse = response as ObjectResult;
			Assert.That(objectResponse?.StatusCode, Is.EqualTo(500));
			
			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("IndexModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
			
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("IndexModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}
		
		private static IndexModel SetupIndexModel(ITrustModelService mockTrustModelService, ICachedService mockCachedService, ILogger<IndexModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new IndexModel(mockTrustModelService, mockCachedService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}