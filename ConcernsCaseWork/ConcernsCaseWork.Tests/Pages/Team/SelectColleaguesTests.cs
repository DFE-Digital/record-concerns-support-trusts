using ConcernsCaseWork.Models.Teams;
using ConcernsCaseWork.Pages.Team;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITeamsModelService = ConcernsCaseWork.Services.Teams.ITeamsModelService;

namespace ConcernsCaseWork.Tests.Pages.Team
{
	[Parallelizable(ParallelScope.All)]
	public class SelectColleaguesTests
	{
		[Test]
		public async Task WhenOnGetAsync_WithPreviously_Selected_Team_Members_Return_Page()
		{
			// arrange
			var testFixture = new TestFixture()
				.WithUsersAvailableForSelection("user1", "user2", "Mr.Bean")
				.WithPreviouslySelectedUser("Mr.Bean");

			var sut = testFixture.BuildSut(authenticatedPage: true);

			// act
			var pageResponse = await sut.OnGetAsync();

			// assert
			Assert.That(pageResponse, Is.Not.Null);
			Assert.That(sut.SelectedColleagues, Is.Not.Null);
			Assert.That(sut.Users, Is.Not.Null);
			Assert.That(sut.Users.Length, Is.EqualTo(3));
			Assert.That(sut.Users.Count(x => x.Equals("user1")), Is.EqualTo(1));
			Assert.That(sut.Users.Count(x => x.Equals("user2")), Is.EqualTo(1));
			Assert.That(sut.Users.Count(x => x.Equals("Mr.Bean")), Is.EqualTo(1));
			Assert.That(sut.TempData, Is.Empty);

			testFixture.VerifyMethodEntered(nameof(SelectColleaguesPageModel.OnGetAsync));
		}

		[Test]
		public async Task WhenOnGetAsync_With_No_Previously_Selected_Team_Members_Return_Page()
		{
			// arrange
			var testFixture = new TestFixture();

			var sut = testFixture
				.WithNoUsersAvailableForSelection()
				.WithNoPreviouslySelectedUsers()
				.BuildSut(authenticatedPage: true);

			// act
			var pageResponse = await sut.OnGetAsync();

			// assert
			Assert.That(pageResponse, Is.Not.Null);
			Assert.That(sut.SelectedColleagues, Is.Not.Null);
			Assert.That(sut.Users, Is.Not.Null);
			Assert.That(sut.Users.Length, Is.Zero);
			Assert.That(sut.TempData, Is.Empty);

			testFixture.VerifyMethodEntered(nameof(SelectColleaguesPageModel.OnGetAsync));
		}

		[Test]
		public async Task WhenOnPostSelectColleagues_UpdatesSelections_And_Redirects_To_TeamCaseworkPage()
		{
			// arrange
			const string NewUsernameSelection = "Fred.Flintstone";
			const string ExpectedRedirectUrl = "/TeamCasework";

			var testFixture = new TestFixture()
				.WithPreviouslySelectedUser("Mr.Bean");

			var pageModel = testFixture.BuildSut(authenticatedPage: true);

			// act
			pageModel.SelectedColleagues = new List<string> { NewUsernameSelection };
			var pageResponse = await pageModel.OnPostSelectColleagues();

			// assert
			var page = pageResponse as RedirectResult;
			Assert.That(page, Is.Not.Null);
			Assert.That(page, Is.Not.Null);
			Assert.That(page.Url.Equals(ExpectedRedirectUrl), Is.True);

			testFixture.VerifyMethodEntered(nameof(SelectColleaguesPageModel.OnPostSelectColleagues));
			testFixture.MockTeamsService.Verify(x => x.UpdateCaseworkTeam(It.Is<ConcernsTeamCaseworkModel>(m => m.OwnerId == testFixture.CurrentUserName
				&& m.TeamMembers.Length == 1
				&& m.TeamMembers[0] == NewUsernameSelection
			)));
		}

		private static SelectColleaguesPageModel BuildPageModel(ILogger<SelectColleaguesPageModel> logger, ITeamsModelService teamsService, bool isAuthenticated, string userName = "Tester")
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated, userName);

			return new SelectColleaguesPageModel(logger, teamsService)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}

		private class TestFixture
		{
			public TestFixture()
			{
				CurrentUserName = "John.Smith";
				MockLogger = new Mock<ILogger<SelectColleaguesPageModel>>();
				MockTeamsService = new Mock<ITeamsModelService>();
				MockFeatureManager = new Mock<IFeatureManager>();
			}

			public string CurrentUserName { get; private set; }
			public Mock<ILogger<SelectColleaguesPageModel>> MockLogger { get; }
			public Mock<ITeamsModelService> MockTeamsService { get; }

			public Mock<IFeatureManager> MockFeatureManager { get; }

			internal SelectColleaguesPageModel BuildSut(bool authenticatedPage = true)
			{
				(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(authenticatedPage, CurrentUserName);

				return new SelectColleaguesPageModel(MockLogger.Object, MockTeamsService.Object)
				{
					PageContext = pageContext,
					TempData = tempData,
					Url = new UrlHelper(actionContext),
					MetadataProvider = pageContext.ViewData.ModelMetadata
				};
			}

			internal void VerifyMethodEntered(string methodName)
			{
				TestHelpers.VerifyMethodEntryLogged(MockLogger, methodName);
			}

			internal TestFixture WithUsersAvailableForSelection(params string[] users)
			{
				this.MockTeamsService.Setup(r => r.GetTeamOwners(It.IsAny<string[]>())).ReturnsAsync(users);
				return this;
			}


			internal TestFixture WithNoUsersAvailableForSelection()
			{
				this.MockTeamsService.Setup(r => r.GetTeamOwners(It.IsAny<string[]>())).ReturnsAsync(Array.Empty<string>());
				return this;
			}

			internal TestFixture WitNohUsersAvailableForSelection()
			{
				this.MockTeamsService.Setup(r => r.GetTeamOwners(It.IsAny<string[]>())).ReturnsAsync(Array.Empty<string>());
				return this;
			}

			internal TestFixture WithPreviouslySelectedUser(string userName)
			{
				MockTeamsService.Setup(x => x.GetCaseworkTeam(CurrentUserName))
					.ReturnsAsync(new ConcernsTeamCaseworkModel(CurrentUserName, new[] { userName }));

				return this;
			}

			internal TestFixture WithNoPreviouslySelectedUsers()
			{
				MockTeamsService.Setup(x => x.GetCaseworkTeam(CurrentUserName))
					.ReturnsAsync(new ConcernsTeamCaseworkModel(CurrentUserName, Array.Empty<string>()));

				return this;
			}


			internal TestFixture WithNoCurrentUser()
			{
				this.CurrentUserName = null;
				return this;
			}
		}
	}
}
