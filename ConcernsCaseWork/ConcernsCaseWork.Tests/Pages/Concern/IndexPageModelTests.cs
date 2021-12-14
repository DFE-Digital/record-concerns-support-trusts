using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case.Concern;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Services.Types;
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

namespace ConcernsCaseWork.Tests.Pages.Concern
{
	[Parallelizable(ParallelScope.All)]
	public class ConcernPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_ReturnsModel()
		{
			// arrange
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockCachedService = new Mock<ICachedService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			
			var expected = TrustFactory.BuildTrustDetailsModel();
			var expectedTypeModel = TypeFactory.BuildTypeModel();
			var expectedRatingsModel = RatingFactory.BuildListRatingModel();

			mockTypeModelService.Setup(t => t.GetTypeModel()).ReturnsAsync(expectedTypeModel);
			mockCachedService.Setup(c => c.GetData<UserState>(It.IsAny<string>())).ReturnsAsync(new UserState { TrustUkPrn = "trust-ukprn" });
			mockTrustModelService.Setup(s => s.GetTrustByUkPrn(It.IsAny<string>())).ReturnsAsync(expected);
			mockRatingModelService.Setup(r => r.GetRatingsModel()).ReturnsAsync(expectedRatingsModel);
			
			var pageModel = SetupConcernTypePageModel(mockTrustModelService.Object, mockCachedService.Object, 
				mockTypeModelService.Object, mockRatingModelService.Object, mockLogger.Object, true);
			
			// act
			await pageModel.OnGetAsync();
			
			// assert
			Assert.That(pageModel, Is.Not.Null);
			Assert.That(pageModel.RatingsModel, Is.Not.Null);
			Assert.That(pageModel.TypeModel, Is.Not.Null);
			Assert.That(pageModel.TrustDetailsModel, Is.Not.Null);
			Assert.That(pageModel.CreateRecordsModel, Is.Not.Null);
			
			var trustDetailsModel = pageModel.TrustDetailsModel;
			var typesDictionary = pageModel.TypeModel.TypesDictionary;
			var ratingsModel = pageModel.RatingsModel;
			var createRecordsModel = pageModel.CreateRecordsModel;
			
			Assert.IsAssignableFrom<List<RatingModel>>(ratingsModel);
			Assert.IsAssignableFrom<TrustDetailsModel>(trustDetailsModel);
			Assert.IsAssignableFrom<List<CreateRecordModel>>(createRecordsModel);
			Assert.IsAssignableFrom<Dictionary<string, IList<TypeModel.TypeValueModel>>>(typesDictionary);

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
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("IndexPageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
			
			Assert.That(pageModel.TempData["Error.Message"], Is.Null);
		}
		
		[Test]
		public async Task WhenOnGetAsync_MissingCacheUserState_ThrowsException()
		{
			// arrange
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
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
			
			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("IndexPageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
			
			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
			
			Assert.That(pageModel.TypeModel, Is.Null);
		}

		[Test]
		public async Task WhenOnPostAsync_UserStateIsNull_ThrowsException()
		{
			// arrange
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
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
					{ "sub-type", new StringValues("999:subType") },
					{ "rating", new StringValues("ragRating:123") }
				});
			
			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;
			
			Assert.That(page, Is.Not.Null);
			
			mockCachedService.Verify(c => c.GetData<UserState>(It.IsAny<string>()), Times.Exactly(2));
			mockCachedService.Verify(c => c.StoreData(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<int>()), Times.Never);
		}
		
		[Test]
		public async Task WhenOnPostAsync_ReturnConcernPage()
		{
			// arrange
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
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
					{ "sub-type", new StringValues("123:subType") },
					{ "rating", new StringValues("123:ragRating") }
				});
			
			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectToPageResult>());
			var page = pageResponse as RedirectToPageResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(page.PageName, Is.EqualTo("add"));
			
			mockCachedService.Verify(c => c.GetData<UserState>(It.IsAny<string>()), Times.Once);
			mockCachedService.Verify(c => c.StoreData(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<int>()), Times.Once);
		}
		
		[Test]
		public async Task WhenOnPostAsync_MissingFormData_ThrowsException()
		{
			// arrange
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
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
			
			mockCachedService.Verify(c => c.GetData<UserState>(It.IsAny<string>()), Times.Once);
			mockCachedService.Verify(c => c.StoreData(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<int>()), Times.Never);
		}

		[Test]
		public async Task WhenOnGetCancelCreateCase_EmptiesCreateRecords_ReturnsHome()
		{
			// arrange
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
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
					{ "sub-type", new StringValues("123:subType") },
					{ "rating", new StringValues("123:ragRating") }
				});

			// act
			var pageResponse = await pageModel.OnGetCancel();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(page.Url, Is.EqualTo("/"));

			mockCachedService.Verify(c => c.GetData<UserState>(It.IsAny<string>()), Times.Once);
			mockCachedService.Verify(c => c.StoreData(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<int>()), Times.Once);
		}
		
		[TestCase("", "", "")]
		[TestCase(null, null, null)]
		[TestCase("test", "", "ragRating")]
		public async Task WhenOnPostAsync_InvalidFormData_ThrowsException(string type, string subType, string ragRating)
		{
			// arrange
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
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
					{ "type", new StringValues(type) },
					{ "sub-type", new StringValues(subType) },
					{ "rating", new StringValues(ragRating) }
				});
			
			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;
			
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
			Assert.That(page, Is.Not.Null);
			
			mockCachedService.Verify(c => c.GetData<UserState>(It.IsAny<string>()), Times.Once);
			mockCachedService.Verify(c => c.StoreData(It.IsAny<string>(), It.IsAny<UserState>(), It.IsAny<int>()), Times.Never);
		}
		
		private static IndexPageModel SetupConcernTypePageModel(
			ITrustModelService mockTrustModelService, ICachedService mockCachedService, ITypeModelService mockTypeModelService, 
			IRatingModelService mockRatingModelService, ILogger<IndexPageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new IndexPageModel(mockTrustModelService, mockCachedService, mockTypeModelService, mockRatingModelService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}