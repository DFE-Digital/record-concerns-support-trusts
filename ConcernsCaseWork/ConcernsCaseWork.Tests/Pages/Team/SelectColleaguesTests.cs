using ConcernsCaseWork.Models.Teams;
using ConcernsCaseWork.Pages.Team;
using ConcernsCaseWork.Security;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ITeamsService = ConcernsCaseWork.Services.Teams.ITeamsService;

namespace ConcernsCaseWork.Tests.Pages.Team
{
	[Parallelizable(ParallelScope.All)]
	public class SelectColleaguesTests
	{
		[Test]
		public async Task WhenOnGetAsync_Return_Page()
		{
			// arrange
			var testFixture = new TestFixture()
				.WithUsersAvailableForSelection("user1", "user2", "Mr.Bean")
				.WithPreviouslySelectedUsers("Mr.Bean");

			var sut = testFixture.BuildSut(authenticatedPage: true);

			// act
			var pageResponse = await sut.OnGetAsync();

			// assert
			Assert.That(pageResponse, Is.Not.Null);
			Assert.That(sut.SelectedColleagues, Is.Not.Null);
			Assert.That(sut.Users, Is.Not.Null);
			Assert.That(sut.Users.Length, Is.EqualTo(3));
			Assert.That(sut.TempData, Is.Empty);

			testFixture.VerifyMethodEntered(nameof(SelectColleaguesPageModel.OnGetAsync));
		}

		[Test]
		public async Task WhenOnPostSelectColleagues_UpdatesSelections_And_Redirects_To_TeamCaseworkPage()
		{
			// arrange
			const string NewUsernameSelection = "Fred.Flintstone";
			const string ExpectedRedirectUrl = "/#team-casework";

			var testFixture = new TestFixture()
				.WithPreviouslySelectedUsers("Mr.Bean");

			var pageModel = testFixture.BuildSut(authenticatedPage: true);

			// act
			pageModel.SelectedColleagues = new List<string> { NewUsernameSelection };
			var pageResponse = await pageModel.OnPostSelectColleagues();

			// assert
			var page = pageResponse as RedirectResult;
			Assert.NotNull(page);
			Assert.That(page, Is.Not.Null);
			Assert.IsTrue(page.Url.Equals(ExpectedRedirectUrl));

			testFixture.VerifyMethodEntered(nameof(SelectColleaguesPageModel.OnPostSelectColleagues));
			testFixture.MockTeamsService.Verify(x => x.UpdateTeamCaseworkSelectedUsers(It.Is<TeamCaseworkUsersSelectionModel>(m => m.UserName == testFixture.CurrentUserName
				&& m.SelectedTeamMembers.Length == 1
				&& m.SelectedTeamMembers[0] == NewUsernameSelection
			)));
		}

		[Test]
		public async Task WhenOnPostSelectColleagues_RequestForm_Missing_Return_ErrorOnPage()
		{
			// arrange			
			var testFixture = new TestFixture()
				.WithNoCurrentUser();
			// act
			var sut = testFixture.BuildSut(authenticatedPage: false);
			var pageResponse = await sut.OnPostSelectColleagues();

			// assert
			var page = pageResponse as PageResult;
			Assert.That(page, Is.Not.Null);
			Assert.IsEmpty(sut.SelectedColleagues);
			Assert.IsEmpty(sut.Users);
			Assert.IsNotEmpty(sut.TempData);
			Assert.That(sut.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));

			testFixture.VerifyMethodEntered(nameof(SelectColleaguesPageModel.OnPostSelectColleagues));
		}

		private static SelectColleaguesPageModel BuildPageModel(IRbacManager rbacManager, ILogger<SelectColleaguesPageModel> logger, ITeamsService teamsService, bool isAuthenticated, string userName = "Tester")
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated, userName);

			return new SelectColleaguesPageModel(rbacManager, logger, teamsService)
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
				MockTeamsService = new Mock<ITeamsService>();
				MockRbacManager = new Mock<IRbacManager>();
			}

			public string CurrentUserName { get; private set; }
			public Mock<ILogger<SelectColleaguesPageModel>> MockLogger { get; }
			public Mock<ITeamsService> MockTeamsService { get; }
			public Mock<IRbacManager> MockRbacManager { get; }

			internal SelectColleaguesPageModel BuildSut(bool authenticatedPage = true)
			{
				(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(authenticatedPage, CurrentUserName);

				return new SelectColleaguesPageModel(MockRbacManager.Object, MockLogger.Object, MockTeamsService.Object)
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
				this.MockRbacManager.Setup(r => r.GetDefaultUsers(It.IsAny<string[]>())).ReturnsAsync(users);
				return this;
			}

			internal TestFixture WithoutPreviouslySelectedUsers(string userName)
			{
				MockTeamsService.Setup(x => x.GetTeamCaseworkSelectedUsers(CurrentUserName))
					.ReturnsAsync(new TeamCaseworkUsersSelectionModel(CurrentUserName, Array.Empty<string>()));

				return this;
			}

			internal TestFixture WithPreviouslySelectedUsers(string userName)
			{
				MockTeamsService.Setup(x => x.GetTeamCaseworkSelectedUsers(CurrentUserName))
					.ReturnsAsync(new TeamCaseworkUsersSelectionModel(CurrentUserName, new[] { userName }));

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
