using AutoFixture;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.Teams;
using ConcernsCaseWork.Pages;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Security;
using ConcernsCaseWork.Service.Status;
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
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ITeamsModelService = ConcernsCaseWork.Services.Teams.ITeamsModelService;

namespace ConcernsCaseWork.Tests.Pages
{
	[Parallelizable(ParallelScope.All)]
	public class HomePageModelTests
	{
		[Test]
		public async Task WhenInstanceOfIndexPageOnGetAsync_ReturnCases()
		{
			// arrange
			var mockLogger = new Mock<ILogger<HomePageModel>>();
			var mockUserStateCache = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			var mockCaseSummaryService = new Mock<ICaseSummaryService>();

			var activeCases = new List<ActiveCaseSummaryModel>()
			{
				new ActiveCaseSummaryModel
				{
					ActiveActionsAndDecisions = new string[]
					{
						"Some action"
					},
					ActiveConcerns = null,
					CaseUrn = 123,
					CreatedBy = "some user",
					CreatedAt = DateTime.Now.ToDayMonthYear(),
					IsMoreActionsAndDecisions = false,
					Rating = null,
					StatusName = null,
					TrustName = null,
					UpdatedAt = null
				}
			};
			mockCaseSummaryService
				.Setup(s => s.GetActiveCaseSummariesByCaseworker(It.IsAny<string>()))
				.ReturnsAsync(activeCases);

			var mockTeamService = new Mock<ITeamsModelService>();
			mockTeamService.Setup(x => x.GetCaseworkTeam(It.IsAny<string>()))
				.ReturnsAsync(new ConcernsTeamCaseworkModel("random.user", Array.Empty<string>()));
		
			var homePageModel = SetupHomeModel(mockLogger.Object, mockTeamService.Object, mockUserStateCache.Object, mockCaseSummaryService.Object, mockClaimsPrincipalHelper.Object);
		
			// act
			await homePageModel.OnGetAsync();
		
			// assert
			Assert.IsAssignableFrom<List<ActiveCaseSummaryModel>>(homePageModel.ActiveCases);
			
			Assert.That(homePageModel.ActiveCases.Count, Is.EqualTo(activeCases.Count));
			Assert.That(homePageModel.ActiveCases, Is.EquivalentTo(activeCases));

			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("HomePageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		
			mockTeamService.Verify(x => x.GetCaseworkTeam(It.IsAny<string>()), Times.Once);
		}

		[Test]
		public async Task WhenInstanceOfIndexPageOnGetAsync_ReturnEmptyCases()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockRbacManager = new Mock<IRbacManager>();
			var mockLogger = new Mock<ILogger<HomePageModel>>();
			var mockUserStateCache = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			var mockCaseSummaryService = new Mock<ICaseSummaryService>();
			var emptyList = new List<HomeModel>();

			mockCaseModelService.Setup(model => model.GetCasesByCaseworkerAndStatus(It.IsAny<string[]>(), It.IsAny<StatusEnum>()))
				.ReturnsAsync(emptyList);
			mockCaseModelService.Setup(model => model.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<StatusEnum>()))
				.ReturnsAsync(emptyList);

			mockCaseSummaryService.Setup(model => model.GetActiveCaseSummariesByCaseworker(It.IsAny<string>())).ReturnsAsync(new List<ActiveCaseSummaryModel>());

			var mockTeamService = new Mock<ITeamsModelService>();
			mockTeamService.Setup(x => x.GetCaseworkTeam(It.IsAny<string>()))
				.ReturnsAsync(new ConcernsTeamCaseworkModel("random.user", Array.Empty<string>()));

			// act
			var indexModel = SetupHomeModel(mockLogger.Object, mockTeamService.Object, mockUserStateCache.Object, mockCaseSummaryService.Object, mockClaimsPrincipalHelper.Object);
			await indexModel.OnGetAsync();

			// assert
			Assert.IsAssignableFrom<List<ActiveCaseSummaryModel>>(indexModel.ActiveCases);
			Assert.That(indexModel.ActiveCases.Count, Is.Zero);

			// Not sure that these verifications should take place. it leads to a brittle test.
			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("HomePageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);

			mockTeamService.Verify(x => x.GetCaseworkTeam(It.IsAny<string>()), Times.Once);
			mockCaseModelService.Verify(c => c.GetCasesByCaseworkerAndStatus(It.IsAny<string[]>(), It.IsAny<StatusEnum>()), Times.Once);
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
				MockLogger = new Mock<ILogger<HomePageModel>>();
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

			public HomePageModel CreateSut()
			{
				(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(IsAuthenticated);

				return new HomePageModel(MockLogger.Object, MockTeamService.Object, MockCaseSummaryService.Object, MockUserStateCache.Object, MockClaimsPrincipalHelper.Object)
				{
					PageContext = pageContext,
					TempData = tempData,
					Url = new UrlHelper(actionContext),
					MetadataProvider = pageContext.ViewData.ModelMetadata
				};
			}

			public Mock<IClaimsPrincipalHelper> MockClaimsPrincipalHelper { get; set; }

			public Mock<IUserStateCachedService> MockUserStateCache { get; set; }

			public Mock<ILogger<HomePageModel>> MockLogger { get; set; }
			
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

		private static HomePageModel SetupHomeModel(
			ILogger<HomePageModel> mockLogger,
			ITeamsModelService mockTeamService,
			IUserStateCachedService mockUserStateCachedService,
			ICaseSummaryService mockCaseSummaryService,
			IClaimsPrincipalHelper mockClaimsPrincipalHelper,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new HomePageModel(mockLogger, mockTeamService, mockCaseSummaryService, mockUserStateCachedService, mockClaimsPrincipalHelper)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}