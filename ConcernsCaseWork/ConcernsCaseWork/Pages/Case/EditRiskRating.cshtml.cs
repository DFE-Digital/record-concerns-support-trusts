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

		public string PreviousUrl { get; private set; }
		
		public EditRiskRatingPageModel(ICaseModelService caseModelService, ILogger<EditRiskRatingPageModel> logger)
		{
			_caseModelService = caseModelService;
			_logger = logger;
		}
		
		public ActionResult OnGet()
		{
			_logger.LogInformation("EditRiskRatingPageModel::OnGet");
			
			return LoadPage(Request.Headers["Referer"].ToString());
		}
		
		public async Task<ActionResult> OnPostEditRiskRating(string url)
		{
			try
			{
				_logger.LogInformation("EditRiskRatingPageModel::OnPostEditRiskRating");
				
				var caseUrnValue = RouteData.Values["id"];
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out var caseUrn))
				{
					throw new Exception("EditRiskRatingPageModel::CaseUrn is null or invalid to parse");
				}
				
				var riskRating = Request.Form["riskRating"];

				if (string.IsNullOrEmpty(riskRating)) throw new Exception("Missing form values");
				
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

			return LoadPage(url);
		}
		
		private ActionResult LoadPage(string url)
		{
			PreviousUrl = url;

			return Page();
		} 
	}
}