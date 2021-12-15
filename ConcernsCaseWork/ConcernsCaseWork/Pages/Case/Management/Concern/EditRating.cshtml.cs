using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Services.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Concern
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditRatingPageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly IRecordModelService _recordModelService;
		private readonly IRatingModelService _ratingModelService; 
		private readonly ITrustModelService _trustModelService;
		private readonly ITypeModelService _typeModelService;
		private readonly ILogger<EditRatingPageModel> _logger;

		public CaseModel CaseModel { get; private set; }
		public IList<RatingModel> RatingsModel { get; private set; }
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public TypeModel TypeModel { get; private set; }

		public EditRatingPageModel(ICaseModelService caseModelService, 
			IRatingModelService ratingModelService, 
			IRecordModelService recordModelService,
			ITrustModelService trustModelService, 
			ITypeModelService typeModelService,
			ILogger<EditRatingPageModel> logger)
		{
			_caseModelService = caseModelService;
			_recordModelService = recordModelService;
			_ratingModelService = ratingModelService;
			_trustModelService = trustModelService;
			_typeModelService = typeModelService;
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
				
				var riskRating = Request.Form["rating"].ToString();

				if (string.IsNullOrEmpty(riskRating)) 
					throw new Exception("Case::EditRiskRatingPageModel::Missing form values");

				var splitRagRating = riskRating.Split(":");
				var ratingUrn = splitRagRating[0];

				// Create patch record model
				var patchRecordModel = new PatchRecordModel
				{
					UpdatedAt = DateTimeOffset.Now,
					Urn = recordUrn,
					CaseUrn = caseUrn,
					RatingUrn = long.Parse(ratingUrn),
					CreatedBy = User.Identity.Name
				};

				await _caseModelService.PatchRecordRating(patchRecordModel);
					
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
			if (caseUrn == 0 || recordUrn == 0) return Page();
			
			CaseModel = await _caseModelService.GetCaseByUrn(User.Identity.Name, caseUrn);
			var recordModel = await _recordModelService.GetRecordModelByUrn(User.Identity.Name, caseUrn, recordUrn);
			RatingsModel = await _ratingModelService.GetSelectedRatingsModelByUrn(recordModel.RatingUrn);
			TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(CaseModel.TrustUkPrn);
			TypeModel = await _typeModelService.GetSelectedTypeModelByUrn(recordModel.TypeUrn);
			CaseModel.PreviousUrl = url;

			return Page();
		} 

		private (long caseUrn, long recordUrn) GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];
			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn) || caseUrn == 0)
			{
				throw new Exception("Case::EditRiskRatingPageModel::CaseUrn is null or invalid to parse");
			}

			var recordUrnValue = RouteData.Values["recordUrn"];
			if (recordUrnValue == null || !long.TryParse(recordUrnValue.ToString(), out long recordUrn) || recordUrn == 0)
			{
				throw new Exception("Case::EditRiskRatingPageModel::RecordUrn is null or invalid to parse");
			}

			return (caseUrn, recordUrn);
		}
	}
}