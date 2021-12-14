using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case.Management;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Records;
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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Management
{
	[Parallelizable(ParallelScope.All)]
	public class EditConcernTypePageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_ReturnsPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockLogger = new Mock<ILogger<EditConcernPageModel>>();
			
			var caseModel = CaseFactory.BuildCaseModel();
			var recordModel = RecordFactory.BuildRecordModel();
			var typeModel = TypeFactory.BuildTypeModel();

			mockRecordModelService.Setup(r => r.GetRecordModelByUrn(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>()))
				.ReturnsAsync(recordModel);
			mockTypeModelService.Setup(t => t.GetSelectedTypeModelByUrn(It.IsAny<long>()))
				.ReturnsAsync(typeModel);
			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);

			var pageModel = SetupEditConcernTypePageModel(mockCaseModelService.Object, mockTypeModelService.Object, mockRecordModelService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("recordUrn", 1);
			pageModel.Request.Headers.Add("Referer", "https://returnto/thispage");

			// act
			var pageResponse = await pageModel.OnGetAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.CaseModel.PreviousUrl, Is.EqualTo(caseModel.PreviousUrl));
			Assert.That(pageModel.CaseModel.Description, Is.EqualTo(caseModel.Description));
			Assert.That(pageModel.CaseModel.Issue, Is.EqualTo(caseModel.Issue));
			Assert.That(pageModel.CaseModel.Urn, Is.EqualTo(caseModel.Urn));
			Assert.That(pageModel.CaseModel.CaseAim, Is.EqualTo(caseModel.CaseAim));
			Assert.That(pageModel.CaseModel.ClosedAt, Is.EqualTo(caseModel.ClosedAt));
			Assert.That(pageModel.CaseModel.CreatedAt, Is.EqualTo(caseModel.CreatedAt));
			Assert.That(pageModel.CaseModel.CreatedBy, Is.EqualTo(caseModel.CreatedBy));
			Assert.That(pageModel.CaseModel.CrmEnquiry, Is.EqualTo(caseModel.CrmEnquiry));
			Assert.That(pageModel.CaseModel.CurrentStatus, Is.EqualTo(caseModel.CurrentStatus));
			Assert.That(pageModel.CaseModel.DeEscalation, Is.EqualTo(caseModel.DeEscalation));
			Assert.That(pageModel.CaseModel.NextSteps, Is.EqualTo(caseModel.NextSteps));
			Assert.That(pageModel.CaseModel.ReviewAt, Is.EqualTo(caseModel.ReviewAt));
			Assert.That(pageModel.CaseModel.StatusName, Is.EqualTo(caseModel.StatusName));
			Assert.That(pageModel.CaseModel.StatusUrn, Is.EqualTo(caseModel.StatusUrn));
			Assert.That(pageModel.CaseModel.UpdatedAt, Is.EqualTo(caseModel.UpdatedAt));
			Assert.That(pageModel.CaseModel.DeEscalationPoint, Is.EqualTo(caseModel.DeEscalationPoint));
			Assert.That(pageModel.CaseModel.DirectionOfTravel, Is.EqualTo(caseModel.DirectionOfTravel));
			Assert.That(pageModel.CaseModel.ReasonAtReview, Is.EqualTo(caseModel.ReasonAtReview));
			Assert.That(pageModel.CaseModel.TrustUkPrn, Is.EqualTo(caseModel.TrustUkPrn));
			
			Assert.That(pageModel.TypeModel, Is.Not.Null);
			Assert.That(pageModel.TypeModel.CheckedType, Is.EqualTo(typeModel.CheckedType));
			Assert.That(pageModel.TypeModel.TypeDisplay, Is.EqualTo(typeModel.TypeDisplay));
			Assert.That(pageModel.TypeModel.TypesDictionary, Is.EqualTo(typeModel.TypesDictionary));
			Assert.That(pageModel.TypeModel.CheckedSubType, Is.EqualTo(typeModel.CheckedSubType));
			
			mockRecordModelService.Verify(r => r.GetRecordModelByUrn(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>()), Times.Once);
			mockTypeModelService.Verify(t => t.GetSelectedTypeModelByUrn(It.IsAny<long>()), Times.Once);
			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Once);
		}
		
		[Test]
		public async Task WhenOnGetAsync_Missing_RouteData_ReturnsPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockLogger = new Mock<ILogger<EditConcernPageModel>>();
			
			var pageModel = SetupEditConcernTypePageModel(mockCaseModelService.Object, mockTypeModelService.Object, mockRecordModelService.Object, mockLogger.Object);
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			pageModel.Request.Headers.Add("Referer", "https://returnto/thispage");
			
			// act
			var pageResponse = await pageModel.OnGetAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.CaseModel, Is.Null);

			mockRecordModelService.Verify(r => r.GetRecordModelByUrn(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>()), Times.Never);
			mockTypeModelService.Verify(t => t.GetSelectedTypeModelByUrn(It.IsAny<long>()), Times.Never);
			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
		}		
		
		[Test]
		public async Task WhenOnGetAsync_Missing_RouteData_RecordUrn_ReturnsPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockLogger = new Mock<ILogger<EditConcernPageModel>>();
			
			var pageModel = SetupEditConcernTypePageModel(mockCaseModelService.Object, mockTypeModelService.Object, mockRecordModelService.Object, mockLogger.Object);
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("recordUrn", 1);
			pageModel.Request.Headers.Add("Referer", "https://returnto/thispage");
			
			// act
			var pageResponse = await pageModel.OnGetAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.CaseModel, Is.Null);

			mockRecordModelService.Verify(r => r.GetRecordModelByUrn(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>()), Times.Never);
			mockTypeModelService.Verify(t => t.GetSelectedTypeModelByUrn(It.IsAny<long>()), Times.Never);
			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
		}
		
		[Test]
		public async Task WhenOnPostEditConcernType_MissingRouteData_ThrowsException_ReloadPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockLogger = new Mock<ILogger<EditConcernPageModel>>();
			
			var pageModel = SetupEditConcernTypePageModel(mockCaseModelService.Object, mockTypeModelService.Object, mockRecordModelService.Object, mockLogger.Object);
			
			// act
			var pageResponse = await pageModel.OnPostEditConcernType("https://returnto/thispage");

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.CaseModel, Is.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
			
			mockRecordModelService.Verify(r => r.GetRecordModelByUrn(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>()), Times.Never);
			mockTypeModelService.Verify(t => t.GetSelectedTypeModelByUrn(It.IsAny<long>()), Times.Never);
			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
		}
		
		[Test]
		public async Task WhenOnPostEditConcernType_MissingRouteData_ThrowsException()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockLogger = new Mock<ILogger<EditConcernPageModel>>();
			var casesDto = CaseFactory.BuildCaseModel();
			
			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(casesDto);
			mockTypeModelService.Setup(t => t.GetTypeModel()).ReturnsAsync((TypeModel)null);

			var pageModel = SetupEditConcernTypePageModel(mockCaseModelService.Object, mockTypeModelService.Object, mockRecordModelService.Object, mockLogger.Object);

			// act
			var pageResponse = await pageModel.OnPostEditConcernType("https://returnto/thispage");
			
			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.CaseModel, Is.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));

			mockRecordModelService.Verify(r => r.GetRecordModelByUrn(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>()), Times.Never);
			mockTypeModelService.Verify(t => t.GetSelectedTypeModelByUrn(It.IsAny<long>()), Times.Never);
			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
		}

		[TestCase("", "")]
		[TestCase(null, null)]
		[TestCase("test", "")]
		public async Task WhenOnPostEditConcernType_RouteData_MissingRequestForm_ReloadPage(string type, string subType)
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockLogger = new Mock<ILogger<EditConcernPageModel>>();

			var caseModel = CaseFactory.BuildCaseModel();
			
			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			
			var pageModel = SetupEditConcernTypePageModel(mockCaseModelService.Object, mockTypeModelService.Object, mockRecordModelService.Object, mockLogger.Object);
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("recordUrn", 1);
			
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "type", new StringValues(type) },
					{ "sub-type", new StringValues(subType) }
				});
			
			// act
			var pageResponse = await pageModel.OnPostEditConcernType("https://returnto/thispage");

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.CaseModel, Is.Null);
			
			mockRecordModelService.Verify(r => r.GetRecordModelByUrn(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>()), Times.Never);
			mockTypeModelService.Verify(t => t.GetSelectedTypeModelByUrn(It.IsAny<long>()), Times.Never);
			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
		}
		
		[TestCase("123:Force Majeure", "test")]
		[TestCase("123:Force Majeure", "")]
		[TestCase("123:Force Majeure", null)]
		[TestCase("Compliance", "123:Financial reporting")]
		[TestCase("Compliance", "123:Financial returns")]
		[TestCase("Financial", "123:Deficit")]
		[TestCase("Financial", "123:Projected deficit / Low future surplus")]
		[TestCase("Financial", "123:Cash flow shortfall")]
		[TestCase("Financial", "123:Clawback")]
		[TestCase("Governance", "123:Governance")]
		[TestCase("Irregularity", "123:Allegations and self reported concerns")]
		public async Task WhenOnPostEditConcernType_RouteData_RequestForm_ReturnsToPreviousUrl(string type, string subType)
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockLogger = new Mock<ILogger<EditConcernPageModel>>();

			mockCaseModelService.Setup(c => c.PatchConcernType(It.IsAny<PatchCaseModel>()));
			
			var pageModel = SetupEditConcernTypePageModel(mockCaseModelService.Object, mockTypeModelService.Object, mockRecordModelService.Object, mockLogger.Object);
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("recordUrn", 1);
			
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "type", new StringValues(type) },
					{ "sub-type", new StringValues(subType) }
				});
			
			// act
			var pageResponse = await pageModel.OnPostEditConcernType("https://returnto/thispage");

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.CaseModel, Is.Null);
			Assert.That(page.Url, Is.EqualTo("https://returnto/thispage"));
		}
		
		private static EditConcernPageModel SetupEditConcernTypePageModel(ICaseModelService mockCaseModelService, 
			ITypeModelService mockTypeModelService, 
			IRecordModelService mockRecordModelService,
			ILogger<EditConcernPageModel> mockLogger, 
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new EditConcernPageModel(mockTypeModelService, mockCaseModelService, mockRecordModelService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}