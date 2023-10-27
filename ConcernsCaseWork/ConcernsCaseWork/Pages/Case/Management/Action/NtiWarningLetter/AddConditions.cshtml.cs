using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Redis.NtiWarningLetter;
using ConcernsCaseWork.Service.NtiWarningLetter;
using ConcernsCaseWork.Services.NtiWarningLetter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.NtiWarningLetter
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddConditionsPageModel : AbstractPageModel
	{
		private readonly INtiWarningLetterModelService _ntiWarningLetterModelService;
		private readonly INtiWarningLetterConditionsCachedService _ntiWarningLetterConditionsCachedService;
		private readonly ILogger<AddConditionsPageModel> _logger;

		[TempData]
		public string ContinuationId { get; set; }

		[TempData]
		public bool IsReturningFromConditions { get; set; }

		[BindProperty(SupportsGet = true, Name = "urn")]
		public long CaseUrn { get; set; }

		[BindProperty(SupportsGet = true, Name = "warningLetterId")]
		public long? WarningLetterId { get; set; }

		public ICollection<NtiWarningLetterConditionModel> SelectedConditions { get; private set; }
		public ICollection<NtiWarningLetterConditionDto> AllConditions { get; private set; }

		public AddConditionsPageModel(
			INtiWarningLetterModelService ntiWarningLetterModelService,
			INtiWarningLetterConditionsCachedService ntiWarningLetterConditionsCachedService,
			ILogger<AddConditionsPageModel> logger)
		{
			_ntiWarningLetterModelService = ntiWarningLetterModelService;
			_ntiWarningLetterConditionsCachedService = ntiWarningLetterConditionsCachedService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				if (ContinuationId == null)
				{
					throw new InvalidOperationException("Continuation Id not found");
				}

				var model = await GetUpToDateModel();
				SelectedConditions = model.Conditions;

				AllConditions = await _ntiWarningLetterConditionsCachedService.GetAllConditionsAsync();

				IsReturningFromConditions = true;
				TempData.Keep(nameof(ContinuationId));
				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
				return Page();
			}
		}

		public async Task<IActionResult> OnPostAsync(string action)
		{
			_logger.LogMethodEntered();

			try
			{
				AllConditions = await _ntiWarningLetterConditionsCachedService.GetAllConditionsAsync();
				var conditions = Request.Form["condition"];
				var model = await GetUpToDateModel();
				model.Conditions = conditions.Select(s => NtiWarningLetterMappers.ToServiceModel(AllConditions.Single(c => c.Id == int.Parse(s)))).ToArray();
				await _ntiWarningLetterModelService.StoreWarningLetter(model, ContinuationId);

				IsReturningFromConditions = true;
				TempData.Keep(nameof(ContinuationId));

				if (WarningLetterId != null)
				{
					return Redirect($"/case/{CaseUrn}/management/action/NtiWarningLetter/{WarningLetterId}/edit");
				}
				else
				{
					return Redirect($"/case/{CaseUrn}/management/action/NtiWarningLetter/add");
				}
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}

		public bool IsConditionSelected(NtiWarningLetterConditionDto condition)
		{
			return SelectedConditions?.Any(c => c.Id == condition.Id) ?? false;
		}

		private async Task<NtiWarningLetterModel> GetUpToDateModel()
		{
			return await _ntiWarningLetterModelService.GetWarningLetter(ContinuationId);
		}
	}
}