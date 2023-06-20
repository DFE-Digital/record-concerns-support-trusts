using ConcernsCaseWork.Services.Nti;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConcernsCaseWork.Pages.Case.Management.Action.Nti;
using Microsoft.AspNetCore.Mvc.Routing;
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Models;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.Nti
{
	[Parallelizable(ParallelScope.All)]
	internal class DeletePageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_WithNtiId_Calls_API()
		{
			// arrange
			var ntiId = 123;
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockLogger = new Mock<ILogger<DeletePageModel>>();

			var pageModel = SetupPageModel(mockNtiModelService, mockLogger);

			pageModel.CaseUrn = 1;
			pageModel.NtiId = ntiId;

			// act
			await pageModel.OnGet();

			// assert
			mockNtiModelService.Verify(
					m => m.GetNtiByIdAsync(ntiId),
					Times.Once());
		}

		[Test]
		public async Task WhenOnPostAsync_CallsAPI()
		{
			// arrange
			var caseUrn = 111;
			var ntiId = 123;

			var mockNtiModelService = new Mock<INtiModelService>();
			var mockLogger = new Mock<ILogger<DeletePageModel>>();

			mockNtiModelService.Setup(ms => ms.GetNtiByIdAsync(ntiId)).Returns(Task.FromResult(new NtiModel { Id = ntiId }));

			var pageModel = SetupPageModel(mockNtiModelService, mockLogger);

			pageModel.CaseUrn = caseUrn;
			pageModel.NtiId = ntiId;

			// act
			await pageModel.OnPostAsync();

			// assert
			mockNtiModelService.Verify(m => m.DeleteAsync(ntiId), Times.Once);
		}


		private static DeletePageModel SetupPageModel(Mock<INtiModelService> mockNtiModelService,
			Mock<ILogger<DeletePageModel>> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new DeletePageModel(mockNtiModelService.Object, mockLogger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}
