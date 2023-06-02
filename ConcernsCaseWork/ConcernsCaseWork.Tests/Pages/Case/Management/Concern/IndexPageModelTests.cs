using AutoFixture;
using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case.Management.Concern;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.MeansOfReferral;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Services.Types;
using ConcernsCaseWork.Shared.Tests.Factory;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Concern
{
	[Parallelizable(ParallelScope.All)]
	public class IndexPageModelTests
	{
		private static Fixture _fixture = new();

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

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			mockRecordModelService.Setup(r => r.GetCreateRecordsModelByCaseUrn(It.IsAny<long>()))
				.ReturnsAsync(createRecordsModel);
			mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);
			mockRatingModelService.Setup(r => r.GetRatingsModel())
				.ReturnsAsync(ratingsModel);

			var pageModel = SetupIndexPageModel(mockCaseModelService.Object, mockTrustModelService.Object,
				mockRecordModelService.Object, mockRatingModelService.Object, mockLogger.Object);
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			
			// act
			await pageModel.OnGetAsync();
			
			// assert
			Assert.NotNull(pageModel.CreateRecordsModel);
			Assert.NotNull(pageModel.TrustDetailsModel);
			Assert.That(pageModel.CreateRecordsModel.Count, Is.EqualTo(createRecordsModel.Count));
			Assert.Null(pageModel.TempData["Error.Message"]);
			pageModel.ConcernRiskRating.Should().NotBeNull();
			pageModel.ConcernType.Should().NotBeNull();
			pageModel.MeansOfReferral.Should().NotBeNull();
			
			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<long>()), Times.Exactly(2));
			mockRecordModelService.Verify(r => r.GetCreateRecordsModelByCaseUrn(It.IsAny<long>()), Times.Once);
			mockTrustModelService.Verify(t => t.GetTrustByUkPrn(It.IsAny<string>()), Times.Once);
			mockRatingModelService.Verify(r => r.GetRatingsModel(), Times.Once);
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
			
			mockRecordModelService.Setup(r => r.PostRecordByCaseUrn(It.IsAny<CreateRecordModel>()));

			var pageModel = SetupIndexPageModel(mockCaseModelService.Object, mockTrustModelService.Object,
				mockRecordModelService.Object, mockRatingModelService.Object, mockLogger.Object);

			pageModel.ConcernType = _fixture.Create<RadioButtonsUiComponent>();
			pageModel.ConcernType.SelectedId = (int)ConcernType.ForceMajeure;
			pageModel.ConcernType.SelectedSubId = null;

			pageModel.ConcernRiskRating = _fixture.Create<RadioButtonsUiComponent>();
			pageModel.ConcernRiskRating.SelectedId = 1;

			pageModel.MeansOfReferral = _fixture.Create<RadioButtonsUiComponent>();
			pageModel.MeansOfReferral.SelectedId = 1;

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			
			// act
			var pageResponse = await pageModel.OnPostAsync();
			pageResponse = pageResponse as RedirectResult;
			
			// assert
			Assert.NotNull(pageResponse);
			Assert.Null(pageModel.CreateRecordsModel);
			Assert.Null(pageModel.TrustDetailsModel);
			Assert.Null(pageModel.TempData["Error.Message"]);
			
			mockRecordModelService.Verify(r => r.PostRecordByCaseUrn(It.IsAny<CreateRecordModel>()), Times.Once());
		}		
		
		private static IndexPageModel SetupIndexPageModel(
			ICaseModelService mockCaseModelService, 
			ITrustModelService mockTrustModelService,
			IRecordModelService mockRecordModelService,
			IRatingModelService mockRatingModelService,
			ILogger<IndexPageModel> mockLogger, 
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new IndexPageModel(mockCaseModelService, mockRecordModelService, mockTrustModelService, mockRatingModelService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}