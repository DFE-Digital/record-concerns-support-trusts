using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Redis.FinancialPlan;
using ConcernsCaseWork.Services.FinancialPlan;
using ConcernsCaseWork.Service.FinancialPlan;
using ConcernsCaseWork.Mappers;
using System.Linq;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.API.Contracts.Enums.TrustFinancialForecast;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.CoreTypes;
using Microsoft.Graph.Models;

namespace ConcernsCaseWork.Pages.Case.Management.Action.FinancialPlan
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ClosePageModel : FinancialPlanBasePageModel
	{
		private readonly ILogger<ClosePageModel> _logger;
		private readonly IFinancialPlanModelService _financialPlanModelService;
		private readonly IFinancialPlanStatusCachedService _financialPlanStatusCachedService;

		public ClosePageModel(
			IFinancialPlanModelService financialPlanModelService, IFinancialPlanStatusCachedService financialPlanStatusService, ILogger<ClosePageModel> logger)
		{
			_financialPlanModelService = financialPlanModelService;
			_financialPlanStatusCachedService = financialPlanStatusService;
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
					//DatePlanRequested = planRequested,
					DateViablePlanReceived = !DatePlanRequested.Date?.IsEmpty() ?? false ? DatePlanRequested.Date?.ToDateTime() : null,
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