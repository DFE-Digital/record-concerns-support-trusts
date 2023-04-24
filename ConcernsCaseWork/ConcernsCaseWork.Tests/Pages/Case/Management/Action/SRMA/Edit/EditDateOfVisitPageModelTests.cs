﻿using AutoFixture;
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Case.Management.Action.SRMA.Edit;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.SRMA.Edit
{
	[Parallelizable(ParallelScope.All)]
	public class EditDateOfVisitPageModelTests
	{
		private Fixture _fixture;

		public EditDateOfVisitPageModelTests()
		{
			_fixture = new();
		}

		[Test]
		public async Task WhenOnGetAsync_ReturnsPage()
		{
			// arrange 
			var mockSRMAModelService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<EditDateOfVisitPageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

			mockSRMAModelService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupDateOfVisitPageModel(mockSRMAModelService.Object, mockLogger.Object);
			pageModel.CaseId = (int)srmaModel.CaseUrn;
			pageModel.SrmaId = (int)srmaModel.Id;

			// act
			var pageResponse = await pageModel.OnGetAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
		}
		
		[Test]
		public async Task WhenOnGetAsync_AndSrmaIsOpen_RedirectsToClosedPage()
		{
			// arrange 
			var mockSRMAModelService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<EditDateOfVisitPageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed, closedAt:DateTime.Now);

			mockSRMAModelService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupDateOfVisitPageModel(mockSRMAModelService.Object, mockLogger.Object);
			pageModel.CaseId = (int)srmaModel.CaseUrn;
			pageModel.SrmaId = (int)srmaModel.Id;

			// act
			var pageResponse = await pageModel.OnGetAsync();
			
			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;

			Assert.That(page?.Url, Is.EqualTo($"/case/{srmaModel.CaseUrn}/management/action/srma/{srmaModel.Id}/closed"));
			Assert.That(pageModel.TempData["Error.Message"], Is.Null);
		}

		[Test]
		public async Task WhenOnPostAsync_RouteData_RequestForm_Return_To_SRMA_Page()
		{
			// arrange 
			var mockSRMAModelService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<EditDateOfVisitPageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

			var pageModel = SetupDateOfVisitPageModel(mockSRMAModelService.Object, mockLogger.Object);
			pageModel.CaseId = (int)srmaModel.CaseUrn;
			pageModel.SrmaId = (int)srmaModel.Id;

			pageModel.StartDate.Date = new OptionalDateModel(new DateTime(2020, 1, 2));
			pageModel.EndDate.Date = new OptionalDateModel(new DateTime(2020, 3, 2));

			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(page.Url, Is.EqualTo($"/case/{srmaModel.CaseUrn}/management/action/srma/{srmaModel.Id}"));
		}

		private static EditDateOfVisitPageModel SetupDateOfVisitPageModel(
		ISRMAService mockSrmaModelService, ILogger<EditDateOfVisitPageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new EditDateOfVisitPageModel(mockSrmaModelService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}
