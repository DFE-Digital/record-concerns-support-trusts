using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditIssuePageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<EditIssuePageModel> _logger;
		
		public CaseModel CaseModel { get; private set; }
		
		public EditIssuePageModel(ICaseModelService caseModelService, ILogger<EditIssuePageModel> logger)
		{
			_caseModelService = caseModelService;
			_logger = logger;
		}
		
		public async Task<ActionResult> OnGetAsync()
		{
			long caseUrn = 0;
			
			try
			{
				_logger.LogInformation("Case::EditIssuePageModel::OnGetAsync");

				var caseUrnValue = RouteData.Values["id"];
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out caseUrn))
				{
					throw new Exception("Case::EditIssuePageModel::CaseUrn is null or invalid to parse");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError($"Case::EditIssuePageModel::OnGetAsync::Exception - { ex.Message }");
				
				TempData["Error.Message"] = ErrorOnGetPage;
			}
			
			return await LoadPage(Request.Headers["Referer"].ToString(), caseUrn);
		}
		
		public async Task<ActionResult> OnPostEditIssue(string url)
		{
			long caseUrn = 0;
			
			try
			{
				_logger.LogInformation("Case::EditIssuePageModel::OnPostEditIssue");
				
				var caseUrnValue = RouteData.Values["id"];
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out caseUrn))
				{
					throw new Exception("Case::EditIssuePageModel::CaseUrn is null or invalid to parse");
				}
				
				var issue = Request.Form["issue"];

				if (string.IsNullOrEmpty(issue)) 
					throw new Exception("Case::EditIssuePageModel::Missing form values");
				
				// TODO update issue
				
					
				//await _caseModelService.PatchDirectionOfTravel(patchCaseModel);
					
				return Redirect(url);
			}
			catch (Exception ex)
			{
				_logger.LogError($"Case::EditIssuePageModel::OnPostEditIssue::Exception - {ex.Message}");

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return await LoadPage(url, caseUrn);
		}
		
		private async Task<ActionResult> LoadPage(string url, long caseUrn)
		{
			if (caseUrn != 0)
			{
				CaseModel = await _caseModelService.GetCaseByUrn(User.Identity.Name, caseUrn);
			}
			else
			{
				CaseModel = new CaseModel();
			}
			
			CaseModel.PreviousUrl = url;
			
			return Page();
		}
	}
}