using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case;
using ConcernsCaseWork.Services.Rating;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Services.Type;
using ConcernsCaseWork.Shared.Tests.Factory;
using ConcernsCaseWork.Shared.Tests.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using Service.Redis.Base;
using Service.Redis.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages
{
	[Parallelizable(ParallelScope.All)]
	public class ConcernTypePageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_ReturnsModel()
		{
			// arrange
			var mockLogger = new Mock<ILogger<ConcernTypePageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockCachedService = new Mock<ICachedService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			
			var expected = TrustFactory.BuildTrustDetailsModel();

			mockTypeModelService.Setup(t => t.GetTypeModel()).ReturnsAsync(new TypeModel());
			mockCachedService.Setup(c => c.GetData<UserState>(It.IsAny<string>())).ReturnsAsync(new UserState { TrustUkPrn = "trust-ukprn" });
			mockTrustModelService.Setup(s => s.GetTrustByUkPrn(It.IsAny<string>())).ReturnsAsync(expected);
			
			var pageModel = SetupConcernTypePageModel(mockTrustModelService.Object, mockCachedService.Object, 
				mockTypeModelService.Object, mockRatingModelService.Object, mockLogger.Object, true);
			
			// act
			await pageModel.OnGetAsync();
			var trustDetailsModel = pageModel.TrustDetailsModel;
			var typesDictionary = pageModel.TypeModel.TypesDictionary;

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.Null);
			Assert.IsAssignableFrom<TrustDetailsModel>(trustDetailsModel);
			Assert.IsAssignableFrom<Dictionary<string, IList<string>>>(typesDictionary);

			Assert.That(typesDictionary, Is.Not.Null);
			Assert.That(trustDetailsModel, Is.Not.Null);
			Assert.That(trustDetailsModel.GiasData, Is.Not.Null);
			Assert.That(trustDetailsModel.GiasData.GroupId, Is.EqualTo(expected.GiasData.GroupId));
			Assert.That(trustDetailsModel.GiasData.GroupName, Is.EqualTo(expected.GiasData.GroupName));
			Assert.That(trustDetailsModel.GiasData.UkPrn, Is.EqualTo(expected.GiasData.UkPrn));
			Assert.That(trustDetailsModel.GiasData.CompaniesHouseNumber, Is.EqualTo(expected.GiasData.CompaniesHouseNumber));
			Assert.That(trustDetailsModel.GiasData.GroupContactAddress, Is.Not.Null);
			Assert.That(trustDetailsModel.GiasData.GroupContactAddress.County, Is.EqualTo(expected.GiasData.GroupContactAddress.County));
			Assert.That(trustDetailsModel.GiasData.GroupContactAddress.Locality, Is.EqualTo(expected.GiasData.GroupContactAddress.Locality));
			Assert.That(trustDetailsModel.GiasData.GroupContactAddress.Postcode, Is.EqualTo(expected.GiasData.GroupContactAddress.Postcode));
			Assert.That(trustDetailsModel.GiasData.GroupContactAddress.Street, Is.EqualTo(expected.GiasData.GroupContactAddress.Street));
			Assert.That(trustDetailsModel.GiasData.GroupContactAddress.Town, Is.EqualTo(expected.GiasData.GroupContactAddress.Town));
			Assert.That(trustDetailsModel.GiasData.GroupContactAddress.AdditionalLine, Is.EqualTo(expected.GiasData.GroupContactAddress.AdditionalLine));
			Assert.That(trustDetailsModel.GiasData.GroupContactAddress.DisplayAddress, Is.EqualTo(SharedBuilder.BuildDisplayAddress(expected.GiasData.GroupContactAddress)));
			
			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("ConcernTypePageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}
		
		[Test]
		public async Task WhenOnGetAsync_MissingCacheUserState_ThrowsException()
		{
			// arrange
			var mockLogger = new Mock<ILogger<ConcernTypePageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockCachedService = new Mock<ICachedService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			
			var expected = TrustFactory.BuildTrustDetailsModel();

			mockTypeModelService.Setup(t => t.GetTypeModel()).ReturnsAsync(new TypeModel());
			mockCachedService.Setup(c => c.GetData<UserState>(It.IsAny<string>())).ReturnsAsync((UserState)null);
			mockTrustModelService.Setup(s => s.GetTrustByUkPrn(It.IsAny<string>())).ReturnsAsync(expected);
			
			var pageModel = SetupConcernTypePageModel(mockTrustModelService.Object, mockCachedService.Object, 
				mockTypeModelService.Object, mockRatingModelService.Object, mockLogger.Object, true);
			
			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
			Assert.That(pageModel.TypeModel, Is.Null);

			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("ConcernTypePageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}

		[Test]
		public async Task WhenOnPostAsync_UserStateIsNull_ThrowsException()
		{
			// arrange
			var mockLogger = new Mock<ILogger<ConcernTypePageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockCachedService = new Mock<ICachedService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			
			mockCachedService.Setup(c => c.GetData<UserState>(It.IsAny<string>()))
				.ReturnsAsync((UserState)null);

			var pageModel = SetupConcernTypePageModel(mockTrustModelService.Object, mockCachedService.Object, 
				mockTypeModelService.Object, mockRatingModelService.Object, mockLogger.Object, true);
			
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "type", new StringValues("type") },
					{ "subType", new StringValues("subType") },
					{ "ragRating", new StringValues("ragRating:123") },
					{ "trustUkprn", new StringValues("trustUkprn") },
					{ "trustName", new StringValues("trustName") }
				});
			
			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;
			
			Assert.That(page, Is.Not.Null);
			
			mockCachedService.Verify(c => c.GetData<UserState>(It.IsAny<string>()), Times.Once);
			mockCachedService.Verify(c => c.StoreData(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<int>()), Times.Never);
		}
		
		[Test]
		public async Task WhenOnPostAsync_ReturnDetailsPage()
		{
			// arrange
			var mockLogger = new Mock<ILogger<ConcernTypePageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockCachedService = new Mock<ICachedService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			
			var expected = CaseFactory.BuildCreateCaseModel();
			var userState = new UserState { TrustUkPrn = "trust-ukprn", CreateCaseModel = expected };

			mockCachedService.Setup(c => c.GetData<UserState>(It.IsAny<string>())).ReturnsAsync(userState);

			var pageModel = SetupConcernTypePageModel(mockTrustModelService.Object, mockCachedService.Object, 
				mockTypeModelService.Object, mockRatingModelService.Object, mockLogger.Object, true);
			
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "type", new StringValues("Force Majeure") },
					{ "subType", new StringValues("subType") },
					{ "ragRating", new StringValues("123:ragRating") },
					{ "trustUkprn", new StringValues("trustUkprn") },
					{ "trustName", new StringValues("trustName") }
				});
			
			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectToPageResult>());
			var page = pageResponse as RedirectToPageResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(page.PageName, Is.EqualTo("details"));
			
			mockCachedService.Verify(c => c.GetData<UserState>(It.IsAny<string>()), Times.Once);
			mockCachedService.Verify(c => c.StoreData(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<int>()), Times.Once);
		}
		
		[Test]
		public async Task WhenOnPostAsync_MissingFormData_ThrowsException()
		{
			// arrange
			var mockLogger = new Mock<ILogger<ConcernTypePageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockCachedService = new Mock<ICachedService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			
			var expected = CaseFactory.BuildCreateCaseModel();
			var userState = new UserState { TrustUkPrn = "trust-ukprn", CreateCaseModel = expected };

			mockCachedService.Setup(c => c.GetData<UserState>(It.IsAny<string>())).ReturnsAsync(userState);

			var pageModel = SetupConcernTypePageModel(mockTrustModelService.Object, mockCachedService.Object, 
				mockTypeModelService.Object, mockRatingModelService.Object, mockLogger.Object, true);
			
			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;
			
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
			Assert.That(page, Is.Not.Null);
			
			mockCachedService.Verify(c => c.GetData<UserState>(It.IsAny<string>()), Times.Never);
			mockCachedService.Verify(c => c.StoreData(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<int>()), Times.Never);
		}
		
		[TestCase("", "", "", "")]
		[TestCase(null, null, null, null)]
		[TestCase("test", "", "ragRating", "trustUkprn")]
		public async Task WhenOnPostAsync_InvalidFormData_ThrowsException(string type, string subType, string ragRating, string trustUkprn)
		{
			// arrange
			var mockLogger = new Mock<ILogger<ConcernTypePageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockCachedService = new Mock<ICachedService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			
			var expected = CaseFactory.BuildCreateCaseModel(type, subType);
			var userState = new UserState { TrustUkPrn = "trust-ukprn", CreateCaseModel = expected };

			mockCachedService.Setup(c => c.GetData<UserState>(It.IsAny<string>())).ReturnsAsync(userState);

			var pageModel = SetupConcernTypePageModel(mockTrustModelService.Object, mockCachedService.Object, 
				mockTypeModelService.Object, mockRatingModelService.Object, mockLogger.Object, true);
			
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "type", new StringValues(type) },
					{ "subType", new StringValues(subType) },
					{ "ragRating", new StringValues(ragRating) },
					{ "trustUkprn", new StringValues(trustUkprn) },
					{ "trustName", new StringValues() }
				});
			
			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;
			
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
			Assert.That(page, Is.Not.Null);
			
			mockCachedService.Verify(c => c.GetData<UserState>(It.IsAny<string>()), Times.Never);
			mockCachedService.Verify(c => c.StoreData(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<int>()), Times.Never);
		}
		
		private static ConcernTypePageModel SetupConcernTypePageModel(
			ITrustModelService mockTrustModelService, ICachedService mockCachedService, ITypeModelService mockTypeModelService, 
			IRatingModelService mockRatingModelService, ILogger<ConcernTypePageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new ConcernTypePageModel(mockTrustModelService, mockCachedService, mockTypeModelService, mockRatingModelService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}