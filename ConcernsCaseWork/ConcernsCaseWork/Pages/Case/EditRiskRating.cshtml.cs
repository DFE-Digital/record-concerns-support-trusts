using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Rating;
using ConcernsCaseWork.Services.Records;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditRiskRatingPageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly IRecordModelService _recordModelService;
		private readonly IRatingModelService _ratingModelService;
		private readonly ILogger<EditRiskRatingPageModel> _logger;

		public CaseModel CaseModel { get; private set; }
		public IList<RatingModel> RatingsModel { get; private set; }

		public EditRiskRatingPageModel(ICaseModelService caseModelService, IRatingModelService ratingModelService, IRecordModelService recordModelService, ILogger<EditRiskRatingPageModel> logger)
		{
			_caseModelService = caseModelService;
			_recordModelService = recordModelService;
			_ratingModelService = ratingModelService;
			_logger = logger;
		}
		
		public async Task<ActionResult> OnGet()
		{
			long caseUrn = 0;
			long recordUrn = 0;
			
			try
			{
				_logger.LogInformation("Case::EditRiskRatingPageModel::OnGet");
				(caseUrn, recordUrn) = GetRouteData();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditRiskRatingPageModel::OnGetAsync::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnGetPage;
			}
			
			return await LoadPage(Request.Headers["Referer"].ToString(), caseUrn, recordUrn);
		}
		
		public async Task<ActionResult> OnPostEditRiskRating(string url)
		{
			long caseUrn = 0;
			long recordUrn = 0;

			try
			{
				_logger.LogInformation("Case::EditRiskRatingPageModel::OnPostEditRiskRating");
				(caseUrn, recordUrn) = GetRouteData();
				
				var riskRating = Request.Form["ragRating"];

				if (string.IsNullOrEmpty(riskRating)) 
					throw new Exception("Case::EditRiskRatingPageModel::Missing form values");

				var splitRagRating = riskRating.ToString().Split(":");
				var ratingUrn = splitRagRating[0];
				var ratingName = splitRagRating[1];

				// Create patch case model
				var patchCaseModel = new PatchCaseModel
				{
					Urn = caseUrn,
					CreatedBy = User.Identity.Name,
					UpdatedAt = DateTimeOffset.Now,
					RiskRating = ratingName
				};
					
				await _caseModelService.PatchRiskRating(patchCaseModel);
					
				return Redirect(url);

			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditRiskRatingPageModel::OnPostEditRiskRating::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return await LoadPage(url, caseUrn, recordUrn);
		}
		
		private async Task<ActionResult> LoadPage(string url, long caseUrn, long recordUrn)
		{
			if (caseUrn == 0)
			{
				throw new Exception("Case::EditRiskRatingPageModel::LoadPage caseUrn cannot be 0");
			}

			string username = User.Identity.Name;

			CaseModel = await _caseModelService.GetCaseByUrn(username, caseUrn);
			var recordModel = await _recordModelService.GetRecordModelByUrn(username, caseUrn, recordUrn);
			RatingsModel = await _ratingModelService.GetSelectedRatingsByUrn(recordModel.RatingUrn);
			CaseModel.PreviousUrl = url;

			return Page();
		} 

		private (long caseUrn, long recordUrn) GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];
			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn))
			{
				throw new Exception("Case::EditConcernTypePageModel::CaseUrn is null or invalid to parse");
			}

			var recordUrnValue = RouteData.Values["recordUrn"];
			if (recordUrnValue == null || !long.TryParse(recordUrnValue.ToString(), out long recordUrn))
			{
				throw new Exception("Case::EditConcernTypePageModel::RecordUrn is null or invalid to parse");
			}

			return (caseUrn, recordUrn);
		}
	}
}