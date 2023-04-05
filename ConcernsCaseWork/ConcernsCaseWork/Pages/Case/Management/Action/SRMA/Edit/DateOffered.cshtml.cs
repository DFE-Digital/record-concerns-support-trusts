using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.SRMA.Edit
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditDateOfferedPageModel : AbstractPageModel
	{
		private readonly ISRMAService _srmaModelService;
		private readonly ILogger<EditDateOfferedPageModel> _logger;

		[BindProperty(SupportsGet = true, Name = "caseUrn")] public int CaseId { get; set; }
		[BindProperty(SupportsGet = true, Name = "srmaId")] public int SrmaId { get; set; }

		[BindProperty]
		public OptionalDateTimeUiComponent DateOffered { get; set; } = BuidDateOfferedComponent();

		public EditDateOfferedPageModel(ISRMAService srmaService, ILogger<EditDateOfferedPageModel> logger)
		{
			_srmaModelService = srmaService;
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

				await _srmaModelService.SetOfferedDate(SrmaId, DateOffered.Date.ToDateTime());
				return Redirect($"/case/{CaseId}/management/action/srma/{SrmaId}");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}

		private void ResetOnValidationError()
		{
			DateOffered = BuidDateOfferedComponent(DateOffered.Date);
		}

		private void LoadPageComponents(SRMAModel model)
		{
			DateOffered.Date = new OptionalDateModel(model.DateOffered)
			{
				Required = true
			};
		}

		private static OptionalDateTimeUiComponent BuidDateOfferedComponent([CanBeNull] OptionalDateModel date = default)
		{
			var dateToSet = date ?? new OptionalDateModel();
			dateToSet.Required = true;

			return new OptionalDateTimeUiComponent("date-offered", nameof(DateOffered), "")
			{
				Date = dateToSet
			};
		}
	}
}