using ConcernsCaseWork.Pages.Case.Management.Concern;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.MeansOfReferral;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Services.Types;
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
using Service.Redis.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Concern
{
	[Parallelizable(ParallelScope.All)]
	public class IndexPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_ReturnPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockMeansOfReferralModelService = new Mock<IMeansOfReferralModelService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var caseModel = CaseFactory.BuildCaseModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			var ratingsModel = RatingFactory.BuildListRatingModel();
			var typeModel = TypeFactory.BuildTypeModel();
			var createRecordsModel = RecordFactory.BuildListCreateRecordModel();

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			mockRecordModelService.Setup(r => r.GetCreateRecordsModelByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(createRecordsModel);
			mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);
			mockRatingModelService.Setup(r => r.GetRatingsModel())
				.ReturnsAsync(ratingsModel);
			mockTypeModelService.Setup(t => t.GetTypeModel())
				.ReturnsAsync(typeModel);
			
			var pageModel = SetupIndexPageModel(mockCaseModelService.Object, mockTrustModelService.Object,
				mockTypeModelService.Object, mockRecordModelService.Object, mockRatingModelService.Object, mockMeansOfReferralModelService.Object, mockLogger.Object);

			pageModel.Request.Headers.Add("Referer", "https://returnto/thispage");
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			
			// act
			await pageModel.OnGetAsync();
			
			// assert
			Assert.NotNull(pageModel.PreviousUrl);
			Assert.NotNull(pageModel.RatingsModel);
			Assert.NotNull(pageModel.TypeModel);
			Assert.NotNull(pageModel.CreateRecordsModel);
			Assert.NotNull(pageModel.TrustDetailsModel);
			Assert.That(pageModel.CreateRecordsModel.Count, Is.EqualTo(createRecordsModel.Count));
			Assert.Null(pageModel.TempData["Error.Message"]);
			
			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Once);
			mockRecordModelService.Verify(r => r.GetCreateRecordsModelByCaseUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Once);
			mockTrustModelService.Verify(t => t.GetTrustByUkPrn(It.IsAny<string>()), Times.Once);
			mockRatingModelService.Verify(r => r.GetRatingsModel(), Times.Once);
			mockTypeModelService.Verify(t => t.GetTypeModel(), Times.Once);
		}
		
		[Test]
		public async Task WhenOnGetAsync_MissingRouteData_ReturnPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockMeansOfReferralModelService = new Mock<IMeansOfReferralModelService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
			
			var pageModel = SetupIndexPageModel(mockCaseModelService.Object, mockTrustModelService.Object,
				mockTypeModelService.Object, mockRecordModelService.Object, mockRatingModelService.Object, mockMeansOfReferralModelService.Object, mockLogger.Object);
			
			// act
			await pageModel.OnGetAsync();
			
			// assert
			Assert.Null(pageModel.PreviousUrl);
			Assert.Null(pageModel.RatingsModel);
			Assert.Null(pageModel.TypeModel);
			Assert.Null(pageModel.CreateRecordsModel);
			Assert.Null(pageModel.TrustDetailsModel);
			Assert.NotNull(pageModel.TempData["Error.Message"]);
			
			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
			mockRecordModelService.Verify(r => r.GetCreateRecordsModelByCaseUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
			mockTrustModelService.Verify(t => t.GetTrustByUkPrn(It.IsAny<string>()), Times.Never);
			mockRatingModelService.Verify(r => r.GetRatingsModel(), Times.Never);
			mockTypeModelService.Verify(t => t.GetTypeModel(), Times.Never);
		}
		
		[Test]
		public async Task WhenOnPostAsync_Redirect_ToUrl()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockMeansOfReferralModelService = new Mock<IMeansOfReferralModelService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
			
			mockRecordModelService.Setup(r => r.PostRecordByCaseUrn(It.IsAny<CreateRecordModel>(), It.IsAny<string>()));
			
			var pageModel = SetupIndexPageModel(mockCaseModelService.Object, mockTrustModelService.Object,
				mockTypeModelService.Object, mockRecordModelService.Object, mockRatingModelService.Object, mockMeansOfReferralModelService.Object, mockLogger.Object);

			pageModel.Request.Headers.Add("Referer", "https://returnto/thispage");
			
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "type", new StringValues("governance") },
					{ "sub-type", new StringValues("123:governance") },
					{ "rating", new StringValues("123:red") },
					{ "means-of-referral-id", new StringValues("1") }
				});
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			
			// act
			var pageResponse = await pageModel.OnPostAsync("https://returnto/thispage");
			pageResponse = pageResponse as RedirectResult;
			
			// assert
			Assert.NotNull(pageResponse);
			Assert.Null(pageModel.PreviousUrl);
			Assert.Null(pageModel.RatingsModel);
			Assert.Null(pageModel.TypeModel);
			Assert.Null(pageModel.CreateRecordsModel);
			Assert.Null(pageModel.TrustDetailsModel);
			Assert.Null(pageModel.MeansOfReferralModel);
			Assert.Null(pageModel.TempData["Error.Message"]);
			
			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
			mockRecordModelService.Verify(r => r.GetCreateRecordsModelByCaseUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
			mockTrustModelService.Verify(t => t.GetTrustByUkPrn(It.IsAny<string>()), Times.Never);
			mockRatingModelService.Verify(r => r.GetRatingsModel(), Times.Never);
			mockTypeModelService.Verify(t => t.GetTypeModel(), Times.Never);
			mockMeansOfReferralModelService.Verify(t => t.GetMeansOfReferrals(), Times.Never);
			mockRecordModelService.Verify(r => r.PostRecordByCaseUrn(It.IsAny<CreateRecordModel>(), It.IsAny<string>()), Times.Once());
		}		
		
		[Test]
		public async Task WhenOnPostAsync_InvalidForm_ReturnPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockMeansOfReferralModelService = new Mock<IMeansOfReferralModelService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
			
			mockRecordModelService.Setup(r => r.PostRecordByCaseUrn(It.IsAny<CreateRecordModel>(), It.IsAny<string>()));
			
			var pageModel = SetupIndexPageModel(mockCaseModelService.Object, mockTrustModelService.Object,
				mockTypeModelService.Object, mockRecordModelService.Object, mockRatingModelService.Object, mockMeansOfReferralModelService.Object, mockLogger.Object);

			pageModel.Request.Headers.Add("Referer", "https://returnto/thispage");
			
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "type", new StringValues("") },
					{ "sub-type", new StringValues("123:governance") },
					{ "rating", new StringValues("123:red") }
				});
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			
			// act
			var pageResponse = await pageModel.OnPostAsync("https://returnto/thispage");
			
			// assert
			Assert.NotNull(pageResponse);
			Assert.Null(pageModel.PreviousUrl);
			Assert.Null(pageModel.RatingsModel);
			Assert.Null(pageModel.TypeModel);
			Assert.Null(pageModel.CreateRecordsModel);
			Assert.Null(pageModel.TrustDetailsModel);
			Assert.NotNull(pageModel.TempData["Error.Message"]);
			
			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
			mockRecordModelService.Verify(r => r.GetCreateRecordsModelByCaseUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
			mockTrustModelService.Verify(t => t.GetTrustByUkPrn(It.IsAny<string>()), Times.Never);
			mockRatingModelService.Verify(r => r.GetRatingsModel(), Times.Never);
			mockTypeModelService.Verify(t => t.GetTypeModel(), Times.Never);
			mockRecordModelService.Verify(r => r.PostRecordByCaseUrn(It.IsAny<CreateRecordModel>(), It.IsAny<string>()), Times.Never());
		}		
		
		[Test]
		public async Task WhenOnPostAsync_MissingRouteData_ReturnPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockMeansOfReferralModelService = new Mock<IMeansOfReferralModelService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
			
			mockRecordModelService.Setup(r => r.PostRecordByCaseUrn(It.IsAny<CreateRecordModel>(), It.IsAny<string>()));
			
			var pageModel = SetupIndexPageModel(mockCaseModelService.Object, mockTrustModelService.Object,
				mockTypeModelService.Object, mockRecordModelService.Object, mockRatingModelService.Object, mockMeansOfReferralModelService.Object, mockLogger.Object);

			pageModel.Request.Headers.Add("Referer", "https://returnto/thispage");
			
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "type", new StringValues("governance") },
					{ "sub-type", new StringValues("123:governance") },
					{ "rating", new StringValues("123:red") }
				});
			
			// act
			var pageResponse = await pageModel.OnPostAsync("https://returnto/thispage");
			
			// assert
			Assert.NotNull(pageResponse);
			Assert.Null(pageModel.PreviousUrl);
			Assert.Null(pageModel.RatingsModel);
			Assert.Null(pageModel.TypeModel);
			Assert.Null(pageModel.CreateRecordsModel);
			Assert.Null(pageModel.TrustDetailsModel);
			Assert.NotNull(pageModel.TempData["Error.Message"]);
			
			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
			mockRecordModelService.Verify(r => r.GetCreateRecordsModelByCaseUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
			mockTrustModelService.Verify(t => t.GetTrustByUkPrn(It.IsAny<string>()), Times.Never);
			mockRatingModelService.Verify(r => r.GetRatingsModel(), Times.Never);
			mockTypeModelService.Verify(t => t.GetTypeModel(), Times.Never);
			mockRecordModelService.Verify(r => r.PostRecordByCaseUrn(It.IsAny<CreateRecordModel>(), It.IsAny<string>()), Times.Never());
		}		
		
		private static IndexPageModel SetupIndexPageModel(
			ICaseModelService mockCaseModelService, 
			ITrustModelService mockTrustModelService,
			ITypeModelService mockTypeModelService,
			IRecordModelService mockRecordModelService,
			IRatingModelService mockRatingModelService,
			IMeansOfReferralModelService mockMeansOfReferralModelService,
			ILogger<IndexPageModel> mockLogger, 
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new IndexPageModel(mockCaseModelService, mockRecordModelService, mockTrustModelService, mockTypeModelService, mockRatingModelService, mockMeansOfReferralModelService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}