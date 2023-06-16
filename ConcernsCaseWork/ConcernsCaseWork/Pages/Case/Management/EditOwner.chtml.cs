﻿using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Service.Teams;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management
{
	public class EditOwnerPageModel : AbstractPageModel
	{
		private ICaseModelService _caseModelService;
		private ITeamsService _teamsService;
		private ILogger<EditOwnerPageModel> _logger;
		

		[BindProperty(SupportsGet = true)] public int Urn { get; init; }

		public string CurrentCaseOwner { get; set; }
		public string CaseNumber { get; set; }
		public bool ShowValidationMessage { get; set; }
		[TempData]
		public bool CaseOwnerChanged { get; set; }

		public EditOwnerPageModel(
			ICaseModelService caseModelService,
			ITeamsService teamsService,
			ILogger<EditOwnerPageModel> logger) 
		{
			_caseModelService = caseModelService;
			_teamsService = teamsService;
			_logger = logger;
		}
		public async Task<ActionResult> OnGet()
		{
			_logger.LogMethodEntered();

			try
			{
				var caseModel = await _caseModelService.GetCaseByUrn(Urn);

				CurrentCaseOwner = caseModel.CreatedBy;
				CaseNumber = caseModel.Urn.ToString();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
			}

			return Page();
		}
		public async Task<ActionResult> OnPost(string selectedOwner, string currentOwner,int valueInList)

		{
			_logger.LogMethodEntered();
			if (selectedOwner == currentOwner)
			{
				return Redirect($"/case/{Urn}/management");
			}

			if (valueInList != -1)
			{
				if (!String.IsNullOrEmpty(selectedOwner) && !String.IsNullOrWhiteSpace(selectedOwner))
				{
					return await UpdateCaseOwner(selectedOwner);
				}
			}




			var caseModel = await _caseModelService.GetCaseByUrn(Urn);
			CurrentCaseOwner = caseModel.CreatedBy;
			CaseNumber = caseModel.Urn.ToString();
			ShowValidationMessage = true;
			return Page();
			
		}

		private async Task<ActionResult> UpdateCaseOwner(string selectedOwner)
		{
			try
			{
				await _caseModelService.PatchOwner(Urn, selectedOwner);
				CaseOwnerChanged = true;
				return  Redirect($"/case/{Urn}/management"); 
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
			}
			return Redirect($"/case/{Urn}/management");
		}

		public async Task<ActionResult> OnGetTeamList()
		{
			_logger.LogMethodEntered();

			try
			{
				var result = await _teamsService.GetTeamOwners();

				return new JsonResult(result);
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				return new JsonResult(new string[] { });
			}
		}
	}
}
