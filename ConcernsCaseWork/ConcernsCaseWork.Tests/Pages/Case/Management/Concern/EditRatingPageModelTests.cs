using ConcernsCaseWork.Pages.Case.Management.Concern;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Shared.Tests.Factory;
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
	public class EditRatingPageModelTests
	{
		[Test]
		public async Task WhenOnGet_ReturnsPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockLogger = new Mock<ILogger<EditRatingPageModel>>();

			var caseModel = CaseFactory.BuildCaseModel();
			var recordModel = RecordFactory.BuildRecordModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<long>()))
				.ReturnsAsync(caseModel);

			mockRecordModelService.Setup(r => r.GetRecordModelById(It.IsAny<long>(), It.IsAny<long>()))
				.ReturnsAsync(recordModel);

			mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);

			var pageModel = SetupEditRiskRatingPageModel(mockCaseModelService.Object, mockRecordModelService.Object, mockTrustModelService.Object, mockLogger.Object);
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1); 
			routeData.Add("recordId", 1);
			
			// act
			var pageResponse = await pageModel.OnGet();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.CaseModel, Is.Not.Null);
			Assert.That(pageModel.TrustDetailsModel, Is.Not.Null);
			
			mockCaseModelService.Verify(c => 
				c.GetCaseByUrn(It.IsAny<long>()), Times.Once);
		}

		private static EditRatingPageModel SetupEditRiskRatingPageModel(
			ICaseModelService mockCaseModelService, IRecordModelService mockRecordModelService, ITrustModelService mockTrustModelService,  ILogger<EditRatingPageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new EditRatingPageModel(mockCaseModelService, mockRecordModelService, mockTrustModelService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}