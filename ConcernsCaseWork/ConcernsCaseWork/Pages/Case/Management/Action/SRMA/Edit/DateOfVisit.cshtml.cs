using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.SRMA.Edit
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditDateOfVisitPageModel : AbstractPageModel
	{
		private readonly ISRMAService _srmaModelService;
		private readonly ILogger<EditDateOfVisitPageModel> _logger;

		[BindProperty(SupportsGet = true, Name = "caseUrn")] public int CaseId { get; set; }
		[BindProperty(SupportsGet = true, Name = "srmaId")] public int SrmaId { get; set; }

		[BindProperty]
		public OptionalDateTimeUiComponent StartDate { get; set; } = BuidStartDateComponent();

		[BindProperty]
		public OptionalDateTimeUiComponent EndDate { get; set; } = BuidEndDateComponent();

		public EditDateOfVisitPageModel(ISRMAService srmaModelService, ILogger<EditDateOfVisitPageModel> logger)
		{
			_srmaModelService = srmaModelService;
			_logger = logger;
		}

		public async Task<ActionResult> OnGetAsync()
		{
			try
			{
				_logger.LogMethodEntered();

				var model = await _srmaModelService.GetSRMAById(SrmaId);
				
				if (model.IsClosed)
				{
					return Redirect($"/case/{CaseId}/management/action/srma/{SrmaId}/closed");
				}

				LoadPageComponents(model);
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
			}

			return Page();
		}

		public async Task<ActionResult> OnPostAsync()
		{
			try
			{
				_logger.LogMethodEntered();

				if (!ModelState.IsValid) 
				{
					ResetOnValidationError();
					return Page();
				}

				if (EndDateHasNoStartDate())
				{
					ResetOnValidationError();
					ModelState.AddModelError($"{nameof(StartDate)}.{StartDate.DisplayName}", "Start date must be the entered if an end date has been entered");
					return Page();
				}

				if (EndIsBeforeStart())
				{
					ResetOnValidationError();
					ModelState.AddModelError($"{nameof(StartDate)}.{StartDate.DisplayName}", "Start date must be the same as or come before the end date");
					ModelState.AddModelError($"{nameof(EndDate)}.{EndDate.DisplayName}", "End date must be the same as or come after the start date");
					return Page();
				}

				await _srmaModelService.SetVisitDates(SrmaId, StartDate.Date.ToDateTime(), EndDate.Date.ToDateTime());
				return Redirect($"/case/{CaseId}/management/action/srma/{SrmaId}");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}

		private bool EndIsBeforeStart()
		{
			return EndDate.Date.ToDateTime() < StartDate.Date.ToDateTime();
		}

		private bool EndDateHasNoStartDate()
		{
			return !EndDate.Date.IsEmpty() && StartDate.Date.IsEmpty();
		}

		private void LoadPageComponents(SRMAModel model)
		{
			if (model.DateVisitStart.HasValue)
				StartDate.Date = new OptionalDateModel(model.DateVisitStart.Value);

			if (model.DateVisitEnd.HasValue)
				EndDate.Date = new OptionalDateModel(model.DateVisitEnd.Value);
		}

		private void ResetOnValidationError()
		{
			StartDate = BuidStartDateComponent(StartDate.Date);
			EndDate = BuidEndDateComponent(EndDate.Date);
		}

		private static OptionalDateTimeUiComponent BuidStartDateComponent([CanBeNull] OptionalDateModel date = default)
		{
			return new OptionalDateTimeUiComponent("start", nameof(StartDate), "Start date")
			{
				Date = date,
				DisplayName = "Start date"
			};
		}

		private static OptionalDateTimeUiComponent BuidEndDateComponent([CanBeNull] OptionalDateModel date = default)
		{
			return new OptionalDateTimeUiComponent("end", nameof(EndDate), "End date")
			{
				Date = date,
				DisplayName = "End date"
			};
		}
	}
}