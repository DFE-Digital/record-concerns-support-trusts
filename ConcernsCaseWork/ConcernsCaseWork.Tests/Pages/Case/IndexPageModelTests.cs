using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Shared.Tests.Factory;
using ConcernsCaseWork.Shared.Tests.MockHelpers;
using ConcernsCaseWork.Shared.Tests.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.TRAMS.Trusts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case
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
			var mockUserStateCasesCachedService = new Mock<ICreateCaseService>();
			
			var pageModel = SetupIndexModel(mockTrustModelService.Object, mockUserStateCasesCachedService.Object, mockLogger.Object, true);
			
			// act
			var response = await pageModel.OnGetTrustsSearchResult(searchQuery);
			
			// assert
			Assert.IsInstanceOf(typeof(JsonResult), response);
			var partialPage = response as JsonResult;
			
			Assert.That(partialPage, Is.Not.Null);
			Assert.That(partialPage.Value, Is.InstanceOf<IList<TrustSearchModel>>());
			
			// Verify ILogger
			mockLogger.VerifyLogInformationWasCalled("OnGetTrustsSearchResult");
			mockLogger.VerifyNoOtherCalls();
		}
		
		[Test]
		public async Task WhenOnGetTrustsSearchResult_ReturnJsonResult()
		{
			// arrange
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockCreateCaseService = new Mock<ICreateCaseService>();
			var trustSummaryModel = TrustFactory.BuildListTrustSummaryModel();

			mockTrustModelService.Setup(t => t.GetTrustsBySearchCriteria(It.IsAny<TrustSearch>())).ReturnsAsync(trustSummaryModel);
			var pageModel = SetupIndexModel(mockTrustModelService.Object, mockCreateCaseService.Object, mockLogger.Object, true);
			
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
			mockLogger.VerifyLogInformationWasCalled("OnGetTrustsSearchResult");
			mockLogger.VerifyNoOtherCalls();
		}
		
		[Test]
		public async Task WhenOnGetTrustsSearchResult_ReturnStatusCodeException()
		{
			// arrange
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockCreateCaseService = new Mock<ICreateCaseService>();

			mockTrustModelService.Setup(t => t.GetTrustsBySearchCriteria(It.IsAny<TrustSearch>())).Throws<Exception>();
			var pageModel = SetupIndexModel(mockTrustModelService.Object, mockCreateCaseService.Object, mockLogger.Object, true);
			
			// act
			var response = await pageModel.OnGetTrustsSearchResult("north");
			
			// assert
			Assert.That(response, Is.TypeOf<ObjectResult>());
			var objectResponse = response as ObjectResult;
			Assert.That(objectResponse?.StatusCode, Is.EqualTo(500));
			
			// Verify ILogger
			mockLogger.VerifyLogInformationWasCalled("OnGetTrustsSearchResult");
			mockLogger.VerifyLogErrorWasCalled("Exception");
		}
		
		[Test]
		public async Task WhenOnGetSelectedTrust_ReturnJsonResultWithRedirectUrl()
		{
			// arrange
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockCreateCaseService = new Mock<ICreateCaseService>();
			
			mockCreateCaseService
				.Setup(c => c.StartCreateNewCaseWizard(It.IsAny<string>()))
				.Verifiable();
			
			mockCreateCaseService
				.Setup(c => c.SetTrustInCreateCaseWizard(It.IsAny<string>(), It.IsAny<string>()))
				.Verifiable();
			
			var pageModel = SetupIndexModel(mockTrustModelService.Object, mockCreateCaseService.Object, mockLogger.Object, true);
			
			// act
			var response = await pageModel.OnGetSelectedTrust("selectedTrust", "trust name");
			
			// assert
			Assert.IsInstanceOf(typeof(JsonResult), response);
			var jsonPageResponse = response as JsonResult;
			
			Assert.That(jsonPageResponse, Is.Not.Null);

			// Verify ILogger
			mockLogger.VerifyLogInformationWasCalled("OnGetSelectedTrust");
			mockLogger.VerifyNoOtherCalls();
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
			var mockCreateCaseService = new Mock<ICreateCaseService>();
			
			mockTrustModelService.Setup(t => t.GetTrustsBySearchCriteria(It.IsAny<TrustSearch>())).Throws<Exception>();
			var pageModel = SetupIndexModel(mockTrustModelService.Object, mockCreateCaseService.Object, mockLogger.Object, true);
			
			// act
			var response = await pageModel.OnGetSelectedTrust(selectedTrust, "");
			
			// assert
			Assert.That(response, Is.TypeOf<ObjectResult>());
			var objectResponse = response as ObjectResult;
			Assert.That(objectResponse?.StatusCode, Is.EqualTo(500));
			
			// Verify ILogger
			mockLogger.VerifyLogInformationWasCalled("OnGetSelectedTrust");
			mockLogger.VerifyLogErrorWasCalled("Selected trust is incorrect");
		}
		
		private static IndexPageModel SetupIndexModel(ITrustModelService mockTrustModelService, ICreateCaseService mockCreateCaseService, ILogger<IndexPageModel> mockLogger, bool isAuthenticated = false)
		{
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			mockClaimsPrincipalHelper.Setup(x => x.GetPrincipalName(It.IsAny<ClaimsPrincipal>())).Returns("Tester");

			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new IndexPageModel(mockTrustModelService, mockLogger, mockClaimsPrincipalHelper.Object, mockCreateCaseService)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}