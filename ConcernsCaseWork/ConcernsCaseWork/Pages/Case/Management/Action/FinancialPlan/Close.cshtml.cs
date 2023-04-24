using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Redis.FinancialPlan;
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
	public class ClosePageModel : FinancialPlanBasePageModel
	{
		private readonly ILogger<ClosePageModel> _logger;
		private readonly IFinancialPlanModelService _financialPlanModelService;

		public ClosePageModel(
			IFinancialPlanModelService financialPlanModelService, ILogger<ClosePageModel> logger)
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

				var financialPlanModel = await _financialPlanModelService.GetFinancialPlansModelById(CaseUrn, financialPlanId);

				var now = DateTime.Now;

				var patchFinancialPlanModel = new PatchFinancialPlanModel
				{
					Id = financialPlanId,
					CaseUrn = CaseUrn,
					StatusId = FinancialPlanClosureStatus.SelectedId,
					Notes = Notes.Text.StringContents,
					DatePlanRequested = financialPlanModel.DatePlanRequested,
					DateViablePlanReceived = !DateViablePlanReceived.Date?.IsEmpty() ?? false ? DateViablePlanReceived.Date?.ToDateTime() : null,
					// todo: closed date is currently set to server date across the system. This should ideally be converted to UTC
					ClosedAt = now,
					UpdatedAt = now
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
			if (financialPlanModel.DateViablePlanReceived.HasValue)
			{
				DateViablePlanReceived.Date = new OptionalDateModel((DateTime)financialPlanModel.DateViablePlanReceived);
			}

			Notes.Text.StringContents = financialPlanModel.Notes;
		}
	}
}