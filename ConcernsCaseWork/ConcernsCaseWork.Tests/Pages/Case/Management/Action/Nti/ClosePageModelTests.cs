using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Case.Management.Action.Nti;
using ConcernsCaseWork.Services.Nti;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.Nti
{
	[Parallelizable(ParallelScope.All)]
	public class ClosePageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_WithNtiId_Calls_API()
		{
			// arrange
			var ntiId = 123L;
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockLogger = new Mock<ILogger<ClosePageModel>>();

			var pageModel = SetupClosePageModel(mockNtiModelService, mockLogger);

			pageModel.RouteData.Values["urn"] = 1;
			pageModel.RouteData.Values["NtiId"] = ntiId;

			// act
			await pageModel.OnGetAsync();

			// assert
			mockNtiModelService.Verify(
					m => m.GetNtiByIdAsync(ntiId),
					Times.Once());
		}

		[Test]
		public async Task WhenOnPostAsync_CallsAPI_WithUpdatedValues()
		{
			// arrange
			var caseUrn = 111L;
			var ntiId = 123L;
			var closureNotes = "closure notes";
			var closedDate = new DateTime(2022, 03, 03);

			var mockNtiModelService = new Mock<INtiModelService>();
			var mockLogger = new Mock<ILogger<ClosePageModel>>();

			mockNtiModelService.Setup(ms => ms.GetNtiByIdAsync(ntiId)).Returns(Task.FromResult(new NtiModel { Id = ntiId }));

			var pageModel = SetupClosePageModel(mockNtiModelService, mockLogger);

			pageModel.RouteData.Values["urn"] = caseUrn;
			pageModel.RouteData.Values["NtiId"] = ntiId;

			pageModel.HttpContext.Request.Form = new FormCollection(
			new Dictionary<string, StringValues>
			{
					{ "nti-notes", new StringValues(closureNotes) },
					{ "dtr-day", new StringValues(closedDate.Day.ToString()) },
					{ "dtr-month", new StringValues(closedDate.Month.ToString()) },
					{ "dtr-year", new StringValues(closedDate.Year.ToString()) }
			});

			// act
			await pageModel.OnPostAsync();

			// assert
			mockNtiModelService.Verify(m => m.GetNtiByIdAsync(ntiId), Times.Once);

			mockNtiModelService.Verify(m => m.PatchNtiAsync(
				It.Is<NtiModel>(nti =>
					nti.Id == ntiId
					&& nti.DateNTIClosed.Value.Year == closedDate.Year
					&& nti.DateNTIClosed.Value.Month == closedDate.Month
					&& nti.DateNTIClosed.Value.Day == closedDate.Day
					&& string.Equals(nti.Notes, closureNotes, StringComparison.Ordinal)
					&& nti.ClosedStatusId == (int)NTIStatus.Closed
				)),
				Times.Once());
		}

		[Test]
		[TestCase("2022", "03", "04", true)]
		[TestCase("", "", "", true)]
		[TestCase("2022", "03", "31", true)]
		[TestCase("", "03", "04", false)]
		[TestCase("2022", "", "04", false)]
		[TestCase("2022", "04", "", false)]
		[TestCase("2022", "02", "30", false)]
		public async Task OnPostAsync_CorrectlyValidatesDates(string year, string month, string day, bool isValid)
		{
			// arrange
			var caseUrn = 111L;
			var ntiId = 123L;
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockLogger = new Mock<ILogger<ClosePageModel>>();

			mockNtiModelService.Setup(ms => ms.GetNtiByIdAsync(ntiId)).Returns(Task.FromResult(new NtiModel { Id = ntiId }));

			var pageModel = SetupClosePageModel(mockNtiModelService, mockLogger);

			pageModel.RouteData.Values["urn"] = caseUrn;
			pageModel.RouteData.Values["NtiId"] = ntiId;

			pageModel.HttpContext.Request.Form = new FormCollection(
			new Dictionary<string, StringValues>
			{
					{ "dtr-day", new StringValues(day) },
					{ "dtr-month", new StringValues(month) },
					{ "dtr-year", new StringValues(year) }
			});

			// act
			await pageModel.OnPostAsync();

			// assert
			if (isValid)
			{
				Assert.That(pageModel.TempData["Error.Message"], Is.Null);
			}
			else
			{
				Assert.That(pageModel.TempData["Error.Message"], Is.Not.Null);
			}
		}

		[Test]
		[TestCase("")]
		[TestCase("Valid notes")]
		public async Task OnPostAsync_ValidNotes_AcceptedWithoutError(string validNotes)
		{
			// arrange
			var caseUrn = 111L;
			var ntiId = 123L;
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockLogger = new Mock<ILogger<ClosePageModel>>();

			mockNtiModelService.Setup(ms => ms.GetNtiByIdAsync(ntiId)).Returns(Task.FromResult(new NtiModel { Id = ntiId }));

			var pageModel = SetupClosePageModel(mockNtiModelService, mockLogger);

			pageModel.RouteData.Values["urn"] = caseUrn;
			pageModel.RouteData.Values["NtiId"] = ntiId;

			pageModel.HttpContext.Request.Form = new FormCollection(
			new Dictionary<string, StringValues>
			{
				{ "nti-notes", new StringValues(validNotes) }
			});

			// act
			await pageModel.OnPostAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.Null);
		}


		[Test]
		public async Task OnPostAsync_ValidNotes_AcceptedWithoutError()
		{
			// arrange
			var caseUrn = 111L;
			var ntiId = 123L;
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockLogger = new Mock<ILogger<ClosePageModel>>();

			mockNtiModelService.Setup(ms => ms.GetNtiByIdAsync(ntiId)).Returns(Task.FromResult(new NtiModel { Id = ntiId }));

			var pageModel = SetupClosePageModel(mockNtiModelService, mockLogger);

			pageModel.RouteData.Values["urn"] = caseUrn;
			pageModel.RouteData.Values["NtiId"] = ntiId;

			var validNotes = GenereateString(pageModel.NotesMaxLength);

			pageModel.HttpContext.Request.Form = new FormCollection(
			new Dictionary<string, StringValues>
			{
				{ "nti-notes", new StringValues(validNotes) }
			});

			// act
			await pageModel.OnPostAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.Null);
		}


		[Test]
		public async Task OnPostAsync_InValidNotes_ValidationError()
		{
			// arrange
			var caseUrn = 111L;
			var ntiId = 123L;
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockLogger = new Mock<ILogger<ClosePageModel>>();

			mockNtiModelService.Setup(ms => ms.GetNtiByIdAsync(ntiId)).Returns(Task.FromResult(new NtiModel { Id = ntiId }));

			var pageModel = SetupClosePageModel(mockNtiModelService, mockLogger);

			pageModel.RouteData.Values["urn"] = caseUrn;
			pageModel.RouteData.Values["NtiId"] = ntiId;

			var invalidNotes = GenereateString(pageModel.NotesMaxLength + 1);

			pageModel.HttpContext.Request.Form = new FormCollection(
			new Dictionary<string, StringValues>
			{
				{ "nti-notes", new StringValues(invalidNotes) }
			});

			// act
			await pageModel.OnPostAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.Not.Null);
		}

		private static ClosePageModel SetupClosePageModel(Mock<INtiModelService> mockNtiModelService,
			Mock<ILogger<ClosePageModel>> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new ClosePageModel(mockNtiModelService.Object, mockLogger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}

		private string GenereateString(int length)
		{
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
	}


}