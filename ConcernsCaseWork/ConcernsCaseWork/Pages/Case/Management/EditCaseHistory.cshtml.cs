using Ardalis.GuardClauses;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditCaseHistoryPageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<EditCaseHistoryPageModel> _logger;
		private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;
		
		[BindProperty(SupportsGet = true, Name="Urn")]
		[Required(ErrorMessage = "CaseUrn is null or invalid to parse")]
		public long CaseUrn { get; set; }

		[BindProperty]
		public TextAreaUiComponent CaseHistory { get; set; }

		public EditCaseHistoryPageModel(ICaseModelService caseModelService, IClaimsPrincipalHelper claimsPrincipalHelper,
			ILogger<EditCaseHistoryPageModel> logger)
		{
			_caseModelService = Guard.Against.Null(caseModelService);
			_claimsPrincipalHelper = Guard.Against.Null(claimsPrincipalHelper);
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

		public async Task<ActionResult> OnPost()
		{
			_logger.LogMethodEntered();
			
			try
			{
				if (!ModelState.IsValid)
				{
					LoadPage();
					return Page();	
				}
				
				var userName = GetUserName();
				await _caseModelService.PatchCaseHistory((long)CaseUrn, userName, CaseHistory.Text.StringContents);

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

			CaseHistory.Text.StringContents = model.CaseHistory;
		}

		private void LoadPage()
		{
			CaseHistory = CaseComponentBuilder.BuildCaseHistory(nameof(CaseHistory), CaseHistory?.Text.StringContents);
			CaseHistory.Heading = "";
		}

		private string GetUserName() => _claimsPrincipalHelper.GetPrincipalName(User);
	}
}