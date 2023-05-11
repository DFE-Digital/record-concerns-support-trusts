using ConcernsCaseWork.API.Contracts.Constants;
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Models.Validatable;
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
using Microsoft.Graph.Models.TermStore;
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
			var ntiId = 123;
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockLogger = new Mock<ILogger<ClosePageModel>>();

			var pageModel = SetupClosePageModel(mockNtiModelService, mockLogger);

			pageModel.CaseUrn = 1;
			pageModel.NtiId = ntiId;

			// act
			await pageModel.OnGetAsync();

			// assert
			mockNtiModelService.Verify(
					m => m.GetNtiByIdAsync(ntiId),
					Times.Once());
		}
		
		[Test]
		public async Task WhenOnGetAsync_WhenNtiIsClosed_RedirectsToClosedPage()
		{
			// arrange
			var ntiId = 123;
			var caseUrn = 765;
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockLogger = new Mock<ILogger<ClosePageModel>>();

			var pageModel = SetupClosePageModel(mockNtiModelService, mockLogger);
			
			var ntiModel = NTIFactory.BuildClosedNTIModel();
			mockNtiModelService.Setup(n => n.GetNtiByIdAsync(It.IsAny<long>()))
				.ReturnsAsync(ntiModel);

			pageModel.CaseUrn = caseUrn;
			pageModel.NtiId = ntiId;

			// act
			var response = await pageModel.OnGetAsync();

			// assert
			mockNtiModelService.Verify(
				m => m.GetNtiByIdAsync(ntiId),
				Times.Once());
			
			Assert.Multiple(() =>
			{
				Assert.That(response, Is.InstanceOf<RedirectResult>());
				Assert.That(((RedirectResult)response).Url, Is.EqualTo($"/case/{caseUrn}/management/action/nti/{ntiId}"));
				Assert.That(pageModel.TempData["Error.Message"], Is.Null);
			});
		}

		[Test]
		public async Task WhenOnPostAsync_CallsAPI_WithUpdatedValues()
		{
			// arrange
			var caseUrn = 111;
			var ntiId = 123;
			var closureNotes = "closure notes";
			var closedDate = new DateTime(2022, 03, 03);

			var mockNtiModelService = new Mock<INtiModelService>();
			var mockLogger = new Mock<ILogger<ClosePageModel>>();

			mockNtiModelService.Setup(ms => ms.GetNtiByIdAsync(ntiId)).Returns(Task.FromResult(new NtiModel { Id = ntiId }));

			var pageModel = SetupClosePageModel(mockNtiModelService, mockLogger);

			pageModel.CaseUrn = caseUrn;
			pageModel.NtiId = ntiId;
			pageModel.Notes = new TextAreaUiComponent("", "", "") { Text = new ValidateableString() { StringContents = closureNotes } };
			pageModel.DateNTIClosed  = new OptionalDateTimeUiComponent("", "", "") { Date = new OptionalDateModel(closedDate) };



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