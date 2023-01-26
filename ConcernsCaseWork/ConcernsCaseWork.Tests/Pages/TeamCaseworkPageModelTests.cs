using AutoFixture;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.Teams;
using ConcernsCaseWork.Pages;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Shared.Tests.Factory;
using ConcernsCaseWork.Shared.Tests.MockHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using ITeamsModelService = ConcernsCaseWork.Services.Teams.ITeamsModelService;

namespace ConcernsCaseWork.Tests.Pages
{
	[Parallelizable(ParallelScope.All)]
	public class TeamCaseworkPageModelTests
	{
		private readonly IFixture _fixture = new Fixture();


		[Test]
		public async Task WhenOnGetAsync_ThrowsException_ReturnsPage()
		{
			// arrange
			var currentUserName = _fixture.Create<string>();

			var mockLogger = new Mock<ILogger<TeamCaseworkPageModel>>();
			var mockUserStateCache = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			var mockCaseSummaryService = new Mock<ICaseSummaryService>();
			var mockTeamService = new Mock<ITeamsModelService>();

			var activeTeamCases = _fixture.CreateMany<ActiveCaseSummaryModel>().ToList();
			var teamUserNames = activeTeamCases.Select(t => t.CreatedBy).Distinct().ToArray();

			mockClaimsPrincipalHelper.Setup(s => s.GetPrincipalName(It.IsAny<IPrincipal>())).Returns(currentUserName);
			mockTeamService.Setup(x => x.GetCaseworkTeam(currentUserName))
				.ReturnsAsync(new ConcernsTeamCaseworkModel(currentUserName, teamUserNames));

			mockCaseSummaryService.Setup(s => s.GetActiveCaseSummariesForTeamMembers(currentUserName)).Throws(new Exception("Bad request"));
			
			var sut = SetupTeamCaseworkPageModel(mockLogger.Object, mockTeamService.Object, mockUserStateCache.Object, mockCaseSummaryService.Object, mockClaimsPrincipalHelper.Object);

			// act
			await sut.OnGetAsync();

			Assert.That(sut.TempData, Is.Not.Null);
			Assert.That(sut.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnGetPage));
		}

