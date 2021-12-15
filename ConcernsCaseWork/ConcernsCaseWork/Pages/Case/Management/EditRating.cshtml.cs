using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Records;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditRatingPageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly IRatingModelService _ratingModelService;
		private readonly ILogger<EditRatingPageModel> _logger;

		public CaseModel CaseModel { get; private set; }
		public IList<RatingModel> RatingsModel { get; private set; }

		public EditRatingPageModel(ICaseModelService caseModelService, 
			IRatingModelService ratingModelService, 
			ILogger<EditRatingPageModel> logger)
		{
			_caseModelService = caseModelService;
			_ratingModelService = ratingModelService;
			_logger = logger;
		}
		
		public async Task<ActionResult> OnGet()
		{
			long caseUrn = 0;
			
			try
			{
				_logger.LogInformation("Case::EditRiskRatingPageModel::OnGet");
				caseUrn = GetRouteData();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditRiskRatingPageModel::OnGetAsync::Exception - {Message}", ex.Message);
				
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
				
				caseUrn = GetRouteData();
				
				var riskRating = Request.Form["rating"].ToString();

				if (string.IsNullOrEmpty(riskRating)) 
					throw new Exception("Case::EditRiskRatingPageModel::Missing form values");

				var splitRagRating = riskRating.Split(":");
				var ratingUrn = splitRagRating[0];

				// Create patch case model
				var patchCaseModel = new PatchCaseModel
				{
					Urn = caseUrn,
					CreatedBy = User.Identity.Name,
					UpdatedAt = DateTimeOffset.Now,
					RatingUrn = long.Parse(ratingUrn)
				};
					
				await _caseModelService.PatchCaseRating(patchCaseModel);
					
				return Redirect(url);

			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditRiskRatingPageModel::OnPostEditRiskRating::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return await LoadPage(url, caseUrn);
		}
		
		private async Task<ActionResult> LoadPage(string url, long caseUrn)
		{
			if (caseUrn == 0) return Page();
			
			CaseModel = await _caseModelService.GetCaseByUrn(User.Identity.Name, caseUrn);
			RatingsModel = await _ratingModelService.GetSelectedRatingsModelByUrn(CaseModel.RatingUrn);
			CaseModel.PreviousUrl = url;

			return Page();
		} 

		private long GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];
			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn) || caseUrn == 0)
			{
				throw new Exception("Case::EditRiskRatingPageModel::CaseUrn is null or invalid to parse");
			}

			return caseUrn;
		}
	}
}