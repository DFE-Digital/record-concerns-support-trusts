using Ardalis.GuardClauses;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Models;
using Service.Redis.Users;
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
		private readonly IUserStateCachedService _userStateCache;
		private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;

		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public IList<CreateRecordModel> CreateRecordsModel { get; private set; }
		public IList<RatingModel> RatingsModel { get; private set; }
		
		public RatingPageModel(ITrustModelService trustModelService, 
			IUserStateCachedService userStateCache,
			IRatingModelService ratingModelService,
			ILogger<RatingPageModel> logger, 
			IClaimsPrincipalHelper claimsPrincipalHelper)
		{
			_ratingModelService = Guard.Against.Null(ratingModelService);
			_trustModelService = Guard.Against.Null(trustModelService);
			_userStateCache = Guard.Against.Null(userStateCache);
			_logger = Guard.Against.Null(logger);
			_claimsPrincipalHelper = Guard.Against.Null(claimsPrincipalHelper);
		}
		
		public async Task OnGetAsync()
		{
			_logger.LogInformation("Case::RatingPageModel::OnGetAsync");
				
			// Fetch UI data
			await LoadPage();
		}
		
		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
				_logger.LogInformation("Case::RatingPageModel::OnPostAsync");
				
				var ragRating = Request.Form["rating"].ToString();
				if (string.IsNullOrEmpty(ragRating))
					throw new Exception("Missing form values");
				
				// Rating
				var splitRagRating = ragRating.Split(":");
				var ragRatingUrn = splitRagRating[0];
				var ragRatingName = splitRagRating[1];
				
				// validate that the links from case to other data is valid. This really should be in a domain layer or at least the trams service.
				var rating = await _ratingModelService.GetRatingModelByUrn(long.Parse(ragRatingUrn));

				if (rating == null)
				{
					throw new InvalidOperationException($"The given ratingUrn '{ragRatingUrn}' does not match any known rating in the system");
				}

				// Redis state
				var userState = await GetUserState();

				// Update cache model
				userState.CreateCaseModel.RatingUrn = long.Parse(ragRatingUrn);
				userState.CreateCaseModel.RagRatingName = ragRatingName;
				userState.CreateCaseModel.RagRating = RatingMapping.FetchRag(ragRatingName);
				userState.CreateCaseModel.RagRatingCss = RatingMapping.FetchRagCss(ragRatingName);
				
				// Store case model in cache for the details page
				await _userStateCache.StoreData(GetUserName(), userState);
				
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
			try
			{
				var userState = await GetUserState();
				userState.CreateCaseModel = new CreateCaseModel();
				await _userStateCache.StoreData(GetUserName(), userState);
				
				return Redirect("/");
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::RatingPageModel::OnGetCancel::Exception - {Message}", ex.Message);
					
				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		}
		
		private async Task<ActionResult> LoadPage()
		{
			try
			{
				var userState = await GetUserState();
				var trustUkPrn = userState.TrustUkPrn;
				
				if (string.IsNullOrEmpty(trustUkPrn)) 
					throw new Exception("Cache TrustUkprn is null");
				
				CreateRecordsModel = userState.CreateCaseModel.CreateRecordsModel;
				TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(trustUkPrn);
				RatingsModel = await _ratingModelService.GetRatingsModel();
				
				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::RatingPageModel::LoadPage::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		}
		
		private async Task<UserState> GetUserState()
		{
			var userState = await _userStateCache.GetData(GetUserName());
			if (userState is null)
				throw new Exception("Cache CaseStateData is null");
			
			return userState;
		}

		private string GetUserName() => _claimsPrincipalHelper.GetPrincipalName(User);
	}
}