		[Test]
		public async Task WhenInstanceOfHomePageOnGetAsync_ReturnCases()
		{
			// arrange
			var currentUserName = _fixture.Create<string>();
			
			var mockLogger = new Mock<ILogger<TeamCaseworkPageModel>>();
			var mockUserStateCache = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			var mockCaseSummaryService = new Mock<ICaseSummaryService>();
			var mockTeamService = new Mock<ITeamsModelService>();

			var activeTeamCases = _fixture.CreateMany<ActiveCaseSummaryModel>().ToList();
			var teamUserNames = activeTeamCases.Select(t => t.CreatedBy).Distinct().ToArray();
			
			mockCaseSummaryService.Setup(s => s.GetActiveCaseSummariesForTeamMembers(currentUserName)).ReturnsAsync(activeTeamCases);
			mockClaimsPrincipalHelper.Setup(s => s.GetPrincipalName(It.IsAny<IPrincipal>())).Returns(currentUserName);
			mockTeamService.Setup(x => x.GetCaseworkTeam(currentUserName))
				.ReturnsAsync(new ConcernsTeamCaseworkModel(currentUserName, teamUserNames));
		
			var sut = SetupTeamCaseworkPageModel(mockLogger.Object, mockTeamService.Object, mockUserStateCache.Object, mockCaseSummaryService.Object, mockClaimsPrincipalHelper.Object);
		
			// act
			await sut.OnGetAsync();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.ActiveTeamCases, Is.EquivalentTo(activeTeamCases));
			});

			mockLogger.VerifyLogInformationWasCalled("TeamCaseworkPageModel");
			mockLogger.VerifyLogErrorWasNotCalled();
		
			mockTeamService.Verify(x => x.GetCaseworkTeam(It.IsAny<string>()), Times.Once);
		}

		[Test]
		public async Task WhenInstanceOfHomePageOnGetAsync_ReturnEmptyCases()
		{
			// arrange
			var mockLogger = new Mock<ILogger<TeamCaseworkPageModel>>();
			var mockUserStateCache = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			var mockCaseSummaryService = new Mock<ICaseSummaryService>();

			mockCaseSummaryService
				.Setup(model => model.GetActiveCaseSummariesForTeamMembers(It.IsAny<string>()))
				.ReturnsAsync(new List<ActiveCaseSummaryModel>());

			var mockTeamService = new Mock<ITeamsModelService>();
			mockTeamService.Setup(x => x.GetCaseworkTeam(It.IsAny<string>()))
				.ReturnsAsync(new ConcernsTeamCaseworkModel("random.user", Array.Empty<string>()));

			// act
			var sut = SetupTeamCaseworkPageModel(mockLogger.Object, mockTeamService.Object, mockUserStateCache.Object, mockCaseSummaryService.Object, mockClaimsPrincipalHelper.Object);
			await sut.OnGetAsync();

			// assert
			Assert.IsAssignableFrom<List<ActiveCaseSummaryModel>>(sut.ActiveTeamCases);
			Assert.That(sut.ActiveTeamCases, Is.Empty);

			// Not sure that these verifications should take place. it leads to a brittle test.
			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("TeamCaseworkPageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);

			mockTeamService.Verify(x => x.GetCaseworkTeam(It.IsAny<string>()), Times.Once);
		}

		[Test]
		public async Task OnGet_When_No_UserState_In_Cache_Records_User_Signin()
		{
			// arrange
			var testBuilder = new TestBuilder()
				.WithAuthenticatedUser()
				.WithNoCachedUserState()
				.WithNoTeamCaseworkModel();

			// act
			var sut = testBuilder.CreateSut();
			await sut.OnGetAsync();

			// assert
			testBuilder.MockUserStateCache.Verify(x => x.GetData(testBuilder.CaseworkerId), Times.Once);
			testBuilder.MockTeamService.Verify(x => x.GetCaseworkTeam(testBuilder.CaseworkerId), Times.Once);
			testBuilder.MockTeamService.Verify(x => x.UpdateCaseworkTeam(It.Is<ConcernsTeamCaseworkModel>(m => m.OwnerId == testBuilder.CaseworkerId && m.TeamMembers.Length == 0)), Times.Once);
			testBuilder.MockUserStateCache.Verify(x => x.StoreData(testBuilder.CaseworkerId, It.Is<UserState>(s => s.UserName == testBuilder.CaseworkerId)), Times.Once);
		}

		[Test]
		public async Task OnGet_When_UserState_In_Cache_Does_Not_Record_User_Signin()
		{
			// arrange
			var testBuilder = new TestBuilder()
				.WithAuthenticatedUser()
				.WithCachedUserState()
				.WithNoTeamCaseworkModel();

			// act
			var sut = testBuilder.CreateSut();
			await sut.OnGetAsync();

			// assert
			testBuilder.MockUserStateCache.Verify(x => x.GetData(testBuilder.CaseworkerId), Times.Once);
			testBuilder.MockTeamService.Verify(x => x.GetCaseworkTeam(testBuilder.CaseworkerId), Times.Once);
			testBuilder.MockTeamService.Verify(x => x.UpdateCaseworkTeam(It.Is<ConcernsTeamCaseworkModel>(m => m.OwnerId == testBuilder.CaseworkerId && m.TeamMembers.Length == 0)), Times.Never);
			testBuilder.MockUserStateCache.Verify(x => x.StoreData(testBuilder.CaseworkerId, It.Is<UserState>(s => s.UserName == testBuilder.CaseworkerId)), Times.Never);
		}
		
		private class TestBuilder
		{
			public TestBuilder()
			{
				this.Fixture = new Fixture();
				CaseworkerId = this.Fixture.Create<string>();
				MockLogger = new Mock<ILogger<TeamCaseworkPageModel>>();
				MockUserStateCache = new Mock<IUserStateCachedService>();
				MockTeamService = new Mock<ITeamsModelService>();
				MockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
				MockCaseSummaryService = new Mock<ICaseSummaryService>();
				IsAuthenticated = false;
			}

			public string CaseworkerId { get; set; }

			public Fixture Fixture { get; set; }

			public Mock<ITeamsModelService> MockTeamService { get; set; }

			public bool IsAuthenticated { get; set; }

			public TeamCaseworkPageModel CreateSut()
			{
				(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(IsAuthenticated);

				return new TeamCaseworkPageModel(MockLogger.Object, MockTeamService.Object, MockCaseSummaryService.Object, MockUserStateCache.Object, MockClaimsPrincipalHelper.Object)
				{
					PageContext = pageContext,
					TempData = tempData,
					Url = new UrlHelper(actionContext),
					MetadataProvider = pageContext.ViewData.ModelMetadata
				};
			}

			public Mock<IClaimsPrincipalHelper> MockClaimsPrincipalHelper { get; set; }

			public Mock<IUserStateCachedService> MockUserStateCache { get; set; }

			public Mock<ILogger<TeamCaseworkPageModel>> MockLogger { get; set; }
			
			public Mock<ICaseSummaryService> MockCaseSummaryService { get; set; }

			public TestBuilder WithNoTeamCaseworkModel()
			{
				this.MockTeamService.Setup(x => x.GetCaseworkTeam(CaseworkerId))
					.ReturnsAsync(new ConcernsTeamCaseworkModel(CaseworkerId, Array.Empty<string>()));

				return this;
			}

			public TestBuilder WithNoCachedUserState()
			{
				MockUserStateCache.Setup(x => x.GetData(CaseworkerId)).ReturnsAsync(default(UserState));
				return this;
			}

			public TestBuilder WithAuthenticatedUser()
			{
				MockClaimsPrincipalHelper.Setup(x => x.GetPrincipalName(It.IsAny<ClaimsPrincipal>())).Returns(CaseworkerId);
				IsAuthenticated = true;
				return this;
			}

			public TestBuilder WithCachedUserState()
			{
				MockUserStateCache.Setup(x => x.GetData(CaseworkerId)).ReturnsAsync(new UserState(CaseworkerId));
				return this;
			}
		}

		private static TeamCaseworkPageModel SetupTeamCaseworkPageModel(
			ILogger<TeamCaseworkPageModel> mockLogger,
			ITeamsModelService mockTeamService,
			IUserStateCachedService mockUserStateCachedService,
			ICaseSummaryService mockCaseSummaryService,
			IClaimsPrincipalHelper mockClaimsPrincipalHelper,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new TeamCaseworkPageModel(mockLogger, mockTeamService, mockCaseSummaryService, mockUserStateCachedService, mockClaimsPrincipalHelper)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}