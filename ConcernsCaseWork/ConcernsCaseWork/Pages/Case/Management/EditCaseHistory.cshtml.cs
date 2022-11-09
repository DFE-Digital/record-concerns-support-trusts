using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.CoreTypes;
using ConcernsCaseWork.Extensions;
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
		[Required]
		public long CaseUrn { get; set; }
		
		[BindProperty]
		public string CaseHistory { get; set; }
		public CaseModel CaseModel { get; private set; }
		
		public EditCaseHistoryPageModel(ICaseModelService caseModelService, IClaimsPrincipalHelper claimsPrincipalHelper, ILogger<EditCaseHistoryPageModel> logger)
		{
			_caseModelService = caseModelService;
			_claimsPrincipalHelper = claimsPrincipalHelper;
			_logger = logger;
		}
		
		public async Task<ActionResult> OnGetAsync()
		{
			try
			{
				_logger.LogInformation("Case::EditCaseHistoryPageModel::OnGetAsync");

			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditCaseHistoryPageModel::OnGetAsync::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnGetPage;
			}
			
			return await LoadPage(Request.Headers["Referer"].ToString());
		}
		
		public async Task<ActionResult> OnPost(string url)
		{
			try
			{
				_logger.LogInformation("Case::EditCaseHistoryPageModel::OnPostEditCaseHistory");

				var userName = GetUserName();
				await _caseModelService.PatchCaseHistory(CaseUrn, userName, CaseHistory);
					
				return Redirect(url);
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return await LoadPage(url);
		}
		
		private async Task<ActionResult> LoadPage(string url)
		{
			try
			{
				var userName = GetUserName();
				CaseModel = await _caseModelService.GetCaseByUrn(userName, CaseUrn);
				CaseModel.PreviousUrl = url;
				
				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditCaseHistoryPageModel::LoadPage::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		}
		
		private string GetUserName() => _claimsPrincipalHelper.GetPrincipalName(User);
	}
}