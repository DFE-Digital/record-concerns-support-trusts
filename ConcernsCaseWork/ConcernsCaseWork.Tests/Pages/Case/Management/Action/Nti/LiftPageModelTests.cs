using AutoFixture;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.CoreTypes;
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
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.Nti
{
	[Parallelizable(ParallelScope.All)]
	public class LiftPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockLogger = new Mock<ILogger<LiftPageModel>>();

			var ntiModel = NTIFactory.BuildNTIModel();
			mockNtiModelService.Setup(n => n.GetNtiByIdAsync(It.IsAny<long>()))
				.ReturnsAsync(ntiModel);

			var pageModel = SetupCancelPageModel(mockNtiModelService, mockLogger);

			pageModel.CaseUrn = 1;
			pageModel.NtiId = 1;

			// act
			await pageModel.OnGetAsync();

			// assert
			mockNtiModelService.Verify(n => n.GetNtiByIdAsync(It.IsAny<long>()), Times.Once);
		}
		
		[Test]
		public async Task WhenOnGetAsync_WhenNtiIsClosed_RedirectsToClosedPage()
		{
			// arrange
			var caseUrn = 482;
			var ntiId = 293;
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockLogger = new Mock<ILogger<LiftPageModel>>();

			var ntiModel = NTIFactory.BuildClosedNTIModel();
			mockNtiModelService.Setup(n => n.GetNtiByIdAsync(It.IsAny<long>()))
				.ReturnsAsync(ntiModel);

			var pageModel = SetupCancelPageModel(mockNtiModelService, mockLogger);

			pageModel.CaseUrn = caseUrn;
			pageModel.NtiId = ntiId;

			// act
			var response = await pageModel.OnGetAsync();

			// assert
			mockNtiModelService.Verify(n => n.GetNtiByIdAsync(It.IsAny<long>()), Times.Once);

			Assert.Multiple(() =>
			{
				Assert.That(response, Is.InstanceOf<RedirectResult>());
				Assert.That(((RedirectResult)response).Url, Is.EqualTo($"/case/{caseUrn}/management/action/nti/{ntiId}"));
				Assert.That(pageModel.TempData["Error.Message"], Is.Null);
			});
		}


		[Test]
		public async Task WhenOnPostAsync_ReturnsPage()
		{
			// arrange
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockLogger = new Mock<ILogger<LiftPageModel>>();

			var ntiModel = NTIFactory.BuildNTIModel();
			mockNtiModelService.Setup(n => n.GetNtiByIdAsync(It.IsAny<long>()))
				.ReturnsAsync(ntiModel);

			var pageModel = SetupCancelPageModel(mockNtiModelService, mockLogger);

			pageModel.CaseUrn = 1;
			pageModel.NtiId = 1;

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "nti-notes", new StringValues("valid string") }
				});

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;

			Assert.That(page, Is.Not.Null);
			mockNtiModelService.Verify(n => n.PatchNtiAsync(It.IsAny<NtiModel>()), Times.Once);
		}




		private static LiftPageModel SetupCancelPageModel(
			Mock<INtiModelService> mockNtiModelService,
			Mock<ILogger<LiftPageModel>> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new LiftPageModel(mockNtiModelService.Object, mockLogger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}

}