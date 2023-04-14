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
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.SRMA.Edit
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditDateAcceptedPageModel : AbstractPageModel
	{
		private readonly ISRMAService _srmaModelService;
		private readonly ILogger<EditDateAcceptedPageModel> _logger;

		public SRMAModel SRMAModel { get; set; }

		[BindProperty(SupportsGet = true, Name = "caseUrn")] public int CaseId { get; set; }
		[BindProperty(SupportsGet = true, Name = "srmaId")] public int SrmaId { get; set; }

		[BindProperty]
		public OptionalDateTimeUiComponent DateAccepted { get; set; } = BuidDateAcceptedComponent();

		public EditDateAcceptedPageModel(ISRMAService srmaModelService, ILogger<EditDateAcceptedPageModel> logger)
		{
			_srmaModelService = srmaModelService;
			_logger = logger;
		}

		public async Task<ActionResult> OnGetAsync()
		{
			try
			{
				_logger.LogMethodEntered();

				SRMAModel = await _srmaModelService.GetSRMAById(SrmaId);
				
				if (SRMAModel.IsClosed)
				{
					return Redirect($"/case/{CaseId}/management/action/srma/{SrmaId}/closed");
				}

				LoadPageComponents(SRMAModel);
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
					DateAccepted = BuidDateAcceptedComponent(DateAccepted.Date);
					return Page();
				}

				await _srmaModelService.SetDateAccepted(SrmaId, DateAccepted.Date.ToDateTime());
				return Redirect($"/case/{CaseId}/management/action/srma/{SrmaId}");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}

		private void LoadPageComponents(SRMAModel model)
		{
			if (model.DateAccepted.HasValue)
				DateAccepted.Date = new OptionalDateModel(model.DateAccepted.Value);
		}

		private static OptionalDateTimeUiComponent BuidDateAcceptedComponent([CanBeNull] OptionalDateModel date = default)
		{
			return new OptionalDateTimeUiComponent("date-accepted", nameof(DateAccepted), "")
			{
				Date = date,
				DisplayName = "Date accepted",
			};
		}
	}
}