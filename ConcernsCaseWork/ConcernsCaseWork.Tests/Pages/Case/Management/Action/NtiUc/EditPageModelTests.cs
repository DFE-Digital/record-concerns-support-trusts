using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Case.Management.Action.NtiUnderConsideration;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Shared.Tests.Factory;
using ConcernsCaseWork.Shared.Tests.MockHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using Service.Redis.FinancialPlan;
using Service.Redis.NtiUnderConsideration;
using Service.TRAMS.NtiUnderConsideration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.NtiUc
{
	[Parallelizable(ParallelScope.All)]
	public class EditPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_MissingCaseUrn_ThrowsException_ReturnPage()
		{
			// arrange
			Mock<INtiUnderConsiderationModelService> mockNtiModelService = new Mock<INtiUnderConsiderationModelService>();
			Mock<INtiUnderConsiderationReasonsCachedService> mockNtiReasonsCachedService = new Mock<INtiUnderConsiderationReasonsCachedService>();
			Mock<ILogger<EditPageModel>> mockLogger = new Mock<ILogger<EditPageModel>>();

			var pageModel = SetupAddPageModel(mockNtiModelService, mockNtiReasonsCachedService, mockLogger);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
		}

		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			var ntiUcId = 834;
			var caseUrn = 234;
			
			Mock<INtiUnderConsiderationModelService> mockNtiModelService = new Mock<INtiUnderConsiderationModelService>();
			Mock<INtiUnderConsiderationReasonsCachedService> mockNtiReasonsCachedService = new Mock<INtiUnderConsiderationReasonsCachedService>();
			Mock<ILogger<EditPageModel>> mockLogger = new Mock<ILogger<EditPageModel>>();

			var pageModel = SetupAddPageModel(mockNtiModelService, mockNtiReasonsCachedService, mockLogger);
			
			var ntiUcModel = new NtiUnderConsiderationModel();
			
			mockNtiModelService
				.Setup(fp => fp.GetNtiUnderConsideration(ntiUcId))
				.ReturnsAsync(ntiUcModel);

			mockNtiReasonsCachedService.Setup(s => s.GetAllReasons()).ReturnsAsync(new List<NtiUnderConsiderationReasonDto>());

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseUrn);
			routeData.Add("ntiucid", ntiUcId);

			// act
			await pageModel.OnGetAsync();

			// assert
			mockLogger.VerifyLogErrorWasNotCalled();
			
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::NTI-UC::EditPageModel::OnGetAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}
		
		[Test]
		public async Task WhenOnGetAsync_WhenNtiUcIsClosed_RedirectsToIndexPage()
		{
			// arrange
			var ntiUcId = 834;
			var caseUrn = 234;
			
			Mock<INtiUnderConsiderationModelService> mockNtiModelService = new Mock<INtiUnderConsiderationModelService>();
			Mock<INtiUnderConsiderationReasonsCachedService> mockNtiReasonsCachedService = new Mock<INtiUnderConsiderationReasonsCachedService>();
			Mock<ILogger<EditPageModel>> mockLogger = new Mock<ILogger<EditPageModel>>();

			var pageModel = SetupAddPageModel(mockNtiModelService, mockNtiReasonsCachedService, mockLogger);

			var ntiUcModel = new NtiUnderConsiderationModel(){ ClosedAt = DateTime.Now };
			
			mockNtiModelService
				.Setup(fp => fp.GetNtiUnderConsideration(ntiUcId))
				.ReturnsAsync(ntiUcModel);

			mockNtiReasonsCachedService.Setup(s => s.GetAllReasons()).ReturnsAsync(new List<NtiUnderConsiderationReasonDto>());

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseUrn);
			routeData.Add("ntiucid", ntiUcId);

			// act
			var response = await pageModel.OnGetAsync();

			// assert
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::NTI-UC::EditPageModel::OnGetAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
			
			Assert.Multiple(() =>
			{
				Assert.That(response, Is.InstanceOf<RedirectResult>());
				Assert.That(((RedirectResult)response).Url, Is.EqualTo($"/case/{caseUrn}/management/action/ntiunderconsideration/{ntiUcId}"));
				Assert.That(pageModel.TempData["Error.Message"], Is.Null);
			});
		}

		[Test]
		public async Task WhenOnPostAsync_MissingRouteData_ThrowsException_ReturnsPage()
		{
			// arrange
			Mock<INtiUnderConsiderationModelService> mockNtiModelService = new Mock<INtiUnderConsiderationModelService>();
			Mock<INtiUnderConsiderationReasonsCachedService> mockNtiReasonsCachedService = new Mock<INtiUnderConsiderationReasonsCachedService>();
			Mock<ILogger<EditPageModel>> mockLogger = new Mock<ILogger<EditPageModel>>();

			var pageModel = SetupAddPageModel(mockNtiModelService, mockNtiReasonsCachedService, mockLogger);

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
		}

		[Test]
		public async Task WhenOnPostAsync_ValidatesNotesLength_ThrowsException_ReturnsPage()
		{
			// arrange
			Mock<INtiUnderConsiderationModelService> mockNtiModelService = new Mock<INtiUnderConsiderationModelService>();
			Mock<INtiUnderConsiderationReasonsCachedService> mockNtiReasonsCachedService = new Mock<INtiUnderConsiderationReasonsCachedService>();
			Mock<ILogger<EditPageModel>> mockLogger = new Mock<ILogger<EditPageModel>>();

			var pageModel = SetupAddPageModel(mockNtiModelService, mockNtiReasonsCachedService, mockLogger);

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
		}

		[Test]
		[TestCase(0, true)]
		[TestCase(1, true)]
		[TestCase(AddPageModel.NotesMaxLength, true)]
		[TestCase(AddPageModel.NotesMaxLength + 1, false)]
		public async Task WhenOnPostAsync_ValidateNotes(int notesLength, bool isValid)
		{
			// arrange
			var ntiId = 123;
			Mock<INtiUnderConsiderationModelService> mockNtiModelService = new Mock<INtiUnderConsiderationModelService>();
			Mock<INtiUnderConsiderationReasonsCachedService> mockNtiReasonsCachedService = new Mock<INtiUnderConsiderationReasonsCachedService>();
			Mock<ILogger<EditPageModel>> mockLogger = new Mock<ILogger<EditPageModel>>();

			mockNtiModelService.Setup(ms => ms.GetNtiUnderConsideration(It.IsAny<long>()))
				.ReturnsAsync(new NtiUnderConsiderationModel
			{
				Id = ntiId
			});

			mockNtiModelService.Setup(ms => ms.PatchNtiUnderConsideration(It.IsAny<NtiUnderConsiderationModel>()))
				.ReturnsAsync(new NtiUnderConsiderationModel
			{
				Id = ntiId
			});

			var pageModel = SetupAddPageModel(mockNtiModelService, mockNtiReasonsCachedService, mockLogger);
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("ntiUCId", ntiId);

			pageModel.HttpContext.Request.Form = new FormCollection(
			new Dictionary<string, StringValues>
			{
				{ "nti-notes", new StringValues(GenereateString(notesLength)) }
			});

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			if (isValid)
			{
				Assert.That(pageModel.TempData["NTI-UC.Message"], Is.Null);
			}
			else
			{
				Assert.That(pageModel.TempData["NTI-UC.Message"], Is.Not.Null);
			}
		}

		private string GenereateString(int length)
		{
			if (length == 0)
			{
				return String.Empty;
			}

			var chars = Enumerable.Range(65, 50).Select(i => (char)i).ToArray();
			var stringChars = new char[length];
			var random = new Random();

			for (int i = 0; i < stringChars.Length; i++)
			{
				stringChars[i] = chars[random.Next(chars.Length)];
			}

			var finalString = new String(stringChars);

			return finalString;
		}

		private static EditPageModel SetupAddPageModel(
			Mock<INtiUnderConsiderationModelService> mockNtiModelService,
			Mock<INtiUnderConsiderationReasonsCachedService> mockNtiReasonsCachedService,
			Mock<ILogger<EditPageModel>> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new EditPageModel(mockNtiModelService.Object, mockNtiReasonsCachedService.Object, mockLogger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}


}