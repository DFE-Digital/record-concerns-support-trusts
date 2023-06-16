using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditDeEscalationPointPageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<EditDeEscalationPointPageModel> _logger;

		[BindProperty(SupportsGet = true, Name = "Urn")]
		public int CaseUrn { get; set; }

		[BindProperty]
		public TextAreaUiComponent DeEscalationPoint { get; set; }

		public EditDeEscalationPointPageModel(ICaseModelService caseModelService, ILogger<EditDeEscalationPointPageModel> logger)
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

		public async Task<ActionResult> OnPostEditDeEscalationPoint()
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
					DeEscalationPoint = DeEscalationPoint.Text.StringContents
				};

				await _caseModelService.PatchDeEscalationPoint(patchCaseModel);

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

			DeEscalationPoint.Text.StringContents = model.DeEscalationPoint;
		}

		private void LoadPage()
		{
			DeEscalationPoint = CaseComponentBuilder.BuildDeEscalationPoint(nameof(DeEscalationPoint), DeEscalationPoint?.Text.StringContents);
			DeEscalationPoint.Heading = "";
		}
	}
}