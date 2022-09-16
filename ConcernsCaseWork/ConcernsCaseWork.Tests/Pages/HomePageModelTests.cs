using AutoFixture;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.Teams;
using ConcernsCaseWork.Pages;
using ConcernsCaseWork.Security;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Models;
using Service.Redis.Security;
using Service.Redis.Users;
using Service.TRAMS.Status;
using System;
using System.Collections.Generic;
using System.Linq;
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
			var homeModels = HomePageFactory.BuildHomeModels();

			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockRbacManager = new Mock<IRbacManager>();
			var mockLogger = new Mock<ILogger<HomePageModel>>();
			var mockUserStateCache = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			var roles = RoleFactory.BuildListRoleEnum();
			var defaultUsers = new[] { "user1", "user2" };
			var roleClaimWrapper = new RoleClaimWrapper { Roles = roles, Users = defaultUsers };

			//mockRbacManager.Setup(r => r.GetUserRoleClaimWrapper(It.IsAny<string>()))
			//	.ReturnsAsync(roleClaimWrapper);
			mockCaseModelService.Setup(c => c.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<StatusEnum>()))
				.ReturnsAsync(homeModels);

			var mockTeamService = new Mock<ITeamsModelService>();
			mockTeamService.Setup(x => x.GetCaseworkTeam(It.IsAny<string>()))
				.ReturnsAsync(new ConcernsCaseWork.Models.Teams.ConcernsTeamCaseworkModel("random.user", Array.Empty<string>()));

			var homePageModel = SetupHomeModel(mockCaseModelService.Object, mockRbacManager.Object, mockLogger.Object, mockTeamService.Object, mockUserStateCache.Object, mockClaimsPrincipalHelper.Object);

			// act
			await homePageModel.OnGetAsync();

			// assert
			Assert.IsAssignableFrom<List<HomeModel>>(homePageModel.CasesActive);
			Assert.That(homePageModel.CasesActive.Count, Is.EqualTo(homeModels.Count));

			foreach (var expected in homePageModel.CasesActive)
			{
				foreach (var actual in homeModels.Where(actual => expected.CaseUrn.Equals(actual.CaseUrn)))
				{
					Assert.That(expected.Closed, Is.EqualTo(actual.Closed));
					Assert.That(expected.Created, Is.EqualTo(actual.Created));
					Assert.That(DateTimeOffset.FromUnixTimeMilliseconds(expected.CreatedUnixTime).ToString("dd-MM-yyyy"), Is.EqualTo(actual.Created));
					Assert.That(expected.Updated, Is.EqualTo(actual.Updated));
					Assert.That(DateTimeOffset.FromUnixTimeMilliseconds(expected.UpdatedUnixTime).ToString("dd-MM-yyyy"), Is.EqualTo(actual.Updated));
					Assert.That(expected.Review, Is.EqualTo(actual.Review));
					Assert.That(expected.CreatedBy, Is.EqualTo(actual.CreatedBy));
					Assert.That(expected.CaseUrn, Is.EqualTo(actual.CaseUrn));
					Assert.That(expected.TrustName, Is.EqualTo(actual.TrustName));
					Assert.That(expected.TrustNameTitle, Is.EqualTo(actual.TrustName.ToTitle()));

					var expectedRecordsModel = expected.RecordsModel;
					var actualRecordsModel = actual.RecordsModel;

					for (var index = 0; index < expectedRecordsModel.Count; ++index)
					{
						Assert.That(expectedRecordsModel.ElementAt(index).Urn, Is.EqualTo(actualRecordsModel.ElementAt(index).Urn));
						Assert.That(expectedRecordsModel.ElementAt(index).CaseUrn, Is.EqualTo(actualRecordsModel.ElementAt(index).CaseUrn));
						Assert.That(expectedRecordsModel.ElementAt(index).RatingUrn, Is.EqualTo(actualRecordsModel.ElementAt(index).RatingUrn));
						Assert.That(expectedRecordsModel.ElementAt(index).StatusUrn, Is.EqualTo(actualRecordsModel.ElementAt(index).StatusUrn));
						Assert.That(expectedRecordsModel.ElementAt(index).TypeUrn, Is.EqualTo(actualRecordsModel.ElementAt(index).TypeUrn));

						var expectedRecordRatingModel = expectedRecordsModel.ElementAt(index).RatingModel;
						var actualRecordRatingModel = actualRecordsModel.ElementAt(index).RatingModel;
						Assert.NotNull(expectedRecordRatingModel);
						Assert.NotNull(actualRecordRatingModel);
						Assert.That(expectedRecordRatingModel.Checked, Is.EqualTo(actualRecordRatingModel.Checked));
						Assert.That(expectedRecordRatingModel.Name, Is.EqualTo(actualRecordRatingModel.Name));
						Assert.That(expectedRecordRatingModel.Urn, Is.EqualTo(actualRecordRatingModel.Urn));
						Assert.That(expectedRecordRatingModel.RagRating, Is.EqualTo(actualRecordRatingModel.RagRating));
						Assert.That(expectedRecordRatingModel.RagRatingCss, Is.EqualTo(actualRecordRatingModel.RagRatingCss));

						var expectedRecordTypeModel = expectedRecordsModel.ElementAt(index).TypeModel;
						var actualRecordTypeModel = actualRecordsModel.ElementAt(index).TypeModel;
						Assert.NotNull(expectedRecordTypeModel);
						Assert.NotNull(actualRecordTypeModel);
						Assert.That(expectedRecordTypeModel.Type, Is.EqualTo(actualRecordTypeModel.Type));
						Assert.That(expectedRecordTypeModel.SubType, Is.EqualTo(actualRecordTypeModel.SubType));
						Assert.That(expectedRecordTypeModel.TypeDisplay, Is.EqualTo(actualRecordTypeModel.TypeDisplay));
						Assert.That(expectedRecordTypeModel.TypesDictionary, Is.EqualTo(actualRecordTypeModel.TypesDictionary));

						var expectedRecordStatusModel = expectedRecordsModel.ElementAt(index).StatusModel;
						var actualRecordStatusModel = actualRecordsModel.ElementAt(index).StatusModel;
						Assert.NotNull(expectedRecordStatusModel);
						Assert.NotNull(actualRecordTypeModel);
						Assert.That(expectedRecordStatusModel.Name, Is.EqualTo(actualRecordStatusModel.Name));
						Assert.That(expectedRecordStatusModel.Urn, Is.EqualTo(actualRecordStatusModel.Urn));
					}
				}
			}

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
			mockCaseModelService.Verify(c => c.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<StatusEnum>()), Times.Exactly(1));
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
			var emptyList = new List<HomeModel>();

			var roles = RoleFactory.BuildListUserRoleEnum();
			var defaultUsers = new[] { "user1", "user2" };
			var roleClaimWrapper = new RoleClaimWrapper { Roles = roles, Users = defaultUsers };

			//mockRbacManager.Setup(r => r.GetUserRoleClaimWrapper(It.IsAny<string>()))
			//	.ReturnsAsync(roleClaimWrapper);
			mockCaseModelService.Setup(model => model.GetCasesByCaseworkerAndStatus(It.IsAny<string[]>(), It.IsAny<StatusEnum>()))
				.ReturnsAsync(emptyList);
			mockCaseModelService.Setup(model => model.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<StatusEnum>()))
				.ReturnsAsync(emptyList);

			var mockTeamService = new Mock<ITeamsModelService>();
			mockTeamService.Setup(x => x.GetCaseworkTeam(It.IsAny<string>()))
				.ReturnsAsync(new ConcernsCaseWork.Models.Teams.ConcernsTeamCaseworkModel("random.user", Array.Empty<string>()));

			// act
			var indexModel = SetupHomeModel(mockCaseModelService.Object, mockRbacManager.Object, mockLogger.Object, mockTeamService.Object, mockUserStateCache.Object, mockClaimsPrincipalHelper.Object);
			await indexModel.OnGetAsync();

			// assert
			Assert.IsAssignableFrom<List<HomeModel>>(indexModel.CasesActive);
			Assert.That(indexModel.CasesActive.Count, Is.Zero);

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
				MockCaseModelService = new Mock<ICaseModelService>();
				MockRbacManager = new Mock<IRbacManager>();
				MockLogger = new Mock<ILogger<HomePageModel>>();
				MockUserStateCache = new Mock<IUserStateCachedService>();
				MockTeamService = new Mock<ITeamsModelService>();
				MockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
				IsAuthenticated = false;
			}

			public string CaseworkerId { get; set; }

			public Fixture Fixture { get; set; }

			public Mock<ITeamsModelService> MockTeamService { get; set; }

			public bool IsAuthenticated { get; set; }

			public HomePageModel CreateSut()
			{
				(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(IsAuthenticated);

				return new HomePageModel(MockCaseModelService.Object, MockRbacManager.Object, MockLogger.Object, MockTeamService.Object, MockUserStateCache.Object, MockClaimsPrincipalHelper.Object)
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

			public Mock<IRbacManager> MockRbacManager { get; set; }

			public Mock<ICaseModelService> MockCaseModelService { get; set; }

			public TestBuilder WithNoTeamCaseworkModel()
			{
				this.MockTeamService.Setup(x => x.GetCaseworkTeam(CaseworkerId))
					.ReturnsAsync(new ConcernsCaseWork.Models.Teams.ConcernsTeamCaseworkModel(CaseworkerId, Array.Empty<string>()));

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

		private static HomePageModel SetupHomeModel(ICaseModelService mockCaseModelService,
			IRbacManager mockRbacManager,
			ILogger<HomePageModel> mockLogger,
			ITeamsModelService mockTeamService,
			IUserStateCachedService mockUserStateCachedService,
			IClaimsPrincipalHelper mockClaimsPrincipalHelper,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new HomePageModel(mockCaseModelService, mockRbacManager, mockLogger, mockTeamService, mockUserStateCachedService, mockClaimsPrincipalHelper)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}