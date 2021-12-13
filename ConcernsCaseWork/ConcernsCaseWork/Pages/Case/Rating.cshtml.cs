using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class RatingPageModel : AbstractPageModel
	{
		private readonly IRatingModelService _ratingModelService;
		private readonly ILogger<RatingPageModel> _logger;
		private readonly ITrustModelService _trustModelService;
		private readonly ICachedService _cachedService;
		
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public IList<CreateRecordModel> CreateRecordsModel { get; private set; }
		public IList<RatingModel> RatingsModel { get; private set; }
		
		public RatingPageModel(ITrustModelService trustModelService, 
			ICachedService cachedService,
			IRatingModelService ratingModelService,
			ILogger<RatingPageModel> logger)
		{
			_ratingModelService = ratingModelService;
			_trustModelService = trustModelService;
			_cachedService = cachedService;
			_logger = logger;
		}
		
		public async Task OnGetAsync()
		{
			try
			{
				_logger.LogInformation("Case::RatingPageModel::OnGetAsync");
				
				// Fetch UI data
				await LoadPage();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::RatingPageModel::OnGetAsync::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}
		
		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
				_logger.LogInformation("Case::RatingPageModel::OnPostAsync");
				
				var ragRating = Request.Form["rating"].ToString();
				if (string.IsNullOrEmpty(ragRating))
					throw new Exception("Case::RatingPageModel::Missing form values");
				
				// Rating
				var splitRagRating = ragRating.Split(":");
				var ragRatingUrn = splitRagRating[0];
				var ragRatingName = splitRagRating[1];
				
				// Redis state
				var userState = await GetUserState();
				
				// Update cache model
				userState.CreateCaseModel.RatingUrn = long.Parse(ragRatingUrn);
				userState.CreateCaseModel.RagRatingName = ragRatingName;
				
				// Store case model in cache for the details page
				await _cachedService.StoreData(User.Identity.Name, userState);
				
				return RedirectToPage("details");
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::RatingPageModel::OnPostAsync::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnPostPage;
			}
			
			return await LoadPage();
		}
		
		public async Task<ActionResult> OnGetCancel()
		{
			var userState = await GetUserState();
			userState.CreateCaseModel = new CreateCaseModel();
			await _cachedService.StoreData(User.Identity.Name, userState);
			
			return Redirect("/");
		}
		
		private async Task<ActionResult> LoadPage()
		{
			var userState = await GetUserState();
			var trustUkPrn = userState.TrustUkPrn;
			
			if (string.IsNullOrEmpty(trustUkPrn)) return Page();
			
			CreateRecordsModel = userState.CreateCaseModel.CreateRecordsModel;
			TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(trustUkPrn);
			RatingsModel = await _ratingModelService.GetRatingsModel();
			
			return Page();
		}
		
		private async Task<UserState> GetUserState()
		{
			var userState = await _cachedService.GetData<UserState>(User.Identity.Name);
			if (userState is null)
				throw new Exception("Case::RatingPageModel::Cache CaseStateData is null");
			
			return userState;
		}
	}
}