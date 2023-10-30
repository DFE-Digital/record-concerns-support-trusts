using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Redis.Nti;
using ConcernsCaseWork.Service.Nti;
using ConcernsCaseWork.Services.Nti;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.Nti
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddConditionsPageModel : AbstractPageModel
	{
		private readonly INtiReasonsCachedService _ntiReasonsCachedService;
		private readonly INtiModelService _ntiModelService;
		private readonly INtiConditionsCachedService _ntiConditionsCachedService;
		private readonly ILogger<AddConditionsPageModel> _logger;

		[TempData]
		public string ContinuationId { get; set; }

		[TempData]
		public bool IsReturningFromConditions { get; set; }

		[BindProperty(SupportsGet = true, Name = "urn")]
		public long CaseUrn { get; set; }

		[BindProperty(SupportsGet = true, Name = "ntiId")]
		public long? NtiId { get; set; }

		public ICollection<NtiConditionModel> SelectedConditions { get; private set; }
		public ICollection<NtiConditionDto> AllConditions { get; private set; }

		public AddConditionsPageModel(
			INtiReasonsCachedService ntiWarningLetterReasonsCachedService,
			INtiModelService ntiModelService,
			INtiConditionsCachedService ntiConditionsCachedService,
			ILogger<AddConditionsPageModel> logger)
		{
			_ntiReasonsCachedService = ntiWarningLetterReasonsCachedService;
			_ntiModelService = ntiModelService;
			_ntiConditionsCachedService = ntiConditionsCachedService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogInformation("Case::Action::NTI::AddConditionsPageModel::OnGetAsync");

			if (string.IsNullOrEmpty(ContinuationId))
			{
				throw new InvalidOperationException("Continuation Id not found");
			}
			
			try
			{
				var model = await GetUpToDateModel();
				
				SelectedConditions = model.Conditions;

				AllConditions = await _ntiConditionsCachedService.GetAllConditionsAsync();

				IsReturningFromConditions = true;
				TempData.Keep(nameof(ContinuationId));
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
			}

			return Page();
		}

		public async Task<IActionResult> OnPostAsync(string action)
		{
			try
			{
				AllConditions = await _ntiConditionsCachedService.GetAllConditionsAsync();
				var conditions = Request.Form["condition"];
				var model = await GetUpToDateModel();
				model.Conditions = conditions.Select(s => NtiMappers.ToServiceModel(AllConditions.Single(c => c.Id == int.Parse(s)))).ToArray();
				await _ntiModelService.StoreNtiAsync(model, ContinuationId);

				IsReturningFromConditions = true;
				TempData.Keep(nameof(ContinuationId));

				if (NtiId != null)
				{
					return Redirect($"/case/{CaseUrn}/management/action/nti/{NtiId}/edit");
				}
				else
				{
					return Redirect($"/case/{CaseUrn}/management/action/nti/add");
				}
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}

		public bool IsConditionSelected(NtiConditionDto condition)
		{
			return SelectedConditions?.Any(c => c.Id == condition.Id) ?? false;
		}

		private async Task<NtiModel> GetUpToDateModel()
		{
			return await _ntiModelService.GetNtiAsync(ContinuationId);
		}

		private async Task<IEnumerable<RadioItem>> GetReasons()
		{
			var reasons = await _ntiReasonsCachedService.GetAllReasonsAsync();
			return reasons.Select(r => new RadioItem
			{
				Id = Convert.ToString(r.Id),
				Text = r.Name
			});
		}
	}
}