using ConcernsCaseWork.CoreTypes;
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
	public class EditIssuePageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<EditIssuePageModel> _logger;
		
		public CaseModel CaseModel { get; private set; }

		[BindProperty(SupportsGet = true, Name = "Urn")]
		public int CaseUrn { get; set; }

		[BindProperty]
		public TextAreaUiComponent Issue { get; set; }

		public EditIssuePageModel(ICaseModelService caseModelService, ILogger<EditIssuePageModel> logger)
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
		
		public async Task<ActionResult> OnPostEditIssue()
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
					Issue = Issue.Text.StringContents
				};

				await _caseModelService.PatchIssue(patchCaseModel);

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

			Issue.Text.StringContents = model.Issue;
		}
		
		private void LoadPage()
		{
			Issue = CaseComponentBuilder.BuildIssue(nameof(Issue), Issue?.Text.StringContents);
			Issue.Heading = "";
		}
	}
}