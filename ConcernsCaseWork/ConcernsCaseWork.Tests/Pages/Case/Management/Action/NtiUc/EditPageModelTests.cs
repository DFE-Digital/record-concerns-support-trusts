using AutoFixture;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Case.Management.Action.NtiUnderConsideration;
using ConcernsCaseWork.Redis.NtiUnderConsideration;
using ConcernsCaseWork.Service.NtiUnderConsideration;
using ConcernsCaseWork.Services.NtiUnderConsideration;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.NtiUc
{
	[Parallelizable(ParallelScope.All)]
	public class EditPageModelTests
	{
		private readonly IFixture _fixture;

		public EditPageModelTests()
		{
			_fixture = new Fixture();
		}
		
		[Test]
		public async Task WhenOnGetAsync_MissingCaseUrn_ThrowsException_ReturnPage()
		{
			// arrange
			Mock<INtiUnderConsiderationModelService> mockNtiModelService = new Mock<INtiUnderConsiderationModelService>();
			Mock<ILogger<EditPageModel>> mockLogger = new Mock<ILogger<EditPageModel>>();

			var pageModel = SetupAddPageModel(mockNtiModelService, mockLogger);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnGetPage));
		}

		
		[Test]
		public async Task WhenOnGetAsync_WhenNtiUcIsClosed_RedirectsToIndexPage()
		{
			// arrange
			var ntiUcId = 834;
			var caseUrn = 234;
			
			Mock<INtiUnderConsiderationModelService> mockNtiModelService = new Mock<INtiUnderConsiderationModelService>();
			Mock<ILogger<EditPageModel>> mockLogger = new Mock<ILogger<EditPageModel>>();

			var pageModel = SetupAddPageModel(mockNtiModelService, mockLogger);

			var ntiUcModel = new NtiUnderConsiderationModel(){ ClosedAt = DateTime.Now };
			
			mockNtiModelService
				.Setup(fp => fp.GetNtiUnderConsideration(ntiUcId))
				.ReturnsAsync(ntiUcModel);

			var routeData = pageModel.RouteData.Values;
			pageModel.CaseUrn = caseUrn;
			routeData.Add("ntiucid", ntiUcId);

			// act
			var response = await pageModel.OnGetAsync();

			// assert
			
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
			Mock<ILogger<EditPageModel>> mockLogger = new Mock<ILogger<EditPageModel>>();

			var pageModel = SetupAddPageModel(mockNtiModelService, mockLogger);

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnPostPage));
		}

		[Test]
		public async Task WhenOnPostAsync_ValidatesNotesLength_ThrowsException_ReturnsPage()
		{
			// arrange
			Mock<INtiUnderConsiderationModelService> mockNtiModelService = new Mock<INtiUnderConsiderationModelService>();
			Mock<ILogger<EditPageModel>> mockLogger = new Mock<ILogger<EditPageModel>>();

			var pageModel = SetupAddPageModel(mockNtiModelService, mockLogger);

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnPostPage));
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
			Mock<ILogger<EditPageModel>> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new EditPageModel(mockNtiModelService.Object, mockLogger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}

}