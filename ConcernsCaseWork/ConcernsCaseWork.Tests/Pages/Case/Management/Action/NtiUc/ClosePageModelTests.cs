using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Case.Management.Action.NtiUnderConsideration;
using ConcernsCaseWork.Redis.NtiUnderConsideration;
using ConcernsCaseWork.Service.NtiUnderConsideration;
using ConcernsCaseWork.Services.NtiUnderConsideration;
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

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.NtiUc
{
	[Parallelizable(ParallelScope.All)]
	public class ClosePageModelTests
	{
		[Test]
		[TestCase("1", "")]
		[TestCase("", "2")]
		[TestCase("", "")]
		public async Task WhenOnGetAsync_EmptyRouteData_ThrowsException_ReturnPage(string urn, string ntiUcId)
		{
			// arrange
			var mockNtiModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockNtiStatusesCachedService = new Mock<INtiUnderConsiderationStatusesCachedService>();
			var mockLogger = new Mock<ILogger<ClosePageModel>>();

			var pageModel = SetupClosePageModel(mockNtiModelService, mockNtiStatusesCachedService, mockLogger);
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", urn);
			routeData.Add("ntiUCId", ntiUcId);

			// act
			var pageResponse = await pageModel.OnGetAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());

			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnGetPage));
			Assert.That(pageModel.NtiModel, Is.Null);
			Assert.That(pageModel.NTIStatuses, Is.Null);
		}
		
		[Test]
		public async Task WhenOnGetAsync_MissingRouteData_ThrowsException_ReturnPage()
		{
			// arrange
			var mockNtiModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockNtiStatusesCachedService = new Mock<INtiUnderConsiderationStatusesCachedService>();
			var mockLogger = new Mock<ILogger<ClosePageModel>>();

			var pageModel = SetupClosePageModel(mockNtiModelService, mockNtiStatusesCachedService, mockLogger);

			// act
			var pageResponse = await pageModel.OnGetAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());

			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnGetPage));
			Assert.That(pageModel.NtiModel, Is.Null);
			Assert.That(pageModel.NTIStatuses, Is.Null);
		}
		
		[Test]
		public async Task WhenOnPostAsync_MissingRouteData_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockNtiModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockNtiStatusesCachedService = new Mock<INtiUnderConsiderationStatusesCachedService>();
			var mockLogger = new Mock<ILogger<ClosePageModel>>();

			var pageModel = SetupClosePageModel(mockNtiModelService, mockNtiStatusesCachedService, mockLogger);

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());

			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnPostPage));
			Assert.That(pageModel.NtiModel, Is.Null);
			Assert.That(pageModel.NTIStatuses, Is.Null);
		}
		
		[Test]
		[TestCase("1", "")]
		[TestCase("", "2")]
		[TestCase("", "")]
		public async Task WhenOnPostAsync_EmptyRouteData_ThrowsException_ReturnPage(string urn, string ntiUcId)
		{
			// arrange
			var mockNtiModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockNtiStatusesCachedService = new Mock<INtiUnderConsiderationStatusesCachedService>();
			var mockLogger = new Mock<ILogger<ClosePageModel>>();

			var pageModel = SetupClosePageModel(mockNtiModelService, mockNtiStatusesCachedService, mockLogger);
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", urn);
			routeData.Add("ntiUCId", ntiUcId);

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());

			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnPostPage));
			Assert.That(pageModel.NtiModel, Is.Null);
			Assert.That(pageModel.NTIStatuses, Is.Null);
		}

		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			Mock<INtiUnderConsiderationModelService> mockNtiModelService = new Mock<INtiUnderConsiderationModelService>();
			Mock<INtiUnderConsiderationStatusesCachedService> mockNtiStatusesCachedService = new Mock<INtiUnderConsiderationStatusesCachedService>();
			Mock<ILogger<ClosePageModel>> mockLogger = new Mock<ILogger<ClosePageModel>>();

			var caseUrn = 3;
			var ntiId = 4;

			var validStatuses = GetListValidStatuses();
			var pageModel = SetupClosePageModel(mockNtiModelService, mockNtiStatusesCachedService, mockLogger);
			var ntiModel = SetupOpenNtiUnderConsiderationModel(ntiId, caseUrn);

			mockNtiStatusesCachedService
				.Setup(fp => fp.GetAllStatuses())
				.ReturnsAsync(validStatuses);
				
			mockNtiModelService
				.Setup(fp => fp.GetNtiUnderConsideration(ntiId))
				.ReturnsAsync(ntiModel);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseUrn);
			routeData.Add("ntiUCId", ntiId);

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "status", validStatuses.First().Id.ToString()},
					{ "nti-notes", "Some notes"}
				});

			// act
			var pageResponse = await pageModel.OnGetAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Empty);
			
			Assert.That(pageModel.NtiModel, Is.EqualTo(ntiModel));
			
			Assert.That(pageModel.NTIStatuses.Distinct().Count(), Is.EqualTo(validStatuses.Count));
			Assert.Contains(validStatuses.First().Id.ToString(), pageModel.NTIStatuses.Select(s => s.Id).ToList());
			Assert.Contains(validStatuses.Last().Id.ToString(), pageModel.NTIStatuses.Select(s => s.Id).ToList());
			
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::NTI-UC::ClosePageModel::OnGetAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}
		
		[Test]
		public async Task WhenOnGetAsync_WhenNtiUcIsClosed_RedirectsToIndexPage()
		{
			// arrange
			Mock<INtiUnderConsiderationModelService> mockNtiModelService = new Mock<INtiUnderConsiderationModelService>();
			Mock<INtiUnderConsiderationStatusesCachedService> mockNtiStatusesCachedService = new Mock<INtiUnderConsiderationStatusesCachedService>();
			Mock<ILogger<ClosePageModel>> mockLogger = new Mock<ILogger<ClosePageModel>>();

			var caseUrn = 3;
			var ntiId = 4;

			var validStatuses = GetListValidStatuses();
			var pageModel = SetupClosePageModel(mockNtiModelService, mockNtiStatusesCachedService, mockLogger);
			var ntiModel = SetupClosedNtiUnderConsiderationModel(ntiId, caseUrn);

			mockNtiStatusesCachedService
				.Setup(fp => fp.GetAllStatuses())
				.ReturnsAsync(validStatuses);
				
			mockNtiModelService
				.Setup(fp => fp.GetNtiUnderConsideration(ntiId))
				.ReturnsAsync(ntiModel);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseUrn);
			routeData.Add("ntiUCId", ntiId);

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "status", validStatuses.First().Id.ToString()},
					{ "nti-notes", "Some notes"}
				});

			// act
			var pageResponse = await pageModel.OnGetAsync();

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
				Assert.That(((RedirectResult)pageResponse).Url, Is.EqualTo($"/case/{caseUrn}/management/action/ntiunderconsideration/{ntiId}"));
				Assert.That(pageModel.TempData["Error.Message"], Is.Null);
			});
			
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::NTI-UC::ClosePageModel::OnGetAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}
		
		[Test]
		public async Task WhenOnPostAsync_WithEmptyStatus_ReturnsError()
		{
			// arrange
			var mockNtiModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockNtiStatusesCachedService = new Mock<INtiUnderConsiderationStatusesCachedService>();
			var mockLogger = new Mock<ILogger<ClosePageModel>>();

			var caseUrn = 3;
			var ntiId = 4;

			var validStatuses = GetListValidStatuses();
			var pageModel = SetupClosePageModel(mockNtiModelService, mockNtiStatusesCachedService, mockLogger);
			var ntiModel = SetupOpenNtiUnderConsiderationModel(ntiId, caseUrn);

			mockNtiStatusesCachedService
				.Setup(fp => fp.GetAllStatuses())
				.ReturnsAsync(validStatuses);
				
			mockNtiModelService
				.Setup(fp => fp.GetNtiUnderConsideration(ntiId))
				.ReturnsAsync(ntiModel);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseUrn);
			routeData.Add("ntiUCId", ntiId);

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "status", ""}
				});

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["NTI-UC.Message"], Is.EqualTo("Please select a reason for closing NTI under consideration"));
			
			Assert.That(pageModel.NtiModel, Is.EqualTo(ntiModel));
			
			Assert.That(pageModel.NTIStatuses.Distinct().Count(), Is.EqualTo(validStatuses.Count));
			Assert.Contains(validStatuses.First().Id.ToString(), pageModel.NTIStatuses.Select(s => s.Id).ToList());
			Assert.Contains(validStatuses.Last().Id.ToString(), pageModel.NTIStatuses.Select(s => s.Id).ToList());
				
			mockNtiModelService.Verify(f => f.PatchNtiUnderConsideration(It.IsAny<NtiUnderConsiderationModel>()), Times.Never);
		}
		
		[Test]
		[TestCase(2001)]
		[TestCase(2050)]
		public async Task WhenOnPostAsync_WithNotesLengthTooLong_ReturnsError(int notesLength)
		{
			// arrange
			var mockNtiModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockNtiStatusesCachedService = new Mock<INtiUnderConsiderationStatusesCachedService>();
			var mockLogger = new Mock<ILogger<ClosePageModel>>();

			var caseUrn = 3;
			var ntiId = 4;
			var notes = new string('*', notesLength);

			var validStatuses = GetListValidStatuses();
			var pageModel = SetupClosePageModel(mockNtiModelService, mockNtiStatusesCachedService, mockLogger);
			var ntiModel = SetupOpenNtiUnderConsiderationModel(ntiId, caseUrn);

			mockNtiStatusesCachedService
				.Setup(fp => fp.GetAllStatuses())
				.ReturnsAsync(validStatuses);
				
			mockNtiModelService
				.Setup(fp => fp.GetNtiUnderConsideration(ntiId))
				.ReturnsAsync(ntiModel);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseUrn);
			routeData.Add("ntiUCId", ntiId);

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "status", validStatuses.First().Id.ToString()},
					{ "nti-notes", notes}
				});

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["NTI-UC.Message"], Is.EqualTo("Notes provided exceed maximum allowed length (2000 characters)."));
			
			Assert.That(pageModel.NtiModel, Is.EqualTo(ntiModel));
			
			Assert.That(pageModel.NTIStatuses.Distinct().Count(), Is.EqualTo(validStatuses.Count));
			Assert.Contains(validStatuses.First().Id.ToString(), pageModel.NTIStatuses.Select(s => s.Id).ToList());
			Assert.Contains(validStatuses.Last().Id.ToString(), pageModel.NTIStatuses.Select(s => s.Id).ToList());
				
			mockNtiModelService.Verify(f => f.PatchNtiUnderConsideration(It.IsAny<NtiUnderConsiderationModel>()), Times.Never);
		}
		
		[Test]
		public async Task WhenOnPostAsync_WithEmptyNotes_DoesNotError()
		{
			// arrange
			var mockNtiModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockNtiStatusesCachedService = new Mock<INtiUnderConsiderationStatusesCachedService>();
			var mockLogger = new Mock<ILogger<ClosePageModel>>();

			var caseUrn = 3;
			var ntiId = 4;
			var notes = string.Empty;

			var validStatuses = GetListValidStatuses();
			var pageModel = SetupClosePageModel(mockNtiModelService, mockNtiStatusesCachedService, mockLogger);
			var ntiModel = SetupOpenNtiUnderConsiderationModel(ntiId, caseUrn);

			mockNtiStatusesCachedService
				.Setup(fp => fp.GetAllStatuses())
				.ReturnsAsync(validStatuses);
				
			mockNtiModelService
				.Setup(fp => fp.GetNtiUnderConsideration(ntiId))
				.ReturnsAsync(ntiModel);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseUrn);
			routeData.Add("ntiUCId", ntiId);

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "status", validStatuses.First().Id.ToString()},
					{ "nti-notes", notes}
				});

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			Assert.That(pageResponse, Is.Not.Null);
			Assert.That((pageResponse as RedirectResult)?.Url, Is.EqualTo($"/case/{caseUrn}/management"));
		}

		private static ClosePageModel SetupClosePageModel(
			Mock<INtiUnderConsiderationModelService> mockNtiModelService,
			Mock<INtiUnderConsiderationStatusesCachedService> mockNtiStatusesCachedService,
			Mock<ILogger<ClosePageModel>> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new ClosePageModel(mockNtiModelService.Object, mockNtiStatusesCachedService.Object, mockLogger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}

		private static NtiUnderConsiderationModel SetupOpenNtiUnderConsiderationModel(int id, long caseUrn)
			=> new() { Id = id, CaseUrn = caseUrn };
		
		private static NtiUnderConsiderationModel SetupClosedNtiUnderConsiderationModel(int id, long caseUrn)
			=> new() { Id = id, CaseUrn = caseUrn, ClosedAt = DateTime.Now};

		private static List<NtiUnderConsiderationStatusDto> GetListValidStatuses() => new List<NtiUnderConsiderationStatusDto>
		{
			new ()
			{
				Id = 1,
				Name = "Some status"
			},
			new ()
			{
				Id = 2,
				Name = "Another status"
			}
		};
	}

}