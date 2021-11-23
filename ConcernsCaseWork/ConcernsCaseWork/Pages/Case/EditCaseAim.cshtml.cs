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
	public class EditCaseAimPageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<EditCaseAimPageModel> _logger;
		
		public CaseModel CaseModel { get; private set; }
		
		public EditCaseAimPageModel(ICaseModelService caseModelService, ILogger<EditCaseAimPageModel> logger)
		{
			_caseModelService = caseModelService;
			_logger = logger;
		}
		
		public async Task<ActionResult> OnGetAsync()
		{
			long caseUrn = 0;
			
			try
			{
				_logger.LogInformation("Case::EditCaseAimPageModel::OnGetAsync");

				var caseUrnValue = RouteData.Values["urn"];
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out caseUrn) || caseUrn == 0)
				{
					throw new Exception("Case::EditCaseAimPageModel::CaseUrn is null or invalid to parse");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditCaseAimPageModel::OnGetAsync::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnGetPage;
			}
			
			return await LoadPage(Request.Headers["Referer"].ToString(), caseUrn);
		}
		
		public async Task<ActionResult> OnPostEditCaseAim(string url)
		{
			long caseUrn = 0;
			
			try
			{
				_logger.LogInformation("Case::EditCaseAimPageModel::OnPostEditCaseAim");
				
				var caseUrnValue = RouteData.Values["urn"];
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out caseUrn) || caseUrn == 0)
				{
					throw new Exception("Case::EditCaseAimPageModel::CaseUrn is null or invalid to parse");
				}
				
				var caseAim = Request.Form["case-aim"];
				
				// Create patch case model
				var patchCaseModel = new PatchCaseModel
				{
					Urn = caseUrn,
					CreatedBy = User.Identity.Name,
					UpdatedAt = DateTimeOffset.Now,
					CaseAim = caseAim
				};

				await _caseModelService.PatchCaseAim(patchCaseModel);
					
				return Redirect(url);
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditCaseAimPageModel::OnPostEditCaseAim::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return await LoadPage(url, caseUrn);
		}
		
		private async Task<ActionResult> LoadPage(string url, long caseUrn)
		{
			CaseModel = await _caseModelService.GetCaseByUrn(User.Identity.Name, caseUrn);
			CaseModel.PreviousUrl = url;
			
			return Page();
		}
	}
}