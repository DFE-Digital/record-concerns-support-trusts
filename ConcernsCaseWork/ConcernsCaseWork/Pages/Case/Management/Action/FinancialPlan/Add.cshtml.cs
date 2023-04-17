using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Redis.Models;
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
	public class AddPageModel : FinancialPlanBasePageModel
	{
		private readonly IFinancialPlanModelService _financialPlanModelService;
		private readonly ILogger<AddPageModel> _logger;

		public AddPageModel(
			IFinancialPlanModelService financialPlanModelService,
			ILogger<AddPageModel> logger)
		{
			_financialPlanModelService = financialPlanModelService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogMethodEntered();

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

				var now = DateTime.Now;
				var model = new CreateFinancialPlanModel
				{
					CaseUrn = CaseUrn,
					CreatedAt = now,
					UpdatedAt = now,
					DatePlanRequested = DatePlanRequested.Date?.ToDateTime(),
					CreatedBy = User.Identity.Name.GetValueOrNullIfWhitespace(),
					Notes = Notes.Text.StringContents
				};

				await _financialPlanModelService.PostFinancialPlanByCaseUrn(model);

				return Redirect($"/case/{CaseUrn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				SetErrorMessage(ErrorOnPostPage);
			}
			return Page();
		}
	}
}