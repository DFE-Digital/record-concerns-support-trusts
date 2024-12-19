using AutoFixture;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case.Management.Action.NtiUnderConsideration;
using ConcernsCaseWork.Services.NtiUnderConsideration;
using ConcernsCaseWork.Shared.Tests.Factory;
using ConcernsCaseWork.Shared.Tests.MockHelpers;
using FluentAssertions;
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
	public class AddPageModelTests
	{
		private readonly IFixture _fixture;


		public AddPageModelTests()
		{
			_fixture = new Fixture();
		}

		[Test]
		public void WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			Mock<INtiUnderConsiderationModelService> mockNtiModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();
			var caseUrn = 1;
			var pageModel = SetupAddPageModel(mockNtiModelService, mockLogger);
			var routeData = pageModel.RouteData.Values;
			pageModel.CaseUrn = caseUrn;

			// act
			var result = pageModel.OnGet();

			// assert
			result.Should().BeAssignableTo<PageResult>();
			mockLogger.VerifyLogErrorWasNotCalled();
				
		}

		[Test]
		public async Task WhenOnPostAsync_MissingRouteData_ThrowsException_ReturnsPage()
		{
			// arrange
			Mock<INtiUnderConsiderationModelService> mockNtiModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();
			var pageModel = SetupAddPageModel(mockNtiModelService, mockLogger);
			pageModel.Notes = _fixture.Create<TextAreaUiComponent>();

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
		[TestCase(0, true)]
		[TestCase(1, true)]
		[TestCase(AddPageModel.NotesMaxLength, true)]
		[TestCase(AddPageModel.NotesMaxLength + 1, false)]
		public async Task WhenOnPostAsync_ValidateNotes(int notesLength, bool isValid)
		{
			// arrange
			Mock<INtiUnderConsiderationModelService> mockNtiModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();
			var caseUrn = 1;
			var pageModel = SetupAddPageModel(mockNtiModelService, mockLogger);
			pageModel.CaseUrn = caseUrn;
			pageModel.Notes = _fixture.Create<TextAreaUiComponent>();
			var routeData = pageModel.RouteData.Values;
			pageModel.Notes.Text.StringContents = GenereateString(notesLength);
			pageModel.NTIReasonsToConsider = new List<RadioItem>()
			{
				new RadioItem()
				{
					Text = "Test",
					Description = "Test",
					Id = "1",
					IsChecked = true
					
				}
			};
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "reason", pageModel.NTIReasonsToConsider.First().Id},
					{ "nti-notes", pageModel.Notes.Text.StringContents}
				});
			

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse,Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;
			Assert.That(pageModel.TempData, Is.Empty);
			Assert.That(page, Is.Not.Null);
			Assert.That(page.Url, Is.EqualTo($"/case/1/management"));
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

		private static AddPageModel SetupAddPageModel(
			Mock<INtiUnderConsiderationModelService> mockNtiModelService,
			Mock<ILogger<AddPageModel>> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new AddPageModel(mockNtiModelService.Object, mockLogger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}

}