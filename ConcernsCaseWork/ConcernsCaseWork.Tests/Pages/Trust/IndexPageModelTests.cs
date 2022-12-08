using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Trust;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Service.Trusts;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Trust
{
	[Parallelizable(ParallelScope.All)]
	public class IndexPageModelTests
	{
		[TestCase("")]
		[TestCase("a")]
		[TestCase("as")]
		public async Task WhenOnGetTrustsSearchResult_ReturnEmptyPartialPage(string searchQuery)
		{
			// arrange
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();

			var pageModel = SetupIndexModel(mockTrustModelService.Object, mockUserStateCachedService.Object, mockLogger.Object, true);

			// act
			var response = await pageModel.OnGetTrustsSearchResult(searchQuery);

			// assert
			Assert.IsInstanceOf(typeof(JsonResult), response);
			var partialPage = response as JsonResult;

			Assert.That(partialPage, Is.Not.Null);
			Assert.That(partialPage.Value, Is.InstanceOf<IList<TrustSearchModel>>());

			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("IndexPageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}

		[Test]
		public async Task WhenOnGetTrustsSearchResult_ReturnJsonResult()
		{
			// arrange
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();
			var trustSummaryModel = TrustFactory.BuildListTrustSummaryModel();

			mockTrustModelService.Setup(t => t.GetTrustsBySearchCriteria(It.IsAny<TrustSearch>())).ReturnsAsync(trustSummaryModel);
			var pageModel = SetupIndexModel(mockTrustModelService.Object, mockUserStateCachedService.Object, mockLogger.Object, true);

			// act
			var response = await pageModel.OnGetTrustsSearchResult("north");

			// assert
			Assert.IsInstanceOf(typeof(JsonResult), response);
			var partialPage = response as JsonResult;

			Assert.That(partialPage, Is.Not.Null);
			Assert.That(partialPage.Value, Is.InstanceOf<IList<TrustSearchModel>>());

			foreach (var expected in (IList<TrustSearchModel>)partialPage.Value)
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
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("IndexPageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}

		[Test]
		public async Task WhenOnGetTrustsSearchResult_ReturnStatusCodeException()
		{
			// arrange
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();

			mockTrustModelService.Setup(t => t.GetTrustsBySearchCriteria(It.IsAny<TrustSearch>())).Throws<Exception>();
			var pageModel = SetupIndexModel(mockTrustModelService.Object, mockUserStateCachedService.Object, mockLogger.Object, true);

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
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("IndexPageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);

			mockLogger.Verify(
				m => m.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("IndexPageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}

		[Test]
		public async Task WhenOnGetSelectedTrust_ReturnJsonResultWithRedirectUrl()
		{
			// arrange
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockUserStateService = new Mock<IUserStateCachedService>();

			var userState = new UserState("testing");

			mockUserStateService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync(userState);
			mockUserStateService.Setup(c => c.StoreData(It.IsAny<string>(), It.IsAny<UserState>()))
				.Returns(Task.FromResult(true));

			var pageModel = SetupIndexModel(mockTrustModelService.Object, mockUserStateService.Object, mockLogger.Object, true);

			// act
			pageModel.SelectedTrustUkprn = "12345";
			var response = await pageModel.OnPost();

			// assert
			Assert.IsInstanceOf(typeof(ObjectResult), response);
			var jsonPageResponse = response as ObjectResult;

			Assert.That(jsonPageResponse, Is.Not.Null);

			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("IndexPageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}

		[TestCase("")]
		[TestCase(null)]
		[TestCase("a")]
		[TestCase("as")]
		public async Task WhenOnGetSelectedTrust_ReturnStatusCodeException(string selectedTrust)
		{
			// arrange
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();

			mockTrustModelService.Setup(t => t.GetTrustsBySearchCriteria(It.IsAny<TrustSearch>())).Throws<Exception>();
			var pageModel = SetupIndexModel(mockTrustModelService.Object, mockUserStateCachedService.Object, mockLogger.Object, true);

			// act
			pageModel.SelectedTrustUkprn = "12345";
			var response = await pageModel.OnPost();

			// assert
			Assert.That(response, Is.TypeOf<ObjectResult>());
			var objectResponse = response as ObjectResult;
			Assert.That(objectResponse?.StatusCode, Is.EqualTo(500));

			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("IndexPageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);

			mockLogger.Verify(
				m => m.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("IndexPageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}

		private static IndexPageModel SetupIndexModel(ITrustModelService mockTrustModelService, IUserStateCachedService mockUSerStateCachedService, ILogger<IndexPageModel> mockLogger, bool isAuthenticated = false)
		{
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			mockClaimsPrincipalHelper.Setup(x => x.GetPrincipalName(It.IsAny<ClaimsPrincipal>())).Returns("Tester");

			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new IndexPageModel(mockTrustModelService, mockUSerStateCachedService, mockLogger, mockClaimsPrincipalHelper.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}