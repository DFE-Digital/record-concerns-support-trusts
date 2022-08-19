using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Nti;
using ConcernsCaseWork.Services.NtiWarningLetter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Redis.Nti;
using Service.Redis.NtiWarningLetter;
using Service.TRAMS.Nti;
using Service.TRAMS.NtiWarningLetter;
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
		private readonly INtiWarningLetterStatusesCachedService _ntiWarningLetterStatusesCachedService;
		private readonly INtiWarningLetterReasonsCachedService _ntiWarningLetterReasonsCachedService;
		private readonly INtiModelService _ntiModelService;
		private readonly INtiConditionsCachedService _ntiConditionsCachedService;
		private readonly ILogger _logger;

		[TempData]
		public string ContinuationId { get; set; }

		[TempData]
		public bool IsReturningFromConditions { get; set; }

		public long CaseUrn { get; private set; }
		public long? NtiId { get; set; }

		public ICollection<NtiConditionModel> SelectedConditions { get; private set; }
		public ICollection<NtiConditionDto> AllConditions { get; private set; }

		public AddConditionsPageModel(INtiWarningLetterStatusesCachedService ntiWarningLetterStatusesCachedService,
			INtiWarningLetterReasonsCachedService ntiWarningLetterReasonsCachedService,
			INtiModelService ntiModelService,
			INtiConditionsCachedService ntiConditionsCachedService,
			ILogger<NtiWarningLetterConditionModel> logger)
		{
			_ntiWarningLetterStatusesCachedService = ntiWarningLetterStatusesCachedService;
			_ntiWarningLetterReasonsCachedService = ntiWarningLetterReasonsCachedService;
			_ntiModelService = ntiModelService;
			_ntiConditionsCachedService = ntiConditionsCachedService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogInformation("Case::Action::NTI-WL::AddConditionsPageModel::OnGetAsync");

			try
			{
				if (ContinuationId == null)
				{
					throw new InvalidOperationException("Continuation Id not found");
				}

				ExtractCaseUrnFromRoute();
				ExtractWarningLetterIdFromRoute();

				var model = await GetUpToDateModel();
				SelectedConditions = model.Conditions;

				AllConditions = await _ntiConditionsCachedService.GetAllConditionsAsync();

				IsReturningFromConditions = true;
				TempData.Keep(nameof(ContinuationId));
				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::NTI-WL::AddConditionsPageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		}

		public async Task<IActionResult> OnPostAsync(string action)
		{
			try
			{
				ExtractCaseUrnFromRoute();
				ExtractWarningLetterIdFromRoute();

				AllConditions = await _ntiConditionsCachedService.GetAllConditionsAsync();
				var conditions = Request.Form["condition"];
				var model = await GetUpToDateModel();
				model.Conditions = conditions.Select(s => NtiMappers.ToServiceModel(AllConditions.Single(c => c.Id == int.Parse(s)))).ToArray();
				await _ntiModelService.StoreNti(model, ContinuationId);

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
				_logger.LogError("Case::NTI::AddConditionsPageModel::OnPostAsync::Exception - {Message}", ex.Message);
				TempData["Error.Message"] = ErrorOnPostPage;
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

		private void ExtractCaseUrnFromRoute()
		{
			if (TryGetRouteValueInt64("urn", out var caseUrn))
			{
				CaseUrn = caseUrn;
			}
			else
			{
				throw new InvalidOperationException("CaseUrn not found in the route");
			}
		}

		private void ExtractWarningLetterIdFromRoute()
		{
			NtiId = TryGetRouteValueInt64("warningLetterId", out var warningLetterId) ? (long?)warningLetterId : null;
		}

		private async Task<IEnumerable<RadioItem>> GetStatuses()
		{
			var statuses = await _ntiWarningLetterStatusesCachedService.GetAllStatusesAsync();
			return statuses.Select(r => new RadioItem
			{
				Id = Convert.ToString(r.Id),
				Text = r.Name
			});
		}

		private async Task<IEnumerable<RadioItem>> GetReasons()
		{
			var reasons = await _ntiWarningLetterReasonsCachedService.GetAllReasonsAsync();
			return reasons.Select(r => new RadioItem
			{
				Id = Convert.ToString(r.Id),
				Text = r.Name
			});
		}




	}
}