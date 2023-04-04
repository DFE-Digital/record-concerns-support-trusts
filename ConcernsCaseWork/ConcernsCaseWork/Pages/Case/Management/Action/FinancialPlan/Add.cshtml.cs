using ConcernsCaseWork.API.Contracts.Enums.TrustFinancialForecast;
using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Contracts.ResponseModels.TrustFinancialForecasts;
using ConcernsCaseWork.CoreTypes;
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Service.Helpers;
using ConcernsCaseWork.Service.TrustFinancialForecast;
using ConcernsCaseWork.Services.FinancialPlan;
using ConcernsCaseWork.Services.Nti;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.FinancialPlan
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddPageModel : FPlanBasePage
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
					DatePlanRequested = !DatePlanRequested.Date?.IsEmpty() ?? false ? DatePlanRequested.Date?.ToDateTime() : null,
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