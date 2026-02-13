using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Service.AzureAd.Services;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management
{
    public class EditTeamLeaderPageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly IGraphUserService _graphUserService;
		private readonly ILogger<EditTeamLeaderPageModel> _logger;

		[BindProperty(SupportsGet = true)] public int Urn { get; init; }

		public string CurrentTeamLeader { get; set; }

		public string CaseNumber { get; set; }

		public EditTeamLeaderPageModel(
			ICaseModelService caseModelService,
			IGraphUserService graphUserService,
			ILogger<EditTeamLeaderPageModel> logger)
		{
			_caseModelService = caseModelService;
			_graphUserService = graphUserService;
			_logger = logger;
		}

		public async Task<ActionResult> OnGet()
		{
			_logger.LogMethodEntered();

			try
			{
				await LoadPage();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
			}

			return Page();
		}

		public async Task<ActionResult> OnPost(string selectedTeamLeader, string currentTeamLeader, int valueInList)
		{
			_logger.LogMethodEntered();

			if (valueInList == -1 || string.IsNullOrWhiteSpace(selectedTeamLeader))
			{
				await LoadPage();

				ModelState.AddModelError("SelectedCaseTeamLeader", "A team leader must be selected");

				return Page();
			}

			if (selectedTeamLeader == currentTeamLeader)
			{
				return Redirect($"/case/{Urn}/management");
			}

			return await UpdateCaseTeamLeader(selectedTeamLeader);
		}

		public async Task<ActionResult> OnGetUsersList()
		{
			_logger.LogMethodEntered();

			try
			{
				var adUsers = await _graphUserService.GetCaseWorkersAndAdmins();

				return new JsonResult(adUsers.Select(u => u.Mail));
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				return new JsonResult(Array.Empty<string>());
			}
		}

		private async Task<ActionResult> UpdateCaseTeamLeader(string selectedTeamLeader)
		{
			try
			{
				await _caseModelService.PatchTeamLeader(Urn, selectedTeamLeader);

				return Redirect($"/case/{Urn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
			}

			return Redirect($"/case/{Urn}/management");
		}

		private async Task LoadPage()
		{
			var caseModel = await _caseModelService.GetCaseByUrn(Urn);
			CurrentTeamLeader = caseModel.TeamLedBy;
		}
	}
}
