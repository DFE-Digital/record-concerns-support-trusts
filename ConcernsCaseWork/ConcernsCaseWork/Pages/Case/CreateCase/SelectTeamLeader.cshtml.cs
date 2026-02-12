using ConcernsCaseWork.Service.AzureAd.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;
using System.Threading.Tasks;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Redis.Users;

namespace ConcernsCaseWork.Pages.Case.CreateCase
{
    public class SelectTeamLeaderModel : CreateCaseBasePageModel
	{
		private readonly IGraphUserService _graphUserService;
		private readonly ILogger<SelectTeamLeaderModel> _logger;
		private readonly IUserStateCachedService _cachedUserService;

		public SelectTeamLeaderModel(
			IGraphUserService graphUserService,
			ILogger<SelectTeamLeaderModel> logger,
			IUserStateCachedService cachedUserService,
			IClaimsPrincipalHelper claimsPrincipalHelper) : base(cachedUserService, claimsPrincipalHelper)
		{
			_graphUserService = graphUserService;
			_logger = logger;
			_cachedUserService = cachedUserService;
		}

		public string CurrentTeamLeader { get; set; }

		public async Task<IActionResult> OnGet()
		{
			return Page();
		}

		public async Task<ActionResult> OnPost(string selectedTeamLeader, int valueInList)
		{
			_logger.LogMethodEntered();

			if (valueInList == -1 || string.IsNullOrWhiteSpace(selectedTeamLeader))
			{
				ModelState.AddModelError("SelectedCaseTeamLeader", "A team leader must be selected");

				return Page();
			}

			var userName = GetUserName();
			var userState = await GetUserState();
			userState.TeamLeader = selectedTeamLeader;
			await _cachedUserService.StoreData(userName, userState);

			return RedirectToPage("SelectCaseDivision");
		}

		public async Task<ActionResult> OnGetUsersList()
		{
			_logger.LogMethodEntered();

			try
			{
				var adUsers = await _graphUserService.GetTeamleaders();

				return new JsonResult(adUsers.Select(u => u.Mail));
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				return new JsonResult(Array.Empty<string>());
			}
		}
	}
}
