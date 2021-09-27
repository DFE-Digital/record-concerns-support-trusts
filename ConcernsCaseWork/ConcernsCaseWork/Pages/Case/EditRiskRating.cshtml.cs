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
	public class EditRiskRatingPageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<EditRiskRatingPageModel> _logger;

		public CaseModel CaseModel { get; private set; }
		
		public EditRiskRatingPageModel(ICaseModelService caseModelService, ILogger<EditRiskRatingPageModel> logger)
		{
			_caseModelService = caseModelService;
			_logger = logger;
		}
		
		public async Task<ActionResult> OnGet()
		{
			long caseUrn = 0;
			
			try
			{
				_logger.LogInformation("Case::EditRiskRatingPageModel::OnGet");
				
				var caseUrnValue = RouteData.Values["id"];
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out caseUrn))
				{
					throw new Exception("Case::EditConcernTypePageModel::CaseUrn is null or invalid to parse");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError($"Case::EditRiskRatingPageModel::OnGetAsync::Exception - { ex.Message }");
				
				TempData["Error.Message"] = ErrorOnGetPage;
			}
			
			return await LoadPage(Request.Headers["Referer"].ToString(), caseUrn);
		}
		
		public async Task<ActionResult> OnPostEditRiskRating(string url)
		{
			long caseUrn = 0;
			
			try
			{
				_logger.LogInformation("Case::EditRiskRatingPageModel::OnPostEditRiskRating");
				
				var caseUrnValue = RouteData.Values["id"];
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out caseUrn))
				{
					throw new Exception("Case::EditRiskRatingPageModel::CaseUrn is null or invalid to parse");
				}
				
				var riskRating = Request.Form["riskRating"];

				if (string.IsNullOrEmpty(riskRating)) throw new Exception("Case::EditRiskRatingPageModel::Missing form values");
				
				// Create patch case model
				var patchCaseModel = new PatchCaseModel
				{
					Urn = caseUrn,
					CreatedBy = User.Identity.Name,
					UpdatedAt = DateTimeOffset.Now,
					RiskRating = riskRating
				};
					
				await _caseModelService.PatchRiskRating(patchCaseModel);
					
				return Redirect(url);

			}
			catch (Exception ex)
			{
				_logger.LogError($"Case::EditRiskRatingPageModel::OnPostEditRiskRating::Exception - {ex.Message}");

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