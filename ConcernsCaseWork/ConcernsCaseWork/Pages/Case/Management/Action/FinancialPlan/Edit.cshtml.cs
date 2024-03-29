﻿using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Services.FinancialPlan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.FinancialPlan
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditPageModel : FinancialPlanBasePageModel
	{
		private readonly ILogger<EditPageModel> _logger;
		private readonly IFinancialPlanModelService _financialPlanModelService;

		public EditPageModel(
			IFinancialPlanModelService financialPlanModelService, ILogger<EditPageModel> logger)
		{
			_financialPlanModelService = financialPlanModelService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				var financialPlanModel = await _financialPlanModelService.GetFinancialPlansModelById(CaseUrn, financialPlanId);

				if (financialPlanModel.IsClosed)
				{
					return Redirect($"/case/{CaseUrn}/management/action/financialplan/{financialPlanId}/closed");
				}

				LoadPageComponents(financialPlanModel);

			}

			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				SetErrorMessage(ErrorOnGetPage);
			}
			
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				if (!ModelState.IsValid)
				{
					ResetPageComponentsOnValidationError();
					return Page();
				}

				var patchFinancialPlanModel = new PatchFinancialPlanModel
				{
					Id = financialPlanId,
					CaseUrn = CaseUrn,
					DatePlanRequested = DatePlanRequested.Date?.ToDateTime(),
					Notes = Notes.Text.StringContents,
					UpdatedAt = DateTime.Now
				};

				await _financialPlanModelService.PatchFinancialById(patchFinancialPlanModel);
				return Redirect($"/case/{CaseUrn}/management");

			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}

		private void LoadPageComponents(FinancialPlanModel financialPlanModel)
		{
			if (financialPlanModel.DatePlanRequested.HasValue)
				DatePlanRequested.Date = new OptionalDateModel(financialPlanModel.DatePlanRequested.Value);

			Notes.Text.StringContents = financialPlanModel.Notes;
		}
	}
}