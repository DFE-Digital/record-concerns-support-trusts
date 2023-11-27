using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Pages.Base;
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
		private readonly ILogger<SelectColleaguesPageModel> _logger;
		private readonly ITeamsModelService _teamsService;

		[BindProperty]
		public IList<string> SelectedColleagues { get; set; } = new List<string>();
		public string[] Users { get; set; } = Array.Empty<string>();

		public SelectColleaguesPageModel(ILogger<SelectColleaguesPageModel> logger, ITeamsModelService teamsService)
		{
			_logger = logger;
			_teamsService = teamsService;
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
				SetErrorMessage(ErrorOnGetPage);

				throw;
			}

			return Page();
		}

		public async Task<ActionResult> OnPostSelectColleagues()
		{
			try
			{
				_logger.LogMethodEntered();

				await _teamsService.UpdateCaseworkTeam(new Models.Teams.ConcernsTeamCaseworkModel(_CurrentUserName, SelectedColleagues.ToArray()));
				return Redirect("/TeamCasework");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);

				throw;
			}
		}

		private async Task LoadPage()
		{
			if (string.IsNullOrWhiteSpace(_CurrentUserName))
			{
				return;
			}

			var teamMembers = _teamsService.GetCaseworkTeam(_CurrentUserName);
			var users = _teamsService.GetTeamOwners(excludes: _CurrentUserName);

			await Task.WhenAll(teamMembers, users);

			SelectedColleagues = teamMembers.Result.TeamMembers;
			Users = users.Result.ToArray();
		}

		public async Task<ActionResult> OnGetTeamList()
		{
			_logger.LogMethodEntered();

			try
			{
				var users = await _teamsService.GetTeamOwners(excludes: _CurrentUserName);

				Users = users.ToArray();

				return new JsonResult(Users);
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				return new JsonResult(new string[] { });
			}
		}

		public IActionResult OnGetColleaguesTable(string data)
		{
			var colleagueList = new List<string>();

			if (!string.IsNullOrEmpty(data))
			{
				colleagueList = data.Split(",").ToList();
			}

			return Partial("_ColleaguesTable", colleagueList);
		}

		private string _CurrentUserName { get => User.Identity.Name; }
	}
}