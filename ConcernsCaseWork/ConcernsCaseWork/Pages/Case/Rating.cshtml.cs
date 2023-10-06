using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.ApplicationInsights;
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
	public class RatingPageModel : AbstractPageModel
	{
		private readonly IRatingModelService _ratingModelService;
		private readonly ILogger<RatingPageModel> _logger;
		private readonly ITrustModelService _trustModelService;
		private readonly IUserStateCachedService _userStateCache;
		private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;
		private TelemetryClient _telemetryClient;
		
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public CreateCaseModel CreateCaseModel { get; private set; }
		public IList<CreateRecordModel> CreateRecordsModel { get; private set; }
		public IList<RatingModel> RatingsModel { get; private set; }

		[BindProperty]
		public RadioButtonsUiComponent RiskToTrust { get; set; }

		[BindProperty(SupportsGet = true, Name = "Urn")]
		public int? CaseUrn { get; set; }

		public RatingPageModel(ITrustModelService trustModelService, 
			IUserStateCachedService userStateCache,
			IRatingModelService ratingModelService,
			ILogger<RatingPageModel> logger, 
			IClaimsPrincipalHelper claimsPrincipalHelper,
			TelemetryClient telemetryClient)
		{
			_ratingModelService = Guard.Against.Null(ratingModelService);
			_trustModelService = Guard.Against.Null(trustModelService);
			_userStateCache = Guard.Against.Null(userStateCache);
			_logger = Guard.Against.Null(logger);
			_claimsPrincipalHelper = Guard.Against.Null(claimsPrincipalHelper);
			_telemetryClient = Guard.Against.Null(telemetryClient);
		}
		
		public async Task OnGetAsync()
		{
			_logger.LogMethodEntered();
				
			// Fetch UI data
			await LoadPage();
		}
		
		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
				_logger.LogMethodEntered();

				if (!ModelState.IsValid)
				{
					await LoadPage();
					return Page();
				}

				var ragRatingId = (ConcernRating)RiskToTrust.SelectedId.Value;

				if (!Enum.IsDefined(typeof(ConcernRating), ragRatingId))
					throw new InvalidOperationException($"Unrecognised risk to trust {ragRatingId}");

				var ragRatingName = ragRatingId.Description();

				// Redis state
				var userState = await GetUserState();

				// Update cache model
				userState.CreateCaseModel.RatingId = (long)ragRatingId;
				userState.CreateCaseModel.RagRatingName = ragRatingName;
				userState.CreateCaseModel.RagRating = RatingMapping.FetchRag(ragRatingName);
				userState.CreateCaseModel.RagRatingCss = RatingMapping.FetchRagCss(ragRatingName);
				AppInsightsHelper.LogEvent(_telemetryClient, new AppInsightsModel()
				{
					EventName = "CREATE CASE",
					EventDescription = $"Rating added {ragRatingName}",
					EventPayloadJson = "",
					EventUserName = userState.UserName
				});
				// Store case model in cache for the details page
				await _userStateCache.StoreData(GetUserName(), userState);

				if (CaseUrn.HasValue)
				{
					return RedirectToPage("details",new {urn = CaseUrn });
					
				}

				return RedirectToPage("details");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
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
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
			}

			return Page();
		}
		
		private async Task<ActionResult> LoadPage()
		{
			try
			{
				var userState = await GetUserState();
				var trustUkPrn = userState.TrustUkPrn;

				if (string.IsNullOrEmpty(trustUkPrn)) 
					throw new Exception("Cache TrustUkprn is null");

				CreateCaseModel = userState.CreateCaseModel;
				CreateRecordsModel = userState.CreateCaseModel.CreateRecordsModel;
				TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(trustUkPrn);
				RatingsModel = await _ratingModelService.GetRatingsModel();
				AppInsightsHelper.LogEvent(_telemetryClient, new AppInsightsModel()
				{
					EventName = "CREATE CASE",
					EventDescription = "Rating being added",
					EventPayloadJson = "",
					EventUserName = userState.UserName
				});

				RiskToTrust = CaseComponentBuilder.BuildRiskToTrust(nameof(RiskToTrust), RatingsModel, RiskToTrust?.SelectedId);

				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
			}

			return Page();
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