using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.Configuration;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Security;
using ConcernsCaseWork.Services.Teams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Team
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class SelectColleaguesPageModel : AbstractPageModel
	{
		private readonly IRbacManager _rbacManager;
		private readonly ILogger<SelectColleaguesPageModel> _logger;
		private readonly ITeamsModelService _teamsService;
		private readonly IFeatureManager _featureManager;

		[BindProperty]
		public IList<string> SelectedColleagues { get; set; }
		public string[] Users { get; set; }

		public bool IsSelectTeamCaseWorkRedesignEnabled { get; set; }

		public SelectColleaguesPageModel(IRbacManager rbacManager,
			ILogger<SelectColleaguesPageModel> logger, 
			ITeamsModelService teamsService,
			IFeatureManager featureManager
			)
		{
			_rbacManager = Guard.Against.Null(rbacManager);
			_logger = Guard.Against.Null(logger);
			_teamsService = Guard.Against.Null(teamsService);
			_featureManager = featureManager;
		}

		public async Task<ActionResult> OnGetAsync()
		{
			try
			{
				_logger.LogMethodEntered();
				await LoadPage();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				TempData["Error.Message"] = ErrorOnGetPage;
			}
			return Page();
		}

		public async Task<ActionResult> OnPostSelectColleagues()
		{
			try
			{
				_logger.LogMethodEntered();

				var isSelectTeamCaseWorkRedesignEnabled = await _featureManager.IsEnabledAsync(nameof(FeatureFlags.IsSelectTeamCaseWorkRedesignEnabled));

				if (isSelectTeamCaseWorkRedesignEnabled)
				{
					SelectedColleagues = RetrieveColleagueList();
				}

				Guard.Against.Null(SelectedColleagues);
				await _teamsService.UpdateCaseworkTeam(new Models.Teams.ConcernsTeamCaseworkModel(_CurrentUserName, SelectedColleagues.ToArray()));
				TempData["selectedColleagues"] = null;
				return Redirect("/TeamCasework");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				TempData["Error.Message"] = ErrorOnPostPage;

				await LoadPage();
				return Page();
			}
		}

		private async Task LoadPage()
		{
			IsSelectTeamCaseWorkRedesignEnabled = await _featureManager.IsEnabledAsync(nameof(FeatureFlags.IsSelectTeamCaseWorkRedesignEnabled));

			if (!string.IsNullOrWhiteSpace(_CurrentUserName))
			{
				try
				{
					var teamMembers = _teamsService.GetCaseworkTeam(_CurrentUserName);
					var users = _rbacManager.GetSystemUsers(excludes: _CurrentUserName);

					await Task.WhenAll(teamMembers, users);

					// TODO. Get selected colleagues from somewhere, using actual live data not hard coded.
					// Get users from somewhere, possibly the rbacManager
					SelectedColleagues = teamMembers.Result.TeamMembers;
					Users = users.Result.ToArray();
				}
				catch (AggregateException aggregateException)
				{
					foreach (var ex in aggregateException.InnerExceptions)
					{
						_logger.LogErrorMsg(ex);
					}
				}
				catch (Exception ex)
				{
					_logger.LogErrorMsg(ex);
				}
			}
			else
			{
				SelectedColleagues = new List<string>();
				Users = Array.Empty<string>();
			}
		}

		public async Task<ActionResult> OnGetTeamList()
		{
			_logger.LogMethodEntered();

			try
			{
				var teamMembers = _teamsService.GetCaseworkTeam(_CurrentUserName);
				var users = _rbacManager.GetSystemUsers(excludes: _CurrentUserName);

				await Task.WhenAll(teamMembers, users);

				// TODO. Get selected colleagues from somewhere, using actual live data not hard coded.
				// Get users from somewhere, possibly the rbacManager
				//SelectedColleagues = teamMembers.Result.TeamMembers;
				Users = users.Result.ToArray();

				return new JsonResult(Users);
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				return new JsonResult(new string[] { });
			}
		}

		public async Task<IActionResult> OnGetSelectColleague(string colleague)
		{
			var colleagues = RetrieveColleagueList();

			colleagues.Add(colleague);

			return UpdateColleagueList(colleagues.ToArray());
		}

		public async Task<IActionResult> OnGetRemoveColleague(string colleague)
		{
			var colleagues = RetrieveColleagueList();

			colleagues.Remove(colleague);

			return UpdateColleagueList(colleagues.ToArray());
		}

		private List<string> RetrieveColleagueList()
		{
			var t = TempData["selectedColleagues"];
			var colleagues = (string[])TempData["selectedColleagues"] ?? Array.Empty<string>();

			return colleagues.ToList();
		}

		private ActionResult UpdateColleagueList(string[] colleagues)
		{
			TempData["selectedColleagues"] = colleagues;
			SelectedColleagues = colleagues;

			return Partial("_SelectedColleagues", colleagues);
		}

		private string _CurrentUserName { get => this.User.Identity.Name; }
	}
}