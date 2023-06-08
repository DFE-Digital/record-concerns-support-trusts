using System;
using System.Threading.Tasks;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Pages.Case.Management
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditNextStepsPageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<EditNextStepsPageModel> _logger;

		[BindProperty(SupportsGet = true, Name = "Urn")]
		public int CaseUrn { get; set; }

		[BindProperty]
		public TextAreaUiComponent NextSteps { get; set; }

		public EditNextStepsPageModel(ICaseModelService caseModelService, ILogger<EditNextStepsPageModel> logger)
		{
			_caseModelService = caseModelService;
			_logger = logger;
		}

		public async Task<ActionResult> OnGetAsync()
		{
			try
			{
				_logger.LogMethodEntered();

				var model = await _caseModelService.GetCaseByUrn(CaseUrn);

				LoadPage(model);
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
			}

			return Page();
		}

		public async Task<ActionResult> OnPostEditNextSteps()
		{
			try
			{
				_logger.LogMethodEntered();

				if (!ModelState.IsValid)
				{
					LoadPage();
					return Page();
				}

				// Create patch case model
				var patchCaseModel = new PatchCaseModel
				{
					Urn = CaseUrn,
					CreatedBy = User.Identity.Name,
					UpdatedAt = DateTimeOffset.Now,
					NextSteps = NextSteps.Text.StringContents
				};

				await _caseModelService.PatchNextSteps(patchCaseModel);

				return Redirect($"/case/{CaseUrn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}

		private void LoadPage(CaseModel model)
		{
			LoadPage();

			NextSteps.Text.StringContents = model.NextSteps;
		}

		private void LoadPage()
		{
			NextSteps = CaseComponentBuilder.BuildNextSteps(nameof(NextSteps), NextSteps?.Text.StringContents);
			NextSteps.Heading = "";
		}
	}
}
